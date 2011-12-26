/* ***** BEGIN LICENSE BLOCK *****
 * Version: MPL 1.1/GPL 2.0/LGPL 2.1
 *
 * The contents of this file are subject to the Mozilla Public License
 * Version 1.1 (the "License"); you may not use this file except in
 * compliance with the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS"basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 * The CPR Broker concept was initally developed by
 * Gentofte Kommune / Municipality of Gentofte, Denmark.
 * Contributor(s):
 * Steen Deth
 *
 *
 * The Initial Code for CPR Broker and related components is made in
 * cooperation between Magenta, Gentofte Kommune and IT- og Telestyrelsen /
 * Danish National IT and Telecom Agency
 *
 * Contributor(s):
 * Beemen Beshara
 * Niels Elgaard Larsen
 * Leif Lodahl
 * Steen Deth
 *
 * The code is currently governed by IT- og Telestyrelsen / Danish National
 * IT and Telecom Agency
 *
 * Alternatively, the contents of this file may be used under the terms of
 * either the GNU General Public License Version 2 or later (the "GPL"), or
 * the GNU Lesser General Public License Version 2.1 or later (the "LGPL"),
 * in which case the provisions of the GPL or the LGPL are applicable instead
 * of those above. If you wish to allow use of your version of this file only
 * under the terms of either the GPL or the LGPL, and not to allow others to
 * use your version of this file under the terms of the MPL, indicate your
 * decision by deleting the provisions above and replace them with the notice
 * and other provisions required by the GPL or the LGPL. If you do not delete
 * the provisions above, a recipient may use your version of this file under
 * the terms of any one of the MPL, the GPL or the LGPL.
 *
 * ***** END LICENSE BLOCK ***** */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace CprBroker.Installers
{
    public class InternetSecurityManager
    {
        public static class URLZONE
        {
            public const int URLZONE_INVALID = -1;
            public const int URLZONE_PREDEFINED_MIN = 0;
            public const int URLZONE_LOCAL_MACHINE = 0;
            public const int URLZONE_INTRANET = 1;
            public const int URLZONE_TRUSTED = 2;
            public const int URLZONE_INTERNET = 3;
            public const int URLZONE_UNTRUSTED = 4;
            public const int URLZONE_PREDEFINED_MAX = 999;
            public const int URLZONE_USER_MIN = 1000;
            public const int URLZONE_USER_MAX = 1000;
        } ;

        public static class SZM_FLAGS
        {
            public const int SZM_CREATE = 0x00000000;
            public const int SZM_DELETE = 0x00000001;
        } ;

        public class WinError
        {
            public const UInt32 S_OK = 0;
            public const UInt32 E_ACCESSDENIED = 0x80070005;
            public const UInt32 E_FAIL = 0x80004005;

            public static void TestHResult(int hResult)
            {
                if (hResult != WinError.S_OK)
                    throw new Exception(string.Format("Failed, HRESULT={0}", hResult));
            }
        }

        public static Guid CLSID_InternetSecurityManager = new Guid("7b8a2d94-0ac9-11d1-896c-00c04fb6bfc4");
        public static Guid IID_IInternetSecurityManager = new Guid("79eac9ee-baf9-11ce-8c82-00aa004ba90b");

        public static IInternetSecurityManager CreateObject()
        {
            Type t = Type.GetTypeFromCLSID(CLSID_InternetSecurityManager);
            object obj = Activator.CreateInstance(t);
            return obj as IInternetSecurityManager;
        }

        [ComImport]
        [GuidAttribute("79EAC9EE-BAF9-11CE-8C82-00AA004BA90B")]
        [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IInternetSecurityManager
        {
            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int SetSecuritySite([In] IntPtr pSite);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int GetSecuritySite([Out] IntPtr pSite);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int MapUrlToZone([In, MarshalAs(UnmanagedType.LPWStr)] string pwszUrl, out UInt32 pdwZone, UInt32 dwFlags);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int GetSecurityId([MarshalAs(UnmanagedType.LPWStr)] string pwszUrl, [MarshalAs(UnmanagedType.LPArray)] byte[] pbSecurityId, ref UInt32 pcbSecurityId, uint dwReserved);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int ProcessUrlAction([In, MarshalAs(UnmanagedType.LPWStr)] string pwszUrl, UInt32 dwAction, out byte pPolicy, UInt32 cbPolicy, byte pContext, UInt32 cbContext, UInt32 dwFlags, UInt32 dwReserved);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int QueryCustomPolicy([In, MarshalAs(UnmanagedType.LPWStr)] string pwszUrl, ref Guid guidKey, ref byte ppPolicy, ref UInt32 pcbPolicy, ref byte pContext, UInt32 cbContext, UInt32 dwReserved);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int SetZoneMapping(UInt32 dwZone, [In, MarshalAs(UnmanagedType.LPWStr)] string lpszPattern, UInt32 dwFlags);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int GetZoneMappings(UInt32 dwZone, out UCOMIEnumString ppenumString, UInt32 dwFlags);
        }

    }
}
