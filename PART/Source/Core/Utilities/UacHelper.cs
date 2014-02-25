using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Diagnostics;
using System.Security.Principal;

namespace CprBroker.Utilities
{
    public static class UacHelper
    {
        private const string uacRegistryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System";
        private const string uacRegistryValue = "EnableLUA";

        private const uint STANDARD_RIGHTS_READ = 0x00020000;
        private const uint TOKEN_QUERY = 0x0008;
        private const uint TOKEN_READ = (STANDARD_RIGHTS_READ | TOKEN_QUERY);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool OpenProcessToken(IntPtr ProcessHandle, UInt32 DesiredAccess, out IntPtr TokenHandle);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool GetTokenInformation(IntPtr TokenHandle, TOKEN_INFORMATION_CLASS TokenInformationClass,
                                                      IntPtr TokenInformation, uint TokenInformationLength,
                                                      out uint ReturnLength);

        public enum TOKEN_INFORMATION_CLASS
        {
            TokenUser = 1,
            TokenGroups,
            TokenPrivileges,
            TokenOwner,
            TokenPrimaryGroup,
            TokenDefaultDacl,
            TokenSource,
            TokenType,
            TokenImpersonationLevel,
            TokenStatistics,
            TokenRestrictedSids,
            TokenSessionId,
            TokenGroupsAndPrivileges,
            TokenSessionReference,
            TokenSandBoxInert,
            TokenAuditPolicy,
            TokenOrigin,
            TokenElevationType,
            TokenLinkedToken,
            TokenElevation,
            TokenHasRestrictions,
            TokenAccessInformation,
            TokenVirtualizationAllowed,
            TokenVirtualizationEnabled,
            TokenIntegrityLevel,
            TokenUIAccess,
            TokenMandatoryPolicy,
            TokenLogonSid,
            MaxTokenInfoClass
        }

        public enum TOKEN_ELEVATION_TYPE
        {
            TokenElevationTypeDefault = 1,
            TokenElevationTypeFull,
            TokenElevationTypeLimited
        }

        private static bool? _isUacEnabled;
        public static bool IsUacEnabled
        {
            get
            {
                if (_isUacEnabled == null)
                {
                    var uacKey = Registry.LocalMachine.OpenSubKey(uacRegistryKey, false);
                    if (uacKey == null)
                    {
                        _isUacEnabled = false;
                    }
                    else
                    {
                        var enableLua = uacKey.GetValue(uacRegistryValue);
                        _isUacEnabled = enableLua.Equals(1);
                    }
                }
                return _isUacEnabled.Value;
            }
        }

        private static bool? _isAdministrator;
        public static bool IsAdministrator
        {
            get
            {
                if (_isAdministrator == null)
                {
                    var identity = WindowsIdentity.GetCurrent();
                    Debug.Assert(identity != null);
                    var principal = new WindowsPrincipal(identity);
                    _isAdministrator = principal.IsInRole(WindowsBuiltInRole.Administrator);
                }
                return _isAdministrator.Value;
            }
        }

        struct _TOKEN_ELEVATION
        {
            public UInt32 TokenIsElevated;
        }

        private static bool? _isProcessElevated;
        public static bool IsProcessElevated
        {
            get
            {
                if (_isProcessElevated == null)
                {
                    if (IsUacEnabled)
                    {
                        var process = Process.GetCurrentProcess();

                        IntPtr tokenHandle;
                        if (!OpenProcessToken(Process.GetCurrentProcess().Handle, TOKEN_READ, out tokenHandle))
                        {
                            throw new ApplicationException("Could not get process token.  Win32 Error Code: " + Marshal.GetLastWin32Error());
                        }

                        var elevationResult = new _TOKEN_ELEVATION();

                        var elevationResultSize = Marshal.SizeOf(elevationResult);
                        uint returnedSize;
                        var elevationTypePtr = Marshal.AllocHGlobal(elevationResultSize);

                        var success = GetTokenInformation(tokenHandle, TOKEN_INFORMATION_CLASS.TokenElevation, elevationTypePtr, (uint)elevationResultSize, out returnedSize);
                        if (!success)
                        {
                            Marshal.FreeHGlobal(elevationTypePtr);
                            throw new ApplicationException("Unable to determine the current elevation.");
                        }

                        elevationResult = (_TOKEN_ELEVATION)Marshal.PtrToStructure(elevationTypePtr, typeof(_TOKEN_ELEVATION));
                        Marshal.FreeHGlobal(elevationTypePtr);

                        _isProcessElevated = elevationResult.TokenIsElevated > 0;
                        
                    }
                    else
                    {
                        _isProcessElevated = IsAdministrator;
                    }
                }
                return _isProcessElevated.Value;
            }
        }
    }
}
