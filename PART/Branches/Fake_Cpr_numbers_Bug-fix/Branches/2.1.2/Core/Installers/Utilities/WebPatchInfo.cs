using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Installers
{
    public class WebPatchInfo : PatchInfo
    {
        public Action PatchAction;

        public static WebPatchInfo Merge(WebPatchInfo[] webPatchInfo, Version oldVersion)
        {
            var relevant = Filter<WebPatchInfo>(webPatchInfo, oldVersion);
            var actions = relevant.Where(inf => inf.PatchAction != null)
                .Select(inf => inf.PatchAction)
                .ToArray();

            return new WebPatchInfo()
            {
                Version = oldVersion,
                PatchAction = () => Array.ForEach<Action>(actions, act => act())
            };
        }
    }
}
