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

namespace FluentUsbTreeView {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow {

        private static MainWindow s_instance = null;
        public static MainWindow Instance {
            get {
                return s_instance;
            }
        }

        private Settings m_settings;

        private UsbTreeState m_usbTreeState = new UsbTreeState();

        public MainWindow() {
            if ( s_instance == null ) {
                s_instance = this;
            } else {
                throw new Exception("Somehow ended up with two main windows from the same instance?");
            }
            DataContext = this;
            m_settings = Settings.Instance;
            Wpf.Ui.Appearance.SystemThemeWatcher.Watch(this);
            InitializeComponent();
            restartAsAdmin.IsEnabled = !Util.IsCurrentProcessElevated();

            RefreshUsbTree();

            // Auto-select tree node root
            ((WpfTreeViewItem)usbTreeList.Items.GetItemAt(0)).IsSelected = true;

            // Register for update notifications
            DeviceManaged.OnDeviceAdded     += HandleUsbTreeDeviceUpdate;
            DeviceManaged.OnDeviceRemoved   += HandleUsbTreeDeviceUpdate;
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
            ContextMenu contextMenu = frameworkElement.ContextMenu;
            object nodeMetadata = frameworkElement.Tag;
            
            // Fetch entries    
            var safelyRemove    = LogicalTreeHelper.FindLogicalNode(contextMenu, "contextMenu_safelyRemove")    as WpfMenuItem;
            var restartDevice   = LogicalTreeHelper.FindLogicalNode(contextMenu, "contextMenu_restartDevice")   as WpfMenuItem;
            var restartPort     = LogicalTreeHelper.FindLogicalNode(contextMenu, "contextMenu_restartPort")     as WpfMenuItem;
            var copyTreeRoot    = LogicalTreeHelper.FindLogicalNode(contextMenu, "contextMenu_copyTreeRoot")    as WpfMenuItem;
            var copyReportRoot  = LogicalTreeHelper.FindLogicalNode(contextMenu, "contextMenu_copyReportRoot")  as WpfMenuItem;
            var copyOtherRoot   = LogicalTreeHelper.FindLogicalNode(contextMenu, "contextMenu_copyOtherRoot")   as WpfMenuItem;
            var regeditRoot     = LogicalTreeHelper.FindLogicalNode(contextMenu, "contextMenu_regedit")         as WpfMenuItem;
            var devprops        = LogicalTreeHelper.FindLogicalNode(contextMenu, "contextMenu_devprops")        as WpfMenuItem;

            if ( nodeMetadata == null ) {
                // Computer node
                safelyRemove.IsEnabled      = false;
                restartDevice.IsEnabled     = false;
                restartPort.IsEnabled       = false;
                copyTreeRoot.IsEnabled      = true;
                copyReportRoot.IsEnabled    = true;
                copyOtherRoot.IsEnabled     = false;
                regeditRoot.IsEnabled       = true;
                devprops.IsEnabled          = false;

            } else {
                Type metadataType = nodeMetadata?.GetType();
                
                // Assume everything should be enabled
                safelyRemove.IsEnabled      = true;
                restartDevice.IsEnabled     = true;
                restartPort.IsEnabled       = true;
                copyTreeRoot.IsEnabled      = true;
                copyReportRoot.IsEnabled    = true;
                copyOtherRoot.IsEnabled     = true;
                regeditRoot.IsEnabled       = true;
                devprops.IsEnabled          = true;

                if ( metadataType == typeof(UsbHostControllerInfo) ) {
                    UsbHostControllerInfo metadata = (UsbHostControllerInfo) nodeMetadata;
                    if ( metadata.UsbDeviceProperties == null ) {
                        devprops.IsEnabled = false;
                    } else {
                        devprops.Tag = metadata.UsbDeviceProperties.DeviceId;
                    }

                } else if ( metadataType == typeof(UsbHubInfo) ) {
                    UsbHubInfo metadata = (UsbHubInfo) nodeMetadata;
                    if ( metadata.UsbDeviceProperties == null ) {
                        devprops.IsEnabled = false;
                    } else {
                        devprops.Tag = metadata.UsbDeviceProperties.DeviceId;
                    }

                } else if ( metadataType == typeof(USBDEVICEINFO) ) {
                    USBDEVICEINFO metadata = (USBDEVICEINFO) nodeMetadata;
                    if (metadata.ConnectionInfo.ConnectionStatus != USB_CONNECTION_STATUS.NoDeviceConnected) {
                        if ( metadata.UsbDeviceProperties == null ) {
                            devprops.IsEnabled = false;
                        } else {
                            devprops.Tag = metadata.UsbDeviceProperties.DeviceId;
                        }
                    } else {
                        safelyRemove.IsEnabled  = false;
                        restartDevice.IsEnabled = false;
                        copyOtherRoot.IsEnabled = false;
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

        }

        #endregion

        #region Dependency Props

        // Settings
        public bool Settings_AutoRefresh {
            get { return m_settings.AutoRefresh; }
            set { m_settings.AutoRefresh = value; }
        }
        public bool Settings_EndpointDescriptors {
            get { return m_settings.EndpointDescriptors; }
            set { m_settings.EndpointDescriptors = value; }
        }
        public bool Settings_ScanAllStringDescriptors {
            get { return m_settings.ScanAllStringDescriptors; }
            set { m_settings.ScanAllStringDescriptors = value; }
        }
        public bool Settings_DescriptorHexDumps {
            get { return m_settings.DescriptorHexDumps; }
            set { m_settings.DescriptorHexDumps = value; }
        }
        public bool Settings_DriveNumbersInTree {
            get { return m_settings.DriveNumbersInTree; }
            set { m_settings.DriveNumbersInTree = value; }
        }
        public bool Settings_EndpointsInTree {
            get { return m_settings.EndpointsInTree; }
            set { m_settings.EndpointsInTree = value; }
        }

        // Automatic expansion
        public bool Settings_ExpandForEmptyPorts {
            get { return m_settings.ExpandForEmptyPorts; }
            set { m_settings.ExpandForEmptyPorts = value; }
        }
        public bool Settings_ExpandForEmptyHubs {
            get { return m_settings.ExpandForEmptyHubs; }
            set { m_settings.ExpandForEmptyHubs = value; }
        }
        public bool Settings_ExpandForNewDevices {
            get { return m_settings.ExpandForNewDevices; }
            set { m_settings.ExpandForNewDevices = value; }
        }
        public bool Settings_JumpToNewDevices {
            get { return m_settings.JumpToNewDevices; }
            set { m_settings.JumpToNewDevices = value; }
        }
        public bool Settings_JumpToRemovedDevices {
            get { return m_settings.JumpToRemovedDevices; }
            set { m_settings.JumpToRemovedDevices = value; }
        }

        #endregion
    }
}
