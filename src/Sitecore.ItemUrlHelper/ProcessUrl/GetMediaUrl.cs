using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Resources.Media;
using Sitecore.SharedSource.ItemUrlHelper.Model;

namespace Sitecore.SharedSource.ItemUrlHelper.ProcessUrl
{
	public class GetMediaUrl : AProcessUrl
	{
		public GetMediaUrl(UrlContext urlContext) : base(urlContext)
		{
		}

		/// <summary>
		/// Process the url
		/// </summary>
		public override void Process()
		{
			if(!UrlContext.IsMedia)
			{
				return;
			}

			string mediaUrl = MediaManager.GetMediaUrl(UrlContext.Item);

			//if media url doesn't start with a forward slash
			//and it does not begin wit http or www add a forward slash
			if (!mediaUrl.StartsWith("/") && !(mediaUrl.StartsWith("http://") || mediaUrl.StartsWith("www.")))
			{
				mediaUrl = "/" + mediaUrl;
			}

		    if (!(mediaUrl.StartsWith("http://") || mediaUrl.StartsWith("www.")))
		    {
		        SiteUrl defaultSite = SiteUrl.GetDefaultSite();
		        if (defaultSite != null && !string.IsNullOrEmpty(defaultSite.Url))
		        {
		            mediaUrl = defaultSite.Url + mediaUrl;
		        }
		    }
			
            UrlContext.Url = mediaUrl;
		}
	}
}
