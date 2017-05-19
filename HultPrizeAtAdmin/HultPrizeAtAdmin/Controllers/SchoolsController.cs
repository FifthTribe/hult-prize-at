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
  public class SchoolsController : BaseController
  {

    #region List of all schools
    public ActionResult Index()
    {
      // Get list of all the schools
      ViewBag.Schools = HultBusiness.Organization.GetAllOrganizations_OrderByName(this.RunTimeEnvironment, 0, 1000, true);

      // Send the runtime environment to the view bag
      ViewBag.RuntimeEnvironment = this.RunTimeEnvironment;

      // Reset the school id since the user is on the schools page
      Session["schoolId"] = null;

      return View();
    }
    #endregion

  }
}