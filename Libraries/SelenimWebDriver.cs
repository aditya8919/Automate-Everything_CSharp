using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebDriverManager.DriverConfigs.Impl;

namespace EnterpriseFramework.Libraries
{
    public class SeleniumWebDriver
    {

        public static IWebDriver driver;
        public static ChromeDriver chromeDriver;
        public static EdgeDriver edgeDriver;

        public static void GoToURL(string url)
        {
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl(url);

        }

        public static bool CreateDriverInstance(string browserType)
        {
            bool IsDriverInitiated = false;
            if(driver == null)
            {

                switch (browserType)
                {
                    case BrowserType.CHROME:
                        driver = DriverOptions.GetChromeDriver();
                        break;

                    case BrowserType.EDGE:
                        driver = DriverOptions.GetEdgeDriver();
                        break;

                        default:
                        break;
                }

                if(driver != null)
                {
                    IsDriverInitiated = true;
                }

            }

            return IsDriverInitiated;
        }


        public static class BrowserType
        {

            public const String CHROME = "CHROME";
            public const String EDGE = "EDGE";


        }

        public static void KillDriver()
        {
            driver.Close();
            driver.Quit();
            driver=null;
        }

        public class DriverOptions
        {

            public static Configuration configValues;


            public static ChromeDriver GetChromeDriver()
            {

                //new WebDriverManager.DriverManager().SetUpDriver(new ChromeConfig(), WebDriverManager.Helpers.VersionResolveStrategy.MatchingBrowser);
                SeleniumWebDriver.chromeDriver = new ChromeDriver();




                return SeleniumWebDriver.chromeDriver;

            }


            public static EdgeDriver GetEdgeDriver()
            {
        //new WebDriverManager.DriverManager().SetUpDriver(new EdgeConfig(), WebDriverManager.Helpers.VersionResolveStrategy.MatchingBrowser);
        SeleniumWebDriver.edgeDriver = new EdgeDriver();

                return SeleniumWebDriver.edgeDriver;

            }




        }






    }
}
