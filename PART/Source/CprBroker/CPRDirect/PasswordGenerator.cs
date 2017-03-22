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
 * Dennis Isaksen
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
using System.Security.Cryptography;

namespace CprBroker.Providers.CPRDirect
{
    class PasswordGenerator
    {
        // Rules for passwords according to CPR Services as of 21.03.2017 15:39:
        // Minimum 1 non-capitalized letter (a-z)
        // Minimum 1 capitalized letter (A-Z)
        // Minimum 1 numeric digit (0-9)
        // Minimum 1 special character ~ ` ! @ # $ % ^ * ( ) _ - + = , . / \ { } [ ] ; :
        // Prohibited characters: " ' < > & ? æ ø å

        private int minimumLength;
        private int maximumLength;
        private int minimumNumberOfNonCapitalizedChars;
        private int minimumNumberOfCapitalizedChars;
        private int minimumNumberOfNumericDigits;
        private int minimumNumberOfSpecialChars;
        private bool allowRepeatingCharacters;
        private bool allowConsecutiveCharacters;
        private bool includeSymbols;
        private string excludedCharacters;
        private char[] charArray;
        private RNGCryptoServiceProvider rng;

        private const int DefaultMinimum = 6;
        private const int DefaultMaximum = 12;
        private const string DefaultChars = "abcdefghijklmnopqrstuvwxyzæøåABCDEFGHIJKLMNOPQRSTUVWXYZÆØÅ0123456789`~!@#$%^&*()-_=+[]{}\\|;:'\",<.>/?";

        public PasswordGenerator()
        {
            this.MinimumLength = PasswordGenerator.DefaultMinimum;
            this.MaximumLength = PasswordGenerator.DefaultMaximum;
            this.MinimumNumberOfNonCapitalizedChars = 1;
            this.MinimumNumberOfCapitalizedChars = 1;
            this.MinimumNumberOfNumericDigits = 1;
            this.MinimumNumberOfSpecialChars = 1;
            this.AllowRepeatingCharacters = true;
            this.AllowConsecutiveCharacters = false;
            this.IncludeSymbols = true;
            this.ExcludedCharacters = null;
            this.charArray = PasswordGenerator.DefaultChars.ToCharArray();
            this.rng = new RNGCryptoServiceProvider();
        }


        private int GenerateRandomNumber(int lowerBound, int upperBound)
        {
            byte[] randomNumber = new byte[1];
            do
            {
                // Fill the array with a random value.
                rng.GetBytes(randomNumber);
            }
            while (// Not in range);
        }

        public string Generate()
        {
            string password;

            rng.Dispose();
            return password;
        }

        public int MinimumLength
        {
            get { return this.minimumLength; }
            set { this.minimumLength = value > PasswordGenerator.DefaultMinimum ? value: PasswordGenerator.DefaultMinimum; }
        }

        public int MaximumLength
        {
            get { return this.maximumLength; }
            set { this.maximumLength = value < PasswordGenerator.DefaultMaximum ? value : PasswordGenerator.DefaultMaximum; }
        }

        public int MinimumNumberOfNonCapitalizedChars
        {
            get { return this.minimumNumberOfNonCapitalizedChars; }
            set { this.minimumNumberOfNonCapitalizedChars = value; }
        }

        public int MinimumNumberOfCapitalizedChars
        {
            get { return this.minimumNumberOfCapitalizedChars; }
            set { this.minimumNumberOfCapitalizedChars = value; }
        }

        public int MinimumNumberOfNumericDigits
        {
            get { return this.minimumNumberOfNumericDigits; }
            set { this.minimumNumberOfNumericDigits = value; }
        }

        public int MinimumNumberOfSpecialChars
        {
            get { return this.minimumNumberOfSpecialChars; }
            set { this.minimumNumberOfSpecialChars = value; }
        }

        public bool AllowRepeatingCharacters
        {
            get { return this.allowRepeatingCharacters; }
            set { this.allowRepeatingCharacters = value; }
        }

        public bool AllowConsecutiveCharacters
        {
            get { return this.allowConsecutiveCharacters; }
            set { this.allowConsecutiveCharacters = value; }
        }

        public bool IncludeSymbols
        {
            get { return this.includeSymbols; }
            set { this.includeSymbols = value; }
        }

        public string ExcludedCharacters
        {
            get { return this.excludedCharacters; }
            set { this.excludedCharacters = value; }
        }
    }
}
