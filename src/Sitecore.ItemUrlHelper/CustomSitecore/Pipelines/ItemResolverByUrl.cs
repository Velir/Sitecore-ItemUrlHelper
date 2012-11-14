using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Pipelines.Search;
using Sitecore.Search;
using Sitecore.SharedSource.Commons.Extensions;

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

			//try and get the item from the passed text query and database
			Item item = args.TextQuery.GetItemByUrlParts(args.Database);
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
	}
}