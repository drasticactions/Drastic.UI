﻿using System;
using System.Collections.Generic;
using CoreGraphics;
using Drastic.UI.Shapes;

#if __MOBILE__
namespace Drastic.UI.Platform.iOS
#else
namespace Drastic.UI.Platform.MacOS
#endif
{
    public static class GeometryExtensions
    {
        public static PathData ToCGPath(this Geometry geometry, Transform renderTransform = null)
        {
            PathData pathData = new PathData
            {
                Data = new CGPath()
            };

            CGAffineTransform transform;

            if (renderTransform == null)
                transform = CGAffineTransform.MakeIdentity();
            else
                transform = renderTransform.ToCGAffineTransform();

            if (geometry is LineGeometry)
            {
                LineGeometry lineGeometry = geometry as LineGeometry;
                pathData.Data.MoveToPoint(transform, lineGeometry.StartPoint.ToPointF());
                pathData.Data.AddLineToPoint(transform, lineGeometry.EndPoint.ToPointF());
            }
            else if (geometry is RectangleGeometry)
            {
                Rect rect = (geometry as RectangleGeometry).Rect;
                pathData.Data.AddRect(transform, new CGRect(rect.X, rect.Y, rect.Width, rect.Height));
            }
            else if (geometry is EllipseGeometry)
            {
                EllipseGeometry ellipseGeometry = geometry as EllipseGeometry;

                CGRect rect = new CGRect(
                    ellipseGeometry.Center.X - ellipseGeometry.RadiusX,
                    ellipseGeometry.Center.Y - ellipseGeometry.RadiusY,
                    ellipseGeometry.RadiusX * 2,
                    ellipseGeometry.RadiusY * 2);

                pathData.Data.AddEllipseInRect(transform, rect);
            }
            else if (geometry is GeometryGroup)
            {
                GeometryGroup geometryGroup = geometry as GeometryGroup;

                pathData.IsNonzeroFillRule = geometryGroup.FillRule == FillRule.Nonzero;

                foreach (Geometry child in geometryGroup.Children)
                {
                    PathData pathChild = child.ToCGPath(renderTransform);
                    pathData.Data.AddPath(pathChild.Data);
                }
            }
            else if (geometry is PathGeometry)
            {
                PathGeometry pathGeometry = geometry as PathGeometry;

                pathData.IsNonzeroFillRule = pathGeometry.FillRule == FillRule.Nonzero;

                foreach (PathFigure pathFigure in pathGeometry.Figures)
                {
                    pathData.Data.MoveToPoint(transform, pathFigure.StartPoint.ToPointF());
                    Point lastPoint = pathFigure.StartPoint;

                    foreach (PathSegment pathSegment in pathFigure.Segments)
                    {
                        // LineSegment
                        if (pathSegment is LineSegment)
                        {
                            LineSegment lineSegment = pathSegment as LineSegment;

                            pathData.Data.AddLineToPoint(transform, lineSegment.Point.ToPointF());
                            lastPoint = lineSegment.Point;
                        }
                        // PolyLineSegment
                        else if (pathSegment is PolyLineSegment)
                        {
                            PolyLineSegment polylineSegment = pathSegment as PolyLineSegment;
                            PointCollection points = polylineSegment.Points;

                            for (int i = 0; i < points.Count; i++)
                                pathData.Data.AddLineToPoint(transform, points[i].ToPointF());

                            lastPoint = points[points.Count - 1];
                        }
                        // BezierSegment
                        else if (pathSegment is BezierSegment)
                        {
                            BezierSegment bezierSegment = pathSegment as BezierSegment;

                            pathData.Data.AddCurveToPoint(
                                transform,
                                bezierSegment.Point1.ToPointF(),
                                bezierSegment.Point2.ToPointF(),
                                bezierSegment.Point3.ToPointF());

                            lastPoint = bezierSegment.Point3;
                        }
                        // PolyBezierSegment
                        else if (pathSegment is PolyBezierSegment)
                        {
                            PolyBezierSegment polyBezierSegment = pathSegment as PolyBezierSegment;
                            PointCollection points = polyBezierSegment.Points;

                            if (points.Count >= 3)
                            {
                                for (int i = 0; i < points.Count; i += 3)
                                {
                                    pathData.Data.AddCurveToPoint(
                                        transform,
                                        points[i].ToPointF(),
                                        points[i + 1].ToPointF(),
                                        points[i + 2].ToPointF());
                                }
                            }

                            lastPoint = points[points.Count - 1];
                        }
                        // QuadraticBezierSegment
                        else if (pathSegment is QuadraticBezierSegment)
                        {
                            QuadraticBezierSegment bezierSegment = pathSegment as QuadraticBezierSegment;

                            pathData.Data.AddQuadCurveToPoint(
                                transform,
                                new nfloat(bezierSegment.Point1.X),
                                new nfloat(bezierSegment.Point1.Y),
                                new nfloat(bezierSegment.Point2.X),
                                new nfloat(bezierSegment.Point2.Y));

                            lastPoint = bezierSegment.Point2;
                        }
                        // PolyQuadraticBezierSegment
                        else if (pathSegment is PolyQuadraticBezierSegment)
                        {
                            PolyQuadraticBezierSegment polyBezierSegment = pathSegment as PolyQuadraticBezierSegment;
                            PointCollection points = polyBezierSegment.Points;

                            if (points.Count >= 2)
                            {
                                for (int i = 0; i < points.Count; i += 2)
                                {
                                    pathData.Data.AddQuadCurveToPoint(
                                        transform,
                                        new nfloat(points[i + 0].X),
                                        new nfloat(points[i + 0].Y),
                                        new nfloat(points[i + 1].X),
                                        new nfloat(points[i + 1].Y));
                                }
                            }

                            lastPoint = points[points.Count - 1];
                        }
                        // ArcSegment
                        else if (pathSegment is ArcSegment)
                        {
                            ArcSegment arcSegment = pathSegment as ArcSegment;

                            List<Point> points = new List<Point>();

                            GeometryHelper.FlattenArc(
                                points,
                                lastPoint,
                                arcSegment.Point,
                                arcSegment.Size.Width,
                                arcSegment.Size.Height,
                                arcSegment.RotationAngle,
                                arcSegment.IsLargeArc,
                                arcSegment.SweepDirection == SweepDirection.CounterClockwise,
                                1);

                            for (int i = 0; i < points.Count; i++)
                            {
                                pathData.Data.AddLineToPoint(
                                    (nfloat)points[i].X,
                                    (nfloat)points[i].Y);
                            }

                            if (points.Count > 0)
                                lastPoint = points[points.Count - 1];
                        }
                    }

                    if (pathFigure.IsClosed)
                        pathData.Data.CloseSubpath();
                }
            }

            return pathData;
        }
    }
}