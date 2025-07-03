using EnterpriseFramework.Libraries;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EComerceProject.Test;
using EComerceProject.Test;

namespace EComerceProject.ProductModel.Login
{
  public class LoginPageObject : Basetest
  {

    #region Repository

    private readonly string MyAccountBtn = "//span[text()='My Account']";
    private readonly string TopLoginBtn = "//ul[@class='dropdown-menu dropdown-menu-right']//a[text()='Login']";
    private readonly string InputEmailText = "//input[@id='input-email']";
    private readonly string InputPasswordText = "//input[@id='input-password']";
    private readonly string LoginBtn = "//input[@value='Login']";
    private readonly string LogoutBtn = "//li[@class='dropdown open']//a[text()='Logout']";
    private readonly string VerifyLogoutText = "//div[@id='content']//h1[text()='Account Logout']";
    private readonly string VerifyLoginText = "//div[@id='content']//h2[text()='My Account']";

    #endregion

    #region Properties

    private IWebElement MyAccountButton
    {

      get
      {
        return WebElementFinder.FindElement(SeleniumWebDriver.driver, ElementBy.Xpath, MyAccountBtn);
      }

    }


    private IWebElement TopLoginButton
    {
      get
      {
        return WebElementFinder.FindElement(SeleniumWebDriver.driver, ElementBy.Xpath, TopLoginBtn);
      }
    }


    private IWebElement InputEmailTextBox
    {
      get
      {
        return WebElementFinder.FindElement(SeleniumWebDriver.driver, ElementBy.Xpath, InputEmailText);
      }
    }

    private IWebElement InputPasswordTextBox
    {
      get
      {
        return WebElementFinder.FindElement(SeleniumWebDriver.driver, ElementBy.Xpath, InputPasswordText);
      }
    }


    private IWebElement LoginButton
    {
      get
      {
        return WebElementFinder.FindElement(SeleniumWebDriver.driver, ElementBy.Xpath, LoginBtn);
      }
    }

    private IWebElement LogoutButton
    {
      get
      {
        return WebElementFinder.FindElement(SeleniumWebDriver.driver, ElementBy.Xpath, LogoutBtn);
      }
    }

    private IWebElement TextAfterLogout
    {
      get
      {
        return WebElementFinder.FindElement(SeleniumWebDriver.driver, ElementBy.Xpath, VerifyLogoutText);
      }
    }


    private IWebElement TextAfterLogin
    {
      get
      {
        return WebElementFinder.FindElement(SeleniumWebDriver.driver, ElementBy.Xpath, VerifyLoginText);
      }
    }

    #endregion

    #region Methods

    public bool PerformLogin()
    {

      bool flag;

      MyAccountButton.Click();
      TopLoginButton.Click();
      Thread.Sleep(2000);
      InputEmailTextBox.SendKeys(TestConfig.UserID);
      InputPasswordTextBox.SendKeys(TestConfig.Password);
      LoginButton.Click();

      Thread.Sleep(2000);

      if (TextAfterLogin.Text.Equals("My Account"))
        flag = true;
      else
        flag = false;

      return flag;

    }

    public bool PerformLogout()
    {
      bool flag;

      MyAccountButton.Click();
      LogoutButton.Click();

      if (TextAfterLogout.Text.Equals("Account Logout"))
      {
        Log.WriteLine("User logged out successfully.");
        flag = true;
      }

      else
      {
        Log.WriteLine("User failed to logout successfully.",Libraries.Logger.Log.LogType.FAIL, true, true);
        flag = false;
      }

      Thread.Sleep(2000);

      return flag;
    }

  }

}

#endregion

#region VO

public class LoginPageVO
{
  public string UserID { get; set; }
  public string Password { get; set; }
}





#endregion
