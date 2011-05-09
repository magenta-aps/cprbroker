﻿/*
 * Copyright (c) 2011 Danish National IT and Tele agency / IT- og Telestyrelsen

* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:

* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.

* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CPR_Business_Application_Demo.Business;

namespace CPR_Business_Application_Demo
{
    public partial class NotificationsForm : Form
    {
        public NotificationsForm()
        {
            InitializeComponent();
        }


        /// <summary>
        /// This is very basic. Just show the cpr and name of the person.
        /// </summary>
        /// <param name="notifications"></param>
        public void AddNotification(List<NotificationPerson> notifications)
        {
            foreach (var notificationPerson in notifications)
            {
                NotificationsText.Text += notificationPerson.Cpr + ": "
                                       + notificationPerson.FirstName + " "
                                       + notificationPerson.LastName
                                       + "\r\n";
            }
            
        }

        private void ClearNotificationsButton_Click(object sender, EventArgs e)
        {
            NotificationsText.Clear();
        }
    }
}
