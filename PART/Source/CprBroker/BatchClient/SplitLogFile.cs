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

namespace BatchClient
{
    class SplitLogFile
    {
        static int Code = -1;

        static void Run()
        {
            var th = new System.Threading.Thread(new System.Threading.ThreadStart(SplitLogFile));
            th.Start();

            Code = Console.Read();
        }

        static void SplitLogFile()
        {
            string path = "C:\\Cpr Broker.log";
            int partSize = 10 * 1024 * 1024;

            int fileCount = (int)Math.Ceiling(new FileInfo(path).Length / (double)partSize);
            var locations = new List<long>();
            using (FileStream s = new FileStream(path, FileMode.Open, FileAccess.ReadWrite))
            {
                var size = s.Length;
                long newSize = 0;
                var partBuffer = new byte[partSize];
                while (true)
                {
                    int readCount = s.Read(partBuffer, 0, partSize);
                    var b = s.ReadByte();
                    while (b != 10 && b != 13 && b >= 0)
                    {
                        b = s.ReadByte();
                    }

                    while (b == 10 || b == 13)
                    {
                        b = s.ReadByte();
                    }
                    if (b != -1)
                    {
                        s.Seek(-1, SeekOrigin.Current);
                        locations.Add(s.Position);
                        Console.WriteLine("Identified segment <{0}> of about <{1}>", locations.Count, s.Length / partSize);
                    }
                    else
                    {
                        break;
                    }
                }
                locations.Insert(0, 0);
                locations.Add(s.Length);

                for (int iFile = locations.Count - 2; iFile >= 0; iFile--)
                {
                    string partFileName = string.Format("c:\\Log\\CprBroker.{0}.log", iFile);
                    if (File.Exists(partFileName))
                        throw new Exception("File exists");

                    Console.WriteLine("Writing file <{0}>. Press ENTER to stop after this file", partFileName);

                    long partStartPosition = locations[iFile];
                    long partLength = locations[iFile + 1] - partStartPosition;
                    var data = new byte[partLength];

                    s.Seek(partStartPosition, SeekOrigin.Begin);
                    var c = s.Read(data, 0, data.Length);
                    newSize += c;

                    File.WriteAllBytes(partFileName, data);
                    s.SetLength(partStartPosition);
                    s.Flush();

                    if (Code == -1)
                    {
                        System.Threading.Thread.Sleep(500);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            Console.WriteLine("Done !!");
            Console.WriteLine("Press ENTER to exit");

        }
    }
}
