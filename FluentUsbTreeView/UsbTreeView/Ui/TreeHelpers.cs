using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows;
using WpfUiImage = Wpf.Ui.Controls.Image;
using WpfUiTextBlock = Wpf.Ui.Controls.TextBlock;

namespace FluentUsbTreeView.Ui {
    public static class TreeHelpers {
        public static StackPanel CreateTreeViewContentWithIcon(string text, UsbTreeIcon icon) {
            // Add Icon
            // Create Stack Panel
            StackPanel stkPanelElement = new StackPanel();
            stkPanelElement.Orientation = Orientation.Horizontal;

            // @TODO: Figure out how to make this DPI aware

            // Create Image
            WpfUiImage imageElement = new WpfUiImage();
            string iconUrl;
            switch ( icon ) {
                case UsbTreeIcon.Computer:
                    iconUrl = "pack://application:,,,/Assets/computer-256.png";
                    break;
                case UsbTreeIcon.UsbHub:
                    iconUrl = "pack://application:,,,/Assets/usb-hub-256.png";
                    break;
                case UsbTreeIcon.GoodDevice:
                    iconUrl = "pack://application:,,,/Assets/usb-256.png";
                    break;
                case UsbTreeIcon.GoodSsDevice:
                    iconUrl = "pack://application:,,,/Assets/usb-ssusb-256.png";
                    break;
                case UsbTreeIcon.HostController:
                    iconUrl = "pack://application:,,,/Assets/usb-controller-256.png";
                    break;
                case UsbTreeIcon.NoSsDevice:
                    iconUrl = "pack://application:,,,/Assets/usb-ssport-256.png";
                    break;
                case UsbTreeIcon.NoDevice:
                    iconUrl = "pack://application:,,,/Assets/usb-port-256.png";
                    break;
                case UsbTreeIcon.BadDevice:
                    iconUrl = "pack://application:,,,/Assets/usb-malfunction-256.png";
                    break;
                default:
                    iconUrl = "pack://application:,,,/Assets/computer-256.png";
                    break;
            }
            imageElement.Source = new BitmapImage(new Uri(iconUrl));
            imageElement.Width = 16;
            imageElement.Height = 16;
            imageElement.Margin = new Thickness(0, 0, 8, 0);

            // Create TextBlock
            WpfUiTextBlock textElement = new WpfUiTextBlock();
            textElement.Text = text;

            // Add to stack
            stkPanelElement.Children.Add(imageElement);
            stkPanelElement.Children.Add(textElement);

            return stkPanelElement;
        }
    }
}
