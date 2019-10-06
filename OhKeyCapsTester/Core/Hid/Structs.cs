using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace OhKeyCapsTester.Core.Hid
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct LIST_ENTRY
    {
        public IntPtr Flink;
        public IntPtr Blink;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct DEVICE_LIST_NODE
    {
        public LIST_ENTRY Hdr;
        public IntPtr NotificationHandle;
        public HID_DEVICE HidDeviceInfo;
        public bool DeviceOpened;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct SP_DEVICE_INTERFACE_DATA
    {
        public Int32 cbSize;
        public Guid interfaceClassGuid;
        public Int32 flags;
        private UIntPtr reserved;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal struct SP_DEVICE_INTERFACE_DETAIL_DATA
    {
        private const int DEVICE_PATH = 260;
        public int cbSize;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = DEVICE_PATH)]
        public string DevicePath;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct SP_DEVINFO_DATA
    {
        public int cbSize;
        public Guid classGuid;
        public UInt32 devInst;
        public IntPtr reserved;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct HIDP_CAPS
    {
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 Usage;
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 UsagePage;
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 InputReportByteLength;
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 OutputReportByteLength;
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 FeatureReportByteLength;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
        public UInt16[] Reserved;
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 NumberLinkCollectionNodes;
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 NumberInputButtonCaps;
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 NumberInputValueCaps;
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 NumberInputDataIndices;
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 NumberOutputButtonCaps;
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 NumberOutputValueCaps;
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 NumberOutputDataIndices;
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 NumberFeatureButtonCaps;
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 NumberFeatureValueCaps;
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 NumberFeatureDataIndices;
    };

    [StructLayout(LayoutKind.Sequential)]
    internal struct HIDD_ATTRIBUTES
    {
        public Int32 Size;
        public Int16 VendorID;
        public Int16 ProductID;
        public Int16 VersionNumber;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ButtonData
    {
        public Int32 UsageMin;
        public Int32 UsageMax;
        public Int32 MaxUsageLength;
        public Int16 Usages;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ValueData
    {
        public ushort Usage;
        public ushort Reserved;

        public ulong Value;
        public long ScaledValue;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct HID_DATA
    {
        [FieldOffset(0)]
        public bool IsButtonData;
        [FieldOffset(1)]
        public byte Reserved;
        [FieldOffset(2)]
        public ushort UsagePage;
        [FieldOffset(4)]
        public Int32 Status;
        [FieldOffset(8)]
        public Int32 ReportID;
        [FieldOffset(16)]
        public bool IsDataSet;

        [FieldOffset(17)]
        public ButtonData ButtonData;
        [FieldOffset(17)]
        public ValueData ValueData;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct HIDP_Range
    {
        public ushort UsageMin, UsageMax;
        public ushort StringMin, StringMax;
        public ushort DesignatorMin, DesignatorMax;
        public ushort DataIndexMin, DataIndexMax;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct HIDP_NotRange
    {
        public ushort Usage, Reserved1;
        public ushort StringIndex, Reserved2;
        public ushort DesignatorIndex, Reserved3;
        public ushort DataIndex, Reserved4;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct HIDP_BUTTON_CAPS
    {
        [FieldOffset(0)]
        public ushort UsagePage;
        [FieldOffset(2)]
        public byte ReportID;
        [FieldOffset(3), MarshalAs(UnmanagedType.U1)]
        public bool IsAlias;
        [FieldOffset(4)]
        public short BitField;
        [FieldOffset(6)]
        public short LinkCollection;
        [FieldOffset(8)]
        public short LinkUsage;
        [FieldOffset(10)]
        public short LinkUsagePage;
        [FieldOffset(12), MarshalAs(UnmanagedType.U1)]
        public bool IsRange;
        [FieldOffset(13), MarshalAs(UnmanagedType.U1)]
        public bool IsStringRange;
        [FieldOffset(14), MarshalAs(UnmanagedType.U1)]
        public bool IsDesignatorRange;
        [FieldOffset(15), MarshalAs(UnmanagedType.U1)]
        public bool IsAbsolute;
        [FieldOffset(16), MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public int[] Reserved;

        [FieldOffset(56)]
        public HIDP_Range Range;
        [FieldOffset(56)]
        public HIDP_NotRange NotRange;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct HIDP_VALUE_CAPS
    {
        [FieldOffset(0)]
        public ushort UsagePage;
        [FieldOffset(2)]
        public byte ReportID;
        [FieldOffset(3), MarshalAs(UnmanagedType.U1)]
        public bool IsAlias;
        [FieldOffset(4)]
        public ushort BitField;
        [FieldOffset(6)]
        public ushort LinkCollection;
        [FieldOffset(8)]
        public ushort LinkUsage;
        [FieldOffset(10)]
        public ushort LinkUsagePage;
        [FieldOffset(12), MarshalAs(UnmanagedType.U1)]
        public bool IsRange;
        [FieldOffset(13), MarshalAs(UnmanagedType.U1)]
        public bool IsStringRange;
        [FieldOffset(14), MarshalAs(UnmanagedType.U1)]
        public bool IsDesignatorRange;
        [FieldOffset(15), MarshalAs(UnmanagedType.U1)]
        public bool IsAbsolute;
        [FieldOffset(16), MarshalAs(UnmanagedType.U1)]
        public bool HasNull;
        [FieldOffset(17)]
        public byte Reserved;
        [FieldOffset(18)]
        public short BitSize;
        [FieldOffset(20)]
        public short ReportCount;
        [FieldOffset(22)]
        public ushort Reserved2a;
        [FieldOffset(24)]
        public ushort Reserved2b;
        [FieldOffset(26)]
        public ushort Reserved2c;
        [FieldOffset(28)]
        public ushort Reserved2d;
        [FieldOffset(30)]
        public ushort Reserved2e;
        [FieldOffset(32)]
        public int UnitsExp;
        [FieldOffset(36)]
        public int Units;
        [FieldOffset(40)]
        public int LogicalMin;
        [FieldOffset(44)]
        public int LogicalMax;
        [FieldOffset(48)]
        public int PhysicalMin;
        [FieldOffset(52)]
        public int PhysicalMax;

        [FieldOffset(56)]
        public HIDP_Range Range;
        [FieldOffset(56)]
        public HIDP_NotRange NotRange;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal struct HID_DEVICE
    {
        private const int DEVICE_PATH = 260;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = DEVICE_PATH)]
        public string DevicePath;
        public SafeFileHandle HidDevice;
        public bool OpenedForRead;
        public bool OpenedForWrite;
        public bool OpenedOverlapped;
        public bool OpenedExclusive;

        public IntPtr Ppd;
        public HIDP_CAPS Caps;
        public HIDD_ATTRIBUTES Attributes;

        public IntPtr[] InputReportBuffer;
        public HID_DATA[] InputData;
        public Int32 InputDataLength;
        public HIDP_BUTTON_CAPS[] InputButtonCaps;
        public HIDP_VALUE_CAPS[] InputValueCaps;

        public IntPtr[] OutputReportBuffer;
        public HID_DATA[] OutputData;
        public Int32 OutputDataLength;
        public HIDP_BUTTON_CAPS[] OutputButtonCaps;
        public HIDP_VALUE_CAPS[] OutputValueCaps;

        public IntPtr[] FeatureReportBuffer;
        public HID_DATA[] FeatureData;
        public Int32 FeatureDataLength;
        public HIDP_BUTTON_CAPS[] FeatureButtonCaps;
        public HIDP_VALUE_CAPS[] FeatureValueCaps;
    }
    internal enum FileMapProtection : uint
    {
        PageReadonly = 0x02,
        PageReadWrite = 0x04,
        PageWriteCopy = 0x08,
        PageExecuteRead = 0x20,
        PageExecuteReadWrite = 0x40,
        SectionCommit = 0x8000000,
        SectionImage = 0x1000000,
        SectionNoCache = 0x10000000,
        SectionReserve = 0x4000000,
    }

    internal enum HIDP_REPORT_TYPE : ushort
    {
        HidP_Input = 0x00,
        HidP_Output = 0x01,
        HidP_Feature = 0x02,
    }
}
