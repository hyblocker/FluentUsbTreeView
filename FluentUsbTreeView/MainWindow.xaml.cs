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
using WpfTreeViewItem = System.Windows.Controls.TreeViewItem;

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

        private uint m_devicesConnected = 0;

        public MainWindow() {
            if ( s_instance == null ) {
                s_instance = this;
            } else {
                throw new Exception("Somehow ended up with two main windows from the same instance?");
            }
            DataContext = this;
            Wpf.Ui.Appearance.SystemThemeWatcher.Watch(this);
            InitializeComponent();
            restartAsAdmin.IsEnabled = !Util.IsCurrentProcessElevated();

            UsbDatabase.GetUsbProductName(0x28DE, 0x2300);

            RefreshUsbTree();

            // Auto-select tree node root
            ((WpfTreeViewItem)usbTreeList.Items.GetItemAt(0)).IsSelected = true;
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
            usbTreeList.Items.Add(computerNode);
            UsbEnumator.EnumerateHostControllers(computerNode, ref m_devicesConnected);
        }

        public WpfTreeViewItem AddLeaf(WpfTreeViewItem hTreeParent, object metadata, string leafName, UsbTreeIcon icon, bool shouldExpand) {
            WpfTreeViewItem leafNode = new WpfTreeViewItem();
            leafNode.Header = TreeHelpers.CreateTreeViewContentWithIcon(leafName, icon);
            leafNode.Tag = metadata;
            leafNode.Selected += TreeNode_Selected;
            hTreeParent.Items.Add(leafNode);
            leafNode.IsExpanded = shouldExpand;
            return leafNode;
        }
        private void TreeNode_Selected(object sender, RoutedEventArgs e) {
            WpfTreeViewItem treeNode = (WpfTreeViewItem)sender;

            if ( treeNode.Tag == null ) {
                // Root node
                rawTextContent.Text = DetailViewDataGenerator.GetInfoStringForRootNode().Replace("\t", "    "); // tabs to 4 spaces
            } else {
                Type metadataType = treeNode.Tag?.GetType();

                if ( metadataType == typeof(USBHOSTCONTROLLERINFO) ) {
                    USBHOSTCONTROLLERINFO metadata = (USBHOSTCONTROLLERINFO) treeNode.Tag;
                    rawTextContent.Text = DetailViewDataGenerator.GetInfoStringForHostController(metadata).Replace("\t", "    "); // tabs to 4 spaces
                } else if ( metadataType == typeof(UsbHubInfoUnion) ) {
                    UsbHubInfoUnion metadata = (UsbHubInfoUnion) treeNode.Tag;
                    rawTextContent.Text = DetailViewDataGenerator.GetInfoStringForUsbHub(metadata).Replace("\t", "    "); // tabs to 4 spaces
                }
            }

            e.Handled = true;
        }

        #endregion
    }
}
