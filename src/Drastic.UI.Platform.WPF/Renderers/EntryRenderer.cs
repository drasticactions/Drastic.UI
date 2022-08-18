﻿using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using static System.String;
using WBrush = System.Windows.Media.Brush;
using WControl = System.Windows.Controls.Control;

namespace Drastic.UI.Platform.WPF
{
	public class EntryRenderer : ViewRenderer<Entry, FormsTextBox>
	{
		bool _fontApplied;
		bool _ignoreTextChange;
		WBrush _placeholderDefaultBrush;
		string _transformedText;

		protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
		{
			if (e.NewElement != null)
			{
				if (Control == null) // Construct and SetNativeControl and suscribe control event
				{
					SetNativeControl(new FormsTextBox());
					Control.LostFocus += OnTextBoxUnfocused;
					Control.TextChanged += TextBoxOnTextChanged;
					Control.KeyUp += TextBoxOnKeyUp;
					Control.SelectionChanged += TextBoxOnSelectionChanged;
				}

				// Update Control properties
				UpdateInputScope();
				UpdateIsPassword();
				UpdateText();
				UpdatePlaceholder();
				UpdateColor();
				UpdateFont();
				UpdateHorizontalTextAlignment();
				UpdateVerticalTextAlignment();
				UpdatePlaceholderColor();
				UpdateMaxLength();
				UpdateIsReadOnly();
				UpdateCursorPosition();
			}

			base.OnElementChanged(e);
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == Entry.TextProperty.PropertyName ||
				e.PropertyName == Entry.TextTransformProperty.PropertyName)
				UpdateText();
			else if (e.PropertyName == Entry.PlaceholderProperty.PropertyName)
				UpdatePlaceholder();
			else if (e.PropertyName == Entry.IsPasswordProperty.PropertyName)
				UpdateIsPassword();
			else if (e.PropertyName == Entry.TextColorProperty.PropertyName)
				UpdateColor();
			else if (e.PropertyName == InputView.KeyboardProperty.PropertyName)
				UpdateInputScope();
			else if (e.PropertyName == Entry.FontAttributesProperty.PropertyName)
				UpdateFont();
			else if (e.PropertyName == Entry.FontFamilyProperty.PropertyName)
				UpdateFont();
			else if (e.PropertyName == Entry.FontSizeProperty.PropertyName)
				UpdateFont();
			else if (e.PropertyName == Entry.HorizontalTextAlignmentProperty.PropertyName)
				UpdateHorizontalTextAlignment();
			else if (e.PropertyName == Entry.VerticalTextAlignmentProperty.PropertyName)
				UpdateVerticalTextAlignment();
			else if (e.PropertyName == Entry.PlaceholderColorProperty.PropertyName)
				UpdatePlaceholderColor();
			else if (e.PropertyName == Entry.CursorPositionProperty.PropertyName)
				UpdateCursorPosition();
			else if (e.PropertyName == InputView.MaxLengthProperty.PropertyName)
				UpdateMaxLength();
			else if (e.PropertyName == InputView.IsReadOnlyProperty.PropertyName)
				UpdateIsReadOnly();
		}

		internal override void OnModelFocusChangeRequested(object sender, VisualElement.FocusRequestArgs args)
		{
			if (args.Focus)
				args.Result = Control.Focus();
			else
			{
				UnfocusControl(Control);
				args.Result = true;
			}
		}

		void OnTextBoxUnfocused(object sender, RoutedEventArgs e)
		{
			if (Element.TextColor.IsDefault)
				return;

			if (!IsNullOrEmpty(Element.Text))
				Control.Foreground = Element.TextColor.ToBrush();
		}

		void TextBoxOnKeyUp(object sender, KeyEventArgs keyEventArgs)
		{
			if (keyEventArgs.Key == Key.Enter)
				((IEntryController)Element).SendCompleted();
		}

		void TextBoxOnTextChanged(object sender, System.Windows.Controls.TextChangedEventArgs textChangedEventArgs)
		{
			if (Control.Text == _transformedText)
				return;

			// Signal to the UpdateText method that the change to TextProperty doesn't need to update the control
			// This prevents the cursor position from getting lost
			_ignoreTextChange = true;
			_transformedText = Element.UpdateFormsText(Control.Text, Element.TextTransform);
			((IElementController)Element).SetValueFromRenderer(Entry.TextProperty, _transformedText);

			// If an Entry.TextChanged handler modified the value of the Entry's text, the values could now be 
			// out-of-sync; re-sync them and fix TextBox cursor position
			string entryText = Element.Text;
			if (Control.Text != entryText)
			{
				Control.Text = entryText;
				if (Control.Text != null)
				{
					var savedSelectionStart = Control.SelectionStart;
					var len = Control.Text.Length;
					Control.SelectionStart = savedSelectionStart > len ? len : savedSelectionStart;
				}
			}

			_ignoreTextChange = false;
		}

		private void TextBoxOnSelectionChanged(object sender, RoutedEventArgs e)
		{
			if (Control != null && Element != null)
				Element.CursorPosition = Control.CaretIndex;
		}

		void UpdateHorizontalTextAlignment()
		{
			if (Control == null)
				return;

			Control.TextAlignment = Element.HorizontalTextAlignment.ToNativeTextAlignment();
		}

		void UpdateVerticalTextAlignment()
		{
			if (Control == null)
				return;

			Control.VerticalContentAlignment = Element.VerticalTextAlignment.ToNativeVerticalAlignment();
		}

		void UpdateColor()
		{
			if (Control == null)
				return;

			Entry entry = Element;
			if (entry != null)
			{
				if (!entry.TextColor.IsDefault)
					Control.Foreground = entry.TextColor.ToBrush();
				else
					Control.Foreground = (WBrush)WControl.ForegroundProperty.GetMetadata(typeof(FormsTextBox)).DefaultValue;

				// Force the PhoneTextBox control to do some internal bookkeeping
				// so the colors change immediately and remain changed when the control gets focus
				Control.OnApplyTemplate();
			}
			else
				Control.Foreground = (WBrush)WControl.ForegroundProperty.GetMetadata(typeof(FormsTextBox)).DefaultValue;
		}

		void UpdateFont()
		{
			if (Control == null)
				return;

			Entry entry = Element;

			if (entry == null)
				return;

			bool entryIsDefault = entry.FontFamily == null && entry.FontSize == Device.GetNamedSize(NamedSize.Default, typeof(Entry), true) && entry.FontAttributes == FontAttributes.None;

			if (entryIsDefault && !_fontApplied)
				return;

			if (entryIsDefault)
			{
				Control.ClearValue(WControl.FontStyleProperty);
				Control.ClearValue(WControl.FontSizeProperty);
				Control.ClearValue(WControl.FontFamilyProperty);
				Control.ClearValue(WControl.FontWeightProperty);
				Control.ClearValue(WControl.FontStretchProperty);
			}
			else
				Control.ApplyFont(entry);

			_fontApplied = true;
		}

		void UpdateInputScope()
		{
			Control.InputScope = Element.Keyboard.ToInputScope();
		}

		void UpdateIsPassword()
		{
			Control.IsPassword = Element.IsPassword;
		}

		void UpdatePlaceholder()
		{
			Control.PlaceholderText = Element.Placeholder ?? string.Empty;
		}

		void UpdatePlaceholderColor()
		{
			Color placeholderColor = Element.PlaceholderColor;

			if (placeholderColor.IsDefault)
			{
				if (_placeholderDefaultBrush == null)
				{
					_placeholderDefaultBrush = (WBrush)WControl.ForegroundProperty.GetMetadata(typeof(FormsTextBox)).DefaultValue;
				}

				// Use the cached default brush
				Control.PlaceholderForegroundBrush = _placeholderDefaultBrush;
				return;
			}

			if (_placeholderDefaultBrush == null)
			{
				// Cache the default brush in case we need to set the color back to default
				_placeholderDefaultBrush = Control.PlaceholderForegroundBrush;
			}

			Control.PlaceholderForegroundBrush = placeholderColor.ToBrush();
		}

		void UpdateText()
		{
			// If the text property has changed because TextBoxOnTextChanged called SetValueFromRenderer,
			// we don't want to re-update the text and reset the cursor position
			if (_ignoreTextChange)
				return;

			var text = _transformedText = Element.UpdateFormsText(Element.Text, Element.TextTransform);
			if (Control.Text == text)
				return;

			Control.Text = text;
			Control.Select(Control.Text == null ? 0 : Control.Text.Length, 0);
		}

		void UpdateMaxLength()
		{
			Control.MaxLength = Element.MaxLength;

			var currentControlText = Control.Text;

			if (currentControlText.Length > Element.MaxLength)
				Control.Text = currentControlText.Substring(0, Element.MaxLength);
		}

		bool _isDisposed;

		protected override void Dispose(bool disposing)
		{
			if (_isDisposed)
				return;

			if (disposing)
			{
				if (Control != null)
				{
					Control.LostFocus -= OnTextBoxUnfocused;
					Control.TextChanged -= TextBoxOnTextChanged;
					Control.KeyUp -= TextBoxOnKeyUp;
					Control.SelectionChanged -= TextBoxOnSelectionChanged;
				}
			}

			_isDisposed = true;
			base.Dispose(disposing);
		}

		void UpdateIsReadOnly()
		{
			Control.IsReadOnly = Element.IsReadOnly;
		}

		void UpdateCursorPosition()
		{
			if (Control.CaretIndex != Element.CursorPosition)
				Control.CaretIndex = Element.CursorPosition;
		}
	}
}
