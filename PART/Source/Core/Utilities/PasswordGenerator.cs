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
 * Mathias Dam Hedelund
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

namespace CprBroker.Utilities
{
    public class PasswordGenerator
    {
        private int minimumLength;
        private int maximumLength;
        private int minimumNumberOfLowercaseChars;
        private int minimumNumberOfUppercaseChars;
        private int minimumNumberOfNumericDigits;
        private int minimumNumberOfSymbols;
        private bool allowRepeatingCharacters;
        private bool allowConsecutiveCharacters;
        private bool includeSymbols;
        private string excludedCharacters;
        private char[][] charArrays;
        private RNGCryptoServiceProvider rng;

        private const int DefaultMinimum = 6;
        private const int DefaultMaximum = 12;
        private const string DefaultLowercaseChars = "abcdefghijklmnopqrstuvwxyzæøå";
        private const string DefaultUppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZÆØÅ";
        private const string DefaultNumericDigits = "0123456789";
        private const string DefaultSymbols = "`~!@#$%^&*()-_=+[]{}\\|;:'\",<.>/?";

        public PasswordGenerator()
        {
            this.MinimumLength = PasswordGenerator.DefaultMinimum;
            this.MaximumLength = PasswordGenerator.DefaultMaximum;
            this.MinimumNumberOfLowercaseChars = 1;
            this.MinimumNumberOfUppercaseChars = 1;
            this.MinimumNumberOfNumericDigits = 1;
            this.MinimumNumberOfSymbols = 1;
            this.AllowRepeatingCharacters = true;
            this.AllowConsecutiveCharacters = false;
            this.IncludeSymbols = true;
            this.ExcludedCharacters = null;
            this.charArrays = new char[4][];
            this.charArrays[0] = PasswordGenerator.DefaultLowercaseChars.ToCharArray();
            this.charArrays[1] = PasswordGenerator.DefaultUppercaseChars.ToCharArray();
            this.charArrays[2] = PasswordGenerator.DefaultNumericDigits.ToCharArray();
            this.charArrays[3] = PasswordGenerator.DefaultSymbols.ToCharArray();
            this.rng = new RNGCryptoServiceProvider();
        }

        private int GenerateRandomNumber(int lowerBound, int upperBound)
        {
            uint urndnum;
            byte[] rndnum = new Byte[4];

            if (lowerBound == upperBound - 1)
            {
                return lowerBound;
            }

            uint excludeRndBase = (uint.MaxValue - (uint.MaxValue % (uint)(upperBound - lowerBound)));

            do
            {
                rng.GetBytes(rndnum);
                urndnum = BitConverter.ToUInt32(rndnum, 0);
            } while (urndnum >= excludeRndBase);

            return (int)(urndnum % (upperBound - lowerBound)) + lowerBound;
        }

        private char GetRandomCharacter(char[] charArray)
        {
            int randomCharIndex = GenerateRandomNumber(charArray.GetLowerBound(0), charArray.GetUpperBound(0) + 1);
            return charArray[randomCharIndex];
        }

        public string Generate()
        {
            int passwordLength = GenerateRandomNumber(this.MinimumLength, this.MaximumLength);
            char[] passwordBuffer = null;
            int[] timesUsedCharArrays = new int[charArrays.Length];
            int charArrayUpperBound = this.IncludeSymbols ? charArrays.GetUpperBound(0) + 1 : charArrays.GetUpperBound(0);

            char prevCharacter = 'a';
            char nextCharacter = 'a';

            while (timesUsedCharArrays[0] < this.MinimumNumberOfLowercaseChars
                || timesUsedCharArrays[1] < this.MinimumNumberOfUppercaseChars
                || timesUsedCharArrays[2] < this.MinimumNumberOfNumericDigits
                || (this.IncludeSymbols && timesUsedCharArrays[3] < this.MinimumNumberOfSymbols))
            {
                passwordBuffer = new char[passwordLength];
                timesUsedCharArrays = new int[charArrays.Length];

                for (int i = 0; i < passwordLength; i++)
                {
                    int charArrayIndex = GenerateRandomNumber(charArrays.GetLowerBound(0), charArrayUpperBound);
                    timesUsedCharArrays[charArrayIndex]++;
                    nextCharacter = GetRandomCharacter(charArrays[charArrayIndex]);

                    if (!this.AllowConsecutiveCharacters)
                    {
                        while (prevCharacter == nextCharacter)
                        {
                            nextCharacter = GetRandomCharacter(charArrays[charArrayIndex]);
                        }
                    }

                    if (!this.AllowRepeatingCharacters)
                    {
                        string tempBuffer = passwordBuffer.ToString();
                        int duplicateIndex = tempBuffer.IndexOf(nextCharacter);
                        while (duplicateIndex != -1)
                        {
                            nextCharacter = GetRandomCharacter(charArrays[charArrayIndex]);
                            duplicateIndex = tempBuffer.IndexOf(nextCharacter);
                        }
                    }

                    if (this.ExcludedCharacters != null) // Assumes that not all chararacters in this char array have been excluded
                    {
                        while (this.ExcludedCharacters.IndexOf(nextCharacter) != -1)
                        {
                            nextCharacter = GetRandomCharacter(charArrays[charArrayIndex]);
                        }
                    }

                    passwordBuffer[i] = nextCharacter;
                    prevCharacter = nextCharacter;
                }
            }

            rng.Dispose();

            if (passwordBuffer != null)
            {
                return new string(passwordBuffer);
            }
            else
            {
                return String.Empty;
            }

        }

        public int MinimumLength
        {
            get { return this.minimumLength; }
            set { this.minimumLength = value > PasswordGenerator.DefaultMinimum ? value : PasswordGenerator.DefaultMinimum; }
        }

        public int MaximumLength
        {
            get { return this.maximumLength; }
            set { this.maximumLength = value < PasswordGenerator.DefaultMaximum ? value : PasswordGenerator.DefaultMaximum; }
        }

        public int MinimumNumberOfLowercaseChars
        {
            get { return this.minimumNumberOfLowercaseChars; }
            set { this.minimumNumberOfLowercaseChars = value; }
        }

        public int MinimumNumberOfUppercaseChars
        {
            get { return this.minimumNumberOfUppercaseChars; }
            set { this.minimumNumberOfUppercaseChars = value; }
        }

        public int MinimumNumberOfNumericDigits
        {
            get { return this.minimumNumberOfNumericDigits; }
            set { this.minimumNumberOfNumericDigits = value; }
        }

        public int MinimumNumberOfSymbols // Ignored if this.IncludeSymbols is false
        {
            get { return this.minimumNumberOfSymbols; }
            set { this.minimumNumberOfSymbols = value; }
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

        public string ExcludedCharacters
        {
            get { return this.excludedCharacters; }
            set { this.excludedCharacters = value; }
        }

        public bool IncludeSymbols
        {
            get { return this.includeSymbols; }
            set { this.includeSymbols = value; }
        }
    }
}
