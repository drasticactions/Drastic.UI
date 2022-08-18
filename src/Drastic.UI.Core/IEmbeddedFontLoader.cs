using System;
namespace Drastic.UI
{
	public interface IEmbeddedFontLoader
	{
		(bool success, string filePath) LoadFont(EmbeddedFont font);
	}
}
