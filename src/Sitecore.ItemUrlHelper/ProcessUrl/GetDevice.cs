using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sitecore.SharedSource.ItemUrlHelper.ProcessUrl
{
	public class GetDevice : AProcessUrl
	{
		public GetDevice(UrlContext urlContext) : base(urlContext)
		{
		}

		public override void Process()
		{
			//try and capture device from passed menu item command parameter
			if (UrlContext.Parameters["deviceId"] != null && !string.IsNullOrEmpty(UrlContext.Parameters["deviceId"]))
			{
				string deviceId = UrlContext.Parameters["deviceId"];
				if(Sitecore.Data.ID.IsID(deviceId))
				{
					UrlContext.Device = Sitecore.Context.ContentDatabase.GetItem(deviceId);
				}
			}
		}
	}
}
