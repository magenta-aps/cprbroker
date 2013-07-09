using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Installers
{
    public class PatchInfo
    {
        public Version Version;

        public static T[] Filter<T>(T[] info, Version oldVersion) where T : PatchInfo
        {
            var relevant = info
                .Where(inf => oldVersion == null || inf.Version >= oldVersion)
                .OrderBy(inf => inf.Version)
                .ToArray();
            return relevant;
        }
    }
}
