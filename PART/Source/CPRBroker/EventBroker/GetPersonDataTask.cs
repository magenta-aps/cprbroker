using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPRBroker.Engine;
using CPRBroker.Engine.Tasks;

namespace CprBroker.EventBroker
{
    /// <summary>
    /// Gets the full data of a person as a task
    /// </summary>
    public class GetPersonDataTask : Task
    {
        public string CprNumber;
        /// <summary>
        /// Catch the current context here to pass it later to the task execution thread
        /// </summary>
        public BrokerContext BrokerContext = BrokerContext.Current;


        public override bool Run()
        {
            CPRBroker.Schemas.QualityLevel? qualityLevel;

            // Set the current context to the context of the thread that created this task
            BrokerContext.Current = BrokerContext;

            var person = CPRBroker.Engine.Manager.Citizen.GetCitizenFull(this.BrokerContext.UserToken, this.BrokerContext.ApplicationToken, false, CprNumber, out qualityLevel);
            Result = person;
            return true;
        }
    }
}
