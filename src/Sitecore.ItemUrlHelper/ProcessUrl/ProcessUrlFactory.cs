using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Sitecore.SharedSource.ItemUrlHelper.ProcessUrl
{
	public class ProcessUrlFactory
	{
		public static IProcessUrl GetProcessUrl(XmlNode node, UrlContext context)
		{
			if (node.Name != "processor")
			{
				return null;
			}

			if (node.Attributes["type"] != null && !string.IsNullOrEmpty(node.Attributes["type"].Value))
			{
				return GetItem_FromReflection(node, context);
			}

			return null;
		}

		/// <summary>
		/// Return item from reflection
		/// </summary>
		/// <param name="node"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		private static IProcessUrl GetItem_FromReflection(XmlNode node, UrlContext context)
		{
			try
			{
				//verify we can break up the type string into a namespace and assembly name
				string[] split = node.Attributes["type"].Value.Split(',');
				if (split.Length == 0)
				{
					return null;
				}

				string Namespace = split[0];
				string AssemblyName = split[1];

				// load the assemly
				Assembly assembly = GetAssembly(AssemblyName);

				// Walk through each type in the assembly looking for our class
				Type type = assembly.GetType(Namespace);
				if (type == null || !type.IsClass)
				{
					return null;
				}

				object[] parameters = new object[1];
				parameters[0] = context;

				//cast to process url class
				IProcessUrl processUrl = (IProcessUrl)Activator.CreateInstance(type, parameters);

				return processUrl;
			}
			catch (Exception e)
			{
				Sitecore.Diagnostics.Log.Error("Item Url Helper: Could not instantiate ICleanUrl object.", e, typeof(ProcessUrlFactory));
			}

			return null;
		}

		/// <summary>
		/// Retrieve the Assembly
		/// </summary>
		/// <param name="assemblyName"></param>
		/// <returns></returns>
		private static Assembly GetAssembly(string assemblyName)
		{
			//try and find it in the currently loaded assemblies
			AppDomain appDomain = AppDomain.CurrentDomain;
			foreach (Assembly assembly in appDomain.GetAssemblies())
			{
				if (assembly.FullName == assemblyName)
				{
					return assembly;
				}
			}

			//load assembly
			return appDomain.Load(assemblyName);
		}
	}
}
