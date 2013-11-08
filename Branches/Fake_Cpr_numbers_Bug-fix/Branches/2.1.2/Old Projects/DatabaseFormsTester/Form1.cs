using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DatabaseFormsTester
{
    public partial class Databaseformstester : Form
    {
        public Databaseformstester()
        {
            InitializeComponent();
        }

        private void TestDatabaseForm_Click(object sender, EventArgs e)
        {
            var databaseForm = new CPRBroker.SetupDatabase.DatabaseForm();
            databaseForm.Show();
        }
    }
}
