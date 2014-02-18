using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection;
using Microsoft.CSharp;
using System.IO;
using System.Xml;

namespace SchemaGeneration
{
    class ByCompileAndGenerate
    {
        public static void Run()
        {
            // Assuming all file exists

            CodeDomProvider prov = new CSharpCodeProvider();
            var options = new CompilerParameters()
            {
                GenerateInMemory = true,
                GenerateExecutable = false,
            };

            options.ReferencedAssemblies.AddRange(new string[] { "System.dll", "System.Core.dll", "System.Xml.dll", "System.Xml.Linq.dll" });
            var result = prov.CompileAssemblyFromFile(
                options,
                @"C:\Magenta Workspace\PART\Source\Core\Schemas\OIOXSD\XSD.cs"
                );

            if (!result.Errors.HasErrors)
            {
                var asm = result.CompiledAssembly;
                var allTypes = asm.GetTypes();
                var nameSpace = allTypes[0].Namespace;


                // Now parse the XSD files
                string dir = @"C:\Magenta Workspace\PART\Source\Core\Schemas\OIOXSD\";
                var files = Directory.GetFiles(dir, "*.xsd");

                foreach (var file in files)
                {
                    var fileTypes = SplitCodeBySchemaSource.GetTypesInFile(file);
                    var typesToInclude = allTypes.Where(t => fileTypes.Contains(t.Name)).ToArray();

                    CodeNamespace localCodeNamespace = new CodeNamespace(nameSpace);
                    localCodeNamespace.Imports.Add(new CodeNamespaceImport("System.Xml.Serialization"));
                    localCodeNamespace.Types.AddRange(typesToInclude.Select(t => FromType(t)).ToArray());


                    string codeFileName = file.Replace(".xsd", ".designer.cs");


                    using (var w = new StreamWriter(codeFileName))
                    {
                        prov.GenerateCodeFromNamespace(localCodeNamespace, w, new CodeGeneratorOptions() { });
                    }
                }
            }
        }

        public static CodeTypeDeclaration FromType(Type type)
        {
            var ret = new CodeTypeDeclaration()
            {
                Name = type.Name,
                IsEnum = type.IsEnum,
                IsClass = type.IsClass,
                IsInterface = type.IsInterface,

            };

            if (!type.BaseType.Equals(typeof(object)) && !type.BaseType.Equals(typeof(Enum)))
            {
                ret.BaseTypes.Add(type.BaseType.Name);
            }
            ret.CustomAttributes.AddRange(GetCodeAttributes(type));

            BindingFlags searchFlags = BindingFlags.DeclaredOnly | BindingFlags.Public;
            if (type.IsClass)
            {
                searchFlags = searchFlags | BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
            }
            else if (type.IsEnum)
            {
                searchFlags = searchFlags | BindingFlags.Static;
            }

            var fields = type.GetFields(searchFlags);
            foreach (var field in fields)
            {
                var codeField = new CodeMemberField()
                {
                    Name = field.Name,
                    Attributes = field.IsPrivate ? MemberAttributes.Private : MemberAttributes.Public,
                    Type = new CodeTypeReference(field.FieldType.Name)
                };
                codeField.CustomAttributes.AddRange(GetCodeAttributes(field));
                ret.Members.Add(codeField);
            }

            foreach (var prop in type.GetProperties(searchFlags))
            {
                var codeProp = new CodeMemberProperty()
                {
                    Name = prop.Name,
                    HasGet = prop.CanRead,
                    HasSet = prop.CanWrite,
                    Type = new CodeTypeReference(prop.PropertyType.Name),
                };
                codeProp.CustomAttributes.AddRange(GetCodeAttributes(prop));
                var correspondingField = fields.Where(f => f.Name.ToLower() == prop.Name.ToLower() + "field").FirstOrDefault();

                if (codeProp.HasGet && correspondingField != null)
                {
                    codeProp.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), correspondingField.Name)));
                }
                if (codeProp.HasSet && correspondingField != null)
                {
                    codeProp.SetStatements.Add(new CodeAssignStatement(
                        new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), correspondingField.Name),
                        new CodeVariableReferenceExpression("value")
                        ));
                }
                ret.Members.Add(codeProp);
            }
            return ret;
        }


        static CodeAttributeDeclaration[] GetCodeAttributes(MemberInfo member)
        {
            var attribs = member.GetCustomAttributesData();
            return attribs.Select(a =>
            {
                var ret = new CodeAttributeDeclaration() { Name = a.Constructor.DeclaringType.Name };

                foreach (var arg in a.ConstructorArguments)
                {
                    ret.Arguments.Add(new CodeAttributeArgument()
                    {

                        //Name = arg.Value.ToString(),
                        Value = new CodeVariableReferenceExpression("DD")
                    });
                }
                return ret;
            }).ToArray();
        }
    }
}
