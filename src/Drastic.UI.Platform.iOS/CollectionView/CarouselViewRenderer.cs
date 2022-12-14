﻿using System;
using System.ComponentModel;
using Foundation;

namespace Drastic.UI.Platform.iOS
{
	public class CarouselViewRenderer : ItemsViewRenderer<CarouselView, CarouselViewController>
	{
		CarouselView Carousel => Element;
		ItemsViewLayout _layout;

		[Preserve(Conditional = true)]
		public CarouselViewRenderer()
		{
		}

		protected override CarouselViewController CreateController(CarouselView newElement, ItemsViewLayout layout)
		{
			return new CarouselViewController(newElement, layout);
		}

		protected override void ScrollToRequested(object sender, ScrollToRequestEventArgs args)
		{
			if (Carousel?.Loop == true)
			{
				var goToIndexPath = Controller.GetScrollToIndexPath(args.Index);

				if (!IsIndexPathValid(goToIndexPath))
				{
					return;
				}

				Controller.CollectionView.ScrollToItem(goToIndexPath,
					args.ScrollToPosition.ToCollectionViewScrollPosition(_layout.ScrollDirection),
					args.IsAnimated);
			}
			else
			{
				base.ScrollToRequested(sender, args);
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs changedProperty)
		{
			base.OnElementPropertyChanged(sender, changedProperty);

			if (changedProperty.Is(CarouselView.PeekAreaInsetsProperty))
			{
				(Controller.Layout as CarouselViewLayout)?.UpdateConstraints(Frame.Size);
				Controller.Layout.InvalidateLayout();
			}
			else if (changedProperty.Is(CarouselView.IsSwipeEnabledProperty))
				UpdateIsSwipeEnabled();
			else if (changedProperty.Is(CarouselView.IsBounceEnabledProperty))
				UpdateIsBounceEnabled();
		}

		protected override ItemsViewLayout SelectLayout() =>
				_layout = new CarouselViewLayout(Carousel.ItemsLayout, Carousel);


		protected override void SetUpNewElement(CarouselView newElement)
		{
			base.SetUpNewElement(newElement);
			UpdateIsSwipeEnabled();
			UpdateIsBounceEnabled();
		}

		protected override void TearDownOldElement(CarouselView oldElement)
		{
			Controller?.TearDown();
			base.TearDownOldElement(oldElement);
		}

		void UpdateIsSwipeEnabled()
		{
			if (Carousel == null)
				return;

			Controller.CollectionView.ScrollEnabled = Carousel.IsSwipeEnabled;
		}

		void UpdateIsBounceEnabled()
		{
			if (Carousel == null)
				return;

			Controller.CollectionView.Bounces = Carousel.IsBounceEnabled;
		}
	}
}
