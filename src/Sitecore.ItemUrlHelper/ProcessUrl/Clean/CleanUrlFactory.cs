using System;
using System.Reflection;
using System.Xml;

namespace Sitecore.SharedSource.ItemUrlHelper.ProcessUrl.Clean
{
	public class CleanUrlFactory
	{
		/// <summary>
		/// Returns the proper implementation of the clean class
		/// </summary>
		/// <param name="url"></param>
		/// <param name="node"></param>
		/// <returns></returns>
		public static ICleanUrl GetCleanUrl(string url, XmlNode node)
		{
			return GetItem_FromReflection(url, node);
		}

		/// <summary>
		/// Return item from reflection
		/// </summary>
		/// <param name="node"></param>
		/// <param name="url"></param>
		/// <returns></returns>
		private static ICleanUrl GetItem_FromReflection(string url, XmlNode node)
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

				object[] parameters = new object[2];
				parameters[0] = url;
				parameters[1] = node;

				//cast to clean url class
				ICleanUrl cleanUrl = (ICleanUrl)Activator.CreateInstance(type, parameters);

				return cleanUrl;
			}
			catch (Exception e)
			{
				Sitecore.Diagnostics.Log.Error("Item Url Helper: Could not instantiate ICleanUrl object.", e, typeof(CleanUrlFactory));
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
