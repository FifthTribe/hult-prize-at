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
  public class SchoolController : BaseController
  {

    #region Edit School Event Page
    public ActionResult Index(long? id)
    {
      // Default school id
      long schoolId = 0;

      // CHeck if the school id was passed
      if (id.HasValue)
      {
        // get school id
        schoolId = (long)id;
      }
      else
      {
        // check if the school id is in the session
        if (Session["schoolId"] != null)
        {
          // get the school id from the session
          schoolId = (long)Session["schoolId"];
        }
        else
        {
          // If it wasn't passed get it from the currently logged in user
          List<Bus_Organization> schools = HultBusiness.Organization.GetUserOrganizations(this.RunTimeEnvironment, HultPrizeAtApplication.CurrentUser.UserId);

          // if it's greater than 1 then redirect them to schools page
          if (schools.Count > 1)
          {

            // They can edit more than one school page
            return RedirectToAction("Index", "Schools");

          }
          else if (schools.Count == 1)
          {

            // Get the first school
            schoolId = schools[0].OrganizationId;
          }
        }


      }

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

      // Set current school they are editing in the session
      Session["schoolId"] = schoolId;


      return View();
    }
    #endregion

    // ===========
    // SCHOOL 
    // ===========

    #region Add New School Form Post
    [HttpPost]
    public ActionResult New(HultBusiness.CreateNewOrganizationObject info, string phoneIsInternational, string phoneAreaCode, string phoneFirstThree, string phoneLastFour, string phoneInternationalPhoneNumber, string phoneCountryCode, string phoneExtension)
    {
      // Create telephone object
      CommonTelephone tel = new CommonTelephone();

      // Check if international
      if (phoneIsInternational.Length > 0)
      {
        // It's international
        tel.CountryCode = phoneCountryCode;
        tel.InternationalPhoneNumber = phoneInternationalPhoneNumber;
        tel.Extension = phoneExtension;
        tel.IsInternational = true;
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

      // Update the telephone object to the info object
      info.Telephone = tel;

      // Create the new school
      WebCommon.Business.BusinessResult formResult = HultBusiness.CreateNewOrganizationAndUser(this.RunTimeEnvironment, info, HultPrizeAtApplication.CurrentUser.UserId);

      // Send formResult to session
      Session["NewSchoolFormResult"] = formResult;

      // Redirect to list of schools page
      return RedirectToAction("Index", "Schools");
    }
    #endregion

    #region Add School Background Image
    [HttpPost]
    public ActionResult Image(HttpPostedFileBase file, long orgId)
    {

      // Verify that the user selected a file
      if (file != null && file.ContentLength > 0)
      {

        // Get the school (will alwyas work becuase the orgId is from the school object on the edit page)
        Bus_Organization_Result result = HultBusiness.Organization.GetOrganizationInfo(this.RunTimeEnvironment, orgId);
        Bus_Organization school = result.Organization;

        // Get the file name of the background image
        string backgroundImageFilename = Bus_Organization.GetNewBackgroundImageFilename();

        // Upload the file
        HultPrizeAtCommon.UploadFile(this.RunTimeEnvironment, file, backgroundImageFilename, school);

        // Update the database with the file name
        HultBusiness.Organization.BackgroundImageWasUploaded(this.RunTimeEnvironment, orgId, backgroundImageFilename);

        // Image was added
        Session["EditSchoolSuccessMessage"] = "School Image Added";
      }
      else
      {
        // No file was selected
        Session["EditSchoolErrorMessage"] = "No image was selected.";
      }

      // Return to the school edit page
      return RedirectToAction("Index", "School", new { id = orgId });
    }
    #endregion

    #region Add School Logo Image
    [HttpPost]
    public ActionResult Logo(HttpPostedFileBase file, long orgId, bool comingFromSchoolsPage)
    {

      // Verify that the user selected a file
      if (file != null && file.ContentLength > 0)
      {

        // Get the school (will alwyas work becuase the orgId is from the school object on the edit page)
        Bus_Organization_Result result = HultBusiness.Organization.GetOrganizationInfo(this.RunTimeEnvironment, orgId);
        Bus_Organization school = result.Organization;

        // Get the file name of the logo image
        string logoImageFileName = Bus_Organization.GetNewLogoImageFilename();

        // Upload the file
        HultPrizeAtCommon.UploadFile(this.RunTimeEnvironment, file, logoImageFileName, school);

        // Update the database with the file name
        HultBusiness.Organization.LogoImageWasUploaded(this.RunTimeEnvironment, orgId, logoImageFileName);

        // Check if they are coming from the schools page
        if (comingFromSchoolsPage)
        {
          Session["SchooLogoMessageSuccess"] = "School Logo Image Added";
        }
        else
        {
          // Coming from individual school edit page

          // Image was added
          Session["EditSchoolSuccessMessage"] = "School Logo Image Added";
        }


      }
      else
      {

        // Check if they are coming from the schools page
        if (comingFromSchoolsPage)
        {
          Session["SchooLogoMessageError"] = "School Logo Image Added";
        }
        else
        {
          // Coming from individual school edit page

          // No file was selected
          Session["EditSchoolErrorMessage"] = "No image was selected.";
        }

      }


      // Check if they are coming from the schools page
      if (comingFromSchoolsPage)
      {
        // Return to the list of schools pages
        return RedirectToAction("Index", "Schools");
      }
      else
      {
        // Coming from individual school edit page

        // Return to the school edit page
        return RedirectToAction("Index", "School", new { id = orgId });
      }
    }
    #endregion

    #region Delete School Logo Image
    [HttpPost]
    public ActionResult LogoDelete(long orgId, bool comingFromSchoolsPage)
    {
      // Delete the school image
      HultBusiness.Organization.DeleteLogoImage(this.RunTimeEnvironment, orgId);

      // Check if they are coming from the schools page
      if (comingFromSchoolsPage)
      {
        Session["SchooLogoMessageSuccess"] = "School Logo Image Deleted";

        // Return to the list of schools pages
        return RedirectToAction("Index", "Schools");
      }
      else
      {
        // Coming from individual school edit page
        Session["EditSchoolSuccessMessage"] = "School Logo Image Deleted";

        // Return to the school edit page
        return RedirectToAction("Index", "School", new { id = orgId });
      }
    }
    #endregion

    #region Update School URL
    public ActionResult UpdateUrl(long schoolId, string url)
    {
      // Get the result
      WebCommon.Business.BusinessResult result = HultBusiness.OrganizationExtra.UpdateCustomUrl(this.RunTimeEnvironment, schoolId, url);

      // Check if it was successful
      if (result.Success)
      {
        // Success!
        Session["EditSchoolSuccessMessage"] = "School URL Updated";
      }
      else
      {
        // Error!
        Session["EditSchoolErrorMessage"] = result.ErrorDisplayMessage;
      }

      return RedirectToAction("Index", "Schools");
    }
    #endregion

    #region Update School Name
    public ActionResult Name(long schoolId, string schoolName)
    {
      // Get the result
      WebCommon.Business.BusinessResult result = HultBusiness.Organization.UpdateSchoolName(this.RunTimeEnvironment, schoolId, schoolName);

      // Check if it was successful
      if (result.Success)
      {
        // Success!
        Session["EditSchoolSuccessMessage"] = "School Name Updated";
      }
      else
      {
        // Error!
        Session["EditSchoolErrorMessage"] = result.ErrorDisplayMessage;
      }

      return RedirectToAction("Index", "School", new { id = schoolId });
    }
    #endregion

    #region Update School Social Links
    public ActionResult SocialLinks(long schoolId, string schoolFacebook, string schoolTwitter, string schoolLinkedIn, string schoolInstagram)
    {
      // Get the Organization
      Bus_Organization_Result result = HultBusiness.Organization.GetOrganizationInfo(this.RunTimeEnvironment, schoolId);
      Bus_Organization school = result.Organization;

      // Update social links
      school.ExtraInformation.FacebookUrl = schoolFacebook;
      school.ExtraInformation.TwitterUrl = schoolTwitter;
      school.ExtraInformation.LinkedInUrl = schoolLinkedIn;
      school.ExtraInformation.InstagramUrl = schoolInstagram;

      // Save organization
      HultBusiness.OrganizationExtra.SaveExtraInformation(this.RunTimeEnvironment, school.ExtraInformation);

      // Set success message
      Session["EditSchoolSuccessMessage"] = "School Social Links Updated";

      // Return to the school editing page
      return RedirectToAction("Index", "School", new { id = schoolId });
    }
    #endregion

    #region Delete School
    public ActionResult Remove(long schoolId)
    {
      HultBusiness.Organization.DeleteOrganization(this.RunTimeEnvironment, schoolId);
      
      Session["EditSchoolSuccessMessage"] = "School Has Been Removed";

      return RedirectToAction("Index", "Schools");
    }
    #endregion

    #region Update Registration Text
    [HttpPost]
    public ActionResult RegistrationText(long schoolId, string text)
    {
      // Get the school (will alwyas work becuase the orgId is from the school object on the edit page)
      Bus_Organization_Result result = HultBusiness.Organization.GetOrganizationInfo(this.RunTimeEnvironment, schoolId);
      Bus_Organization school = result.Organization;

      // Update the registration form text
      school.ExtraInformation.RegistrationFormText = text;

      // Save school
      HultBusiness.OrganizationExtra.SaveExtraInformation(this.RunTimeEnvironment, school.ExtraInformation);

      // Success!
      Session["EditSchoolSuccessMessage"] = "School Registration Form Text Updated";

      return RedirectToAction("Index", "School", new { id = schoolId });
    }
    #endregion

    #region Update Winner Image
    [HttpPost]
    public ActionResult WinnerImage(HttpPostedFileBase file, long orgId)
    {

      // Verify that the user selected a file
      if (file != null && file.ContentLength > 0)
      {

        // Get the school (will alwyas work becuase the orgId is from the school object on the edit page)
        Bus_Organization_Result result = HultBusiness.Organization.GetOrganizationInfo(this.RunTimeEnvironment, orgId);
        Bus_Organization school = result.Organization;

        // Get the file name of the winner image
        string winnerImageFilename = Bus_Organization.GetNewWinnerImageFilename();

        // Upload the file
        HultPrizeAtCommon.UploadFile(this.RunTimeEnvironment, file, winnerImageFilename, school);

        // Update the database with the file name
        HultBusiness.Organization.WinnerImageWasUploaded(this.RunTimeEnvironment, orgId, winnerImageFilename);

        // Image was added
        Session["EditSchoolSuccessMessage"] = "School Winner Updated";

      }
      else
      {
        // No file was selected
        Session["EditSchoolErrorMessage"] = "No image was selected.";

      }

      // Return to the school edit page
      return RedirectToAction("Index", "School", new { id = orgId });
    }
    #endregion

    #region Update Winner Text
    [HttpPost]
    public ActionResult WinnerText(long schoolId, string caption)
    {
      // Get the school (will alwyas work becuase the orgId is from the school object on the edit page)
      Bus_Organization_Result result = HultBusiness.Organization.GetOrganizationInfo(this.RunTimeEnvironment, schoolId);
      Bus_Organization school = result.Organization;

      // Update the winner caption
      school.ExtraInformation.WinnerCaption = caption;

      // Save school
      HultBusiness.OrganizationExtra.SaveExtraInformation(this.RunTimeEnvironment, school.ExtraInformation);

      // Success!
      Session["EditSchoolSuccessMessage"] = "School Winner Caption Updated";

      return RedirectToAction("Index", "School", new { id = schoolId });
    }
    #endregion


    // ===========
    // DIRECTOR 
    // ===========

    #region Add Director Image
    [HttpPost]
    public ActionResult DirectorImage(HttpPostedFileBase file, long orgId)
    {

      // Verify that the user selected a file
      if (file != null && file.ContentLength > 0)
      {

        // Get the school (will alwyas work becuase the orgId is from the school object on the edit page)
        Bus_Organization_Result result = HultBusiness.Organization.GetOrganizationInfo(this.RunTimeEnvironment, orgId);
        Bus_Organization school = result.Organization;

        // Get the file name of the image
        string imageFileName = Bus_Organization.GetNewCampusDirectorImageFilename();

        // Upload the file
        HultPrizeAtCommon.UploadFile(this.RunTimeEnvironment, file, imageFileName, school);

        // Update the database with the file name
        HultBusiness.Organization.CampusDirectorImageWasUploaded(this.RunTimeEnvironment, orgId, imageFileName);

        // No file was selected
        Session["EditSchoolSuccessMessage"] = "Campus Director Image Added";
      }
      else
      {
        // No file was selected
        Session["EditSchoolErrorMessage"] = "No image was selected.";
      }

      // Return to the school edit page
      return RedirectToAction("Index", "School", new { id = orgId });
    }
    #endregion

    #region Update Director's Message
    [HttpPost]
    public ActionResult DirectorMessage(string message, long orgId)
    {
      // Get the school (will alwyas work becuase the orgId is from the school object on the edit page)
      Bus_Organization_Result result = HultBusiness.Organization.GetOrganizationInfo(this.RunTimeEnvironment, orgId);
      Bus_Organization school = result.Organization;

      // Update the school message
      school.ExtraInformation.DirectorMessage = message;

      // Save the school
      HultBusiness.OrganizationExtra.SaveExtraInformation(this.RunTimeEnvironment, school.ExtraInformation);

      // Set session message
      Session["EditSchoolSuccessMessage"] = "Successfully updated director's message.";

      // Return to the school edit page
      return RedirectToAction("Index", "School", new { id = orgId });
    }
    #endregion

    // ===========
    // EVENTS 
    // ===========

    #region Add New Event
    [HttpPost]
    public ActionResult NewEvent(long orgId, string title, string date)
    {
      // Conver to datetime object
      DateTime eventDateTime = Convert.ToDateTime(date);

      // Create the new judge
      HultBusiness.Events.CreateEvent(this.RunTimeEnvironment, orgId, title, eventDateTime);

      // Set session message
      Session["EditSchoolSuccessMessage"] = "Successfully added event.";

      // Redirect to list of schools page
      return RedirectToAction("Index", "School", new { id = orgId });
    }
    #endregion

    #region Save Event
    [HttpPost]
    public ActionResult SaveEvent(Bus_Event e, string date)
    {
      // Convert to datetime object
      DateTime eventDateTime = Convert.ToDateTime(date);

      // Set the date to the event
      e.EventDtime = eventDateTime;

      // Save the judge
      HultBusiness.Events.SaveEvent(this.RunTimeEnvironment, e);

      // Set session message
      Session["EditSchoolSuccessMessage"] = "Successfully saved event.";

      // Redirect to edit school page
      return RedirectToAction("Index", "School", new { id = e.OrganizationId });
    }
    #endregion

    #region Get Event Info
    [HttpPost]
    public ActionResult Event(long eventId)
    {

      // Get the staff result
      Bus_Event_Result result = HultBusiness.Events.GetEventInfo(this.RunTimeEnvironment, eventId);

      // Return the result
      return Json(result);
    }
    #endregion

    #region Remove Event
    [HttpPost]
    public ActionResult RemoveEvent(long schoolId, long eventId)
    {
      // Remove the judge
      HultBusiness.Events.DeleteEvent(this.RunTimeEnvironment, eventId);

      // Set session message
      Session["EditSchoolSuccessMessage"] = "Successfully removed Event.";

      // Redirect to edit school page
      return RedirectToAction("Index", "School", new { id = schoolId });
    }
    #endregion

    // ===========
    // JUDGES 
    // ===========

    #region Get Judge Info
    [HttpPost]
    public ActionResult Judge(long judgeId)
    {

      // Get the judge result
      Bus_Judge_Result result = HultBusiness.Judges.GetJudgeInfo(this.RunTimeEnvironment, judgeId);

      // Return the result
      return Json(result);
    }
    #endregion

    #region Add New Judge
    [HttpPost]
    public ActionResult NewJudge(long orgId, string firstName, string lastName, string title)
    {

      // Create the new judge
      HultBusiness.Judges.CreateJudge(this.RunTimeEnvironment, orgId, firstName, lastName, title);

      // Set session message
      Session["EditSchoolSuccessMessage"] = "Successfully added judge.";

      // Redirect to edit school page
      return RedirectToAction("Index", "School", new { id = orgId });
    }
    #endregion

    #region Remove Judge
    [HttpPost]
    public ActionResult RemoveJudge(long orgId, long judgeId)
    {
      // Remove the judge
      HultBusiness.Judges.DeleteJudge(this.RunTimeEnvironment, judgeId);

      // Set session message
      Session["EditSchoolSuccessMessage"] = "Successfully removed judge.";

      // Redirect to edit school page
      return RedirectToAction("Index", "School", new { id = orgId });
    }
    #endregion

    #region Save Judge
    [HttpPost]
    public ActionResult SaveJudge(Bus_Judge judge)
    {
      // Save the judge
      HultBusiness.Judges.SaveJudge(this.RunTimeEnvironment, judge);

      // Set session message
      Session["EditSchoolSuccessMessage"] = "Successfully saved judge.";

      // Redirect to edit school page
      return RedirectToAction("Index", "School", new { id = judge.OrganizationId });
    }
    #endregion

    #region Add Judge Image
    [HttpPost]
    public ActionResult JudgeImage(HttpPostedFileBase file, long orgId, long judgeId)
    {

      // Verify that the user selected a file
      if (file != null && file.ContentLength > 0)
      {

        // Get the school (will alwyas work becuase the orgId is from the school object on the edit page)
        Bus_Organization_Result result = HultBusiness.Organization.GetOrganizationInfo(this.RunTimeEnvironment, orgId);
        Bus_Organization school = result.Organization;

        // Get the file name of the image
        string imageFileName = Bus_Judge.GetNewImageFilename();

        // Upload the file
        HultPrizeAtCommon.UploadFile(this.RunTimeEnvironment, file, imageFileName, school);

        // Update the database with the file name
        HultBusiness.Judges.ImageWasUploaded(this.RunTimeEnvironment, judgeId, imageFileName);

        // No file was selected
        Session["EditSchoolSuccessMessage"] = "Judge Image Added";
      }
      else
      {
        // No file was selected
        Session["EditSchoolErrorMessage"] = "No image was selected.";
      }

      // Return to the school edit page
      return RedirectToAction("Index", "School", new { id = orgId });
    }
    #endregion

    #region Move Judge Up
    [HttpPost]
    public ActionResult MoveJudgeUp(long judgeId)
    {
      // Get the judge (it will always have a judge since it's coming from the judge object on the edit school page
      Bus_Judge_Result result = HultBusiness.Judges.GetJudgeInfo(this.RunTimeEnvironment,judgeId);
      Bus_Judge judge = result.Judge;

      // Move judge up
      HultBusiness.Judges.MoveJudgeUpInDisplayOrder(this.RunTimeEnvironment, judge);

      // Get the new school object
      Bus_Organization_Result orgResult = HultBusiness.Organization.GetOrganizationInfo(this.RunTimeEnvironment, judge.OrganizationId);
      Bus_Organization school = orgResult.Organization;

      // Return new school object
      return Json(school);
    }
    #endregion

    #region Move Judge Down
    [HttpPost]
    public ActionResult MoveJudgeDown(long judgeId)
    {
      // Get the judge (it will always have a judge since it's coming from the judge object on the edit school page
      Bus_Judge_Result result = HultBusiness.Judges.GetJudgeInfo(this.RunTimeEnvironment, judgeId);
      Bus_Judge judge = result.Judge;

      // Move judge up
      HultBusiness.Judges.MoveJudgeDownInDisplayOrder(this.RunTimeEnvironment, judge);

      // Get the new school object
      Bus_Organization_Result orgResult = HultBusiness.Organization.GetOrganizationInfo(this.RunTimeEnvironment, judge.OrganizationId);
      Bus_Organization school = orgResult.Organization;

      // Return new school object
      return Json(school);
    }
    #endregion

    // ===========
    // STAFF 
    // ===========

    #region Get Staff Info
    [HttpPost]
    public ActionResult Staff(long staffId)
    {

      // Get the staff result
      Bus_Staff_Result result = HultBusiness.Staff.GetStaffInfo(this.RunTimeEnvironment, staffId);

      // Return the result
      return Json(result);
    }
    #endregion

    #region Add New Staff Member
    [HttpPost]
    public ActionResult NewStaff(long orgId, string firstName, string lastName, string position)
    {

      // Create the new judge
      HultBusiness.Staff.CreateStaff(this.RunTimeEnvironment, orgId, firstName, lastName, position);

      // Set session message
      Session["EditSchoolSuccessMessage"] = "Successfully added staff member.";

      // Redirect to list of schools page
      return RedirectToAction("Index", "School", new { id = orgId });
    }
    #endregion

    #region Remove Staff
    [HttpPost]
    public ActionResult RemoveStaff(long orgId, long staffId)
    {
      // Remove the judge
      HultBusiness.Staff.DeleteStaff(this.RunTimeEnvironment, staffId);

      // Set session message
      Session["EditSchoolSuccessMessage"] = "Successfully removed staff member.";

      // Redirect to edit school page
      return RedirectToAction("Index", "School", new { id = orgId });
    }
    #endregion

    #region Save Staff Member
    [HttpPost]
    public ActionResult SaveStaff(Bus_Staff staff)
    {
      // Save the staff memeber
      HultBusiness.Staff.SaveStaff(this.RunTimeEnvironment, staff);

      // Set session message
      Session["EditSchoolSuccessMessage"] = "Successfully saved staff member.";

      // Redirect to edit school page
      return RedirectToAction("Index", "School", new { id = staff.OrganizationId });
    }
    #endregion

    #region Add Staff Image
    [HttpPost]
    public ActionResult StaffImage(HttpPostedFileBase file, long orgId, long staffId)
    {

      // Verify that the user selected a file
      if (file != null && file.ContentLength > 0)
      {

        // Get the school (will alwyas work becuase the orgId is from the school object on the edit page)
        Bus_Organization_Result result = HultBusiness.Organization.GetOrganizationInfo(this.RunTimeEnvironment, orgId);
        Bus_Organization school = result.Organization;

        // Get the file name of the image
        string imageFileName = Bus_Staff.GetNewImageFilename();

        // Upload the file
        HultPrizeAtCommon.UploadFile(this.RunTimeEnvironment, file, imageFileName, school);

        // Update the database with the file name
        HultBusiness.Staff.ImageWasUploaded(this.RunTimeEnvironment, staffId, imageFileName);

        // No file was selected
        Session["EditSchoolSuccessMessage"] = "Staff Member Image Added";
      }
      else
      {
        // No file was selected
        Session["EditSchoolErrorMessage"] = "No image was selected.";
      }

      // Return to the school edit page
      return RedirectToAction("Index", "School", new { id = orgId });
    }
    #endregion

    #region Move Staff Up
    [HttpPost]
    public ActionResult MoveStaffUp(long staffId)
    {
      // Get the staff (it will always have a staff since it's coming from the staff object on the edit school page
      Bus_Staff_Result result = HultBusiness.Staff.GetStaffInfo(this.RunTimeEnvironment, staffId);
      Bus_Staff staff = result.Staff;

      // Move staff up
      HultBusiness.Staff.MoveStaffUpInDisplayOrder(this.RunTimeEnvironment, staff);

      // Get the new school object
      Bus_Organization_Result orgResult = HultBusiness.Organization.GetOrganizationInfo(this.RunTimeEnvironment, staff.OrganizationId);
      Bus_Organization school = orgResult.Organization;

      // Return new school object
      return Json(school);
    }
    #endregion

    #region Move Staff Down
    [HttpPost]
    public ActionResult MoveStaffDown(long staffId)
    {
      // Get the staff (it will always have a staff since it's coming from the staff object on the edit school page
      Bus_Staff_Result result = HultBusiness.Staff.GetStaffInfo(this.RunTimeEnvironment, staffId);
      Bus_Staff staff = result.Staff;

      // Move staff up
      HultBusiness.Staff.MoveStaffDownInDisplayOrder(this.RunTimeEnvironment, staff);

      // Get the new school object
      Bus_Organization_Result orgResult = HultBusiness.Organization.GetOrganizationInfo(this.RunTimeEnvironment, staff.OrganizationId);
      Bus_Organization school = orgResult.Organization;

      // Return new school object
      return Json(school);
    }
    #endregion

    // ===========
    // SPONSORS 
    // ===========

    #region Get Sponsor Info
    [HttpPost]
    public ActionResult Sponsor(long sponsorId)
    {

      // Get the staff result
      Bus_Sponsor_Result result = HultBusiness.Sponsors.GetSponsorInfo(this.RunTimeEnvironment, sponsorId);

      // Return the result
      return Json(result);
    }
    #endregion

    #region Add New Sponsor
    [HttpPost]
    public ActionResult NewSponsor(long orgId, string name)
    {

      // Create the new judge
      HultBusiness.Sponsors.CreateSponsor(this.RunTimeEnvironment, orgId, name);

      // Set session message
      Session["EditSchoolSuccessMessage"] = "Successfully added sponsor.";

      // Redirect to list of schools page
      return RedirectToAction("Index", "School", new { id = orgId });
    }
    #endregion

    #region Remove Sponsor
    [HttpPost]
    public ActionResult RemoveSponsor(long orgId, long sponsorId)
    {
      // Remove the judge
      HultBusiness.Sponsors.DeleteSponsor(this.RunTimeEnvironment, sponsorId);

      // Set session message
      Session["EditSchoolSuccessMessage"] = "Successfully removed sponsor.";

      // Redirect to edit school page
      return RedirectToAction("Index", "School", new { id = orgId });
    }
    #endregion

    #region Save Sponsor
    [HttpPost]
    public ActionResult SaveSponsor(Bus_Sponsor sponsor)
    {
      // Save the staff memeber
      HultBusiness.Sponsors.SaveSponsor(this.RunTimeEnvironment, sponsor);

      // Set session message
      Session["EditSchoolSuccessMessage"] = "Successfully saved sponsor.";

      // Redirect to edit school page
      return RedirectToAction("Index", "School", new { id = sponsor.OrganizationId });
    }
    #endregion

    #region Add Sponsor Image
    [HttpPost]
    public ActionResult SponsorImage(HttpPostedFileBase file, long orgId, long sponsorId)
    {

      // Verify that the user selected a file
      if (file != null && file.ContentLength > 0)
      {

        // Get the school (will alwyas work becuase the orgId is from the school object on the edit page)
        Bus_Organization_Result result = HultBusiness.Organization.GetOrganizationInfo(this.RunTimeEnvironment, orgId);
        Bus_Organization school = result.Organization;

        // Get the file name of the image
        string imageFileName = Bus_Sponsor.GetNewImageFilename();

        // Upload the file
        HultPrizeAtCommon.UploadFile(this.RunTimeEnvironment, file, imageFileName, school);

        // Update the database with the file name
        HultBusiness.Sponsors.ImageWasUploaded(this.RunTimeEnvironment, sponsorId, imageFileName);

        // No file was selected
        Session["EditSchoolSuccessMessage"] = "Sponsor Image Added";
      }
      else
      {
        // No file was selected
        Session["EditSchoolErrorMessage"] = "No image was selected.";
      }

      // Return to the school edit page
      return RedirectToAction("Index", "School", new { id = orgId });
    }
    #endregion

    #region Move Sponsor Up
    [HttpPost]
    public ActionResult MoveSponsorUp(long sponsorId)
    {
      // Get the staff (it will always have a sponsor since it's coming from the sponsor object on the edit school page
      Bus_Sponsor_Result result = HultBusiness.Sponsors.GetSponsorInfo(this.RunTimeEnvironment, sponsorId);
      Bus_Sponsor sponsor = result.Sponsor;

      // Move staff up
      HultBusiness.Sponsors.MoveSponsorUpInDisplayOrder(this.RunTimeEnvironment, sponsor);

      // Get the new school object
      Bus_Organization_Result orgResult = HultBusiness.Organization.GetOrganizationInfo(this.RunTimeEnvironment, sponsor.OrganizationId);
      Bus_Organization school = orgResult.Organization;

      // Return new school object
      return Json(school);
    }
    #endregion

    #region Move Sponsor Down
    [HttpPost]
    public ActionResult MoveSponsorDown(long sponsorId)
    {
      // Get the staff (it will always have a sponsor since it's coming from the sponsor object on the edit school page
      Bus_Sponsor_Result result = HultBusiness.Sponsors.GetSponsorInfo(this.RunTimeEnvironment, sponsorId);
      Bus_Sponsor sponsor = result.Sponsor;

      // Move staff up
      HultBusiness.Sponsors.MoveSponsorDownInDisplayOrder(this.RunTimeEnvironment, sponsor);

      // Get the new school object
      Bus_Organization_Result orgResult = HultBusiness.Organization.GetOrganizationInfo(this.RunTimeEnvironment, sponsor.OrganizationId);
      Bus_Organization school = orgResult.Organization;

      // Return new school object
      return Json(school);
    }
    #endregion


    // ===========
    // PRESS 
    // ===========

    #region Get Press Info
    [HttpPost]
    public ActionResult Press(long pressId)
    {

      // Get the result
      Bus_Press_Result result = HultBusiness.Press.GetPressInfo(this.RunTimeEnvironment, pressId);

      // Return the result
      return Json(result);
    }
    #endregion

    #region Add New Press
    [HttpPost]
    public ActionResult NewPress(long orgId, string title, string url)
    {

      // Create the new press
      WebCommon.Business.BusinessResult result = HultBusiness.Press.CreatePress(this.RunTimeEnvironment, orgId, title, url);

      // Check if it is successful;
      if (result.Success)
      {
        // Set session message
        Session["EditSchoolSuccessMessage"] = "Successfully added press item.";
      }
      else
      {
        // There was an error
        Session["EditSchoolErrorMessage"] = result.ErrorDisplayMessage;
      }

      // Redirect to list of schools page
      return RedirectToAction("Index", "School", new { id = orgId });
    }
    #endregion

    #region Remove Press
    [HttpPost]
    public ActionResult RemovePress(long orgId, long pressId)
    {
      // Remove the press item
      HultBusiness.Press.DeletePress(this.RunTimeEnvironment, pressId);

      // Set session message
      Session["EditSchoolSuccessMessage"] = "Successfully removed press item.";

      // Redirect to edit school page
      return RedirectToAction("Index", "School", new { id = orgId });
    }
    #endregion

    #region Save Press
    [HttpPost]
    public ActionResult SavePress(Bus_Press press)
    {
      // Save the staff memeber
      HultBusiness.Press.SavePress(this.RunTimeEnvironment, press);

      // Set session message
      Session["EditSchoolSuccessMessage"] = "Successfully saved press.";

      // Redirect to edit school page
      return RedirectToAction("Index", "School", new { id = press.OrganizationId });
    }
    #endregion

    #region Move Press Up
    [HttpPost]
    public ActionResult MovePressUp(long pressId)
    {
      // Get the press (it will always have a press since it's coming from the press object on the edit school page
      Bus_Press_Result result = HultBusiness.Press.GetPressInfo(this.RunTimeEnvironment, pressId);
      Bus_Press press = result.Press;

      // Move press up
      HultBusiness.Press.MovePressUpInDisplayOrder(this.RunTimeEnvironment, press);

      // Get the new school object
      Bus_Organization_Result orgResult = HultBusiness.Organization.GetOrganizationInfo(this.RunTimeEnvironment, press.OrganizationId);
      Bus_Organization school = orgResult.Organization;

      // Return new school object
      return Json(school);
    }
    #endregion

    #region Move Press Down
    [HttpPost]
    public ActionResult MovePressDown(long pressId)
    {
      // Get the press (it will always have a press since it's coming from the press object on the edit school page
      Bus_Press_Result result = HultBusiness.Press.GetPressInfo(this.RunTimeEnvironment, pressId);
      Bus_Press press = result.Press;

      // Move press down
      HultBusiness.Press.MovePressDownInDisplayOrder(this.RunTimeEnvironment, press);

      // Get the new school object
      Bus_Organization_Result orgResult = HultBusiness.Organization.GetOrganizationInfo(this.RunTimeEnvironment, press.OrganizationId);
      Bus_Organization school = orgResult.Organization;

      // Return new school object
      return Json(school);
    }
    #endregion

    #region Get Meta Tags
    [HttpGet]
    public ActionResult PressUrlMeta(string url)
    {
      List<string> metaTags = new List<string>();
      if (url.Length > 0)
      {
        // Get list of meta tags
        metaTags = HultPrizeAtCommon.GetMetaTags(url);
      }

      // Return in Json format
      return Json(metaTags,JsonRequestBehavior.AllowGet);
    }
    #endregion

    // ===========
    // MISC
    // ===========

    #region Page Activation
    [HttpPost]
    public ActionResult ActivatePage(bool activate, long orgId)
    {
        if (activate)
        {
            HultBusiness.Organization.ActivatePage(this.RunTimeEnvironment, orgId);
        }
        else
        {
            HultBusiness.Organization.DeactivatePage(this.RunTimeEnvironment, orgId);
        }

        return Content("success");
    }
    #endregion

    #region Section Toggle
    [HttpPost]
    public ActionResult ToggleSection(string section, bool on, long id)
    {
        List<Bus_Organization> schoolsForUser = HultBusiness.Organization.GetUserOrganizations(this.RunTimeEnvironment, HultPrizeAtApplication.CurrentUser.UserId);
        Bus_Organization currSchool = HultBusiness.Organization.GetOrganizationInfo(this.RunTimeEnvironment, id).Organization;

        switch (section)
        {
            case "directorSectionVisible":
                currSchool.ExtraInformation.ShowDirectorMessage_Bool = on;
                break;
            case "eventSectionVisible":
                currSchool.ExtraInformation.ShowEvents_Bool = on;
                break;
            case "judgesSectionVisible":
                currSchool.ExtraInformation.ShowJudges_Bool = on;
                break;
            case "staffSectionVisible":
                currSchool.ExtraInformation.ShowStaff_Bool = on;
                break;
            case "sponsorSectionVisible":
                currSchool.ExtraInformation.ShowSponsor_Bool = on;
                break;
            case "pressSectionVisible":
                currSchool.ExtraInformation.ShowPress_Bool = on;
                break;
            case "registrationFormSectionVisible":
                currSchool.ExtraInformation.ShowRegistrationForm_Bool = on;
                break;
            case "winnerSectionVisible":
                currSchool.ExtraInformation.ShowWinner_Bool = on;
                break;
        }

        HultBusiness.OrganizationExtra.SaveExtraInformation(this.RunTimeEnvironment, currSchool.ExtraInformation);

        return Content("success");
    }
    #endregion

  }
}