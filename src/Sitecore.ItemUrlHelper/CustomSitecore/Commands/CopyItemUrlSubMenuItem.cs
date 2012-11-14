using System;
using System.Web;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Links;
using Sitecore.Resources.Media;
using Sitecore.SharedSource.Commons.Extensions;
using Sitecore.Shell.Framework.Commands;

namespace Sitecore.SharedSource.ItemUrlHelper.CustomSitecore.Commands
{
	[Serializable]
	public class CopyItemUrlSubMenuItem : Command
	{
		public override void Execute(CommandContext context)
		{
			Assert.ArgumentNotNull(context, "context");
			if (context.Items.Length != 1)
			{
				return;
			}

			//get database
			Database database = Database.GetDatabase("master");
			if (database == null)
			{
				return;
			}

			//get item from parameters
			if (context.Parameters["itemId"] == null || string.IsNullOrEmpty(context.Parameters["itemId"]))
			{
				return;
			}

			Item selectedItem = database.GetItem(context.Parameters["itemId"]);
			if (selectedItem.IsNull())
			{
				return;
			}

			//try and capture device from passed menu item command parameter
			string deviceId = string.Empty;
			if (context.Parameters["deviceId"] != null && !string.IsNullOrEmpty(context.Parameters["deviceId"]))
			{
				deviceId = context.Parameters["deviceId"];
			}

			//get item url
			string url = GetUrl(selectedItem, deviceId);

			//close context menu
			Sitecore.Context.ClientPage.ClientResponse.ClosePopups(true);

			string clientResponseText = "The Url for the selected item is: " + url;
			//if we are on IE we can copy to the clipboard
			if (UIUtil.IsIE())
			{
				//set clipboard
				Sitecore.Context.ClientPage.ClientResponse.Eval(string.Format("window.clipboardData.setData('Text','{0}')", url));
				clientResponseText = "The Url for the selected item has been copied: " + url;
			}

			//send user message
			if (string.IsNullOrEmpty(url))
			{
				Sitecore.Context.ClientPage.ClientResponse.Alert("The selected item does not have presentation settings.");
			}
			else
			{
				Sitecore.Context.ClientPage.ClientResponse.Alert(clientResponseText);
			}
		}

		/// <summary>
		/// Gets the Url for the passed item and device
		/// </summary>
		/// <param name="item"></param>	
		/// <param name="deviceId"></param>
		/// <returns></returns>
		protected virtual string GetUrl(Item item, string deviceId)
		{
			string url = string.Empty;

			//check for the media item catch
			if (deviceId == "mediaitem")
			{
				return AppendHost(GetMediaItemUrl(item));
			}

			url = GetItemUrl(item, deviceId);
			return AppendHost(CleanUrl(url));
		}

		/// <summary>
		/// Gets the url for the passed device
		/// </summary>
		/// <param name="item"></param>
		/// <param name="deviceId"></param>
		/// <returns></returns>
		protected virtual string GetItemUrl(Item item, string deviceId)
		{
			string url = string.Empty;
			//try and get the url using the passed device
			Database database = Database.GetDatabase("master");
			if (database != null)
			{
				//get device item
				DeviceItem deviceItem = database.GetItem(deviceId);
				if (deviceItem != null && !string.IsNullOrEmpty(deviceItem.QueryString))
				{
					url = LinkManager.GetItemUrl(item);
					char appender = url.Contains("?") ? '&' : '?';
					url = string.Format("{0}{1}{2}", url, appender, deviceItem.QueryString);
				}
			}

			//could not find based on device, fall back to default
			if (string.IsNullOrEmpty(url))
			{
				url = LinkManager.GetItemUrl(item);
			}

			return url;
		}

		/// <summary>
		/// Gets the media url for the passed item
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		protected virtual string GetMediaItemUrl(Item item)
		{
			string mediaUrl = MediaManager.GetMediaUrl(item);

			//if media url doesn't start with a forward slash
			//and it does not begin wit http or www add a forward slash
			if(!mediaUrl.StartsWith("/") && !(mediaUrl.StartsWith("http://") || mediaUrl.StartsWith("www.")))
			{
				mediaUrl = "/" + mediaUrl;
			}

			return mediaUrl;
		}

		/// <summary>
		/// Cleans the url
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		protected virtual string CleanUrl(string url)
		{
			//remove various strings
			url = url.ToLower();
			url = url.Replace("/sitecore/shell", "");
			url = url.Replace("sitecore/content/home/", "");
			url = url.Replace(" ", "-");
			url = url.Replace("/en/", "/");
			url = url.Replace("%20", "-");

			return url;
		}

		/// <summary>
		/// Appends the host to the url
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		protected virtual string AppendHost(string url)
		{
			//verify no host is currently present
			if (url.Contains("http:") || url.Contains("www."))
			{
				return url;
			}

			return string.Format("http://{0}{1}", HttpContext.Current.Request.Url.Host, url);
		}
	}
}