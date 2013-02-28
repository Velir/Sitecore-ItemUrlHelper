using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Links;
using Sitecore.SharedSource.ItemUrlHelper.Model;
using Sitecore.Sites;
using Sitecore.Web;
using Sitecore.Xml;

namespace Sitecore.SharedSource.ItemUrlHelper.ProcessUrl
{
	public class GetUrl : AProcessUrl
	{
		public GetUrl(UrlContext urlContext) : base(urlContext)
		{
		}

		public override void Process()
		{
			SiteInfo site = ResolveSite(UrlContext.Item);
			SiteContext siteContext = new SiteContext(site);

			UrlOptions options = new UrlOptions();
			options.Site = siteContext;
			options.LanguageEmbedding = LanguageEmbedding.Never;
			options.AlwaysIncludeServerUrl = false;
			
			if (UrlContext.Item.Languages.Count() > 1)
			{
				options.LanguageEmbedding = LanguageEmbedding.AsNeeded;
			}

			SiteUrl siteUrlItem = SiteUrl.GetSiteInfo_ByName(site.Name);
			if(siteUrlItem == null)
			{
				return;
			}

			string host = siteUrlItem.Url;
			string siteUrl = LinkManager.GetItemUrl(UrlContext.Item, options);

			UrlContext.Url = string.Format("{0}{1}", host, siteUrl);

			//get device item
			if (UrlContext.Device != null && !string.IsNullOrEmpty(UrlContext.Device.QueryString))
			{
				char appender = UrlContext.Url.Contains("?") ? '&' : '?';
				UrlContext.Url = string.Format("{0}{1}{2}", UrlContext.Url, appender, UrlContext.Device.QueryString);
			}
		}

		/// <summary>
		/// Find proper site based on the passed item and fall back to default website
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		private SiteInfo ResolveSite(Item item)
		{
			SiteInfo siteInfo = Sitecore.SharedSource.Commons.Utilities.ItemUtil.GetSite(item);
			if(siteInfo != null)
			{
				return siteInfo;
			}

			//fall back to default
			siteInfo = SiteContextFactory.GetSiteInfo("website");
			if (siteInfo != null)
			{
				return siteInfo;
			}

			return null;
		}
	}
}
