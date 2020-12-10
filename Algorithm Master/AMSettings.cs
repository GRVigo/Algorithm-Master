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
using System.Windows.Media;
using System.Windows.Controls;
using System.Xml;

namespace Algorithm_Master
{
    #region Structs to group settings

    /// <summary>
    /// General options for application
    /// </summary>
    public struct GeneralAppSettings
    {
        /// <summary>
        /// True if it's a portable application installation
        /// </summary>
        public bool Portable;

        /// <summary>
        /// Save messages in a log file
        /// </summary>
        public bool SaveLog;

        /// <summary>
        /// Default language file for application
        /// </summary>
        public string DefaultLanguageFile;

        /// <summary>
        /// Application colors
        /// </summary>
        public Color U, D, F, B, R, L, Neutral, Base, Background, Arrows;

        /// <summary>
        /// Aplication brushes
        /// </summary>
        public Brush UBrush, DBrush, FBrush, BBrush, RBrush, LBrush,
                     NeutralBrush, BaseBrush, BackgroundBrush, ArrowsBrush;

        /// <summary>
        /// 3D Camera settings
        /// </summary>
        public double Distance, Alpha, Beta;

        /// <summary>
        /// Video quality (%)
        /// </summary>
        public int VideoQuality;

        /// <summary>
        /// Video frame rate (for camera and video file)
        /// </summary>
        public float VideoFrameRate;
    }

    /// <summary>
    /// Settings for scramble notation
    /// </summary>
    public struct NotationSettings
    {
        /// <summary>
        /// Chars to define a layer
        /// </summary>
        public char[] LayerChars; // 'U', 'D', 'F', 'B', 'R', 'L', 'E', 'S', 'M'

        /// <summary>
        /// Chars to define a movement
        /// </summary>
        public char[] MovementsChars;  // '2', '\''

        /// <summary>
        /// Chars to define a modifier
        /// </summary>
        public char[] ModifiersChars; // 'w', 's', 'a', 'o'

        /// <summary>
        /// Chars to define a turn (full cube rotation)
        /// </summary>
        public char[] RotationsChars; // 'x', 'y' , 'z'

        /// <summary>
        /// Chars to define alternative layers
        /// </summary>
        public char[] AlternativeWChars; // 'u' for 'Uw' ...

        /// <summary>
        /// Other chars to define end of step
        /// </summary>
        public char[] EndStepChars; // '(', ')' and numbers from '3' to '9'

        /// <summary>
        /// If true, *w movements are represented by alternative chars (u for Uw, ...)
        /// </summary>
        public bool UseAltwChars;

        /// <summary>
        /// Rotation direction of E layer
        /// </summary>
        public bool ERotationAsU;

        /// <summary>
        /// Rotation direction of S layer
        /// </summary>
        public bool SRotationAsF;

        /// <summary>
        /// Rotation direction of M layer
        /// </summary>
        public bool MRotationAsR;
    }

    /// <summary>
    /// Settings for chronometer
    /// </summary>
    public struct ChronoSettings
    {
        /// <summary>
        /// Time for a cube movement animation
        /// </summary>
        public int AnimationTime;

        /// <summary>
        /// Beep sound enabled during inspection
        /// </summary>
        public bool BeepEnabled;

        /// <summary>
        /// Default cube
        /// </summary>
        public string DefaultCube;

        /// <summary>
        /// Default comment
        /// </summary>
        public string DefaultComment;

        /// <summary>
        /// When importing scramble to chrono, convert to basic steps
        /// </summary>
        public bool OptionToBasic;

        /// <summary>
        /// When importing scramble to chrono, remove turns
        /// </summary>
        public bool OptionRemoveTurns;

        /// <summary>
        /// When importing scramble to chrono, shrink scramble
        /// </summary>
        public bool OptionShrink;

        /// <summary>
        /// Save chrono times (if false, times are lost)
        /// </summary>
        public bool OptionSaveTimes;

        /// <summary>
        /// Load current day times when program starts
        /// </summary>
        public bool OptionLoadAtStart;
    }

    /// <summary>
    /// Settings for progress
    /// </summary>
    public struct ProgressSettings
    {
        /// <summary>
        /// Minimum solves in a period to be represented in the chart
        /// </summary>
        public int MinimumSolvesPeriod;

        /// <summary>
        /// Chart colors
        /// </summary>
        public Color ColorBackground, ColorMaximum, ColorMinimum, ColorAverage, ColorMedium, ColorDeviation, ColorAmount;
    }

    /// <summary>
    /// Settings for editor
    /// </summary>
    public struct EditorSettings
    {
        /// <summary>
        /// Allow extended compount movements (Uo, Ra, Ls, ...)
        /// </summary>
        public bool OptionAllowExtendedCompoundMovements;

        /// <summary>
        /// Time for a cube movement animation
        /// </summary>
        public int AnimationTime;
    }

    /// <summary>
    /// Settings for library
    /// </summary>
    public struct LibrarySettings
    {
        /// <summary>
        /// If true, when in the library a change in a view is made, check the algorithm unsaved flag
        /// </summary>
        public bool OptionViewChanges;

        /// <summary>
        /// Develop parentheses when creating initial pos. from algorithm 
        /// </summary>
        public bool OptionDevelopParentheses;

        /// <summary>
        /// Conver to to basic steps when creating initial pos. from algorithm 
        /// </summary>
        public bool OptionBasicSteps;

        /// <summary>
        /// Remove turns when creating initial pos. from algorithm 
        /// </summary>
        public bool OptionRemoveTurns;

        /// <summary>
        /// Shrink steps when creating initial pos. from algorithm 
        /// </summary>
        public bool OptionShrink;

        /// <summary>
        /// Inverse & reverse steps when creating initial pos. from algorithm 
        /// </summary>
        public bool OptionInverse;

        /// <summary>
        /// Insert steps at beginning when creating initial pos. from algorithm 
        /// </summary>
        public bool OptionInsertBeginning;

        /// <summary>
        /// Steps to insert when creating initial pos. from algorithm 
        /// </summary>
        public string OptionStepsToInsert;

        /// <summary>
        /// If true, algorithm default position is initial position. Otherwise is last position
        /// </summary>
        public bool OptionDefaultPositionStart;

        /// <summary>
        /// If true show parentheses in algorithm buttons
        /// </summary>
        public bool OptionShowParentheses;
    }

    #endregion Structs to group settings

    /// <summary>
    /// Static class to manage application settings
    /// </summary>
    public static class AMSettings
    {
        #region Fields

        /// <summary>
        /// General appication settings
        /// </summary>
        private static GeneralAppSettings AppSettings;

        /// <summary>
        /// Notation settings
        /// </summary>
        private static NotationSettings NotationSettings;

        /// <summary>
        /// Chronometer settings
        /// </summary>
        private static ChronoSettings ChronoSettings;

        /// <summary>
        /// Progress settings
        /// </summary>
        private static ProgressSettings ProgressSettings;

        /// <summary>
        /// Editor settings
        /// </summary>
        private static EditorSettings EditorSettings;

        /// <summary>
        /// Library settings
        /// </summary>
        private static LibrarySettings LibrarySettings;

        #endregion Fields

        #region General application properties

        /// <summary>
        /// Set the TextBlock to show status messages
        /// </summary>
        public static TextBlock SetStatus { set; get; }

        /// <summary>
        /// Write status message
        /// </summary>
        public static string Status
        {
            set
            {
                if (SetStatus != null && value != null) SetStatus.Text = value;
                Log = value;
            }
        }

        /// <summary>
        /// Write log message
        /// </summary>
        public static string Log
        {
            set
            {
                if (LogEnabled && value != null)
                    try
                    {
                        System.IO.File.AppendAllText(LogFile,
                            "[" + DateTime.Now.ToLongTimeString() + "] " + value + Environment.NewLine);
                    }
                    catch { }
            }
        }

        /// <summary>
        /// Portable application flag
        /// </summary>
        public static bool PortableApp
        {
            get { return AppSettings.Portable; }
            set { AppSettings.Portable = value; }
        }

        /// <summary>
        /// If true, messages are saved in an log file
        /// </summary>
        public static bool LogEnabled
        {
            get { return AppSettings.SaveLog; }
            set { AppSettings.SaveLog = value; }
        }

        /// <summary>
        /// Frame rate (fps) for solve videos
        /// </summary>
        public static float VideoFrameRate
        {
            get { return AppSettings.VideoFrameRate; }
            set
            {
                if (value < 1f) AppSettings.VideoFrameRate = 1f;
                else if (value > 60f) AppSettings.VideoFrameRate = 60f;
                else AppSettings.VideoFrameRate = value;
            }
        }

        /// <summary>
        /// Video quality (%) for solve videos
        /// </summary>
        public static int VideoQuality
        {
            get { return AppSettings.VideoQuality; }
            set
            {
                if (value < 10) AppSettings.VideoQuality = 10; // 10%
                else if (value > 100) AppSettings.VideoQuality = 100; // 100%
                else AppSettings.VideoQuality = value;
            }
        }

        /// <summary>
        /// Application system folder
        /// </summary>
        public static string AppFolder => AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>
        /// Data folder to store user's program data
        /// </summary>
        public static string DataFolder => AppSettings.Portable ? System.IO.Path.Combine(AppFolder, "AppData") :
                    System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                                           "Algorithm Master");

        /// <summary>
        /// Folder for language files
        /// </summary>
        public static string LangFolder => System.IO.Path.Combine(DataFolder, "lan");

        /// <summary>
        /// Folder for solves files
        /// </summary>
        public static string SolvesFolder => System.IO.Path.Combine(DataFolder, "solves");

        /// <summary>
        /// Full path for LiteDB solves data base
        /// </summary>
        public static string SolvesDBPath => System.IO.Path.Combine(SolvesFolder, "Solves.LiteDB");

        /// <summary>
        /// Full path for backup 1 LiteDB solves data base
        /// </summary>
        public static string SolvesDBBack1 => System.IO.Path.Combine(SolvesFolder, "Solves.LiteDB.1.bak");

        /// <summary>
        /// Full path for backup 2 LiteDB solves data base
        /// </summary>
        public static string SolvesDBBack2 => System.IO.Path.Combine(SolvesFolder, "Solves.LiteDB.2.bak");

        /// <summary>
        /// Full path for backup 3 LiteDB solves data base
        /// </summary>
        public static string SolvesDBBack3 => System.IO.Path.Combine(SolvesFolder, "Solves.LiteDB.3.bak");

        /// <summary>
        /// Folder for library files
        /// </summary>
        public static string LibsFolder => System.IO.Path.Combine(DataFolder, "libs");

        /// <summary>
        /// Folder for settings files
        /// </summary>
        public static string SettingsFolder => DataFolder;

        /// <summary>
        /// Documents folder
        /// </summary>
        public static string DocsFolder => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        /// <summary>
        /// Temporary folder
        /// </summary>
        public static string TempFolder => System.IO.Path.Combine(DataFolder, "temp");

        /// <summary>
        /// Temporary video folder
        /// </summary>
        public static string VideoTempFolder => TempFolder;

        /// <summary>
        /// Video folder
        /// </summary>
        public static string VideoFolder => Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);

        /// <summary>
        /// Searche folder
        /// </summary>
        public static string SearchsFolder => System.IO.Path.Combine(DataFolder, "searches");

        /// <summary>
        /// File extension for language files (includes point)
        /// </summary>
        public static string LangExt => ".lan";

        /// <summary>
        /// Extension for settings files (includes point)
        /// </summary>
        public static string SettingsExt => ".smxml";

        /// <summary>
        /// Extension for backup files (includes point)
        /// </summary>
        public static string BackUpExt => ".bak";

        /// <summary>
        /// Extension for library files (includes point)
        /// </summary>
        public static string LibsExt => ".sml";

        /// <summary>
        /// Extension for CSV files (includes point)
        /// </summary>
        public static string CSVExt => ".csv";

        /// <summary>
        /// Extension for search files (includes point)
        /// </summary>
        public static string SearchExt => ".sch";

        /// <summary>
        /// System path separator
        /// </summary>
        public static string PathSeparator => System.IO.Path.DirectorySeparatorChar.ToString();

        /// <summary>
        /// CSV separator 
        /// </summary>
        public static string CSVSeparator => System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator;

        /// <summary>
        /// Log file
        /// </summary>
        public static string LogFile => System.IO.Path.Combine(DataFolder, "Algorithm Master.log");

        /// <summary>
        /// Temporray video file
        /// </summary>
        public static string TempVideoFile => System.IO.Path.Combine(VideoTempFolder, "__temp_solve_video__.avi");

        /// <summary>
        /// Folder for readme files (txt)
        /// </summary>
        public static string ReadmeFolder => AppFolder;

        /// <summary>
        /// Current language settings file name
        /// </summary>
        public static string CurrentLanguageFile
        {
            get { return AppSettings.DefaultLanguageFile; }
            set
            {
                if (string.IsNullOrWhiteSpace(value) || !System.IO.File.Exists(System.IO.Path.Combine(LangFolder, value)))
                    AppSettings.DefaultLanguageFile = "default" + LangExt;
                else
                    AppSettings.DefaultLanguageFile = value;
            }
        }

        /// <summary>
        /// Default settings file full path
        /// </summary>
        public static string DefaultSettingsFile => System.IO.Path.Combine(SettingsFolder, "default" + SettingsExt);

        /// <summary>
        /// Factory settings file full path
        /// </summary>
        public static string FactorySettingsFile => System.IO.Path.Combine(SettingsFolder, "factory" + SettingsExt);

        /// <summary>
        /// Beep wav file path for ready message
        /// </summary>
        public static string BeepReadyFile => System.IO.Path.Combine(AppFolder, "AppData", "BeepShort.wav");

        /// <summary>
        /// Beep wav file path for start message
        /// </summary>
        public static string BeepStartFile => System.IO.Path.Combine(AppFolder, "AppData", "BeepLong.wav");

        /// <summary>
        /// Beep wav file path for 8 seconds message
        /// </summary>
        public static string Beep8sFile => System.IO.Path.Combine(AppFolder, "AppData", "BeepShort.wav");

        /// <summary>
        /// Beep wav file path for 12 seconds message
        /// </summary>
        public static string Beep12sFile => System.IO.Path.Combine(AppFolder, "AppData", "Beep2.wav");

        /// <summary>
        /// Beep wav file path for 15 seconds message
        /// </summary>
        public static string Beep15sFile => System.IO.Path.Combine(AppFolder, "AppData", "Beep3.wav");

        /// <summary>
        /// Beep wav file path for DNS message
        /// </summary>
        public static string BeepDNFFile => System.IO.Path.Combine(AppFolder, "AppData", "BeepDNF.wav");

        /// <summary>
        /// Beep wav file path for end message
        /// </summary>
        public static string BeepEndFile => System.IO.Path.Combine(AppFolder, "AppData", "BeepLong.wav");

        /// <summary>
        /// Camera distance for 3D cube views (3 ≤ distance ≤ 100)
        /// </summary>
        public static double CameraDistance
        {
            get { return AppSettings.Distance; }
            set
            {
                if (value < 3.0) AppSettings.Distance = 3.0;
                else if (value > 100.0) AppSettings.Distance = 100.0;
                else AppSettings.Distance = value;
            }
        }

        /// <summary>
        /// Camera alpha angle for 3D cube views (start value) (-90 ≤ angle ≤ 270)
        /// </summary>
        public static double CameraAlpha
        {
            get { return AppSettings.Alpha; }
            set
            {
                if (value < -90.0) AppSettings.Alpha = -90.0;
                else if (value > 270.0) AppSettings.Alpha = 270.0;
                else AppSettings.Alpha = value;
            }
        }

        /// <summary>
        /// Camera beta angle for 3D cube views (start value) (-90 ≤ angle ≤ 90)
        /// </summary>
        public static double CameraBeta
        {
            get { return AppSettings.Beta; }
            set
            {
                if (value < -90.0) AppSettings.Beta = -90.0;
                else if (value > 90.0) AppSettings.Beta = 90.0;
                else AppSettings.Beta = value;
            }
        }

        /// <summary>
        /// Color for U face stickers
        /// </summary>
        public static Color UColor
        {
            get { return AppSettings.U; }
            set { AppSettings.U = value; AppSettings.UBrush = GetBrush(value); }
        }

        /// <summary>
        /// Color for D face stickers
        /// </summary>
        public static Color DColor
        {
            get { return AppSettings.D; }
            set { AppSettings.D = value; AppSettings.DBrush = GetBrush(value); }
        }

        /// <summary>
        /// Color for F face stickers
        /// </summary>
        public static Color FColor
        {
            get { return AppSettings.F; }
            set { AppSettings.F = value; AppSettings.FBrush = GetBrush(value); }
        }

        /// <summary>
        /// Color for B face stickers
        /// </summary>
        public static Color BColor
        {
            get { return AppSettings.B; }
            set { AppSettings.B = value; AppSettings.BBrush = GetBrush(value); }
        }

        /// <summary>
        /// Color for R face stickers
        /// </summary>
        public static Color RColor
        {
            get { return AppSettings.R; }
            set { AppSettings.R = value; AppSettings.RBrush = GetBrush(value); }
        }

        /// <summary>
        /// Color for L face stickers
        /// </summary>
        public static Color LColor
        {
            get { return AppSettings.L; }
            set { AppSettings.L = value; AppSettings.LBrush = GetBrush(value); }
        }

        /// <summary>
        /// Color for neutral stickers
        /// </summary>
        public static Color NeutralColor
        {
            get { return AppSettings.Neutral; }
            set { AppSettings.Neutral = value; AppSettings.NeutralBrush = GetBrush(value); }
        }

        /// <summary>
        /// Color for base
        /// </summary>
        public static Color BaseColor
        {
            get { return AppSettings.Base; }
            set { AppSettings.Base = value; AppSettings.BaseBrush = GetBrush(value); }
        }

        /// <summary>
        /// Color for background
        /// </summary>
        public static Color BackgroundColor
        {
            get { return AppSettings.Background; }
            set { AppSettings.Background = value; AppSettings.BackgroundBrush = GetBrush(value); }
        }

        /// <summary>
        /// Color for arrows
        /// </summary>
        public static Color ArrowsColor
        {
            get { return AppSettings.Arrows; }
            set { AppSettings.Arrows = value; AppSettings.ArrowsBrush = GetBrush(value); }
        }

        /// <summary>
        /// Brush with U color
        /// </summary>
        public static Brush UBrush => AppSettings.UBrush;

        /// <summary>
        /// Brush with D color
        /// </summary>
        public static Brush DBrush => AppSettings.DBrush;

        /// <summary>
        /// Brush with F color
        /// </summary>
        public static Brush FBrush => AppSettings.FBrush;

        /// <summary>
        /// Brush with B color
        /// </summary>
        public static Brush BBrush => AppSettings.BBrush;

        /// <summary>
        /// Brush with R color
        /// </summary>
        public static Brush RBrush => AppSettings.RBrush;

        /// <summary>
        /// Brush with L color
        /// </summary>
        public static Brush LBrush => AppSettings.LBrush;

        /// <summary>
        /// Brush with Neutral color
        /// </summary>
        public static Brush NeutralBrush => AppSettings.NeutralBrush;

        /// <summary>
        /// Brush with Base color
        /// </summary>
        public static Brush BaseBrush => AppSettings.BaseBrush;

        /// <summary>
        /// Brush with Background color
        /// </summary>
        public static Brush BackgroundBrush => AppSettings.BackgroundBrush;

        /// <summary>
        /// Brush with Arrows color
        /// </summary>
        public static Brush ArrowsBrush => AppSettings.ArrowsBrush;

        #endregion General application properties

        #region Notation properties

        /// <summary>
        /// Get the char for U movements
        /// </summary>
        public static char UChar => NotationSettings.LayerChars[(int)Layers.U];

        /// <summary>
        /// Get the char for D movements
        /// </summary>
        public static char DChar => NotationSettings.LayerChars[(int)Layers.D];

        /// <summary>
        /// Get the char for F movements
        /// </summary>
        public static char FChar => NotationSettings.LayerChars[(int)Layers.F];

        /// <summary>
        /// Get the char for B movements
        /// </summary>
        public static char BChar => NotationSettings.LayerChars[(int)Layers.B];

        /// <summary>
        /// Get the char for R movements
        /// </summary>
        public static char RChar => NotationSettings.LayerChars[(int)Layers.R];

        /// <summary>
        /// Get the char for L movements
        /// </summary>
        public static char LChar => NotationSettings.LayerChars[(int)Layers.L];

        /// <summary>
        /// Get the char for E movements
        /// </summary>
        public static char EChar => NotationSettings.LayerChars[(int)Layers.E];

        /// <summary>
        /// Get the char for S movements
        /// </summary>
        public static char SChar => NotationSettings.LayerChars[(int)Layers.S];

        /// <summary>
        /// Get the char for M movements
        /// </summary>
        public static char MChar => NotationSettings.LayerChars[(int)Layers.M];

        /// <summary>
        /// Get the char for x movements
        /// </summary>
        public static char xChar => NotationSettings.RotationsChars[0];

        /// <summary>
        /// Get the char for y movements
        /// </summary>
        public static char yChar => NotationSettings.RotationsChars[1];

        /// <summary>
        /// Get the char for z movements
        /// </summary>
        public static char zChar => NotationSettings.RotationsChars[2];

        /// <summary>
        /// Get the char for double movement (default '2')
        /// </summary>
        public static char DoubleChar => NotationSettings.MovementsChars[0];

        /// <summary>
        /// Get the char for prime (default '\'') movements
        /// </summary>
        public static char PrimeChar => NotationSettings.MovementsChars[1];

        /// <summary>
        /// Get the char for w modifier
        /// </summary>
        public static char wChar => NotationSettings.ModifiersChars[(int)Modifiers.DOUBLE_ADJACENT_LAYERS_SAME_DIRECTION - 1];

        /// <summary>
        /// Get the char for o modifier
        /// </summary>
        public static char oChar => NotationSettings.ModifiersChars[(int)Modifiers.DOUBLE_ADJACENT_LAYERS_OPPOSITE_DIRECTION - 1];

        /// <summary>
        /// Get the char for s modifier
        /// </summary>
        public static char sChar => NotationSettings.ModifiersChars[(int)Modifiers.DOUBLE_OPPOSITE_LAYERS_SAME_DIRECTION - 1];

        /// <summary>
        /// Get the char for a modifier
        /// </summary>
        public static char aChar => NotationSettings.ModifiersChars[(int)Modifiers.DOUBLE_OPPOSITE_LAYERS_OPPOSITE_DIRECTION - 1];

        /// <summary>
        /// Get the char for u movements
        /// </summary>
        public static char AltUwChar => NotationSettings.AlternativeWChars[(int)Layers.U];

        /// <summary>
        /// Get the char for d movements
        /// </summary>
        public static char AltDwChar => NotationSettings.AlternativeWChars[(int)Layers.D];

        /// <summary>
        /// Get the char for f movements
        /// </summary>
        public static char AltFwChar => NotationSettings.AlternativeWChars[(int)Layers.F];

        /// <summary>
        /// Get the char for b movements
        /// </summary>
        public static char AltBwChar => NotationSettings.AlternativeWChars[(int)Layers.B];

        /// <summary>
        /// Get the char for Rw movements
        /// </summary>
        public static char AltRwChar => NotationSettings.AlternativeWChars[(int)Layers.R];

        /// <summary>
        /// Get the char for Lw movements
        /// </summary>
        public static char AltLwChar => NotationSettings.AlternativeWChars[(int)Layers.L];

        /// <summary>
        /// Get char for open parenthesis
        /// </summary>
        public static char OpenParenthesisChar => NotationSettings.EndStepChars[0];

        /// <summary>
        /// Get char for close parenthesis
        /// </summary>
        public static char CloseParenthesisChar => NotationSettings.EndStepChars[1];

        /// <summary>
        /// Get end step chars array
        /// </summary>
        public static char[] EndStepChars => NotationSettings.EndStepChars;

        /// <summary>
        /// Get layer chars array
        /// </summary>
        public static char[] LayerChars => NotationSettings.LayerChars;

        /// <summary>
        /// Get rotation chars array
        /// </summary>
        public static char[] RotationsChars => NotationSettings.RotationsChars;

        /// <summary>
        /// Get modifier chars array
        /// </summary>
        public static char[] ModifiersChars => NotationSettings.ModifiersChars;

        /// <summary>
        /// Get movement chars array
        /// </summary>
        public static char[] MovementsChars => NotationSettings.MovementsChars;

        /// <summary>
        /// Gets true if E layer rotation is set as same direction than U
        /// </summary>
        public static bool RotationOfEAsU => NotationSettings.ERotationAsU;

        /// <summary>
        /// Gets true if E layer rotation is set as same direction than D
        /// </summary>
        public static bool RotationOfEAsD => !NotationSettings.ERotationAsU;

        /// <summary>
        /// Gets true if S layer rotation is set as same direction than F
        /// </summary>
        public static bool RotationOfSAsF => NotationSettings.SRotationAsF;

        /// <summary>
        /// Gets true if S layer rotation is set as same direction than B
        /// </summary>
        public static bool RotationOfSAsB => !NotationSettings.SRotationAsF;

        /// <summary>
        /// Gets true if M layer rotation is set as same direction than R
        /// </summary>
        public static bool RotationOfMAsR => NotationSettings.MRotationAsR;

        /// <summary>
        /// Gets true if M layer rotation is set as same direction than L
        /// </summary>
        public static bool RotationOfMAsL => !NotationSettings.MRotationAsR;

        /// <summary>
        /// Returns true if using alternative *w chars (u for Uw, ...)
        /// </summary>
        public static bool UsingAltwChars => NotationSettings.UseAltwChars;

        #endregion Notation properties

        #region Chronometer properties

        /// <summary>
        /// Get if the beep sounds are enabled
        /// </summary>
        public static bool Beep
        {
            get { return ChronoSettings.BeepEnabled; }
            set { ChronoSettings.BeepEnabled = value; }
        }

        /// <summary>
        /// Default cube for solves
        /// </summary>
        public static string DefaultComment
        {
            get { return ChronoSettings.DefaultComment ?? string.Empty; }
            set { ChronoSettings.DefaultComment = value; }
        }

        /// <summary>
        /// Default cube for solves
        /// </summary>
        public static string DefaultCube
        {
            get { return ChronoSettings.DefaultCube ?? string.Empty; }
            set { ChronoSettings.DefaultCube = value; }
        }

        /// <summary>
        /// When importing scramble to chrono, convert to basic steps
        /// </summary>
        public static bool ChronoToBasic
        {
            get { return ChronoSettings.OptionToBasic; }
            set { ChronoSettings.OptionToBasic = value; }
        }

        /// <summary>
        /// When importing scramble to chrono, remove turns
        /// </summary>
        public static bool ChronoRemoveTurns
        {
            get { return ChronoSettings.OptionRemoveTurns; }
            set { ChronoSettings.OptionRemoveTurns = value; }
        }

        /// <summary>
        /// When importing scramble to chrono, shrink scramble
        /// </summary>
        public static bool ChronoShrink
        {
            get { return ChronoSettings.OptionShrink; }
            set { ChronoSettings.OptionShrink = value; }
        }

        /// <summary>
        /// Save chrono times (if false, times are lost)
        /// </summary>
        public static bool ChronoSaveTimes
        {
            get { return ChronoSettings.OptionSaveTimes; }
            set { ChronoSettings.OptionSaveTimes = value; }
        }

        /// <summary>
        /// Load current day times when program starts
        /// </summary>
        public static bool ChronoLoadAtStart
        {
            get { return ChronoSettings.OptionLoadAtStart; }
            set { ChronoSettings.OptionLoadAtStart = value; }
        }

        /// <summary>
        /// Time for a cube movement animation (50 ms ≤ Time ≤ 2000 ms)
        /// </summary>
        public static int ChronoAnimTime
        {
            get { return ChronoSettings.AnimationTime; }
            set
            {
                if (value < 50) ChronoSettings.AnimationTime = 50;
                else if (value > 2000) ChronoSettings.AnimationTime = 2000;
                else ChronoSettings.AnimationTime = value;
            }
        }

        #endregion Chronometer properties

        #region Progress properties

        /// <summary>
        /// Minimum solves to represent a period
        /// </summary>
        public static int MinimumSolvesPeriod
        {
            get { return ProgressSettings.MinimumSolvesPeriod; }
            set
            {
                if (value <= 1) ProgressSettings.MinimumSolvesPeriod = 1;
                else ProgressSettings.MinimumSolvesPeriod = value;
            }
        }

        /// <summary>
        /// Color for chart background
        /// </summary>
        public static Color ChartBackgroundColor
        {
            get { return ProgressSettings.ColorBackground; }
            set { ProgressSettings.ColorBackground = value; }
        }

        /// <summary>
        /// Color for maximum chart
        /// </summary>
        public static Color ChartMaximumColor
        {
            get { return ProgressSettings.ColorMaximum; }
            set { ProgressSettings.ColorMaximum = value; }
        }

        /// <summary>
        /// Color for minimum chart
        /// </summary>
        public static Color ChartMinimumColor
        {
            get { return ProgressSettings.ColorMinimum; }
            set { ProgressSettings.ColorMinimum = value; }
        }

        /// <summary>
        /// Color for average chart
        /// </summary>
        public static Color ChartAverageColor
        {
            get { return ProgressSettings.ColorAverage; }
            set { ProgressSettings.ColorAverage = value; }
        }

        /// <summary>
        /// Color for standard deviation chart
        /// </summary>
        public static Color ChartDeviationColor
        {
            get { return ProgressSettings.ColorDeviation; }
            set { ProgressSettings.ColorDeviation = value; }
        }

        /// <summary>
        /// Color for medium chart
        /// </summary>
        public static Color ChartMediumColor
        {
            get { return ProgressSettings.ColorMedium; }
            set { ProgressSettings.ColorMedium = value; }
        }

        /// <summary>
        /// Color for deviation chart
        /// </summary>
        public static Color ChartAmountColor
        {
            get { return ProgressSettings.ColorAmount; }
            set { ProgressSettings.ColorAmount = value; }
        }

        #endregion Progress properties

        #region Progress properties

        /// <summary>
        /// Upper margin for progress chart
        /// </summary>
        public static double ChartMarginUp => 10d;

        /// <summary>
        /// Down margin for progress chart
        /// </summary>
        public static double ChartMarginDown => 60d;

        /// <summary>
        /// Left margin for progress chart
        /// </summary>
        public static double ChartMarginLeft => 60d;

        /// <summary>
        /// Right margin for progress chart
        /// </summary>
        public static double ChartMarginRight => 40d;

        /// <summary>
        /// Max number of density of points to draw charts texts
        /// </summary>
        public static double ChartMaxDensity => 0.04d;

        #endregion Progress properties

        #region Editor properties

        /// <summary>
        /// Time for a cube movement animation (50 ms ≤ Time ≤ 2000 ms)
        /// </summary>
        public static int EditorAnimTime
        {
            get { return EditorSettings.AnimationTime; }
            set
            {
                if (value < 50) EditorSettings.AnimationTime = 50;
                else if (value > 2000) EditorSettings.AnimationTime = 2000;
                else EditorSettings.AnimationTime = value;
            }
        }

        /// <summary>
        /// Allow extended compound movements
        /// </summary>
        public static bool AllowExtendedCompoundMovements
        {
            get { return EditorSettings.OptionAllowExtendedCompoundMovements; }
            set { EditorSettings.OptionAllowExtendedCompoundMovements = value; }
        }

        #endregion Editor properties

        #region Library properties

        /// <summary>
        /// If true, when in the library a change in a view is made, check the algorithm unsaved flag
        /// </summary>
        public static bool ChangesInLibraryViews
        {
            get { return LibrarySettings.OptionViewChanges; }
            set { LibrarySettings.OptionViewChanges = value; }
        }

        /// <summary>
        /// Develop parentheses when creating initial pos. from algorithm 
        /// </summary>
        public static bool LibraryDevelopParentheses
        {
            get { return LibrarySettings.OptionDevelopParentheses; }
            set { LibrarySettings.OptionDevelopParentheses = value; }
        }

        /// <summary>
        /// Conver to to asic steps when creating initial pos. from algorithm 
        /// </summary>
        public static bool LibraryBasicSteps
        {
            get { return LibrarySettings.OptionBasicSteps; }
            set { LibrarySettings.OptionBasicSteps = value; }
        }

        /// <summary>
        /// Remove turns when creating initial pos. from algorithm 
        /// </summary>
        public static bool LibraryRemoveTurns
        {
            get { return LibrarySettings.OptionRemoveTurns; }
            set { LibrarySettings.OptionRemoveTurns = value; }
        }

        /// <summary>
        /// Shrink steps when creating initial pos. from algorithm 
        /// </summary>
        public static bool LibraryShrink
        {
            get { return LibrarySettings.OptionShrink; }
            set { LibrarySettings.OptionShrink = value; }
        }

        /// <summary>
        /// Inverse & reverse steps when creating initial pos. from algorithm 
        /// </summary>
        public static bool LibraryInverse
        {
            get { return LibrarySettings.OptionInverse; }
            set { LibrarySettings.OptionInverse = value; }
        }

        /// <summary>
        /// Insert steps at beginning when creating initial pos. from algorithm 
        /// </summary>
        public static bool LibraryInsertBeginning
        {
            get { return LibrarySettings.OptionInsertBeginning; }
            set { LibrarySettings.OptionInsertBeginning = value; }
        }

        /// <summary>
        /// Steps to insert when creating initial pos. from algorithm 
        /// </summary>
        public static string LibraryStepsToInsert
        {
            get { return LibrarySettings.OptionStepsToInsert; }
            set
            {
                if (value == null) LibrarySettings.OptionStepsToInsert = "";
                else LibrarySettings.OptionStepsToInsert = value;
            }
        }

        /// <summary>
        /// If true, algorithm default position is initial position. Otherwise is last position
        /// </summary>
        public static bool LibraryDefaultPositionStart
        {
            get { return LibrarySettings.OptionDefaultPositionStart; }
            set { LibrarySettings.OptionDefaultPositionStart = value; }
        }

        /// <summary>
        /// If true show parentheses in algorithm buttons
        /// </summary>
        public static bool LibraryShowParentheses
        {
            get { return LibrarySettings.OptionShowParentheses; }
            set { LibrarySettings.OptionShowParentheses = value; }
        }

        #endregion Library properties

        #region Constructor (static)

        /// <summary>
        /// Static constructor
        /// </summary>
        static AMSettings()
        {
            try
            {
                if (!System.IO.Directory.Exists(DataFolder)) System.IO.Directory.CreateDirectory(DataFolder);
            }
            catch (Exception ex) { Log = "Data folder error: " + ex.Message; }

            SetFactorySettings();
        }

        #endregion Constructor (static)

        #region General application functions

        /// <summary>
        /// Set program factory settings
        /// </summary>
        public static void SetFactorySettings()
        {
            SetDefaultChars();
            SetDefaultColors();

            ChronoSettings.AnimationTime = 500; // 500 ms

            CameraDistance = 30.0;
            CameraAlpha = 60.0;
            CameraBeta = 30.0;

            Beep = true;

            DefaultComment = string.Empty;
            DefaultCube = string.Empty;

            ChronoSettings.OptionToBasic = true;
            ChronoSettings.OptionRemoveTurns = true;
            ChronoSettings.OptionShrink = true;

            ChronoSettings.OptionSaveTimes = true;
            ChronoSettings.OptionLoadAtStart = true;

            EditorSettings.AnimationTime = 350; // 350 ms
            EditorSettings.OptionAllowExtendedCompoundMovements = false;

            ChangesInLibraryViews = true;
        }

        /// <summary>
        /// Set default colors for stickers and others
        /// </summary>
        public static void SetDefaultColors()
        {
            UColor = Colors.White;
            DColor = Colors.Yellow;
            FColor = Colors.Green;
            BColor = Colors.Blue;
            RColor = Colors.Red;
            LColor = Colors.Orange;
            NeutralColor = Colors.DarkGray;
            BackgroundColor = Colors.AntiqueWhite;
            BaseColor = Colors.Black;
            ArrowsColor = Colors.DarkRed;

            ChartBackgroundColor = Colors.Beige;
            ChartMaximumColor = Colors.Red;
            ChartMinimumColor = Colors.DarkGreen;
            ChartAverageColor = Colors.DarkBlue;
            ChartDeviationColor = Colors.DarkCyan;
            ChartMediumColor = Colors.DarkOrange;
            ChartAmountColor = Colors.ForestGreen;
        }

        /// <summary>
        /// Gets color in media format
        /// </summary>
        /// <param name="sdc">Color in drawing format</param>
        /// <returns>Color in media format</returns>
        public static Color ConvertColor(System.Drawing.Color sdc) => new Color
        {
            A = sdc.A,
            R = sdc.R,
            G = sdc.G,
            B = sdc.B
        };

        /// <summary>
        /// Gets color in drawing format
        /// </summary>
        /// <param name="sdc">Color in media format</param>
        /// <returns>Color in drawing format</returns>
        public static System.Drawing.Color ConvertColor(Color swmc) =>
            System.Drawing.Color.FromArgb(swmc.A, swmc.R, swmc.G, swmc.B);

        /// <summary>
        /// Get solid color brush
        /// </summary>
        /// <param name="sdc">Color in system.drawing.color format</param>
        /// <returns>Solid color brush</returns>
        public static Brush GetBrush(System.Drawing.Color sdc) => new SolidColorBrush(ConvertColor(sdc));

        /// <summary>
        /// Get solid color brush
        /// </summary>
        /// <param name="swmc">Color in system.windows.media.color format</param>
        /// <returns>Solid color brush</returns>
        public static Brush GetBrush(Color swmc) => new SolidColorBrush(swmc);

        /// <summary>
        /// Gets the position in the chart canvas of a given point with coordinates between 0 and 1 proportional
        /// </summary>
        /// <param name="Width">Canvas width</param>
        /// <param name="Height">Canvas height</param>
        /// <param name="PosX">X position (between 0 and 1 proportional)</param>
        /// <param name="PosY">Y position (between 0 and 1 proportional)</param>
        /// <returns>Chart position point</returns>
        public static System.Windows.Point GetChartPosition(double Width, double Height, double PosX, double PosY)
        {
            return new System.Windows.Point()
            {
                X = PosX * (Width - ChartMarginLeft - ChartMarginRight) + ChartMarginLeft,
                Y = Height - (PosY * (Height - ChartMarginUp - ChartMarginDown) + ChartMarginDown)
            };
        }

        /// <summary>
        /// Gets the position in the chart bars of a given point with coordinates between 0 and 1 proportional (no vertical margins)
        /// </summary>
        /// <param name="Width">Canvas width</param>
        /// <param name="Height">Canvas height</param>
        /// <param name="PosX">X position (between 0 and 1 proportional)</param>
        /// <param name="PosY">Y position (between 0 and 1 proportional)</param>
        /// <returns>Chart position point</returns>
        public static System.Windows.Point GetBarsPosition(double Width, double Height, double PosX, double PosY)
        {
            return new System.Windows.Point()
            {
                X = PosX * (Width - ChartMarginLeft - ChartMarginRight) + ChartMarginLeft,
                Y = Height - (PosY * Height)
            };
        }

        #endregion General application functions

        #region Arrays of chars functions

        /// <summary>
        /// Set default chars for steps
        /// </summary>
        public static void SetDefaultChars()
        {
            NotationSettings.LayerChars = new char[] { 'U', 'D', 'F', 'B', 'R', 'L', 'E', 'S', 'M' };
            NotationSettings.ModifiersChars = new char[] { 'w', 'o', 's', 'a' };
            NotationSettings.MovementsChars = new char[] { '2', '\'' };
            NotationSettings.RotationsChars = new char[] { 'x', 'y', 'z' };
            NotationSettings.AlternativeWChars = new char[] { 'u', 'd', 'f', 'b', 'r', 'l' };
            NotationSettings.EndStepChars = new char[] { '(', ')', '3', '4', '5', '6', '7', '8', '9' };
        }

        /// <summary>
        /// Builds a chars array with all posible main chars in the step (step's first char)
        /// </summary>
        /// <returns>Char's array with main step chars</returns>
        public static char[] GetMainCharsStepArray()
        {
            char[] MainCharsStepArray = new char[NotationSettings.LayerChars.Length + NotationSettings.RotationsChars.Length + NotationSettings.AlternativeWChars.Length];
            Array.Copy(NotationSettings.LayerChars, MainCharsStepArray, NotationSettings.LayerChars.Length);
            Array.Copy(NotationSettings.RotationsChars, 0, MainCharsStepArray, NotationSettings.LayerChars.Length, NotationSettings.RotationsChars.Length);
            Array.Copy(NotationSettings.AlternativeWChars, 0, MainCharsStepArray, NotationSettings.LayerChars.Length + NotationSettings.RotationsChars.Length, NotationSettings.AlternativeWChars.Length);
            return MainCharsStepArray;
        }

        /// <summary>
        /// Get an array with all possible chars
        /// </summary>
        /// <returns>Chars array with all possible chars</returns>
        public static char[] GetAllCharsArray()
        {
            int L1 = NotationSettings.LayerChars.Length,
                L2 = L1 + NotationSettings.RotationsChars.Length,
                L3 = L2 + NotationSettings.AlternativeWChars.Length,
                L4 = L3 + NotationSettings.ModifiersChars.Length,
                L5 = L4 + NotationSettings.MovementsChars.Length,
                TotalLength = L5 + NotationSettings.EndStepChars.Length;

            // Builds a chars array with all posible chars in a scramble
            char[] AllCharsArray = new char[TotalLength];
            Array.Copy(NotationSettings.LayerChars, AllCharsArray, NotationSettings.LayerChars.Length);
            Array.Copy(NotationSettings.RotationsChars, 0, AllCharsArray, L1, NotationSettings.RotationsChars.Length);
            Array.Copy(NotationSettings.AlternativeWChars, 0, AllCharsArray, L2, NotationSettings.AlternativeWChars.Length);
            Array.Copy(NotationSettings.ModifiersChars, 0, AllCharsArray, L3, NotationSettings.ModifiersChars.Length);
            Array.Copy(NotationSettings.MovementsChars, 0, AllCharsArray, L4, NotationSettings.MovementsChars.Length);
            Array.Copy(NotationSettings.EndStepChars, 0, AllCharsArray, L5, NotationSettings.EndStepChars.Length);
            return AllCharsArray;
        }

        /// <summary>
        /// Check is a char is already in the chars arrays
        /// </summary>
        /// <param name="c">Char to check</param>
        /// <returns>True if char is in char's arrays</returns>
        private static bool CheckChar(char c)
        {
            foreach (char cc in NotationSettings.LayerChars) if (c == cc) return true;
            foreach (char cc in NotationSettings.ModifiersChars) if (c == cc) return true;
            foreach (char cc in NotationSettings.MovementsChars) if (c == cc) return true;
            foreach (char cc in NotationSettings.RotationsChars) if (c == cc) return true;
            foreach (char cc in NotationSettings.AlternativeWChars) if (c == cc) return true;
            foreach (char cc in NotationSettings.EndStepChars) if (c == cc) return true;
            return false;
        }

        #endregion Arrays of chars

        #region Set chars functions

        /// <summary>
        /// Set the char for U movements
        /// </summary>
        /// <param name="c">Char for U movement</param>
        /// <returns>True if char is accepted</returns>
        public static bool SetUChar(char c)
        {
            if (c == UChar) return true;
            if (CheckChar(c)) return false; // Char is already in char's arrays
            NotationSettings.LayerChars[(int)Layers.U] = c;
            return true;
        }

        /// <summary>
        /// Set the char for D movements
        /// </summary>
        /// <param name="c">Char for D movement</param>
        /// <returns>True if char is accepted</returns>
        public static bool SetDChar(char c)
        {
            if (c == DChar) return true;
            if (CheckChar(c)) return false; // Char is already in char's arrays
            NotationSettings.LayerChars[(int)Layers.D] = c;
            return true;
        }

        /// <summary>
        /// Set the char for F movements
        /// </summary>
        /// <param name="c">Char for F movement</param>
        /// <returns>True if char is accepted</returns>
        public static bool SetFChar(char c)
        {
            if (c == FChar) return true;
            if (CheckChar(c)) return false; // Char is already in char's arrays
            NotationSettings.LayerChars[(int)Layers.F] = c;
            return true;
        }

        /// <summary>
        /// Set the char for B movements
        /// </summary>
        /// <param name="c">Char for B movement</param>
        /// <returns>True if char is accepted</returns>
        public static bool SetBChar(char c)
        {
            if (c == BChar) return true;
            if (CheckChar(c)) return false; // Char is already in char's arrays
            NotationSettings.LayerChars[(int)Layers.B] = c;
            return true;
        }

        /// <summary>
        /// Set the char for R movements
        /// </summary>
        /// <param name="c">Char for R movement</param>
        /// <returns>True if char is accepted</returns>
        public static bool SetRChar(char c)
        {
            if (c == RChar) return true;
            if (CheckChar(c)) return false; // Char is already in char's arrays
            NotationSettings.LayerChars[(int)Layers.R] = c;
            return true;
        }

        /// <summary>
        /// Set the char for L movements
        /// </summary>
        /// <param name="c">Char for L movement</param>
        /// <returns>True if char is accepted</returns>
        public static bool SetLChar(char c)
        {
            if (c == LChar) return true;
            if (CheckChar(c)) return false; // Char is already in char's arrays
            NotationSettings.LayerChars[(int)Layers.L] = c;
            return true;
        }

        /// <summary>
        /// Set the char for E movements
        /// </summary>
        /// <param name="c">Char for E movement</param>
        /// <returns>True if char is accepted</returns>
        public static bool SetEChar(char c)
        {
            if (c == EChar) return true;
            if (CheckChar(c)) return false; // Char is already in char's arrays
            NotationSettings.LayerChars[(int)Layers.E] = c;
            return true;
        }

        /// <summary>
        /// Set the char for S movements
        /// </summary>
        /// <param name="c">Char for S movement</param>
        /// <returns>True if char is accepted</returns>
        public static bool SetSChar(char c)
        {
            if (c == SChar) return true;
            if (CheckChar(c)) return false; // Char is already in char's arrays
            NotationSettings.LayerChars[(int)Layers.S] = c;
            return true;
        }

        /// <summary>
        /// Set the char for M movements
        /// </summary>
        /// <param name="c">Char for M movement</param>
        /// <returns>True if char is accepted</returns>
        public static bool SetMChar(char c)
        {
            if (c == MChar) return true;
            if (CheckChar(c)) return false; // Char is already in char's arrays
            NotationSettings.LayerChars[(int)Layers.M] = c;
            return true;
        }

        /// <summary>
        /// Set the alternative char for Uw movements
        /// </summary>
        /// <param name="c">Char for Uw movement</param>
        /// <returns>True if char is accepted</returns>
        public static bool SetAltUwChar(char c)
        {
            if (c == AltUwChar) return true;
            if (CheckChar(c)) return false; // Char is already in char's arrays
            NotationSettings.AlternativeWChars[(int)Layers.U] = c;
            return true;
        }

        /// <summary>
        /// Set the alternative char for Dw movements
        /// </summary>
        /// <param name="c">Char for Dw movement</param>
        /// <returns>True if char is accepted</returns>
        public static bool SetAltDwChar(char c)
        {
            if (c == AltDwChar) return true;
            if (CheckChar(c)) return false; // Char is already in char's arrays
            NotationSettings.AlternativeWChars[(int)Layers.D] = c;
            return true;
        }

        /// <summary>
        /// Set the alternative char for Fw movements
        /// </summary>
        /// <param name="c">Char for Fw movement</param>
        /// <returns>True if char is accepted</returns>
        public static bool SetAltFwChar(char c)
        {
            if (c == AltFwChar) return true;
            if (CheckChar(c)) return false; // Char is already in char's arrays
            NotationSettings.AlternativeWChars[(int)Layers.F] = c;
            return true;
        }

        /// <summary>
        /// Set the alternative char for Bw movements
        /// </summary>
        /// <param name="c">Char for Bw movement</param>
        /// <returns>True if char is accepted</returns>
        public static bool SetAltBwChar(char c)
        {
            if (c == AltBwChar) return true;
            if (CheckChar(c)) return false; // Char is already in char's arrays
            NotationSettings.AlternativeWChars[(int)Layers.B] = c;
            return true;
        }

        /// <summary>
        /// Set the alternative char for Rw movements
        /// </summary>
        /// <param name="c">Char for Rw movement</param>
        /// <returns>True if char is accepted</returns>
        public static bool SetAltRwChar(char c)
        {
            if (c == AltRwChar) return true;
            if (CheckChar(c)) return false; // Char is already in char's arrays
            NotationSettings.AlternativeWChars[(int)Layers.R] = c;
            return true;
        }

        /// <summary>
        /// Set the alternative char for Lw movements
        /// </summary>
        /// <param name="c">Char for Lw movement</param>
        /// <returns>True if char is accepted</returns>
        public static bool SetAltLwChar(char c)
        {
            if (c == AltLwChar) return true;
            if (CheckChar(c)) return false; // Char is already in char's arrays
            NotationSettings.AlternativeWChars[(int)Layers.L] = c;
            return true;
        }

        /// <summary>
        /// Set the char for x movements
        /// </summary>
        /// <param name="c">Char for x movement</param>
        /// <returns>True if char is accepted</returns>
        public static bool SetxChar(char c)
        {
            if (c == xChar) return true;
            if (CheckChar(c)) return false; // Char is already in char's arrays
            NotationSettings.RotationsChars[1] = c;
            return true;
        }

        /// <summary>
        /// Set the char for y movements
        /// </summary>
        /// <param name="c">Char for y movement</param>
        /// <returns>True if char is accepted</returns>
        public static bool SetyChar(char c)
        {
            if (c == yChar) return true;
            if (CheckChar(c)) return false; // Char is already in char's arrays
            NotationSettings.RotationsChars[1] = c;
            return true;
        }

        /// <summary>
        /// Set the char for z movements
        /// </summary>
        /// <param name="c">Char for z movement</param>
        /// <returns>True if char is accepted</returns>
        public static bool SetzChar(char c)
        {
            if (c == zChar) return true;
            if (CheckChar(c)) return false; // Char is already in char's arrays
            NotationSettings.RotationsChars[2] = c;
            return true;
        }

        /// <summary>
        /// Set the char for prime movements
        /// </summary>
        /// <param name="c">Char for prime movement</param>
        /// <returns>True if char is accepted</returns>
        public static bool SetPrimeChar(char c)
        {
            if (c == PrimeChar) return true;
            if (CheckChar(c)) return false; // Char is already in char's arrays
            NotationSettings.MovementsChars[1] = c;
            return true;
        }

        /// <summary>
        /// Set the char for double movements (default '2')
        /// </summary>
        /// <param name="c">Char for double movement (default '2')</param>
        /// <returns>True if char is accepted</returns>
        public static bool SetDoubleChar(char c)
        {
            if (c == DoubleChar) return true;
            if (CheckChar(c)) return false; // Char is already in char's arrays
            NotationSettings.MovementsChars[0] = c;
            return true;
        }

        /// <summary>
        /// Set the char for w modifier
        /// </summary>
        /// <param name="c">Char for w modifier</param>
        /// <returns>True if char is accepted</returns>
        public static bool SetwChar(char c)
        {
            if (c == wChar) return true;
            if (CheckChar(c)) return false; // Char is already in char's arrays
            NotationSettings.ModifiersChars[(int)Modifiers.DOUBLE_ADJACENT_LAYERS_SAME_DIRECTION - 1] = c;
            return true;
        }

        /// <summary>
        /// Set the char for o modifier
        /// </summary>
        /// <param name="c">Char for o modifier</param>
        /// <returns>True if char is accepted</returns>
        public static bool SetoChar(char c)
        {
            if (c == oChar) return true;
            if (CheckChar(c)) return false; // Char is already in char's arrays
            NotationSettings.ModifiersChars[(int)Modifiers.DOUBLE_ADJACENT_LAYERS_OPPOSITE_DIRECTION - 1] = c;
            return true;
        }

        /// <summary>
        /// Set the char for s modifier
        /// </summary>
        /// <param name="c">Char for s modifier</param>
        /// <returns>True if char is accepted</returns>
        public static bool SetsChar(char c)
        {
            if (c == SChar) return true;
            if (CheckChar(c)) return false; // Char is already in char's arrays
            NotationSettings.ModifiersChars[(int)Modifiers.DOUBLE_OPPOSITE_LAYERS_SAME_DIRECTION - 1] = c;
            return true;
        }

        /// <summary>
        /// Set the char for a modifier
        /// </summary>
        /// <param name="c">Char for a modifier</param>
        /// <returns>True if char is accepted</returns>
        public static bool SetaChar(char c)
        {
            if (c == aChar) return true;
            if (CheckChar(c)) return false; // Char is already in char's arrays
            NotationSettings.ModifiersChars[(int)Modifiers.DOUBLE_OPPOSITE_LAYERS_OPPOSITE_DIRECTION - 1] = c;
            return true;
        }

        /// <summary>
        /// Set the char for a given layer
        /// </summary>
        /// <param name="c">Char for a layer</param>
        /// <returns>True if char is accepted</returns>
        public static bool SetLayerChar(Layers Lyr, char c)
        {
            if (c == NotationSettings.LayerChars[(int)Lyr]) return true;
            if (CheckChar(c)) return false; // Char is already in char's arrays
            NotationSettings.LayerChars[(int)Lyr] = c;
            return true;
        }

        /// <summary>
        /// Get the current char for givel layer
        /// </summary>
        /// <param name="Lyr">Layer</param>
        /// <returns>Char for given layer</returns>
        public static char GetLayerChar(Layers Lyr) => NotationSettings.LayerChars[(int)Lyr];

        #endregion Set chars

        #region Direction of rotation and alternative chars functions

        /// <summary>
        /// Set the rotation direction of E layer
        /// </summary>
        /// <param name="dir">If true same direction as U layer</param>
        public static void SetERotDirection(bool dir) => NotationSettings.ERotationAsU = dir;

        /// <summary>
        /// Set the rotation direction of S layer
        /// </summary>
        /// <param name="dir">If true same direction as F layer</param>
        public static void SetSRotDirection(bool dir) => NotationSettings.SRotationAsF = dir;

        /// <summary>
        /// Set the rotation direction of M layer
        /// </summary>
        /// <param name="dir">If true same direction as R layer</param>
        public static void SetMRotDirection(bool dir) => NotationSettings.MRotationAsR = dir;

        /// <summary>
        /// To enable the use of alternative w chars
        /// </summary>
        public static void EnableUseAltwChars()
        {
            NotationSettings.UseAltwChars = true;
            ScrambleStep.SetStepsTexts();
        }

        /// <summary>
        /// To disable the use of alternative w chars
        /// </summary>
        public static void DisableUseAltwChars()
        {
            NotationSettings.UseAltwChars = false;
            ScrambleStep.SetStepsTexts();
        }

        #endregion Rotation direction and alternative chars

        #region Load & save functions

        /// <summary>
        /// Load an XML settings file
        /// </summary>
        /// <param name="SettingsFilePath">Path to a settings file</param>
        public static bool LoadXML(string SettingsFilePath)
        {
            if (!System.IO.File.Exists(SettingsFilePath)) return false;

            XmlDocument SettingsFile = new XmlDocument();
            try { SettingsFile.Load(SettingsFilePath); }
            catch (Exception ex)
            {
                Log = string.Format("Error loading settings file '{0}': ", SettingsFilePath) + ex.Message;
            }

            SetFactorySettings();

            XmlNode Node;

            try
            {
                Node = SettingsFile.DocumentElement.SelectSingleNode("/SettingsAM/Language");
                CurrentLanguageFile = Node.Attributes["file"].Value;

                if (!AMTexts.LoadTexts(CurrentLanguageFile)) CurrentLanguageFile = string.Empty;
            }
            catch { CurrentLanguageFile = string.Empty; }

            try
            {
                Node = SettingsFile.DocumentElement.SelectSingleNode("/SettingsAM/Notation/ELayerRotation");
                switch (Node.Attributes["as_layer"].Value)
                {
                    case "U": NotationSettings.ERotationAsU = true; break;
                    case "D": NotationSettings.ERotationAsU = false; break;
                    default: break;
                }
            }
            catch { NotationSettings.ERotationAsU = true; }

            try
            {
                Node = SettingsFile.DocumentElement.SelectSingleNode("/SettingsAM/Notation/SLayerRotation");
                switch (Node.Attributes["as_layer"].Value)
                {
                    case "F": NotationSettings.SRotationAsF = true; break;
                    case "B": NotationSettings.SRotationAsF = false; break;
                    default: break;
                }
            }
            catch { NotationSettings.SRotationAsF = false; }

            try
            {
                Node = SettingsFile.DocumentElement.SelectSingleNode("/SettingsAM/Notation/MLayerRotation");
                switch (Node.Attributes["as_layer"].Value)
                {
                    case "R": NotationSettings.MRotationAsR = true; break;
                    case "L": NotationSettings.MRotationAsR = false; break;
                    default: break;
                }
            }
            catch { NotationSettings.MRotationAsR = false; }

            try
            {
                Node = SettingsFile.DocumentElement.SelectSingleNode("/SettingsAM/Notation/AltwChars");
                switch (Node.Attributes["using"].Value)
                {
                    case "Yes": NotationSettings.UseAltwChars = true; break;
                    case "No": NotationSettings.UseAltwChars = false; break;
                    default: break;
                }
            }
            catch { NotationSettings.UseAltwChars = true; }

            try
            {
                Node = SettingsFile.DocumentElement.SelectSingleNode("/SettingsAM/Chronometer/ToBasicSteps");
                switch (Node.Attributes["apply"].Value)
                {
                    case "Yes": ChronoSettings.OptionToBasic = true; break;
                    case "No": ChronoSettings.OptionToBasic = false; break;
                    default: break;
                }
            }
            catch { ChronoSettings.OptionToBasic = false; }

            try
            {
                Node = SettingsFile.DocumentElement.SelectSingleNode("/SettingsAM/Chronometer/RemoveTurns");
                switch (Node.Attributes["apply"].Value)
                {
                    case "Yes": ChronoSettings.OptionRemoveTurns = true; break;
                    case "No": ChronoSettings.OptionRemoveTurns = false; break;
                    default: break;
                }
            }
            catch { ChronoSettings.OptionRemoveTurns = false; }

            try
            {
                Node = SettingsFile.DocumentElement.SelectSingleNode("/SettingsAM/Chronometer/Shrink");
                switch (Node.Attributes["apply"].Value)
                {
                    case "Yes": ChronoSettings.OptionShrink = true; break;
                    case "No": ChronoSettings.OptionShrink = false; break;
                    default: break;
                }
            }
            catch { ChronoSettings.OptionShrink = false; }

            try
            {
                Node = SettingsFile.DocumentElement.SelectSingleNode("/SettingsAM/Chronometer/Times");
                switch (Node.Attributes["save"].Value)
                {
                    case "Yes": ChronoSettings.OptionSaveTimes = true; break;
                    case "No": ChronoSettings.OptionSaveTimes = false; break;
                    default: break;
                }
                switch (Node.Attributes["load_at_start"].Value)
                {
                    case "Yes": ChronoSettings.OptionLoadAtStart = true; break;
                    case "No": ChronoSettings.OptionLoadAtStart = false; break;
                    default: break;
                }
            }
            catch
            {
                ChronoSettings.OptionSaveTimes = true;
                ChronoSettings.OptionLoadAtStart = true;
            }

            try
            {
                Node = SettingsFile.DocumentElement.SelectSingleNode("/SettingsAM/Chronometer/Beep");
                switch (Node.Attributes["enabled"].Value)
                {
                    case "Yes": ChronoSettings.BeepEnabled = true; break;
                    case "No": ChronoSettings.BeepEnabled = false; break;
                    default: break;
                }
            }
            catch { ChronoSettings.BeepEnabled = true; }

            try
            {
                Node = SettingsFile.DocumentElement.SelectSingleNode("/SettingsAM/Chronometer/DefaultCube");
                DefaultCube = Node.Attributes["name"].Value;
            }
            catch { DefaultCube = string.Empty; }

            try
            {
                Node = SettingsFile.DocumentElement.SelectSingleNode("/SettingsAM/Chronometer/DefaultComment");
                DefaultComment = Node.Attributes["text"].Value;
            }
            catch { DefaultComment = string.Empty; }

            try
            {
                Node = SettingsFile.DocumentElement.SelectSingleNode("/SettingsAM/Chronometer/AnimTime");
                ChronoAnimTime = int.Parse(Node.Attributes["ms"].Value);
            }
            catch { ChronoAnimTime = 350; }

            try
            {
                Node = SettingsFile.DocumentElement.SelectSingleNode("/SettingsAM/Colors/ColorU");
                UColor = Color.FromRgb(byte.Parse(Node.Attributes["R"].Value),
                                       byte.Parse(Node.Attributes["G"].Value),
                                       byte.Parse(Node.Attributes["B"].Value));

                Node = SettingsFile.DocumentElement.SelectSingleNode("/SettingsAM/Colors/ColorD");
                DColor = Color.FromRgb(byte.Parse(Node.Attributes["R"].Value),
                                       byte.Parse(Node.Attributes["G"].Value),
                                       byte.Parse(Node.Attributes["B"].Value));

                Node = SettingsFile.DocumentElement.SelectSingleNode("/SettingsAM/Colors/ColorF");
                FColor = Color.FromRgb(byte.Parse(Node.Attributes["R"].Value),
                                       byte.Parse(Node.Attributes["G"].Value),
                                       byte.Parse(Node.Attributes["B"].Value));

                Node = SettingsFile.DocumentElement.SelectSingleNode("/SettingsAM/Colors/ColorB");
                BColor = Color.FromRgb(byte.Parse(Node.Attributes["R"].Value),
                                       byte.Parse(Node.Attributes["G"].Value),
                                       byte.Parse(Node.Attributes["B"].Value));

                Node = SettingsFile.DocumentElement.SelectSingleNode("/SettingsAM/Colors/ColorR");
                RColor = Color.FromRgb(byte.Parse(Node.Attributes["R"].Value),
                                       byte.Parse(Node.Attributes["G"].Value),
                                       byte.Parse(Node.Attributes["B"].Value));

                Node = SettingsFile.DocumentElement.SelectSingleNode("/SettingsAM/Colors/ColorL");
                LColor = Color.FromRgb(byte.Parse(Node.Attributes["R"].Value),
                                       byte.Parse(Node.Attributes["G"].Value),
                                       byte.Parse(Node.Attributes["B"].Value));

                Node = SettingsFile.DocumentElement.SelectSingleNode("/SettingsAM/Colors/ColorNeutral");
                NeutralColor = Color.FromRgb(byte.Parse(Node.Attributes["R"].Value),
                                       byte.Parse(Node.Attributes["G"].Value),
                                       byte.Parse(Node.Attributes["B"].Value));

                Node = SettingsFile.DocumentElement.SelectSingleNode("/SettingsAM/Colors/ColorBase");
                BaseColor = Color.FromRgb(byte.Parse(Node.Attributes["R"].Value),
                                       byte.Parse(Node.Attributes["G"].Value),
                                       byte.Parse(Node.Attributes["B"].Value));

                Node = SettingsFile.DocumentElement.SelectSingleNode("/SettingsAM/Colors/ColorBackground");
                BackgroundColor = Color.FromRgb(byte.Parse(Node.Attributes["R"].Value),
                                       byte.Parse(Node.Attributes["G"].Value),
                                       byte.Parse(Node.Attributes["B"].Value));

                Node = SettingsFile.DocumentElement.SelectSingleNode("/SettingsAM/Colors/ColorArrows");
                ArrowsColor = Color.FromRgb(byte.Parse(Node.Attributes["R"].Value),
                                       byte.Parse(Node.Attributes["G"].Value),
                                       byte.Parse(Node.Attributes["B"].Value));

                Node = SettingsFile.DocumentElement.SelectSingleNode("/SettingsAM/Colors/ColorChartBackground");
                ChartBackgroundColor = Color.FromRgb(byte.Parse(Node.Attributes["R"].Value),
                                       byte.Parse(Node.Attributes["G"].Value),
                                       byte.Parse(Node.Attributes["B"].Value));

                Node = SettingsFile.DocumentElement.SelectSingleNode("/SettingsAM/Colors/ColorChartMaximum");
                ChartMaximumColor = Color.FromRgb(byte.Parse(Node.Attributes["R"].Value),
                                       byte.Parse(Node.Attributes["G"].Value),
                                       byte.Parse(Node.Attributes["B"].Value));

                Node = SettingsFile.DocumentElement.SelectSingleNode("/SettingsAM/Colors/ColorChartMinimum");
                ChartMinimumColor = Color.FromRgb(byte.Parse(Node.Attributes["R"].Value),
                                       byte.Parse(Node.Attributes["G"].Value),
                                       byte.Parse(Node.Attributes["B"].Value));

                Node = SettingsFile.DocumentElement.SelectSingleNode("/SettingsAM/Colors/ColorChartAverage");
                ChartAverageColor = Color.FromRgb(byte.Parse(Node.Attributes["R"].Value),
                                       byte.Parse(Node.Attributes["G"].Value),
                                       byte.Parse(Node.Attributes["B"].Value));

                Node = SettingsFile.DocumentElement.SelectSingleNode("/SettingsAM/Colors/ColorChartDeviation");
                ChartDeviationColor = Color.FromRgb(byte.Parse(Node.Attributes["R"].Value),
                                       byte.Parse(Node.Attributes["G"].Value),
                                       byte.Parse(Node.Attributes["B"].Value));

                Node = SettingsFile.DocumentElement.SelectSingleNode("/SettingsAM/Colors/ColorChartMedium");
                ChartMediumColor = Color.FromRgb(byte.Parse(Node.Attributes["R"].Value),
                                       byte.Parse(Node.Attributes["G"].Value),
                                       byte.Parse(Node.Attributes["B"].Value));

                Node = SettingsFile.DocumentElement.SelectSingleNode("/SettingsAM/Colors/ColorChartAmount");
                ChartAmountColor = Color.FromRgb(byte.Parse(Node.Attributes["R"].Value),
                                       byte.Parse(Node.Attributes["G"].Value),
                                       byte.Parse(Node.Attributes["B"].Value));
            }
            catch { SetDefaultColors(); }

            try
            {
                Node = SettingsFile.DocumentElement.SelectSingleNode("/SettingsAM/Progress/MinimumSolves");
                MinimumSolvesPeriod = int.Parse(Node.Attributes["amount"].Value);
            }
            catch { MinimumSolvesPeriod = 5; }

            try
            {
                Node = SettingsFile.DocumentElement.SelectSingleNode("/SettingsAM/Editor/ExtendedCompoundMovements");
                switch (Node.Attributes["allow"].Value)
                {
                    case "Yes": EditorSettings.OptionAllowExtendedCompoundMovements = true; break;
                    case "No": EditorSettings.OptionAllowExtendedCompoundMovements = false; break;
                    default: break;
                }
            }
            catch { EditorSettings.OptionAllowExtendedCompoundMovements = false; }

            try
            {
                Node = SettingsFile.DocumentElement.SelectSingleNode("/SettingsAM/Editor/AnimTime");
                EditorAnimTime = int.Parse(Node.Attributes["ms"].Value);
            }
            catch { EditorAnimTime = 350; }

            try
            {
                Node = SettingsFile.DocumentElement.SelectSingleNode("/SettingsAM/Library/ChangesInViews");
                switch (Node.Attributes["ask_to_save"].Value)
                {
                    case "Yes": LibrarySettings.OptionViewChanges = true; break;
                    case "No": LibrarySettings.OptionViewChanges = false; break;
                    default: break;
                }
            }
            catch { LibrarySettings.OptionViewChanges = false; }

            try
            {
                Node = SettingsFile.DocumentElement.SelectSingleNode("/SettingsAM/Library/DefaultStart");
                switch (Node.Attributes["position"].Value)
                {
                    case "Beggining": LibrarySettings.OptionDefaultPositionStart = true; break;
                    case "End": LibrarySettings.OptionDefaultPositionStart = false; break;
                    default: break;
                }
            }
            catch { LibrarySettings.OptionDefaultPositionStart = true; }

            try
            {
                Node = SettingsFile.DocumentElement.SelectSingleNode("/SettingsAM/Library/Parentheses");
                switch (Node.Attributes["show"].Value)
                {
                    case "Yes": LibrarySettings.OptionShowParentheses = true; break;
                    case "No": LibrarySettings.OptionShowParentheses = false; break;
                    default: break;
                }
            }
            catch { LibrarySettings.OptionShowParentheses = false; }

            try
            {
                Node = SettingsFile.DocumentElement.SelectSingleNode("/SettingsAM/Library/WhenCreatingStartPosition");
                switch (Node.Attributes["develop_parentheses"].Value)
                {
                    case "Yes": LibrarySettings.OptionDevelopParentheses = true; break;
                    case "No": LibrarySettings.OptionDevelopParentheses = false; break;
                    default: break;
                }
                switch (Node.Attributes["remove_turns"].Value)
                {
                    case "Yes": LibrarySettings.OptionRemoveTurns = true; break;
                    case "No": LibrarySettings.OptionRemoveTurns = false; break;
                    default: break;
                }
                switch (Node.Attributes["convert_to_basic_steps"].Value)
                {
                    case "Yes": LibrarySettings.OptionBasicSteps = true; break;
                    case "No": LibrarySettings.OptionBasicSteps = false; break;
                    default: break;
                }
                switch (Node.Attributes["shrink_steps"].Value)
                {
                    case "Yes": LibrarySettings.OptionShrink = true; break;
                    case "No": LibrarySettings.OptionShrink = false; break;
                    default: break;
                }
                switch (Node.Attributes["invert_steps"].Value)
                {
                    case "Yes": LibrarySettings.OptionInverse = true; break;
                    case "No": LibrarySettings.OptionInverse = false; break;
                    default: break;
                }
                switch (Node.Attributes["insert_steps"].Value)
                {
                    case "Yes": LibrarySettings.OptionInsertBeginning = true; break;
                    case "No": LibrarySettings.OptionInsertBeginning = false; break;
                    default: break;
                }
                LibrarySettings.OptionStepsToInsert = Node.Attributes["steps_to_insert"].Value;
            }
            catch
            {
                LibrarySettings.OptionDevelopParentheses = false;
                LibrarySettings.OptionRemoveTurns = false;
                LibrarySettings.OptionBasicSteps = false;
                LibrarySettings.OptionShrink = false;
                LibrarySettings.OptionInverse = false;
                LibrarySettings.OptionInsertBeginning = false;
                LibrarySettings.OptionStepsToInsert = string.Empty;
            }

            Status = AMTexts.Message("SettingsFileLoadedMessage") + SettingsFilePath;
            return true;
        }

        /// <summary>
        /// Save an XML settings file
        /// </summary>
        /// <param name="SettingsFilePath">Path to a settings file</param>
        public static bool SaveXML(string SettingsFilePath)
        {
            try { System.IO.File.Delete(SettingsFilePath); }
            catch (Exception ex)
            {
                Log = "Error deleting settings file: " + ex.Message;
                return false;
            }

            XmlDocument XmlSettings = new XmlDocument();
            XmlElement RootElement = XmlSettings.CreateElement("SettingsAM");

            XmlElement AuxElement;

            // Language settings
            XmlElement LanguageElement = XmlSettings.CreateElement("Language");
            LanguageElement.SetAttribute("file", CurrentLanguageFile);

            // Notation settings
            XmlElement NotationElements = XmlSettings.CreateElement("Notation");

            AuxElement = XmlSettings.CreateElement("ELayerRotation");
            AuxElement.SetAttribute("as_layer", RotationOfEAsU ? "U" : "D");
            NotationElements.AppendChild(AuxElement);

            AuxElement = XmlSettings.CreateElement("SLayerRotation");
            AuxElement.SetAttribute("as_layer", RotationOfSAsF ? "F" : "B");
            NotationElements.AppendChild(AuxElement);

            AuxElement = XmlSettings.CreateElement("MLayerRotation");
            AuxElement.SetAttribute("as_layer", RotationOfMAsR ? "R" : "L");
            NotationElements.AppendChild(AuxElement);

            AuxElement = XmlSettings.CreateElement("AltwChars");
            AuxElement.SetAttribute("using", UsingAltwChars ? "Yes" : "No");
            NotationElements.AppendChild(AuxElement);

            // Colors settings
            XmlElement ColorElements = XmlSettings.CreateElement("Colors");

            AuxElement = XmlSettings.CreateElement("ColorU");
            AuxElement.SetAttribute("R", UColor.R.ToString("000"));
            AuxElement.SetAttribute("G", UColor.G.ToString("000"));
            AuxElement.SetAttribute("B", UColor.B.ToString("000"));
            ColorElements.AppendChild(AuxElement);

            AuxElement = XmlSettings.CreateElement("ColorD");
            AuxElement.SetAttribute("R", DColor.R.ToString("000"));
            AuxElement.SetAttribute("G", DColor.G.ToString("000"));
            AuxElement.SetAttribute("B", DColor.B.ToString("000"));
            ColorElements.AppendChild(AuxElement);

            AuxElement = XmlSettings.CreateElement("ColorF");
            AuxElement.SetAttribute("R", FColor.R.ToString("000"));
            AuxElement.SetAttribute("G", FColor.G.ToString("000"));
            AuxElement.SetAttribute("B", FColor.B.ToString("000"));
            ColorElements.AppendChild(AuxElement);

            AuxElement = XmlSettings.CreateElement("ColorB");
            AuxElement.SetAttribute("R", BColor.R.ToString("000"));
            AuxElement.SetAttribute("G", BColor.G.ToString("000"));
            AuxElement.SetAttribute("B", BColor.B.ToString("000"));
            ColorElements.AppendChild(AuxElement);

            AuxElement = XmlSettings.CreateElement("ColorR");
            AuxElement.SetAttribute("R", RColor.R.ToString("000"));
            AuxElement.SetAttribute("G", RColor.G.ToString("000"));
            AuxElement.SetAttribute("B", RColor.B.ToString("000"));
            ColorElements.AppendChild(AuxElement);

            AuxElement = XmlSettings.CreateElement("ColorL");
            AuxElement.SetAttribute("R", LColor.R.ToString("000"));
            AuxElement.SetAttribute("G", LColor.G.ToString("000"));
            AuxElement.SetAttribute("B", LColor.B.ToString("000"));
            ColorElements.AppendChild(AuxElement);

            AuxElement = XmlSettings.CreateElement("ColorNeutral");
            AuxElement.SetAttribute("R", NeutralColor.R.ToString("000"));
            AuxElement.SetAttribute("G", NeutralColor.G.ToString("000"));
            AuxElement.SetAttribute("B", NeutralColor.B.ToString("000"));
            ColorElements.AppendChild(AuxElement);

            AuxElement = XmlSettings.CreateElement("ColorBase");
            AuxElement.SetAttribute("R", BaseColor.R.ToString("000"));
            AuxElement.SetAttribute("G", BaseColor.G.ToString("000"));
            AuxElement.SetAttribute("B", BaseColor.B.ToString("000"));
            ColorElements.AppendChild(AuxElement);

            AuxElement = XmlSettings.CreateElement("ColorBackground");
            AuxElement.SetAttribute("R", BackgroundColor.R.ToString("000"));
            AuxElement.SetAttribute("G", BackgroundColor.G.ToString("000"));
            AuxElement.SetAttribute("B", BackgroundColor.B.ToString("000"));
            ColorElements.AppendChild(AuxElement);

            AuxElement = XmlSettings.CreateElement("ColorArrows");
            AuxElement.SetAttribute("R", ArrowsColor.R.ToString("000"));
            AuxElement.SetAttribute("G", ArrowsColor.G.ToString("000"));
            AuxElement.SetAttribute("B", ArrowsColor.B.ToString("000"));
            ColorElements.AppendChild(AuxElement);

            AuxElement = XmlSettings.CreateElement("ColorChartBackground");
            AuxElement.SetAttribute("R", ChartBackgroundColor.R.ToString("000"));
            AuxElement.SetAttribute("G", ChartBackgroundColor.G.ToString("000"));
            AuxElement.SetAttribute("B", ChartBackgroundColor.B.ToString("000"));
            ColorElements.AppendChild(AuxElement);

            AuxElement = XmlSettings.CreateElement("ColorChartMaximum");
            AuxElement.SetAttribute("R", ChartMaximumColor.R.ToString("000"));
            AuxElement.SetAttribute("G", ChartMaximumColor.G.ToString("000"));
            AuxElement.SetAttribute("B", ChartMaximumColor.B.ToString("000"));
            ColorElements.AppendChild(AuxElement);

            AuxElement = XmlSettings.CreateElement("ColorChartMinimum");
            AuxElement.SetAttribute("R", ChartMinimumColor.R.ToString("000"));
            AuxElement.SetAttribute("G", ChartMinimumColor.G.ToString("000"));
            AuxElement.SetAttribute("B", ChartMinimumColor.B.ToString("000"));
            ColorElements.AppendChild(AuxElement);

            AuxElement = XmlSettings.CreateElement("ColorChartAverage");
            AuxElement.SetAttribute("R", ChartAverageColor.R.ToString("000"));
            AuxElement.SetAttribute("G", ChartAverageColor.G.ToString("000"));
            AuxElement.SetAttribute("B", ChartAverageColor.B.ToString("000"));
            ColorElements.AppendChild(AuxElement);

            AuxElement = XmlSettings.CreateElement("ColorChartDeviation");
            AuxElement.SetAttribute("R", ChartDeviationColor.R.ToString("000"));
            AuxElement.SetAttribute("G", ChartDeviationColor.G.ToString("000"));
            AuxElement.SetAttribute("B", ChartDeviationColor.B.ToString("000"));
            ColorElements.AppendChild(AuxElement);

            AuxElement = XmlSettings.CreateElement("ColorChartMedium");
            AuxElement.SetAttribute("R", ChartMediumColor.R.ToString("000"));
            AuxElement.SetAttribute("G", ChartMediumColor.G.ToString("000"));
            AuxElement.SetAttribute("B", ChartMediumColor.B.ToString("000"));
            ColorElements.AppendChild(AuxElement);

            AuxElement = XmlSettings.CreateElement("ColorChartAmount");
            AuxElement.SetAttribute("R", ChartAmountColor.R.ToString("000"));
            AuxElement.SetAttribute("G", ChartAmountColor.G.ToString("000"));
            AuxElement.SetAttribute("B", ChartAmountColor.B.ToString("000"));
            ColorElements.AppendChild(AuxElement);

            // Chronometer settings
            XmlElement ProgressElements = XmlSettings.CreateElement("Progress");

            AuxElement = XmlSettings.CreateElement("MinimumSolves");
            AuxElement.SetAttribute("amount", MinimumSolvesPeriod.ToString());
            ProgressElements.AppendChild(AuxElement);

            // Chronometer settings
            XmlElement ChronoElements = XmlSettings.CreateElement("Chronometer");

            AuxElement = XmlSettings.CreateElement("ToBasicSteps");
            AuxElement.SetAttribute("apply", ChronoToBasic ? "Yes" : "No");
            ChronoElements.AppendChild(AuxElement);

            AuxElement = XmlSettings.CreateElement("RemoveTurns");
            AuxElement.SetAttribute("apply", ChronoRemoveTurns ? "Yes" : "No");
            ChronoElements.AppendChild(AuxElement);

            AuxElement = XmlSettings.CreateElement("Shrink");
            AuxElement.SetAttribute("apply", ChronoShrink ? "Yes" : "No");
            ChronoElements.AppendChild(AuxElement);

            AuxElement = XmlSettings.CreateElement("Times");
            AuxElement.SetAttribute("save", ChronoSaveTimes ? "Yes" : "No");
            AuxElement.SetAttribute("load_at_start", ChronoLoadAtStart ? "Yes" : "No");
            ChronoElements.AppendChild(AuxElement);

            AuxElement = XmlSettings.CreateElement("Beep");
            AuxElement.SetAttribute("enabled", Beep ? "Yes" : "No");
            ChronoElements.AppendChild(AuxElement);

            AuxElement = XmlSettings.CreateElement("DefaultCube");
            AuxElement.SetAttribute("name", DefaultCube);
            ChronoElements.AppendChild(AuxElement);

            AuxElement = XmlSettings.CreateElement("DefaultComment");
            AuxElement.SetAttribute("text", DefaultComment);
            ChronoElements.AppendChild(AuxElement);

            AuxElement = XmlSettings.CreateElement("AnimTime");
            AuxElement.SetAttribute("ms", ChronoAnimTime.ToString());
            ChronoElements.AppendChild(AuxElement);

            // Editor settings
            XmlElement EditorElements = XmlSettings.CreateElement("Editor");

            AuxElement = XmlSettings.CreateElement("ExtendedCompoundMovements");
            AuxElement.SetAttribute("allow", AllowExtendedCompoundMovements ? "Yes" : "No");
            EditorElements.AppendChild(AuxElement);

            AuxElement = XmlSettings.CreateElement("AnimTime");
            AuxElement.SetAttribute("ms", EditorAnimTime.ToString());
            EditorElements.AppendChild(AuxElement);

            // Library settings
            XmlElement LibraryElements = XmlSettings.CreateElement("Library");

            AuxElement = XmlSettings.CreateElement("ChangesInViews");
            AuxElement.SetAttribute("ask_to_save", ChangesInLibraryViews ? "Yes" : "No");
            LibraryElements.AppendChild(AuxElement);

            AuxElement = XmlSettings.CreateElement("DefaultStart");
            AuxElement.SetAttribute("position", LibraryDefaultPositionStart ? "Beggining" : "End");
            LibraryElements.AppendChild(AuxElement);

            AuxElement = XmlSettings.CreateElement("Parentheses");
            AuxElement.SetAttribute("show", LibraryShowParentheses ? "Yes" : "No");
            LibraryElements.AppendChild(AuxElement);

            AuxElement = XmlSettings.CreateElement("WhenCreatingStartPosition");
            AuxElement.SetAttribute("develop_parentheses", LibraryDevelopParentheses ? "Yes" : "No");
            AuxElement.SetAttribute("remove_turns", LibraryRemoveTurns ? "Yes" : "No");
            AuxElement.SetAttribute("convert_to_basic_steps", LibraryBasicSteps ? "Yes" : "No");
            AuxElement.SetAttribute("shrink_steps", LibraryShrink ? "Yes" : "No");
            AuxElement.SetAttribute("invert_steps", LibraryInverse ? "Yes" : "No");
            AuxElement.SetAttribute("insert_steps", LibraryInsertBeginning ? "Yes" : "No");
            AuxElement.SetAttribute("steps_to_insert", LibraryStepsToInsert);
            LibraryElements.AppendChild(AuxElement);

            // Add children to root
            RootElement.AppendChild(LanguageElement);
            RootElement.AppendChild(NotationElements);
            RootElement.AppendChild(ColorElements);
            RootElement.AppendChild(ChronoElements);
            RootElement.AppendChild(ProgressElements);
            RootElement.AppendChild(EditorElements);
            RootElement.AppendChild(LibraryElements);

            XmlSettings.AppendChild(RootElement);
            try { XmlSettings.Save(SettingsFilePath); }
            catch (Exception ex)
            {
                Log = string.Format("Error saving settings file '{0}': ", SettingsFilePath) + ex.Message;
                return false;
            }

            Status = AMTexts.Message("SettingsFileSavedMessage") + SettingsFilePath;

            return true;
        }

        #endregion Load & Save
    }
}
