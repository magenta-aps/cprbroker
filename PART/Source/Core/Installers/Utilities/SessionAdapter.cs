using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;

namespace CprBroker.Installers
{
    public interface ISessionAdapter
    {
        string this[string name] { get; set; }
        CustomActionData CustomActionData { get; }
    }

    public class SessionAdapter : ISessionAdapter
    {
        private Session _Session;

        public SessionAdapter(Session session)
        {
            _Session = session;
        }

        
        public CustomActionData CustomActionData
        {
            get { return _Session.CustomActionData; }
        }

        public string this[string name]
        {
            get
            {
 
            }
        }
    }
}
