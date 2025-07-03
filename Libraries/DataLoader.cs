using EComerceProject.Libraries.Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EComerceProject.Libraries
{
  public class DataLoader : UtilityBase
  {
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="FilePath"></param>
    /// <returns></returns>
    public static T GetData<T>(string FilePath)
    {
      try
      {
        Type dataType = Type.GetType(typeof(T).ToString() + "," + typeof(T).Assembly.ToString());

        //serializing XML based on input object type
        XmlSerializer serializer = new XmlSerializer(dataType);
        using (StreamReader reader = new StreamReader(FilePath))
        {
          return (T)serializer.Deserialize(reader);
        }
      }
      catch (Exception ex)
      {
        //string str = ex.Message;
        // ExtentLogger.LogError(ex.Message);
        Log.WriteLine("Error encountered while Deserializing" + ex.Message);
        return default(T);
      }
    }
  }
}
