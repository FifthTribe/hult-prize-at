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
using HultPrizeAt.Common;

namespace HultPrizeAt.Controllers
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



    // This handles exceptions for all actions globally that inherits from this controller
    #region Error Handling
    protected override void OnException(ExceptionContext filterContext)
    {


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
