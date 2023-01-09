using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using static lab1.MainWindow;

namespace lab1
{
    public class BezierSegment
    {
        public List<List<Point3D>> points { get; set; }
        public List<List<ControlPoint>> ctrlPoints { get; set; }
        public List<LinesVisual3D> normals { get; set; }

        public MeshGeometry3D mesh;
        public ModelVisual3D model;
        public GeometryModel3D geometry;
        public DiffuseMaterial material;
        public DiffuseMaterial back;
        public Brush brush;

        IEnumerable<double> u;
        IEnumerable<double> v;
        double[] r;
        double[] c;

        public List<SphereVisual3D> pointsUI;
        public List<LinesVisual3D> edgesUI;
        public List<LinesVisual3D> connectorsUI;

        MoveDelegate move;

        public BezierSegment(int count, double[] r, double[] c, MoveDelegate move)
        {
            u = Misc.LinSpace(0, 1, count);
            v = Misc.LinSpace(0, 2 * Math.PI, count);
            this.r = r;
            this.c = c;
            this.move = move;
        }

        #region BUILD
        public void Build()
        {
            mesh = new MeshGeometry3D();
            Calc();
            Texture();
            Mesh();
            geometry = new GeometryModel3D
            {
                Geometry = mesh,
                Material = material,
                BackMaterial = back
            };
            model = new ModelVisual3D();
            model.Content = geometry;
        }

        public void Build(MaterialGroup mg)
        {
            mesh = new MeshGeometry3D();
            Calc();
            Mesh();
            geometry = new GeometryModel3D
            {
                Geometry = mesh,
                Material = mg,
                BackMaterial = mg
            };
            model = new ModelVisual3D();
            model.Content = geometry;
        }

        public void Mesh()
        {
            mesh.TriangleIndices.Clear();
            for (int i = 0; i < points.Count - 1; i++)
            {
                int n = points[i].Count;

                for (int j = 0; j < n - 1; j++)
                {
                    mesh.TriangleIndices.Add(Index(i, j, n));
                    mesh.TriangleIndices.Add(Index(i, j + 1, n));
                    mesh.TriangleIndices.Add(Index(i + 1, j, n));

                    mesh.TriangleIndices.Add(Index(i, j + 1, n));
                    mesh.TriangleIndices.Add(Index(i + 1, j + 1, n));
                    mesh.TriangleIndices.Add(Index(i + 1, j, n));
                }
            }
        }

        private void Texture()
        {
            material = new DiffuseMaterial(Brushes.LightSteelBlue);
            back = new DiffuseMaterial(Brushes.LightSteelBlue);
        }
        #endregion

        #region CALCULATION
        public void Calc()
        {
            points = new List<List<Point3D>>();
            ctrlPoints = new List<List<ControlPoint>>();
            int idx = 0;

            foreach (var j in v)
            {
                points.Add(new List<Point3D>());
                ctrlPoints.Add(new List<ControlPoint>());

                List<Point3D> p = new List<Point3D>();
                for (int k = 0; k < r.Length; k++)
                {
                    var cp = Circle(r[k], j, c[k]);
                    p.Add(cp);
                    ctrlPoints[idx].Add(new ControlPoint(cp, idx, k, this, move));
                }

                foreach (var i in u)
                {
                    double x = Bezier3(p[0].X, p[1].X, p[2].X, p[3].X, i);
                    double y = Bezier3(p[0].Y, p[1].Y, p[2].Y, p[3].Y, i);
                    double z = Bezier3(p[0].Z, p[1].Z, p[2].Z, p[3].Z, i);

                    points[idx].Add(new Point3D(x, y, z));
                    mesh.Positions.Add(new Point3D(x, y, z));
                }
                idx++;
            }
        }
        public void Rebuild(ControlPoint point)
        {
            try
            {
                Recalc(point);
                Mesh();
                geometry = new GeometryModel3D
                {
                    Geometry = mesh,
                    Material = material,
                    BackMaterial = back
                };
                model = new ModelVisual3D();
                model.Content = geometry;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        public void Recalc(ControlPoint point)
        {
            try
            {
                int idx = point.i;
                int n = points[0].Count;
                int j = 0;

                points[idx] = new List<Point3D>();
                foreach (var i in u)
                {
                    double x = Bezier3(ctrlPoints[idx][0].X, ctrlPoints[idx][1].X, ctrlPoints[idx][2].X, ctrlPoints[idx][3].X, i);
                    double y = Bezier3(ctrlPoints[idx][0].Y, ctrlPoints[idx][1].Y, ctrlPoints[idx][2].Y, ctrlPoints[idx][3].Y, i);
                    double z = Bezier3(ctrlPoints[idx][0].Z, ctrlPoints[idx][1].Z, ctrlPoints[idx][2].Z, ctrlPoints[idx][3].Z, i);

                    points[idx].Add(new Point3D(x, y, z));
                    mesh.Positions[Index(idx, j, n)] = new Point3D(x, y, z);
                    j++;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        public Point3D Circle(double r, double t, double y)
        {
            return new Point3D(r * Math.Cos(t), y, r * Math.Sin(t));
        }

        private double Bezier3(double r0, double r1, double r2, double r3, double t)
        {
            return r0 * Math.Pow(1 - t, 3)
                    + 3 * r1 * t * Math.Pow(1 - t, 2)
                    + 3 * r2 * Math.Pow(t, 2) * (1 - t)
                    + r3 * Math.Pow(t, 3);
        }
        #endregion

        #region DRAWING
        public void DrawModel()
        {
            try
            {
                Drawer.Draw(model);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        public void DrawPoints()
        {
            pointsUI = new List<SphereVisual3D>();
            foreach (var pts in points)
            {
                foreach (var p in pts)
                {
                    var point = Figures.Sphere(p, 0.01, Brushes.Red);
                    pointsUI.Add(point);
                    Drawer.Draw(point);
                }
            }
        }

        public void BuildNormals()
        {
            normals = new List<LinesVisual3D>();
            for (int i = 0; i < points.Count - 1; i++)
            {
                for (int j = 0; j < points[i].Count - 1; j++)
                {
                    var norm1 = FindTriangleNormal(points[i][j], points[i + 1][j], points[i][j + 1], 0.5, 1);
                    var norm2 = FindTriangleNormal(points[i + 1][j + 1], points[i + 1][j], points[i][j + 1], 0.5, 1);

                    normals.Add(norm1);
                    normals.Add(norm2);
                }
            }
        }

        public void DrawEdges()
        {
            edgesUI = new List<LinesVisual3D>();
            normals = new List<LinesVisual3D>();
            for (int i = 0; i < points.Count - 1; i++)
            {
                for (int j = 0; j < points[i].Count - 1; j++)
                {
                    var line1 = Figures.Line(points[i][j], points[i][j + 1], Color.FromRgb(0, 0, 128));
                    var line2 = Figures.Line(points[i][j], points[i + 1][j], Color.FromRgb(0, 0, 128));
                    var line3 = Figures.Line(points[i + 1][j], points[i + 1][j + 1], Color.FromRgb(0, 0, 128));
                    var line4 = Figures.Line(points[i][j + 1], points[i + 1][j + 1], Color.FromRgb(0, 0, 128));
                    var line5 = Figures.Line(points[i + 1][j], points[i][j + 1], Color.FromRgb(0, 0, 128));

                    Drawer.Draw(line1);
                    Drawer.Draw(line2);
                    Drawer.Draw(line3);
                    Drawer.Draw(line4);
                    Drawer.Draw(line5);

                    edgesUI.Add(line1);
                    edgesUI.Add(line2);
                    edgesUI.Add(line3);
                    edgesUI.Add(line4);
                    edgesUI.Add(line5);

                    var norm1 = FindTriangleNormal(points[i][j], points[i + 1][j], points[i][j + 1], 0.5, 1);
                    var norm2 = FindTriangleNormal(points[i + 1][j + 1], points[i + 1][j], points[i][j + 1], 0.5, 1);

                    normals.Add(norm1);
                    normals.Add(norm2);
                }
            }
        }

        public void DrawNormals()
        {
            if (normals == null)
            {
                BuildNormals();
            }
            foreach (var n in normals)
            {
                Drawer.Draw(n);
            }
        }

        public void DrawControls()
        {
            connectorsUI = new List<LinesVisual3D>();
            for (int i = 0; i < ctrlPoints.Count - 1; i++)
            {
                for (int j = 0; j < ctrlPoints[i].Count - 1; j++)
                {
                    ctrlPoints[i][j].Draw();
                    var line1 = Figures.Line(ctrlPoints[i][j].center, ctrlPoints[i][j + 1].center, Color.FromRgb(0, 0, 0), 1);
                    var line2 = Figures.Line(ctrlPoints[i][j].center, ctrlPoints[i + 1][j].center, Color.FromRgb(0, 0, 0), 1);

                    connectorsUI.Add(line1);
                    connectorsUI.Add(line2);

                    Drawer.Draw(line1);
                    Drawer.Draw(line2);
                }
                ctrlPoints[i][ctrlPoints[i].Count - 1].Draw();
            }
        }

        public void DrawConnectors()
        {
            connectorsUI = new List<LinesVisual3D>();
            for (int i = 0; i < ctrlPoints.Count - 1; i++)
            {
                for (int j = 0; j < ctrlPoints[i].Count - 1; j++)
                {
                    var line1 = Figures.Line(ctrlPoints[i][j].center, ctrlPoints[i][j + 1].center, Color.FromRgb(0, 0, 0), 1);
                    var line2 = Figures.Line(ctrlPoints[i][j].center, ctrlPoints[i + 1][j].center, Color.FromRgb(0, 0, 0), 1);

                    connectorsUI.Add(line1);
                    connectorsUI.Add(line2);

                    Drawer.Draw(line1);
                    Drawer.Draw(line2);
                }
            }
        }

        public void Clear()
        {
            if (Drawer.grid.Children.Contains(model))
            {
                Drawer.Erase(model);
            }

            if (edgesUI != null)
            {
                foreach (var e in edgesUI)
                {
                    if (Drawer.grid.Children.Contains(e))
                    {
                        Drawer.Erase(e);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (pointsUI != null)
            {
                foreach (var p in pointsUI)
                {
                    if (Drawer.grid.Children.Contains(p))
                    {
                        Drawer.Erase(p);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (normals != null)
            {
                foreach (var n in normals)
                {
                    if (Drawer.grid.Children.Contains(n))
                    {
                        Drawer.Erase(n);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (connectorsUI != null)
            {
                foreach (var c in connectorsUI)
                {
                    if (Drawer.grid.Children.Contains(c))
                    {
                        Drawer.Erase(c);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
        #endregion

        #region NORMALS
        public LinesVisual3D FindTriangleNormal(Point3D point1, Point3D point2, Point3D point3, double length, double thickness)
        {
            Vector3D n = FindTriangleVector(point1, point2, point3);
            n = ScaleVector(n, length);
            Point3D start = new Point3D((point1.X + point2.X + point3.X) / 3.0,
                                            (point1.Y + point2.Y + point3.Y) / 3.0,
                                            (point1.Z + point2.Z + point3.Z) / 3.0);

            // Find the segment's other end point.
            Point3D end = start + n;

            var line = Figures.Line(start, end, Colors.Green, thickness);

            return line;
        }

        public Vector3D FindTriangleVector(Point3D point1, Point3D point2, Point3D point3)
        {
            // Get two edge vectors.
            Vector3D v1 = point2 - point1;
            Vector3D v2 = point3 - point2;

            // Get the cross product.
            Vector3D n = Vector3D.CrossProduct(v1, v2);

            // Normalize.
            n.Normalize();

            return n;
        }

        public Vector3D ScaleVector(Vector3D vector, double length)
        {
            double scale = length / vector.Length;
            return new Vector3D(
                vector.X * scale,
                vector.Y * scale,
                vector.Z * scale);
        }

        #endregion

        #region MISC
        private int Index(int i, int j, int n) => j + (i * n);
        #endregion
    }
}
