using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CvrDemo.Data;

namespace CvrDemo.Pages
{
    public partial class Search : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void SearchButton_Click(object sender, EventArgs e)
        {
            using (var dataContext = new CvrDataContext())
            {
                LegalUnit unit;
                decimal cvrNum;
                if (Decimal.TryParse(CvrSearchBox.Text, out cvrNum))
                {
                    unit = dataContext.Units.Where(u => u.LegalUnitIdentifier == cvrNum && u.ProductionUnitIdentifier == 0).FirstOrDefault() as LegalUnit;

                    if (unit != null)
                    {
                        var prod = unit.ProductionUnits.Where(pU => pU != null);
                        foreach (var pU in prod)
                        {
                            object o = pU.Name;
                        }

                        var owner = unit.Owners.Where(o => o != null);
                        foreach (var ow in owner)
                        {
                            object o = ow.Ajourføringsmarkering;
                        }
                    }
                    else
                    {
                        unit = new LegalUnit();
                        unit.Name = " CVR-nummer ukendt";
                        unit.TelephoneNumberIdentifier = "";
                    }
                }
                else
                {
                    unit = new LegalUnit();
                    unit.Name = " Forkert input!";
                    unit.TelephoneNumberIdentifier = "";
                }
                frmLegalUnit.DataSource = new LegalUnit[] { unit };
                frmLegalUnit.DataBind();
            }
        }
    }
}