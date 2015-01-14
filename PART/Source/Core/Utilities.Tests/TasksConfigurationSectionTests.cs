using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Utilities.Config;

namespace CprBroker.Tests.Utilities
{
    [TestFixture]
    class TasksConfigurationSectionTests
    {
        [Test]
        public void Load_AddAndSave_OK()
        {
            var config = System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.None);
            var section = new TasksConfigurationSection();
            if (config.Sections[TasksConfigurationSection.SectionName] != null)
                config.Sections.Remove(TasksConfigurationSection.SectionName);

            section.KnownTypes.Add(
                new TasksConfigurationSection.TaskElement() { BatchSize = 100, Type = typeof(string), RunEvery = TimeSpan.FromMinutes(2) }
            );
            config.Sections.Add("tasks", section);
            config.Save();
        }
    }
}
