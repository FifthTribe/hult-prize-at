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

namespace HultPrizeAt.Common
{
  public class HultPrizeAtCommon
  {

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

        if ( url != null && url != "" )
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