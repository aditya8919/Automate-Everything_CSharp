using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using WebDriverManager.DriverConfigs.Impl;


namespace EComerceProject
{
    public class Tests
    {
        IWebDriver driver;


        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            new WebDriverManager.DriverManager().SetUpDriver(new ChromeConfig());
            driver = new ChromeDriver();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            driver.Manage().Window.Maximize();
            driver.Url = "http://tutorialsninja.com/demo/";
        }


        [SetUp]
        public void Setup()
        {


            driver.FindElement(By.XPath("//span[text()='My Account']")).Click();
            driver.FindElement(By.XPath("//ul[@class='dropdown-menu dropdown-menu-right']//a[text()='Login']")).Click();
            driver.FindElement(By.XPath("//input[@id='input-email']")).SendKeys("ag89111@gmail.com");
            driver.FindElement(By.XPath("//input[@id='input-password']")).SendKeys("Aditya@8919");
            driver.FindElement(By.XPath("//input[@value='Login']")).Click();

        }

        [Test]
        public void EditAccName()
        {
            driver.FindElement(By.XPath("//a[text()='Edit Account']")).Click();
            IWebElement Fname = driver.FindElement(By.XPath("//input[@id='input-firstname']"));
            Fname.Clear();
            Fname.SendKeys("Aditya");
            IWebElement Lname = driver.FindElement(By.XPath("//input[@id='input-lastname']"));
            Lname.Clear();
            Lname.SendKeys("Ganjkar");

            driver.FindElement(By.XPath("//input[@value='Continue']")).Click();

            IWebElement AlertMsg = driver.FindElement(By.XPath("//div[@class='alert alert-success alert-dismissible']"));

            if (AlertMsg.Text.Contains("Success"))
                Assert.Pass();
            else
                Assert.Fail();

        }

        [Test, Order(2)]
        public void EditAddr()
        {
            driver.FindElement(By.XPath("//aside[@id='column-right']//a[text()='Address Book']")).Click();

            driver.FindElement(By.XPath("//div[@class='table-responsive']//a[text()='Edit']")).Click();

            driver.FindElement(By.XPath("//input[@value='Continue']")).Click();

            IWebElement AlertMsg = driver.FindElement(By.XPath("//div[@class='alert alert-success alert-dismissible']"));

            if (AlertMsg.Text.Contains("successfully updated"))
                Assert.Pass();
            else
                Assert.Fail();

        }

        [Test, Order(3)]
        public void EditPassword()
        {
            driver.FindElement(By.XPath("//div[@class='list-group']//a[text()='Password']")).Click();
            driver.FindElement(By.XPath("//input[@id='input-password']")).SendKeys("Aditya@8919");
            driver.FindElement(By.XPath("//input[@id='input-confirm']")).SendKeys("Aditya@8919");
            driver.FindElement(By.XPath("//input[@value='Continue']")).Click();

            IWebElement AlertMsg = driver.FindElement(By.XPath("//div[@class='alert alert-success alert-dismissible']"));

            if (AlertMsg.Text.Contains("Success"))
                Assert.Pass();
            else
                Assert.Fail();




        }

        [TearDown]
        public void Teardown()
        {
            driver.FindElement(By.XPath("//span[text()='My Account']")).Click();
            driver.FindElement(By.XPath("//li[@class='dropdown open']//a[text()='Logout']")).Click();
        }


        [OneTimeTearDown]
        public void OneTimeTeardown()
        {
            driver.Close();
        }
    }
}