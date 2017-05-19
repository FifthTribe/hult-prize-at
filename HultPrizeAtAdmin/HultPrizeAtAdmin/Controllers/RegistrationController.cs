using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;

/* Adding Greg's back end*/
using FifthTribe.Database.HultPrize;
using FifthTribe.HultPrize;
using FifthTribe.Common;
using HultPrizeAtAdmin.Common;

namespace HultPrizeAtAdmin.Controllers
{
  [Authorize]
  public class RegistrationController : BaseController
  {

    /* Page that lists the registration for a school */
    #region Registration page
    public ActionResult Index()
    {
      if (Session["schoolId"] == null)
      {
        return RedirectToAction("Index", "Home");
      }

      long schoolId = (long)Session["schoolId"];
      /*
      // If it wasn't passed get it from the currently logged in user
      List<Bus_Organization> schools = HultBusiness.Organization.GetUserOrganizations(this.RunTimeEnvironment, HultPrizeAtApplication.CurrentUser.UserId);

      // if it's greater than 1 then redirect them to schools page
      if (schools.Count > 1)
      {

        // They can edit more than one school page
        return RedirectToAction("Index", "Schools");
      }

      // Get the first school
      schoolId = schools[0].OrganizationId;
      */

      // Check if user has permission
      bool isMember = HultBusiness.Organization.IsUserMemberOfOrganization(this.RunTimeEnvironment, HultPrizeAtApplication.CurrentUser.UserId, schoolId) || !HultPrizeAtApplication.CurrentUser.IsSchoolAdmin;

      // Send current user to View
      ViewBag.CurrentUser = HultPrizeAtApplication.CurrentUser;

      if (isMember)
      {
        // Get the school object
        Bus_Organization_Result result = HultBusiness.Organization.GetOrganizationInfo(this.RunTimeEnvironment, schoolId);

        // Send result to View
        ViewBag.Result = result;

      }
      else
      {

        // No result
        ViewBag.Result = null;

      }

      // Send the runtime environment to the view bag
      ViewBag.RuntimeEnvironment = this.RunTimeEnvironment;
      return View();
    }
    #endregion

    #region Registration Admin Page
    public ActionResult Admin()
    {
      // Check if the user is an admin
      if (!HultPrizeAtApplication.CurrentUser.IsSuperUser || !HultPrizeAtApplication.CurrentUser.IsHultAdmin)
      {
        // They are not a hult or super user admin so send them to their school page
        return RedirectToAction("Index", "School");
      }

      return View();
    }
    #endregion

    /* ============ */
    /* POSTS */
    /* ============ */

    #region Get registration details
    [HttpPost]
    public ActionResult Details(long registrationId)
    {
      // Get the registration details result
      Bus_Registration_Result result = HultBusiness.Registrations.GetRegistrationInfoWithTeamMembers(this.RunTimeEnvironment, registrationId);

      // Return the result
      return Json(result);
    }
    #endregion


    #region Delete Registration
    [HttpPost]
    public ActionResult Delete(long registrationId)
    {
      // Get registration object
      Bus_Registration_Result registrationResult = HultBusiness.Registrations.GetRegistrationInfo(this.RunTimeEnvironment, registrationId);
      Bus_Registration registration = registrationResult.Registration;

      // Get the registration details result
      WebCommon.Business.BusinessResult result = HultBusiness.Registrations.DeleteRegistration(this.RunTimeEnvironment, registration);

      // Check if it was successful
      if (result.Success)
      {
        // Success!
        Session["RegistrationSuccessMessage"] = "Successfully removed registration";
      }
      else
      {
        // Error!
        Session["RegistrationErrorMessage"] = result.ErrorDisplayMessage;
      }

      return RedirectToAction("Index", "Registration");
    }
    #endregion

  }
}