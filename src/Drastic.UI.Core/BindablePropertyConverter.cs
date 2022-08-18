using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml;
using Drastic.UI.Internals;
using Drastic.UI.Xaml;

namespace Drastic.UI
{
	[Xaml.ProvideCompiled("Drastic.UI.Core.XamlC.BindablePropertyConverter")]
	[Xaml.TypeConversion(typeof(BindableProperty))]
	public sealed class BindablePropertyConverter : TypeConverter, IExtendedTypeConverter
	{
		object IExtendedTypeConverter.ConvertFrom(CultureInfo culture, object value, IServiceProvider serviceProvider)
		{
			return ((IExtendedTypeConverter)this).ConvertFromInvariantString(value as string, serviceProvider);
		}

		object IExtendedTypeConverter.ConvertFromInvariantString(string value, IServiceProvider serviceProvider)
		{
			if (string.IsNullOrWhiteSpace(value))
				return null;
			if (serviceProvider == null)
				return null;
			if (!(serviceProvider.GetService(typeof(IXamlTypeResolver)) is IXamlTypeResolver typeResolver))
				return null;
			IXmlLineInfo lineinfo = null;
			if (serviceProvider.GetService(typeof(IXmlLineInfoProvider)) is IXmlLineInfoProvider xmlLineInfoProvider)
				lineinfo = xmlLineInfoProvider.XmlLineInfo;
			string[] parts = value.Split('.');
			Type type = null;
			if (parts.Length == 1)
			{
				if (!(serviceProvider.GetService(typeof(IProvideValueTarget)) is IProvideParentValues parentValuesProvider))
				{
					string msg = string.Format("Can't resolve {0}", parts[0]);
					throw new XamlParseException(msg, lineinfo);
				}
				object parent = parentValuesProvider.ParentObjects.Skip(1).FirstOrDefault();
				if (parentValuesProvider.TargetObject is Setter)
				{
					if (parent is Style style)
						type = style.TargetType;
					else if (parent is TriggerBase triggerBase)
						type = triggerBase.TargetType;
					else if (parent is VisualState visualState)
						type = FindTypeForVisualState(parentValuesProvider, lineinfo);
				}
				else if (parentValuesProvider.TargetObject is Trigger)
					type = (parentValuesProvider.TargetObject as Trigger).TargetType;
				else if (parentValuesProvider.TargetObject is PropertyCondition && (parent as TriggerBase) != null)
					type = (parent as TriggerBase).TargetType;

				if (type == null)
					throw new XamlParseException($"Can't resolve {parts[0]}", lineinfo);

				return ConvertFrom(type, parts[0], lineinfo);
			}
			if (parts.Length == 2)
			{
				if (!typeResolver.TryResolve(parts[0], out type))
				{
					string msg = string.Format("Can't resolve {0}", parts[0]);
					throw new XamlParseException(msg, lineinfo);
				}
				return ConvertFrom(type, parts[1], lineinfo);
			}
			throw new XamlParseException($"Can't resolve {value}. Syntax is [[prefix:]Type.]PropertyName.", lineinfo);
		}

		public override object ConvertFromInvariantString(string value)
		{
			if (string.IsNullOrWhiteSpace(value))
				return null;
			if (value.Contains(":"))
			{
				Log.Warning(null, "Can't resolve properties with xml namespace prefix.");
				return null;
			}
			string[] parts = value.Split('.');
			if (parts.Length != 2)
			{
				Log.Warning(null, $"Can't resolve {value}. Accepted syntax is Type.PropertyName.");
				return null;
			}
			Type type = Type.GetType("Drastic.UI." + parts[0]);
			return ConvertFrom(type, parts[1], null);
		}

		BindableProperty ConvertFrom(Type type, string propertyName, IXmlLineInfo lineinfo)
		{
			string name = propertyName + "Property";
			FieldInfo bpinfo = type.GetField(fi => fi.Name == name && fi.IsStatic && fi.IsPublic && fi.FieldType == typeof(BindableProperty));
			if (bpinfo == null)
				throw new XamlParseException($"Can't resolve {name} on {type.Name}", lineinfo);
			var bp = bpinfo.GetValue(null) as BindableProperty;
			var isObsolete = bpinfo.GetCustomAttribute<ObsoleteAttribute>() != null;
			if (bp.PropertyName != propertyName && !isObsolete)
				throw new XamlParseException($"The PropertyName of {type.Name}.{name} is not {propertyName}", lineinfo);
			return bp;
		}

		Type FindTypeForVisualState(IProvideParentValues parentValueProvider, IXmlLineInfo lineInfo)
		{
			var parents = parentValueProvider.ParentObjects.ToList();

			// Skip 0; we would not be making this check if TargetObject were not a Setter
			// Skip 1; we would not be making this check if the immediate parent were not a VisualState

			// VisualStates must be in a VisualStateGroup
			if (!(parents[2] is VisualStateGroup))
			{
				throw new XamlParseException($"Expected {nameof(VisualStateGroup)} but found {parents[2]}.", lineInfo);
			}

			var vsTarget = parents[3];

			// Are these Visual States directly on a VisualElement?
			if (vsTarget is VisualElement)
			{
				return vsTarget.GetType();
			}

			if (!(parents[3] is VisualStateGroupList))
			{
				throw new XamlParseException($"Expected {nameof(VisualStateGroupList)} but found {parents[3]}.", lineInfo);
			}

			if (!(parents[4] is Setter))
			{
				throw new XamlParseException($"Expected {nameof(Setter)} but found {parents[4]}.", lineInfo);
			}

			// These must be part of a Style; verify that 
			if (!(parents[5] is Style style))
			{
				throw new XamlParseException($"Expected {nameof(Style)} but found {parents[5]}.", lineInfo);
			}

			return style.TargetType;
		}

		public override string ConvertToInvariantString(object value)
		{
			if (!(value is BindableProperty bp))
				throw new NotSupportedException();
			return $"{bp.DeclaringType.Name}.{bp.PropertyName}";
		}
	}
}