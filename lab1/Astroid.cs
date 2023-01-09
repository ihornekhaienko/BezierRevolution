using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace lab1
{
    public class Astroid
    {
        public List<Point> points;
        public List<Point3D> points3D;
        public List<LinesVisual3D> curve;
        public List<SphereVisual3D> pointsUI;
        public double R;
        public int count;

        public Astroid(double r, int count)
        {
            R = r;
            this.count = count;
        }

        public void Build()
        {
            points = new List<Point>();

            var phi = Misc.LinSpace(0, 2 * Math.PI, count);

            foreach (var t in phi)
            {
                double x = R * Math.Pow(Math.Cos(t), 3);
                double y = R * Math.Pow(Math.Sin(t), 3);

                points.Add(new Point(x, y));
            }
        }

        public void DrawPoints(double radius = 0.01)
        {
            pointsUI = new List<SphereVisual3D>();
            foreach (var p in points3D)
            {
                var point = Figures.Sphere(p, radius, Brushes.Black);
                Drawer.Draw(point);
                pointsUI.Add(point);
            }
        }

        public void DrawCurve()
        {
            curve = new List<LinesVisual3D>();
            for (int i = 0; i < points3D.Count - 1; i++)
            {
                var line = Figures.Line(points3D[i], points3D[i + 1], Colors.Crimson);
                curve.Add(line);
                Drawer.Draw(line);
            }
        }
    }
}
