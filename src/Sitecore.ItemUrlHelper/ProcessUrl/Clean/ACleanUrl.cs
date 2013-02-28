using System;
using System.Xml;

namespace Sitecore.SharedSource.ItemUrlHelper.ProcessUrl.Clean
{
	public abstract class ACleanUrl : ICleanUrl
	{
		protected readonly string _url;
		protected readonly XmlNode _node;

		public ACleanUrl(string url, XmlNode node)
		{
			_url = url;
			_node = node;
		}

		public abstract string Clean();
	}
}
