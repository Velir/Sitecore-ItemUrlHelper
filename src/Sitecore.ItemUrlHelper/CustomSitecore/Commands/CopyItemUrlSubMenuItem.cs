using System;
using System.Web;
using System.Xml;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Links;
using Sitecore.Resources.Media;
using Sitecore.SharedSource.Commons.Extensions;
using Sitecore.SharedSource.ItemUrlHelper.ProcessUrl;
using Sitecore.SharedSource.ItemUrlHelper.ProcessUrl.Clean;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Xml;

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

			string url = GetUrl(context);
			SendResponse(url);
		}

		/// <summary>
		/// Sends reponse to client
		/// </summary>
		/// <param name="url"></param>
		private void SendResponse(string url)
		{
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
		/// Retrieves the Url for the item in context
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		private string GetUrl(CommandContext context)
		{
			//start retrieval process
			UrlContext urlContext = new UrlContext();
			urlContext.Parameters = context.Parameters;

			foreach (XmlNode node in Factory.GetConfigNodes("itemUrlHelper/processUrl/child::node()"))
			{
				if (node.Name != "processor")
				{
					continue;
				}

				IProcessUrl processUrl = ProcessUrlFactory.GetProcessUrl(node, urlContext);
				if (processUrl == null)
				{
					continue;
				}

				processUrl.Process();
				urlContext = processUrl.UrlContext;
			}

			return urlContext.Url;
		}
	}
}