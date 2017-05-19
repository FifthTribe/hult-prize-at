using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

// Amir Added
using System.Web.Security;

/* Adding Greg's back end*/
using FifthTribe.Database.HultPrize;
using FifthTribe.HultPrize;
using FifthTribe.Common;
using HultPrizeAtAdmin.Common;

namespace HultPrizeAtAdmin.Controllers
{
  [Authorize]
  public class AccountController : BaseController
  {
    #region account main page
    public ActionResult Index()
    {
      return View();
    }
    #endregion

    #region Page where user accepts invitation
    [AllowAnonymous]
    public ActionResult AcceptInvite()
    {
      return View();
    }
    #endregion

    #region Page where user accepts invitation process
    [HttpPost]
    [AllowAnonymous]
    public ActionResult InviteProcess(string email, string code, string password)
    {
      // Get the result of changing the password
      WebCommon.Business.ChangePasswordResult result = HultBusiness.User.ConfirmSchoolAdministrator(this.RunTimeEnvironment, email, code, password);

      // Check if it was successful or not
      if (result.ResultCode == WebCommon.Business.ChangePasswordResult.ResultCodes.SUCCESS)
      {
        // Success!
        Session["InvitationError"] = null;
        return RedirectToAction("InvitationAccepted");
      }
      else
      {
        // Uh oh we have some problems
        Session["InvitationError"] = result.FailMessage;
        return RedirectToAction("AcceptInvite");
      }
    }
    #endregion

    #region Invitation Acceptance Success
    [AllowAnonymous]
    public ActionResult InvitationAccepted()
    {

      return View();
    }
    #endregion


    #region login page
    [AllowAnonymous]
    public ActionResult Login()
    {
      ViewBag.LoginMessage = (string)Session["LoginErrorMessage"];

      return View();
    }
    #endregion

    #region logout page
    public ActionResult Logout()
    {
      FormsAuthentication.SignOut();
      Session.Abandon();

      return RedirectToAction("Login");
    }
    #endregion

    #region Login Process
    [HttpPost]
    [AllowAnonymous]
    public ActionResult LoginProcess(string username, string password, string returnUrl)
    {
      if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
      {
        Session["LoginErrorMessage"] = "Invalid username or password";
        return RedirectToAction("Login");
      }

      //Check credentials
      WebCommon.Business.LoginResult loginResult = HultBusiness.User.Login(this.RunTimeEnvironment, username, password);

      // Check if the user successfully logged in
      if (loginResult.ResultCode == WebCommon.Business.LoginResult.ResultCodes.SUCCESS)
      {
        // Get the user and set it as a user object
        Bus_User user = (Bus_User)loginResult.User;

        // Set cookie for user logged in
        HultPrizeAtCommon.RenewCookie(HttpContext.Response, user.Email, user);

        // Set the currently logged in user
        HultPrizeAtApplication.CurrentUser = user;

        // Redurect to the appropiate url
        return RedirectToLocal(returnUrl);
      }

      // Login failed show message
      Session["LoginErrorMessage"] = loginResult.FailMessage;

      // If we got this far, something failed, redisplay form
      return RedirectToAction("Login");
    }

    //
    // POST: /Account/GetUserInformation
    [HttpPost]
    public ActionResult ChangePassword(string oldPassword, string newPassword)
    {
      if (HultPrizeAtApplication.CurrentUser == null || HultPrizeAtApplication.CurrentUser.UserId < 1)
      {
        throw new Exception("Invalid User");
      }

      if (newPassword.Length == 0)
      {
        throw new Exception("You must enter a new password");
      }

      // change the password in the database
      HultBusiness.User.ChangePassword(this.RunTimeEnvironment, HultPrizeAtApplication.CurrentUser.UserId, oldPassword, newPassword);

      return Json(string.Empty);
    }
    #endregion

    #region Helpers

    private ActionResult RedirectToLocal(string returnUrl)
    {
      if (Url.IsLocalUrl(returnUrl))
        return Redirect(returnUrl);

      return RedirectToAction("Index", "Home");
    }

    #endregion


    //=================
    // PASSWORD FORGOT
    //=================

    #region User Forgot Password Page
    [AllowAnonymous]
    public ActionResult ForgotPassword()
    {
      return View();
    }
    #endregion

    #region User Forgot Password Process
    [HttpPost]
    [AllowAnonymous]
    public ActionResult ForgotPassword(string email)
    {

      // Get result
      WebCommon.Business.BusinessResult result = HultBusiness.User.SendPasswordRecoveryEmail(this.RunTimeEnvironment, email);

      // Set session form errors
      Session["ForgotPasswordResult"] = result;

      return RedirectToAction("ForgotPassword");
    }
    #endregion

    //=================
    // PASSWORD RESET
    //=================

    #region User Forgot Password Page
    [AllowAnonymous]
    public ActionResult Reset()
    {
      return View();
    }
    #endregion

    #region User Forgot Password Process
    [HttpPost]
    [AllowAnonymous]
    public ActionResult Reset(string email, string code, string password)
    {

      // Get result
      WebCommon.Business.BusinessResult result = HultBusiness.User.ChangePasswordFromRecoveryEmail(this.RunTimeEnvironment, email, code, password);

      // Set session form errors
      Session["ResetPassword"] = result;

      return RedirectToAction("Reset");
    }
    #endregion

  }
}