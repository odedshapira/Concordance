using System;
using System.Collections.Generic;
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
using System.Text;
using OdedShapira.Concordance.BLL;

public class Utils
{
    public static List<int> GetDisplayedFileIDs()
    {
        List<int> displayedFileIDs;

        displayedFileIDs = HttpContext.Current.Session["DisplayedFileIDs"] as List<int>;
        if (displayedFileIDs == null)
        {
            displayedFileIDs = new List<int>();
            foreach (BLOTextFile tf in BLOTextFile.Get(new BLOTextFile.SearchDescriptor()))
                displayedFileIDs.Add(tf.ID);
            HttpContext.Current.Session["DisplayedFileIDs"] = displayedFileIDs;
        }
        return displayedFileIDs;
    }

    public static List<int> GetSelectedFileIDs()
    {
        List<int> selectedFileIDs;

        selectedFileIDs = HttpContext.Current.Session["SelectedFileIDs"] as List<int>;
        if (selectedFileIDs == null)
        {
            selectedFileIDs = new List<int>();
            HttpContext.Current.Session["SelectedFileIDs"] = selectedFileIDs;
        }
        return selectedFileIDs;
    }

    public static string SelectedWord
    {
        get
        {
            if (HttpContext.Current.Session["SelectedWord"] == null)
                return null;
            return HttpContext.Current.Session["SelectedWord"].ToString();
        }
        set
        {
            HttpContext.Current.Session["SelectedWord"] = value;
        }
    }

    public string[] IntArrayToStringArray(int[] a)
    {
        List<string> l;

        l = new List<string>();
        foreach (int i in a)
            l.Add(i.ToString());
        return l.ToArray();
    }

    public static string Concat(string[] a)
    {
        StringBuilder sb;

        sb = new StringBuilder();
        foreach (string s in a)
        {
            if (sb.Length > 0)
                sb.Append(",");
            sb.Append("'" + s.Replace("'", "''") + "'");
        }
        return sb.ToString();
    }

    public static string FormatCurrency(double currency)
    {
        return String.Format("{0:0.00}", currency);
    }
}
