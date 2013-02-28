using System.Xml;
using Sitecore.Xml;

namespace Sitecore.SharedSource.ItemUrlHelper.ProcessUrl.Clean
{
	public class ReplaceCleanUrl : ACleanUrl
	{
		public ReplaceCleanUrl(string url, XmlNode node) : base(url, node)
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

			if (string.IsNullOrEmpty(XmlUtil.GetAttribute("value", _node)) || string.IsNullOrEmpty(XmlUtil.GetAttribute("replaceWith", _node)))
			{
				return _url;
			}

			string replaceThisValue = XmlUtil.GetAttribute("value", _node);
			string replaceWithThisValue = XmlUtil.GetAttribute("replaceWith", _node);

			return _url.Replace(replaceThisValue, replaceWithThisValue);
		}
	}
}
