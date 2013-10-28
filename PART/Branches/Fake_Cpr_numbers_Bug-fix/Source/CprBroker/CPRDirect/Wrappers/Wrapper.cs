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
using System.IO;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.CPRDirect
{
    public abstract class Wrapper
    {
        public Wrapper()
        {
            _Contents = new string(' ', Length);
        }

        public Wrapper(int length)
        {
            _Contents = new string(' ', length);
        }

        private string _Contents;
        public string Contents
        {
            get { return _Contents; }
            set
            {
                int len = string.Format("{0}", value).Length;
                if (Length > 0 && len != Length)
                {
                    throw new ArgumentOutOfRangeException(
                        "Contents",
                        value,
                        string.Format("Should be exactly {0} characters", Length)
                        );
                }
                _Contents = value;
            }
        }

        public abstract int Length { get; }

        public string Code
        {
            get { return Contents.Substring(0, 3); }
        }

        public int IntCode
        {
            get { return int.Parse(Code); }
        }

        private string this[int pos, int length]
        {
            get
            {
                int index = pos - 1;

                if (this.Length == 0)
                    length = Math.Min(length, Contents.Length - index);

                return Contents.Substring(index, length);
            }
            set
            {
                if (value.Length != length)
                {
                    throw new ArgumentOutOfRangeException("value", string.Format("Should be exactly <{0}> characters", length));
                }
                if (this.Length > 0 && pos + length - 1 > this.Length)
                {
                    throw new ArgumentOutOfRangeException("pos,length", string.Format("Should sum to less than or equal to <{0}>", Length));
                }
                int startIndex = pos - 1;
                var charArr =
                    Contents.Take(startIndex)
                    .Concat(value)
                    .Concat(
                        _Contents.Skip(startIndex + length)
                    )
                    .ToArray();
                _Contents = new string(charArr);
            }
        }

        public string GetString(int pos, int len)
        {
            var ret = this[pos, len];
            return ret.Trim();
        }

        public void SetString(string value, int pos, int len)
        {
            value = string.Format("{0}", value);
            if (value.Length < len)
            {
                value = value + new string(' ', len - value.Length);
            }
            this[pos, len] = value;
        }

        public char GetChar(int pos)
        {
            return this[pos, 1][0];
        }

        public void SetChar(char value, int pos)
        {
            this[pos, 1] = value.ToString();
        }

        public decimal GetDecimal(int pos, int length)
        {
            return decimal.Parse(this[pos, length]);
        }

        public void SetDecimal(decimal value, int pos, int length)
        {
            this[pos, length] = Converters.DecimalToString(value, length);
        }

        public DateTime? GetDateTime(int pos, int length, string format)
        {
            DateTime ret;
            List<string> formats = new List<string>();
            formats.Add(format);
            if (format.Contains("mm"))
            {
                formats.Add(format.Replace("mm", "99"));
            }

            if (DateTime.TryParseExact(this[pos, length], formats.ToArray(), null, System.Globalization.DateTimeStyles.None, out ret))
            {
                return ret;
            }
            else
            {
                return null;
            }
        }

        public void SetDateTime(DateTime? value, int pos, int length, string format)
        {
            if (value.HasValue)
            {
                this[pos, length] = value.Value.ToString(format);
            }
            else
            {
                this[pos, length] = new string(' ', length);
            }
        }
    }
}
