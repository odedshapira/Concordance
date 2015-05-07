using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using System.Web.Script.Services;
using OdedShapira.Concordance.BLL;
using System.Text;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[ScriptService()]
public class UtilsService : System.Web.Services.WebService
{
    public UtilsService()
    {
    }

    [WebMethod]
    public string HelloWorld()
    {
        return "Hello World";
    }

    [WebMethod]
    public string GetFileDetails(int fileID)
    {
        StringBuilder sb;
        BLOTextFile tf;

        sb = new StringBuilder();
        tf = BLOTextFile.Edit(fileID);
        sb.Append("<File>\r\n");
        sb.AppendFormat(" <ID>{0}</ID>\r\n", tf.ID);
        sb.AppendFormat(" <SongName>{0}</SongName>\r\n", tf.SongName);
        sb.AppendFormat(" <ArtistName>{0}</ArtistName>\r\n", tf.ArtistName);
        sb.AppendFormat(" <AlbumName>{0}</AlbumName>\r\n", tf.AlbumName);
        sb.AppendFormat(" <Year>{0}</Year>\r\n", tf.Year.HasValue ? tf.Year.Value.ToString() : "");
        sb.AppendFormat(" <SongWriters>{0}</SongWriters>\r\n", tf.SongWriters);
        sb.AppendFormat(" <Duration>{0}</Duration>\r\n", tf.Duration);
        sb.Append("</File>");
        return sb.ToString();
    }

    [WebMethod]
    public string DeleteFile(int fileID)
    {
        BLOTextFile tf;

        tf = BLOTextFile.Edit(fileID);
        try
        {
            System.IO.File.Delete(tf.GetFilePath(Server.MapPath("~/Files")));
        }
        catch
        {
            return null;
        }
        tf.Delete();
        return null;
    }

    [WebMethod(EnableSession = true)]
    public void SetFileSelection(int fileID, bool isChecked)
    {
        List<int> selectedFileIDs;

        selectedFileIDs = Utils.GetSelectedFileIDs();
        if (isChecked)
        {
            if (!selectedFileIDs.Contains(fileID))
                selectedFileIDs.Add(fileID);
        }
        else
            if (selectedFileIDs.Contains(fileID))
                selectedFileIDs.Remove(fileID);
    }

    [WebMethod(EnableSession = true)]
    public string GetFileSelectionWords()
    {
        StringBuilder sb;
        List<int> selectedFileIDs;
        string[] a;

        sb = new StringBuilder();
        selectedFileIDs = Utils.GetSelectedFileIDs();
        a = BLOWord.GetDistinct(selectedFileIDs.ToArray());
        sb.Append("<Words>");
        foreach (string s in a)
            sb.AppendFormat(" <Word>{0}</Word>", s);
        sb.Append("</Words>");
        return sb.ToString();
    }

    [WebMethod(EnableSession = true)]
    public string GetFileSelectionWordContexts()
    {
        StringBuilder sb;
        List<int> selectedFileIDs;
        string[] a;

        sb = new StringBuilder();
        selectedFileIDs = Utils.GetSelectedFileIDs();
        a = BLOWord.GetDistinct(selectedFileIDs.ToArray());
        sb.Append("<Words>");
        foreach (string s in a)
            sb.AppendFormat(" <Word>{0}</Word>", s);
        sb.Append("</Words>");
        return sb.ToString();
    }

    [WebMethod]
    public string GetFullText(int fileID, int contextID)
    {
        StringBuilder sb;
        BLOFullText.SearchDescriptor sd;
        BLOFullText[] a;

        sd = new BLOFullText.SearchDescriptor(fileID);
        a = BLOFullText.Get(sd);
        sb = new StringBuilder();
        sb.AppendFormat("<FullText FileID=\"{0}\">\r\n", fileID);
        foreach (BLOFullText ft in a)
        {
            sb.Append(" <Phrase>\r\n");
            sb.AppendFormat("  <WordID>{0}</WordID>\r\n", ft.WordID.HasValue ? ft.WordID.Value.ToString() : "");
            sb.AppendFormat("  <Text>{0}</Text>\r\n", ft.Phrase);
            sb.AppendFormat("  <IsContextWord>{0}</IsContextWord>\r\n", ((ft.PhraseType == "1") && (ft.WordID.HasValue) && (ft.WordID.Value == contextID)) ? "1" : "0");
            sb.Append(" </Phrase>\r\n");
        }
        sb.Append("</FullText>");
        return sb.ToString();
    }

    [WebMethod(EnableSession = true)]
    public string GetFileSelectionFullTexts()
    {
        StringBuilder sb;
        BLOFullText.SearchDescriptor sd;
        BLOFullText[] a;
        int currentFileID = -1;

        sd = new BLOFullText.SearchDescriptor(Utils.GetSelectedFileIDs().ToArray());
        a = BLOFullText.Get(sd);
        sb = new StringBuilder();
        sb.Append("<FullTexts>\r\n");
        foreach (BLOFullText ft in a)
        {
            if (currentFileID != ft.FileID)
            {
                if (currentFileID > 0)
                    sb.Append("</FullText>\r\n");
                sb.AppendFormat("<FullText FileID=\"{0}\" SongName=\"{1}\">\r\n", new object[] { ft.FileID, ft.SongName });
                currentFileID = ft.FileID;
            }
            sb.Append(" <Phrase>\r\n");
            sb.AppendFormat("  <FileID>{0}</FileID>\r\n", ft.FileID);
            sb.AppendFormat("  <WordID>{0}</WordID>\r\n", ft.WordID.HasValue ? ft.WordID.Value.ToString() : "");
            sb.AppendFormat("  <PhraseType>{0}</PhraseType>\r\n", ft.PhraseType);
            sb.AppendFormat("  <PhraseNumber>{0}</PhraseNumber>\r\n", ft.PhraseNumber);
            sb.AppendFormat("  <Text>{0}</Text>\r\n", ft.Phrase);
            sb.Append(" </Phrase>\r\n");
        }
        if (a.Length > 0)
            sb.Append("</FullText>");
        sb.Append("</FullTexts>");
        return sb.ToString();
    }

    [WebMethod]
    public void AddGroup(string groupName)
    {
        BLOGroup g;

        g = BLOGroup.Insert(groupName);
        g.Submit();
    }

    [WebMethod]
    public void EditGroup(int groupID, string groupName)
    {
        BLOGroup g;

        g = BLOGroup.Edit(groupID);
        g.Name = groupName;
        g.Submit();
    }

    [WebMethod]
    public void DeleteGroup(int groupID)
    {
        BLOGroup g;

        g = BLOGroup.Edit(groupID);
        g.Delete();
    }

    [WebMethod]
    public string GetGroupWords(int groupID)
    {
        StringBuilder sb;
        BLOGroup g;

        sb = new StringBuilder();
        g = BLOGroup.Edit(groupID);
        sb.Append("<GroupWords>\r\n");
        foreach (string word in g.Words)
            sb.AppendFormat(" <Word>{0}</Word>\r\n", word);
        sb.Append("</GroupWords>");
        return sb.ToString();
    }

    [WebMethod]
    public void SetGroupWords(int groupID, string value)
    {
        StringBuilder sb;
        BLOGroup g;

        sb = new StringBuilder();
        g = BLOGroup.Edit(groupID);
        g.Words.Clear();
        g.Words.AddRange(value.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries));
        g.Submit();
    }

    [WebMethod]
    public void AddRelation(string relationName)
    {
        BLORelation r;

        r = BLORelation.Insert(relationName);
        r.Submit();
    }

    [WebMethod]
    public void EditRelation(int relationID, string relationName)
    {
        BLORelation r;

        r = BLORelation.Edit(relationID);
        r.Name = relationName;
        r.Submit();
    }

    [WebMethod]
    public void DeleteRelation(int relationID)
    {
        BLORelation r;

        r = BLORelation.Edit(relationID);
        r.Delete();
    }

    [WebMethod]
    public string GetRelationWords(int relationID)
    {
        StringBuilder sb;
        BLORelation r;

        sb = new StringBuilder();
        r = BLORelation.Edit(relationID);
        sb.Append("<RelationWords>\r\n");
        foreach (BLORelation.WordsPair wordsPair in r.WordsPairs)
        {
            sb.Append(" <WordsPair>\r\n");
            sb.AppendFormat("  <Word1>{0}</Word1>\r\n", wordsPair.Word1);
            sb.AppendFormat("  <Word2>{0}</Word2>\r\n", wordsPair.Word2);
            sb.Append(" </WordsPair>\r\n");
        }
        sb.Append("</RelationWords>");
        return sb.ToString();
    }

    [WebMethod]
    public string GetRelationPairedWords(int relationID, string word)
    {
        StringBuilder sb;
        BLORelation r;

        sb = new StringBuilder();
        r = BLORelation.Edit(relationID);
        sb.Append("<PairedWords>\r\n");
        foreach (BLORelation.WordsPair wordsPair in r.WordsPairs)
        {
            if (wordsPair.Word1 == word)
                sb.AppendFormat(" <Word>{0}</Word>\r\n", wordsPair.Word2);
            if (wordsPair.Word2 == word)
                sb.AppendFormat(" <Word>{0}</Word>\r\n", wordsPair.Word1);
        }
        sb.Append("</PairedWords>");
        return sb.ToString();
    }

    [WebMethod]
    public void SetRelationWords(int relationID, string value)
    {
        StringBuilder sb;
        BLORelation r;
        string[] parts;

        sb = new StringBuilder();
        r = BLORelation.Edit(relationID);
        r.WordsPairs.Clear();
        foreach (string s in value.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries))
        {
            parts = s.Split(new string[] { "+" }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2)
                r.WordsPairs.Add(new BLORelation.WordsPair(parts[0], parts[1]));
        }
        r.Submit();
    }

    [WebMethod(EnableSession = true)]
    public string GetLingualExpressionRecurrences(string expression)
    {
        StringBuilder sb;
        BLOLingualExpressionRecurrence.SearchDescriptor sd;
        BLOLingualExpressionRecurrence[] a;

        sb = new StringBuilder();
        sd = new BLOLingualExpressionRecurrence.SearchDescriptor(expression);
        sd.FileIDs.AddRange(Utils.GetSelectedFileIDs());
        a = BLOLingualExpressionRecurrence.Get(sd);
        sb.Append("<LingualExpressionRecurrences>\r\n");
        foreach (BLOLingualExpressionRecurrence leo in a)
        {
            sb.Append(" <Recurrence>\r\n");
            sb.AppendFormat("  <FirstWordID>{0}</FirstWordID>\r\n", leo.FirstWordID);
            sb.AppendFormat("  <FileID>{0}</FileID>\r\n", leo.FileID);
            sb.AppendFormat("  <StartWordNumber>{0}</StartWordNumber>\r\n", leo.FileWordNumber);
            sb.AppendFormat("  <EndWordNumber>{0}</EndWordNumber>\r\n", leo.FileWordNumber + expression.Split(new char[] { ' ' } ).Length - 1);
            sb.Append(" </Recurrence>\r\n");
        }
        sb.Append("</LingualExpressionRecurrences>");
        return sb.ToString();
    }
}
