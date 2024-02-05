using FluentUsbTreeView.PInvoke;
using FluentUsbTreeView.UsbTreeView;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Wpf.Ui.Controls;
using static FluentUsbTreeView.PInvoke.UsbApi;

namespace FluentUsbTreeView.Ui {
    public static class DetailViewDataGenerator {

        const int LEFT_COLUMN_SIZE = 25;
        const int HEX_DUMP_MAX_ELEMENTS_PER_LINE = 16;

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
        private static string WriteHex(int num, int places) {
            return $"0x{num.ToString($"X{places}")}";
        }
        private static string WriteHex(short num, int places) {
            return $"0x{num.ToString($"X{places}")}";
        }
        private static string WriteHex(ushort num, int places) {
            return $"0x{num.ToString($"X{places}")}";
        }
        private static string WriteHex<T>(T num, int places) where T : Enum {
            return $"0x{Convert.ToUInt64(num).ToString($"X{places}")}";
        }

        /// <summary>
        /// Writes a hex dump without the ascii interpretation on the right
        /// </summary>
        private static string WriteHexDumpNoAscii(byte[] bytes, int offsetFromLeft = 0) {
            StringBuilder stringBuilder = new StringBuilder();

            for ( int i = 0; i < bytes.Length; i++ ) {
                stringBuilder.Append(bytes[i].ToString("X2"));
                if ( i != 0 && i % HEX_DUMP_MAX_ELEMENTS_PER_LINE == 0 ) {
                    stringBuilder.Append($"\n{new string(' ', offsetFromLeft)}"); // new line + padding
                } else {
                    stringBuilder.Append("  ");
                }
            }

            return stringBuilder.ToString();
        }

        public static string ReadBytesAsString(byte[] data) {
            string unicodeString = Encoding.Unicode.GetString(data);
            unicodeString = unicodeString.Remove(unicodeString.IndexOf('\0'));
            return unicodeString;
        }

        /// <summary>
        /// Writes a hex dump with the ascii interpretation on the right
        /// </summary>
        private static string WriteHexDumpWithAscii(byte[] bytes, int length = -1, int offsetFromLeft = 0) {
            StringBuilder stringBuilder = new StringBuilder();
            StringBuilder asciiBuffer = new StringBuilder();

            if ( length == -1 ) {
                length = bytes.Length;
            }

            for ( int i = 0; i < length; i++ ) {
                stringBuilder.Append(bytes[i].ToString("X2"));

                // compute the ascii char, replace control characters with period
                char currentByte = (char)bytes[i];
                asciiBuffer.Append(char.IsControl(currentByte) ? '.' : currentByte);

                if ( i != 0 && i % HEX_DUMP_MAX_ELEMENTS_PER_LINE == 0 ) {
                    stringBuilder.Append("  ");
                    stringBuilder.Append(asciiBuffer.ToString()); // ascii on the right
                    asciiBuffer.Clear();
                    stringBuilder.Append($"\n{new string(' ', offsetFromLeft)}"); // new line + padding
                } else {
                    stringBuilder.Append(" ");
                }
            }

            // if we still have characters to print
            if (asciiBuffer.Length > 0) {
                int lengthTillEOL = (length - 1) % HEX_DUMP_MAX_ELEMENTS_PER_LINE;
                lengthTillEOL = Math.Max(HEX_DUMP_MAX_ELEMENTS_PER_LINE - lengthTillEOL, 0); // now stores bytes we need to "print" to reach the EOL
                lengthTillEOL = lengthTillEOL * 3 + 1; // * (2 chars for hex code + 1 char for whitespace) + 1 space at the end for the 3 spaces at EOL
                stringBuilder.Append(new string(' ', lengthTillEOL)); // add padding till EOL
                stringBuilder.Append(asciiBuffer.ToString());
            }

            return stringBuilder.ToString();
        }

        private static string WriteRegistryProp(RegistryKey key, string propName) {
            object regValue = key.GetValue(propName);
            if (regValue != null) {
                RegistryValueKind propType = key.GetValueKind(propName);
                switch (propType) {
                    case RegistryValueKind.DWord:
                        return $" {PropertyTitle(propName)}: REG_DWORD {( ( int ) regValue ).ToString($"X8")} ({( int ) regValue})";
                    case RegistryValueKind.QWord:
                        return $" {PropertyTitle(propName)}: REG_QWORD {( ( long ) regValue ).ToString($"X16")} ({( long ) regValue})";
                    case RegistryValueKind.Binary:
                        string prefix = $" {PropertyTitle(propName)}: REG_BINARY ";
                        string hexDump = WriteHexDumpNoAscii(( byte[]) regValue, prefix.Length );
                        return prefix + hexDump;
                }
            }
            return $" {PropertyTitle(propName)}: Not found...    ";
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

        private static string WriteDevicePowerState(CM_POWER_DATA powerState) {

            StringBuilder contentBuffer = new StringBuilder();

            switch ( powerState.PD_MostRecentPowerState ) {
                case DEVICE_POWER_STATE.PowerDeviceD0:
                    contentBuffer.Append("D0");
                    break;
                case DEVICE_POWER_STATE.PowerDeviceD1:
                    contentBuffer.Append("D1");
                    break;
                case DEVICE_POWER_STATE.PowerDeviceD2:
                    contentBuffer.Append("D2");
                    break;
                case DEVICE_POWER_STATE.PowerDeviceD3:
                    contentBuffer.Append("D3");
                    break;
            }

            if ( powerState.PD_Capabilities != PDCAP_CAPABILITIES.PDCAP_NONE ) {
                // Begin building the capabilities string
                contentBuffer.Append(" (");

                bool isD0supported = (powerState.PD_Capabilities & PDCAP_CAPABILITIES.PDCAP_D0_SUPPORTED) == PDCAP_CAPABILITIES.PDCAP_D0_SUPPORTED;
                bool isD1supported = (powerState.PD_Capabilities & PDCAP_CAPABILITIES.PDCAP_D1_SUPPORTED) == PDCAP_CAPABILITIES.PDCAP_D1_SUPPORTED;
                bool isD2supported = (powerState.PD_Capabilities & PDCAP_CAPABILITIES.PDCAP_D2_SUPPORTED) == PDCAP_CAPABILITIES.PDCAP_D2_SUPPORTED;
                bool isD3supported = (powerState.PD_Capabilities & PDCAP_CAPABILITIES.PDCAP_D3_SUPPORTED) == PDCAP_CAPABILITIES.PDCAP_D3_SUPPORTED;

                bool isWakeFromD0supported = (powerState.PD_Capabilities & PDCAP_CAPABILITIES.PDCAP_WAKE_FROM_D0_SUPPORTED) == PDCAP_CAPABILITIES.PDCAP_WAKE_FROM_D0_SUPPORTED;
                bool isWakeFromD1supported = (powerState.PD_Capabilities & PDCAP_CAPABILITIES.PDCAP_WAKE_FROM_D1_SUPPORTED) == PDCAP_CAPABILITIES.PDCAP_WAKE_FROM_D1_SUPPORTED;
                bool isWakeFromD2supported = (powerState.PD_Capabilities & PDCAP_CAPABILITIES.PDCAP_WAKE_FROM_D2_SUPPORTED) == PDCAP_CAPABILITIES.PDCAP_WAKE_FROM_D2_SUPPORTED;
                bool isWakeFromD3supported = (powerState.PD_Capabilities & PDCAP_CAPABILITIES.PDCAP_WAKE_FROM_D3_SUPPORTED) == PDCAP_CAPABILITIES.PDCAP_WAKE_FROM_D3_SUPPORTED;

                bool isEjectSupported = (powerState.PD_Capabilities & PDCAP_CAPABILITIES.PDCAP_WARM_EJECT_SUPPORTED) == PDCAP_CAPABILITIES.PDCAP_WARM_EJECT_SUPPORTED;

                // supported isn't empty
                bool supportedHasContent = false;
                if ( isD0supported || isD1supported || isD2supported || isD3supported ) {
                    supportedHasContent = true;
                    contentBuffer.Append("supported: ");
                    bool appendedSomething = false;
                    if ( isD0supported ) {
                        contentBuffer.Append("D0");
                        appendedSomething = true;
                    }
                    if ( isD1supported ) {
                        if ( appendedSomething )
                            contentBuffer.Append(", ");
                        contentBuffer.Append("D1");
                        appendedSomething = true;
                    }
                    if ( isD2supported ) {
                        if ( appendedSomething )
                            contentBuffer.Append(", ");
                        contentBuffer.Append("D2");
                        appendedSomething = true;
                    }
                    if ( isD3supported ) {
                        if ( appendedSomething )
                            contentBuffer.Append(", ");
                        contentBuffer.Append("D3");
                        appendedSomething = true;
                    }
                }

                // wake from isn't empty
                bool wakeFromHasContent = false;
                if ( isWakeFromD0supported || isWakeFromD1supported || isWakeFromD2supported || isWakeFromD3supported ) {
                    wakeFromHasContent = true;
                    if ( supportedHasContent ) {
                        contentBuffer.Append(", ");
                    }
                    bool appendedSomething = false;
                    if ( isWakeFromD0supported ) {
                        contentBuffer.Append("wake from D0");
                        appendedSomething = true;
                    }
                    if ( isWakeFromD1supported ) {
                        if ( appendedSomething )
                            contentBuffer.Append(", ");
                        contentBuffer.Append("wake from D1");
                        appendedSomething = true;
                    }
                    if ( isWakeFromD2supported ) {
                        if ( appendedSomething )
                            contentBuffer.Append(", ");
                        contentBuffer.Append("wake from D2");
                        appendedSomething = true;
                    }
                    if ( isWakeFromD3supported ) {
                        if ( appendedSomething )
                            contentBuffer.Append(", ");
                        contentBuffer.Append("wake from D3");
                        appendedSomething = true;
                    }
                }

                if (isEjectSupported) {
                    if ( wakeFromHasContent || supportedHasContent ) 
                        contentBuffer.Append(", ");
                    contentBuffer.Append("warm eject");
                }

                contentBuffer.Append(")");
            }

            return contentBuffer.ToString();
        }

        private static string TryPrintEnum<T>(T enumValue) where T : struct, IConvertible {
            if ( !typeof(T).IsEnum ) {
                throw new ArgumentException("T must be an enumerated type");
            }

            uint enumAsNum = Convert.ToUInt32(enumValue);
            string enumAsString = enumValue.ToString();
            if ( enumAsNum.ToString() == enumAsString )
                return " (-)";
            return $" ({enumAsString})";
        }

        #endregion

        public static string GetInfoStringForRootNode(UsbTreeState treeState) {

            StringBuilder contentString = new StringBuilder();

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

            #region Up time
            TimeSpan upTime = TimeSpan.FromMilliseconds(Kernel32.GetTickCount64());
            string finalUptimeString = "";
            {
                string prefix = "";
                bool addComma = false;

                if ( upTime.Days > 0 ) {
                    prefix = ( addComma ? ", " : string.Empty );
                    finalUptimeString += prefix + upTime.Days + ( upTime.Days == 1 ? " day" : " days" );
                    addComma = true;
                }
                if ( upTime.Hours > 0 ) {
                    prefix = ( addComma ? ", " : string.Empty );
                    finalUptimeString += prefix + upTime.Hours + ( upTime.Hours == 1 ? " hour" : " hours" );
                    addComma = true;
                }
                if ( upTime.Minutes > 0 ) {
                    prefix = ( addComma ? ", " : string.Empty );
                    finalUptimeString += prefix + upTime.Minutes + ( upTime.Minutes == 1 ? " minute" : " minutes" );
                    addComma = true;
                }
            }
            #endregion

            string osString = "";

            RegistryKey registryUsbCommonSettings = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\USB", false);
            RegistryKey registryAutoRemovalKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\USB\\AutomaticSurpriseRemoval", false);

            contentString.Append("\n\t========================== My Computer ==========================\n");
            contentString.Append($"{PropertyTitle("Operating System")}: {osString}\n");
            contentString.Append($"{PropertyTitle("Up Time")}: {finalUptimeString}\n");
            contentString.Append($"{PropertyTitle("Computer Name")}: {Environment.MachineName}\n");
            contentString.Append($"{PropertyTitle("Admin Privileges")}: {WriteBool(Util.IsCurrentProcessElevated())}\n");
            contentString.Append($"{PropertyTitle("User Account Control")}: {WriteBool(true)}\n"); // @TODO: Implement

            contentString.Append("\n");

            contentString.Append($"{PropertyTitle("UsbTreeView Version")}: {Assembly.GetExecutingAssembly().GetName().Version} ({processorArch})\n");
            contentString.Append($"{PropertyTitle("Settings")}: {Settings.Instance.SettingsPath}\n");

            contentString.Append("\n");

            contentString.Append($"{PropertyTitle("USB Host Controllers")}: {treeState.HostControllers}\n");
            contentString.Append($"{PropertyTitle("USB Root Hubs")}: {treeState.RootHubs}\n");
            contentString.Append($"{PropertyTitle("USB Standard Hubs")}: {treeState.ExternalHubs}\n");
            contentString.Append($"{PropertyTitle("USB Peripheral Devices")}: {treeState.PeripheralDevices}\n");

            contentString.Append("\n");

            contentString.Append($"{PropertyTitle("Device Class Filter Drivers")}:\n");
            // contentString.Append($"{PropertyTitle("USB Upper")}: {friendlyName}\n");

            contentString.Append("\n\n");
            contentString.Append("\t\t+++++++++++++++++ Registry USB Flags +++++++++++++++++\n");
            contentString.Append($"{PropertyTitle("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\USB")}\n");
            contentString.Append($"{WriteRegistryProp(registryUsbCommonSettings, "DualRoleFeatures")}\n");
            contentString.Append($"{WriteRegistryProp(registryUsbCommonSettings, "OsDefaultRoleSwitchMode")}\n");
            contentString.Append($"{WriteRegistryProp(registryUsbCommonSettings, "UcmIsPresent")}\n");
            contentString.Append($"{PropertyTitle("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\USB\\AutomaticSurpriseRemoval")}\n");
            contentString.Append($"{WriteRegistryProp(registryAutoRemovalKey, "AttemptRecoveryFromUsbPowerDrain")}\n");

            contentString.Append($@"
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
");

            contentString.Append("\n\n"); // End

            return contentString.ToString();
        }

        public static string GetInfoStringForHostController(UsbHostControllerInfo usbController) {

            StringBuilder contentString = new StringBuilder();

            string vendorIdName = UsbDatabase.GetPCIeVendorName((ushort)usbController.ControllerInfo.PciVendorId);
            string productIdName = UsbDatabase.GetPCIeProductName((ushort)usbController.ControllerInfo.PciVendorId, (ushort)usbController.ControllerInfo.PciDeviceId);

            contentString.Append("\n\t===================== USB Host Controller =======================\n");

            #region Device Information

            // DeviceDesc

            contentString.Append("\t\t+++++++++++++++++ Device Information ++++++++++++++++++\n");
            contentString.Append($"{PropertyTitle("Friendly Name")}: {usbController.UsbDeviceProperties.FriendlyName}\n");
            contentString.Append($"{PropertyTitle("Device Description")}: {usbController.UsbDeviceProperties.DeviceDesc}\n");
            contentString.Append($"{PropertyTitle("Device Path")}: {usbController.DevicePath}\n");
            contentString.Append($"{PropertyTitle("Kernel Name")}: {usbController.UsbDeviceProperties.DeviceDesc}\n");
            contentString.Append($"{PropertyTitle("Device ID")}: {usbController.UsbDeviceProperties.DeviceId}\n");
            contentString.Append($"{PropertyTitle("Vendor")}: {WriteHex(usbController.ControllerInfo.PciVendorId, 4)}{( vendorIdName == null ? "" : $" ({vendorIdName})" )}\n");
            contentString.Append($"{PropertyTitle("Hardware IDs")}: {usbController.UsbDeviceProperties.HwId}\n");
            contentString.Append($"{PropertyTitle("Driver KeyName")}: {usbController.DriverKey}\n");
            contentString.Append($"{PropertyTitle("Driver")}: {usbController.UsbDeviceProperties.DeviceDesc}\n");
            contentString.Append($"{PropertyTitle("Driver Inf")}: {usbController.UsbDeviceProperties.DeviceDesc}\n");
            contentString.Append($"{PropertyTitle("Legacy BusType")}: {usbController.UsbDeviceProperties.LegacyBusType}\n");
            contentString.Append($"{PropertyTitle("Class")}: {usbController.UsbDeviceProperties.DeviceClass}\n");
            contentString.Append($"{PropertyTitle("Class GUID")}: {usbController.UsbDeviceProperties.DeviceClassGuid}\n");
            contentString.Append($"{PropertyTitle("Service")}: {usbController.UsbDeviceProperties.Service}\n");
            contentString.Append($"{PropertyTitle("Enumerator")}: {usbController.UsbDeviceProperties.Enumerator}\n");
            contentString.Append($"{PropertyTitle("Location Info")}: {usbController.UsbDeviceProperties.LocationInfo}\n");
            contentString.Append($"{PropertyTitle("Location IDs")}: {usbController.UsbDeviceProperties.LocationPaths}\n");
            contentString.Append($"{PropertyTitle("Container ID")}: {usbController.UsbDeviceProperties.ContainerId}\n");
            contentString.Append($"{PropertyTitle("Manufacturer Info")}: {usbController.UsbDeviceProperties.Manufacturer}\n");
            contentString.Append($"{PropertyTitle("Capabilities")}: {WriteHex(usbController.UsbDeviceProperties.Capabilities, 2)}{TryPrintEnum(usbController.UsbDeviceProperties.Capabilities)}\n");
            contentString.Append($"{PropertyTitle("Status")}: {WriteHex(usbController.UsbDeviceProperties.Status, 8)}{TryPrintEnum(usbController.UsbDeviceProperties.Status)}\n");
            contentString.Append($"{PropertyTitle("Problem Code")}: {usbController.UsbDeviceProperties.ProblemCode}\n");
            contentString.Append($"{PropertyTitle("Address")}: {usbController.UsbDeviceProperties.Address}\n");
            contentString.Append($"{PropertyTitle("Power State")}: {WriteDevicePowerState(usbController.UsbDeviceProperties.PowerState)}\n");

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
            contentString.Append($"\n{PropertyTitle("Roothub Symbolic Link")}: {usbController.SymbolicLink}\n");

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

        public static string GetInfoStringForUsbHub(UsbHubInfo usbHubInfo) {

            StringBuilder contentString = new StringBuilder();

            switch ( usbHubInfo.DeviceInfoType ) {
                case UsbDeviceInfoType.RootHub:
                    contentString.Append("\n\t========================= USB Root Hub =========================\n");
                    contentString.Append($"{PropertyTitle("Num ports")}: {usbHubInfo.HubInfo.Value.u.HubInformation.HubDescriptor.bNumberOfPorts}\n");
                    break;
                case UsbDeviceInfoType.ExternalHub:
                    contentString.Append("\n\t  ========================== USB Hub =========================\n");
                    break;
            }

            if ( usbHubInfo.UsbDeviceProperties != null ) {
                contentString.Append("\n\t\t+++++++++++++++++ Device Information ++++++++++++++++++\n");
                contentString.Append($"{PropertyTitle("Device Description")}: {usbHubInfo.UsbDeviceProperties.DeviceDesc}\n");
                contentString.Append($"{PropertyTitle("Device Path")}: {usbHubInfo.DevicePath}\n");
                contentString.Append($"{PropertyTitle("Kernel Name")}: {usbHubInfo.UsbDeviceProperties.DeviceDesc}\n");
                contentString.Append($"{PropertyTitle("Device ID")}: {usbHubInfo.UsbDeviceProperties.DeviceId}\n");
                contentString.Append($"{PropertyTitle("Hardware IDs")}: {usbHubInfo.UsbDeviceProperties.HwId}\n");
                contentString.Append($"{PropertyTitle("Driver KeyName")}: {usbHubInfo.DriverKey}\n");
                contentString.Append($"{PropertyTitle("Driver")}: {usbHubInfo.UsbDeviceProperties.DeviceDesc}\n");
                contentString.Append($"{PropertyTitle("Driver Inf")}: {usbHubInfo.UsbDeviceProperties.DeviceDesc}\n");
                contentString.Append($"{PropertyTitle("Legacy BusType")}: {usbHubInfo.UsbDeviceProperties.LegacyBusType}\n");
                contentString.Append($"{PropertyTitle("Class")}: {usbHubInfo.UsbDeviceProperties.DeviceClass}\n");
                contentString.Append($"{PropertyTitle("Class GUID")}: {usbHubInfo.UsbDeviceProperties.DeviceClassGuid}\n");
                contentString.Append($"{PropertyTitle("Service")}: {usbHubInfo.UsbDeviceProperties.Service}\n");
                contentString.Append($"{PropertyTitle("Enumerator")}: {usbHubInfo.UsbDeviceProperties.Enumerator}\n");
                contentString.Append($"{PropertyTitle("Location Info")}: {usbHubInfo.UsbDeviceProperties.LocationInfo}\n");
                contentString.Append($"{PropertyTitle("Location IDs")}: {usbHubInfo.UsbDeviceProperties.LocationPaths}\n");
                contentString.Append($"{PropertyTitle("Container ID")}: {usbHubInfo.UsbDeviceProperties.ContainerId}\n");
                contentString.Append($"{PropertyTitle("Manufacturer Info")}: {usbHubInfo.UsbDeviceProperties.Manufacturer}\n");
                contentString.Append($"{PropertyTitle("Capabilities")}: {WriteHex(( uint ) usbHubInfo.UsbDeviceProperties.Capabilities, 2)}{TryPrintEnum(usbHubInfo.UsbDeviceProperties.Capabilities)}\n");
                contentString.Append($"{PropertyTitle("Status")}: {WriteHex(( uint ) usbHubInfo.UsbDeviceProperties.Status, 8)}{TryPrintEnum(usbHubInfo.UsbDeviceProperties.Status)}\n");
                contentString.Append($"{PropertyTitle("Problem Code")}: {usbHubInfo.UsbDeviceProperties.ProblemCode}\n");
                contentString.Append($"{PropertyTitle("Address")}: {usbHubInfo.UsbDeviceProperties.Address}\n");
                contentString.Append($"{PropertyTitle("IdleInWorkingState")}: {usbHubInfo.UsbDeviceProperties.Address}\n");
                contentString.Append($"{PropertyTitle("Power State")}: {WriteDevicePowerState(usbHubInfo.UsbDeviceProperties.PowerState)}\n");
            }

            if ( usbHubInfo.HubInfo.HasValue ) {
                contentString.Append("\n\t\t------------------- USB Hub Descriptor -----------------\r\n\n");
                switch ( usbHubInfo.HubInfo.Value.NodeType ) {
                    case USB_HUB_NODE.UsbHub:
                        contentString.Append($"{PropertyTitle("bDescriptorLength")}: {  usbHubInfo.HubInfo.Value.u.HubInformation.HubDescriptor.bDescriptorLength}\n");
                        contentString.Append($"{PropertyTitle("bDescriptorType")}: {    usbHubInfo.HubInfo.Value.u.HubInformation.HubDescriptor.bDescriptorType}\n");
                        contentString.Append($"{PropertyTitle("bNumberOfPorts")}: {     usbHubInfo.HubInfo.Value.u.HubInformation.HubDescriptor.bNumberOfPorts}\n");
                        contentString.Append($"{PropertyTitle("wHubCharacteristics")}: {usbHubInfo.HubInfo.Value.u.HubInformation.HubDescriptor.wHubCharacteristics}\n");
                        contentString.Append($"{PropertyTitle("bPowerOnToPowerGood")}: {WriteHex(usbHubInfo.HubInfo.Value.u.HubInformation.HubDescriptor.bPowerOnToPowerGood, 2)} ({usbHubInfo.HubInfo.Value.u.HubInformation.HubDescriptor.bPowerOnToPowerGood} ms)\n");
                        contentString.Append($"{PropertyTitle("bHubControlCurrent")}: { WriteHex(usbHubInfo.HubInfo.Value.u.HubInformation.HubDescriptor.bHubControlCurrent, 2)} ({usbHubInfo.HubInfo.Value.u.HubInformation.HubDescriptor.bHubControlCurrent} mA)\n");
                        // contentString.Append($"{PropertyTitle("bRemoveAndPowerMask")}: {usbHubInfo.HubInfo.Value.UsbHubDescriptor.bRemoveAndPowerMask}\n");
                        contentString.Append($"{PropertyTitle("HubIsBusPowered")}: {usbHubInfo.HubInfo.Value.u.HubInformation.HubIsBusPowered}\n");
                        break;
                    case USB_HUB_NODE.UsbMIParent:
                        contentString.Append($"{PropertyTitle("NumberOfInterfaces")}: {usbHubInfo.HubInfo.Value.u.MiParentInformation.NumberOfInterfaces}\n");
                        break;
                }
            }

            if ( usbHubInfo.HubInfoEx.HasValue ) {
                contentString.Append("\n\t  ---------------- Extended USB Hub Descriptor ---------------\n");
                switch (usbHubInfo.HubInfoEx.Value.HubType) {
                    case USB_HUB_TYPE.Usb20Hub:
                        contentString.Append($"{PropertyTitle("HubType")}: {WriteHex(usbHubInfo.HubInfoEx.Value.HubType, 2)} ({usbHubInfo.HubInfoEx.Value.HubType} - hub descriptor is defined in USB 3.0 specification)\n");
                        break;
                    case USB_HUB_TYPE.Usb30Hub:
                        contentString.Append($"{PropertyTitle("HubType")}: {WriteHex(usbHubInfo.HubInfoEx.Value.HubType, 2)} ({usbHubInfo.HubInfoEx.Value.HubType} - hub descriptor is defined in USB 2.0 and 1.1 specifications)\n");
                        break;
                    case USB_HUB_TYPE.UsbRootHub:
                        contentString.Append($"{PropertyTitle("HubType")}: {WriteHex(usbHubInfo.HubInfoEx.Value.HubType, 2)} ({usbHubInfo.HubInfoEx.Value.HubType} - a root hub)\n");
                        break;

                }
                contentString.Append($"{PropertyTitle("HighestPortNumber")}: {WriteHex(usbHubInfo.HubInfoEx.Value.HighestPortNumber, 2)} (Port {usbHubInfo.HubInfoEx.Value.HighestPortNumber} is the highest)\n");
            }

            if ( usbHubInfo.HubCapabilityEx.HasValue ) {
                contentString.Append("\n\t\t----------------- USB Hub Capabilities ----------------\n");
                // the actual structure is deprecated but we can infer its data like so:
                contentString.Append($"{PropertyTitle("HubIs2xCapable")}: {(usbHubInfo.HubCapabilityEx.Value.CapabilityFlags.HasFlag(USB_HUB_CAP_FLAGS.HubIsHighSpeedCapable) ? "1 (Is 2.x capable)" : "0 (Is not 2.x capable)" ) }\n");
            }
            
            if ( usbHubInfo.HubCapabilityEx.HasValue ) {
                contentString.Append("\n\t\t--------------- USB Hub Capabilities Ex ---------------\n");
                bool HubIsHighSpeedCapable      = usbHubInfo.HubCapabilityEx.Value.CapabilityFlags.HasFlag(USB_HUB_CAP_FLAGS.HubIsHighSpeedCapable);
                bool HubIsHighSpeed             = usbHubInfo.HubCapabilityEx.Value.CapabilityFlags.HasFlag(USB_HUB_CAP_FLAGS.HubIsHighSpeed);
                bool HubIsMultiTtCapable        = usbHubInfo.HubCapabilityEx.Value.CapabilityFlags.HasFlag(USB_HUB_CAP_FLAGS.HubIsMultiTtCapable);
                bool HubIsMultiTt               = usbHubInfo.HubCapabilityEx.Value.CapabilityFlags.HasFlag(USB_HUB_CAP_FLAGS.HubIsMultiTt);
                bool HubIsRoot                  = usbHubInfo.HubCapabilityEx.Value.CapabilityFlags.HasFlag(USB_HUB_CAP_FLAGS.HubIsRoot);
                bool HubIsArmedWakeOnConnect    = usbHubInfo.HubCapabilityEx.Value.CapabilityFlags.HasFlag(USB_HUB_CAP_FLAGS.HubIsArmedWakeOnConnect);
                bool HubIsBusPowered            = usbHubInfo.HubCapabilityEx.Value.CapabilityFlags.HasFlag(USB_HUB_CAP_FLAGS.HubIsBusPowered);
                contentString.Append($"{PropertyTitle("HubIsHighSpeedCapable")}: {( HubIsHighSpeedCapable ? "1" : "0")} ({WriteBool(HubIsHighSpeedCapable)})\n");
                contentString.Append($"{PropertyTitle("HighSpeed")}: {(HubIsHighSpeed ? "1" : "0")} ({WriteBool(HubIsHighSpeed)})\n");
                contentString.Append($"{PropertyTitle("MultiTtCapable")}: {( HubIsMultiTtCapable ? "1" : "0")} ({WriteBool(HubIsMultiTtCapable)})\n");
                contentString.Append($"{PropertyTitle("HubIsMultiTt")}: {( HubIsMultiTt ? "1" : "0")} ({WriteBool(HubIsMultiTt)})\n");
                contentString.Append($"{PropertyTitle("IsRoot")}: {( HubIsRoot ? "1" : "0")} ({WriteBool(HubIsRoot)})\n");
                contentString.Append($"{PropertyTitle("ArmedWakeOnConnect")}: {( HubIsArmedWakeOnConnect ? "1" : "0")} ({WriteBool(HubIsArmedWakeOnConnect)})\n");
                contentString.Append($"{PropertyTitle("IsBusPowered")}: {( HubIsBusPowered ? "1" : "0")} ({WriteBool(HubIsBusPowered)})\n");
            }

            contentString.Append("\n\n"); // End

            return contentString.ToString();
        }

        public static string GetInfoStringForUsbDevice(USBDEVICEINFO usbDevice) {

            StringBuilder contentString = new StringBuilder();

            contentString.Append($"\n\t  ========================== USB Port{usbDevice.ConnectionInfo.ConnectionIndex} =========================\n");

            contentString.Append($"{PropertyTitle("Connection Status")}: {usbDevice.ConnectionInfo.ConnectionStatus}\n");
            contentString.Append($"{PropertyTitle("CompanionPortNumber")}: {usbDevice.PortConnectorProps.CompanionPortNumber}\n");

            if ( usbDevice.ConnectionInfo.ConnectionStatus != USB_CONNECTION_STATUS.NoDeviceConnected ) {
                if ( usbDevice.UsbDeviceProperties != null ) {

                    string vendorIdName = UsbDatabase.GetUsbVendorName((ushort)usbDevice.UsbDeviceProperties.VendorID);
                    string productIdName = UsbDatabase.GetUsbProductName((ushort)usbDevice.UsbDeviceProperties.VendorID, (ushort)usbDevice.UsbDeviceProperties.ProductID);

                    contentString.Append($"\n      ======================== USB Device ========================\n");

                    contentString.Append($"\n\t\t+++++++++++++++++ Device Information ++++++++++++++++++\n");
                    contentString.Append($"{PropertyTitle("Device Description")}: {usbDevice.UsbDeviceProperties.DeviceDesc}\n");
                    contentString.Append($"{PropertyTitle("Device Path")}: {usbDevice.DevicePath}\n");
                    contentString.Append($"{PropertyTitle("Kernel Name")}: {usbDevice.UsbDeviceProperties.DeviceDesc}\n");
                    contentString.Append($"{PropertyTitle("Device ID")}: {usbDevice.UsbDeviceProperties.DeviceId}\n");
                    contentString.Append($"{PropertyTitle("Vendor ID")}: {WriteHex(usbDevice.UsbDeviceProperties.VendorID, 4)}{( vendorIdName == null ? "" : $" ({vendorIdName})" )}\n");
                    contentString.Append($"{PropertyTitle("Product ID")}: {WriteHex(usbDevice.UsbDeviceProperties.ProductID, 4)}{( productIdName == null ? "" : $" ({productIdName})" )}\n");
                    contentString.Append($"{PropertyTitle("Hardware IDs")}: {usbDevice.UsbDeviceProperties.HwId}\n");
                    contentString.Append($"{PropertyTitle("Driver KeyName")}: {usbDevice.DriverKey}\n");
                    contentString.Append($"{PropertyTitle("Driver")}: {usbDevice.UsbDeviceProperties.DeviceDesc}\n");
                    contentString.Append($"{PropertyTitle("Driver Inf")}: {usbDevice.UsbDeviceProperties.DeviceDesc}\n");
                    contentString.Append($"{PropertyTitle("Legacy BusType")}: {usbDevice.UsbDeviceProperties.LegacyBusType}\n");
                    contentString.Append($"{PropertyTitle("Class")}: {usbDevice.UsbDeviceProperties.DeviceClass}\n");
                    contentString.Append($"{PropertyTitle("Class GUID")}: {usbDevice.UsbDeviceProperties.DeviceClassGuid}\n");
                    contentString.Append($"{PropertyTitle("Service")}: {usbDevice.UsbDeviceProperties.Service}\n");
                    contentString.Append($"{PropertyTitle("Enumerator")}: {usbDevice.UsbDeviceProperties.Enumerator}\n");
                    contentString.Append($"{PropertyTitle("Location Info")}: {usbDevice.UsbDeviceProperties.LocationInfo}\n");
                    contentString.Append($"{PropertyTitle("Location IDs")}: {usbDevice.UsbDeviceProperties.LocationPaths}\n");
                    contentString.Append($"{PropertyTitle("Container ID")}: {usbDevice.UsbDeviceProperties.ContainerId}\n");
                    contentString.Append($"{PropertyTitle("Manufacturer Info")}: {usbDevice.UsbDeviceProperties.Manufacturer}\n");
                    contentString.Append($"{PropertyTitle("Capabilities")}: {WriteHex(usbDevice.UsbDeviceProperties.Capabilities, 2)}{TryPrintEnum(usbDevice.UsbDeviceProperties.Capabilities)}\n");
                    contentString.Append($"{PropertyTitle("Status")}: {WriteHex(usbDevice.UsbDeviceProperties.Status, 8)}{TryPrintEnum(usbDevice.UsbDeviceProperties.Status)}\n");
                    contentString.Append($"{PropertyTitle("Problem Code")}: {usbDevice.UsbDeviceProperties.ProblemCode}\n");
                    contentString.Append($"{PropertyTitle("Address")}: {usbDevice.UsbDeviceProperties.Address}\n");
                    contentString.Append($"{PropertyTitle("Power State")}: {WriteDevicePowerState(usbDevice.UsbDeviceProperties.PowerState)}\n");

                }

                contentString.Append($"\n\t  -------------------- String Descriptors -------------------\n");
                if ( usbDevice.StringDescs != null ) {
                    contentString.Append($"\n\t\t\t ------ String Descriptor 0 ------\n");

                    // Special case: String descriptor 0 (language identifier)
                    contentString.Append($"{PropertyTitle("bLength")}: {WriteHex(usbDevice.StringDescs.Lang_bLength, 2)} ({usbDevice.StringDescs.Lang_bLength} bytes)\n");
                    contentString.Append($"{PropertyTitle("bDescriptorType")}: {WriteHex(usbDevice.StringDescs.Lang_bDescriptorType, 2)} ({usbDevice.StringDescs.Lang_bDescriptorType})\n");
                    for ( int langId = 0; langId < usbDevice.StringDescs.LanguageIds.Length; langId++ ) {
                        contentString.Append($"{PropertyTitle($"LanguageID[{langId}]")}: {WriteHex(usbDevice.StringDescs.LanguageIds[langId], 4)}{( usbDevice.StringDescs.LanguageIds[langId] == 0x0409 ? " (English - United States)" : "" )}\n");
                    }

                    string hexDumpStr = $"{PropertyTitle("Data (HexDump)")}: ";
                    foreach ( var stringDesc in usbDevice.StringDescs.Strings ) {
                        contentString.Append($"\n\t\t\t ------ String Descriptor {stringDesc.DescriptorIndex} ------\n");
                        contentString.Append($"{PropertyTitle("bLength")}: {WriteHex(stringDesc.StringDescriptor.bLength, 2)} ({stringDesc.StringDescriptor.bLength} bytes)\n");
                        contentString.Append($"{PropertyTitle("bDescriptorType")}: {WriteHex(stringDesc.StringDescriptor.bDescriptorType, 2)} ({stringDesc.StringDescriptor.bDescriptorType})\n");
                        contentString.Append($"{PropertyTitle($"Language {WriteHex(stringDesc.LanguageID, 4)}")}: \"{ReadBytesAsString(stringDesc.StringDescriptor.bString)}\"\n");
                        contentString.Append(hexDumpStr);
                        contentString.Append(WriteHexDumpWithAscii(stringDesc.StringDescriptor.bString, stringDesc.StringDescriptor.bLength, hexDumpStr.Length));
                        contentString.Append("\n");
                    }
                } else {
                    if ( usbDevice.DeviceInfoNode?.LatestDevicePowerState == DEVICE_POWER_STATE.PowerDeviceD0 ) {
                        contentString.Append($"String descriptors are not available for this device!\n");
                    } else {
                        contentString.Append($"String descriptors are not available  (because the device is in low power state).\n");
                    }
                }
            }

            contentString.Append("\n\n"); // End

            return contentString.ToString();
        }
    }
}
