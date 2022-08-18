using System;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Drastic.UI
{
	interface IResourcesLoader
	{
		T CreateFromResource<T>(string resourcePath, Assembly assembly, IXmlLineInfo lineInfo) where T : new();
		string GetResource(string resourcePath, Assembly assembly, object target, IXmlLineInfo lineInfo);
	}
}