using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace lab1
{
    public class BezierPatch
    {
        public List<List<Point3D>> points { get; set; }
        public Control[,] ctrlPoints { get; set; }
        public List<LinesVisual3D> normals { get; set; }

        public int un;
        public int vn;
        public int m;
        public int n;

        public MeshGeometry3D mesh;
        public ModelVisual3D model;
        public GeometryModel3D geometry;
        public DiffuseMaterial material;
        public DiffuseMaterial back;

        public List<SphereVisual3D> pointsUI;
        public List<LinesVisual3D> edgesUI;
        public List<LinesVisual3D> connectorsUI;

        public BezierPatch(int n, int m, int un, int vn)
        {
            this.n = n;
            this.m = m;
            this.un = un;
            this.vn = vn;
        }

        #region BUILD
        public void Build()
        {
            mesh = new MeshGeometry3D();
            Calc4();
            //Texture();
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
            int idx = 0;

            var u = Misc.LinSpace(0, 1, un);
            var v = Misc.LinSpace(0, 1, vn);

            foreach (var du in u)
            {
                points.Add(new List<Point3D>());
                foreach (var dv in v)
                {

                    List<Point3D> r = new List<Point3D>();
                    for (int i = 0; i < n; i++)
                    {
                        Point3D p = new Point3D(Bezier3(ctrlPoints[i, 0].X, ctrlPoints[i, 1].X, ctrlPoints[i, 2].X, ctrlPoints[i, 3].X, dv),
                                                Bezier3(ctrlPoints[i, 0].Y, ctrlPoints[i, 1].Y, ctrlPoints[i, 2].Y, ctrlPoints[i, 3].Y, dv),
                                                Bezier3(ctrlPoints[i, 0].Z, ctrlPoints[i, 1].Z, ctrlPoints[i, 2].Z, ctrlPoints[i, 3].Z, dv));
                        r.Add(p);
                    }

                    double x = Bezier2(r[0].X, r[1].X, r[2].X, du);
                    double y = Bezier2(r[0].Y, r[1].Y, r[2].Y, du);
                    double z = Bezier2(r[0].Z, r[1].Z, r[2].Z, du);

                    points[idx].Add(new Point3D(x, y, z));
                    mesh.Positions.Add(new Point3D(x, y, z));
                }
                idx++;
            }
        }

        public void Calc4()
        {
            points = new List<List<Point3D>>();
            int idx = 0;

            var u = Misc.LinSpace(0, 1, un);
            var v = Misc.LinSpace(0, 1, vn);

            foreach (var du in u)
            {
                points.Add(new List<Point3D>());
                foreach (var dv in v)
                {
                    List<Point3D> r = new List<Point3D>();
                    for (int i = 0; i < n; i++)
                    {
                        Point3D p = new Point3D(Bezier4(ctrlPoints[i, 0].X, ctrlPoints[i, 1].X, ctrlPoints[i, 2].X, ctrlPoints[i, 3].X, ctrlPoints[i, 4].X, dv),
                                                Bezier4(ctrlPoints[i, 0].Y, ctrlPoints[i, 1].Y, ctrlPoints[i, 2].Y, ctrlPoints[i, 3].Y, ctrlPoints[i, 4].Y, dv),
                                                Bezier4(ctrlPoints[i, 0].Z, ctrlPoints[i, 1].Z, ctrlPoints[i, 2].Z, ctrlPoints[i, 3].Z, ctrlPoints[i, 4].Z, dv));
                        r.Add(p);
                    }

                    double x = Bezier2(r[0].X, r[1].X, r[2].X, du);
                    double y = Bezier2(r[0].Y, r[1].Y, r[2].Y, du);
                    double z = Bezier2(r[0].Z, r[1].Z, r[2].Z, du);

                    points[idx].Add(new Point3D(x, y, z));
                    mesh.Positions.Add(new Point3D(x, y, z));
                }
                idx++;
            }
        }

        public static double Bezier2(double p0, double p1, double p2, double t)
        {
            return (Math.Pow(1 - t, 2) * p0)
                + (2 * t * (1 - t) * p1)
                + (t * t * p2);
        }

        private double Bezier3(double r0, double r1, double r2, double r3, double t)
        {
            return r0 * Math.Pow(1 - t, 3)
                    + 3 * r1 * t * Math.Pow(1 - t, 2)
                    + 3 * r2 * Math.Pow(t, 2) * (1 - t)
                    + r3 * Math.Pow(t, 3);
        }

        private double Bezier4(double r0, double r1, double r2, double r3, double r4, double t)
        {
            return r0 * Math.Pow(1 - t, 4)
                     + 4 * r1 * t * Math.Pow(1 - t, 3)
                     + 6 * r2 * Math.Pow(t, 2) * Math.Pow(1 - t, 2)
                     + 4 * r3 * Math.Pow(t, 3) * (1 - t)
                     + r4 * Math.Pow(t, 4);
        }

        public double Bernstein(int n, int i, double t)
        {
            return Misc.Factorial(n)
                / (Misc.Factorial(i)
                    * Misc.Factorial(n - i))
                    * Math.Pow(t, i)
                    * Math.Pow(1 - t, n - i);
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
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    ctrlPoints[i, j].Draw();
                }
            }
            DrawConnectors();
        }

        public void DrawConnectors()
        {
            connectorsUI = new List<LinesVisual3D>();
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < m - 1; j++)
                {
                    var line1 = Figures.Line(ctrlPoints[i, j].center, ctrlPoints[i, j + 1].center, Color.FromRgb(0, 0, 0), 1);
                    var line2 = Figures.Line(ctrlPoints[i, j].center, ctrlPoints[i + 1, j].center, Color.FromRgb(0, 0, 0), 1);
                    var line3 = Figures.Line(ctrlPoints[i + 1, j + 1].center, ctrlPoints[i + 1, j].center, Color.FromRgb(0, 0, 0), 1);
                    var line4 = Figures.Line(ctrlPoints[i + 1, j + 1].center, ctrlPoints[i, j + 1].center, Color.FromRgb(0, 0, 0), 1);

                    connectorsUI.Add(line1);
                    connectorsUI.Add(line2);
                    connectorsUI.Add(line3);
                    connectorsUI.Add(line4);

                    Drawer.Draw(line1);
                    Drawer.Draw(line2);
                    Drawer.Draw(line3);
                    Drawer.Draw(line4);
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
        private void Fek()
        {
            //Cal();
        }
        #endregion

        #region TEXTURING
        public Point3D CalcTexture(double u, double v)
        {
            List<Point3D> r = new List<Point3D>();
            for (int j = 0; j < n; j++)
            {
                Point3D p = new Point3D(Bezier3(ctrlPoints[j, 0].X, ctrlPoints[j, 1].X, ctrlPoints[j, 2].X, ctrlPoints[j, 3].X, v),
                                        Bezier3(ctrlPoints[j, 0].Y, ctrlPoints[j, 1].Y, ctrlPoints[j, 2].Y, ctrlPoints[j, 3].Y, v),
                                        Bezier3(ctrlPoints[j, 0].Z, ctrlPoints[j, 1].Z, ctrlPoints[j, 2].Z, ctrlPoints[j, 3].Z, v));
                r.Add(p);
            }

            double x = Bezier2(r[0].X, r[1].X, r[2].X, u);
            double y = Bezier2(r[0].Y, r[1].Y, r[2].Y, u);
            double z = Bezier2(r[0].Z, r[1].Z, r[2].Z, u);

            return new Point3D(x, y, z);
        }
        public int CalcTexture(Astroid texture, double cu, double cv, int start)
        {
            List<int> idxs = new List<int>();

            for (int i = 0; i < texture.points.Count; i++)
            {
                double u = cu + 0.1 * texture.points[i].X / 10;
                double v = cv + 0.1 * texture.points[i].Y / 10;

                if (u > 1 || v > 1)
                {
                    return i;
                }

                List<Point3D> r = new List<Point3D>();
                for (int j = 0; j < n; j++)
                {
                    Point3D p = new Point3D(Bezier3(ctrlPoints[j, 0].X, ctrlPoints[j, 1].X, ctrlPoints[j, 2].X, ctrlPoints[j, 3].X, v),
                                            Bezier3(ctrlPoints[j, 0].Y, ctrlPoints[j, 1].Y, ctrlPoints[j, 2].Y, ctrlPoints[j, 3].Y, v),
                                            Bezier3(ctrlPoints[j, 0].Z, ctrlPoints[j, 1].Z, ctrlPoints[j, 2].Z, ctrlPoints[j, 3].Z, v));
                    r.Add(p);
                }

                double x = Bezier2(r[0].X, r[1].X, r[2].X, u);
                double y = Bezier2(r[0].Y, r[1].Y, r[2].Y, u);
                double z = Bezier2(r[0].Z, r[1].Z, r[2].Z, u);

                texture.points3D.Add(new Point3D(x, y, z));
            }

            return -1;
        }
        #endregion

        #region MATERIAL
        public void SetMaterial(MaterialGroup mg)
        {
            geometry = new GeometryModel3D
            {
                Geometry = mesh,
                Material = mg,
                BackMaterial = mg
            };
            model = new ModelVisual3D();
            model.Content = geometry;
        }

        public void EraseModel()
        {
            Drawer.Erase(model);
        }
        #endregion
    }
}
