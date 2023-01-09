using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace lab1
{
    public static class Drawer
    {
        public static Viewport3D grid;
        private static ModelVisual3D lightSource;

        static Drawer()
        {
            var light = new AmbientLight(Colors.Transparent);
            var group = new Model3DGroup();
            group.Children.Add(light);
            lightSource = new ModelVisual3D();
            lightSource.Content = group;
        }
        public static void Draw(Visual3D obj)
        {
            obj.Transform = new Transform3DGroup();
            grid.Children.Add(obj);
        }

        public static void Erase(Visual3D obj)
        {
            grid.Children.Remove(obj);
        }

        public static void Clear()
        {
            grid.Children.Clear();
            grid.Children.Add(lightSource);
        }

        public static void ChangeLighting(ModelVisual3D newSource)
        {
            grid.Children.Remove(lightSource);
            lightSource = newSource;
            grid.Children.Add(lightSource);
        }
    }
}
