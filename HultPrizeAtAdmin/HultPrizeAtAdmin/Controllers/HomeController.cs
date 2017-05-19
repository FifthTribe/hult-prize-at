using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

/* Amir added */
using System.Reflection;
using System.Web.Configuration;

/* Adding Greg's back end*/
using FifthTribe.Database.HultPrize;
using FifthTribe.HultPrize;
using FifthTribe.Common;

namespace HultPrizeAtAdmin.Controllers
{
  [Authorize]
  public class HomeController : BaseController
  {

    #region Homepage
    public ActionResult Index()
    {
      if (HultPrizeAtApplication.CurrentUser == null)
      {
        return RedirectToAction("Logout", "Account");
      }

      if (HultPrizeAtApplication.CurrentUser.IsSchoolAdmin)
      {
        return RedirectToAction("Index", "School");
      }

      if (HultPrizeAtApplication.CurrentUser.IsHultAdmin)
      {
        return RedirectToAction("Index", "Schools");
      }

      if (HultPrizeAtApplication.CurrentUser.IsSuperUser)
      {
        return RedirectToAction("Index", "Schools");
      }

      return View();
    }
    #endregion

  }
}