using HelixToolkit.Wpf;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace lab1
{
    public static class Figures
    {
        public static LinesVisual3D Line(Point3D start, Point3D end, Color color, double thickness = 2)
        {
            LinesVisual3D line = new LinesVisual3D();

            line.Color = color;
            line.Thickness = thickness;
            line.Points.Add(start);
            line.Points.Add(end);

            return line;
        }

        public static TruncatedConeVisual3D Cone(double based, Brush filler, Point3D origin, double height, Vector3D orientation)
        {
            return Cylinder(based, 0, filler, origin, height, orientation);
        }

        public static TruncatedConeVisual3D Cylinder(double based, double top, Brush filler, Point3D origin, double height, Vector3D orientation)
        {
            TruncatedConeVisual3D cyl = new TruncatedConeVisual3D();

            cyl.BaseRadius = based;
            cyl.TopRadius = top;
            cyl.Fill = filler;
            cyl.Origin = origin;
            cyl.Height = height;
            cyl.Normal = orientation;

            return cyl;
        }

        public static SphereVisual3D Sphere(Point3D center, double radius, Brush filler)
        {
            var sphere = new SphereVisual3D();

            sphere.Center = center;
            sphere.Fill = filler;
            sphere.Radius = radius;

            return sphere;
        }

        public static RectangleVisual3D Rectangle(Point3D origin, double length, double width, Vector3D normal, Brush filler)
        {
            var rect = new RectangleVisual3D();

            rect.Origin = origin;
            rect.Length = length;
            rect.Width = width;
            rect.Normal = normal;
            var brush = new SolidColorBrush()
            {
                Color = Colors.Blue,
                Opacity = 0.5
            };
            rect.Fill = brush;

            return rect;
        }
    }
}
