﻿using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Drastic.UI.Platform.WPF.Extensions;
using Drastic.UI.Platform.WPF.Helpers;

namespace Drastic.UI.Platform.WPF
{
	public class LayoutRenderer : ViewRenderer<Layout, FormsPanel>
	{
		IElementController ElementController => Element as IElementController;
		bool _isZChanged;

		protected override void OnElementChanged(ElementChangedEventArgs<Layout> e)
		{
			if (e.NewElement != null)
			{
				if (Control == null) // construct and SetNativeControl and suscribe control event
				{
					SetNativeControl(new FormsPanel(Element));
				}

				// Update control property 
				UpdateClipToBounds();
				foreach (Element child in ElementController.LogicalChildren)
					HandleChildAdded(Element, new ElementEventArgs(child));

				// Suscribe element event
				Element.ChildAdded += HandleChildAdded;
				Element.ChildRemoved += HandleChildRemoved;
				Element.ChildrenReordered += HandleChildrenReordered;
			}

			base.OnElementChanged(e);
		}

		void HandleChildAdded(object sender, ElementEventArgs e)
		{
			UiHelper.ExecuteInUiThread(() =>
			{
				var view = e.Element as VisualElement;

				if (view == null)
					return;

				IVisualElementRenderer renderer;
				Platform.SetRenderer(view, renderer = Platform.CreateRenderer(view));
				Control.Children.Add(renderer.GetNativeElement());
				if (_isZChanged)
					EnsureZIndex();
			});
		}

		void HandleChildRemoved(object sender, ElementEventArgs e)
		{
			UiHelper.ExecuteInUiThread(() =>
			{
				var view = e.Element as VisualElement;

				if (view == null)
					return;

				FrameworkElement native = Platform.GetRenderer(view)?.GetNativeElement() as FrameworkElement;
				if (native != null)
				{
					Control.Children.Remove(native);
					view.Cleanup();
					if (_isZChanged)
						EnsureZIndex();
				}
			});
		}

		void HandleChildrenReordered(object sender, EventArgs e)
		{
			EnsureZIndex();
		}

		void EnsureZIndex()
		{
			if (ElementController.LogicalChildren.Count == 0)
				return;

			for (var z = 0; z < ElementController.LogicalChildren.Count; z++)
			{
				var child = ElementController.LogicalChildren[z] as VisualElement;
				if (child == null)
					continue;

				IVisualElementRenderer childRenderer = Platform.GetRenderer(child);

				if (childRenderer == null)
					continue;

				if (Canvas.GetZIndex(childRenderer.GetNativeElement()) != (z + 1))
				{
					if (!_isZChanged)
						_isZChanged = true;

					Canvas.SetZIndex(childRenderer.GetNativeElement(), z + 1);
				}
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == Layout.IsClippedToBoundsProperty.PropertyName)
				UpdateClipToBounds();
		}

		protected override void UpdateBackground()
		{
			Brush background = Element.Background;

			if (Brush.IsNullOrEmpty(background))
				Control.UpdateDependencyColor(FormsPanel.BackgroundProperty, Element.BackgroundColor);
			else
				Control.Background = background.ToBrush();
		}

		void UpdateClipToBounds()
		{
			Control.ClipToBounds = Element.IsClippedToBounds;
		}

		bool _isDisposed;

		protected override void Dispose(bool disposing)
		{
			if (_isDisposed)
				return;

			if (disposing)
			{
				if (Element != null)
				{
					Element.ChildAdded -= HandleChildAdded;
					Element.ChildRemoved -= HandleChildRemoved;
					Element.ChildrenReordered -= HandleChildrenReordered;
				}
			}

			_isDisposed = true;
			base.Dispose(disposing);
		}
	}
}
