using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Sitecore.Data.Items;

namespace Sitecore.SharedSource.ItemUrlHelper.ProcessUrl
{
	public class UrlContext
	{
		public bool IsMedia { get; set; }
		public Item Item { get; set; }
		public DeviceItem Device { get; set; }
		public string Url { get; set; }
		public NameValueCollection Parameters { get; set; }
	}
}
