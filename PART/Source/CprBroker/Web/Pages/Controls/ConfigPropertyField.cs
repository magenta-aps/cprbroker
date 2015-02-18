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