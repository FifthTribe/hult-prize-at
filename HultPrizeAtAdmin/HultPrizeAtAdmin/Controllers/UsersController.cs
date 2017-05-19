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
  public class UsersController : BaseController
  {

    #region List of users belonging to the school
    public ActionResult Index()
    {
      if (Session["schoolId"] == null)
      {
        return RedirectToAction("Index", "Home");
      }

      // Get school id from session
      long schoolId = (long)Session["schoolId"];

      // Default bool to check if user has access
      bool isMember = false;

      // Check if user is not an admin
      if (!HultPrizeAtApplication.CurrentUser.IsHultAdmin && !HultPrizeAtApplication.CurrentUser.IsSuperUser)
      {

        // Check if user has permission
        isMember = HultBusiness.Organization.IsUserMemberOfOrganization(this.RunTimeEnvironment, HultPrizeAtApplication.CurrentUser.UserId, schoolId);
      }
      else
      {

        // They are an admin so they can see the school
        isMember = true;
      }

      // Send current user to View
      ViewBag.CurrentUser = HultPrizeAtApplication.CurrentUser;

      if (isMember)
      {
        // Get the list of users belonging to the school
        List<Bus_User> users = HultBusiness.Organization.GetOrganizationUsers(this.RunTimeEnvironment, schoolId);

        // Send the list of users to the view to View
        ViewBag.Users = users;

        // Send the school id
        ViewBag.SchoolId = schoolId;

      }
      else
      {

        // No result
        ViewBag.Users = null;

      }

      // Send the runtime environment to the view bag
      ViewBag.RuntimeEnvironment = this.RunTimeEnvironment;

      return View();
    }
    #endregion

    #region Add new user
    [HttpPost]
    public ActionResult New(string firstName, string lastName, string email, CommonTelephone phone, long schoolId, long createdByUserId)
    {

      // Create the invite new user object
      HultBusiness.InviteUserToOrganizationObject inviteInfo = new HultBusiness.InviteUserToOrganizationObject();

      // Add info to the object
      inviteInfo.AdminFirstName = firstName;
      inviteInfo.AdminLastName = lastName;
      inviteInfo.Email = email;

      // Invite new user result
      WebCommon.Business.BusinessResult result = HultBusiness.InviteNewUserToOrganization(this.RunTimeEnvironment, schoolId, inviteInfo, createdByUserId);

      // Check the result
      if (result.Success)
      {
        Session["UsersSuccessMessage"] = "Successfully invited user.";
      }
      else
      {
        Session["UsersErrorMessage"] = result.ErrorDisplayMessage;
      }
      // Return the list of users page
      return RedirectToAction("Index", new { id = schoolId });
    }
    #endregion

    #region Edit user
    [HttpPost]
    public ActionResult Edit(string firstName, string lastName, string email, long userId, bool phoneIsInternational, string phoneAreaCode, string phoneFirstThree, string phoneLastFour, string phonePhoneNumberDigits, string phoneCountryCode, string phoneExtension)
    {
      // Create telephone object
      CommonTelephone tel = new CommonTelephone();

      // Check if international
      if (phoneIsInternational)
      {
        // It's international
        tel.AreaCode = phoneCountryCode;
        tel.IsInternational = true;
        tel.InternationalPhoneNumber = phonePhoneNumberDigits;
        tel.Extension = phoneExtension;
      }
      else
      {
        // It's american
        tel.AreaCode = phoneAreaCode;
        tel.FirstThree = phoneFirstThree;
        tel.LastFour = phoneLastFour;
        tel.Extension = phoneExtension;
        tel.IsInternational = false;
      }

      // Get User Info
      Bus_User user = HultBusiness.User.GetUserInfo(this.RunTimeEnvironment, userId);

      // Add info to the object
      user.NameFirst = firstName;
      user.NameLast = lastName;
      user.Email = email;
      user.PhoneDetails = tel;

      // Save user
      HultBusiness.User.SaveUserInfo(this.RunTimeEnvironment, user);

      // Update current user
      HultPrizeAtApplication.CurrentUser = user;

      // Session message for notifcation on page
      Session["UserSuccessMessage"] = true;

      // Return the list of users page
      return RedirectToAction("Index", "Account");
    }
    #endregion


  }
}
