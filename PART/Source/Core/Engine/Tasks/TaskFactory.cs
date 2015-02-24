/* ***** BEGIN LICENSE BLOCK *****
 * Version: MPL 1.1/GPL 2.0/LGPL 2.1
 *
 * The contents of this file are subject to the Mozilla Public License
 * Version 1.1 (the "License"); you may not use this file except in
 * compliance with the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS"basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 * The CPR Broker concept was initally developed by
 * Gentofte Kommune / Municipality of Gentofte, Denmark.
 * Contributor(s):
 * Steen Deth
 *
 *
 * The Initial Code for CPR Broker and related components is made in
 * cooperation between Magenta, Gentofte Kommune and IT- og Telestyrelsen /
 * Danish National IT and Telecom Agency
 *
 * Contributor(s):
 * Beemen Beshara
 *
 * The code is currently governed by IT- og Telestyrelsen / Danish National
 * IT and Telecom Agency
 *
 * Alternatively, the contents of this file may be used under the terms of
 * either the GNU General Public License Version 2 or later (the "GPL"), or
 * the GNU Lesser General Public License Version 2.1 or later (the "LGPL"),
 * in which case the provisions of the GPL or the LGPL are applicable instead
 * of those above. If you wish to allow use of your version of this file only
 * under the terms of either the GPL or the LGPL, and not to allow others to
 * use your version of this file under the terms of the MPL, indicate your
 * decision by deleting the provisions above and replace them with the notice
 * and other provisions required by the GPL or the LGPL. If you do not delete
 * the provisions above, a recipient may use your version of this file under
 * the terms of any one of the MPL, the GPL or the LGPL.
 *
 * ***** END LICENSE BLOCK ***** */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Utilities.Config;

namespace CprBroker.Engine.Tasks
{
    /// <summary>
    /// Creates the tasks from configuration file
    /// It is highly recommended to handle the error events raised by this class
    /// </summary>
    public class TaskFactory
    {
        #region Error events
        public class ErrorEventArgs<T> : EventArgs
        {
            public T Object;
            public string Message;
        }

        public event EventHandler<ErrorEventArgs<TasksConfigurationSection>> ConfigSectionError;
        protected void OnConfigSectionError(TasksConfigurationSection section, string message)
        {
            if (this.ConfigSectionError != null)
            {
                var args = new ErrorEventArgs<TasksConfigurationSection>() { Object = section, Message = message };
                ConfigSectionError(this, args);
            }
        }

        public event EventHandler<ErrorEventArgs<TasksConfigurationSection.TaskElement>> TaskElementConfigError;
        protected void OnTaskElementConfigError(TasksConfigurationSection.TaskElement element, string message)
        {
            if (this.TaskElementConfigError != null)
            {
                var args = new ErrorEventArgs<TasksConfigurationSection.TaskElement>() { Object = element, Message = message };
                TaskElementConfigError(this, args);
            }
        }
        #endregion

        public PeriodicTaskExecuter CreateTask<T>(TasksConfigurationSection.TaskElement element) where T : PeriodicTaskExecuter
        {
            var task = Utilities.Reflection.CreateInstance<T>(element.Type);
            if (task != null)
            {
                // TODO: handle incorrect config here
                task.TimerInterval = element.RunEvery;
                task.BatchSize = element.BatchSize;
            }
            else
            {
                OnTaskElementConfigError(element, string.Format("Invalid task type <{0}>", element.TypeName));
            }
            return task;
        }

        public virtual TasksConfigurationSection.TaskElement[] LoadTaskConfigElements()
        {
            var section = LoadTasksSection();
            if (section != null)
            {
                var autoLoaded = section.AutoLoaded;
                if (autoLoaded != null)
                {
                    return autoLoaded
                        .OfType<TasksConfigurationSection.TaskElement>()
                        .Where(elm => elm != null)
                        .ToArray();
                }
                else
                {
                    OnConfigSectionError(section, "Tasks configuration section is missing autoLoaded section");
                }
            }
            else
            {
                OnConfigSectionError(section, "Tasks configuration section not found!!");
            }
            return new TasksConfigurationSection.TaskElement[0];
        }

        public virtual TasksConfigurationSection LoadTasksSection()
        {
            var section = CprBroker.Utilities.Config.ConfigManager.Current.TasksSection;
            return section;
        }

        public PeriodicTaskExecuter[] LoadTasks()
        {
            var ret = new List<PeriodicTaskExecuter>();
            foreach (var element in LoadTaskConfigElements())
            {
                var task = CreateTask<PeriodicTaskExecuter>(element);
                if (task != null)
                    ret.Add(task);
                else
                    this.OnTaskElementConfigError(element, string.Format("Task creation failed from typeName <{0}>", element.TypeName));
            }
            return ret.ToArray();
        }
    }
}
