using System;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Pipelines.Search;
using Sitecore.Search;
using Sitecore.SharedSource.Commons.Extensions;
using Sitecore.SharedSource.ItemUrlHelper.Model;
using Sitecore.Sites;
using Sitecore.Web;

namespace Sitecore.SharedSource.ItemUrlHelper.CustomSitecore.Pipelines
{
	public class ItemResolverByUrl
	{
		/// <summary>
		/// This will take input (url) from the search pipeline and try to bring back an item.
		/// </summary>
		/// <param name="args"></param>
		public void Process(SearchArgs args)
		{
			Assert.ArgumentNotNull(args, "args");

			//validate argument's properties
			if (string.IsNullOrEmpty(args.TextQuery) || args.Database == null)
			{
				return;
			}

			SiteUrl siteUrlItem = SiteUrl.GetSiteInfo_ByUrl(args.TextQuery);
			if(siteUrlItem == null)
			{
				return;
			}

			SiteInfo siteInfo = SiteContextFactory.GetSiteInfo(siteUrlItem.Name);
			if (siteInfo == null)
			{
				return;
			}

			string sitePath = siteInfo.RootPath + siteInfo.StartItem;

			//try and get the item from the passed text query and database
			Item item = args.TextQuery.GetItemByUrlParts(args.Database, true, sitePath);
			if (item != null)
			{
				//we have a hit, set the search result and abort from the search pipeline
				SearchResult result = SearchResult.FromItem(item);
				args.Result.AddResultToCategory(result, Translate.Text("Direct Hit"));
				args.AbortPipeline();
				return;
			}

			//first attempt failed, it could be due to the dashes in the url
			item = args.TextQuery.GetItemByUrlParts(args.Database, false);
			if (item != null)
			{
				//we have a hit, set the search result and abort from the search pipeline
				SearchResult result = SearchResult.FromItem(item);
				args.Result.AddResultToCategory(result, Translate.Text("Direct Hit"));
				args.AbortPipeline();
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
			if (siteInfo != null)
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