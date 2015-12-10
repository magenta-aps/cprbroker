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
using System.Web.UI.WebControls;
using CprBroker.Engine;
using CprBroker.Data;

namespace CprBroker.Web.Controls
{
    public class ConfigPropertyField : BoundField
    {
        public event Func<IHasEncryptedAttributes, IHasConfigurationProperties> ObjectCreating;

        protected override DataControlField CreateField()
        {
            return new ConfigPropertyField();
        }

        public override void InitializeCell(DataControlFieldCell cell, DataControlCellType cellType, DataControlRowState rowState, int rowIndex)
        {
            string path = null;
            if (rowIndex != -1)
            {
                if ((rowState & DataControlRowState.Edit) > 0)
                {
                    path = "~/Pages/Controls/ConfigPropertyEditor.ascx";
                }
                else if (rowState == DataControlRowState.Normal || rowState == DataControlRowState.Alternate)
                {
                    path = "~/Pages/Controls/ConfigPropertyViewer.ascx";
                }
            }

            //if (controlType != null)
            if (path != null)
            {
                //UserControl ctl = (HttpContext.Current.Handler as Page).LoadControl(controlType, null) as UserControl;
                UserControl ctl = (HttpContext.Current.Handler as Page).LoadControl(path) as UserControl;
                ctl.DataBinding += new EventHandler(ctl_DataBinding);


                cell.Controls.Add(ctl);
            }
            else
            {
                base.InitializeCell(cell, cellType, rowState, rowIndex);
            }
        }

        void ctl_DataBinding(object sender, EventArgs e)
        {
            var ctl = sender as IControlWithDataSource;
            var dbObject = DataBinder.GetDataItem(ctl.NamingContainer) as IHasEncryptedAttributes;
            if (ObjectCreating != null)
            {
                ctl.DataSource = ObjectCreating(dbObject).ToDisplayableProperties();
            }
        }

        public override void ExtractValuesFromCell(System.Collections.Specialized.IOrderedDictionary dictionary, DataControlFieldCell cell, DataControlRowState rowState, bool includeReadOnly)
        {
            if (cell.Controls.Count > 0)
            {
                var grd = cell.NamingContainer.NamingContainer as GridView;
                if (grd != null)
                {
                    var ds = grd.DataSourceObject as LinqDataSource;
                    ds.Updating += new EventHandler<LinqDataSourceUpdateEventArgs>(ds_Updating);
                }
            }
        }

        void ds_Updating(object sender, LinqDataSourceUpdateEventArgs e)
        {
            var grd = this.Control as GridView;
            var editor = grd.Rows[grd.EditIndex].Cells[grd.Columns.IndexOf(this)].Controls[0] as ConfigPropertyEditor;
            
            var originalObj = e.OriginalObject as IHasEncryptedAttributes;
            var newObj = e.NewObject as IHasEncryptedAttributes;
            originalObj.PreLoadAttributes();
            
            // First, fill with original attributes, to avoid clearing passwords
            newObj.SetAll(originalObj.ToPropertiesDictionary());

            // Now update with the new values
            newObj.SetAll(editor.ToDictionary());
        }
    }
}