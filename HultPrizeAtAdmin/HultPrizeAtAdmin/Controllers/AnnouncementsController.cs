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
  public class AnnouncementsController : BaseController
  {

    #region List of announcements for Hult Admin
    public ActionResult Index()
    {

      // Get list of announcements
      ViewBag.Announcements = HultBusiness.Announcements.GetAnnouncementList(this.RunTimeEnvironment, null, null, null, false);

      return View();
    }
    #endregion

    #region Create new announcement
    [HttpPost]
    public ActionResult New(long postedByUserId, int messageType, int recipientType, string message )
    {

      // Create the announcement
      HultBusiness.Announcements.CreateAnnouncement(this.RunTimeEnvironment, postedByUserId, messageType, recipientType, message);
      
      // Set session message
      Session["AnnouncementsSuccessMessage"] = "New announcement created.";

      // Return to the announcements page
      return RedirectToAction("Index");
    }
    #endregion

    #region Delete announcement
    [HttpPost]
    public ActionResult Delete(long announcementId)
    {
      // Get the announcement result
      Bus_Announcement_Result result = HultBusiness.Announcements.GetAnnouncementInfo(this.RunTimeEnvironment, announcementId);

      // Check if it was successful
      if (result.Success)
      {

        // Delete the announcement
        HultBusiness.Announcements.DeleteAnnouncement(this.RunTimeEnvironment, result.Announcement);

        // Set session message
        Session["AnnouncementsSuccessMessage"] = "Announcement succesfully deleted.";
      }
      else
      {

        // Announcement was not found so unable to delete.
        Session["AnnouncementsErrorMessage"] = "Announcement was not found so it was unable to be removed.";

      }

      // Return to the announcements page
      return RedirectToAction("Index");
    }
    #endregion

    #region Update publish/unpublish status of an announcement
    [HttpPost]
    public ActionResult Status(long announcementId, string status)
    {
      // Get the announcement result
      Bus_Announcement_Result result = HultBusiness.Announcements.GetAnnouncementInfo(this.RunTimeEnvironment, announcementId);

      // Check if it was successful
      if (result.Success)
      {

        // Check what the status is
        switch (status)
        {
          case "publish":
            // Publish the announcement
            HultBusiness.Announcements.PublishAnnouncement(this.RunTimeEnvironment, result.Announcement);

            // Set session message
            Session["AnnouncementsSuccessMessage"] = "Announcement published.";

            break;
          case "unpublish":

            // Unpublish the announcement
            HultBusiness.Announcements.UnpublishAnnouncement(this.RunTimeEnvironment, result.Announcement);

            // Set session message
            Session["AnnouncementsSuccessMessage"] = "Announcement unpublished.";

            break;
        }

      }
      else
      {

        // Announcement was not found so unable to delete.
        Session["AnnouncementsErrorMessage"] = "Announcement was not found.";

      }

      // Return to the announcements page
      return RedirectToAction("Index");
    }
    #endregion

  }
}