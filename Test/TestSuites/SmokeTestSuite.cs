using EComerceProject.ProductModel;
using EComerceProject.Test.TestData;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EComerceProject.Test.TestSuites
{
    public class SmokeTestSuite : Basetest
    {

        [Test, Order(1), Category(CategoryClass.EDIT_ACC)]
        public void EditAccNameTest()
        {
            Assert.That(EcommApp.RHSEditPage.PerformEditAccName(), "Account Name Changed Successfully");
        }

        [Test, Order(2), Category(CategoryClass.EDIT_ADDR)]
        public void EditAddrTest()
        {
            Assert.That(EcommApp.RHSEditPage.PerfomEditAddr(), "Address Changed Successfully");
        }
        
        [Test,Order(3), Category(CategoryClass.EDIT_PASS)]
        public void EditPasswordTest()
        {
            Assert.That(EcommApp.RHSEditPage.PerformEditPass(), "Password Changed Successfully");
        }






    }
}
