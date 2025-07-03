using EComerceProject.Test;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnterpriseFramework.Libraries
{


    public enum ElementBy
        {
            Id, Xpath, ClassName
        }


    public class WebElementFinder
    {

        // method to find the webeement
        // Parameters --> 
        //ElementBy
        //Value
        //Framename
        //needToWaitUntilDisplay 

        public static IWebElement FindElement(IWebDriver driver, ElementBy elementBy, String value)
        {
            IWebElement element;
            
            switch (elementBy)
            {
                case ElementBy.Id:
                    element = driver.FindElement(By.Id(value));

                    break;


                case ElementBy.Xpath:
                    element = driver.FindElement(By.XPath(value));

                    break;

                case ElementBy.ClassName:
                    element = driver.FindElement(By.ClassName(value));

                    break;

                    default:
                    element = null;
                    break;
            }

            return element;


        }
       







    }
}
