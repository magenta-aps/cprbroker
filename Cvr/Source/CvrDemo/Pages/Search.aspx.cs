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
                var cvrNum = decimal.Parse(CvrSearchBox.Text);
                var unit = dataContext.Units.Where(u => u.LegalUnitIdentifier == cvrNum).FirstOrDefault() as LegalUnit;

                var prod = unit.ProductionUnits.Where(pU => pU != null);
                foreach (var pU in prod)
                {
                    object o = pU.Name;
                }
                /*
                var legalUnit = new LegalUnit()
                {
                    Name = "JHJJKHJKHJ",
                    StartDate = 20050601,
                    ProductionUnits = new System.Data.Linq.EntitySet<ProductionUnit>()
                };
                legalUnit.ProductionUnits.Add(new ProductionUnit()
                {
                    Name = "sgfæjglæsdglæ",
                    StartDate = 20050601,

                });
                legalUnit.ProductionUnits.Add(new ProductionUnit()
                {
                    Name = "Production 2",
                    StartDate = 20050601,
                });
                 */
                frmLegalUnit.DataSource = new LegalUnit[] { unit };
                frmLegalUnit.DataBind();
            }
        }
    }
}