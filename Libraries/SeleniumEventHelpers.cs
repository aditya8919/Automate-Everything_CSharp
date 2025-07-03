using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EnterpriseFramework.Libraries
{
    public static class SeleniumEventHelpers
    {
        


        public static void MoveToElement(IWebElement element)
        {

            Actions act = new Actions(SeleniumWebDriver.driver);
            act.MoveToElement(element).Perform();
            Thread.Sleep(1000);


        }

        public static void ScrollToElement(IWebElement ele)
        {

            IJavaScriptExecutor js = (IJavaScriptExecutor)SeleniumWebDriver.driver;
            js.ExecuteScript("arguments[0].scrollIntoView(true)", ele);


        }

        public static void DoClickByJavascript(IWebElement ele)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)SeleniumWebDriver.driver;
            js.ExecuteScript("arguments[0].click()", ele);

        }

        public static void RightClick()
        {

        }






    }













}
