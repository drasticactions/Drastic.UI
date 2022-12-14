﻿using System.Collections.Specialized;
using System.ComponentModel;
using CoreGraphics;
using Drastic.UI.Shapes;

#if __MOBILE__
namespace Drastic.UI.Platform.iOS
#else
namespace Drastic.UI.Platform.MacOS
#endif
{
    public class PolylineRenderer : ShapeRenderer<Polyline, PolylineView>
    {
        PointCollection _points;

        [Internals.Preserve(Conditional = true)]
        public PolylineRenderer()
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<Polyline> args)
        {
            if (Control == null && args.NewElement != null)
            {
                SetNativeControl(new PolylineView());
            }

            base.OnElementChanged(args);

            if (args.NewElement != null)
            {
                UpdatePoints();
                UpdateFillRule();
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            base.OnElementPropertyChanged(sender, args);

            if (args.PropertyName == Polyline.PointsProperty.PropertyName)
                UpdatePoints();
            else if (args.PropertyName == Polyline.FillRuleProperty.PropertyName)
                UpdateFillRule();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (_points != null)
                {
                    _points.CollectionChanged -= OnCollectionChanged;
                    _points = null;
                }
            }
        }

        void UpdatePoints()
        {
            if (_points != null)
                _points.CollectionChanged -= OnCollectionChanged;

            _points = Element.Points;

            _points.CollectionChanged += OnCollectionChanged;

            Control.UpdatePoints(_points.ToCGPoints());
        }

        public void UpdateFillRule()
        {
            Control.UpdateFillMode(Element.FillRule == FillRule.Nonzero);
        }

        void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdatePoints();
        }
    }

    public class PolylineView : ShapeView
    {
        public void UpdatePoints(CGPoint[] points)
        {
            var path = new CGPath();
            path.AddLines(points);
            ShapeLayer.UpdateShape(path);
        }

        public void UpdateFillMode(bool fillMode)
        {
            ShapeLayer.UpdateFillMode(fillMode);
        }
    }
}