﻿using System;
using System.Runtime.InteropServices;

namespace FluentUsbTreeView.PInvoke {


    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct DEVPROPKEY {
        public Guid fmtid;
        public uint pid;

        public DEVPROPKEY(uint a, ushort b, ushort c, byte d, byte e, byte f, byte g, byte h, byte i, byte j, byte k, uint pid) {
            this.pid = pid;
            this.fmtid = new Guid(a, b, c, d, e, f, g, h, i, j, k);
        }
    }

    public enum DEVPROPSTORE {
        /// <summary/>
        DEVPROP_STORE_SYSTEM,

        /// <summary/>
        DEVPROP_STORE_USER
    }

    [Flags]
    public enum DEVPROPTYPE : uint {
        DEVPROP_TYPEMOD_ARRAY = 0x00001000,
        DEVPROP_TYPEMOD_LIST = 0x00002000,
        DEVPROP_TYPE_EMPTY = 0x00000000,
        DEVPROP_TYPE_NULL = 0x00000001,
        DEVPROP_TYPE_SBYTE = 0x00000002,
        DEVPROP_TYPE_BYTE = 0x00000003,
        DEVPROP_TYPE_INT16 = 0x00000004,
        DEVPROP_TYPE_UINT16 = 0x00000005,
        DEVPROP_TYPE_INT32 = 0x00000006,
        DEVPROP_TYPE_UINT32 = 0x00000007,
        DEVPROP_TYPE_INT64 = 0x00000008,
        DEVPROP_TYPE_UINT64 = 0x00000009,
        DEVPROP_TYPE_FLOAT = 0x0000000A,
        DEVPROP_TYPE_DOUBLE = 0x0000000B,
        DEVPROP_TYPE_DECIMAL = 0x0000000C,
        DEVPROP_TYPE_GUID = 0x0000000D,
        DEVPROP_TYPE_CURRENCY = 0x0000000E,
        DEVPROP_TYPE_DATE = 0x0000000F,
        DEVPROP_TYPE_FILETIME = 0x00000010,
        DEVPROP_TYPE_BOOLEAN = 0x00000011,
        DEVPROP_TYPE_STRING = 0x00000012,
        DEVPROP_TYPE_STRING_LIST = DEVPROP_TYPE_STRING | DEVPROP_TYPEMOD_LIST,
        DEVPROP_TYPE_SECURITY_DESCRIPTOR = 0x00000013,
        DEVPROP_TYPE_SECURITY_DESCRIPTOR_STRING = 0x00000014,
        DEVPROP_TYPE_DEVPROPKEY = 0x00000015,
        DEVPROP_TYPE_DEVPROPTYPE = 0x00000016,
        DEVPROP_TYPE_BINARY = DEVPROP_TYPE_BYTE | DEVPROP_TYPEMOD_ARRAY,
        DEVPROP_TYPE_ERROR = 0x00000017,
        DEVPROP_TYPE_NTSTATUS = 0x00000018,
        DEVPROP_TYPE_STRING_INDIRECT = 0x00000019,
        DEVPROP_MASK_TYPE = 0x00000FFF,
        DEVPROP_MASK_TYPEMOD = 0x0000F000,
    }

    public static class Devpkey {

        public static readonly DEVPROPKEY DEVPKEY_NAME                                                  = new DEVPROPKEY(0xb725f130, 0x47ef, 0x101a, 0xa5, 0xf1, 0x02, 0x60, 0x8c, 0x9e, 0xeb, 0xac, 10); // DEVPROP_TYPE_STRING

        //
        // Device properties
        // These DEVPKEYs correspond to the SetupAPI SPDRP_XXX device properties.
        //
        public static readonly DEVPROPKEY DEVPKEY_Device_DeviceDesc                                     = new DEVPROPKEY(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 2);   // DEVPROP_TYPE_STRING
        public static readonly DEVPROPKEY DEVPKEY_Device_HardwareIds                                    = new DEVPROPKEY(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 3);   // DEVPROP_TYPE_STRING_LIST
        public static readonly DEVPROPKEY DEVPKEY_Device_CompatibleIds                                  = new DEVPROPKEY(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 4);   // DEVPROP_TYPE_STRING_LIST
        public static readonly DEVPROPKEY DEVPKEY_Device_Service                                        = new DEVPROPKEY(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 6);   // DEVPROP_TYPE_STRING
        public static readonly DEVPROPKEY DEVPKEY_Device_Class                                          = new DEVPROPKEY(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 9);   // DEVPROP_TYPE_STRING
        public static readonly DEVPROPKEY DEVPKEY_Device_ClassGuid                                      = new DEVPROPKEY(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 10);  // DEVPROP_TYPE_GUID
        public static readonly DEVPROPKEY DEVPKEY_Device_Driver                                         = new DEVPROPKEY(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 11);  // DEVPROP_TYPE_STRING
        public static readonly DEVPROPKEY DEVPKEY_Device_ConfigFlags                                    = new DEVPROPKEY(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 12);  // DEVPROP_TYPE_UINT32
        public static readonly DEVPROPKEY DEVPKEY_Device_Manufacturer                                   = new DEVPROPKEY(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 13);  // DEVPROP_TYPE_STRING
        public static readonly DEVPROPKEY DEVPKEY_Device_FriendlyName                                   = new DEVPROPKEY(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 14);  // DEVPROP_TYPE_STRING
        public static readonly DEVPROPKEY DEVPKEY_Device_LocationInfo                                   = new DEVPROPKEY(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 15);  // DEVPROP_TYPE_STRING
        public static readonly DEVPROPKEY DEVPKEY_Device_PDOName                                        = new DEVPROPKEY(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 16);  // DEVPROP_TYPE_STRING
        public static readonly DEVPROPKEY DEVPKEY_Device_Capabilities                                   = new DEVPROPKEY(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 17);  // DEVPROP_TYPE_UINT32
        public static readonly DEVPROPKEY DEVPKEY_Device_UINumber                                       = new DEVPROPKEY(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 18);  // DEVPROP_TYPE_UINT32
        public static readonly DEVPROPKEY DEVPKEY_Device_UpperFilters                                   = new DEVPROPKEY(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 19);  // DEVPROP_TYPE_STRING_LIST
        public static readonly DEVPROPKEY DEVPKEY_Device_LowerFilters                                   = new DEVPROPKEY(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 20);  // DEVPROP_TYPE_STRING_LIST
        public static readonly DEVPROPKEY DEVPKEY_Device_BusTypeGuid                                    = new DEVPROPKEY(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 21);  // DEVPROP_TYPE_GUID
        public static readonly DEVPROPKEY DEVPKEY_Device_LegacyBusType                                  = new DEVPROPKEY(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 22);  // DEVPROP_TYPE_UINT32
        public static readonly DEVPROPKEY DEVPKEY_Device_BusNumber                                      = new DEVPROPKEY(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 23);  // DEVPROP_TYPE_UINT32
        public static readonly DEVPROPKEY DEVPKEY_Device_EnumeratorName                                 = new DEVPROPKEY(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 24);  // DEVPROP_TYPE_STRING
        public static readonly DEVPROPKEY DEVPKEY_Device_Security                                       = new DEVPROPKEY(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 25);  // DEVPROP_TYPE_SECURITY_DESCRIPTOR
        public static readonly DEVPROPKEY DEVPKEY_Device_SecuritySDS                                    = new DEVPROPKEY(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 26);  // DEVPROP_TYPE_SECURITY_DESCRIPTOR_STRING
        public static readonly DEVPROPKEY DEVPKEY_Device_DevType                                        = new DEVPROPKEY(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 27);  // DEVPROP_TYPE_UINT32
        public static readonly DEVPROPKEY DEVPKEY_Device_Exclusive                                      = new DEVPROPKEY(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 28);  // DEVPROP_TYPE_BOOLEAN
        public static readonly DEVPROPKEY DEVPKEY_Device_Characteristics                                = new DEVPROPKEY(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 29);  // DEVPROP_TYPE_UINT32
        public static readonly DEVPROPKEY DEVPKEY_Device_Address                                        = new DEVPROPKEY(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 30);  // DEVPROP_TYPE_UINT32
        public static readonly DEVPROPKEY DEVPKEY_Device_UINumberDescFormat                             = new DEVPROPKEY(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 31);  // DEVPROP_TYPE_STRING
        public static readonly DEVPROPKEY DEVPKEY_Device_PowerData                                      = new DEVPROPKEY(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 32);  // DEVPROP_TYPE_BINARY
        public static readonly DEVPROPKEY DEVPKEY_Device_RemovalPolicy                                  = new DEVPROPKEY(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 33);  // DEVPROP_TYPE_UINT32
        public static readonly DEVPROPKEY DEVPKEY_Device_RemovalPolicyDefault                           = new DEVPROPKEY(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 34);  // DEVPROP_TYPE_UINT32
        public static readonly DEVPROPKEY DEVPKEY_Device_RemovalPolicyOverride                          = new DEVPROPKEY(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 35);  // DEVPROP_TYPE_UINT32
        public static readonly DEVPROPKEY DEVPKEY_Device_InstallState                                   = new DEVPROPKEY(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 36);  // DEVPROP_TYPE_UINT32
        public static readonly DEVPROPKEY DEVPKEY_Device_LocationPaths                                  = new DEVPROPKEY(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 37);  // DEVPROP_TYPE_STRING_LIST
        public static readonly DEVPROPKEY DEVPKEY_Device_BaseContainerId                                = new DEVPROPKEY(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 38);  // DEVPROP_TYPE_GUID

        //
        // Device and Device Interface property
        // Common DEVPKEY used to retrieve the device instance id associated with devices and device interfaces.
        //
        public static readonly DEVPROPKEY DEVPKEY_Device_InstanceId                                     = new DEVPROPKEY(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57, 256); // DEVPROP_TYPE_STRING

        //
        // Device properties
        // These DEVPKEYs correspond to a device's status and problem code.
        //
        public static readonly DEVPROPKEY DEVPKEY_Device_DevNodeStatus                                  = new DEVPROPKEY(0x4340a6c5, 0x93fa, 0x4706, 0x97, 0x2c, 0x7b, 0x64, 0x80, 0x08, 0xa5, 0xa7, 2);  // DEVPROP_TYPE_UINT32
        public static readonly DEVPROPKEY DEVPKEY_Device_ProblemCode                                    = new DEVPROPKEY(0x4340a6c5, 0x93fa, 0x4706, 0x97, 0x2c, 0x7b, 0x64, 0x80, 0x08, 0xa5, 0xa7, 3);  // DEVPROP_TYPE_UINT32

        //
        // Device properties
        // These DEVPKEYs correspond to a device's relations.
        //
        public static readonly DEVPROPKEY DEVPKEY_Device_EjectionRelations                              = new DEVPROPKEY(0x4340a6c5, 0x93fa, 0x4706, 0x97, 0x2c, 0x7b, 0x64, 0x80, 0x08, 0xa5, 0xa7, 4);  // DEVPROP_TYPE_STRING_LIST
        public static readonly DEVPROPKEY DEVPKEY_Device_RemovalRelations                               = new DEVPROPKEY(0x4340a6c5, 0x93fa, 0x4706, 0x97, 0x2c, 0x7b, 0x64, 0x80, 0x08, 0xa5, 0xa7, 5);  // DEVPROP_TYPE_STRING_LIST
        public static readonly DEVPROPKEY DEVPKEY_Device_PowerRelations                                 = new DEVPROPKEY(0x4340a6c5, 0x93fa, 0x4706, 0x97, 0x2c, 0x7b, 0x64, 0x80, 0x08, 0xa5, 0xa7, 6);  // DEVPROP_TYPE_STRING_LIST
        public static readonly DEVPROPKEY DEVPKEY_Device_BusRelations                                   = new DEVPROPKEY(0x4340a6c5, 0x93fa, 0x4706, 0x97, 0x2c, 0x7b, 0x64, 0x80, 0x08, 0xa5, 0xa7, 7);  // DEVPROP_TYPE_STRING_LIST
        public static readonly DEVPROPKEY DEVPKEY_Device_Parent                                         = new DEVPROPKEY(0x4340a6c5, 0x93fa, 0x4706, 0x97, 0x2c, 0x7b, 0x64, 0x80, 0x08, 0xa5, 0xa7, 8);  // DEVPROP_TYPE_STRING
        public static readonly DEVPROPKEY DEVPKEY_Device_Children                                       = new DEVPROPKEY(0x4340a6c5, 0x93fa, 0x4706, 0x97, 0x2c, 0x7b, 0x64, 0x80, 0x08, 0xa5, 0xa7, 9);  // DEVPROP_TYPE_STRING_LIST
        public static readonly DEVPROPKEY DEVPKEY_Device_Siblings                                       = new DEVPROPKEY(0x4340a6c5, 0x93fa, 0x4706, 0x97, 0x2c, 0x7b, 0x64, 0x80, 0x08, 0xa5, 0xa7, 10); // DEVPROP_TYPE_STRING_LIST
        public static readonly DEVPROPKEY DEVPKEY_Device_TransportRelations                             = new DEVPROPKEY(0x4340a6c5, 0x93fa, 0x4706, 0x97, 0x2c, 0x7b, 0x64, 0x80, 0x08, 0xa5, 0xa7, 11); // DEVPROP_TYPE_STRING_LIST

        //
        // Device property
        // This DEVPKEY corresponds to a the status code that resulted in a device to be in a problem state.
        //
        public static readonly DEVPROPKEY DEVPKEY_Device_ProblemStatus                                  = new DEVPROPKEY(0x4340a6c5, 0x93fa, 0x4706, 0x97, 0x2c, 0x7b, 0x64, 0x80, 0x08, 0xa5, 0xa7, 12); // DEVPROP_TYPE_NTSTATUS

        //
        // Device properties
        // These DEVPKEYs are set for the corresponding types of root-enumerated devices.
        //
        public static readonly DEVPROPKEY DEVPKEY_Device_Reported                                       = new DEVPROPKEY(0x80497100, 0x8c73, 0x48b9, 0xaa, 0xd9, 0xce, 0x38, 0x7e, 0x19, 0xc5, 0x6e, 2);  // DEVPROP_TYPE_BOOLEAN
        public static readonly DEVPROPKEY DEVPKEY_Device_Legacy                                         = new DEVPROPKEY(0x80497100, 0x8c73, 0x48b9, 0xaa, 0xd9, 0xce, 0x38, 0x7e, 0x19, 0xc5, 0x6e, 3);  // DEVPROP_TYPE_BOOLEAN


        //
        // Device Container Id
        //
        public static readonly DEVPROPKEY DEVPKEY_Device_ContainerId                                    = new DEVPROPKEY(0x8c7ed206, 0x3f8a, 0x4827, 0xb3, 0xab, 0xae, 0x9e, 0x1f, 0xae, 0xfc, 0x6c, 2);   // DEVPROP_TYPE_GUID
        public static readonly DEVPROPKEY DEVPKEY_Device_InLocalMachineContainer                        = new DEVPROPKEY(0x8c7ed206, 0x3f8a, 0x4827, 0xb3, 0xab, 0xae, 0x9e, 0x1f, 0xae, 0xfc, 0x6c, 4);   // DEVPROP_TYPE_BOOLEAN

        //
        // Device property
        // This DEVPKEY correspond to a device's model.
        //
        public static readonly DEVPROPKEY DEVPKEY_Device_Model                                          = new DEVPROPKEY(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57, 39);   // DEVPROP_TYPE_STRING

        //
        // Device Experience related Keys
        //
        public static readonly DEVPROPKEY DEVPKEY_Device_ModelId                                        = new DEVPROPKEY(0x80d81ea6, 0x7473, 0x4b0c, 0x82, 0x16, 0xef, 0xc1, 0x1a, 0x2c, 0x4c, 0x8b, 2);    // DEVPROP_TYPE_GUID
        public static readonly DEVPROPKEY DEVPKEY_Device_FriendlyNameAttributes                         = new DEVPROPKEY(0x80d81ea6, 0x7473, 0x4b0c, 0x82, 0x16, 0xef, 0xc1, 0x1a, 0x2c, 0x4c, 0x8b, 3);    // DEVPROP_TYPE_UINT32
        public static readonly DEVPROPKEY DEVPKEY_Device_ManufacturerAttributes                         = new DEVPROPKEY(0x80d81ea6, 0x7473, 0x4b0c, 0x82, 0x16, 0xef, 0xc1, 0x1a, 0x2c, 0x4c, 0x8b, 4);    // DEVPROP_TYPE_UINT32
        public static readonly DEVPROPKEY DEVPKEY_Device_PresenceNotForDevice                           = new DEVPROPKEY(0x80d81ea6, 0x7473, 0x4b0c, 0x82, 0x16, 0xef, 0xc1, 0x1a, 0x2c, 0x4c, 0x8b, 5);    // DEVPROP_TYPE_BOOLEAN
        public static readonly DEVPROPKEY DEVPKEY_Device_SignalStrength                                 = new DEVPROPKEY(0x80d81ea6, 0x7473, 0x4b0c, 0x82, 0x16, 0xef, 0xc1, 0x1a, 0x2c, 0x4c, 0x8b, 6);    // DEVPROP_TYPE_INT32
        public static readonly DEVPROPKEY DEVPKEY_Device_IsAssociateableByUserAction                    = new DEVPROPKEY(0x80d81ea6, 0x7473, 0x4b0c, 0x82, 0x16, 0xef, 0xc1, 0x1a, 0x2c, 0x4c, 0x8b, 7);    // DEVPROP_TYPE_BOOLEAN
        public static readonly DEVPROPKEY DEVPKEY_Device_ShowInUninstallUI                              = new DEVPROPKEY(0x80d81ea6, 0x7473, 0x4b0c, 0x82, 0x16, 0xef, 0xc1, 0x1a, 0x2c, 0x4c, 0x8b, 8);    // DEVPROP_TYPE_BOOLEAN

        //
        // Other Device properties
        //
// #define DEVPKEY_Numa_Proximity_Domain  DEVPKEY_Device_Numa_Proximity_Domain
        public static readonly DEVPROPKEY DEVPKEY_Numa_Proximity_Domain                                 = new DEVPROPKEY(0x540b947e, 0x8b40, 0x45bc, 0xa8, 0xa2, 0x6a, 0x0b, 0x89, 0x4c, 0xbd, 0xa2, 1);    // DEVPROP_TYPE_UINT32
        public static readonly DEVPROPKEY DEVPKEY_Device_Numa_Proximity_Domain                          = new DEVPROPKEY(0x540b947e, 0x8b40, 0x45bc, 0xa8, 0xa2, 0x6a, 0x0b, 0x89, 0x4c, 0xbd, 0xa2, 1);    // DEVPROP_TYPE_UINT32
        public static readonly DEVPROPKEY DEVPKEY_Device_DHP_Rebalance_Policy                           = new DEVPROPKEY(0x540b947e, 0x8b40, 0x45bc, 0xa8, 0xa2, 0x6a, 0x0b, 0x89, 0x4c, 0xbd, 0xa2, 2);    // DEVPROP_TYPE_UINT32
        public static readonly DEVPROPKEY DEVPKEY_Device_Numa_Node                                      = new DEVPROPKEY(0x540b947e, 0x8b40, 0x45bc, 0xa8, 0xa2, 0x6a, 0x0b, 0x89, 0x4c, 0xbd, 0xa2, 3);    // DEVPROP_TYPE_UINT32
        public static readonly DEVPROPKEY DEVPKEY_Device_BusReportedDeviceDesc                          = new DEVPROPKEY(0x540b947e, 0x8b40, 0x45bc, 0xa8, 0xa2, 0x6a, 0x0b, 0x89, 0x4c, 0xbd, 0xa2, 4);    // DEVPROP_TYPE_STRING
        public static readonly DEVPROPKEY DEVPKEY_Device_IsPresent                                      = new DEVPROPKEY(0x540b947e, 0x8b40, 0x45bc, 0xa8, 0xa2, 0x6a, 0x0b, 0x89, 0x4c, 0xbd, 0xa2, 5);    // DEVPROP_TYPE_BOOL
        public static readonly DEVPROPKEY DEVPKEY_Device_HasProblem                                     = new DEVPROPKEY(0x540b947e, 0x8b40, 0x45bc, 0xa8, 0xa2, 0x6a, 0x0b, 0x89, 0x4c, 0xbd, 0xa2, 6);    // DEVPROP_TYPE_BOOL
        public static readonly DEVPROPKEY DEVPKEY_Device_ConfigurationId                                = new DEVPROPKEY(0x540b947e, 0x8b40, 0x45bc, 0xa8, 0xa2, 0x6a, 0x0b, 0x89, 0x4c, 0xbd, 0xa2, 7);    // DEVPROP_TYPE_STRING
        public static readonly DEVPROPKEY DEVPKEY_Device_ReportedDeviceIdsHash                          = new DEVPROPKEY(0x540b947e, 0x8b40, 0x45bc, 0xa8, 0xa2, 0x6a, 0x0b, 0x89, 0x4c, 0xbd, 0xa2, 8);    // DEVPROP_TYPE_UINT32
        public static readonly DEVPROPKEY DEVPKEY_Device_PhysicalDeviceLocation                         = new DEVPROPKEY(0x540b947e, 0x8b40, 0x45bc, 0xa8, 0xa2, 0x6a, 0x0b, 0x89, 0x4c, 0xbd, 0xa2, 9);    // DEVPROP_TYPE_BINARY
        public static readonly DEVPROPKEY DEVPKEY_Device_BiosDeviceName                                 = new DEVPROPKEY(0x540b947e, 0x8b40, 0x45bc, 0xa8, 0xa2, 0x6a, 0x0b, 0x89, 0x4c, 0xbd, 0xa2, 10);   // DEVPROP_TYPE_STRING

        public static readonly DEVPROPKEY DEVPKEY_Device_DriverProblemDesc                              = new DEVPROPKEY(0x540b947e, 0x8b40, 0x45bc, 0xa8, 0xa2, 0x6a, 0x0b, 0x89, 0x4c, 0xbd, 0xa2, 11);   // DEVPROP_TYPE_STRING
        public static readonly DEVPROPKEY DEVPKEY_Device_DebuggerSafe                                   = new DEVPROPKEY(0x540b947e, 0x8b40, 0x45bc, 0xa8, 0xa2, 0x6a, 0x0b, 0x89, 0x4c, 0xbd, 0xa2, 12);   // DEVPROP_TYPE_UINT32
        public static readonly DEVPROPKEY DEVPKEY_Device_PostInstallInProgress                          = new DEVPROPKEY(0x540b947e, 0x8b40, 0x45bc, 0xa8, 0xa2, 0x6a, 0x0b, 0x89, 0x4c, 0xbd, 0xa2, 13);   // DEVPROP_TYPE_BOOLEAN
        public static readonly DEVPROPKEY DEVPKEY_Device_Stack                                          = new DEVPROPKEY(0x540b947e, 0x8b40, 0x45bc, 0xa8, 0xa2, 0x6a, 0x0b, 0x89, 0x4c, 0xbd, 0xa2, 14);   // DEVPROP_TYPE_STRING_LIST
        public static readonly DEVPROPKEY DEVPKEY_Device_ExtendedConfigurationIds                       = new DEVPROPKEY(0x540b947e, 0x8b40, 0x45bc, 0xa8, 0xa2, 0x6a, 0x0b, 0x89, 0x4c, 0xbd, 0xa2, 15);   // DEVPROP_TYPE_STRING_LIST
        public static readonly DEVPROPKEY DEVPKEY_Device_IsRebootRequired                               = new DEVPROPKEY(0x540b947e, 0x8b40, 0x45bc, 0xa8, 0xa2, 0x6a, 0x0b, 0x89, 0x4c, 0xbd, 0xa2, 16);   // DEVPROP_TYPE_BOOLEAN
        public static readonly DEVPROPKEY DEVPKEY_Device_FirmwareDate                                   = new DEVPROPKEY(0x540b947e, 0x8b40, 0x45bc, 0xa8, 0xa2, 0x6a, 0x0b, 0x89, 0x4c, 0xbd, 0xa2, 17);   // DEVPROP_TYPE_FILETIME
        public static readonly DEVPROPKEY DEVPKEY_Device_FirmwareVersion                                = new DEVPROPKEY(0x540b947e, 0x8b40, 0x45bc, 0xa8, 0xa2, 0x6a, 0x0b, 0x89, 0x4c, 0xbd, 0xa2, 18);   // DEVPROP_TYPE_STRING
        public static readonly DEVPROPKEY DEVPKEY_Device_FirmwareRevision                               = new DEVPROPKEY(0x540b947e, 0x8b40, 0x45bc, 0xa8, 0xa2, 0x6a, 0x0b, 0x89, 0x4c, 0xbd, 0xa2, 19);   // DEVPROP_TYPE_STRING
        public static readonly DEVPROPKEY DEVPKEY_Device_DependencyProviders                            = new DEVPROPKEY(0x540b947e, 0x8b40, 0x45bc, 0xa8, 0xa2, 0x6a, 0x0b, 0x89, 0x4c, 0xbd, 0xa2, 20);   // DEVPROP_TYPE_STRING_LIST
        public static readonly DEVPROPKEY DEVPKEY_Device_DependencyDependents                           = new DEVPROPKEY(0x540b947e, 0x8b40, 0x45bc, 0xa8, 0xa2, 0x6a, 0x0b, 0x89, 0x4c, 0xbd, 0xa2, 21);   // DEVPROP_TYPE_STRING_LIST
        public static readonly DEVPROPKEY DEVPKEY_Device_SoftRestartSupported                           = new DEVPROPKEY(0x540b947e, 0x8b40, 0x45bc, 0xa8, 0xa2, 0x6a, 0x0b, 0x89, 0x4c, 0xbd, 0xa2, 22);   // DEVPROP_TYPE_BOOLEAN
        public static readonly DEVPROPKEY DEVPKEY_Device_ExtendedAddress                                = new DEVPROPKEY(0x540b947e, 0x8b40, 0x45bc, 0xa8, 0xa2, 0x6a, 0x0b, 0x89, 0x4c, 0xbd, 0xa2, 23);   // DEVPROP_TYPE_UINT64

        public static readonly DEVPROPKEY DEVPKEY_Device_SessionId                                      = new DEVPROPKEY(0x83da6326, 0x97a6, 0x4088, 0x94, 0x53, 0xa1, 0x92, 0x3f, 0x57, 0x3b, 0x29, 6);    // DEVPROP_TYPE_UINT32

        //
        // Device activity timestamp properties
        //
        public static readonly DEVPROPKEY DEVPKEY_Device_InstallDate                                    = new DEVPROPKEY(0x83da6326, 0x97a6, 0x4088, 0x94, 0x53, 0xa1, 0x92, 0x3f, 0x57, 0x3b, 0x29, 100);  // DEVPROP_TYPE_FILETIME
        public static readonly DEVPROPKEY DEVPKEY_Device_FirstInstallDate                               = new DEVPROPKEY(0x83da6326, 0x97a6, 0x4088, 0x94, 0x53, 0xa1, 0x92, 0x3f, 0x57, 0x3b, 0x29, 101);  // DEVPROP_TYPE_FILETIME
        public static readonly DEVPROPKEY DEVPKEY_Device_LastArrivalDate                                = new DEVPROPKEY(0x83da6326, 0x97a6, 0x4088, 0x94, 0x53, 0xa1, 0x92, 0x3f, 0x57, 0x3b, 0x29, 102);  // DEVPROP_TYPE_FILETIME
        public static readonly DEVPROPKEY DEVPKEY_Device_LastRemovalDate                                = new DEVPROPKEY(0x83da6326, 0x97a6, 0x4088, 0x94, 0x53, 0xa1, 0x92, 0x3f, 0x57, 0x3b, 0x29, 103);  // DEVPROP_TYPE_FILETIME


        //
        // Device driver properties
        //
        public static readonly DEVPROPKEY DEVPKEY_Device_DriverDate                                     = new DEVPROPKEY(0xa8b865dd, 0x2e3d, 0x4094, 0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6,  2);     // DEVPROP_TYPE_FILETIME
        public static readonly DEVPROPKEY DEVPKEY_Device_DriverVersion                                  = new DEVPROPKEY(0xa8b865dd, 0x2e3d, 0x4094, 0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6,  3);     // DEVPROP_TYPE_STRING
        public static readonly DEVPROPKEY DEVPKEY_Device_DriverDesc                                     = new DEVPROPKEY(0xa8b865dd, 0x2e3d, 0x4094, 0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6,  4);     // DEVPROP_TYPE_STRING
        public static readonly DEVPROPKEY DEVPKEY_Device_DriverInfPath                                  = new DEVPROPKEY(0xa8b865dd, 0x2e3d, 0x4094, 0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6,  5);     // DEVPROP_TYPE_STRING
        public static readonly DEVPROPKEY DEVPKEY_Device_DriverInfSection                               = new DEVPROPKEY(0xa8b865dd, 0x2e3d, 0x4094, 0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6,  6);     // DEVPROP_TYPE_STRING
        public static readonly DEVPROPKEY DEVPKEY_Device_DriverInfSectionExt                            = new DEVPROPKEY(0xa8b865dd, 0x2e3d, 0x4094, 0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6,  7);     // DEVPROP_TYPE_STRING
        public static readonly DEVPROPKEY DEVPKEY_Device_MatchingDeviceId                               = new DEVPROPKEY(0xa8b865dd, 0x2e3d, 0x4094, 0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6,  8);     // DEVPROP_TYPE_STRING
        public static readonly DEVPROPKEY DEVPKEY_Device_DriverProvider                                 = new DEVPROPKEY(0xa8b865dd, 0x2e3d, 0x4094, 0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6,  9);     // DEVPROP_TYPE_STRING
        public static readonly DEVPROPKEY DEVPKEY_Device_DriverPropPageProvider                         = new DEVPROPKEY(0xa8b865dd, 0x2e3d, 0x4094, 0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6, 10);     // DEVPROP_TYPE_STRING
        public static readonly DEVPROPKEY DEVPKEY_Device_DriverCoInstallers                             = new DEVPROPKEY(0xa8b865dd, 0x2e3d, 0x4094, 0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6, 11);     // DEVPROP_TYPE_STRING_LIST
        public static readonly DEVPROPKEY DEVPKEY_Device_ResourcePickerTags                             = new DEVPROPKEY(0xa8b865dd, 0x2e3d, 0x4094, 0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6, 12);     // DEVPROP_TYPE_STRING
        public static readonly DEVPROPKEY DEVPKEY_Device_ResourcePickerExceptions                       = new DEVPROPKEY(0xa8b865dd, 0x2e3d, 0x4094, 0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6, 13);     // DEVPROP_TYPE_STRING
        public static readonly DEVPROPKEY DEVPKEY_Device_DriverRank                                     = new DEVPROPKEY(0xa8b865dd, 0x2e3d, 0x4094, 0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6, 14);     // DEVPROP_TYPE_UINT32
        public static readonly DEVPROPKEY DEVPKEY_Device_DriverLogoLevel                                = new DEVPROPKEY(0xa8b865dd, 0x2e3d, 0x4094, 0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6, 15);     // DEVPROP_TYPE_UINT32

        //
        // Device properties
        // These DEVPKEYs may be set by the driver package installed for a device.
        //
        public static readonly DEVPROPKEY DEVPKEY_Device_NoConnectSound                                 = new DEVPROPKEY(0xa8b865dd, 0x2e3d, 0x4094, 0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6, 17);     // DEVPROP_TYPE_BOOLEAN
        public static readonly DEVPROPKEY DEVPKEY_Device_GenericDriverInstalled                         = new DEVPROPKEY(0xa8b865dd, 0x2e3d, 0x4094, 0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6, 18);     // DEVPROP_TYPE_BOOLEAN
        public static readonly DEVPROPKEY DEVPKEY_Device_AdditionalSoftwareRequested                    = new DEVPROPKEY(0xa8b865dd, 0x2e3d, 0x4094, 0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6, 19);     // DEVPROP_TYPE_BOOLEAN

        //
        // Device safe-removal properties
        //
        public static readonly DEVPROPKEY DEVPKEY_Device_SafeRemovalRequired                            = new DEVPROPKEY(0xafd97640, 0x86a3, 0x4210, 0xb6, 0x7c, 0x28, 0x9c, 0x41, 0xaa, 0xbe, 0x55, 2);     // DEVPROP_TYPE_BOOLEAN
        public static readonly DEVPROPKEY DEVPKEY_Device_SafeRemovalRequiredOverride                    = new DEVPROPKEY(0xafd97640, 0x86a3, 0x4210, 0xb6, 0x7c, 0x28, 0x9c, 0x41, 0xaa, 0xbe, 0x55, 3);     // DEVPROP_TYPE_BOOLEAN

        //
        // Device properties
        // These DEVPKEYs may be set by the driver package installed for a device.
        //
        public static readonly DEVPROPKEY DEVPKEY_DrvPkg_Model                                          = new DEVPROPKEY(0xcf73bb51, 0x3abf, 0x44a2, 0x85, 0xe0, 0x9a, 0x3d, 0xc7, 0xa1, 0x21, 0x32, 2);     // DEVPROP_TYPE_STRING
        public static readonly DEVPROPKEY DEVPKEY_DrvPkg_VendorWebSite                                  = new DEVPROPKEY(0xcf73bb51, 0x3abf, 0x44a2, 0x85, 0xe0, 0x9a, 0x3d, 0xc7, 0xa1, 0x21, 0x32, 3);     // DEVPROP_TYPE_STRING
        public static readonly DEVPROPKEY DEVPKEY_DrvPkg_DetailedDescription                            = new DEVPROPKEY(0xcf73bb51, 0x3abf, 0x44a2, 0x85, 0xe0, 0x9a, 0x3d, 0xc7, 0xa1, 0x21, 0x32, 4);     // DEVPROP_TYPE_STRING
        public static readonly DEVPROPKEY DEVPKEY_DrvPkg_DocumentationLink                              = new DEVPROPKEY(0xcf73bb51, 0x3abf, 0x44a2, 0x85, 0xe0, 0x9a, 0x3d, 0xc7, 0xa1, 0x21, 0x32, 5);     // DEVPROP_TYPE_STRING
        public static readonly DEVPROPKEY DEVPKEY_DrvPkg_Icon                                           = new DEVPROPKEY(0xcf73bb51, 0x3abf, 0x44a2, 0x85, 0xe0, 0x9a, 0x3d, 0xc7, 0xa1, 0x21, 0x32, 6);     // DEVPROP_TYPE_STRING_LIST
        public static readonly DEVPROPKEY DEVPKEY_DrvPkg_BrandingIcon                                   = new DEVPROPKEY(0xcf73bb51, 0x3abf, 0x44a2, 0x85, 0xe0, 0x9a, 0x3d, 0xc7, 0xa1, 0x21, 0x32, 7);     // DEVPROP_TYPE_STRING_LIST


        //
        // Device setup class properties
        // These DEVPKEYs correspond to the SetupAPI SPCRP_XXX setup class properties.
        //
        public static readonly DEVPROPKEY DEVPKEY_DeviceClass_UpperFilters                              = new DEVPROPKEY(0x4321918b, 0xf69e, 0x470d, 0xa5, 0xde, 0x4d, 0x88, 0xc7, 0x5a, 0xd2, 0x4b, 19);    // DEVPROP_TYPE_STRING_LIST
        public static readonly DEVPROPKEY DEVPKEY_DeviceClass_LowerFilters                              = new DEVPROPKEY(0x4321918b, 0xf69e, 0x470d, 0xa5, 0xde, 0x4d, 0x88, 0xc7, 0x5a, 0xd2, 0x4b, 20);    // DEVPROP_TYPE_STRING_LIST
        public static readonly DEVPROPKEY DEVPKEY_DeviceClass_Security                                  = new DEVPROPKEY(0x4321918b, 0xf69e, 0x470d, 0xa5, 0xde, 0x4d, 0x88, 0xc7, 0x5a, 0xd2, 0x4b, 25);    // DEVPROP_TYPE_SECURITY_DESCRIPTOR
        public static readonly DEVPROPKEY DEVPKEY_DeviceClass_SecuritySDS                               = new DEVPROPKEY(0x4321918b, 0xf69e, 0x470d, 0xa5, 0xde, 0x4d, 0x88, 0xc7, 0x5a, 0xd2, 0x4b, 26);    // DEVPROP_TYPE_SECURITY_DESCRIPTOR_STRING
        public static readonly DEVPROPKEY DEVPKEY_DeviceClass_DevType                                   = new DEVPROPKEY(0x4321918b, 0xf69e, 0x470d, 0xa5, 0xde, 0x4d, 0x88, 0xc7, 0x5a, 0xd2, 0x4b, 27);    // DEVPROP_TYPE_UINT32
        public static readonly DEVPROPKEY DEVPKEY_DeviceClass_Exclusive                                 = new DEVPROPKEY(0x4321918b, 0xf69e, 0x470d, 0xa5, 0xde, 0x4d, 0x88, 0xc7, 0x5a, 0xd2, 0x4b, 28);    // DEVPROP_TYPE_BOOLEAN
        public static readonly DEVPROPKEY DEVPKEY_DeviceClass_Characteristics                           = new DEVPROPKEY(0x4321918b, 0xf69e, 0x470d, 0xa5, 0xde, 0x4d, 0x88, 0xc7, 0x5a, 0xd2, 0x4b, 29);    // DEVPROP_TYPE_UINT32

        //
        // Device setup class properties
        //
        public static readonly DEVPROPKEY DEVPKEY_DeviceClass_Name                                      = new DEVPROPKEY(0x259abffc, 0x50a7, 0x47ce, 0xaf, 0x8, 0x68, 0xc9, 0xa7, 0xd7, 0x33, 0x66, 2);      // DEVPROP_TYPE_STRING
        public static readonly DEVPROPKEY DEVPKEY_DeviceClass_ClassName                                 = new DEVPROPKEY(0x259abffc, 0x50a7, 0x47ce, 0xaf, 0x8, 0x68, 0xc9, 0xa7, 0xd7, 0x33, 0x66, 3);      // DEVPROP_TYPE_STRING
        public static readonly DEVPROPKEY DEVPKEY_DeviceClass_Icon                                      = new DEVPROPKEY(0x259abffc, 0x50a7, 0x47ce, 0xaf, 0x8, 0x68, 0xc9, 0xa7, 0xd7, 0x33, 0x66, 4);      // DEVPROP_TYPE_STRING
        public static readonly DEVPROPKEY DEVPKEY_DeviceClass_ClassInstaller                            = new DEVPROPKEY(0x259abffc, 0x50a7, 0x47ce, 0xaf, 0x8, 0x68, 0xc9, 0xa7, 0xd7, 0x33, 0x66, 5);      // DEVPROP_TYPE_STRING
        public static readonly DEVPROPKEY DEVPKEY_DeviceClass_PropPageProvider                          = new DEVPROPKEY(0x259abffc, 0x50a7, 0x47ce, 0xaf, 0x8, 0x68, 0xc9, 0xa7, 0xd7, 0x33, 0x66, 6);      // DEVPROP_TYPE_STRING
        public static readonly DEVPROPKEY DEVPKEY_DeviceClass_NoInstallClass                            = new DEVPROPKEY(0x259abffc, 0x50a7, 0x47ce, 0xaf, 0x8, 0x68, 0xc9, 0xa7, 0xd7, 0x33, 0x66, 7);      // DEVPROP_TYPE_BOOLEAN
        public static readonly DEVPROPKEY DEVPKEY_DeviceClass_NoDisplayClass                            = new DEVPROPKEY(0x259abffc, 0x50a7, 0x47ce, 0xaf, 0x8, 0x68, 0xc9, 0xa7, 0xd7, 0x33, 0x66, 8);      // DEVPROP_TYPE_BOOLEAN
        public static readonly DEVPROPKEY DEVPKEY_DeviceClass_SilentInstall                             = new DEVPROPKEY(0x259abffc, 0x50a7, 0x47ce, 0xaf, 0x8, 0x68, 0xc9, 0xa7, 0xd7, 0x33, 0x66, 9);      // DEVPROP_TYPE_BOOLEAN
        public static readonly DEVPROPKEY DEVPKEY_DeviceClass_NoUseClass                                = new DEVPROPKEY(0x259abffc, 0x50a7, 0x47ce, 0xaf, 0x8, 0x68, 0xc9, 0xa7, 0xd7, 0x33, 0x66, 10);     // DEVPROP_TYPE_BOOLEAN
        public static readonly DEVPROPKEY DEVPKEY_DeviceClass_DefaultService                            = new DEVPROPKEY(0x259abffc, 0x50a7, 0x47ce, 0xaf, 0x8, 0x68, 0xc9, 0xa7, 0xd7, 0x33, 0x66, 11);     // DEVPROP_TYPE_STRING
        public static readonly DEVPROPKEY DEVPKEY_DeviceClass_IconPath                                  = new DEVPROPKEY(0x259abffc, 0x50a7, 0x47ce, 0xaf, 0x8, 0x68, 0xc9, 0xa7, 0xd7, 0x33, 0x66, 12);     // DEVPROP_TYPE_STRING_LIST
        //
        // Other Device setup class properties
        //
        public static readonly DEVPROPKEY DEVPKEY_DeviceClass_DHPRebalanceOptOut                        = new DEVPROPKEY(0xd14d3ef3, 0x66cf, 0x4ba2, 0x9d, 0x38, 0x0d, 0xdb, 0x37, 0xab, 0x47, 0x01, 2);     // DEVPROP_TYPE_BOOLEAN

        public static readonly DEVPROPKEY DEVPKEY_DeviceClass_ClassCoInstallers                         = new DEVPROPKEY(0x713d1703, 0xa2e2, 0x49f5, 0x92, 0x14, 0x56, 0x47, 0x2e, 0xf3, 0xda, 0x5c, 2);     // DEVPROP_TYPE_STRING_LIST

        //
        // Device interface properties
        //
        public static readonly DEVPROPKEY DEVPKEY_DeviceInterface_FriendlyName                          = new DEVPROPKEY(0x026e516e, 0xb814, 0x414b, 0x83, 0xcd, 0x85, 0x6d, 0x6f, 0xef, 0x48, 0x22, 2);     // DEVPROP_TYPE_STRING
        public static readonly DEVPROPKEY DEVPKEY_DeviceInterface_Enabled                               = new DEVPROPKEY(0x026e516e, 0xb814, 0x414b, 0x83, 0xcd, 0x85, 0x6d, 0x6f, 0xef, 0x48, 0x22, 3);     // DEVPROP_TYPE_BOOLEAN
        public static readonly DEVPROPKEY DEVPKEY_DeviceInterface_ClassGuid                             = new DEVPROPKEY(0x026e516e, 0xb814, 0x414b, 0x83, 0xcd, 0x85, 0x6d, 0x6f, 0xef, 0x48, 0x22, 4);     // DEVPROP_TYPE_GUID
        public static readonly DEVPROPKEY DEVPKEY_DeviceInterface_ReferenceString                       = new DEVPROPKEY(0x026e516e, 0xb814, 0x414b, 0x83, 0xcd, 0x85, 0x6d, 0x6f, 0xef, 0x48, 0x22, 5);     // DEVPROP_TYPE_STRING
        public static readonly DEVPROPKEY DEVPKEY_DeviceInterface_Restricted                            = new DEVPROPKEY(0x026e516e, 0xb814, 0x414b, 0x83, 0xcd, 0x85, 0x6d, 0x6f, 0xef, 0x48, 0x22, 6);     // DEVPROP_TYPE_BOOLEAN
        public static readonly DEVPROPKEY DEVPKEY_DeviceInterface_UnrestrictedAppCapabilities           = new DEVPROPKEY(0x026e516e, 0xb814, 0x414b, 0x83, 0xcd, 0x85, 0x6d, 0x6f, 0xef, 0x48, 0x22, 8);     // DEVPROP_TYPE_STRING_LIST
        public static readonly DEVPROPKEY DEVPKEY_DeviceInterface_SchematicName                         = new DEVPROPKEY(0x026e516e, 0xb814, 0x414b, 0x83, 0xcd, 0x85, 0x6d, 0x6f, 0xef, 0x48, 0x22, 9);     // DEVPROP_TYPE_STRING

        //
        // Device interface class properties
        //
        public static readonly DEVPROPKEY DEVPKEY_DeviceInterfaceClass_DefaultInterface                 = new DEVPROPKEY(0x14c83a99, 0x0b3f, 0x44b7, 0xbe, 0x4c, 0xa1, 0x78, 0xd3, 0x99, 0x05, 0x64, 2);     // DEVPROP_TYPE_STRING
        public static readonly DEVPROPKEY DEVPKEY_DeviceInterfaceClass_Name                             = new DEVPROPKEY(0x14c83a99, 0x0b3f, 0x44b7, 0xbe, 0x4c, 0xa1, 0x78, 0xd3, 0x99, 0x05, 0x64, 3);     // DEVPROP_TYPE_STRING

        //
        // Device Container Properties
        //
        public static readonly DEVPROPKEY DEVPKEY_DeviceContainer_Address                               = new DEVPROPKEY(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57, 51);    // DEVPROP_TYPE_STRING | DEVPROP_TYPE_STRING_LIST
        public static readonly DEVPROPKEY DEVPKEY_DeviceContainer_DiscoveryMethod                       = new DEVPROPKEY(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57, 52);    // DEVPROP_TYPE_STRING_LIST
        public static readonly DEVPROPKEY DEVPKEY_DeviceContainer_IsEncrypted                           = new DEVPROPKEY(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57, 53);    // DEVPROP_TYPE_BOOLEAN
        public static readonly DEVPROPKEY DEVPKEY_DeviceContainer_IsAuthenticated                       = new DEVPROPKEY(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57, 54);    // DEVPROP_TYPE_BOOLEAN
        public static readonly DEVPROPKEY DEVPKEY_DeviceContainer_IsConnected                           = new DEVPROPKEY(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57, 55);    // DEVPROP_TYPE_BOOLEAN
        public static readonly DEVPROPKEY DEVPKEY_DeviceContainer_IsPaired                              = new DEVPROPKEY(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57, 56);    // DEVPROP_TYPE_BOOLEAN
        public static readonly DEVPROPKEY DEVPKEY_DeviceContainer_Icon                                  = new DEVPROPKEY(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57, 57);    // DEVPROP_TYPE_STRING
        public static readonly DEVPROPKEY DEVPKEY_DeviceContainer_Version                               = new DEVPROPKEY(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57, 65);    // DEVPROP_TYPE_STRING
        public static readonly DEVPROPKEY DEVPKEY_DeviceContainer_Last_Seen                             = new DEVPROPKEY(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57, 66);    // DEVPROP_TYPE_FILETIME
        public static readonly DEVPROPKEY DEVPKEY_DeviceContainer_Last_Connected                        = new DEVPROPKEY(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57, 67);    // DEVPROP_TYPE_FILETIME
        public static readonly DEVPROPKEY DEVPKEY_DeviceContainer_IsShowInDisconnectedState             = new DEVPROPKEY(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57, 68);    // DEVPROP_TYPE_BOOLEAN
        public static readonly DEVPROPKEY DEVPKEY_DeviceContainer_IsLocalMachine                        = new DEVPROPKEY(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57, 70);    // DEVPROP_TYPE_BOOLEAN
        public static readonly DEVPROPKEY DEVPKEY_DeviceContainer_MetadataPath                          = new DEVPROPKEY(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57, 71);    // DEVPROP_TYPE_STRING
        public static readonly DEVPROPKEY DEVPKEY_DeviceContainer_IsMetadataSearchInProgress            = new DEVPROPKEY(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57, 72);    // DEVPROP_TYPE_BOOLEAN
        public static readonly DEVPROPKEY DEVPKEY_DeviceContainer_MetadataChecksum                      = new DEVPROPKEY(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57, 73);    // DEVPROP_TYPE_BINARY
        public static readonly DEVPROPKEY DEVPKEY_DeviceContainer_IsNotInterestingForDisplay            = new DEVPROPKEY(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57, 74);    // DEVPROP_TYPE_BOOLEAN
        public static readonly DEVPROPKEY DEVPKEY_DeviceContainer_LaunchDeviceStageOnDeviceConnect      = new DEVPROPKEY(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57, 76);    // DEVPROP_TYPE_BOOLEAN
        public static readonly DEVPROPKEY DEVPKEY_DeviceContainer_LaunchDeviceStageFromExplorer         = new DEVPROPKEY(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57, 77);    // DEVPROP_TYPE_BOOLEAN
        public static readonly DEVPROPKEY DEVPKEY_DeviceContainer_BaselineExperienceId                  = new DEVPROPKEY(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57, 78);    // DEVPROP_TYPE_GUID
        public static readonly DEVPROPKEY DEVPKEY_DeviceContainer_IsDeviceUniquelyIdentifiable          = new DEVPROPKEY(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57, 79);    // DEVPROP_TYPE_BOOLEAN
        public static readonly DEVPROPKEY DEVPKEY_DeviceContainer_AssociationArray                      = new DEVPROPKEY(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57, 80);    // DEVPROP_TYPE_STRING_LIST
        public static readonly DEVPROPKEY DEVPKEY_DeviceContainer_DeviceDescription1                    = new DEVPROPKEY(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57, 81);    // DEVPROP_TYPE_STRING
        public static readonly DEVPROPKEY DEVPKEY_DeviceContainer_DeviceDescription2                    = new DEVPROPKEY(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57, 82);    // DEVPROP_TYPE_STRING
        public static readonly DEVPROPKEY DEVPKEY_DeviceContainer_HasProblem                            = new DEVPROPKEY(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57, 83);    // DEVPROP_TYPE_BOOLEAN
        public static readonly DEVPROPKEY DEVPKEY_DeviceContainer_IsSharedDevice                        = new DEVPROPKEY(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57, 84);    // DEVPROP_TYPE_BOOLEAN
        public static readonly DEVPROPKEY DEVPKEY_DeviceContainer_IsNetworkDevice                       = new DEVPROPKEY(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57, 85);    // DEVPROP_TYPE_BOOLEAN
        public static readonly DEVPROPKEY DEVPKEY_DeviceContainer_IsDefaultDevice                       = new DEVPROPKEY(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57, 86);    // DEVPROP_TYPE_BOOLEAN
        public static readonly DEVPROPKEY DEVPKEY_DeviceContainer_MetadataCabinet                       = new DEVPROPKEY(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57, 87);    // DEVPROP_TYPE_STRING
        public static readonly DEVPROPKEY DEVPKEY_DeviceContainer_RequiresPairingElevation              = new DEVPROPKEY(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57, 88);    // DEVPROP_TYPE_BOOLEAN
        public static readonly DEVPROPKEY DEVPKEY_DeviceContainer_ExperienceId                          = new DEVPROPKEY(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57, 89);    // DEVPROP_TYPE_GUID
        public static readonly DEVPROPKEY DEVPKEY_DeviceContainer_Category                              = new DEVPROPKEY(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57, 90);    // DEVPROP_TYPE_STRING_LIST
        public static readonly DEVPROPKEY DEVPKEY_DeviceContainer_Category_Desc_Singular                = new DEVPROPKEY(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57, 91);    // DEVPROP_TYPE_STRING_LIST
        public static readonly DEVPROPKEY DEVPKEY_DeviceContainer_Category_Desc_Plural                  = new DEVPROPKEY(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57, 92);    // DEVPROP_TYPE_STRING_LIST
        public static readonly DEVPROPKEY DEVPKEY_DeviceContainer_Category_Icon                         = new DEVPROPKEY(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57, 93);    // DEVPROP_TYPE_STRING
        public static readonly DEVPROPKEY DEVPKEY_DeviceContainer_CategoryGroup_Desc                    = new DEVPROPKEY(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57, 94);    // DEVPROP_TYPE_STRING_LIST
        public static readonly DEVPROPKEY DEVPKEY_DeviceContainer_CategoryGroup_Icon                    = new DEVPROPKEY(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57, 95);    // DEVPROP_TYPE_STRING
        public static readonly DEVPROPKEY DEVPKEY_DeviceContainer_PrimaryCategory                       = new DEVPROPKEY(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57, 97);    // DEVPROP_TYPE_STRING
        public static readonly DEVPROPKEY DEVPKEY_DeviceContainer_UnpairUninstall                       = new DEVPROPKEY(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57, 98);    // DEVPROP_TYPE_BOOLEAN
        public static readonly DEVPROPKEY DEVPKEY_DeviceContainer_RequiresUninstallElevation            = new DEVPROPKEY(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57, 99);    // DEVPROP_TYPE_BOOLEAN
        public static readonly DEVPROPKEY DEVPKEY_DeviceContainer_DeviceFunctionSubRank                 = new DEVPROPKEY(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57, 100);   // DEVPROP_TYPE_UINT32
        public static readonly DEVPROPKEY DEVPKEY_DeviceContainer_AlwaysShowDeviceAsConnected           = new DEVPROPKEY(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57, 101);   // DEVPROP_TYPE_BOOLEAN
        public static readonly DEVPROPKEY DEVPKEY_DeviceContainer_ConfigFlags                           = new DEVPROPKEY(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57, 105);   // DEVPROP_TYPE_UINT32
        public static readonly DEVPROPKEY DEVPKEY_DeviceContainer_PrivilegedPackageFamilyNames          = new DEVPROPKEY(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57, 106);   // DEVPROP_TYPE_STRING_LIST
        public static readonly DEVPROPKEY DEVPKEY_DeviceContainer_CustomPrivilegedPackageFamilyNames    = new DEVPROPKEY(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57, 107);   // DEVPROP_TYPE_STRING_LIST
        public static readonly DEVPROPKEY DEVPKEY_DeviceContainer_IsRebootRequired                      = new DEVPROPKEY(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57, 108);   // DEVPROP_TYPE_BOOLEAN

        public static readonly DEVPROPKEY DEVPKEY_DeviceContainer_FriendlyName                          = new DEVPROPKEY(0x656A3BB3, 0xECC0, 0x43FD, 0x84, 0x77, 0x4A, 0xE0, 0x40, 0x4A, 0x96, 0xCD, 12288); // DEVPROP_TYPE_STRING
        public static readonly DEVPROPKEY DEVPKEY_DeviceContainer_Manufacturer                          = new DEVPROPKEY(0x656A3BB3, 0xECC0, 0x43FD, 0x84, 0x77, 0x4A, 0xE0, 0x40, 0x4A, 0x96, 0xCD, 8192);  // DEVPROP_TYPE_STRING
        public static readonly DEVPROPKEY DEVPKEY_DeviceContainer_ModelName                             = new DEVPROPKEY(0x656A3BB3, 0xECC0, 0x43FD, 0x84, 0x77, 0x4A, 0xE0, 0x40, 0x4A, 0x96, 0xCD, 8194);  // DEVPROP_TYPE_STRING (localizable)
        public static readonly DEVPROPKEY DEVPKEY_DeviceContainer_ModelNumber                           = new DEVPROPKEY(0x656A3BB3, 0xECC0, 0x43FD, 0x84, 0x77, 0x4A, 0xE0, 0x40, 0x4A, 0x96, 0xCD, 8195);  // DEVPROP_TYPE_STRING

        public static readonly DEVPROPKEY DEVPKEY_DeviceContainer_InstallInProgress                     = new DEVPROPKEY(0x83da6326, 0x97a6, 0x4088, 0x94, 0x53, 0xa1, 0x92, 0x3f, 0x57, 0x3b, 0x29, 9);     // DEVPROP_TYPE_BOOLEAN
    }
}
