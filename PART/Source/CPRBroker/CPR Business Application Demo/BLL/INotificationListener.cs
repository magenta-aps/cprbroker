using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPR_Business_Application_Demo.Business
{
    public interface INotificationListener
    {
        void ReportChangedPersonRegistrationIds(List<NotificationPerson> changedPersonRegistrationIds);
    }

    public class NotificationPerson
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Cpr { get; set; }
    }

}
