using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sitecore.SharedSource.ItemUrlHelper.ProcessUrl
{
	public interface IProcessUrl
	{
		/// <summary>
		/// Process the url
		/// </summary>
		void Process();

		/// <summary>
		/// Context
		/// </summary>
		UrlContext UrlContext { get; set; }
	}
}
