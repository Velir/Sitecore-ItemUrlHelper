using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.SharedSource.Commons.Extensions;
using Sitecore.SharedSource.Contextualizer.Filters;
using Sitecore.SharedSource.ItemUrlHelper.CustomSitecore.Commands;

namespace Sitecore.SharedSource.ItemUrlHelper.Modules.Contextualizer
{
	public class ItemUrlHelperFilter : IFilter
	{
		public void Process(FilterArgs args)
		{
			//get master database
			Database database = Database.GetDatabase("master");
			if (database == null)
			{
				args.HideCommand = false;
				return;
			}

			Item deviceFolderItem = database.GetItem("/sitecore/layout/Devices");
			if (deviceFolderItem.IsNull())
			{
				args.HideCommand = false;
				return;
			}

			if (args.ContentItem.IsNull() || args.ContentItem.IsMediaItem())
			{
				args.HideCommand = false;
				return;
			}

			//iterate over devices to find layers with a layout assigned))
			foreach (Item item in deviceFolderItem.Axes.GetDescendants())
			{
				//verify that this item is of a device item
				if (item.IsNull() || !item.IsOfTemplate(CopyItemUrlMenuItem.DeviceTemplate))
				{
					continue;
				}

				//cast to device item
				DeviceItem deviceItem = item;

				//get layout for this device
				CopyItemUrlMenuItem copyItemUrlMenuItem = new CopyItemUrlMenuItem();
				copyItemUrlMenuItem.CurrentItem = args.ContentItem;

				LayoutItem layoutItem = copyItemUrlMenuItem.GetLayout(deviceItem);

				//verify there is a layout and make sure it is not that of the fallback device.)
				if (layoutItem == null)
				{
					continue;
				}

				args.HideCommand = false;
				return;
			}

			args.HideCommand = true;
		}
	}
}