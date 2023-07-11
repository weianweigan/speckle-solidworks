using Objects.Geometry;
using SolidWorks.Interop.sldworks;
using System;

namespace Objects.Converter.SolidWorks.Converters;

public static class SwSketchToCurveConverter
{
    public static Point SketchPointToSpecklePoint(ISketchPoint sketchPoint)
    {
        return new Point(sketchPoint.X, sketchPoint.Y, sketchPoint.Z);
    }

    public static Line SketchLineToSpeckleLine(ISketchLine sketchLine)
    {
        return new Line(
            SketchPointToSpecklePoint((ISketchPoint)sketchLine.IGetStartPoint2()), 
            SketchPointToSpecklePoint((ISketchPoint)sketchLine.GetEndPoint2()));
    }

    public static ICurve SketchCircleToSpeckleCircle(ISketchArc sketchArc, Plane plane)
    {
        var arcPlane = new Plane(
            SketchPointToSpecklePoint((ISketchPoint)sketchArc.GetCenterPoint2()),
            plane.normal, 
            plane.xdir, 
            plane.ydir);
        if(sketchArc.IsCircle() == 1)
        {
            return new Circle(arcPlane, sketchArc.GetRadius());
        }
        else
        {
            return new Arc(
                arcPlane,
                SketchPointToSpecklePoint((ISketchPoint)sketchArc.GetStartPoint2()),
                SketchPointToSpecklePoint((ISketchPoint)sketchArc.GetEndPoint2()),
                sketchArc.GetRadius());
        }
    }

    public static Ellipse SketchEllipseToSpeckleEllipse(ISketchEllipse sketchEllipse, Plane plane)
    {
        var ellipsePlane = new Plane(
            SketchPointToSpecklePoint((ISketchPoint)sketchEllipse.GetCenterPoint2()),
            plane.normal,
            plane.xdir,
            plane.ydir);

        var point1 = SketchPointToSpecklePoint((ISketchPoint)sketchEllipse.GetMajorPoint2());
        var point2 = SketchPointToSpecklePoint((ISketchPoint)sketchEllipse.GetMinorPoint2());

        // TODO: if the ellipse had rotated or cut by other sketch segment.
        return new Ellipse(ellipsePlane, Math.Abs(point1.x - point2.x), Math.Abs(point1.y - point2.y));
    }
}
