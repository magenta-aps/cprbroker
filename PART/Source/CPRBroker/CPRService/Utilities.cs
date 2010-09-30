using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace CPRService
{
    /// <summary>
    /// Contains utility methods that are used with pages
    /// </summary>
    public static class Utilities
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
