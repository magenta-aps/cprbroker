using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Serialization;
using CprBroker.Schemas.Part;
using CprBroker.Utilities;
using System.Web;
using System.Web.Services;

namespace CprBroker.Engine
{
    public class SchemaSoapExtension : System.Web.Services.Protocols.SoapExtension
    {
        #region Initialization

        Stream oldStream;
        Stream newStream;


        public override object GetInitializer(Type serviceType)
        {
            return "";
        }

        public override object GetInitializer(System.Web.Services.Protocols.LogicalMethodInfo methodInfo, System.Web.Services.Protocols.SoapExtensionAttribute attribute)
        {
            return "";
        }

        public override void Initialize(object initializer)
        {

        }

        public override System.IO.Stream ChainStream(System.IO.Stream stream)
        {
            oldStream = stream;
            newStream = new MemoryStream();
            return newStream;
        }

        #endregion

        #region Utility classes & methods

        class HeaderException : Exception
        {
        }

        private void Copy(Stream from, Stream to)
        {
            from.Position = 0;
            TextReader reader = new StreamReader(from);
            TextWriter writer = new StreamWriter(to);
            writer.WriteLine(reader.ReadToEnd());
            writer.Flush();
            if (to.CanSeek)
            {
                to.Seek(0, SeekOrigin.Begin);
            }
        }

        private IBasicOutput CreateReturnValue(SoapMessage message)
        {
            Exception ex = Misc.GetDeepestInnerException(message.Exception);
            var ret = Utilities.Reflection.CreateInstance(message.MethodInfo.ReturnType);
            var output = ret as Schemas.Part.IBasicOutput;
            if (ex is XmlException)
            {
                output.StandardRetur = StandardReturType.MalformedXml();
            }
            else if (ex is HeaderException)
            {
                output.StandardRetur = StandardReturType.NullInput("applicationHeader");
            }
            else
            {
                output.StandardRetur = StandardReturType.UnspecifiedError();
            }
            return output;
        }

        private void GetWebMethodNames(SoapMessage message, out string messageName, out string serviceNamespace)
        {
            messageName = message.MethodInfo.Name;
            serviceNamespace = "http://tempuri.org";
            if (message.MethodInfo.MethodInfo.IsDefined(typeof(WebMethodAttribute), true))
            {
                var methodAttr = message.MethodInfo.MethodInfo.GetCustomAttributes(typeof(WebMethodAttribute), true)[0] as WebMethodAttribute;
                if (!string.IsNullOrEmpty(methodAttr.MessageName))
                {
                    messageName = methodAttr.MessageName;
                }

                if (message.MethodInfo.MethodInfo.DeclaringType.IsDefined(typeof(WebServiceAttribute), true))
                {
                    var serviceAttr = message.MethodInfo.MethodInfo.DeclaringType.GetCustomAttributes(typeof(WebServiceAttribute), true)[0] as WebServiceAttribute;
                    if (!string.IsNullOrEmpty(serviceAttr.Namespace))
                    {
                        serviceNamespace = serviceAttr.Namespace;
                    }
                }
            }
        }

        private XmlNode GetBodyNode()
        {
            // Now modify the response
            newStream.Position = 0;
            TextReader reader = new StreamReader(newStream);
            string xml = reader.ReadToEnd();

            if (string.IsNullOrEmpty(xml))
            {
                xml = "<soap:Envelope xmlns:soap=\"http://www.w3.org/2003/05/soap-envelope\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"> <soap:Body></soap:Body></soap:Envelope>";
            }

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("soap", doc.LastChild.GetNamespaceOfPrefix("soap"));

            var bodyNode = doc.SelectSingleNode("//soap:Body", nsmgr);
            return bodyNode;
        }

        private void WriteResponse(XmlDocument responseSoap)
        {
            StreamWriter writer = new StreamWriter(oldStream);
            writer.Write(responseSoap.OuterXml);
            writer.Flush();
        }

        #endregion

        public override void ProcessMessage(SoapMessage message)
        {
            switch (message.Stage)
            {
                case SoapMessageStage.BeforeDeserialize:
                    OnBeforeDeserialize(message);
                    break;
                case SoapMessageStage.AfterDeserialize:
                    OnAfterDeserialize(message);
                    break;
                case SoapMessageStage.BeforeSerialize:
                    break;
                case SoapMessageStage.AfterSerialize:
                    OnAfterSerialize(message);
                    break;
            }
        }

        void OnBeforeDeserialize(SoapMessage message)
        {
            Copy(oldStream, newStream);
        }

        void OnAfterDeserialize(SoapMessage message)
        {
            foreach (var header in message.Headers)
            {
                if (header is Schemas.ApplicationHeader)
                {
                    return;
                }
            }
            throw new HeaderException();
        }

        void OnAfterSerialize(SoapMessage message)
        {
            if (message.Exception != null && typeof(IBasicOutput).IsAssignableFrom(message.MethodInfo.ReturnType))
            {
                var output = CreateReturnValue(message);

                string messageName, serviceNamespace;
                GetWebMethodNames(message, out messageName, out serviceNamespace);

                var bodyNode = GetBodyNode();
                var outNode = bodyNode.OwnerDocument.CreateElement("", messageName + "Response", serviceNamespace);

                string bodyInnerXml = Strings.SerializeObject(output, true);
                outNode.InnerXml = bodyInnerXml;

                bodyNode.RemoveAll();
                bodyNode.AppendChild(outNode);

                WriteResponse(bodyNode.OwnerDocument);
                return;
            }

            newStream.Position = 0;
            Copy(newStream, oldStream);
        }

    }
}
