using EnterpriseFramework.Libraries;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EComerceProject.Test;
using EComerceProject;
using EnterpriseFramework.ProductModel;
using EComerceProject.Libraries.Logger;

namespace EComerceProject.ProductModel.EditMenu
{
  public class RHSEditPageObject
  {



    #region Repository

    private readonly string EditAccBtn = "//a[text()='Edit Account']";
    private readonly string Fname = "//input[@id='input-firstname']";
    private readonly string Lname = "//input[@id='input-lastname']";
    private readonly string ContinueBtn = "//input[@value='Continue']";
    private readonly string AlertMsg = "//div[@class='alert alert-success alert-dismissible']";

    private readonly string AddrBookBtn = "//aside[@id='column-right']//a[text()='Address Book']";
    private readonly string EditBtn = "//div[@class='table-responsive']//a[text()='Edit']";

    private readonly string EditPassbtn = "//div[@class='list-group']//a[text()='Password']";
    private readonly string PasswordField = "//input[@id='input-password']";
    private readonly string ConfirmPassField = "//input[@id='input-confirm']";

    #endregion

    #region Properties

    private IWebElement EditAccountButton
    {

      get
      {
        return WebElementFinder.FindElement(SeleniumWebDriver.driver, ElementBy.Xpath, EditAccBtn);
      }

    }


    private IWebElement Firstname
    {
      get
      {
        return WebElementFinder.FindElement(SeleniumWebDriver.driver, ElementBy.Xpath, Fname);
      }
    }


    private IWebElement LastName
    {
      get
      {
        return WebElementFinder.FindElement(SeleniumWebDriver.driver, ElementBy.Xpath, Lname);
      }
    }

    private IWebElement ContinueButton
    {
      get
      {
        return WebElementFinder.FindElement(SeleniumWebDriver.driver, ElementBy.Xpath, ContinueBtn);
      }
    }


    private IWebElement AlertMessage
    {
      get
      {
        return WebElementFinder.FindElement(SeleniumWebDriver.driver, ElementBy.Xpath, AlertMsg);
      }
    }

    private IWebElement AddressBookButton
    {
      get
      {
        return WebElementFinder.FindElement(SeleniumWebDriver.driver, ElementBy.Xpath, AddrBookBtn);
      }
    }

    private IWebElement EditAddrButton
    {
      get
      {
        return WebElementFinder.FindElement(SeleniumWebDriver.driver, ElementBy.Xpath, EditBtn);
      }
    }


    private IWebElement EditPasswordButton
    {
      get
      {
        return WebElementFinder.FindElement(SeleniumWebDriver.driver, ElementBy.Xpath, EditPassbtn);
      }
    }

    private IWebElement InputPasswordField
    {
      get
      {
        return WebElementFinder.FindElement(SeleniumWebDriver.driver, ElementBy.Xpath, PasswordField);
      }
    }

    private IWebElement ConfirmPasswordField
    {
      get
      {
        return WebElementFinder.FindElement(SeleniumWebDriver.driver, ElementBy.Xpath, ConfirmPassField);
      }
    }

    #endregion

    #region Methods

    public bool PerformEditAccName()
    {
      bool flag = true;

      EditAccountButton.Click();
      Firstname.Clear();
      Firstname.SendKeys("Aditya");
      LastName.Clear();
      LastName.SendKeys("Ganjkar");
      Thread.Sleep(2000);
      ContinueButton.Click();
      Thread.Sleep(2000);

      String Alertmessage = AlertMessage.Text;

      if (!(Alertmessage.Equals(ApplicationCommonMessages.YOUR_ACCOUNT_HAS_BEEN_SUCCESSFULLY_UPDATED)));
      {
        UtilityBase.Log.WriteLine("Failed to update account name.", Log.LogType.FAIL, true, true);
        flag = false;
      }

      return flag;
    }

    public bool PerfomEditAddr()
    {
      bool flag = false;

      AddressBookButton.Click();
      EditAddrButton.Click();
      ContinueButton.Click();
      Thread.Sleep(2000);

      String Alert = AlertMessage.Text;

      if (Alert.Equals(ApplicationCommonMessages.YOUR_ADDRESS_HAS_BEEN_SUCCESSFULLY_UPDATED))
      {
        UtilityBase.Log.WriteLine("Address updated successfully.", Log.LogType.PASS);
        flag = true;
      }
      else
      {
        UtilityBase.Log.WriteLine("Address updated successfully.", Log.LogType.PASS, true, true);
      }


      return flag;
    }

    public bool PerformEditPass()
    {


      bool flag = false;

      EditPasswordButton.Click();
      Thread.Sleep(2000);
      InputPasswordField.SendKeys(TestConfig.Password);
      ConfirmPasswordField.SendKeys(TestConfig.Password);
      Thread.Sleep(2000);
      ContinueButton.Click();
      Thread.Sleep(2000);

      String Alert = AlertMessage.Text;
      if (Alert.Equals(ApplicationCommonMessages.YOUR_PASSWORD_HAS_BEEN_SUCCESSFULLY_UPDATED)) ;
      flag = true;

      return flag;

    }





    #endregion
  }

}








