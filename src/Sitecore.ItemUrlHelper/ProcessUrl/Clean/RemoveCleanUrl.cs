using System.Xml;
using Sitecore.Xml;

namespace Sitecore.SharedSource.ItemUrlHelper.ProcessUrl.Clean
{
	public class RemoveCleanUrl : ACleanUrl
	{
		public RemoveCleanUrl(string url, XmlNode node) : base(url, node)
		{
		}

		/// <summary>
		/// Clean the url
		/// </summary>
		/// <returns></returns>
		public override string Clean()
		{
			if (string.IsNullOrEmpty(_url))
			{
				return string.Empty;
			}

			if (string.IsNullOrEmpty(XmlUtil.GetAttribute("value", _node)))
			{
				return _url;
			}

			string removeThisValue = XmlUtil.GetAttribute("value", _node);
			return _url.Replace(removeThisValue, string.Empty);
		}
	}
}
