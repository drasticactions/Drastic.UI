﻿using System;
using AppKit;
using System.ComponentModel;
using CoreGraphics;
using Drastic.UI.Platform.MacOS.Controls;
using Foundation;

namespace Drastic.UI.Platform.MacOS
{
	internal class FormsSliderCell : NSSliderCell
	{
		public Color MinimumTrackColor { get; set; }

		public Color MaximumTrackColor { get; set; }

		public Color ThumbColor { get; set; }

		public override void DrawBar(CGRect aRect, bool flipped)
		{ 
			// Mimick the dimensions of the original slider
			var originalHeight = aRect.Height;
			aRect.Height = 2.7f;
			var radius = aRect.Height / 2;
			aRect.Y += (originalHeight - aRect.Height) / 2;

			// Calc the progress percentage to know where one bar starts
			var progress = (float)((DoubleValue - MinValue) / (MaxValue - MinValue));

			var minTrackRect = aRect;
			minTrackRect.Width *= progress;

			var maxTrackRect = aRect;
			maxTrackRect.X += maxTrackRect.Width * progress;
			maxTrackRect.Width = maxTrackRect.Width * (1 - progress);

			// Draw min track
			var minTrackPath = NSBezierPath.FromRoundedRect(minTrackRect, radius, radius);

			var defaultMinTrackColor = Color.Accent.ToNSColor();

			if (UI.IsMojaveOrNewer)
				defaultMinTrackColor = NSColor.ControlAccent;

			var minTrackColor = MinimumTrackColor.IsDefault ? defaultMinTrackColor : MinimumTrackColor.ToNSColor();
			minTrackColor.SetFill();
			minTrackPath.Fill();

			var defaultMaxTrackColor = NSColor.ControlShadow;

			if (UI.IsMojaveOrNewer)
				defaultMaxTrackColor = NSColor.Separator;

			// Draw max track
			var maxTrackPath = NSBezierPath.FromRoundedRect(maxTrackRect, radius, radius);
			var maxTrackColor = MaximumTrackColor.IsDefault ? defaultMaxTrackColor : MaximumTrackColor.ToNSColor();
			maxTrackColor.SetFill();
			maxTrackPath.Fill();
		}

		public override void DrawKnob(CGRect knobRect)
		{
			// Mimick the dimensions of the original slider
			knobRect.Width -= 6;
			knobRect.Height -= 6;
			knobRect.Y += 3;
			knobRect.X += 3;
			var radius = 7.5f;

			var path = new NSBezierPath();
			path.AppendPathWithRoundedRect(knobRect, radius, radius);
			// Draw inside

			var defaultKnobColor = NSColor.ControlLightHighlight;

			if (UI.IsMojaveOrNewer)
				defaultKnobColor = NSColor.Highlight;

			var knobColor = ThumbColor.IsDefault ? defaultKnobColor : ThumbColor.ToNSColor();
			knobColor.SetFill();
			path.Fill();

			// Draw border
			if (UI.IsMojaveOrNewer)
				NSColor.ControlShadow.SetStroke();
			else
				NSColor.Separator.SetStroke();

			path.Stroke();
		}
	}

	public class SliderRenderer : ViewRenderer<Slider, NSSlider>
	{
		bool _disposed;

		IElementController ElementController => Element;

		protected override void OnElementChanged(ElementChangedEventArgs<Slider> e)
		{
			if (e.NewElement != null)
			{
				if (Control == null)
				{
					SetNativeControl(new FormsNSSlider());
					Control.Cell = new FormsSliderCell();
					Control.Action = new ObjCRuntime.Selector(nameof(ValueChanged));
					Control.Target = this;
				}

				UpdateMaximum();
				UpdateMinimum();
				UpdateValue();
				UpdateMinimumTrackColor();
				UpdateMaximumTrackColor();
				UpdateThumbColor();
			}

			base.OnElementChanged(e);
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == Slider.MaximumProperty.PropertyName)
				UpdateMaximum();
			else if (e.PropertyName == Slider.MinimumProperty.PropertyName)
				UpdateMinimum();
			else if (e.PropertyName == Slider.ValueProperty.PropertyName)
				UpdateValue();
			else if (e.PropertyName == Slider.MinimumTrackColorProperty.PropertyName)
				UpdateMinimumTrackColor();
			else if (e.PropertyName == Slider.MaximumTrackColorProperty.PropertyName)
				UpdateMaximumTrackColor();
			else if (e.PropertyName == Slider.ThumbColorProperty.PropertyName)
			{
				UpdateThumbColor();
			}
		}

		void UpdateMaximumTrackColor()
		{
			// Cell could be overwritten with an other custom cell
			if (Control.Cell is FormsSliderCell sliderCell)
			{
				sliderCell.MaximumTrackColor = Element.MaximumTrackColor;
			}
		}

		void UpdateMinimumTrackColor()
		{
			// Cell could be overwritten with an other custom cell
			if (Control.Cell is FormsSliderCell sliderCell)
			{
				sliderCell.MinimumTrackColor = Element.MinimumTrackColor;
			}
		}

		void UpdateThumbColor()
		{
			// Cell could be overwritten with an other custom cell
			if (Control.Cell is FormsSliderCell sliderCell)
			{
				sliderCell.ThumbColor = Element.ThumbColor;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && !_disposed)
			{
				_disposed = true;
				if (Control != null)
				{
					Control.Target = null;
					Control.Action = null;
				}
			}

			base.Dispose(disposing);
		}


		[Export(nameof(ValueChanged))]
		void ValueChanged()
		{
			ElementController?.SetValueFromRenderer(Slider.ValueProperty, Control.DoubleValue);

			var controlEvent = NSApplication.SharedApplication.CurrentEvent;
			if (controlEvent.Type == NSEventType.LeftMouseDown)
			{
				((ISliderController)Element)?.SendDragStarted();
			}
			else if (controlEvent.Type == NSEventType.LeftMouseUp)
			{
				((ISliderController)Element)?.SendDragCompleted();
			}
		}

		void UpdateMaximum()
		{
			Control.MaxValue = (float)Element.Maximum;
		}

		void UpdateMinimum()
		{
			Control.MinValue = (float)Element.Minimum;
		}

		void UpdateValue()
		{
			if (Math.Abs(Element.Value - Control.DoubleValue) > 0)
				Control.DoubleValue = (float)Element.Value;
		}
	}
}
