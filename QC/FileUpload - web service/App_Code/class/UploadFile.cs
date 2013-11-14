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
using System.Collections.Generic;

/// <summary>
/// Summary description for UploadFile
/// </summary>
public class UploadFile
{
    public Int32 response { get; set; }
    public String msg { get; set; }
    public String BatchNumber { get; set; }
    public List<string> data { get; set; }
    public List<string> array { get; set; }
    public String session { get; set; }
   
}
