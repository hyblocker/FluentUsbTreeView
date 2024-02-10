using FluentUsbTreeView.PInvoke;
using FluentUsbTreeView.Ui;
using FluentUsbTreeView.UsbTreeView;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf.Ui.Controls;
using static FluentUsbTreeView.PInvoke.UsbApi;
using WpfTreeViewItem = System.Windows.Controls.TreeViewItem;
using WpfMenuItem = System.Windows.Controls.MenuItem;
using ContextMenu = System.Windows.Controls.ContextMenu;
using System.Windows.Forms;

namespace FluentUsbTreeView {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow {

        const int DEFAULT_WIDTH = -1;
        const int DEFAULT_HEIGHT = -1;


        private static MainWindow s_instance = null;
        public static MainWindow Instance {
            get {
                return s_instance;
            }
        }

        private UsbTreeState m_usbTreeState = new UsbTreeState();

        public MainWindow() {
            if ( s_instance == null ) {
                s_instance = this;
            } else {
                throw new Exception("Somehow ended up with two main windows from the same instance?");
            }
            DataContext = this;
            Wpf.Ui.Appearance.SystemThemeWatcher.Watch(this);
            InitializeComponent();
            TrySetWindowPositionAndBounds();
            if ( Settings.Instance.SplitterPosition != Settings.INVALID_POSITION ) {
                rootGrid.ColumnDefinitions[0].Width = new GridLength(Settings.Instance.SplitterPosition, GridUnitType.Star);
                rootGrid.ColumnDefinitions[2].Width = new GridLength(Settings.Instance.SizeX - Settings.Instance.SplitterPosition - rootGrid.ColumnDefinitions[1].ActualWidth, GridUnitType.Star);
            }
            restartAsAdmin.IsEnabled = !Util.IsCurrentProcessElevated();

            RefreshUsbTree();

            // Auto-select tree node root
            ((WpfTreeViewItem)usbTreeList.Items.GetItemAt(0)).IsSelected = true;

            // Register for update notifications
            DeviceManaged.OnDeviceAdded     += HandleUsbTreeDeviceUpdate;
            DeviceManaged.OnDeviceRemoved   += HandleUsbTreeDeviceUpdate;
        }

        private void TrySetWindowPositionAndBounds() {
            // s_instance.Width = DEFAULT_WIDTH;
            // s_instance.Height = DEFAULT_HEIGHT;

            if ( Settings.Instance.PositionX != Settings.INVALID_POSITION && Settings.Instance.PositionY != Settings.INVALID_POSITION &&
                Settings.Instance.SizeX != Settings.INVALID_POSITION && Settings.Instance.SizeY != Settings.INVALID_POSITION ) {

                bool areSavedBoundsValid = IsOnScreen(Settings.Instance.PositionX, Settings.Instance.PositionY, Settings.Instance.SizeX, Settings.Instance.SizeY);

                if ( !areSavedBoundsValid ) {
                    // Window is out of the screen! We will ignore window parameters as a result
                    // @TODO: Devise an algorithm to get like a "closest fit" window bounds if out of bounds
                } else {
                    // We are overriding the startup location, so inform the window
                    WindowStartupLocation = WindowStartupLocation.Manual;
                    this.Left   = Settings.Instance.PositionX;
                    this.Top    = Settings.Instance.PositionY;
                    this.Width  = Settings.Instance.SizeX;
                    this.Height = Settings.Instance.SizeY;
                }
            }
        }

        #region File menu handling

        private void refreshDevices_Click(object sender, RoutedEventArgs e) {
            RefreshUsbTree();
        }

        private void restartAsAdmin_Click(object sender, RoutedEventArgs e) {
            if ( !Util.IsCurrentProcessElevated() ) {
                if ( Util.ElevateSelf() ) {
                    Util.Quit();
                }
            }
        }

        private void exitProgram_Click(object sender, RoutedEventArgs e) {
            Util.Quit();
        }
        #endregion

        #region Options
        private void updateTheme_Click(object sender, RoutedEventArgs e) {

            if ( highContrast.IsChecked == true ) {
                Wpf.Ui.Appearance.ApplicationThemeManager.Apply(Wpf.Ui.Appearance.ApplicationTheme.HighContrast);
            } else {
                if ( darkTheme.IsChecked == true ) {
                    Wpf.Ui.Appearance.ApplicationThemeManager.Apply(Wpf.Ui.Appearance.ApplicationTheme.Dark);
                } else {
                    Wpf.Ui.Appearance.ApplicationThemeManager.Apply(Wpf.Ui.Appearance.ApplicationTheme.Light);
                }
            }
        }
        #endregion

        #region Draw USB devices

        public void RefreshUsbTree() {
            // Clear to reset
            usbTreeList.Items.Clear();

            // Create root node (computer)
            WpfTreeViewItem computerNode = new WpfTreeViewItem();
            computerNode.Header = TreeHelpers.CreateTreeViewContentWithIcon("My Computer", UsbTreeIcon.Computer);
            computerNode.Tag = null;
            computerNode.Selected += TreeNode_Selected;
            computerNode.IsExpanded = true; // Root node is always expanded
            computerNode.ContextMenu = this.FindResource("devContextMenu") as ContextMenu;
            computerNode.ContextMenuOpening += DeviceTreeNode_ContextMenuOpening;
            usbTreeList.Items.Add(computerNode);
            UsbEnumator.EnumerateHostControllers(computerNode, ref m_usbTreeState);

            Logger.Info("Finished enumerating devices!");

            UpdateStatusBar();
        }

        private void UpdateStatusBar() {
            statusbarUsbStatus.Text = $"Host Controllers: {m_usbTreeState.HostControllers}    Root Hubs: {m_usbTreeState.RootHubs}    Standard Hubs: {m_usbTreeState.ExternalHubs}    Peripheral Devices: {m_usbTreeState.PeripheralDevices}";
        }

        private void DeviceTreeNode_ContextMenuOpening(object sender, ContextMenuEventArgs e) {
            FrameworkElement frameworkElement = e.Source as FrameworkElement;
            // search for tree node
            frameworkElement = Util.FindAncestor<WpfTreeViewItem>(frameworkElement);
            if ( frameworkElement == null ) { return; }
            ContextMenu contextMenu = frameworkElement.ContextMenu;
            object nodeMetadata = frameworkElement.Tag;
            
            // Fetch entries    
            // var safelyRemove    = LogicalTreeHelper.FindLogicalNode(contextMenu, "contextMenu_safelyRemove")    as WpfMenuItem;
            // var restartDevice   = LogicalTreeHelper.FindLogicalNode(contextMenu, "contextMenu_restartDevice")   as WpfMenuItem;
            var restartPort     = LogicalTreeHelper.FindLogicalNode(contextMenu, "contextMenu_restartPort")     as WpfMenuItem;
            // var copyTreeRoot    = LogicalTreeHelper.FindLogicalNode(contextMenu, "contextMenu_copyTreeRoot")    as WpfMenuItem;
            // var copyReportRoot  = LogicalTreeHelper.FindLogicalNode(contextMenu, "contextMenu_copyReportRoot")  as WpfMenuItem;
            // var copyOtherRoot   = LogicalTreeHelper.FindLogicalNode(contextMenu, "contextMenu_copyOtherRoot")   as WpfMenuItem;
            // var regeditRoot     = LogicalTreeHelper.FindLogicalNode(contextMenu, "contextMenu_regedit")         as WpfMenuItem;
            var devprops        = LogicalTreeHelper.FindLogicalNode(contextMenu, "contextMenu_devprops")        as WpfMenuItem;

            if ( nodeMetadata == null ) {
                // Computer node
                // safelyRemove.IsEnabled      = false;
                // restartDevice.IsEnabled     = false;
                restartPort.IsEnabled       = false;
                // copyTreeRoot.IsEnabled      = true;
                // copyReportRoot.IsEnabled    = true;
                // copyOtherRoot.IsEnabled     = false;
                // regeditRoot.IsEnabled       = true;
                devprops.IsEnabled          = false;

            } else {
                Type metadataType = nodeMetadata?.GetType();
                
                // Assume everything should be enabled
                // safelyRemove.IsEnabled      = true;
                // restartDevice.IsEnabled     = true;
                restartPort.IsEnabled       = true;
                // copyTreeRoot.IsEnabled      = true;
                // copyReportRoot.IsEnabled    = true;
                // copyOtherRoot.IsEnabled     = true;
                // regeditRoot.IsEnabled       = true;
                devprops.IsEnabled          = true;

                if ( metadataType == typeof(UsbHostControllerInfo) ) {
                    UsbHostControllerInfo metadata = (UsbHostControllerInfo) nodeMetadata;
                    restartPort.IsEnabled = false;
                    if ( metadata.UsbDeviceProperties == null ) {
                        devprops.IsEnabled = false;
                    } else {
                        devprops.Tag = metadata.UsbDeviceProperties.DeviceId;
                    }

                } else if ( metadataType == typeof(UsbHubInfo) ) {
                    UsbHubInfo metadata = (UsbHubInfo) nodeMetadata;
                    restartPort.IsEnabled = metadata.DeviceInfoType == UsbDeviceInfoType.ExternalHub;
                    if ( metadata.UsbDeviceProperties == null ) {
                        devprops.IsEnabled = false;
                        restartPort.IsEnabled = false;
                    } else {
                        devprops.Tag = metadata.UsbDeviceProperties.DeviceId;
                        if ( restartPort.IsEnabled ) {
                            restartPort.Tag = metadata.DeviceInfoNode.DeviceInfoData.DevInst;
                        }
                    }

                } else if ( metadataType == typeof(USBDEVICEINFO) ) {
                    USBDEVICEINFO metadata = (USBDEVICEINFO) nodeMetadata;
                    if (metadata.ConnectionInfo.ConnectionStatus != USB_CONNECTION_STATUS.NoDeviceConnected) {
                        if ( metadata.UsbDeviceProperties == null ) {
                            devprops.IsEnabled = false;
                            restartPort.IsEnabled = false;
                        } else {
                            devprops.Tag = metadata.UsbDeviceProperties.DeviceId;
                            restartPort.IsEnabled = false;
                            // restartPort.Tag = metadata.DeviceInfoNode.DeviceInfoData.DevInst;
                        }
                    } else {
                        // safelyRemove.IsEnabled  = false;
                        // restartDevice.IsEnabled = false;
                        // copyOtherRoot.IsEnabled = false;
                        devprops.IsEnabled      = false;
                    }
                }
            }

        }

        // Handles a device in the device tree being updated
        private void HandleUsbTreeDeviceUpdate(string devicePath) {
            Dispatcher.Invoke(() => {
                Logger.Info($"Device update notification: \"{devicePath}\"");

                RefreshUsbTree();

                // @TODO: Figure out how to requery the usb tree info for only that device and update the tree
            });
        }

        public WpfTreeViewItem AddLeaf(WpfTreeViewItem hTreeParent, object metadata, string leafName, UsbTreeIcon icon, bool shouldBeShown) {
            WpfTreeViewItem leafNode = new WpfTreeViewItem();
            leafNode.Header = TreeHelpers.CreateTreeViewContentWithIcon(leafName, icon);
            leafNode.Tag = metadata;
            leafNode.Selected += TreeNode_Selected;
            leafNode.ContextMenu = this.FindResource("devContextMenu") as ContextMenu;
            hTreeParent.Items.Add(leafNode);
            // Expand all parent nodes too if need be
            if ( shouldBeShown ) {
                WpfTreeViewItem currentParentElement = hTreeParent;
                while ( true ) {
                    currentParentElement.IsExpanded = true;
                    if ( currentParentElement.Parent.GetType() == typeof(WpfTreeViewItem) &&
                         currentParentElement.Parent != usbTreeList ) {
                        currentParentElement = (WpfTreeViewItem) currentParentElement.Parent;
                        // if the parent is expanded it means that the other nodes from the parent up to the root node are also expanded
                        if ( currentParentElement.IsExpanded ) {
                            break;
                        }
                    } else {
                        break;
                    }
                }
            }
            return leafNode;
        }
        private void TreeNode_Selected(object sender, RoutedEventArgs e) {
            WpfTreeViewItem treeNode = (WpfTreeViewItem)sender;

            if ( treeNode.Tag == null ) {
                // Root node
                rawTextContent.Text = DetailViewDataGenerator.GetInfoStringForRootNode(m_usbTreeState).Replace("\t", "    "); // tabs to 4 spaces
            } else {
                Type metadataType = treeNode.Tag?.GetType();

                if ( metadataType == typeof(UsbHostControllerInfo) ) {
                    UsbHostControllerInfo metadata = (UsbHostControllerInfo) treeNode.Tag;
                    rawTextContent.Text = DetailViewDataGenerator.GetInfoStringForHostController(metadata).Replace("\t", "    "); // tabs to 4 spaces
                } else if ( metadataType == typeof(UsbHubInfo) ) {
                    UsbHubInfo metadata = (UsbHubInfo) treeNode.Tag;
                    rawTextContent.Text = DetailViewDataGenerator.GetInfoStringForUsbHub(metadata).Replace("\t", "    "); // tabs to 4 spaces
                } else if ( metadataType == typeof(USBDEVICEINFO) ) {
                    USBDEVICEINFO metadata = (USBDEVICEINFO) treeNode.Tag;
                    rawTextContent.Text = DetailViewDataGenerator.GetInfoStringForUsbDevice(metadata).Replace("\t", "    "); // tabs to 4 spaces
                }
            }

            e.Handled = true;
        }

        #endregion

        private void MainWindow_Closed(object sender, EventArgs e) {
            
            // Get window props to save them before exit
            Settings.Instance.PositionX         = ( int ) Left;
            Settings.Instance.PositionY         = ( int ) Top;
            Settings.Instance.SizeX             = ( int ) Width;
            Settings.Instance.SizeY             = ( int ) Height;
            Settings.Instance.SplitterPosition  = ( int ) rootGrid.ColumnDefinitions[0].ActualWidth;

            Settings.Instance.WriteSettings();

            Dispatcher.Invoke(() => DeviceManaged.UnregisterDeviceNotifications());
        }

        #region Context menu

        private void contextMenu_devprops_Click(object sender, RoutedEventArgs e) {
            // Device path is stored in the tag
            WpfMenuItem menuElement = (sender as WpfMenuItem);
            WindowsTools.OpenDeviceManagerAtDevice( menuElement.Tag as string );
        }

        private void contextMenu_regedit_Click(object sender, RoutedEventArgs e) {

        }

        private void contextMenu_safelyRemove_Click(object sender, RoutedEventArgs e) {

        }

        private void contextMenu_restartDevice_Click(object sender, RoutedEventArgs e) {

        }

        private void contextMenu_restartPort_Click(object sender, RoutedEventArgs e) {
            WpfMenuItem menuElement = (sender as WpfMenuItem);
            uint devInst = (uint) menuElement.Tag;
            DeviceNode.CycleUsbDevice( devInst );
        }

        #endregion

        #region Dependency Props

        // Settings
        public bool Settings_AutoRefresh {
            get { return Settings.Instance.AutoRefresh; }
            set { Settings.Instance.AutoRefresh = value; Settings.Instance.WriteSettings(); }
        }
        public bool Settings_EndpointDescriptors {
            get { return Settings.Instance.EndpointDescriptors; }
            set { Settings.Instance.EndpointDescriptors = value; Settings.Instance.WriteSettings(); }
        }
        public bool Settings_ScanAllStringDescriptors {
            get { return Settings.Instance.ScanAllStringDescriptors; }
            set { Settings.Instance.ScanAllStringDescriptors = value; Settings.Instance.WriteSettings(); }
        }
        public bool Settings_DescriptorHexDumps {
            get { return Settings.Instance.DescriptorHexDumps; }
            set { Settings.Instance.DescriptorHexDumps = value; Settings.Instance.WriteSettings(); }
        }
        public bool Settings_DriveNumbersInTree {
            get { return Settings.Instance.DriveNumbersInTree; }
            set { Settings.Instance.DriveNumbersInTree = value; Settings.Instance.WriteSettings(); }
        }
        public bool Settings_EndpointsInTree {
            get { return Settings.Instance.EndpointsInTree; }
            set { Settings.Instance.EndpointsInTree = value; Settings.Instance.WriteSettings(); }
        }

        // Automatic expansion
        public bool Settings_ExpandForEmptyPorts {
            get { return Settings.Instance.ExpandForEmptyPorts; }
            set { Settings.Instance.ExpandForEmptyPorts = value; Settings.Instance.WriteSettings(); }
        }
        public bool Settings_ExpandForEmptyHubs {
            get { return Settings.Instance.ExpandForEmptyHubs; }
            set { Settings.Instance.ExpandForEmptyHubs = value; Settings.Instance.WriteSettings(); }
        }
        public bool Settings_ExpandForNewDevices {
            get { return Settings.Instance.ExpandForNewDevices; }
            set { Settings.Instance.ExpandForNewDevices = value; Settings.Instance.WriteSettings(); }
        }
        public bool Settings_JumpToNewDevices {
            get { return Settings.Instance.JumpToNewDevices; }
            set { Settings.Instance.JumpToNewDevices = value; Settings.Instance.WriteSettings(); }
        }
        public bool Settings_JumpToRemovedDevices {
            get { return Settings.Instance.JumpToRemovedDevices; }
            set { Settings.Instance.JumpToRemovedDevices = value; Settings.Instance.WriteSettings(); }
        }

        #endregion

        // Return True if a certain percent of a rectangle is shown across the total screen area of all monitors, otherwise return False.
        public bool IsOnScreen(int posX, int posY, int sizeX, int sizeY, double MinPercentOnScreen = 0.2) {
            return IsOnScreen(new System.Drawing.Point(posX, posY), new System.Drawing.Size(sizeX, sizeY), MinPercentOnScreen);
        }

        // Return True if a certain percent of a rectangle is shown across the total screen area of all monitors, otherwise return False.
        public bool IsOnScreen(System.Drawing.Point RecLocation, System.Drawing.Size RecSize, double MinPercentOnScreen = 0.2) {
            double PixelsVisible = 0;
            System.Drawing.Rectangle Rec = new System.Drawing.Rectangle(RecLocation, RecSize);

            foreach ( Screen Scrn in Screen.AllScreens ) {
                System.Drawing.Rectangle r = System.Drawing.Rectangle.Intersect(Rec, Scrn.WorkingArea);
                // intersect rectangle with screen
                if ( r.Width != 0 & r.Height != 0 ) {
                    PixelsVisible += ( r.Width * r.Height );
                    // tally visible pixels
                }
            }
            return PixelsVisible >= ( Rec.Width * Rec.Height ) * MinPercentOnScreen;
        }
    }
}
