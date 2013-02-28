using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.SharedSource.Commons.Extensions;

namespace Sitecore.SharedSource.ItemUrlHelper.ProcessUrl
{
	public class GetItem : AProcessUrl
	{
		public GetItem(UrlContext urlContext) : base(urlContext)
		{
		}

		public override void Process()
		{
			//get database
			Database database = Database.GetDatabase("master");
			if (database == null)
			{
				return;
			}

			//get item from parameters
			if (UrlContext.Parameters["itemId"] == null || string.IsNullOrEmpty(UrlContext.Parameters["itemId"]))
			{
				return;
			}

			Item selectedItem = database.GetItem(UrlContext.Parameters["itemId"]);
			if (selectedItem.IsNull())
			{
				return;
			}

			UrlContext.IsMedia = selectedItem.IsMediaItem();
			UrlContext.Item = selectedItem;
		}
	}
}
