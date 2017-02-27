using System;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace iGP11.Library
{
    public static class WindowsPermissionUtility
    {
        private enum TokenElevationType
        {
            TokenElevationTypeDefault = 1,
            TokenElevationTypeFull,
            TokenElevationTypeLimited
        }

        private enum TokenInformationClass
        {
            TokenElevationType = 18
        }

        public static bool IsCurrentUserAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            if (identity == null)
            {
                throw new InvalidOperationException("Current user identity could not be obtained");
            }

            var principal = new WindowsPrincipal(identity);
            if (principal.IsInRole(WindowsBuiltInRole.Administrator))
            {
                return true;
            }

            if ((Environment.OSVersion.Platform != PlatformID.Win32NT) || (Environment.OSVersion.Version.Major < 6))
            {
                return false;
            }

            var tokenLength = Marshal.SizeOf(typeof(int));
            var tokenInformation = Marshal.AllocHGlobal(tokenLength);

            try
            {
                var token = identity.Token;
                var result = GetTokenInformation(token, TokenInformationClass.TokenElevationType, tokenInformation, tokenLength, out tokenLength);

                if (!result)
                {
                    var exception = Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
                    throw new InvalidOperationException("Token information could not be obtained", exception);
                }

                var elevationType = (TokenElevationType)Marshal.ReadInt32(tokenInformation);
                switch (elevationType)
                {
                    case TokenElevationType.TokenElevationTypeDefault:
                        return false;
                    case TokenElevationType.TokenElevationTypeFull:
                        return true;
                    case TokenElevationType.TokenElevationTypeLimited:
                        return true;
                    default:
                        return false;
                }
            }
            finally
            {
                if (tokenInformation != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(tokenInformation);
                }
            }
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool GetTokenInformation(IntPtr tokenHandle, TokenInformationClass tokenInformationClass, IntPtr tokenInformation, int tokenInformationLength, out int returnLength);
    }
}