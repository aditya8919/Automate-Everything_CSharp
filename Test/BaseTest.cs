/* <02-12-2023> < Removed unwanted code and updated asertion method:  Assert.That> <Aditya G>
 */


using NUnit.Framework;
using OpenQA.Selenium;
using EComerceProject.ProductModel;
using EnterpriseFramework.Libraries;
using System.Threading;
using EComerceProject.Libraries.Logger;
using EComerceProject.Libraries;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System;
using System.Diagnostics;
using System.Linq;

namespace EComerceProject.Test
{
  public class Basetest
  {
    IWebDriver driver;

    protected static Log Log;
    protected static string logLocation;

    public Basetest()
    {

      //
      // TODO: Add constructor logic here
      //
    }

    private TestContext testContextInstance;

    /// <summary>
    ///Gets or sets the test context which provides
    ///information about and functionality for the current test run.
    ///</summary>
    public TestContext TestContext
    {
      get
      {
        return testContextInstance;
      }
      set
      {
        testContextInstance = value;
      }
    }

    [OneTimeSetUp]
    public static void ExecutionInitialization()
    {
      string dateTimeStamp = DateTime.Now.ToString("ddMMyy_HHmm");

      #region RunTime xml file name selection
      IEnumerable<Type> result = GetExecutingTestClassName();
      var runtimeTestDataFileName = GetCustomAttribute(result.FirstOrDefault());
      if (runtimeTestDataFileName != null)
        XMLHelper.runTimeXmlFileName = runtimeTestDataFileName;
      else
        XMLHelper.runTimeXmlFileName = "RunTimeTestData.xml";
      #endregion

      XMLHelper.RunTimeXMl.WriteNode("dateTimeStamp", "_" + dateTimeStamp);
      //Get Working Directory
      string workingDirectory = AppDomain.CurrentDomain.BaseDirectory;
      //Get Sol Directory
      string solDirectory = Directory.GetParent(workingDirectory).Parent.Parent.Parent.FullName;
      logLocation = Path.Combine(solDirectory, "TestResults");


#if !NUNIT
      Log = LogFactory.LoadLog(LoggerType.NUnit, logLocation, new CCH.Automation.Utilities.HTMLLogger.CaptureScreen());
#else
            Log = LogFactory.LoadLog(LoggerType.NUnit, logLocation);
#endif

      Log.OverrideImageSwitch(true, false);
    }

    /// <summary>
    /// Desc : Method to get collection of types from executing assembly
    /// </summary>
    /// <returns>IEnumerable types</returns>
    private static IEnumerable<Type> GetExecutingTestClassName()
    {
      var types = Assembly.GetExecutingAssembly().GetTypes();
      var result = from typeClass in types
                   where typeClass.Name.ToString() == TestContext.CurrentContext.Test.Name
                   select typeClass;
      if (result.Count() > 0)
        return result;
      else return null;
    }



    [SetUp]
    public void Setup()
    {
      string assembliesToIgnoreLog = string.Empty;
      Log.TestEnvironment = TestConfig.ApplicationURL + " -- Browser:" + TestConfig.Browser;
      Log.StartLogging(Log.LogFolderPath, TestContext.CurrentContext, assembliesToIgnoreLog);
      SeleniumWebDriver.CreateDriverInstance(TestConfig.Browser);
      SeleniumWebDriver.GoToURL(TestConfig.ApplicationURL);
      Log.WriteLine("Test Started => URL: " + TestConfig.ApplicationURL + " -- Browser:" + TestConfig.Browser, Log.LogType.INFO, true, false);
      var testCategory = TestContext.CurrentContext.Test.Properties.Get("Category");
      Thread.Sleep(2000);
      Assert.That(EcommApp.LoginPage.PerformLogin(), "Login Successful");

    }


    //[TearDown]   Logout Method
    //public void Teardown()
    //{
    //    driver.FindElement(By.XPath("//span[text()='My Account']")).Click();
    //    driver.FindElement(By.XPath("//li[@class='dropdown open']//a[text()='Logout']")).Click();
    //}

    //[TearDown]
    //public void Teardown()
    //{
     // SeleniumWebDriver.KillDriver();
    //}


    /// <summary>
    /// Test Cleanup
    /// </summary>
    [TearDown]
    public void TestTearDown()
    {
      try
      {
        var testCategory = TestContext.CurrentContext.Test.Properties.Get("Category");
        if (!Convert.ToString(testCategory).ToLower().Equals("skiplogin"))
        {
          if (TestConfig.IsSSOEnabled.Equals("Yes"))
            Log.AssertTrue(EcommApp.SSOLoginPage.SSOLogout(), "User has been logged out successfully using SSO");
          else
            Log.AssertTrue(EcommApp.LoginPage.PerformLogout(), "User has been logged out successfully");
        }

        SeleniumWebDriver.KillDriver();
        Log.EndLogging(TestContext.CurrentContext);
        // Logging Ends for the Current Running Test Case and Log Test Case Result.
      }
      catch (Exception ex)
      {
        Log.WriteLine(ex.StackTrace, Log.LogType.ERROR, true, true);
        SeleniumWebDriver.KillDriver();
        Log.EndLogging(TestContext.CurrentContext);
      }
    }

    [OneTimeTearDown]
    public static void OneTimeTearDown()
    {
      var vstsAgent = Process.GetProcessesByName("Agent.Listener");
      if (vstsAgent.Count() > 0)
      {
        string folderName = Log.LogFolderPath.Split('\\').Last();
        string targetPath = Path.GetPathRoot(Environment.SystemDirectory) + "\\AutomationLogs\\";
        FileUtility.CopyFolder(Log.LogFolderPath, targetPath + folderName);
      }
    }

    /// <summary>
    /// Desc : method to get custom attribute name
    /// </summary>
    /// <param name="type">type : class name</param>
    /// <returns>string : if found attribute else return false</returns>
    private static string GetCustomAttribute(Type type)
    {
      var runTimeTestData = (RunTimeTestDataAttribute[])
          type.GetCustomAttributes(typeof(RunTimeTestDataAttribute), false);

      if (runTimeTestData.Length == 0)
      {
        return null;
      }
      return runTimeTestData[0].FileName;
    }
  }

  [AttributeUsage(AttributeTargets.Class)]
  public class RunTimeTestDataAttribute : Attribute
  {
    // Provides name of the runtimefile
    private string fileName;

    // Constructor
    public RunTimeTestDataAttribute(string fileName)
    {
      this.fileName = fileName;
    }

    // property to get name
    public string FileName
    {
      get { return fileName; }
    }
  }
}
