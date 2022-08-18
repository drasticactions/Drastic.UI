﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Drastic.UI.Xaml.Diagnostics
{
	public class VisualTreeChangeEventArgs : EventArgs
	{
		public VisualTreeChangeEventArgs(object parent, object child, int childIndex, VisualTreeChangeType changeType)
		{
			Parent = parent;
			Child = child;
			ChildIndex = childIndex;
			ChangeType = changeType;
		}

		public object Parent { get; }
		public object Child { get; }
		public int ChildIndex { get; }
		public VisualTreeChangeType ChangeType { get; }
	}
}
