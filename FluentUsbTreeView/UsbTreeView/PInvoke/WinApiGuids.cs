﻿using System;

namespace FluentUsbTreeView.PInvoke {
    public static class WinApiGuids {
        
        /* USB specific GUIDs */
        public static readonly Guid GUID_DEVCLASS_USB                       = new Guid(0xF18A0E88, 0xC30C, 0x11D0, 0x88, 0x15, 0x00, 0xA0, 0xC9, 0x06, 0xBE, 0xD8);
        public static readonly Guid GUID_DEVINTERFACE_USB_HUB               = new Guid(0xF18A0E88, 0xC30C, 0x11D0, 0x88, 0x15, 0x00, 0xA0, 0xC9, 0x06, 0xBE, 0xD8);
        public static readonly Guid GUID_DEVINTERFACE_USB_BILLBOARD         = new Guid(0x5E9ADAEF, 0xF879, 0x473F, 0xB8, 0x07, 0x4E, 0x5E, 0xA7, 0x7D, 0x1B, 0x1C);
        public static readonly Guid GUID_DEVINTERFACE_USB_DEVICE            = new Guid(0xA5DCBF10, 0x6530, 0x11D2, 0x90, 0x1F, 0x00, 0xC0, 0x4F, 0xB9, 0x51, 0xED);
        public static readonly Guid GUID_DEVINTERFACE_USB_HOST_CONTROLLER   = new Guid(0x3ABF6F2D, 0x71c4, 0x462a, 0x8a, 0x92, 0x1e, 0x68, 0x61, 0xE6, 0xAF, 0x27);
        public static readonly Guid GUID_USB_WMI_STD_DATA                   = new Guid(0x4E623B20, 0xCB14, 0x11D1, 0xB3, 0x31, 0x00, 0xA0, 0xC9, 0x59, 0xBB, 0xD2);
        public static readonly Guid GUID_USB_WMI_STD_NOTIFICATION           = new Guid(0x4E623B20, 0xCB14, 0x11D1, 0xB3, 0x31, 0x00, 0xA0, 0xC9, 0x59, 0xBB, 0xD2);


        public static readonly Guid GUID_CLASS_USB_HOST_CONTROLLER          = new Guid(0x3ABF6F2D, 0x71c4, 0x462a, 0x8a, 0x92, 0x1e, 0x68, 0x61, 0xE6, 0xAF, 0x27);
        public static readonly Guid GUID_CLASS_USB_DEVICE                   = new Guid(0xA5DCBF10, 0x6530, 0x11D2, 0x90, 0x1F, 0x00, 0xC0, 0x4F, 0xB9, 0x51, 0xED);
        public static readonly Guid GUID_CLASS_USBHUB                       = new Guid(0xF18A0E88, 0xC30C, 0x11D0, 0x88, 0x15, 0x00, 0xA0, 0xC9, 0x06, 0xBE, 0xD8);

    }
}
