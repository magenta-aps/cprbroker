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
 * Niels Elgaard Larsen
 * Leif Lodahl
 * Steen Deth
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
using System.Xml.Serialization;

namespace CprBroker.Schemas.Part
{
    /// <summary>
    /// Represents an interface that all web method outputs should implement
    /// This is used to ensure that a StandardReturType is always returned from a web method
    /// </summary>
    public interface IBasicOutput
    {
        StandardReturType StandardRetur { get; set; }
    }

    public interface IBasicOutput<T> : IBasicOutput
    {
        void SetMainItem(T mainItem);
        T Item { get; set; }
    }

    public class BasicOutputType<T> : BasicOutputType, IBasicOutput<T>
    {
        public T Item { get; set; }
        public static BasicOutputType<T> CreateAsOK(T item)
        {
            return new BasicOutputType<T>()
            {
                Item = item,
                StandardRetur = StandardReturType.OK()
            };
        }

        public static BasicOutputType<T> CreateAsOKFromFirstResult(object[] result)
        {
            return CreateAsOK((T)result[0]);
        }

        public void SetMainItem(T mainItem)
        {
            Item = mainItem;
        }
    }

    public partial class SoegOutputType : IBasicOutput<string[]>
    {
        [XmlIgnore]
        public string[] Item
        {
            get { return Idliste; }
            set { Idliste = value; }
        }
        public void SetMainItem(string[] mainItem)
        {
            Idliste = mainItem;
        }
    }

    public partial class SoegListOutputType : IBasicOutput<LaesResultatType[]>
    {
        [XmlIgnore]
        public LaesResultatType[] Item
        {
            get { return LaesResultat; }
            set { LaesResultat = value; }
        }
        public void SetMainItem(LaesResultatType[] mainItem)
        {
            LaesResultat = mainItem;
        }
    }

    public partial class ListOutputType1 : IBasicOutput<LaesResultatType[]>
    {
        [XmlIgnore]
        public LaesResultatType[] Item
        {
            get { return LaesResultat; }
            set { LaesResultat = value; }
        }
        public void SetMainItem(LaesResultatType[] mainItem)
        {
            LaesResultat = mainItem;
        }
    }

    public partial class GetUuidOutputType : IBasicOutput<string>
    {
        [XmlIgnore]
        public string Item
        {
            get { return UUID; }
            set { UUID = value; }
        }
        public void SetMainItem(string mainItem)
        {
            UUID = mainItem;
        }
    }

    public partial class GetUuidArrayOutputType : IBasicOutput<string[]>
    {
        [XmlIgnore]
        public string[] Item
        {
            get { return UUID; }
            set { UUID = value; }
        }
        public void SetMainItem(string[] mainItem)
        {
            UUID = mainItem;
        }
    }

    public partial class LaesOutputType : IBasicOutput<LaesResultatType>
    {
        [XmlIgnore]
        public LaesResultatType Item
        {
            get { return LaesResultat; }
            set { LaesResultat = value; }
        }
        public void SetMainItem(LaesResultatType mainItem)
        {
            LaesResultat = mainItem;
        }
    }
}
