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
    /// Enum with all possible steps
    /// </summary>
    public enum Steps
    {
        NONE = 0,

        // Single layer
        U = 1, Up = 2, U2 = 3, U2p = 4,
        D = 5, Dp = 6, D2 = 7, D2p = 8,
        F = 9, Fp = 10, F2 = 11, F2p = 12,
        B = 13, Bp = 14, B2 = 15, B2p = 16,
        R = 17, Rp = 18, R2 = 19, R2p = 20,
        L = 21, Lp = 22, L2 = 23, L2p = 24,

        // Double layer, adjacent layers, same direction
        Uw = 25, Uwp = 26, Uw2 = 27, Uw2p = 28,
        Dw = 29, Dwp = 30, Dw2 = 31, Dw2p = 32,
        Fw = 33, Fwp = 34, Fw2 = 35, Fw2p = 36,
        Bw = 37, Bwp = 38, Bw2 = 39, Bw2p = 40,
        Rw = 41, Rwp = 42, Rw2 = 43, Rw2p = 44,
        Lw = 45, Lwp = 46, Lw2 = 47, Lw2p = 48,

        // Double layer, adjacent layers, opposite direction
        Uo = 49, Uop = 50, Uo2 = 51, Uo2p = 52,
        Do = 53, Dop = 54, Do2 = 55, Do2p = 56,
        Fo = 57, Fop = 58, Fo2 = 59, Fo2p = 60,
        Bo = 61, Bop = 62, Bo2 = 63, Bo2p = 64,
        Ro = 65, Rop = 66, Ro2 = 67, Ro2p = 68,
        Lo = 69, Lop = 70, Lo2 = 71, Lo2p = 72,

        // Double layer, opposite layers, same direction
        Us = 73, Usp = 74, Us2 = 75, Us2p = 76,
        Ds = 77, Dsp = 78, Ds2 = 79, Ds2p = 80,
        Fs = 81, Fsp = 82, Fs2 = 83, Fs2p = 84,
        Bs = 85, Bsp = 86, Bs2 = 87, Bs2p = 88,
        Rs = 89, Rsp = 90, Rs2 = 91, Rs2p = 92,
        Ls = 93, Lsp = 94, Ls2 = 95, Ls2p = 96,

        // Double layer, opposite layers, opposite direction
        Ua = 97, Uap = 98, Ua2 = 99, Ua2p = 76,
        Da = 101, Dap = 102, Da2 = 103, Da2p = 104,
        Fa = 105, Fap = 106, Fa2 = 107, Fa2p = 108,
        Ba = 109, Bap = 110, Ba2 = 111, Ba2p = 112,
        Ra = 113, Rap = 114, Ra2 = 115, Ra2p = 116,
        La = 117, Lap = 118, La2 = 119, La2p = 120,
        
        // Middle layers
        E = 121, Ep = 122, E2 = 123, E2p = 124,
        S = 125, Sp = 126, S2 = 127, S2p = 128,
        M = 129, Mp = 130, M2 = 131, M2p = 132,

        // Full cube
        x = 133, xp = 134, x2 = 135, x2p = 136,
        y = 137, yp = 138, y2 = 139, y2p = 140,
        z = 141, zp = 142, z2 = 143, z2p = 144,

        // Parentheses
        OPEN_PARENTHESIS = 145, // Step is open parenthesis
        CLOSE_PARENTHESIS_1_REP = 146, // Step is close parenthesis
        CLOSE_PARENTHESIS_2_REP = 147, // Step is close parenthesis with two repetitions
        CLOSE_PARENTHESIS_3_REP = 148, // Step is close parenthesis with three repetitions
        CLOSE_PARENTHESIS_4_REP = 149, // Step is close parenthesis with four repetitions
        CLOSE_PARENTHESIS_5_REP = 150, // Step is close parenthesis with five repetitions
        CLOSE_PARENTHESIS_6_REP = 151, // Step is close parenthesis with six repetitions
        CLOSE_PARENTHESIS_7_REP = 152, // Step is close parenthesis with seven repetitions
        CLOSE_PARENTHESIS_8_REP = 153, // Step is close parenthesis with eight repetitions
        CLOSE_PARENTHESIS_9_REP = 154  // Step is close parenthesis with nine repetitions
    }

    /// <summary>
    /// Enumeration of 3x3x3 cube layers: up, down, front, back, right, left, equator, standing, middle 
    /// </summary>
    public enum Layers : int
    {
        NONE = -1, U = 0, D = 1, F = 2, B = 3, R = 4, L = 5, E = 6, S = 7, M = 8
    }

    /// <summary>
    ///  Enumeration of layer movements: none, 90 degrees clock wise rotation,
    ///  180 degrees clock wise rotation, 90 degrees counter clock wise rotation and 
    ///  180 degrees counter clock wise rotation
    /// </summary>
    public enum Movements : int
    {
        NONE = 0, ROT90CW = 90, ROT180CW = 180, ROT90CCW = -90, ROT180CCW = -180
    }

    /// <summary>
    /// Enumeration of layer modifiers and parentheses
    /// </summary>
    public enum Modifiers : int
    {
        SINGLE_LAYER = 0, // { U, D, F, B, R, L, E, S, M }
        DOUBLE_ADJACENT_LAYERS_SAME_DIRECTION = 1, // { Uw, Dw, Fw, Bw, Rw, Lw } or  { u, d, f, b, r, l }
        DOUBLE_ADJACENT_LAYERS_OPPOSITE_DIRECTION = 2, // { Uo, Do, Fo, Bo, Ro, Lo } VERY UNUSUAL!!!
        DOUBLE_OPPOSITE_LAYERS_SAME_DIRECTION = 3,  // { Us, Ds, Fs, Bs, Rs, Ls }
        DOUBLE_OPPOSITE_LAYERS_OPPOSITE_DIRECTION = 4, // { Ua, Da, Fa, Ba, Ra, La } UNUSUAL
        FULL_CUBE = 5, // { x, y, z }
        OPEN_PARENTHESIS = 10, // Step is open parenthesis
        CLOSE_PARENTHESIS_1_REP = 11, // Step is close parenthesis
        CLOSE_PARENTHESIS_2_REP = 12, // Step is close parenthesis with two repetitions
        CLOSE_PARENTHESIS_3_REP = 13, // Step is close parenthesis with three repetitions
        CLOSE_PARENTHESIS_4_REP = 14, // Step is close parenthesis with four repetitions
        CLOSE_PARENTHESIS_5_REP = 15, // Step is close parenthesis with five repetitions
        CLOSE_PARENTHESIS_6_REP = 16, // Step is close parenthesis with six repetitions
        CLOSE_PARENTHESIS_7_REP = 17, // Step is close parenthesis with seven repetitions
        CLOSE_PARENTHESIS_8_REP = 18, // Step is close parenthesis with eight repetitions
        CLOSE_PARENTHESIS_9_REP = 19  // Step is close parenthesis with nine repetitions
    }

    /// <summary>
    /// Rubik's cube faces: Up, Down, Front, Back, Right, Left
    /// </summary>
    public enum Faces : int
    {
        U = 0, D = 1, F = 2, B = 3, R = 4, L = 5
    } 

    /// <summary>
    /// Rubik's cube edge stickers positions 
    /// </summary>
    public enum EdgeStickersPositions : int
    {
        UF_U = 0, UF_F = 1, UR_U = 2, UR_R = 3, UB_U = 4, UB_B = 5, UL_U = 6, UL_L = 7, // Upper layer edges
        FR_F = 8, FR_R = 9, RB_R = 10, RB_B = 11, BL_B = 12, BL_L = 13, LF_L = 14, LF_F = 15, // Middle layer edges
        DF_D = 16, DF_F = 17, DR_D = 18, DR_R = 19, DB_D = 20, DB_B = 21, DL_D = 22, DL_L = 23 // Lower layer edges
    }

    /// <summary>
    /// Rubik's cube corner stickers positions
    /// </summary>
    public enum CornerStickersPositions : int
    {
        UFR_U = 0, UFR_F = 1, UFR_R = 2, UBR_U = 3, UBR_B = 4, UBR_R = 5,
        UBL_U = 6, UBL_B = 7, UBL_L = 8, UFL_U = 9, UFL_F = 10, UFL_L = 11, // Upper layer corners
        DFR_D = 12, DFR_F = 13, DFR_R = 14, DBR_D = 15, DBR_B = 16, DBR_R = 17,
        DBL_D = 18, DBL_B = 19, DBL_L = 20, DFL_D = 21, DFL_F = 22, DFL_L = 23 // Lower layer corners
    }

    /// <summary>
    /// Rubik's cube all sticker positions
    /// </summary>
    public enum StickerPositions : int
    {
        U = 0, D = 1, F = 2, B = 3, R = 4, L = 5, // Centers
        UF_U = 6, UF_F = 7, UR_U = 8, UR_R = 9, UB_U = 10, UB_B = 11, UL_U = 12, UL_L = 13, // Upper layer edges
        FR_F = 14, FR_R = 15, RB_R = 16, RB_B = 17, BL_B = 18, BL_L = 19, LF_L = 20, LF_F = 21, // Middle layer edges
        DF_D = 22, DF_F = 23, DR_D = 24, DR_R = 25, DB_D = 26, DB_B = 27, DL_D = 28, DL_L = 29, // Lower layer edges
        UFR_U = 30, UFR_F = 31, UFR_R = 32, UBR_U = 33, UBR_B = 34, UBR_R = 35, UBL_U = 36, UBL_B = 37, UBL_L = 38, UFL_U = 39, UFL_F = 40, UFL_L = 41, // Upper layer corners
        DFR_D = 42, DFR_F = 43, DFR_R = 44, DBR_D = 45, DBR_B = 46, DBR_R = 47, DBL_D = 48, DBL_B = 49, DBL_L = 50, DFL_D = 51, DFL_F = 52, DFL_L = 53 // Lower layer corners
    }

    /// <summary>
    /// Rubik's cube U layer stickers positions
    /// </summary>
    public enum StickersULayer : int
    {
        U = 0, // Center
        UF_U = 6, UF_F = 7, UR_U = 8, UR_R = 9, UB_U = 10, UB_B = 11, UL_U = 12, UL_L = 13, // Upper layer edges
        UFR_U = 30, UFR_F = 31, UFR_R = 32, UBR_U = 33, UBR_B = 34, UBR_R = 35, UBL_U = 36, UBL_B = 37, UBL_L = 38, UFL_U = 39, UFL_F = 40, UFL_L = 41, // Upper layer corners
    }

    /// <summary>
    /// Rubik's cube D layer stickers positions
    /// </summary>
    public enum StickersDLayer : int
    {
        D = 1, // Center
        DF_D = 22, DF_F = 23, DR_D = 24, DR_R = 25, DB_D = 26, DB_B = 27, DL_D = 28, DL_L = 29, // Lower layer edges
        DFR_D = 42, DFR_F = 43, DFR_R = 44, DBR_D = 45, DBR_B = 46, DBR_R = 47, DBL_D = 48, DBL_B = 49, DBL_L = 50, DFL_D = 51, DFL_F = 52, DFL_L = 53 // Lower layer corners
    }

    /// <summary>
    /// Rubik's cube F layer stickers positions
    /// </summary>
    public enum StickersFLayer : int
    {
        F = 2, // Center
        UF_U = 6, UF_F = 7, // Upper layer edges
        FR_F = 14, FR_R = 15, LF_L = 20, LF_F = 21, // Middle layer edges
        DF_D = 22, DF_F = 23, // Lower layer edges
        UFR_U = 30, UFR_F = 31, UFR_R = 32, UFL_U = 39, UFL_F = 40, UFL_L = 41, // Upper layer corners
        DFR_D = 42, DFR_F = 43, DFR_R = 44, DFL_D = 51, DFL_F = 52, DFL_L = 53 // Lower layer corners
    }

    /// <summary>
    /// Rubik's cube B layer stickers positions
    /// </summary>
    public enum StickersBLayer : int
    {
        B = 3, // Center
        UB_U = 10, UB_B = 11, // Upper layer edges
        RB_R = 16, RB_B = 17, BL_B = 18, BL_L = 19, // Middle layer edges
        DB_D = 26, DB_B = 27, // Lower layer edges
        UBR_U = 33, UBR_B = 34, UBR_R = 35, UBL_U = 36, UBL_B = 37, UBL_L = 38, // Upper layer corners
        DBR_D = 45, DBR_B = 46, DBR_R = 47, DBL_D = 48, DBL_B = 49, DBL_L = 50 // Lower layer corners
    }

    /// <summary>
    /// Rubik's cube R layer stickers positions
    /// </summary>
    public enum StickersRLayer : int
    {
        R = 4, // Center
        UR_U = 8, UR_R = 9, // Upper layer edges
        FR_F = 14, FR_R = 15, RB_R = 16, RB_B = 17, // Middle layer edges
        DR_D = 24, DR_R = 25, // Lower layer edges
        UFR_U = 30, UFR_F = 31, UFR_R = 32, UBR_U = 33, UBR_B = 34, UBR_R = 35, // Upper layer corners
        DFR_D = 42, DFR_F = 43, DFR_R = 44, DBR_D = 45, DBR_B = 46, DBR_R = 47 // Lower layer corners
    }

    /// <summary>
    /// Rubik's cube L layer stickers positions
    /// </summary>
    public enum StickersLLayer : int
    {
        L = 5, // Center
        UL_U = 12, UL_L = 13, // Upper layer edges
        BL_B = 18, BL_L = 19, LF_L = 20, LF_F = 21, // Middle layer edges
        DL_D = 28, DL_L = 29, // Lower layer edges
        UBL_U = 36, UBL_B = 37, UBL_L = 38, UFL_U = 39, UFL_F = 40, UFL_L = 41, // Upper layer corners
        DBL_D = 48, DBL_B = 49, DBL_L = 50, DFL_D = 51, DFL_F = 52, DFL_L = 53 // Lower layer corners
    }

    /// <summary>
    /// Rubik's cube E layer sticker positions
    /// </summary>
    public enum StickersELayer : int
    {
        F = 2, B = 3, R = 4, L = 5, // Centers
        FR_F = 14, FR_R = 15, RB_R = 16, RB_B = 17, BL_B = 18, BL_L = 19, LF_L = 20, LF_F = 21 // Middle layer edges
    }

    /// <summary>
    /// Rubik's cube S layer sticker positions
    /// </summary>
    public enum StickersSLayer : int
    {
        U = 0, D = 1, R = 4, L = 5, // Centers
        UR_U = 8, UR_R = 9, UL_U = 12, UL_L = 13, // Upper layer edges
        DR_D = 24, DR_R = 25, DL_D = 28, DL_L = 29 // Lower layer edges
    }

    /// <summary>
    /// Rubik's cube M layer sticker positions
    /// </summary>
    public enum StickersMLayer : int
    {
        U = 0, D = 1, F = 2, B = 3, // Centers
        UF_U = 6, UF_F = 7, UB_U = 10, UB_B = 11, // Upper layer edges
        DF_D = 22, DF_F = 23, DB_D = 26, DB_B = 27 // Lower layer edges
    }

    /// <summary>
    /// Rubik's cube U Face stickers positions
    /// </summary>
    public enum StickersUFace : int
    {
        U = 0, // Center
        UF_U = 6, UR_U = 8, UB_U = 10, UL_U = 12, // Face edges
        UFR_U = 30, UBR_U = 33, UBL_U = 36, UFL_U = 39 // Face corners
    }

    /// <summary>
    /// Rubik's cube D Face stickers positions
    /// </summary>
    public enum StickersDFace : int
    {
        D = 1, // Center
        DF_D = 22, DR_D = 24, DB_D = 26, DL_D = 28, // Face edges
        DFR_D = 42, DBR_D = 45, DBL_D = 48, DFL_D = 51 // Face corners
    }

    /// <summary>
    /// Rubik's cube F Face stickers positions
    /// </summary>
    public enum StickersFFace : int
    {
        F = 2, // Center
        UF_F = 7, FR_F = 14, LF_F = 21, DF_F = 23, // Face edges
        UFR_F = 31, UFL_F = 40, DFR_F = 43, DFL_F = 52 // Face corners
    }

    /// <summary>
    /// Rubik's cube B Face stickers positions
    /// </summary>
    public enum StickersBFace : int
    {
        B = 3, // Center
        UB_B = 11, RB_B = 17, BL_B = 18, DB_B = 27,// Face edges
        UBR_B = 34, UBL_B = 37, DBR_B = 46, DBL_B = 49 // Face corners
    }

    /// <summary>
    /// Rubik's cube R Face stickers positions
    /// </summary>
    public enum StickersRFace : int
    {
        R = 4, // Center
        UR_R = 9, FR_R = 15, RB_R = 16, DR_R = 25, // Face edges
        UFR_R = 32, UBR_R = 35, DFR_R = 44, DBR_R = 47 // Face corners
    }

    /// <summary>
    /// Rubik's cube L Face stickers positions
    /// </summary>
    public enum StickersLFace : int
    {
        L = 5, // Center
        UL_L = 13, BL_L = 19, LF_L = 20, DL_L = 29, // Face edges
        UBL_L = 38, UFL_L = 41, DBL_L = 50, DFL_L = 53 // Lower Face corners
    }

    /// <summary>
    /// All cube possible spins as faces oriented to up and front
    /// </summary>
    public enum CubeSpins : int
    {
        UF = 0, UR = 1, UB = 2, UL = 3,
        DF = 4, DR = 5, DB = 6, DL = 7,
        FU = 8, FR = 9, FD = 10, FL = 11,
        BU = 12, BR = 13, BD = 14, BL = 15,
        RU = 16, RF = 17, RD = 18, RB = 19,
        LU = 20, LF = 21, LD = 22, LB = 23
    }

    /// <summary>
    /// All cube pieces
    /// </summary>
    public enum Pieces : int
    {
        U = 0, D = 1, F = 2, B = 3, R = 4, L = 5, // Centers
        UF = 6, UR = 7, UB = 8, UL = 9, // Up layer edges
        FR = 10, RB = 11, BL = 12, LF = 13, // Equator layer edges
        DF = 14, DR = 15, DB = 16, DL = 17, // Down layer edges
        UFR = 18, UBR = 19, UBL = 20, UFL = 21, // Up layer corners
        DFR = 22, DBR = 23, DBL = 24, DFL = 25 // Down layer corners 
    }
}
