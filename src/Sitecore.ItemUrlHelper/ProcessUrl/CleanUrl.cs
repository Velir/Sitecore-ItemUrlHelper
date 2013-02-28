using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using Sitecore.Configuration;
using Sitecore.SharedSource.ItemUrlHelper.ProcessUrl.Clean;

namespace Sitecore.SharedSource.ItemUrlHelper.ProcessUrl
{
	public class CleanUrl : AProcessUrl
	{
		public CleanUrl(UrlContext urlContext) : base(urlContext)
		{
		}

		public override void Process()
		{
			if (UrlContext.IsMedia)
			{
				return;
			}

			XmlNode cleanUrlNode = Factory.GetConfigNode("itemUrlHelper/cleanUrl");
			if (cleanUrlNode == null)
			{
				Sitecore.Diagnostics.Log.Error("Item Url Helper: Could not retrieve clean url node from configuration file", this);
				return;
			}

			foreach (XmlNode node in cleanUrlNode.ChildNodes)
			{
				ICleanUrl cleanUrl = CleanUrlFactory.GetCleanUrl(UrlContext.Url, node);
				if (cleanUrl == null)
				{
					continue;
				}

				UrlContext.Url = cleanUrl.Clean();
			}
		}
	}
}
