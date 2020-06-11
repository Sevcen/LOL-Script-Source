﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using ClipperLib;
using SharpDX;
using Color = System.Drawing.Color;

namespace EloBuddy.SDK
{
    public static class Geometry
    {
        public static Vector3 SetZ(this Vector3 v, float? value = null)
        {
            if (value == null)
            {
                v.Z = Game.CursorPos.Z;
            }
            else
            {
                v.Z = (float) value;
            }
            return v;
        }

        // ReSharper disable once InconsistentNaming
        public static Vector3 SwitchYZ(this Vector3 v)
        {
            return new Vector3(v.X, v.Z, v.Y);
        }

        public static float Distance(this Vector2 point, Vector2 segmentStart, Vector2 segmentEnd, bool onlyIfOnSegment = false, bool squared = false)
        {
            var objects = point.ProjectOn(segmentStart, segmentEnd);

            if (objects.IsOnSegment || onlyIfOnSegment == false)
            {
                return squared
                    ? Vector2.DistanceSquared(objects.SegmentPoint, point)
                    : Vector2.Distance(objects.SegmentPoint, point);
            }
            return float.MaxValue;
        }

        public static Vector2 Shorten(this Vector2 v, Vector2 to, float distance)
        {
            return v - distance * (to - v).Normalized();
        }

        public static Vector3 Shorten(this Vector3 v, Vector3 to, float distance)
        {
            return v - distance * (to - v).Normalized();
        }

        public static Vector2 Perpendicular(this Vector2 v)
        {
            return new Vector2(-v.Y, v.X);
        }

        public static Vector2 Perpendicular2(this Vector2 v)
        {
            return new Vector2(v.Y, -v.X);
        }

        public static Vector2 Rotated(this Vector2 v, float angle)
        {
            var c = Math.Cos(angle);
            var s = Math.Sin(angle);

            return new Vector2((float) (v.X * c - v.Y * s), (float) (v.Y * c + v.X * s));
        }

        public static float CrossProduct(this Vector2 self, Vector2 other)
        {
            return other.Y * self.X - other.X * self.Y;
        }

        public static float DotProduct(this Vector2 self, Vector2 other)
        {
            return self.X * other.X + self.Y * other.Y;
        }

        public static float Magnitude(this Vector2 self, bool squared = false)
        {
            if (squared)
            {
                return self.X.Pow() + self.Y.Pow();
            }

            return (float) Math.Sqrt(self.X.Pow() + self.Y.Pow());
        }

        public static float RadianToDegree(double angle)
        {
            return (float) (angle * (180.0 / Math.PI));
        }

        public static float DegreeToRadian(double angle)
        {
            return (float) (Math.PI * angle / 180.0);
        }

        public static float Polar(this Vector2 v1)
        {
            if (Close(v1.X, 0, 0))
            {
                if (v1.Y > 0)
                {
                    return 90;
                }
                return v1.Y < 0 ? 270 : 0;
            }

            var theta = RadianToDegree(Math.Atan((v1.Y) / v1.X));
            if (v1.X < 0)
            {
                theta = theta + 180;
            }
            if (theta < 0)
            {
                theta = theta + 360;
            }
            return theta;
        }

        public static float AngleBetween(this Vector2 p1, Vector2 p2)
        {
            var theta = p1.Polar() - p2.Polar();
            if (theta < 0)
            {
                theta = theta + 360;
            }
            if (theta > 180)
            {
                theta = 360 - theta;
            }
            return theta;
        }

        public static Vector2 Closest(this Vector2 v, IEnumerable<Vector2> vList)
        {
            var result = new Vector2();
            var dist = float.MaxValue;

            foreach (var vector in vList)
            {
                var distance = v.Distance(vector, true);
                if (distance < dist)
                {
                    dist = distance;
                    result = vector;
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the total distance of a path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static float PathLength(this List<Vector2> path)
        {
            var distance = 0f;
            for (var i = 0; i < path.Count - 1; i++)
            {
                distance += path[i].Distance(path[i + 1]);
            }
            return distance;
        }

        public static ProjectionInfo ProjectOn(this Vector2 point, Vector2 segmentStart, Vector2 segmentEnd)
        {
            var cx = point.X;
            var cy = point.Y;
            var ax = segmentStart.X;
            var ay = segmentStart.Y;
            var bx = segmentEnd.X;
            var by = segmentEnd.Y;
            var rL = ((cx - ax) * (bx - ax) + (cy - ay) * (@by - ay)) / ((bx - ax).Pow() + (@by - ay).Pow());
            var pointLine = new Vector2(ax + rL * (bx - ax), ay + rL * (@by - ay));
            float rS;
            if (rL < 0)
            {
                rS = 0;
            }
            else if (rL > 1)
            {
                rS = 1;
            }
            else
            {
                rS = rL;
            }

            var isOnSegment = rS.CompareTo(rL) == 0;
            var pointSegment = isOnSegment ? pointLine : new Vector2(ax + rS * (bx - ax), ay + rS * (@by - ay));
            return new ProjectionInfo(isOnSegment, pointSegment, pointLine);
        }

        // Source: http://social.msdn.microsoft.com/Forums/vstudio/en-US/e5993847-c7a9-46ec-8edc-bfb86bd689e3/help-on-line-segment-intersection-algorithm
        public static IntersectionResult Intersection(this Vector2 lineSegment1Start, Vector2 lineSegment1End, Vector2 lineSegment2Start, Vector2 lineSegment2End)
        {
            double deltaACy = lineSegment1Start.Y - lineSegment2Start.Y;
            double deltaDCx = lineSegment2End.X - lineSegment2Start.X;
            double deltaACx = lineSegment1Start.X - lineSegment2Start.X;
            double deltaDCy = lineSegment2End.Y - lineSegment2Start.Y;
            double deltaBAx = lineSegment1End.X - lineSegment1Start.X;
            double deltaBAy = lineSegment1End.Y - lineSegment1Start.Y;

            var denominator = deltaBAx * deltaDCy - deltaBAy * deltaDCx;
            var numerator = deltaACy * deltaDCx - deltaACx * deltaDCy;

            if (Math.Abs(denominator) < float.Epsilon)
            {
                if (Math.Abs(numerator) < float.Epsilon)
                {
                    // collinear. Potentially infinite intersection points.
                    // Check and return one of them.
                    if (lineSegment1Start.X >= lineSegment2Start.X && lineSegment1Start.X <= lineSegment2End.X)
                    {
                        return new IntersectionResult(true, lineSegment1Start);
                    }
                    if (lineSegment2Start.X >= lineSegment1Start.X && lineSegment2Start.X <= lineSegment1End.X)
                    {
                        return new IntersectionResult(true, lineSegment2Start);
                    }
                    return new IntersectionResult();
                }
                // parallel
                return new IntersectionResult();
            }

            var r = numerator / denominator;
            if (r < 0 || r > 1)
            {
                return new IntersectionResult();
            }

            var s = (deltaACy * deltaBAx - deltaACx * deltaBAy) / denominator;
            if (s < 0 || s > 1)
            {
                return new IntersectionResult();
            }

            return new IntersectionResult(true,
                new Vector2((float) (lineSegment1Start.X + r * deltaBAx), (float) (lineSegment1Start.Y + r * deltaBAy)));
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public static object[] VectorMovementCollision(Vector2 startPoint1, Vector2 endPoint1, float v1, Vector2 startPoint2, float v2, float delay = 0f)
        {
            var sP1x = startPoint1.X;
            var sP1y = startPoint1.Y;
            var eP1x = endPoint1.X;
            var eP1y = endPoint1.Y;
            var sP2x = startPoint2.X;
            var sP2y = startPoint2.Y;

            float d = eP1x - sP1x, e = eP1y - sP1y;
            float dist = (float) Math.Sqrt(d * d + e * e), t1 = float.NaN;
            float S = Math.Abs(dist) > float.Epsilon ? v1 * d / dist : 0,
                K = (Math.Abs(dist) > float.Epsilon) ? v1 * e / dist : 0f;

            float r = sP2x - sP1x, j = sP2y - sP1y;
            var c = r * r + j * j;

            if (dist > 0f)
            {
                if (Math.Abs(v1 - float.MaxValue) < float.Epsilon)
                {
                    var t = dist / v1;
                    t1 = v2 * t >= 0f ? t : float.NaN;
                }
                else if (Math.Abs(v2 - float.MaxValue) < float.Epsilon)
                {
                    t1 = 0f;
                }
                else
                {
                    float a = S * S + K * K - v2 * v2, b = -r * S - j * K;

                    if (Math.Abs(a) < float.Epsilon)
                    {
                        if (Math.Abs(b) < float.Epsilon)
                        {
                            t1 = (Math.Abs(c) < float.Epsilon) ? 0f : float.NaN;
                        }
                        else
                        {
                            var t = -c / (2 * b);
                            t1 = (v2 * t >= 0f) ? t : float.NaN;
                        }
                    }
                    else
                    {
                        var sqr = b * b - a * c;
                        if (sqr >= 0)
                        {
                            var nom = (float) Math.Sqrt(sqr);
                            var t = (-nom - b) / a;
                            t1 = v2 * t >= 0f ? t : float.NaN;
                            t = (nom - b) / a;
                            var t2 = (v2 * t >= 0f) ? t : float.NaN;

                            if (!float.IsNaN(t2) && !float.IsNaN(t1))
                            {
                                if (t1 >= delay && t2 >= delay)
                                {
                                    t1 = Math.Min(t1, t2);
                                }
                                else if (t2 >= delay)
                                {
                                    t1 = t2;
                                }
                            }
                        }
                    }
                }
            }
            else if (Math.Abs(dist) < float.Epsilon)
            {
                t1 = 0f;
            }

            return new object[] { t1, (!float.IsNaN(t1)) ? new Vector2(sP1x + S * t1, sP1y + K * t1) : new Vector2() };
        }

        public static bool PointInLineSegment(Vector2 segmentStart, Vector2 segmentEnd, Vector2 point)
        {
            var crossproduct = (point.Y - segmentStart.Y) * (segmentEnd.X - segmentStart.X) - (point.X - segmentStart.X) * (segmentEnd.Y - segmentStart.Y);
            if (Math.Abs(crossproduct) > 2)
            {
                return false;
            }

            var dotproduct = (point.X - segmentStart.X) * (segmentEnd.X - segmentStart.X) + (point.Y - segmentStart.Y) * (segmentEnd.Y - segmentStart.Y);
            if (dotproduct < 0)
            {
                return false;
            }

            var squaredlengthba = (segmentEnd.X - segmentStart.X) * (segmentEnd.X - segmentStart.X) + (segmentEnd.Y - segmentStart.Y) * (segmentEnd.Y - segmentStart.Y);
            if (dotproduct > squaredlengthba)
            {
                return false;
            }

            return true;
        }

        [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
        public static int LineCircleIntersection(float cX, float cY, float radius, Vector2 segmentStart, Vector2 segmentEnd, out Vector2 intersection1, out Vector2 intersection2)
        {
            float t;

            var dx = segmentEnd.X - segmentStart.X;
            var dy = segmentEnd.Y - segmentStart.Y;

            var a = dx * dx + dy * dy;
            var b = 2 * (dx * (segmentStart.X - cX) + dy * (segmentStart.Y - cY));
            var c = (segmentStart.X - cX) * (segmentStart.X - cX) + (segmentStart.Y - cY) * (segmentStart.Y - cY) - radius * radius;
            var det = b * b - 4 * a * c;

            if (Math.Abs(a) <= float.Epsilon || (det < 0))
            {
                intersection1 = new Vector2(float.NaN, float.NaN);
                intersection2 = new Vector2(float.NaN, float.NaN);

                return 0;
            }
            if (det == 0)
            {
                t = -b / (2 * a);
                intersection1 = new Vector2(segmentStart.X + t * dx, segmentStart.Y + t * dy);
                intersection2 = new Vector2(float.NaN, float.NaN);

                return 1;
            }

            t = (float) ((-b + Math.Sqrt(det)) / (2 * a));
            intersection1 = new Vector2(segmentStart.X + t * dx, segmentStart.Y + t * dy);
            t = (float) ((-b - Math.Sqrt(det)) / (2 * a));
            intersection2 = new Vector2(segmentStart.X + t * dx, segmentStart.Y + t * dy);

            return 2;
        }

        public static int LineSegmentCircleIntersection(float cX, float cY, float radius, Vector2 segmentStart, Vector2 segmentEnd, out Vector2 intersection1, out Vector2 intersection2)
        {
            var count = LineCircleIntersection(cX, cY, radius, segmentStart, segmentEnd, out intersection1, out intersection2);
            var nan = new Vector2(float.NaN, float.NaN);

            if (!PointInLineSegment(segmentStart, segmentEnd, intersection2))
            {
                intersection2 = nan;
                count--;
            }

            if (!PointInLineSegment(segmentStart, segmentEnd, intersection1))
            {
                intersection1 = intersection2;
                count--;
            }

            return count;
        }

        public static int SegmentCircleIntersectionPriority(Vector2 segmentStart, Vector2 segmentEnd, Vector2 circlePos1, float radius1, Vector2 circlePos2, float radius2)
        {
            Vector2 aIntersection1;
            Vector2 aIntersection2;
            Vector2 bIntersection1;
            Vector2 bIntersection2;

            var aCount = LineSegmentCircleIntersection(circlePos1.X, circlePos1.Y, radius1, segmentStart, segmentEnd, out aIntersection1, out aIntersection2);
            var bCount = LineSegmentCircleIntersection(circlePos2.X, circlePos2.Y, radius2, segmentStart, segmentEnd, out bIntersection1, out bIntersection2);

            if (aCount == 0 && bCount == 0)
            {
                return 0;
            }

            if (bCount == 0 && aCount > 0)
            {
                return 1;
            }

            if (aCount == 0 && bCount > 0)
            {
                return 2;
            }

            var aPoint = aCount == 1
                ? aIntersection1
                : (segmentEnd.Distance(aIntersection1, true) > segmentEnd.Distance(aIntersection2, true)
                    ? aIntersection1
                    : aIntersection2);

            var bPoint = bCount == 1
                ? bIntersection1
                : (segmentEnd.Distance(bIntersection1, true) > segmentEnd.Distance(bIntersection2, true)
                    ? bIntersection1
                    : bIntersection2);

            return segmentEnd.Distance(aPoint, true) > segmentEnd.Distance(bPoint, true) ? 1 : 2;
        }

        [SuppressMessage("ReSharper", "ConvertIfStatementToReturnStatement")]
        public static Vector2 CenterPoint(this Vector2[] points)
        {
            if (points.Length == 0)
            {
                return Vector2.Zero;
            }

            return new Vector2(points.Sum(v => v.X) / points.Length, points.Sum(v => v.Y) / points.Length);
        }

        public static bool Close(float a, float b, float eps)
        {
            if (Math.Abs(eps) < float.Epsilon)
            {
                eps = (float) 1e-9;
            }
            return Math.Abs(a - b) <= eps;
        }

        public static Vector2 RotateAroundPoint(this Vector2 rotated, Vector2 around, float angle)
        {
            var sin = Math.Sin(angle);
            var cos = Math.Cos(angle);

            var x = ((rotated.X - around.X) * cos) - ((around.Y - rotated.Y) * sin) + around.X;
            var y = ((around.Y - rotated.Y) * cos) + ((rotated.X - around.X) * sin) + around.Y;

            return new Vector2((float) x, (float) y);
        }

        public static Polygon RotatePolygon(this Polygon polygon, Vector2 around, float angle)
        {
            var p = new Polygon();

            foreach (var polygonePoint in polygon.Points.Select(poinit => poinit.RotateAroundPoint(around, angle)))
            {
                p.Add(polygonePoint);
            }
            return p;
        }

        public static Polygon RotatePolygon(this Polygon polygon, Vector2 around, Vector2 direction)
        {
            var deltaX = around.X - direction.X;
            var deltaY = around.Y - direction.Y;
            var angle = (float) Math.Atan2(deltaY, deltaX);
            return RotatePolygon(polygon, around, angle - DegreeToRadian(90));
        }

        public static Polygon MovePolygon(this Polygon polygon, Vector2 moveTo)
        {
            var p = new Polygon();

            p.Add(moveTo);

            var count = polygon.Points.Count;

            var startPoint = polygon.Points[0];

            for (var i = 1; i < count; i++)
            {
                var polygonePoint = polygon.Points[i];
                p.Add(new Vector2(moveTo.X + (polygonePoint.X - startPoint.X), moveTo.Y + (polygonePoint.Y - startPoint.Y)));
            }
            return p;
        }

        public static Vector2 CenterOfPolygon(this Polygon p)
        {
            var cX = 0f;
            var cY = 0f;
            var pc = p.Points.Count;
            foreach (var point in p.Points)
            {
                cX += point.X;
                cY += point.Y;
            }
            return new Vector2(cX / pc, cY / pc);
        }

        public static List<Polygon> JoinPolygons(this List<Polygon> sList)
        {
            var p = ClipPolygons(sList);
            var tList = new List<List<IntPoint>>();

            var c = new Clipper();
            c.AddPaths(p, PolyType.ptClip, true);
            c.Execute(ClipType.ctUnion, tList, PolyFillType.pftNonZero, PolyFillType.pftNonZero);

            return ToPolygons(tList);
        }

        public static List<Polygon> JoinPolygons(
            this List<Polygon> sList,
            ClipType cType,
            PolyType pType = PolyType.ptClip,
            PolyFillType pFType1 = PolyFillType.pftNonZero,
            PolyFillType pFType2 = PolyFillType.pftNonZero)
        {
            var p = ClipPolygons(sList);
            var tList = new List<List<IntPoint>>();

            var c = new Clipper();
            c.AddPaths(p, pType, true);
            c.Execute(cType, tList, pFType1, pFType2);

            return ToPolygons(tList);
        }

        public static List<Polygon> ToPolygons(this IEnumerable<List<IntPoint>> v)
        {
            return v.Select(path => path.ToPolygon()).ToList();
        }

        public static Vector2 PositionAfter(this IEnumerable<Vector2> self, int t, int s, int delay = 0)
        {
            var self2 = self.ToList();
            var distance = Math.Max(0, t - delay) * s / 1000;
            for (var i = 0; i <= self2.Count - 2; i++)
            {
                var from = self2[i];
                var to = self2[i + 1];
                var d = (int) to.Distance(@from);
                if (d > distance)
                {
                    return @from + distance * (to - @from).Normalized();
                }
                distance -= d;
            }
            return self2[self2.Count - 1];
        }

        public static Polygon ToPolygon(this IEnumerable<IntPoint> v)
        {
            var polygon = new Polygon();
            foreach (var point in v)
            {
                polygon.Add(new Vector2(point.X, point.Y));
            }
            return polygon;
        }

        public static List<List<IntPoint>> ClipPolygons(IEnumerable<Polygon> polygons)
        {
            var polyg = polygons as Polygon[] ?? polygons.ToArray();
            var subj = new List<List<IntPoint>>(polyg.Length);
            var clip = new List<List<IntPoint>>(polyg.Length);
            foreach (var polygon in polyg)
            {
                subj.Add(polygon.ToClipperPath());
                clip.Add(polygon.ToClipperPath());
            }
            var solution = new List<List<IntPoint>>();
            var c = new Clipper();
            c.AddPaths(subj, PolyType.ptSubject, true);
            c.AddPaths(clip, PolyType.ptClip, true);
            c.Execute(ClipType.ctUnion, solution, PolyFillType.pftPositive, PolyFillType.pftEvenOdd);
            return solution;
        }

        public struct IntersectionResult
        {
            public bool Intersects;
            public Vector2 Point;

            public IntersectionResult(bool intersects = false, Vector2 point = new Vector2())
            {
                Intersects = intersects;
                Point = point;
            }
        }

        public struct ProjectionInfo
        {
            public bool IsOnSegment;
            public Vector2 LinePoint;
            public Vector2 SegmentPoint;

            public ProjectionInfo(bool isOnSegment, Vector2 segmentPoint, Vector2 linePoint)
            {
                IsOnSegment = isOnSegment;
                SegmentPoint = segmentPoint;
                LinePoint = linePoint;
            }
        }

        public class Polygon
        {
            public readonly List<Vector2> Points = new List<Vector2>();

            public void Add(Vector2 point)
            {
                Points.Add(point);
            }

            public void Add(Vector3 point)
            {
                Points.Add(point.To2D());
            }

            public void Add(Polygon polygon)
            {
                foreach (var point in polygon.Points)
                {
                    Points.Add(point);
                }
            }

            public List<IntPoint> ToClipperPath()
            {
                var result = new List<IntPoint>(Points.Count);
                result.AddRange(Points.Select(point => new IntPoint(point.X, point.Y)));
                return result;
            }

            public virtual void Draw(Color color, int width = 1)
            {
                for (var i = 0; i <= Points.Count - 1; i++)
                {
                    var nextIndex = (Points.Count - 1 == i) ? 0 : (i + 1);
                    var from = Drawing.WorldToScreen(Points[i].To3DWorld());
                    var to = Drawing.WorldToScreen(Points[nextIndex].To3DWorld());
                    Drawing.DrawLine(from[0], from[1], to[0], to[1], width, color);
                }
            }

            public bool IsInside(Vector2 point)
            {
                return !IsOutside(point);
            }

            public bool IsInside(Vector3 point)
            {
                return !IsOutside(point.To2D());
            }

            public bool IsInside(GameObject point)
            {
                return !IsOutside(point.Position.To2D());
            }

            public bool IsOutside(Vector2 point)
            {
                var p = new IntPoint(point.X, point.Y);
                return Clipper.PointInPolygon(p, ToClipperPath()) != 1;
            }

            public class Arc : Polygon
            {
                public float Angle;
                public Vector2 EndPos;
                public float Radius;
                public Vector2 StartPos;
                private readonly int _quality;

                public Arc(Vector3 start, Vector3 direction, float angle, float radius, int quality = 20)
                    : this(start.To2D(), direction.To2D(), angle, radius, quality)
                {
                }

                public Arc(Vector2 start, Vector2 direction, float angle, float radius, int quality = 20)
                {
                    StartPos = start;
                    EndPos = (direction - start).Normalized();
                    Angle = angle;
                    Radius = radius;
                    _quality = quality;
                    UpdatePolygon();
                }

                public void UpdatePolygon(int offset = 0)
                {
                    Points.Clear();
                    var outRadius = (Radius + offset) / (float) Math.Cos(2 * Math.PI / _quality);
                    var side1 = EndPos.Rotated(-Angle * 0.5f);
                    for (var i = 0; i <= _quality; i++)
                    {
                        var cDirection = side1.Rotated(i * Angle / _quality).Normalized();
                        Points.Add(
                            new Vector2(StartPos.X + outRadius * cDirection.X, StartPos.Y + outRadius * cDirection.Y));
                    }
                }
            }

            public class Line : Polygon
            {
                public Vector2 LineStart;
                public Vector2 LineEnd;

                public float Length
                {
                    get { return LineStart.Distance(LineEnd); }
                    set { LineEnd = (LineEnd - LineStart).Normalized() * value + LineStart; }
                }

                public Line(Vector3 start, Vector3 end, float length = -1)
                    : this(start.To2D(), end.To2D(), length)
                {
                }

                public Line(Vector2 start, Vector2 end, float length = -1)
                {
                    LineStart = start;
                    LineEnd = end;
                    if (length > 0)
                    {
                        Length = length;
                    }
                    UpdatePolygon();
                }

                public void UpdatePolygon()
                {
                    Points.Clear();
                    Points.Add(LineStart);
                    Points.Add(LineEnd);
                }
            }

            public class Circle : Polygon
            {
                public Vector2 Center;
                public float Radius;
                private readonly int _quality;

                public Circle(Vector3 center, float radius, int quality = 20)
                    : this(center.To2D(), radius, quality)
                {
                }

                public Circle(Vector2 center, float radius, int quality = 20)
                {
                    Center = center;
                    Radius = radius;
                    _quality = quality;
                    UpdatePolygon();
                }

                public void UpdatePolygon(int offset = 0, float overrideWidth = -1)
                {
                    Points.Clear();
                    var outRadius = (overrideWidth > 0
                        ? overrideWidth
                        : (offset + Radius) / (float) Math.Cos(2 * Math.PI / _quality));
                    for (var i = 1; i <= _quality; i++)
                    {
                        var angle = i * 2 * Math.PI / _quality;
                        var point = new Vector2(
                            Center.X + outRadius * (float) Math.Cos(angle), Center.Y + outRadius * (float) Math.Sin(angle));
                        Points.Add(point);
                    }
                }
            }

            public class Rectangle : Polygon
            {
                public Vector2 Direction
                {
                    get { return (End - Start).Normalized(); }
                }

                public Vector2 Perpendicular
                {
                    get { return Direction.Perpendicular(); }
                }

                public Vector2 End;
                public Vector2 Start;
                public float Width;

                public Rectangle(Vector3 start, Vector3 end, float width)
                    : this(start.To2D(), end.To2D(), width)
                {
                }

                public Rectangle(Vector2 start, Vector2 end, float width)
                {
                    Start = start;
                    End = end;
                    Width = width;
                    UpdatePolygon();
                }

                public void UpdatePolygon(int offset = 0, float overrideWidth = -1)
                {
                    Points.Clear();
                    Points.Add(
                        Start + (overrideWidth > 0 ? overrideWidth : Width + offset) * Perpendicular - offset * Direction);
                    Points.Add(
                        Start - (overrideWidth > 0 ? overrideWidth : Width + offset) * Perpendicular - offset * Direction);
                    Points.Add(
                        End - (overrideWidth > 0 ? overrideWidth : Width + offset) * Perpendicular + offset * Direction);
                    Points.Add(
                        End + (overrideWidth > 0 ? overrideWidth : Width + offset) * Perpendicular + offset * Direction);
                }
            }

            public class Ring : Polygon
            {
                public Vector2 Center;
                public float InnerRadius;
                public float OuterRadius;
                private readonly int _quality;

                public Ring(Vector3 center, float innerRadius, float outerRadius, int quality = 20)
                    : this(center.To2D(), innerRadius, outerRadius, quality)
                {
                }

                public Ring(Vector2 center, float innerRadius, float outerRadius, int quality = 20)
                {
                    Center = center;
                    InnerRadius = innerRadius;
                    OuterRadius = outerRadius;
                    _quality = quality;
                    UpdatePolygon();
                }

                public void UpdatePolygon(int offset = 0)
                {
                    Points.Clear();
                    var outRadius = (offset + InnerRadius + OuterRadius) / (float) Math.Cos(2 * Math.PI / _quality);
                    var innerRadius = InnerRadius - OuterRadius - offset;
                    for (var i = 0; i <= _quality; i++)
                    {
                        var angle = i * 2 * Math.PI / _quality;
                        var point = new Vector2(
                            Center.X - outRadius * (float) Math.Cos(angle), Center.Y - outRadius * (float) Math.Sin(angle));
                        Points.Add(point);
                    }
                    for (var i = 0; i <= _quality; i++)
                    {
                        var angle = i * 2 * Math.PI / _quality;
                        var point = new Vector2(
                            Center.X + innerRadius * (float) Math.Cos(angle),
                            Center.Y - innerRadius * (float) Math.Sin(angle));
                        Points.Add(point);
                    }
                }
            }

            public class Sector : Polygon
            {
                public float Angle;
                public Vector2 Center;
                public Vector2 Direction;
                public float Radius;
                private readonly int _quality;

                public Sector(Vector3 center, Vector3 direction, float angle, float radius, int quality = 20)
                    : this(center.To2D(), direction.To2D(), angle, radius, quality)
                {
                }

                public Sector(Vector2 center, Vector2 direction, float angle, float radius, int quality = 20)
                {
                    Center = center;
                    Direction = (direction - center).Normalized();
                    Angle = angle;
                    Radius = radius;
                    _quality = quality;
                    UpdatePolygon();
                }

                public void UpdatePolygon(int offset = 0)
                {
                    Points.Clear();
                    var outRadius = (Radius + offset) / (float) Math.Cos(2 * Math.PI / _quality);
                    Points.Add(Center);
                    var side1 = Direction.Rotated(-Angle * 0.5f);
                    for (var i = 0; i <= _quality; i++)
                    {
                        var cDirection = side1.Rotated(i * Angle / _quality).Normalized();
                        Points.Add(new Vector2(Center.X + outRadius * cDirection.X, Center.Y + outRadius * cDirection.Y));
                    }
                }
            }
        }
    }
}
