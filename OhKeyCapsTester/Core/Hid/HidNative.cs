using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using OhKeyCapsTester.Contracts;

namespace OhKeyCapsTester.Core.Hid
{
    public interface IHidNative : IService
    {
        SafeFileHandle Open(ushort vid, ushort pid, int usagePage, int usage);
        int Read(SafeFileHandle h, byte[] buffer, int bufferSize);
        void Close(SafeFileHandle h);
    }

    public class HidNative : IHidNative
    {
        private const int DIGCF_PRESENT = 0x00000002;
        private const int DIGCF_DEVICEINTERFACE = 0x00000010;
        private const uint GENERIC_READ = 0x80000000;
        private const uint GENERIC_WRITE = 0x40000000;
        private const uint FILE_SHARE_READ = 0x00000001;
        private const uint FILE_SHARE_WRITE = 0x00000002;
        private const uint FILE_FLAG_OVERLAPPED = 0x40000000;
        private const uint OPEN_EXISTING = 3;
        private const int HIDP_STATUS_SUCCESS = 0;

        private static readonly IntPtr INVALID_HANDLE_VALUE = IntPtr.Zero;
        private static readonly IntPtr NULL = IntPtr.Zero;

        public SafeFileHandle Open(ushort vid, ushort pid, int usagePage, int usage)
        {
            HidD_GetHidGuid(out Guid guid);
            var info = SetupDiGetClassDevs(ref guid, NULL, NULL, DIGCF_PRESENT | DIGCF_DEVICEINTERFACE);
            if (info == INVALID_HANDLE_VALUE)
            {
//                printf("HID/win32: SetupDiGetClassDevs failed");
                return new SafeFileHandle(IntPtr.Zero, false);
            }
            var deviceInfoData = new SP_DEVICE_INTERFACE_DATA
            {
                cbSize = Marshal.SizeOf(typeof(SP_DEVICE_INTERFACE_DATA))
            };

            for (var index = 0;; index++)
            {
                var hasMore = SetupDiEnumDeviceInterfaces(info, NULL, ref guid, index, ref deviceInfoData);
                if (!hasMore)
                {
                    SetupDiDestroyDeviceInfoList(info);
                    return new SafeFileHandle(IntPtr.Zero, false);
                }

                if (GetDeviceDetails(info, deviceInfoData, out var details)) continue;

                var h = CreateFile(details.DevicePath, GENERIC_READ | GENERIC_WRITE,
                    FILE_SHARE_READ | FILE_SHARE_WRITE, 0, OPEN_EXISTING,
                    0, NULL);
                if (h.IsInvalid) continue;
                var attrib = new HIDD_ATTRIBUTES {Size = Marshal.SizeOf<HIDD_ATTRIBUTES>()};
                hasMore = HidD_GetAttributes(h, ref attrib);
                if (!hasMore)
                {
                    h.Close();
                    continue;
                }

                //printf("HID/win32:   USB Device:\n");
                //printf("HID/win32:     vid =        0x%04X\n", (int)(attrib.VendorID));
                //printf("HID/win32:     pid =        0x%04X\n", (int)(attrib.ProductID));
                if (vid > 0 && vid != (int) (attrib.VendorID))
                {
                    h.Close();
                    continue;
                }

                if (pid > 0 && pid != (int) (attrib.ProductID))
                {
                    h.Close();
                    continue;
                }
                var hid_data = IntPtr.Zero;
                if (!HidD_GetPreparsedData(h, ref hid_data))
                {
//                    printf("HID/win32: HidD_GetPreparsedData failed\n");
                    h.Close();
                    continue;
                }
                var capabilities = new HIDP_CAPS();
                HidP_GetCaps(hid_data, ref capabilities);
//                if (hidPGetCaps != 0)
//                {
////                    printf("HID/win32: HidP_GetCaps failed\n");
//                    HidD_FreePreparsedData(ref hid_data);
//                    h.Close();
//                    continue;
//                }

                if (usagePage > 0 && usagePage != (int) (capabilities.UsagePage))
                {
                    HidD_FreePreparsedData(ref hid_data);
                    h.Close();
                    continue;
                }

                if (usage > 0 && usage != (int) (capabilities.Usage))
                {
                    HidD_FreePreparsedData(ref hid_data);
                    h.Close();
                    continue;
                }

                HidD_FreePreparsedData(ref hid_data);
                return h;
            }
        }

        private static bool GetDeviceDetails(IntPtr info, SP_DEVICE_INTERFACE_DATA deviceInfoData, out SP_DEVICE_INTERFACE_DETAIL_DATA details)
        {
            bool hasMore;
            int required_size = 0;
            SetupDiGetDeviceInterfaceDetail(info, ref deviceInfoData, NULL, 0, ref required_size, NULL);
            details = new SP_DEVICE_INTERFACE_DETAIL_DATA();
            if (IntPtr.Size == 8)
                details.cbSize = 8;
            else if (IntPtr.Size == 4)
                details.cbSize = 5;

            hasMore = SetupDiGetDeviceInterfaceDetail(info, ref deviceInfoData, ref details, required_size, ref required_size, IntPtr.Zero);
            if (!hasMore) return true;
            return false;
        }

        public int Read(SafeFileHandle h, byte[] buffer, int bufferSize)
        {
//            var overlapped = new Overlapped();
//            overlapped.Pack((code, bytes, overlap) =>
//            {
//                if(Thread.Ge)
//            })
            uint bytesRead = 0;
            ReadFile(h, buffer, (uint)bufferSize, ref bytesRead, IntPtr.Zero);
            return (int)bytesRead;
        }

        public void Close(SafeFileHandle h)
        {
            h.Close();
        }
        [DllImport("kernel32.dll")]
        static extern bool ReadFile(SafeFileHandle hFile, [Out] byte[] lpBuffer, uint nNumberOfBytesToRead, ref uint lpNumberOfBytesRead, IntPtr lpOverlapped);

        /*

int rawhid_status(rawhid_t *hid)
{
    PHIDP_PREPARSED_DATA hid_data;

    if (!hid) return -1;
    if (!HidD_GetPreparsedData(((struct rawhid_struct *)hid)->handle, &hid_data)) {
        printf("HID/win32: HidD_GetPreparsedData failed, device assumed disconnected\n");
        return -1;
    }
    printf("HID/win32: HidD_GetPreparsedData ok, device still online :-)\n");
    HidD_FreePreparsedData(hid_data);
    return 0;
}

void rawhid_close(rawhid_t *hid)
{
    if (!hid) return;
    CloseHandle(((struct rawhid_struct *)hid)->handle);
    free(hid);
}

int rawhid_read(rawhid_t *h, void *buf, int bufsize, int timeout_ms)
{
    DWORD num=0, result;
    BOOL ret;
    OVERLAPPED ov;
    struct rawhid_struct *hid;
    int r;

    hid = (struct rawhid_struct *)h;
    if (!hid) return -1;

    memset(&ov, 0, sizeof(OVERLAPPED));
    ov.hEvent = CreateEvent(NULL, TRUE, FALSE, NULL);
    if (ov.hEvent == NULL) return -1;

    ret = ReadFile(hid->handle, buf, bufsize, &num, &ov);
    if (ret) {
        //printf("HID/win32:   read success (immediate)\n");
        r = num;
    } else {
        if (GetLastError() == ERROR_IO_PENDING) {
            result = WaitForSingleObject(ov.hEvent, timeout_ms);
            if (result == WAIT_OBJECT_0) {
                if (GetOverlappedResult(hid->handle, &ov, &num, FALSE)) {
                    //printf("HID/win32:   read success (delayed)\n");
                    r = num;
                } else {
                    //printf("HID/win32:   read failure (delayed)\n");
                    r = -1;
                }
            } else {
                //printf("HID/win32:   read timeout, %lx\n", result);
                CancelIo(hid->handle);
                r = 0;
            }
        } else {
            //printf("HID/win32:   read error (immediate)\n");
            r = -1;
        }
    }
    CloseHandle(ov.hEvent);
    return r;
}


int rawhid_write(rawhid_t *h, const void *buf, int len, int timeout_ms)
{
    DWORD num=0;
    BOOL ret;
    OVERLAPPED ov;
    struct rawhid_struct *hid;
    int r;

    hid = (struct rawhid_struct *)h;
    if (!hid) return -1;

    memset(&ov, 0, sizeof(OVERLAPPED));
    ov.hEvent = CreateEvent(NULL, TRUE, FALSE, NULL);
    if (ov.hEvent == NULL) return -1;

    // first byte is report ID, must be zero if report IDs not used
    ret = WriteFile(hid->handle, buf, len, &num, &ov);
    if (ret) {
        if (num == len) {
            //printf("HID/win32:   write success (immediate)\n");
            r = 0;
        } else {
            //printf("HID/win32:   partial write (immediate)\n");
            r = -1;
        }
    } else {
        if (GetLastError() == ERROR_IO_PENDING) {
            if (GetOverlappedResult(hid->handle, &ov, &num, TRUE)) {
                if (num == len) {
                    //printf("HID/win32:   write success (delayed)\n");
                    r = 0;
                } else {
                    //printf("HID/win32:   partial write (delayed)\n");
                    r = -1;
                }
            } else {
                //printf("HID/win32:   write error (delayed)\n");
                r = -1;
            }
        } else {
            //printf("HID/win32:   write error (immediate)\n");
            r = -1;
        }
    }
    CloseHandle(ov.hEvent);
    return r;
}
         */
        [DllImport("hid.dll")]
        private static extern void HidD_GetHidGuid(out Guid Guid);

        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern bool SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);

        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern IntPtr SetupDiGetClassDevs(ref Guid ClassGuid, IntPtr Enumerator, IntPtr hwndParent, int Flags);

        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern bool SetupDiEnumDeviceInterfaces(IntPtr hDevInfo, IntPtr devInfo, ref Guid interfaceClassGuid, int memberIndex, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);

        [DllImport(@"kernel32.dll", SetLastError = true)]
        private static extern SafeFileHandle CreateFile(string fileName, uint fileAccess, uint fileShare, FileMapProtection securityAttributes, uint creationDisposition, uint flags, IntPtr overlapped);

        [DllImport("hid.dll", SetLastError = true)]
        private static extern bool HidD_GetPreparsedData(SafeFileHandle HidDeviceObject, ref IntPtr PreparsedData);

        [DllImport("hid.dll", SetLastError = true)]
        private static extern bool HidD_GetAttributes(SafeFileHandle DeviceObject, ref HIDD_ATTRIBUTES Attributes);

        [DllImport("hid.dll", SetLastError = true)]
        private static extern uint HidP_GetCaps(IntPtr PreparsedData, ref HIDP_CAPS Capabilities);

        [DllImport(@"setupapi.dll", SetLastError = true)]
        private static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr DeviceInfoSet, ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData, ref SP_DEVICE_INTERFACE_DETAIL_DATA DeviceInterfaceDetailData, int DeviceInterfaceDetailDataSize,
            ref int RequiredSize, IntPtr DeviceInfoData);

        [DllImport(@"setupapi.dll", SetLastError = true)]
        private static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr DeviceInfoSet, ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData, IntPtr DeviceInterfaceDetailData, int DeviceInterfaceDetailDataSize, ref int RequiredSize,
            IntPtr DeviceInfoData);

        [DllImport("hid.dll", SetLastError = true)]
        private static extern bool HidD_FreePreparsedData(ref IntPtr PreparsedData);

        [DllImport("kernel32.dll")]
        private static extern uint GetLastError();
    }
}
