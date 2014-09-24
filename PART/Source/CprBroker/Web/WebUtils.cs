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
using System.Web;
using System.Web.UI;

namespace CprBroker.Web
{
    public static class WebUtils
    {
        public static string UrlWithoutQuery(string url)
        {
            var full = url;
            var ind = full.IndexOf('?');
            var urlWithoutQuery = ind >= 0 ? full.Substring(0, ind) : full;
            return urlWithoutQuery;
        }

        public static string CreateLink(string name, string value, Uri baseUrl, params string[] namesToRemove)
        {
            var parameters = HttpUtility.ParseQueryString(baseUrl.Query);

            if (string.IsNullOrEmpty(value))
                parameters.Remove(name);
            else
                parameters[name] = value;

            foreach (var nameToRemove in namesToRemove)
                parameters.Remove(nameToRemove);

            var urlWithoutQuery = UrlWithoutQuery(baseUrl.ToString());
            if (parameters.Count > 0)
                return urlWithoutQuery + "?" + parameters.ToString();
            else
                return urlWithoutQuery;
        }

        public static T FromViewstate<T>(this StateBag ViewState, string key)
        {
            var obj = ViewState[key];
            if (obj == null)
                ViewState[key] = default(T);
            return (T)ViewState[key];
        }
    }
}