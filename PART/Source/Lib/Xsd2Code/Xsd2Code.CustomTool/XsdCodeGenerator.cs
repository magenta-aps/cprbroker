using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Xsd2Code.Library;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Xsd2Code.CustomTool
{
    [Guid("C869B15D-12A6-42D4-B024-7EFBA337D842")]
    [CustomTool(Name= "XsdCodeGenerator", Description = "Generates code from XSD file.")]
    public class XsdCodeGenerator :  IVsSingleFileGenerator
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
            GeneratorParams generatorParams = new GeneratorParams()
            {
                NameSpace = wszDefaultNamespace,
                InputFilePath = wszInputFilePath,
                OutputFilePath = wszInputFilePath + ".designer.cs.temp"
            };
            generatorParams.Miscellaneous.ExcludeIncludedTypes = true;

            // Create an instance of Generator
            var generator = new GeneratorFacade(generatorParams);

            // Generate code
            var result = generator.GenerateBytes();
            if (result.Success)
            {
                var bytes = result.Entity;
                rgbOutputFileContents[0] = Marshal.AllocCoTaskMem(bytes.Length);
                Marshal.Copy(bytes, 0, rgbOutputFileContents[0], bytes.Length);

                pcbOutput = (uint)bytes.Length;
                return Microsoft.VisualStudio.VSConstants.S_OK;
            }
            else
            {
                pcbOutput = 0;
                return Microsoft.VisualStudio.VSConstants.S_FALSE;
            }
            
        }

        [ComRegisterFunction]
        public static void RegisterClass(Type t)
        {
            GuidAttribute guidAttribute = getGuidAttribute(t);
            CustomToolAttribute customToolAttribute = getCustomToolAttribute(t);
            using (RegistryKey key = Registry.LocalMachine.CreateSubKey(
              GetKeyName(CSharpCategoryGuid, customToolAttribute.Name)))
            {
                key.SetValue("", customToolAttribute.Description);
                key.SetValue("CLSID", "{" + guidAttribute.Value + "}");
                key.SetValue("GeneratesDesignTimeSource", 1);
            }
        }

        [ComUnregisterFunction]
        public static void UnregisterClass(Type t)
        {
            CustomToolAttribute customToolAttribute = getCustomToolAttribute(t);
            Registry.LocalMachine.DeleteSubKey(GetKeyName(
              CSharpCategoryGuid, customToolAttribute.Name), false);
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

        internal static string GetKeyName(Guid categoryGuid, string toolName)
        {
            return
              String.Format("SOFTWARE\\Microsoft\\VisualStudio\\" + VisualStudioVersion +
                "\\Generators\\{{{0}}}\\{1}\\", categoryGuid, toolName);
        }


    }
}
