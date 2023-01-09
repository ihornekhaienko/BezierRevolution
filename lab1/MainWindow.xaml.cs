using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace lab1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BezierRevolution revolution;
        BezierSurface surface;
        public MainWindow()
        {
            InitializeComponent();
        }

        #region INIT
        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            Drawer.grid = grid;
        }
        #endregion

        #region REVOLUTION BUILDING
        private void buildButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(countTB.Text))
                {
                    return;
                }

                int count = Convert.ToInt32(countTB.Text);
                double[] r = { 2, 1, 1, 2 };

                if (revolution != null)
                    Drawer.Clear();

                revolution = new BezierRevolution();

                if ((bool)materialCB.IsChecked)
                {
                    var diffuse = new DiffuseMaterial(Brushes.DimGray);
                    var specular = new SpecularMaterial(Brushes.DimGray, 24);
                    var mg = new MaterialGroup();
                    mg.Children.Add(diffuse);
                    mg.Children.Add(specular);
                    revolution.Build(count, MoveControlCallback, mg);
                }
                else
                {
                    revolution.Build(count, MoveControlCallback);
                }

                if ((bool)showEdgesCB.IsChecked)
                {
                    revolution.DrawEdges();
                }

                if ((bool)showNormalsCB.IsChecked)
                {
                    revolution.DrawNormals();
                }

                if ((bool)showControlsCB.IsChecked)
                {
                    revolution.DrawControls();
                }

                if ((bool)showModelCB.IsChecked)
                {
                    revolution.DrawModels();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }
        #endregion

        #region PATCH BUILDING
        private void buildPatchButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(duTB.Text) || string.IsNullOrWhiteSpace(dvTB.Text))
                {
                    return;
                }

                int un = Convert.ToInt32(duTB.Text);
                int vn = Convert.ToInt32(dvTB.Text);

                Drawer.Clear();

                surface = new BezierSurface();

                surface.Build(3, 5, un, vn, MoveControlCB);

                if ((bool)showEdgesPCB.IsChecked)
                {
                    surface.DrawEdges();
                }

                if ((bool)showNormalsPCB.IsChecked)
                {
                    surface.DrawNormals();
                }

                if ((bool)showControlsPCB.IsChecked)
                {
                    surface.DrawControls();
                }

                if ((bool)showModelPCB.IsChecked)
                {
                    surface.DrawModels();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }
        #endregion

        #region OFFSET
        private void offsetButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dxTB.Text) || string.IsNullOrWhiteSpace(dyTB.Text) || string.IsNullOrWhiteSpace(dzTB.Text))
                {
                    return;
                }

                double dx = Convert.ToDouble(dxTB.Text) / 10;
                double dy = Convert.ToDouble(dyTB.Text) / 10;
                double dz = Convert.ToDouble(dzTB.Text) / 10;

                OffsetAnime(dx, dy, dz);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void OffsetAnime(double dx, double dy, double dz)
        {
            double i = 0;
            double j = 0;
            double k = 0;
            bool flag = true;

            while (flag)
            {
                flag = false;

                if (Math.Abs(i) < Math.Abs(dx))
                {
                    flag = true;
                    if (dx > 0)
                        i += 0.1;
                    else
                        i -= 0.1;
                }

                if (Math.Abs(j) < Math.Abs(dy))
                {
                    flag = true;
                    if (dy > 0)
                        j += 0.1;
                    else
                        j -= 0.1;
                }

                if (Math.Abs(k) < Math.Abs(dz))
                {
                    flag = true;
                    if (dz > 0)
                        k += 0.1;
                    else
                        k -= 0.1;
                }

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() => Offset(i, j, k))).Wait();
            }
        }

        private void Offset(double dx, double dy, double dz)
        {
            foreach (var obj in grid.Children)
            {
                if (obj.Transform is Transform3DGroup)
                {
                    var transforms = (Transform3DGroup)obj.Transform;
                    transforms.Children.Add(new TranslateTransform3D(dx, dy, dz));
                    obj.Transform = transforms;
                }
            }
        }
        #endregion

        #region ROTATION
        private void rotateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(angleTB.Text))
                {
                    return;
                }

                double angle = Convert.ToDouble(angleTB.Text);

                Vector3D axis;
                var selected = ((TextBlock)((ComboBoxItem)axisCB.SelectedItem).Content).Text;
                if (selected == "x")
                {
                    axis = new Vector3D(1, 0, 0);
                }
                else if (selected == "y")
                {
                    axis = new Vector3D(0, 1, 0);
                }
                else
                {
                    axis = new Vector3D(0, 0, 1);
                }

                RotateAnime(axis, angle);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        public void RotateAnime(Vector3D axis, double angle)
        {
            double sign = Math.Abs(angle) / 10;
            if (angle < 0)
            {
                sign *= -1;
                angle *= -1;
            }

            for (double i = 0; i < angle; i += Math.Abs(sign))
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() => Rotate(axis, sign))).Wait();
            }
        }

        private void Rotate(Vector3D axis, double angle)
        {
            foreach (var obj in grid.Children)
            {
                if (obj.Transform is Transform3DGroup)
                {
                    var transforms = (Transform3DGroup)obj.Transform;
                    transforms.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(axis, angle)));
                    obj.Transform = transforms;
                }
            }
        }

        #endregion

        #region CONTROL POINTS MANAGEMENT
        public delegate void MoveDelegate(ControlPoint point);
        public delegate void MoveDel(Control point);
        ControlPoint selected;
        Control sel;

        public void MoveControlCallback(ControlPoint control)
        {
            selected = control;
            cpxTB.Text = selected.X.ToString();
            cpyTB.Text = selected.Y.ToString();
            cpzTB.Text = selected.Z.ToString();
        }

        private void movePoint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((selected == null && sel == null) || string.IsNullOrWhiteSpace(cpxTB.Text)
                    || string.IsNullOrWhiteSpace(cpyTB.Text) || string.IsNullOrWhiteSpace(cpzTB.Text))
                {
                    return;
                }

                if (sel == null)
                {
                    MoveCP();
                }
                else
                {
                    MoveC();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void MoveC()
        {
            double x = Convert.ToDouble(cpxTB.Text);
            double y = Convert.ToDouble(cpyTB.Text);
            double z = Convert.ToDouble(cpzTB.Text);

            bool showEdges = (bool)showEdgesPCB.IsChecked;
            bool showNormals = (bool)showNormalsPCB.IsChecked;
            bool showModel = (bool)showModelPCB.IsChecked;

            sel.Move(x, y, z, showEdges, showNormals, showModel);

            if (surface.texture != null)
            {
                surface.RebuildTexture();
                surface.DrawTexture();
            }
        }

        private void MoveCP()
        {
            double x = Convert.ToDouble(cpxTB.Text);
            double y = Convert.ToDouble(cpyTB.Text);
            double z = Convert.ToDouble(cpzTB.Text);

            bool showEdges = (bool)showEdgesCB.IsChecked;
            bool showNormals = (bool)showNormalsCB.IsChecked;
            bool showModel = (bool)showModelCB.IsChecked;

            selected.Move(x, y, z, showEdges, showNormals, showModel);
        }

        public void MoveControlCB(Control control)
        {
            sel = control;
            cpxTB.Text = sel.X.ToString();
            cpyTB.Text = sel.Y.ToString();
            cpzTB.Text = sel.Z.ToString();
        }
        #endregion

        #region LIGHTING
        private void lightButton_Click(object sender, RoutedEventArgs e)
        {
            if (lightCombo.SelectedIndex == -1 || !cp.SelectedColor.HasValue)
            {
                return;
            }

            var selected = (ComboBoxItem)lightCombo.SelectedItem;
            string text = (string)selected.Content;
            Color color = cp.SelectedColor.Value;

            if (text == "Directional")
            {
                DirectionalLight(color);
            }
            else if (text == "Ambient")
            {
                AmbientLight(color);
            }
            else if (text == "Point")
            {
                PointLight(color);
            }
            else
            {
                SpotLight(color);
            }
        }

        private void DirectionalLight(Color color)
        {
            if (string.IsNullOrWhiteSpace(ldxTB.Text) || string.IsNullOrWhiteSpace(ldyTB.Text) || string.IsNullOrWhiteSpace(ldzTB.Text))
            {
                return;
            }

            double x = Convert.ToDouble(ldxTB.Text);
            double y = Convert.ToDouble(ldxTB.Text);
            double z = Convert.ToDouble(ldxTB.Text);

            var light = new DirectionalLight(color, new Vector3D(x, y, z));
            var group = new Model3DGroup();
            group.Children.Add(light);
            var lightSource = new ModelVisual3D();
            lightSource.Content = group;

            Drawer.ChangeLighting(lightSource);
        }

        private void AmbientLight(Color color)
        {
            var light = new AmbientLight(color);
            var group = new Model3DGroup();
            group.Children.Add(light);
            var lightSource = new ModelVisual3D();
            lightSource.Content = group;

            Drawer.ChangeLighting(lightSource);
        }

        private void PointLight(Color color)
        {
            if (string.IsNullOrWhiteSpace(lpxTB.Text) || string.IsNullOrWhiteSpace(lpyTB.Text) || string.IsNullOrWhiteSpace(lpzTB.Text))
            {
                return;
            }

            double x = Convert.ToDouble(lpxTB.Text);
            double y = Convert.ToDouble(lpxTB.Text);
            double z = Convert.ToDouble(lpxTB.Text);

            var light = new PointLight(color, new Point3D(x, y, z));
            var group = new Model3DGroup();
            group.Children.Add(light);
            var lightSource = new ModelVisual3D();
            lightSource.Content = group;

            Drawer.ChangeLighting(lightSource);
        }

        private void SpotLight(Color color)
        {
            if (string.IsNullOrWhiteSpace(ldxTB.Text) || string.IsNullOrWhiteSpace(ldyTB.Text) || string.IsNullOrWhiteSpace(ldzTB.Text)
                || string.IsNullOrWhiteSpace(lpxTB.Text) || string.IsNullOrWhiteSpace(lpyTB.Text) || string.IsNullOrWhiteSpace(lpzTB.Text)
                || string.IsNullOrWhiteSpace(innerTB.Text) || string.IsNullOrWhiteSpace(outerTB.Text))
            {
                return;
            }

            double outer = Convert.ToDouble(outerTB.Text);
            double inner = Convert.ToDouble(innerTB.Text);

            double dx = Convert.ToDouble(ldxTB.Text);
            double dy = Convert.ToDouble(ldxTB.Text);
            double dz = Convert.ToDouble(ldxTB.Text);

            double px = Convert.ToDouble(lpxTB.Text);
            double py = Convert.ToDouble(lpxTB.Text);
            double pz = Convert.ToDouble(lpxTB.Text);

            var light = new SpotLight(color, new Point3D(px, py, pz), new Vector3D(dx, dy, dz), outer, inner);
            var group = new Model3DGroup();
            group.Children.Add(light);
            var lightSource = new ModelVisual3D();
            lightSource.Content = group;

            Drawer.ChangeLighting(lightSource);
        }

        private void lightCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var selected = (ComboBoxItem)lightCombo.SelectedItem;
                string text = (string)selected.Content;

                if (text == "Directional")
                {
                    ldxTB.IsEnabled = true;
                    ldyTB.IsEnabled = true;
                    ldzTB.IsEnabled = true;

                    lpxTB.IsEnabled = false;
                    lpyTB.IsEnabled = false;
                    lpzTB.IsEnabled = false;

                    innerTB.IsEnabled = false;
                    outerTB.IsEnabled = false;
                }
                else if (text == "Ambient")
                {
                    ldxTB.IsEnabled = false;
                    ldyTB.IsEnabled = false;
                    ldzTB.IsEnabled = false;

                    lpxTB.IsEnabled = false;
                    lpyTB.IsEnabled = false;
                    lpzTB.IsEnabled = false;

                    innerTB.IsEnabled = false;
                    outerTB.IsEnabled = false;
                }
                else if (text == "Point")
                {
                    ldxTB.IsEnabled = false;
                    ldyTB.IsEnabled = false;
                    ldzTB.IsEnabled = false;

                    lpxTB.IsEnabled = true;
                    lpyTB.IsEnabled = true;
                    lpzTB.IsEnabled = true;

                    innerTB.IsEnabled = false;
                    outerTB.IsEnabled = false;
                }
                else if (text == "Spot")
                {
                    ldxTB.IsEnabled = true;
                    ldyTB.IsEnabled = true;
                    ldzTB.IsEnabled = true;

                    lpxTB.IsEnabled = true;
                    lpyTB.IsEnabled = true;
                    lpzTB.IsEnabled = true;

                    innerTB.IsEnabled = true;
                    outerTB.IsEnabled = true;
                }
                else
                {
                    throw new Exception("The light source isn't selected");
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }
        #endregion

        #region MISC
        private void testButton_Click(object sender, RoutedEventArgs e)
        {
            var mesh = new MeshGeometry3D();
            var material = new DiffuseMaterial(Brushes.Blue);
            mesh.Positions.Add(new Point3D(0, 0, 0));
            mesh.Positions.Add(new Point3D(1, 0, 0));
            mesh.Positions.Add(new Point3D(0, 1, 0));
            mesh.Positions.Add(new Point3D(1, 1, 0));
            mesh.Positions.Add(new Point3D(0, 0, 1));
            mesh.Positions.Add(new Point3D(1, 0, 1));
            mesh.Positions.Add(new Point3D(0, 1, 1));
            mesh.Positions.Add(new Point3D(1, 1, 1));

            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(1);

            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(3);

            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(4);
            mesh.TriangleIndices.Add(2);

            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(4);
            mesh.TriangleIndices.Add(6);

            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(4);

            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(5);
            mesh.TriangleIndices.Add(4);

            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(7);
            mesh.TriangleIndices.Add(5);

            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(7);

            mesh.TriangleIndices.Add(4);
            mesh.TriangleIndices.Add(5);
            mesh.TriangleIndices.Add(6);

            mesh.TriangleIndices.Add(7);
            mesh.TriangleIndices.Add(6);
            mesh.TriangleIndices.Add(5);

            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(6);
            mesh.TriangleIndices.Add(3);

            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(6);
            mesh.TriangleIndices.Add(7);

            var geometry = new GeometryModel3D(mesh, material);
            var model = new ModelVisual3D();
            model.Content = geometry;
            Drawer.Draw(model);
        }
        #endregion

        #region TEXTURING
        private void textureButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(rTB.Text) || string.IsNullOrWhiteSpace(tcountTB.Text) || surface == null)
            {
                return;
            }

            double r = Convert.ToDouble(rTB.Text);
            int count = Convert.ToInt32(tcountTB.Text);

            surface.BuildTexture(r, count);
            surface.DrawTexture();
        }

        private void toffsetButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tdxTB.Text) || string.IsNullOrWhiteSpace(tdyTB.Text) || surface == null)
                {
                    return;
                }

                double dx = Convert.ToDouble(tdxTB.Text);
                double dy = Convert.ToDouble(tdyTB.Text);

                surface.OffsetTexture(dx, dy);
                surface.DrawTexture();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void trotateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tangleTB.Text) || surface == null)
                {
                    return;
                }

                double angle = Convert.ToDouble(tangleTB.Text);

                surface.RotateTexture(angle);
                surface.DrawTexture();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }
        #endregion

        #region MATERIAL
        private void materialButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(powerTB.Text) || !diffuseCP.SelectedColor.HasValue || !specularCP.SelectedColor.HasValue || surface == null)
            {
                return;
            }

            Color dc = diffuseCP.SelectedColor.Value;
            var db = new SolidColorBrush(dc);
            Color sc = specularCP.SelectedColor.Value;
            var sb = new SolidColorBrush(sc);
            double power = Convert.ToDouble(powerTB.Text);

            MaterialGroup mg = new MaterialGroup();
            DiffuseMaterial diffuse = new DiffuseMaterial(db);
            SpecularMaterial specular = new SpecularMaterial(sb, power);
            mg.Children.Add(diffuse);
            mg.Children.Add(specular);

            surface.EraseSurface();
            surface.SetMaterial(mg);
            surface.DrawModels();
        }
        #endregion
    }
}
