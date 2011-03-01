using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
namespace CprBroker.Utilities
{
    public static class Web
    {
        public static T ObjectFromViewState<T>(StateBag bag, string key)
        {
            return ObjectFromViewState<T>(bag, key, default(T));
        }

        public static T ObjectFromViewState<T>(StateBag bag, string key, T defaultValue)
        {
            object ret = bag[key];
            if (ret == null || !(ret is T))
            {
                return defaultValue;
            }
            else
            {
                return (T)ret;
            }
        }
    }
}
