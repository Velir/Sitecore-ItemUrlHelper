using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Sitecore.Configuration;
using Sitecore.Web;
using Sitecore.Xml;

namespace Sitecore.SharedSource.ItemUrlHelper.Model
{
	public class SiteUrl
	{
		public string Name { get; set; }
		public string Url { get; set; }

		/// <summary>
		/// The site's url is returned through the mapping in the configuration file
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static SiteUrl GetSiteInfo_ByName(string name)
		{
			name = name.ToLower();
			SiteUrl item = SiteUrlItems.Where(x => x.Name.ToLower() == name).FirstOrDefault();
			if (item != null)
			{
				return item;
			}

			return null;
		}

		/// <summary>
		/// The site's url is returned through the mapping in the configuration file
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public static SiteUrl GetSiteInfo_ByUrl(string url)
		{
			if(!IsValidUri(url))
			{
				return null;
			}
			
			try
			{
				Uri uri = new Uri(url);
				url = uri.Host.ToLower();

				SiteUrl item = SiteUrlItems.Where(x => x.Url.ToLower().Contains(url)).FirstOrDefault();
				if (item != null)
				{
					return item;
				}
			}
			catch (Exception e)
			{
				Sitecore.Diagnostics.Log.Error("Item Url Helper - Could not convert to Uri:" + url, e);
			}

			return null;
		}

		private static bool IsValidUri(string uri)
		{
			try
			{
				new Uri(uri);
				return true;
			}
			catch
			{
				return false;
			}
		}

		private static List<SiteUrl> SiteUrlItems
		{
			get
			{
				try
				{
					List<SiteUrl> items = new List<SiteUrl>();

					XmlNodeList configNodes = Factory.GetConfigNodes("itemUrlHelper/siteurls/site");
					if (configNodes == null)
					{
						return new List<SiteUrl>();
					}

					foreach (XmlNode node in configNodes)
					{
						if (node.Name != "site" || string.IsNullOrEmpty(XmlUtil.GetAttribute("name", node)) || string.IsNullOrEmpty(XmlUtil.GetAttribute("url", node)))
						{
							continue;
						}

						SiteUrl siteInfo = new SiteUrl();
						siteInfo.Name = XmlUtil.GetAttribute("name", node);
						siteInfo.Url = XmlUtil.GetAttribute("url", node);

						items.Add(siteInfo);
					}

					return items;
				}
				catch (Exception e)
				{
					Sitecore.Diagnostics.Log.Error("Item Url Helper: Could not retrieve site url from configuration file", e, typeof(SiteUrl));
				}

				return new List<SiteUrl>();
			}
		}
	}
}
