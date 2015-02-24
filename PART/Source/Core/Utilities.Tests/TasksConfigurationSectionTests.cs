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
            if (config.Sections[TasksConfigurationSection.SectionName] != null)
                config.Sections.Remove(TasksConfigurationSection.SectionName);

            var section = new TasksConfigurationSection();
            section.AutoLoaded.Add(new TasksConfigurationSection.TaskElement() { BatchSize = 100, Type = typeof(string), RunEvery = TimeSpan.FromMinutes(2) });
            section.AutoLoaded.Add(new TasksConfigurationSection.TaskElement() { Type = typeof(object) });
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

        [Test]
        public void ImportDiffFrom_MixedNewAndExisting_NewAdded()
        {
            var existing = new TasksConfigurationSection();
            existing.AutoLoaded.Add(new TasksConfigurationSection.TaskElement() { Type = typeof(string) });

            var newSection = new TasksConfigurationSection();
            newSection.AutoLoaded.Add(new TasksConfigurationSection.TaskElement() { Type = typeof(string) });
            newSection.AutoLoaded.Add(new TasksConfigurationSection.TaskElement() { Type = typeof(object) });

            Assert.AreEqual(1, existing.AutoLoaded.Count);
            existing.AutoLoaded.ImportDiffFrom(newSection);
            Assert.AreEqual(2, existing.AutoLoaded.Count);
        }

        [Test]
        public void ImportDiffFrom_FakeExistinggOneNew_NewAdded()
        {
            var existing = new TasksConfigurationSection();
            existing.AutoLoaded.Add(new TasksConfigurationSection.TaskElement() { TypeName = "Fake type" });

            var newSection = new TasksConfigurationSection();
            newSection.AutoLoaded.Add(new TasksConfigurationSection.TaskElement() { Type = typeof(string) });
            newSection.AutoLoaded.Add(new TasksConfigurationSection.TaskElement() { Type = typeof(object) });

            Assert.AreEqual(1, existing.AutoLoaded.Count);
            existing.AutoLoaded.ImportDiffFrom(newSection);
            Assert.AreEqual(3, existing.AutoLoaded.Count);
        }
    }
}
