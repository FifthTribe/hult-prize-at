using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

/* Amir added */
using System.Reflection;
using System.Web.Configuration;
using System.Text;

/* Adding Greg's back end*/
using FifthTribe.Database.HultPrize;
using FifthTribe.HultPrize;
using FifthTribe.Common;

namespace HultPrizeAt.Controllers
{
  public class HomeController : BaseController
  {

    // Homepage
    #region homepage
    public ActionResult Index(string id = "")
    {
      // Send environment to view bag
      ViewBag.RunTimeEnvironment = this.RunTimeEnvironment;

        
      if (!string.IsNullOrWhiteSpace(id))
      {
        // Bool var for preview
        bool preview = false;

        // Check if they are previewing from the admin
        if (id.Contains("_preview"))
        {
          // It's a preview
          preview = true;
          
          // Get the school id
          id = id.Split('_')[0];
        }

        // Check if there is a school that exists
        Bus_Organization_Result org = HultBusiness.Organization.GetOrganizationInfo(this.RunTimeEnvironment, id, preview);

        // If the school exists
        if (org.Success)
        {
          ViewBag.School = org.Organization;
          return View();
        }
        else
        {
          // Handle all non-school URLs (lowercase the url)
          id.ToLower();

          // Route the user where they need to go
          switch (id)
          {
            case "schoolform":
              return RedirectToAction("Index", "SchoolForm");
            case "home/process":
              return RedirectToAction("Process");
            default:
              return RedirectToAction("Index", "Error");
          }

        }
      }
      else
      {
        return View();
      }
    }
    #endregion

    // Grabbing the URL user enters and sends them where they need to go
    #region URL Routing
    /*public ActionResult Handle(string id)
    {

      // Route the user where they need to go
      switch (id)
      {
        case "SchoolForm":
          return RedirectToAction("Index", "SchoolForm");
    
        default:
          return RedirectToAction("Index", "Error");
      }

    }*/
    #endregion


    // Process the main homepage form
    #region Process from the main homepage form
    [HttpPost]
    public ActionResult Process(string email, string name, string school)
    {
      // Create message object for the message to the student
      StringBuilder message = new StringBuilder();

      // Create message for student
      message.Append("Dear Friend,<br />");
      message.Append("We would like to thank you for your interest in bringing our latest program, Hult Prize@ to your university. With over 100 currently active Hult Prize@ chapters we are aggressively looking to triple in size and are recruiting over 200 new campus directors. The 2016 Hult Prize, our 7th annual challenge, will be announced summer 2015. Later this year, we will reach back out to you as we open our 2016 applications for the campus director roles.<br />");
      message.Append("<br />");
      message.Append("For your reference only, below you will find last year's application form and campus director overview.<br />");
      message.Append("<br />");
      message.Append("Here is the link to the application: <a href=\"https://hultglobalcasechallenge.wufoo.eu/forms/hult-prize-at-campus-director-application/\" target=\"_blank\">https://hultglobalcasechallenge.wufoo.eu/forms/hult-prize-at-campus-director-application/</a><br />");
      message.Append("Here is the link to download the campus director overview: <a href=\"http://www.hultprizeat.com/files/Campus_Director_Manual%20_v3.1.pdf\" target=\"_blank\">http://www.hultprizeat.com/files/Campus_Director_Manual%20_v3.1.pdf</a><br />");
      message.Append("<br />");
      message.Append("I founded the Hult Prize six years ago with one simple vision: introducing the idea that profitable and sustainable businesses were the way forward if we are going to make significant progress in solving the world's toughest social challenges. What started as an awareness movement, has quickly turned into the world's largest student platform for impact. To think that six years ago, I didn't event know what a social enterprise was. Hardly any success stories to talk about, and very little seed capital to launch audacious and disruptive ideas that could lead to a real breakthrough.<br />");
      message.Append("<br />");
      message.Append("Today, with the help of organizations like our partners the Clinton Global Initiative and Net Impact, social enterprise is as real and vibrant as it has ever been. Non-usual suspects are entering the market. Start-ups that aim to be disruptive are being launched by i-bankers, consultants and rogue entrepreneurs who have identified this unbelievable market segment that is bursting at the seams for innovation. Through the Hult Prize, every single student on the planet has a chance to come up with an idea and get the support they need to launch and scale through funding, training, mentorship and most important, meeting peers and joining a community of next generation leaders who are dedicated to the pursuit of impact.<br />");
      message.Append("<br />");
      message.Append("Join us as we build the next big company, designed for impact. Help us launch the next generation of social entrepreneurs by becoming a campus director and leave your mark on the world.<br />");
      message.Append("<br />");
      message.Append("We launched Hult Prize@ so that we can have 100x the impact. Those of you that have previously participated in the Hult Prize, know that once you get a taste of what this sector is all about and the opportunities that exist within, that you will never again see the world the same. Through Hult Prize@ we hope that this infection point can come sooner and go beyond the traditional boundaries of the Hult Prize and our regional final locations. By opening up the Hult Prize to you and your campuses, we hope to reach more students, and through your support we can impact their life trajectories so that no matter what careers they pursuit or companies they launch, that they will forever ask: what can I do to change the world?<br />");
      message.Append("<br />");
      message.Append("Join us as we build the next big company, designed for impact. Help us launch the next generation of social entrepreneurs by becoming a campus director and leave your mark on the world!<br />");
      message.Append("<br />");
      message.Append("Feel free to contact us with any questions, comments or concerns.<br />");
      message.Append("<br />");
      message.Append("Best,<br />");
      message.Append("Ahmad<br />");
      message.Append("<br />");
      message.Append("Ahmad A. Ashkar<br />");
      message.Append("CEO & Founder<br />");
      message.Append("Hult Prize Foundation<br />");
      message.Append("<br />");
      message.Append("Skype ahmadashkar<br />");
      message.Append("t @askashkar<br />");
      message.Append("US +1.571.269.6151<br />");
      message.Append("DUBAI +971.(0)56.178.5398<br />");

      //HultBusiness.SendEmail("amir@fifthtribe.com", "Re: Hult Prize Campus Director", message.ToString(), HultCommon.SUBMISSION_EMAIL_FROM_ADDRESS, name, true);
      HultBusiness.SendEmail(email, "Re: Hult Prize Campus Director", message.ToString(), HultCommon.SUBMISSION_EMAIL_FROM_ADDRESS, name, true);

      // Create Message for Hult
      StringBuilder messageToHult = new StringBuilder();

      // Add the content to message
      messageToHult.Append("Name: " + name + "<br />");
      messageToHult.Append("School: " + school + "<br />");
      messageToHult.Append("Email: " + email + "<br />");

      //HultBusiness.SendEmail("amir@fifthtribe.com", school + " is interested in hosting Hult Prize", messageToHult.ToString(), HultCommon.SUBMISSION_EMAIL_FROM_ADDRESS, name, true);
      HultBusiness.SendEmail("info@hultprize.org", school + " is interested in hosting Hult Prize", messageToHult.ToString(), HultCommon.SUBMISSION_EMAIL_FROM_ADDRESS, name, true);
      //HultBusiness.SendEmail("hultprize@netimpact.org", school + " is interested in hosting Hult Prize", messageToHult.ToString(), HultCommon.SUBMISSION_EMAIL_FROM_ADDRESS, name, true);





      // Return success
      return Content("success");
    }
    #endregion


  }
}
