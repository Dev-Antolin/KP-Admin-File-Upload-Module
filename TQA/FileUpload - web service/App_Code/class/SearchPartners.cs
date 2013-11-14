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
/// Summary description for SearchPartners
/// </summary>
public class SearchPartners
{
    public Int32 respcode { get; set; }
    public String message { get; set; }
    public String batchNo { get; set; }
    public String ErrorDetail { get; set; }
    public DateTime getDate { get; set; }
    public getArrayClass[] Data { get; set; }
    public String[] ReturnData { get; set; }
    
 }
