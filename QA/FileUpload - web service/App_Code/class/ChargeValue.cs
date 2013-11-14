using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

/// <summary>
/// Summary description for ChargeValue
/// </summary>
public class ChargeValue
{
    public Int32 response { get; set; }
    public Decimal Charges { get; set; }
    public String msg { get; set; }
}
