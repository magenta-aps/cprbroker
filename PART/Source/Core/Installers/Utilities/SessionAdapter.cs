using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CprBroker.Installers
{
    public class SessionAdapter
    {

        public Session InnerSession { get; set; }

        public string this[string property]
        {
            get { return InnerSession[property]; }
            set { InnerSession[property] = value; }
        }

        public bool GetMode(InstallRunMode mode)
        {
            return InnerSession.GetMode(mode);
        }

        public void DoAction(string action)
        {
            InnerSession.DoAction(action);
        }

        public Database Database
        {
            get { return InnerSession.Database; }
        }

        public CustomActionData CustomActionData
        {
            get { return InnerSession.CustomActionData; }
        }

        public MessageResult Message(InstallMessage messageType, Record record)
        {
            return InnerSession.Message(messageType, record);
        }

        public void Log(string msg)
        {
            InnerSession.Log(msg);
        }
    }
}
