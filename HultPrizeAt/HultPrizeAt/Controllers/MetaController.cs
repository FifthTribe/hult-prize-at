using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using HultPrizeAt.Common;

namespace HultPrizeAt.Controllers
{
  public class MetaController : Controller
  {


    #region Get Meta Tags
    [HttpGet]
    public ActionResult Index(string url)
    {
      // Get list of meta tags
      List<string> metaTags = HultPrizeAtCommon.GetMetaTags(url);

      // Return in Json format
      return Json(metaTags, JsonRequestBehavior.AllowGet);
    }
    #endregion

  }
}
