using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;
using UIKit;
using Drastic.UI.Internals;
using static Drastic.UI.PlatformConfiguration.iOSSpecific.Page;
using PageUIStatusBarAnimation = Drastic.UI.PlatformConfiguration.iOSSpecific.UIStatusBarAnimation;
using TabbedPageConfiguration = Drastic.UI.PlatformConfiguration.iOSSpecific.TabbedPage;
using TranslucencyMode = Drastic.UI.PlatformConfiguration.iOSSpecific.TranslucencyMode;
using RectangleF = CoreGraphics.CGRect;

namespace Drastic.UI.Platform.iOS
{
	public class TabbedRenderer : UITabBarController, IVisualElementRenderer, IEffectControlProvider
	{
		bool _barBackgroundColorWasSet;
		bool _barTextColorWasSet;
		UIColor _defaultBarTextColor;
		bool _defaultBarTextColorSet;
		UIColor _defaultBarColor;
		bool _defaultBarColorSet;
		bool? _defaultBarTranslucent;
		bool _loaded;
		Size _queuedSize;
		UITabBarAppearance _tabBarAppearance;

		Page Page => Element as Page;

		[Internals.Preserve(Conditional = true)]
		public TabbedRenderer()
		{

		}

		public override UIViewController SelectedViewController
		{
			get { return base.SelectedViewController; }
			set
			{
				base.SelectedViewController = value;
				UpdateCurrentPage();
			}
		}

		protected TabbedPage Tabbed
		{
			get { return (TabbedPage)Element; }
		}

		public VisualElement Element { get; private set; }

		public event EventHandler<VisualElementChangedEventArgs> ElementChanged;

		public SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			return NativeView.GetSizeRequest(widthConstraint, heightConstraint);
		}

		public UIView NativeView
		{
			get { return View; }
		}

		public void SetElement(VisualElement element)
		{
			var oldElement = Element;
			Element = element;

			FinishedCustomizingViewControllers += HandleFinishedCustomizingViewControllers;
			Tabbed.PropertyChanged += OnPropertyChanged;
			Tabbed.PagesChanged += OnPagesChanged;

			OnElementChanged(new VisualElementChangedEventArgs(oldElement, element));

			OnPagesChanged(null, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

			if (element != null)
				element.SendViewInitialized(NativeView);

			//disable edit/reorder of tabs
			CustomizableViewControllers = null;

			UpdateBarBackgroundColor();
			UpdateBarBackground();
			UpdateBarTextColor();
			UpdateSelectedTabColors();
			UpdateBarTranslucent();

			EffectUtilities.RegisterEffectControlProvider(this, oldElement, element);
		}

		public void SetElementSize(Size size)
		{
			if (_loaded)
				Element.Layout(new Rectangle(Element.X, Element.Y, size.Width, size.Height));
			else
				_queuedSize = size;
		}

		public UIViewController ViewController
		{
			get { return this; }
		}

		public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
		{
			base.DidRotate(fromInterfaceOrientation);

			View.SetNeedsLayout();
		}

		public override void ViewDidAppear(bool animated)
		{
			Page.SendAppearing();
			base.ViewDidAppear(animated);
		}

		public override void ViewDidDisappear(bool animated)
		{
			base.ViewDidDisappear(animated);
			Page.SendDisappearing();
		}

		public override void ViewDidLayoutSubviews()
		{
			base.ViewDidLayoutSubviews();

			if (Element == null)
				return;

			if (Element.Parent is BaseShellItem)
				Element.Layout(View.Bounds.ToRectangle());

			var frame = View.Frame;
			var tabBarFrame = TabBar.Frame;
			Page.ContainerArea = new Rectangle(0, 0, frame.Width, frame.Height - tabBarFrame.Height);
			if (!_queuedSize.IsZero)
			{
				Element.Layout(new Rectangle(Element.X, Element.Y, _queuedSize.Width, _queuedSize.Height));
				_queuedSize = Size.Zero;
			}
			_loaded = true;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				Page.SendDisappearing();

				_tabBarAppearance?.Dispose();
				_tabBarAppearance = null;

				Tabbed.PropertyChanged -= OnPropertyChanged;
				Tabbed.PagesChanged -= OnPagesChanged;
				FinishedCustomizingViewControllers -= HandleFinishedCustomizingViewControllers;
			}

			base.Dispose(disposing);
		}

		protected virtual void OnElementChanged(VisualElementChangedEventArgs e)
		{
			var changed = ElementChanged;
			if (changed != null)
				changed(this, e);
		}

		UIViewController GetViewController(Page page)
		{
			var renderer = Platform.GetRenderer(page);
			if (renderer == null)
				return null;

			return renderer.ViewController;
		}

		void HandleFinishedCustomizingViewControllers(object sender, UITabBarCustomizeChangeEventArgs e)
		{
			if (e.Changed)
				UpdateChildrenOrderIndex(e.ViewControllers);
		}

		void OnPagePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			// Setting TabBarItem.Title in iOS 10 causes rendering bugs
			// Work around this by creating a new UITabBarItem on each change
			if (e.PropertyName == Page.TitleProperty.PropertyName && !Forms.IsiOS10OrNewer)
			{
				var page = (Page)sender;
				var renderer = Platform.GetRenderer(page);
				if (renderer == null)
					return;

				if (renderer.ViewController.TabBarItem != null)
					renderer.ViewController.TabBarItem.Title = page.Title;
			}
			else if (e.PropertyName == Page.IconImageSourceProperty.PropertyName || e.PropertyName == Page.TitleProperty.PropertyName && Forms.IsiOS10OrNewer)
			{
				var page = (Page)sender;

				IVisualElementRenderer renderer = Platform.GetRenderer(page);

				if (renderer?.ViewController.TabBarItem == null)
					return;

				SetTabBarItem(renderer);
			}
		}

		void OnPagesChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			e.Apply((o, i, c) => SetupPage((Page)o, i), (o, i) => TeardownPage((Page)o, i), Reset);

			SetControllers();

			UIViewController controller = null;
			if (Tabbed.CurrentPage != null)
				controller = GetViewController(Tabbed.CurrentPage);
			if (controller != null && controller != base.SelectedViewController)
				base.SelectedViewController = controller;

			UpdateBarBackgroundColor();
			UpdateBarTextColor();
			UpdateSelectedTabColors();
		}

		void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(TabbedPage.CurrentPage))
			{
				var current = Tabbed.CurrentPage;
				if (current == null)
					return;

				var controller = GetViewController(current);
				if (controller == null)
					return;

				SelectedViewController = controller;
			}
			else if (e.PropertyName == TabbedPage.BarBackgroundColorProperty.PropertyName)
				UpdateBarBackgroundColor();
			else if (e.PropertyName == TabbedPage.BarBackgroundProperty.PropertyName)
				UpdateBarBackground();
			else if (e.PropertyName == TabbedPage.BarTextColorProperty.PropertyName)
				UpdateBarTextColor();
			else if (e.PropertyName == PrefersStatusBarHiddenProperty.PropertyName)
				UpdatePrefersStatusBarHiddenOnPages();
			else if (e.PropertyName == PreferredStatusBarUpdateAnimationProperty.PropertyName)
				UpdateCurrentPagePreferredStatusBarUpdateAnimation();
			else if (e.PropertyName == TabbedPage.SelectedTabColorProperty.PropertyName || e.PropertyName == TabbedPage.UnselectedTabColorProperty.PropertyName)
				UpdateSelectedTabColors();
			else if (e.PropertyName == PrefersHomeIndicatorAutoHiddenProperty.PropertyName)
				UpdatePrefersHomeIndicatorAutoHiddenOnPages();
			else if (e.PropertyName == TabbedPageConfiguration.TranslucencyModeProperty.PropertyName)
				UpdateBarTranslucent();

		}

		public override UIViewController ChildViewControllerForStatusBarHidden()
		{
			var current = Tabbed.CurrentPage;
			if (current == null)
				return null;

			return GetViewController(current);
		}

		void UpdateCurrentPagePreferredStatusBarUpdateAnimation()
		{
			PageUIStatusBarAnimation animation = ((Page)Element).OnThisPlatform().PreferredStatusBarUpdateAnimation();
			Tabbed.CurrentPage.OnThisPlatform().SetPreferredStatusBarUpdateAnimation(animation);
		}

		void UpdatePrefersStatusBarHiddenOnPages()
		{
			for (var i = 0; i < ViewControllers.Length; i++)
			{
				Tabbed.GetPageByIndex(i).OnThisPlatform().SetPrefersStatusBarHidden(Tabbed.OnThisPlatform().PrefersStatusBarHidden());
			}
		}

		public override UIViewController ChildViewControllerForHomeIndicatorAutoHidden
		{
			get
			{
				var current = Tabbed.CurrentPage;
				if (current == null)
					return null;

				return GetViewController(current);
			}
		}

		void UpdatePrefersHomeIndicatorAutoHiddenOnPages()
		{
			bool isHomeIndicatorHidden = Tabbed.OnThisPlatform().PrefersHomeIndicatorAutoHidden();
			for (var i = 0; i < ViewControllers.Length; i++)
			{
				Tabbed.GetPageByIndex(i).OnThisPlatform().SetPrefersHomeIndicatorAutoHidden(isHomeIndicatorHidden);
			}
		}

		void Reset()
		{
			var i = 0;
			foreach (var page in Tabbed.Children)
				SetupPage(page, i++);
		}

		void SetControllers()
		{
			var list = new List<UIViewController>();
			for (var i = 0; i < Element.LogicalChildren.Count; i++)
			{
				var child = Element.LogicalChildren[i];
				var v = child as VisualElement;
				if (v == null)
					continue;
				if (Platform.GetRenderer(v) != null)
					list.Add(Platform.GetRenderer(v).ViewController);
			}
			ViewControllers = list.ToArray();
		}

		void SetupPage(Page page, int index)
		{
			IVisualElementRenderer renderer = Platform.GetRenderer(page);
			if (renderer == null)
			{
				renderer = Platform.CreateRenderer(page);
				Platform.SetRenderer(page, renderer);
			}

			page.PropertyChanged += OnPagePropertyChanged;

			SetTabBarItem(renderer);
		}

		void TeardownPage(Page page, int index)
		{
			page.PropertyChanged -= OnPagePropertyChanged;

			Platform.SetRenderer(page, null);
		}

		void UpdateBarBackgroundColor()
		{
			if (Tabbed == null || TabBar == null)
				return;

			var barBackgroundColor = Tabbed.BarBackgroundColor;
			var isDefaultColor = barBackgroundColor.IsDefault;

			if (isDefaultColor && !_barBackgroundColorWasSet)
				return;

			if (!_defaultBarColorSet)
			{
				_defaultBarColor = TabBar.BarTintColor;

				_defaultBarColorSet = true;
			}

			if (!isDefaultColor)
				_barBackgroundColorWasSet = true;

			if (Forms.IsiOS15OrNewer)
				UpdateiOS15TabBarAppearance();
			else
				TabBar.BarTintColor = isDefaultColor ? _defaultBarColor : barBackgroundColor.ToUIColor();
		}

		void UpdateBarBackground()
		{
			if (Tabbed == null || TabBar == null)
				return;

			var barBackground = Tabbed.BarBackground;

			TabBar.UpdateBackground(barBackground);
		}

		void UpdateBarTextColor()
		{
			if (Tabbed == null || TabBar == null || TabBar.Items == null)
				return;

			var barTextColor = Tabbed.BarTextColor;
			var isDefaultColor = barTextColor.IsDefault;

			if (isDefaultColor && !_barTextColorWasSet)
				return;

			if (!_defaultBarTextColorSet)
			{
				_defaultBarTextColor = TabBar.TintColor;
				_defaultBarTextColorSet = true;
			}

			if (!isDefaultColor)
				_barTextColorWasSet = true;

			var attributes = new UIStringAttributes();

			//if (isDefaultColor)
			//	attributes.TextColor = _defaultBarTextColor;
			//else
			//	attributes.TextColor = barTextColor.ToUIColor();

			//foreach (UITabBarItem item in TabBar.Items)
			//{
			//	item.SetTitleTextAttributes(attributes, UIControlState.Normal);
			//}

			// set TintColor for selected icon
			// setting the unselected icon tint is not supported by iOS
			if (Forms.IsiOS15OrNewer)
				UpdateiOS15TabBarAppearance();
			else
				TabBar.TintColor = isDefaultColor ? _defaultBarTextColor : barTextColor.ToUIColor();
		}

		void UpdateBarTranslucent()
		{
			if (Tabbed == null || TabBar == null || Element == null)
				return;

			_defaultBarTranslucent = _defaultBarTranslucent ?? TabBar.Translucent;
			switch (TabbedPageConfiguration.GetTranslucencyMode(Element))
			{
				case TranslucencyMode.Translucent:
					TabBar.Translucent = true;
					return;
				case TranslucencyMode.Opaque:
					TabBar.Translucent = false;
					return;
				default:
					TabBar.Translucent = _defaultBarTranslucent.GetValueOrDefault();
					return;
			}
		}

		void UpdateChildrenOrderIndex(UIViewController[] viewControllers)
		{
			for (var i = 0; i < viewControllers.Length; i++)
			{
				var originalIndex = -1;
				if (int.TryParse(viewControllers[i].TabBarItem.Tag.ToString(), out originalIndex))
				{
					var page = (Page)Tabbed.InternalChildren[originalIndex];
					TabbedPage.SetIndex(page, i);
				}
			}
		}

		void UpdateCurrentPage()
		{
			var count = Tabbed.InternalChildren.Count;
			var index = (int)SelectedIndex;
			((TabbedPage)Element).CurrentPage = index >= 0 && index < count ? Tabbed.GetPageByIndex(index) : null;
		}

		void IEffectControlProvider.RegisterEffect(Effect effect)
		{
			VisualElementRenderer<VisualElement>.RegisterEffect(effect, View);
		}

		async void SetTabBarItem(IVisualElementRenderer renderer)
		{
			var page = renderer.Element as Page;
			if (page == null)
				throw new InvalidCastException($"{nameof(renderer)} must be a {nameof(Page)} renderer.");

			var icons = await GetIcon(page);
			renderer.ViewController.TabBarItem = new UITabBarItem(page.Title, icons?.Item1, icons?.Item2)
			{
				Tag = Tabbed.Children.IndexOf(page),
				AccessibilityIdentifier = page.AutomationId
			};
			icons?.Item1?.Dispose();
			icons?.Item2?.Dispose();
		}

		void UpdateSelectedTabColors()
		{
			if (Tabbed == null || TabBar == null || TabBar.Items == null)
				return;

			if (Tabbed.IsSet(TabbedPage.SelectedTabColorProperty) && Tabbed.SelectedTabColor != Color.Default)
			{
				if (Forms.IsiOS10OrNewer)
					TabBar.TintColor = Tabbed.SelectedTabColor.ToUIColor();
				else
					TabBar.SelectedImageTintColor = Tabbed.SelectedTabColor.ToUIColor();
			}
			else
			{
				if (Forms.IsiOS10OrNewer)
					TabBar.TintColor = UITabBar.Appearance.TintColor;
				else
					TabBar.SelectedImageTintColor = UITabBar.Appearance.SelectedImageTintColor;
			}

			if (!Forms.IsiOS10OrNewer)
				return;

			if (Forms.IsiOS15OrNewer)
				UpdateiOS15TabBarAppearance();
			else
			{
				if (Tabbed.IsSet(TabbedPage.UnselectedTabColorProperty) && Tabbed.UnselectedTabColor != Color.Default)
					TabBar.UnselectedItemTintColor = Tabbed.UnselectedTabColor.ToUIColor();
				else
					TabBar.UnselectedItemTintColor = UITabBar.Appearance.TintColor;
			}
		}

		/// <summary>
		/// Get the icon for the tab bar item of this page
		/// </summary>
		/// <returns>
		/// A tuple containing as item1: the unselected version of the icon, item2: the selected version of the icon (item2 can be null),
		/// or null if no icon should be set.
		/// </returns>
		protected virtual async Task<Tuple<UIImage, UIImage>> GetIcon(Page page)
		{
			var icon = await page.IconImageSource.GetNativeImageAsync();
			return icon == null ? null : Tuple.Create(icon, (UIImage)null);
		}

		void UpdateiOS15TabBarAppearance()
		{
			if (_tabBarAppearance == null)
			{
				_tabBarAppearance = new UITabBarAppearance();
				_tabBarAppearance.ConfigureWithDefaultBackground();
			}

			var barBackgroundColor = Tabbed.BarBackgroundColor;
			var isDefaultBarBackgroundColor = barBackgroundColor.IsDefault;

			// Set BarBackgroundColor
			_tabBarAppearance.BackgroundColor = isDefaultBarBackgroundColor ? _defaultBarColor : barBackgroundColor.ToUIColor();

			var barTextColor = Tabbed.BarTextColor;
			var isDefaultBarTextColor = barTextColor.IsDefault;

			// Set BarTextColor
			_tabBarAppearance.StackedLayoutAppearance.Normal.TitleTextAttributes = new UIStringAttributes
			{
				ForegroundColor = isDefaultBarTextColor ? _defaultBarTextColor : barTextColor.ToUIColor()
			};

			// Update colors for all variations of the appearance to also make it work for iPads, etc. which use different layouts for the tabbar
			// Also, set ParagraphStyle explicitly. This seems to be an iOS bug. If we don't do this, tab titles will be truncat...

			// Set SelectedTabColor
			if (Tabbed.IsSet(TabbedPage.SelectedItemProperty) && Tabbed.SelectedTabColor != Color.Default)
			{
				var foregroundColor = Tabbed.SelectedTabColor.ToUIColor();
				_tabBarAppearance.StackedLayoutAppearance.Selected.TitleTextAttributes = new UIStringAttributes { ForegroundColor = foregroundColor, ParagraphStyle = NSParagraphStyle.Default };
				_tabBarAppearance.StackedLayoutAppearance.Selected.IconColor = foregroundColor;

				_tabBarAppearance.InlineLayoutAppearance.Selected.TitleTextAttributes = new UIStringAttributes { ForegroundColor = foregroundColor, ParagraphStyle = NSParagraphStyle.Default };
				_tabBarAppearance.InlineLayoutAppearance.Selected.IconColor = foregroundColor;

				_tabBarAppearance.CompactInlineLayoutAppearance.Selected.TitleTextAttributes = new UIStringAttributes { ForegroundColor = foregroundColor, ParagraphStyle = NSParagraphStyle.Default };
				_tabBarAppearance.CompactInlineLayoutAppearance.Selected.IconColor = foregroundColor;
			}
			else
			{
				var foregroundColor = UITabBar.Appearance.TintColor;
				_tabBarAppearance.StackedLayoutAppearance.Selected.TitleTextAttributes = new UIStringAttributes { ForegroundColor = foregroundColor, ParagraphStyle = NSParagraphStyle.Default };
				_tabBarAppearance.StackedLayoutAppearance.Selected.IconColor = foregroundColor;

				_tabBarAppearance.InlineLayoutAppearance.Selected.TitleTextAttributes = new UIStringAttributes { ForegroundColor = foregroundColor, ParagraphStyle = NSParagraphStyle.Default };
				_tabBarAppearance.InlineLayoutAppearance.Selected.IconColor = foregroundColor;

				_tabBarAppearance.CompactInlineLayoutAppearance.Selected.TitleTextAttributes = new UIStringAttributes { ForegroundColor = foregroundColor, ParagraphStyle = NSParagraphStyle.Default };
				_tabBarAppearance.CompactInlineLayoutAppearance.Selected.IconColor = foregroundColor;
			}

			// Set UnselectedTabColor
			if (Tabbed.IsSet(TabbedPage.UnselectedTabColorProperty) && Tabbed.UnselectedTabColor != Color.Default)
			{
				var foregroundColor = Tabbed.UnselectedTabColor.ToUIColor();
				_tabBarAppearance.StackedLayoutAppearance.Normal.TitleTextAttributes = new UIStringAttributes { ForegroundColor = foregroundColor, ParagraphStyle = NSParagraphStyle.Default };
				_tabBarAppearance.StackedLayoutAppearance.Normal.IconColor = foregroundColor;

				_tabBarAppearance.InlineLayoutAppearance.Normal.TitleTextAttributes = new UIStringAttributes { ForegroundColor = foregroundColor, ParagraphStyle = NSParagraphStyle.Default };
				_tabBarAppearance.InlineLayoutAppearance.Normal.IconColor = foregroundColor;

				_tabBarAppearance.CompactInlineLayoutAppearance.Normal.TitleTextAttributes = new UIStringAttributes { ForegroundColor = foregroundColor, ParagraphStyle = NSParagraphStyle.Default };
				_tabBarAppearance.CompactInlineLayoutAppearance.Normal.IconColor = foregroundColor;
			}
			else
			{
				var foreground = UITabBar.Appearance.TintColor;
				_tabBarAppearance.StackedLayoutAppearance.Normal.TitleTextAttributes = new UIStringAttributes { ForegroundColor = foreground, ParagraphStyle = NSParagraphStyle.Default };
				_tabBarAppearance.StackedLayoutAppearance.Normal.IconColor = foreground;

				_tabBarAppearance.InlineLayoutAppearance.Normal.TitleTextAttributes = new UIStringAttributes { ForegroundColor = foreground, ParagraphStyle = NSParagraphStyle.Default };
				_tabBarAppearance.InlineLayoutAppearance.Normal.IconColor = foreground;

				_tabBarAppearance.CompactInlineLayoutAppearance.Normal.TitleTextAttributes = new UIStringAttributes { ForegroundColor = foreground, ParagraphStyle = NSParagraphStyle.Default };
				_tabBarAppearance.CompactInlineLayoutAppearance.Normal.IconColor = foreground;
			}

			// Set the TabBarAppearance
			TabBar.StandardAppearance = TabBar.ScrollEdgeAppearance = _tabBarAppearance;
		}
	}
}
