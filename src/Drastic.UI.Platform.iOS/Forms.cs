﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Drastic.UI.Internals;
using Foundation;
using Drastic.UI.PlatformConfiguration;
using Drastic.UI.PlatformConfiguration.iOSSpecific;
using Drastic.UI;

#if __MOBILE__
using UIKit;
using Drastic.UI.Platform.iOS;
using TNativeView = UIKit.UIView;
#else
using AppKit;
using Drastic.UI.Platform.MacOS;
using TNativeView = AppKit.NSView;
#endif

namespace Drastic.UI
{
	public static class Forms
	{
		public static bool IsInitialized { get; private set; }

#if __MOBILE__
		static bool? s_isiOS9OrNewer;
		static bool? s_isiOS10OrNewer;
		static bool? s_isiOS11OrNewer;
		static bool? s_isiOS12OrNewer;
		static bool? s_isiOS13OrNewer;
		static bool? s_isiOS14OrNewer;
		static bool? s_isiOS15OrNewer;
		static bool? s_isiOS154OrNewer;
		static bool? s_isiOS16OrNewer;
		static bool? s_respondsTosetNeedsUpdateOfHomeIndicatorAutoHidden;

		internal static bool IsiOS9OrNewer
		{
			get
			{
				if (!s_isiOS9OrNewer.HasValue)
					s_isiOS9OrNewer = UIDevice.CurrentDevice.CheckSystemVersion(9, 0);
				return s_isiOS9OrNewer.Value;
			}
		}


		internal static bool IsiOS10OrNewer
		{
			get
			{
				if (!s_isiOS10OrNewer.HasValue)
					s_isiOS10OrNewer = UIDevice.CurrentDevice.CheckSystemVersion(10, 0);
				return s_isiOS10OrNewer.Value;
			}
		}

		internal static bool IsiOS11OrNewer
		{
			get
			{
				if (!s_isiOS11OrNewer.HasValue)
					s_isiOS11OrNewer = UIDevice.CurrentDevice.CheckSystemVersion(11, 0);
				return s_isiOS11OrNewer.Value;
			}
		}

		internal static bool IsiOS12OrNewer
		{
			get
			{
				if (!s_isiOS12OrNewer.HasValue)
					s_isiOS12OrNewer = UIDevice.CurrentDevice.CheckSystemVersion(12, 0);
				return s_isiOS12OrNewer.Value;
			}
		}

		internal static bool IsiOS13OrNewer
		{
			get
			{
				if (!s_isiOS13OrNewer.HasValue)
					s_isiOS13OrNewer = UIDevice.CurrentDevice.CheckSystemVersion(13, 0);
				return s_isiOS13OrNewer.Value;
			}
		}

		internal static bool IsiOS14OrNewer
		{
			get
			{
				if (!s_isiOS14OrNewer.HasValue)
					s_isiOS14OrNewer = UIDevice.CurrentDevice.CheckSystemVersion(14, 0);
				return s_isiOS14OrNewer.Value;
			}
		}

		internal static bool IsiOS15OrNewer
		{
			get
			{
				if (!s_isiOS15OrNewer.HasValue)
					s_isiOS15OrNewer = UIDevice.CurrentDevice.CheckSystemVersion(15, 0);
				return s_isiOS15OrNewer.Value;
			}
		}

		internal static bool IsiOS154OrNewer
		{
			get
			{
				if (!s_isiOS154OrNewer.HasValue)
					s_isiOS154OrNewer = UIDevice.CurrentDevice.CheckSystemVersion(15, 4);
				return s_isiOS154OrNewer.Value;
			}
		}

		internal static bool IsiOS16OrNewer
		{
			get
			{
				if (!s_isiOS16OrNewer.HasValue)
					s_isiOS16OrNewer = UIDevice.CurrentDevice.CheckSystemVersion(16, 0);
				return s_isiOS16OrNewer.Value;
			}
		}

		internal static bool RespondsToSetNeedsUpdateOfHomeIndicatorAutoHidden
		{
			get
			{
				if (!s_respondsTosetNeedsUpdateOfHomeIndicatorAutoHidden.HasValue)
					s_respondsTosetNeedsUpdateOfHomeIndicatorAutoHidden = new UIViewController().RespondsToSelector(new ObjCRuntime.Selector("setNeedsUpdateOfHomeIndicatorAutoHidden"));
				return s_respondsTosetNeedsUpdateOfHomeIndicatorAutoHidden.Value;
			}
		}
#else
		static bool? s_isSierraOrNewer;

		internal static bool IsSierraOrNewer
		{
			get
			{
				if (!s_isSierraOrNewer.HasValue)
					s_isSierraOrNewer = NSProcessInfo.ProcessInfo.IsOperatingSystemAtLeastVersion(new NSOperatingSystemVersion(10, 12, 0));
				return s_isSierraOrNewer.Value;
			}
		}

		static bool? s_isHighSierraOrNewer;

		internal static bool IsHighSierraOrNewer
		{
			get
			{
				if (!s_isHighSierraOrNewer.HasValue)
					s_isHighSierraOrNewer = NSProcessInfo.ProcessInfo.IsOperatingSystemAtLeastVersion(new NSOperatingSystemVersion(10, 13, 0));
				return s_isHighSierraOrNewer.Value;
			}
		}

		static bool? s_isMojaveOrNewer;

		internal static bool IsMojaveOrNewer
		{
			get
			{
				if (!s_isMojaveOrNewer.HasValue)
					s_isMojaveOrNewer = NSProcessInfo.ProcessInfo.IsOperatingSystemAtLeastVersion(new NSOperatingSystemVersion(10, 14, 0));
				return s_isMojaveOrNewer.Value;
			}
		}

#endif

		static IReadOnlyList<string> s_flags;
		public static IReadOnlyList<string> Flags => s_flags ?? (s_flags = new string[0]);

		public static void SetFlags(params string[] flags)
		{
			if (IsInitialized)
			{
				throw new InvalidOperationException($"{nameof(SetFlags)} must be called before {nameof(Init)}");
			}

			s_flags = (string[])flags.Clone();
			if (s_flags.Contains("Profile"))
				Profile.Enable();
		}

		public static void Init()
		{
			if (IsInitialized)
				return;
			IsInitialized = true;

			Color.SetAccent(ColorExtensions.AccentColor.ToColor());

			Log.Listeners.Add(new DelegateLogListener((c, m) => Trace.WriteLine(m, c)));

#if __MOBILE__
			Device.SetIdiom(UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? TargetIdiom.Tablet : TargetIdiom.Phone);
			Device.SetFlowDirection(UIApplication.SharedApplication.UserInterfaceLayoutDirection.ToFlowDirection());
#else
			// Subscribe to notifications in OS Theme changes
			NSDistributedNotificationCenter.GetDefaultCenter().AddObserver((NSString)"AppleInterfaceThemeChangedNotification", (n) =>
			{
				var interfaceStyle = NSUserDefaults.StandardUserDefaults.StringForKey("AppleInterfaceStyle");

				var aquaAppearance = NSAppearance.GetAppearance(interfaceStyle == "Dark" ? NSAppearance.NameDarkAqua : NSAppearance.NameAqua);
				NSApplication.SharedApplication.Appearance = aquaAppearance;

				Application.Current?.TriggerThemeChanged(new AppThemeChangedEventArgs(interfaceStyle == "Dark" ? OSAppTheme.Dark : OSAppTheme.Light));
			});

			Device.SetIdiom(TargetIdiom.Desktop);
			Device.SetFlowDirection(NSApplication.SharedApplication.UserInterfaceLayoutDirection.ToFlowDirection());

			if (IsMojaveOrNewer)
			{
				var interfaceStyle = NSUserDefaults.StandardUserDefaults.StringForKey("AppleInterfaceStyle");
				var aquaAppearance = NSAppearance.GetAppearance(interfaceStyle == "Dark" ? NSAppearance.NameDarkAqua : NSAppearance.NameAqua);
				NSApplication.SharedApplication.Appearance = aquaAppearance;
			}
#endif
			Device.SetFlags(s_flags);
			var platformServices = new IOSPlatformServices();

			Device.PlatformServices = platformServices;

#if __MOBILE__
			Device.PlatformInvalidator = platformServices;
			Device.Info = new IOSDeviceInfo();
#else
			Device.Info = new Platform.macOS.MacDeviceInfo();
#endif

			Internals.Registrar.RegisterAll(new[]
				{ typeof(ExportRendererAttribute), typeof(ExportCellAttribute), typeof(ExportImageSourceHandlerAttribute), typeof(ExportFontAttribute) });
			ExpressionSearch.Default = new iOSExpressionSearch();
		}

		public static event EventHandler<ViewInitializedEventArgs> ViewInitialized;

		internal static void SendViewInitialized(this VisualElement self, TNativeView nativeView)
		{
			ViewInitialized?.Invoke(self, new ViewInitializedEventArgs { View = self, NativeView = nativeView });
		}

		class iOSExpressionSearch : ExpressionVisitor, IExpressionSearch
		{
			List<object> _results;
			Type _targetType;

			public List<T> FindObjects<T>(Expression expression) where T : class
			{
				_results = new List<object>();
				_targetType = typeof(T);
				Visit(expression);
				return _results.Select(o => o as T).ToList();
			}

			protected override Expression VisitMember(MemberExpression node)
			{
				if (node.Expression is ConstantExpression && node.Member is FieldInfo)
				{
					var container = ((ConstantExpression)node.Expression).Value;
					var value = ((FieldInfo)node.Member).GetValue(container);

					if (_targetType.IsInstanceOfType(value))
						_results.Add(value);
				}
				return base.VisitMember(node);
			}
		}

		class IOSPlatformServices : IPlatformServices
#if __MOBILE__
			, IPlatformInvalidate
#endif
		{
			readonly double _fontScalingFactor = 1;
			public IOSPlatformServices()
			{
#if __MOBILE__
				//The standard accessibility size for a font is 17, we can get a
				//close approximation to the new Size by multiplying by this scale factor
				_fontScalingFactor = (double)UIFont.PreferredBody.PointSize / 17f;
#endif
			}

			public void BeginInvokeOnMainThread(Action action)
			{
				NSRunLoop.Main.BeginInvokeOnMainThread(action.Invoke);
			}

			public Ticker CreateTicker()
			{
				return new CADisplayLinkTicker();
			}

			public Assembly[] GetAssemblies()
			{
				return AppDomain.CurrentDomain.GetAssemblies();
			}

			public string GetHash(string input) => Crc64.GetHash(input);

			string IPlatformServices.GetMD5Hash(string input) => GetHash(input);

			public double GetNamedSize(NamedSize size, Type targetElementType, bool useOldSizes)
			{
				// We make these up anyway, so new sizes didn't really change
				// iOS docs say default button font size is 15, default label font size is 17 so we use those as the defaults.
				var scalingFactor = _fontScalingFactor;

				if (Application.Current?.On<iOS>().GetEnableAccessibilityScalingForNamedFontSizes() == false)
				{
					scalingFactor = 1;
				}

				switch (size)
				{
					//We multiply the fonts by the scale factor, and cast to an int, to make them whole numbers.
					case NamedSize.Default:
						return (int)((typeof(Button).IsAssignableFrom(targetElementType) ? 15 : 17) * scalingFactor);
					case NamedSize.Micro:
						return (int)(12 * scalingFactor);
					case NamedSize.Small:
						return (int)(14 * scalingFactor);
					case NamedSize.Medium:
						return (int)(17 * scalingFactor);
					case NamedSize.Large:
						return (int)(22 * scalingFactor);
#if __IOS__
					case NamedSize.Body:
						return (double)UIFont.PreferredBody.PointSize;
					case NamedSize.Caption:
						return (double)UIFont.PreferredCaption1.PointSize;
					case NamedSize.Header:
						return (double)UIFont.PreferredHeadline.PointSize;
					case NamedSize.Subtitle:
						return (double)UIFont.PreferredTitle2.PointSize;
					case NamedSize.Title:
						return (double)UIFont.PreferredTitle1.PointSize;
#else
					case NamedSize.Body:
						return 23;
					case NamedSize.Caption:
						return 18;
					case NamedSize.Header:
						return 23;
					case NamedSize.Subtitle:
						return 28;
					case NamedSize.Title:
						return 34;

#endif
					default:
						throw new ArgumentOutOfRangeException("size");
				}
			}

			public Color GetNamedColor(string name)
			{
#if __IOS__
				UIColor resultColor = null;

				// If not iOS 13, but 11+ we can only get the named colors
				if (!IsiOS13OrNewer && IsiOS11OrNewer)
					return (resultColor = UIColor.FromName(name)) == null ? Color.Default : resultColor.ToColor();

				// If iOS 13+ check all dynamic colors too
				switch (name)
				{
					// Ignore obsolete Xamarin.iOS colors for now
					// until this Xamarin.iOS version has been adopted more widely
					// This is just a change in Xamarin.iOS not UIKit
#pragma warning disable CS0618 // Type or member is obsolete
					case NamedPlatformColor.Label:
						resultColor = UIColor.Label;
						break;
					case NamedPlatformColor.Link:
						resultColor = UIColor.Link;
						break;
					case NamedPlatformColor.OpaqueSeparator:
						resultColor = UIColor.OpaqueSeparator;
						break;
					case NamedPlatformColor.PlaceholderText:
						resultColor = UIColor.PlaceholderText;
						break;
					case NamedPlatformColor.QuaternaryLabel:
						resultColor = UIColor.QuaternaryLabel;
						break;
					case NamedPlatformColor.SecondaryLabel:
						resultColor = UIColor.SecondaryLabel;
						break;
					case NamedPlatformColor.Separator:
						resultColor = UIColor.Separator;
						break;
					case NamedPlatformColor.SystemBlue:
						resultColor = UIColor.SystemBlue;
						break;
					case NamedPlatformColor.SystemGray:
						resultColor = UIColor.SystemGray;
						break;
					case NamedPlatformColor.SystemGray2:
						resultColor = UIColor.SystemGray2;
						break;
					case NamedPlatformColor.SystemGray3:
						resultColor = UIColor.SystemGray3;
						break;
					case NamedPlatformColor.SystemGray4:
						resultColor = UIColor.SystemGray4;
						break;
					case NamedPlatformColor.SystemGray5:
						resultColor = UIColor.SystemGray5;
						break;
					case NamedPlatformColor.SystemGray6:
						resultColor = UIColor.SystemGray6;
						break;
					case NamedPlatformColor.SystemGreen:
						resultColor = UIColor.SystemGreen;
						break;
					case NamedPlatformColor.SystemIndigo:
						resultColor = UIColor.SystemIndigo;
						break;
					case NamedPlatformColor.SystemOrange:
						resultColor = UIColor.SystemOrange;
						break;
					case NamedPlatformColor.SystemPink:
						resultColor = UIColor.SystemPink;
						break;
					case NamedPlatformColor.SystemPurple:
						resultColor = UIColor.SystemPurple;
						break;
					case NamedPlatformColor.SystemRed:
						resultColor = UIColor.SystemRed;
						break;
					case NamedPlatformColor.SystemTeal:
						resultColor = UIColor.SystemTeal;
						break;
					case NamedPlatformColor.SystemYellow:
						resultColor = UIColor.SystemYellow;
						break;
					case NamedPlatformColor.TertiaryLabel:
						resultColor = UIColor.TertiaryLabel;
						break;
#pragma warning restore CS0618 // Type or member is obsolete
					default:
						resultColor = UIColor.FromName(name);
						break;
				}

				if (resultColor == null)
					return Color.Default;

				return resultColor.ToColor();
#elif __MACOS__

				NSColor resultColor = null;

				switch (name)
				{
					case NamedPlatformColor.AlternateSelectedControlTextColor:
						resultColor = NSColor.AlternateSelectedControlText;
							break;
					case NamedPlatformColor.ControlAccent:
						if (IsMojaveOrNewer)
							resultColor = NSColor.ControlAccent;
						break;
					case NamedPlatformColor.ControlBackgroundColor:
						resultColor = NSColor.ControlBackground;
						break;
					case NamedPlatformColor.ControlColor:
						resultColor = NSColor.Control;
						break;
					case NamedPlatformColor.ControlTextColor:
						resultColor = NSColor.ControlText;
						break;
					case NamedPlatformColor.DisabledControlTextColor:
						resultColor = NSColor.DisabledControlText;
						break;
					case NamedPlatformColor.FindHighlightColor:
						if (IsHighSierraOrNewer)
							resultColor = NSColor.FindHighlight;
						break;
					case NamedPlatformColor.GridColor:
						resultColor = NSColor.Grid;
						break;
					case NamedPlatformColor.HeaderTextColor:
						resultColor = NSColor.HeaderText;
						break;
					case NamedPlatformColor.HighlightColor:
						resultColor = NSColor.Highlight;
						break;
					case NamedPlatformColor.KeyboardFocusIndicatorColor:
						resultColor = NSColor.KeyboardFocusIndicator;
						break;
					case NamedPlatformColor.LabelColor:
						resultColor = NSColor.Label;
						break;
					case NamedPlatformColor.LinkColor:
						resultColor = NSColor.Link;
						break;
					case NamedPlatformColor.PlaceholderTextColor:
						resultColor = NSColor.PlaceholderText;
						break;
					case NamedPlatformColor.QuaternaryLabelColor:
						resultColor = NSColor.QuaternaryLabel;
						break;
					case NamedPlatformColor.SecondaryLabelColor:
						resultColor = NSColor.SecondaryLabel;
						break;
					case NamedPlatformColor.SelectedContentBackgroundColor:
						resultColor = NSColor.SelectedContentBackground;
						break;
					case NamedPlatformColor.SelectedControlColor:
						resultColor = NSColor.SelectedControl;
						break;
					case NamedPlatformColor.SelectedControlTextColor:
						resultColor = NSColor.SelectedControlText;
						break;
					case NamedPlatformColor.SelectedMenuItemTextColor:
						resultColor = NSColor.SelectedMenuItemText;
						break;
					case NamedPlatformColor.SelectedTextBackgroundColor:
						resultColor = NSColor.SelectedTextBackground;
						break;
					case NamedPlatformColor.SelectedTextColor:
						resultColor = NSColor.SelectedText;
						break;
					case NamedPlatformColor.SeparatorColor:
						resultColor = NSColor.Separator;
						break;
					case NamedPlatformColor.ShadowColor:
						resultColor = NSColor.Shadow;
						break;
					case NamedPlatformColor.TertiaryLabelColor:
						resultColor = NSColor.TertiaryLabel;
						break;
					case NamedPlatformColor.TextBackgroundColor:
						resultColor = NSColor.TextBackground;
						break;
					case NamedPlatformColor.TextColor:
						resultColor = NSColor.Text;
						break;
					case NamedPlatformColor.UnderPageBackgroundColor:
						resultColor = NSColor.UnderPageBackground;
						break;
					case NamedPlatformColor.UnemphasizedSelectedContentBackgroundColor:
						if (IsMojaveOrNewer)
							resultColor = NSColor.UnemphasizedSelectedContentBackground;
						break;
					case NamedPlatformColor.UnemphasizedSelectedTextBackgroundColor:
						if (IsMojaveOrNewer)
							resultColor = NSColor.UnemphasizedSelectedTextBackground;
						break;
					case NamedPlatformColor.UnemphasizedSelectedTextColor:
						if (IsMojaveOrNewer)
							resultColor = NSColor.UnemphasizedSelectedText;
						break;
					case NamedPlatformColor.WindowBackgroundColor:
						resultColor = NSColor.WindowBackground;
						break;
					case NamedPlatformColor.WindowFrameTextColor:
						resultColor = NSColor.WindowFrameText;
						break;
					case NamedPlatformColor.Label:
						resultColor = NSColor.Label;
						break;
					case NamedPlatformColor.Link:
						resultColor = NSColor.Link;
						break;
					case NamedPlatformColor.PlaceholderText:
						resultColor = NSColor.PlaceholderText;
						break;
					case NamedPlatformColor.QuaternaryLabel:
						resultColor = NSColor.QuaternaryLabel;
						break;
					case NamedPlatformColor.SecondaryLabel:
						resultColor = NSColor.SecondaryLabel;
						break;
					case NamedPlatformColor.Separator:
						if (IsMojaveOrNewer)
							resultColor = NSColor.Separator;
						break;
					case NamedPlatformColor.SystemBlue:
						resultColor = NSColor.SystemBlue;
						break;
					case NamedPlatformColor.SystemGray:
						resultColor = NSColor.SystemGray;
						break;
					case NamedPlatformColor.SystemGreen:
						resultColor = NSColor.SystemGreen;
						break;
					case NamedPlatformColor.SystemIndigo:
						resultColor = NSColor.SystemIndigo;
						break;
					case NamedPlatformColor.SystemOrange:
						resultColor = NSColor.SystemOrange;
						break;
					case NamedPlatformColor.SystemPink:
						resultColor = NSColor.SystemPink;
						break;
					case NamedPlatformColor.SystemPurple:
						resultColor = NSColor.SystemPurple;
						break;
					case NamedPlatformColor.SystemRed:
						resultColor = NSColor.SystemRed;
						break;
					case NamedPlatformColor.SystemTeal:
						resultColor = NSColor.SystemTeal;
						break;
					case NamedPlatformColor.SystemYellow:
						resultColor = NSColor.SystemYellow;
						break;
					case NamedPlatformColor.TertiaryLabel:
						resultColor = NSColor.TertiaryLabel;
						break;
					default:
						resultColor = NSColor.FromName(name);
						break;
				}

				if (resultColor == null)
					return Color.Default;

				return resultColor.ToColor(NSColorSpace.GenericRGBColorSpace);
#else
				return Color.Default;
#endif
			}

			public async Task<Stream> GetStreamAsync(Uri uri, CancellationToken cancellationToken)
			{
				using (var client = GetHttpClient())
				{
					// Do not remove this await otherwise the client will dispose before
					// the stream even starts
					var result = await StreamWrapper.GetStreamAsync(uri, cancellationToken, client).ConfigureAwait(false);

					return result;
				}
			}

			public IIsolatedStorageFile GetUserStoreForApplication()
			{
				return new _IsolatedStorageFile(IsolatedStorageFile.GetUserStoreForApplication());
			}

			public bool IsInvokeRequired => !NSThread.IsMain;

#if __MOBILE__
			public string RuntimePlatform => Device.iOS;
#else
			public string RuntimePlatform => Device.macOS;
#endif

			public void OpenUriAction(Uri uri)
			{
				NSUrl url;

				if (uri.Scheme == "tel" || uri.Scheme == "mailto")
					url = new NSUrl(uri.AbsoluteUri);
				else
					url = NSUrl.FromString(uri.OriginalString) ?? new NSUrl(uri.Scheme, uri.Host, uri.PathAndQuery);
#if __MOBILE__
				UIApplication.SharedApplication.OpenUrl(url);
#else
				NSWorkspace.SharedWorkspace.OpenUrl(url);
#endif
			}

			public void StartTimer(TimeSpan interval, Func<bool> callback)
			{
				NSTimer timer = NSTimer.CreateRepeatingTimer(interval, t =>
				{
					if (!callback())
						t.Invalidate();
				});
				NSRunLoop.Main.AddTimer(timer, NSRunLoopMode.Common);
			}

			HttpClient GetHttpClient()
			{
				var proxy = CoreFoundation.CFNetwork.GetSystemProxySettings();
				var handler = new HttpClientHandler();
				if (!string.IsNullOrEmpty(proxy.HTTPProxy))
				{
					handler.Proxy = CoreFoundation.CFNetwork.GetDefaultProxy();
					handler.UseProxy = true;
				}
				return new HttpClient(handler);
			}

			static int Hex(int v)
			{
				if (v < 10)
					return '0' + v;
				return 'a' + v - 10;
			}

			public class _IsolatedStorageFile : IIsolatedStorageFile
			{
				readonly IsolatedStorageFile _isolatedStorageFile;

				public _IsolatedStorageFile(IsolatedStorageFile isolatedStorageFile)
				{
					_isolatedStorageFile = isolatedStorageFile;
				}

				public Task CreateDirectoryAsync(string path)
				{
					_isolatedStorageFile.CreateDirectory(path);
					return Task.FromResult(true);
				}

				public Task<bool> GetDirectoryExistsAsync(string path)
				{
					return Task.FromResult(_isolatedStorageFile.DirectoryExists(path));
				}

				public Task<bool> GetFileExistsAsync(string path)
				{
					return Task.FromResult(_isolatedStorageFile.FileExists(path));
				}

				public Task<DateTimeOffset> GetLastWriteTimeAsync(string path)
				{
					return Task.FromResult(_isolatedStorageFile.GetLastWriteTime(path));
				}

				public Task<Stream> OpenFileAsync(string path, FileMode mode, FileAccess access)
				{
					Stream stream = _isolatedStorageFile.OpenFile(path, mode, access);
					return Task.FromResult(stream);
				}

				public Task<Stream> OpenFileAsync(string path, FileMode mode, FileAccess access, FileShare share)
				{
					Stream stream = _isolatedStorageFile.OpenFile(path, mode, access, share);
					return Task.FromResult(stream);
				}
			}

			public void QuitApplication()
			{
#if __MOBILE__
				Log.Warning(nameof(IOSPlatformServices), "Platform doesn't implement QuitApp");
#else
				NSApplication.SharedApplication.Terminate(new NSObject());
#endif
			}

			public SizeRequest GetNativeSize(VisualElement view, double widthConstraint, double heightConstraint)
			{
#if __MOBILE__
				return Platform.iOS.Platform.GetNativeSize(view, widthConstraint, heightConstraint);
#else
				return Platform.MacOS.Platform.GetNativeSize(view, widthConstraint, heightConstraint);
#endif
			}

			public OSAppTheme RequestedTheme
			{
				get
				{
#if __IOS__ || __TVOS__
					if (!IsiOS13OrNewer)
						return OSAppTheme.Unspecified;
					var uiStyle = GetCurrentUIViewController()?.TraitCollection?.UserInterfaceStyle ??
						UITraitCollection.CurrentTraitCollection.UserInterfaceStyle;

					switch (uiStyle)
					{
						case UIUserInterfaceStyle.Light:
							return OSAppTheme.Light;
						case UIUserInterfaceStyle.Dark:
							return OSAppTheme.Dark;
						default:
							return OSAppTheme.Unspecified;
					};
#else
                    return AppearanceIsDark() ? OSAppTheme.Dark : OSAppTheme.Light;
#endif
				}
			}

#if __MACOS__
			bool AppearanceIsDark()
			{
				if (IsMojaveOrNewer)
				{
					var appearance = NSApplication.SharedApplication.EffectiveAppearance;
					var matchedAppearance = appearance.FindBestMatch(new string[] { NSAppearance.NameAqua, NSAppearance.NameDarkAqua });

					return matchedAppearance == NSAppearance.NameDarkAqua;
				}
				else
				{
					return false;
				}
			}
#endif

#if __IOS__ || __TVOS__

			static UIViewController GetCurrentUIViewController() =>
				GetCurrentViewController(false);

			static UIViewController GetCurrentViewController(bool throwIfNull = true)
			{
				UIViewController viewController = null;

				var window = UIApplication.SharedApplication.GetKeyWindow();

				if (window != null && window.WindowLevel == UIWindowLevel.Normal)
					viewController = window.RootViewController;

				if (viewController == null)
				{
					window = UIApplication.SharedApplication
						.Windows
						.OrderByDescending(w => w.WindowLevel)
						.FirstOrDefault(w => w.RootViewController != null && w.WindowLevel == UIWindowLevel.Normal);

					if (window == null && throwIfNull)
						throw new InvalidOperationException("Could not find current view controller.");
					else
						viewController = window?.RootViewController;
				}

				while (viewController?.PresentedViewController != null)
					viewController = viewController.PresentedViewController;

				if (throwIfNull && viewController == null)
					throw new InvalidOperationException("Could not find current view controller.");

				return viewController;
			}

			public void Invalidate(VisualElement visualElement)
			{
				var renderer = Platform.iOS.Platform.GetRenderer(visualElement);

				if (renderer == null)
				{
					return;
				}

				renderer.NativeView.SetNeedsLayout();
			}
#endif
		}
	}
}