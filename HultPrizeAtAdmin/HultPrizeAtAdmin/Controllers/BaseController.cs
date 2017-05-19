using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

// Added by Amir
using System.Net;
using System.Web.Routing;

/* Adding Greg's back end*/
using FifthTribe.Database.HultPrize;
using FifthTribe.HultPrize;
using FifthTribe.Common;
using HultPrizeAtAdmin.Common;

namespace HultPrizeAtAdmin.Controllers
{
  public class BaseController : Controller
  {
    #region Properties

    #region RunTimeEnvironment (read-only)
    // Common class used throughout the project
    public int RunTimeEnvironment
    {
      get
      {
        object runtimeEvironmentObject = System.Web.Configuration.WebConfigurationManager.AppSettings["Environment"];

        // if this variable is not set
        if (runtimeEvironmentObject == null)
        {
          throw new Exception("Runtime environment not set");
        }

        // Get Current environment integer value
        return Convert.ToInt32(runtimeEvironmentObject);
      }
    }
    #endregion

    #endregion


    #region This is called before any method is called
    protected override void Initialize(RequestContext requestContext)
    {
      // Get the url user is going to
      string url = requestContext.HttpContext.Request.RawUrl.ToLower();

      // bool if they are going to the login page - default to false
      bool goingToLogin = false;

      // Check if they are going to login page
      if (url.Contains("login") || url.Contains("acceptinvite") || url.Contains("forgotpassword") || url.Contains("invitationaccepted") || url.Contains("reset"))
      {
        // They are going to the login
        goingToLogin = true;
      }

      // Check if the user is logged in, which will solve most problems
      if (HultPrizeAtApplication.CurrentUser == null && !goingToLogin)
      {
        requestContext.HttpContext.Response.BufferOutput = true;
        requestContext.HttpContext.Response.Redirect("/Account/Login");
      }

      base.Initialize(requestContext);
    }
    #endregion

    // This handles exceptions for all actions globally that inherits from this controller
    #region Error Handling
    protected override void OnException(ExceptionContext filterContext)
    {
      // initialize the fifth tribe web exception object
      FTWebException webEx = new FTWebException(filterContext);

      // if there is a current user defined, add that info
      if (HultPrizeAtApplication.CurrentUser != null)
      {
        webEx.AddUserInfo(HultPrizeAtApplication.CurrentUser.UserId, HultPrizeAtApplication.CurrentUser.NameFirst + " " + HultPrizeAtApplication.CurrentUser.NameLast);
      }

      // add browser info
      webEx.AddBrowserInfo(this.Request.Browser);
      
      // add session info
      webEx.AddSessionInfo(this.Session);

      // Send error email
      HultBusiness.SendErrorEmail(webEx);

      //If the exception is already handled we do nothing
      if (filterContext.ExceptionHandled || Request.Headers["CallType"] != "Ajax")
        return;

      // Build the error?
      Error error = new Error
      {
        Message = filterContext.Exception.Message,
        CallStack = filterContext.Exception.ToString()
      };

      // Not sure?? Need to ask Jeremy
      Response.StatusCode = (int)HttpStatusCode.InternalServerError;
      filterContext.Result = Json(error);

      //Make sure that we mark the exception as handled
      filterContext.ExceptionHandled = true;

    }

    public class Error
    {
      public string Message { get; set; }
      public string CallStack { get; set; }
    }
    #endregion



  }
}
