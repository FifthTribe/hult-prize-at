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
  public class MessagesController : BaseController
  {
 
    #region List of messages for School Campus Director
    public ActionResult Index()
    {

      // Get list of announcements for campus director
      ViewBag.Announcements = HultBusiness.Announcements.GetAnnouncementList(this.RunTimeEnvironment, null, HultCommon.AnnouncementRecipientTypes.SchoolAdmins.IntegerValue, null, true);

      return View();
    }
    #endregion

  }
}