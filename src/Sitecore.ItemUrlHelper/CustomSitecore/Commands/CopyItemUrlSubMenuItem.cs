using System;
using System.Linq;
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

			UrlContext urlContext = GetContext(context);
			SendResponse(urlContext);
		}

		/// <summary>
		/// Sends reponse to client
		/// </summary>
		/// <param name="url"></param>
		private void SendResponse(UrlContext urlContext)
		{
			//close context menu
			Sitecore.Context.ClientPage.ClientResponse.ClosePopups(true);

			//check for messages
			if (urlContext.Messages.Count > 0)
			{
				string messages = string.Empty;
				foreach(string s in urlContext.Messages)
				{
					string message = s.Trim();
					if(!s.Contains("."))
					{
						message += s;
					}

					messages += message + " ";
				}

				Sitecore.Context.ClientPage.ClientResponse.Alert(messages.Trim());
			}
			//verify there is a url
			else if (string.IsNullOrEmpty(urlContext.Url))
			{
				Sitecore.Context.ClientPage.ClientResponse.Alert("The selected item does not have presentation settings.");
			}
			//return url to front end
			else
			{
				string clientResponseText = "The Url for the selected item is: " + urlContext.Url;

				//if we are on IE we can copy to the clipboard
				if (UIUtil.IsIE())
				{
					//set clipboard
					Sitecore.Context.ClientPage.ClientResponse.Eval(string.Format("window.clipboardData.setData('Text','{0}')", urlContext.Url));
					clientResponseText = "The Url for the selected item has been copied: " + urlContext.Url;
				}
				else if (UIUtil.IsFirefox() || UIUtil.IsWebkit())
				{
					Sitecore.Context.ClientPage.ClientResponse.Eval(string.Format("window.prompt('Copy to clipboard: Ctrl+C, Enter','{0}')", urlContext.Url));
					return;
				}

				Sitecore.Context.ClientPage.ClientResponse.Alert(clientResponseText);
			}
		}

		/// <summary>
		/// Retrieves the UrlContext for the item in context
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		private UrlContext GetContext(CommandContext context)
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

			return urlContext;
		}
	}
}