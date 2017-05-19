using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

/* Adding Greg's back end*/
using FifthTribe.Database.HultPrize;
using FifthTribe.HultPrize;
using FifthTribe.Common;

namespace HultPrizeAtAdmin
{
  // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
  // visit http://go.microsoft.com/?LinkId=9394801
  public class HultPrizeAtApplication : HttpApplication
  {
    // Session string variable
    private const string SESSION_CURRENT_USER = "CurrentUser";

    protected void Application_Start()
    {
      AreaRegistration.RegisterAllAreas();

      WebApiConfig.Register(GlobalConfiguration.Configuration);
      FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
      RouteConfig.RegisterRoutes(RouteTable.Routes);
    }

    // The currently logged in user, set and get the user in the session
    #region
    /// <summary>
    /// The currently logged in user.
    /// </summary>
    public static Bus_User CurrentUser
    {
      get
      {
        Bus_User returnUser = null;
        if (HttpContext.Current.Session[SESSION_CURRENT_USER] != null)
        {
          returnUser = (Bus_User)HttpContext.Current.Session[SESSION_CURRENT_USER];
        }
        else
        {
          // If user is authenticated but the session is null, get the user object
          if (HttpContext.Current.User.Identity.IsAuthenticated)
          {
            int env = Convert.ToInt32(System.Web.Configuration.WebConfigurationManager.AppSettings["Environment"]);
            returnUser = HultBusiness.User.GetUserInfo(env, HttpContext.Current.User.Identity.Name);
          }
        }
        return returnUser;
      }
      set
      {
        HttpContext.Current.Session[SESSION_CURRENT_USER] = value;
      }
    }
    #endregion
  }
}