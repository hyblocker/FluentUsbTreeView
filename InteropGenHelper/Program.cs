using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.System.Services;

namespace InteropGenHelper {
    internal class Program {
        static void Main(string[] args) {

        }

        /*
        unsafe void GetServicePath(Windows.Win32.Security.SC_HANDLE hSCManager, string szSvcName) {
            Windows.Win32.Security.SC_HANDLE hService;
            QUERY_SERVICE_CONFIGW* lpsc = null;
            // SERVICE_DESCRIPTION lpsd = NULL;
            int dwBytesNeeded, cbBufSize, dwError;

            // Open a handle to the service.
            hService = PInvoke.OpenService(hSCManager, szSvcName, SERVICE_QUERY_CONFIG);

            if ( hService == IntPtr.Zero ) {
                // printf("OpenService failed (%d)\n", GetLastError());
                Console.WriteLine($"OpenService failed ({Marshal.GetLastWin32Error()})");
                return;
            }

            // Allocate a buffer for the configuration information.
            lpsc = ( QUERY_SERVICE_CONFIGW* ) LocalAlloc(LMEM_FIXED, cbBufSize);

            if ( !PInvoke.QueryServiceConfig(hService, lpsc, cbBufSize, &dwBytesNeeded) ) {
                dwError = Marshal.GetLastWin32Error();
                if ( ERROR_INSUFFICIENT_BUFFER == dwError ) {
                    cbBufSize = dwBytesNeeded;
                    lpsc = ( LPQUERY_SERVICE_CONFIG ) LocalAlloc(LMEM_FIXED, cbBufSize);
                } else {
                    // printf("QueryServiceConfig failed (%d)\n", dwError);
                    Console.WriteLine($"QueryServiceConfig failed ({Marshal.GetLastWin32Error()})");
                    return;
                }
            }

            if ( !PInvoke.QueryServiceConfig(hService, lpsc, (uint)cbBufSize, out dwBytesNeeded) ) {
                // printf("QueryServiceConfig failed (%d)\n", GetLastError());
                Console.WriteLine($"QueryServiceConfig failed ({Marshal.GetLastWin32Error()})");
                return;
            }

            // Print the binary path name.
            // printf("Binary Path Name: %s\n", lpsc->lpBinaryPathName);

            // Cleanup
            PInvoke.CloseServiceHandle(hService);
        }
        */
    }
}
