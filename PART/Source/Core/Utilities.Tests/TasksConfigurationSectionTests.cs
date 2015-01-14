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
            System.Diagnostics.Debugger.Launch();
            var config = System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.None);
            var section = new TasksConfigurationSection();
            if (config.Sections[TasksConfigurationSection.SectionName] != null)
                config.Sections.Remove(TasksConfigurationSection.SectionName);

            section.KnownTypes.Add(new TasksConfigurationSection.TaskElement() { BatchSize = 100, Type = typeof(string), RunEvery = TimeSpan.FromMinutes(2) });
            section.KnownTypes.Add(new TasksConfigurationSection.TaskElement() { Type = typeof(object) });
            config.Sections.Add(TasksConfigurationSection.SectionName, section);
            config.Save();
        }

        [Test]
        public void New_NoBatchSize_100()
        {
            var last = new TasksConfigurationSection.TaskElement();
            var b = last.BatchSize;
            Assert.AreEqual(100, b);
        }

        [Test]
        public void New_NoInterval_OneMinute()
        {
            var last = new TasksConfigurationSection.TaskElement();
            var b = last.RunEvery;
            Assert.AreEqual(TimeSpan.FromMinutes(1), b);
        }
    }
}
