using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EComerceProject.Libraries
{
  public class XMLHelper
  {
    public static XMLWriteOperations runTimeXMl;
    public static string dateTimeStamp;
    public static string runTimeXmlFileName = "RunTimeTestData.xml";
    //RunTimeTestData.xml
    public static XMLWriteOperations RunTimeXMl
    {
      get
      {
        if (runTimeXMl == null)
        {
          runTimeXMl = new XMLWriteOperations(Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory))).Replace("bin", "") + string.Format("Test\\TestData\\RunTimeTestData\\" + runTimeXmlFileName));
        }
        return runTimeXMl;
      }
    }

  }

  public class XMLWriteOperations
  {
    private readonly string fileName = string.Empty;
    XmlDocument xmlDoc = null;
    XmlNode rootNode = null;
    public XMLWriteOperations(string fileName)
    {
      this.fileName = fileName;
      this.xmlDoc = new XmlDocument();
    }

    public void Close()
    {
      this.rootNode = null;
      this.xmlDoc = null;
    }
    private bool RootNode
    {
      get
      {
        if (this.rootNode == null)
          return false;
        return true;
      }
      set
      {
        if (value && !this.RootNode)
        {
          if (File.Exists(this.fileName))
          {
            xmlDoc.Load(this.fileName);
            XmlNodeList userNodes = this.xmlDoc.SelectNodes(string.Format("//TestData"));
            this.rootNode = userNodes[0];
          }
          else
          {
            this.rootNode = this.xmlDoc.CreateElement("TestData");
            this.xmlDoc.AppendChild(this.rootNode);
          }
        }
      }
    }

    public void WriteNode(string node, string nodeValue)
    {
      this.RootNode = true;
      XmlNodeList userNodes = this.xmlDoc.SelectNodes(string.Format("//TestData//{0}", node));
      if (userNodes.Count == 0)
      {
        XmlNode userNode = xmlDoc.CreateElement(node);
        userNode.InnerText = nodeValue;
        rootNode.AppendChild(userNode);
      }
      else
      {
        userNodes[0].InnerText = nodeValue;
      }
      this.xmlDoc.Save(this.fileName);
    }

    public string ReadNode(string node)
    {
      if (this.rootNode == null)
        xmlDoc.Load(this.fileName);
      XmlNodeList userNodes = this.xmlDoc.SelectNodes(string.Format("//TestData/{0}", node));
      return userNodes[0].InnerText;
    }

    public void RemoveNode()
    {
      xmlDoc.Load(this.fileName);

      foreach (XmlNode node in this.xmlDoc.DocumentElement.ChildNodes)
        node.InnerText = "";

      xmlDoc.Save(this.fileName);
    }
  }
}
