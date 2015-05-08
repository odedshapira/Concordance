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
using OdedShapira.Concordance.BLL;

public class BasePage : System.Web.UI.Page
{
    public enum MainMenuItem { Contexts, Index, WordSearch, Groups, Relations, LingualExpressions, Statistics };

    public virtual MainMenuItem? GetMainMenuItem()
    {
        return null;
    }

// 1
}

