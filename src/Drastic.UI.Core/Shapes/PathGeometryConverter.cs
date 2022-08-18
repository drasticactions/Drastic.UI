﻿using System;

namespace Drastic.UI.Shapes
{
	public class PathGeometryConverter : TypeConverter
	{
		public override object ConvertFromInvariantString(string value)
		{
			PathGeometry pathGeometry = new PathGeometry();

			PathFigureCollectionConverter.ParseStringToPathFigureCollection(pathGeometry.Figures, value);

			return pathGeometry;
		}

		public override string ConvertToInvariantString(object value) => throw new NotSupportedException();
	}
}