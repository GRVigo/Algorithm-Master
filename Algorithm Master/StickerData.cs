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
    /// Struct for manage stickers
    /// </summary>
    public struct StickerData
    {
        #region Static arrays

        #region Arrays for stickers positions

        /// <summary>
        /// Array for stickers U movement
        /// </summary>
        private static readonly StickerPositions[] s_U = 
        {
            StickerPositions.U,
            StickerPositions.D,
            StickerPositions.F,
            StickerPositions.B,
            StickerPositions.R,
            StickerPositions.L,
            StickerPositions.UL_U, // UF_U
            StickerPositions.UL_L, // UF_F
            StickerPositions.UF_U, // UR_U
            StickerPositions.UF_F, // UR_R
            StickerPositions.UR_U, // UB_U
            StickerPositions.UR_R, // UB_B
            StickerPositions.UB_U, // UL_U
            StickerPositions.UB_B, // UL_L
            StickerPositions.FR_F,
            StickerPositions.FR_R,
            StickerPositions.RB_R,
            StickerPositions.RB_B,
            StickerPositions.BL_B,
            StickerPositions.BL_L,
            StickerPositions.LF_L,
            StickerPositions.LF_F,
            StickerPositions.DF_D,
            StickerPositions.DF_F,
            StickerPositions.DR_D,
            StickerPositions.DR_R,
            StickerPositions.DB_D,
            StickerPositions.DB_B,
            StickerPositions.DL_D,
            StickerPositions.DL_L,
            StickerPositions.UFL_U, // UFR_U
            StickerPositions.UFL_L, // UFR_F
            StickerPositions.UFL_F, // UFR_R
            StickerPositions.UFR_U, // UBR_U
            StickerPositions.UFR_R, // UBR_B
            StickerPositions.UFR_F, // UBR_R
            StickerPositions.UBR_U, // UBL_U
            StickerPositions.UBR_R, // UBL_B
            StickerPositions.UBR_B, // UBL_L
            StickerPositions.UBL_U, // UFL_U
            StickerPositions.UBL_L, // UFL_F
            StickerPositions.UBL_B, // UFL_L
            StickerPositions.DFR_D,
            StickerPositions.DFR_F,
            StickerPositions.DFR_R,
            StickerPositions.DBR_D,
            StickerPositions.DBR_B,
            StickerPositions.DBR_R,
            StickerPositions.DBL_D,
            StickerPositions.DBL_B,
            StickerPositions.DBL_L,
            StickerPositions.DFL_D,
            StickerPositions.DFL_F,
            StickerPositions.DFL_L
        };

        /// <summary>
        /// Array for stickers D movement
        /// </summary>
        private static readonly StickerPositions[] s_D =
        {
            StickerPositions.U,
            StickerPositions.D,
            StickerPositions.F,
            StickerPositions.B,
            StickerPositions.R,
            StickerPositions.L,
            StickerPositions.UF_U,
            StickerPositions.UF_F,
            StickerPositions.UR_U,
            StickerPositions.UR_R,
            StickerPositions.UB_U,
            StickerPositions.UB_B,
            StickerPositions.UL_U,
            StickerPositions.UL_L,
            StickerPositions.FR_F,
            StickerPositions.FR_R,
            StickerPositions.RB_R,
            StickerPositions.RB_B,
            StickerPositions.BL_B,
            StickerPositions.BL_L,
            StickerPositions.LF_L,
            StickerPositions.LF_F,
            StickerPositions.DR_D, // DF_D
            StickerPositions.DR_R, // DF_F
            StickerPositions.DB_D, // DR_D
            StickerPositions.DB_B, // DR_R
            StickerPositions.DL_D, // DB_D
            StickerPositions.DL_L, // DB_B
            StickerPositions.DF_D, // DL_D
            StickerPositions.DF_F, // DL_L
            StickerPositions.UFR_U,
            StickerPositions.UFR_F,
            StickerPositions.UFR_R,
            StickerPositions.UBR_U,
            StickerPositions.UBR_B,
            StickerPositions.UBR_R,
            StickerPositions.UBL_U,
            StickerPositions.UBL_B,
            StickerPositions.UBL_L,
            StickerPositions.UFL_U,
            StickerPositions.UFL_F,
            StickerPositions.UFL_L,
            StickerPositions.DBR_D, // DFR_D
            StickerPositions.DBR_R, // DFR_F
            StickerPositions.DBR_B, // DFR_R
            StickerPositions.DBL_D, // DBR_D
            StickerPositions.DBL_L, // DBR_B
            StickerPositions.DBL_B, // DBR_R
            StickerPositions.DFL_D, // DBL_D
            StickerPositions.DFL_L, // DBL_B
            StickerPositions.DFL_F, // DBL_L
            StickerPositions.DFR_D, // DFL_D
            StickerPositions.DFR_R, // DFL_F
            StickerPositions.DFR_F  // DFL_L
        };

        /// <summary>
        /// Array for stickers F movement
        /// </summary>
        private static readonly StickerPositions[] s_F =
        {
            StickerPositions.U,
            StickerPositions.D,
            StickerPositions.F,
            StickerPositions.B,
            StickerPositions.R,
            StickerPositions.L,
            StickerPositions.FR_R, // UF_U
            StickerPositions.FR_F, // UF_F
            StickerPositions.UR_U,
            StickerPositions.UR_R,
            StickerPositions.UB_U,
            StickerPositions.UB_B,
            StickerPositions.UL_U,
            StickerPositions.UL_L,
            StickerPositions.DF_F, // FR_F
            StickerPositions.DF_D, // FR_R
            StickerPositions.RB_R,
            StickerPositions.RB_B,
            StickerPositions.BL_B,
            StickerPositions.BL_L,
            StickerPositions.UF_U, // LF_L
            StickerPositions.UF_F, // LF_F
            StickerPositions.LF_L, // DF_D
            StickerPositions.LF_F, // DF_F
            StickerPositions.DR_D,
            StickerPositions.DR_R,
            StickerPositions.DB_D,
            StickerPositions.DB_B,
            StickerPositions.DL_D,
            StickerPositions.DL_L,
            StickerPositions.DFR_R, // UFR_U
            StickerPositions.DFR_F, // UFR_F
            StickerPositions.DFR_D, // UFR_R
            StickerPositions.UBR_U,
            StickerPositions.UBR_B,
            StickerPositions.UBR_R,
            StickerPositions.UBL_U,
            StickerPositions.UBL_B,
            StickerPositions.UBL_L,
            StickerPositions.UFR_R, // UFL_U
            StickerPositions.UFR_F, // UFL_F
            StickerPositions.UFR_U, // UFL_L
            StickerPositions.DFL_L, // DFR_D
            StickerPositions.DFL_F, // DFR_F
            StickerPositions.DFL_D, // DFR_R
            StickerPositions.DBR_D,
            StickerPositions.DBR_B,
            StickerPositions.DBR_R,
            StickerPositions.DBL_D,
            StickerPositions.DBL_B,
            StickerPositions.DBL_L,
            StickerPositions.UFL_L, // DFL_D
            StickerPositions.UFL_F, // DFL_F
            StickerPositions.UFL_U  // DFL_L
        };

        /// <summary>
        /// Array for stickers B movement
        /// </summary>
        private static readonly StickerPositions[] s_B =
        {
            StickerPositions.U,
            StickerPositions.D,
            StickerPositions.F,
            StickerPositions.B,
            StickerPositions.R,
            StickerPositions.L,
            StickerPositions.UF_U,
            StickerPositions.UF_F,
            StickerPositions.UR_U,
            StickerPositions.UR_R,
            StickerPositions.BL_L, // UB_U
            StickerPositions.BL_B, // UB_B
            StickerPositions.UL_U,
            StickerPositions.UL_L,
            StickerPositions.FR_F,
            StickerPositions.FR_R,
            StickerPositions.UB_U, // RB_R
            StickerPositions.UB_B, // RB_B
            StickerPositions.DB_B, // BL_B
            StickerPositions.DB_D, // BL_L
            StickerPositions.LF_L,
            StickerPositions.LF_F,
            StickerPositions.DF_D,
            StickerPositions.DF_F,
            StickerPositions.DR_D,
            StickerPositions.DR_R,
            StickerPositions.RB_R, // DB_D
            StickerPositions.RB_B, // DB_B
            StickerPositions.DL_D,
            StickerPositions.DL_L,
            StickerPositions.UFR_U,
            StickerPositions.UFR_F,
            StickerPositions.UFR_R,
            StickerPositions.UBL_L, // UBR_U
            StickerPositions.UBL_B, // UBR_B
            StickerPositions.UBL_U, // UBR_R
            StickerPositions.DBL_L, // UBL_U
            StickerPositions.DBL_B, // UBL_B
            StickerPositions.DBL_D, // UBL_L
            StickerPositions.UFL_U,
            StickerPositions.UFL_F,
            StickerPositions.UFL_L,
            StickerPositions.DFR_D,
            StickerPositions.DFR_F,
            StickerPositions.DFR_R,
            StickerPositions.UBR_R, // DBR_D
            StickerPositions.UBR_B, // DBR_B
            StickerPositions.UBR_U, // DBR_R
            StickerPositions.DBR_R, // DBL_D
            StickerPositions.DBR_B, // DBL_B
            StickerPositions.DBR_D, // DBL_L
            StickerPositions.DFL_D,
            StickerPositions.DFL_F,
            StickerPositions.DFL_L
        };

        /// <summary>
        /// Array for stickers R movement
        /// </summary>
        private static readonly StickerPositions[] s_R =
        {
            StickerPositions.U,
            StickerPositions.D,
            StickerPositions.F,
            StickerPositions.B,
            StickerPositions.R,
            StickerPositions.L,
            StickerPositions.UF_U,
            StickerPositions.UF_F,
            StickerPositions.RB_B, // UR_U
            StickerPositions.RB_R, // UR_R
            StickerPositions.UB_U,
            StickerPositions.UB_B,
            StickerPositions.UL_U,
            StickerPositions.UL_L,
            StickerPositions.UR_U, // FR_F
            StickerPositions.UR_R, // FR_R
            StickerPositions.DR_R, // RB_R
            StickerPositions.DR_D, // RB_B
            StickerPositions.BL_B,
            StickerPositions.BL_L,
            StickerPositions.LF_L,
            StickerPositions.LF_F,
            StickerPositions.DF_D,
            StickerPositions.DF_F,
            StickerPositions.FR_F, // DR_D
            StickerPositions.FR_R, // DR_R
            StickerPositions.DB_D,
            StickerPositions.DB_B,
            StickerPositions.DL_D,
            StickerPositions.DL_L,
            StickerPositions.UBR_B, // UFR_U
            StickerPositions.UBR_U, // UFR_F
            StickerPositions.UBR_R, // UFR_R
            StickerPositions.DBR_B, // UBR_U
            StickerPositions.DBR_D, // UBR_B
            StickerPositions.DBR_R, // UBR_R
            StickerPositions.UBL_U,
            StickerPositions.UBL_B,
            StickerPositions.UBL_L,
            StickerPositions.UFL_U,
            StickerPositions.UFL_F,
            StickerPositions.UFL_L,
            StickerPositions.UFR_F, // DFR_D
            StickerPositions.UFR_U, // DFR_F
            StickerPositions.UFR_R, // DFR_R
            StickerPositions.DFR_F, // DBR_D
            StickerPositions.DFR_D, // DBR_B
            StickerPositions.DFR_R, // DBR_R
            StickerPositions.DBL_D,
            StickerPositions.DBL_B,
            StickerPositions.DBL_L,
            StickerPositions.DFL_D,
            StickerPositions.DFL_F,
            StickerPositions.DFL_L
        };

        /// <summary>
        /// Array for stickers L movement
        /// </summary>
        private static readonly StickerPositions[] s_L =
        {
            StickerPositions.U,
            StickerPositions.D,
            StickerPositions.F,
            StickerPositions.B,
            StickerPositions.R,
            StickerPositions.L,
            StickerPositions.UF_U,
            StickerPositions.UF_F,
            StickerPositions.UR_U,
            StickerPositions.UR_R,
            StickerPositions.UB_U,
            StickerPositions.UB_B,
            StickerPositions.LF_F, // UL_U
            StickerPositions.LF_L, // UL_L
            StickerPositions.FR_F,
            StickerPositions.FR_R,
            StickerPositions.RB_R,
            StickerPositions.RB_B,
            StickerPositions.UL_U, // BL_B
            StickerPositions.UL_L, // BL_L
            StickerPositions.DL_L, // LF_L
            StickerPositions.DL_D, // LF_F
            StickerPositions.DF_D,
            StickerPositions.DF_F,
            StickerPositions.DR_D,
            StickerPositions.DR_R,
            StickerPositions.DB_D,
            StickerPositions.DB_B,
            StickerPositions.BL_B, // DL_D
            StickerPositions.BL_L, // DL_L
            StickerPositions.UFR_U,
            StickerPositions.UFR_F,
            StickerPositions.UFR_R,
            StickerPositions.UBR_U,
            StickerPositions.UBR_B,
            StickerPositions.UBR_R,
            StickerPositions.UFL_F, // UBL_U
            StickerPositions.UFL_U, // UBL_B
            StickerPositions.UFL_L, // UBL_L
            StickerPositions.DFL_F, // UFL_U
            StickerPositions.DFL_D, // UFL_F
            StickerPositions.DFL_L, // UFL_L
            StickerPositions.DFR_D,
            StickerPositions.DFR_F,
            StickerPositions.DFR_R,
            StickerPositions.DBR_D,
            StickerPositions.DBR_B,
            StickerPositions.DBR_R,
            StickerPositions.UBL_B, // DBL_D
            StickerPositions.UBL_U, // DBL_B
            StickerPositions.UBL_L, // DBL_L
            StickerPositions.DBL_B, // DFL_D
            StickerPositions.DBL_D, // DFL_F
            StickerPositions.DBL_L  // DFL_L
        };

        /// <summary>
        /// Array for stickers U' movement (assigned in static constructor)
        /// </summary>
        private static readonly StickerPositions[] s_Up;

        /// <summary>
        /// Array for stickers D' movement (assigned in static constructor)
        /// </summary>
        private static readonly StickerPositions[] s_Dp;

        /// <summary>
        /// Array for stickers F' movement (assigned in static constructor)
        /// </summary>
        private static readonly StickerPositions[] s_Fp;

        /// <summary>
        /// Array for stickers B' movement (assigned in static constructor)
        /// </summary>
        private static readonly StickerPositions[] s_Bp;

        /// <summary>
        /// Array for stickers R' movement (assigned in static constructor)
        /// </summary>
        private static readonly StickerPositions[] s_Rp;

        /// <summary>
        /// Array for stickers L' movement (assigned in static constructor)
        /// </summary>
        private static readonly StickerPositions[] s_Lp;

        /// <summary>
        /// Array for stickers U2 movement (assigned in static constructor)
        /// </summary>
        private static readonly StickerPositions[] s_U2;

        /// <summary>
        /// Array for stickers D2 movement (assigned in static constructor)
        /// </summary>
        private static readonly StickerPositions[] s_D2;

        /// <summary>
        /// Array for stickers F2 movement (assigned in static constructor)
        /// </summary>
        private static readonly StickerPositions[] s_F2;

        /// <summary>
        /// Array for stickers B2 movement (assigned in static constructor)
        /// </summary>
        private static readonly StickerPositions[] s_B2;

        /// <summary>
        /// Array for stickers R2 movement (assigned in static constructor)
        /// </summary>
        private static readonly StickerPositions[] s_R2;

        /// <summary>
        /// Array for stickers L2 movement (assigned in static constructor)
        /// </summary>
        private static readonly StickerPositions[] s_L2;

        /// <summary>
        /// Array for stickers x turn (use for turning neutral stickers only)
        /// </summary>
        private static readonly StickerPositions[] s_x =
        {
            StickerPositions.B, // U
            StickerPositions.F, // D
            StickerPositions.U, // F
            StickerPositions.D, // B
            StickerPositions.R, // R
            StickerPositions.L, // L
            StickerPositions.UB_B, // UF_U
            StickerPositions.UB_U, // UF_F
            StickerPositions.RB_B, // UR_U
            StickerPositions.RB_R, // UR_R
            StickerPositions.DB_B, // UB_U
            StickerPositions.DB_D, // UB_B
            StickerPositions.BL_B, // UL_U
            StickerPositions.BL_L, // UL_L
            StickerPositions.UR_U, // FR_F
            StickerPositions.UR_R, // FR_R
            StickerPositions.DR_R, // RB_R
            StickerPositions.DR_D, // RB_B
            StickerPositions.DL_D, // BL_B
            StickerPositions.DL_L, // BL_L
            StickerPositions.UL_L, // LF_L
            StickerPositions.UL_U, // LF_F
            StickerPositions.UF_F, // DF_D
            StickerPositions.UF_U, // DF_F
            StickerPositions.FR_F, // DR_D
            StickerPositions.FR_R, // DR_R
            StickerPositions.DF_F, // DB_D
            StickerPositions.DF_D, // DB_B
            StickerPositions.LF_F, // DL_D
            StickerPositions.LF_L, // DL_L
            StickerPositions.UBR_B, // UFR_U
            StickerPositions.UBR_U, // UFR_F
            StickerPositions.UBR_R, // UFR_R
            StickerPositions.DBR_B, // UBR_U
            StickerPositions.DBR_D, // UBR_B
            StickerPositions.DBR_R, // UBR_R
            StickerPositions.DBL_B, // UBL_U
            StickerPositions.DBL_D, // UBL_B
            StickerPositions.DBL_L, // UBL_L
            StickerPositions.UBL_B, // UFL_U
            StickerPositions.UBL_U, // UFL_F
            StickerPositions.UBL_L, // UFL_L
            StickerPositions.UFR_F, // DFR_D
            StickerPositions.UFR_U, // DFR_F
            StickerPositions.UFR_R, // DFR_R
            StickerPositions.DFR_F, // DBR_D
            StickerPositions.DFR_D, // DBR_B
            StickerPositions.DFR_R, // DBR_R
            StickerPositions.DFL_F, // DBL_D
            StickerPositions.DFL_D, // DBL_B
            StickerPositions.DFL_L, // DBL_L
            StickerPositions.UFL_F, // DFL_D
            StickerPositions.UFL_U, // DFL_F
            StickerPositions.UFL_L  // DFL_L
        };

        /// <summary>
        /// Array for stickers x2 turn (use for turning neutral stickers only, assigned in static constructor)
        /// </summary>
        private static readonly StickerPositions[] s_x2;

        /// <summary>
        /// Array for stickers x' turn (use for turning neutral stickers only, assigned in static constructor)
        /// </summary>
        private static readonly StickerPositions[] s_xp;

        /// <summary>
        /// Array for stickers y turn (use for turning neutral stickers only)
        /// </summary>
        private static readonly StickerPositions[] s_y =
        {
            StickerPositions.U, // U
            StickerPositions.D, // D
            StickerPositions.L, // F
            StickerPositions.R, // B
            StickerPositions.F, // R
            StickerPositions.B, // L
            StickerPositions.UL_U, // UF_U
            StickerPositions.UL_L, // UF_F
            StickerPositions.UF_U, // UR_U
            StickerPositions.UF_F, // UR_R
            StickerPositions.UR_U, // UB_U
            StickerPositions.UR_R, // UB_B
            StickerPositions.UB_U, // UL_U
            StickerPositions.UB_B, // UL_L
            StickerPositions.LF_L, // FR_F
            StickerPositions.LF_F, // FR_R
            StickerPositions.FR_F, // RB_R
            StickerPositions.FR_R, // RB_B
            StickerPositions.RB_R, // BL_B
            StickerPositions.RB_B, // BL_L
            StickerPositions.BL_B, // LF_L
            StickerPositions.BL_L, // LF_F
            StickerPositions.DL_D, // DF_D
            StickerPositions.DL_L, // DF_F
            StickerPositions.DF_D, // DR_D
            StickerPositions.DF_F, // DR_R
            StickerPositions.DR_D, // DB_D
            StickerPositions.DR_R, // DB_B
            StickerPositions.DB_D, // DL_D
            StickerPositions.DB_B, // DL_L
            StickerPositions.UFL_U, // UFR_U
            StickerPositions.UFL_L, // UFR_F
            StickerPositions.UFL_F, // UFR_R
            StickerPositions.UFR_U, // UBR_U
            StickerPositions.UFR_R, // UBR_B
            StickerPositions.UFR_F, // UBR_R
            StickerPositions.UBR_U, // UBL_U
            StickerPositions.UBR_R, // UBL_B
            StickerPositions.UBR_B, // UBL_L
            StickerPositions.UBL_U, // UFL_U
            StickerPositions.UBL_L, // UFL_F
            StickerPositions.UBL_B, // UFL_L
            StickerPositions.DFL_D, // DFR_D
            StickerPositions.DFL_L, // DFR_F
            StickerPositions.DFL_F, // DFR_R
            StickerPositions.DFR_D, // DBR_D
            StickerPositions.DFR_R, // DBR_B
            StickerPositions.DFR_F, // DBR_R
            StickerPositions.DBR_D, // DBL_D
            StickerPositions.DBR_R, // DBL_B
            StickerPositions.DBR_B, // DBL_L
            StickerPositions.DBL_D, // DFL_D
            StickerPositions.DBL_L, // DFL_F
            StickerPositions.DBL_B  // DFL_L
        };

        /// <summary>
        /// Array for stickers y2 turn (use for turning neutral stickers only, assigned in static constructor)
        /// </summary>
        private static readonly StickerPositions[] s_y2;

        /// <summary>
        /// Array for stickers y' turn (use for turning neutral stickers only, assigned in static constructor)
        /// </summary>
        private static readonly StickerPositions[] s_yp;

        /// <summary>
        /// Array for stickers z turn (use for turning neutral stickers only)
        /// </summary>
        private static readonly StickerPositions[] s_z =
        {
            StickerPositions.R, // U
            StickerPositions.L, // D
            StickerPositions.F, // F
            StickerPositions.B, // B
            StickerPositions.D, // R
            StickerPositions.U, // L
            StickerPositions.FR_R, // UF_U
            StickerPositions.FR_F, // UF_F
            StickerPositions.DR_R, // UR_U
            StickerPositions.DR_D, // UR_R
            StickerPositions.RB_R, // UB_U
            StickerPositions.RB_B, // UB_B
            StickerPositions.UR_R, // UL_U
            StickerPositions.UR_U, // UL_L
            StickerPositions.DF_F, // FR_F
            StickerPositions.DF_D, // FR_R
            StickerPositions.DB_D, // RB_R
            StickerPositions.DB_B, // RB_B
            StickerPositions.UB_B, // BL_B
            StickerPositions.UB_U, // BL_L
            StickerPositions.UF_U, // LF_L
            StickerPositions.UF_F, // LF_F
            StickerPositions.LF_L, // DF_D
            StickerPositions.LF_F, // DF_F
            StickerPositions.DL_L, // DR_D
            StickerPositions.DL_D, // DR_R
            StickerPositions.BL_L, // DB_D
            StickerPositions.BL_B, // DB_B
            StickerPositions.UL_L, // DL_D
            StickerPositions.UL_U, // DL_L
            StickerPositions.DFR_R, // UFR_U
            StickerPositions.DFR_F, // UFR_F
            StickerPositions.DFR_D, // UFR_R
            StickerPositions.DBR_R, // UBR_U
            StickerPositions.DBR_B, // UBR_B
            StickerPositions.DBR_D, // UBR_R
            StickerPositions.UBR_R, // UBL_U
            StickerPositions.UBR_B, // UBL_B
            StickerPositions.UBL_U, // UBL_L
            StickerPositions.UFR_R, // UFL_U
            StickerPositions.UFR_F, // UFL_F
            StickerPositions.UFR_U, // UFL_L
            StickerPositions.DFL_L, // DFR_D
            StickerPositions.DFL_F, // DFR_F
            StickerPositions.DFL_D, // DFR_R
            StickerPositions.DBL_L, // DBR_D
            StickerPositions.DBL_B, // DBR_B
            StickerPositions.DBL_D, // DBR_R
            StickerPositions.UBL_L, // DBL_D
            StickerPositions.UBL_B, // DBL_B
            StickerPositions.UBL_U, // DBL_L
            StickerPositions.UFL_L, // DFL_D
            StickerPositions.UFL_F, // DFL_F
            StickerPositions.UFL_U  // DFL_L
        };

        /// <summary>
        /// Array for stickers z2 turn (use for turning neutral stickers only, assigned in static constructor)
        /// </summary>
        private static readonly StickerPositions[] s_z2;

        /// <summary>
        /// Array for stickers z' turn (use for turning neutral stickers only, assigned in static constructor)
        /// </summary>
        private static readonly StickerPositions[] s_zp;

        #endregion Arrays for stickers positions

        #region Arrays to know in witch layer is each piece 

        /// <summary>
        /// Array to know if a piece is in layer U
        /// </summary>
        private static readonly bool[] PieceIsInLayerU =
        {
            true, false, false, false, false, false, // Centers
            true, true, true, true, // Up layer edges
            false, false, false, false, // Equator layer edges
            false, false, false, false, // Down layer edges
            true, true, true, true, // Up layer corners
            false, false, false, false // Down layer corners 
        };

        /// <summary>
        /// Array to know if a piece is in layer D
        /// </summary>
        private static readonly bool[] PieceIsInLayerD =
        {
            false, true, false, false, false, false, // Centers
            false, false, false, false, // Up layer edges
            false, false, false, false, // Equator layer edges
            true, true, true, true, // Down layer edges
            false, false, false, false, // Up layer corners
            true, true, true, true // Down layer corners 
        };

        /// <summary>
        /// Array to know if a piece is in layer F
        /// </summary>
        private static readonly bool[] PieceIsInLayerF =
        {
            false, false, true, false, false, false, // Centers
            true, false, false, false, // Up layer edges
            true, false, false, true, // Equator layer edges
            true, false, false, false, // Down layer edges
            true, false, false, true, // Up layer corners
            true, false, false, true // Down layer corners 
        };

        /// <summary>
        /// Array to know if a piece is in layer B
        /// </summary>
        private static readonly bool[] PieceIsInLayerB =
        {
            false, false, false, true, false, false, // Centers
            false, false, true, false, // Up layer edges
            false, true, true, false, // Equator layer edges
            false, false, true, false, // Down layer edges
            false, true, true, false, // Up layer corners
            false, true, true, false // Down layer corners 
        };

        /// <summary>
        /// Array to know if a piece is in layer R
        /// </summary>
        private static readonly bool[] PieceIsInLayerR =
        {
            false, false, false, false, true, false, // Centers
            false, true, false, false, // Up layer edges
            true, true, false, false, // Equator layer edges
            false, true, false, false, // Down layer edges
            true, true, false, false, // Up layer corners
            true, true, false, false // Down layer corners 
        };

        /// <summary>
        /// Array to know if a piece is in layer L
        /// </summary>
        private static readonly bool[] PieceIsInLayerL =
        {
            false, false, false, false, false, true, // Centers
            false, false, false, true, // Up layer edges
            false, false, true, true, // Equator layer edges
            false, false, false, true, // Down layer edges
            false, false, true, true, // Up layer corners
            false, false, true, true // Down layer corners 
        };

        /// <summary>
        /// Array to know if a piece is in layer E
        /// </summary>
        private static readonly bool[] PieceIsInLayerE =
        {
            false, false, true, true, true, true, // Centers
            false, false, false, false, // Up layer edges
            true, true, true, true, // Equator layer edges
            false, false, false, false, // Down layer edges
            false, false, false, false, // Up layer corners
            false, false, false, false // Down layer corners 
        };

        /// <summary>
        /// Array to know if a piece is in layer S
        /// </summary>
        private static readonly bool[] PieceIsInLayerS =
        {
            true, true, false, false, true, true, // Centers
            false, true, false, true, // Up layer edges
            false, false, false, false, // Equator layer edges
            false, true, false, true, // Down layer edges
            false, false, false, false, // Up layer corners
            false, false, false, false // Down layer corners 
        };

        /// <summary>
        /// Array to know if a piece is in layer M
        /// </summary>
        private static readonly bool[] PieceIsInLayerM =
        {
            true, true, true, true, false, false, // Centers
            true, false, true, false, // Up layer edges
            false, false, false, false, // Equator layer edges
            true, false, true, false, // Down layer edges
            false, false, false, false, // Up layer corners
            false, false, false, false // Down layer corners 
        };

        #endregion Arrays to know in witch layer is each piece 

        #region Arrays to know sticker faces and pieces

        /// <summary>
        /// Array of faces for sticker positions
        /// </summary>
        private static readonly Faces[] StickerFace =
        {
            Faces.U, Faces.D, Faces.F, Faces.B, Faces.R, Faces.L, // Centers
            Faces.U, Faces.F, Faces.U, Faces.R, Faces.U, Faces.B, Faces.U, Faces.L, // Upper layer edges
            Faces.F, Faces.R, Faces.R, Faces.B, Faces.B, Faces.L, Faces.L, Faces.F, // Middle layer edges
            Faces.D, Faces.F, Faces.D, Faces.R, Faces.D, Faces.B, Faces.D, Faces.L, // Lower layer edges
            Faces.U, Faces.F, Faces.R, Faces.U, Faces.B, Faces.R, Faces.U, Faces.B, Faces.L, Faces.U, Faces.F, Faces.L, // Upper layer corners
            Faces.D, Faces.F, Faces.R, Faces.D, Faces.B, Faces.R, Faces.D, Faces.B, Faces.L, Faces.D, Faces.F, Faces.L, // Lower layer corners
        };

        /// <summary>
        /// Array of pieces for each sticker
        /// </summary>
        private static readonly Pieces[] StickerPiece =
        {
            Pieces.U, Pieces.D, Pieces.F, Pieces.B, Pieces.R, Pieces.L, // Centers
            Pieces.UF, Pieces.UF, Pieces.UR, Pieces.UR, Pieces.UB, Pieces.UB, Pieces.UL, Pieces.UL, // Upper layer edges
            Pieces.FR, Pieces.FR, Pieces.RB, Pieces.RB, Pieces.BL, Pieces.BL, Pieces.LF, Pieces.LF, // Middle layer edges
            Pieces.DF, Pieces.DF, Pieces.DR, Pieces.DR, Pieces.DB, Pieces.DB, Pieces.DL, Pieces.DL, // Lower layer edges
            Pieces.UFR, Pieces.UFR, Pieces.UFR, Pieces.UBR, Pieces.UBR, Pieces.UBR,
            Pieces.UBL, Pieces.UBL, Pieces.UBL, Pieces.UFL, Pieces.UFL, Pieces.UFL, // Upper layer corners
            Pieces.DFR, Pieces.DFR, Pieces.DFR, Pieces.DBR, Pieces.DBR, Pieces.DBR,
            Pieces.DBL, Pieces.DBL, Pieces.DBL, Pieces.DFL, Pieces.DFL, Pieces.DFL // Lower layer corners
        };

        #endregion Arrays to know sticker faces and pieces

        #endregion Static arrays

        #region Fields

        /// <summary>
        /// Current sticker
        /// </summary>
        private readonly StickerPositions SolvedSticker;

        /// <summary>
        /// Sticker position
        /// </summary>
        private StickerPositions StickerPosition;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Current sticker (named as his position in solved condition)
        /// </summary>
        public StickerPositions Sticker => SolvedSticker;

        /// <summary>
        /// Current sticker position
        /// </summary>
        public StickerPositions Position => StickerPosition;

        /// <summary>
        /// Sticker piece
        /// </summary>
        public Pieces Piece => StickerPiece[(int)SolvedSticker];

        /// <summary>
        /// Piece position where the sticker piece is currently located
        /// </summary>
        public Pieces PiecePosition => StickerPiece[(int)StickerPosition];

        /// <summary>
        /// Current sticker face (color)
        /// </summary>
        public Faces Face => StickerFace[(int)Sticker];

        /// <summary>
        /// Face where the sticker is currently located
        /// </summary>
        public Faces FacePosition => StickerFace[(int)StickerPosition];

        /// <summary>
        /// Current sticker is located in layer U
        /// </summary>
        public bool IsInLayerU => PieceIsInLayerU[(int)PiecePosition];

        /// <summary>
        /// Current sticker is located in layer D
        /// </summary>
        public bool IsInLayerD => PieceIsInLayerD[(int)PiecePosition];

        /// <summary>
        /// Current sticker is located in layer F
        /// </summary>
        public bool IsInLayerF => PieceIsInLayerF[(int)PiecePosition];

        /// <summary>
        /// Current sticker is located in layer B
        /// </summary>
        public bool IsInLayerB => PieceIsInLayerB[(int)PiecePosition];

        /// <summary>
        /// Current sticker is located in layer R
        /// </summary>
        public bool IsInLayerR => PieceIsInLayerR[(int)PiecePosition];

        /// <summary>
        /// Current sticker is located in layer L
        /// </summary>
        public bool IsInLayerL => PieceIsInLayerL[(int)PiecePosition];

        /// <summary>
        /// Current sticker is located in layer E
        /// </summary>
        public bool IsInLayerE => PieceIsInLayerE[(int)PiecePosition];

        /// <summary>
        /// Current sticker is located in layer S
        /// </summary>
        public bool IsInLayerS => PieceIsInLayerS[(int)PiecePosition];

        /// <summary>
        /// Current sticker is located in layer M
        /// </summary>
        public bool IsInLayerM => PieceIsInLayerM[(int)PiecePosition];

        /// <summary>
        /// True if sticker is a center
        /// </summary>
        public bool IsCenter => SolvedSticker <= StickerPositions.L;

        /// <summary>
        /// True if sticker is an edge
        /// </summary>
        public bool IsEdge => SolvedSticker >= StickerPositions.UF_U && Sticker <= StickerPositions.DL_L;

        /// <summary>
        /// True if sticker is a corner
        /// </summary>
        public bool IsCorner => SolvedSticker >= StickerPositions.UFR_U;

        /// <summary>
        /// True if sticker is in his solved position
        /// </summary>
        public bool IsSolved => SolvedSticker == StickerPosition;

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Static constructor
        /// </summary>
        static StickerData()
        {
            int l = s_U.Length;

            s_U2 = new StickerPositions[l];
            s_Up = new StickerPositions[l];
            s_D2 = new StickerPositions[l];
            s_Dp = new StickerPositions[l];
            s_F2 = new StickerPositions[l];
            s_Fp = new StickerPositions[l];
            s_B2 = new StickerPositions[l];
            s_Bp = new StickerPositions[l];
            s_R2 = new StickerPositions[l];
            s_Rp = new StickerPositions[l];
            s_L2 = new StickerPositions[l];
            s_Lp = new StickerPositions[l];

            s_x2 = new StickerPositions[l];
            s_xp = new StickerPositions[l];
            s_y2 = new StickerPositions[l];
            s_yp = new StickerPositions[l];
            s_z2 = new StickerPositions[l];
            s_zp = new StickerPositions[l];

            for (int n = 0; n < l; n++)
            {
                s_U2[n] = s_U[(int)s_U[n]];
                s_Up[n] = s_U[(int)s_U2[n]];
                s_D2[n] = s_D[(int)s_D[n]];
                s_Dp[n] = s_D[(int)s_D2[n]];
                s_F2[n] = s_F[(int)s_F[n]];
                s_Fp[n] = s_F[(int)s_F2[n]];
                s_B2[n] = s_B[(int)s_B[n]];
                s_Bp[n] = s_B[(int)s_B2[n]];
                s_R2[n] = s_R[(int)s_R[n]];
                s_Rp[n] = s_R[(int)s_R2[n]];
                s_L2[n] = s_L[(int)s_L[n]];
                s_Lp[n] = s_L[(int)s_L2[n]];

                s_x2[n] = s_x[(int)s_x[n]];
                s_xp[n] = s_x[(int)s_x2[n]];
                s_y2[n] = s_y[(int)s_y[n]];
                s_yp[n] = s_y[(int)s_y2[n]];
                s_z2[n] = s_z[(int)s_z[n]];
                s_zp[n] = s_z[(int)s_z2[n]];
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="St">Sticker</param>
        public StickerData(StickerPositions St)
        {
            SolvedSticker = StickerPosition = St;
        }

        #endregion Constructors

        #region Functions

        /// <summary>
        /// Reset sticker to solved position
        /// </summary>
        public void Reset() => StickerPosition = Sticker;

        #endregion Functions

        #region Sticker movements

        /// <summary>
        /// Sticker U movement
        /// </summary>
        public void U() => StickerPosition = s_U[(int)StickerPosition];

        /// <summary>
        /// Sticker D movement
        /// </summary>
        public void D() => StickerPosition = s_D[(int)StickerPosition];

        /// <summary>
        /// Sticker F movement
        /// </summary>
        public void F() => StickerPosition = s_F[(int)StickerPosition];

        /// <summary>
        /// Sticker B movement
        /// </summary>
        public void B() => StickerPosition = s_B[(int)StickerPosition];

        /// <summary>
        /// Sticker R movement
        /// </summary>
        public void R() => StickerPosition = s_R[(int)StickerPosition];

        /// <summary>
        /// Sticker L movement
        /// </summary>
        public void L() => StickerPosition = s_L[(int)StickerPosition];

        /// <summary>
        /// Sticker U' movement
        /// </summary>
        public void Up() => StickerPosition = s_Up[(int)StickerPosition];

        /// <summary>
        /// Sticker D' movement
        /// </summary>
        public void Dp() => StickerPosition = s_Dp[(int)StickerPosition];

        /// <summary>
        /// Sticker F' movement
        /// </summary>
        public void Fp() => StickerPosition = s_Fp[(int)StickerPosition];

        /// <summary>
        /// Sticker B' movement
        /// </summary>
        public void Bp() => StickerPosition = s_Bp[(int)StickerPosition];

        /// <summary>
        /// Sticker R' movement
        /// </summary>
        public void Rp() => StickerPosition = s_Rp[(int)StickerPosition];

        /// <summary>
        /// Sticker L' movement
        /// </summary>
        public void Lp() => StickerPosition = s_Lp[(int)StickerPosition];

        /// <summary>
        /// Sticker U2 movement
        /// </summary>
        public void U2() => StickerPosition = s_U2[(int)StickerPosition];

        /// <summary>
        /// Sticker D2 movement
        /// </summary>
        public void D2() => StickerPosition = s_D2[(int)StickerPosition];

        /// <summary>
        /// Sticker F2 movement
        /// </summary>
        public void F2() => StickerPosition = s_F2[(int)StickerPosition];

        /// <summary>
        /// Sticker B2 movement
        /// </summary>
        public void B2() => StickerPosition = s_B2[(int)StickerPosition];

        /// <summary>
        /// Sticker R2 movement
        /// </summary>
        public void R2() => StickerPosition = s_R2[(int)StickerPosition];

        /// <summary>
        /// Sticker L2 movement
        /// </summary>
        public void L2() => StickerPosition = s_L2[(int)StickerPosition];

        #endregion Sticker movements

        #region Static functions

        /// <summary>
        /// Get the piece of given sticker position
        /// </summary>
        /// <param name="Pos">Sticker position</param>
        /// <returns>Sticker piece</returns>
        public static Pieces GetStickerPiece(StickerPositions Pos) => StickerPiece[(int)Pos];

        /// <summary>
        /// Get the face of the sticker in solved position
        /// </summary>
        /// <param name="Pos">Sticker position</param>
        /// <returns>Edge sticker face</returns>
        public static Faces GetStickerFace(StickerPositions Pos) => StickerFace[(int)Pos];

        /// <summary>
        /// Check if a sticker is a center
        /// </summary>
        /// <param name="S">Sticker position</param>
        /// <returns>True if sticker is a center</returns>
        public static bool StickerIsCenter(StickerPositions S) => S <= StickerPositions.L;

        /// <summary>
        /// Check if a sticker is a edge
        /// </summary>
        /// <param name="S">Sticker position</param>
        /// <returns>True if sticker position is a edge</returns>
        public static bool StickerIsEdge(StickerPositions S) => S >= StickerPositions.UF_U && S <= StickerPositions.DL_L;

        /// <summary>
        /// Check if a sticker is a corner
        /// </summary>
        /// <param name="S">Sticker position</param>
        /// <returns>True if sticker position is a corner</returns>
        public static bool StickerIsCorner(StickerPositions S) => S >= StickerPositions.UFR_U;

        /// <summary>
        /// Gets the name of the given piece
        /// </summary>
        /// <param name="p">Piece</param>
        /// <returns>Piece name</returns>
        public static string GetPieceName(Pieces p)
        {
            switch (p)
            {
                case Pieces.U: return AMSettings.UChar.ToString();
                case Pieces.D: return AMSettings.DChar.ToString();
                case Pieces.F: return AMSettings.FChar.ToString();
                case Pieces.B: return AMSettings.BChar.ToString();
                case Pieces.R: return AMSettings.RChar.ToString();
                case Pieces.L: return AMSettings.LChar.ToString();

                case Pieces.UF: return AMSettings.UChar.ToString() + AMSettings.FChar;
                case Pieces.UR: return AMSettings.UChar.ToString() + AMSettings.RChar;
                case Pieces.UB: return AMSettings.UChar.ToString() + AMSettings.BChar;
                case Pieces.UL: return AMSettings.UChar.ToString() + AMSettings.LChar;

                case Pieces.FR: return AMSettings.FChar.ToString() + AMSettings.RChar;
                case Pieces.RB: return AMSettings.RChar.ToString() + AMSettings.BChar;
                case Pieces.BL: return AMSettings.BChar.ToString() + AMSettings.LChar;
                case Pieces.LF: return AMSettings.LChar.ToString() + AMSettings.FChar;

                case Pieces.DF: return AMSettings.DChar.ToString() + AMSettings.FChar;
                case Pieces.DR: return AMSettings.DChar.ToString() + AMSettings.RChar;
                case Pieces.DB: return AMSettings.DChar.ToString() + AMSettings.BChar;
                case Pieces.DL: return AMSettings.DChar.ToString() + AMSettings.LChar;

                case Pieces.UFR: return AMSettings.UChar.ToString() + AMSettings.FChar + AMSettings.RChar;
                case Pieces.UBR: return AMSettings.UChar.ToString() + AMSettings.BChar + AMSettings.RChar;
                case Pieces.UBL: return AMSettings.UChar.ToString() + AMSettings.BChar + AMSettings.LChar;
                case Pieces.UFL: return AMSettings.UChar.ToString() + AMSettings.FChar + AMSettings.LChar;

                case Pieces.DFR: return AMSettings.DChar.ToString() + AMSettings.FChar + AMSettings.RChar;
                case Pieces.DBR: return AMSettings.DChar.ToString() + AMSettings.BChar + AMSettings.RChar;
                case Pieces.DBL: return AMSettings.DChar.ToString() + AMSettings.BChar + AMSettings.LChar;
                case Pieces.DFL: return AMSettings.DChar.ToString() + AMSettings.FChar + AMSettings.LChar;

                default: return string.Empty;
            }
        }

        /// <summary>
        /// Rotate an sticker position as x rotation (use only for rotate neutral stickers positions) 
        /// </summary>
        /// <param name="sp">Sticker position</param>
        /// <returns>x rotated sticker position</returns>
        public static StickerPositions RotateX(StickerPositions sp) => s_xp[(int)sp];

        /// <summary>
        /// Rotate an sticker position as x' rotation (use only for rotate neutral stickers positions) 
        /// </summary>
        /// <param name="sp">Sticker position</param>
        /// <returns>x' rotated sticker position</returns>
        public static StickerPositions RotateXp(StickerPositions sp) => s_x[(int)sp];

        /// <summary>
        /// Rotate an sticker position as x2 rotation (use only for rotate neutral stickers positions) 
        /// </summary>
        /// <param name="sp">Sticker position</param>
        /// <returns>x2 rotated sticker position</returns>
        public static StickerPositions RotateX2(StickerPositions sp) => s_x2[(int)sp];

        /// <summary>
        /// Rotate an sticker position as y rotation (use only for rotate neutral stickers positions) 
        /// </summary>
        /// <param name="sp">Sticker position</param>
        /// <returns>y rotated sticker position</returns>
        public static StickerPositions RotateY(StickerPositions sp) => s_yp[(int)sp];

        /// <summary>
        /// Rotate an sticker position as y' rotation (use only for rotate neutral stickers positions) 
        /// </summary>
        /// <param name="sp">Sticker position</param>
        /// <returns>y' rotated sticker position</returns>
        public static StickerPositions RotateYp(StickerPositions sp) => s_y[(int)sp];

        /// <summary>
        /// Rotate an sticker position as y2 rotation (use only for rotate neutral stickers positions) 
        /// </summary>
        /// <param name="sp">Sticker position</param>
        /// <returns>y2 rotated sticker position</returns>
        public static StickerPositions RotateY2(StickerPositions sp) => s_y2[(int)sp];

        /// <summary>
        /// Rotate an sticker position as z rotation (use only for rotate neutral stickers positions) 
        /// </summary>
        /// <param name="sp">Sticker position</param>
        /// <returns>z rotated sticker position</returns>
        public static StickerPositions RotateZ(StickerPositions sp) => s_zp[(int)sp];

        /// <summary>
        /// Rotate an sticker position as z' rotation (use only for rotate neutral stickers positions) 
        /// </summary>
        /// <param name="sp">Sticker position</param>
        /// <returns>z' rotated sticker position</returns>
        public static StickerPositions RotateZp(StickerPositions sp) => s_z[(int)sp];

        /// <summary>
        /// Rotate an sticker position as z2 rotation (use only for rotate neutral stickers positions) 
        /// </summary>
        /// <param name="sp">Sticker position</param>
        /// <returns>z2 rotated sticker position</returns>
        public static StickerPositions RotateZ2(StickerPositions sp) => s_z2[(int)sp];

        #endregion Static functions
    }
}
