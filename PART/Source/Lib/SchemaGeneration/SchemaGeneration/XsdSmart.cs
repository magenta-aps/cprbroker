using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;

namespace SchemaGeneration
{
    [Guid("10D17A3E-579D-4FF9-AA2C-5A3234391046")]
    [CustomTool(Name = "XsdSmart", Description = "Generates code from XSD file.")]
    public class XsdSmart : IVsSingleFileGenerator
    {
        internal static Guid CSharpCategoryGuid = new Guid("FAE04EC1-301F-11D3-BF4B-00C04F79EFBC");
        private const string VisualStudioVersion = "10.0";

        public int DefaultExtension(out string pbstrDefaultExtension)
        {
            pbstrDefaultExtension = ".designer.cs";
            return pbstrDefaultExtension.Length;
        }

        public int Generate(string wszInputFilePath, string bstrInputFileContents, string wszDefaultNamespace, IntPtr[] rgbOutputFileContents, out uint pcbOutput, IVsGeneratorProgress pGenerateProgress)
        {
            var file = wszInputFilePath;
            var NameSpace = wszDefaultNamespace;

            // Generate code            
            var bytes = SplitCodeBySchemaSource.GetCodeFileBytes(file, NameSpace);

            // OK
            rgbOutputFileContents[0] = Marshal.AllocCoTaskMem(bytes.Length);
            Marshal.Copy(bytes, 0, rgbOutputFileContents[0], bytes.Length);

            pcbOutput = (uint)bytes.Length;
            return Microsoft.VisualStudio.VSConstants.S_OK;

            // Error
            pcbOutput = 0;
            return Microsoft.VisualStudio.VSConstants.S_FALSE;

        }

        [ComRegisterFunction]
        public static void RegisterClass(Type t)
        {
            GuidAttribute guidAttribute = getGuidAttribute(t);
            CustomToolAttribute customToolAttribute = getCustomToolAttribute(t);
            foreach (var k in GetKeyNames(CSharpCategoryGuid, customToolAttribute.Name))
            {
                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(k))
                {
                    key.SetValue("", customToolAttribute.Description);
                    key.SetValue("CLSID", "{" + guidAttribute.Value + "}");
                    key.SetValue("GeneratesDesignTimeSource", 1);
                }
            }
        }

        [ComUnregisterFunction]
        public static void UnregisterClass(Type t)
        {
            CustomToolAttribute customToolAttribute = getCustomToolAttribute(t);
            foreach (var k in GetKeyNames(CSharpCategoryGuid, customToolAttribute.Name))
            {
                Registry.LocalMachine.DeleteSubKey(k, false);
            }
        }

        internal static GuidAttribute getGuidAttribute(Type t)
        {
            return (GuidAttribute)getAttribute(t, typeof(GuidAttribute));
        }

        internal static CustomToolAttribute getCustomToolAttribute(Type t)
        {
            return (CustomToolAttribute)getAttribute(t, typeof(CustomToolAttribute));
        }

        internal static Attribute getAttribute(Type t, Type attributeType)
        {
            object[] attributes = t.GetCustomAttributes(attributeType, /* inherit */ true);
            if (attributes.Length == 0)
                throw new Exception(
                  String.Format("Class '{0}' does not provide a '{1}' attribute.",
                  t.FullName, attributeType.FullName));
            return (Attribute)attributes[0];
        }

        internal static string[] GetKeyNames(Guid categoryGuid, string toolName)
        {
            var ret = new List<string>();
            ret.Add(GetKeyName(categoryGuid, toolName, false));
            if (Is64Bit())
                ret.Add(GetKeyName(categoryGuid, toolName, true));
            return ret.ToArray();
        }

        internal static string GetKeyName(Guid categoryGuid, string toolName, bool is64Bit)
        {
            return String.Format(
                "SOFTWARE\\{0}Microsoft\\VisualStudio\\{1}\\Generators\\{{{2}}}\\{3}\\",
                is64Bit ? "Wow6432Node\\" : "",
                VisualStudioVersion,
                categoryGuid,
                toolName);
        }

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWow64Process([In] IntPtr hProcess, [Out] out bool lpSystemInfo);

        public static bool Is64Bit()
        {
            if (IntPtr.Size == 8 || (IntPtr.Size == 4 && Is32BitProcessOn64BitProcessor()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool Is32BitProcessOn64BitProcessor()
        {
            bool retVal;

            IsWow64Process(System.Diagnostics.Process.GetCurrentProcess().Handle, out retVal);

            return retVal;
        }
    }
}
