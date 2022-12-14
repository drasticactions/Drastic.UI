using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreAnimation;

namespace Drastic.UI.Platform.iOS

{
	public class FormsCAKeyFrameAnimation : CAKeyFrameAnimation
	{
		public int Width { get; set; }

		public int Height { get; set; }
	}
}