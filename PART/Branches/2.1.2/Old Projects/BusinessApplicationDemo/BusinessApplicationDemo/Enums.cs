using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessApplicationDemo
{
    /// <summary>
    /// Contains enumerations that represent the system web methods
    /// </summary>
    public class Enums
    {
        public enum PersonMethod
        {
            GetCitizenBasic = 1,
            GetCitizenNameAndAddress = 2,
            GetCitizenFull = 3,
            GetCitizenRelations = 4,
            GetCitizenChildren = 5,
            RemoveParentAuthorityOverChild = 106,
            SetParentAuthorityOverChild = 107,
            GetParentAuthorityOverChildChanges = 8
        }
        public class Admin
        {
            public enum ApplicationMethod
            {
                RequestAppRegisteration = 101,
                ApproveAppRegisteration = 102,
                ListAppRegisteration = 3,
                UnregisterApp = 104
            }
            public enum SubscriptionMethod
            {
                Subscribe = 101,
                Unsubscribe = 102,
                SubscribeOnBirthdate = 103,
                RemoveBirthDateSubscriptions = 104,
                GetActiveSubsciptionsList = 5
            }
            public enum OtherMethod
            {
                GetCapabilities = 1,
                IsImplementing = 102,
                GetCPRDataProviderList = 3,
                SetCPRDataProviderList = 104,
                LogFunctions = 105,
                CreateTestCitizen = 106
            }
        }
    }
}

