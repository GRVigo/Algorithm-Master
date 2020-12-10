/*  This file is part of "Algorithm Master"
  
    Copyright (C) 2018 Germán Ramos Rodríguez

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.

    Germán Ramos Rodríguez
    Vigo, Spain
    grvigo@hotmail.com
*/

using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Algorithm_Master
{
    /// <summary>
    /// Class for create and manage stickers 3D geometry
    /// </summary>
    public class Sticker3D
    {
        /// <summary>
        /// Sticker geometry model for ViewPort3D
        /// </summary>
        public GeometryModel3D StickerModel3D;

        /// <summary>
        /// Base geometry model for ViewPort3D
        /// </summary>
        public GeometryModel3D BaseModel3D;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sp">Sticker position</param>
        /// <param name="sm">Sticker material</param>
        /// <param name="bm">Base material</param>
        public Sticker3D(StickerPositions sp, DiffuseMaterial sm, DiffuseMaterial bm)
        {
            StickerModel3D = new GeometryModel3D();
            BaseModel3D = new GeometryModel3D();

            StickerModel3D.Material = sm;
            BaseModel3D.Material = bm;

            MeshGeometry3D StickerMeshGeometry = new MeshGeometry3D();
            MeshGeometry3D BaseMeshGeometry = new MeshGeometry3D();

            StickerMeshGeometry.Positions = BaseMeshGeometry.Positions = AssignStickerPoints(sp, 2.0);

            StickerMeshGeometry.TriangleIndices = new Int32Collection { 2, 10, 11,
                                                                        2, 11, 7,
                                                                        2, 7, 1,
                                                                        1, 7, 8,
                                                                        1, 8, 4,
                                                                        1, 4, 5 };
            BaseMeshGeometry.TriangleIndices = new Int32Collection { 0, 2, 1,
                                                                     0, 1, 3,
                                                                     1, 5, 3,
                                                                     3, 5, 4,
                                                                     3, 4, 6,
                                                                     4, 8, 6,
                                                                     6, 8, 7,
                                                                     6, 7, 9,
                                                                     7, 11, 9,
                                                                     9, 11, 10,
                                                                     9, 10, 0,
                                                                     10, 2 ,0,
                                                                     0, 13, 12,
                                                                     0, 3, 13,
                                                                     3, 14, 13,
                                                                     3, 6, 14,
                                                                     6, 15, 14,
                                                                     6, 9 , 15,
                                                                     9, 12, 15,
                                                                     9, 0, 12 };

            StickerModel3D.Geometry = StickerMeshGeometry;
            BaseModel3D.Geometry = BaseMeshGeometry;
        }

        /// <summary>
        /// Assigngs the sticker points for 3D
        /// </summary>
        /// <param name="sp">Sticker position</param>
        /// <param name="scale">Sticker scale (length)</param>
        /// <returns></returns>
        private static Point3DCollection AssignStickerPoints(StickerPositions sp, double scale)
        {
            const double K1 = 0.176777;
            const double K2 = 0.073223;                    

            const double H = 0.05;

            Point3D[] P3D = new Point3D[16];
            Point3DCollection P3DCol = new Point3DCollection();

            P3D[0] = new Point3D(0, 0, 0);
            P3D[1] = new Point3D(K1, H, K2);
            P3D[2] = new Point3D(K2, H, K1);

            P3D[3] = new Point3D(1, 0, 0);
            P3D[4] = new Point3D(1 - K2, H, K1);
            P3D[5] = new Point3D(1 - K1, H, K2);

            P3D[6] = new Point3D(1, 0, 1);
            P3D[7] = new Point3D(1 - K1, H, 1 - K2);
            P3D[8] = new Point3D(1 - K2, H, 1 - K1);

            P3D[9] = new Point3D(0, 0, 1);
            P3D[10] = new Point3D(K2, H, 1 - K1);
            P3D[11] = new Point3D(K1, H, 1 - K2);

            P3D[12] = new Point3D(0, -1, 0);
            P3D[13] = new Point3D(1, -1, 0);
            P3D[14] = new Point3D(1, -1, 1);
            P3D[15] = new Point3D(0, -1, 1);

            for (int n = 0; n < P3D.Length; n++)
            { // Center the sticker in (0,0,0) and scale
                P3D[n].X -= 0.5;
                P3D[n].Z -= 0.5;
                P3D[n].X *= scale;
                P3D[n].Y *= scale;
                P3D[n].Z *= scale;
            }

            Matrix3D matrix = Matrix3D.Identity;
            
            switch (StickerData.GetStickerFace(sp))
            { // Rotate the sticker according his face
                case Faces.D: matrix.Rotate(new Quaternion(new Vector3D(1, 0, 0), 180)); break;
                case Faces.F: matrix.Rotate(new Quaternion(new Vector3D(1, 0, 0), 90)); break;
                case Faces.B: matrix.Rotate(new Quaternion(new Vector3D(1, 0, 0), 270)); break;
                case Faces.R: matrix.Rotate(new Quaternion(new Vector3D(0, 0, 1), 270)); break;
                case Faces.L: matrix.Rotate(new Quaternion(new Vector3D(0, 0, 1), 90)); break;
                default: matrix.Rotate(new Quaternion(new Vector3D(1, 0, 0), 0)); break;
            }
            for (int n = 0; n < P3D.Length; n++) P3D[n] = matrix.Transform(P3D[n]); // Orientate points

            double dist = scale * 1.5;

            switch (StickerData.GetStickerFace(sp))
            { // Positioning the sticker according his face
                case Faces.U:
                    for (int n = 0; n < P3D.Length; n++) P3D[n].Y += dist;
                    switch (sp)
                    {
                        case StickerPositions.UF_U: for (int n = 0; n < P3D.Length; n++) P3D[n].Z += scale; break;
                        case StickerPositions.UR_U: for (int n = 0; n < P3D.Length; n++) P3D[n].X += scale; break;
                        case StickerPositions.UB_U: for (int n = 0; n < P3D.Length; n++) P3D[n].Z -= scale; break;
                        case StickerPositions.UL_U: for (int n = 0; n < P3D.Length; n++) P3D[n].X -= scale; break;

                        case StickerPositions.UFR_U:
                            for (int n = 0; n < P3D.Length; n++) P3D[n].Z += scale;
                            for (int n = 0; n < P3D.Length; n++) P3D[n].X += scale;
                            break;
                        case StickerPositions.UFL_U:
                            for (int n = 0; n < P3D.Length; n++) P3D[n].Z += scale;
                            for (int n = 0; n < P3D.Length; n++) P3D[n].X -= scale;
                            break;
                        case StickerPositions.UBL_U:
                            for (int n = 0; n < P3D.Length; n++) P3D[n].Z -= scale;
                            for (int n = 0; n < P3D.Length; n++) P3D[n].X -= scale;
                            break;
                        case StickerPositions.UBR_U:
                            for (int n = 0; n < P3D.Length; n++) P3D[n].Z -= scale;
                            for (int n = 0; n < P3D.Length; n++) P3D[n].X += scale;
                            break;
                    }
                    break;

                case Faces.D:
                    for (int n = 0; n < P3D.Length; n++) P3D[n].Y -= dist;
                    switch (sp)
                    {
                        case StickerPositions.DF_D: for (int n = 0; n < P3D.Length; n++) P3D[n].Z += scale; break;
                        case StickerPositions.DR_D: for (int n = 0; n < P3D.Length; n++) P3D[n].X += scale; break;
                        case StickerPositions.DB_D: for (int n = 0; n < P3D.Length; n++) P3D[n].Z -= scale; break;
                        case StickerPositions.DL_D: for (int n = 0; n < P3D.Length; n++) P3D[n].X -= scale; break;

                        case StickerPositions.DFR_D:
                            for (int n = 0; n < P3D.Length; n++) P3D[n].Z += scale;
                            for (int n = 0; n < P3D.Length; n++) P3D[n].X += scale;
                            break;
                        case StickerPositions.DFL_D:
                            for (int n = 0; n < P3D.Length; n++) P3D[n].Z += scale;
                            for (int n = 0; n < P3D.Length; n++) P3D[n].X -= scale;
                            break;
                        case StickerPositions.DBL_D:
                            for (int n = 0; n < P3D.Length; n++) P3D[n].Z -= scale;
                            for (int n = 0; n < P3D.Length; n++) P3D[n].X -= scale;
                            break;
                        case StickerPositions.DBR_D:
                            for (int n = 0; n < P3D.Length; n++) P3D[n].Z -= scale;
                            for (int n = 0; n < P3D.Length; n++) P3D[n].X += scale;
                            break;
                    }
                    break;

                case Faces.F:
                    for (int n = 0; n < P3D.Length; n++) P3D[n].Z += dist;
                    switch (sp)
                    {
                        case StickerPositions.UF_F: for (int n = 0; n < P3D.Length; n++) P3D[n].Y += scale; break;
                        case StickerPositions.FR_F: for (int n = 0; n < P3D.Length; n++) P3D[n].X += scale; break;
                        case StickerPositions.DF_F: for (int n = 0; n < P3D.Length; n++) P3D[n].Y -= scale; break;
                        case StickerPositions.LF_F: for (int n = 0; n < P3D.Length; n++) P3D[n].X -= scale; break;

                        case StickerPositions.UFR_F:
                            for (int n = 0; n < P3D.Length; n++) P3D[n].Y += scale;
                            for (int n = 0; n < P3D.Length; n++) P3D[n].X += scale;
                            break;
                        case StickerPositions.UFL_F:
                            for (int n = 0; n < P3D.Length; n++) P3D[n].Y += scale;
                            for (int n = 0; n < P3D.Length; n++) P3D[n].X -= scale;
                            break;
                        case StickerPositions.DFR_F:
                            for (int n = 0; n < P3D.Length; n++) P3D[n].Y -= scale;
                            for (int n = 0; n < P3D.Length; n++) P3D[n].X += scale;
                            break;
                        case StickerPositions.DFL_F:
                            for (int n = 0; n < P3D.Length; n++) P3D[n].Y -= scale;
                            for (int n = 0; n < P3D.Length; n++) P3D[n].X -= scale;
                            break;
                    }
                    break;
                case Faces.B:
                    for (int n = 0; n < P3D.Length; n++) P3D[n].Z -= dist;
                    switch (sp)
                    {
                        case StickerPositions.UB_B: for (int n = 0; n < P3D.Length; n++) P3D[n].Y += scale; break;
                        case StickerPositions.RB_B: for (int n = 0; n < P3D.Length; n++) P3D[n].X += scale; break;
                        case StickerPositions.DB_B: for (int n = 0; n < P3D.Length; n++) P3D[n].Y -= scale; break;
                        case StickerPositions.BL_B: for (int n = 0; n < P3D.Length; n++) P3D[n].X -= scale; break;

                        case StickerPositions.UBR_B:
                            for (int n = 0; n < P3D.Length; n++) P3D[n].Y += scale;
                            for (int n = 0; n < P3D.Length; n++) P3D[n].X += scale;
                            break;
                        case StickerPositions.UBL_B:
                            for (int n = 0; n < P3D.Length; n++) P3D[n].Y += scale;
                            for (int n = 0; n < P3D.Length; n++) P3D[n].X -= scale;
                            break;
                        case StickerPositions.DBR_B:
                            for (int n = 0; n < P3D.Length; n++) P3D[n].Y -= scale;
                            for (int n = 0; n < P3D.Length; n++) P3D[n].X += scale;
                            break;
                        case StickerPositions.DBL_B:
                            for (int n = 0; n < P3D.Length; n++) P3D[n].Y -= scale;
                            for (int n = 0; n < P3D.Length; n++) P3D[n].X -= scale;
                            break;
                    }
                    break;
                case Faces.R:
                    for (int n = 0; n < P3D.Length; n++) P3D[n].X += dist;
                    switch (sp)
                    {
                        case StickerPositions.UR_R: for (int n = 0; n < P3D.Length; n++) P3D[n].Y += scale; break;
                        case StickerPositions.RB_R: for (int n = 0; n < P3D.Length; n++) P3D[n].Z -= scale; break;
                        case StickerPositions.DR_R: for (int n = 0; n < P3D.Length; n++) P3D[n].Y -= scale; break;
                        case StickerPositions.FR_R: for (int n = 0; n < P3D.Length; n++) P3D[n].Z += scale; break;

                        case StickerPositions.UFR_R:
                            for (int n = 0; n < P3D.Length; n++) P3D[n].Y += scale;
                            for (int n = 0; n < P3D.Length; n++) P3D[n].Z += scale;
                            break;
                        case StickerPositions.UBR_R:
                            for (int n = 0; n < P3D.Length; n++) P3D[n].Y += scale;
                            for (int n = 0; n < P3D.Length; n++) P3D[n].Z -= scale;
                            break;
                        case StickerPositions.DBR_R:
                            for (int n = 0; n < P3D.Length; n++) P3D[n].Y -= scale;
                            for (int n = 0; n < P3D.Length; n++) P3D[n].Z -= scale;
                            break;
                        case StickerPositions.DFR_R:
                            for (int n = 0; n < P3D.Length; n++) P3D[n].Y -= scale;
                            for (int n = 0; n < P3D.Length; n++) P3D[n].Z += scale;
                            break;
                    }
                    break;
                case Faces.L:
                    for (int n = 0; n < P3D.Length; n++) P3D[n].X -= dist;
                    switch (sp)
                    {
                        case StickerPositions.UL_L: for (int n = 0; n < P3D.Length; n++) P3D[n].Y += scale; break;
                        case StickerPositions.LF_L: for (int n = 0; n < P3D.Length; n++) P3D[n].Z += scale; break;
                        case StickerPositions.DL_L: for (int n = 0; n < P3D.Length; n++) P3D[n].Y -= scale; break;
                        case StickerPositions.BL_L: for (int n = 0; n < P3D.Length; n++) P3D[n].Z -= scale; break;

                        case StickerPositions.UFL_L:
                            for (int n = 0; n < P3D.Length; n++) P3D[n].Y += scale;
                            for (int n = 0; n < P3D.Length; n++) P3D[n].Z += scale;
                            break;
                        case StickerPositions.UBL_L:
                            for (int n = 0; n < P3D.Length; n++) P3D[n].Y += scale;
                            for (int n = 0; n < P3D.Length; n++) P3D[n].Z -= scale;
                            break;
                        case StickerPositions.DBL_L:
                            for (int n = 0; n < P3D.Length; n++) P3D[n].Y -= scale;
                            for (int n = 0; n < P3D.Length; n++) P3D[n].Z -= scale;
                            break;
                        case StickerPositions.DFL_L:
                            for (int n = 0; n < P3D.Length; n++) P3D[n].Y -= scale;
                            for (int n = 0; n < P3D.Length; n++) P3D[n].Z += scale;
                            break;
                    }
                    break;
            }

            for (int n = 0; n < P3D.Length; n++) P3DCol.Add(P3D[n]);
            return P3DCol;
        }

        /// <summary>
        /// Set sticker materials
        /// </summary>
        /// <param name="SM">Material for sticker</param>
        /// <param name="BM">Material for base</param>
        public void SetStickerMaterials(DiffuseMaterial SM, DiffuseMaterial BM)
        {
            StickerModel3D.Material = SM;
            BaseModel3D.Material = BM;
        }

        /// <summary>
        /// Rotation transform in y axis
        /// </summary>
        /// <param name="Angle">Angle for rotation</param>
        public void RotateY(double Angle)
        {
            StickerModel3D.Transform = BaseModel3D.Transform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), Angle));
        }

        /// <summary>
        /// Rotation transform in z axis
        /// </summary>
        /// <param name="Angle">Angle for rotation</param>
        public void RotateZ(double Angle)
        {
            StickerModel3D.Transform = BaseModel3D.Transform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), Angle));
        }

        /// <summary>
        /// Rotation transform in x axis
        /// </summary>
        /// <param name="Angle">Angle for rotation</param>
        public void RotateX(double Angle)
        {
            StickerModel3D.Transform = BaseModel3D.Transform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), Angle));
        }
    }
}
