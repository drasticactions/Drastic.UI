using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Drastic.UI.Platform;

namespace Drastic.UI
{
	[RenderWith(typeof(_CollectionViewRenderer))]
	public class CollectionView : GroupableItemsView
	{
	}
}
