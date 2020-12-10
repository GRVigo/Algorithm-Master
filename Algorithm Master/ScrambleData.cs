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

using System.Collections.Generic;

namespace Algorithm_Master
{
    /// <summary>
    /// Class to manage a scramble
    /// </summary>
    public class Scramble
    {
        #region Fields

        /// <summary>
        /// List of steps
        /// </summary>
        private List<Steps> StepsList;
        
        #endregion Fields

        #region Properties

        /// <summary>
        /// Current number of steps in the scramble
        /// </summary>
        public int Length => StepsList.Count;

        /// <summary>
        /// Checks if the scramble has no steps
        /// </summary>
        public bool IsEmpty => Length == 0;

        /// <summary>
        /// Check if scramble parentheses are OK (nesting = 0 and first parenthesis must be open)
        /// </summary>
        public bool AreParenthesesOK
        {
            get
            {
                int n = 0;
                while (n < Length && StepsList[n] != Steps.OPEN_PARENTHESIS)
                { // First parenthesis must be open parenthesis
                    if (ScrambleStep.IsCloseParenthesis(StepsList[n])) return false;
                    n++;
                }
                return NestValue == 0;
            }
        }

        /// <summary>
        /// Checks if a scramble has parentheses
        /// </summary>
        public bool HasParentheses
        {
            get
            {
                for (int i = 0; i < Length; i++) if (ScrambleStep.IsParenthesis(StepsList[i])) return true;
                return false;
            }
        }

        /// <summary>
        /// Checks if a scramble has parentheses with repetitions
        /// </summary>
        public bool HasParenthesesWithRepetitions
        {
            get
            {
                for (int i = 0; i < Length; i++)
                    if (ScrambleStep.Modifier(StepsList[i]) >= Modifiers.CLOSE_PARENTHESIS_2_REP) return true;
                return false;
            }
        }

        /// <summary>
        /// Check if a scramble has turns
        /// </summary>
        public bool HasTurns
        {
            get
            {
                for (int i = 0; i < Length; i++) if (ScrambleStep.IsTurn(StepsList[i])) return true;
                return false;
            }
        }

        /// <summary>
        /// Gets the nesting value of parentheses (0 if parentheses are OK, more than 0 if more open than close and less than 0 viceversa
        /// </summary>
        public int NestValue
        {
            get
            {
                int nest = 0;
                for (int i = 0; i < Length; i++)
                {
                    if (StepsList[i] == Steps.OPEN_PARENTHESIS) nest++;
                    else if (ScrambleStep.IsCloseParenthesis(StepsList[i])) nest--;
                }
                return nest;
            }
        }

        #endregion Properties

        #region Constructors & indexer

        /// <summary>
        /// Constructor: creates an empty scramble
        /// </summary>
        public Scramble()
        {
            StepsList = new List<Steps>();
        }

        /// <summary>
        /// Constructor: creates a random scramble
        /// </summary>
        /// <param name="Size">Number of steps</param>
        /// <param name="Seed">Seed for random number sequence</param>
        public Scramble(int Size, int Seed)
        {
            StepsList = new List<Steps>();

            if (Size < 1) return;

            System.Random RandomGenerator = new System.Random((int)(System.DateTime.Now.Ticks % int.MaxValue));

            while (Length < Size)
            {
                int RandomStep = RandomGenerator.Next((int)Steps.U, (int)Steps.L2p + 1); // From U to L; layers E, S and M excluded 
                if (ScrambleStep.Movement((Steps)RandomStep) != Movements.ROT180CCW) AddStepAndShrink((Steps)RandomStep);
            }
            for (int n = 0; n < Length; n++)
                if (ScrambleStep.Movement(StepsList[n]) == Movements.ROT180CCW)
                    StepsList[n] = ScrambleStep.Inverse(StepsList[n]);
        }

        /// <summary>
        /// Class indexer
        /// </summary>
        /// <param name="StepNum">Step position</param>
        /// <returns>Step in given position</returns>
        public Steps this[int StepNum] => StepNum >= 0 && StepNum < Length ? StepsList[StepNum] : Steps.NONE;

        #endregion Constructors & indexer

        #region Adding basic steps and turns

        /// <summary>
        /// Add U movement to the scramble
        /// </summary>
        /// <param name="n">Number of movements to be added</param>
        public void U(int n)
        {
            switch (ScrambleStep.mod4(n))
            {
                case 1: AddStepAndShrink(Steps.U); break;
                case 2: AddStepAndShrink(Steps.U2); break;
                case 3: AddStepAndShrink(Steps.Up); break;
            }
        }

        /// <summary>
        /// Add D movement to the scramble
        /// </summary>
        /// <param name="n">Number of movements to be added</param>
        public void D(int n)
        {
            switch (ScrambleStep.mod4(n))
            {
                case 1: AddStepAndShrink(Steps.D); break;
                case 2: AddStepAndShrink(Steps.D2); break;
                case 3: AddStepAndShrink(Steps.Dp); break;
            }
        }

        /// <summary>
        /// Add F movement to the scramble
        /// </summary>
        /// <param name="n">Number of movements to be added</param>
        public void F(int n)
        {
            switch (ScrambleStep.mod4(n))
            {
                case 1: AddStepAndShrink(Steps.F); break;
                case 2: AddStepAndShrink(Steps.F2); break;
                case 3: AddStepAndShrink(Steps.Fp); break;
            }
        }

        /// <summary>
        /// Add B movement to the scramble
        /// </summary>
        /// <param name="n">Number of movements to be added</param>
        public void B(int n)
        {
            switch (ScrambleStep.mod4(n))
            {
                case 1: AddStepAndShrink(Steps.B); break;
                case 2: AddStepAndShrink(Steps.B2); break;
                case 3: AddStepAndShrink(Steps.Bp); break;
            }
        }

        /// <summary>
        /// Add R movement to the scramble
        /// </summary>
        /// <param name="n">Number of movements to be added</param>
        public void R(int n)
        {
            switch (ScrambleStep.mod4(n))
            {
                case 1: AddStepAndShrink(Steps.R); break;
                case 2: AddStepAndShrink(Steps.R2); break;
                case 3: AddStepAndShrink(Steps.Rp); break;
            }
        }

        /// <summary>
        /// Add L movement to the scramble
        /// </summary>
        /// <param name="n">Number of movements to be added</param>
        public void L(int n)
        {
            switch (ScrambleStep.mod4(n))
            {
                case 1: AddStepAndShrink(Steps.L); break;
                case 2: AddStepAndShrink(Steps.L2); break;
                case 3: AddStepAndShrink(Steps.Lp); break;
            }
        }

        /// <summary>
        /// Add x movement to the scramble
        /// </summary>
        /// <param name="n">Number of movements to be added</param>
        public void x(int n)
        {
            switch (ScrambleStep.mod4(n))
            {
                case 1: AddStepAndShrink(Steps.x); break;
                case 2: AddStepAndShrink(Steps.x2); break;
                case 3: AddStepAndShrink(Steps.xp); break;
            }
        }

        /// <summary>
        /// Add y movement to the scramble
        /// </summary>
        /// <param name="n">Number of movements to be added</param>
        public void y(int n)
        {
            switch (ScrambleStep.mod4(n))
            {
                case 1: AddStepAndShrink(Steps.y); break;
                case 2: AddStepAndShrink(Steps.y2); break;
                case 3: AddStepAndShrink(Steps.yp); break;
            }
        }

        /// <summary>
        /// Add z movement to the scramble
        /// </summary>
        /// <param name="n">Number of movements to be added</param>
        public void z(int n)
        {
            switch (ScrambleStep.mod4(n))
            {
                case 1: AddStepAndShrink(Steps.z); break;
                case 2: AddStepAndShrink(Steps.z2); break;
                case 3: AddStepAndShrink(Steps.zp); break;
            }
        }

        #endregion

        #region Adding compound steps

        /// <summary>
        /// E layer movement n times
        /// </summary>
        /// <param name="n">Number of movements to be added</param>
        public void E(int n)
        {
            switch (ScrambleStep.mod4(n))
            {
                case 1: AddStepAndShrink(Steps.E); break;
                case 2: AddStepAndShrink(Steps.E2); break;
                case 3: AddStepAndShrink(Steps.Ep); break;
            }
        }

        /// <summary>
        /// S layer movement n times
        /// </summary>
        /// <param name="n">Number of movements to be added</param>
        public void S(int n)
        {
            switch (ScrambleStep.mod4(n))
            {
                case 1: AddStepAndShrink(Steps.S); break;
                case 2: AddStepAndShrink(Steps.S2); break;
                case 3: AddStepAndShrink(Steps.Sp); break;
            }
        }

        /// <summary>
        /// M layer movement n times
        /// </summary>
        /// <param name="n">Number of M movements to be added</param>
        public void M(int n)
        {
            switch (ScrambleStep.mod4(n))
            {
                case 1: AddStepAndShrink(Steps.M); break;
                case 2: AddStepAndShrink(Steps.M2); break;
                case 3: AddStepAndShrink(Steps.Mp); break;
            }
        }

        /// <summary>
        /// Add Uw (u) movement to the scramble
        /// </summary>
        /// <param name="n">Number of movements to be added</param>
        public void Uw(int n)
        {
            switch (ScrambleStep.mod4(n))
            {
                case 1: AddStepAndShrink(Steps.Uw); break;
                case 2: AddStepAndShrink(Steps.Uw2); break;
                case 3: AddStepAndShrink(Steps.Uwp); break;
            }
        }

        /// <summary>
        /// Add Dw (d) movement to the scramble
        /// </summary>
        /// <param name="n">Number of movements to be added</param>
        public void Dw(int n)
        {
            switch (ScrambleStep.mod4(n))
            {
                case 1: AddStepAndShrink(Steps.Dw); break;
                case 2: AddStepAndShrink(Steps.Dw2); break;
                case 3: AddStepAndShrink(Steps.Dwp); break;
            }
        }

        /// <summary>
        /// Add Fw (f) movement to the scramble
        /// </summary>
        /// <param name="n">Number of movements to be added</param>
        public void Fw(int n)
        {
            switch (ScrambleStep.mod4(n))
            {
                case 1: AddStepAndShrink(Steps.Fw); break;
                case 2: AddStepAndShrink(Steps.Fw2); break;
                case 3: AddStepAndShrink(Steps.Fwp); break;
            }
        }

        /// <summary>
        /// Add Bw (b) movement to the scramble
        /// </summary>
        /// <param name="n">Number of movements to be added</param>
        public void Bw(int n)
        {
            switch (ScrambleStep.mod4(n))
            {
                case 1: AddStepAndShrink(Steps.Bw); break;
                case 2: AddStepAndShrink(Steps.Bw2); break;
                case 3: AddStepAndShrink(Steps.Bwp); break;
            }
        }

        /// <summary>
        /// Add Rw (r) movement to the scramble
        /// </summary>
        /// <param name="n">Number of movements to be added</param>
        public void Rw(int n)
        {
            switch (ScrambleStep.mod4(n))
            {
                case 1: AddStepAndShrink(Steps.Rw); break;
                case 2: AddStepAndShrink(Steps.Rw2); break;
                case 3: AddStepAndShrink(Steps.Rwp); break;
            }
        }

        /// <summary>
        /// Add Lw (l) movement to the scramble
        /// </summary>
        /// <param name="n">Number of movements to be added</param>
        public void Lw(int n)
        {
            switch (ScrambleStep.mod4(n))
            {
                case 1: AddStepAndShrink(Steps.Lw); break;
                case 2: AddStepAndShrink(Steps.Lw2); break;
                case 3: AddStepAndShrink(Steps.Lwp); break;
            }
        }

        /// <summary>
        /// Add Uo movement to the scramble
        /// </summary>
        /// <param name="n">Number of movements to be added</param>
        public void Uo(int n)
        {
            switch (ScrambleStep.mod4(n))
            {
                case 1: AddStepAndShrink(Steps.Uo); break;
                case 2: AddStepAndShrink(Steps.Uo2); break;
                case 3: AddStepAndShrink(Steps.Uop); break;
            }
        }

        /// <summary>
        /// Add Do movement to the scramble
        /// </summary>
        /// <param name="n">Number of movements to be added</param>
        public void Do(int n)
        {
            switch (ScrambleStep.mod4(n))
            {
                case 1: AddStepAndShrink(Steps.Do); break;
                case 2: AddStepAndShrink(Steps.Do2); break;
                case 3: AddStepAndShrink(Steps.Dop); break;
            }
        }

        /// <summary>
        /// Add Fo movement to the scramble
        /// </summary>
        /// <param name="n">Number of movements to be added</param>
        public void Fo(int n)
        {
            switch (ScrambleStep.mod4(n))
            {
                case 1: AddStepAndShrink(Steps.Fo); break;
                case 2: AddStepAndShrink(Steps.Fo2); break;
                case 3: AddStepAndShrink(Steps.Fop); break;
            }
        }

        /// <summary>
        /// Add Bo movement to the scramble
        /// </summary>
        /// <param name="n">Number of movements to be added</param>
        public void Bo(int n)
        {
            switch (ScrambleStep.mod4(n))
            {
                case 1: AddStepAndShrink(Steps.Bo); break;
                case 2: AddStepAndShrink(Steps.Bo2); break;
                case 3: AddStepAndShrink(Steps.Bop); break;
            }
        }

        /// <summary>
        /// Add Ro movement to the scramble
        /// </summary>
        /// <param name="n">Number of movements to be added</param>
        public void Ro(int n)
        {
            switch (ScrambleStep.mod4(n))
            {
                case 1: AddStepAndShrink(Steps.Ro); break;
                case 2: AddStepAndShrink(Steps.Ro2); break;
                case 3: AddStepAndShrink(Steps.Rop); break;
            }
        }

        /// <summary>
        /// Add Lo movement to the scramble
        /// </summary>
        /// <param name="n">Number of movements to be added</param>
        public void Lo(int n)
        {
            switch (ScrambleStep.mod4(n))
            {
                case 1: AddStepAndShrink(Steps.Lo); break;
                case 2: AddStepAndShrink(Steps.Lo2); break;
                case 3: AddStepAndShrink(Steps.Lop); break;
            }
        }

        /// <summary>
        /// Add Us movement to the scramble
        /// </summary>
        /// <param name="n">Number of movements to be added</param>
        public void Us(int n)
        {
            switch (ScrambleStep.mod4(n))
            {
                case 1: AddStepAndShrink(Steps.Us); break;
                case 2: AddStepAndShrink(Steps.Us2); break;
                case 3: AddStepAndShrink(Steps.Usp); break;
            }
        }

        /// <summary>
        /// Add Ds movement to the scramble
        /// </summary>
        /// <param name="n">Number of movements to be added</param>
        public void Ds(int n)
        {
            switch (ScrambleStep.mod4(n))
            {
                case 1: AddStepAndShrink(Steps.Ds); break;
                case 2: AddStepAndShrink(Steps.Ds2); break;
                case 3: AddStepAndShrink(Steps.Dsp); break;
            }
        }

        /// <summary>
        /// Add Fs movement to the scramble
        /// </summary>
        /// <param name="n">Number of movements to be added</param>
        public void Fs(int n)
        {
            switch (ScrambleStep.mod4(n))
            {
                case 1: AddStepAndShrink(Steps.Fs); break;
                case 2: AddStepAndShrink(Steps.Fs2); break;
                case 3: AddStepAndShrink(Steps.Fsp); break;
            }
        }

        /// <summary>
        /// Add Bs movement to the scramble
        /// </summary>
        /// <param name="n">Number of movements to be added</param>
        public void Bs(int n)
        {
            switch (ScrambleStep.mod4(n))
            {
                case 1: AddStepAndShrink(Steps.Bs); break;
                case 2: AddStepAndShrink(Steps.Bs2); break;
                case 3: AddStepAndShrink(Steps.Bsp); break;
            }
        }

        /// <summary>
        /// Add Rs movement to the scramble
        /// </summary>
        /// <param name="n">Number of movements to be added</param>
        public void Rs(int n)
        {
            switch (ScrambleStep.mod4(n))
            {
                case 1: AddStepAndShrink(Steps.Rs); break;
                case 2: AddStepAndShrink(Steps.Rs2); break;
                case 3: AddStepAndShrink(Steps.Rsp); break;
            }
        }

        /// <summary>
        /// Add Ls movement to the scramble
        /// </summary>
        /// <param name="n">Number of movements to be added</param>
        public void Ls(int n)
        {
            switch (ScrambleStep.mod4(n))
            {
                case 1: AddStepAndShrink(Steps.Ls); break;
                case 2: AddStepAndShrink(Steps.Ls2); break;
                case 3: AddStepAndShrink(Steps.Lsp); break;
            }
        }

        /// <summary>
        /// Add Ua movement to the scramble
        /// </summary>
        /// <param name="n">Number of movements to be added</param>
        public void Ua(int n)
        {
            switch (ScrambleStep.mod4(n))
            {
                case 1: AddStepAndShrink(Steps.Ua); break;
                case 2: AddStepAndShrink(Steps.Ua2); break;
                case 3: AddStepAndShrink(Steps.Uap); break;
            }
        }

        /// <summary>
        /// Add Da movement to the scramble
        /// </summary>
        /// <param name="n">Number of movements to be added</param>
        public void Da(int n)
        {
            switch (ScrambleStep.mod4(n))
            {
                case 1: AddStepAndShrink(Steps.Da); break;
                case 2: AddStepAndShrink(Steps.Da2); break;
                case 3: AddStepAndShrink(Steps.Dap); break;
            }
        }

        /// <summary>
        /// Add Fa movement to the scramble
        /// </summary>
        /// <param name="n">Number of movements to be added</param>
        public void Fa(int n)
        {
            switch (ScrambleStep.mod4(n))
            {
                case 1: AddStepAndShrink(Steps.Fa); break;
                case 2: AddStepAndShrink(Steps.Fa2); break;
                case 3: AddStepAndShrink(Steps.Fap); break;
            }
        }

        /// <summary>
        /// Add Ba movement to the scramble
        /// </summary>
        /// <param name="n">Number of movements to be added</param>
        public void Ba(int n)
        {
            switch (ScrambleStep.mod4(n))
            {
                case 1: AddStepAndShrink(Steps.Ba); break;
                case 2: AddStepAndShrink(Steps.Ba2); break;
                case 3: AddStepAndShrink(Steps.Bap); break;
            }
        }

        /// <summary>
        /// Add Ra movement to the scramble
        /// </summary>
        /// <param name="n">Number of movements to be added</param>
        public void Ra(int n)
        {
            switch (ScrambleStep.mod4(n))
            {
                case 1: AddStepAndShrink(Steps.Ra); break;
                case 2: AddStepAndShrink(Steps.Ra2); break;
                case 3: AddStepAndShrink(Steps.Rap); break;
            }
        }

        /// <summary>
        /// Add La movement to the scramble
        /// </summary>
        /// <param name="n">Number of movements to be added</param>
        public void La(int n)
        {
            switch (ScrambleStep.mod4(n))
            {
                case 1: AddStepAndShrink(Steps.La); break;
                case 2: AddStepAndShrink(Steps.La2); break;
                case 3: AddStepAndShrink(Steps.Lap); break;
            }
        }

        #endregion

        #region Applying turns

        /// <summary>
        /// Apply x turn to a range of steps
        /// </summary>
        /// <param name="FirstStep">First step in the list to apply the turn</param>
        /// <param name="LastStep">Last step in the list to apply the turn</param>
        public void ApplyTurnx(int FirstStep, int LastStep)
        {
            if (FirstStep < 0) FirstStep = 0;
            if (LastStep >= Length) LastStep = Length - 1;
            for (int i = FirstStep; i <= LastStep; i++) StepsList[i] = ScrambleStep.x(StepsList[i]);
        }

        /// <summary>
        /// Apply x' turn to a range of steps
        /// </summary>
        /// <param name="FirstStep">First step in the list to apply the turn</param>
        /// <param name="LastStep">Last step in the list to apply the turn</param>
        public void ApplyTurnxp(int FirstStep, int LastStep)
        {
            if (FirstStep < 0) FirstStep = 0;
            if (LastStep >= Length) LastStep = Length - 1;
            for (int i = FirstStep; i <= LastStep; i++) StepsList[i] = ScrambleStep.xp(StepsList[i]);
        }

        /// <summary>
        /// Apply x2 turn to a range of steps
        /// </summary>
        /// <param name="FirstStep">First step in the list to apply the turn</param>
        /// <param name="LastStep">Last step in the list to apply the turn</param>
        public void ApplyTurnx2(int FirstStep, int LastStep)
        {
            if (FirstStep < 0) FirstStep = 0;
            if (LastStep >= Length) LastStep = Length - 1;
            for (int i = FirstStep; i <= LastStep; i++) StepsList[i] = ScrambleStep.x2(StepsList[i]);
        }

        /// <summary>
        /// Apply y turn to a range of steps
        /// </summary>
        /// <param name="FirstStep">First step in the list to apply the turn</param>
        /// <param name="LastStep">Last step in the list to apply the turn</param>
        public void ApplyTurny(int FirstStep, int LastStep)
        {
            if (FirstStep < 0) FirstStep = 0;
            if (LastStep >= Length) LastStep = Length - 1;
            for (int i = FirstStep; i <= LastStep; i++) StepsList[i] = ScrambleStep.y(StepsList[i]);
        }

        /// <summary>
        /// Apply y' turn to a range of steps
        /// </summary>
        /// <param name="FirstStep">First step in the list to apply the turn</param>
        /// <param name="LastStep">Last step in the list to apply the turn</param>
        public void ApplyTurnyp(int FirstStep, int LastStep)
        {
            if (FirstStep < 0) FirstStep = 0;
            if (LastStep >= Length) LastStep = Length - 1;
            for (int i = FirstStep; i <= LastStep; i++) StepsList[i] = ScrambleStep.yp(StepsList[i]);
        }

        /// <summary>
        /// Apply y2 turn to a range of steps
        /// </summary>
        /// <param name="FirstStep">First step in the list to apply the turn</param>
        /// <param name="LastStep">Last step in the list to apply the turn</param>
        public void ApplyTurny2(int FirstStep, int LastStep)
        {
            if (FirstStep < 0) FirstStep = 0;
            if (LastStep >= Length) LastStep = Length - 1;
            for (int i = FirstStep; i <= LastStep; i++) StepsList[i] = ScrambleStep.y2(StepsList[i]);
        }

        /// <summary>
        /// Apply z turn to a range of steps
        /// </summary>
        /// <param name="FirstStep">First step in the list to apply the turn</param>
        /// <param name="LastStep">Last step in the list to apply the turn</param>
        public void ApplyTurnz(int FirstStep, int LastStep)
        {
            if (FirstStep < 0) FirstStep = 0;
            if (LastStep >= Length) LastStep = Length - 1;
            for (int i = FirstStep; i <= LastStep; i++) StepsList[i] = ScrambleStep.z(StepsList[i]);
        }

        /// <summary>
        /// Apply z' turn to a range of steps
        /// </summary>
        /// <param name="FirstStep">First step in the list to apply the turn</param>
        /// <param name="LastStep">Last step in the list to apply the turn</param>
        public void ApplyTurnzp(int FirstStep, int LastStep)
        {
            if (FirstStep < 0) FirstStep = 0;
            if (LastStep >= Length) LastStep = Length - 1;
            for (int i = FirstStep; i <= LastStep; i++) StepsList[i] = ScrambleStep.zp(StepsList[i]);
        }

        /// <summary>
        /// Apply z2 turn to a range of steps
        /// </summary>
        /// <param name="FirstStep">First step in the list to apply the turn</param>
        /// <param name="LastStep">Last step in the list to apply the turn</param>
        public void ApplyTurnz2(int FirstStep, int LastStep)
        {
            if (FirstStep < 0) FirstStep = 0;
            if (LastStep >= Length) LastStep = Length - 1;
            for (int i = FirstStep; i <= LastStep; i++) StepsList[i] = ScrambleStep.z2(StepsList[i]);
        }

        #endregion Applying turns

        #region Reading scramble from string

        /// <summary>
        /// Parse a scramble from a string (parentheses are developed)
        /// </summary>
        /// <param name="SS">Scramble string</param>
        public void ParseScramble(string SS)
        {
            Reset(); // Clear scramble

            string S = RemoveNoScrambleCharacters(SS);
            if (string.IsNullOrEmpty(S)) return;

            for (int i = 0; i < S.Length; i++)
            {
                // Looking for parentheses
                if (S[i] == AMSettings.OpenParenthesisChar) // '('
                {
                    int j, nestlevel = 0;
                    for (j = i + 1; j < S.Length; j++)
                    {
                        if (S[j] == AMSettings.OpenParenthesisChar) { nestlevel++; continue; }
                        if (S[j] == AMSettings.CloseParenthesisChar && nestlevel == 0) break;
                        else if (S[j] == AMSettings.CloseParenthesisChar) nestlevel--;
                    }

                    if (j >= S.Length - 1) continue; // Nothing to do, no end parenthesis or no number after end parenthesis

                    if (j - i - 1 <= 0) continue; // Nothing between parentheses

                    // At this moment S[i] = '(' and S[j] = ')'

                    // Repetitions is the number >=2 and <=9 adjacent to end parenthesis
                    int Repetitions = (int)char.GetNumericValue(S[j + 1]);
                    if (Repetitions < 2) continue; // No numeric value adjacent to end parenthesis or one repetition

                    string AuxString = S.Substring(i + 1, j - i - 1); // Get sub-scramble string between parentheses

                    if (AuxString.Length <= 0) continue;

                    // Add parentheses scramble (recursive)
                    Scramble AuxScramble = new Scramble();
                    AuxScramble.ParseScramble(AuxString);
                    for (int r = 0; r < Repetitions; r++)
                        for (int k = 0; k < AuxScramble.Length; k++)
                            AddStepWithoutShrink(AuxScramble[k]);

                    i = j + 1; // Parentheses core processed, skip parentheses
                }

                else if (S[i] == AMSettings.CloseParenthesisChar) continue; // ')' Skip close parenthesis (without open parenthesis or open parenthesis skipped)

                else // Looking for steps
                {
                    AddStepWithoutShrink(ScrambleStep.StepFromText(ScrambleStep.GetCleanStepString(S, i, out int Next)));
                    if (Next > i) i = Next - 1;
                }
            }
        }

        /// <summary>
        /// Reads a scramble from a string (as is)
        /// </summary>
        /// <param name="SS">Scramble string</param>
        public void ReadScramble(string SS)
        {
            Reset(); // Clear scramble

            string S = RemoveNoScrambleCharacters(SS);
            if (string.IsNullOrEmpty(S)) return;

            for (int i = 0; i < S.Length; i++)
            {
                AddStepWithoutShrink(ScrambleStep.StepFromText(ScrambleStep.GetCleanStepString(S, i, out int Next)));
                if (Next > i) i = Next - 1;
            }
        }

        #endregion

        #region Scramble to text

        /// <summary>
        /// Gets the text associated to a scramble
        /// </summary>
        /// <param name="Separator">Separator between steps</param>
        /// <returns>Scramble in string format</returns>
        public string GetText(string Separator)
        {
            if (Length <= 0) return Separator;

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.Append(ScrambleStep.GetText(StepsList[0]));

            for (int i = 1; i < Length; i++)
            {
                if ((!ScrambleStep.IsOpenParenthesis(StepsList[i - 1]) || ScrambleStep.IsCloseParenthesis(StepsList[i])) && !string.IsNullOrEmpty(Separator))
                    sb.Append(Separator);

                sb.Append(ScrambleStep.GetText(StepsList[i]));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets the text associated to a scramble
        /// </summary>
        /// <returns>Scramble with steps separated with spaces</returns>
        public override string ToString() => GetText(" ");

        #endregion

        #region Modifying a scramble

        /// <summary>
        /// Add scramble to current scramble
        /// </summary>
        /// <param name="SD">Scramble source to add</param>
        public void Add(Scramble SD)
        {
            if (SD != null) for (int n = 0; n < SD.Length; n++) AddStepWithoutShrink(SD[n]);
        }

        /// <summary>
        /// Copy scramble from given scramble
        /// </summary>
        /// <param name="SD">Scramble source to copy</param>
        public void Copy(Scramble SD)
        {
            Reset();
            if (SD != null) for (int n = 0; n < SD.Length; n++) AddStepWithoutShrink(SD[n]);
        }

        /// <summary>
        /// Change the scramble steps order
        /// </summary>
        /// <returns>True if scramble has been reversed</returns>
        public bool Reverse()
        {
            if (!AreParenthesesOK) if (!CompleteParentheses()) return false;

            if (Length <= 1) return true;

            Steps AuxStep;

            for (int n = 0; n < Length / 2; n++)
            {
                AuxStep = StepsList[n];
                StepsList[n] = StepsList[Length - 1 - n];
                StepsList[Length - 1 - n] = AuxStep;
            }

            // Check if there are parentheses and move them to the right position in the scramble
            int nest, j;

            bool[] ParControl = new bool[Length]; // all false by default

            for (int i = 0; i < Length - 1; i++)
            {
                if (ScrambleStep.IsCloseParenthesis(StepsList[i]) && !ParControl[i])
                {
                    nest = 1;
                    for (j = i + 1; j < Length; j++)
                    {
                        if (ScrambleStep.IsCloseParenthesis(StepsList[j])) nest++;
                        else if (StepsList[j] == Steps.OPEN_PARENTHESIS)
                        {
                            nest--;
                            if (nest == 0) break;
                        }
                    }
                    AuxStep = StepsList[i];
                    StepsList[i] = StepsList[j];
                    StepsList[j] = AuxStep;
                    ParControl[i] = ParControl[j] = true;
                } 
            }
            return true;
        }

        /// <summary>
        /// Invert the scramble steps (U/U', R'/R, ...)
        /// </summary>
        /// <param name="FirstStep">First step to invert</param>
        /// <param name="LastStep">Last step to invert</param>
        public void Inverse(int FirstStep, int LastStep)
        {
            if (FirstStep < 0) FirstStep = 0;
            if (LastStep >= Length) LastStep = Length - 1;
            for (int n = FirstStep; n <= LastStep; n++) ReplaceStepInPosition(ScrambleStep.Inverse(StepsList[n]), n);
        }

        /// <summary>
        /// Change the scramble steps order and invert
        /// </summary>
        /// <returns>False if scramble includes parentheses</returns>
        public bool InverseAndReverseScramble()
        {
            if (!Reverse()) return false;
            Inverse(0, Length - 1);
            return true;
        }

        /// <summary>
        /// Changes every step in the list for his more simple form
        /// </summary>
        /// <param name="FirstStep">First step to simplify</param>
        /// <param name="LastStep">Last step to simplify</param>
        public void Simplify(int FirstStep, int LastStep)
        {
            if (FirstStep < 0) FirstStep = 0;
            if (LastStep >= Length) LastStep = Length - 1;
            for (int n = FirstStep; n <= LastStep; n++) StepsList[n] = ScrambleStep.Simple(StepsList[n]);
        }

        /// <summary>
        /// Changes every step in the list for his more simple form
        /// </summary>
        public void Simplify()
        {
            for (int n = 0; n <= Length - 1; n++) StepsList[n] = ScrambleStep.Simple(StepsList[n]);
        }

        /// <summary>
        /// Remove x, y and z turns from scramble (maintaining the scramble functionality)
        /// </summary>
        public bool RemoveTurns()
        {
            if (!HasTurns) return false;

            if (HasParenthesesWithRepetitions) Copy(DevelopParentheses(true));

            for (int n = Length - 1; n >= 0; n--)
            {
                if (ScrambleStep.IsTurn(StepsList[n]))
                {
                    switch (StepsList[n])
                    {
                        case Steps.x:
                            for (int i = n + 1; i < Length; i++) StepsList[i] = ScrambleStep.xp(StepsList[i]);
                            break;
                        case Steps.xp:
                            for (int i = n + 1; i < Length; i++) StepsList[i] = ScrambleStep.x(StepsList[i]);
                            break;
                        case Steps.x2:
                        case Steps.x2p:
                            for (int i = n + 1; i < Length; i++) StepsList[i] = ScrambleStep.x2(StepsList[i]);
                            break;

                        case Steps.y:
                            for (int i = n + 1; i < Length; i++) StepsList[i] = ScrambleStep.yp(StepsList[i]);
                            break;
                        case Steps.yp:
                            for (int i = n + 1; i < Length; i++) StepsList[i] = ScrambleStep.y(StepsList[i]);
                            break;
                        case Steps.y2:
                        case Steps.y2p:
                            for (int i = n + 1; i < Length; i++) StepsList[i] = ScrambleStep.y2(StepsList[i]);
                            break;

                        case Steps.z:
                            for (int i = n + 1; i < Length; i++) StepsList[i] = ScrambleStep.zp(StepsList[i]);
                            break;
                        case Steps.zp:
                            for (int i = n + 1; i < Length; i++) StepsList[i] = ScrambleStep.z(StepsList[i]);
                            break;
                        case Steps.z2:
                        case Steps.z2p:
                            for (int i = n + 1; i < Length; i++) StepsList[i] = ScrambleStep.z2(StepsList[i]);
                            break;
                    }
                    DeleteStep(n);
                }
            }
            return true;
        }

        /// <summary>
        /// Changes compound steps to basic steps
        /// </summary>
        public void ToBasicSteps()
        {
            int StepsAdded;
            for (int i = 0; i < Length; i++)
            {
                StepsAdded = AddStepSequenceInPosition(ScrambleStep.GetEquivalentStepSequence(StepsList[i]), i);
                DeleteStep(i + StepsAdded);
                if (StepsAdded > 0) i += StepsAdded - 1;
            }
        }

        /// <summary>
        /// Shrink the scramble steps
        /// </summary>
        /// <returns>Returns false if the scramble is shrinked. If returns true apply shrink again.</returns>
        public bool Shrink()
        {
            if (Length <= 1) return false;

            bool ChangesDone = false;

            Movements M1, M2;

            for (int i = Length - 1; i >= 0; i--)
                if (StepsList[i] == Steps.NONE)
                {
                    DeleteStep(i);
                    ChangesDone = true;
                }

            for (int i = 1; i < Length; i++) // Shrink adjacent steps
            {
                if (ScrambleStep.Layer(StepsList[i]) == ScrambleStep.Layer(StepsList[i - 1]) && 
                         ScrambleStep.Modifier(StepsList[i]) == ScrambleStep.Modifier(StepsList[i - 1]) && 
                         !ScrambleStep.IsParenthesis(StepsList[i]))
                {
                    M1 = ScrambleStep.Movement(StepsList[i - 1]);
                    M2 = ScrambleStep.Movement(StepsList[i]);

                    StepsList[i - 1] = ScrambleStep.SetMovementToStep(StepsList[i - 1], ScrambleStep.JoinMovements(M1, M2));
                    DeleteStep(i);
                    if (StepsList[i - 1] == Steps.NONE) DeleteStep(i - 1);
                    ChangesDone = true;
                }
            }         

            if (ChangesDone) return true;

            for (int i = 2; i < Length; i++) // Shrink alternate steps
            {
                if (ScrambleStep.Layer(StepsList[i]) == ScrambleStep.Layer(StepsList[i - 2]) && 
                    ScrambleStep.OppositeLayer(StepsList[i]) == ScrambleStep.Layer(StepsList[i - 1]) &&
                    ScrambleStep.Modifier(StepsList[i]) == Modifiers.SINGLE_LAYER &&
                    ScrambleStep.Modifier(StepsList[i - 1]) == Modifiers.SINGLE_LAYER &&
                    ScrambleStep.Modifier(StepsList[i - 2]) == Modifiers.SINGLE_LAYER)
                {
                    M1 = ScrambleStep.Movement(StepsList[i - 2]);
                    M2 = ScrambleStep.Movement(StepsList[i]);

                    StepsList[i - 2] = ScrambleStep.SetMovementToStep(StepsList[i - 2], ScrambleStep.JoinMovements(M1, M2));
                    DeleteStep(i);
                    if (StepsList[i - 2] == Steps.NONE) DeleteStep(i - 2);
                    ChangesDone = true;
                }
            }

            return ChangesDone;
        }             

        /// <summary>
        /// Delete a step in the scramble
        /// </summary>
        /// <param name="i">Position of the step to be deleted</param>
        public void DeleteStep(int i)
        {
            if (i >= 0 && i < Length) StepsList.RemoveAt(i);
        }

        /// <summary>
        /// Deletes the scramble last step
        /// </summary>
        public void DeleteLastStep()
        {
            if (Length > 0) StepsList.RemoveAt(Length - 1);
        }

        /// <summary>
        /// Add a step to the end of the scramble and shrink the scramble (if necessary)
        /// </summary>
        /// <param name="S">Step to be added</param>
        public void AddStepAndShrink(Steps S)
        {
            StepsList.Add(S);
            while (Shrink()) ;
        }

        /// <summary>
        /// Add a step to the end of the scramble without shrinking the scramble
        /// </summary>
        /// <param name="S">Step to be added</param>
        public void AddStepWithoutShrink(Steps S)
        {
            StepsList.Add(S);
        }

        /// <summary>
        /// Add step to the scramble in the given position (no shrinking)
        /// </summary>
        /// <param name="S">Step to be added</param>
        /// <param name="Pos">Position where the step will be added</param>
        /// <returns>True if the step has been added</returns>
        public bool AddStepInPosition(Steps S, int Pos)
        {
            if (Pos >= 0 && Pos <= Length)
            {
                StepsList.Insert(Pos, S);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Add scramble steps in a position (no shrinking)
        /// </summary>
        /// <param name="Pos">Position where introduce steps</param>
        /// <param name="Steps">Steps to introduce</param>
        /// <returns>Number of steps introduced</returns>
        public int AddStepsInPosition(int Pos, params Steps[] Stps)
        {
            if (Stps == null) return 0;
            int NumSteps = 0;
            for (int n = Stps.Length - 1; n >= 0; n--)
                if (AddStepInPosition(Stps[n], Pos)) NumSteps++;
            return NumSteps;
        }

        /// <summary>
        /// Add sequence of three steps to the scramble in the given position (no shrinking)
        /// </summary>
        /// <param name="SQ">Steps sequence</param>
        /// <param name="Pos">Position where the steps will be added</param>
        /// <returns>Returns the number of steps added</returns>
        public int AddStepSequenceInPosition(Steps[] SQ, int Pos)
        {
            if (SQ == null || SQ.Length != 3) return 0; // Array must have a length of three elements

            int NumSteps = 0;
            for (int n = 0; n < 3; n++) if (SQ[n] != Steps.NONE) NumSteps++;
            if (NumSteps == 0) return 0;

            if (Pos >= 0 && Pos < Length)
            {
                if (SQ[2] != Steps.NONE) AddStepInPosition(SQ[2], Pos);
                if (SQ[1] != Steps.NONE) AddStepInPosition(SQ[1], Pos);
                if (SQ[0] != Steps.NONE) AddStepInPosition(SQ[0], Pos);
                return NumSteps;
            }
            return 0;
        }

        /// <summary>
        /// Replace step to the scramble in the given position (no shrinking)
        /// </summary>
        /// <param name="S">Step to be added</param>
        /// <param name="Pos">Position where the step will be replaced</param>
        /// <returns>True if the step has been replaced</returns>
        public bool ReplaceStepInPosition(Steps S, int Pos)
        {
            if (Pos >= 0 && Pos <= Length)
            {
                StepsList.RemoveAt(Pos);
                StepsList.Insert(Pos, S);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Reset the scramble (no steps)
        /// </summary>
        public void Reset()
        {
            if (StepsList == null) StepsList = new List<Steps>();
            else StepsList.Clear();
        }

        /// <summary>
        /// Complete scramble parentheses if possible
        /// </summary>
        /// <returns>True if parentheses are OK</returns>
        public bool CompleteParentheses()
        {
            while (NestValue > 0) // More open than close parentheses, add parentheses to complete
                AddStepWithoutShrink(Steps.CLOSE_PARENTHESIS_1_REP);

            return AreParenthesesOK;
        }

        #endregion

        #region Other functions

        /// <summary>
        /// Gets a scramble with simple or without parentheses (developed steps)
        /// </summary>
        /// <returns>Same scramble with simple parentheses or no parentheses</returns>
        /// <param name="AllowSimpleParentheses">Don't remove simple parentheses</param>
        public Scramble DevelopParentheses(bool AllowSimpleParentheses)
        {
            if (!CompleteParentheses()) return null;

            Scramble sdp = new Scramble();

            if (!AllowSimpleParentheses && !HasParentheses || 
                AllowSimpleParentheses && !HasParenthesesWithRepetitions) sdp.Copy(this);
            else
            {
                for (int i = 0; i < Length; i++)
                {
                    if (StepsList[i] == Steps.OPEN_PARENTHESIS)
                    {
                        int j, nestlevel = 0;
                        for (j = i + 1; j < Length; j++)
                        {
                            if (StepsList[j] == Steps.OPEN_PARENTHESIS) { nestlevel++; continue; }
                            if (ScrambleStep.IsCloseParenthesis(StepsList[j]) && nestlevel == 0) break;
                            else if (ScrambleStep.IsCloseParenthesis(StepsList[j])) nestlevel--;
                        }

                        if (j >= Length) continue; // Nothing to do, no end parenthesis

                        if (j - i - 1 <= 0) continue; // Nothing between parentheses

                        // At this moment Steps[i] = '(' and Steps[j] = ')'

                        int Repetitions = ScrambleStep.GetCloseParenthesisRepetions(StepsList[j]);
                        if (Repetitions < 2) continue; // One repetition

                        Scramble AuxScramble = SubScramble(i + 1, j - i - 1); // Get sub-scramble between parentheses

                        if (AuxScramble.Length <= 0) continue;

                        // Add parentheses scramble
                        for (int r = 0; r < Repetitions; r++)
                        {
                            if (AllowSimpleParentheses)
                                sdp.AddStepWithoutShrink(Steps.OPEN_PARENTHESIS);
                            for (int k = 0; k < AuxScramble.Length; k++)
                                sdp.AddStepWithoutShrink(AuxScramble[k]);
                            if (AllowSimpleParentheses)
                                sdp.AddStepWithoutShrink(Steps.CLOSE_PARENTHESIS_1_REP);
                        }
                        i = j; // Parentheses core processed, skip parentheses

                        if (!AllowSimpleParentheses && sdp.HasParentheses ||
                            AllowSimpleParentheses && sdp.HasParenthesesWithRepetitions)
                            sdp = sdp.DevelopParentheses(AllowSimpleParentheses); // Recursivity
                    }
                    else if (ScrambleStep.IsCloseParenthesis(StepsList[i])) continue; // ')' Skip close parenthesis (without open parenthesis or open parenthesis skipped)

                    else sdp.AddStepWithoutShrink(StepsList[i]);
                }
            }
            return sdp;
        }

        /// <summary>
        /// Creates a new scramble, part of current scramble
        /// </summary>
        /// <param name="pos">Initial subscramble position</param>
        /// <param name="length">Length of subscramble</param>
        /// <returns>Subscramble</returns>
        public Scramble SubScramble(int pos, int slength)
        {
            Scramble SS = new Scramble();

            if (pos < 0 || pos >= Length || slength <= 0) return SS;

            int n = pos;
            while (n < pos + slength && n < Length)
            {
                SS.AddStepWithoutShrink(StepsList[n]);
                n++;
            }
            return SS;
        }

        #endregion Other functions

        #region Static functions

        /// <summary>
        /// Remove no scramble characters from a string
        /// </summary>
        /// <param name="S">String to process</param>
        /// <returns>String without non scramble characters</returns>
        public static string RemoveNoScrambleCharacters(string S)
        {
            if (string.IsNullOrEmpty(S)) return string.Empty;

            char[] CharsScrambleArray = AMSettings.GetAllCharsArray();

            System.Text.StringBuilder sb = new System.Text.StringBuilder(S.Length);

            foreach (char sc in S)
                foreach (char ac in CharsScrambleArray)
                    if (sc == ac)
                    {
                        sb.Append(sc);
                        break;
                    }

            return sb.ToString();
        }

        #endregion Static functions
    }
}
