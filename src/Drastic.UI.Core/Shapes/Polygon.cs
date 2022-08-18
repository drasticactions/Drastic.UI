﻿using Drastic.UI.Platform;

namespace Drastic.UI.Shapes
{
	[RenderWith(typeof(_PolygonRenderer))]
	public sealed class Polygon : Shape
	{
		public static readonly BindableProperty PointsProperty =
			BindableProperty.Create(nameof(Points), typeof(PointCollection), typeof(Polygon), null, defaultValueCreator: bindable => new PointCollection());

		public static readonly BindableProperty FillRuleProperty =
			BindableProperty.Create(nameof(FillRule), typeof(FillRule), typeof(Polygon), FillRule.EvenOdd);

		public PointCollection Points
		{
			set { SetValue(PointsProperty, value); }
			get { return (PointCollection)GetValue(PointsProperty); }
		}

		public FillRule FillRule
		{
			set { SetValue(FillRuleProperty, value); }
			get { return (FillRule)GetValue(FillRuleProperty); }

		}
	}
}