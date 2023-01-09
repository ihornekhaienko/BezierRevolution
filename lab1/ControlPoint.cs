using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using static lab1.MainWindow;

namespace lab1
{
    public class ControlPoint
    {
        public MeshGeometry3D mesh;
        public ModelUIElement3D model;
        public GeometryModel3D geometry;
        public DiffuseMaterial material;
        public DiffuseMaterial back;
        public Brush brush = Brushes.Red;

        public double radius;

        public Point3D center;
        public double X => center.X;
        public double Y => center.Y;
        public double Z => center.Z;

        private int num_phi = 15;
        private int num_theta = 15;

        public int i;
        public int j;
        BezierSegment bezier;
        public ControlPoint clone;

        public event MoveDelegate OnMove;

        #region INIT
        public ControlPoint(Point3D center, int i, int j, BezierSegment bezier, MoveDelegate move, double radius = 0.05)
        {
            this.center = center;
            this.radius = radius;
            this.i = i;
            this.j = j;
            this.bezier = bezier;
            this.OnMove += move;

            Build();
        }
        #endregion

        #region BUILD
        public void Build()
        {
            Mesh();
            Texture();
            geometry = new GeometryModel3D(mesh, material);
            geometry.BackMaterial = back;
            model = new ModelUIElement3D();
            model.Model = geometry;
            SetupEvents();
        }

        private void Texture()
        {
            material = new DiffuseMaterial(brush);
            back = new DiffuseMaterial(brush);
        }

        private void Mesh()
        {
            mesh = new MeshGeometry3D();

            double dphi = Math.PI / num_phi;
            double dtheta = 2 * Math.PI / num_theta;

            // Remember the first point.
            int pt0 = mesh.Positions.Count;

            // Make the points.
            double phi1 = Math.PI / 2;
            for (int p = 0; p <= num_phi; p++)
            {
                double r1 = radius * Math.Cos(phi1);
                double y1 = radius * Math.Sin(phi1);

                double theta = 0;
                for (int t = 0; t <= num_theta; t++)
                {
                    mesh.Positions.Add(new Point3D(
                        X + r1 * Math.Cos(theta),
                        Y + y1,
                        Z + -r1 * Math.Sin(theta)));
                    theta += dtheta;
                }
                phi1 -= dphi;
            }

            // Make the triangles.
            int i1, i2, i3, i4;
            for (int p = 0; p <= num_phi - 1; p++)
            {
                i1 = p * (num_theta + 1);
                i2 = i1 + num_theta + 1;
                for (int t = 0; t <= num_theta - 1; t++)
                {
                    i3 = i1 + 1;
                    i4 = i2 + 1;
                    mesh.TriangleIndices.Add(pt0 + i1);
                    mesh.TriangleIndices.Add(pt0 + i2);
                    mesh.TriangleIndices.Add(pt0 + i4);

                    mesh.TriangleIndices.Add(pt0 + i1);
                    mesh.TriangleIndices.Add(pt0 + i4);
                    mesh.TriangleIndices.Add(pt0 + i3);
                    i1 += 1;
                    i2 += 1;
                }
            }
        }
        #endregion

        #region METHODS
        public void Draw()
        {
            Drawer.Draw(model);
        }

        public void Move(double x, double y, double z, bool showEdges, bool showNormals, bool showModel, bool moveClone = true)
        {
            var transforms = (Transform3DGroup)model.Transform;
            var transform = new TranslateTransform3D(x - X, y - Y, z - Z);
            transforms.Children.Add(transform);
            center = new Point3D(x, y, z);
            model.Transform = transforms;

            bezier.Clear();
            bezier.Rebuild(this);

            bezier.DrawConnectors();
            if (showEdges)
                bezier.DrawEdges();
            if (showNormals)
                bezier.DrawNormals();
            if (showModel)
                bezier.DrawModel();

            if (moveClone == true)
            {
                if (clone != null)
                {
                    clone.Move(x, y, z, showEdges, showNormals, showModel, false);
                }
            }
        }
        #endregion

        #region EVENTS
        private void SetupEvents()
        {
            model.MouseLeftButtonDown += OnMouseLeftButtonDown;
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OnMove(this);
        }
        #endregion
    }
}
