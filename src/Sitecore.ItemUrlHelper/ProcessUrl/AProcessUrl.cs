using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sitecore.SharedSource.ItemUrlHelper.ProcessUrl
{
	public abstract class AProcessUrl : IProcessUrl
	{
		public AProcessUrl(UrlContext urlContext)
		{
			UrlContext = urlContext;
		}

		/// <summary>
		/// Process the url
		/// </summary>
		public abstract void Process();

		/// <summary>
		/// Context
		/// </summary>
		public UrlContext UrlContext { get; set;}
	}
}
