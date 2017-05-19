using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Compilation;
using System.Web.Routing;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Web.UI;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

/* Adding Greg's back end*/
using FifthTribe.Database.HultPrize;
using FifthTribe.HultPrize;
using FifthTribe.Common;
using FifthTribe.WebServices.Common;
using FifthTribe.WebServices.FileServer;


namespace HultPrizeAtAdmin.Common
{
  public class HultPrizeAtCommon
  {

    // This is a method to set/renew cookie
    #region RenewCookie
    public static void RenewCookie(HttpResponseBase response, string username, Bus_User authUser = null)
    {
      FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, username, DateTime.Now, DateTime.Now.AddHours(2), true, string.Empty, FormsAuthentication.FormsCookiePath);

      // Encrypt the ticket.
      string encTicket = FormsAuthentication.Encrypt(ticket);

      // Create the cookie.
      HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
      cookie.Expires = DateTime.Now.AddHours(2);
      response.Cookies.Add(cookie);

      if (authUser == null) return;

      JavaScriptSerializer jsonSerialiser = new JavaScriptSerializer();
      string authUserString = jsonSerialiser.Serialize(authUser);

      try
      {
        HttpCookie userCookie = new HttpCookie("HultUser", authUserString.EncodeTo64());
        userCookie.Expires = DateTime.Now.AddHours(2);
        response.Cookies.Add(userCookie);
      }
      catch (Exception) { }
    }
    #endregion

    // This is to upload a file to the server
    #region Upload File
    public static void UploadFile(int currentEnvironment, HttpPostedFileBase file, string filename, Bus_Organization school)
    {
      try
      {
        // Get File contents to upload
        byte[] fileBytes = new byte[file.InputStream.Length];
        int byteCount = file.InputStream.Read(fileBytes, 0, (int)file.InputStream.Length);
        string fileContentBase64 = Convert.ToBase64String(fileBytes);

        // Get the server name
        string serverName = HultCommon.EnvironmentalValues.ServerName(currentEnvironment);

        // Get the local server path
        string localServerPath = Path.Combine(school.OrganizationDirectory_LocalFilePath(currentEnvironment), filename);

        // get a security token
        SecurityToken token = HultBusiness.CreateSecurityToken(0, filename);

        // Upload the image to the server
        FileServerBusiness.TransmitFileToServer(token, fileContentBase64, serverName, localServerPath, false);
      }
      catch (Exception ex)
      {
        throw new Exception(MethodBase.GetCurrentMethod().Name + ":" + Environment.NewLine, ex);
      }
    }
    #endregion

    // This is to build the url path to the asset
    #region BuildAssetURL
    public static string BuildAssetURL(string directory, string filename)
    {
      return Path.Combine(directory, filename);
    }
    #endregion

    #region Load URL and Get Meta Tags


    #region get the html of a url
    private static string AcquireHTML(string address)
    {
      HttpWebRequest request;
      HttpWebResponse response = null;
      StreamReader reader;
      StringBuilder sbSource;

      try
      {
        // Create and initialize the web request  
        request = System.Net.WebRequest.Create(address) as HttpWebRequest;
        request.UserAgent = "your-search-bot";
        request.KeepAlive = false;
        request.Timeout = 10 * 1000;

        // Get response  
        response = request.GetResponse() as HttpWebResponse;

        if (request.HaveResponse == true && response != null)
        {
          // Get the response stream  
          reader = new StreamReader(response.GetResponseStream());

          // Read it into a StringBuilder  
          sbSource = new StringBuilder(reader.ReadToEnd());

          response.Close();

          // Console application output  
          return sbSource.ToString();
        }
        else
          return "";
      }
      catch (Exception ex)
      {
        response.Close();
        return "";
      }
    }
    #endregion

    #region Get Meta Tags
    public static List<string> GetMetaTags(string url)
    {
      try
      {
        // Create list
        List<string> metaTags = new List<string>();

        if (url.Length > 0)
        {
          // Get the html from the url
          string strIn = AcquireHTML(url);

          // Create HTML doc
          HtmlDocument htmlDoc = new HtmlDocument();
          htmlDoc.LoadHtml(strIn);

          // Get the description from the html
          HtmlNode descriptionNode = htmlDoc.DocumentNode.SelectSingleNode("//meta[@name='description']");
          if (descriptionNode != null)
          {
            HtmlAttribute desc = descriptionNode.Attributes["content"];
            string metaDescription = desc.Value;

            // Add to the list
            metaTags.Add(metaDescription);
          }
          else
          {
            metaTags.Add("No Description Found");
          }

          // Get the og image from the html
          HtmlNode imageNode = htmlDoc.DocumentNode.SelectSingleNode("//meta[@property='og:image']");
          if (imageNode != null)
          {
            HtmlAttribute image = imageNode.Attributes["content"];
            string metaImage = image.Value;

            // Add to the list
            metaTags.Add(metaImage);
          }
          else
          {
            metaTags.Add("No Image Found");
          }
        }

        // Return the list
        return metaTags;
      }
      catch (Exception ex)
      {
        throw new Exception(MethodBase.GetCurrentMethod().Name + ":" + Environment.NewLine, ex);
      }
    }
    #endregion

    #endregion

  }
}