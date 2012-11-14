using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.SharedSource.Commons.Extensions;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.HtmlControls;

namespace Sitecore.SharedSource.ItemUrlHelper.CustomSitecore.Commands
{
	[Serializable]
	public class CopyItemUrlMenuItem : Command
	{
		public static string DeviceTemplate = "{B6F7EEB4-E8D7-476F-8936-5ACE6A76F20B}";

		public Item CurrentItem { get; set; }

		public override void Execute(CommandContext context)
		{
		}

		public override string GetClick(CommandContext context, string click)
		{
			return string.Empty;
		}

		public override Control[] GetSubmenuItems(CommandContext context)
		{
			if (!context.Items.Any())
			{
				return new Control[0];
			}

			//instantiate control array
			List<Control> controls = new List<Control>();

			//get current item
			CurrentItem = context.Items[0];

			//get master database
			Database database = Database.GetDatabase("master");
			if (database == null)
			{
				return new Control[0];
			}

			//get device folder
			//need to use the database object specifically because ItemReference was returning items from the core database
			Item deviceFolderItem = database.GetItem("/sitecore/layout/Devices");
			if (deviceFolderItem.IsNull())
			{
				return new Control[0];
			}

			//if we are on a media item, add link directly to it
			if(CurrentItem.IsMediaItem())
			{
				MenuItem menuItem = new MenuItem();
				controls.Add(menuItem);
				menuItem.Header = "Media Item";
				menuItem.Icon = "Applications/16x16/photo_scenery.png";
				menuItem.Click = string.Format("itemurlhelper:subMenuItem(deviceId={0}, itemId={1})", "mediaitem", CurrentItem.ID);
			}

			//iterate over devices to find layers with a layout assigned))
			foreach (Item item in deviceFolderItem.Axes.GetDescendants())
			{
				//verify that this item is of a device item
				if (item.IsNull() || !item.IsOfTemplate(DeviceTemplate))
				{
					continue;
				}

				//cast to device item
				DeviceItem deviceItem = item;

				//get layout for this device
				LayoutItem layoutItem = GetLayout(deviceItem);

				//verify there is a layout and make sure it is not that of the fallback device.)
				if (layoutItem == null)
				{
					continue;
				}

				//add to array if we have a layout/presentation details for this device
				MenuItem menuItem = new MenuItem();
				controls.Add(menuItem);
				menuItem.Header = item.DisplayName;
				menuItem.Icon = item.Appearance.Icon;
				menuItem.Click = string.Format("itemurlhelper:subMenuItem(deviceId={0}, itemId={1})", deviceItem.ID, CurrentItem.ID);
			}

			return controls.ToArray();
		}

		//Sitecore methods pulled out using reflector
		#region Sitecore methods

		public LayoutItem GetLayout(DeviceItem device)
		{
			Assert.ArgumentNotNull(device, "device");
			ID layoutID = GetLayoutID(device);
			if (layoutID.IsNull)
			{
				return null;
			}
			return CurrentItem.Database.Resources.Layouts[layoutID];
		}

		private ID GetLayoutID(DeviceItem device)
		{
			Assert.ArgumentNotNull(device, "device");
			XmlNode deviceNode = GetDeviceNode(device, true);
			if (deviceNode != null)
			{
				return LayoutField.ExtractLayoutID(deviceNode);
			}
			return GetOldLayoutId();
		}

		private XmlNode GetDeviceNode(DeviceItem device, bool requireLayout)
		{
			Assert.ArgumentNotNull(device, "device");
			return DoGetDeviceNode(device, requireLayout, string.Empty);
		}

		private XmlNode DoGetDeviceNode(DeviceItem device, bool requireLayout, string visited)
		{
			Assert.ArgumentNotNull(device, "device");
			Assert.ArgumentNotNull(visited, "visited");
			if (visited.IndexOf(device.ID.ToString()) >= 0)
			{
				return null;
			}

			XmlNode deviceNode = DoGetDeviceNode(device);
			if ((deviceNode != null) && (!requireLayout || !ID.IsNullOrEmpty(LayoutField.ExtractLayoutID(deviceNode))))
			{
				return deviceNode;
			}

			return null;
		}

		private XmlNode DoGetDeviceNode(DeviceItem device)
		{
			Assert.ArgumentNotNull(device, "device");
			XmlNode node = DoGetDeviceNode(device, CurrentItem);
			if (node != null)
			{
				return node;
			}
			TemplateItem template = CurrentItem.Template;
			if (template == null)
			{
				return null;
			}
			return DoGetDeviceNode(device, template.InnerItem);
		}

		private static XmlNode DoGetDeviceNode(DeviceItem device, Item item)
		{
			Assert.ArgumentNotNull(device, "device");
			Assert.ArgumentNotNull(item, "item");
			LayoutField field = item.Fields[FieldIDs.LayoutField];
			return field.GetDeviceNode(device);
		}

		private ID GetOldLayoutId()
		{
			Field field = CurrentItem.Fields[new ID("{E1D68787-D22B-4EA2-82B3-84C282E375EB}")];
			string inheritedValue = field.InheritedValue;
			if (inheritedValue.Length <= 0)
			{
				return ID.Null;
			}
			return ID.Parse(inheritedValue);
		}

		#endregion
	}
}