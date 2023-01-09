using System.Windows;
using System.Windows.Media.Media3D;
using static lab1.MainWindow;

namespace lab1
{
    public class BezierRevolution
    {
        public BezierSegment segment1;
        public BezierSegment segment2;
        public BezierSegment segment3;

        public BezierRevolution()
        {

        }

        public void Build(int count, MoveDelegate move)
        {
            double[] r1 = { 2, 1, 1, 2 };
            double[] r2 = { 2, 2, 0.4, 3 };
            double[] r3 = { 3, 2, 2, 1 };

            double[] c1 = { -2, -1.6, -1.2, -0.8 };
            //double[] c1 = { -2, -1, 0, 1 };
            double[] c2 = { -0.8, -0.4, 0, 0.4 };
            double[] c3 = { 0.4, 0.8, 1.2, 1.4 };

            segment1 = new BezierSegment(count, r1, c1, move);
            segment1.Build();
            segment2 = new BezierSegment(count, r2, c2, move);
            segment2.Build();
            segment3 = new BezierSegment(count, r3, c3, move);
            segment3.Build();
            
            //MessageBox.Show($"{segment1.points.Count * segment1.points[0].Count * 3}");
            BuildReferences(segment1, segment2);
            BuildReferences(segment2, segment3);
        }

        public void Build(int count, MoveDelegate move, MaterialGroup mg)
        {
            double[] r1 = { 2, 1, 1, 2 };
            double[] r2 = { 2, 2, 0.4, 3 };
            double[] r3 = { 3, 2, 2, 1 };

            double[] c1 = { -2, -1.6, -1.2, -0.8 };
            //double[] c1 = { -2, -1, 0, 1 };
            double[] c2 = { -0.8, -0.4, 0, 0.4 };
            double[] c3 = { 0.4, 0.8, 1.2, 1.4 };

            segment1 = new BezierSegment(count, r1, c1, move);
            segment1.Build(mg);
            segment2 = new BezierSegment(count, r2, c2, move);
            segment2.Build(mg);
            segment3 = new BezierSegment(count, r3, c3, move);
            segment3.Build(mg);

            //MessageBox.Show($"{segment1.points.Count * segment1.points[0].Count * 3}");
            BuildReferences(segment1, segment2);
            BuildReferences(segment2, segment3);
        }

        public void DrawEdges()
        {
            segment1.DrawEdges();
            segment2.DrawEdges();
            segment3.DrawEdges();
        }

        public void DrawPoints()
        {
            segment1.DrawPoints();
            segment2.DrawPoints();
            segment3.DrawPoints();
        }

        public void DrawControls()
        {
            segment1.DrawControls();
            segment2.DrawControls();
            segment3.DrawControls();
        }

        public void DrawModels()
        {
            segment1.DrawModel();
            segment2.DrawModel();
            segment3.DrawModel();
        }

        public void DrawNormals()
        {
            segment1.DrawNormals();
            segment2.DrawNormals();
            segment3.DrawNormals();
        }

        private void BuildReferences(BezierSegment segment1, BezierSegment segment2)
        {
            foreach (var ctrlLists1 in segment1.ctrlPoints)
            {
                foreach (var cp1 in ctrlLists1)
                {
                    foreach (var ctrlLists2 in segment2.ctrlPoints)
                    {
                        foreach (var cp2 in ctrlLists2)
                        {
                            if (cp1.X == cp2.X && cp1.Y == cp2.Y && cp1.Z == cp2.Z)
                            {
                                cp2.clone = cp1;
                                cp1.clone = cp2;
                            }
                        }
                    }
                }
            }
        }
    }
}
