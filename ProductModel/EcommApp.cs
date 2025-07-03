using EComerceProject.ProductModel.EditMenu;
using EComerceProject.ProductModel.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EComerceProject.ProductModel
{
    public class EcommApp
    {

        #region LoginPage

        private static LoginPageObject _LoginPageObject;

        public static LoginPageObject LoginPage
        {
            get
            {
                _LoginPageObject = new LoginPageObject();
                return _LoginPageObject;
            }
        }
    private static SSOLoginPageObject _SSOLoginPage;

    public static SSOLoginPageObject SSOLoginPage
    {
      get
      {
        _SSOLoginPage = new SSOLoginPageObject();
        return _SSOLoginPage;
      }
    }

    


    #endregion


    #region RHSEditMenuPage

        private static RHSEditPageObject _RHSEditPageObject;

        public static RHSEditPageObject RHSEditPage
        {
            get
            {
                _RHSEditPageObject = new RHSEditPageObject();
                return _RHSEditPageObject;
            }
        }

        #endregion







    }
}
