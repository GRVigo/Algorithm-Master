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

using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace Algorithm_Master
{
    /// <summary>
    /// Structure for Rubik's cube face materials (for ViewPort3D)
    /// </summary>
    public struct FaceMaterial
    {
        public DiffuseMaterial U, D, F, B, R, L, Base, Neutral;
    }

    /// <summary>
    /// Class for manage a Rubik's cube
    /// </summary>
    public class Cube3D
    {
        #region Fields

        /// <summary>
        /// Scene object
        /// </summary>
        private Viewport3D RubikViewport3D;

        /// <summary>
        /// Scene object
        /// </summary>
        private Model3DGroup RubikModel3DGroup;

        /// <summary>
        /// Scene object
        /// </summary>
        private ModelVisual3D RubikModelVisual3D;

        /// <summary>
        /// 3D Stickers - a Rubik's cube has 54 stickers
        /// </summary>
        private Sticker3D[] Stickers3D; 

        /// <summary>
        /// Cube data
        /// </summary>
        public CubeData Cube;

        /// <summary>
        /// Struct for materials
        /// </summary>
        public FaceMaterial RubikMaterials;

        /// <summary>
        /// Camera position with cube in UF orientation
        /// </summary>
        private Point3D NormalCameraPosition;

        /// <summary>
        /// Camera alpha angle (in radians)
        /// </summary>
        private double CamAlpha;

        /// <summary>
        /// Camera beta angle (in radians)
        /// </summary>
        private double CamBeta;

        /// <summary>
        /// Render image object
        /// </summary>
        public RenderTargetBitmap renderBMP;

        /// <summary>
        /// Render image object
        /// </summary>
        private DrawingVisual BitmapBackground;

        /// <summary>
        /// Render image object
        /// </summary>
        private DrawingContext Content;

        /// <summary>
        /// Which stickers are represented in neutral color
        /// </summary>
        public bool[] NeutralStickers;

        /// <summary>
        /// Mask with no neutral stickers
        /// </summary>
        public static readonly string NoNeutralMask = "000000000000000000000000000000000000000000000000000000";

        /// <summary>
        /// Mask with all stickers neutral
        /// </summary>
        public static readonly string AllNeutralMask = "111111111111111111111111111111111111111111111111111111";

        /// <summary>
        /// Mask with all stickers neutral except centers
        /// </summary>
        public static readonly string NoCentersNeutralMask = "000000111111111111111111111111111111111111111111111111";

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the camera distance to the center of the cube
        /// </summary>
        public double CameraDistance
        {
            get
            {
                if (RubikViewport3D == null || RubikViewport3D.Camera == null) return 0.0;

                PerspectiveCamera RubikCamera = (PerspectiveCamera)RubikViewport3D.Camera;

                return new Vector3D(RubikCamera.Position.X, RubikCamera.Position.Y, RubikCamera.Position.Z).Length;
            }
        }

        /// <summary>
        /// Gets the camera horizontal angle (alpha in degrees)
        /// </summary>
        public double CameraAlpha => RadiansToDegrees(CamAlpha);

        /// <summary>
        /// Gets the camera vertical angle (beta in degrees)
        /// </summary>
        public double CameraBeta => RadiansToDegrees(CamBeta);

        /// <summary>
        /// Gets or sets the neutral color table throw a string mask (54 chars string "00100...")
        /// </summary>
        public string NeutralMask
        {
            get
            {
                if (NeutralStickers != null)
                {
                    StringBuilder sb = new StringBuilder();
                    for (int n = 0; n < NeutralStickers.Length; n++)
                    {
                        if (NeutralStickers[n]) sb.Append('1');
                        else sb.Append('0');
                    }
                    return sb.ToString();
                }
                return NoNeutralMask;         
            }
            set
            {
                if (NeutralStickers == null) return;
                if (string.IsNullOrEmpty(value) || value.Length != NeutralStickers.Length)
                    for (int n = 0; n < NeutralStickers.Length; n++) NeutralStickers[n] = false;
                else
                    for (int n = 0; n < NeutralStickers.Length; n++) NeutralStickers[n] = value[n] == '1';
                UpdateMaterials();
            }
        } 

        #endregion Properties

        /// <summary>
        /// Contructor, creates a solved cube
        /// </summary>
        public Cube3D()
        {
            // Declare scene objects
            RubikViewport3D = new Viewport3D();
            RubikModel3DGroup = new Model3DGroup();
            RubikModelVisual3D = new ModelVisual3D();

            Cube = new CubeData();

            Stickers3D = new Sticker3D[54]; // A 3x3x3 Rubik's cube has 54 stickers

            RubikMaterials.U = new DiffuseMaterial(AMSettings.UBrush);
            RubikMaterials.D = new DiffuseMaterial(AMSettings.DBrush);
            RubikMaterials.F = new DiffuseMaterial(AMSettings.FBrush);
            RubikMaterials.B = new DiffuseMaterial(AMSettings.BBrush);
            RubikMaterials.R = new DiffuseMaterial(AMSettings.RBrush);
            RubikMaterials.L = new DiffuseMaterial(AMSettings.LBrush);
            RubikMaterials.Base = new DiffuseMaterial(AMSettings.BaseBrush);
            RubikMaterials.Neutral = new DiffuseMaterial(AMSettings.NeutralBrush);

            NeutralStickers = new bool[54]; // 54 stickers in a 3x3x3 cube
            for (int i = 0; i < NeutralStickers.Length; i++) NeutralStickers[i] = false; // No neutral stickers at start

            // Assign color to the stickers
            foreach (StickerPositions Pos in Enum.GetValues(typeof(StickerPositions)))
            {
                switch (Cube.GetStickerFacePosition(Pos))
                {
                    case Faces.U: Stickers3D[(int)Pos] = new Sticker3D(Pos, RubikMaterials.U, RubikMaterials.Base); break;
                    case Faces.D: Stickers3D[(int)Pos] = new Sticker3D(Pos, RubikMaterials.D, RubikMaterials.Base); break;
                    case Faces.F: Stickers3D[(int)Pos] = new Sticker3D(Pos, RubikMaterials.F, RubikMaterials.Base); break;
                    case Faces.B: Stickers3D[(int)Pos] = new Sticker3D(Pos, RubikMaterials.B, RubikMaterials.Base); break;
                    case Faces.R: Stickers3D[(int)Pos] = new Sticker3D(Pos, RubikMaterials.R, RubikMaterials.Base); break;
                    case Faces.L: Stickers3D[(int)Pos] = new Sticker3D(Pos, RubikMaterials.L, RubikMaterials.Base); break;
                }
            }

            // Set default camera in the scene in spheric (distance, alpha, beta) format
            SetCamera(AMSettings.CameraDistance, AMSettings.CameraAlpha, AMSettings.CameraBeta);

            // Define the lights cast in the scene
            RubikModel3DGroup.Children.Add(new AmbientLight(Colors.White));

            // Add stickers to the model 3D
            foreach (StickerPositions Pos in Enum.GetValues(typeof(StickerPositions)))
            {
                RubikModel3DGroup.Children.Add(Stickers3D[(int)Pos].StickerModel3D);
                RubikModel3DGroup.Children.Add(Stickers3D[(int)Pos].BaseModel3D);
            }

            // Add the group of models to the ModelVisual3d
            RubikModelVisual3D.Content = RubikModel3DGroup;
            RubikViewport3D.Children.Add(RubikModelVisual3D);

            // For speed-up the render
            RubikViewport3D.ClipToBounds = false;
            RubikViewport3D.IsHitTestVisible = false;
        }

        /// <summary>
        /// Sets the camera with distance and alpha & beta angles
        /// </summary>
        /// <param name="dist">Distane from the camera to the center of the cube</param>
        /// <param name="alpha">Horizontal angle referred to the x axis</param>
        /// <param name="beta">Vertical angle</param>
        public void SetCamera(double dist, double alpha, double beta)
        {
            CamAlpha = DegreesToRadians(alpha);
            CamBeta = DegreesToRadians(beta);

            Point3D p = new Point3D
            {
                X = dist * Math.Cos(CamBeta) * Math.Cos(CamAlpha),
                Y = dist * Math.Sin(CamBeta),
                Z = dist * Math.Cos(CamBeta) * Math.Sin(CamAlpha)
            };

            SetCamera(p);
        }

        /// <summary>
        /// Set a camera in the scene looking to the cube
        /// </summary>
        /// <param name="cp">Camera position</param>
        private void SetCamera(Point3D cp)
        {
            // Defines the camera used to view the 3D object. In order to view the 3D object, 
            // the camera must be positioned and pointed such that the object is within view  
            // of the camera.
            PerspectiveCamera RubikCamera = new PerspectiveCamera
            {
                // Define camera's horizontal field of view in degrees.
                FieldOfView = 30d
            };

            NormalCameraPosition = cp;

            // Specify where in the 3D scene the camera is.
            switch (Cube.Spin)
            {
                case CubeSpins.UF:
                    RubikCamera.Position = new Point3D(cp.X, cp.Y, cp.Z);
                    RubikCamera.UpDirection = new Vector3D(0, 1, 0);
                    break;
                case CubeSpins.UR:
                    RubikCamera.Position = new Point3D(cp.Z, cp.Y, -cp.X);
                    RubikCamera.UpDirection = new Vector3D(0, 1, 0);
                    break;
                case CubeSpins.UB:
                    RubikCamera.Position = new Point3D(-cp.X, cp.Y, -cp.Z);
                    RubikCamera.UpDirection = new Vector3D(0, 1, 0);
                    break;
                case CubeSpins.UL:
                    RubikCamera.Position = new Point3D(-cp.Z, cp.Y, cp.X);
                    RubikCamera.UpDirection = new Vector3D(0, 1, 0);
                    break;
                case CubeSpins.DF:
                    RubikCamera.Position = new Point3D(-cp.X, -cp.Y, cp.Z);
                    RubikCamera.UpDirection = new Vector3D(0, -1, 0);
                    break;
                case CubeSpins.DR:
                    RubikCamera.Position = new Point3D(cp.Z, -cp.Y, cp.X);
                    RubikCamera.UpDirection = new Vector3D(0, -1, 0);
                    break;
                case CubeSpins.DB:
                    RubikCamera.Position = new Point3D(cp.X, -cp.Y, -cp.Z);
                    RubikCamera.UpDirection = new Vector3D(0, -1, 0);
                    break;
                case CubeSpins.DL:
                    RubikCamera.Position = new Point3D(-cp.Z, -cp.Y, -cp.X);
                    RubikCamera.UpDirection = new Vector3D(0, -1, 0);
                    break;
                case CubeSpins.FU:
                    RubikCamera.Position = new Point3D(-cp.X, cp.Z, cp.Y);
                    RubikCamera.UpDirection = new Vector3D(0, 0, 1);
                    break;
                case CubeSpins.FR:
                    RubikCamera.Position = new Point3D(cp.Z, cp.X, cp.Y);
                    RubikCamera.UpDirection = new Vector3D(0, 0, 1);
                    break;
                case CubeSpins.FD:
                    RubikCamera.Position = new Point3D(cp.X, -cp.Z, cp.Y);
                    RubikCamera.UpDirection = new Vector3D(0, 0, 1);
                    break;
                case CubeSpins.FL:
                    RubikCamera.Position = new Point3D(-cp.Z, -cp.X, cp.Y);
                    RubikCamera.UpDirection = new Vector3D(0, 0, 1);
                    break;
                case CubeSpins.BU:
                    RubikCamera.Position = new Point3D(cp.X, cp.Z, -cp.Y);
                    RubikCamera.UpDirection = new Vector3D(0, 0, -1);
                    break;
                case CubeSpins.BR:
                    RubikCamera.Position = new Point3D(cp.Z, -cp.X, -cp.Y);
                    RubikCamera.UpDirection = new Vector3D(0, 0, -1);
                    break;
                case CubeSpins.BD:
                    RubikCamera.Position = new Point3D(-cp.X, -cp.Z, -cp.Y);
                    RubikCamera.UpDirection = new Vector3D(0, 0, -1);
                    break;
                case CubeSpins.BL:
                    RubikCamera.Position = new Point3D(-cp.Z, cp.X, -cp.Y);
                    RubikCamera.UpDirection = new Vector3D(0, 0, -1);
                    break;
                case CubeSpins.RU:
                    RubikCamera.Position = new Point3D(cp.Y, cp.Z, cp.X);
                    RubikCamera.UpDirection = new Vector3D(1, 0, 0);
                    break;
                case CubeSpins.RF:
                    RubikCamera.Position = new Point3D(cp.Y, -cp.X, cp.Z);
                    RubikCamera.UpDirection = new Vector3D(1, 0, 0);
                    break;
                case CubeSpins.RD:
                    RubikCamera.Position = new Point3D(cp.Y, -cp.Z, -cp.X);
                    RubikCamera.UpDirection = new Vector3D(1, 0, 0);
                    break;
                case CubeSpins.RB:
                    RubikCamera.Position = new Point3D(cp.Y, cp.X, -cp.Z);
                    RubikCamera.UpDirection = new Vector3D(1, 0, 0);
                    break;
                case CubeSpins.LU:
                    RubikCamera.Position = new Point3D(-cp.Y, cp.Z, -cp.X);
                    RubikCamera.UpDirection = new Vector3D(-1, 0, 0);
                    break;
                case CubeSpins.LF:
                    RubikCamera.Position = new Point3D(-cp.Y, cp.X, cp.Z);
                    RubikCamera.UpDirection = new Vector3D(-1, 0, 0);
                    break;
                case CubeSpins.LD:
                    RubikCamera.Position = new Point3D(-cp.Y, -cp.Z, cp.X);
                    RubikCamera.UpDirection = new Vector3D(-1, 0, 0);
                    break;
                case CubeSpins.LB:
                    RubikCamera.Position = new Point3D(-cp.Y, -cp.X, -cp.Z);
                    RubikCamera.UpDirection = new Vector3D(-1, 0, 0);
                    break;
            }

            // Specify the direction that the camera is pointing (looking to (0,0,0))
            RubikCamera.LookDirection = new Vector3D(-RubikCamera.Position.X, 
                                                     -RubikCamera.Position.Y,
                                                     -RubikCamera.Position.Z);

            // Asign the camera to the viewport
            RubikViewport3D.Camera = RubikCamera;
        }

        /// <summary>
        /// Renders the scene in a bitmap
        /// </summary>
        /// <param name="width">Bitmap width</param>
        /// <param name="height">Bitmap height</param>
        public void UpdateBitmap(int width, int height)
        {
            if (width <= 0) width = 1;
            if (height <= 0) height = 1;

            RubikViewport3D.Width = width;
            RubikViewport3D.Height = height;
            RubikViewport3D.Measure(new System.Windows.Size(width, height));
            RubikViewport3D.Arrange(new Rect(0, 0, width, height));
            renderBMP = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            SetCamera(NormalCameraPosition); // Update camera

            //Background
            BitmapBackground = new DrawingVisual();
            Content = BitmapBackground.RenderOpen();
            Content.DrawRectangle(AMSettings.BackgroundBrush, null, new Rect(0, 0, renderBMP.Width, renderBMP.Height));
            Content.Close();
            renderBMP.Render(BitmapBackground);
            renderBMP.Render(RubikViewport3D);
        }

        #region Movements

        /// <summary>
        /// Up layer movement n times
        /// </summary>
        /// <param name="n">Number of U movements</param>
        public void U(int n)
        {
            Cube.U(n); // Apply movement to the cube data
            UpdateMaterials(); // Change stickers colors based on cube data
        }

        /// <summary>
        /// Down layer movement n times
        /// </summary>
        /// <param name="n">Number of D movements</param>
        public void D(int n)
        {
            Cube.D(n); // Apply movement to the cube data
            UpdateMaterials(); // Change stickers colors based on cube data
        }

        /// <summary>
        /// Front layer movement n times
        /// </summary>
        /// <param name="n">Number of F movements</param>
        public void F(int n)
        {
            Cube.F(n); // Apply movement to the cube data
            UpdateMaterials(); // Change stickers colors based on cube data
        }

        /// <summary>
        /// Back layer movement n times
        /// </summary>
        /// <param name="n">Number of B movements</param>
        public void B(int n)
        {
            Cube.B(n); // Apply movement to the cube data
            UpdateMaterials(); // Change stickers colors based on cube data
        }

        /// <summary>
        /// Right layer movement n times
        /// </summary>
        /// <param name="n">Number of R movements</param>
        public void R(int n)
        {
            Cube.R(n); // Apply movement to the cube data
            UpdateMaterials(); // Change stickers colors based on cube data
        }

        /// <summary>
        /// Left layer movement n times
        /// </summary>
        /// <param name="n">Number of L movements</param>
        public void L(int n)
        {
            Cube.L(n); // Apply movement to the cube data
            UpdateMaterials(); // Change stickers colors based on cube data
        }

        /// <summary>
        /// X rotation n times
        /// </summary>
        /// <param name="n">Number of X rotations</param>
        public void x(int n)
        {
            Cube.x(n); // Apply movement to the cube data
            UpdateMaterials(); // Change stickers colors based on cube data
        }

        /// <summary>
        /// Y rotation n times
        /// </summary>
        /// <param name="n">Number of Y rotations</param>
        public void y(int n)
        {
            Cube.y(n); // Apply movement to the cube data
            UpdateMaterials(); // Change stickers colors based on cube data
        }

        /// <summary>
        /// Z rotation n times
        /// </summary>
        /// <param name="n">Number of Z rotations</param>
        public void z(int n)
        {
            Cube.z(n); // Apply movement to the cube data
            UpdateMaterials(); // Change stickers colors based on cube data
        }

        #endregion Movements

        /// <summary>
        /// Reset cube status to solved condition
        /// </summary>
        public void Reset()
        {
            Cube.Reset(); // Reset cube data
            UpdateMaterials(); // Change stickers colors based on cube data
        }

        /// <summary>
        /// Changes a face material
        /// </summary>
        /// <param name="F">Face to change material</param>
        public void ChangeMaterial(Faces F)
        {
            switch (F)
            {
                case Faces.U: RubikMaterials.U = new DiffuseMaterial(AMSettings.UBrush); break;
                case Faces.D: RubikMaterials.D = new DiffuseMaterial(AMSettings.DBrush); break;
                case Faces.F: RubikMaterials.F = new DiffuseMaterial(AMSettings.FBrush); break;
                case Faces.B: RubikMaterials.B = new DiffuseMaterial(AMSettings.BBrush); break;
                case Faces.R: RubikMaterials.R = new DiffuseMaterial(AMSettings.RBrush); break;
                case Faces.L: RubikMaterials.L = new DiffuseMaterial(AMSettings.LBrush); break;
            }
            UpdateMaterials();
        }

        /// <summary>
        /// Changes all face materials
        /// </summary>
        public void ChangeAllMaterials()
        {
            RubikMaterials.U = new DiffuseMaterial(AMSettings.UBrush);
            RubikMaterials.D = new DiffuseMaterial(AMSettings.DBrush);
            RubikMaterials.F = new DiffuseMaterial(AMSettings.FBrush);
            RubikMaterials.B = new DiffuseMaterial(AMSettings.BBrush);
            RubikMaterials.R = new DiffuseMaterial(AMSettings.RBrush);
            RubikMaterials.L = new DiffuseMaterial(AMSettings.LBrush);
            RubikMaterials.Neutral = new DiffuseMaterial(AMSettings.NeutralBrush);
            RubikMaterials.Base = new DiffuseMaterial(AMSettings.BaseBrush);

            UpdateMaterials();
        }

        /// <summary>
        /// Changes Neutral color and material
        /// </summary>
        /// <param name="c">New neutral color</param>
        public void ChangeNeutralColorMaterial(System.Windows.Media.Color c)
        {
            AMSettings.NeutralColor = c;
            RubikMaterials.Neutral = new DiffuseMaterial(AMSettings.NeutralBrush);
        }

        /// <summary>
        /// Changes base color and material
        /// </summary>
        /// <param name="c">New base color</param>
        public void ChangeBaseColorMaterial(System.Windows.Media.Color c)
        {
            AMSettings.BaseColor = c;
            RubikMaterials.Base = new DiffuseMaterial(AMSettings.BaseBrush);
        }

        /// <summary>
        /// Update material for each sticker
        /// </summary>
        public void UpdateMaterials()
        {
            for (int i = 0; i < Stickers3D.Length; i++)
            {
                StickerPositions St = Cube.GetStickerSolvedPosition((StickerPositions)i);
                if (NeutralStickers[(int)St])
                    Stickers3D[i].SetStickerMaterials(RubikMaterials.Neutral, RubikMaterials.Base);
                else
                    switch (Cube.GetStickerFacePosition((StickerPositions)i))
                    {
                        case Faces.U: Stickers3D[i].SetStickerMaterials(RubikMaterials.U, RubikMaterials.Base); break;
                        case Faces.D: Stickers3D[i].SetStickerMaterials(RubikMaterials.D, RubikMaterials.Base); break;
                        case Faces.F: Stickers3D[i].SetStickerMaterials(RubikMaterials.F, RubikMaterials.Base); break;
                        case Faces.B: Stickers3D[i].SetStickerMaterials(RubikMaterials.B, RubikMaterials.Base); break;
                        case Faces.R: Stickers3D[i].SetStickerMaterials(RubikMaterials.R, RubikMaterials.Base); break;
                        case Faces.L: Stickers3D[i].SetStickerMaterials(RubikMaterials.L, RubikMaterials.Base); break;
                    }
            }
        }

        /// <summary>
        /// Gets the color of a sticker
        /// </summary>
        /// <param name="Pos">Sticker position</param>
        /// <returns>Sticker color</returns>
        public System.Drawing.Color GetColor(StickerPositions Pos)
        {
            System.Windows.Media.Color AuxColor = new System.Windows.Media.Color();

            switch (Cube.GetStickerFacePosition(Pos))
            {
                case Faces.U: AuxColor = AMSettings.UColor; break;
                case Faces.D: AuxColor = AMSettings.DColor; break;
                case Faces.F: AuxColor = AMSettings.FColor; break;
                case Faces.B: AuxColor = AMSettings.BColor; break;
                case Faces.R: AuxColor = AMSettings.RColor; break;
                case Faces.L: AuxColor = AMSettings.LColor; break;
            }
            return System.Drawing.Color.FromArgb(AuxColor.A, AuxColor.R, AuxColor.G, AuxColor.B);
        }

        #region 3D Rotations

        /// <summary>
        /// Rotate U layer stickers
        /// </summary>
        /// <param name="angle">Angle for rotation</param>
        public void RotateU(double angle)
        {
            switch (Cube.Spin)
            {
                case CubeSpins.UF:
                case CubeSpins.UR:
                case CubeSpins.UB:
                case CubeSpins.UL:
                    foreach (StickersULayer S in Enum.GetValues(typeof(StickersULayer))) Stickers3D[(int)S].RotateY(-angle);
                    break;
                case CubeSpins.DF:
                case CubeSpins.DR:
                case CubeSpins.DB:
                case CubeSpins.DL:
                    foreach (StickersDLayer S in Enum.GetValues(typeof(StickersDLayer))) Stickers3D[(int)S].RotateY(angle);
                    break;
                case CubeSpins.FU:
                case CubeSpins.FR:
                case CubeSpins.FD:
                case CubeSpins.FL:
                    foreach (StickersFLayer S in Enum.GetValues(typeof(StickersFLayer))) Stickers3D[(int)S].RotateZ(-angle);
                    break;
                case CubeSpins.BU:
                case CubeSpins.BR:
                case CubeSpins.BD:
                case CubeSpins.BL:
                    foreach (StickersBLayer S in Enum.GetValues(typeof(StickersBLayer))) Stickers3D[(int)S].RotateZ(angle);
                    break;
                case CubeSpins.RU:
                case CubeSpins.RF:
                case CubeSpins.RD:
                case CubeSpins.RB:
                    foreach (StickersRLayer S in Enum.GetValues(typeof(StickersRLayer))) Stickers3D[(int)S].RotateX(-angle);
                    break;
                case CubeSpins.LU:
                case CubeSpins.LF:
                case CubeSpins.LD:
                case CubeSpins.LB:
                    foreach (StickersLLayer S in Enum.GetValues(typeof(StickersLLayer))) Stickers3D[(int)S].RotateX(angle);
                    break;
            }
        }

        /// <summary>
        /// Rotate D layer stickers
        /// </summary>
        /// <param name="angle">Angle for rotation</param>
        public void RotateD(double angle)
        {
            switch (Cube.Spin)
            {
                case CubeSpins.UF:
                case CubeSpins.UR:
                case CubeSpins.UB:
                case CubeSpins.UL:
                    foreach (StickersDLayer S in Enum.GetValues(typeof(StickersDLayer))) Stickers3D[(int)S].RotateY(angle);
                    break;
                case CubeSpins.DF:
                case CubeSpins.DR:
                case CubeSpins.DB:
                case CubeSpins.DL:
                    foreach (StickersULayer S in Enum.GetValues(typeof(StickersULayer))) Stickers3D[(int)S].RotateY(-angle);
                    break;
                case CubeSpins.FU:
                case CubeSpins.FR:
                case CubeSpins.FD:
                case CubeSpins.FL:
                    foreach (StickersBLayer S in Enum.GetValues(typeof(StickersBLayer))) Stickers3D[(int)S].RotateZ(angle);
                    break;
                case CubeSpins.BU:
                case CubeSpins.BR:
                case CubeSpins.BD:
                case CubeSpins.BL:
                    foreach (StickersFLayer S in Enum.GetValues(typeof(StickersFLayer))) Stickers3D[(int)S].RotateZ(-angle);
                    break;
                case CubeSpins.RU:
                case CubeSpins.RF:
                case CubeSpins.RD:
                case CubeSpins.RB:
                    foreach (StickersLLayer S in Enum.GetValues(typeof(StickersLLayer))) Stickers3D[(int)S].RotateX(angle);
                    break;
                case CubeSpins.LU:
                case CubeSpins.LF:
                case CubeSpins.LD:
                case CubeSpins.LB:
                    foreach (StickersRLayer S in Enum.GetValues(typeof(StickersRLayer))) Stickers3D[(int)S].RotateX(-angle);
                    break;
            }
        }

        /// <summary>
        /// Rotate F layer stickers
        /// </summary>
        /// <param name="angle">Angle for rotation</param>
        public void RotateF(double angle)
        {
            switch (Cube.Spin)
            {
                case CubeSpins.UF:
                case CubeSpins.RF:
                case CubeSpins.DF:
                case CubeSpins.LF:
                    foreach (StickersFLayer S in Enum.GetValues(typeof(StickersFLayer))) Stickers3D[(int)S].RotateZ(-angle);
                    break;
                case CubeSpins.UB:
                case CubeSpins.RB:
                case CubeSpins.DB:
                case CubeSpins.LB:
                    foreach (StickersBLayer S in Enum.GetValues(typeof(StickersBLayer))) Stickers3D[(int)S].RotateZ(angle);
                    break;
                case CubeSpins.FU:
                case CubeSpins.RU:
                case CubeSpins.BU:
                case CubeSpins.LU:
                    foreach (StickersULayer S in Enum.GetValues(typeof(StickersULayer))) Stickers3D[(int)S].RotateY(-angle);
                    break;
                case CubeSpins.FD:
                case CubeSpins.RD:
                case CubeSpins.BD:
                case CubeSpins.LD:
                    foreach (StickersDLayer S in Enum.GetValues(typeof(StickersDLayer))) Stickers3D[(int)S].RotateY(angle);
                    break;
                case CubeSpins.UR:
                case CubeSpins.FR:
                case CubeSpins.DR:
                case CubeSpins.BR:
                    foreach (StickersRLayer S in Enum.GetValues(typeof(StickersRLayer))) Stickers3D[(int)S].RotateX(-angle);
                    break;
                case CubeSpins.UL:
                case CubeSpins.FL:
                case CubeSpins.DL:
                case CubeSpins.BL:
                    foreach (StickersLLayer S in Enum.GetValues(typeof(StickersLLayer))) Stickers3D[(int)S].RotateX(angle);
                    break;
            }
        }

        /// <summary>
        /// Rotate B layer stickers
        /// </summary>
        /// <param name="angle">Angle for rotation</param>
        public void RotateB(double angle)
        {
            switch (Cube.Spin)
            {
                case CubeSpins.UF:
                case CubeSpins.RF:
                case CubeSpins.DF:
                case CubeSpins.LF:
                    foreach (StickersBLayer S in Enum.GetValues(typeof(StickersBLayer))) Stickers3D[(int)S].RotateZ(angle);
                    break;
                case CubeSpins.UB:
                case CubeSpins.RB:
                case CubeSpins.DB:
                case CubeSpins.LB:
                    foreach (StickersFLayer S in Enum.GetValues(typeof(StickersFLayer))) Stickers3D[(int)S].RotateZ(-angle);
                    break;
                case CubeSpins.FU:
                case CubeSpins.RU:
                case CubeSpins.BU:
                case CubeSpins.LU:
                    foreach (StickersDLayer S in Enum.GetValues(typeof(StickersDLayer))) Stickers3D[(int)S].RotateY(angle);
                    break;
                case CubeSpins.FD:
                case CubeSpins.RD:
                case CubeSpins.BD:
                case CubeSpins.LD:
                    foreach (StickersULayer S in Enum.GetValues(typeof(StickersULayer))) Stickers3D[(int)S].RotateY(-angle);
                    break;
                case CubeSpins.UR:
                case CubeSpins.FR:
                case CubeSpins.DR:
                case CubeSpins.BR:
                    foreach (StickersLLayer S in Enum.GetValues(typeof(StickersLLayer))) Stickers3D[(int)S].RotateX(angle);
                    break;
                case CubeSpins.UL:
                case CubeSpins.FL:
                case CubeSpins.DL:
                case CubeSpins.BL:
                    foreach (StickersRLayer S in Enum.GetValues(typeof(StickersRLayer))) Stickers3D[(int)S].RotateX(-angle);
                    break;
            }
        }

        /// <summary>
        /// Rotate R layer stickers
        /// </summary>
        /// <param name="angle">Angle for rotation</param>
        public void RotateR(double angle)
        {
            switch (Cube.Spin)
            {
                case CubeSpins.UF:
                case CubeSpins.FD:
                case CubeSpins.DB:
                case CubeSpins.BU:
                    foreach (StickersRLayer S in Enum.GetValues(typeof(StickersRLayer))) Stickers3D[(int)S].RotateX(-angle);
                    break;
                case CubeSpins.DF:
                case CubeSpins.FU:
                case CubeSpins.UB:
                case CubeSpins.BD:
                    foreach (StickersLLayer S in Enum.GetValues(typeof(StickersLLayer))) Stickers3D[(int)S].RotateX(angle);
                    break;
                case CubeSpins.LF:
                case CubeSpins.FR:
                case CubeSpins.RB:
                case CubeSpins.BL:
                    foreach (StickersULayer S in Enum.GetValues(typeof(StickersULayer))) Stickers3D[(int)S].RotateY(-angle);
                    break;
                case CubeSpins.RF:
                case CubeSpins.FL:
                case CubeSpins.LB:
                case CubeSpins.BR:
                    foreach (StickersDLayer S in Enum.GetValues(typeof(StickersDLayer))) Stickers3D[(int)S].RotateY(angle);
                    break;
                case CubeSpins.UL:
                case CubeSpins.LD:
                case CubeSpins.DR:
                case CubeSpins.RU:
                    foreach (StickersFLayer S in Enum.GetValues(typeof(StickersFLayer))) Stickers3D[(int)S].RotateZ(-angle);
                    break;
                case CubeSpins.UR:
                case CubeSpins.RD:
                case CubeSpins.DL:
                case CubeSpins.LU:
                    foreach (StickersBLayer S in Enum.GetValues(typeof(StickersBLayer))) Stickers3D[(int)S].RotateZ(angle);
                    break;
            }
        }

        /// <summary>
        /// Rotate L layer stickers
        /// </summary>
        /// <param name="angle">Angle for rotation</param>
        public void RotateL(double angle)
        {
            switch (Cube.Spin)
            {
                case CubeSpins.UF:
                case CubeSpins.FD:
                case CubeSpins.DB:
                case CubeSpins.BU:
                    foreach (StickersLLayer S in Enum.GetValues(typeof(StickersLLayer))) Stickers3D[(int)S].RotateX(angle);
                    break;
                case CubeSpins.UB:
                case CubeSpins.BD:
                case CubeSpins.DF:
                case CubeSpins.FU:
                    foreach (StickersRLayer S in Enum.GetValues(typeof(StickersRLayer))) Stickers3D[(int)S].RotateX(-angle);
                    break;
                case CubeSpins.LF:
                case CubeSpins.FR:
                case CubeSpins.RB:
                case CubeSpins.BL:
                    foreach (StickersDLayer S in Enum.GetValues(typeof(StickersDLayer))) Stickers3D[(int)S].RotateY(angle);
                    break;
                case CubeSpins.RF:
                case CubeSpins.FL:
                case CubeSpins.LB:
                case CubeSpins.BR:
                    foreach (StickersULayer S in Enum.GetValues(typeof(StickersULayer))) Stickers3D[(int)S].RotateY(-angle);
                    break;
                case CubeSpins.UL:
                case CubeSpins.LD:
                case CubeSpins.DR:
                case CubeSpins.RU:
                    foreach (StickersBLayer S in Enum.GetValues(typeof(StickersBLayer))) Stickers3D[(int)S].RotateZ(angle);
                    break;
                case CubeSpins.UR:
                case CubeSpins.RD:
                case CubeSpins.DL:
                case CubeSpins.LU:
                    foreach (StickersFLayer S in Enum.GetValues(typeof(StickersFLayer))) Stickers3D[(int)S].RotateZ(-angle);
                    break;
            }
        }

        /// <summary>
        /// Rotate E layer stickers
        /// </summary>
        /// <param name="angle">Angle for rotation</param>
        public void RotateE(double angle)
        {
            switch (Cube.Spin)
            {
                case CubeSpins.UF:
                case CubeSpins.UR:
                case CubeSpins.UB:
                case CubeSpins.UL:
                    foreach (StickersELayer S in Enum.GetValues(typeof(StickersELayer))) Stickers3D[(int)S].RotateY(-angle);
                    break;
                case CubeSpins.DF:
                case CubeSpins.DR:
                case CubeSpins.DB:
                case CubeSpins.DL:
                    foreach (StickersELayer S in Enum.GetValues(typeof(StickersELayer))) Stickers3D[(int)S].RotateY(angle);
                    break;
                case CubeSpins.FU:
                case CubeSpins.FR:
                case CubeSpins.FD:
                case CubeSpins.FL:
                    foreach (StickersSLayer S in Enum.GetValues(typeof(StickersSLayer))) Stickers3D[(int)S].RotateZ(-angle);
                    break;
                case CubeSpins.BU:
                case CubeSpins.BR:
                case CubeSpins.BD:
                case CubeSpins.BL:
                    foreach (StickersSLayer S in Enum.GetValues(typeof(StickersSLayer))) Stickers3D[(int)S].RotateZ(angle);
                    break;
                case CubeSpins.RU:
                case CubeSpins.RF:
                case CubeSpins.RD:
                case CubeSpins.RB:
                    foreach (StickersMLayer S in Enum.GetValues(typeof(StickersMLayer))) Stickers3D[(int)S].RotateX(-angle);
                    break;
                case CubeSpins.LU:
                case CubeSpins.LF:
                case CubeSpins.LD:
                case CubeSpins.LB:
                    foreach (StickersMLayer S in Enum.GetValues(typeof(StickersMLayer))) Stickers3D[(int)S].RotateX(angle);
                    break;
            }
        }

        /// <summary>
        /// Rotate S layer stickers
        /// </summary>
        /// <param name="angle">Angle for rotation</param>
        public void RotateS(double angle)
        {
            switch (Cube.Spin)
            {
                case CubeSpins.UF:
                case CubeSpins.RF:
                case CubeSpins.DF:
                case CubeSpins.LF:
                    foreach (StickersSLayer S in Enum.GetValues(typeof(StickersSLayer))) Stickers3D[(int)S].RotateZ(-angle);
                    break;
                case CubeSpins.UB:
                case CubeSpins.RB:
                case CubeSpins.DB:
                case CubeSpins.LB:
                    foreach (StickersSLayer S in Enum.GetValues(typeof(StickersSLayer))) Stickers3D[(int)S].RotateZ(angle);
                    break;
                case CubeSpins.FU:
                case CubeSpins.RU:
                case CubeSpins.BU:
                case CubeSpins.LU:
                    foreach (StickersELayer S in Enum.GetValues(typeof(StickersELayer))) Stickers3D[(int)S].RotateY(-angle);
                    break;
                case CubeSpins.FD:
                case CubeSpins.RD:
                case CubeSpins.BD:
                case CubeSpins.LD:
                    foreach (StickersELayer S in Enum.GetValues(typeof(StickersELayer))) Stickers3D[(int)S].RotateY(angle);
                    break;
                case CubeSpins.UR:
                case CubeSpins.FR:
                case CubeSpins.DR:
                case CubeSpins.BR:
                    foreach (StickersMLayer S in Enum.GetValues(typeof(StickersMLayer))) Stickers3D[(int)S].RotateX(-angle);
                    break;
                case CubeSpins.UL:
                case CubeSpins.FL:
                case CubeSpins.DL:
                case CubeSpins.BL:
                    foreach (StickersMLayer S in Enum.GetValues(typeof(StickersMLayer))) Stickers3D[(int)S].RotateX(angle);
                    break;
            }
        }

        /// <summary>
        /// Rotate M layer stickers
        /// </summary>
        /// <param name="angle">Angle for rotation</param>
        public void RotateM(double angle)
        {
            switch (Cube.Spin)
            {
                case CubeSpins.UF:
                case CubeSpins.FD:
                case CubeSpins.DB:
                case CubeSpins.BU:
                    foreach (StickersMLayer S in Enum.GetValues(typeof(StickersMLayer))) Stickers3D[(int)S].RotateX(-angle);
                    break;
                case CubeSpins.DF:
                case CubeSpins.FU:
                case CubeSpins.UB:
                case CubeSpins.BD:
                    foreach (StickersMLayer S in Enum.GetValues(typeof(StickersMLayer))) Stickers3D[(int)S].RotateX(angle);
                    break;
                case CubeSpins.UL:
                case CubeSpins.LD:
                case CubeSpins.DR:
                case CubeSpins.RU:
                    foreach (StickersSLayer S in Enum.GetValues(typeof(StickersSLayer))) Stickers3D[(int)S].RotateZ(-angle);
                    break;
                case CubeSpins.UR:
                case CubeSpins.RD:
                case CubeSpins.DL:
                case CubeSpins.LU:
                    foreach (StickersSLayer S in Enum.GetValues(typeof(StickersSLayer))) Stickers3D[(int)S].RotateZ(angle);
                    break;
                case CubeSpins.FR:
                case CubeSpins.RB:
                case CubeSpins.BL:
                case CubeSpins.LF:
                    foreach (StickersELayer S in Enum.GetValues(typeof(StickersELayer))) Stickers3D[(int)S].RotateY(-angle);
                    break;
                case CubeSpins.FL:
                case CubeSpins.LB:
                case CubeSpins.BR:
                case CubeSpins.RF:
                    foreach (StickersELayer S in Enum.GetValues(typeof(StickersELayer))) Stickers3D[(int)S].RotateY(angle);
                    break;
            }
        }

        /// <summary>
        /// Rotate X layer stickers
        /// </summary>
        /// <param name="angle">Angle for rotation</param>
        public void RotateX(double angle)
        {
            switch (Cube.Spin)
            {
                case CubeSpins.UF:
                case CubeSpins.FD:
                case CubeSpins.DB:
                case CubeSpins.BU:
                    foreach (StickerPositions S in Enum.GetValues(typeof(StickerPositions))) Stickers3D[(int)S].RotateX(-angle);
                    break;
                case CubeSpins.DF:
                case CubeSpins.FU:
                case CubeSpins.UB:
                case CubeSpins.BD:
                    foreach (StickerPositions S in Enum.GetValues(typeof(StickerPositions))) Stickers3D[(int)S].RotateX(angle);
                    break;
                case CubeSpins.LF:
                case CubeSpins.FR:
                case CubeSpins.RB:
                case CubeSpins.BL:
                    foreach (StickerPositions S in Enum.GetValues(typeof(StickerPositions))) Stickers3D[(int)S].RotateY(-angle);
                    break;
                case CubeSpins.FL:
                case CubeSpins.LB:
                case CubeSpins.BR:
                case CubeSpins.RF:
                    foreach (StickerPositions S in Enum.GetValues(typeof(StickerPositions))) Stickers3D[(int)S].RotateY(angle);
                    break;
                case CubeSpins.UL:
                case CubeSpins.LD:
                case CubeSpins.DR:
                case CubeSpins.RU:
                    foreach (StickerPositions S in Enum.GetValues(typeof(StickerPositions))) Stickers3D[(int)S].RotateZ(-angle);
                    break;
                case CubeSpins.LU:
                case CubeSpins.UR:
                case CubeSpins.RD:
                case CubeSpins.DL:
                    foreach (StickerPositions S in Enum.GetValues(typeof(StickerPositions))) Stickers3D[(int)S].RotateZ(angle);
                    break;
            }
        }

        /// <summary>
        /// Rotate Y layer stickers
        /// </summary>
        /// <param name="angle">Angle for rotation</param>
        public void RotateY(double angle)
        {
            switch (Cube.Spin)
            {
                case CubeSpins.UF:
                case CubeSpins.UR:
                case CubeSpins.UB:
                case CubeSpins.UL:
                    foreach (StickerPositions S in Enum.GetValues(typeof(StickerPositions))) Stickers3D[(int)S].RotateY(-angle);
                    break;
                case CubeSpins.DF:
                case CubeSpins.DR:
                case CubeSpins.DB:
                case CubeSpins.DL:
                    foreach (StickerPositions S in Enum.GetValues(typeof(StickerPositions))) Stickers3D[(int)S].RotateY(angle);
                    break;
                case CubeSpins.FU:
                case CubeSpins.FR:
                case CubeSpins.FD:
                case CubeSpins.FL:
                    foreach (StickerPositions S in Enum.GetValues(typeof(StickerPositions))) Stickers3D[(int)S].RotateZ(-angle);
                    break;
                case CubeSpins.BU:
                case CubeSpins.BR:
                case CubeSpins.BD:
                case CubeSpins.BL:
                    foreach (StickerPositions S in Enum.GetValues(typeof(StickerPositions))) Stickers3D[(int)S].RotateZ(angle);
                    break;
                case CubeSpins.RU:
                case CubeSpins.RF:
                case CubeSpins.RD:
                case CubeSpins.RB:
                    foreach (StickerPositions S in Enum.GetValues(typeof(StickerPositions))) Stickers3D[(int)S].RotateX(-angle);
                    break;
                case CubeSpins.LU:
                case CubeSpins.LF:
                case CubeSpins.LD:
                case CubeSpins.LB:
                    foreach (StickerPositions S in Enum.GetValues(typeof(StickerPositions))) Stickers3D[(int)S].RotateX(angle);
                    break;
            }
        }

        /// <summary>
        /// Rotate Z layer stickers
        /// </summary>
        /// <param name="angle">Angle for rotation</param>
        public void RotateZ(double angle)
        {
            switch (Cube.Spin)
            {
                case CubeSpins.UF:
                case CubeSpins.RF:
                case CubeSpins.DF:
                case CubeSpins.LF:
                    foreach (StickerPositions S in Enum.GetValues(typeof(StickerPositions))) Stickers3D[(int)S].RotateZ(-angle);
                    break;
                case CubeSpins.UB:
                case CubeSpins.RB:
                case CubeSpins.DB:
                case CubeSpins.LB:
                    foreach (StickerPositions S in Enum.GetValues(typeof(StickerPositions))) Stickers3D[(int)S].RotateZ(angle);
                    break;
                case CubeSpins.UR:
                case CubeSpins.FR:
                case CubeSpins.DR:
                case CubeSpins.BR:
                    foreach (StickerPositions S in Enum.GetValues(typeof(StickerPositions))) Stickers3D[(int)S].RotateX(-angle);
                    break;
                case CubeSpins.UL:
                case CubeSpins.FL:
                case CubeSpins.DL:
                case CubeSpins.BL:
                    foreach (StickerPositions S in Enum.GetValues(typeof(StickerPositions))) Stickers3D[(int)S].RotateX(angle);
                    break;
                case CubeSpins.FU:
                case CubeSpins.RU:
                case CubeSpins.BU:
                case CubeSpins.LU:
                    foreach (StickerPositions S in Enum.GetValues(typeof(StickerPositions))) Stickers3D[(int)S].RotateY(-angle);
                    break;
                case CubeSpins.FD:
                case CubeSpins.RD:
                case CubeSpins.BD:
                case CubeSpins.LD:
                    foreach (StickerPositions S in Enum.GetValues(typeof(StickerPositions))) Stickers3D[(int)S].RotateY(angle);
                    break;
            }
        }

        #endregion 3D Rotations

        /// <summary>
        /// Apply scramble (add)
        /// </summary>
        /// <param name="SD">Scramble to apply</param>
        public void ApplyScramble(Scramble SD)
        {
            Cube.ApplyScramble(SD);
            UpdateMaterials(); // Change stickers colors based on cube data
        }

        /// <summary>
        /// Apply scramble step
        /// </summary>
        /// <param name="SD">Scramble step to apply</param>
        public void ApplyStep(Steps SS)
        {
            Cube.ApplyScrambleStep(SS);
            UpdateMaterials();
        }

        /// <summary>
        /// Rotate cube to zero position
        /// </summary>
        public void RotateToZero()
        {
            RotateU(0);
            RotateE(0);
            RotateD(0);

            RotateF(0);
            RotateS(0);
            RotateB(0);

            RotateR(0);
            RotateM(0);
            RotateL(0);
        }

        /// <summary>
        /// Rotate the cube in the given step in a porcentage
        /// </summary>
        /// <param name="step">Step to animate</param>
        /// <param name="porc">Rotation porcentage (0 to 1)</param>
        public void RotateStepPorcentage(Steps step, double porc)
        {
            if (!ScrambleStep.IsTurnOrMovement(step)) return;
            if (porc < 0.0) porc = 0.0;
            else if (porc > 1.0) porc = 1.0;

            double Angle = (double)ScrambleStep.Movement(step) * porc;

            if (ScrambleStep.Modifier(step) == Modifiers.FULL_CUBE)
            {
                switch (ScrambleStep.Layer(step))
                {
                    case Layers.R: RotateX(Angle); break;
                    case Layers.U: RotateY(Angle); break;
                    case Layers.F: RotateZ(Angle); break;
                }
            }
            else
            {
                switch (ScrambleStep.Layer(step))
                {
                    case Layers.U: RotateU(Angle); break;
                    case Layers.D: RotateD(Angle); break;
                    case Layers.F: RotateF(Angle); break;
                    case Layers.B: RotateB(Angle); break;
                    case Layers.R: RotateR(Angle); break;
                    case Layers.L: RotateL(Angle); break;
                    case Layers.E:
                        if (AMSettings.RotationOfEAsU) RotateE(Angle);
                        else RotateE(-Angle);
                        break;
                    case Layers.S:
                        if (AMSettings.RotationOfSAsF) RotateS(Angle);
                        else RotateS(-Angle);
                        break;
                    case Layers.M:
                        if (AMSettings.RotationOfMAsR) RotateM(Angle);
                        else RotateM(-Angle);
                        break;
                }

                switch (ScrambleStep.Modifier(step))
                {
                    case Modifiers.DOUBLE_ADJACENT_LAYERS_SAME_DIRECTION:
                        switch (ScrambleStep.Layer(step))
                        {
                            case Layers.U: RotateE(Angle); break;
                            case Layers.D: RotateE(-Angle); break;
                            case Layers.F: RotateS(Angle); break;
                            case Layers.B: RotateS(-Angle); break;
                            case Layers.R: RotateM(Angle); break;
                            case Layers.L: RotateM(-Angle); break;
                        }
                        break;
                    case Modifiers.DOUBLE_ADJACENT_LAYERS_OPPOSITE_DIRECTION:
                        switch (ScrambleStep.Layer(step))
                        {
                            case Layers.U: RotateE(-Angle); break;
                            case Layers.D: RotateE(Angle); break;
                            case Layers.F: RotateS(-Angle); break;
                            case Layers.B: RotateS(Angle); break;
                            case Layers.R: RotateM(-Angle); break;
                            case Layers.L: RotateM(Angle); break;
                        }
                        break;
                    case Modifiers.DOUBLE_OPPOSITE_LAYERS_SAME_DIRECTION:
                        switch (ScrambleStep.Layer(step))
                        {
                            case Layers.U: RotateD(-Angle); break;
                            case Layers.D: RotateU(-Angle); break;
                            case Layers.F: RotateB(-Angle); break;
                            case Layers.B: RotateF(-Angle); break;
                            case Layers.R: RotateL(-Angle); break;
                            case Layers.L: RotateR(-Angle); break;
                        }
                        break;
                    case Modifiers.DOUBLE_OPPOSITE_LAYERS_OPPOSITE_DIRECTION:
                        switch (ScrambleStep.Layer(step))
                        {
                            case Layers.U: RotateD(Angle); break;
                            case Layers.D: RotateU(Angle); break;
                            case Layers.F: RotateB(Angle); break;
                            case Layers.B: RotateF(Angle); break;
                            case Layers.R: RotateL(Angle); break;
                            case Layers.L: RotateR(Angle); break;
                        }
                        break;
                    default: break;
                }
            }
        }

        /// <summary>
        /// Set neutral stickers for all pieces except given ones
        /// </summary>
        /// <param name="pieces">Array of pieces</param>
        public void SetVisiblePieces(Pieces[] pieces)
        {
            if (pieces == null)
            {
                NeutralMask = NoNeutralMask;
                foreach (StickerPositions sp in Enum.GetValues(typeof(StickerPositions)))
                    NeutralStickers[(int)sp] = false;
            }
            else
            {
                NeutralMask = NoCentersNeutralMask;
                foreach (StickerPositions sp in Enum.GetValues(typeof(StickerPositions)))
                    for (int n = 0; n < pieces.Length; n++)
                        if (StickerData.GetStickerPiece(sp) == pieces[n]) NeutralStickers[(int)sp] = false;
            }
        }

        /// <summary>
        /// Convert degrees to radians
        /// </summary>
        /// <param name="angle">Angle in radians</param>
        /// <returns>Angle in degrees</returns>
        private static double DegreesToRadians(double angle) => Math.PI * angle / 180.0;

        /// <summary>
        /// Convert radians to degrees
        /// </summary>
        /// <param name="angle">Angle in degrees</param>
        /// <returns>Angle in radians</returns>
        private static double RadiansToDegrees(double angle) => angle * 180.0 / Math.PI;

        /// <summary>
        /// Rotates neutral mask with given turn
        /// </summary>
        /// <param name="S">Turn (x, y, z2, ...)</param>
        /// <returns>False if the given step is not a turn</returns>
        public bool RotateNeutral(Steps S)
        {
            if (!ScrambleStep.IsTurn(S)) return false;

            bool[] RotatedNeutralStickers = new bool[NeutralStickers.Length];

            switch (S)
            {
                case Steps.x:
                    for (int i = 0; i < NeutralStickers.Length; i++)
                        RotatedNeutralStickers[i] = NeutralStickers[(int)StickerData.RotateX((StickerPositions)i)];
                    break;

                case Steps.y:
                    for (int i = 0; i < NeutralStickers.Length; i++)
                        RotatedNeutralStickers[i] = NeutralStickers[(int)StickerData.RotateY((StickerPositions)i)];
                    break;

                case Steps.z:
                    for (int i = 0; i < NeutralStickers.Length; i++)
                        RotatedNeutralStickers[i] = NeutralStickers[(int)StickerData.RotateZ((StickerPositions)i)];
                    break;

                default: return false;
            }

            NeutralStickers = RotatedNeutralStickers;

            return true;
        }
    }
}
