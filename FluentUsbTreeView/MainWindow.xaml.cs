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
                throw new Exception("Somehow ended up with two main windows?");
            }
            DataContext = this;
            Wpf.Ui.Appearance.SystemThemeWatcher.Watch(this);
            InitializeComponent();
            restartAsAdmin.IsEnabled = !Util.IsCurrentProcessElevated();

            UsbDatabase.GetUsbProductName(0x28DE, 0x2300);

            RefreshUsbTree();
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
            computerNode.Tag = null; // @TODO: Struct metadata
            computerNode.Selected += RootNode_Selected;
            usbTreeList.Items.Add(computerNode);

            UsbEnumator.EnumerateHostControllers(computerNode, ref m_devicesConnected);
        }

        private void RootNode_Selected(object sender, RoutedEventArgs e) {

            string processorArch = "x64";
            switch ( Assembly.GetExecutingAssembly().GetName().ProcessorArchitecture ) {
                case ProcessorArchitecture.None:
                    processorArch = "Unknown";
                    break;
                case ProcessorArchitecture.Arm:
                    processorArch = "Arm";
                    break;
                case ProcessorArchitecture.X86:
                    processorArch = "x86";
                    break;
                case ProcessorArchitecture.Amd64:
                    processorArch = "x64";
                    break;
                case ProcessorArchitecture.MSIL:
                    processorArch = "MSIL";
                    break;
                case ProcessorArchitecture.IA64:
                    processorArch = "IA64";
                    break;
            }

            rawTextContent.Text = $@"
    ========================== My Computer ==========================
Operating System       : Windows 11 Pro: NT10.0 Build 22621.2361 Version 22H2 SP0 type=1 suite=100 x64
Up Time                : 9 days 10 hours 24 minutes 59 seconds
Computer Name          : {Environment.MachineName}
Admin Privileges       : {( Util.IsCurrentProcessElevated() ? "yes" : "no" )}
User Account Control   : {( true ? "yes" : "no" )}

UsbTreeView Version    : {Assembly.GetExecutingAssembly().GetName().Version} ({processorArch})
Settings               : F:\Applications\UsbTreeView.ini

USB Host Controllers   : 6
USB Root Hubs          : 6
USB Standard Hubs      : 7
USB Peripheral Devices : 19

Device Class Filter Drivers:
USB Upper              : USBPcap


        +++++++++++++++++ Registry USB Flags +++++++++++++++++
HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\USB
 DualRoleFeatures        : REG_DWORD 00000001 (1)
 OsDefaultRoleSwitchMode : REG_DWORD 00000006 (6)
 UcmIsPresent            : REG_DWORD 00000001 (1)
HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\USB\AutomaticSurpriseRemoval
 AttemptRecoveryFromUsbPowerDrain: REG_DWORD 00000001 (1)
"; // @TODO: Dynamically generate

            e.Handled = true;


        }

        public WpfTreeViewItem AddLeaf(WpfTreeViewItem hTreeParent, object metadata, string leafName, UsbTreeIcon icon) {
            WpfTreeViewItem leafNode = new WpfTreeViewItem();
            leafNode.Header = TreeHelpers.CreateTreeViewContentWithIcon(leafName, icon);
            leafNode.Tag = metadata; // @TODO: Struct metadata
            leafNode.Selected += Node_Selected;
            hTreeParent.Items.Add(leafNode);
            return hTreeParent;
        }
        private void Node_Selected(object sender, RoutedEventArgs e) {
            WpfTreeViewItem treeNode = (WpfTreeViewItem)sender;

            Type metadataType = treeNode.Tag.GetType();

            if ( metadataType == typeof(USBHOSTCONTROLLERINFO) ) {
                USBHOSTCONTROLLERINFO metadata = (USBHOSTCONTROLLERINFO) treeNode.Tag;
                rawTextContent.Text = DetailViewDataGenerator.GetInfoStringForHostController(metadata).Replace("\t", "    "); // tabs to 4 spaces
            }

            e.Handled = true;
        }

        #endregion
    }
}
