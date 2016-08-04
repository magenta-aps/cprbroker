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

        public virtual string this[string property]
        {
            get { return InnerSession[property]; }
            set { InnerSession[property] = value; }
        }

        public virtual bool GetMode(InstallRunMode mode)
        {
            return InnerSession.GetMode(mode);
        }

        public virtual void DoAction(string action)
        {
            InnerSession.DoAction(action);
        }

        public virtual Database Database
        {
            get { return InnerSession.Database; }
        }

        public virtual CustomActionData CustomActionData
        {
            get { return InnerSession.CustomActionData; }
        }

        public virtual MessageResult Message(InstallMessage messageType, Record record)
        {
            return InnerSession.Message(messageType, record);
        }

        public virtual void Log(string msg)
        {
            InnerSession.Log(msg);
        }
    }
}
