using Foundation;
using System;
using UIKit;
using Drastic.UI.Internals;

namespace Drastic.UI.Platform.iOS
{
	public static class Extensions
	{
		public static void ApplyKeyboard(this IUITextInput textInput, Keyboard keyboard)
		{
			if(textInput is IUITextInputTraits traits)
				ApplyKeyboard(traits, keyboard);
		}

		public static void ApplyKeyboard(this IUITextInputTraits textInput, Keyboard keyboard)
		{
            textInput.SetAutocapitalizationType(UITextAutocapitalizationType.None);
            textInput.SetAutocorrectionType(UITextAutocorrectionType.No);
            textInput.SetSpellCheckingType(UITextSpellCheckingType.No);
            textInput.SetKeyboardType(UIKeyboardType.Default);

            if (keyboard == Keyboard.Default)
            {
                textInput.SetAutocapitalizationType(UITextAutocapitalizationType.Sentences);
                textInput.SetAutocorrectionType(UITextAutocorrectionType.Default);
                textInput.SetSpellCheckingType(UITextSpellCheckingType.Default);
            }
            else if (keyboard == Keyboard.Chat)
            {
                textInput.SetAutocapitalizationType(UITextAutocapitalizationType.Sentences);
                textInput.SetAutocorrectionType(UITextAutocorrectionType.Yes);
            }
            else if (keyboard == Keyboard.Email)
                textInput.SetKeyboardType(UIKeyboardType.EmailAddress);
            else if (keyboard == Keyboard.Numeric)
                textInput.SetKeyboardType(UIKeyboardType.DecimalPad);
            else if (keyboard == Keyboard.Telephone)
                textInput.SetKeyboardType(UIKeyboardType.PhonePad);
            else if (keyboard == Keyboard.Text)
            {
                textInput.SetAutocapitalizationType(UITextAutocapitalizationType.Sentences);
                textInput.SetAutocorrectionType(UITextAutocorrectionType.Yes);
                textInput.SetSpellCheckingType(UITextSpellCheckingType.Yes);
            }
            else if (keyboard == Keyboard.Url)
                textInput.SetKeyboardType(UIKeyboardType.Url);
            else if (keyboard is CustomKeyboard)
            {
                var custom = (CustomKeyboard)keyboard;

                var capitalizedSentenceEnabled = (custom.Flags & KeyboardFlags.CapitalizeSentence) == KeyboardFlags.CapitalizeSentence;
                var capitalizedWordsEnabled = (custom.Flags & KeyboardFlags.CapitalizeWord) == KeyboardFlags.CapitalizeWord;
                var capitalizedCharacterEnabled = (custom.Flags & KeyboardFlags.CapitalizeCharacter) == KeyboardFlags.CapitalizeCharacter;
                var capitalizedNone = (custom.Flags & KeyboardFlags.None) == KeyboardFlags.None;

                var spellcheckEnabled = (custom.Flags & KeyboardFlags.Spellcheck) == KeyboardFlags.Spellcheck;
                var suggestionsEnabled = (custom.Flags & KeyboardFlags.Suggestions) == KeyboardFlags.Suggestions;


                UITextAutocapitalizationType capSettings = UITextAutocapitalizationType.None;

                // Sentence being first ensures that the behavior of ALL is backwards compatible
                if (capitalizedSentenceEnabled)
                    capSettings = UITextAutocapitalizationType.Sentences;
                else if (capitalizedWordsEnabled)
                    capSettings = UITextAutocapitalizationType.Words;
                else if (capitalizedCharacterEnabled)
                    capSettings = UITextAutocapitalizationType.AllCharacters;
                else if (capitalizedNone)
                    capSettings = UITextAutocapitalizationType.None;

                textInput.SetAutocapitalizationType(capSettings);
                textInput.SetAutocorrectionType(suggestionsEnabled ? UITextAutocorrectionType.Yes : UITextAutocorrectionType.No);
                textInput.SetSpellCheckingType(spellcheckEnabled ? UITextSpellCheckingType.Yes : UITextSpellCheckingType.No);
            }
        }

		public static UIModalPresentationStyle ToNativeModalPresentationStyle(this PlatformConfiguration.iOSSpecific.UIModalPresentationStyle style)
		{
			switch (style)
			{
				case PlatformConfiguration.iOSSpecific.UIModalPresentationStyle.FormSheet:
					return UIModalPresentationStyle.FormSheet;
				case PlatformConfiguration.iOSSpecific.UIModalPresentationStyle.FullScreen:
					return UIModalPresentationStyle.FullScreen;
				case PlatformConfiguration.iOSSpecific.UIModalPresentationStyle.Automatic:
					return UIModalPresentationStyle.Automatic;
				case PlatformConfiguration.iOSSpecific.UIModalPresentationStyle.OverFullScreen:
					return UIModalPresentationStyle.OverFullScreen;
				case PlatformConfiguration.iOSSpecific.UIModalPresentationStyle.PageSheet:
					return UIModalPresentationStyle.PageSheet;
				default:
					throw new ArgumentOutOfRangeException(nameof(style));
			}
		}

		internal static UISearchBarStyle ToNativeSearchBarStyle(this PlatformConfiguration.iOSSpecific.UISearchBarStyle style)
		{
			switch (style)
			{
				case PlatformConfiguration.iOSSpecific.UISearchBarStyle.Default:
					return UISearchBarStyle.Default;
				case PlatformConfiguration.iOSSpecific.UISearchBarStyle.Prominent:
					return UISearchBarStyle.Prominent;
				case PlatformConfiguration.iOSSpecific.UISearchBarStyle.Minimal:
					return UISearchBarStyle.Minimal;
				default:
					throw new ArgumentOutOfRangeException(nameof(style));
			}
		}

		internal static UIReturnKeyType ToUIReturnKeyType(this ReturnType returnType)
		{
			switch (returnType)
			{
				case ReturnType.Go:
					return UIReturnKeyType.Go;
				case ReturnType.Next:
					return UIReturnKeyType.Next;
				case ReturnType.Send:
					return UIReturnKeyType.Send;
				case ReturnType.Search:
					return UIReturnKeyType.Search;
				case ReturnType.Done:
					return UIReturnKeyType.Done;
				case ReturnType.Default:
					return UIReturnKeyType.Default;
				default:
					throw new System.NotImplementedException($"ReturnType {returnType} not supported");
			}
		}

		internal static DeviceOrientation ToDeviceOrientation(this UIDeviceOrientation orientation)
		{
			switch (orientation)
			{
				case UIDeviceOrientation.Portrait:
					return DeviceOrientation.Portrait;
				case UIDeviceOrientation.PortraitUpsideDown:
					return DeviceOrientation.PortraitDown;
				case UIDeviceOrientation.LandscapeLeft:
					return DeviceOrientation.LandscapeLeft;
				case UIDeviceOrientation.LandscapeRight:
					return DeviceOrientation.LandscapeRight;
				default:
					return DeviceOrientation.Other;
			}
		}

		internal static NSMutableAttributedString AddCharacterSpacing(this NSAttributedString attributedString, string text, double characterSpacing)
		{
			if (attributedString == null && characterSpacing == 0)
				return null;

			NSMutableAttributedString mutableAttributedString = attributedString as NSMutableAttributedString;
			if (attributedString == null || attributedString.Length == 0)
			{
				mutableAttributedString = text == null ? new NSMutableAttributedString() : new NSMutableAttributedString(text);
			}
			else
			{
				mutableAttributedString = new NSMutableAttributedString(attributedString);
			}

			AddKerningAdjustment(mutableAttributedString, text, characterSpacing);

			return mutableAttributedString;
		}

		internal static bool HasCharacterAdjustment(this NSMutableAttributedString mutableAttributedString)
		{
			if (mutableAttributedString == null)
				return false;

			NSRange removalRange;
			var attributes = mutableAttributedString.GetAttributes(0, out removalRange);

			for (uint i = 0; i < attributes.Count; i++)
				if (attributes.Keys[i] is NSString nSString && nSString == UIStringAttributeKey.KerningAdjustment)
					return true;

			return false;
		}

		internal static void AddKerningAdjustment(NSMutableAttributedString mutableAttributedString, string text, double characterSpacing)
		{
			if (!string.IsNullOrEmpty(text))
			{
				if (characterSpacing == 0 && !mutableAttributedString.HasCharacterAdjustment())
					return;

				mutableAttributedString.AddAttribute
				(
					UIStringAttributeKey.KerningAdjustment,
					NSObject.FromObject(characterSpacing), new NSRange(0, text.Length - 1)
				);
			}
		}

		internal static bool IsHorizontal(this Button.ButtonContentLayout layout) =>
			layout.Position == Button.ButtonContentLayout.ImagePosition.Left ||
			layout.Position == Button.ButtonContentLayout.ImagePosition.Right;
	}
}