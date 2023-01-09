using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Media3D;
using static lab1.MainWindow;

namespace lab1
{
    public class BezierSurface
    {
        public BezierPatch segment1;
        public BezierPatch segment2;
        Control[,] ctrlPts1;
        Control[,] ctrlPts2;

        public Astroid texture;

        public BezierSurface()
        {

        }

        #region BUILDING
        public void Build(int n, int m, int un, int uv, MoveDel move)
        {
            segment1 = new BezierPatch(n, m, un, uv);
            ctrlPts1 = new Control[,]
            {
                { new Control(-4, 0, -3, segment1, move), new Control(-3, 0, -3, segment1, move), new Control(-2, 0, -3, segment1, move), new Control(-1, 0, -3, segment1, move), new Control(0, 0, -3, segment1, move) },
                { new Control(-4, 0, 0, segment1, move), new Control(-3, 0, 0, segment1, move), new Control(-2, 0, 0, segment1, move), new Control(-1, 0, 0, segment1, move), new Control(0, 0, 0, segment1, move) },
                { new Control(-4, 0, 3, segment1, move), new Control(-3, 0, 3, segment1, move), new Control(-2, 0, 3, segment1, move), new Control(-1, 0, 3, segment1, move), new Control(0, 0, 3, segment1, move) }
            };

            segment1.ctrlPoints = ctrlPts1;
            segment1.Build();

            segment2 = new BezierPatch(n, m, un, uv);
            ctrlPts2 = new Control[,]
            {
                { new Control(0, 0, -3, segment1, move), new Control(1, 0, -3, segment2, move), new Control(2, 0, -3, segment2, move), new Control(3, 0, -3, segment2, move), new Control(4, 0, -3, segment2, move) },
                { new Control(0, 0, 0, segment1, move), new Control(1, 0, 0, segment2, move), new Control(2, 0, 0, segment2, move), new Control(3, 0, 0, segment2, move), new Control(4, 0, 0, segment2, move) },
                { new Control(0, 0, 3, segment1, move), new Control(1, 0, 3, segment2, move), new Control(2, 0, 3, segment2, move), new Control(3, 0, 3, segment2, move), new Control(4, 0, 3, segment2, move) }
            };
            segment2.ctrlPoints = ctrlPts2;
            segment2.Build();

            BuildReferences();
        }
        #endregion

        #region DRAWING
        public void DrawEdges()
        {
            segment1.DrawEdges();
            segment2.DrawEdges();
        }

        public void DrawPoints()
        {
            segment1.DrawPoints();
            segment2.DrawPoints();
        }

        public void DrawControls()
        {
            segment1.DrawControls();
            segment2.DrawControls();
        }

        public void DrawModels()
        {
            segment1.DrawModel();
            segment2.DrawModel();
        }

        public void DrawNormals()
        {
            segment1.DrawNormals();
            segment2.DrawNormals();
        }

        public void DrawTexturePoints(double radius = 0.01)
        {
            texture.DrawPoints(radius);
        }

        public void DrawTexture()
        {
            texture.DrawCurve();
        }

        private void BuildReferences()
        {
            ctrlPts1[0, 4].clone = ctrlPts2[0, 0];
            ctrlPts2[0, 0].clone = ctrlPts1[0, 4];

            ctrlPts1[1, 4].clone = ctrlPts2[1, 0];
            ctrlPts2[1, 0].clone = ctrlPts1[1, 4];

            ctrlPts1[2, 4].clone = ctrlPts2[2, 0];
            ctrlPts2[2, 0].clone = ctrlPts1[2, 4];
        }
        #endregion

        #region TEXTURING
        public void BuildTexture(double r, int count)
        {
            ClearTexture();

            texture = new Astroid(r, count);
            texture.Build();
            texture.points3D = new List<Point3D>();

            CalcTexture();
        }

        public void RebuildTexture()
        {
            ClearTexture();
            texture.points3D = new List<Point3D>();

            CalcTexture();
        }

        public void ClearTexture()
        {
            if (texture != null)
            {
                if (texture.curve != null)
                {
                    foreach (var l in texture.curve)
                    {
                        Drawer.Erase(l);
                    }
                }
                if (texture.pointsUI != null)
                {
                    foreach (var p in texture.pointsUI)
                    {
                        Drawer.Erase(p);
                    }
                }
            }
        }

        public void OffsetTexture(double dx, double dy)
        {
            ClearTexture();
            texture.points3D = new List<Point3D>();

            for (int i = 0; i < texture.points.Count; i++)
            {
                texture.points[i] = new Point(texture.points[i].X + dx, texture.points[i].Y + dy);
            }

            CalcTexture();
        }

        public void RotateTexture(double angle)
        {
            double teta = Misc.DegreesToRadians(angle);
            ClearTexture();
            texture.points3D = new List<Point3D>();

            for (int i = 0; i < texture.points.Count; i++)
            {
                double x = texture.points[i].X * Math.Cos(teta) - texture.points[i].Y * Math.Sin(teta);
                double y = texture.points[i].X * Math.Sin(teta) + texture.points[i].Y * Math.Cos(teta);
                texture.points[i] = new Point(x, y);
            }

            CalcTexture();
        }

        public void CalcTexture()
        {
            texture.points3D = new List<Point3D>();

            for (int i = 0; i < texture.points.Count; i++)
            {
                double u = 0.5 + 0.1 * texture.points[i].X / 10;
                double v = 0.5 + 0.1 * texture.points[i].Y / 10;

                if (u > 1 || u < 0 || v > 2 || v < 0)
                {
                    continue;
                }
                else if (v > 1)
                {
                    var point = segment2.CalcTexture(u, v - 1);
                    texture.points3D.Add(point);
                }
                else
                {
                    var point = segment1.CalcTexture(u, v);
                    texture.points3D.Add(point);
                }
            }
        }
        #endregion

        #region MATERIAL
        public void SetMaterial(MaterialGroup mg)
        {
            segment1.SetMaterial(mg);
            segment2.SetMaterial(mg);
        }

        public void EraseSurface()
        {
            segment1.EraseModel();
            segment2.EraseModel();
        }
        #endregion
    }
}
