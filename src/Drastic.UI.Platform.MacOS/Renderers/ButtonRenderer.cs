﻿using System;
using System.ComponentModel;
using AppKit;
using CoreGraphics;
using Foundation;

namespace Drastic.UI.Platform.MacOS
{
	public class ButtonRenderer : ViewRenderer<Button, NSButton>
	{
		const float DefaultCornerRadius = 6;

		class FormsNSButton : NSButton
		{
			class FormsNSButtonCell : NSButtonCell
			{
				public override CGRect DrawTitle(NSAttributedString title, CGRect frame, NSView controlView)
				{
					if (controlView is FormsNSButton button)
					{
						var paddedFrame = new CGRect(frame.X + button._leftPadding,
							frame.Y + button._topPadding,
							frame.Width - button._leftPadding - button._rightPadding,
							frame.Height - button._topPadding - button._bottomPadding);
						return base.DrawTitle(title, paddedFrame, controlView);
					}
					return base.DrawTitle(title, frame, controlView);
				}
			}

			public FormsNSButton()
			{
				Cell = new FormsNSButtonCell();
			}

			public event Action Pressed;

			public event Action Released;

			public override void MouseDown(NSEvent theEvent)
			{
				Pressed?.Invoke();

				base.MouseDown(theEvent);

				Released?.Invoke();
			}

			nfloat _leftPadding;
			nfloat _topPadding;
			nfloat _rightPadding;
			nfloat _bottomPadding;

			internal void UpdatePadding(Thickness padding)
			{
				_leftPadding = (nfloat)padding.Left;
				_topPadding = (nfloat)padding.Top;
				_rightPadding = (nfloat)padding.Right;
				_bottomPadding = (nfloat)padding.Bottom;

				InvalidateIntrinsicContentSize();
			}

			public override CGSize IntrinsicContentSize
			{
				get
				{
					var baseSize = base.IntrinsicContentSize;
					return new CGSize(baseSize.Width + _leftPadding + _rightPadding, baseSize.Height + _topPadding + _bottomPadding);
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (Control != null)
				Control.Activated -= OnButtonActivated;

			var formsButton = Control as FormsNSButton;
			if (formsButton != null)
			{
				formsButton.Pressed -= HandleButtonPressed;
				formsButton.Released -= HandleButtonReleased;
			}

			base.Dispose(disposing);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				if (Control == null)
				{
					var btn = new FormsNSButton();
					btn.SetButtonType(NSButtonType.MomentaryPushIn);
					btn.BezelStyle = NSBezelStyle.TexturedSquare;
			
					btn.Pressed += HandleButtonPressed;
					btn.Released += HandleButtonReleased;
					SetNativeControl(btn);

					Control.Activated += OnButtonActivated;
				}

				UpdateText();
				UpdateCharacterSpacing();
				UpdateFont();
				UpdateBorder();
				UpdateImage();
				UpdatePadding();
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == Button.TextProperty.PropertyName ||
				e.PropertyName == Button.TextColorProperty.PropertyName ||
				e.PropertyName == Button.TextTransformProperty.PropertyName)
				UpdateText();
			else if (e.PropertyName == Button.FontProperty.PropertyName)
				UpdateFont();
			else if (e.PropertyName == Button.BorderWidthProperty.PropertyName ||
					e.PropertyName == Button.CornerRadiusProperty.PropertyName ||
					e.PropertyName == Button.BorderColorProperty.PropertyName)
				UpdateBorder();
			else if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName || e.PropertyName == VisualElement.BackgroundProperty.PropertyName)
				UpdateBackgroundVisibility();
			else if (e.PropertyName == Button.ImageSourceProperty.PropertyName)
				UpdateImage();
			else if (e.PropertyName == Button.PaddingProperty.PropertyName)
				UpdatePadding();
			else if (e.PropertyName == Button.CharacterSpacingProperty.PropertyName)
				UpdateCharacterSpacing();
		}

		void OnButtonActivated(object sender, EventArgs eventArgs)
		{
			((IButtonController)Element)?.SendClicked();
		}

		void UpdateBackgroundVisibility()
		{
			var model = Element;
			var backgroundColor = model.BackgroundColor;
			var background = model.Background;

			var shouldDrawImage = backgroundColor == Color.Default && background == null;

			if (!shouldDrawImage)
			{
				if (background != null)
				{
					var backgroundLayer = this.GetBackgroundLayer(background);

					if (backgroundLayer != null)
					{
						Layer.BackgroundColor = NSColor.Clear.CGColor;
						Layer.InsertBackgroundLayer(backgroundLayer, 0);
					}
					else
						Layer.RemoveBackgroundLayer();
				}
				else
					Control.Cell.BackgroundColor = backgroundColor.ToNSColor();
			}

			UpdateBordered();
		}

		void UpdateBorder()
		{
			var uiButton = Control;
			var button = Element;

			if (button.BorderColor != Color.Default)
				uiButton.Layer.BorderColor = button.BorderColor.ToCGColor();

			uiButton.Layer.BorderWidth = (float)button.BorderWidth;
			uiButton.Layer.CornerRadius = button.CornerRadius > 0 ? button.CornerRadius : DefaultCornerRadius;

			UpdateBackgroundVisibility();
		}

		void UpdateFont()
		{
			Control.Font = Element.Font.ToNSFont();
		}

		void UpdateImage()
		{
			this.ApplyNativeImageAsync(Button.ImageSourceProperty, image =>
			{
				NSButton button = Control;
				if (button != null && image != null)
				{
					button.Image = image;
					if (!string.IsNullOrEmpty(button.Title))
						button.ImagePosition = Element.ToNSCellImagePosition();
					((IVisualElementController)Element).NativeSizeChanged();
				}
			});
		}

		void UpdateText()
		{
			var color = Element.TextColor;
			var text = Internals.TextTransformUtilites.GetTransformedText(Element.Text, Element.TextTransform) ?? "";
			if (color == Color.Default)
			{
				Control.Title = text;
			}
			else
			{
				var textWithColor = new NSAttributedString(text ?? "", font: Element.Font.ToNSFont(), foregroundColor: color.ToNSColor(), paragraphStyle: new NSMutableParagraphStyle() { Alignment = NSTextAlignment.Center });
				textWithColor = textWithColor.AddCharacterSpacing(Element.Text ?? string.Empty, Element.CharacterSpacing);
				Control.AttributedTitle = textWithColor;
			}
		}

		void UpdatePadding()
		{
			(Control as FormsNSButton)?.UpdatePadding(Element.Padding);
		}

		void UpdateCharacterSpacing()
		{
			Control.AttributedTitle = Control.AttributedTitle.AddCharacterSpacing(Element.Text ?? string.Empty, Element.CharacterSpacing);
		}

		void UpdateBordered()
		{
			bool hasBackground = Element.BackgroundColor != Color.Default || !Brush.IsNullOrEmpty(Element.Background);
			bool hasBorder = Element.BorderColor != Color.Default && Element.BorderWidth > 0;
			Control.Bordered = !(hasBackground || hasBorder);
		}

		void HandleButtonPressed()
		{
			Element?.SendPressed();
		}

		void HandleButtonReleased()
		{
			Element?.SendReleased();
		}
	}
}