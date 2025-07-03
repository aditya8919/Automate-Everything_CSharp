using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace EComerceProject.Test
{
  public class TestConfig
  {

    public static string ApplicationURL
    {
      get
      {
        return ConfigurationManager.AppSettings["ApplicationURL"];
      }

      set
      {
        ConfigurationManager.AppSettings["ApplicationURL"] = value;
        ConfigurationManager.RefreshSection("appSettings");
      }
    }

    public static string UserID
    {
      get
      {
        return ConfigurationManager.AppSettings["UserID"];
      }

      set
      {
        ConfigurationManager.AppSettings["UserID"] = value;
        ConfigurationManager.RefreshSection("appSettings");
      }
    }

    public static string Password
    {
      get
      {
        return ConfigurationManager.AppSettings["Password"];

      }

      set
      {
        ConfigurationManager.AppSettings["Password"] = value;
        ConfigurationManager.RefreshSection("appSettings");
      }
    }

    public static string Browser
    {
      get
      {
        return ConfigurationManager.AppSettings["Browser"];
      }

      set
      {
        ConfigurationManager.AppSettings["Browser"] = value;
        ConfigurationManager.RefreshSection("appSettings");
      }
    }

    public static string IsSSOEnabled
    {
      get
      {
        return ConfigurationManager.AppSettings["IsSSOEnabled"];
      }

      set
      {
        ConfigurationManager.AppSettings["IsSSOEnabled"] = value;
        ConfigurationManager.RefreshSection("appSettings");
      }
    }
  }
}
