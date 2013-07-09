using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Installers;

namespace CprBroker.Tests.Installers
{
    [TestFixture]
    class DatabaseSetupInfoTests
    {
        class DatabaseSetupInfoStub : DatabaseSetupInfo
        {
            public bool _AppLoginExists = false;
            public bool _DatabaseExists = false;
            public Dictionary<string,List<string>> _DatabaseRoleMembers = new Dictionary<string,List<string>>();
            public bool _IsIntegratedSecurityOnly = false;
            public Dictionary<string,List<string>> _ServerRoleMembers = new Dictionary<string,List<string>>();
            public bool _TryOpenConnection = false;
            public bool _ValidateEncryptionKey = false;

            protected override bool ValidateEncryptionKey(ref string message)
            {
                return _ValidateEncryptionKey;
            }
        }

        string _Message;
        [SetUp]
        public void Initialize()
        {
            _Message = null;
        }

        [Test]
        public void Validate_EncryptionEnabled_ReturnsFalse()
        {
            var info = new DatabaseSetupInfoStub() { EncryptionKeyEnabled = true, _ValidateEncryptionKey = false };
            var result = info.Validate(ref _Message);
            Assert.False(result);
        }


    }
}
