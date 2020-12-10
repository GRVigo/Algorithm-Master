/*  This file is part of "Algorithm Master"
  
    Copyright (C) 2018 Germán Ramos Rodríguez

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
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
    /// Static class to manage an scramble step
    /// </summary>
    public static class ScrambleStep
    {
        #region Static arrays

        /// <summary>
        /// Array for steps no movement (for reference only)
        /// </summary>
        private static readonly Steps[] s_ident =
        {
            Steps.NONE,

            // Single layer
            Steps.U, Steps.Up, Steps.U2, Steps.U2p,
            Steps.D, Steps.Dp, Steps.D2, Steps.D2p,
            Steps.F, Steps.Fp, Steps.F2, Steps.F2p,
            Steps.B, Steps.Bp, Steps.B2, Steps.B2p,
            Steps.R, Steps.Rp, Steps.R2, Steps.R2p,
            Steps.L, Steps.Lp, Steps.L2, Steps.L2p,

            // Double layer, adjacent layers, same direction
            Steps.Uw, Steps.Uwp, Steps.Uw2, Steps.Uw2p,
            Steps.Dw, Steps.Dwp, Steps.Dw2, Steps.Dw2p,
            Steps.Fw, Steps.Fwp, Steps.Fw2, Steps.Fw2p,
            Steps.Bw, Steps.Bwp, Steps.Bw2, Steps.Bw2p,
            Steps.Rw, Steps.Rwp, Steps.Rw2, Steps.Rw2p,
            Steps.Lw, Steps.Lwp, Steps.Lw2, Steps.Lw2p,

            // Double layer, adjacent layers, opposite direction
            Steps.Uo, Steps.Uop, Steps.Uo2, Steps.Uo2p,
            Steps.Do, Steps.Dop, Steps.Do2, Steps.Do2p,
            Steps.Fo, Steps.Fop, Steps.Fo2, Steps.Fo2p,
            Steps.Bo, Steps.Bop, Steps.Bo2, Steps.Bo2p,
            Steps.Ro, Steps.Rop, Steps.Ro2, Steps.Ro2p,
            Steps.Lo, Steps.Lop, Steps.Lo2, Steps.Lo2p,

            // Double layer, opposite layers, same direction
            Steps.Us, Steps.Usp, Steps.Us2, Steps.Us2p,
            Steps.Ds, Steps.Dsp, Steps.Ds2, Steps.Ds2p,
            Steps.Fs, Steps.Fsp, Steps.Fs2, Steps.Fs2p,
            Steps.Bs, Steps.Bsp, Steps.Bs2, Steps.Bs2p,
            Steps.Rs, Steps.Rsp, Steps.Rs2, Steps.Rs2p,
            Steps.Ls, Steps.Lsp, Steps.Ls2, Steps.Ls2p,

            // Double layer, opposite layers, opposite direction
            Steps.Ua, Steps.Uap, Steps.Ua2, Steps.Ua2p,
            Steps.Da, Steps.Dap, Steps.Da2, Steps.Da2p,
            Steps.Fa, Steps.Fap, Steps.Fa2, Steps.Fa2p,
            Steps.Ba, Steps.Bap, Steps.Ba2, Steps.Ba2p,
            Steps.Ra, Steps.Rap, Steps.Ra2, Steps.Ra2p,
            Steps.La, Steps.Lap, Steps.La2, Steps.La2p,

            // Middle layers
            Steps.E, Steps.Ep, Steps.E2, Steps.E2p,
            Steps.S, Steps.Sp, Steps.S2, Steps.S2p,
            Steps.M, Steps.Mp, Steps.M2, Steps.M2p,

            // Full cube
            Steps.x, Steps.xp, Steps.x2, Steps.x2p,
            Steps.y, Steps.yp, Steps.y2, Steps.y2p,
            Steps.z, Steps.zp, Steps.z2, Steps.z2p,

            // Parentheses
            Steps.OPEN_PARENTHESIS, // Step is open parenthesis
            Steps.CLOSE_PARENTHESIS_1_REP, // Step is close parenthesis
            Steps.CLOSE_PARENTHESIS_2_REP, // Step is close parenthesis with two repetitions
            Steps.CLOSE_PARENTHESIS_3_REP, // Step is close parenthesis with three repetitions
            Steps.CLOSE_PARENTHESIS_4_REP, // Step is close parenthesis with four repetitions
            Steps.CLOSE_PARENTHESIS_5_REP, // Step is close parenthesis with five repetitions
            Steps.CLOSE_PARENTHESIS_6_REP, // Step is close parenthesis with six repetitions
            Steps.CLOSE_PARENTHESIS_7_REP, // Step is close parenthesis with seven repetitions
            Steps.CLOSE_PARENTHESIS_8_REP, // Step is close parenthesis with eight repetitions
            Steps.CLOSE_PARENTHESIS_9_REP  // Step is close parenthesis with nine repetitions
        };

        /// <summary>
        /// Array for steps strings (first assigned in static constructor)
        /// Use of alternative characters if needed (u, l, ...)
        /// </summary>
        private static string[] s_string;

        /// <summary>
        /// Array for steps strings (first assigned in static constructor)
        /// Don't use alternative characters
        /// </summary>
        private static string[] s_string_base;

        /// <summary>
        /// Array for steps in his more simple form
        /// </summary>
        private static readonly Steps[] s_simple =
        {
            Steps.NONE,

            // Single layer
            Steps.U, Steps.Up, Steps.U2, Steps.U2,
            Steps.D, Steps.Dp, Steps.D2, Steps.D2,
            Steps.F, Steps.Fp, Steps.F2, Steps.F2,
            Steps.B, Steps.Bp, Steps.B2, Steps.B2,
            Steps.R, Steps.Rp, Steps.R2, Steps.R2,
            Steps.L, Steps.Lp, Steps.L2, Steps.L2,

            // Double layer, adjacent layers, same direction
            Steps.Uw, Steps.Uwp, Steps.Uw2, Steps.Uw2,
            Steps.Dw, Steps.Dwp, Steps.Dw2, Steps.Dw2,
            Steps.Fw, Steps.Fwp, Steps.Fw2, Steps.Fw2,
            Steps.Bw, Steps.Bwp, Steps.Bw2, Steps.Bw2,
            Steps.Rw, Steps.Rwp, Steps.Rw2, Steps.Rw2,
            Steps.Lw, Steps.Lwp, Steps.Lw2, Steps.Lw2,

            // Double layer, adjacent layers, opposite direction
            Steps.Uo, Steps.Uop, Steps.Uw2, Steps.Uw2,
            Steps.Do, Steps.Dop, Steps.Dw2, Steps.Dw2,
            Steps.Fo, Steps.Fop, Steps.Fw2, Steps.Fw2,
            Steps.Bo, Steps.Bop, Steps.Bw2, Steps.Bw2,
            Steps.Ro, Steps.Rop, Steps.Rw2, Steps.Rw2,
            Steps.Lo, Steps.Lop, Steps.Lw2, Steps.Lw2,

            // Double layer, opposite layers, same direction
            Steps.Us, Steps.Ds, Steps.Us2, Steps.Us2,
            Steps.Ds, Steps.Us, Steps.Us2, Steps.Us2,
            Steps.Fs, Steps.Bs, Steps.Fs2, Steps.Fs2,
            Steps.Bs, Steps.Fs, Steps.Fs2, Steps.Fs2,
            Steps.Rs, Steps.Ls, Steps.Rs2, Steps.Rs2,
            Steps.Ls, Steps.Rs, Steps.Rs2, Steps.Rs2,

            // Double layer, opposite layers, opposite direction
            Steps.Ua, Steps.Uap, Steps.Us2, Steps.Us2,
            Steps.Ua, Steps.Uap, Steps.Us2, Steps.Us2,
            Steps.Fa, Steps.Fap, Steps.Fs2, Steps.Fs2,
            Steps.Fa, Steps.Fap, Steps.Fs2, Steps.Fs2,
            Steps.Ra, Steps.Rap, Steps.Rs2, Steps.Rs2,
            Steps.Ra, Steps.Rap, Steps.Rs2, Steps.Rs2,

            // Middle layers
            Steps.E, Steps.Ep, Steps.E2, Steps.E2,
            Steps.S, Steps.Sp, Steps.S2, Steps.S2,
            Steps.M, Steps.Mp, Steps.M2, Steps.M2,

            // Full cube
            Steps.x, Steps.xp, Steps.x2, Steps.x2,
            Steps.y, Steps.yp, Steps.y2, Steps.y2,
            Steps.z, Steps.zp, Steps.z2, Steps.z2,

            // Parentheses
            Steps.OPEN_PARENTHESIS, // Step is open parenthesis
            Steps.CLOSE_PARENTHESIS_1_REP, // Step is close parenthesis
            Steps.CLOSE_PARENTHESIS_2_REP, // Step is close parenthesis with two repetitions
            Steps.CLOSE_PARENTHESIS_3_REP, // Step is close parenthesis with three repetitions
            Steps.CLOSE_PARENTHESIS_4_REP, // Step is close parenthesis with four repetitions
            Steps.CLOSE_PARENTHESIS_5_REP, // Step is close parenthesis with five repetitions
            Steps.CLOSE_PARENTHESIS_6_REP, // Step is close parenthesis with six repetitions
            Steps.CLOSE_PARENTHESIS_7_REP, // Step is close parenthesis with seven repetitions
            Steps.CLOSE_PARENTHESIS_8_REP, // Step is close parenthesis with eight repetitions
            Steps.CLOSE_PARENTHESIS_9_REP  // Step is close parenthesis with nine repetitions
        };

        /// <summary>
        /// Array for inversed steps
        /// </summary>
        private static readonly Steps[] s_inverse =
        {
            Steps.NONE,

            // Single layer
            Steps.Up, Steps.U, Steps.U2p, Steps.U2,
            Steps.Dp, Steps.D, Steps.D2p, Steps.D2,
            Steps.Fp, Steps.F, Steps.F2p, Steps.F2,
            Steps.Bp, Steps.B, Steps.B2p, Steps.B2,
            Steps.Rp, Steps.R, Steps.R2p, Steps.R2,
            Steps.Lp, Steps.L, Steps.L2p, Steps.L2,

            // Double layer, adjacent layers, same direction
            Steps.Uwp, Steps.Uw, Steps.Uw2p, Steps.Uw2,
            Steps.Dwp, Steps.Dw, Steps.Dw2p, Steps.Dw2,
            Steps.Fwp, Steps.Fw, Steps.Fw2p, Steps.Fw2,
            Steps.Bwp, Steps.Bw, Steps.Bw2p, Steps.Bw2,
            Steps.Rwp, Steps.Rw, Steps.Rw2p, Steps.Rw2,
            Steps.Lwp, Steps.Lw, Steps.Lw2p, Steps.Lw2,

            // Double layer, adjacent layers, opposite direction
            Steps.Uop, Steps.Uo, Steps.Uo2p, Steps.Uo2,
            Steps.Dop, Steps.Do, Steps.Do2p, Steps.Do2,
            Steps.Fop, Steps.Fo, Steps.Fo2p, Steps.Fo2,
            Steps.Bop, Steps.Bo, Steps.Bo2p, Steps.Bo2,
            Steps.Rop, Steps.Ro, Steps.Ro2p, Steps.Ro2,
            Steps.Lop, Steps.Lo, Steps.Lo2p, Steps.Lo2,

            // Double layer, opposite layers, same direction
            Steps.Usp, Steps.Us, Steps.Us2p, Steps.Us2,
            Steps.Dsp, Steps.Ds, Steps.Ds2p, Steps.Ds2,
            Steps.Fsp, Steps.Fs, Steps.Fs2p, Steps.Fs2,
            Steps.Bsp, Steps.Bs, Steps.Bs2p, Steps.Bs2,
            Steps.Rsp, Steps.Rs, Steps.Rs2p, Steps.Rs2,
            Steps.Lsp, Steps.Ls, Steps.Ls2p, Steps.Ls2,

            // Double layer, opposite layers, opposite direction
            Steps.Uap, Steps.Ua, Steps.Ua2p, Steps.Ua2,
            Steps.Dap, Steps.Da, Steps.Da2p, Steps.Da2,
            Steps.Fap, Steps.Fa, Steps.Fa2p, Steps.Fa2,
            Steps.Bap, Steps.Ba, Steps.Ba2p, Steps.Ba2,
            Steps.Rap, Steps.Ra, Steps.Ra2p, Steps.Ra2,
            Steps.Lap, Steps.La, Steps.La2p, Steps.La2,

            // Middle layers
            Steps.Ep, Steps.E, Steps.E2p, Steps.E2,
            Steps.Sp, Steps.S, Steps.S2p, Steps.S2,
            Steps.Mp, Steps.M, Steps.M2p, Steps.M2,

            // Full cube
            Steps.xp, Steps.x, Steps.x2p, Steps.x2,
            Steps.yp, Steps.y, Steps.y2p, Steps.y2,
            Steps.zp, Steps.z, Steps.z2p, Steps.z2,

            // Parentheses
            Steps.OPEN_PARENTHESIS, // Step is open parenthesis
            Steps.CLOSE_PARENTHESIS_1_REP, // Step is close parenthesis
            Steps.CLOSE_PARENTHESIS_2_REP, // Step is close parenthesis with two repetitions
            Steps.CLOSE_PARENTHESIS_3_REP, // Step is close parenthesis with three repetitions
            Steps.CLOSE_PARENTHESIS_4_REP, // Step is close parenthesis with four repetitions
            Steps.CLOSE_PARENTHESIS_5_REP, // Step is close parenthesis with five repetitions
            Steps.CLOSE_PARENTHESIS_6_REP, // Step is close parenthesis with six repetitions
            Steps.CLOSE_PARENTHESIS_7_REP, // Step is close parenthesis with seven repetitions
            Steps.CLOSE_PARENTHESIS_8_REP, // Step is close parenthesis with eight repetitions
            Steps.CLOSE_PARENTHESIS_9_REP  // Step is close parenthesis with nine repetitions
        };

        /// <summary>
        /// Array for steps x turn
        /// </summary>
        private static readonly Steps[] s_x =
        {
            Steps.x,

            // Single layer
            Steps.B, Steps.Bp, Steps.B2, Steps.B2p,
            Steps.F, Steps.Fp, Steps.F2, Steps.F2p,
            Steps.U, Steps.Up, Steps.U2, Steps.U2p,
            Steps.D, Steps.Dp, Steps.D2, Steps.D2p,
            Steps.R, Steps.Rp, Steps.R2, Steps.R2p,
            Steps.L, Steps.Lp, Steps.L2, Steps.L2p,

            // Double layer, adjacent layers, same direction
            Steps.Bw, Steps.Bwp, Steps.Bw2, Steps.Bw2p,
            Steps.Fw, Steps.Fwp, Steps.Fw2, Steps.Fw2p,
            Steps.Uw, Steps.Uwp, Steps.Uw2, Steps.Uw2p,
            Steps.Dw, Steps.Dwp, Steps.Dw2, Steps.Dw2p,
            Steps.Rw, Steps.Rwp, Steps.Rw2, Steps.Rw2p,
            Steps.Lw, Steps.Lwp, Steps.Lw2, Steps.Lw2p,

            // Double layer, adjacent layers, opposite direction
            Steps.Bo, Steps.Bop, Steps.Bo2, Steps.Bo2p,
            Steps.Fo, Steps.Fop, Steps.Fo2, Steps.Fo2p,
            Steps.Uo, Steps.Uop, Steps.Uo2, Steps.Uo2p,
            Steps.Do, Steps.Dop, Steps.Do2, Steps.Do2p,
            Steps.Ro, Steps.Rop, Steps.Ro2, Steps.Ro2p,
            Steps.Lo, Steps.Lop, Steps.Lo2, Steps.Lo2p,

            // Double layer, opposite layers, same direction
            Steps.Bs, Steps.Bsp, Steps.Bs2, Steps.Bs2p,
            Steps.Fs, Steps.Fsp, Steps.Fs2, Steps.Fs2p,
            Steps.Us, Steps.Usp, Steps.Us2, Steps.Us2p,
            Steps.Ds, Steps.Dsp, Steps.Ds2, Steps.Ds2p,
            Steps.Rs, Steps.Rsp, Steps.Rs2, Steps.Rs2p,
            Steps.Ls, Steps.Lsp, Steps.Ls2, Steps.Ls2p,

            // Double layer, opposite layers, opposite direction
            Steps.Ba, Steps.Bap, Steps.Ba2, Steps.Ba2p,
            Steps.Fa, Steps.Fap, Steps.Fa2, Steps.Fa2p,
            Steps.Ua, Steps.Uap, Steps.Ua2, Steps.Ua2p,
            Steps.Da, Steps.Dap, Steps.Da2, Steps.Da2p,
            Steps.Ra, Steps.Rap, Steps.Ra2, Steps.Ra2p,
            Steps.La, Steps.Lap, Steps.La2, Steps.La2p,

            // Middle layers
            Steps.Sp, Steps.S, Steps.S2p, Steps.S2,
            Steps.E, Steps.Ep, Steps.E2, Steps.E2p,
            Steps.M, Steps.Mp, Steps.M2, Steps.M2p,

            // Full cube
            Steps.x, Steps.xp, Steps.x2, Steps.x2p,
            Steps.zp, Steps.z, Steps.z2p, Steps.z2,
            Steps.y, Steps.yp, Steps.y2, Steps.y2p,

            // Parentheses
            Steps.OPEN_PARENTHESIS, // Step is open parenthesis
            Steps.CLOSE_PARENTHESIS_1_REP, // Step is close parenthesis
            Steps.CLOSE_PARENTHESIS_2_REP, // Step is close parenthesis with two repetitions
            Steps.CLOSE_PARENTHESIS_3_REP, // Step is close parenthesis with three repetitions
            Steps.CLOSE_PARENTHESIS_4_REP, // Step is close parenthesis with four repetitions
            Steps.CLOSE_PARENTHESIS_5_REP, // Step is close parenthesis with five repetitions
            Steps.CLOSE_PARENTHESIS_6_REP, // Step is close parenthesis with six repetitions
            Steps.CLOSE_PARENTHESIS_7_REP, // Step is close parenthesis with seven repetitions
            Steps.CLOSE_PARENTHESIS_8_REP, // Step is close parenthesis with eight repetitions
            Steps.CLOSE_PARENTHESIS_9_REP  // Step is close parenthesis with nine repetitions
        };

        /// <summary>
        /// Array for steps x' turn (assigned in static constructor)
        /// </summary>
        private static readonly Steps[] s_xp;

        /// <summary>
        /// Array for steps x2 turn (assigned in static constructor)
        /// </summary>
        private static readonly Steps[] s_x2;

        /// <summary>
        /// Array for steps y turn
        /// </summary>
        private static readonly Steps[] s_y =
        {
            Steps.y,

            // Single layer
            Steps.U, Steps.Up, Steps.U2, Steps.U2p,
            Steps.D, Steps.Dp, Steps.D2, Steps.D2p,
            Steps.L, Steps.Lp, Steps.L2, Steps.L2p,
            Steps.R, Steps.Rp, Steps.R2, Steps.R2p,
            Steps.F, Steps.Fp, Steps.F2, Steps.F2p,
            Steps.B, Steps.Bp, Steps.B2, Steps.B2p,

            // Double layer, adjacent layers, same direction
            Steps.Uw, Steps.Uwp, Steps.Uw2, Steps.Uw2p,
            Steps.Dw, Steps.Dwp, Steps.Dw2, Steps.Dw2p,
            Steps.Lw, Steps.Lwp, Steps.Lw2, Steps.Lw2p,
            Steps.Rw, Steps.Rwp, Steps.Rw2, Steps.Rw2p,
            Steps.Fw, Steps.Fwp, Steps.Fw2, Steps.Fw2p,
            Steps.Bw, Steps.Bwp, Steps.Bw2, Steps.Bw2p,

            // Double layer, adjacent layers, opposite direction
            Steps.Uo, Steps.Uop, Steps.Uo2, Steps.Uo2p,
            Steps.Do, Steps.Dop, Steps.Do2, Steps.Do2p,
            Steps.Lo, Steps.Lop, Steps.Lo2, Steps.Lo2p,
            Steps.Ro, Steps.Rop, Steps.Ro2, Steps.Ro2p,
            Steps.Fo, Steps.Fop, Steps.Fo2, Steps.Fo2p,
            Steps.Bo, Steps.Bop, Steps.Bo2, Steps.Bo2p,

            // Double layer, opposite layers, same direction
            Steps.Us, Steps.Usp, Steps.Us2, Steps.Us2p,
            Steps.Ds, Steps.Dsp, Steps.Ds2, Steps.Ds2p,
            Steps.Ls, Steps.Lsp, Steps.Ls2, Steps.Ls2p,
            Steps.Rs, Steps.Rsp, Steps.Rs2, Steps.Rs2p,
            Steps.Fs, Steps.Fsp, Steps.Fs2, Steps.Fs2p,
            Steps.Bs, Steps.Bsp, Steps.Bs2, Steps.Bs2p,

            // Double layer, opposite layers, opposite direction
            Steps.Ua, Steps.Uap, Steps.Ua2, Steps.Ua2p,
            Steps.Da, Steps.Dap, Steps.Da2, Steps.Da2p,
            Steps.La, Steps.Lap, Steps.La2, Steps.La2p,
            Steps.Ra, Steps.Rap, Steps.Ra2, Steps.Ra2p,
            Steps.Fa, Steps.Fap, Steps.Fa2, Steps.Fa2p,
            Steps.Ba, Steps.Bap, Steps.Ba2, Steps.Ba2p,

            // Middle layers
            Steps.E, Steps.Ep, Steps.E2, Steps.E2p,
            Steps.Mp, Steps.M, Steps.M2p, Steps.M2,
            Steps.S, Steps.Sp, Steps.S2, Steps.S2p,

            // Full cube
            Steps.z, Steps.zp, Steps.z2, Steps.z2p,
            Steps.y, Steps.yp, Steps.y2, Steps.y2p,
            //Steps.y2, Steps.NONE, Steps.yp, Steps.yp,
            Steps.xp, Steps.x, Steps.x2p, Steps.x2,

            // Parentheses
            Steps.OPEN_PARENTHESIS, // Step is open parenthesis
            Steps.CLOSE_PARENTHESIS_1_REP, // Step is close parenthesis
            Steps.CLOSE_PARENTHESIS_2_REP, // Step is close parenthesis with two repetitions
            Steps.CLOSE_PARENTHESIS_3_REP, // Step is close parenthesis with three repetitions
            Steps.CLOSE_PARENTHESIS_4_REP, // Step is close parenthesis with four repetitions
            Steps.CLOSE_PARENTHESIS_5_REP, // Step is close parenthesis with five repetitions
            Steps.CLOSE_PARENTHESIS_6_REP, // Step is close parenthesis with six repetitions
            Steps.CLOSE_PARENTHESIS_7_REP, // Step is close parenthesis with seven repetitions
            Steps.CLOSE_PARENTHESIS_8_REP, // Step is close parenthesis with eight repetitions
            Steps.CLOSE_PARENTHESIS_9_REP  // Step is close parenthesis with nine repetitions
        };

        /// <summary>
        /// Array for steps y' turn (assigned in static constructor)
        /// </summary>
        private static readonly Steps[] s_yp;

        /// <summary>
        /// Array for steps y2 turn (assigned in static constructor)
        /// </summary>
        private static readonly Steps[] s_y2;

        /// <summary>
        /// Array for steps z turn
        /// </summary>
        private static readonly Steps[] s_z =
{
            Steps.z,

            // Single layer
            Steps.R, Steps.Rp, Steps.R2, Steps.R2p,
            Steps.L, Steps.Lp, Steps.L2, Steps.L2p,
            Steps.F, Steps.Fp, Steps.F2, Steps.F2p,
            Steps.B, Steps.Bp, Steps.B2, Steps.B2p,
            Steps.D, Steps.Dp, Steps.D2, Steps.D2p,
            Steps.U, Steps.Up, Steps.U2, Steps.U2p,

            // Double layer, adjacent layers, same direction
            Steps.Rw, Steps.Rwp, Steps.Rw2, Steps.Rw2p,
            Steps.Lw, Steps.Lwp, Steps.Lw2, Steps.Lw2p,
            Steps.Fw, Steps.Fwp, Steps.Fw2, Steps.Fw2p,
            Steps.Bw, Steps.Bwp, Steps.Bw2, Steps.Bw2p,
            Steps.Dw, Steps.Dwp, Steps.Dw2, Steps.Dw2p,
            Steps.Uw, Steps.Uwp, Steps.Uw2, Steps.Uw2p,

            // Double layer, adjacent layers, opposite direction
            Steps.Ro, Steps.Rop, Steps.Ro2, Steps.Ro2p,
            Steps.Lo, Steps.Lop, Steps.Lo2, Steps.Lo2p,
            Steps.Fo, Steps.Fop, Steps.Fo2, Steps.Fo2p,
            Steps.Bo, Steps.Bop, Steps.Bo2, Steps.Bo2p,
            Steps.Do, Steps.Dop, Steps.Do2, Steps.Do2p,
            Steps.Uo, Steps.Uop, Steps.Uo2, Steps.Uo2p,

            // Double layer, opposite layers, same direction
            Steps.Rs, Steps.Rsp, Steps.Rs2, Steps.Rs2p,
            Steps.Ls, Steps.Lsp, Steps.Ls2, Steps.Ls2p,
            Steps.Fs, Steps.Fsp, Steps.Fs2, Steps.Fs2p,
            Steps.Bs, Steps.Bsp, Steps.Bs2, Steps.Bs2p,
            Steps.Ds, Steps.Dsp, Steps.Ds2, Steps.Ds2p,
            Steps.Us, Steps.Usp, Steps.Us2, Steps.Us2p,

            // Double layer, opposite layers, opposite direction
            Steps.Ra, Steps.Rap, Steps.Ra2, Steps.Ra2p,
            Steps.La, Steps.Lap, Steps.La2, Steps.La2p,
            Steps.Fa, Steps.Fap, Steps.Fa2, Steps.Fa2p,
            Steps.Ba, Steps.Bap, Steps.Ba2, Steps.Ba2p,
            Steps.Da, Steps.Dap, Steps.Da2, Steps.Da2p,
            Steps.Ua, Steps.Uap, Steps.Ua2, Steps.Ua2p,

            // Middle layers
            Steps.M, Steps.Mp, Steps.M2p, Steps.M2,
            Steps.S, Steps.Sp, Steps.S2, Steps.S2p,
            Steps.Ep, Steps.E, Steps.E2, Steps.E2p,

            // Full cube
            Steps.yp, Steps.y, Steps.y2p, Steps.y2,
            Steps.x, Steps.xp, Steps.x2, Steps.x2p,
            //Steps.z2, Steps.NONE, Steps.zp, Steps.zp,
            Steps.z, Steps.zp, Steps.z2, Steps.z2p,

            // Parentheses
            Steps.OPEN_PARENTHESIS, // Step is open parenthesis
            Steps.CLOSE_PARENTHESIS_1_REP, // Step is close parenthesis
            Steps.CLOSE_PARENTHESIS_2_REP, // Step is close parenthesis with two repetitions
            Steps.CLOSE_PARENTHESIS_3_REP, // Step is close parenthesis with three repetitions
            Steps.CLOSE_PARENTHESIS_4_REP, // Step is close parenthesis with four repetitions
            Steps.CLOSE_PARENTHESIS_5_REP, // Step is close parenthesis with five repetitions
            Steps.CLOSE_PARENTHESIS_6_REP, // Step is close parenthesis with six repetitions
            Steps.CLOSE_PARENTHESIS_7_REP, // Step is close parenthesis with seven repetitions
            Steps.CLOSE_PARENTHESIS_8_REP, // Step is close parenthesis with eight repetitions
            Steps.CLOSE_PARENTHESIS_9_REP  // Step is close parenthesis with nine repetitions
        };

        /// <summary>
        /// Array for steps z' turn (assigned in static constructor)
        /// </summary>
        private static readonly Steps[] s_zp;

        /// <summary>
        /// Array for steps z2 turn (assigned in static constructor)
        /// </summary>
        private static readonly Steps[] s_z2;

        /// <summary>
        /// Array for steps layer
        /// </summary>
        private static readonly Layers[] s_layer =
        {
            Layers.NONE,

            // Single layer
            Layers.U, Layers.U, Layers.U, Layers.U,
            Layers.D, Layers.D, Layers.D, Layers.D,
            Layers.F, Layers.F, Layers.F, Layers.F,
            Layers.B, Layers.B, Layers.B, Layers.B,
            Layers.R, Layers.R, Layers.R, Layers.R,
            Layers.L, Layers.L, Layers.L, Layers.L,

            // Double layer, adjacent layers, same direction
            Layers.U, Layers.U, Layers.U, Layers.U,
            Layers.D, Layers.D, Layers.D, Layers.D,
            Layers.F, Layers.F, Layers.F, Layers.F,
            Layers.B, Layers.B, Layers.B, Layers.B,
            Layers.R, Layers.R, Layers.R, Layers.R,
            Layers.L, Layers.L, Layers.L, Layers.L,

            // Double layer, adjacent layers, opposite direction
            Layers.U, Layers.U, Layers.U, Layers.U,
            Layers.D, Layers.D, Layers.D, Layers.D,
            Layers.F, Layers.F, Layers.F, Layers.F,
            Layers.B, Layers.B, Layers.B, Layers.B,
            Layers.R, Layers.R, Layers.R, Layers.R,
            Layers.L, Layers.L, Layers.L, Layers.L,

            // Double layer, opposite layers, same direction
            Layers.U, Layers.U, Layers.U, Layers.U,
            Layers.D, Layers.D, Layers.D, Layers.D,
            Layers.F, Layers.F, Layers.F, Layers.F,
            Layers.B, Layers.B, Layers.B, Layers.B,
            Layers.R, Layers.R, Layers.R, Layers.R,
            Layers.L, Layers.L, Layers.L, Layers.L,

            // Double layer, opposite layers, opposite direction
            Layers.U, Layers.U, Layers.U, Layers.U,
            Layers.D, Layers.D, Layers.D, Layers.D,
            Layers.F, Layers.F, Layers.F, Layers.F,
            Layers.B, Layers.B, Layers.B, Layers.B,
            Layers.R, Layers.R, Layers.R, Layers.R,
            Layers.L, Layers.L, Layers.L, Layers.L,

            // Middle layers
            Layers.E, Layers.E, Layers.E, Layers.E,
            Layers.S, Layers.S, Layers.S, Layers.S,
            Layers.M, Layers.M, Layers.M, Layers.M,

            // Full cube
            Layers.R, Layers.R, Layers.R, Layers.R,
            Layers.U, Layers.U, Layers.U, Layers.U,
            Layers.F, Layers.F, Layers.F, Layers.F,

            // Parentheses
            Layers.NONE, // Step is open parenthesis
            Layers.NONE, // Step is close parenthesis
            Layers.NONE, // Step is close parenthesis with two repetitions
            Layers.NONE, // Step is close parenthesis with three repetitions
            Layers.NONE, // Step is close parenthesis with four repetitions
            Layers.NONE, // Step is close parenthesis with five repetitions
            Layers.NONE, // Step is close parenthesis with six repetitions
            Layers.NONE, // Step is close parenthesis with seven repetitions
            Layers.NONE, // Step is close parenthesis with eight repetitions
            Layers.NONE  // Step is close parenthesis with nine repetitions
        };

        /// <summary>
        /// Array for steps movement
        /// </summary>
        private static readonly Movements[] s_mov =
        {
            Movements.NONE,

            // Single layer
            Movements.ROT90CW, Movements.ROT90CCW, Movements.ROT180CW, Movements.ROT180CCW,
            Movements.ROT90CW, Movements.ROT90CCW, Movements.ROT180CW, Movements.ROT180CCW,
            Movements.ROT90CW, Movements.ROT90CCW, Movements.ROT180CW, Movements.ROT180CCW,
            Movements.ROT90CW, Movements.ROT90CCW, Movements.ROT180CW, Movements.ROT180CCW,
            Movements.ROT90CW, Movements.ROT90CCW, Movements.ROT180CW, Movements.ROT180CCW,
            Movements.ROT90CW, Movements.ROT90CCW, Movements.ROT180CW, Movements.ROT180CCW,

            // Double layer, adjacent layers, same direction
            Movements.ROT90CW, Movements.ROT90CCW, Movements.ROT180CW, Movements.ROT180CCW,
            Movements.ROT90CW, Movements.ROT90CCW, Movements.ROT180CW, Movements.ROT180CCW,
            Movements.ROT90CW, Movements.ROT90CCW, Movements.ROT180CW, Movements.ROT180CCW,
            Movements.ROT90CW, Movements.ROT90CCW, Movements.ROT180CW, Movements.ROT180CCW,
            Movements.ROT90CW, Movements.ROT90CCW, Movements.ROT180CW, Movements.ROT180CCW,
            Movements.ROT90CW, Movements.ROT90CCW, Movements.ROT180CW, Movements.ROT180CCW,

            // Double layer, adjacent layers, opposite direction
            Movements.ROT90CW, Movements.ROT90CCW, Movements.ROT180CW, Movements.ROT180CCW,
            Movements.ROT90CW, Movements.ROT90CCW, Movements.ROT180CW, Movements.ROT180CCW,
            Movements.ROT90CW, Movements.ROT90CCW, Movements.ROT180CW, Movements.ROT180CCW,
            Movements.ROT90CW, Movements.ROT90CCW, Movements.ROT180CW, Movements.ROT180CCW,
            Movements.ROT90CW, Movements.ROT90CCW, Movements.ROT180CW, Movements.ROT180CCW,
            Movements.ROT90CW, Movements.ROT90CCW, Movements.ROT180CW, Movements.ROT180CCW,

            // Double layer, opposite layers, same direction
            Movements.ROT90CW, Movements.ROT90CCW, Movements.ROT180CW, Movements.ROT180CCW,
            Movements.ROT90CW, Movements.ROT90CCW, Movements.ROT180CW, Movements.ROT180CCW,
            Movements.ROT90CW, Movements.ROT90CCW, Movements.ROT180CW, Movements.ROT180CCW,
            Movements.ROT90CW, Movements.ROT90CCW, Movements.ROT180CW, Movements.ROT180CCW,
            Movements.ROT90CW, Movements.ROT90CCW, Movements.ROT180CW, Movements.ROT180CCW,
            Movements.ROT90CW, Movements.ROT90CCW, Movements.ROT180CW, Movements.ROT180CCW,

            // Double layer, opposite layers, opposite direction
            Movements.ROT90CW, Movements.ROT90CCW, Movements.ROT180CW, Movements.ROT180CCW,
            Movements.ROT90CW, Movements.ROT90CCW, Movements.ROT180CW, Movements.ROT180CCW,
            Movements.ROT90CW, Movements.ROT90CCW, Movements.ROT180CW, Movements.ROT180CCW,
            Movements.ROT90CW, Movements.ROT90CCW, Movements.ROT180CW, Movements.ROT180CCW,
            Movements.ROT90CW, Movements.ROT90CCW, Movements.ROT180CW, Movements.ROT180CCW,
            Movements.ROT90CW, Movements.ROT90CCW, Movements.ROT180CW, Movements.ROT180CCW,

            // Middle layers
            Movements.ROT90CW, Movements.ROT90CCW, Movements.ROT180CW, Movements.ROT180CCW,
            Movements.ROT90CW, Movements.ROT90CCW, Movements.ROT180CW, Movements.ROT180CCW,
            Movements.ROT90CW, Movements.ROT90CCW, Movements.ROT180CW, Movements.ROT180CCW,

            // Full cube
            Movements.ROT90CW, Movements.ROT90CCW, Movements.ROT180CW, Movements.ROT180CCW,
            Movements.ROT90CW, Movements.ROT90CCW, Movements.ROT180CW, Movements.ROT180CCW,
            Movements.ROT90CW, Movements.ROT90CCW, Movements.ROT180CW, Movements.ROT180CCW,

            // Parentheses
            Movements.NONE, // Step is open parenthesis
            Movements.NONE, // Step is close parenthesis
            Movements.NONE, // Step is close parenthesis with two repetitions
            Movements.NONE, // Step is close parenthesis with three repetitions
            Movements.NONE, // Step is close parenthesis with four repetitions
            Movements.NONE, // Step is close parenthesis with five repetitions
            Movements.NONE, // Step is close parenthesis with six repetitions
            Movements.NONE, // Step is close parenthesis with seven repetitions
            Movements.NONE, // Step is close parenthesis with eight repetitions
            Movements.NONE  // Step is close parenthesis with nine repetitions
        };

        /// <summary>
        /// Array of equivalent steps sequence
        /// </summary>
        private static readonly Steps[][] s_seq =
        {
            new Steps[] { Steps.NONE, Steps.NONE, Steps.NONE }, // NONE

            // Single layer
            new Steps[] { Steps.U, Steps.NONE, Steps.NONE },    // U
            new Steps[] { Steps.Up, Steps.NONE, Steps.NONE },   // Up
            new Steps[] { Steps.U2, Steps.NONE, Steps.NONE },   // U2
            new Steps[] { Steps.U2, Steps.NONE, Steps.NONE },   // U2p
            new Steps[] { Steps.D, Steps.NONE, Steps.NONE },    // D
            new Steps[] { Steps.Dp, Steps.NONE, Steps.NONE },   // Dp
            new Steps[] { Steps.D2, Steps.NONE, Steps.NONE },   // D2
            new Steps[] { Steps.D2, Steps.NONE, Steps.NONE },   // D2p
            new Steps[] { Steps.F, Steps.NONE, Steps.NONE },    // F
            new Steps[] { Steps.Fp, Steps.NONE, Steps.NONE },   // Fp
            new Steps[] { Steps.F2, Steps.NONE, Steps.NONE },   // F2
            new Steps[] { Steps.F2, Steps.NONE, Steps.NONE },   // F2p
            new Steps[] { Steps.B, Steps.NONE, Steps.NONE },    // B
            new Steps[] { Steps.Bp, Steps.NONE, Steps.NONE },   // Bp
            new Steps[] { Steps.B2, Steps.NONE, Steps.NONE },   // B2
            new Steps[] { Steps.B2, Steps.NONE, Steps.NONE },   // B2p
            new Steps[] { Steps.R, Steps.NONE, Steps.NONE },    // R
            new Steps[] { Steps.Rp, Steps.NONE, Steps.NONE },   // Rp
            new Steps[] { Steps.R2, Steps.NONE, Steps.NONE },   // R2
            new Steps[] { Steps.R2, Steps.NONE, Steps.NONE },   // R2p
            new Steps[] { Steps.L, Steps.NONE, Steps.NONE },    // L
            new Steps[] { Steps.Lp, Steps.NONE, Steps.NONE },   // Lp
            new Steps[] { Steps.L2, Steps.NONE, Steps.NONE },   // L2
            new Steps[] { Steps.L2, Steps.NONE, Steps.NONE },   // L2p

            // Double layer, adjacent layers, same direction
            new Steps[] { Steps.D, Steps.y, Steps.NONE },       // Uw
            new Steps[] { Steps.Dp, Steps.yp, Steps.NONE },     // Uwp
            new Steps[] { Steps.D2, Steps.y2, Steps.NONE },     // Uw2
            new Steps[] { Steps.D2, Steps.y2, Steps.NONE },     // Uw2p
            new Steps[] { Steps.U, Steps.yp, Steps.NONE },      // Dw
            new Steps[] { Steps.Up, Steps.y, Steps.NONE },      // Dwp
            new Steps[] { Steps.U2, Steps.y2, Steps.NONE },     // Dw2
            new Steps[] { Steps.U2, Steps.y2, Steps.NONE },     // Dw2p
            new Steps[] { Steps.B, Steps.z, Steps.NONE },       // Fw
            new Steps[] { Steps.Bp, Steps.zp, Steps.NONE },     // Fwp
            new Steps[] { Steps.B2, Steps.z2, Steps.NONE },     // Fw2
            new Steps[] { Steps.B2, Steps.z2, Steps.NONE },     // Fw2p
            new Steps[] { Steps.F, Steps.zp, Steps.NONE },      // Bw
            new Steps[] { Steps.Fp, Steps.z, Steps.NONE },      // Bwp
            new Steps[] { Steps.F2, Steps.z2, Steps.NONE },     // Bw2
            new Steps[] { Steps.F2, Steps.z2, Steps.NONE },     // Bw2p
            new Steps[] { Steps.L, Steps.x, Steps.NONE },       // Rw
            new Steps[] { Steps.Lp, Steps.xp, Steps.NONE },     // Rwp
            new Steps[] { Steps.L2, Steps.x2, Steps.NONE },     // Rw2
            new Steps[] { Steps.L2, Steps.x2, Steps.NONE },     // Rw2p
            new Steps[] { Steps.R, Steps.xp, Steps.NONE },      // Lw
            new Steps[] { Steps.Rp, Steps.x, Steps.NONE },      // Lwp
            new Steps[] { Steps.R2, Steps.x2, Steps.NONE },     // Lw2
            new Steps[] { Steps.R2, Steps.x2, Steps.NONE },     // Lw2p

            // Double layer, adjacent layers, opposite direction
            new Steps[] { Steps.U2, Steps.Dp, Steps.yp },       // Uo
            new Steps[] { Steps.U2, Steps.D, Steps.y },         // Uop
            new Steps[] { Steps.D2, Steps.y2, Steps.NONE },     // Uo2
            new Steps[] { Steps.D2, Steps.y2, Steps.NONE },     // Uo2p
            new Steps[] { Steps.D2, Steps.Up, Steps.y },        // Do
            new Steps[] { Steps.D2, Steps.U, Steps.yp },        // Dop
            new Steps[] { Steps.U2, Steps.y2, Steps.NONE },     // Do2
            new Steps[] { Steps.U2, Steps.y2, Steps.NONE },     // Do2p
            new Steps[] { Steps.F2, Steps.Bp, Steps.zp },       // Fo
            new Steps[] { Steps.F2, Steps.B, Steps.z },         // Fop
            new Steps[] { Steps.B2, Steps.z2, Steps.NONE },     // Fo2
            new Steps[] { Steps.B2, Steps.z2, Steps.NONE },     // Fo2p
            new Steps[] { Steps.B2, Steps.Fp, Steps.z },        // Bo
            new Steps[] { Steps.B2, Steps.F, Steps.zp },        // Bop
            new Steps[] { Steps.F2, Steps.z2, Steps.NONE },     // Bo2
            new Steps[] { Steps.F2, Steps.z2, Steps.NONE },     // Bo2p
            new Steps[] { Steps.R2, Steps.Lp, Steps.xp },       // Ro
            new Steps[] { Steps.R2, Steps.L, Steps.x },         // Rop
            new Steps[] { Steps.L2, Steps.x2, Steps.NONE },     // Ro2
            new Steps[] { Steps.L2, Steps.x2, Steps.NONE },     // Ro2p
            new Steps[] { Steps.L2, Steps.Rp, Steps.x },        // Lo
            new Steps[] { Steps.L2, Steps.R, Steps.xp },        // Lop
            new Steps[] { Steps.R2, Steps.x2, Steps.NONE },     // Lo2
            new Steps[] { Steps.R2, Steps.x2, Steps.NONE },     // Lo2p

            // Double layer, opposite layers, same direction
            new Steps[] { Steps.U, Steps.Dp, Steps.NONE },      // Us
            new Steps[] { Steps.D, Steps.Up, Steps.NONE },      // Usp
            new Steps[] { Steps.U2, Steps.D2, Steps.NONE },     // Us2
            new Steps[] { Steps.U2, Steps.D2, Steps.NONE },     // Us2p
            new Steps[] { Steps.D, Steps.Up, Steps.NONE },      // Ds
            new Steps[] { Steps.U, Steps.Dp, Steps.NONE },      // Dsp
            new Steps[] { Steps.U2, Steps.D2, Steps.NONE },     // Ds2
            new Steps[] { Steps.U2, Steps.D2, Steps.NONE },     // Ds2p
            new Steps[] { Steps.F, Steps.Bp, Steps.NONE },      // Fs
            new Steps[] { Steps.B, Steps.Fp, Steps.NONE },      // Fsp
            new Steps[] { Steps.F2, Steps.B2, Steps.NONE },     // Fs2
            new Steps[] { Steps.F2, Steps.B2, Steps.NONE },     // Fs2p
            new Steps[] { Steps.B, Steps.Fp, Steps.NONE },      // Bs
            new Steps[] { Steps.F, Steps.Bp, Steps.NONE },      // Bsp
            new Steps[] { Steps.F2, Steps.B2, Steps.NONE },     // Bs2
            new Steps[] { Steps.F2, Steps.B2, Steps.NONE },     // Bs2p
            new Steps[] { Steps.R, Steps.Lp, Steps.NONE },      // Rs
            new Steps[] { Steps.L, Steps.Rp, Steps.NONE },      // Rsp
            new Steps[] { Steps.R2, Steps.L2, Steps.NONE },     // Rs2
            new Steps[] { Steps.R2, Steps.L2, Steps.NONE },     // Rs2p
            new Steps[] { Steps.L, Steps.Rp, Steps.NONE },      // Ls
            new Steps[] { Steps.R, Steps.Lp, Steps.NONE },      // Lsp
            new Steps[] { Steps.R2, Steps.L2, Steps.NONE },     // Ls2
            new Steps[] { Steps.R2, Steps.L2, Steps.NONE },     // Ls2p

            // Double layer, opposite layers, opposite direction
            new Steps[] { Steps.U, Steps.D, Steps.NONE },       // Ua
            new Steps[] { Steps.Up, Steps.Dp, Steps.NONE },     // Uap
            new Steps[] { Steps.U2, Steps.D2, Steps.NONE },     // Ua2
            new Steps[] { Steps.U2, Steps.D2, Steps.NONE },     // Ua2p
            new Steps[] { Steps.U, Steps.D, Steps.NONE },       // Da
            new Steps[] { Steps.Up, Steps.Dp, Steps.NONE },     // Dap
            new Steps[] { Steps.U2, Steps.D2, Steps.NONE },     // Da2
            new Steps[] { Steps.U2, Steps.D2, Steps.NONE },     // Da2p
            new Steps[] { Steps.F, Steps.B, Steps.NONE },       // Fa
            new Steps[] { Steps.Fp, Steps.Bp, Steps.NONE },     // Fap
            new Steps[] { Steps.F2, Steps.B2, Steps.NONE },     // Fa2
            new Steps[] { Steps.F2, Steps.B2, Steps.NONE },     // Fa2p
            new Steps[] { Steps.F, Steps.B, Steps.NONE },       // Ba
            new Steps[] { Steps.Fp, Steps.Bp, Steps.NONE },     // Bap
            new Steps[] { Steps.F2, Steps.B2, Steps.NONE },     // Ba2
            new Steps[] { Steps.F2, Steps.B2, Steps.NONE },     // Ba2p
            new Steps[] { Steps.R, Steps.L, Steps.NONE },       // Ra
            new Steps[] { Steps.Rp, Steps.Lp, Steps.NONE },     // Rap
            new Steps[] { Steps.R2, Steps.L2, Steps.NONE },     // Ra2
            new Steps[] { Steps.R2, Steps.L2, Steps.NONE },     // Ra2p
            new Steps[] { Steps.R, Steps.L, Steps.NONE },       // La
            new Steps[] { Steps.Rp, Steps.Lp, Steps.NONE },     // Lap
            new Steps[] { Steps.R2, Steps.L2, Steps.NONE },     // La2
            new Steps[] { Steps.R2, Steps.L2, Steps.NONE },     // La2p

            // Middle layers
            new Steps[] { Steps.Up, Steps.D, Steps.y },         // E
            new Steps[] { Steps.U, Steps.Dp, Steps.yp },        // Ep
            new Steps[] { Steps.U2, Steps.D2, Steps.y2 },       // E2
            new Steps[] { Steps.U2, Steps.D2, Steps.y2 },       // E2p
            new Steps[] { Steps.Fp, Steps.B, Steps.z },         // S
            new Steps[] { Steps.F, Steps.Bp, Steps.zp },        // Sp
            new Steps[] { Steps.F2, Steps.B2, Steps.z2 },       // S2
            new Steps[] { Steps.F2, Steps.B2, Steps.z2 },       // S2p
            new Steps[] { Steps.Rp, Steps.L, Steps.x },         // M
            new Steps[] { Steps.R, Steps.Lp, Steps.xp },        // Mp
            new Steps[] { Steps.R2, Steps.L2, Steps.x2 },       // M2
            new Steps[] { Steps.R2, Steps.L2, Steps.x2 },       // M2p

            // Full cube
            new Steps[] { Steps.x, Steps.NONE, Steps.NONE },    // x
            new Steps[] { Steps.xp, Steps.NONE, Steps.NONE },   // xp
            new Steps[] { Steps.x2, Steps.NONE, Steps.NONE },   // x2
            new Steps[] { Steps.x2, Steps.NONE, Steps.NONE },   // x2p
            new Steps[] { Steps.y, Steps.NONE, Steps.NONE },    // y
            new Steps[] { Steps.yp, Steps.NONE, Steps.NONE },   // yp
            new Steps[] { Steps.y2, Steps.NONE, Steps.NONE },   // y2
            new Steps[] { Steps.y2, Steps.NONE, Steps.NONE },   // y2p
            new Steps[] { Steps.z, Steps.NONE, Steps.NONE },    // z
            new Steps[] { Steps.zp, Steps.NONE, Steps.NONE },   // zp
            new Steps[] { Steps.z2, Steps.NONE, Steps.NONE },   // z2
            new Steps[] { Steps.z2, Steps.NONE, Steps.NONE },   // z2p

            // Parentheses
            new Steps[] { Steps.OPEN_PARENTHESIS, Steps.NONE, Steps.NONE }, // Step is open parenthesis
            new Steps[] { Steps.CLOSE_PARENTHESIS_1_REP, Steps.NONE, Steps.NONE }, // Step is close parenthesis
            new Steps[] { Steps.CLOSE_PARENTHESIS_2_REP, Steps.NONE, Steps.NONE }, // Step is close parenthesis with two repetitions
            new Steps[] { Steps.CLOSE_PARENTHESIS_3_REP, Steps.NONE, Steps.NONE }, // Step is close parenthesis with three repetitions
            new Steps[] { Steps.CLOSE_PARENTHESIS_4_REP, Steps.NONE, Steps.NONE }, // Step is close parenthesis with four repetitions
            new Steps[] { Steps.CLOSE_PARENTHESIS_5_REP, Steps.NONE, Steps.NONE }, // Step is close parenthesis with five repetitions
            new Steps[] { Steps.CLOSE_PARENTHESIS_6_REP, Steps.NONE, Steps.NONE }, // Step is close parenthesis with six repetitions
            new Steps[] { Steps.CLOSE_PARENTHESIS_7_REP, Steps.NONE, Steps.NONE }, // Step is close parenthesis with seven repetitions
            new Steps[] { Steps.CLOSE_PARENTHESIS_8_REP, Steps.NONE, Steps.NONE }, // Step is close parenthesis with eight repetitions
            new Steps[] { Steps.CLOSE_PARENTHESIS_9_REP, Steps.NONE, Steps.NONE }  // Step is close parenthesis with nine repetitions
        };

        #endregion Static arrays

        #region Static constructor

        /// <summary>
        /// Static constructor
        /// </summary>
        static ScrambleStep()
        {
            int s = s_ident.Length;

            s_xp = new Steps[s];
            s_x2 = new Steps[s];
            s_yp = new Steps[s];
            s_y2 = new Steps[s];
            s_zp = new Steps[s];
            s_z2 = new Steps[s];

            for (int n = 0; n < s; n++)
            {
                s_x2[n] = s_x[(int)s_x[n]];
                s_xp[n] = s_x[(int)s_x2[n]];
                s_y2[n] = s_y[(int)s_y[n]];
                s_yp[n] = s_y[(int)s_y2[n]];
                s_z2[n] = s_z[(int)s_z[n]];
                s_zp[n] = s_z[(int)s_z2[n]];
            }

            s_string = new string[s];
            s_string_base = new string[s];
            SetStepsTexts();
        }

        #endregion Static constructor

        #region General functions

        /// <summary>
        /// Check if step is consistent
        /// </summary>
        public static bool IsConsistent(Steps Stp) => Stp >= Steps.NONE && Stp <= Steps.CLOSE_PARENTHESIS_9_REP;

        /// <summary>
        /// True if step is none
        /// </summary>
        public static bool IsNone(Steps Stp) => Stp == Steps.NONE;

        /// <summary>
        /// True if step is a movement or a turn (U, M, Da, x, z',...)
        /// </summary>
        public static bool IsTurnOrMovement(Steps Stp) => Stp >= Steps.U && Stp <= Steps.z2p;

        /// <summary>
        /// Get inverse step (U' for U, x2 for x2', ...)
        /// </summary>
        public static Steps Inverse(Steps Stp) => IsConsistent(Stp) ? s_inverse[(int)Stp] : Steps.NONE;

        /// <summary>
        /// Returns current step layer
        /// </summary>
        public static Layers Layer(Steps Stp) => IsConsistent(Stp) ? s_layer[(int)Stp] : Layers.NONE;

        /// <summary>
        /// Gets the opposite layer of step
        /// </summary>
        public static Layers OppositeLayer(Steps Stp)
        {
            Layers Ly = Layer(Stp);
            switch (Ly)
            {
                case Layers.U: return Layers.D;
                case Layers.D: return Layers.U;
                case Layers.F: return Layers.B;
                case Layers.B: return Layers.F;
                case Layers.R: return Layers.L;
                case Layers.L: return Layers.R;
                default: return Ly;
            }
        }

        /// <summary>
        /// Returns current step movement
        /// </summary>
        public static Movements Movement(Steps Stp) => IsConsistent(Stp) ? s_mov[(int)Stp] : Movements.NONE;

        /// <summary>
        /// Assign a movement to a given step
        /// </summary>
        /// <param name="Stp">Step to assign the movement</param>
        /// <param name="Mov">Movement to assign</param>
        /// <returns>Step with movement assigned</returns>
        public static Steps SetMovementToStep(Steps Stp, Movements Mov)
        {
            if (Mov == Movement(Stp) || !IsTurnOrMovement(Stp)) return Stp;

            if (Mov == Movements.NONE) return Steps.NONE;

            switch (Movement(Stp))
            {
                case Movements.ROT90CW:
                    if (Mov == Movements.ROT90CCW) Stp++;
                    else if (Mov == Movements.ROT180CW) Stp += 2;
                    else Stp += 3; // ROT180CCW
                    return Stp;

                case Movements.ROT90CCW:
                    if (Mov == Movements.ROT90CW) Stp--;
                    else if (Mov == Movements.ROT180CW) Stp++;
                    else Stp += 2; // ROT180CCW
                    return Stp;

                case Movements.ROT180CW:
                    if (Mov == Movements.ROT90CW) Stp -= 2;
                    else if (Mov == Movements.ROT90CCW) Stp -= 1;
                    else Stp++; // ROT180CCW
                    return Stp;

                case Movements.ROT180CCW:
                    if (Mov == Movements.ROT90CW) Stp -= 3;
                    else if (Mov == Movements.ROT90CCW) Stp -= 2;
                    else Stp--; // ROT180CW
                    return Stp;

                default:
                    throw new System.Exception("Error assigning Movement to ScrambleStep");
            }
        }

        /// <summary>
        /// Returns current step modifier
        /// </summary>
        public static Modifiers Modifier(Steps Stp)
        {
            if (Stp <= Steps.L2p) return Modifiers.SINGLE_LAYER;
            if (Stp <= Steps.Lw2p) return Modifiers.DOUBLE_ADJACENT_LAYERS_SAME_DIRECTION;
            if (Stp <= Steps.Lo2p) return Modifiers.DOUBLE_ADJACENT_LAYERS_OPPOSITE_DIRECTION;
            if (Stp <= Steps.Ls2p) return Modifiers.DOUBLE_OPPOSITE_LAYERS_SAME_DIRECTION;
            if (Stp <= Steps.La2p) return Modifiers.DOUBLE_OPPOSITE_LAYERS_OPPOSITE_DIRECTION;
            if (Stp <= Steps.M2p) return Modifiers.SINGLE_LAYER;
            if (Stp <= Steps.z2p) return Modifiers.FULL_CUBE;
            if (Stp <= Steps.CLOSE_PARENTHESIS_9_REP)
                return (Stp - Steps.OPEN_PARENTHESIS) + Modifiers.OPEN_PARENTHESIS;

            throw new System.Exception("Invalid Step in Modifier");
        }

        /// <summary>
        /// True if step is a turn (x, y', z2, ...)
        /// </summary>
        public static bool IsTurn(Steps Stp) => Stp >= Steps.x && Stp <= Steps.z2p;

        /// <summary>
        /// True if step is an x axis turn
        /// </summary>
        public static bool IsAxisxTurn(Steps Stp) => Stp >= Steps.x && Stp <= Steps.x2p;

        /// <summary>
        /// True if step is an y axis turn
        /// </summary>
        public static bool IsAxisyTurn(Steps Stp) => Stp >= Steps.y && Stp <= Steps.y2p;

        /// <summary>
        /// True if step is an z axis turn
        /// </summary>
        public static bool IsAxiszTurn(Steps Stp) => Stp >= Steps.z && Stp <= Steps.z2p;

        /// <summary>
        /// True if step is a movement (U, M, Da, ...)
        /// </summary>
        public static bool IsMovement(Steps Stp) => Stp >= Steps.U && Stp <= Steps.M2p;

        /// <summary>
        /// True if step is parenthesis (open or close)
        /// </summary>
        public static bool IsParenthesis(Steps Stp) => Stp >= Steps.OPEN_PARENTHESIS && Stp <= Steps.CLOSE_PARENTHESIS_9_REP;

        /// <summary>
        /// True if step is open parenthesis
        /// </summary>
        public static bool IsOpenParenthesis(Steps Stp) => Stp == Steps.OPEN_PARENTHESIS;

        /// <summary>
        /// True if step is close parenthesis
        /// </summary>
        public static bool IsCloseParenthesis(Steps Stp) => Stp >= Steps.CLOSE_PARENTHESIS_1_REP && Stp <= Steps.CLOSE_PARENTHESIS_9_REP;

        /// <summary>
        /// Gets the number of repetitions of close parenthesis
        /// </summary>
        public static int GetCloseParenthesisRepetions(Steps Stp) => IsCloseParenthesis(Stp) ? Stp - Steps.OPEN_PARENTHESIS : 0;

        /// <summary>
        /// Gets current step in the most simple (usual) form
        /// </summary>
        public static Steps Simple(Steps Stp) => IsConsistent(Stp) ? s_simple[(int)Stp] : Steps.NONE;

        /// <summary>
        /// Gets the text associated to a step
        /// </summary>
        /// <returns>Step text</returns>
        public static string GetText(Steps Stp) => IsConsistent(Stp) ? s_string[(int)Stp] : string.Empty;

         /// <summary>
        /// Join two movements
        /// </summary>
        /// <param name="Mv1">Movement 1</param>
        /// <param name="Mv2">Movement 2</param>
        /// <returns>Result of join Mv1 and Mv2</returns>
        public static Movements JoinMovements(Movements Mv1, Movements Mv2)
        {
            if (Mv1 == Movements.NONE) return Mv2;
            if (Mv2 == Movements.NONE) return Mv1;

            if (Mv1 == Movements.ROT90CW && Mv2 == Movements.ROT90CW) return Movements.ROT180CW;
            if (Mv1 == Movements.ROT90CCW && Mv2 == Movements.ROT90CCW) return Movements.ROT180CCW;

            if (Mv1 == Movements.ROT90CW && (Mv2 == Movements.ROT180CW || Mv2 == Movements.ROT180CCW) ||
                Mv2 == Movements.ROT90CW && (Mv1 == Movements.ROT180CW || Mv1 == Movements.ROT180CCW)) return Movements.ROT90CCW;

            if (Mv1 == Movements.ROT90CCW && (Mv2 == Movements.ROT180CW || Mv2 == Movements.ROT180CCW) ||
                Mv2 == Movements.ROT90CCW && (Mv1 == Movements.ROT180CW || Mv1 == Movements.ROT180CCW)) return Movements.ROT90CW;

            return Movements.NONE;
        }

        /// <summary>
        /// Module by four operation
        /// </summary>
        /// <param name="n">Integer to calculate module by four</param>
        /// <returns>Result  (0, 1, 2 or 3)</returns>
        public static int mod4(int n) => (n %= 4) < 0 ? n + 4 : n;

        /// <summary>
        /// Updates the texts array for steps
        /// </summary>
        public static void SetStepsTexts()
        {
            s_string[(int)Steps.NONE] = "NONE";

            // Single layer
            s_string[(int)Steps.U] = AMSettings.UChar.ToString();
            s_string[(int)Steps.Up] = s_string[(int)Steps.U] + AMSettings.PrimeChar;
            s_string[(int)Steps.U2] = s_string[(int)Steps.U] + AMSettings.DoubleChar;
            s_string[(int)Steps.U2p] = s_string[(int)Steps.U] + AMSettings.DoubleChar + AMSettings.PrimeChar;

            s_string[(int)Steps.D] = AMSettings.DChar.ToString();
            s_string[(int)Steps.Dp] = s_string[(int)Steps.D] + AMSettings.PrimeChar;
            s_string[(int)Steps.D2] = s_string[(int)Steps.D] + AMSettings.DoubleChar;
            s_string[(int)Steps.D2p] = s_string[(int)Steps.D] + AMSettings.DoubleChar + AMSettings.PrimeChar;

            s_string[(int)Steps.F] = AMSettings.FChar.ToString();
            s_string[(int)Steps.Fp] = s_string[(int)Steps.F] + AMSettings.PrimeChar;
            s_string[(int)Steps.F2] = s_string[(int)Steps.F] + AMSettings.DoubleChar;
            s_string[(int)Steps.F2p] = s_string[(int)Steps.F] + AMSettings.DoubleChar + AMSettings.PrimeChar;

            s_string[(int)Steps.B] = AMSettings.BChar.ToString();
            s_string[(int)Steps.Bp] = s_string[(int)Steps.B] + AMSettings.PrimeChar;
            s_string[(int)Steps.B2] = s_string[(int)Steps.B] + AMSettings.DoubleChar;
            s_string[(int)Steps.B2p] = s_string[(int)Steps.B] + AMSettings.DoubleChar + AMSettings.PrimeChar;

            s_string[(int)Steps.R] = AMSettings.RChar.ToString();
            s_string[(int)Steps.Rp] = s_string[(int)Steps.R] + AMSettings.PrimeChar;
            s_string[(int)Steps.R2] = s_string[(int)Steps.R] + AMSettings.DoubleChar;
            s_string[(int)Steps.R2p] = s_string[(int)Steps.R] + AMSettings.DoubleChar + AMSettings.PrimeChar;

            s_string[(int)Steps.L] = AMSettings.LChar.ToString();
            s_string[(int)Steps.Lp] = s_string[(int)Steps.L] + AMSettings.PrimeChar;
            s_string[(int)Steps.L2] = s_string[(int)Steps.L] + AMSettings.DoubleChar;
            s_string[(int)Steps.L2p] = s_string[(int)Steps.L] + AMSettings.DoubleChar + AMSettings.PrimeChar;

            // Double layer, adjacent layers, same direction
            s_string[(int)Steps.Uw] = AMSettings.UChar.ToString() + AMSettings.wChar;
            s_string[(int)Steps.Uwp] = s_string[(int)Steps.Uw] + AMSettings.PrimeChar;
            s_string[(int)Steps.Uw2] = s_string[(int)Steps.Uw] + AMSettings.DoubleChar;
            s_string[(int)Steps.Uw2p] = s_string[(int)Steps.Uw] + AMSettings.DoubleChar + AMSettings.PrimeChar;

            s_string[(int)Steps.Dw] = AMSettings.DChar.ToString() + AMSettings.wChar;
            s_string[(int)Steps.Dwp] = s_string[(int)Steps.Dw] + AMSettings.PrimeChar;
            s_string[(int)Steps.Dw2] = s_string[(int)Steps.Dw] + AMSettings.DoubleChar;
            s_string[(int)Steps.Dw2p] = s_string[(int)Steps.Dw] + AMSettings.DoubleChar + AMSettings.PrimeChar;

            s_string[(int)Steps.Fw] = AMSettings.FChar.ToString() + AMSettings.wChar;
            s_string[(int)Steps.Fwp] = s_string[(int)Steps.Fw] + AMSettings.PrimeChar;
            s_string[(int)Steps.Fw2] = s_string[(int)Steps.Fw] + AMSettings.DoubleChar;
            s_string[(int)Steps.Fw2p] = s_string[(int)Steps.Fw] + AMSettings.DoubleChar + AMSettings.PrimeChar;

            s_string[(int)Steps.Bw] = AMSettings.BChar.ToString() + AMSettings.wChar;
            s_string[(int)Steps.Bwp] = s_string[(int)Steps.Bw] + AMSettings.PrimeChar;
            s_string[(int)Steps.Bw2] = s_string[(int)Steps.Bw] + AMSettings.DoubleChar;
            s_string[(int)Steps.Bw2p] = s_string[(int)Steps.Bw] + AMSettings.DoubleChar + AMSettings.PrimeChar;

            s_string[(int)Steps.Rw] = AMSettings.RChar.ToString() + AMSettings.wChar;
            s_string[(int)Steps.Rwp] = s_string[(int)Steps.Rw] + AMSettings.PrimeChar;
            s_string[(int)Steps.Rw2] = s_string[(int)Steps.Rw] + AMSettings.DoubleChar;
            s_string[(int)Steps.Rw2p] = s_string[(int)Steps.Rw] + AMSettings.DoubleChar + AMSettings.PrimeChar;

            s_string[(int)Steps.Lw] = AMSettings.LChar.ToString() + AMSettings.wChar;
            s_string[(int)Steps.Lwp] = s_string[(int)Steps.Lw] + AMSettings.PrimeChar;
            s_string[(int)Steps.Lw2] = s_string[(int)Steps.Lw] + AMSettings.DoubleChar;
            s_string[(int)Steps.Lw2p] = s_string[(int)Steps.Lw] + AMSettings.DoubleChar + AMSettings.PrimeChar;

            // Double layer, adjacent layers, opposite direction
            s_string[(int)Steps.Uo] = AMSettings.UChar.ToString() + AMSettings.oChar;
            s_string[(int)Steps.Uop] = s_string[(int)Steps.Uo] + AMSettings.PrimeChar;
            s_string[(int)Steps.Uo2] = s_string[(int)Steps.Uo] + AMSettings.DoubleChar;
            s_string[(int)Steps.Uo2p] = s_string[(int)Steps.Uo] + AMSettings.DoubleChar + AMSettings.PrimeChar;

            s_string[(int)Steps.Do] = AMSettings.DChar.ToString() + AMSettings.oChar;
            s_string[(int)Steps.Dop] = s_string[(int)Steps.Do] + AMSettings.PrimeChar;
            s_string[(int)Steps.Do2] = s_string[(int)Steps.Do] + AMSettings.DoubleChar;
            s_string[(int)Steps.Do2p] = s_string[(int)Steps.Do] + AMSettings.DoubleChar + AMSettings.PrimeChar;

            s_string[(int)Steps.Fo] = AMSettings.FChar.ToString() + AMSettings.oChar;
            s_string[(int)Steps.Fop] = s_string[(int)Steps.Fo] + AMSettings.PrimeChar;
            s_string[(int)Steps.Fo2] = s_string[(int)Steps.Fo] + AMSettings.DoubleChar;
            s_string[(int)Steps.Fo2p] = s_string[(int)Steps.Fo] + AMSettings.DoubleChar + AMSettings.PrimeChar;

            s_string[(int)Steps.Bo] = AMSettings.BChar.ToString() + AMSettings.oChar;
            s_string[(int)Steps.Bop] = s_string[(int)Steps.Bo] + AMSettings.PrimeChar;
            s_string[(int)Steps.Bo2] = s_string[(int)Steps.Bo] + AMSettings.DoubleChar;
            s_string[(int)Steps.Bo2p] = s_string[(int)Steps.Bo] + AMSettings.DoubleChar + AMSettings.PrimeChar;

            s_string[(int)Steps.Ro] = AMSettings.RChar.ToString() + AMSettings.oChar;
            s_string[(int)Steps.Rop] = s_string[(int)Steps.Ro] + AMSettings.PrimeChar;
            s_string[(int)Steps.Ro2] = s_string[(int)Steps.Ro] + AMSettings.DoubleChar;
            s_string[(int)Steps.Ro2p] = s_string[(int)Steps.Ro] + AMSettings.DoubleChar + AMSettings.PrimeChar;

            s_string[(int)Steps.Lo] = AMSettings.LChar.ToString() + AMSettings.oChar;
            s_string[(int)Steps.Lop] = s_string[(int)Steps.Lo] + AMSettings.PrimeChar;
            s_string[(int)Steps.Lo2] = s_string[(int)Steps.Lo] + AMSettings.DoubleChar;
            s_string[(int)Steps.Lo2p] = s_string[(int)Steps.Lo] + AMSettings.DoubleChar + AMSettings.PrimeChar;

            // Double layer, opposite layers, same direction
            s_string[(int)Steps.Us] = AMSettings.UChar.ToString() + AMSettings.sChar;
            s_string[(int)Steps.Usp] = s_string[(int)Steps.Us] + AMSettings.PrimeChar;
            s_string[(int)Steps.Us2] = s_string[(int)Steps.Us] + AMSettings.DoubleChar;
            s_string[(int)Steps.Us2p] = s_string[(int)Steps.Us] + AMSettings.DoubleChar + AMSettings.PrimeChar;

            s_string[(int)Steps.Ds] = AMSettings.DChar.ToString() + AMSettings.sChar;
            s_string[(int)Steps.Dsp] = s_string[(int)Steps.Ds] + AMSettings.PrimeChar;
            s_string[(int)Steps.Ds2] = s_string[(int)Steps.Ds] + AMSettings.DoubleChar;
            s_string[(int)Steps.Ds2p] = s_string[(int)Steps.Ds] + AMSettings.DoubleChar + AMSettings.PrimeChar;

            s_string[(int)Steps.Fs] = AMSettings.FChar.ToString() + AMSettings.sChar;
            s_string[(int)Steps.Fsp] = s_string[(int)Steps.Fs] + AMSettings.PrimeChar;
            s_string[(int)Steps.Fs2] = s_string[(int)Steps.Fs] + AMSettings.DoubleChar;
            s_string[(int)Steps.Fs2p] = s_string[(int)Steps.Fs] + AMSettings.DoubleChar + AMSettings.PrimeChar;

            s_string[(int)Steps.Bs] = AMSettings.BChar.ToString() + AMSettings.sChar;
            s_string[(int)Steps.Bsp] = s_string[(int)Steps.Bs] + AMSettings.PrimeChar;
            s_string[(int)Steps.Bs2] = s_string[(int)Steps.Bs] + AMSettings.DoubleChar;
            s_string[(int)Steps.Bs2p] = s_string[(int)Steps.Bs] + AMSettings.DoubleChar + AMSettings.PrimeChar;

            s_string[(int)Steps.Rs] = AMSettings.RChar.ToString() + AMSettings.sChar;
            s_string[(int)Steps.Rsp] = s_string[(int)Steps.Rs] + AMSettings.PrimeChar;
            s_string[(int)Steps.Rs2] = s_string[(int)Steps.Rs] + AMSettings.DoubleChar;
            s_string[(int)Steps.Rs2p] = s_string[(int)Steps.Rs] + AMSettings.DoubleChar + AMSettings.PrimeChar;

            s_string[(int)Steps.Ls] = AMSettings.LChar.ToString() + AMSettings.sChar;
            s_string[(int)Steps.Lsp] = s_string[(int)Steps.Ls] + AMSettings.PrimeChar;
            s_string[(int)Steps.Ls2] = s_string[(int)Steps.Ls] + AMSettings.DoubleChar;
            s_string[(int)Steps.Ls2p] = s_string[(int)Steps.Ls] + AMSettings.DoubleChar + AMSettings.PrimeChar;

            // Double layer, opposite layers, opposite direction
            s_string[(int)Steps.Ua] = AMSettings.UChar.ToString() + AMSettings.aChar;
            s_string[(int)Steps.Uap] = s_string[(int)Steps.Ua] + AMSettings.PrimeChar;
            s_string[(int)Steps.Ua2] = s_string[(int)Steps.Ua] + AMSettings.DoubleChar;
            s_string[(int)Steps.Ua2p] = s_string[(int)Steps.Ua] + AMSettings.DoubleChar + AMSettings.PrimeChar;

            s_string[(int)Steps.Da] = AMSettings.DChar.ToString() + AMSettings.aChar;
            s_string[(int)Steps.Dap] = s_string[(int)Steps.Da] + AMSettings.PrimeChar;
            s_string[(int)Steps.Da2] = s_string[(int)Steps.Da] + AMSettings.DoubleChar;
            s_string[(int)Steps.Da2p] = s_string[(int)Steps.Da] + AMSettings.DoubleChar + AMSettings.PrimeChar;

            s_string[(int)Steps.Fa] = AMSettings.FChar.ToString() + AMSettings.aChar;
            s_string[(int)Steps.Fap] = s_string[(int)Steps.Fa] + AMSettings.PrimeChar;
            s_string[(int)Steps.Fa2] = s_string[(int)Steps.Fa] + AMSettings.DoubleChar;
            s_string[(int)Steps.Fa2p] = s_string[(int)Steps.Fa] + AMSettings.DoubleChar + AMSettings.PrimeChar;

            s_string[(int)Steps.Ba] = AMSettings.BChar.ToString() + AMSettings.aChar;
            s_string[(int)Steps.Bap] = s_string[(int)Steps.Ba] + AMSettings.PrimeChar;
            s_string[(int)Steps.Ba2] = s_string[(int)Steps.Ba] + AMSettings.DoubleChar;
            s_string[(int)Steps.Ba2p] = s_string[(int)Steps.Ba] + AMSettings.DoubleChar + AMSettings.PrimeChar;

            s_string[(int)Steps.Ra] = AMSettings.RChar.ToString() + AMSettings.aChar;
            s_string[(int)Steps.Rap] = s_string[(int)Steps.Ra] + AMSettings.PrimeChar;
            s_string[(int)Steps.Ra2] = s_string[(int)Steps.Ra] + AMSettings.DoubleChar;
            s_string[(int)Steps.Ra2p] = s_string[(int)Steps.Ra] + AMSettings.DoubleChar + AMSettings.PrimeChar;

            s_string[(int)Steps.La] = AMSettings.LChar.ToString() + AMSettings.aChar;
            s_string[(int)Steps.Lap] = s_string[(int)Steps.La] + AMSettings.PrimeChar;
            s_string[(int)Steps.La2] = s_string[(int)Steps.La] + AMSettings.DoubleChar;
            s_string[(int)Steps.La2p] = s_string[(int)Steps.La] + AMSettings.DoubleChar + AMSettings.PrimeChar;

            // Middle layers
            s_string[(int)Steps.E] = AMSettings.EChar.ToString();
            s_string[(int)Steps.Ep] = s_string[(int)Steps.E] + AMSettings.PrimeChar;
            s_string[(int)Steps.E2] = s_string[(int)Steps.E] + AMSettings.DoubleChar;
            s_string[(int)Steps.E2p] = s_string[(int)Steps.E] + AMSettings.DoubleChar + AMSettings.PrimeChar;

            s_string[(int)Steps.S] = AMSettings.SChar.ToString();
            s_string[(int)Steps.Sp] = s_string[(int)Steps.S] + AMSettings.PrimeChar;
            s_string[(int)Steps.S2] = s_string[(int)Steps.S] + AMSettings.DoubleChar;
            s_string[(int)Steps.S2p] = s_string[(int)Steps.S] + AMSettings.DoubleChar + AMSettings.PrimeChar;

            s_string[(int)Steps.M] = AMSettings.MChar.ToString();
            s_string[(int)Steps.Mp] = s_string[(int)Steps.M] + AMSettings.PrimeChar;
            s_string[(int)Steps.M2] = s_string[(int)Steps.M] + AMSettings.DoubleChar;
            s_string[(int)Steps.M2p] = s_string[(int)Steps.M] + AMSettings.DoubleChar + AMSettings.PrimeChar;

            // Full cube
            s_string[(int)Steps.x] = AMSettings.xChar.ToString();
            s_string[(int)Steps.xp] = s_string[(int)Steps.x] + AMSettings.PrimeChar;
            s_string[(int)Steps.x2] = s_string[(int)Steps.x] + AMSettings.DoubleChar;
            s_string[(int)Steps.x2p] = s_string[(int)Steps.x] + AMSettings.DoubleChar + AMSettings.PrimeChar;

            s_string[(int)Steps.y] = AMSettings.yChar.ToString();
            s_string[(int)Steps.yp] = s_string[(int)Steps.y] + AMSettings.PrimeChar;
            s_string[(int)Steps.y2] = s_string[(int)Steps.y] + AMSettings.DoubleChar;
            s_string[(int)Steps.y2p] = s_string[(int)Steps.y] + AMSettings.DoubleChar + AMSettings.PrimeChar;

            s_string[(int)Steps.z] = AMSettings.zChar.ToString();
            s_string[(int)Steps.zp] = s_string[(int)Steps.z] + AMSettings.PrimeChar;
            s_string[(int)Steps.z2] = s_string[(int)Steps.z] + AMSettings.DoubleChar;
            s_string[(int)Steps.z2p] = s_string[(int)Steps.z] + AMSettings.DoubleChar + AMSettings.PrimeChar;

            s_string[(int)Steps.OPEN_PARENTHESIS] = AMSettings.OpenParenthesisChar.ToString();
            s_string[(int)Steps.CLOSE_PARENTHESIS_1_REP] = AMSettings.CloseParenthesisChar.ToString();
            for (int i = (int)Steps.CLOSE_PARENTHESIS_2_REP; i <= (int)Steps.CLOSE_PARENTHESIS_9_REP; i++)
                s_string[i] = s_string[(int)Steps.CLOSE_PARENTHESIS_1_REP] + ((int)(i - Steps.OPEN_PARENTHESIS)).ToString();

            s_string.CopyTo(s_string_base, 0);

            // Double layer, adjacent layers, same direction
            if (AMSettings.UsingAltwChars)
            {
                s_string[(int)Steps.Uw] = AMSettings.AltUwChar.ToString();
                s_string[(int)Steps.Uwp] = s_string[(int)Steps.Uw] + AMSettings.PrimeChar;
                s_string[(int)Steps.Uw2] = s_string[(int)Steps.Uw] + AMSettings.DoubleChar;
                s_string[(int)Steps.Uw2p] = s_string[(int)Steps.Uw] + AMSettings.DoubleChar + AMSettings.PrimeChar;

                s_string[(int)Steps.Dw] = AMSettings.AltDwChar.ToString();
                s_string[(int)Steps.Dwp] = s_string[(int)Steps.Dw] + AMSettings.PrimeChar;
                s_string[(int)Steps.Dw2] = s_string[(int)Steps.Dw] + AMSettings.DoubleChar;
                s_string[(int)Steps.Dw2p] = s_string[(int)Steps.Dw] + AMSettings.DoubleChar + AMSettings.PrimeChar;

                s_string[(int)Steps.Fw] = AMSettings.AltFwChar.ToString();
                s_string[(int)Steps.Fwp] = s_string[(int)Steps.Fw] + AMSettings.PrimeChar;
                s_string[(int)Steps.Fw2] = s_string[(int)Steps.Fw] + AMSettings.DoubleChar;
                s_string[(int)Steps.Fw2p] = s_string[(int)Steps.Fw] + AMSettings.DoubleChar + AMSettings.PrimeChar;

                s_string[(int)Steps.Bw] = AMSettings.AltBwChar.ToString();
                s_string[(int)Steps.Bwp] = s_string[(int)Steps.Bw] + AMSettings.PrimeChar;
                s_string[(int)Steps.Bw2] = s_string[(int)Steps.Bw] + AMSettings.DoubleChar;
                s_string[(int)Steps.Bw2p] = s_string[(int)Steps.Bw] + AMSettings.DoubleChar + AMSettings.PrimeChar;

                s_string[(int)Steps.Rw] = AMSettings.AltRwChar.ToString();
                s_string[(int)Steps.Rwp] = s_string[(int)Steps.Rw] + AMSettings.PrimeChar;
                s_string[(int)Steps.Rw2] = s_string[(int)Steps.Rw] + AMSettings.DoubleChar;
                s_string[(int)Steps.Rw2p] = s_string[(int)Steps.Rw] + AMSettings.DoubleChar + AMSettings.PrimeChar;

                s_string[(int)Steps.Lw] = AMSettings.AltLwChar.ToString();
                s_string[(int)Steps.Lwp] = s_string[(int)Steps.Lw] + AMSettings.PrimeChar;
                s_string[(int)Steps.Lw2] = s_string[(int)Steps.Lw] + AMSettings.DoubleChar;
                s_string[(int)Steps.Lw2p] = s_string[(int)Steps.Lw] + AMSettings.DoubleChar + AMSettings.PrimeChar;
            }
        }

        /// <summary>
        /// Step from string (exact match)
        /// </summary>
        /// <param name="ss">Step string</param>
        /// <returns>Scramble step</returns>
        public static Steps StepFromText(string ss)
        {
            if (string.IsNullOrEmpty(ss)) return Steps.NONE;

            string cs = GetCleanStepString(ss, 0, out int ns);

            if (string.IsNullOrEmpty(cs) || cs.Length > 4) return Steps.NONE;

            for (int n = 0; n < s_string.Length; n++)
                if (string.Compare(cs, s_string_base[n]) == 0)
                {
                    Steps AuxStep = (Steps)n;
                    if (AuxStep >= Steps.NONE && AuxStep <= Steps.CLOSE_PARENTHESIS_9_REP) return AuxStep;
                    return Steps.NONE;
                }

            return Steps.NONE;
        }

        /// <summary>
        /// Remove no step characters from a string and get clean step text.
        /// If there is more than one scramble step, the rest are ignored.
        /// </summary>
        /// <param name="S">String to process</param>
        /// <param name="Pos">Character position for starting search of first step</param>
        /// <param name="NextPos">Output: character position where the next step starts. (-1 if there isn't a next step)</param>
        /// <returns>Clean step string</returns>
        public static string GetCleanStepString(string Ss, int Pos, out int NextPos)
        {
            if (string.IsNullOrWhiteSpace(Ss))
            {
                NextPos = 0;
                return string.Empty;
            }

            // Replace "'2" modifiers by "2'"
            string S = Ss.Replace(AMSettings.PrimeChar.ToString() + AMSettings.DoubleChar,
                                  AMSettings.DoubleChar.ToString() + AMSettings.PrimeChar);

            System.Text.StringBuilder sb = new System.Text.StringBuilder(S.Length + 1);

            // Builds a chars array with all posible main chars in the step (step's first char)
            char[] MainCharsStepArray = AMSettings.GetMainCharsStepArray();

            int FirstMainCharIndex = -1,
                SecondMainCharIndex = -1,
                FirstMainCharTypeIndex = -1,
                ModifierCharIndex = -1,
                FirstMovementCharIndex = -1;

            bool ModifierCharFound = false,
                 MovementCharFound = false;

            // The string is traversed looking for the main step character
            for (int i = Pos; i < S.Length; i++)
            {
                // Check if current char is an additional end step character: '(', ')' or a cipher > 1
                foreach (char c in AMSettings.EndStepChars)
                    if (c == S[i])
                    {
                        if (c == AMSettings.OpenParenthesisChar && FirstMainCharIndex < 0)
                        {
                            sb.Append(AMSettings.OpenParenthesisChar);
                            NextPos = i + 1;
                            return sb.ToString();
                        }
                        if (c == AMSettings.CloseParenthesisChar && FirstMainCharIndex < 0)
                        {
                            // If exists repetitions must be a number attached to the parenthesis
                            sb.Append(AMSettings.CloseParenthesisChar);
                            if (i + 1 >= S.Length) NextPos = i + 1;
                            else
                            {
                                int rep = (int)char.GetNumericValue(S[i + 1]);

                                if (rep > 1 && rep < 10)
                                {
                                    sb.Append(rep);
                                    NextPos = i + 2;
                                }
                                else NextPos = i + 1;
                            }
                            return sb.ToString();
                        }
                        SecondMainCharIndex = i;
                    }
                if (SecondMainCharIndex >= 0) break;

                // Check if current char is a main step character: 'U', 'r', 'x', ...
                for (int j = 0; j < MainCharsStepArray.Length; j++)
                {
                    if (S[i] == MainCharsStepArray[j])
                    {
                        if (FirstMainCharIndex < 0)
                        {
                            FirstMainCharIndex = i;
                            FirstMainCharTypeIndex = j;
                        }
                        else SecondMainCharIndex = i;
                        break;
                    }
                }
                if (SecondMainCharIndex >= 0) break;
            }

            if (FirstMainCharIndex < 0)
            {
                if (SecondMainCharIndex < 0) NextPos = S.Length;
                else NextPos = SecondMainCharIndex;
                return ""; // No step found!!!
            }

            // Add main char step to return string 
            if (FirstMainCharTypeIndex < AMSettings.LayerChars.Length + AMSettings.RotationsChars.Length)
            {
                sb.Append(S[FirstMainCharIndex]);
                NextPos = FirstMainCharIndex + 1;
            }
            else // If it's alternative, add modifier too: u = Uw, r = Rw, ...
            {
                sb.Append(char.ToUpper(S[FirstMainCharIndex]));
                sb.Append(AMSettings.wChar);
                NextPos = FirstMainCharIndex + 1;
                ModifierCharFound = true;
                ModifierCharIndex = FirstMainCharIndex;
            }

            // SecondMainCharIndex marks until where the step is present in the string
            if (SecondMainCharIndex < 0) SecondMainCharIndex = S.Length;

            // Search for modifier char (could not be present)
            if (!ModifierCharFound)
            {
                for (int i = FirstMainCharIndex + 1; i < SecondMainCharIndex; i++)
                {
                    if (!ModifierCharFound && !MovementCharFound)
                    {
                        foreach (char c in AMSettings.ModifiersChars)
                        {
                            if (c == S[i])
                            {
                                ModifierCharFound = true;
                                ModifierCharIndex = i;
                                break;
                            }
                        }
                        foreach (char c in AMSettings.MovementsChars) // If movement char found first, no modifier char presence
                        {
                            if (c == S[i])
                            {
                                MovementCharFound = true;
                                FirstMovementCharIndex = i;
                                break;
                            }
                        }
                    }
                    if (ModifierCharFound || MovementCharFound) break;
                }
            }
            else // Modifier char already present in return string
            {
                for (int i = FirstMainCharIndex + 1; i < SecondMainCharIndex; i++)
                {
                    foreach (char c in AMSettings.MovementsChars)
                    {
                        if (c == S[i])
                        {
                            MovementCharFound = true;
                            FirstMovementCharIndex = i;
                            break;
                        }
                    }
                    if (MovementCharFound) break;
                }
                ModifierCharFound = false; // Already present, must not be appended again
            }

            // If found a movement char '2', check if there is a second movement char '\''
            if (!ModifierCharFound && MovementCharFound)
            {
                sb.Append(S[FirstMovementCharIndex]);
                if (S[FirstMovementCharIndex] == AMSettings.PrimeChar) // Movement char is '\'' (final char for step)
                {
                    NextPos = FirstMovementCharIndex + 1;
                    return sb.ToString();
                }
                else // Movement is '2', check for a '\'' char
                {
                    for (int i = FirstMovementCharIndex + 1; i < SecondMainCharIndex; i++)
                    {
                        if (S[i] == AMSettings.PrimeChar)
                        {
                            sb.Append(S[i]);
                            NextPos = i + 1;
                            return sb.ToString(); // Step finish in "2'"
                        }
                    }

                    NextPos = FirstMovementCharIndex + 1;
                    sb.ToString(); // No '\'' char found, return string
                }
            }

            if (ModifierCharFound && !MovementCharFound)
            {
                sb.Append(S[ModifierCharIndex]);

                // Search for first movement char
                for (int i = ModifierCharIndex + 1; i < SecondMainCharIndex; i++)
                {
                    foreach (char c in AMSettings.MovementsChars)
                    {
                        if (c == S[i])
                        {
                            MovementCharFound = true;
                            FirstMovementCharIndex = i;
                            break;
                        }
                    }
                    if (MovementCharFound) break;
                }

                if (!MovementCharFound)
                {
                    NextPos = ModifierCharIndex + 1;
                    return sb.ToString();
                }

                sb.Append(S[FirstMovementCharIndex]);

                if (S[FirstMovementCharIndex] == AMSettings.PrimeChar)
                {
                    NextPos = FirstMovementCharIndex + 1;
                    return sb.ToString(); // Movement char is '\'', return string.
                }

                // Movement is '2', check for a '\'' char
                for (int i = FirstMovementCharIndex + 1; i < SecondMainCharIndex; i++)
                {
                    if (S[i] == AMSettings.PrimeChar)
                    {
                        sb.Append(S[i]);
                        NextPos = i + 1;
                        return sb.ToString(); // Step finish in "2'"
                    }
                }
                NextPos = FirstMovementCharIndex + 1;
                return sb.ToString(); // No '\'' char found, return string
            }

            // No modifier neither movement chars
            return sb.ToString();
        }

        /// <summary>
        /// Get the close parenthesis step with given repetitions
        /// </summary>
        /// <param name="rep">Number of repetitions of parentheses content</param>
        /// <returns>Close parenthesis step with given repetitions</returns>
        public static Steps GetCloseParenthesis(int rep)
        {
            if (rep < 1) rep = 0;
            else if (rep > 9) rep = 8;
            else rep--;

            return Steps.CLOSE_PARENTHESIS_1_REP + rep;
        }

        /// <summary>
        /// Gets a compound step as an array of three basic steps
        /// </summary>
        /// <returns>Array of threee steps</returns>
        public static Steps[] GetEquivalentStepSequence(Steps St)
        {
            if ((AMSettings.RotationOfEAsD && (St == Steps.E || St == Steps.Ep)) || 
                (AMSettings.RotationOfSAsB && (St == Steps.S || St == Steps.Sp)) ||
                (AMSettings.RotationOfMAsL && (St == Steps.M || St == Steps.Mp))) St = Inverse(St);

            if (St >= Steps.NONE && St <= Steps.CLOSE_PARENTHESIS_9_REP) return s_seq[(int)St];
            return s_seq[(int)Steps.NONE];
        }

        #endregion General functions

        #region Step turns

        /// <summary>
        /// Gets the step with x turn
        /// </summary>
        public static Steps x(Steps Stp)
        {
            if ((AMSettings.RotationOfEAsU != AMSettings.RotationOfSAsF) &&
                (Stp == Steps.E || Stp == Steps.Ep || Stp == Steps.S || Stp == Steps.Sp)) Stp = Inverse(Stp);
            return s_x[(int)Stp];
        }

        /// <summary>
        /// Gets the step with x' turn
        /// </summary>
        public static Steps xp(Steps Stp)
        {
            if ((AMSettings.RotationOfEAsU != AMSettings.RotationOfSAsF) && 
                (Stp == Steps.E || Stp == Steps.Ep || Stp == Steps.S || Stp == Steps.Sp)) Stp = Inverse(Stp);
            return s_xp[(int)Stp];
        }

        /// <summary>
        /// Gets the step with x2 turn
        /// </summary>
        public static Steps x2(Steps Stp) => s_x2[(int)Stp];

        /// <summary>
        /// Gets the step with y turn
        /// </summary>
        public static Steps y(Steps Stp)
        {
            if ((AMSettings.RotationOfSAsF != AMSettings.RotationOfMAsR) &&
                (Stp == Steps.S || Stp == Steps.Sp || Stp == Steps.M || Stp == Steps.Mp)) Stp = Inverse(Stp);
            return s_y[(int)Stp];
        }

        /// <summary>
        /// Gets the step with y' turn
        /// </summary>
        public static Steps yp(Steps Stp)
        {
            if ((AMSettings.RotationOfSAsF != AMSettings.RotationOfMAsR) &&
                (Stp == Steps.S || Stp == Steps.Sp || Stp == Steps.M || Stp == Steps.Mp)) Stp = Inverse(Stp);
            return s_yp[(int)Stp];
        }

        /// <summary>
        /// Gets the step with y2 turn
        /// </summary>
        public static Steps y2(Steps Stp) => s_y2[(int)Stp];

        /// <summary>
        /// Gets the step with z turn
        /// </summary>
        public static Steps z(Steps Stp)
        {
            if ((AMSettings.RotationOfEAsU != AMSettings.RotationOfMAsR) &&
                (Stp == Steps.E || Stp == Steps.Ep || Stp == Steps.M || Stp == Steps.Mp)) Stp = Inverse(Stp);
            return s_z[(int)Stp];
        }

        /// <summary>
        /// Gets the step with z' turn
        /// </summary>
        public static Steps zp(Steps Stp)
        {
            if ((AMSettings.RotationOfEAsU != AMSettings.RotationOfMAsR) &&
                (Stp == Steps.E || Stp == Steps.Ep || Stp == Steps.M || Stp == Steps.Mp)) Stp = Inverse(Stp);
            return s_zp[(int)Stp];
        }

        /// <summary>
        /// Gets the step with z2 turn
        /// </summary>
        public static Steps z2(Steps Stp) => s_z2[(int)Stp];

        /// <summary>
        /// Apply x rotation to the step n times
        /// </summary>
        /// <param name="n">Number of x rotations</param>
        public static Steps x(Steps Stp, int n)
        {
            int turns = mod4(n);
            if (turns == 1) return x(Stp);
            else if (turns == 2) return x2(Stp);
            else return xp(Stp);
        }

        /// <summary>
        /// Apply y rotation to the step n times
        /// </summary>
        /// <param name="n">Number of y rotations</param>
        public static Steps y(Steps Stp, int n)
        {
            int turns = mod4(n);
            if (turns == 1) return y(Stp);
            else if (turns == 2) return y2(Stp);
            else return yp(Stp);
        }

        /// <summary>
        /// Apply z rotation to the step n times
        /// </summary>
        /// <param name="n">Number of z rotations</param>
        public static Steps z(Steps Stp, int n)
        {
            int turns = mod4(n);
            if (turns == 1) return z(Stp);
            else if (turns == 2) return z2(Stp);
            else return zp(Stp);
        }

        #endregion Step turns
    }
}
