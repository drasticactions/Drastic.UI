using System.Diagnostics;
using Drastic.UI.Internals;
using AppKit;
using System.Collections.Generic;
using NativeFont = AppKit.NSFont;
using System;

namespace Drastic.UI.Platform.MacOS
{
	public static partial class FontExtensions
	{
        static readonly Dictionary<ToNativeFontFontKey, NativeFont> ToUiFont = new Dictionary<ToNativeFontFontKey, NativeFont>();

        internal static bool IsDefault(this Span self)
        {
            return self.FontFamily == null && self.FontSize == Device.GetNamedSize(NamedSize.Default, typeof(Label), true) &&
                    self.FontAttributes == FontAttributes.None;
        }

        static NativeFont ToNativeFont(this IFontElement element)
        {
            var fontFamily = element.FontFamily;
            var fontSize = (float)element.FontSize;
            var fontAttributes = element.FontAttributes;
            return ToNativeFont(fontFamily, fontSize, fontAttributes, _ToNativeFont);
        }

        static NativeFont ToNativeFont(this Font self)
        {
            var size = (float)self.FontSize;
            if (self.UseNamedSize)
            {
                switch (self.NamedSize)
                {
                    case NamedSize.Micro:
                        size = 12;
                        break;
                    case NamedSize.Small:
                        size = 14;
                        break;
                    case NamedSize.Medium:
                        size = 17; // as defined by iOS documentation
                        break;
                    case NamedSize.Large:
                        size = 22;
                        break;
                    default:
                        size = 17;
                        break;
                }
            }

            var fontAttributes = self.FontAttributes;

            return ToNativeFont(self.FontFamily, size, fontAttributes, _ToNativeFont);
        }

        static NativeFont ToNativeFont(string family, float size, FontAttributes attributes, Func<string, float, FontAttributes, NativeFont> factory)
        {
            var key = new ToNativeFontFontKey(family, size, attributes);

            lock (ToUiFont)
            {
                NativeFont value;
                if (ToUiFont.TryGetValue(key, out value))
                    return value;
            }

            var generatedValue = factory(family, size, attributes);

            lock (ToUiFont)
            {
                NativeFont value;
                if (!ToUiFont.TryGetValue(key, out value))
                    ToUiFont.Add(key, value = generatedValue);
                return value;
            }
        }

        struct ToNativeFontFontKey
        {
            internal ToNativeFontFontKey(string family, float size, FontAttributes attributes)
            {
                _family = family;
                _size = size;
                _attributes = attributes;
            }
#pragma warning disable 0414 // these are not called explicitly, but they are used to establish uniqueness. allow it!
            string _family;
            float _size;
            FontAttributes _attributes;
#pragma warning restore 0414
        }

        static readonly string DefaultFontName = NSFont.SystemFontOfSize(12).FontName;

		public static NSFont ToNSFont(this Font self) => ToNativeFont(self);

		internal static NSFont ToNSFont(this IFontElement element) => ToNativeFont(element);

		static NSFont _ToNativeFont(string family, float size, FontAttributes attributes)
		{
			NSFont defaultFont = NSFont.SystemFontOfSize(size);
			NSFont font = null;
			NSFontDescriptor descriptor = null;
			var bold = (attributes & FontAttributes.Bold) != 0;
			var italic = (attributes & FontAttributes.Italic) != 0;

			if (family != null && family != DefaultFontName)
			{
				try
				{
					descriptor = new NSFontDescriptor().FontDescriptorWithFamily(family);
					font = NSFont.FromDescription(descriptor, size);

					if (font == null)
					{
						var cleansedFont = CleanseFontName(family);
						font = NSFont.FromFontName(cleansedFont, size);
					}
						
				}
				catch
				{
					Debug.WriteLine("Could not load font named: {0}", family);
				}
			}

			//if we didn't found a Font or Descriptor for the FontFamily use the default one 
			if (font == null)
			{
				font = defaultFont;
				descriptor = defaultFont.FontDescriptor;
			}
		
			if (descriptor == null)
				descriptor = defaultFont.FontDescriptor;


			if (bold || italic)
			{
				var traits = (NSFontSymbolicTraits)0;
				if (bold)
					traits |= NSFontSymbolicTraits.BoldTrait;
				if (italic)
					traits |= NSFontSymbolicTraits.ItalicTrait;

				var fontDescriptorWithTraits = descriptor.FontDescriptorWithSymbolicTraits(traits);

				font = NSFont.FromDescription(fontDescriptorWithTraits, size);
			}
			
			return font.ScreenFontWithRenderingMode(NSFontRenderingMode.AntialiasedIntegerAdvancements);
		}

		internal static string CleanseFontName(string fontName)
		{

			//First check Alias
			var (hasFontAlias, fontPostScriptName) = FontRegistrar.HasFont(fontName);
			if (hasFontAlias)
				return fontPostScriptName;

			var fontFile = FontFile.FromString(fontName);

			if (!string.IsNullOrWhiteSpace(fontFile.Extension))
			{
				var (hasFont, filePath) = FontRegistrar.HasFont(fontFile.FileNameWithExtension());
				if (hasFont)
					return filePath ?? fontFile.PostScriptName;
			}
			else
			{
				foreach (var ext in FontFile.Extensions)
				{

					var formated = fontFile.FileNameWithExtension(ext);
					var (hasFont, filePath) = FontRegistrar.HasFont(formated);
					if (hasFont)
						return filePath;
				}
			}
			return fontFile.PostScriptName;
		}
	}
}