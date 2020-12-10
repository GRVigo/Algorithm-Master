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

namespace Algorithm_Master
{
    /// <summary>
    /// Class to store a 3x3x3 Rubik's cube status.
    /// </summary>
    public class CubeData
    {
        #region Static arrays

        /// <summary>
        /// Array to follow spins when a cube has a turn. 
        /// 24x9 elements array; Files: Spins, Columns: x x2 xp y y2 yp z z2 zp
        /// </summary>
        private static readonly CubeSpins[,] SpinsArray =
        {
            { CubeSpins.FD, CubeSpins.DB, CubeSpins.BU, CubeSpins.UR, CubeSpins.UB, CubeSpins.UL, CubeSpins.LF, CubeSpins.DF, CubeSpins.RF }, // UF
            { CubeSpins.RD, CubeSpins.DL, CubeSpins.LU, CubeSpins.UB, CubeSpins.UL, CubeSpins.UF, CubeSpins.FR, CubeSpins.DR, CubeSpins.BR }, // UR
            { CubeSpins.BD, CubeSpins.DF, CubeSpins.FU, CubeSpins.UL, CubeSpins.UF, CubeSpins.UR, CubeSpins.RB, CubeSpins.DB, CubeSpins.LB }, // UB
            { CubeSpins.LD, CubeSpins.DR, CubeSpins.RU, CubeSpins.UF, CubeSpins.UR, CubeSpins.UB, CubeSpins.BL, CubeSpins.DL, CubeSpins.FL }, // UL
            { CubeSpins.FU, CubeSpins.UB, CubeSpins.BD, CubeSpins.DL, CubeSpins.DB, CubeSpins.DR, CubeSpins.RF, CubeSpins.UF, CubeSpins.LF }, // DF
            { CubeSpins.RU, CubeSpins.UL, CubeSpins.LD, CubeSpins.DF, CubeSpins.DL, CubeSpins.DB, CubeSpins.BR, CubeSpins.UR, CubeSpins.FR }, // DR
            { CubeSpins.BU, CubeSpins.UF, CubeSpins.FD, CubeSpins.DR, CubeSpins.DF, CubeSpins.DL, CubeSpins.LB, CubeSpins.UB, CubeSpins.RB }, // DB
            { CubeSpins.LU, CubeSpins.UR, CubeSpins.RD, CubeSpins.DB, CubeSpins.DR, CubeSpins.DF, CubeSpins.FL, CubeSpins.UL, CubeSpins.BL }, // DL
            { CubeSpins.UB, CubeSpins.BD, CubeSpins.DF, CubeSpins.FL, CubeSpins.FD, CubeSpins.FR, CubeSpins.RU, CubeSpins.BU, CubeSpins.LU }, // FU
            { CubeSpins.RB, CubeSpins.BL, CubeSpins.LF, CubeSpins.FU, CubeSpins.FL, CubeSpins.FD, CubeSpins.DR, CubeSpins.BR, CubeSpins.UR }, // FR
            { CubeSpins.DB, CubeSpins.BU, CubeSpins.UF, CubeSpins.FR, CubeSpins.FU, CubeSpins.FL, CubeSpins.LD, CubeSpins.BD, CubeSpins.RD }, // FD
            { CubeSpins.LB, CubeSpins.BR, CubeSpins.RF, CubeSpins.FD, CubeSpins.FR, CubeSpins.FU, CubeSpins.UL, CubeSpins.BL, CubeSpins.DL }, // FL
            { CubeSpins.UF, CubeSpins.FD, CubeSpins.DB, CubeSpins.BR, CubeSpins.BD, CubeSpins.BL, CubeSpins.LU, CubeSpins.FU, CubeSpins.RU }, // BU
            { CubeSpins.RF, CubeSpins.FL, CubeSpins.LB, CubeSpins.BD, CubeSpins.BL, CubeSpins.BU, CubeSpins.UR, CubeSpins.FR, CubeSpins.DR }, // BR
            { CubeSpins.DF, CubeSpins.FU, CubeSpins.UB, CubeSpins.BL, CubeSpins.BU, CubeSpins.BR, CubeSpins.RD, CubeSpins.FD, CubeSpins.LD }, // BD
            { CubeSpins.LF, CubeSpins.FR, CubeSpins.RB, CubeSpins.BU, CubeSpins.BR, CubeSpins.BD, CubeSpins.DL, CubeSpins.FL, CubeSpins.UL }, // BL
            { CubeSpins.UL, CubeSpins.LD, CubeSpins.DR, CubeSpins.RF, CubeSpins.RU, CubeSpins.RB, CubeSpins.BU, CubeSpins.LU, CubeSpins.FU }, // RU
            { CubeSpins.FL, CubeSpins.LB, CubeSpins.BR, CubeSpins.RD, CubeSpins.RB, CubeSpins.RU, CubeSpins.UF, CubeSpins.LF, CubeSpins.DF }, // RF
            { CubeSpins.DL, CubeSpins.LU, CubeSpins.UR, CubeSpins.RB, CubeSpins.RD, CubeSpins.RF, CubeSpins.FD, CubeSpins.LD, CubeSpins.BD }, // RD
            { CubeSpins.BL, CubeSpins.LF, CubeSpins.FR, CubeSpins.RU, CubeSpins.RF, CubeSpins.RD, CubeSpins.DB, CubeSpins.LB, CubeSpins.UB }, // RB
            { CubeSpins.UR, CubeSpins.RD, CubeSpins.DL, CubeSpins.LB, CubeSpins.LD, CubeSpins.LF, CubeSpins.FU, CubeSpins.RU, CubeSpins.BU }, // LU
            { CubeSpins.FR, CubeSpins.RB, CubeSpins.BL, CubeSpins.LU, CubeSpins.LB, CubeSpins.LD, CubeSpins.DF, CubeSpins.RF, CubeSpins.UF }, // LF
            { CubeSpins.DR, CubeSpins.RU, CubeSpins.UL, CubeSpins.LF, CubeSpins.LU, CubeSpins.LB, CubeSpins.BD, CubeSpins.RD, CubeSpins.FD }, // LD
            { CubeSpins.BR, CubeSpins.RF, CubeSpins.FL, CubeSpins.LD, CubeSpins.LF, CubeSpins.LU, CubeSpins.UB, CubeSpins.RB, CubeSpins.DB }  // LB
        };
		
		/// <summary>
        /// Array to get necessary turns to get a spin. 
        /// 24x24 elements array; Files: Initial spins, Columns: Final Spins
        /// </summary>
        private static readonly CubeSpins[,] TurnsSpinsArray =
        {
            { CubeSpins.FD, CubeSpins.DB, CubeSpins.BU, CubeSpins.UR, CubeSpins.UB, CubeSpins.UL, CubeSpins.LF, CubeSpins.DF, CubeSpins.RF }, // UF
            { CubeSpins.RD, CubeSpins.DL, CubeSpins.LU, CubeSpins.UB, CubeSpins.UL, CubeSpins.UF, CubeSpins.FR, CubeSpins.DR, CubeSpins.BR }, // UR
            { CubeSpins.BD, CubeSpins.DF, CubeSpins.FU, CubeSpins.UL, CubeSpins.UF, CubeSpins.UR, CubeSpins.RB, CubeSpins.DB, CubeSpins.LB }, // UB
            { CubeSpins.LD, CubeSpins.DR, CubeSpins.RU, CubeSpins.UF, CubeSpins.UR, CubeSpins.UB, CubeSpins.BL, CubeSpins.DL, CubeSpins.FL }, // UL
            { CubeSpins.FU, CubeSpins.UB, CubeSpins.BD, CubeSpins.DL, CubeSpins.DB, CubeSpins.DR, CubeSpins.RF, CubeSpins.UF, CubeSpins.LF }, // DF
            { CubeSpins.RU, CubeSpins.UL, CubeSpins.LD, CubeSpins.DF, CubeSpins.DL, CubeSpins.DB, CubeSpins.BR, CubeSpins.UR, CubeSpins.FR }, // DR
            { CubeSpins.BU, CubeSpins.UF, CubeSpins.FD, CubeSpins.DR, CubeSpins.DF, CubeSpins.DL, CubeSpins.LB, CubeSpins.UB, CubeSpins.RB }, // DB
            { CubeSpins.LU, CubeSpins.UR, CubeSpins.RD, CubeSpins.DB, CubeSpins.DR, CubeSpins.DF, CubeSpins.FL, CubeSpins.UL, CubeSpins.BL }, // DL
            { CubeSpins.UB, CubeSpins.BD, CubeSpins.DF, CubeSpins.FL, CubeSpins.FD, CubeSpins.FR, CubeSpins.RU, CubeSpins.BU, CubeSpins.LU }, // FU
            { CubeSpins.RB, CubeSpins.BL, CubeSpins.LF, CubeSpins.FU, CubeSpins.FL, CubeSpins.FD, CubeSpins.DR, CubeSpins.BR, CubeSpins.UR }, // FR
            { CubeSpins.DB, CubeSpins.BU, CubeSpins.UF, CubeSpins.FR, CubeSpins.FU, CubeSpins.FL, CubeSpins.LD, CubeSpins.BD, CubeSpins.RD }, // FD
            { CubeSpins.LB, CubeSpins.BR, CubeSpins.RF, CubeSpins.FD, CubeSpins.FR, CubeSpins.FU, CubeSpins.UL, CubeSpins.BL, CubeSpins.DL }, // FL
            { CubeSpins.UF, CubeSpins.FD, CubeSpins.DB, CubeSpins.BR, CubeSpins.BD, CubeSpins.BL, CubeSpins.LU, CubeSpins.FU, CubeSpins.RU }, // BU
            { CubeSpins.RF, CubeSpins.FL, CubeSpins.LB, CubeSpins.BD, CubeSpins.BL, CubeSpins.BU, CubeSpins.UR, CubeSpins.FR, CubeSpins.DR }, // BR
            { CubeSpins.DF, CubeSpins.FU, CubeSpins.UB, CubeSpins.BL, CubeSpins.BU, CubeSpins.BR, CubeSpins.RD, CubeSpins.FD, CubeSpins.LD }, // BD
            { CubeSpins.LF, CubeSpins.FR, CubeSpins.RB, CubeSpins.BU, CubeSpins.BR, CubeSpins.BD, CubeSpins.DL, CubeSpins.FL, CubeSpins.UL }, // BL
            { CubeSpins.UL, CubeSpins.LD, CubeSpins.DR, CubeSpins.RF, CubeSpins.RU, CubeSpins.RB, CubeSpins.BU, CubeSpins.LU, CubeSpins.FU }, // RU
            { CubeSpins.FL, CubeSpins.LB, CubeSpins.BR, CubeSpins.RD, CubeSpins.RB, CubeSpins.RU, CubeSpins.UF, CubeSpins.LF, CubeSpins.DF }, // RF
            { CubeSpins.DL, CubeSpins.LU, CubeSpins.UR, CubeSpins.RB, CubeSpins.RU, CubeSpins.RF, CubeSpins.FD, CubeSpins.LD, CubeSpins.BD }, // RD
            { CubeSpins.BL, CubeSpins.LF, CubeSpins.FR, CubeSpins.RU, CubeSpins.RF, CubeSpins.RD, CubeSpins.DB, CubeSpins.LB, CubeSpins.UB }, // RB
            { CubeSpins.UR, CubeSpins.RD, CubeSpins.DL, CubeSpins.LB, CubeSpins.LD, CubeSpins.LF, CubeSpins.FU, CubeSpins.RU, CubeSpins.BU }, // LU
            { CubeSpins.FR, CubeSpins.RB, CubeSpins.BL, CubeSpins.LU, CubeSpins.LB, CubeSpins.LD, CubeSpins.DF, CubeSpins.RF, CubeSpins.UF }, // LF
            { CubeSpins.DR, CubeSpins.RU, CubeSpins.UL, CubeSpins.LF, CubeSpins.LU, CubeSpins.LB, CubeSpins.BD, CubeSpins.RD, CubeSpins.FD }, // LD
            { CubeSpins.BR, CubeSpins.RF, CubeSpins.FL, CubeSpins.LD, CubeSpins.LF, CubeSpins.LU, CubeSpins.UB, CubeSpins.RB, CubeSpins.DB }  // LB
        };

        #endregion Static arrays

        #region Fields

        /// <summary>
        /// Array of stickers 
        /// </summary>
        private StickerData[] Stickers;

        /// <summary>
        /// Cube orientation (spin) as Up layer + Front layer (Ex: BL means Back layer up and Left layer front) 
        /// </summary>
        private CubeSpins CubeSpin;

        #endregion

        #region Properties

        /// <summary>
        /// Cube spin (orientation) (Ex: BL means Back layer up and Left layer front) 
        /// </summary>
        public CubeSpins Spin
        {
            get { return CubeSpin; }
            set { CubeSpin = value; }
        }

        /// <summary>
        /// Get cube spin in text format
        /// </summary>
        /// <returns>Cube spin text</returns>
        public string SpinText => Spin.ToString();

        /// <summary>
        /// Checks the cube stickers consistency: all stickers assigned and only once (for debugging)
        /// </summary>
        private bool ConsistencyOK
        {
            get
            {
                bool[] Control = new bool[Stickers.Length];

                for (int n = 0; n < Stickers.Length; n++)
                {
                    int p = (int)Stickers[n].Position;
                    if (Control[p]) return false;
                    else Control[p] = true;
                }
                for (int n = 0; n < Control.Length; n++)
                    if (!Control[n]) return false;
                return true;
            }
        }

        #endregion

        #region Constructors & indexer

        /// <summary>
        /// Constructor, creates cube data in solved condition
        /// </summary>
        public CubeData()
        {
            Stickers = new StickerData[54];
            for (int n = 0; n < Stickers.Length; n++) Stickers[n] = new StickerData((StickerPositions)n);
            Spin = CubeSpins.UF;
        }

        /// <summary>
        /// Class indexer
        /// </summary>
        /// <param name="StickerNum">Sticker position</param>
        /// <returns>Sticker in given position</returns>
        public StickerData this[int StickerNum]
        {
            get
            {
                if (StickerNum >= 0 && StickerNum < Stickers.Length) return Stickers[StickerNum];
                throw new System.Exception("Sticker out of range!");
            }
        }

        #endregion Constructors & indexer

        #region Basic steps without spin consideration

        /// <summary>
        /// U (Up) movement for cube data - no spin consideration
        /// </summary>
        private void U()
        {
            for (int n = 0; n < Stickers.Length; n++) Stickers[n].U();
            // if (!ConsistencyOK) throw new System.Exception("No consistency! (U)");
        }

        /// <summary>
        /// D (Down) movement for cube data - no spin consideration
        /// </summary>
        private void D()
        {
            for (int n = 0; n < Stickers.Length; n++) Stickers[n].D();
            // if (!ConsistencyOK) throw new System.Exception("No consistency! (D)");
        }

        /// <summary>
        /// F (Front) movement for cube data - no spin consideration
        /// </summary>
        private void F()
        {
            for (int n = 0; n < Stickers.Length; n++) Stickers[n].F();
            // if (!ConsistencyOK) throw new System.Exception("No consistency! (F)");
        }

        /// <summary>
        /// B (Back) movement for cube data - no spin consideration
        /// </summary>
        private void B()
        {
            for (int n = 0; n < Stickers.Length; n++) Stickers[n].B();
            // if (!ConsistencyOK) throw new System.Exception("No consistency! (B)");
        }

        /// <summary>
        /// R (Right) movement for cube data - no spin consideration
        /// </summary>
        private void R()
        {
            for (int n = 0; n < Stickers.Length; n++) Stickers[n].R();
            // if (!ConsistencyOK) throw new System.Exception("No consistency! (R)");
        }

        /// <summary>
        /// L (Left) movement for cube data - no spin consideration
        /// </summary>
        private void L()
        {
            for (int n = 0; n < Stickers.Length; n++) Stickers[n].L();
            // if (!ConsistencyOK) throw new System.Exception("No consistency! (L)");
        }

        /// <summary>
        /// U' (Up) movement for cube data - no spin consideration
        /// </summary>
        private void Up()
        {
            for (int n = 0; n < Stickers.Length; n++) Stickers[n].Up();
            // if (!ConsistencyOK) throw new System.Exception("No consistency! (U')");
        }

        /// <summary>
        /// D' (Down) movement for cube data - no spin consideration
        /// </summary>
        private void Dp()
        {
            for (int n = 0; n < Stickers.Length; n++) Stickers[n].Dp();
            // if (!ConsistencyOK) throw new System.Exception("No consistency! (D')");
        }

        /// <summary>
        /// F' (Front) movement for cube data - no spin consideration
        /// </summary>
        private void Fp()
        {
            for (int n = 0; n < Stickers.Length; n++) Stickers[n].Fp();
            // if (!ConsistencyOK) throw new System.Exception("No consistency! (F')");
        }

        /// <summary>
        /// B' (Back) movement for cube data - no spin consideration
        /// </summary>
        private void Bp()
        {
            for (int n = 0; n < Stickers.Length; n++) Stickers[n].Bp();
            // if (!ConsistencyOK) throw new System.Exception("No consistency! (B')");
        }

        /// <summary>
        /// R' (Right) movement for cube data - no spin consideration
        /// </summary>
        private void Rp()
        {
            for (int n = 0; n < Stickers.Length; n++) Stickers[n].Rp();
            // if (!ConsistencyOK) throw new System.Exception("No consistency! (R')");
        }

        /// <summary>
        /// L' (Left) movement for cube data - no spin consideration
        /// </summary>
        private void Lp()
        {
            for (int n = 0; n < Stickers.Length; n++) Stickers[n].Lp();
            // if (!ConsistencyOK) throw new System.Exception("No consistency! (L')");
        }

        /// <summary>
        /// U2 (Up) movement for cube data - no spin consideration
        /// </summary>
        private void U2()
        {
            for (int n = 0; n < Stickers.Length; n++) Stickers[n].U2();
            // if (!ConsistencyOK) throw new System.Exception("No consistency! (U2)");
        }

        /// <summary>
        /// D2 (Down) movement for cube data - no spin consideration
        /// </summary>
        private void D2()
        {
            for (int n = 0; n < Stickers.Length; n++) Stickers[n].D2();
            // if (!ConsistencyOK) throw new System.Exception("No consistency! (D2)");
        }

        /// <summary>
        /// F2 (Front) movement for cube data - no spin consideration
        /// </summary>
        private void F2()
        {
            for (int n = 0; n < Stickers.Length; n++) Stickers[n].F2();
            // if (!ConsistencyOK) throw new System.Exception("No consistency! (F2)");
        }

        /// <summary>
        /// B2 (Back) movement for cube data - no spin consideration
        /// </summary>
        private void B2()
        {
            for (int n = 0; n < Stickers.Length; n++) Stickers[n].B2();
            // if (!ConsistencyOK) throw new System.Exception("No consistency! (B2)");
        }

        /// <summary>
        /// R2 (Right) movement for cube data - no spin consideration
        /// </summary>
        private void R2()
        {
            for (int n = 0; n < Stickers.Length; n++) Stickers[n].R2();
            // if (!ConsistencyOK) throw new System.Exception("No consistency! (R2)");
        }

        /// <summary>
        /// L2 (Left) movement for cube data - no spin consideration
        /// </summary>
        private void L2()
        {
            for (int n = 0; n < Stickers.Length; n++) Stickers[n].L2();
            // if (!ConsistencyOK) throw new System.Exception("No consistency! (L2)");
        }

        #endregion

        #region Step turns

        /// <summary>
        /// Turn the cube around x axis
        /// </summary>
        /// <param name="n">Number of 90º turns</param>
        public void x(int n)
        {
            int turns = ScrambleStep.mod4(n);
            if (turns > 0) Spin = SpinsArray[(int)Spin, turns - 1];
        }

        /// <summary>
        /// Turn the cube around y axis
        /// </summary>
        /// <param name="n">Number of 90º turns</param>
        public void y(int n)
        {
            int turns = ScrambleStep.mod4(n);
            if (turns > 0) Spin = SpinsArray[(int)Spin, turns + 2];
        }

        /// <summary>
        /// Turn the cube around z axis
        /// </summary>
        /// <param name="n">Number of 90º turns</param>
        public void z(int n)
        {
            int turns = ScrambleStep.mod4(n);
            if (turns > 0) Spin = SpinsArray[(int)Spin, turns + 5];
        }

        #endregion

        #region Steps with spin consideration

        /// <summary>
        /// Up layer movement n times
        /// </summary>
        /// <param name="n">Number of U movements</param>
        public void U(int n)
        {
            int turns = ScrambleStep.mod4(n);

            if (turns == 0) return;

            switch (Spin)
            {
                case CubeSpins.UF:
                case CubeSpins.UR:
                case CubeSpins.UB:
                case CubeSpins.UL: if (turns == 1) U(); else if (turns == 2) U2(); else Up(); break;
                case CubeSpins.DF:
                case CubeSpins.DR:
                case CubeSpins.DB:
                case CubeSpins.DL: if (turns == 1) D(); else if (turns == 2) D2(); else Dp(); break;
                case CubeSpins.FU:
                case CubeSpins.FR:
                case CubeSpins.FD:
                case CubeSpins.FL: if (turns == 1) F(); else if (turns == 2) F2(); else Fp(); break;
                case CubeSpins.BU:
                case CubeSpins.BR:
                case CubeSpins.BD:
                case CubeSpins.BL: if (turns == 1) B(); else if (turns == 2) B2(); else Bp(); break;
                case CubeSpins.RU:
                case CubeSpins.RF:
                case CubeSpins.RD:
                case CubeSpins.RB: if (turns == 1) R(); else if (turns == 2) R2(); else Rp(); break;
                case CubeSpins.LU:
                case CubeSpins.LF:
                case CubeSpins.LD:
                case CubeSpins.LB: if (turns == 1) L(); else if (turns == 2) L2(); else Lp(); break;

                default:
                    throw new System.Exception("Spin out of range! (U)");
            }
        }

        /// <summary>
        /// Down layer movement n times
        /// </summary>
        /// <param name="n">Number of D movements</param>
        public void D(int n)
        {
            int turns = ScrambleStep.mod4(n);

            if (turns == 0) return;

            switch (Spin)
            {
                case CubeSpins.UF:
                case CubeSpins.UR:
                case CubeSpins.UB:
                case CubeSpins.UL: if (turns == 1) D(); else if (turns == 2) D2(); else Dp(); break;
                case CubeSpins.DF:
                case CubeSpins.DR:
                case CubeSpins.DB:
                case CubeSpins.DL: if (turns == 1) U(); else if (turns == 2) U2(); else Up(); break;
                case CubeSpins.FU:
                case CubeSpins.FR:
                case CubeSpins.FD:
                case CubeSpins.FL: if (turns == 1) B(); else if (turns == 2) B2(); else Bp(); break;
                case CubeSpins.BU:
                case CubeSpins.BR:
                case CubeSpins.BD:
                case CubeSpins.BL: if (turns == 1) F(); else if (turns == 2) F2(); else Fp(); break;
                case CubeSpins.RU:
                case CubeSpins.RF:
                case CubeSpins.RD:
                case CubeSpins.RB: if (turns == 1) L(); else if (turns == 2) L2(); else Lp(); break;
                case CubeSpins.LU:
                case CubeSpins.LF:
                case CubeSpins.LD:
                case CubeSpins.LB: if (turns == 1) R(); else if (turns == 2) R2(); else Rp(); break;

                default:
                    throw new System.Exception("Spin out of range! (D)");
            }
        }

        /// <summary>
        /// Front layer movement n times
        /// </summary>
        /// <param name="n">Number of F movements</param>
        public void F(int n)
        {
            int turns = ScrambleStep.mod4(n);

            if (turns == 0) return;

            switch (Spin)
            {
                case CubeSpins.FU:
                case CubeSpins.BU:
                case CubeSpins.RU:
                case CubeSpins.LU: if (turns == 1) U(); else if (turns == 2) U2(); else Up(); break;
                case CubeSpins.FD:
                case CubeSpins.BD:
                case CubeSpins.RD:
                case CubeSpins.LD: if (turns == 1) D(); else if (turns == 2) D2(); else Dp(); break;
                case CubeSpins.UF:
                case CubeSpins.DF:
                case CubeSpins.RF:
                case CubeSpins.LF: if (turns == 1) F(); else if (turns == 2) F2(); else Fp(); break;
                case CubeSpins.UB:
                case CubeSpins.DB:
                case CubeSpins.RB:
                case CubeSpins.LB: if (turns == 1) B(); else if (turns == 2) B2(); else Bp(); break;
                case CubeSpins.UR:
                case CubeSpins.DR:
                case CubeSpins.FR:
                case CubeSpins.BR: if (turns == 1) R(); else if (turns == 2) R2(); else Rp(); break;
                case CubeSpins.UL:
                case CubeSpins.DL:
                case CubeSpins.FL:
                case CubeSpins.BL: if (turns == 1) L(); else if (turns == 2) L2(); else Lp(); break;

                default:
                    throw new System.Exception("Spin out of range! (F)");
            }
        }

        /// <summary>
        /// Back layer movement n times
        /// </summary>
        /// <param name="n">Number of B movements</param>
        public void B(int n)
        {
            int turns = ScrambleStep.mod4(n);

            if (turns == 0) return;

            switch (Spin)
            {
                case CubeSpins.FU:
                case CubeSpins.BU:
                case CubeSpins.RU:
                case CubeSpins.LU: if (turns == 1) D(); else if (turns == 2) D2(); else Dp(); break;
                case CubeSpins.FD:
                case CubeSpins.BD:
                case CubeSpins.RD:
                case CubeSpins.LD: if (turns == 1) U(); else if (turns == 2) U2(); else Up(); break;
                case CubeSpins.UF:
                case CubeSpins.DF:
                case CubeSpins.RF:
                case CubeSpins.LF: if (turns == 1) B(); else if (turns == 2) B2(); else Bp(); break;
                case CubeSpins.UB:
                case CubeSpins.DB:
                case CubeSpins.RB:
                case CubeSpins.LB: if (turns == 1) F(); else if (turns == 2) F2(); else Fp(); break;
                case CubeSpins.UR:
                case CubeSpins.DR:
                case CubeSpins.FR:
                case CubeSpins.BR: if (turns == 1) L(); else if (turns == 2) L2(); else Lp(); break;
                case CubeSpins.UL:
                case CubeSpins.DL:
                case CubeSpins.FL:
                case CubeSpins.BL: if (turns == 1) R(); else if (turns == 2) R2(); else Rp(); break;

                default:
                    throw new System.Exception("Spin out of range! (B)");
            }
        }

        /// <summary>
        /// Right layer movement n times
        /// </summary>
        /// <param name="n">Number of R movements</param>
        public void R(int n)
        {
            int turns = ScrambleStep.mod4(n);

            if (turns == 0) return;

            switch (Spin)
            {
                case CubeSpins.FR:
                case CubeSpins.BL:
                case CubeSpins.RB:
                case CubeSpins.LF: if (turns == 1) U(); else if (turns == 2) U2(); else Up(); break;
                case CubeSpins.FL:
                case CubeSpins.BR:
                case CubeSpins.RF:
                case CubeSpins.LB: if (turns == 1) D(); else if (turns == 2) D2(); else Dp(); break;
                case CubeSpins.UL:
                case CubeSpins.DR:
                case CubeSpins.RU:
                case CubeSpins.LD: if (turns == 1) F(); else if (turns == 2) F2(); else Fp(); break;
                case CubeSpins.UR:
                case CubeSpins.DL:
                case CubeSpins.RD:
                case CubeSpins.LU: if (turns == 1) B(); else if (turns == 2) B2(); else Bp(); break;
                case CubeSpins.UF:
                case CubeSpins.DB:
                case CubeSpins.FD:
                case CubeSpins.BU: if (turns == 1) R(); else if (turns == 2) R2(); else Rp(); break;
                case CubeSpins.UB:
                case CubeSpins.DF:
                case CubeSpins.FU:
                case CubeSpins.BD: if (turns == 1) L(); else if (turns == 2) L2(); else Lp(); break;

                default:
                    throw new System.Exception("Spin out of range! (R)");
            }
        }

        /// <summary>
        /// Left layer movement n times
        /// </summary>
        /// <param name="n">Number of L movements</param>
        public void L(int n)
        {
            int turns = ScrambleStep.mod4(n);

            if (turns == 0) return;

            switch (Spin)
            {
                case CubeSpins.FR:
                case CubeSpins.BL:
                case CubeSpins.RB:
                case CubeSpins.LF: if (turns == 1) D(); else if (turns == 2) D2(); else Dp(); break;
                case CubeSpins.FL:
                case CubeSpins.BR:
                case CubeSpins.RF:
                case CubeSpins.LB: if (turns == 1) U(); else if (turns == 2) U2(); else Up(); break;
                case CubeSpins.UL:
                case CubeSpins.DR:
                case CubeSpins.RU:
                case CubeSpins.LD: if (turns == 1) B(); else if (turns == 2) B2(); else Bp(); break;
                case CubeSpins.UR:
                case CubeSpins.DL:
                case CubeSpins.RD:
                case CubeSpins.LU: if (turns == 1) F(); else if (turns == 2) F2(); else Fp(); break;
                case CubeSpins.UF:
                case CubeSpins.DB:
                case CubeSpins.FD:
                case CubeSpins.BU: if (turns == 1) L(); else if (turns == 2) L2(); else Lp(); break;
                case CubeSpins.UB:
                case CubeSpins.DF:
                case CubeSpins.FU:
                case CubeSpins.BD: if (turns == 1) R(); else if (turns == 2) R2(); else Rp(); break;

                default:
                    throw new System.Exception("Spin out of range! (L)");
            }
        }

        #endregion

        #region Other functions

        /// <summary>
        /// Check if the cube data matches with given cube data
        /// </summary>
        /// <param name="c">Cube data to compare</param>
        /// <returns>True if stickers positions matches (orientation -spin- is not important)</returns>
        public bool Match(CubeData c)
        {
            if (c == null) return false;
            for (int Pos = 0; Pos < Stickers.Length; Pos++)
                if (Stickers[Pos].Position != c.Stickers[Pos].Position) return false; // TODO: Check!

            return true; // Return true if all the stickers match
        }

        /// <summary>
        /// Copy the cube status
        /// </summary>
        /// <param name="c">Cube witch data will be copied</param>
        public void Copy(CubeData c)
        {
            if (c != null)
            {
                for (int Pos = 0; Pos < Stickers.Length; Pos++) Stickers[Pos] = c.Stickers[Pos];
                Spin = c.Spin;
            }
        }

        /// <summary>
        /// Cube to solved position
        /// </summary>
        public void Reset()
        {
            // Sticker number equals sticker position (in solved condition)
            for (byte Pos = 0; Pos < Stickers.Length; Pos++) Stickers[Pos].Reset();
            Spin = CubeSpins.UF;
        }

        /// <summary>
        /// Apply scramble (add) to cube data
        /// </summary>
        /// <param name="SD">Scramble to be applied</param>
        public void ApplyScramble(Scramble SD)
        {
            if (SD == null) return;
            for (int n = 0; n < SD.Length; n++)
            {
                if (SD[n] == Steps.OPEN_PARENTHESIS)
                {
                    int i = n + 1, nested = 1;
                    while (i < SD.Length && nested > 0)
                    {
                        if (SD[i] == Steps.OPEN_PARENTHESIS) nested++;
                        else if (SD[i] >= Steps.CLOSE_PARENTHESIS_1_REP) nested--;
                        if (nested > 0) i++;
                    }
                    if (i >= SD.Length) continue;

                    int rep = ScrambleStep.GetCloseParenthesisRepetions(SD[i]);

                    Scramble sub = SD.SubScramble(n + 1, i - n - 1);

                    for (int j = 0; j < rep; j++) ApplyScramble(sub);
                    n = i;
                }
                else ApplyScrambleStep(SD[n]);
            }
        }

        /// <summary>
        /// Apply single scramble step to cube data
        /// </summary>
        /// <param name="SS">Scramble step to be applied</param>
        public void ApplyScrambleStep(Steps SS)
        {
            if (SS >= Steps.OPEN_PARENTHESIS) return; // Parentheses as single step can't be applied

            Steps[] Seq = ScrambleStep.GetEquivalentStepSequence(SS);

            int n;
            foreach (Steps ST in Seq)
            {
                if (ST == Steps.NONE) continue;

                switch (ScrambleStep.Movement(ST))
                {
                    case Movements.NONE: n = 0; break;
                    case Movements.ROT90CW: n = 1; break;
                    case Movements.ROT90CCW: n = 3; break;
                    default: n = 2; break; // ROT180CW or ROT180CCW
                }

                if (ScrambleStep.IsMovement(ST))
                {
                    switch (ScrambleStep.Layer(ST))
                    {
                        case Layers.U: U(n); break;
                        case Layers.D: D(n); break;
                        case Layers.F: F(n); break;
                        case Layers.B: B(n); break;
                        case Layers.R: R(n); break;
                        case Layers.L: L(n); break;
                    }
                    continue;
                }

                if (ScrambleStep.IsTurn(ST))
                {
                    switch (ScrambleStep.Layer(ST))
                    {
                        case Layers.R: x(n); break;
                        case Layers.U: y(n); break;
                        case Layers.F: z(n); break;
                    }
                }
            }
        }

        /// <summary>
        /// Get the sticker (as solved cube sticker position) placed in the specified position
        /// </summary>
        /// <param name="sp">Sticker position</param>
        /// <returns>Solved cube sticker position</returns>
        public StickerPositions GetStickerSolvedPosition(StickerPositions sp)
        {
            for (int n = 0; n < Stickers.Length; n++)
                if (Stickers[n].Position == sp) return Stickers[n].Sticker;

            throw new System.Exception("Cube inconsistence: Sticker solved position not Found!");
        }

        /// <summary>
        /// Get the piece (as solved cube piece position) placed in the specified position
        /// </summary>
        /// <param name="PP">Piece position</param>
        /// <returns>Solved cube piece position</returns>
        public Pieces GetPieceSolvedPosition(Pieces PP)
        {
            for (int n = 0; n < Stickers.Length; n++)
                if (PP == Stickers[n].PiecePosition) return Stickers[n].Piece;

            throw new System.Exception("Cube inconsistence: No sticker piece position!");
        }

        /// <summary>
        /// Get the solved face for the sticker currently located in given position (useful for color)
        /// </summary>
        /// <param name="sp">Sticker position</param>
        /// <returns></returns>
        public Faces GetStickerFacePosition(StickerPositions sp)
        {
            for (int n = 0; n < Stickers.Length; n++)
                if (Stickers[n].Position == sp) return Stickers[n].Face;

            throw new System.Exception("Cube inconsistence: No sticker position in GetStickerFacePosition");
        }

        #endregion
    }
}