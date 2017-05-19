using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HultPrizeAtAdmin.Controllers
{
  public class ErrorController : Controller
  {

    #region Error Page
    public ActionResult Index()
    {
      return View();
    }
    #endregion

  }
}