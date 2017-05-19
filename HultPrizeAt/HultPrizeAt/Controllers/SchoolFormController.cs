using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FifthTribe.HultPrize;
using FifthTribe.Common;

namespace HultPrizeAt.Controllers
{
  public class SchoolFormController : BaseController
  {

    [HttpPost]
    public ActionResult Index(FormCollection collection)
    {
      string startUpIdea = collection.Get("startUpIdea");
      string teamName = collection.Get("teamName");

      long schoolOrganizationId = long.Parse(collection.Get("orgId"));

      HultBusiness.TeamRegistrationObject reg = new HultBusiness.TeamRegistrationObject(schoolOrganizationId, teamName, startUpIdea);

      CommonTelephone phone;

      string member1FirstName = collection.Get("member1FirstName");
      string member1LastName = collection.Get("member1LastName");
      string member1Email = collection.Get("member1Email");
      string member1LinkedInUrl = collection.Get("member1LinkedInUrl");
      phone = getPhoneNumberFromData(collection, "member1");

      reg.AddTeamMember(member1FirstName, member1LastName, member1Email, member1LinkedInUrl, phone);

      string member2FirstName = collection.Get("member2FirstName");
      string member2LastName = collection.Get("member2LastName");
      string member2Email = collection.Get("member2Email");
      string member2LinkedInUrl = collection.Get("member2LinkedInUrl");
      phone = getPhoneNumberFromData(collection, "member2");
      reg.AddTeamMember(member2FirstName, member2LastName, member2Email, member2LinkedInUrl, phone);

      string member3FirstName = collection.Get("member3FirstName");
      string member3LastName = collection.Get("member3LastName");
      string member3Email = collection.Get("member3Email");
      string member3LinkedInUrl = collection.Get("member3LinkedInUrl");
      phone = getPhoneNumberFromData(collection, "member3");
      reg.AddTeamMember(member3FirstName, member3LastName, member3Email, member3LinkedInUrl, phone);

      string member4FirstName = collection.Get("member4FirstName");
      string member4LastName = collection.Get("member4LastName");
      string member4Email = collection.Get("member4Email");
      string member4LinkedInUrl = collection.Get("member4LinkedInUrl");
      phone = getPhoneNumberFromData(collection, "member4");
      if (member4FirstName != null && !member4FirstName.Equals(""))
      {
        reg.AddTeamMember(member4FirstName, member4LastName, member4Email, member4LinkedInUrl, phone);
      }

      /*string member5FirstName = collection.Get("member5FirstName");
      string member5LastName = collection.Get("member5LastName");
      string member5Email = collection.Get("member5Email");
      phone = getPhoneNumberFromData(collection, "member5");

      if (member5FirstName != null && !member5FirstName.Equals(""))
      {
        reg.AddTeamMember(member5FirstName, member5LastName, member5Email, phone);
      }*/

      WebCommon.Business.BusinessResult result = HultBusiness.Registrations.RegisterTeam(this.RunTimeEnvironment, reg);

      // if it's success
      if ( result.Success )
      {
        Bus_Organization_Result org = HultBusiness.Organization.GetOrganizationInfo(this.RunTimeEnvironment, schoolOrganizationId);

        if ( org.Success)
        {
          // Send emails to team members
          string toTeamSubject = "Congratulations, you have successfully registered for your Hult Prize@ event!";
          string toTeamMessage = "Congratulations, you have successfully registered for your Hult Prize@ event! Your campus director will be in touch soon with further directions, and make sure to keep an eye on your school's page for event updates!";

          foreach (Bus_RegistrationMember teamMember in reg.TeamMembers)
          {
            HultBusiness.SendEmail(teamMember.Email, toTeamSubject, toTeamMessage, HultCommon.ORGANIZATION_ADMIN_INVITE_EMAIL_FROM_ADDRESS, "Hult Prize at");
          }

          // Send email to school director
          Bus_Organization school = org.Organization;
          string toDirectorSubject = "A new team registered for Hult Prize at " + school.OrganizationName;
          string toDirectorMessage = "Hey "+ school.CampusDirectorInfo.NameFirst + ", you have a new team registration! Please login to " + HultCommon.EnvironmentalValues.HultAdminDomain(this.RunTimeEnvironment) + " to see full details.";

          HultBusiness.SendEmail(org.Organization.CampusDirectorInfo.Email, toDirectorSubject, toDirectorMessage, HultCommon.ORGANIZATION_ADMIN_INVITE_EMAIL_FROM_ADDRESS, "Hult Prize at");

        }


      }



      JsonResult obj = Json(result);

      return Json(result);
    }

    public CommonTelephone getPhoneNumberFromData(FormCollection collection, String memberNumber)
    {
      CommonTelephone phone = new CommonTelephone();
      string isInternational = collection.Get(memberNumber + "PhoneInternationalBox");

      string extension = collection.Get(memberNumber + "PhoneExtension");

      if (isInternational != null && isInternational.Equals("on"))
      {
        string countryCode = collection.Get(memberNumber + "PhoneCountryCode");
        string number = collection.Get(memberNumber + "PhoneInternational");
        phone.CountryCode = countryCode;
        phone.IsInternational = true;
        phone.InternationalPhoneNumber = number;
        phone.Extension = extension;
      }
      else
      {
        string areaCode = collection.Get(memberNumber + "PhoneAreaCode");
        string firstThreeDigits = collection.Get(memberNumber + "Phonefirstthree");
        string lastFourDigits = collection.Get(memberNumber + "Phonelastfour");
        phone.AreaCode = areaCode;
        phone.FirstThree = firstThreeDigits;
        phone.LastFour = lastFourDigits;
        phone.IsInternational = false;
      }

      return phone;
    }
  }
}