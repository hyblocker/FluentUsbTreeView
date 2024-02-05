using FluentUsbTreeView.UsbTreeView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentUsbTreeView.Ui {
    public static class DeviceNameUtil {

        private static string CleanupManufacturerString(string manufacturer) {
            return manufacturer.Replace("Corporation", "").Replace("Corp.", "").Replace("Corp", "").Trim();
        }

        private static string GetFriendlyName(string friendlyName, string manufacturer, string deviceDesc) {
            // @TODO: Respect settings
            if ( friendlyName != null && friendlyName.Trim().Length > 0 )
                return friendlyName;

            if ( manufacturer != null ) {
                return $"{manufacturer} {deviceDesc}";
            }

            return deviceDesc;
        }

        public static string GetFriendlyUsbHostControllerName(UsbHostControllerInfo usbHostControllerInfo) {
            return GetFriendlyName(usbHostControllerInfo.UsbDeviceProperties.FriendlyName, usbHostControllerInfo.UsbDeviceProperties.Manufacturer, usbHostControllerInfo.UsbDeviceProperties.DeviceDesc);
        }

        public static string GetFriendlyUsbHubName(UsbHubInfo usbHubInfo) {
            // @TODO: Improve lol
            return usbHubInfo.UsbDeviceProperties.DeviceDesc;
            return GetFriendlyName(usbHubInfo.UsbDeviceProperties.FriendlyName, usbHubInfo.UsbDeviceProperties.Manufacturer, usbHubInfo.UsbDeviceProperties.DeviceDesc);
        }

        public static string GetFriendlyUsbDeviceName(USBDEVICEINFO usbDeviceInfo) {
            // get string descriptor values
            string manufacturer = null;
            string product = null;
            string serialNumber = null;

            if ( usbDeviceInfo.StringDescs != null ) {
                // @TODO: Make less error prone
                try {

                    if ( usbDeviceInfo.ConnectionInfo.DeviceDescriptor.iManufacturer != 0 ) {
                        manufacturer = usbDeviceInfo.StringDescs.Strings[usbDeviceInfo.ConnectionInfo.DeviceDescriptor.iManufacturer - 1].GetStringData();
                    }
                    if ( usbDeviceInfo.ConnectionInfo.DeviceDescriptor.iProduct != 0 ) {
                        product = usbDeviceInfo.StringDescs.Strings[usbDeviceInfo.ConnectionInfo.DeviceDescriptor.iProduct - 1].GetStringData();
                    }
                    if ( usbDeviceInfo.ConnectionInfo.DeviceDescriptor.iSerialNumber != 0 ) {
                        serialNumber = usbDeviceInfo.StringDescs.Strings[usbDeviceInfo.ConnectionInfo.DeviceDescriptor.iSerialNumber - 1].GetStringData();
                    }
                } catch (Exception e) {
                    Logger.Fatal(e.ToString() + "\n" + e.StackTrace);
                }
            }

            if ( manufacturer == null ) {
                manufacturer = UsbDatabase.GetUsbVendorName((ushort)usbDeviceInfo.UsbDeviceProperties.VendorID);
                if ( manufacturer == null ) {
                    manufacturer = usbDeviceInfo.UsbDeviceProperties.Manufacturer;
                }
            }
            if ( product == null ) {
                product = usbDeviceInfo.UsbDeviceProperties.DeviceDesc;
            }

            manufacturer = CleanupManufacturerString(manufacturer);


            return GetFriendlyName(usbDeviceInfo.UsbDeviceProperties.FriendlyName, manufacturer, product);
        }
    }
}
