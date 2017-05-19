using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

/* Adding Greg's back end*/
using FifthTribe.Database.HultPrize;
using FifthTribe.HultPrize;
using FifthTribe.Common;
using HultPrizeAtAdmin.Common;

namespace HultPrizeAtAdmin.Controllers
{
  [Authorize]
  public class RegistrationsController : BaseController
  {

    /* Page that lists the registrations for all schools (hult admin view) */
    #region Registrations page
    public ActionResult Index()
    {
      // Check if they are an admin or super user
      Bus_User currentUser = HultPrizeAtApplication.CurrentUser;

      // Check if they are a not super user or not hult admin
      if (!currentUser.IsHultAdmin && !currentUser.IsSuperUser)
      {
        return RedirectToAction("Index", "School");
      }

      // Send the runtime environment to the view bag
      ViewBag.RuntimeEnvironment = this.RunTimeEnvironment;
      return View();
    }
    #endregion
  }
}