using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EnterpriseFramework.Libraries
{
    public class WebDropdownHelper
    {

        public bool SelectValueFromDropdown(IWebElement ele, string value)
        {
            bool flag = true;
            var option = ele.FindElements(By.XPath("//option[normalize-space(text()='"+value+"']"));
            if(option.Count() > 0)
            {
                option[0].Click();
                Thread.Sleep(1000);
            }
            else
                flag = false;

            return flag;
        }

        public bool SelectValueFromTypeAheadBox(IList<IWebElement> ele, string value)
        {
            bool flag = true;

            if(ele.Count() > 0)
            {
                Thread.Sleep(2000); // add explicit wait here

                for(int i = 0; i < ele.Count(); i++)
                {
                    if (ele[i].GetAttribute("title").Equals(value) || ele[i].GetAttribute("id").Equals(value))
                    {
                        ele[i].Click();
                        Thread.Sleep(2000);
                        break;

                    }
                    else
                        flag = false;

                }
              
            }
            else
            {
                Console.WriteLine("No combobox found for selecting value");
                        flag = false;
            }

            return flag;

        }

    }
}
