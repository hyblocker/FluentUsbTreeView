using FluentUsbTreeView.PInvoke;
using FluentUsbTreeView.UsbTreeView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wpf.Ui.Controls;
using static FluentUsbTreeView.PInvoke.UsbApi;

namespace FluentUsbTreeView.Ui {
    public static class DetailViewDataGenerator {

        const int LEFT_COLUMN_SIZE = 25;

        #region String helpers

        private static string PropertyTitle(string title) {
            return title.PadRight(LEFT_COLUMN_SIZE);
        }
        private static string WriteBool(bool value) {
            return value == true ? "yes" : "no";
        }
        private static string WriteBool(uint value) {
            return value != 0 ? "yes" : "no";
        }
        private static string WriteBool(int value) {
            return value != 0 ? "yes" : "no";
        }
        private static string WriteHex(uint num, int places) {
            return $"0x{num.ToString($"X{places}")}";
        }

        private static string WriteWdmUsbPowerState(WDMUSB_POWER_STATE powerState) {
            switch ( powerState ) {
                case WDMUSB_POWER_STATE.DeviceD0:
                    return "D0";
                case WDMUSB_POWER_STATE.DeviceD1:
                    return "D1";
                case WDMUSB_POWER_STATE.DeviceD2:
                    return "D2";
                case WDMUSB_POWER_STATE.DeviceD3:
                    return "D3";
                case WDMUSB_POWER_STATE.SystemWorking:
                    return "S0";
                case WDMUSB_POWER_STATE.SystemSleeping1:
                    return "S1";
                case WDMUSB_POWER_STATE.SystemSleeping2:
                    return "S2";
                case WDMUSB_POWER_STATE.SystemSleeping3:
                    return "S3";
                case WDMUSB_POWER_STATE.SystemHibernate:
                    return "S4";
                case WDMUSB_POWER_STATE.SystemShutdown:
                    return "S5";
                case WDMUSB_POWER_STATE.SystemUnspecified:
                    return "S6";
                default:
                    return "--";
            }
        }

        private static string WriteDevicePowerState(DEVICE_POWER_STATE powerState) {
            switch ( powerState ) {
                case DEVICE_POWER_STATE.PowerDeviceD0:
                    return "D0";
                case DEVICE_POWER_STATE.PowerDeviceD1:
                    return "D1";
                case DEVICE_POWER_STATE.PowerDeviceD2:
                    return "D2";
                case DEVICE_POWER_STATE.PowerDeviceD3:
                    return "D3";
                default:
                    return "";
            }
        }

        #endregion

        public static string GetInfoStringForHostController(USBHOSTCONTROLLERINFO usbController) {

            StringBuilder contentString = new StringBuilder();

            string friendlyName = usbController.UsbDeviceProperties.DeviceDesc;
            string vendorIdName = UsbDatabase.GetPCIeVendorName((ushort)usbController.ControllerInfo.PciVendorId);
            string productIdName = UsbDatabase.GetPCIeProductName((ushort)usbController.ControllerInfo.PciVendorId, (ushort)usbController.ControllerInfo.PciDeviceId);

            contentString.Append("\n\t===================== USB Host Controller =======================\n");

            #region Device Information

            contentString.Append("\t\t+++++++++++++++++ Device Information ++++++++++++++++++\n");
            contentString.Append($"{PropertyTitle("Friendly Name")}: {friendlyName}\n");
            contentString.Append($"{PropertyTitle("Device Description")}: {usbController.UsbDeviceProperties.DeviceDesc}\n");
            contentString.Append($"{PropertyTitle("Device Path")}: {usbController.UsbDeviceProperties.DeviceDesc}\n");
            contentString.Append($"{PropertyTitle("Kernel Name")}: {usbController.UsbDeviceProperties.DeviceDesc}\n");
            contentString.Append($"{PropertyTitle("Device ID")}: {usbController.UsbDeviceProperties.DeviceId}\n");
            contentString.Append($"{PropertyTitle("Vendor")}: {(vendorIdName == null ? WriteHex(usbController.ControllerInfo.PciVendorId, 4) : vendorIdName)}\n");
            contentString.Append($"{PropertyTitle("Hardware IDs")}: {usbController.UsbDeviceProperties.DeviceDesc}\n");
            contentString.Append($"{PropertyTitle("Driver KeyName")}: {usbController.DriverKey}\n");
            contentString.Append($"{PropertyTitle("Driver")}: {usbController.UsbDeviceProperties.DeviceDesc}\n");
            contentString.Append($"{PropertyTitle("Driver Inf")}: {usbController.UsbDeviceProperties.DeviceDesc}\n");
            contentString.Append($"{PropertyTitle("Legacy BusType")}: {usbController.UsbDeviceProperties.DeviceDesc}\n");
            contentString.Append($"{PropertyTitle("Class")}: {usbController.UsbDeviceProperties.DeviceClass}\n");
            contentString.Append($"{PropertyTitle("Class GUID")}: {usbController.UsbDeviceProperties.DeviceDesc}\n");
            contentString.Append($"{PropertyTitle("Service")}: {usbController.UsbDeviceProperties.Service}\n");
            contentString.Append($"{PropertyTitle("Enumerator")}: PCI\n");
            contentString.Append($"{PropertyTitle("Location Info")}: PCI bus {usbController.BusNumber}, device {usbController.BusDevice}{( usbController.BusDeviceFunctionValid == true ? $", function {usbController.BusFunction}" : "" )}\n");
            contentString.Append($"{PropertyTitle("Location IDs")}: {usbController.UsbDeviceProperties.DeviceDesc}\n");
            contentString.Append($"{PropertyTitle("Container ID")}: {usbController.UsbDeviceProperties.DeviceDesc}\n");
            contentString.Append($"{PropertyTitle("Manufacturer Info")}: {usbController.UsbDeviceProperties.DeviceDesc}\n");
            contentString.Append($"{PropertyTitle("Capabilities")}: {usbController.UsbDeviceProperties.DeviceDesc}\n");
            contentString.Append($"{PropertyTitle("Status")}: {usbController.UsbDeviceProperties.DeviceDesc}\n");
            contentString.Append($"{PropertyTitle("Problem Code")}: {usbController.UsbDeviceProperties.DeviceDesc}\n");
            contentString.Append($"{PropertyTitle("Address")}: {usbController.UsbDeviceProperties.DeviceDesc}\n");
            contentString.Append($"{PropertyTitle("Power State")}: {WriteDevicePowerState(usbController.UsbDeviceProperties.PowerState.PD_MostRecentPowerState)} (supported: ??????)\n");
            contentString.Append(
@"Friendly Name            : Renesas USB 3.0 eXtensible Host Controller - 1.0 (Microsoft)
Device Description       : USB xHCI Compliant Host Controller
Device Path              : \\?\PCI#VEN_1912&DEV_0015&SUBSYS_00151912&REV_02#8&cd93788&0&00200020020B#{3abf6f2d-71c4-462a-8a92-1e6861e6af27} (GUID_DEVINTERFACE_USB_HOST_CONTROLLER)
Kernel Name              : \Device\NTPNP_PCI0049
Device ID                : PCI\VEN_1912&DEV_0015&SUBSYS_00151912&REV_02\8&CD93788&0&00200020020B
Vendor                   : Renesas
Hardware IDs             : PCI\VEN_1912&DEV_0015&SUBSYS_00151912&REV_02 PCI\VEN_1912&DEV_0015&SUBSYS_00151912 PCI\VEN_1912&DEV_0015&CC_0C0330 PCI\VEN_1912&DEV_0015&CC_0C03
Driver KeyName           : {36fc9e60-c465-11cf-8056-444553540000}\0089 (GUID_DEVCLASS_USB)
Driver                   : \SystemRoot\System32\drivers\USBXHCI.SYS (Version: 10.0.22621.2215  Date: 2023-09-05)
Driver Inf               : C:\WINDOWS\inf\usbxhci.inf
Legacy BusType           : PCIBus
Class                    : USB
Class GUID               : {36fc9e60-c465-11cf-8056-444553540000} (GUID_DEVCLASS_USB)
Service                  : USBXHCI
Enumerator               : PCI
Location Info            : PCI bus 42, device 0, function 0
Location IDs             : PCIROOT(0)#PCI(0103)#PCI(0002)#PCI(0400)#PCI(0000)#PCI(0400)#PCI(0000), ACPI(_SB_)#ACPI(PCI0)#ACPI(GPP2)#ACPI(PT02)#ACPI(PT24)#PCI(0000)#PCI(0400)#PCI(0000)
Container ID             : {00000000-0000-0000-ffff-ffffffffffff} (GUID_CONTAINERID_INTERNALLY_CONNECTED_DEVICE)
Manufacturer Info        : Generic USB xHCI Host Controller
Capabilities             : 0x00 (-)
Status                   : 0x0180200A (DN_DRIVER_LOADED, DN_STARTED, DN_DISABLEABLE, DN_NT_ENUMERATOR, DN_NT_DRIVER)
Problem Code             : 0
Address                  : 0
Power State              : D0 (supported: D0, D3, wake from D0, wake from D3)
 Child Device 1          : USB Root Hub (USB 3.0)
  Device Path            : \\?\USB#ROOT_HUB30#9&3849bda4&0&0#{f18a0e88-c30c-11d0-8815-00a0c906bed8} (GUID_DEVINTERFACE_USB_HUB)
  Kernel Name            : \Device\USBPDO-4
  Device ID              : USB\ROOT_HUB30\9&3849BDA4&0&0
  Class                  : USB
  Driver KeyName         : {36fc9e60-c465-11cf-8056-444553540000}\0096 (GUID_DEVCLASS_USB)
  Service                : USBHUB3
  LocationPaths          : PCIROOT(0)#PCI(0103)#PCI(0002)#PCI(0400)#PCI(0000)#PCI(0400)#PCI(0000)#USBROOT(0)  ACPI(_SB_)#ACPI(PCI0)#ACPI(GPP2)#ACPI(PT02)#ACPI(PT24)#PCI(0000)#PCI(0400)#PCI(0000)#USBROOT(0)
  IdleInWorkingState     : 1");

            #endregion

            #region USB Host Controller Info 0

            contentString.Append("\n\t\t--------------- USB Hostcontroller Info0 --------------\n");
            contentString.Append($"{PropertyTitle("PciVendorId")}: {WriteHex(usbController.ControllerInfo.PciVendorId, 4)}");
            if (vendorIdName != null) {
                contentString.Append($" ({vendorIdName})\n");
            } else {
                contentString.Append("\n");
            }
            contentString.Append($"{PropertyTitle("PciDeviceId")}: {WriteHex(usbController.ControllerInfo.PciDeviceId, 4)}\n");
            contentString.Append($"{PropertyTitle("PciRevision")}: {WriteHex(usbController.ControllerInfo.PciRevision, 2)}\n");
            contentString.Append($"{PropertyTitle("NumberOfRootPorts")}: {WriteHex(usbController.ControllerInfo.NumberOfRootPorts, 2)} ({usbController.ControllerInfo.NumberOfRootPorts} Ports)\n");
            contentString.Append($"{PropertyTitle("ControllerFlavor")}: {WriteHex((uint)usbController.ControllerInfo.ControllerFlavor, 2)} ({(uint) usbController.ControllerInfo.ControllerFlavor} = {usbController.ControllerInfo.ControllerFlavor})\n");
            contentString.Append($"{PropertyTitle("HcFeatureFlags")}: {WriteHex(usbController.ControllerInfo.HcFeatureFlags, 2)}\n");
            contentString.Append($"{PropertyTitle(" Port Power Switching")}: {WriteBool(usbController.ControllerInfo.HcFeatureFlags & UsbApi.USB_HC_FEATURE_FLAG_PORT_POWER_SWITCHING)}\n");
            contentString.Append($"{PropertyTitle(" Selective Suspend")}: {WriteBool(usbController.ControllerInfo.HcFeatureFlags & UsbApi.USB_HC_FEATURE_FLAG_SEL_SUSPEND)}\n");
            contentString.Append($"{PropertyTitle(" Legacy BIOS")}: {WriteBool(usbController.ControllerInfo.HcFeatureFlags & UsbApi.USB_HC_FEATURE_LEGACY_BIOS)}\n");
            contentString.Append($"{PropertyTitle(" Time Sync API")}: {WriteBool(usbController.ControllerInfo.HcFeatureFlags & UsbApi.USB_HC_FEATURE_TIME_SYNC_API)}\n");

            // @TODO: Implement
            contentString.Append($"\n{PropertyTitle("Roothub Symbolic Link")}:{"USB#ROOT_HUB30#9&3849bda4&0&0#{f18a0e88-c30c-11d0-8815-00a0c906bed8}"}\n");

            #endregion

            #region USB Hostcontroller BusStatistics

            // @TODO: Implement
            contentString.Append($"\n\t\t----------- USB Hostcontroller BusStatistics ----------\n");
            contentString.Append(@"DeviceCount              : 0x09 (9)
CurrentSystemTime        : 0x01D9FEA873520CCA (2023-10-14 16:12:22)
CurrentUsbFrame          : 0x30956FAA (815099818)
BulkBytes                : 0x00 (0)
IsoBytes                 : 0x00 (0)
InterruptBytes           : 0x00 (0)
ControlDataBytes         : 0x00 (0)
PciInterruptCount        : 0x01 (1)
HardResetCount           : 0x00 (0)
WorkerSignalCount        : 0x00 (0)
CommonBufferBytes        : 0x00 (0)
WorkerIdleTimeMs         : 0x00 (0)
RootHubEnabled           : 0x01 (yes)
RootHubDevicePowerState  : 0x00 (D0)
Unused                   : 0x00 (0)
NameIndex                : 0x00 (0)");

            #endregion

            #region USB Hostcontroller Driver Version Params

            // @TODO: Implement
            contentString.Append($"\n\t\t------ USB Hostcontroller Driver Version Params -------\n");
            contentString.Append(@"DriverTrackingCode       : 0x04
USBDI_Version            : 0x600
USBUSER_Version          : 0x04
CheckedPortDriver        : 0x00
CheckedMiniportDriver    : 0x00
USB_Version              : 0x00");

            #endregion

            #region USB Hostcontroller Bandwidth Info

            const double BITS_MS_TO_MB_S = 0.000125;
            contentString.Append("\n\t\t---------- USB Hostcontroller Bandwidth Info ----------\n");
            contentString.Append($"{PropertyTitle("DeviceCount")}: {WriteHex(usbController.BandwidthInfo.DeviceCount, 8)} ({usbController.BandwidthInfo.DeviceCount})\n");
            contentString.Append($"{PropertyTitle("TotalBusBandwidth")}: {WriteHex(usbController.BandwidthInfo.TotalBusBandwidth, 8)} ({usbController.BandwidthInfo.TotalBusBandwidth} bits/ms = {string.Format("{0:0.##}", usbController.BandwidthInfo.TotalBusBandwidth * BITS_MS_TO_MB_S)} MB/s)\n");
            contentString.Append($"{PropertyTitle("Total32secBandwidth")}: {WriteHex(usbController.BandwidthInfo.Total32secBandwidth, 8)} ({usbController.BandwidthInfo.Total32secBandwidth} bits/ms = {string.Format("{0:0.##}", usbController.BandwidthInfo.Total32secBandwidth * BITS_MS_TO_MB_S / 32.0)} MB/s)\n");
            contentString.Append($"{PropertyTitle("AllocedBulkAndControl")}: {WriteHex(usbController.BandwidthInfo.AllocedBulkAndControl, 8)} ({usbController.BandwidthInfo.AllocedBulkAndControl} bits/ms = {string.Format("{0:0.##}", usbController.BandwidthInfo.AllocedBulkAndControl * BITS_MS_TO_MB_S / 32.0)} MB/s = {string.Format("{0:0.###}", usbController.BandwidthInfo.AllocedBulkAndControl * 100.0 / usbController.BandwidthInfo.Total32secBandwidth)}%)\n");
            contentString.Append($"{PropertyTitle("AllocedIso")}: {WriteHex(usbController.BandwidthInfo.AllocedIso, 8)} ({usbController.BandwidthInfo.AllocedIso} bits/32ms)\n");
            contentString.Append($"{PropertyTitle("AllocedInterrupt_1ms")}: {WriteHex(usbController.BandwidthInfo.AllocedInterrupt_1ms, 8)} ({usbController.BandwidthInfo.AllocedInterrupt_1ms} bits/32ms)\n");
            contentString.Append($"{PropertyTitle("AllocedInterrupt_2ms")}: {WriteHex(usbController.BandwidthInfo.AllocedInterrupt_2ms, 8)} ({usbController.BandwidthInfo.AllocedInterrupt_2ms} bits/32ms)\n");
            contentString.Append($"{PropertyTitle("AllocedInterrupt_4ms")}: {WriteHex(usbController.BandwidthInfo.AllocedInterrupt_4ms, 8)} ({usbController.BandwidthInfo.AllocedInterrupt_4ms} bits/32ms)\n");
            contentString.Append($"{PropertyTitle("AllocedInterrupt_8ms")}: {WriteHex(usbController.BandwidthInfo.AllocedInterrupt_8ms, 8)} ({usbController.BandwidthInfo.AllocedInterrupt_8ms} bits/32ms)\n");
            contentString.Append($"{PropertyTitle("AllocedInterrupt_16ms")}: {WriteHex(usbController.BandwidthInfo.AllocedInterrupt_16ms, 8)} ({usbController.BandwidthInfo.AllocedInterrupt_16ms} bits/32ms)\n");
            contentString.Append($"{PropertyTitle("AllocedInterrupt_32ms")}: {WriteHex(usbController.BandwidthInfo.AllocedInterrupt_32ms, 8)} ({usbController.BandwidthInfo.AllocedInterrupt_32ms} bits/32ms)\n");

            #endregion

            #region USB Hostcontroller Power States Info

            contentString.Append("\n\t\t-------- USB Hostcontroller Power States Info ---------\n");
            contentString.Append($"{PropertyTitle("SystemState")}: {WriteWdmUsbPowerState(usbController.USBPowerInfo[0].SystemState)}\t{WriteWdmUsbPowerState(usbController.USBPowerInfo[1].SystemState)}\t{WriteWdmUsbPowerState(usbController.USBPowerInfo[2].SystemState)}\t{WriteWdmUsbPowerState(usbController.USBPowerInfo[3].SystemState)}\t{WriteWdmUsbPowerState(usbController.USBPowerInfo[4].SystemState)}\t{WriteWdmUsbPowerState(usbController.USBPowerInfo[5].SystemState)}\t\n");
            contentString.Append($"{PropertyTitle("HcDevicePowerState")}: {WriteWdmUsbPowerState(usbController.USBPowerInfo[0].HcDevicePowerState)}\t{WriteWdmUsbPowerState(usbController.USBPowerInfo[1].HcDevicePowerState)}\t{WriteWdmUsbPowerState(usbController.USBPowerInfo[2].HcDevicePowerState)}\t{WriteWdmUsbPowerState(usbController.USBPowerInfo[3].HcDevicePowerState)}\t{WriteWdmUsbPowerState(usbController.USBPowerInfo[4].HcDevicePowerState)}\t{WriteWdmUsbPowerState(usbController.USBPowerInfo[5].HcDevicePowerState)}\t\n");
            contentString.Append($"{PropertyTitle("HcDeviceWake")}: {WriteWdmUsbPowerState(usbController.USBPowerInfo[0].HcDeviceWake)}\t{WriteWdmUsbPowerState(usbController.USBPowerInfo[1].HcDeviceWake)}\t{WriteWdmUsbPowerState(usbController.USBPowerInfo[2].HcDeviceWake)}\t{WriteWdmUsbPowerState(usbController.USBPowerInfo[3].HcDeviceWake)}\t{WriteWdmUsbPowerState(usbController.USBPowerInfo[4].HcDeviceWake)}\t{WriteWdmUsbPowerState(usbController.USBPowerInfo[5].HcDeviceWake)}\t\n");
            contentString.Append($"{PropertyTitle("HcSystemWake")}: {WriteWdmUsbPowerState(usbController.USBPowerInfo[0].HcSystemWake)}\t{WriteWdmUsbPowerState(usbController.USBPowerInfo[1].HcSystemWake)}\t{WriteWdmUsbPowerState(usbController.USBPowerInfo[2].HcSystemWake)}\t{WriteWdmUsbPowerState(usbController.USBPowerInfo[3].HcSystemWake)}\t{WriteWdmUsbPowerState(usbController.USBPowerInfo[4].HcSystemWake)}\t{WriteWdmUsbPowerState(usbController.USBPowerInfo[5].HcSystemWake)}\t\n");
            contentString.Append($"{PropertyTitle("RhDevicePowerState")}: {WriteWdmUsbPowerState(usbController.USBPowerInfo[0].RhDevicePowerState)}\t{WriteWdmUsbPowerState(usbController.USBPowerInfo[1].RhDevicePowerState)}\t{WriteWdmUsbPowerState(usbController.USBPowerInfo[2].RhDevicePowerState)}\t{WriteWdmUsbPowerState(usbController.USBPowerInfo[3].RhDevicePowerState)}\t{WriteWdmUsbPowerState(usbController.USBPowerInfo[4].RhDevicePowerState)}\t{WriteWdmUsbPowerState(usbController.USBPowerInfo[5].RhDevicePowerState)}\t\n");
            contentString.Append($"{PropertyTitle("RhDeviceWake")}: {WriteWdmUsbPowerState(usbController.USBPowerInfo[0].RhDeviceWake)}\t{WriteWdmUsbPowerState(usbController.USBPowerInfo[1].RhDeviceWake)}\t{WriteWdmUsbPowerState(usbController.USBPowerInfo[2].RhDeviceWake)}\t{WriteWdmUsbPowerState(usbController.USBPowerInfo[3].RhDeviceWake)}\t{WriteWdmUsbPowerState(usbController.USBPowerInfo[4].RhDeviceWake)}\t{WriteWdmUsbPowerState(usbController.USBPowerInfo[5].RhDeviceWake)}\t\n");
            contentString.Append($"{PropertyTitle("RhSystemWake")}: {WriteWdmUsbPowerState(usbController.USBPowerInfo[0].RhSystemWake)}\t{WriteWdmUsbPowerState(usbController.USBPowerInfo[1].RhSystemWake)}\t{WriteWdmUsbPowerState(usbController.USBPowerInfo[2].RhSystemWake)}\t{WriteWdmUsbPowerState(usbController.USBPowerInfo[3].RhSystemWake)}\t{WriteWdmUsbPowerState(usbController.USBPowerInfo[4].RhSystemWake)}\t{WriteWdmUsbPowerState(usbController.USBPowerInfo[5].RhSystemWake)}\t\n");
            contentString.Append($"{PropertyTitle("LastSystemSleepState")}: {WriteWdmUsbPowerState(usbController.USBPowerInfo[0].LastSystemSleepState)}\t{WriteWdmUsbPowerState(usbController.USBPowerInfo[1].LastSystemSleepState)}\t{WriteWdmUsbPowerState(usbController.USBPowerInfo[2].LastSystemSleepState)}\t{WriteWdmUsbPowerState(usbController.USBPowerInfo[3].LastSystemSleepState)}\t{WriteWdmUsbPowerState(usbController.USBPowerInfo[4].LastSystemSleepState)}\t{WriteWdmUsbPowerState(usbController.USBPowerInfo[5].LastSystemSleepState)}\t\n");
            contentString.Append($"{PropertyTitle("CanWakeup")}: {WriteBool(usbController.USBPowerInfo[0].CanWakeup).PadRight(6)}{WriteBool(usbController.USBPowerInfo[1].CanWakeup).PadRight(6)}{WriteBool(usbController.USBPowerInfo[2].CanWakeup).PadRight(6)}{WriteBool(usbController.USBPowerInfo[3].CanWakeup).PadRight(6)}{WriteBool(usbController.USBPowerInfo[4].CanWakeup).PadRight(6)}{WriteBool(usbController.USBPowerInfo[5].CanWakeup)}\t\n");
            contentString.Append($"{PropertyTitle("IsPowered")}: {WriteBool(usbController.USBPowerInfo[0].IsPowered).PadRight(6)}{WriteBool(usbController.USBPowerInfo[1].IsPowered).PadRight(6)}{WriteBool(usbController.USBPowerInfo[2].IsPowered).PadRight(6)}{WriteBool(usbController.USBPowerInfo[3].IsPowered).PadRight(6)}{WriteBool(usbController.USBPowerInfo[4].IsPowered).PadRight(6)}{WriteBool(usbController.USBPowerInfo[5].IsPowered)}\t\n");

            #endregion

            contentString.Append("\n\n"); // End

            return contentString.ToString();
        }
    }
}
