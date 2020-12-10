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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Linq;
using CatenaLogic.Windows.Presentation.WebcamPlayer;
using LiteDB;
using SharpAvi.Codecs;
using SharpAvi.Output;

namespace Algorithm_Master
{
    #region Enums, auxiliary classes & structs

    /// <summary>
    /// Identifiers for the main window tabs
    /// </summary>
    public enum MainTabNames { Chrono, Progress, Editor, Library, Search, Settings }

    #region Chronometer auxiliary stuff

    /// <summary>
    /// Types of resolutions
    /// </summary>
    public enum ResolutionTypes : int
    {
        WCA_SPEED_SOLVING = 0,
        WCA_BLINDFOLDED_SOLVING = 1,
        WCA_ONE_HANDED_SOLVING = 2,
        WCA_SOLVING_WITH_FEET = 3,
        /*
        TRAINING_WHITE_CROSS = 4,
        TRAINING_ANY_CROSS = 5,
        TRAINING_WHITE_CROSS_DOWN = 6,
        TRAINING_ANY_CROSS_DOWN = 7,
        TRAINING_F2L = 8,
        TRAINING_CROSS_F2L = 9,
        TRAINING_OLL = 10,
        TRAINING_CROSS_F2L_OLL = 11,
        TRAINING_PLL = 12,
        TRAINING_OLL_PLL = 13
        */
    }

    /// <summary>
    /// Enum with all chronometer states
    /// </summary>
    public enum ChronoStates
    {
        INITIAL_WAIT, PRE_START, WAITING_START, INSPECTION, RUNNING, FINISHED
    }

    /// <summary>
    /// Enum with all inspection states
    /// </summary>
    public enum InspectionStates
    {
        PRE_INSPECTION, FIRST_8_SECONDS, OVER_8_SECONDS, OVER_12_SECONDS, OVER_15_SECONDS, DNF
    }

    /// <summary>
    /// Struct to store chronometer results
    /// </summary>
    public struct ChronoResult
    {
        /// <summary>
        /// Date & time of measure
        /// </summary>
        public DateTime StartTime;

        /// <summary>
        /// Time measured in ms
        /// </summary>
        public long MeasuredTime;

        /// <summary>
        /// Did Not Start
        /// </summary>
        public bool DNS;

        /// <summary>
        /// Did Not Finish
        /// </summary>
        public bool DNF;

        /// <summary>
        /// Two seconds penalty (not completely solved - WCA regulation)
        /// </summary>
        public bool NotSolvedPenalty2s;

        /// <summary>
        /// Two seconds penalty (inspection longer than 15 s - WCA regulation)
        /// </summary>
        public bool StartDelayPenalty2s;

        /// <summary>
        /// DNF penalty (inspection longer than 17 s - WCA regulation)
        /// </summary>
        public bool StartDelayDNF;

        /// <summary>
        /// Scramble
        /// </summary>
        public string Scramble;

        /// <summary>
        /// Solving type
        /// </summary>
        public ResolutionTypes SolvingType;

        /// <summary>
        /// Cube
        /// </summary>
        public string Cube;

        /// <summary>
        /// Comments
        /// </summary>
        public string Comment;

        /// <summary>
        /// Reset chrono result values
        /// </summary>
        public void Reset()
        {
            StartTime = DateTime.Now;
            MeasuredTime = 0;
            DNS = DNF = NotSolvedPenalty2s = StartDelayPenalty2s = false;
            Scramble = string.Empty;
            SolvingType = ResolutionTypes.WCA_SPEED_SOLVING;
            Cube = string.Empty;
            Comment = string.Empty;
        }
    }

    /// <summary>
    /// Class to manage chrono results in a listview
    /// </summary>
    public class ResultDataRow
    { // Same field names in ListView ChronoResultsTable
        public string ChronoDateTime { get; set; }
        public string ChronoTime { get; set; }
        public string ChronoPenalty { get; set; }
        public string ChronoScramble { get; set; }
        public string ChronoType { get; set; }
        public string ChronoCube { get; set; }
        public string ChronoComment { get; set; }
    }

    /// <summary>
    /// Class to manage chrono results in a LiteDB data base
    /// </summary>
    public class ResultDB
    {
        public int Id { get; set; } // Index for LiteDB

        // Same field names in ListView ChronoResultsTable
        public DateTime ChronoDateTime { get; set; }
        public int ChronoTime { get; set; }
        public string ChronoPenalty { get; set; }
        public string ChronoScramble { get; set; }
        public string ChronoType { get; set; }
        public string ChronoCube { get; set; }
        public string ChronoComment { get; set; }
    }

    /// <summary>
    /// Structure to group chronometer fields
    /// </summary>
    public struct ChronoFields
    {
        /// <summary>
        /// Scramble for chronometer
        /// </summary>
        public Scramble Scramble;

        /// <summary>
        /// Cube for chronometer
        /// </summary>
        public Cube3D Cube;

        /// <summary>
        /// Current step position in the scramble
        /// </summary>
        public int ScramblePosition;

        /// <summary>
        /// Buttons array for scramble
        /// </summary>
        public Button[] ScrambleButtons;

        /// <summary>
        /// Flag to know if the 3D cube is being oriented
        /// </summary>
        public bool MovingChronoCube;

        /// <summary>
        /// To avoid create a new scramble without saving last result
        /// </summary>
        public bool ResultNotSavedFlag;

        /// <summary>
        /// For time events
        /// </summary>
        public DispatcherTimer EventsTimerLauncher;

        /// <summary>
        /// To measure chrono times
        /// </summary>
        public Stopwatch Chronometer;

        /// <summary>
        /// Result of current time measure
        /// </summary>
        public ChronoResult Result;

        /// <summary>
        /// Chronometer status
        /// </summary>
        public ChronoStates State;

        /// <summary>
        /// Inspection status
        /// </summary>
        public InspectionStates Inspection;

        /// <summary>
        /// Best time of solving data grid
        /// </summary>
        public int BestTime;

        /// <summary>
        /// Worst time of solving data grid
        /// </summary>
        public int WorstTime;

        /// <summary>
        /// Average time of solving data grid
        /// </summary>
        public int AverageTime;

        /// <summary>
        /// Medium time of solving data grid
        /// </summary>
        public int MediumTime;

        /// <summary>
        /// Standard deviation of solving data grid
        /// </summary>
        public int StandardDeviation;

        /// <summary>
        /// Buttons array for chrono cube 2D view (a button per sticker)
        /// </summary>
        public Button[] Cube2DButtons;

        /// <summary>
        /// Background task for animations
        /// </summary>
        public BackgroundWorker AnimWork;

        /// <summary>
        /// Target position for scramble animation
        /// </summary>
        public int ScramblePosTarget;

        /// <summary>
        /// Sort direction in chrono table column
        /// </summary>
        public ListSortDirection TableSortDirection;

        /// <summary>
        /// Column to order chrono table
        /// </summary>
        public GridViewColumnHeader TableSortColumn;

        /// <summary>
        /// Camera monikers
        /// </summary>
        public FilterInfo[] CameraMonikers;

        /// <summary>
        /// Camera control
        /// </summary>
        public CapDevice Camera;

        /// <summary>
        /// Flag to know if there is a camera ready to save video
        /// </summary>
        public bool CameraReadyFlag;

        /// <summary>
        /// Video writer control
        /// </summary>
        public AviWriter VideoWriter;

        /// <summary>
        /// Video stream control
        /// </summary>
        public IAviVideoStream VideoStream;

        /// <summary>
        /// Video timer control
        /// </summary>
        public DispatcherTimer VideoTimer;

        /// <summary>
        /// Flag to know if video is being saved
        /// </summary>
        public bool VideoSavingFlag;

        /// <summary>
        /// Flag to know if there is a video file ready to move
        /// </summary>
        public bool VideoReadyFlag;

        /// <summary>
        /// Last folder to save the video in
        /// </summary>
        public string LastVideoFolder;
    }

    #endregion Chronometer auxiliary stuff

    #region Progress definitions

    /// <summary>
    /// Structure for draw progress charts
    /// </summary>
    public struct ProgressData
    {
        public string Labels;
        public int Max, Min, Average, StdDeviation, Medium, NumSolves;
    };

    #endregion Progress definitions

    #region Editor auxiliary stuff

    /// <summary>
    /// Structure to group editor fields
    /// </summary>
    public struct EditorFields
    {
        /// <summary>
        /// Scramble in the editor tab
        /// </summary>
        public Scramble Scramble;

        /// <summary>
        /// Steps buttons array in the editor tab
        /// </summary>
        public Button[] ScrambleButtons;

        /// <summary>
        /// Array of buttons for the editor cube skin mosaic
        /// </summary>
        public Button[] Cube2DButtons;

        /// <summary>
        /// Nesting control of parenthesis in editor tab
        /// </summary>
        public int ParenthesesNest;

        /// <summary>
        /// Cube for 3D view in editor tab
        /// </summary>
        public Cube3D Cube;

        /// <summary>
        /// Flag to know if is activated the change of orientation of 3D editor cube
        /// </summary>
        public bool MovingEditorCube;

        /// <summary>
        /// First step selected in editor scramble
        /// </summary>
        public int FirstStepSelected;

        /// <summary>
        /// Last step selected in editor scramble
        /// </summary>
        public int LastStepSelected;

        /// <summary>
        /// List for undo editor scramble
        /// </summary>
        public List<String> UndoList;

        /// <summary>
        /// Index for undo tool
        /// </summary>
        public int UndoIndex;

        /// <summary>
        /// Background task for animations
        /// </summary>
        public BackgroundWorker AnimWork;

        /// <summary>
        /// Animation step
        /// </summary>
        public Steps AnimStep;
    }

    #endregion Editor auxiliary stuff

    #region Library auxiliary stuff

    /// <summary>
    /// Class for root data from a library file
    /// </summary>
    public class LibraryRootData
    {
        /// <summary>
        /// Algorithm name
        /// </summary>
        public string Name;

        /// <summary>
        /// Algorithm description
        /// </summary>
        public string Description;

        /// <summary>
        /// Flag to know if the library has unsaved changes
        /// </summary>
        public bool IsModified;
    }

    /// <summary>
    /// Class for folders data from a library file
    /// </summary>
    public class LibraryFolderData
    {
        /// <summary>
        /// Algorithm name
        /// </summary>
        public string Name;

        /// <summary>
        /// Algorithm description
        /// </summary>
        public string Description;
    }

    /// <summary>
    /// Class for algorithms from a library file
    /// </summary>
    public class LibraryAlgorithmData
    {
        /// <summary>
        /// Algorithm name
        /// </summary>
        public string Name;

        /// <summary>
        /// Algorithm description
        /// </summary>
        public string Description;

        /// <summary>
        /// Initial scramble in string format
        /// </summary>
        public string InitScramble;

        /// <summary>
        /// Algorithm in string format
        /// </summary>
        public string Algorithm;

        /// <summary>
        /// Neutral mask in string format
        /// </summary>
        public string NeutralMask;

        /// <summary>
        /// Default view for start position: 3, 2, U, D, F, B, R, L
        /// </summary>
        private char SView;

        /// <summary>
        /// Default view for end position: 3, 2, U, D, F, B, R, L
        /// </summary>
        private char EView;

        /// <summary>
        /// Angles for 3D camera views
        /// </summary>
        public double S3DViewAlpha, S3DViewBeta, E3DViewAlpha, E3DViewBeta;

        /// <summary>
        /// Draw arrows in scramble 2D views
        /// </summary>
        public bool DrawArrows;

        /// <summary>
        /// Angles in int format
        /// </summary>
        public int SAlphaInt { get { return (int)Math.Round(S3DViewAlpha); } }

        /// <summary>
        /// Angles in int format
        /// </summary>
        public int SBetaInt { get { return (int)Math.Round(S3DViewBeta); } }

        /// <summary>
        /// Angles in int format
        /// </summary>
        public int EAlphaInt { get { return (int)Math.Round(E3DViewAlpha); } }

        /// <summary>
        /// Angles in int format
        /// </summary>
        public int EBetaInt { get { return (int)Math.Round(E3DViewBeta); } }

        /// <summary>
        /// Default view for start position: 3, 2, U, D, F, B, R, L
        /// </summary>
        public char StartView
        {
            get { return SView; }
            set
            {
                if (value != '3' && value != '2' && value != 'U' && value != 'D' &&
                    value != 'F' && value != 'B' && value != 'R' && value != 'L') SView = '2';
                else SView = value;
            }
        }

        /// <summary>
        /// Default view for end position: 3, 2, U, D, F, B, R, L
        /// </summary>
        public char EndView
        {
            get { return EView; }
            set
            {
                if (value != '3' && value != '2' && value != 'U' && value != 'D' &&
                    value != 'F' && value != 'B' && value != 'R' && value != 'L') EView = '3';
                else EView = value;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="AlgNum">Number of algorithm id in the library</param>
        public LibraryAlgorithmData()
        {
            Name = string.Empty;
            Description = string.Empty;
            InitScramble = string.Empty;
            Algorithm = string.Empty;
            NeutralMask = string.Empty;
            SView = '2';
            EView = '3';
            S3DViewAlpha = E3DViewAlpha = AMSettings.CameraAlpha;
            S3DViewBeta = E3DViewBeta = AMSettings.CameraBeta;
            DrawArrows = false;
        }
    }

    /// <summary>
    /// Structure to group library fields
    /// </summary>
    public struct LibraryFields
    {
        /// <summary>
        /// Cube for initial scramble
        /// </summary>
        public Cube3D InitScrambleCube;

        /// <summary>
        /// Cube for algorithm
        /// </summary>
        public Cube3D AlgorithmCube;

        /// <summary>
        /// Scramble for initial position
        /// </summary>
        public Scramble InitScramble;

        /// <summary>
        /// Scramble for algorithm with developed parentheses
        /// </summary>
        public Scramble Algorithm;

        /// <summary>
        /// Scramble for algorithm
        /// </summary>
        public Scramble AlgorithmBase;

        /// <summary>
        /// Node in the library tree of current algorithm
        /// </summary>
        public TreeViewItem CurrentAlgorithm;

        /// <summary>
        /// Node in the library tree of current folder
        /// </summary>
        public TreeViewItem CurrentFolder;

        /// <summary>
        /// Node in the library tree of current library
        /// </summary>
        public TreeViewItem CurrentLibrary;

        /// <summary>
        /// Current step position in the algorithm
        /// </summary>
        public int AlgorithmPosition;

        /// <summary>
        /// Target position for algorithm animation
        /// </summary>
        public int AlgorithmPosTarget;

        /// <summary>
        /// Buttons array for algorithm
        /// </summary>
        public Button[] AlgorithmButtons;

        /// <summary>
        /// Flag to know if the 3D scramble cube is being oriented
        /// </summary>
        public bool MovingScrambleCube;

        /// <summary>
        /// Flag to know if the 3D algorithm cube is being oriented
        /// </summary>
        public bool MovingAlgorithmCube;

        /// <summary>
        /// Array of buttons for the scramble 2D cube skin mosaic
        /// </summary>
        public Button[] InitPositionStickerButtons;

        /// <summary>
        /// Array of buttons for the algorithm 2D cube skin mosaic
        /// </summary>
        public Button[] AlgorithmStickerButtons;

        /// <summary>
        /// Copy of the source algorithm for copy / paste
        /// </summary>
        public LibraryAlgorithmData AlgorithmCopied;

        /// <summary>
        /// Background task for animations
        /// </summary>
        public BackgroundWorker AnimWork;

        /// <summary>
        /// Rectangles for initial position layers 2D views
        /// </summary>
        public Rectangle[][] InitPosLayersRects; // One array per face

        /// <summary>
        /// Rectangles for algorithm layers 2D views
        /// </summary>
        public Rectangle[][] AlgorithmLayersRects; // One array per face

        /// <summary>
        /// Flag to know if current algorithm can be edited
        /// </summary>
        public bool AlgorithmEditionEnabled;
    }

    #endregion Library auxiliary stuff

    #endregion Enums, auxiliary classes & structs

    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Main window fields

        /// <summary>
        /// Flag to know when the application is initialized
        /// </summary>
        private bool AppInitialized = false;

        /// <summary>
        /// Dictionary of TextBlocks with texts to be translated
        /// </summary>
        private Dictionary<string, object> Translate;
		
		/// <summary>
        /// Flags to lock main tabs
        /// </summary>
		private bool LockChronoTabFlag, LockEditorTabFlag, LockLibraryTabFlag, LockSettingsTabFlag, LockProgressTabFlag;

        /// <summary>
        /// Struct data for chrono tool
        /// </summary>
        private ChronoFields ChronoData;

        /// <summary>
        /// Struct data for editor tool
        /// </summary>
        private EditorFields EditorData;

        /// <summary>
        /// Struct data for library tool
        /// </summary>
        private LibraryFields LibraryData;

        /// <summary>
        /// Window for validate a result
        /// </summary>
        private ResultWindow ResultChronoWindow;

        /// <summary>
        /// Chars array for 2D view tabs identification
        /// </summary>
        private static char[] LibraryTabChars = { '3', '2', 'U', 'D', 'F', 'B', 'R', 'L' };

        /// <summary>
        /// Progress chart data list
        /// </summary>
        private List<ProgressData> ChartData;

        /// <summary>
        /// SoundPlayer for ready beep
        /// </summary>
        private System.Media.SoundPlayer BeepReadyPlayer;

        /// <summary>
        /// SoundPlayer for start beep
        /// </summary>
        private System.Media.SoundPlayer BeepStartPlayer;

        /// <summary>
        /// SoundPlayer for 8 seconds beep
        /// </summary>
        private System.Media.SoundPlayer Beep8sPlayer;

        /// <summary>
        /// SoundPlayer for 12 seconds beep
        /// </summary>
        private System.Media.SoundPlayer Beep12sPlayer;

        /// <summary>
        /// SoundPlayer for 15 seconds beep
        /// </summary>
        private System.Media.SoundPlayer Beep15sPlayer;

        /// <summary>
        /// SoundPlayer for DNF beep
        /// </summary>
        private System.Media.SoundPlayer BeepDNFPlayer;

        /// <summary>
        /// SoundPlayer for end beep
        /// </summary>
        private System.Media.SoundPlayer BeepEndPlayer;

        #endregion Main window fields

        #region Main window properties

        /// <summary>
        /// Get current tab identifier
        /// </summary>
        public MainTabNames CurrentTab
        {
            get
            {
                switch (MainTab.SelectedIndex)
                {
                    case 0: return MainTabNames.Chrono;
                    case 1: return MainTabNames.Progress;
                    case 2: return MainTabNames.Editor;
                    case 3: return MainTabNames.Library;
                    default: return MainTabNames.Settings;
                }
            }
        }

        #endregion Main window properties

        #region Main window constructor

        /// <summary>
        /// Main window constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent(); // Auto-generated function, not edit.

            AMSettings.PortableApp = false;
            AMSettings.LogEnabled = true;
            AMSettings.ChronoLoadAtStart = false; // Not used
            AMSettings.ChronoSaveTimes = true; // Disable only for testing

            AMSettings.VideoFrameRate = 24f; // 24 fps
            AMSettings.VideoQuality = 80; // 80%

            bool FirstRun = !AMSettings.PortableApp && !Directory.Exists(AMSettings.SolvesFolder);

            try { Directory.CreateDirectory(AMSettings.DataFolder); }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Fault creating data directory..."); }

            try { Directory.CreateDirectory(AMSettings.SettingsFolder); }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Fault creating settings directory..."); }

            try { Directory.CreateDirectory(AMSettings.LangFolder); }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Fault creating language files directory..."); }

            try { Directory.CreateDirectory(AMSettings.SolvesFolder); }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Fault creating solves directory..."); }

            try { Directory.CreateDirectory(AMSettings.LibsFolder); }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Fault creating libraries directory..."); }

            try { Directory.CreateDirectory(AMSettings.TempFolder); }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Fault creating temporary directory..."); }

            if (FirstRun)
            {
                DirectoryInfo diAppData;

                diAppData = new DirectoryInfo(System.IO.Path.Combine(AMSettings.AppFolder, "AppData"));
                try // Copy settings files
                {
                    FileInfo[] fiSettings = diAppData.GetFiles("*" + AMSettings.SettingsExt);
                    foreach (FileInfo fi in fiSettings)
                        File.Copy(fi.FullName, System.IO.Path.Combine(AMSettings.SettingsFolder, fi.Name));
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Fault copying default settings file..."); }

                diAppData = new DirectoryInfo(System.IO.Path.Combine(AMSettings.AppFolder, "AppData", "libs"));
                try // Copy library files
                {
                    FileInfo[] fiLibs = diAppData.GetFiles("*" + AMSettings.LibsExt);
                    foreach (FileInfo fi in fiLibs)
                        File.Copy(fi.FullName, System.IO.Path.Combine(AMSettings.LibsFolder, fi.Name));
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Fault copying library files..."); }

                diAppData = new DirectoryInfo(System.IO.Path.Combine(AMSettings.AppFolder, "AppData", "lan"));
                try // Copy language files
                {
                    FileInfo[] fiLangs = diAppData.GetFiles("*" + AMSettings.LangExt);
                    foreach (FileInfo fi in fiLangs)
                        File.Copy(fi.FullName, System.IO.Path.Combine(AMSettings.LangFolder, fi.Name));
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Fault copying language files..."); }
            }

            try // Make back up of solves data base
            {
                if (File.Exists(AMSettings.SolvesDBBack2))
                {
                    File.Delete(AMSettings.SolvesDBBack3);
                    File.Copy(AMSettings.SolvesDBBack2, AMSettings.SolvesDBBack3);
                }
                if (File.Exists(AMSettings.SolvesDBBack1))
                {
                    File.Delete(AMSettings.SolvesDBBack2);
                    File.Copy(AMSettings.SolvesDBBack1, AMSettings.SolvesDBBack2);
                }
                if (File.Exists(AMSettings.SolvesDBPath))
                {
                    File.Delete(AMSettings.SolvesDBBack1);
                    File.Copy(AMSettings.SolvesDBPath, AMSettings.SolvesDBBack1);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Fault making solves data base backup..."); }

            AMSettings.SetStatus = StatusTextBlock; // Set the reference to the status TextBlock

            try { File.Delete(AMSettings.LogFile); } // Delete old log
            catch(Exception ex) { MessageBox.Show(ex.Message, "Delete log file failed: "); }

            if (FirstRun) AMSettings.Log = "New installation detected: default settings, language files and libraries copied to user data folder.";

            SetTranslate(); // Set the main window texts susceptible to being translated
            AMTexts.AddToSystemTexts(Translate); // Add main window texts to system texts
			
			TranslationTool TransWindow = new TranslationTool(); // Create translation window
            TransWindow.Close(); // Close translation tool window after getting his translations texts references

            ResultChronoWindow = new ResultWindow(); // Create result window
            ResultChronoWindow.Close(); // Close result tool window after getting his translations texts references

            LibraryTreeViewElement NewLibraryWindow = new LibraryTreeViewElement();  // Create new edit library window
            NewLibraryWindow.Close(); // Close window after getting his translations texts references

            SolveEditWindow EditSolvesWindow = new SolveEditWindow();  // Create new edit solves window
            EditSolvesWindow.Close(); // Close window after getting his translations texts references

            AMTexts.CopySystemTextsToCurrent(); // System texts are current texts in startup
            AMTexts.SaveCurrentTexts("default"); // Save system texts as application default texts

            try
            {
                if (File.Exists(AMSettings.BeepReadyFile))
                {
                    BeepReadyPlayer = new System.Media.SoundPlayer(AMSettings.BeepReadyFile);
                    BeepReadyPlayer.Load();
                }
                else AMSettings.Log = "No ready beep sound file found: " + AMSettings.BeepReadyFile;

                if (File.Exists(AMSettings.BeepStartFile))
                {
                    BeepStartPlayer = new System.Media.SoundPlayer(AMSettings.BeepStartFile);
                    BeepStartPlayer.Load();
                }
                else AMSettings.Log = "No start beep sound file found: " + AMSettings.BeepStartFile;

                if (File.Exists(AMSettings.Beep8sFile))
                {
                    Beep8sPlayer = new System.Media.SoundPlayer(AMSettings.Beep8sFile);
                    Beep8sPlayer.Load();
                }
                else AMSettings.Log = "No 8 seconds beep sound file found: " + AMSettings.Beep8sFile;

                if (File.Exists(AMSettings.Beep12sFile))
                {
                    Beep12sPlayer = new System.Media.SoundPlayer(AMSettings.Beep12sFile);
                    Beep12sPlayer.Load();
                }
                else AMSettings.Log = "No 12 seconds beep sound file found: " + AMSettings.Beep12sFile;

                if (File.Exists(AMSettings.Beep15sFile))
                {
                    Beep15sPlayer = new System.Media.SoundPlayer(AMSettings.Beep15sFile);
                    Beep15sPlayer.Load();
                }
                else AMSettings.Log = "No 15 seconds beep sound file found: " + AMSettings.Beep15sFile;

                if (File.Exists(AMSettings.BeepDNFFile))
                {
                    BeepDNFPlayer = new System.Media.SoundPlayer(AMSettings.BeepDNFFile);
                    BeepDNFPlayer.Load();
                }
                else AMSettings.Log = "No DNF beep sound file found: " + AMSettings.BeepDNFFile;

                if (File.Exists(AMSettings.BeepEndFile))
                {
                    BeepEndPlayer = new System.Media.SoundPlayer(AMSettings.BeepEndFile);
                    BeepEndPlayer.Load();
                }
                else AMSettings.Log = "No end beep sound file found: " + AMSettings.BeepEndFile;
            }
            catch (Exception ex) { AMSettings.Log = "Fault loading sound files: " + ex.Message; }

            if (!AMSettings.LoadXML(AMSettings.DefaultSettingsFile)) // load default settings
                AMSettings.Status = AMTexts.Message("DefaultSettingsFileNotFoundMessage") + AMSettings.DefaultSettingsFile;

            UpdateSettingsFiles();
            UpdateLanguageFiles();

            InitializeChrono();
            InitializeProgress();
			InitializeEditor();
            InitializeLibrary();

            UpdateTexts();
            UpdateAllViews();
			
			LockChronoTabFlag = LockEditorTabFlag = LockLibraryTabFlag = LockSettingsTabFlag = LockProgressTabFlag = false;

            AMSettings.Status = AMTexts.Message("StartUpMessage"); // Startup message in status line

            AppInitialized = true;

            if (FirstRun) new ReadmeWindow().Show();
        }

        #endregion Main window constructor

        #region Main window general functions

        /// <summary>
        /// Set the dictionary of main window TextBlocks that contain texts susceptible to being translated
        /// </summary>
        private void SetTranslate()
        {
            Translate = new Dictionary<string, object>
            {
				// Main window texts
                { "ChronoTabTitle", ChronoTabTitle },
                { "ChronoTabToolTipHeader", ChronoTabToolTipHeader },
                { "ChronoTabToolTipBody", ChronoTabToolTipBody },
                { "EditorTabTitle", EditorTabTitle },
                { "EditorTabToolTipHeader", EditorTabToolTipHeader },
                { "EditorTabToolTipBody", EditorTabToolTipBody },
                { "LibraryTabTitle", LibraryTabTitle },
                { "LibraryTabToolTipHeader", LibraryTabToolTipHeader },
                { "LibraryTabToolTipBody", LibraryTabToolTipBody },
                { "SettingsTabTitle", SettingsTabTitle },
                { "SettingsTabToolTipHeader", SettingsTabToolTipHeader },
                { "SettingsTabToolTipBody", SettingsTabToolTipBody },

				// Chronometer tab texts
                { "ChronoScrambleTitleText", ChronoScrambleTitleText },

                { "Chrono3DStartToolTipHeader", Chrono3DStartToolTipHeader },
                { "Chrono3DStartToolTipBody", Chrono3DStartToolTipBody },
                { "Chrono3DPreviousToolTipHeader", Chrono3DPreviousToolTipHeader },
                { "Chrono3DPreviousToolTipBody", Chrono3DPreviousToolTipBody },
                { "Chrono3DPlayToolTipHeader", Chrono3DPlayToolTipHeader },
                { "Chrono3DPlayToolTipBody", Chrono3DPlayToolTipBody },
                { "Chrono3DNextToolTipHeader", Chrono3DNextToolTipHeader },
                { "Chrono3DNextToolTipBody", Chrono3DNextToolTipBody },
                { "Chrono3DEndToolTipHeader", Chrono3DEndToolTipHeader },
                { "Chrono3DEndToolTipBody", Chrono3DEndToolTipBody },

                { "ChronoPlusStepsButton", ChronoPlusStepsButton },
                { "ChronoPlusStepsToolTipHeader", ChronoPlusStepsToolTipHeader },
                { "ChronoPlusStepToolTipBody", ChronoPlusStepToolTipBody },
                { "ChronoMinusStepsButton", ChronoMinusStepsButton },
                { "ChronoMinusStepsToolTipHeader", ChronoMinusStepsToolTipHeader },
                { "ChronoMinusStepToolTipBody", ChronoMinusStepToolTipBody },
                { "ChronoNewRandomText", ChronoNewRandomText },
                { "ChronoNewRandomToolTipHeader", ChronoNewRandomToolTipHeader },
                { "ChronoNewRandomToolTipBody", ChronoNewRandomToolTipBody },
                { "ChronoFromClipboardText", ChronoFromClipboardText },
                { "ChronoFromClipboardToolTipHeader", ChronoFromClipboardToolTipHeader },
                { "ChronoFromClipboardToolTipBody", ChronoFromClipboardToolTipBody },
                { "ChronoNoScrambleText", ChronoNoScrambleText },
                { "ChronoNoScrambleToolTipHeader", ChronoNoScrambleToolTipHeader },
                { "ChronoNoScrambleToolTipBody", ChronoNoScrambleToolTipBody },
                { "ChronoCopyScrambleText", ChronoCopyScrambleText },
                { "ChronoCopyScrambleToolTipHeader", ChronoCopyScrambleToolTipHeader },
                { "ChronoCopyScrambleToolTipBody", ChronoCopyScrambleToolTipBody },

                { "Chrono3DZoomText", Chrono3DZoomText },
                { "Chrono3DZoomPlusText", Chrono3DZoomPlusText },
                { "Chrono3DZoomPlusToolTipHeader", Chrono3DZoomPlusToolTipHeader },
                { "Chrono3DZoomPlusToolTipBody", Chrono3DZoomPlusToolTipBody },
                { "Chrono3DZoomMinusText", Chrono3DZoomMinusText },
                { "Chrono3DZoomMinusToolTipHeader", Chrono3DZoomMinusToolTipHeader },
                { "Chrono3DZoomMinusToolTipBody", Chrono3DZoomMinusToolTipBody },

                { "Chrono2DViewText", Chrono2DViewText },

                { "ChronoChronometerText", ChronoChronometerText },

                { "ChronoSolvingTypeText", ChronoSolvingTypeText },
                { "ChronoSolvingTypeComboToolTipHeader", ChronoSolvingTypeComboToolTipHeader },
                { "ChronoSolvingTypeComboToolTipBody", ChronoSolvingTypeComboToolTipBody },
                { "ChronoInspectionCheckBoxText", ChronoInspectionCheckBoxText },
                { "ChronoInspectionToolTipHeader", ChronoInspectionToolTipHeader },
                { "ChronoInspectionToolTipBody", ChronoInspectionToolTipBody },

                { "ChronoStadisticsText", ChronoStadisticsText },
                { "ChronoTypeStatsComboToolTipHeader", ChronoTypeStatsComboToolTipHeader },
                { "ChronoTypeStatsComboToolTipBody", ChronoTypeStatsComboToolTipBody },
                { "BestTimeLabelTitle", BestTimeLabelTitle },
                { "WorstTimeLabelTitle", WorstTimeLabelTitle },
                { "AverageTimeLabelTitle", AverageTimeLabelTitle },
                { "MediumTimeLabelTitle", MediumTimeLabelTitle },
                { "StandardDeviationLabelTitle", StandardDeviationLabelTitle },
                { "WCAAverageLabelText", WCAAverageLabelText },

                { "ChronoResultsText", ChronoResultsText },
                { "LoadTodayTimesButtonText", LoadTodayTimesButtonText },
                { "ChronoLoadTodayTimesToolTipHeader", ChronoLoadTodayTimesToolTipHeader },
                { "ChronoLoadTodayTimesToolTipBody", ChronoLoadTodayTimesToolTipBody },
                { "ChronoLoadDateTimesPickerToolTipHeader", ChronoLoadDateTimesPickerToolTipHeader },
                { "ChronoLoadDateTimesPickerToolTipBody", ChronoLoadDateTimesPickerToolTipBody },
                { "ClearTimesTableButtonText", ClearTimesTableButtonText },
                { "ChronoClearTimesTableToolTipHeader", ChronoClearTimesTableToolTipHeader },
                { "ChronoClearTimesTableToolTipBody", ChronoClearTimesTableToolTipBody },
                { "ChronoToolsMenuItemHeaderText", ChronoToolsMenuItemHeaderText },
                { "ChronoExportTableCSVMenuItemHeaderText", ChronoExportTableCSVMenuItemHeaderText },
                { "ChronoExportAllCSVMenuItemHeaderText", ChronoExportAllCSVMenuItemHeaderText },
				{ "ChronoToolEditSolveMenuItemHeaderText", ChronoToolEditSolveMenuItemHeaderText },
                { "ChronoToolDeleteSolveMenuItemHeaderText", ChronoToolDeleteSolveMenuItemHeaderText },

                { "View3DTabTitle", View3DTabTitle },
                { "View3DTabToolTipHeader", View3DTabToolTipHeader },
                { "View3DTabToolTipBody", View3DTabToolTipBody },
                { "CameraTabTitle", CameraTabTitle },
                { "CameraTabToolTipHeader", CameraTabToolTipHeader },
                { "CameraTabToolTipBody", CameraTabToolTipBody },
                { "CamerasComboBoxToolTipHeader", CamerasComboBoxToolTipHeader },
                { "CamerasComboBoxToolTipBody", CamerasComboBoxToolTipBody },
                { "CameraPlayToolTipHeader", CameraPlayToolTipHeader },
                { "CameraPlayToolTipBody", CameraPlayToolTipBody },
                { "CameraStopToolTipHeader", CameraStopToolTipHeader },
                { "CameraStopToolTipBody", CameraStopToolTipBody },
                { "SaveVideoCheckBoxText", SaveVideoCheckBoxText },
                { "SaveVideoCheckBoxToolTipHeader", SaveVideoCheckBoxToolTipHeader },
                { "SaveVideoCheckBoxToolTipBody", SaveVideoCheckBoxToolTipBody },
                { "CamImageToolTipHeader", CamImageToolTipHeader },
                { "CamImageToolTipBody", CamImageToolTipBody },
                
                // Progress chart texts
                
                { "ProgressTabTitle", ProgressTabTitle },
                { "ProgressTabToolTipHeader", ProgressTabToolTipHeader },
                { "ProgressTabToolTipBody", ProgressTabToolTipBody },

                /*
                // Legend texts are inside a canvas, declared as messages
                { "ProgressMaximumCheckBoxText", ProgressMaximumCheckBoxText },
                { "ProgressMaximumCheckBoxToolTipHeader", ProgressMaximumCheckBoxToolTipHeader },
                { "ProgressMaximumCheckBoxToolTipBody", ProgressMaximumCheckBoxToolTipBody },
                { "ProgressMinimumCheckBoxText", ProgressMinimumCheckBoxText },
                { "ProgressMinimumCheckBoxToolTipHeader", ProgressMinimumCheckBoxToolTipHeader },
                { "ProgressMinimumCheckBoxToolTipBody", ProgressMinimumCheckBoxToolTipBody },
                { "ProgressAverageCheckBoxText", ProgressAverageCheckBoxText },
                { "ProgressAverageCheckBoxToolTipHeader", ProgressAverageCheckBoxToolTipHeader },
                { "ProgressAverageCheckBoxToolTipBody", ProgressAverageCheckBoxToolTipBody },
                { "ProgressDeviationCheckBoxText", ProgressDeviationCheckBoxText },
                { "ProgressDeviationCheckBoxToolTipHeader", ProgressDeviationCheckBoxToolTipHeader },
                { "ProgressDeviationCheckBoxToolTipBody", ProgressDeviationCheckBoxToolTipBody },
                { "ProgressMediumCheckBoxText", ProgressMediumCheckBoxText },
                { "ProgressMediumCheckBoxToolTipHeader", ProgressMediumCheckBoxToolTipHeader },
                { "ProgressMediumCheckBoxToolTipBody", ProgressMediumCheckBoxToolTipBody },
                { "ProgressBarsCheckBoxText", "ProgressBarsCheckBoxText" },
                { "ProgressBarsCheckBoxToolTipHeader", "ProgressBarsCheckBoxToolTipHeader" },
                { "ProgressBarsCheckBoxToolTipBody", "ProgressBarsCheckBoxToolTipBody" },
                */
                { "ProgressFromLabelText", ProgressFromLabelText },
                { "ProgressStartDateTimesPickerToolTipHeader", ProgressStartDateTimesPickerToolTipHeader },
                { "ProgressStartDateTimesPickerToolTipBody", ProgressStartDateTimesPickerToolTipBody },
                { "ProgressToLabelText", ProgressToLabelText },
                { "ProgressEndDateTimesPickerToolTipHeader", ProgressEndDateTimesPickerToolTipHeader },
                { "ProgressEndDateTimesPickerToolTipBody", ProgressEndDateTimesPickerToolTipBody },
                { "ProgressPeriodLabelText", ProgressPeriodLabelText },
                { "ProgressPeriodComboToolTipHeader", ProgressPeriodComboToolTipHeader },
                { "ProgressPeriodComboToolTipBody", ProgressPeriodComboToolTipBody },
                { "ProgressTypeLabelText", ProgressTypeLabelText },
                { "ProgressSolvingTypeComboToolTipHeader", ProgressSolvingTypeComboToolTipHeader },
                { "ProgressSolvingTypeComboToolTipBody", ProgressSolvingTypeComboToolTipBody },
                { "ProgressShowLegendCheckBoxText", ProgressShowLegendCheckBoxText },
                { "ProgressShowLegendToolTipHeader", ProgressShowLegendToolTipHeader },
                { "ProgressShowLegendToolTipBody", ProgressShowLegendToolTipBody },

                { "ProgressStartPreviousMonthToolTipHeader", ProgressStartPreviousMonthToolTipHeader },
                { "ProgressStartPreviousMonthToolTipBody", ProgressStartPreviousMonthToolTipBody },
                { "ProgressStartPreviousDayToolTipHeader", ProgressStartPreviousDayToolTipHeader },
                { "ProgressStartPreviousDayToolTipBody", ProgressStartPreviousDayToolTipBody },
                { "ProgressStartNextDayToolTipHeader", ProgressStartNextDayToolTipHeader },
                { "ProgressStartNextDayToolTipBody", ProgressStartNextDayToolTipBody },
                { "ProgressStartNextMonthToolTipHeader", ProgressStartNextMonthToolTipHeader },
                { "ProgressStartNextMonthToolTipBody", ProgressStartNextMonthToolTipBody },
                { "ProgressEndPreviousMonthToolTipHeader", ProgressEndPreviousMonthToolTipHeader },
                { "ProgressEndPreviousMonthToolTipBody", ProgressEndPreviousMonthToolTipBody },
                { "ProgressEndPreviousDayToolTipHeader", ProgressEndPreviousDayToolTipHeader },
                { "ProgressEndPreviousDayToolTipBody", ProgressEndPreviousDayToolTipBody },
                { "ProgressEndNextDayToolTipHeader", ProgressEndNextDayToolTipHeader },
                { "ProgressEndNextDayToolTipBody", ProgressEndNextDayToolTipBody },
                { "ProgressEndNextMonthToolTipHeader", ProgressEndNextMonthToolTipHeader },
                { "ProgressEndNextMonthToolTipBody", ProgressEndNextMonthToolTipBody },

				// Editor tab texts
				{ "EditorScrambleLabelText", EditorScrambleLabelText },
				{ "EditorUndoToolTipHeader", EditorUndoToolTipHeader },
				{ "EditorUndoToolTipBody", EditorUndoToolTipBody },
				{ "EditorRedoToolTipHeader", EditorRedoToolTipHeader },
				{ "EditorRedoToolTipBody", EditorRedoToolTipBody },
				{ "EditorClearScrambleButtonText", EditorClearScrambleButtonText },
				{ "EditorClearScrambleToolTipHeader", EditorClearScrambleToolTipHeader },
				{ "EditorClearScrambleToolTipBody", EditorClearScrambleToolTipBody },

				{ "Editor3DViewLabelText", Editor3DViewLabelText },
				{ "Editor3DZoomPlusButtonText", Editor3DZoomPlusButtonText },
				{ "Editor3DZoomPlusToolTipHeader", Editor3DZoomPlusToolTipHeader },
				{ "Editor3DZoomPlusToolTipBody", Editor3DZoomPlusToolTipBody },
				{ "Editor3DZoomMinusButtonText", Editor3DZoomMinusButtonText },
				{ "Editor3DZoomMinusToolTipHeader", Editor3DZoomMinusToolTipHeader },
				{ "Editor3DZoomMinusToolTipBody", Editor3DZoomMinusToolTipBody },
				
				{ "Editor2DViewLabelText", Editor2DViewLabelText },
				{ "EditorNeutralComboToolTipHeader", EditorNeutralComboToolTipHeader },
				{ "EditorNeutralComboToolTipBody", EditorNeutralComboToolTipBody },
				{ "EditorNeutralCBStickerItem", EditorNeutralCBStickerItem },
				{ "EditorNeutralCBPieceItem", EditorNeutralCBPieceItem },
				{ "EditorNeutralCBFaceItem", EditorNeutralCBFaceItem },
				{ "EditorClearNeutralButtonText", EditorClearNeutralButtonText },
				{ "EditorClearNeutralToolTipHeader", EditorClearNeutralToolTipHeader },
				{ "EditorClearNeutralToolTipBody", EditorClearNeutralToolTipBody },
				{ "EditorInvertNeutralButtonText", EditorInvertNeutralButtonText },
				{ "EditorInvertNeutralToolTipHeader", EditorInvertNeutralToolTipHeader },
				{ "EditorInvertNeutralToolTipBody", EditorInvertNeutralToolTipBody },
				
				{ "EditorModifiersLabelText", EditorModifiersLabelText },
				{ "EditorModifierComboToolTipHeader", EditorModifierComboToolTipHeader },
				{ "EditorModifierComboToolTipBody", EditorModifierComboToolTipBody },
				{ "EditorMovementsLabelText", EditorMovementsLabelText },
				{ "EditorScrambleSimplifyLabelText", EditorScrambleSimplifyLabelText },
				{ "ConvertToBasicStepsButtonText", ConvertToBasicStepsButtonText },
				{ "EditorConvertToBasicToolTipHeader", EditorConvertToBasicToolTipHeader },
				{ "EditorConvertToBasicToolTipBody", EditorConvertToBasicToolTipBody },
				{ "ShrinkStepsButtonText", ShrinkStepsButtonText },
				{ "EditorShrinkStepsToolTipHeader", EditorShrinkStepsToolTipHeader },
				{ "EditorShrinkStepsToolTipBody", EditorShrinkStepsToolTipBody },
				{ "EditorSimplifyStepsButtonText", EditorSimplifyStepsButtonText },
				{ "EditorSimplifyStepsToolTipHeader", EditorSimplifyStepsToolTipHeader },
				{ "EditorSimplifyStepsToolTipBody", EditorSimplifyStepsToolTipBody },
				
				{ "InnerLayersMovementsLabelText", InnerLayersMovementsLabelText },
				{ "FullCubeTurnsLabelText", FullCubeTurnsLabelText },
				{ "DevelopAllturnsButtonText", DevelopAllturnsButtonText },
                { "EditorDevelopAllturnsToolTipHeader", EditorDevelopAllturnsToolTipHeader },
                { "EditorDevelopAllturnsToolTipBody", EditorDevelopAllturnsToolTipBody },
                { "ParenthesesLabelText", ParenthesesLabelText },
				{ "EditorOpenParenthesisToolTipHeader", EditorOpenParenthesisToolTipHeader },
				{ "EditorOpenParenthesisToolTipBody", EditorOpenParenthesisToolTipBody },
				{ "EditorCloseParenthesisToolTipHeader", EditorCloseParenthesisToolTipHeader },
				{ "EditorCloseParenthesisToolTipBody", EditorCloseParenthesisToolTipBody },
				{ "EditorRepetitionsComboToolTipHeader", EditorRepetitionsComboToolTipHeader },
				{ "EditorRepetitionsComboToolTipBody", EditorRepetitionsComboToolTipBody },
				{ "RemoveParenthesesButtonText", RemoveParenthesesButtonText },
				{ "EditorRemoveParenthesesToolTipHeader", EditorRemoveParenthesesToolTipHeader },
				{ "EditorRemoveParenthesesToolTipBody", EditorRemoveParenthesesToolTipBody },
				{ "DevelopParenthesesButtonText", DevelopParenthesesButtonText },
				{ "EditorDevelopParenthesesToolTipHeader", EditorDevelopParenthesesToolTipHeader },
				{ "EditorDevelopParenthesesToolTipBody", EditorDevelopParenthesesToolTipBody },
				
				{ "ScrambleControlLabelText", ScrambleControlLabelText },
				{ "EditorReadFromClipboardButtonText", EditorReadFromClipboardButtonText },
				{ "EditorReadFromClipboardToolTipHeader", EditorReadFromClipboardToolTipHeader },
				{ "EditorReadFromClipboardToolTipBody", EditorReadFromClipboardToolTipBody },
				{ "EditorCopyToClipboardButtonText", EditorCopyToClipboardButtonText },
				{ "EditorCopyToClipboardToolTipHeader", EditorCopyToClipboardToolTipHeader },
				{ "EditorCopyToClipboardToolTipBody", EditorCopyToClipboardToolTipBody },
				{ "EditorClearLastStepButtonText", EditorClearLastStepButtonText },
				{ "EditorClearLastStepToolTipHeader", EditorClearLastStepToolTipHeader },
				{ "EditorClearLastStepToolTipBody", EditorClearLastStepToolTipBody },
				{ "EditorClearSelectionButtonText", EditorClearSelectionButtonText },
				{ "EditorClearSelectionToolTipHeader", EditorClearSelectionToolTipHeader },
				{ "EditorClearSelectionToolTipBody", EditorClearSelectionToolTipBody },
				{ "ScrambleOperationLabelText", ScrambleOperationLabelText },
				{ "EditorInvertStepsOrderButtonText", EditorInvertStepsOrderButtonText },
				{ "EditorInvertStepsOrderToolTipHeader", EditorInvertStepsOrderToolTipHeader },
				{ "EditorInvertStepsOrderToolTipBody", EditorInvertStepsOrderToolTipBody },
				{ "EditorInvertMovementsButtonText", EditorInvertMovementsButtonText },
				{ "EditorInvertMovementsToolTipHeader", EditorInvertMovementsToolTipHeader },
				{ "EditorInvertMovementsToolTipBody", EditorInvertMovementsToolTipBody },
				{ "EditorInvertStepsAllButtonText", EditorInvertStepsAllButtonText },
				{ "EditorInvertStepsAllToolTipHeader", EditorInvertStepsAllToolTipHeader },
				{ "EditorInvertStepsAllToolTipBody", EditorInvertStepsAllToolTipBody },
				{ "ScrambleFromTextLabelText", ScrambleFromTextLabelText },
				{ "EditorScrambleFromTextToolTipHeader", EditorScrambleFromTextToolTipHeader },
				{ "EditorScrambleFromTextToolTipBody", EditorScrambleFromTextToolTipBody },
				{ "EditorNewFromTextButtonText", EditorNewFromTextButtonText },
				{ "EditorNewFromTextToolTipHeader", EditorNewFromTextToolTipHeader },
				{ "EditorNewFromTextToolTipBody", EditorNewFromTextToolTipBody },
				{ "EditorAddFromTextButtonText", EditorAddFromTextButtonText },
				{ "EditorAddFromTextToolTipHeader", EditorAddFromTextToolTipHeader },
				{ "EditorAddFromTextToolTipBody", EditorAddFromTextToolTipBody },
				
				// Library tab texts
				{ "LibraryNavigatorLabelText", LibraryNavigatorLabelText },
				{ "LibraryGoUpToolTipHeader", LibraryGoUpToolTipHeader },
				{ "LibraryGoUpToolTipBody", LibraryGoUpToolTipBody },
				{ "LibraryGoDownToolTipHeader", LibraryGoDownToolTipHeader },
				{ "LibraryGoDownToolTipBody", LibraryGoDownToolTipBody },
				{ "NewLibraryButtonText", NewLibraryButtonText },
				{ "LibraryNewToolTipHeader", LibraryNewToolTipHeader },
				{ "LibraryNewToolTipBody", LibraryNewToolTipBody },

                /* TextBlocks included in treeview resources -not accesible yet- TEXTS AS MESSAGES
                 * 
				{ "LibraryAddFolderMenuItemText", LibraryAddFolderMenuItemText },
				{ "LibraryRenameLibraryMenuItemText", LibraryRenameLibraryMenuItemText },
				{ "LibraryEditLibraryDescriptionMenuItemText", LibraryEditLibraryDescriptionMenuItemText },
				{ "LibrarySaveLibraryMenuItemText", LibrarySaveLibraryMenuItemText },
				{ "LibraryDeleteLibraryMenuItemText", LibraryDeleteLibraryMenuItemText },
				{ "LibraryAddAlgorithmMenuItemText", LibraryAddAlgorithmMenuItemText },
				{ "LibraryPasteAlgorithmMenuItemText", LibraryPasteAlgorithmMenuItemText },
				{ "LibraryRenameFolderMenuItemText", LibraryRenameFolderMenuItemText },
				{ "LibraryEditFolderDescriptionMenuItemText", LibraryEditFolderDescriptionMenuItemText },
				{ "LibraryDeleteFolderMenuItemText", LibraryDeleteFolderMenuItemText },
				{ "LibraryOpenAlgorithmMenuItemText", LibraryOpenAlgorithmMenuItemText },
				{ "LibraryRenameAlgorithmMenuItemText", LibraryRenameAlgorithmMenuItemText },
				{ "LibraryEditAlgorithmDescriptionMenuItemText", LibraryEditAlgorithmDescriptionMenuItemText },
				{ "LibraryCopyAlgorithmMenuItemText", LibraryCopyAlgorithmMenuItemText },
				{ "LibraryDeleteAlgorithmMenuItemText", LibraryDeleteAlgorithmMenuItemText },
				*/

				{ "LibraryAlgorithmLabel", LibraryAlgorithmLabel },
				{ "LibraryAlgorithmStartToolTipHeader", LibraryAlgorithmStartToolTipHeader },
				{ "LibraryAlgorithmStartToolTipBody", LibraryAlgorithmStartToolTipBody },
				{ "LibraryAlgorithmPreviousToolTipHeader", LibraryAlgorithmPreviousToolTipHeader },
				{ "LibraryAlgorithmPreviousToolTipBody", LibraryAlgorithmPreviousToolTipBody },
				{ "LibraryAlgorithmPlayToolTipHeader", LibraryAlgorithmPlayToolTipHeader },
				{ "LibraryAlgorithmPlayToolTipBody", LibraryAlgorithmPlayToolTipBody },
				{ "LibraryAlgorithmNextToolTipHeader", LibraryAlgorithmNextToolTipHeader },
				{ "LibraryAlgorithmNextToolTipBody", LibraryAlgorithmNextToolTipBody },
				{ "LibraryAlgorithmEndToolTipHeader", LibraryAlgorithmEndToolTipHeader },
				{ "LibraryAlgorithmEndToolTipBody", LibraryAlgorithmEndToolTipBody },
				{ "LibraryAlgorithmXToolTipHeader", LibraryAlgorithmXToolTipHeader },
				{ "LibraryAlgorithmXToolTipBody", LibraryAlgorithmXToolTipBody },
				{ "LibraryAlgorithmYToolTipHeader", LibraryAlgorithmYToolTipHeader },
				{ "LibraryAlgorithmYToolTipBody", LibraryAlgorithmYToolTipBody },
				{ "LibraryAlgorithmZToolTipHeader", LibraryAlgorithmZToolTipHeader },
				{ "LibraryAlgorithmZToolTipBody", LibraryAlgorithmZToolTipBody },
				{ "LibraryAlgorithmEditText", LibraryAlgorithmEditText },
				{ "LibraryAlgorithmEditToolTipHeader", LibraryAlgorithmEditToolTipHeader },
				{ "LibraryAlgorithmEditToolTipBody", LibraryAlgorithmEditToolTipBody },
				{ "LibraryEditMenuItemText", LibraryEditMenuItemText },
				{ "LibraryMenuEditToolTipHeader", LibraryMenuEditToolTipHeader },
				{ "LibraryMenuEditToolTipBody", LibraryMenuEditToolTipBody },
				{ "LibraryInitPosFromClipboardMenuItemText", LibraryInitPosFromClipboardMenuItemText },
				{ "LibraryInitPosFromEditorMenuItemText", LibraryInitPosFromEditorMenuItemText },
				{ "LibraryGenerateInitPosMenuitemText", LibraryGenerateInitPosMenuitemText },
				{ "LibraryAlgFromClipboardMenuItemText", LibraryAlgFromClipboardMenuItemText },
				{ "LibraryAlgFromEditorMenuItemText", LibraryAlgFromEditorMenuItemText },
				{ "LibraryCopyMenuItemText", LibraryCopyMenuItemText },
				{ "LibraryMenuSendToolTipHeader", LibraryMenuSendToolTipHeader },
				{ "LibraryMenuSendToolTipBody", LibraryMenuSendToolTipBody },
				{ "LibraryInitPosToClipboardMenuItemText", LibraryInitPosToClipboardMenuItemText },
				{ "LibraryInitPosToEditorMenuItemText", LibraryInitPosToEditorMenuItemText },
				{ "LibraryAlgToClipboardMenuItemText", LibraryAlgToClipboardMenuItemText },
				{ "LibraryAlgToEditorMenuItemText", LibraryAlgToEditorMenuItemText },
				{ "LibraryMaskToEditorMenuItemText", LibraryMaskToEditorMenuItemText },
				
				{ "LibraryInitialPositionViewLabelText", LibraryInitialPositionViewLabelText },
				{ "LibraryInitPosShowArrowsCheckBoxText", LibraryInitPosShowArrowsCheckBoxText },
				{ "LibraryInitPosShowArrowsToolTipHeader", LibraryInitPosShowArrowsToolTipHeader },
				{ "LibraryInitPosShowArrowsToolTipBody", LibraryInitPosShowArrowsToolTipBody },
				{ "LibraryInitPos3DTabLabelText", LibraryInitPos3DTabLabelText },
				{ "Library3DInitViewLabelText", Library3DInitViewLabelText },
				{ "LibraryInitView3DZoomPlusButtonText", LibraryInitView3DZoomPlusButtonText },
				{ "LibraryInitView3DZoomPlusToolTipHeader", LibraryInitView3DZoomPlusToolTipHeader },
				{ "LibraryInitView3DZoomPlusToolTipBody", LibraryInitView3DZoomPlusToolTipBody },
				{ "LibraryInitView3DZoomMinusButtonText", LibraryInitView3DZoomMinusButtonText },
				{ "LibraryInitView3DZoomMinusToolTipHeader", LibraryInitView3DZoomMinusToolTipHeader },
				{ "LibraryInitView3DZoomMinusToolTipBody", LibraryInitView3DZoomMinusToolTipBody },
				{ "LibraryInitPos2DTabLabelText", LibraryInitPos2DTabLabelText },
				{ "LibraryClearNeutralButtonText", LibraryClearNeutralButtonText },
				{ "LibraryClearNeutralToolTipHeader", LibraryClearNeutralToolTipHeader },
				{ "LibraryClearNeutralToolTipBody", LibraryClearNeutralToolTipBody },
				{ "LibraryInvertNeutralButtonText", LibraryInvertNeutralButtonText },
				{ "LibraryInvertNeutralToolTipHeader", LibraryInvertNeutralToolTipHeader },
				{ "LibraryInvertNeutralToolTipBody", LibraryInvertNeutralToolTipBody },
				{ "LibraryCopyFromEditorNeutralButtonText", LibraryCopyFromEditorNeutralButtonText },
				{ "LibraryCopyFromEditorNeutralToolTipHeader", LibraryCopyFromEditorNeutralToolTipHeader },
				{ "LibraryCopyFromEditorNeutralToolTipBody", LibraryCopyFromEditorNeutralToolTipBody },
				{ "LibraryInitPosUpTabLabelText", LibraryInitPosUpTabLabelText },
				{ "LibraryInitPosDownTabLabelText", LibraryInitPosDownTabLabelText },
				{ "LibraryInitPosFrontTabLabelText", LibraryInitPosFrontTabLabelText },
				{ "LibraryInitPosBackTabLabelText", LibraryInitPosBackTabLabelText },
				{ "LibraryInitPosRightTabLabelText", LibraryInitPosRightTabLabelText },
				{ "LibraryInitPosLeftTabLabelText", LibraryInitPosLeftTabLabelText },
				
				{ "LibraryAlgorithmViewLabelText", LibraryAlgorithmViewLabelText },
				{ "LibraryAlgorithm3DTabLabelText", LibraryAlgorithm3DTabLabelText },
				{ "Library3DAlgViewLabelText", Library3DAlgViewLabelText },
				{ "LibraryAlgView3DZoomPlusButtonText", LibraryAlgView3DZoomPlusButtonText },
				{ "LibraryAlgView3DZoomPlusToolTipHeader", LibraryAlgView3DZoomPlusToolTipHeader },
				{ "LibraryAlgView3DZoomPlusToolTipBody", LibraryAlgView3DZoomPlusToolTipBody },
				{ "LibraryAlgView3DZoomMinusButtonText", LibraryAlgView3DZoomMinusButtonText },
				{ "LibraryAlgView3DZoomMinusToolTipHeader", LibraryAlgView3DZoomMinusToolTipHeader },
				{ "LibraryAlgView3DZoomMinusToolTipBody", LibraryAlgView3DZoomMinusToolTipBody },
				{ "LibraryAlgorithm2DTabLabelText", LibraryAlgorithm2DTabLabelText },
				{ "LibraryAlgUpTabLabelText", LibraryAlgUpTabLabelText },
				{ "LibraryAlgDownTabLabelText", LibraryAlgDownTabLabelText },
				{ "LibraryAlgFrontTabLabelText", LibraryAlgFrontTabLabelText },
				{ "LibraryAlgBackTabLabelText", LibraryAlgBackTabLabelText },
				{ "LibraryAlgRightTabLabelText", LibraryAlgRightTabLabelText },
				{ "LibraryAlgLeftTabLabelText", LibraryAlgLeftTabLabelText },
				
				{ "LibraryAlgorithmInfoLabelText", LibraryAlgorithmInfoLabelText },
				{ "LibraryFolderInfoLabelText", LibraryFolderInfoLabelText },
				{ "LibraryInfoLabelText", LibraryInfoLabelText },
                
				// Settings tab texts
				{ "SettingsMainLabelText", SettingsMainLabelText },
				{ "SettingsLanguageComboToolTipHeader", SettingsLanguageComboToolTipHeader },
				{ "SettingsLanguageComboToolTipBody", SettingsLanguageComboToolTipBody },
				{ "SettingsLanguageToolButtonText", SettingsLanguageToolButtonText },
				{ "SettingsLanguageToolButtonToolTipHeader", SettingsLanguageToolButtonToolTipHeader },
				{ "SettingsLoadLanguageToolToolTipBody", SettingsLoadLanguageToolToolTipBody },
				{ "SettingsLoadLabelText", SettingsLoadLabelText },
				{ "SettingsLoadComboToolTipHeader", SettingsLoadComboToolTipHeader },
				{ "SettingsLoadComboToolTipBody", SettingsLoadComboToolTipBody },
				{ "SettingsLoadButtonText", SettingsLoadButtonText },
				{ "SettingsLoadButtonToolTipHeader", SettingsLoadButtonToolTipHeader },
				{ "SettingsLoadButtonToolTipBody", SettingsLoadButtonToolTipBody },
				{ "SettingsSaveLabelText", SettingsSaveLabelText },
				{ "SettingsSaveComboToolTipHeader", SettingsSaveComboToolTipHeader },
				{ "SettingsSaveComboToolTipBody", SettingsSaveComboToolTipBody },
				{ "SettingsSaveTextBoxToolTipHeader", SettingsSaveTextBoxToolTipHeader },
				{ "SettingsSaveTextBoxToolTipBody", SettingsSaveTextBoxToolTipBody },
				{ "SettingsSaveButtonText", SettingsSaveButtonText },
				{ "SettingsSaveButtonToolTipHeader", SettingsSaveButtonToolTipHeader },
				{ "SettingsSaveButtonToolTipBody", SettingsSaveButtonToolTipBody },
				{ "SettingsDeleteButtonText", SettingsDeleteButtonText },
				{ "SettingsDeleteButtonToolTipHeader", SettingsDeleteButtonToolTipHeader },
				{ "SettingsDeleteButtonToolTipBody", SettingsDeleteButtonToolTipBody },
				
				{ "SettingsColorsLabelText", SettingsColorsLabelText },
				{ "SettingsFactoryColorsButtonText", SettingsFactoryColorsButtonText },
				{ "SettingsFactoryColorsToolTipHeader", SettingsFactoryColorsToolTipHeader },
				{ "SettingsFactoryColorsToolTipBody", SettingsFactoryColorsToolTipBody },				
				{ "SettingsUpColorLabelText", SettingsUpColorLabelText },
				{ "SettingsDownColorLabelText", SettingsDownColorLabelText },
				{ "SettiSettingsFrontColorLabelTextngs", SettingsFrontColorLabelText },
				{ "SettingsBackColorLabelText", SettingsBackColorLabelText },
				{ "SettingsRightColorLabelText", SettingsRightColorLabelText },
				{ "SettingsLeftColorLabelText", SettingsLeftColorLabelText },
				{ "SettingsNeutralColorLabelText", SettingsNeutralColorLabelText },
				{ "SettingsBaseColorLabelText", SettingsBaseColorLabelText },
				{ "SettingsBackgroundColorLabelText", SettingsBackgroundColorLabelText },
				{ "SettingsArrowsColorLabelText", SettingsArrowsColorLabelText },
				
				{ "SettingsNotationLabelText", SettingsNotationLabelText },
				{ "SettingsNotationUseAltwCheckBoxText", SettingsNotationUseAltwCheckBoxText },
				{ "SettingsNotationUseAltwToolTipHeader", SettingsNotationUseAltwToolTipHeader },
				{ "SettingsNotationUseAltwToolTipBody", SettingsNotationUseAltwToolTipBody },
				{ "SettingsNotationERotationLabelText", SettingsNotationERotationLabelText },
				{ "SettingsNotationERotationAsUCheckBoxText", SettingsNotationERotationAsUCheckBoxText },
				{ "SettingsNotationERotationAsUToolTipHeader", SettingsNotationERotationAsUToolTipHeader },
				{ "SettingsNotationERotationAsUToolTipBody", SettingsNotationERotationAsUToolTipBody },
				{ "SettingsNotationERotationAsDCheckBoxText", SettingsNotationERotationAsDCheckBoxText },
				{ "SettingsNotationERotationAsDToolTipHeader", SettingsNotationERotationAsDToolTipHeader },
				{ "SettingsNotationERotationAsDToolTipBody", SettingsNotationERotationAsDToolTipBody },
				{ "SettingsNotationSRotationLabelText", SettingsNotationSRotationLabelText },
				{ "SettingsNotationSRotationAsFCheckBoxText", SettingsNotationSRotationAsFCheckBoxText },
				{ "SettingsNotationSRotationAsFDToolTipHeader", SettingsNotationSRotationAsFDToolTipHeader },
				{ "SettingsNotationSRotationAsFToolTipBody", SettingsNotationSRotationAsFToolTipBody },
				{ "SettingsNotationSRotationAsBCheckBoxText", SettingsNotationSRotationAsBCheckBoxText },
				{ "SettingsNotationSRotationAsBDToolTipHeader", SettingsNotationSRotationAsBDToolTipHeader },
				{ "SettingsNotationSRotationAsBToolTipBody", SettingsNotationSRotationAsBToolTipBody },
				{ "SettingsNotationMRotationLabelText", SettingsNotationMRotationLabelText },
				{ "SettingsNotationMRotationAsRCheckBoxText", SettingsNotationMRotationAsRCheckBoxText },
				{ "SettingsNotationMRotationAsRDToolTipHeader", SettingsNotationMRotationAsRDToolTipHeader },
				{ "SettingsNotationMRotationAsRToolTipBody", SettingsNotationMRotationAsRToolTipBody },
				{ "SettingsNotationMRotationAsLCheckBoxText", SettingsNotationMRotationAsLCheckBoxText },
				{ "SettingsNotationMRotationAsLDToolTipHeader", SettingsNotationMRotationAsLDToolTipHeader },
				{ "SettingsNotationMRotationAsLToolTipBody", SettingsNotationMRotationAsLToolTipBody },
				
				{ "SettingsChronoLabelText", SettingsChronoLabelText },
                { "SettingsChronoBeepLabelText", SettingsChronoBeepLabelText },
                { "SettingsChronoBeepCheckBoxText", SettingsChronoBeepCheckBoxText },
                { "SettingsChronoBeepToolTipHeader", SettingsChronoBeepToolTipHeader },
                { "SettingsChronoBeepToolTipBody", SettingsChronoBeepToolTipBody },
                { "SettingsDefaultCubeLabelText", SettingsDefaultCubeLabelText },
				{ "SettingsDefaultCubeToolTipHeader", SettingsDefaultCubeToolTipHeader },
				{ "SettingsDefaultCubeToolTipBody", SettingsDefaultCubeToolTipBody },
				{ "SettingsDefaultCommentLabelText", SettingsDefaultCommentLabelText },
				{ "SettingsDefaultCommentToolTipHeader", SettingsDefaultCommentToolTipHeader },
				{ "SettingsDefaultCommentToolTipBody", SettingsDefaultCommentToolTipBody },
				{ "SettingsChronoAnimSpeedLabelText", SettingsChronoAnimSpeedLabelText },
				{ "SettingsChronoAnimSpeedToolTipHeader", SettingsChronoAnimSpeedToolTipHeader },
				{ "SettingsChronoAnimSpeedToolTipBody", SettingsChronoAnimSpeedToolTipBody },
				{ "SettingsChronoOptionsLabelText", SettingsChronoOptionsLabelText },
				{ "SettingsChronoConvertToBasicCheckBoxText", SettingsChronoConvertToBasicCheckBoxText },
				{ "SettingsChronoConvertToBasicToolTipHeader", SettingsChronoConvertToBasicToolTipHeader },
				{ "SettingsChronoConvertToBasicToolTipBody", SettingsChronoConvertToBasicToolTipBody },
				{ "SettingsChronoRemoveTurnsCheckBoxText", SettingsChronoRemoveTurnsCheckBoxText },
				{ "SettingsChronoRemoveTurnsToolTipHeader", SettingsChronoRemoveTurnsToolTipHeader },
				{ "SettingsChronoRemoveTurnsToolTipBody", SettingsChronoRemoveTurnsToolTipBody },
				{ "SettingsChronoShrinkCheckBoxText", SettingsChronoShrinkCheckBoxText },
				{ "SettingsChronoShrinkToolTipHeader", SettingsChronoShrinkToolTipHeader },
				{ "SettingsChronoShrinkToolTipBody", SettingsChronoShrinkToolTipBody },
				
				{ "SettingsEditorLabelText", SettingsEditorLabelText },
				{ "SettingsEditorAllowCompoundCheckBoxText", SettingsEditorAllowCompoundCheckBoxText },
				{ "SettingsEditorAllowCompoundToolTipHeader", SettingsEditorAllowCompoundToolTipHeader },
				{ "SettingsEditorAllowCompoundToolTipBody", SettingsEditorAllowCompoundToolTipBody },
				{ "SettingsEditorAnimSpeedLabelText", SettingsEditorAnimSpeedLabelText },
				{ "SettingsEditorAnimSpeedToolTipHeader", SettingsEditorAnimSpeedToolTipHeader },
				{ "SettingsEditorAnimSpeedToolTipBody", SettingsEditorAnimSpeedToolTipBody },
				
				{ "SettingsLibraryLabelText", SettingsLibraryLabelText },
				{ "SettingsLibraryStartDefaultPositionCheckBoxText", SettingsLibraryStartDefaultPositionCheckBoxText },
				{ "SettingsLibraryStartDefaultToolTipHeader", SettingsLibraryStartDefaultToolTipHeader },
				{ "SettingsLibraryStartDefaultToolTipBody", SettingsLibraryStartDefaultToolTipBody },
				{ "SettingsLibraryShowParenthesesCheckBoxText", SettingsLibraryShowParenthesesCheckBoxText },
				{ "SettingsLibraryShowParenthesesToolTipHeader", SettingsLibraryShowParenthesesToolTipHeader },
				{ "SettingsLibraryShowParenthesesToolTipBody", SettingsLibraryShowParenthesesToolTipBody },
				{ "SettingsLibraryInitialLabelText", SettingsLibraryInitialLabelText },
				{ "SettingsLibraryConvertToBasicCheckBoxText", SettingsLibraryConvertToBasicCheckBoxText },
				{ "SettingsLibraryConvertToBasicToolTipHeader", SettingsLibraryConvertToBasicToolTipHeader },
				{ "SettingsLibraryConvertToBasicToolTipBody", SettingsLibraryConvertToBasicToolTipBody },
				{ "SettingsLibraryRemoveTurnsCheckBoxText", SettingsLibraryRemoveTurnsCheckBoxText },
				{ "SettingsLibraryRemoveTurnsToolTipHeader", SettingsLibraryRemoveTurnsToolTipHeader },
				{ "SettingsLibraryRemoveTurnsToolTipBody", SettingsLibraryRemoveTurnsToolTipBody },
				{ "SettingsLibraryShrinkCheckBoxText", SettingsLibraryShrinkCheckBoxText },
				{ "SettingsLibraryShrinkToolTipHeader", SettingsLibraryShrinkToolTipHeader },
				{ "SettingsLibraryShrinkToolTipBody", SettingsLibraryShrinkToolTipBody },
				{ "SettingsLibraryInverseCheckBoxText", SettingsLibraryInverseCheckBoxText },
				{ "SettingsLibraryInverseToolTipHeader", SettingsLibraryInverseToolTipHeader },
				{ "SettingsLibraryInverseToolTipBody", SettingsLibraryInverseToolTipBody },
				{ "SettingsLibraryInsertCheckBoxText", SettingsLibraryInsertCheckBoxText },
				{ "SettingsLibraryInsertToolTipHeader", SettingsLibraryInsertToolTipHeader },
				{ "SettingsLibraryInsertToolTipBody", SettingsLibraryInsertToolTipBody },
				{ "SettingsLibraryStepsToInsertToolTipHeader", SettingsLibraryStepsToInsertToolTipHeader },
				{ "SettingsLibraryStepsToInsertToolTipBody", SettingsLibraryStepsToInsertToolTipBody },

                { "SettingsFoldersText", SettingsFoldersText },
                { "SettingsLanguageFolderButtonText", SettingsLanguageFolderButtonText },
                { "SettingsLanguageFolderButtonToolTipHeader", SettingsLanguageFolderButtonToolTipHeader },
                { "SettingsLanguageFolderButtonToolTipBody", SettingsLanguageFolderButtonToolTipBody },
                { "SettingsFolderButtonText", SettingsFolderButtonText },
                { "SettingsFolderButtonToolTipHeader", SettingsFolderButtonToolTipHeader },
                { "SettingsFolderButtonToolTipBody", SettingsFolderButtonToolTipBody },
                { "SettingsSolvesFolderButtonText", SettingsSolvesFolderButtonText },
                { "SettingsSolvesFolderButtonToolTipHeader", SettingsSolvesFolderButtonToolTipHeader },
                { "SettingsSolvesFolderButtonToolTipBody", SettingsSolvesFolderButtonToolTipBody },
                { "SettingsLibrariesFolderButtonText", SettingsLibrariesFolderButtonText },
                { "SettingsLibrariesFolderButtonToolTipHeader", SettingsLibrariesFolderButtonToolTipHeader },
                { "SettingsLibrariesFolderButtonToolTipBody", SettingsLibrariesFolderButtonToolTipBody },
                { "SettingsVideoFolderButtonText", SettingsVideoFolderButtonText },
                { "SettingsVideoFolderButtonToolTipHeader", SettingsVideoFolderButtonToolTipHeader },
                { "SettingsVideoFolderButtonToolTipBody", SettingsVideoFolderButtonToolTipBody },
                { "SettingsReadmeButtonText", SettingsReadmeButtonText },
                { "SettingsReadmeButtonToolTipHeader", SettingsReadmeButtonToolTipHeader },
                { "SettingsReadmeButtonToolTipBody", SettingsReadmeButtonToolTipBody },

                { "SettingsAboutLabelText", SettingsAboutLabelText },
				{ "SettingsCreditsAuthorLabelText", SettingsCreditsAuthorLabelText },
				{ "SettingsCreditsLabelText", SettingsCreditsLabelText },
				{ "SettingsCreditsIDELabelText", SettingsCreditsIDELabelText },
				{ "SettingsCreditsGraphicsLabelText", SettingsCreditsGraphicsLabelText },
				{ "SettingsCreditsXceedLabelText", SettingsCreditsXceedLabelText },
                { "SettingsCreditsLiteDBLabelText", SettingsCreditsLiteDBLabelText },
                { "SettingsCreditsCameraLabelText", SettingsCreditsCameraLabelText },
                { "SettingsCreditsVideoLabelText", SettingsCreditsVideoLabelText },
                { "SettingsCreditsExamplesLabelText", SettingsCreditsExamplesLabelText },
                { "SettingsDonateLabelText", SettingsDonateLabelText },
                { "SettingsDonateToolTip", SettingsDonateToolTip },
                { "SettingsProgressLabelText", SettingsProgressLabelText },
                { "SettingsMinimumSolbesLabelText", SettingsMinimumSolbesLabelText },
                { "ProgresSolvesUpDownToolTipHeader", ProgresSolvesUpDownToolTipHeader },
                { "ProgresSolvesUpDownToolTipBody", ProgresSolvesUpDownToolTipBody },

            };
        }

        /// <summary>
        /// Updates the language files combo box
        /// </summary>
        private void UpdateLanguageFiles()
        {
            int CurrentLanguageIndex = -1;
            SettingsLanguageComboBox.Items.Clear();
            if (AMTexts.LanguageFiles == null) return;
            for (int n = 0; n < AMTexts.LanguageFiles.Length; n++)
            {
                SettingsLanguageComboBox.Items.Add(AMTexts.LanguageFiles[n]);
                if (string.Compare(AMTexts.LanguageFiles[n], AMSettings.CurrentLanguageFile) == 0) CurrentLanguageIndex = n;
            }
            if (CurrentLanguageIndex >= 0) SettingsLanguageComboBox.SelectedIndex = CurrentLanguageIndex;
        }

        /// <summary>
        /// Reads the avaliable settings files and updates the settings files combo box
        /// </summary>
        private void UpdateSettingsFiles()
        {
            try
            {
                if (!Directory.Exists(AMSettings.SettingsFolder))
                    Directory.CreateDirectory(AMSettings.SettingsFolder);
            }
            catch (Exception ex)
            {
                AMSettings.Log = "Fault creating settings folder:" + ex.Message;
                return;
            }

            DirectoryInfo diSettings = new System.IO.DirectoryInfo(AMSettings.SettingsFolder);
            FileInfo[] fiSettings = diSettings.GetFiles("*" + AMSettings.SettingsExt);

            string[] SettingsFiles = null;

            if (fiSettings.Length > 0)
            {
                SettingsFiles = new string[fiSettings.Length];
                for (int n = 0; n < fiSettings.Length; n++) SettingsFiles[n] = fiSettings[n].Name;
            }

            SettingsSaveComboBox.Items.Clear();
            SettingsSaveComboBox.Items.Add(AMTexts.Message("NewSettingsFileComboItemMessage"));

            SettingsLoadComboBox.Items.Clear();

            if (SettingsFiles != null)
                for (int n = 0; n < SettingsFiles.Length; n++)
                {
                    SettingsLoadComboBox.Items.Add(SettingsFiles[n]);
                    if (string.Compare(SettingsFiles[n], "factory" + AMSettings.SettingsExt) != 0)
                        SettingsSaveComboBox.Items.Add(SettingsFiles[n]);
                }

            SettingsLoadButton.IsEnabled = false;
            SettingsSaveButton.IsEnabled = false;
            SettingsSaveTextBox.IsEnabled = false;
            SettingsDeleteButton.IsEnabled = false;
        }

        /// <summary>
        /// Update main window texts
        /// </summary>
        public void UpdateTexts()
        {
            foreach (KeyValuePair<string, object> TextObj in Translate)
                (TextObj.Value as TextBlock).Text = AMTexts.Text(TextObj.Key);

            // Table columns headers
            ChronoTableColumnDateTime.Header = AMTexts.Message("ChronoTableColumnDateTimeText");
            ChronoTableColumnChronoTime.Header = AMTexts.Message("ChronoTableColumnChronoTimeText");
            ChronoTableColumnPenalty.Header = AMTexts.Message("ChronoTableColumnPenaltyText");
            ChronoTableColumnScramble.Header = AMTexts.Message("ChronoTableColumnScrambleText");
            ChronoTableColumnSolveType.Header = AMTexts.Message("ChronoTableColumnSolveTypeText");
            ChronoTableColumnCube.Header = AMTexts.Message("ChronoTableColumnCubeText");
            ChronoTableColumnComment.Header = AMTexts.Message("ChronoTableColumnCommentText");

            // Set the chronometer button text
            ChronoButtonText.Text = AMTexts.Message("EnableChronoMessage");

            UpdateSolveTypeCombos();

            // Ser the progress legend canvas texts
            ProgressMaximumCheckBoxText.Text = AMTexts.Message("ProgressMaximumCheckBoxText");
            ProgressMaximumCheckBoxToolTipBody.Text = AMTexts.Message("ProgressMaximumCheckBoxToolTipBody");
            ProgressMaximumCheckBoxToolTipBody.Text = AMTexts.Message("ProgressMaximumCheckBoxToolTipBody");
            ProgressMinimumCheckBoxText.Text = AMTexts.Message("ProgressMinimumCheckBoxText");
            ProgressMinimumCheckBoxToolTipHeader.Text = AMTexts.Message("ProgressMinimumCheckBoxToolTipHeader");
            ProgressMinimumCheckBoxToolTipBody.Text = AMTexts.Message("ProgressMinimumCheckBoxToolTipBody");
            ProgressAverageCheckBoxText.Text = AMTexts.Message("ProgressAverageCheckBoxText");
            ProgressAverageCheckBoxToolTipHeader.Text = AMTexts.Message("ProgressAverageCheckBoxToolTipHeader");
            ProgressAverageCheckBoxToolTipBody.Text = AMTexts.Message("ProgressAverageCheckBoxToolTipBody");
            ProgressDeviationCheckBoxText.Text = AMTexts.Message("ProgressDeviationCheckBoxText");
            ProgressDeviationCheckBoxToolTipHeader.Text = AMTexts.Message("ProgressDeviationCheckBoxToolTipHeader");
            ProgressDeviationCheckBoxToolTipBody.Text = AMTexts.Message("ProgressDeviationCheckBoxToolTipBody");
            ProgressMediumCheckBoxText.Text = AMTexts.Message("ProgressMediumCheckBoxText");
            ProgressMediumCheckBoxToolTipHeader.Text = AMTexts.Message("ProgressMediumCheckBoxToolTipHeader");
            ProgressMediumCheckBoxToolTipBody.Text = AMTexts.Message("ProgressMediumCheckBoxToolTipBody");
            ProgressBarsCheckBoxText.Text = AMTexts.Message("ProgressBarsCheckBoxText");
            ProgressBarsCheckBoxToolTipHeader.Text = AMTexts.Message("ProgressBarsCheckBoxToolTipHeader");
            ProgressBarsCheckBoxToolTipBody.Text = AMTexts.Message("ProgressBarsCheckBoxToolTipBody");

            ProgressPeriodCombo.Items.Clear();
            ProgressPeriodCombo.Items.Add(AMTexts.Message("ProgressDailyComboBoxText"));
            ProgressPeriodCombo.Items.Add(AMTexts.Message("ProgressWeeklyComboBoxText"));
            ProgressPeriodCombo.Items.Add(AMTexts.Message("ProgressMonthlyComboBoxText"));
            ProgressPeriodCombo.Items.Add(AMTexts.Message("ProgressAnnuallyComboBoxText"));
            ProgressPeriodCombo.SelectedIndex = 0;
        }

        /// <summary>
        /// Updates the cubes materials and all the tabs views
        /// </summary>
        private void UpdateAllViews()
        {
            ChronoData.Cube.ChangeAllMaterials();
            UpdateChronoViews();
            EditorData.Cube.ChangeAllMaterials();
            UpdateEditorViews();
            LibraryData.AlgorithmCube.ChangeAllMaterials();
            LibraryData.InitScrambleCube.ChangeAllMaterials();
            UpdateLibraryViews();
            UpdateSettingsViews();
        }

        #endregion Main window general functions

        #region Main window general events

        /// <summary>
        /// Request navigate event: Send URI to navigator
        /// </summary>
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

		/// <summary>
        /// Selection changed event: control tab changes
        /// </summary>
        private void MainTab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.OriginalSource == MainTab)
			{
				if (LockChronoTabFlag && CurrentTab != MainTabNames.Chrono)
				{
					e.Handled = true;
					MainTab.SelectedIndex = (int)MainTabNames.Chrono;
				}
                else if (LockProgressTabFlag && CurrentTab != MainTabNames.Progress)
                {
                    e.Handled = true;
                    MainTab.SelectedIndex = (int)MainTabNames.Progress;
                }
                else if (LockEditorTabFlag && CurrentTab != MainTabNames.Editor)
				{
					e.Handled = true;
					MainTab.SelectedIndex = (int)MainTabNames.Editor;
				}
				else if (LockLibraryTabFlag && CurrentTab != MainTabNames.Library)
				{
					e.Handled = true;
					MainTab.SelectedIndex = (int)MainTabNames.Library;
				}
				else if (LockSettingsTabFlag && CurrentTab != MainTabNames.Settings)
				{
					e.Handled = true;
					MainTab.SelectedIndex = (int)MainTabNames.Settings;
				}
			}
        }

        /// <summary>
        /// Closing event: Check unsaved libraries
        /// </summary>
        private void AlgorithmMasterWindow_Closing(object sender, CancelEventArgs e)
        {
            // Ask to save modified libraries
            LibraryRootData LibData;
            for (int n = 0; n < LibraryTreeView.Items.Count; n++)
            {
                LibData = (LibraryTreeView.Items[n] as TreeViewItem).Tag as LibraryRootData;
                if (LibData.IsModified)
                {
                    switch (MessageBox.Show(AMTexts.Message("LibrarySaveChangesMessage"),
                                            string.Format(AMTexts.Message("LibraryModifiedMessage"), LibData.Name),
                                            MessageBoxButton.YesNoCancel,
                                            MessageBoxImage.Question))
                    {
                        case MessageBoxResult.Yes:
                            SaveLibrary(LibraryTreeView.Items[n] as TreeViewItem);
                            break;
                        case MessageBoxResult.No:
                            break;
                        default:
                            e.Cancel = true;
                            break;
                    }
                }
            }

            // Stop BackgroundWorker tasks
            if (ChronoData.AnimWork != null)
            {
                ChronoData.AnimWork.CancelAsync();
                ChronoData.AnimWork.Dispose();
            }
            if (EditorData.AnimWork != null)
            {
                EditorData.AnimWork.CancelAsync();
                EditorData.AnimWork.Dispose();
            }
            if (LibraryData.AnimWork != null)
            {
                LibraryData.AnimWork.CancelAsync();
                LibraryData.AnimWork.Dispose();
            }

            // Shutdown application
            // Application.Current.Shutdown();
        }

        /// <summary>
        /// Key down event: avoid system keys in chronometer tab
        /// </summary>
        private void AlgorithmMasterWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (CurrentTab == MainTabNames.Chrono && e.Key == Key.System && e.OriginalSource is Button)
            {
                e.Handled = true;
            }
        }

        #endregion Main window general events

        #region Chronometer functions

        /// <summary>
        /// Initialize chronometer fields
        /// </summary>
        private void InitializeChrono()
        {
            ChronoScrambleBarTray.IsLocked = true;

            SetChronoCube2DButtons();

            ChronoData.MovingChronoCube = false;
            ChronoData.ResultNotSavedFlag = false;
            ChronoData.Chronometer = new Stopwatch();
            ChronoData.EventsTimerLauncher = new DispatcherTimer();
            ChronoData.EventsTimerLauncher.Tick += new EventHandler(EventsTimerLauncher_Tick);
            ChronoData.Result = new ChronoResult();
            ChronoData.State = ChronoStates.INITIAL_WAIT;
            ChronoBorder.Style = (Style)FindResource("ChronoBorderInitialWait");

            Random RandomSeed = new Random((int)(DateTime.Now.Ticks % int.MaxValue));
            ChronoData.Scramble = new Scramble(int.Parse(ChronoStepsLabel.Content as string), RandomSeed.Next());
            ChronoData.Cube = new Cube3D();

            ChronoData.ScramblePosition = ChronoData.ScramblePosTarget = ChronoData.Scramble.Length;

            UpdateChronoScramble();

            ChronoData.Cube.ApplyScramble(ChronoData.Scramble);
            ChronoData.Cube.UpdateBitmap((int)ChronoImage3D.ActualWidth, (int)ChronoImage3D.ActualHeight);
            ChronoImage3D.Source = ChronoData.Cube.renderBMP;
            UpdateChronoCube2D();

            ChronoResultsTable.Items.Clear();

            UpdateSolveTypeCombos();

            // Backgorund worker task for animations
            ChronoData.AnimWork = new BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = false
            };
            ChronoData.AnimWork.DoWork += new DoWorkEventHandler(ChronoAnimWork_DoWork);
            ChronoData.AnimWork.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ChronoAnimWork_RunWorkerCompleted);

            if (AMSettings.ChronoLoadAtStart) LoadTodayTimesButton_Click(null, null);
            else UpdateChronoStadistics();

            UpdateCalendarBlackoutDays();

            try // Add cameras to combo box
            {
                ChronoData.CameraMonikers = CapDevice.DeviceMonikers;
                if (ChronoData.CameraMonikers.Length > 0)
                {
                    for (int n = 0; n < ChronoData.CameraMonikers.Length; n++)
                        CamerasComboBox.Items.Add(ChronoData.CameraMonikers[n].Name);
                    CamerasComboBox.SelectedIndex = 0;
                }
                else throw new Exception("No cameras detected!");
            }
            catch (Exception ex) { AMSettings.Log = "Camera initialization: " + ex.Message; }

            ChronoData.LastVideoFolder = AMSettings.VideoFolder;
            ChronoData.CameraReadyFlag = false;
        }

        /// <summary>
        /// Set buttons array for chrono cube 2D view (a button per sticker)
        /// </summary>
        private void SetChronoCube2DButtons()
        {
            if (ChronoData.Cube2DButtons == null || ChronoData.Cube2DButtons.Length != 54)
                ChronoData.Cube2DButtons = new Button[54];

            // Face up
            ChronoData.Cube2DButtons[(int)StickerPositions.UBL_U] = Chrono_B2D_UBL_U;
            ChronoData.Cube2DButtons[(int)StickerPositions.UB_U] = Chrono_B2D_UB_U;
            ChronoData.Cube2DButtons[(int)StickerPositions.UBR_U] = Chrono_B2D_UBR_U;
            ChronoData.Cube2DButtons[(int)StickerPositions.UL_U] = Chrono_B2D_UL_U;
            ChronoData.Cube2DButtons[(int)StickerPositions.U] = Chrono_B2D_U;
            ChronoData.Cube2DButtons[(int)StickerPositions.UR_U] = Chrono_B2D_UR_U;
            ChronoData.Cube2DButtons[(int)StickerPositions.UFL_U] = Chrono_B2D_UFL_U;
            ChronoData.Cube2DButtons[(int)StickerPositions.UF_U] = Chrono_B2D_UF_U;
            ChronoData.Cube2DButtons[(int)StickerPositions.UFR_U] = Chrono_B2D_UFR_U;

            // Layer up
            ChronoData.Cube2DButtons[(int)StickerPositions.UBL_L] = Chrono_B2D_UBL_L;
            ChronoData.Cube2DButtons[(int)StickerPositions.UL_L] = Chrono_B2D_UL_L;
            ChronoData.Cube2DButtons[(int)StickerPositions.UFL_L] = Chrono_B2D_UFL_L;
            ChronoData.Cube2DButtons[(int)StickerPositions.UFL_F] = Chrono_B2D_UFL_F;
            ChronoData.Cube2DButtons[(int)StickerPositions.UF_F] = Chrono_B2D_UF_F;
            ChronoData.Cube2DButtons[(int)StickerPositions.UFR_F] = Chrono_B2D_UFR_F;
            ChronoData.Cube2DButtons[(int)StickerPositions.UFR_R] = Chrono_B2D_UFR_R;
            ChronoData.Cube2DButtons[(int)StickerPositions.UR_R] = Chrono_B2D_UR_R;
            ChronoData.Cube2DButtons[(int)StickerPositions.UBR_R] = Chrono_B2D_UBR_R;
            ChronoData.Cube2DButtons[(int)StickerPositions.UBR_B] = Chrono_B2D_UBR_B;
            ChronoData.Cube2DButtons[(int)StickerPositions.UB_B] = Chrono_B2D_UB_B;
            ChronoData.Cube2DButtons[(int)StickerPositions.UBL_B] = Chrono_B2D_UBL_B;

            // Layer middle
            ChronoData.Cube2DButtons[(int)StickerPositions.BL_L] = Chrono_B2D_BL_L;
            ChronoData.Cube2DButtons[(int)StickerPositions.L] = Chrono_B2D_L;
            ChronoData.Cube2DButtons[(int)StickerPositions.LF_L] = Chrono_B2D_LF_L;
            ChronoData.Cube2DButtons[(int)StickerPositions.LF_F] = Chrono_B2D_LF_F;
            ChronoData.Cube2DButtons[(int)StickerPositions.F] = Chrono_B2D_F;
            ChronoData.Cube2DButtons[(int)StickerPositions.FR_F] = Chrono_B2D_FR_F;
            ChronoData.Cube2DButtons[(int)StickerPositions.FR_R] = Chrono_B2D_FR_R;
            ChronoData.Cube2DButtons[(int)StickerPositions.R] = Chrono_B2D_R;
            ChronoData.Cube2DButtons[(int)StickerPositions.RB_R] = Chrono_B2D_RB_R;
            ChronoData.Cube2DButtons[(int)StickerPositions.RB_B] = Chrono_B2D_RB_B;
            ChronoData.Cube2DButtons[(int)StickerPositions.B] = Chrono_B2D_B;
            ChronoData.Cube2DButtons[(int)StickerPositions.BL_B] = Chrono_B2D_BL_B;

            // Layer down
            ChronoData.Cube2DButtons[(int)StickerPositions.DBL_L] = Chrono_B2D_DBL_L;
            ChronoData.Cube2DButtons[(int)StickerPositions.DL_L] = Chrono_B2D_DL_L;
            ChronoData.Cube2DButtons[(int)StickerPositions.DFL_L] = Chrono_B2D_DFL_L;
            ChronoData.Cube2DButtons[(int)StickerPositions.DFL_F] = Chrono_B2D_DFL_F;
            ChronoData.Cube2DButtons[(int)StickerPositions.DF_F] = Chrono_B2D_DF_F;
            ChronoData.Cube2DButtons[(int)StickerPositions.DFR_F] = Chrono_B2D_DFR_F;
            ChronoData.Cube2DButtons[(int)StickerPositions.DFR_R] = Chrono_B2D_DFR_R;
            ChronoData.Cube2DButtons[(int)StickerPositions.DR_R] = Chrono_B2D_DR_R;
            ChronoData.Cube2DButtons[(int)StickerPositions.DBR_R] = Chrono_B2D_DBR_R;
            ChronoData.Cube2DButtons[(int)StickerPositions.DBR_B] = Chrono_B2D_DBR_B;
            ChronoData.Cube2DButtons[(int)StickerPositions.DB_B] = Chrono_B2D_DB_B;
            ChronoData.Cube2DButtons[(int)StickerPositions.DBL_B] = Chrono_B2D_DBL_B;

            // Face down
            ChronoData.Cube2DButtons[(int)StickerPositions.DFL_D] = Chrono_B2D_DFL_D;
            ChronoData.Cube2DButtons[(int)StickerPositions.DF_D] = Chrono_B2D_DF_D;
            ChronoData.Cube2DButtons[(int)StickerPositions.DFR_D] = Chrono_B2D_DFR_D;
            ChronoData.Cube2DButtons[(int)StickerPositions.DL_D] = Chrono_B2D_DL_D;
            ChronoData.Cube2DButtons[(int)StickerPositions.D] = Chrono_B2D_D;
            ChronoData.Cube2DButtons[(int)StickerPositions.DR_D] = Chrono_B2D_DR_D;
            ChronoData.Cube2DButtons[(int)StickerPositions.DBL_D] = Chrono_B2D_DBL_D;
            ChronoData.Cube2DButtons[(int)StickerPositions.DB_D] = Chrono_B2D_DB_D;
            ChronoData.Cube2DButtons[(int)StickerPositions.DBR_D] = Chrono_B2D_DBR_D;
        }

        /// <summary>
        /// Update the solve types combo boxes
        /// </summary>
        private void UpdateSolveTypeCombos()
        {
            ChronoTypeCombo.Items.Clear();
            ChronoTypeStatsCombo.Items.Clear();
            ProgressTypeCombo.Items.Clear();

            bool LogState = AMSettings.LogEnabled;
            AMSettings.LogEnabled = false;
            for (int n = 0; n < 100; n++)
            {
                string TypeMessageId = "ResolutionType" + n.ToString("00");
                if (string.IsNullOrEmpty(AMTexts.Message(TypeMessageId))) break;
                ChronoTypeCombo.Items.Add(AMTexts.Message(TypeMessageId));
                ChronoTypeStatsCombo.Items.Add(AMTexts.Message(TypeMessageId));
                ProgressTypeCombo.Items.Add(AMTexts.Message(TypeMessageId));
            }
            AMSettings.LogEnabled = LogState;

            ChronoTypeCombo.SelectedIndex = 0;
            ChronoTypeStatsCombo.SelectedIndex = 0;
            ProgressTypeCombo.SelectedIndex = 0;
        }

        /// <summary>
        /// Update chrono cube skin colors
        /// </summary>
        public void UpdateChronoCube2D()
        {
            Chrono2DGrid.Background = AMSettings.BackgroundBrush;
            if (ChronoData.Cube != null && ChronoData.Cube2DButtons != null)
            {
                for (int Pos = ChronoData.Cube2DButtons.Length - 1; Pos >= 0; Pos--)
                {
                    ChronoData.Cube2DButtons[Pos].BorderBrush = AMSettings.BaseBrush;
                    if (ChronoData.Cube.NeutralStickers[Pos])
                        ChronoData.Cube2DButtons[Pos].Background = AMSettings.NeutralBrush;
                    else
                        ChronoData.Cube2DButtons[Pos].Background =
                            AMSettings.GetBrush(ChronoData.Cube.GetColor((StickerPositions)Pos));
                }
            }
        }

        /// <summary>
        /// Update the chrono scramble list of buttons
        /// </summary>
        public void UpdateChronoScramble()
        {
            ChronoScrambleToolBar.Items.Clear();

            ChronoData.ScrambleButtons = new Button[ChronoData.Scramble.Length + 1];

            for (int i = 0; i <= ChronoData.Scramble.Length; i++)
            {
                ChronoData.ScrambleButtons[i] = new Button();

                if (i < ChronoData.ScramblePosition)
                    ChronoData.ScrambleButtons[i].Style = (Style)FindResource("ScrambleButtonGreenBackKey");
                else if (i == ChronoData.ScramblePosition)
                    ChronoData.ScrambleButtons[i].Style = (Style)FindResource("ScrambleButtonWhiteBackKey");
                else
                    ChronoData.ScrambleButtons[i].Style = (Style)FindResource("ScrambleButtonYellowBackKey");

                if (i == 0)
                    ChronoData.ScrambleButtons[i].Content = AMTexts.Message("NONEStepMessage"); // First step = NONE
                else ChronoData.ScrambleButtons[i].Content = ScrambleStep.GetText(ChronoData.Scramble[i - 1]);
                ChronoData.ScrambleButtons[i].Tag = i;
                ChronoData.ScrambleButtons[i].Click += ChronoScrambleButtons_Click;

                ChronoScrambleToolBar.Items.Add(ChronoData.ScrambleButtons[i]);
            }
        }

        /// <summary>
        /// Updates the scramble view, the 3D cube and the 2D cube skin
        /// </summary>
        public void UpdateChronoViews()
        {
            UpdateChronoScramble();
            ChronoData.Cube.Reset();
            ChronoData.Cube.ApplyScramble(ChronoData.Scramble.SubScramble(0, ChronoData.ScramblePosition));
            ChronoData.Cube.UpdateBitmap((int)ChronoImage3D.ActualWidth, (int)ChronoImage3D.ActualHeight);
            ChronoImage3D.Source = ChronoData.Cube.renderBMP;
            UpdateChronoCube2D();
        }

        /// <summary>
        /// Update results in result window and show it
        /// </summary>
        private void ShowWindowChronoResult()
        {
            ResultChronoWindow = new ResultWindow
            {
                Owner = AlgorithmMasterWindow
            };

            ResultChronoWindow.UpdateResult(ChronoData.Result);

            if (ResultChronoWindow.ShowDialog() ?? true)
            {
                ChronoData.Result.Cube = ResultChronoWindow.Cube;
                ChronoData.Result.Comment = ResultChronoWindow.Comment;

                long FinalTime = ChronoData.Result.MeasuredTime;
                if (ChronoData.Result.StartDelayPenalty2s) FinalTime += 2000;
                if (ResultChronoWindow.SolvePenalty2s)
                {
                    ChronoData.Result.NotSolvedPenalty2s = true;
                    FinalTime += 2000;
                }
                if (ResultChronoWindow.SolveDNS)
                {
                    ChronoData.Result.DNS = true;
                    FinalTime = 0;
                }
                if (ResultChronoWindow.SolveDNF)
                {
                    ChronoData.Result.DNF = true;
                    FinalTime = 0;
                }

                string Penalty;
                if (ChronoData.Result.StartDelayDNF) Penalty = AMTexts.Message("DNFInspectionMessage");
                else if (ChronoData.Result.DNF) Penalty = AMTexts.Message("DNFSolveMessage");
                else if (ChronoData.Result.DNS) Penalty = AMTexts.Message("DNSMessage");
                else if (ChronoData.Result.StartDelayPenalty2s && ChronoData.Result.NotSolvedPenalty2s)
                    Penalty = AMTexts.Message("Penalty4sMessage");
                else if (ChronoData.Result.StartDelayPenalty2s) Penalty = AMTexts.Message("Penalty2sInspectionMessage");
                else if (ChronoData.Result.NotSolvedPenalty2s) Penalty = AMTexts.Message("Penalty2sSolveMessage");
                else Penalty = AMTexts.Message("NoPenaltyMessage");

                ResultDataRow RS = new ResultDataRow
                {
                    ChronoDateTime = ChronoData.Result.StartTime.ToString(),
                    ChronoTime = AMTexts.MilliseconsToString(FinalTime),
                    ChronoPenalty = Penalty,
                    ChronoScramble = ChronoData.Result.Scramble,
                    ChronoType = AMTexts.Message("ResolutionType" + string.Format("{0:00}", (int)ChronoData.Result.SolvingType)),
                    ChronoCube = ChronoData.Result.Cube,
                    ChronoComment = ChronoData.Result.Comment
                };

                ChronoResultsTable.Items.Insert(0, RS);
                ChronoResultsTable.SelectedIndex = 0;

                if (AMSettings.ChronoSaveTimes)
                { // If there is a new solve, update calendar
                    if (SaveChronoResult(RS)) UpdateCalendarBlackoutDays();
                    AMSettings.Status = AMTexts.Message("NewChronoMessage") + RS.ChronoTime;
                }
                UpdateChronoStadistics();
                UpdateChartData();

                ChronoData.ResultNotSavedFlag = false;
                ChronoData.Result.Reset();

                NewRandomScrambleButton.Focus();
                ChronoButtonText.Text = AMTexts.Message("EnableChronoMessage");

                // Save video file
                if ((bool)SaveVideoCheckBox.IsChecked && ChronoData.VideoReadyFlag)
                {
                    Microsoft.Win32.SaveFileDialog SaveVideoDlg = new Microsoft.Win32.SaveFileDialog
                    {
                        Title = AMTexts.Message("NewVideoFileTitle"),
                        FileName = AMTexts.Message("NewVideoFileName"),
                        InitialDirectory = ChronoData.LastVideoFolder,
                        DefaultExt = ".avi",
                        Filter = AMTexts.Message("NewVideoFileFilter"),
                        CheckFileExists = false,
                        OverwritePrompt = true,
                    };

                    if (SaveVideoDlg.ShowDialog() == true)
                    {
                        try
                        {
                            File.Delete(SaveVideoDlg.FileName);
                            File.Move(AMSettings.TempVideoFile, SaveVideoDlg.FileName);
                            ChronoData.LastVideoFolder = System.IO.Path.GetDirectoryName(SaveVideoDlg.FileName);
                        }
                        catch (Exception ex) { AMSettings.Log = "Fault moving video file: " + ex.Message; }

                        ChronoData.VideoReadyFlag = false;
                    }
                }
            }
            else
            {
                ChronoButtonText.Text = AMTexts.Message("ChronoNewRunMessage");
                AMSettings.Status = AMTexts.Message("NoChronoSavedMessage");
            }

            ChronoData.State = ChronoStates.INITIAL_WAIT;
            ChronoBorder.Style = (Style)FindResource("ChronoBorderInitialWait");

            EnableDisableControlsWhileChronoRun(true);
        }

        /// <summary>
        /// Load chrono results from data base
        /// </summary>
        /// <param name="dt">Date of results to load</param>
        public void LoadChronoResults(DateTime dt)
        {
            ChronoResultsTable.Items.Clear();

            if (dt == null || !File.Exists(AMSettings.SolvesDBPath)) return;

            ResultDataRow RS;

            dt = dt.Date; // Discard time data

            // Open solves data base file
            using (LiteDatabase SolvesDB = new LiteDatabase(AMSettings.SolvesDBPath))
            {
                // Get solves collection
                var SolvesCol = SolvesDB.GetCollection<ResultDB>("solves");

                // Find all solves for a date
                var AllSolves = SolvesCol.FindAll().Where(x => x.ChronoDateTime >= dt && x.ChronoDateTime < dt.AddDays(1));

                foreach (ResultDB R in AllSolves)
                {
                    RS = new ResultDataRow
                    {
                        ChronoDateTime = R.ChronoDateTime.ToString(),
                        ChronoTime = AMTexts.MilliseconsToString(R.ChronoTime),
                        ChronoPenalty = R.ChronoPenalty,
                        ChronoScramble = R.ChronoScramble,
                        ChronoType = R.ChronoType,
                        ChronoCube = R.ChronoCube,
                        ChronoComment = R.ChronoComment
                    };
                    ChronoResultsTable.Items.Add(RS);
                }
            }

            ChronoResultsTable.SelectedIndex = 0;
        }

        /// <summary>
        /// Calculate and update stadistics from results table
        /// </summary>
        public void UpdateChronoStadistics()
        {
            if (ChronoResultsTable == null) return;

            var ChronoTableTimes = new List<int>();
            var Average5Times = new List<int>();

            int intTime;
            bool ValidType;

            // If there isn't selected row, select the first row
            if (ChronoResultsTable.Items.Count > 0 && ChronoResultsTable.SelectedIndex < 0)
                ChronoResultsTable.SelectedIndex = 0;

            int RowNumber = 0;
            foreach (ResultDataRow Row in ChronoResultsTable.Items)
            {
                intTime = (int)AMTexts.StringToMilliseconds(Row.ChronoTime);

                ValidType = string.Compare(Row.ChronoType, // Same type of resolution than combo box
                                           AMTexts.Message("ResolutionType" + string.Format("{0:00}", ChronoTypeStatsCombo.SelectedIndex))) == 0;

                if (ValidType && intTime > 0) ChronoTableTimes.Add(intTime);

                if (ChronoResultsTable.SelectedItems.Count > 0 && RowNumber >= ChronoResultsTable.SelectedIndex &&
                    ValidType && Average5Times.Count < 5)
                {
                    if (intTime > 0) Average5Times.Add(intTime); // Valid time
                    else Average5Times.Add(int.MaxValue);
                }

                RowNumber++;
            }

            if (ChronoTableTimes == null || ChronoTableTimes.Count <= 0)
            {
                ChronoData.BestTime = 0;
                ChronoData.WorstTime = 0;
                ChronoData.AverageTime = 0;
                ChronoData.MediumTime = 0;
                ChronoData.StandardDeviation = 0;
            }
            else
            {
                int[] Order = OrderIntCollection(ChronoTableTimes);
                if (Order == null) return;

                int totaling = 0;
                foreach (int t in Order) totaling += t;

                int n = 0;
                while (n < Order.Length && Order[n] <= 0) n++;
                if (n < Order.Length) ChronoData.BestTime = Order[n];
                else ChronoData.BestTime = Order[0];

                ChronoData.WorstTime = Order[Order.Length - 1];
                ChronoData.AverageTime = totaling / ChronoTableTimes.Count;
                if (ChronoTableTimes.Count % 2 == 0)
                    ChronoData.MediumTime = (Order[ChronoTableTimes.Count / 2 - 1] + Order[ChronoTableTimes.Count / 2]) / 2;
                else
                    ChronoData.MediumTime = Order[(ChronoTableTimes.Count - 1) / 2];

                if (ChronoTableTimes.Count > 1)
                {
                    double sdtotaling = 0;
                    for (int i = 0; i < ChronoTableTimes.Count; i++) sdtotaling += Math.Pow(Order[i] - ChronoData.AverageTime, 2.0);
                    ChronoData.StandardDeviation = (int)Math.Sqrt(sdtotaling / (ChronoTableTimes.Count - 1));
                }
                else ChronoData.StandardDeviation = 0;
            }

            BestTimeLabelValue.Text = AMTexts.MilliseconsToString(ChronoData.BestTime);
            WorstTimeLabelValue.Text = AMTexts.MilliseconsToString(ChronoData.WorstTime);
            AverageTimeLabelValue.Text = AMTexts.MilliseconsToString(ChronoData.AverageTime);
            MediumTimeLabelValue.Text = AMTexts.MilliseconsToString(ChronoData.MediumTime);
            StandardDeviationLabelValue.Text = AMTexts.MilliseconsToString(ChronoData.StandardDeviation);

            int[] Table5Times = OrderIntCollection(Average5Times);

            // If there is 2 or more invalid times, no average result
            if (Table5Times == null || Table5Times.Length < 5 || Table5Times[4] > 99999999 && Table5Times[3] > 99999999)
            {
                AverageTime1Value.Text = AMTexts.MilliseconsToString(0);
                AverageTime2Value.Text = AMTexts.MilliseconsToString(0);
                AverageTime3Value.Text = AMTexts.MilliseconsToString(0);
                AverageTime4Value.Text = AMTexts.MilliseconsToString(0);
                AverageTime5Value.Text = AMTexts.MilliseconsToString(0);
                AverageLabelValue.Text = AMTexts.Message("NoTimeMessage");
            }
            else
            {
                AverageTime1Value.Text = AMTexts.MilliseconsToString(Table5Times[0]);
                AverageTime2Value.Text = AMTexts.MilliseconsToString(Table5Times[1]);
                AverageTime3Value.Text = AMTexts.MilliseconsToString(Table5Times[2]);
                AverageTime4Value.Text = AMTexts.MilliseconsToString(Table5Times[3]);
                AverageTime5Value.Text = Table5Times[4] > 99999999 ? AMTexts.Message("NoTimeMessage") : AMTexts.MilliseconsToString(Table5Times[4]);

                AverageLabelValue.Text = AMTexts.MilliseconsToString((Table5Times[1] + Table5Times[2] + Table5Times[3]) / 3);
            }

            /* TODO: Draw distribution and evolution charts

            ChronoDistributionCanvas.Children.Clear();
            ChronoEvolutionCanvas.Children.Clear();

            // Calculate the vertical display values range
            int VertRange = 1000, AuxRange, RangeSteps = 6;
            while (ChronoData.BestTime / VertRange > 0) VertRange *= 10;
            AuxRange = VertRange / 10;
            while (VertRange > ChronoData.BestTime) VertRange -= AuxRange;
            VertRange += 2 * AuxRange;

            for (int n = 0; n < RangeSteps; n++)
            {
                double perc = (double)n / (RangeSteps - 1);

                PointCollection DistHorizPoints = new PointCollection()
                {
                    AMSettings.GetChartPosition(ChronoDistributionCanvas.ActualWidth,
                                                ChronoDistributionCanvas.ActualHeight,
                                                0d, perc),
                    AMSettings.GetChartPosition(ChronoDistributionCanvas.ActualWidth,
                                                ChronoDistributionCanvas.ActualHeight,
                                                1d, perc)
                };

                PointCollection EvolHorizPoints = new PointCollection()
                {
                    AMSettings.GetChartPosition(ChronoEvolutionCanvas.ActualWidth,
                                                ChronoEvolutionCanvas.ActualHeight,
                                                0d, perc),
                    AMSettings.GetChartPosition(ChronoEvolutionCanvas.ActualWidth,
                                                ChronoEvolutionCanvas.ActualHeight,
                                                1d, perc)
                };

                ChronoDistributionCanvas.Children.Add(new Polyline
                {
                    Stroke = AMSettings.GetBrush(Colors.Chocolate),
                    StrokeThickness = 0.75,
                    Points = DistHorizPoints,
                    StrokeDashArray = new DoubleCollection { 6d, 6d }
                });

                ChronoEvolutionCanvas.Children.Add(new Polyline
                {
                    Stroke = AMSettings.GetBrush(Colors.Chocolate),
                    StrokeThickness = 0.75,
                    Points = EvolHorizPoints,
                    StrokeDashArray = new DoubleCollection { 6d, 6d }
                });
            }

            */
        }

        /// <summary>
        /// Enable / disable controls while chorno is running
        /// </summary>
        /// <param name="enable">True to enable controls / false to disable</param>
        public void EnableDisableControlsWhileChronoRun(bool enable)
        {
            ChronoScrambleToolBar.IsEnabled = enable;
            Chrono3DButtonsPanel.IsEnabled = enable;
            ChronoScrambleButtonsPanel.IsEnabled = enable;
            ChronoTypeSolvingPanel.IsEnabled = enable;
            ChronoTypeStatsCombo.IsEnabled = enable;
            View3DCameraTab.IsEnabled = enable;
            LockChronoTabFlag = !enable;
        }

        /// <summary>
        /// Reads the chrono solves data base and bolds the existing days
        /// </summary>
        /// <returns>True if exist data</returns>
        public bool UpdateCalendarBlackoutDays()
        {
            LoadDateTimesPicker.BlackoutDates.Clear();
            ProgressStartDateTimesPicker.BlackoutDates.Clear();
            ProgressEndDateTimesPicker.BlackoutDates.Clear();

            LoadDateTimesPicker.DisplayDateEnd = DateTime.MaxValue;
            ProgressStartDateTimesPicker.DisplayDateEnd = DateTime.MaxValue;
            ProgressEndDateTimesPicker.DisplayDateEnd = DateTime.MaxValue;

            LoadDateTimesPicker.DisplayDateStart = DateTime.MinValue;
            ProgressStartDateTimesPicker.DisplayDateStart = DateTime.MinValue;
            ProgressEndDateTimesPicker.DisplayDateStart = DateTime.MinValue;

            if (!File.Exists(AMSettings.SolvesDBPath)) return false;

            DateTime MinDate, MaxDate;
            List<DateTime> DateList = new List<DateTime>();

            // Open solves data base file
            using (LiteDatabase SolvesDB = new LiteDatabase(AMSettings.SolvesDBPath))
            {
                // Get solves collection
                var SolvesCol = SolvesDB.GetCollection<ResultDB>("solves");

                // Find all date and times solves
                var AllSolves = SolvesCol.Find(Query.All("CronoDateTime", Query.Ascending));

                foreach (ResultDB R in AllSolves)
                {
                    bool DateFound = false;
                    foreach (DateTime DT in DateList)
                    {
                        if (DT == R.ChronoDateTime.Date)
                        {
                            DateFound = true;
                            break;
                        }
                    }
                    if (!DateFound) DateList.Add(R.ChronoDateTime.Date);
                }
            }

            if (DateList.Count > 0)
            {
                DateList.Sort(); // Ascending order

                MinDate = DateList[0];
                MaxDate = DateList[DateList.Count - 1];

                if (MaxDate < DateTime.Now.Date) DateList.Add(DateTime.Now.Date);

                CalendarDateRange CDR;
                int DaysInterval;
                for (int n = 1; n < DateList.Count; n++)
                {
                    DaysInterval = (DateList[n] - DateList[n - 1]).Days;

                    if (DaysInterval == 2)
                        CDR = new CalendarDateRange(DateList[n - 1].AddDays(1));
                    else if (DaysInterval > 2)
                        CDR = new CalendarDateRange(DateList[n - 1].AddDays(1), DateList[n].AddDays(-1));
                    else
                        CDR = null;

                    if (CDR != null)
                    {
                        try
                        {
                            LoadDateTimesPicker.BlackoutDates.Add(CDR);
                            ProgressStartDateTimesPicker.BlackoutDates.Add(CDR);
                            ProgressEndDateTimesPicker.BlackoutDates.Add(CDR);
                        }
                        catch (Exception ex)
                        {
                            AMSettings.Log = "Blackout fault: " + ex.Message;
                        }
                    }
                }

                LoadDateTimesPicker.DisplayDateEnd = DateTime.Now.Date;
                LoadDateTimesPicker.DisplayDateStart = MinDate;

                ProgressStartDateTimesPicker.DisplayDateEnd = MaxDate;
                ProgressStartDateTimesPicker.DisplayDateStart = MinDate;

                ProgressEndDateTimesPicker.DisplayDateEnd = MaxDate;
                ProgressEndDateTimesPicker.DisplayDateStart = MinDate;

                if (LoadDateTimesPicker.SelectedDate == null || 
                    LoadDateTimesPicker.SelectedDate < LoadDateTimesPicker.DisplayDateStart ||
                    LoadDateTimesPicker.SelectedDate > LoadDateTimesPicker.DisplayDateEnd)
                    LoadDateTimesPicker.SelectedDate = DateTime.Now.Date;

                return true;
            }

            // No dates
            LoadDateTimesPicker.DisplayDateEnd = LoadDateTimesPicker.DisplayDateStart = DateTime.Now.Date;
            ProgressStartDateTimesPicker.DisplayDateEnd = ProgressStartDateTimesPicker.DisplayDateStart = DateTime.Now.Date;
            ProgressEndDateTimesPicker.DisplayDateEnd = ProgressEndDateTimesPicker.DisplayDateStart = DateTime.Now.Date;
            return false;
        }

        /*
        /// <summary>
        /// Import from legacy solves format -develop tool-
        /// </summary>
        private void ImportLegacySolves()
        {
            if (!Directory.Exists(AMSettings.SolvesFolder)) return;

            DirectoryInfo diChrono = new DirectoryInfo(AMSettings.SolvesFolder);
            DirectoryInfo[] smcDirs = diChrono.GetDirectories();

            if (smcDirs.Length <= 0) return;

            foreach (DirectoryInfo di in smcDirs)
            {
                if (int.TryParse(di.Name, out int Year))
                {
                    string ChronoYearPath = System.IO.Path.Combine(AMSettings.SolvesFolder, Year.ToString("0000"));
                    if (!Directory.Exists(ChronoYearPath)) continue;

                    FileInfo[] smcFiles = di.GetFiles("*" + ".smc");
                    foreach (FileInfo fi in smcFiles)
                    {
                        if (int.TryParse(fi.Name.Substring(0, 3), out int DayOfYear) && DayOfYear > 0 && DayOfYear <= 366)
                        {
                            string ChronoFile = System.IO.Path.Combine(ChronoYearPath,
                                                                       DayOfYear.ToString("000") + ".smc");
                            if (!File.Exists(ChronoFile)) continue;

                            XmlDocument XmlResult = new XmlDocument();
                            XmlResult.Load(ChronoFile);
                            ResultDB TempResult;

                            foreach (XmlNode Node in XmlResult.SelectNodes("/ChronoSolves/Solve"))
                            {
                                TempResult = new ResultDB
                                {
                                    ChronoDateTime = DateTime.Parse(Node.Attributes["datetime"].Value),
                                    ChronoTime = (int)AMTexts.StringToMilliseconds(Node.Attributes["chrono"].Value),
                                    ChronoPenalty = Node.Attributes["penalty"].Value,
                                    ChronoScramble = Node.Attributes["scramble"].Value,
                                    ChronoType = Node.Attributes["type"].Value,
                                    ChronoCube = Node.Attributes["cube"].Value,
                                    ChronoComment = Node.Attributes["comment"].Value
                                };
                                // Open solves data base file
                                using (LiteDatabase SolvesDB = new LiteDatabase(AMSettings.SolvesDBPath))
                                {
                                    // Get solves collection
                                    var SolvesCol = SolvesDB.GetCollection<ResultDB>("solves");

                                    // Delete solve from data base
                                    SolvesCol.Insert(TempResult);
                                }
                            }
                        }
                    }
                }
            }
        }
        */

        #region Chrono video functions

        /// <summary>
        /// Starts video capture
        /// </summary>
        private void StartVideoCapture()
        {
            try { File.Delete(AMSettings.TempVideoFile); }
            catch (Exception ex) { AMSettings.Log = "Fault deleting temp video file: " + ex.Message; }

            if (ChronoData.VideoTimer == null && ChronoData.Camera?.Framerate > 0f)
            {
                ChronoData.VideoTimer = new DispatcherTimer(DispatcherPriority.Send);
                ChronoData.VideoTimer.Tick += new EventHandler(ChronoCameraVideo_Tick);
                ChronoData.VideoTimer.Interval = new TimeSpan(0, 0, 0, 0, (int)(500f / ChronoData.Camera.Framerate));
            }

            try
            {
                ChronoData.VideoWriter = new AviWriter(AMSettings.TempVideoFile)
                {
                    FramesPerSecond = (int)ChronoData.Camera.Framerate,
                    EmitIndex1 = true
                };

                ChronoData.VideoStream = ChronoData.VideoWriter.AddMotionJpegVideoStream(
                        (int)ChronoData.Camera.BitmapSource.Width,
                        (int)ChronoData.Camera.BitmapSource.Height,
                        quality: AMSettings.VideoQuality);
                /*        
                ChronoData.VideoStream = ChronoData.VideoWriter.AddMpeg4VideoStream((int)ChronoData.Camera.BitmapSource.Width,
                                                                                    (int)ChronoData.Camera.BitmapSource.Height,
                                                                                    ChronoData.Camera.Framerate,
                                                                                    quality: AMSettings.VideoQuality, 
                                                                                    codec: KnownFourCCs.Codecs.X264, 
                                                                                    forceSingleThreadedAccess: true);
                                                                                    */
                ChronoData.VideoTimer?.Start();
                ChronoData.VideoSavingFlag = true;
                ChronoData.VideoReadyFlag = false;
            }
            catch (Exception ex) { AMSettings.Log = "Video fault: " + ex.Message; }
        }

        /// <summary>
        /// Stops video capture
        /// </summary>
        private void StopVideoCapture()
        {
            ChronoData.VideoTimer?.Stop(); // Stop video capture
            ChronoData.VideoWriter?.Close(); // Close temp video file
            if (ChronoData.VideoSavingFlag) ChronoData.VideoReadyFlag = true;
            ChronoData.VideoSavingFlag = false;
            
        }

        #endregion Chrono video functions

        #region Chronometer static functions

        /// <summary>
        /// Takes a collection of int and returns a int table with the values ordered in increasing order
        /// </summary>
        /// <param name="col">Collection of int values</param>
        /// <returns>Array of ints ordered</returns>
        public static int[] OrderIntCollection(List<int> col)
        {
            if (col == null) return null;
            if (col.Count <= 0) return null;

            int[] Order = new int[col.Count];
            bool[] Processed = new bool[col.Count];

            int MinPos = -1;
            for (int i = 0; i < col.Count; i++)
            {
                int MinVal = int.MaxValue;
                for (int n = 0; n < col.Count; n++)
                {
                    if (!Processed[n] && col[n] < MinVal)
                    {
                        MinVal = col[n];
                        MinPos = n;
                    }
                }
                if (MinPos >= 0)
                {
                    Processed[MinPos] = true;
                    Order[i] = MinVal;
                }
            }
            return Order;
        }

        /// <summary>
        /// Save chrono result to data base
        /// </summary>
        /// <param name="rs">Result data row class</param>
        /// <returns>True if result is added to solves data base</returns>
        public static bool SaveChronoResult(ResultDataRow rs)
        {
            if (!DateTime.TryParse(rs.ChronoDateTime, out DateTime dt)) return false;

            // Open solves data base file
            using (LiteDatabase SolvesDB = new LiteDatabase(AMSettings.SolvesDBPath))
            {
                // Get solves collection
                var SolvesCol = SolvesDB.GetCollection<ResultDB>("solves");

                ResultDB R = new ResultDB
                {
                    ChronoDateTime = dt,
                    ChronoTime = (int)AMTexts.StringToMilliseconds(rs.ChronoTime),
                    ChronoPenalty = rs.ChronoPenalty,
                    ChronoScramble = rs.ChronoScramble,
                    ChronoType = rs.ChronoType,
                    ChronoCube = rs.ChronoCube,
                    ChronoComment = rs.ChronoComment
                };

                SolvesCol.Insert(R);
            }
            return true;
        }
        /*
        /// <summary>
        /// Convert chrono data from CSV file to xml format and write in system
        /// CSV format per line: datetime,chronotimeinms,penalty,scramble,type,cube,comment
        /// </summary>
        /// <param name="csvfile">CSV format file</param>
        public static void ConvertCSVToXML(string csvfile)
        {
            try { if (!File.Exists(csvfile)) return; } catch (Exception ex) { AMSettings.Log = "Error with CSV file: " + ex.Message; }

            string[] Lines;

            try { Lines = File.ReadAllLines(csvfile); }
            catch (Exception ex) { AMSettings.Log = "Error reading CSV file: " + ex.Message; return; }

            if (Lines == null) return;

            foreach (string Line in Lines)
            {
                string[] fields = Line.Split(',');
                if (fields.Length >= 7) // Valid line has at least seven fields
                {
                    ResultDataRow r = new ResultDataRow
                    {
                        ChronoDateTime = fields[0],
                        ChronoTime = AMTexts.MilliseconsToString(int.Parse(fields[1])), // in csv files write '34223' directly, not '00:34:223'
                        ChronoPenalty = fields[2],
                        ChronoScramble = fields[3],
                        ChronoType = fields[4],
                        ChronoCube = fields[5],
                        ChronoComment = fields[6]
                    };
                    SaveChronoResult(r);
                }
            }
        }
        */
        /// <summary>
        /// Convert from System.Drawing.Bitmap to Windows.Media.Image
        /// </summary>
        /// <param name="img">System.Drawing.Bitmap</param>
        /// <returns>BitmapImage</returns>
        public static BitmapImage BitmapToBitmapImage(System.Drawing.Bitmap img)
        {
            if (img == null) return null;
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            MemoryStream ms = new MemoryStream();
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            ms.Seek(0, SeekOrigin.Begin);
            bi.StreamSource = ms;
            bi.EndInit();
            return bi;
        }

        /// <summary>
        /// Convert from Windows.Media.Image to System.Drawing.Bitmap
        /// </summary>
        /// <param name="img">BitmapImage</param>
        /// <returns>System.Drawing.Bitmap</returns>
        public static System.Drawing.Bitmap BitmapImageToBitmap(BitmapImage source)
        {
            if (source == null) return null;
            System.Drawing.Bitmap bmp =
                new System.Drawing.Bitmap(source.PixelWidth,
                                          source.PixelHeight,
                                          System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            System.Drawing.Imaging.BitmapData data =
                bmp.LockBits(new System.Drawing.Rectangle(System.Drawing.Point.Empty, bmp.Size),
                             System.Drawing.Imaging.ImageLockMode.WriteOnly,
                             System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            source.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
            bmp.UnlockBits(data);
            return bmp;
        }

        #endregion Chronometer static functions

        #endregion Chronometer functions

        #region Chronometer events

        #region ChronoImage3D events

        /// <summary>
        /// Chrono cube 3D size changed event
        /// </summary>
        private void ChronoImage3D_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ChronoData.Cube.UpdateBitmap((int)ChronoImage3D.ActualWidth, (int)ChronoImage3D.ActualHeight);
            ChronoImage3D.Source = ChronoData.Cube.renderBMP;
        }

        /// <summary>
        /// Mouse down event: start / stop move in 3D view
        /// </summary>
        private void ChronoImage3D_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ChronoData.MovingChronoCube)
            {
                ChronoData.MovingChronoCube = false;
                Cursor = Cursors.Arrow;
            }
            else
            {
                ChronoData.MovingChronoCube = true;
                Cursor = Cursors.ScrollAll;
            }
        }

        /// <summary>
        /// Mouse leave event: change cursor
        /// </summary>
        private void ChronoImage3D_MouseLeave(object sender, MouseEventArgs e)
        {
            if (ChronoData.MovingChronoCube) Cursor = Cursors.Arrow;
        }

        /// <summary>
        /// Mouse enter event: change cursor
        /// </summary>
        private void ChronoImage3D_MouseEnter(object sender, MouseEventArgs e)
        {
            if (ChronoData.MovingChronoCube) Cursor = Cursors.ScrollAll;
        }

        /// <summary>
        /// Mouse move event: move in 3D view
        /// </summary>
        private void ChronoImage3D_MouseMove(object sender, MouseEventArgs e)
        {
            if (ChronoData.MovingChronoCube)
            {
                double alpha = 360.0 * e.GetPosition(ChronoImage3D).X / ChronoImage3D.ActualWidth - 90.0;
                double beta = 180.0 * e.GetPosition(ChronoImage3D).Y / ChronoImage3D.ActualHeight - 90.0;
                ChronoData.Cube.SetCamera(ChronoData.Cube.CameraDistance, alpha, beta);
                ChronoData.Cube.UpdateBitmap((int)ChronoImage3D.ActualWidth, (int)ChronoImage3D.ActualHeight);
                ChronoImage3D.Source = ChronoData.Cube.renderBMP;
            }
        }

        #endregion ChronoImage3D events

        #region Scramble buttons events

        /// <summary>
        /// Click event: Chrono scramble buttons (asigned programatically)
        /// </summary>
        public void ChronoScrambleButtons_Click(object sender, EventArgs e)
        {
            if (!ChronoData.AnimWork.IsBusy)
            {
                Button StepButton = (Button)sender;

                ChronoData.ScramblePosTarget = int.Parse(StepButton.Tag.ToString());

                if (ChronoData.ScramblePosTarget < 0 || ChronoData.ScramblePosTarget > ChronoData.Scramble.Length ||
                    ChronoData.ScramblePosTarget == ChronoData.ScramblePosition) return;

                ChronoData.AnimWork.RunWorkerAsync(); // Start scramble animation
            }
        }

        /// <summary>
        /// Click event: Start scramble step button
        /// </summary>
        private void ChronoStartButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ChronoData.AnimWork.IsBusy)
            {
                ChronoData.ScramblePosition = 0;
                UpdateChronoViews();
            }
        }

        /// <summary>
        /// Click event: Previous scramble step button
        /// </summary>
        private void ChronoPreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ChronoData.AnimWork.IsBusy)
            {
                if (ChronoData.ScramblePosition > 0)
                    ChronoScrambleButtons_Click(ChronoData.ScrambleButtons[ChronoData.ScramblePosition - 1], null);
            }
        }

        /// <summary>
        /// Click event: Play scramble steps sequence button
        /// </summary>
        private void ChronoPlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ChronoData.AnimWork.IsBusy)
            {
                ChronoData.ScramblePosition = 0;
                UpdateChronoViews();
                System.Threading.Thread.Sleep(250);
                ChronoScrambleButtons_Click(ChronoData.ScrambleButtons[ChronoData.ScrambleButtons.Length - 1], null);
            }
        }

        /// <summary>
        /// Click event: Next scramble step button
        /// </summary>
        private void ChronoNextButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ChronoData.AnimWork.IsBusy)
            {
                if (ChronoData.ScramblePosition >= ChronoData.ScrambleButtons.Length - 1) return;
                ChronoScrambleButtons_Click(ChronoData.ScrambleButtons[ChronoData.ScramblePosition + 1], null);
            }
        }

        /// <summary>
        /// Click event: End scramble step button
        /// </summary>
        private void ChronoEndButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ChronoData.AnimWork.IsBusy)
            {
                ChronoData.ScramblePosition = ChronoData.Scramble.Length;
                UpdateChronoViews();
            }
        }

        /// <summary>
        /// Click event: Plus zoom 3D view button
        /// </summary>
        private void ChronoPlusButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ChronoData.AnimWork.IsBusy)
            {
                ChronoData.Cube.SetCamera(ChronoData.Cube.CameraDistance * 0.9, ChronoData.Cube.CameraAlpha, ChronoData.Cube.CameraBeta);
                ChronoData.Cube.UpdateBitmap((int)ChronoImage3D.ActualWidth, (int)ChronoImage3D.ActualHeight);
                ChronoImage3D.Source = ChronoData.Cube.renderBMP;
            }
        }

        /// <summary>
        /// Click event: Minus zoom 3D view button
        /// </summary>
        private void ChronoMinusButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ChronoData.AnimWork.IsBusy)
            {
                ChronoData.Cube.SetCamera(ChronoData.Cube.CameraDistance * 1.1, ChronoData.Cube.CameraAlpha, ChronoData.Cube.CameraBeta);
                ChronoData.Cube.UpdateBitmap((int)ChronoImage3D.ActualWidth, (int)ChronoImage3D.ActualHeight);
                ChronoImage3D.Source = ChronoData.Cube.renderBMP;
            }
        }

        /// <summary>
        /// Click event: Copy scramble to clipboard
        /// </summary>
        private void CopyScrambleButton_Click(object sender, RoutedEventArgs e)
        {
            if (ChronoData.Scramble != null) Clipboard.SetText(ChronoData.Scramble.ToString());
        }

        #endregion Scramble buttons events

        #region BackgroundWorker events

        /// <summary>
        /// Chrono BackgorundWorker DoWork event (asigned programatically)
        /// </summary>
        private void ChronoAnimWork_DoWork(object sender, DoWorkEventArgs e)
        {
            Steps SD;
            Stopwatch AnimTimeControl = new Stopwatch();

            Dispatcher.Invoke(new Action(delegate { Cursor = Cursors.Wait; }));

            if (ChronoData.ScramblePosition > ChronoData.ScramblePosTarget)
            {
                while (ChronoData.ScramblePosTarget != ChronoData.ScramblePosition)
                {
                    SD = ScrambleStep.Inverse(ChronoData.Scramble[ChronoData.ScramblePosition - 1]);

                    Dispatcher.Invoke(new Action(delegate
                    {
                        ChronoData.ScrambleButtons[ChronoData.ScramblePosition].Style = (Style)FindResource("ScrambleButtonWhiteBackKey");
                    }));

                    AnimTimeControl.Restart();
                    while (AnimTimeControl.ElapsedMilliseconds < AMSettings.ChronoAnimTime)
                    {
                        Dispatcher.Invoke(new Action(delegate
                        {
                            ChronoData.Cube.RotateStepPorcentage(SD, (double)AnimTimeControl.ElapsedMilliseconds / AMSettings.ChronoAnimTime);
                            ChronoData.Cube.UpdateBitmap((int)ChronoImage3D.ActualWidth, (int)ChronoImage3D.ActualHeight);
                            ChronoImage3D.Source = ChronoData.Cube.renderBMP;
                        }));
                        System.Threading.Thread.Sleep(10); // Very important to gain animation smoothness
                    }
                    AnimTimeControl.Stop();

                    Dispatcher.Invoke(new Action(delegate
                    {
                        ChronoData.Cube.RotateToZero();
                        ChronoData.Cube.ApplyStep(SD);
                        ChronoData.Cube.UpdateBitmap((int)ChronoImage3D.ActualWidth, (int)ChronoImage3D.ActualHeight);
                        ChronoImage3D.Source = ChronoData.Cube.renderBMP;
                    }));

                    Dispatcher.Invoke(new Action(delegate
                    {
                        ChronoData.ScrambleButtons[ChronoData.ScramblePosition].Style = (Style)FindResource("ScrambleButtonYellowBackKey");
                    }));

                    Dispatcher.Invoke(new Action(delegate
                    {
                        UpdateChronoCube2D();
                    }));

                    ChronoData.ScramblePosition--;
                }
            }
            else if (ChronoData.ScramblePosition < ChronoData.ScramblePosTarget)
            {
                while (ChronoData.ScramblePosTarget != ChronoData.ScramblePosition)
                {
                    Dispatcher.Invoke(new Action(delegate
                    {
                        ChronoData.ScrambleButtons[ChronoData.ScramblePosition].Style = (Style)FindResource("ScrambleButtonGreenBackKey");
                    }));

                    ChronoData.ScramblePosition++;

                    SD = ChronoData.Scramble[ChronoData.ScramblePosition - 1];

                    Dispatcher.Invoke(new Action(delegate
                    {
                        ChronoData.ScrambleButtons[ChronoData.ScramblePosition].Style = (Style)FindResource("ScrambleButtonWhiteBackKey");
                    }));

                    AnimTimeControl.Restart();
                    while (AnimTimeControl.ElapsedMilliseconds < AMSettings.ChronoAnimTime)
                    {
                        Dispatcher.Invoke(new Action(delegate
                        {
                            ChronoData.Cube.RotateStepPorcentage(SD, (double)AnimTimeControl.ElapsedMilliseconds / AMSettings.ChronoAnimTime);
                            ChronoData.Cube.UpdateBitmap((int)ChronoImage3D.ActualWidth, (int)ChronoImage3D.ActualHeight);
                            ChronoImage3D.Source = ChronoData.Cube.renderBMP;
                        }));
                        System.Threading.Thread.Sleep(10); // Very important to gain animation smoothness
                    }
                    AnimTimeControl.Stop();

                    Dispatcher.Invoke(new Action(delegate
                    {
                        ChronoData.Cube.RotateToZero();
                        ChronoData.Cube.ApplyStep(SD);
                        ChronoData.Cube.UpdateBitmap((int)ChronoImage3D.ActualWidth, (int)ChronoImage3D.ActualHeight);
                        ChronoImage3D.Source = ChronoData.Cube.renderBMP;
                    }));

                    Dispatcher.Invoke(new Action(delegate
                    {
                        ChronoData.ScrambleButtons[ChronoData.ScramblePosition].Style = (Style)FindResource("ScrambleButtonGreenBackKey");
                    }));

                    Dispatcher.Invoke(new Action(delegate
                    {
                        UpdateChronoCube2D();
                    }));
                }
            }

            Dispatcher.Invoke(new Action(delegate
            {
                ChronoData.ScrambleButtons[ChronoData.ScramblePosition].Style = (Style)FindResource("ScrambleButtonWhiteBackKey");
            }));
        }

        /// <summary>
        /// RunWorkerCompleted event (asigned programatically)
        /// </summary>
        private void ChronoAnimWork_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((e.Cancelled))
            {
            }
            else if (e.Error != null)
            {
                Dispatcher.Invoke(new Action(delegate { AMSettings.Log = "Chrono animation error: " + e.Error.Message; }));
            }
            else
            {
            }

            Dispatcher.Invoke(new Action(delegate { Cursor = Cursors.Arrow; }));
        }

        #endregion BackgroundWorker events

        #region Generate scramble buttons

        /// <summary>
        /// Click event: Increase step for new random scramble
        /// </summary>
        private void ChronoPlusStepsButton_Click(object sender, RoutedEventArgs e)
        {
            int steps = int.Parse(ChronoStepsLabel.Content.ToString());
            if (steps < 32)
            {
                steps++;
                ChronoStepsLabel.Content = steps.ToString("00");
            }
        }

        /// <summary>
        /// Click event: Decrease step for new random scramble
        /// </summary>
        private void ChronoMinusStepsButton_Click(object sender, RoutedEventArgs e)
        {
            int steps = int.Parse(ChronoStepsLabel.Content.ToString());
            if (steps > 6)
            {
                steps--;
                ChronoStepsLabel.Content = steps.ToString("00");
            }
        }

        /// <summary>
        /// Click event: New random scramble button
        /// </summary>
        private void ChronoNewRandomButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ChronoData.AnimWork.IsBusy)
            {
                if (ChronoData.ResultNotSavedFlag) // If there is no saved data, confirm operation
                {
                    if (MessageBox.Show(AMTexts.Message("DiscardingLastResultMessage"),
                                        AMTexts.Message("LastResultNotSavedMessage"),
                                        MessageBoxButton.YesNo,
                                        MessageBoxImage.Question) == MessageBoxResult.No)
                    {
                        ShowWindowChronoResult();
                        return;
                    }

                    ChronoData.ResultNotSavedFlag = false;
                }

                // TODO: Different scrambles for different solving types

                Random RandomSeed = new Random((int)(DateTime.Now.Ticks % int.MaxValue));

                if (int.TryParse(ChronoStepsLabel.Content.ToString(), out int steps))
                {
                    ChronoData.Scramble = new Scramble(steps, RandomSeed.Next());
                    ChronoData.ScramblePosition = ChronoData.ScramblePosTarget = ChronoData.Scramble.Length;
                    AMSettings.Status = AMTexts.Message("NewRandomScrambleMessage");
                }
                else
                {
                    ChronoData.Scramble = new Scramble();
                    ChronoData.ScramblePosition = ChronoData.ScramblePosTarget = ChronoData.Scramble.Length;
                    AMSettings.Log = "Fault parsing steps number.";
                }
                UpdateChronoViews();
                ChronoButton.Focus();
            }
        }

        /// <summary>
        /// Click event: New scramble from clipboard button
        /// </summary>
        private void ChronoFromClipboardButton_Click(object sender, RoutedEventArgs e)
        {
            if (ChronoData.ResultNotSavedFlag) // If there is no saved data, confirm operation
            {
                if (MessageBox.Show(AMTexts.Message("DiscardingLastResultMessage"),
                                    AMTexts.Message("LastResultNotSavedMessage"),
                                    MessageBoxButton.YesNo,
                                    MessageBoxImage.Question) == MessageBoxResult.No)
                {
                    ShowWindowChronoResult();
                    return;
                }

                ChronoData.ResultNotSavedFlag = false;
            }
            if (Clipboard.GetText().Length > 100)
            {
                if (MessageBox.Show(Clipboard.GetText(),
                                    AMTexts.Message("ConfirmScrambleTextMessage"),
                                    MessageBoxButton.YesNo,
                                    MessageBoxImage.Question) != MessageBoxResult.Yes) return;
            }
            ChronoData.Scramble.Reset();
            ChronoData.Scramble.ParseScramble(Clipboard.GetText());

            AMSettings.Status = AMTexts.Message("NewScrambleClipboardMessage");

            if (AMSettings.ChronoToBasic) ChronoData.Scramble.ToBasicSteps();
            if (AMSettings.ChronoRemoveTurns) ChronoData.Scramble.RemoveTurns();
            if (AMSettings.ChronoShrink) while (ChronoData.Scramble.Shrink()) ;

            ChronoData.ScramblePosition = ChronoData.ScramblePosTarget = ChronoData.Scramble.Length;
            UpdateChronoViews();
            ChronoButton.Focus();
        }

        /// <summary>
        /// Click event: No scramble
        /// </summary>
        private void ChronoNoScrambleButton_Click(object sender, RoutedEventArgs e)
        {
            if (ChronoData.ResultNotSavedFlag) // If there is no saved data, confirm operation
            {
                if (MessageBox.Show(AMTexts.Message("DiscardingLastResultMessage"),
                                    AMTexts.Message("LastResultNotSavedMessage"),
                                    MessageBoxButton.YesNo,
                                    MessageBoxImage.Question) == MessageBoxResult.No)
                {
                    ShowWindowChronoResult();
                    return;
                }

                ChronoData.ResultNotSavedFlag = false;
            }

            ChronoData.Scramble = new Scramble();
            AMSettings.Status = AMTexts.Message("NoScrambleMessage");
            ChronoData.ScramblePosition = ChronoData.ScramblePosTarget = ChronoData.Scramble.Length;
            UpdateChronoViews();
            ChronoButton.Focus();
        }

        #endregion Generate scramble buttons

        #region Chrono run events

        /// <summary>
        /// Tick event: Time controlled event for chrono update (asigned programatically)
        /// </summary>
        private void EventsTimerLauncher_Tick(object sender, EventArgs e)
        {
            if (ChronoData.State == ChronoStates.PRE_START)
            {
                ChronoData.EventsTimerLauncher.Stop();
                if (ChronoInspectionCheckBox.IsChecked ?? true)
                {
                    ChronoData.State = ChronoStates.INSPECTION;
                    ChronoBorder.Style = (Style)FindResource("ChronoBorderInspection");
                    ChronoButtonText.Text = AMTexts.Message("PerformingInspectionMessage");
                    ChronoData.EventsTimerLauncher.Interval = new TimeSpan(0, 0, 0, 0, 33); // 33 ms
                    ChronoLabelText.Text = AMTexts.MilliseconsToString(15000);
                    ChronoData.Chronometer.Restart();
                    ChronoData.EventsTimerLauncher.Start();
                }
                else
                {
                    ChronoData.State = ChronoStates.WAITING_START;
                    ChronoBorder.Style = (Style)FindResource("ChronoBorderWaitingStart");
                    ChronoButtonText.Text = AMTexts.Message("ChronoReadyMessage");
                    ChronoLabelText.Text = AMTexts.MilliseconsToString(0);
                }

                if (SettingsChronBeepCheckBox.IsChecked == true) BeepReadyPlayer?.Play();

                if (SaveVideoCheckBox.IsChecked == true && ChronoData.CameraReadyFlag) StartVideoCapture();
            }
            else if (ChronoData.State == ChronoStates.INSPECTION)
            {
                if (ChronoData.Chronometer.ElapsedMilliseconds < 15000)
                    ChronoLabelText.Text = AMTexts.MilliseconsToString(15000 - ChronoData.Chronometer.ElapsedMilliseconds);
                else ChronoLabelText.Text = AMTexts.MilliseconsToString(ChronoData.Chronometer.ElapsedMilliseconds - 15000);

                if (ChronoData.Inspection == InspectionStates.PRE_INSPECTION)
                {
                    ChronoBorder.Style = (Style)FindResource("ChronoBorderInspection");
                    ChronoData.Inspection = InspectionStates.FIRST_8_SECONDS;
                }
                else if (ChronoData.Inspection == InspectionStates.FIRST_8_SECONDS && ChronoData.Chronometer.ElapsedMilliseconds >= 8000)
                {
                    ChronoBorder.Style = (Style)FindResource("ChronoBorderInspection8s");
                    ChronoData.Inspection = InspectionStates.OVER_8_SECONDS;
                    if (SettingsChronBeepCheckBox.IsChecked == true) Beep8sPlayer?.Play();
                }
                else if (ChronoData.Inspection == InspectionStates.OVER_8_SECONDS && ChronoData.Chronometer.ElapsedMilliseconds >= 12000)
                {
                    ChronoBorder.Style = (Style)FindResource("ChronoBorderInspection12s");
                    ChronoData.Inspection = InspectionStates.OVER_12_SECONDS;
                    if (SettingsChronBeepCheckBox.IsChecked == true) Beep12sPlayer?.Play();
                }
                else if (ChronoData.Inspection == InspectionStates.OVER_12_SECONDS && ChronoData.Chronometer.ElapsedMilliseconds >= 15000)
                {
                    ChronoBorder.Style = (Style)FindResource("ChronoBorderInspection15s");
                    ChronoData.Inspection = InspectionStates.OVER_15_SECONDS;
                    if (SettingsChronBeepCheckBox.IsChecked == true) Beep15sPlayer?.Play();
                }

                else if (ChronoData.Inspection == InspectionStates.OVER_15_SECONDS && ChronoData.Chronometer.ElapsedMilliseconds >= 17000)
                {
                    ChronoData.Inspection = InspectionStates.DNF;
                    if (SettingsChronBeepCheckBox.IsChecked == true) BeepDNFPlayer?.Play();
                }
            }
            else if (ChronoData.State == ChronoStates.RUNNING)
            {
                ChronoLabelText.Text = AMTexts.MilliseconsToString(ChronoData.Chronometer.ElapsedMilliseconds);
            }
        }

        /// <summary>
        /// Got focus event: Change crono start button text
        /// </summary>
        private void ChronoButton_GotFocus(object sender, RoutedEventArgs e)
        {
            ChronoButtonText.Text = AMTexts.Message("KeepPressedMessage");
        }

        /// <summary>
        /// Lost focus event: Change crono start button text
        /// </summary>
        private void ChronoButton_LostFocus(object sender, RoutedEventArgs e)
        {
            if (ChronoData.State == ChronoStates.RUNNING)
                ChronoButtonText.Text = AMTexts.Message("ClickToStopChronoMessage");
            else
                ChronoButtonText.Text = AMTexts.Message("ClickToEnableChronoMessage");
        }

        /// <summary>
        /// Preview mouse down event: If chrono is running, stop the chrono with the start button
        /// </summary>
        private void ChronoButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ChronoData.State == ChronoStates.RUNNING)
            {
                ChronoData.Result.MeasuredTime = ChronoData.Chronometer.ElapsedMilliseconds;
                ChronoData.Chronometer.Stop();
                ChronoData.EventsTimerLauncher.Stop();

                if (SettingsChronBeepCheckBox.IsChecked == true) BeepEndPlayer?.Play();

                StopVideoCapture();

                ChronoLabelText.Text = AMTexts.MilliseconsToString(ChronoData.Result.MeasuredTime);
                ChronoButtonText.Text = AMTexts.Message("CompletedMeasureMessage");
                ChronoData.State = ChronoStates.FINISHED;
                ChronoBorder.Style = (Style)FindResource("ChronoBorderFinished");
                ChronoData.ResultNotSavedFlag = true;
                ChronoData.Result.Scramble = ChronoData.Scramble.GetText(" ");
                ChronoData.Result.SolvingType = (ResolutionTypes)ChronoTypeCombo.SelectedIndex;
                ChronoData.Result.Cube = AMSettings.DefaultCube;
                ChronoData.Result.Comment = AMSettings.DefaultComment;
                ShowWindowChronoResult();
            }
        }

        /// <summary>
        /// Preview key up event: Start chrono with space key
        /// </summary>
        private void ChronoButton_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space && ChronoButton.IsFocused)
            {
                switch (ChronoData.State)
                {
                    case ChronoStates.PRE_START:
                        ChronoData.EventsTimerLauncher.Stop();
                        ChronoButtonText.Text = AMTexts.Message("KeepPressedMessage");
                        ChronoData.State = ChronoStates.INITIAL_WAIT;
                        ChronoBorder.Style = (Style)FindResource("ChronoBorderInitialWait");
                        EnableDisableControlsWhileChronoRun(true);
                        break;
                    case ChronoStates.WAITING_START:
                        ChronoData.Chronometer.Restart();
                        ChronoData.Result.StartTime = DateTime.Now;
                        ChronoData.EventsTimerLauncher.Stop();
                        ChronoData.EventsTimerLauncher.Interval = new TimeSpan(0, 0, 0, 0, 33); // 33 ms
                        ChronoData.EventsTimerLauncher.Start();
                        ChronoButtonText.Text = AMTexts.Message("ChronoRunningMessage");
                        ChronoData.State = ChronoStates.RUNNING;
                        ChronoBorder.Style = (Style)FindResource("ChronoBorderRunning");
                        if (SettingsChronBeepCheckBox.IsChecked == true) BeepStartPlayer?.Play();
                        break;
                    case ChronoStates.INSPECTION:
                        if (ChronoData.Chronometer.ElapsedMilliseconds > 17000)
                        {
                            ChronoData.EventsTimerLauncher.Stop();
                            ChronoData.Chronometer.Stop();
                            StopVideoCapture();
                            ChronoData.State = ChronoStates.FINISHED;
                            ChronoBorder.Style = (Style)FindResource("ChronoBorderFinished");

                            ChronoData.Result.StartDelayDNF = true;
                            ChronoData.Result.StartDelayPenalty2s = false;
                            ChronoData.Result.MeasuredTime = 0;
                            ChronoData.Result.StartTime = DateTime.Now;
                            ChronoData.Result.Scramble = ChronoData.Scramble.GetText(" ");
                            ChronoData.Result.SolvingType = (ResolutionTypes)ChronoTypeCombo.SelectedIndex;
                            ChronoData.Result.Cube = AMSettings.DefaultCube;
                            ChronoData.Result.Comment = AMSettings.DefaultComment;

                            ChronoLabelText.Text = AMTexts.MilliseconsToString(ChronoData.Result.MeasuredTime);
                            ChronoButtonText.Text = AMTexts.Message("CompletedMeasureMessage");
                            ChronoData.ResultNotSavedFlag = true;
                            ShowWindowChronoResult();
                        }
                        else if (ChronoData.Chronometer.ElapsedMilliseconds > 15000)
                        {
                            ChronoData.Chronometer.Restart();
                            ChronoData.Result.StartTime = DateTime.Now;
                            ChronoData.Result.StartDelayPenalty2s = true;
                            ChronoData.Result.StartDelayDNF = false;
                            ChronoData.EventsTimerLauncher.Stop();
                            ChronoData.EventsTimerLauncher.Interval = new TimeSpan(0, 0, 0, 0, 33); // 33 ms
                            ChronoData.EventsTimerLauncher.Start();
                            ChronoButtonText.Text = AMTexts.Message("ChronoRunningMessage");
                            ChronoData.State = ChronoStates.RUNNING;
                            ChronoBorder.Style = (Style)FindResource("ChronoBorderRunning");
                            if (SettingsChronBeepCheckBox.IsChecked == true) BeepStartPlayer?.Play();
                        }
                        else
                        {
                            ChronoData.Chronometer.Restart();
                            ChronoData.Result.StartTime = DateTime.Now;
                            ChronoData.Result.StartDelayPenalty2s = false;
                            ChronoData.Result.StartDelayDNF = false;
                            ChronoData.EventsTimerLauncher.Stop();
                            ChronoData.EventsTimerLauncher.Interval = new TimeSpan(0, 0, 0, 0, 33); // 33 ms
                            ChronoData.EventsTimerLauncher.Start();
                            ChronoButtonText.Text = AMTexts.Message("ChronoRunningMessage");
                            ChronoData.State = ChronoStates.RUNNING;
                            ChronoBorder.Style = (Style)FindResource("ChronoBorderRunning");
                            if (SettingsChronBeepCheckBox.IsChecked == true) BeepStartPlayer?.Play();
                        }
                        break;
                }
                e.Handled = true;
            }
        }

        /// <summary>
        /// Preview key down event: Pre-start or stop chrono with space key
        /// </summary>
        private void ChronoButton_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space && ChronoButton.IsFocused)
            {
                switch (ChronoData.State)
                {
                    case ChronoStates.INITIAL_WAIT:
                        ChronoData.EventsTimerLauncher.Interval = new TimeSpan(0, 0, 0, 0, 500); // 0,5 s
                        ChronoData.EventsTimerLauncher.Start();
                        ChronoButtonText.Text = AMTexts.Message("KeepSpacePressedMessage");
                        ChronoData.State = ChronoStates.PRE_START;
                        ChronoData.Inspection = InspectionStates.PRE_INSPECTION;
                        ChronoBorder.Style = (Style)FindResource("ChronoBorderPreStart");
                        EnableDisableControlsWhileChronoRun(false);
                        break;
                    case ChronoStates.RUNNING:
                        ChronoData.Chronometer.Stop();
                        ChronoData.EventsTimerLauncher.Stop();

                        if (SettingsChronBeepCheckBox.IsChecked == true) BeepEndPlayer?.Play();

                        StopVideoCapture();

                        ChronoData.Result.MeasuredTime = ChronoData.Chronometer.ElapsedMilliseconds;
                        ChronoData.Result.Scramble = ChronoData.Scramble.GetText(" ");
                        ChronoData.Result.SolvingType = (ResolutionTypes)ChronoTypeCombo.SelectedIndex;
                        ChronoData.Result.Cube = AMSettings.DefaultCube;
                        ChronoData.Result.Comment = AMSettings.DefaultComment;

                        ChronoLabelText.Text = AMTexts.MilliseconsToString(ChronoData.Result.MeasuredTime);
                        ChronoData.State = ChronoStates.FINISHED;
                        ChronoBorder.Style = (Style)FindResource("ChronoBorderFinished");
                        ChronoButtonText.Text = AMTexts.Message("CompletedMeasureMessage");
                        ChronoData.ResultNotSavedFlag = true;
                        ShowWindowChronoResult();
                        break;
                }
                e.Handled = true;
            }
        }

        #endregion Chrono run events

        #region Chrono table events

        /// <summary>
        /// Selection changed event: update chrono stadistics
        /// </summary>
        private void ChronoResultsTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateChronoStadistics();
        }

        /// <summary>
        /// Click event: load today times
        /// </summary>
        private void LoadTodayTimesButton_Click(object sender, RoutedEventArgs e)
        {
            LoadDateTimesPicker.SelectedDate = DateTime.Now;
        }

        /// <summary>
        /// Click event: Clear times table
        /// </summary>
        private void ClearTimesTableButton_Click(object sender, RoutedEventArgs e)
        {
            ChronoResultsTable.Items.Clear();
        }

        /// <summary>
        /// Selected date changed event: load date times
        /// </summary>
        private void LoadDateTimesPicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadChronoResults(LoadDateTimesPicker.SelectedDate ?? DateTime.Now);
            UpdateChronoStadistics();
        }

        /// <summary>
        /// Click event: Change sort column order in chrono table
        /// </summary>
        private void ChronoResultsDataView_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = e.OriginalSource as GridViewColumnHeader;

            if (column == null || column.Column == null) return;

            if (ChronoData.TableSortColumn == column) // Toggle sorting direction
            {
                ChronoData.TableSortDirection = ChronoData.TableSortDirection == ListSortDirection.Ascending ?
                                                ListSortDirection.Descending : ListSortDirection.Ascending;
            }
            else
            {
                // Remove arrow from previously sorted header 
                if (ChronoData.TableSortColumn != null)
                {
                    ChronoData.TableSortColumn.Column.HeaderTemplate = null;
                    ChronoData.TableSortColumn.Column.Width = ChronoData.TableSortColumn.ActualWidth - 20;
                }
                ChronoData.TableSortColumn = column;
                ChronoData.TableSortDirection = ListSortDirection.Ascending;
                column.Column.Width = column.ActualWidth + 20;
            }

            if (ChronoData.TableSortDirection == ListSortDirection.Ascending)
                column.Column.HeaderTemplate = (DataTemplate)FindResource("ArrowUp");
            else
                column.Column.HeaderTemplate = (DataTemplate)FindResource("ArrowDown");

            string header;

            // If binding is used and property name doesn't match header content 
            if (ChronoData.TableSortColumn.Column.DisplayMemberBinding is Binding b) header = b.Path.Path;
            else header = string.Empty;

            ICollectionView resultDataView = CollectionViewSource.GetDefaultView(ChronoResultsTable.Items);

            resultDataView.SortDescriptions.Clear();
            resultDataView.SortDescriptions.Add(new SortDescription(header, ChronoData.TableSortDirection));
        }

        /// <summary>
        /// Double click event: load selected scramble for solving
        /// </summary>
        protected void ChronoResultsTable_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ChronoResultsTable.SelectedItem == null) return;

            if (ChronoData.ResultNotSavedFlag) // If there is no saved data, confirm operation
            {
                if (MessageBox.Show(AMTexts.Message("DiscardingLastResultMessage"),
                                    AMTexts.Message("LastResultNotSavedMessage"),
                                    MessageBoxButton.YesNo,
                                    MessageBoxImage.Question) == MessageBoxResult.No)
                {
                    ShowWindowChronoResult();
                    return;
                }
                ChronoData.ResultNotSavedFlag = false;
            }
            ChronoData.Scramble.Reset();
            ChronoData.Scramble.ParseScramble(((ResultDataRow)(ChronoResultsTable.SelectedItem)).ChronoScramble);

            ChronoData.ScramblePosition = ChronoData.ScramblePosTarget = ChronoData.Scramble.Length;
            UpdateChronoViews();
            ChronoButton.Focus();
        }

        #endregion Chrono table events

        #region Other chrono tab events

        /// <summary>
        /// Selection changed event: update chrono stadistics
        /// </summary>
        private void ChronoTypeStatsCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateChronoStadistics();
        }

        #endregion Other chrono tab events

        #region Chrono tools events

        /// <summary>
        /// Click event: export all saved times to CSV file
        /// </summary>
        private void ChronoExportAllCSVMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(AMSettings.SolvesDBPath)) return;

            Microsoft.Win32.SaveFileDialog CSVSaveDialog = new Microsoft.Win32.SaveFileDialog
            {
                FileName = "Export", // Default file name
                DefaultExt = ".csv", // Default file extension
                Filter = "CSV documents (.csv)|*.csv" // Filter files by extension
            };

            if (CSVSaveDialog.ShowDialog() ?? true)
            {
                try { File.Delete(CSVSaveDialog.FileName); }
                catch
                {
                    AMSettings.Status = AMTexts.Message("CantCreateCSVFileMessage") + CSVSaveDialog.FileName;
                    return;
                }

                StringBuilder CSV = new StringBuilder();

                CSV.Append("Date-Time");
                CSV.Append(AMSettings.CSVSeparator);
                CSV.Append("Chronometer time (ms)");
                CSV.Append(AMSettings.CSVSeparator);
                CSV.Append("Penalty");
                CSV.Append(AMSettings.CSVSeparator);
                CSV.Append("Scramble");
                CSV.Append(AMSettings.CSVSeparator);
                CSV.Append("Solve type");
                CSV.Append(AMSettings.CSVSeparator);
                CSV.Append("Cube");
                CSV.Append(AMSettings.CSVSeparator);
                CSV.Append("Comment");
                CSV.Append(Environment.NewLine);

                // Open solves data base file
                using (LiteDatabase SolvesDB = new LiteDatabase(AMSettings.SolvesDBPath))
                {
                    // Get solves collection
                    var SolvesCol = SolvesDB.GetCollection<ResultDB>("solves");

                    var AllSolves = SolvesCol.FindAll();

                    foreach (ResultDB R in AllSolves)
                    {
                        CSV.Append(R.ChronoDateTime.ToString());
                        CSV.Append(AMSettings.CSVSeparator);
                        CSV.Append(R.ChronoTime);
                        CSV.Append(AMSettings.CSVSeparator);
                        CSV.Append(R.ChronoPenalty);
                        CSV.Append(AMSettings.CSVSeparator);
                        CSV.Append(R.ChronoScramble);
                        CSV.Append(AMSettings.CSVSeparator);
                        CSV.Append(R.ChronoType);
                        CSV.Append(AMSettings.CSVSeparator);
                        CSV.Append(R.ChronoCube);
                        CSV.Append(AMSettings.CSVSeparator);
                        CSV.Append(R.ChronoComment);
                        CSV.Append(Environment.NewLine);
                    }
                }

                File.WriteAllText(CSVSaveDialog.FileName, CSV.ToString());
                AMSettings.Status = AMTexts.Message("CSVFileCreatedMessage") + CSVSaveDialog.FileName;
            }
        }

        /// <summary>
        /// Click event: export current table times to CSV file
        /// </summary>
        private void ChronoExportTableCSVMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog CSVSaveDialog = new Microsoft.Win32.SaveFileDialog
            {
                FileName = "Export", // Default file name
                DefaultExt = ".csv", // Default file extension
                Filter = "CSV documents (.csv)|*.csv" // Filter files by extension
            };

            if (CSVSaveDialog.ShowDialog() ?? true)
            {
                try { File.Delete(CSVSaveDialog.FileName); }
                catch
                {
                    AMSettings.Status = AMTexts.Message("CantCreateCSVFileMessage") + CSVSaveDialog.FileName;
                    return;
                }

                StringBuilder CSV = new StringBuilder();

                CSV.Append("Date-Time");
                CSV.Append(AMSettings.CSVSeparator);
                CSV.Append("Chronometer time (ms)");
                CSV.Append(AMSettings.CSVSeparator);
                CSV.Append("Penalty");
                CSV.Append(AMSettings.CSVSeparator);
                CSV.Append("Scramble");
                CSV.Append(AMSettings.CSVSeparator);
                CSV.Append("Solve type");
                CSV.Append(AMSettings.CSVSeparator);
                CSV.Append("Cube");
                CSV.Append(AMSettings.CSVSeparator);
                CSV.Append("Comment");
                CSV.Append(Environment.NewLine);

                foreach (object TableRow in ChronoResultsTable.Items)
                {
                    CSV.Append(((ResultDataRow)TableRow).ChronoDateTime);
                    CSV.Append(AMSettings.CSVSeparator);
                    CSV.Append(AMTexts.StringToMilliseconds(((ResultDataRow)TableRow).ChronoTime));
                    CSV.Append(AMSettings.CSVSeparator);
                    CSV.Append(((ResultDataRow)TableRow).ChronoPenalty);
                    CSV.Append(AMSettings.CSVSeparator);
                    CSV.Append(((ResultDataRow)TableRow).ChronoScramble);
                    CSV.Append(AMSettings.CSVSeparator);
                    CSV.Append(((ResultDataRow)TableRow).ChronoType);
                    CSV.Append(AMSettings.CSVSeparator);
                    CSV.Append(((ResultDataRow)TableRow).ChronoCube);
                    CSV.Append(AMSettings.CSVSeparator);
                    CSV.Append(((ResultDataRow)TableRow).ChronoComment);
                    CSV.Append(Environment.NewLine);
                }

                File.WriteAllText(CSVSaveDialog.FileName, CSV.ToString());
                AMSettings.Status = AMTexts.Message("CSVFileCreatedMessage") + CSVSaveDialog.FileName;
            }
        }

        /// <summary>
        /// Click event: update in data base cube and comment in current selected solve in table
        /// </summary>
        private void ChronoToolEditSolveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (ChronoResultsTable.SelectedItem == null) return;

            SolveEditWindow SEWindow = new SolveEditWindow
            {
                NewCube = ((ResultDataRow)(ChronoResultsTable.SelectedItem)).ChronoCube,
                NewComment = ((ResultDataRow)(ChronoResultsTable.SelectedItem)).ChronoComment,
                Owner = AlgorithmMasterWindow
            };

            if (SEWindow.ShowDialog() ?? true)
            {
                // Open solves data base file
                using (LiteDatabase SolvesDB = new LiteDatabase(AMSettings.SolvesDBPath))
                {
                    // Get solves collection
                    var SolvesCol = SolvesDB.GetCollection<ResultDB>("solves");

                    var Res = SolvesCol.Find(x => x.ChronoDateTime == DateTime.Parse(((ResultDataRow)(ChronoResultsTable.SelectedItem)).ChronoDateTime));

                    if (Res != null && Res.Count() > 0)
                    {
                        foreach (ResultDB R in Res)
                        {
                            R.ChronoCube = SEWindow.NewCube;
                            R.ChronoComment = SEWindow.NewComment;
                            // Update solve in data base
                            SolvesCol.Update(R);
                        }
                    }
                }
                int LastIndex = ChronoResultsTable.SelectedIndex;
                LoadChronoResults(LoadDateTimesPicker.SelectedDate ?? DateTime.Now);
                if (LastIndex < ChronoResultsTable.Items.Count) ChronoResultsTable.SelectedIndex = LastIndex;
            }
        }

        /// <summary>
        /// Click event: delete from data base current selected solve in table
        /// </summary>
        private void ChronoToolDeleteSolveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (ChronoResultsTable.Items.Count <= 0 || ChronoResultsTable.SelectedIndex < 0) return;

            if (!DateTime.TryParse(((ResultDataRow)ChronoResultsTable.SelectedItem).ChronoDateTime, out DateTime dt)) return;

            if (MessageBox.Show(string.Format(AMTexts.Message("DeleteSolveMessage"), dt.ToString()),
                                AMTexts.Message("DeleteSolveTitleMessage"),
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                // Open solves data base file
                using (LiteDatabase SolvesDB = new LiteDatabase(AMSettings.SolvesDBPath))
                {
                    // Get solves collection
                    var SolvesCol = SolvesDB.GetCollection<ResultDB>("solves");

                    // Delete solve from data base
                    SolvesCol.Delete(x => x.ChronoDateTime == dt);
                }

                UpdateCalendarBlackoutDays();
                LoadChronoResults(LoadDateTimesPicker.SelectedDate ?? DateTime.Now);
                UpdateChronoStadistics();
                UpdateChartData();
            }
        }

        #endregion Chrono tools events

        #region Chrono video events

        /// <summary>
        /// Camera combo box selection event
        /// </summary>
        private void CamerasComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try // Stops camera
            {
                if (ChronoData.Camera?.IsRunning == true) ChronoData.Camera.Stop();
                ChronoData.Camera?.Dispose();
            }
            catch (Exception ex) { AMSettings.Log = "Camera exception: " + ex.Message; }
            
            CameraPlayButton.IsEnabled = true;
        }

        /// <summary>
        /// Clcik event: starts camera
        /// </summary>
        private void CameraPlayButton_Click(object sender, RoutedEventArgs e)
        {
            try // New camera
            {
                ChronoData.Camera = new CapDevice(ChronoData.CameraMonikers[CamerasComboBox.SelectedIndex].MonikerString)
                {
                    Framerate = AMSettings.VideoFrameRate
                };
                ChronoData.Camera.NewBitmapReady += ChronoCamera_NewBitmapReady;
            }
            catch (Exception ex) { AMSettings.Log = "Camera exception: " + ex.Message; }
        }

        /// <summary>
        /// Click event: stops camera
        /// </summary>
        private void CameraStopButton_Click(object sender, RoutedEventArgs e)
        {
            CamerasComboBox.IsEnabled = true;
            CameraPlayButton.IsEnabled = true;
            CameraStopButton.IsEnabled = false;
            SaveVideoCheckBox.IsChecked = false;
            SaveVideoCheckBox.IsEnabled = false;

            try // Stops the camera
            {
                ChronoData.VideoTimer?.Stop();
                if (ChronoData.Camera?.IsRunning == true) ChronoData.Camera.Stop();
                ChronoData.VideoWriter?.Close();
                ChronoData.VideoWriter = null;
            }
            catch (Exception ex) { AMSettings.Log = "Stopping camera: " + ex.Message; }
        }

        /// <summary>
        /// This event is only called with the first frame 
        /// </summary>
        private void ChronoCamera_NewBitmapReady(object sender, EventArgs e)
        {
            CamImage.Source = ChronoData.Camera?.BitmapSource;
            CamerasComboBox.IsEnabled = false;
            CameraPlayButton.IsEnabled = false;
            CameraStopButton.IsEnabled = true;
            SaveVideoCheckBox.IsEnabled = true;
            ChronoData.CameraReadyFlag = true;
        }

        /// <summary>
        /// DispatcherTimer Tick event to capture frames
        /// </summary>
        private void ChronoCameraVideo_Tick(object sender, EventArgs e)
        {
            if (ChronoData.Camera?.IsRunning == true && ChronoData.VideoStream != null)
            {
                int FrameWidth = (int)ChronoData.Camera.BitmapSource.Width;
                int FrameHeight = (int)ChronoData.Camera.BitmapSource.Height;

                // Create the new BitmapSource that will be used to rotate the source.
                TransformedBitmap RotatedBitmapSource = new TransformedBitmap();

                RotatedBitmapSource.BeginInit();
                RotatedBitmapSource.Source = ChronoData.Camera.BitmapSource;
                RotatedBitmapSource.Transform = new RotateTransform(180);
                RotatedBitmapSource.EndInit();

                // Write text over video
                DrawingVisual TextVisual = new DrawingVisual();
                using (DrawingContext drawingContext = TextVisual.RenderOpen())
                {
                    drawingContext.DrawImage(RotatedBitmapSource, new Rect(0, 0, FrameWidth, FrameHeight));

                    FormattedText DateTimeText = new FormattedText(DateTime.Now.ToString(),
                                                                   CultureInfo.InvariantCulture,
                                                                   FlowDirection.LeftToRight,
                                                                   new Typeface("Segoe UI"),
                                                                   24,
                                                                   Brushes.DarkGreen);//,
                                                                   //VisualTreeHelper.GetDpi(this).PixelsPerDip);
                    FormattedText SolveTimeText = new FormattedText(ChronoLabelText.Text,
                                                                   CultureInfo.InvariantCulture,
                                                                   FlowDirection.LeftToRight,
                                                                   new Typeface("Segoe UI"),
                                                                   24,
                                                                   Brushes.DarkGreen);//,
                                                                   //VisualTreeHelper.GetDpi(this).PixelsPerDip);

                    drawingContext.DrawRectangle(Brushes.White,
                                                 new Pen(Brushes.LightGray, 2d),
                                                 new Rect(2d, 2d, DateTimeText.Width, DateTimeText.Height));
                    drawingContext.DrawRectangle(Brushes.White,
                                                 new Pen(Brushes.LightGray, 2d),
                                                 new Rect(2d, FrameHeight - SolveTimeText.Height - 2d, SolveTimeText.Width, SolveTimeText.Height));

                    drawingContext.DrawText(DateTimeText, new Point(2d, 2d));
                    drawingContext.DrawText(SolveTimeText, new Point(2d, FrameHeight - SolveTimeText.Height - 2d));
                }
                RenderTargetBitmap TextBitmapSource = new RenderTargetBitmap(FrameWidth, FrameHeight, 96d, 96d, PixelFormats.Pbgra32);
                TextBitmapSource.Render(TextVisual);

                int Stride = TextBitmapSource.PixelWidth *
                             ((TextBitmapSource.Format.BitsPerPixel + 7) / 8);

                byte[] Pixels = new byte[TextBitmapSource.PixelHeight * Stride];

                TextBitmapSource.CopyPixels(Pixels, Stride, 0);

                try
                {
                    ChronoData.VideoStream.WriteFrame(true, // Key frame
                                           Pixels, // Array with frame data
                                           0, // Starting index in the array
                                           Pixels.Length); // Data length
                }
                catch (Exception ex) { AMSettings.Log = "Video frame: " + ex.Message; }
            }
        }

        /// <summary>
        /// Unchecked event: stpos video
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveVideoCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            StopVideoCapture();
            ChronoData.CameraReadyFlag = false;
        }

        #endregion Chrono video events

        #endregion Chronometer events

        #region Progress functions

        /// <summary>
        /// Initialize progress chart data
        /// </summary>
        private void InitializeProgress()
        {
            ChartData = new List<ProgressData>();
            ProgressPeriodCombo.SelectedIndex = 0; // Daily by default

            ProgressStartDateTimesPicker.SelectedDate = ProgressStartDateTimesPicker.DisplayDateStart;
            ProgressEndDateTimesPicker.SelectedDate = ProgressEndDateTimesPicker.DisplayDateEnd;
        }

        /// <summary>
        /// Updates and draws progress chart
        /// </summary>
        public void UpdateChartData()
        {
            if (!AppInitialized || ChartData == null ||
                ProgressTypeCombo.SelectedItem == null ||
                ProgressStartDateTimesPicker.SelectedDate == null ||
                ProgressEndDateTimesPicker.SelectedDate == null
                ) return;

            ChartData.Clear();
            ProgressChartCanvas.Children.Clear();
            ProgressBarsCanvas.Children.Clear();

            if (ProgressBarsCheckBox.IsChecked == true) ProgressBarsCanvas.Visibility = Visibility.Visible;
            else ProgressBarsCanvas.Visibility = Visibility.Collapsed;

            // Open solves data base file
            using (LiteDatabase SolvesDB = new LiteDatabase(AMSettings.SolvesDBPath))
            {
                LiteCollection<ResultDB> SolvesCol;
                IEnumerable<ResultDB> ChartSolves;

                int MaxSolvesPeriod = 0;

                try
                {
                    // Get solves collection
                    SolvesCol = SolvesDB.GetCollection<ResultDB>("solves");

                    // Find chart solves
                    ChartSolves = SolvesCol.FindAll()
                        .Where(x => x.ChronoDateTime.Date >= ((DateTime)ProgressStartDateTimesPicker.SelectedDate).Date &&
                                    x.ChronoDateTime.Date <= ((DateTime)ProgressEndDateTimesPicker.SelectedDate).Date &&
                                    string.Compare(x.ChronoType, ProgressTypeCombo.SelectedItem.ToString()) == 0 &&
                                    !x.ChronoPenalty.Contains(AMTexts.Message("DNFMessage")) &&
                                    !x.ChronoPenalty.Contains(AMTexts.Message("DNSMessage")))
                        .OrderBy(x => x.ChronoDateTime);
                }
                catch (Exception ex)
                {
                    AMSettings.Log = "Database fault: " + ex.Message;
                    return;
                }

                if (ChartSolves.Count() < 2)
                {
                    AMSettings.Status = AMTexts.Message("ProgressNotEnoughPointsMessage");
                    return;
                }

                DateTime LastResultDay = DateTime.MinValue.Date;
                int WeekNumber, WeekYear, LastWeekNumber = 0, LastWeekYear = 0;

                List<int> PeriodTimes = new List<int>(); // Times list for a period (day, week, month or year)

                foreach (ResultDB R in ChartSolves)
                {
                    switch (ProgressPeriodCombo.SelectedIndex)
                    {
                        case 0: // Daily
                            if (LastResultDay == DateTime.MinValue.Date) // First day
                            {
                                LastResultDay = R.ChronoDateTime.Date;
                                PeriodTimes.Clear();
                                PeriodTimes.Add(R.ChronoTime);
                            }
                            else
                            {
                                if (LastResultDay == R.ChronoDateTime.Date) PeriodTimes.Add(R.ChronoTime);
                                else // Current day data full
                                {
                                    if (PeriodTimes.Count >= AMSettings.MinimumSolvesPeriod)
                                    {
                                        // Evaluate stadistics
                                        if (GetStadistics(PeriodTimes, out int Max, out int Min, out int Avg, out int StdDev, out int Mdm))
                                        {
                                            ChartData.Add(new ProgressData
                                            {
                                                Labels = LastResultDay.ToShortDateString(),
                                                Min = Min,
                                                Max = Max,
                                                Average = Avg,
                                                StdDeviation = StdDev,
                                                Medium = Mdm,
                                                NumSolves = PeriodTimes.Count
                                            });
                                            if (PeriodTimes.Count > MaxSolvesPeriod) MaxSolvesPeriod = PeriodTimes.Count;
                                        }
                                    }
                                    // Next day
                                    LastResultDay = R.ChronoDateTime.Date;
                                    PeriodTimes.Clear();
                                    PeriodTimes.Add(R.ChronoTime);
                                }
                            }
                            break;

                        case 1: // Weekly
                            if (LastWeekNumber == 0) // First week
                            {
                                LastWeekNumber = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(R.ChronoDateTime.Date, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
                                LastWeekYear = LastWeekNumber == 52 && R.ChronoDateTime.Month == 1 ? R.ChronoDateTime.Year - 1 : R.ChronoDateTime.Year;
                                PeriodTimes.Clear();
                                PeriodTimes.Add(R.ChronoTime);
                            }
                            else
                            {
                                WeekNumber = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(R.ChronoDateTime.Date, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
                                WeekYear = WeekNumber == 52 && R.ChronoDateTime.Month == 1 ? R.ChronoDateTime.Year - 1 : R.ChronoDateTime.Year;

                                if (LastWeekNumber == WeekNumber && LastWeekYear == WeekYear) PeriodTimes.Add(R.ChronoTime);
                                else // Current week data full
                                {
                                    if (PeriodTimes.Count >= AMSettings.MinimumSolvesPeriod)
                                    {
                                        // Evaluate stadistics
                                        if (GetStadistics(PeriodTimes, out int Max, out int Min, out int Avg, out int StdDev, out int Mdm))
                                        {
                                            ChartData.Add(new ProgressData
                                            {
                                                Labels = LastWeekNumber.ToString() + " - " + LastWeekYear,
                                                Min = Min,
                                                Max = Max,
                                                Average = Avg,
                                                StdDeviation = StdDev,
                                                Medium = Mdm,
                                                NumSolves = PeriodTimes.Count
                                            });
                                            if (PeriodTimes.Count > MaxSolvesPeriod) MaxSolvesPeriod = PeriodTimes.Count;
                                        }
                                    }
                                    // Next week
                                    LastWeekNumber = WeekNumber;
                                    LastWeekYear = WeekYear;
                                    PeriodTimes.Clear();
                                    PeriodTimes.Add(R.ChronoTime);
                                }
                            }
                            break;

                        case 2: // Monthly
                            if (LastResultDay == DateTime.MinValue.Date) // First Month
                            {
                                LastResultDay = R.ChronoDateTime.Date;
                                PeriodTimes.Clear();
                                PeriodTimes.Add(R.ChronoTime);
                            }
                            else
                            {
                                if (LastResultDay.Month == R.ChronoDateTime.Month &&
                                    LastResultDay.Year == R.ChronoDateTime.Year) PeriodTimes.Add(R.ChronoTime);
                                else // Current month data full
                                {
                                    if (PeriodTimes.Count >= AMSettings.MinimumSolvesPeriod)
                                    {
                                        // Evaluate stadistics
                                        if (GetStadistics(PeriodTimes, out int Max, out int Min, out int Avg, out int StdDev, out int Mdm))
                                        {
                                            ChartData.Add(new ProgressData
                                            {
                                                Labels = LastResultDay.Month.ToString() + "/" + LastResultDay.Year,
                                                Min = Min,
                                                Max = Max,
                                                Average = Avg,
                                                StdDeviation = StdDev,
                                                Medium = Mdm,
                                                NumSolves = PeriodTimes.Count
                                            });
                                            if (PeriodTimes.Count > MaxSolvesPeriod) MaxSolvesPeriod = PeriodTimes.Count;
                                        }
                                    }
                                    // Next month
                                    LastResultDay = R.ChronoDateTime.Date;
                                    PeriodTimes.Clear();
                                    PeriodTimes.Add(R.ChronoTime);
                                }
                            }
                            break;

                        case 3: // Annualy
                            if (LastResultDay == DateTime.MinValue.Date) // First year
                            {
                                LastResultDay = R.ChronoDateTime.Date;
                                PeriodTimes.Clear();
                                PeriodTimes.Add(R.ChronoTime);
                            }
                            else
                            {
                                if (LastResultDay.Year == R.ChronoDateTime.Year) PeriodTimes.Add(R.ChronoTime);
                                else // Current year data full
                                {
                                    if (PeriodTimes.Count >= AMSettings.MinimumSolvesPeriod)
                                    {
                                        // Evaluate stadistics
                                        if (GetStadistics(PeriodTimes, out int Max, out int Min, out int Avg, out int StdDev, out int Mdm))
                                        {
                                            ChartData.Add(new ProgressData
                                            {
                                                Labels = LastResultDay.Year.ToString(),
                                                Min = Min,
                                                Max = Max,
                                                Average = Avg,
                                                StdDeviation = StdDev,
                                                Medium = Mdm,
                                                NumSolves = PeriodTimes.Count
                                            });
                                            if (PeriodTimes.Count > MaxSolvesPeriod) MaxSolvesPeriod = PeriodTimes.Count;
                                        }
                                    }
                                    // Next year
                                    LastResultDay = R.ChronoDateTime.Date;
                                    PeriodTimes.Clear();
                                    PeriodTimes.Add(R.ChronoTime);
                                }
                            }
                            break;
                    }
                }

                // Add last period
                if (PeriodTimes.Count >= AMSettings.MinimumSolvesPeriod)
                {
                    // Evaluate stadistics
                    if (GetStadistics(PeriodTimes, out int Max, out int Min, out int Avg, out int StdDev, out int Mdm))
                    {
                        string PeriodLabel;
                        switch (ProgressPeriodCombo.SelectedIndex)
                        {
                            case 0: PeriodLabel = LastResultDay.ToShortDateString(); break;
                            case 1: PeriodLabel = LastWeekNumber.ToString() + " - " + LastWeekYear; break;
                            case 2: PeriodLabel = LastResultDay.Month.ToString() + "/" + LastResultDay.Year; break;
                            case 3: PeriodLabel = LastResultDay.Year.ToString(); break;
                            default: PeriodLabel = string.Empty; break;
                        }

                        ChartData.Add(new ProgressData
                        {
                            Labels = PeriodLabel,
                            Min = Min,
                            Max = Max,
                            Average = Avg,
                            StdDeviation = StdDev,
                            Medium = Mdm,
                            NumSolves = PeriodTimes.Count
                        });

                        if (PeriodTimes.Count > MaxSolvesPeriod) MaxSolvesPeriod = PeriodTimes.Count;
                    }
                }

                if (ChartData.Count > 1)
                {
                    // Draw vertical scale and horizontal lines

                    int MaxTime = 0;
                    foreach (ProgressData M in ChartData)
                    {
                        if ((ProgressMaximumCheckBox.IsChecked ?? true) && M.Max > MaxTime) MaxTime = M.Max;
                        if ((ProgressMinimumCheckBox.IsChecked ?? true) && M.Min > MaxTime) MaxTime = M.Min;
                        if (ProgressAverageCheckBox.IsChecked ?? true)
                        {
                            if (M.Average > MaxTime) MaxTime = M.Average;
                            if ((ProgressDeviationCheckBox.IsChecked ?? true) && 
                                M.Average + M.StdDeviation > MaxTime)
                                MaxTime = M.Average + M.StdDeviation;
                        }
                        if ((ProgressMediumCheckBox.IsChecked ?? true) && M.Medium > MaxTime) MaxTime = M.Medium;
                    }

                    // Calculate the vertical display values range
                    int VertRange = 1000, AuxRange, RangeSteps = 11;
                    while (MaxTime / VertRange > 0) VertRange *= 10;
                    AuxRange = VertRange / 10;
                    while (VertRange > MaxTime) VertRange -= AuxRange;
                    VertRange += 2 * AuxRange;

                    TextBlock[] YLegend = new TextBlock[RangeSteps];
                    for (int n = 0; n < YLegend.Length; n++)
                    {
                        double perc = (double)n / (YLegend.Length - 1);
                        YLegend[n] = new TextBlock
                        {
                            Text = AMTexts.MilliseconsToString((long)(VertRange * perc)),
                            Foreground = AMSettings.GetBrush(Colors.Black),
                            Background = AMSettings.GetBrush(Colors.Transparent),
                            FontSize = 11
                        };
                        Point TPos = AMSettings.GetChartPosition(ProgressChartCanvas.ActualWidth,
                                                                 ProgressChartCanvas.ActualHeight,
                                                                 0d, perc);
                        Canvas.SetLeft(YLegend[n], 0);
                        Canvas.SetTop(YLegend[n], TPos.Y - YLegend[n].FontSize / 1.4);
                        ProgressChartCanvas.Children.Add(YLegend[n]);

                        PointCollection HorizPoints = new PointCollection()
                        {
                            AMSettings.GetChartPosition(ProgressChartCanvas.ActualWidth,
                                                        ProgressChartCanvas.ActualHeight,
                                                        0d, perc),
                            AMSettings.GetChartPosition(ProgressChartCanvas.ActualWidth,
                                                        ProgressChartCanvas.ActualHeight,
                                                        1d, perc)
                        };

                        ProgressChartCanvas.Children.Add(new Polyline
                        {
                            Stroke = AMSettings.GetBrush(Colors.Chocolate),
                            StrokeThickness = 0.75,
                            Points = HorizPoints,
                            StrokeDashArray = new DoubleCollection { 6d, 6d }
                        });
                    }

                    // Draw horizontal scale and vertical lines

                    TextBlock[] XLegend;
                    int Reduction = 1;

                    if (ProgressChartCanvas.ActualWidth > 10.0)
                        while (ChartData.Count / (Reduction * ProgressChartCanvas.ActualWidth) > AMSettings.ChartMaxDensity) Reduction++;

                    XLegend = new TextBlock[ChartData.Count];
                    
                    for (int n = 0; n < XLegend.Length; n++)
                    {
                        double perc = (double)n / (XLegend.Length - 1);

                        if (n % Reduction == 0  && (n + 1) != XLegend.Length - 1 || Reduction == 1 || n == XLegend.Length - 1)
                        {
                            XLegend[n] = new TextBlock
                            {
                                Text = ChartData[n].Labels,
                                Foreground = AMSettings.GetBrush(Colors.Black),
                                Background = AMSettings.GetBrush(Colors.Transparent),
                                FontSize = 11,
                                RenderTransform = new RotateTransform(45d)
                            };
                            Point TPos = AMSettings.GetChartPosition(ProgressChartCanvas.ActualWidth,
                                                                     ProgressChartCanvas.ActualHeight,
                                                                     perc, 0d);
                            Canvas.SetLeft(XLegend[n], TPos.X);
                            Canvas.SetBottom(XLegend[n], AMSettings.ChartMarginDown - 20d);
                            ProgressChartCanvas.Children.Add(XLegend[n]);

                            PointCollection VertPoints = new PointCollection()
                            {
                                AMSettings.GetChartPosition(ProgressChartCanvas.ActualWidth,
                                                            ProgressChartCanvas.ActualHeight,
                                                            perc, 0d),
                                AMSettings.GetChartPosition(ProgressChartCanvas.ActualWidth,
                                                            ProgressChartCanvas.ActualHeight,
                                                            perc, 1d)
                            };

                            ProgressChartCanvas.Children.Add(new Polyline
                            {
                                Stroke = AMSettings.GetBrush(Colors.Chocolate),
                                StrokeThickness = 0.75,
                                Points = VertPoints,
                                StrokeDashArray = new DoubleCollection { 6d, 6d }
                            });
                        }

                        if (MaxSolvesPeriod > 0 && ProgressBarsCheckBox.IsChecked == true) // Draw number of solves bars chart
                        {
                            PointCollection BarsPoints = new PointCollection()
                            {
                                AMSettings.GetBarsPosition(ProgressBarsCanvas.ActualWidth,
                                                            ProgressBarsCanvas.ActualHeight,
                                                            perc, 0d),
                                AMSettings.GetBarsPosition(ProgressBarsCanvas.ActualWidth,
                                                            ProgressBarsCanvas.ActualHeight,
                                                            perc, (double)ChartData[n].NumSolves / MaxSolvesPeriod)
                            };
                            ProgressBarsCanvas.Children.Add(new Polyline
                            {
                                Stroke = AMSettings.GetBrush((Color)ProgressAmountColorPicker.SelectedColor),
                                StrokeThickness = 5.0,
                                Points = BarsPoints
                            });
                        }
                    }

                    // Draw charts

                    PointCollection MaxPoints = new PointCollection();
                    PointCollection MinPoints = new PointCollection();
                    PointCollection AvgPoints = new PointCollection();
                    PointCollection StdPoints = new PointCollection();
                    PointCollection MdmPoints = new PointCollection();

                    int NumberOfPoints = ChartData.Count;
                    for (int n = 0; n < NumberOfPoints; n++)
                    {
                        MaxPoints.Add(AMSettings.GetChartPosition(ProgressChartCanvas.ActualWidth,
                                                                  ProgressChartCanvas.ActualHeight,
                                                                  (double)n / (NumberOfPoints - 1),
                                                                  (double)ChartData[n].Max / VertRange));
                        MinPoints.Add(AMSettings.GetChartPosition(ProgressChartCanvas.ActualWidth,
                                                                  ProgressChartCanvas.ActualHeight,
                                                                  (double)n / (NumberOfPoints - 1),
                                                                  (double)ChartData[n].Min / VertRange));
                        AvgPoints.Add(AMSettings.GetChartPosition(ProgressChartCanvas.ActualWidth,
                                                                  ProgressChartCanvas.ActualHeight,
                                                                  (double)n / (NumberOfPoints - 1),
                                                                  (double)ChartData[n].Average / VertRange));
                        StdPoints.Add(AMSettings.GetChartPosition(ProgressChartCanvas.ActualWidth,
                                                                  ProgressChartCanvas.ActualHeight,
                                                                  (double)n / (NumberOfPoints - 1),
                                                                  (double)(ChartData[n].Average + ChartData[n].StdDeviation) / VertRange));
                        MdmPoints.Add(AMSettings.GetChartPosition(ProgressChartCanvas.ActualWidth,
                                                                  ProgressChartCanvas.ActualHeight,
                                                                  (double)n / (NumberOfPoints - 1),
                                                                  (double)ChartData[n].Medium / VertRange));
                    }
                    for (int n = NumberOfPoints - 1; n >= 0; n--) // Complete standard deviation polygon
                        StdPoints.Add(AMSettings.GetChartPosition(ProgressChartCanvas.ActualWidth,
                                                                  ProgressChartCanvas.ActualHeight,
                                                                  (double)n / (NumberOfPoints - 1),
                                                                  (double)(ChartData[n].Average - ChartData[n].StdDeviation) / VertRange));


                    if (ProgressMaximumCheckBox.IsChecked ?? true)
                        ProgressChartCanvas.Children.Add(new Polyline
                        {
                            Stroke = AMSettings.GetBrush((Color)ProgressMaximumColorPicker.SelectedColor),
                            StrokeThickness = 1.5,
                            Points = MaxPoints,
                            StrokeStartLineCap = PenLineCap.Round,
                            StrokeEndLineCap = PenLineCap.Round
                        });

                    if (ProgressMinimumCheckBox.IsChecked ?? true)
                        ProgressChartCanvas.Children.Add(new Polyline
                        {
                            Stroke = AMSettings.GetBrush((Color)ProgressMinimumColorPicker.SelectedColor),
                            StrokeThickness = 1.5,
                            Points = MinPoints,
                            StrokeStartLineCap = PenLineCap.Round,
                            StrokeEndLineCap = PenLineCap.Round
                        });

                    if (ProgressAverageCheckBox.IsChecked ?? true)
                    {
                        ProgressChartCanvas.Children.Add(new Polyline
                        {
                            Stroke = AMSettings.GetBrush((Color)ProgressAverageColorPicker.SelectedColor),
                            StrokeThickness = 1.5,
                            Points = AvgPoints,
                            StrokeStartLineCap = PenLineCap.Round,
                            StrokeEndLineCap = PenLineCap.Round
                        });

                        if (ProgressDeviationCheckBox.IsChecked ?? true)
                        {
                            Color StrokeColor = (Color)ProgressDeviationColorPicker.SelectedColor;
                            StrokeColor.A = 63; // Transparency

                            Color FillColor = (Color)ProgressDeviationColorPicker.SelectedColor;
                            FillColor.A = 31; // Transparency

                            ProgressChartCanvas.Children.Add(new Polygon
                            {
                                Stroke = AMSettings.GetBrush(StrokeColor),
                                Fill = AMSettings.GetBrush(FillColor),
                                StrokeThickness = 0.5,
                                Points = StdPoints,
                            });
                        }
                    }
                    if (ProgressMediumCheckBox.IsChecked ?? true)
                        ProgressChartCanvas.Children.Add(new Polyline
                        {
                            Stroke = AMSettings.GetBrush((Color)ProgressMediumColorPicker.SelectedColor),
                            StrokeThickness = 1.5,
                            Points = MdmPoints,
                            StrokeStartLineCap = PenLineCap.Round,
                            StrokeEndLineCap = PenLineCap.Round
                        });
                }

                if (CurrentTab == MainTabNames.Progress)
                {
                    if (ChartData.Count <= 1) AMSettings.Status = AMTexts.Message("ProgressNotEnoughPointsMessage");
                    else AMSettings.Status = string.Format(AMTexts.Message("ProgressDrawMessage"),
                                                      ((DateTime)ProgressStartDateTimesPicker.SelectedDate).ToShortDateString(),
                                                      ((DateTime)ProgressEndDateTimesPicker.SelectedDate).ToShortDateString(),
                                                      ProgressPeriodCombo.SelectedItem.ToString());
                }
            }
        }

        /// <summary>
        /// Get stadistic data from a list of integers (data will be sorted)
        /// </summary>
        /// <param name="Data">List of integers</param>
        /// <param name="Max">Maximum value</param>
        /// <param name="Min">Minimum value</param>
        /// <param name="Avg">Average value</param>
        /// <param name="StdDev">Standard deviation</param>
        /// <param name="Mdm">Medium</param>
        /// <returns>True if stadistics are correctly calculated</returns>
        private static bool GetStadistics(List<int> Data, out int Max, out int Min, out int Avg, out int StdDev, out int Mdm)
        {
            Max = Min = Avg = StdDev = Mdm = 0;
            if (Data == null || Data.Count < 1) return false;

            Data.Sort(); // Ascending order

            Min = Data[0]; // Minimum
            Max = Data[Data.Count - 1]; // Maximum

            // Average
            int Totaling = 0;
            for (int n = 0; n < Data.Count; n++) Totaling += Data[n];
            Avg = Totaling / Data.Count;

            // Standard deviation
            if (Data.Count > 1)
            {
                double SDTotaling = 0.0;
                for (int n = 0; n < Data.Count; n++) SDTotaling += Math.Pow(Data[n] - Avg, 2.0);
                StdDev = (int)Math.Sqrt(SDTotaling / (Data.Count - 1));
            }
            else StdDev = 0;

            // Medium
            if (Data.Count % 2 == 0)
                Mdm = (Data[Data.Count / 2 - 1] + Data[Data.Count / 2]) / 2;
            else
                Mdm = Data[(Data.Count - 1) / 2];

            return true;
        }

        #endregion Progress functions

        #region Progress events

        #region Legend events

        /// <summary>
        /// Updates chart on resize
        /// </summary>
        private void ProgressChartCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateChartData();
        }

        /// <summary>
        /// Updates chart on check box change
        /// </summary>
        private void ProgressMaximumCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (ProgressMaximumColorPicker != null)
            {
                ProgressMaximumColorPicker.IsEnabled = true;
                UpdateChartData();
            }
        }

        /// <summary>
        /// Updates chart on check box change
        /// </summary>
        private void ProgressMaximumCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (ProgressMaximumColorPicker != null)
            {
                ProgressMaximumColorPicker.IsEnabled = false;
                UpdateChartData();
            }
        }

        /// <summary>
        /// Updates chart on check box change
        /// </summary>
        private void ProgressMinimumCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (ProgressMinimumColorPicker != null)
            {
                ProgressMinimumColorPicker.IsEnabled = true;
                UpdateChartData();
            }
        }

        /// <summary>
        /// Updates chart on check box change
        /// </summary>
        private void ProgressMinimumCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (ProgressMinimumColorPicker != null)
            {
                ProgressMinimumColorPicker.IsEnabled = false;
                UpdateChartData();
            }
        }

        /// <summary>
        /// Updates chart on check box change
        /// </summary>
        private void ProgressAverageCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (ProgressAverageColorPicker != null)
            {
                ProgressAverageColorPicker.IsEnabled = true;
                ProgressDeviationCheckBox.IsEnabled = true;
                if (ProgressDeviationCheckBox.IsChecked ?? true) ProgressDeviationColorPicker.IsEnabled = true;
                else ProgressDeviationColorPicker.IsEnabled = false;
                UpdateChartData();
            }
        }

        /// <summary>
        /// Updates chart on check box change
        /// </summary>
        private void ProgressAverageCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (ProgressAverageColorPicker != null)
            {
                ProgressAverageColorPicker.IsEnabled = false;
                ProgressDeviationCheckBox.IsEnabled = false;
                ProgressDeviationColorPicker.IsEnabled = false;
                UpdateChartData();
            }
        }

        /// <summary>
        /// Updates chart on check box change
        /// </summary>
        private void ProgressDeviationCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (ProgressDeviationColorPicker != null)
            {
                ProgressDeviationColorPicker.IsEnabled = true;
                UpdateChartData();
            }
        }

        /// <summary>
        /// Updates chart on check box change
        /// </summary>
        private void ProgressDeviationCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (ProgressDeviationColorPicker != null)
            {
                ProgressDeviationColorPicker.IsEnabled = false;
                UpdateChartData();
            }
        }

        /// <summary>
        /// Updates chart on check box change
        /// </summary>
        private void ProgressMediumCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (ProgressMediumColorPicker != null)
            {
                ProgressMediumColorPicker.IsEnabled = true;
                UpdateChartData();
            }
        }

        /// <summary>
        /// Updates chart on check box change
        /// </summary>
        private void ProgressMediumCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (ProgressMediumColorPicker != null)
            {
                ProgressMediumColorPicker.IsEnabled = false;
                UpdateChartData();
            }
        }

        /// <summary>
        /// Updates chart on color change
        /// </summary>
        private void ProgressMaximumColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            AMSettings.ChartMaximumColor = (Color)ProgressMaximumColorPicker.SelectedColor;
            UpdateChartData();
        }

        /// <summary>
        /// Updates chart on color change
        /// </summary>
        private void ProgressMinimumColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            AMSettings.ChartMinimumColor = (Color)ProgressMinimumColorPicker.SelectedColor;
            UpdateChartData();
        }

        /// <summary>
        /// Updates chart on color change
        /// </summary>
        private void ProgressAverageColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            AMSettings.ChartAverageColor = (Color)ProgressAverageColorPicker.SelectedColor;
            UpdateChartData();
        }

        /// <summary>
        /// Updates chart on color change
        /// </summary>
        private void ProgressDeviationColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            AMSettings.ChartDeviationColor = (Color)ProgressDeviationColorPicker.SelectedColor;
            UpdateChartData();
        }

        /// <summary>
        /// Updates chart on color change
        /// </summary>
        private void ProgressMediumColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            AMSettings.ChartMediumColor = (Color)ProgressMediumColorPicker.SelectedColor;
            UpdateChartData();
        }

        /// <summary>
        /// Updates chart on color change
        /// </summary>
        private void ProgressAmountColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            AMSettings.ChartAmountColor = (Color)ProgressAmountColorPicker.SelectedColor;
            UpdateChartData();
        }
        
        /// <summary>
        /// Show legend on check box change
        /// </summary>
        private void ProgressShowLegendCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ProgressLegendCanvas.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Hide legend on check box change
        /// </summary>
        private void ProgressShowLegendCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ProgressLegendCanvas.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Updates chart on show number of solves graph
        /// </summary>
        private void ProgressBarsCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ProgressAmountColorPicker.IsEnabled = true;
            ProgressBarsCanvas.Height = 80d;
            UpdateChartData();
        }

        /// <summary>
        /// Updates chart on hide number of solves graph
        /// </summary>
        private void ProgressBarsCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ProgressAmountColorPicker.IsEnabled = false;
            ProgressBarsCanvas.Height = 0d;
            UpdateChartData();
        }

        #endregion Legend events

        #region Controls events

        /// <summary>
        /// Updates chart on date change
        /// </summary>
        private void ProgressStartDateTimesPicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateChartData();
        }

        /// <summary>
        /// Updates chart on date change
        /// </summary>
        private void ProgressEndDateTimesPicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateChartData();
        }

        /// <summary>
        /// Updates chart on period change
        /// </summary>
        private void ProgressPeriodCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateChartData();
        }

        /// <summary>
        /// Updates chart on type change
        /// </summary>
        private void ProgressTypeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateChartData();
        }

        /// <summary>
        /// Updates chart on period check box change
        /// </summary>
        private void ProgressAllowPeriodsCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            UpdateChartData();
        }

        /// <summary>
        /// Updates chart on period check box change
        /// </summary>
        private void ProgressAllowPeriodsCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateChartData();
        }

        /// <summary>
        /// Set the start date to the previous month day with data
        /// </summary>
        private void ProgressStartPreviousMonthButton_Click(object sender, RoutedEventArgs e)
        {
            if (((DateTime)ProgressStartDateTimesPicker.SelectedDate).AddMonths(-1) > ProgressStartDateTimesPicker.DisplayDateStart)
            {
                DateTime Auxdate = ((DateTime)ProgressStartDateTimesPicker.SelectedDate).AddMonths(-1);
                while (Auxdate > ProgressStartDateTimesPicker.DisplayDateStart &&
                       ProgressStartDateTimesPicker.BlackoutDates.Contains(Auxdate))
                {
                    Auxdate = Auxdate.AddDays(-1);
                }
                ProgressStartDateTimesPicker.SelectedDate = Auxdate;
            }
            else ProgressStartDateTimesPicker.SelectedDate = ProgressStartDateTimesPicker.DisplayDateStart;
        }

        /// <summary>
        /// Set the start date to the previous day with data
        /// </summary>
        private void ProgressStartPreviousDayButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProgressStartDateTimesPicker.SelectedDate > ProgressStartDateTimesPicker.DisplayDateStart)
            {
                DateTime Auxdate = (DateTime)ProgressStartDateTimesPicker.SelectedDate;
                do
                {
                    Auxdate = Auxdate.AddDays(-1);
                }
                while (Auxdate > ProgressStartDateTimesPicker.DisplayDateStart &&
                       ProgressStartDateTimesPicker.BlackoutDates.Contains(Auxdate));
                ProgressStartDateTimesPicker.SelectedDate = Auxdate;
            }
        }

        /// <summary>
        /// Set the end date to the next day with data
        /// </summary>
        private void ProgressStartNextDayButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProgressStartDateTimesPicker.SelectedDate < ProgressEndDateTimesPicker.SelectedDate)
            {
                DateTime Auxdate = (DateTime)ProgressStartDateTimesPicker.SelectedDate;
                do
                {
                    Auxdate = Auxdate.AddDays(1);
                }
                while (Auxdate < ProgressEndDateTimesPicker.SelectedDate &&
                       ProgressStartDateTimesPicker.BlackoutDates.Contains(Auxdate));
                ProgressStartDateTimesPicker.SelectedDate = Auxdate;
            }
        }

        /// <summary>
        /// Set the end date to the next month day with data
        /// </summary>
        private void ProgressStartNextMonthButton_Click(object sender, RoutedEventArgs e)
        {
            if (((DateTime)ProgressStartDateTimesPicker.SelectedDate).AddMonths(1) < ProgressEndDateTimesPicker.SelectedDate)
            {
                DateTime Auxdate = ((DateTime)ProgressStartDateTimesPicker.SelectedDate).AddMonths(1);
                while (Auxdate < ProgressEndDateTimesPicker.SelectedDate &&
                       ProgressStartDateTimesPicker.BlackoutDates.Contains(Auxdate))
                {
                    Auxdate = Auxdate.AddDays(1);
                }
                ProgressStartDateTimesPicker.SelectedDate = Auxdate;
            }
            else ProgressStartDateTimesPicker.SelectedDate = ProgressEndDateTimesPicker.SelectedDate;
        }

        /// <summary>
        /// Set the start date to the previous month day with data
        /// </summary>
        private void ProgressEndPreviousMonthButton_Click(object sender, RoutedEventArgs e)
        {
            if (((DateTime)ProgressEndDateTimesPicker.SelectedDate).AddMonths(-1) > ProgressStartDateTimesPicker.SelectedDate)
            {
                DateTime Auxdate = ((DateTime)ProgressEndDateTimesPicker.SelectedDate).AddMonths(-1);
                while (Auxdate > ProgressStartDateTimesPicker.SelectedDate &&
                       ProgressEndDateTimesPicker.BlackoutDates.Contains(Auxdate))
                {
                    Auxdate = Auxdate.AddDays(-1);
                }
                ProgressEndDateTimesPicker.SelectedDate = Auxdate;
            }
            else ProgressEndDateTimesPicker.SelectedDate = ProgressStartDateTimesPicker.SelectedDate;
        }

        /// <summary>
        /// Set the start date to the previous day with data
        /// </summary>
        private void ProgressEndPreviousDayButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProgressEndDateTimesPicker.SelectedDate > ProgressStartDateTimesPicker.SelectedDate)
            {
                DateTime Auxdate = (DateTime)ProgressEndDateTimesPicker.SelectedDate;
                do
                {
                    Auxdate = Auxdate.AddDays(-1);
                }
                while (Auxdate > ProgressStartDateTimesPicker.SelectedDate &&
                       ProgressEndDateTimesPicker.BlackoutDates.Contains(Auxdate));
                ProgressEndDateTimesPicker.SelectedDate = Auxdate;
            }
        }

        /// <summary>
        /// Set the end date to the next day with data
        /// </summary>
        private void ProgressEndNextDayButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProgressEndDateTimesPicker.SelectedDate < ProgressEndDateTimesPicker.DisplayDateEnd)
            {
                DateTime Auxdate = (DateTime)ProgressEndDateTimesPicker.SelectedDate;
                do
                {
                    Auxdate = Auxdate.AddDays(1);
                }
                while (Auxdate < ProgressEndDateTimesPicker.DisplayDateEnd &&
                       ProgressEndDateTimesPicker.BlackoutDates.Contains(Auxdate));
                ProgressEndDateTimesPicker.SelectedDate = Auxdate;
            }
        }

        /// <summary>
        /// Set the end date to the next month day with data
        /// </summary>
        private void ProgressEndNextMonthButton_Click(object sender, RoutedEventArgs e)
        {
            if (((DateTime)ProgressEndDateTimesPicker.SelectedDate).AddMonths(1) < ProgressEndDateTimesPicker.DisplayDateEnd)
            {
                DateTime Auxdate = ((DateTime)ProgressEndDateTimesPicker.SelectedDate).AddMonths(1);
                while (Auxdate < ProgressEndDateTimesPicker.DisplayDateEnd &&
                       ProgressEndDateTimesPicker.BlackoutDates.Contains(Auxdate))
                {
                    Auxdate = Auxdate.AddDays(1);
                }
                ProgressEndDateTimesPicker.SelectedDate = Auxdate;
            }
            else ProgressEndDateTimesPicker.SelectedDate = ProgressEndDateTimesPicker.DisplayDateEnd;
        }

        #endregion Controls events

        #endregion Progress events

        #region Editor functions

        /// <summary>
        /// If scramble has changed (separator " "), insert it into undo list and return true
        /// </summary>
        /// <returns>True if scramble is added to undo list</returns>
        public bool AddScrambleToUndoList()
        {
            string UndoStr = EditorData.Scramble.GetText(" ");

            if (string.IsNullOrWhiteSpace(UndoStr)) return false;

            if (EditorData.UndoIndex >= 0 && EditorData.UndoIndex < EditorData.UndoList.Count)
                if (string.Compare(UndoStr, EditorData.UndoList[EditorData.UndoIndex]) == 0) return false;

            EditorData.UndoList.Insert(0, UndoStr);
            EditorData.UndoIndex = 0;

            while (EditorData.UndoList.Count > 20) EditorData.UndoList.RemoveAt(EditorData.UndoList.Count - 1);

            EditorUndoButton.IsEnabled = true;
            EditorRedoButton.IsEnabled = false;

            return true;
        }

        /// <summary>
        /// Initialize editor fields
        /// </summary>
        private void InitializeEditor()
        {
            EditorScrambleBarTray.IsLocked = true;

            SetEditorCube2DButtons();

            EditorData.MovingEditorCube = false;

            EditorData.Scramble = new Scramble();
            EditorData.Cube = new Cube3D();

            UpdateEditorScramble();

            EditorData.Cube.ApplyScramble(EditorData.Scramble);
            EditorData.Cube.UpdateBitmap((int)EditorImage3D.ActualWidth, (int)EditorImage3D.ActualHeight);
            EditorImage3D.Source = EditorData.Cube.renderBMP;
            UpdateEditorCube2D();

            // Backgorund worker task for animations
            EditorData.AnimWork = new BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = false
            };
            EditorData.AnimWork.DoWork += new DoWorkEventHandler(EditorAnimWork_DoWork);
            EditorData.AnimWork.RunWorkerCompleted += new RunWorkerCompletedEventHandler(EditorAnimWork_RunWorkerCompleted);

            EditorNeutralComboBox.SelectedIndex = 0;
            EditorModifierCombo.SelectedIndex = 0;
            ParenthesisRepetitionsCombo.SelectedIndex = 0;

            SetEditorButtonsTexts();

            EditorData.ParenthesesNest = 0;
            EditorData.FirstStepSelected = -1;
            EditorData.LastStepSelected = -1;

            EditorData.UndoList = new List<string>();
            EditorData.UndoIndex = -1;

            CloseParenthesisButton.IsEnabled = false;
            EditorUndoButton.IsEnabled = false;
            EditorRedoButton.IsEnabled = false;
            EditorClearSelectionButton.IsEnabled = false;

            UpdateEditorModifierCombo();
        }

        /// <summary>
        /// Set buttons array for chrono cube 2D view (a button per sticker)
        /// </summary>
        private void SetEditorCube2DButtons()
        {
            if (EditorData.Cube2DButtons == null || EditorData.Cube2DButtons.Length != 54)
                EditorData.Cube2DButtons = new Button[54];

            // Face up
            EditorData.Cube2DButtons[(int)StickerPositions.UBL_U] = Editor_B2D_UBL_U;
            EditorData.Cube2DButtons[(int)StickerPositions.UB_U] = Editor_B2D_UB_U;
            EditorData.Cube2DButtons[(int)StickerPositions.UBR_U] = Editor_B2D_UBR_U;
            EditorData.Cube2DButtons[(int)StickerPositions.UL_U] = Editor_B2D_UL_U;
            EditorData.Cube2DButtons[(int)StickerPositions.U] = Editor_B2D_U;
            EditorData.Cube2DButtons[(int)StickerPositions.UR_U] = Editor_B2D_UR_U;
            EditorData.Cube2DButtons[(int)StickerPositions.UFL_U] = Editor_B2D_UFL_U;
            EditorData.Cube2DButtons[(int)StickerPositions.UF_U] = Editor_B2D_UF_U;
            EditorData.Cube2DButtons[(int)StickerPositions.UFR_U] = Editor_B2D_UFR_U;

            // Layer up
            EditorData.Cube2DButtons[(int)StickerPositions.UBL_L] = Editor_B2D_UBL_L;
            EditorData.Cube2DButtons[(int)StickerPositions.UL_L] = Editor_B2D_UL_L;
            EditorData.Cube2DButtons[(int)StickerPositions.UFL_L] = Editor_B2D_UFL_L;
            EditorData.Cube2DButtons[(int)StickerPositions.UFL_F] = Editor_B2D_UFL_F;
            EditorData.Cube2DButtons[(int)StickerPositions.UF_F] = Editor_B2D_UF_F;
            EditorData.Cube2DButtons[(int)StickerPositions.UFR_F] = Editor_B2D_UFR_F;
            EditorData.Cube2DButtons[(int)StickerPositions.UFR_R] = Editor_B2D_UFR_R;
            EditorData.Cube2DButtons[(int)StickerPositions.UR_R] = Editor_B2D_UR_R;
            EditorData.Cube2DButtons[(int)StickerPositions.UBR_R] = Editor_B2D_UBR_R;
            EditorData.Cube2DButtons[(int)StickerPositions.UBR_B] = Editor_B2D_UBR_B;
            EditorData.Cube2DButtons[(int)StickerPositions.UB_B] = Editor_B2D_UB_B;
            EditorData.Cube2DButtons[(int)StickerPositions.UBL_B] = Editor_B2D_UBL_B;

            // Layer middle
            EditorData.Cube2DButtons[(int)StickerPositions.BL_L] = Editor_B2D_BL_L;
            EditorData.Cube2DButtons[(int)StickerPositions.L] = Editor_B2D_L;
            EditorData.Cube2DButtons[(int)StickerPositions.LF_L] = Editor_B2D_LF_L;
            EditorData.Cube2DButtons[(int)StickerPositions.LF_F] = Editor_B2D_LF_F;
            EditorData.Cube2DButtons[(int)StickerPositions.F] = Editor_B2D_F;
            EditorData.Cube2DButtons[(int)StickerPositions.FR_F] = Editor_B2D_FR_F;
            EditorData.Cube2DButtons[(int)StickerPositions.FR_R] = Editor_B2D_FR_R;
            EditorData.Cube2DButtons[(int)StickerPositions.R] = Editor_B2D_R;
            EditorData.Cube2DButtons[(int)StickerPositions.RB_R] = Editor_B2D_RB_R;
            EditorData.Cube2DButtons[(int)StickerPositions.RB_B] = Editor_B2D_RB_B;
            EditorData.Cube2DButtons[(int)StickerPositions.B] = Editor_B2D_B;
            EditorData.Cube2DButtons[(int)StickerPositions.BL_B] = Editor_B2D_BL_B;

            // Layer down
            EditorData.Cube2DButtons[(int)StickerPositions.DBL_L] = Editor_B2D_DBL_L;
            EditorData.Cube2DButtons[(int)StickerPositions.DL_L] = Editor_B2D_DL_L;
            EditorData.Cube2DButtons[(int)StickerPositions.DFL_L] = Editor_B2D_DFL_L;
            EditorData.Cube2DButtons[(int)StickerPositions.DFL_F] = Editor_B2D_DFL_F;
            EditorData.Cube2DButtons[(int)StickerPositions.DF_F] = Editor_B2D_DF_F;
            EditorData.Cube2DButtons[(int)StickerPositions.DFR_F] = Editor_B2D_DFR_F;
            EditorData.Cube2DButtons[(int)StickerPositions.DFR_R] = Editor_B2D_DFR_R;
            EditorData.Cube2DButtons[(int)StickerPositions.DR_R] = Editor_B2D_DR_R;
            EditorData.Cube2DButtons[(int)StickerPositions.DBR_R] = Editor_B2D_DBR_R;
            EditorData.Cube2DButtons[(int)StickerPositions.DBR_B] = Editor_B2D_DBR_B;
            EditorData.Cube2DButtons[(int)StickerPositions.DB_B] = Editor_B2D_DB_B;
            EditorData.Cube2DButtons[(int)StickerPositions.DBL_B] = Editor_B2D_DBL_B;

            // Face down
            EditorData.Cube2DButtons[(int)StickerPositions.DFL_D] = Editor_B2D_DFL_D;
            EditorData.Cube2DButtons[(int)StickerPositions.DF_D] = Editor_B2D_DF_D;
            EditorData.Cube2DButtons[(int)StickerPositions.DFR_D] = Editor_B2D_DFR_D;
            EditorData.Cube2DButtons[(int)StickerPositions.DL_D] = Editor_B2D_DL_D;
            EditorData.Cube2DButtons[(int)StickerPositions.D] = Editor_B2D_D;
            EditorData.Cube2DButtons[(int)StickerPositions.DR_D] = Editor_B2D_DR_D;
            EditorData.Cube2DButtons[(int)StickerPositions.DBL_D] = Editor_B2D_DBL_D;
            EditorData.Cube2DButtons[(int)StickerPositions.DB_D] = Editor_B2D_DB_D;
            EditorData.Cube2DButtons[(int)StickerPositions.DBR_D] = Editor_B2D_DBR_D;

            for (int n = 0; n < EditorData.Cube2DButtons.Length; n++)
            {
                EditorData.Cube2DButtons[n].Click += EditorCube2DButtons_Click;
                EditorData.Cube2DButtons[n].Tag = n;
            }
        }

        /// <summary>
        /// Update the editor scramble list of buttons
        /// </summary>
        public void UpdateEditorScramble()
        {
            EditorScrambleToolBar.Items.Clear();

            EditorData.ScrambleButtons = new Button[EditorData.Scramble.Length];

            for (int i = 0; i < EditorData.Scramble.Length; i++)
            {
                EditorData.ScrambleButtons[i] = new Button
                {
                    Style = (Style)FindResource("ScrambleButtonGreenBackKey"),
                    Content = ScrambleStep.GetText(EditorData.Scramble[i]),
                    Tag = i
                };
                EditorData.ScrambleButtons[i].Click += EditorScrambleButtons_Click;

                EditorScrambleToolBar.Items.Add(EditorData.ScrambleButtons[i]);
            }

            EditorData.FirstStepSelected = -1;
            EditorData.LastStepSelected = -1;
            EnableDisableMovementButtons(true);
            EnableDisableTurnButtons(true);
        }

        /// <summary>
        /// Enable / disable editor movements buttons
        /// </summary>
        /// <param name="Enable">Enable if true / disable if false</param>
        private void EnableDisableMovementButtons(bool Enable)
        {
            UButton.IsEnabled = U2Button.IsEnabled = UpButton.IsEnabled = Enable;
            DButton.IsEnabled = D2Button.IsEnabled = DpButton.IsEnabled = Enable;
            FButton.IsEnabled = F2Button.IsEnabled = FpButton.IsEnabled = Enable;
            BButton.IsEnabled = B2Button.IsEnabled = BpButton.IsEnabled = Enable;
            RButton.IsEnabled = R2Button.IsEnabled = RpButton.IsEnabled = Enable;
            LButton.IsEnabled = L2Button.IsEnabled = LpButton.IsEnabled = Enable;

            EButton.IsEnabled = E2Button.IsEnabled = EpButton.IsEnabled = Enable;
            SButton.IsEnabled = S2Button.IsEnabled = SpButton.IsEnabled = Enable;
            MButton.IsEnabled = M2Button.IsEnabled = MpButton.IsEnabled = Enable;
        }

        /// <summary>
        /// Enable / disable editor turns buttons
        /// </summary>
        /// <param name="Enable">Enable if true / disable if false</param>
        private void EnableDisableTurnButtons(bool Enable)
        {
            xButton.IsEnabled = x2Button.IsEnabled = xpButton.IsEnabled = Enable;
            yButton.IsEnabled = y2Button.IsEnabled = ypButton.IsEnabled = Enable;
            zButton.IsEnabled = z2Button.IsEnabled = zpButton.IsEnabled = Enable;
        }

        /// <summary>
        /// Update editor cube skin colors
        /// </summary>
        public void UpdateEditorCube2D()
        {
            Editor2DGrid.Background = AMSettings.BackgroundBrush;
            if (EditorData.Cube != null && EditorData.Cube2DButtons != null)
            {
                for (int Pos = 0; Pos < EditorData.Cube2DButtons.Length; Pos++)
                {
                    StickerPositions St = EditorData.Cube.Cube.GetStickerSolvedPosition((StickerPositions)Pos);
                    EditorData.Cube2DButtons[Pos].BorderBrush = AMSettings.BaseBrush;
                    if (EditorData.Cube.NeutralStickers[(int)St])
                        EditorData.Cube2DButtons[Pos].Background = AMSettings.NeutralBrush;
                    else
                        EditorData.Cube2DButtons[Pos].Background =
                            AMSettings.GetBrush(EditorData.Cube.GetColor((StickerPositions)Pos));
                }
            }
        }

        /// <summary>
        /// Updates the scramble view, the 3D cube and the 2D cube skin (Editor tab)
        /// </summary>
        public void UpdateEditorViews()
        {
            UpdateEditorScramble();
            EditorData.Cube.Reset();
            EditorData.Cube.ApplyScramble(EditorData.Scramble);
            EditorData.Cube.UpdateBitmap((int)EditorImage3D.ActualWidth, (int)EditorImage3D.ActualHeight);
            EditorImage3D.Source = EditorData.Cube.renderBMP;
            UpdateEditorCube2D();
        }

        /// <summary>
        /// Sets the texts in editor movements buttons (with current scramble char set)
        /// </summary>
        private void SetEditorButtonsTexts()
        {
            EButton.Content = AMSettings.EChar.ToString();
            EpButton.Content = AMSettings.EChar.ToString() + AMSettings.PrimeChar;
            E2Button.Content = AMSettings.EChar.ToString() + AMSettings.DoubleChar;

            SButton.Content = AMSettings.SChar.ToString();
            SpButton.Content = AMSettings.SChar.ToString() + AMSettings.PrimeChar;
            S2Button.Content = AMSettings.SChar.ToString() + AMSettings.DoubleChar;

            MButton.Content = AMSettings.MChar.ToString();
            MpButton.Content = AMSettings.MChar.ToString() + AMSettings.PrimeChar;
            M2Button.Content = AMSettings.MChar.ToString() + AMSettings.DoubleChar;

            xButton.Content = AMSettings.xChar.ToString();
            xpButton.Content = AMSettings.xChar.ToString() + AMSettings.PrimeChar;
            x2Button.Content = AMSettings.xChar.ToString() + AMSettings.DoubleChar;

            yButton.Content = AMSettings.yChar.ToString();
            ypButton.Content = AMSettings.yChar.ToString() + AMSettings.PrimeChar;
            y2Button.Content = AMSettings.yChar.ToString() + AMSettings.DoubleChar;

            zButton.Content = AMSettings.zChar.ToString();
            zpButton.Content = AMSettings.zChar.ToString() + AMSettings.PrimeChar;
            z2Button.Content = AMSettings.zChar.ToString() + AMSettings.DoubleChar;

            switch (EditorModifierCombo.SelectedIndex)
            {
                case 1: // Modifiers.DOUBLE_ADJACENT_LAYERS_SAME_DIRECTION:
                    if (AMSettings.UsingAltwChars)
                    {
                        UButton.Content = AMSettings.AltUwChar.ToString();
                        UpButton.Content = AMSettings.AltUwChar.ToString() + AMSettings.PrimeChar;
                        U2Button.Content = AMSettings.AltUwChar.ToString() + AMSettings.DoubleChar;

                        DButton.Content = AMSettings.AltDwChar.ToString();
                        DpButton.Content = AMSettings.AltDwChar.ToString() + AMSettings.PrimeChar;
                        D2Button.Content = AMSettings.AltDwChar.ToString() + AMSettings.DoubleChar;

                        FButton.Content = AMSettings.AltFwChar.ToString();
                        FpButton.Content = AMSettings.AltFwChar.ToString() + AMSettings.PrimeChar;
                        F2Button.Content = AMSettings.AltFwChar.ToString() + AMSettings.DoubleChar;

                        BButton.Content = AMSettings.AltBwChar.ToString();
                        BpButton.Content = AMSettings.AltBwChar.ToString() + AMSettings.PrimeChar;
                        B2Button.Content = AMSettings.AltBwChar.ToString() + AMSettings.DoubleChar;

                        RButton.Content = AMSettings.AltRwChar.ToString();
                        RpButton.Content = AMSettings.AltRwChar.ToString() + AMSettings.PrimeChar;
                        R2Button.Content = AMSettings.AltRwChar.ToString() + AMSettings.DoubleChar;

                        LButton.Content = AMSettings.AltLwChar.ToString();
                        LpButton.Content = AMSettings.AltLwChar.ToString() + AMSettings.PrimeChar;
                        L2Button.Content = AMSettings.AltLwChar.ToString() + AMSettings.DoubleChar;
                    }
                    else
                    {
                        UButton.Content = AMSettings.UChar.ToString() + AMSettings.wChar;
                        UpButton.Content = AMSettings.UChar.ToString() + AMSettings.wChar + AMSettings.PrimeChar;
                        U2Button.Content = AMSettings.UChar.ToString() + AMSettings.wChar + AMSettings.DoubleChar;

                        DButton.Content = AMSettings.DChar.ToString() + AMSettings.wChar;
                        DpButton.Content = AMSettings.DChar.ToString() + AMSettings.wChar + AMSettings.PrimeChar;
                        D2Button.Content = AMSettings.DChar.ToString() + AMSettings.wChar + AMSettings.DoubleChar;

                        FButton.Content = AMSettings.FChar.ToString() + AMSettings.wChar;
                        FpButton.Content = AMSettings.FChar.ToString() + AMSettings.wChar + AMSettings.PrimeChar;
                        F2Button.Content = AMSettings.FChar.ToString() + AMSettings.wChar + AMSettings.DoubleChar;

                        BButton.Content = AMSettings.BChar.ToString() + AMSettings.wChar;
                        BpButton.Content = AMSettings.BChar.ToString() + AMSettings.wChar + AMSettings.PrimeChar;
                        B2Button.Content = AMSettings.BChar.ToString() + AMSettings.wChar + AMSettings.DoubleChar;

                        RButton.Content = AMSettings.RChar.ToString() + AMSettings.wChar;
                        RpButton.Content = AMSettings.RChar.ToString() + AMSettings.wChar + AMSettings.PrimeChar;
                        R2Button.Content = AMSettings.RChar.ToString() + AMSettings.wChar + AMSettings.DoubleChar;

                        LButton.Content = AMSettings.LChar.ToString() + AMSettings.wChar;
                        LpButton.Content = AMSettings.LChar.ToString() + AMSettings.wChar + AMSettings.PrimeChar;
                        L2Button.Content = AMSettings.LChar.ToString() + AMSettings.wChar + AMSettings.DoubleChar;
                    }
                    break;

                case 2: // Modifiers.DOUBLE_ADJACENT_LAYERS_OPPOSITE_DIRECTION:
                    UButton.Content = AMSettings.UChar.ToString() + AMSettings.oChar;
                    UpButton.Content = AMSettings.UChar.ToString() + AMSettings.oChar + AMSettings.PrimeChar;
                    U2Button.Content = AMSettings.UChar.ToString() + AMSettings.oChar + AMSettings.DoubleChar;

                    DButton.Content = AMSettings.DChar.ToString() + AMSettings.oChar;
                    DpButton.Content = AMSettings.DChar.ToString() + AMSettings.oChar + AMSettings.PrimeChar;
                    D2Button.Content = AMSettings.DChar.ToString() + AMSettings.oChar + AMSettings.DoubleChar;

                    FButton.Content = AMSettings.FChar.ToString() + AMSettings.oChar;
                    FpButton.Content = AMSettings.FChar.ToString() + AMSettings.oChar + AMSettings.PrimeChar;
                    F2Button.Content = AMSettings.FChar.ToString() + AMSettings.oChar + AMSettings.DoubleChar;

                    BButton.Content = AMSettings.BChar.ToString() + AMSettings.oChar;
                    BpButton.Content = AMSettings.BChar.ToString() + AMSettings.oChar + AMSettings.PrimeChar;
                    B2Button.Content = AMSettings.BChar.ToString() + AMSettings.oChar + AMSettings.DoubleChar;

                    RButton.Content = AMSettings.RChar.ToString() + AMSettings.oChar;
                    RpButton.Content = AMSettings.RChar.ToString() + AMSettings.oChar + AMSettings.PrimeChar;
                    R2Button.Content = AMSettings.RChar.ToString() + AMSettings.oChar + AMSettings.DoubleChar;

                    LButton.Content = AMSettings.LChar.ToString() + AMSettings.oChar;
                    LpButton.Content = AMSettings.LChar.ToString() + AMSettings.oChar + AMSettings.PrimeChar;
                    L2Button.Content = AMSettings.LChar.ToString() + AMSettings.oChar + AMSettings.DoubleChar;
                    break;

                case 3: // Modifiers.DOUBLE_OPPOSITE_LAYERS_SAME_DIRECTION:
                    UButton.Content = AMSettings.UChar.ToString() + AMSettings.sChar;
                    UpButton.Content = AMSettings.UChar.ToString() + AMSettings.sChar + AMSettings.PrimeChar;
                    U2Button.Content = AMSettings.UChar.ToString() + AMSettings.sChar + AMSettings.DoubleChar;

                    DButton.Content = AMSettings.DChar.ToString() + AMSettings.sChar;
                    DpButton.Content = AMSettings.DChar.ToString() + AMSettings.sChar + AMSettings.PrimeChar;
                    D2Button.Content = AMSettings.DChar.ToString() + AMSettings.sChar + AMSettings.DoubleChar;

                    FButton.Content = AMSettings.FChar.ToString() + AMSettings.sChar;
                    FpButton.Content = AMSettings.FChar.ToString() + AMSettings.sChar + AMSettings.PrimeChar;
                    F2Button.Content = AMSettings.FChar.ToString() + AMSettings.sChar + AMSettings.DoubleChar;

                    BButton.Content = AMSettings.BChar.ToString() + AMSettings.sChar;
                    BpButton.Content = AMSettings.BChar.ToString() + AMSettings.sChar + AMSettings.PrimeChar;
                    B2Button.Content = AMSettings.BChar.ToString() + AMSettings.sChar + AMSettings.DoubleChar;

                    RButton.Content = AMSettings.RChar.ToString() + AMSettings.sChar;
                    RpButton.Content = AMSettings.RChar.ToString() + AMSettings.sChar + AMSettings.PrimeChar;
                    R2Button.Content = AMSettings.RChar.ToString() + AMSettings.sChar + AMSettings.DoubleChar;

                    LButton.Content = AMSettings.LChar.ToString() + AMSettings.sChar;
                    LpButton.Content = AMSettings.LChar.ToString() + AMSettings.sChar + AMSettings.PrimeChar;
                    L2Button.Content = AMSettings.LChar.ToString() + AMSettings.sChar + AMSettings.DoubleChar;
                    break;

                case 4: // Modifiers.DOUBLE_OPPOSITE_LAYERS_OPPOSITE_DIRECTION:
                    UButton.Content = AMSettings.UChar.ToString() + AMSettings.aChar;
                    UpButton.Content = AMSettings.UChar.ToString() + AMSettings.aChar + AMSettings.PrimeChar;
                    U2Button.Content = AMSettings.UChar.ToString() + AMSettings.aChar + AMSettings.DoubleChar;

                    DButton.Content = AMSettings.DChar.ToString() + AMSettings.aChar;
                    DpButton.Content = AMSettings.DChar.ToString() + AMSettings.aChar + AMSettings.PrimeChar;
                    D2Button.Content = AMSettings.DChar.ToString() + AMSettings.aChar + AMSettings.DoubleChar;

                    FButton.Content = AMSettings.FChar.ToString() + AMSettings.aChar;
                    FpButton.Content = AMSettings.FChar.ToString() + AMSettings.aChar + AMSettings.PrimeChar;
                    F2Button.Content = AMSettings.FChar.ToString() + AMSettings.aChar + AMSettings.DoubleChar;

                    BButton.Content = AMSettings.BChar.ToString() + AMSettings.aChar;
                    BpButton.Content = AMSettings.BChar.ToString() + AMSettings.aChar + AMSettings.PrimeChar;
                    B2Button.Content = AMSettings.BChar.ToString() + AMSettings.aChar + AMSettings.DoubleChar;

                    RButton.Content = AMSettings.RChar.ToString() + AMSettings.aChar;
                    RpButton.Content = AMSettings.RChar.ToString() + AMSettings.aChar + AMSettings.PrimeChar;
                    R2Button.Content = AMSettings.RChar.ToString() + AMSettings.aChar + AMSettings.DoubleChar;

                    LButton.Content = AMSettings.LChar.ToString() + AMSettings.aChar;
                    LpButton.Content = AMSettings.LChar.ToString() + AMSettings.aChar + AMSettings.PrimeChar;
                    L2Button.Content = AMSettings.LChar.ToString() + AMSettings.aChar + AMSettings.DoubleChar;
                    break;

                default: // Modifiers.SINGLE_LAYER
                    UButton.Content = AMSettings.UChar.ToString();
                    UpButton.Content = AMSettings.UChar.ToString() + AMSettings.PrimeChar;
                    U2Button.Content = AMSettings.UChar.ToString() + AMSettings.DoubleChar;

                    DButton.Content = AMSettings.DChar.ToString();
                    DpButton.Content = AMSettings.DChar.ToString() + AMSettings.PrimeChar;
                    D2Button.Content = AMSettings.DChar.ToString() + AMSettings.DoubleChar;

                    FButton.Content = AMSettings.FChar.ToString();
                    FpButton.Content = AMSettings.FChar.ToString() + AMSettings.PrimeChar;
                    F2Button.Content = AMSettings.FChar.ToString() + AMSettings.DoubleChar;

                    BButton.Content = AMSettings.BChar.ToString();
                    BpButton.Content = AMSettings.BChar.ToString() + AMSettings.PrimeChar;
                    B2Button.Content = AMSettings.BChar.ToString() + AMSettings.DoubleChar;

                    RButton.Content = AMSettings.RChar.ToString();
                    RpButton.Content = AMSettings.RChar.ToString() + AMSettings.PrimeChar;
                    R2Button.Content = AMSettings.RChar.ToString() + AMSettings.DoubleChar;

                    LButton.Content = AMSettings.LChar.ToString();
                    LpButton.Content = AMSettings.LChar.ToString() + AMSettings.PrimeChar;
                    L2Button.Content = AMSettings.LChar.ToString() + AMSettings.DoubleChar;
                    break;
            }
        }

        /// <summary>
        /// Updates the movement modifiers combo box
        /// </summary>
        private void UpdateEditorModifierCombo()
        {
            EditorModifierCombo.Items.Clear();
            EditorModifierCombo.Items.Add(AMTexts.Message("SingleLayerModifierMessage"));
            EditorModifierCombo.Items.Add(AMTexts.Message("AdjacentLayersSameDirectionMessage"));
            if (AMSettings.AllowExtendedCompoundMovements)
            {
                EditorModifierCombo.Items.Add(AMTexts.Message("AdjacentLayersInverseDirectionMessage"));
                EditorModifierCombo.Items.Add(AMTexts.Message("OppositeLayersSameDirectionMessage"));
                EditorModifierCombo.Items.Add(AMTexts.Message("OppositeLayersInverseDirectionMessage"));
            }
            EditorModifierCombo.SelectedIndex = 0;
        }

        #endregion Editor functions

        #region Editor events

        #region EditorImage3D events

        /// <summary>
        /// Editor cube 3D size changed event
        /// </summary>
        private void EditorImage3D_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            EditorData.Cube.UpdateBitmap((int)EditorImage3D.ActualWidth, (int)EditorImage3D.ActualHeight);
            EditorImage3D.Source = EditorData.Cube.renderBMP;
        }

        /// <summary>
        /// Mouse down event: start / stop move in 3D view
        /// </summary>
        private void EditorImage3D_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (EditorData.MovingEditorCube)
            {
                EditorData.MovingEditorCube = false;
                Cursor = Cursors.Arrow;
            }
            else
            {
                EditorData.MovingEditorCube = true;
                Cursor = Cursors.ScrollAll;
            }
        }

        /// <summary>
        /// Mouse leave event: change cursor
        /// </summary>
        private void EditorImage3D_MouseLeave(object sender, MouseEventArgs e)
        {
            if (EditorData.MovingEditorCube) Cursor = Cursors.Arrow;
        }

        /// <summary>
        /// Mouse enter event: change cursor
        /// </summary>
        private void EditorImage3D_MouseEnter(object sender, MouseEventArgs e)
        {
            if (EditorData.MovingEditorCube) Cursor = Cursors.ScrollAll;
        }

        /// <summary>
        /// Mouse move event: move in 3D view
        /// </summary>
        private void EditorImage3D_MouseMove(object sender, MouseEventArgs e)
        {
            if (EditorData.MovingEditorCube)
            {
                double alpha = 360.0 * e.GetPosition(EditorImage3D).X / EditorImage3D.ActualWidth - 90.0;
                double beta = 180.0 * e.GetPosition(EditorImage3D).Y / EditorImage3D.ActualHeight - 90.0;
                EditorData.Cube.SetCamera(EditorData.Cube.CameraDistance, alpha, beta);
                EditorData.Cube.UpdateBitmap((int)EditorImage3D.ActualWidth, (int)EditorImage3D.ActualHeight);
                EditorImage3D.Source = EditorData.Cube.renderBMP;
            }
        }

        /// <summary>
        /// Click event: Plus zoom 3D view button
        /// </summary>
        private void Editor3DZoomPlusButton_Click(object sender, RoutedEventArgs e)
        {
            if (!EditorData.AnimWork.IsBusy)
            {
                EditorData.Cube.SetCamera(EditorData.Cube.CameraDistance * 0.9, EditorData.Cube.CameraAlpha, EditorData.Cube.CameraBeta);
                EditorData.Cube.UpdateBitmap((int)EditorImage3D.ActualWidth, (int)EditorImage3D.ActualHeight);
                EditorImage3D.Source = EditorData.Cube.renderBMP;
            }
        }

        /// <summary>
        /// Click event: Minus zoom 3D view button
        /// </summary>
        private void Editor3DZoomMinusButton_Click(object sender, RoutedEventArgs e)
        {
            if (!EditorData.AnimWork.IsBusy)
            {
                EditorData.Cube.SetCamera(EditorData.Cube.CameraDistance * 1.1, EditorData.Cube.CameraAlpha, EditorData.Cube.CameraBeta);
                EditorData.Cube.UpdateBitmap((int)EditorImage3D.ActualWidth, (int)EditorImage3D.ActualHeight);
                EditorImage3D.Source = EditorData.Cube.renderBMP;
            }
        }

        #endregion EditorImage3D events

        #region Editor BackgroundWorker events

        /// <summary>
        /// Editor BackgorundWorker DoWork event (asigned programatically)
        /// </summary>
        private void EditorAnimWork_DoWork(object sender, DoWorkEventArgs e)
        {
            Stopwatch AnimTimeControl = new Stopwatch();

            Dispatcher.Invoke(new Action(delegate { Cursor = Cursors.Wait; })); ;

            AnimTimeControl.Restart();
            while (AnimTimeControl.ElapsedMilliseconds < AMSettings.EditorAnimTime)
            {
                Dispatcher.Invoke(new Action(delegate
                {
                    EditorData.Cube.RotateStepPorcentage(EditorData.AnimStep, (double)AnimTimeControl.ElapsedMilliseconds / AMSettings.EditorAnimTime);
                    EditorData.Cube.UpdateBitmap((int)EditorImage3D.ActualWidth, (int)EditorImage3D.ActualHeight);
                    EditorImage3D.Source = EditorData.Cube.renderBMP;
                }));
                System.Threading.Thread.Sleep(10); // Improve animation smoothness
            }
            AnimTimeControl.Stop();
        }

        /// <summary>
        /// RunWorkerCompleted event (asigned programatically)
        /// </summary>
        private void EditorAnimWork_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((e.Cancelled))
            {
            }
            else if (e.Error != null)
            {
                Dispatcher.Invoke(new Action(delegate { AMSettings.Log = "Editor animation error: " + e.Error.Message; }));
            }
            else
            {
            }

            Dispatcher.Invoke(new Action(delegate
            {
                EditorData.Cube.RotateToZero();
                EditorData.Cube.Reset();
                EditorData.Cube.ApplyScramble(EditorData.Scramble);
                EditorData.Cube.UpdateBitmap((int)EditorImage3D.ActualWidth,
                                             (int)EditorImage3D.ActualHeight);
                EditorImage3D.Source = EditorData.Cube.renderBMP;
                UpdateEditorCube2D();
                Cursor = Cursors.Arrow;
            }));
        }

        #endregion Editor BackgroundWorker events

        #region Neutral controls events

        /// <summary>
        /// Click event: Clear neutral stickers
        /// </summary>
        private void EditorClearNeutralButton_Click(object sender, RoutedEventArgs e)
        {
            for (int n = 0; n < EditorData.Cube.NeutralStickers.Length; n++)
                EditorData.Cube.NeutralStickers[n] = false;
            UpdateEditorViews();
        }

        /// <summary>
        /// Click event: Invert neutral stickers
        /// </summary>
        private void EditorInvertNeutralButton_Click(object sender, RoutedEventArgs e)
        {
            for (int n = 0; n < EditorData.Cube.NeutralStickers.Length; n++)
                EditorData.Cube.NeutralStickers[n] = !EditorData.Cube.NeutralStickers[n];
            UpdateEditorViews();
        }

        /// <summary>
        /// Click event: Editor cube 2D buttons (asigned programatically)
        /// </summary>
        private void EditorCube2DButtons_Click(object sender, EventArgs e)
        {
            StickerPositions SP = EditorData.Cube.Cube.GetStickerSolvedPosition((StickerPositions)((Button)sender).Tag);
            bool NeutralValue = !EditorData.Cube.NeutralStickers[(int)SP];

            if (EditorNeutralComboBox.SelectedIndex == 0) // Sticker
            {
                EditorData.Cube.NeutralStickers[(int)SP] = NeutralValue;
            }
            else if (EditorNeutralComboBox.SelectedIndex == 1) // Piece
            {
                Pieces P = StickerData.GetStickerPiece(SP);

                foreach (StickerPositions Pos in Enum.GetValues(typeof(StickerPositions)))
                    if (StickerData.GetStickerPiece(Pos) == P)
                        EditorData.Cube.NeutralStickers[(int)Pos] = NeutralValue;
            }
            else if (EditorNeutralComboBox.SelectedIndex == 2) // Face / Layer
            {
                if (SP <= StickerPositions.L) // Center
                {
                    switch (SP)
                    {
                        case StickerPositions.U:
                            foreach (StickerPositions Pos in Enum.GetValues(typeof(StickersULayer)))
                                EditorData.Cube.NeutralStickers[(int)Pos] = NeutralValue;
                            break;
                        case StickerPositions.D:
                            foreach (StickerPositions Pos in Enum.GetValues(typeof(StickersDLayer)))
                                EditorData.Cube.NeutralStickers[(int)Pos] = NeutralValue;
                            break;
                        case StickerPositions.F:
                            foreach (StickerPositions Pos in Enum.GetValues(typeof(StickersFLayer)))
                                EditorData.Cube.NeutralStickers[(int)Pos] = NeutralValue;
                            break;
                        case StickerPositions.B:
                            foreach (StickerPositions Pos in Enum.GetValues(typeof(StickersBLayer)))
                                EditorData.Cube.NeutralStickers[(int)Pos] = NeutralValue;
                            break;
                        case StickerPositions.R:
                            foreach (StickerPositions Pos in Enum.GetValues(typeof(StickersRLayer)))
                                EditorData.Cube.NeutralStickers[(int)Pos] = NeutralValue;
                            break;
                        case StickerPositions.L:
                            foreach (StickerPositions Pos in Enum.GetValues(typeof(StickersLLayer)))
                                EditorData.Cube.NeutralStickers[(int)Pos] = NeutralValue;
                            break;
                    }
                }
                else
                {
                    Faces F = StickerData.GetStickerFace(SP);
                    foreach (StickerPositions Pos in Enum.GetValues(typeof(StickerPositions)))
                        if (StickerData.GetStickerFace(Pos) == F)
                            EditorData.Cube.NeutralStickers[(int)Pos] = NeutralValue;
                }
            }
            else return;

            UpdateEditorViews();
        }

        #endregion Neutral controls events

        #region Editor controls events

        /// <summary>
        /// Click event: Editor scramble buttons (asigned programatically)
        /// </summary>
        public void EditorScrambleButtons_Click(object sender, EventArgs e)
        {
            int bn = (int)((Button)sender).Tag;

            // Update selection indexes
            if (EditorData.LastStepSelected < 0)
            {
                if (EditorData.FirstStepSelected < 0) EditorData.FirstStepSelected = bn;
                else if (bn == EditorData.FirstStepSelected) EditorData.FirstStepSelected = -1;
                else EditorData.LastStepSelected = bn;
            }
            else
            {
                if (bn < EditorData.FirstStepSelected || bn > EditorData.LastStepSelected)
                {
                    EditorData.FirstStepSelected = bn;
                    EditorData.LastStepSelected = -1;
                }
                else
                {
                    EditorData.FirstStepSelected = -1;
                    EditorData.LastStepSelected = -1;
                }
            }

            // Check indexes order
            if (EditorData.LastStepSelected >= 0 && EditorData.LastStepSelected < EditorData.FirstStepSelected)
            {
                int auxint = EditorData.LastStepSelected;
                EditorData.LastStepSelected = EditorData.FirstStepSelected;
                EditorData.FirstStepSelected = auxint;
            }

            // Update scramble steps colors for selection section
            for (int i = 0; i < EditorData.ScrambleButtons.Length; i++)
            {
                if (i == EditorData.FirstStepSelected)
                    EditorData.ScrambleButtons[i].Style = (Style)FindResource("ScrambleButtonYellowBackKey");
                else if (i > EditorData.FirstStepSelected && i <= EditorData.LastStepSelected)
                    EditorData.ScrambleButtons[i].Style = (Style)FindResource("ScrambleButtonYellowBackKey");
                else EditorData.ScrambleButtons[i].Style = (Style)FindResource("ScrambleButtonGreenBackKey");
            }

            // Enable / disable buttons
            if (EditorData.LastStepSelected > 0) // Multiple step selection
            {
                CloseParenthesisButton.IsEnabled = true;
                OpenParenthesisButton.IsEnabled = true;
                EnableDisableMovementButtons(false);
                EnableDisableTurnButtons(true);
                EditorClearSelectionButton.IsEnabled = true;
            }
            else if (EditorData.FirstStepSelected >= 0) // Single step selection
            {
                CloseParenthesisButton.IsEnabled = false;
                OpenParenthesisButton.IsEnabled = false;
                EnableDisableMovementButtons(true);
                EnableDisableTurnButtons(true);
                EditorClearSelectionButton.IsEnabled = true;
            }
            else // No steps selection
            {
                OpenParenthesisButton.IsEnabled = true;
                CloseParenthesisButton.IsEnabled = !EditorData.Scramble.AreParenthesesOK;
                EnableDisableMovementButtons(true);
                EnableDisableTurnButtons(true);
                EditorClearSelectionButton.IsEnabled = false;
            }
        }

        /// <summary>
        /// Click event: Add movement/turn buttons
        /// </summary>
        private void AddMovementButtons_Click(object sender, RoutedEventArgs e)
        {
            Button but = sender as Button;
            Steps ss = ScrambleStep.StepFromText((string)but.Content);

            if (EditorData.FirstStepSelected < 0) // No selection in the scramble, add step at the end
            {
                EditorData.Scramble.AddStepAndShrink(ss);
                UpdateEditorScramble();

                EditorData.AnimStep = ss;

                if (!EditorData.AnimWork.IsBusy)
                {
                    EditorData.AnimWork.RunWorkerAsync(); // Start scramble animation
                }
            }
            else if (EditorData.LastStepSelected < 0) // One step selected in the scramble, add step before selected step
            {
                EditorData.Scramble.AddStepInPosition(ss, EditorData.FirstStepSelected);
                UpdateEditorScramble();

                EditorData.Cube.Reset();
                EditorData.Cube.ApplyScramble(EditorData.Scramble);

                EditorData.Cube.UpdateBitmap((int)EditorImage3D.ActualWidth, (int)EditorImage3D.ActualHeight);
                EditorImage3D.Source = EditorData.Cube.renderBMP;
                UpdateEditorCube2D();
            }
            else if (ScrambleStep.IsTurn(ss))// Apply turn to scramble selection
            {
                int NumStepsSelected = EditorData.LastStepSelected - EditorData.FirstStepSelected + 1;

                // Check parentheses consitency in the selection
                Scramble sdaux = EditorData.Scramble.SubScramble(EditorData.FirstStepSelected, NumStepsSelected);

                if (!sdaux.AreParenthesesOK) // No consistency
                {
                    MessageBox.Show(AMTexts.Message("CantApplyTurnMessage"), "", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                switch (ScrambleStep.Layer(ss))
                {
                    case Layers.R: // x turn
                        switch (ScrambleStep.Movement(ss))
                        {
                            case Movements.ROT90CW:
                                EditorData.Scramble.ApplyTurnx(EditorData.FirstStepSelected, EditorData.LastStepSelected);
                                break;
                            case Movements.ROT90CCW:
                                EditorData.Scramble.ApplyTurnxp(EditorData.FirstStepSelected, EditorData.LastStepSelected);
                                break;
                            default: // ROT180CW
                                EditorData.Scramble.ApplyTurnx2(EditorData.FirstStepSelected, EditorData.LastStepSelected);
                                break;
                        }
                        break;

                    case Layers.U: // y turn
                        switch (ScrambleStep.Movement(ss))
                        {
                            case Movements.ROT90CW:
                                EditorData.Scramble.ApplyTurny(EditorData.FirstStepSelected, EditorData.LastStepSelected);
                                break;
                            case Movements.ROT90CCW:
                                EditorData.Scramble.ApplyTurnyp(EditorData.FirstStepSelected, EditorData.LastStepSelected);
                                break;
                            default: // ROT180CW
                                EditorData.Scramble.ApplyTurny2(EditorData.FirstStepSelected, EditorData.LastStepSelected);
                                break;
                        }
                        break;

                    case Layers.F: // z turn
                        switch (ScrambleStep.Movement(ss))
                        {
                            case Movements.ROT90CW:
                                EditorData.Scramble.ApplyTurnz(EditorData.FirstStepSelected, EditorData.LastStepSelected);
                                break;
                            case Movements.ROT90CCW:
                                EditorData.Scramble.ApplyTurnzp(EditorData.FirstStepSelected, EditorData.LastStepSelected);
                                break;
                            default: // ROT180CW
                                EditorData.Scramble.ApplyTurnz2(EditorData.FirstStepSelected, EditorData.LastStepSelected);
                                break;
                        }
                        break;
                }
                EditorData.Scramble.AddStepInPosition(ss, EditorData.FirstStepSelected);
                EditorData.Scramble.AddStepInPosition(ScrambleStep.Inverse(ss), EditorData.LastStepSelected + 2);

                UpdateEditorScramble();

                EditorData.Cube.Reset();
                EditorData.Cube.ApplyScramble(EditorData.Scramble);

                EditorData.Cube.UpdateBitmap((int)EditorImage3D.ActualWidth, (int)EditorImage3D.ActualHeight);
                EditorImage3D.Source = EditorData.Cube.renderBMP;
                UpdateEditorCube2D();
            }
            AddScrambleToUndoList();
        }

        /// <summary>
        /// Selection changed event: Update movements buttons
        /// </summary>
        private void EditorModifierCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetEditorButtonsTexts();
        }

        /// <summary>
        /// Click event: Clear editor scramble button
        /// </summary>
        private void EditorClearScrambleButton_Click(object sender, RoutedEventArgs e)
        {
            EditorData.Scramble.Reset();
            UpdateEditorViews();
            EditorData.ParenthesesNest = 0;
            CloseParenthesisButton.IsEnabled = false;

            AddScrambleToUndoList();
        }

        /// <summary>
        /// Click event: Convert to basic steps button
        /// </summary>
        private void ConvertToBasicStepsButton_Click(object sender, RoutedEventArgs e)
        {
            if (EditorData.FirstStepSelected < 0) // No selection in the scramble, all scramble to basic steps
                EditorData.Scramble.ToBasicSteps();

            else if (EditorData.LastStepSelected < 0) // Only a step is selected
            {
                if (EditorData.Scramble[EditorData.FirstStepSelected] < Steps.OPEN_PARENTHESIS)
                {
                    int StepsAdded =
                        EditorData.Scramble.AddStepSequenceInPosition(
                            ScrambleStep.GetEquivalentStepSequence(EditorData.Scramble[EditorData.FirstStepSelected]),
                            EditorData.FirstStepSelected);
                    EditorData.Scramble.DeleteStep(EditorData.FirstStepSelected + StepsAdded);
                }
            }
            else // To basic steps into selection
            {
                Scramble sd, sdaux;
                sd = EditorData.Scramble.SubScramble(0, EditorData.FirstStepSelected);
                sdaux = EditorData.Scramble.SubScramble(EditorData.FirstStepSelected,
                                                        EditorData.LastStepSelected - EditorData.FirstStepSelected + 1);
                sdaux.ToBasicSteps();
                sd.Add(sdaux);
                sd.Add(EditorData.Scramble.SubScramble(EditorData.LastStepSelected + 1,
                                                       EditorData.Scramble.Length - EditorData.LastStepSelected - 1));
                EditorData.Scramble = sd;
            }
            AddScrambleToUndoList();
            UpdateEditorViews();
        }

        /// <summary>
        /// Click event: Shrink scramble button
        /// </summary>
        private void ShrinkStepsButton_Click(object sender, RoutedEventArgs e)
        {
            if (EditorData.FirstStepSelected < 0) // No selection in the scramble, shrink full scramble
                while (EditorData.Scramble.Shrink()) ;

            else if (EditorData.LastStepSelected < 0) return; // One step selected in the scramble, not possible to shrink
            else // Shrink steps into selection
            {
                Scramble sd, sdaux;
                sd = EditorData.Scramble.SubScramble(0, EditorData.FirstStepSelected);
                sdaux = EditorData.Scramble.SubScramble(EditorData.FirstStepSelected,
                                                        EditorData.LastStepSelected - EditorData.FirstStepSelected + 1);
                while (sdaux.Shrink()) ;
                sd.Add(sdaux);
                sd.Add(EditorData.Scramble.SubScramble(EditorData.LastStepSelected + 1,
                                                       EditorData.Scramble.Length - EditorData.LastStepSelected - 1));
                EditorData.Scramble = sd;
            }
            AddScrambleToUndoList();
            UpdateEditorViews();
        }

        /// <summary>
        /// Click event: Develop (remove without altering final scramble) turns button
        /// </summary>
        private void DevelopAllturnsButton_Click(object sender, RoutedEventArgs e)
        { // Remove turns must be applied to all scramble steps!!! It's not possible to apply it into a selection
            if (EditorData.Scramble.RemoveTurns())
            {
                AddScrambleToUndoList();
                UpdateEditorViews();
            }
        }

        /// <summary>
        /// Click event: Open parenthesis button
        /// </summary>
        private void OpenParenthesisButton_Click(object sender, RoutedEventArgs e)
        {
            if (EditorData.LastStepSelected < 0) // No selection in the scramble
            {
                EditorData.Scramble.AddStepWithoutShrink(Steps.OPEN_PARENTHESIS);
                UpdateEditorScramble();
                EditorData.ParenthesesNest++;
                CloseParenthesisButton.IsEnabled = true;
            }
            else // Selection in the scramble
            {
                // Check parentheses consistency in the selection
                Scramble sdaux = EditorData.Scramble.SubScramble(EditorData.FirstStepSelected,
                                                                 EditorData.LastStepSelected - EditorData.FirstStepSelected + 1);

                if (!sdaux.AreParenthesesOK) // No consistency
                {
                    MessageBox.Show(AMTexts.Message("CantCreateNewParenthesesMessage"), "", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                EditorData.Scramble.AddStepInPosition(Steps.CLOSE_PARENTHESIS_1_REP, EditorData.LastStepSelected + 1);
                EditorData.Scramble.AddStepInPosition(Steps.OPEN_PARENTHESIS, EditorData.FirstStepSelected);
                UpdateEditorScramble();
                EditorData.FirstStepSelected = -1;
                EditorData.LastStepSelected = -1;
                CloseParenthesisButton.IsEnabled = !EditorData.Scramble.AreParenthesesOK;
            }
            AddScrambleToUndoList();
        }

        /// <summary>
        /// Click event: Close parenthesis button
        /// </summary>
        private void CloseParenthesisButton_Click(object sender, RoutedEventArgs e)
        {
            int rep = ParenthesisRepetitionsCombo.SelectedIndex + 1;
            if (rep < 1 || rep > 9) rep = 1;

            if (EditorData.LastStepSelected < 0) // No selection in the scramble
            {
                EditorData.Scramble.AddStepWithoutShrink(ScrambleStep.GetCloseParenthesis(rep));
                UpdateEditorScramble();
                EditorData.ParenthesesNest = EditorData.Scramble.NestValue;
                if (EditorData.ParenthesesNest == 0) CloseParenthesisButton.IsEnabled = false;

                if (rep > 1) // When close parentheses the content must be updated
                {
                    EditorData.Cube.Reset();
                    EditorData.Cube.ApplyScramble(EditorData.Scramble);
                    EditorData.Cube.UpdateBitmap((int)EditorImage3D.ActualWidth, (int)EditorImage3D.ActualHeight);
                    EditorImage3D.Source = EditorData.Cube.renderBMP;
                }
            }
            else // Selection in the scramble
            {
                // Check parentheses consistency in the selection
                Scramble sdaux = EditorData.Scramble.SubScramble(EditorData.FirstStepSelected,
                                                                 EditorData.LastStepSelected - EditorData.FirstStepSelected + 1);

                if (!sdaux.AreParenthesesOK) // No consistency
                {
                    MessageBox.Show(AMTexts.Message("CantCreateNewParenthesesMessage"), "", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                EditorData.Scramble.AddStepInPosition(ScrambleStep.GetCloseParenthesis(rep), EditorData.LastStepSelected + 1);
                EditorData.Scramble.AddStepInPosition(Steps.OPEN_PARENTHESIS, EditorData.FirstStepSelected);
                UpdateEditorScramble();
                EditorData.FirstStepSelected = -1;
                EditorData.LastStepSelected = -1;
                CloseParenthesisButton.IsEnabled = !EditorData.Scramble.AreParenthesesOK;
                if (rep > 1)
                {
                    EditorData.Cube.Reset();
                    EditorData.Cube.ApplyScramble(EditorData.Scramble);
                    EditorData.Cube.UpdateBitmap((int)EditorImage3D.ActualWidth, (int)EditorImage3D.ActualHeight);
                    EditorImage3D.Source = EditorData.Cube.renderBMP;
                    UpdateEditorCube2D();
                }
            }
            AddScrambleToUndoList();
        }

        /// <summary>
        /// Click event: Remove parentheses button
        /// </summary>
        private void RemoveParenthesesButton_Click(object sender, RoutedEventArgs e)
        {
            if (EditorData.FirstStepSelected < 0) // No selection in the scramble, remove all parentheses in scramble
                EditorData.Scramble = EditorData.Scramble.DevelopParentheses(false);

            else if (EditorData.LastStepSelected < 0) return; // One step selected in the scramble, not possible to remove parentheses
            else // Remove parentheses in selection
            {
                Scramble sd, sdaux;
                sd = EditorData.Scramble.SubScramble(0, EditorData.FirstStepSelected);
                sdaux = EditorData.Scramble.SubScramble(EditorData.FirstStepSelected,
                                                        EditorData.LastStepSelected - EditorData.FirstStepSelected + 1);
                if (!sdaux.AreParenthesesOK)
                {
                    MessageBox.Show(AMTexts.Message("CantDevelopParenthesesMessage"), "", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                sd.Add(sdaux.DevelopParentheses(false));
                sd.Add(EditorData.Scramble.SubScramble(EditorData.LastStepSelected + 1,
                                                       EditorData.Scramble.Length - EditorData.LastStepSelected - 1));
                EditorData.Scramble = sd;
            }
            AddScrambleToUndoList();
            UpdateEditorViews();
            EditorData.ParenthesesNest = EditorData.Scramble.NestValue;
            CloseParenthesisButton.IsEnabled = false;
        }

        /// <summary>
        /// Click event: Develop parentheses button
        /// </summary>
        private void DevelopParenthesesButton_Click(object sender, RoutedEventArgs e)
        {
            if (EditorData.FirstStepSelected < 0) // No selection in the scramble, develop all parentheses in scramble
                EditorData.Scramble = EditorData.Scramble.DevelopParentheses(true);

            else if (EditorData.LastStepSelected < 0) return; // One step selected in the scramble, not possible to develop parentheses
            else // Develop parentheses in selection
            {
                Scramble sd, sdaux;
                sd = EditorData.Scramble.SubScramble(0, EditorData.FirstStepSelected);
                sdaux = EditorData.Scramble.SubScramble(EditorData.FirstStepSelected,
                                                        EditorData.LastStepSelected - EditorData.FirstStepSelected + 1);
                if (!sdaux.AreParenthesesOK)
                {
                    MessageBox.Show(AMTexts.Message("CantDevelopParenthesesMessage"), "", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                sd.Add(sdaux.DevelopParentheses(true));
                sd.Add(EditorData.Scramble.SubScramble(EditorData.LastStepSelected + 1,
                                                       EditorData.Scramble.Length - EditorData.LastStepSelected - 1));
                EditorData.Scramble = sd;
            }
            AddScrambleToUndoList();
            UpdateEditorViews();
            EditorData.ParenthesesNest = EditorData.Scramble.NestValue;
            CloseParenthesisButton.IsEnabled = false;
        }

        /// <summary>
        /// Click event: Undo in editor scramble
        /// </summary>
        private void EditorUndoButton_Click(object sender, RoutedEventArgs e)
        {
            if (EditorData.UndoList.Count <= 0)
            {
                EditorUndoButton.IsEnabled = false;
                EditorRedoButton.IsEnabled = false;
                return;
            }

            if (EditorData.Scramble.Length == 0)
                EditorData.Scramble.ReadScramble(EditorData.UndoList[EditorData.UndoIndex]);
            else
            {
                int Index = EditorData.UndoIndex;
                if (Index + 1 >= EditorData.UndoList.Count) EditorUndoButton.IsEnabled = false;
                else
                {
                    EditorData.UndoIndex = Index + 1;
                    EditorData.Scramble.ReadScramble(EditorData.UndoList[EditorData.UndoIndex]);
                    if (EditorData.UndoIndex + 1 >= EditorData.UndoList.Count) EditorUndoButton.IsEnabled = false;
                }
            }
            UpdateEditorViews();

            EditorData.ParenthesesNest = EditorData.Scramble.NestValue;
            CloseParenthesisButton.IsEnabled = EditorData.ParenthesesNest > 0;

            EditorRedoButton.IsEnabled = true;
        }

        /// <summary>
        /// Click event: Redo in editor scramble
        /// </summary>
        private void EditorRedoButton_Click(object sender, RoutedEventArgs e)
        {
            if (EditorData.UndoList.Count <= 0)
            {
                EditorUndoButton.IsEnabled = false;
                EditorRedoButton.IsEnabled = false;
                return;
            }

            if (EditorData.Scramble.Length == 0)
                EditorData.Scramble.ReadScramble(EditorData.UndoList[EditorData.UndoIndex]);
            else
            {
                int Index = EditorData.UndoIndex;
                if (Index - 1 < 0) EditorRedoButton.IsEnabled = false;
                else
                {
                    EditorData.UndoIndex = Index - 1;
                    EditorData.Scramble.ReadScramble(EditorData.UndoList[EditorData.UndoIndex]);
                    if (EditorData.UndoIndex <= 0) EditorRedoButton.IsEnabled = false;
                }
            }
            UpdateEditorViews();

            EditorData.ParenthesesNest = EditorData.Scramble.NestValue;
            CloseParenthesisButton.IsEnabled = EditorData.ParenthesesNest > 0;

            EditorUndoButton.IsEnabled = true;
        }

        /// <summary>
        /// Click event: Delete last step button
        /// </summary>
        private void EditorClearLastStepButton_Click(object sender, RoutedEventArgs e)
        {
            if (EditorData.Scramble[EditorData.Scramble.Length - 1] == Steps.OPEN_PARENTHESIS)
            {
                EditorData.ParenthesesNest--;
                if (EditorData.ParenthesesNest == 0) CloseParenthesisButton.IsEnabled = false;
            }
            else if (EditorData.Scramble[EditorData.Scramble.Length - 1] >= Steps.CLOSE_PARENTHESIS_1_REP)
            {
                EditorData.ParenthesesNest++;
                CloseParenthesisButton.IsEnabled = true;
            }
            EditorData.Scramble.DeleteLastStep();
            AddScrambleToUndoList();
            UpdateEditorViews();
        }

        /// <summary>
        /// Click event: Delete selected steps button
        /// </summary>
        private void EditorClearSelectionButton_Click(object sender, RoutedEventArgs e)
        {
            if (EditorData.FirstStepSelected < 0) return;

            if (EditorData.LastStepSelected < 0) // Delete single step
            {
                if (EditorData.Scramble[EditorData.FirstStepSelected] >= Steps.OPEN_PARENTHESIS)
                {
                    MessageBox.Show(AMTexts.Message("CantDeleteSingleParenthesesMessage"), "", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                EditorData.Scramble.DeleteStep(EditorData.FirstStepSelected);
            }
            else
            {
                // Check parentheses consistency in the selection
                Scramble sdaux = EditorData.Scramble.SubScramble(EditorData.FirstStepSelected,
                                                                 EditorData.LastStepSelected - EditorData.FirstStepSelected + 1);
                if (!sdaux.AreParenthesesOK) // No consistency
                {
                    MessageBox.Show(AMTexts.Message("CantDeleteParenthesisInconsitenceMessage"), "", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                for (int i = EditorData.LastStepSelected; i >= EditorData.FirstStepSelected; i--) EditorData.Scramble.DeleteStep(i);
                EditorClearSelectionButton.IsEnabled = false;

                EditorData.FirstStepSelected = -1;
                EditorData.LastStepSelected = -1;
                CloseParenthesisButton.IsEnabled = !EditorData.Scramble.AreParenthesesOK;
            }
            AddScrambleToUndoList();
            UpdateEditorViews();
        }

        /// <summary>
        /// Click event: Copy editor scramble to clipboard button
        /// </summary>
        private void EditorCopyToClipboardButton_Click(object sender, RoutedEventArgs e)
        {
            if (!EditorData.Scramble.AreParenthesesOK)
            {
                EditorData.Scramble.CompleteParentheses();
                UpdateEditorViews();
            }
            try
            {
                Clipboard.SetText(EditorData.Scramble.GetText(" "));
            }
            catch (Exception ex) { AMSettings.Log = "Error setting text into clipboard: " + ex.Message; }
        }

        /// <summary>
        /// Click event: Reverse editor scramble button
        /// </summary>
        private void EditorInvertStepsOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (EditorData.FirstStepSelected < 0) // No selection in the scramble, reverse all scramble
            {
                if (!EditorData.Scramble.Reverse())
                {
                    MessageBox.Show(AMTexts.Message("CantReverseParenthesisInconsistenceMessage"), "", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
            }
            else if (EditorData.LastStepSelected < 0) return; // One step selected in the scramble, reverse not possible
            else // Reverse selection
            {
                Scramble sd, sdaux;
                sd = EditorData.Scramble.SubScramble(0, EditorData.FirstStepSelected);
                sdaux = EditorData.Scramble.SubScramble(EditorData.FirstStepSelected,
                                                        EditorData.LastStepSelected - EditorData.FirstStepSelected + 1);
                if (!sdaux.Reverse())
                {
                    MessageBox.Show(AMTexts.Message("CantReverseParenthesisInconsistenceSelectionMessage"), "", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                sd.Add(sdaux);
                sd.Add(EditorData.Scramble.SubScramble(EditorData.LastStepSelected + 1,
                                                       EditorData.Scramble.Length - EditorData.LastStepSelected - 1));
                EditorData.Scramble = sd;
            }
            if (EditorData.Scramble.AreParenthesesOK) CloseParenthesisButton.IsEnabled = false;
            AddScrambleToUndoList();
            UpdateEditorViews();
        }

        /// <summary>
        /// Click event: Invert editor scramble button
        /// </summary>
        private void EditorInvertMovementsButton_Click(object sender, RoutedEventArgs e)
        {
            if (EditorData.FirstStepSelected < 0) // No selection in the scramble, inverse all scramble
                EditorData.Scramble.Inverse(0, EditorData.Scramble.Length - 1);
            else if (EditorData.LastStepSelected < 0) // One step selected in the scramble, inverse it
                EditorData.Scramble.Inverse(EditorData.FirstStepSelected, EditorData.FirstStepSelected);
            else // Inverse selection
                EditorData.Scramble.Inverse(EditorData.FirstStepSelected, EditorData.LastStepSelected);
            AddScrambleToUndoList();
            UpdateEditorViews();
        }

        /// <summary>
        /// Click event: Reverse & invert scramble button
        /// </summary>
        private void EditorInvertStepsAllButton_Click(object sender, RoutedEventArgs e)
        {
            if (EditorData.FirstStepSelected < 0) // No selection in the scramble, reverse and invert all scramble
            {
                if (!EditorData.Scramble.InverseAndReverseScramble())
                {
                    MessageBox.Show(AMTexts.Message("CantReverseParenthesisInconsistenceMessage"), "", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
            }
            else if (EditorData.LastStepSelected < 0) // One step selected in the scramble: reverse not possible, only invert
                EditorData.Scramble.Inverse(EditorData.FirstStepSelected, EditorData.FirstStepSelected);
            else // Reverse and invert selection
            {
                Scramble sd, sdaux;
                sd = EditorData.Scramble.SubScramble(0, EditorData.FirstStepSelected);
                sdaux = EditorData.Scramble.SubScramble(EditorData.FirstStepSelected,
                                                        EditorData.LastStepSelected - EditorData.FirstStepSelected + 1);
                if (!sdaux.Reverse())
                {
                    MessageBox.Show(AMTexts.Message("CantReverseParenthesisInconsistenceSelectionMessage"), "", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                sdaux.Inverse(0, sdaux.Length - 1);
                sd.Add(sdaux);
                sd.Add(EditorData.Scramble.SubScramble(EditorData.LastStepSelected + 1,
                                                       EditorData.Scramble.Length - EditorData.LastStepSelected - 1));
                EditorData.Scramble = sd;
            }
            if (EditorData.Scramble.AreParenthesesOK) CloseParenthesisButton.IsEnabled = false;
            AddScrambleToUndoList();
            UpdateEditorViews();
        }

        /// <summary>
        /// Click event: Simplify steps
        /// </summary>
        private void EditorSimplifyStepsButton_Click(object sender, RoutedEventArgs e)
        {
            if (EditorData.FirstStepSelected < 0) // No selection in the scramble, inverse all scramble
                EditorData.Scramble.Simplify(0, EditorData.Scramble.Length - 1);
            else if (EditorData.LastStepSelected < 0) // One step selected in the scramble, inverse it
                EditorData.Scramble.Simplify(EditorData.FirstStepSelected, EditorData.FirstStepSelected);
            else // Inverse selection
                EditorData.Scramble.Simplify(EditorData.FirstStepSelected, EditorData.LastStepSelected);
            AddScrambleToUndoList();
            UpdateEditorViews();
        }

        /// <summary>
        /// Click event: New editor scramble from text box button
        /// </summary>
        private void EditorNewFromTextButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EditorScrambleFromTextBox.Text)) return;

            EditorData.Scramble.ReadScramble(EditorScrambleFromTextBox.Text);
            AddScrambleToUndoList();
            UpdateEditorViews();

            EditorData.ParenthesesNest = EditorData.Scramble.NestValue;
            CloseParenthesisButton.IsEnabled = EditorData.ParenthesesNest > 0;
        }

        /// <summary>
        /// Click event: Add text box scramble to current editor scramble button
        /// </summary>
        private void EditorAddFromTextButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EditorScrambleFromTextBox.Text)) return;

            Scramble CS = new Scramble();
            CS.ReadScramble(EditorScrambleFromTextBox.Text);

            EditorData.Scramble.Add(CS);
            AddScrambleToUndoList();
            UpdateEditorViews();

            EditorData.ParenthesesNest = EditorData.Scramble.NestValue;
            CloseParenthesisButton.IsEnabled = EditorData.ParenthesesNest > 0;
        }

        /// <summary>
        /// Click event: Read scramble from clipboard
        /// </summary>
        private void EditorReadFromClipboardButton_Click(object sender, RoutedEventArgs e)
        {
            string scramblefromclipboard = null;

            try { scramblefromclipboard = Clipboard.GetText(); }
            catch { }

            if (string.IsNullOrWhiteSpace(scramblefromclipboard)) return;

            if (scramblefromclipboard.Length > 100)
            {
                if (MessageBox.Show(scramblefromclipboard,
                                    AMTexts.Message("ConfirmScrambleTextMessage"),
                                    MessageBoxButton.YesNo,
                                    MessageBoxImage.Question) != MessageBoxResult.Yes) return;
            }
            EditorData.Scramble.ReadScramble(scramblefromclipboard);

            AddScrambleToUndoList();
            UpdateEditorViews();

            EditorData.ParenthesesNest = EditorData.Scramble.NestValue;
            CloseParenthesisButton.IsEnabled = EditorData.ParenthesesNest > 0;
        }

        #endregion Editor controls events

        #endregion Editor events

        #region Library functions

        /// <summary>
        /// Initialize library fields
        /// </summary>
        public void InitializeLibrary()
        {
            LibraryAlgorithmBarTray.IsLocked = true;

            GoUpLibraryItemButton.IsEnabled = false;
            GoDownLibraryItemButton.IsEnabled = false;

            UpdateLibraryTree();

            LibraryData.InitScramble = new Scramble();
            LibraryData.AlgorithmBase = new Scramble();
            LibraryData.Algorithm = new Scramble();
            LibraryData.InitScrambleCube = new Cube3D();
            LibraryData.AlgorithmCube = new Cube3D();

            LibraryData.CurrentAlgorithm = null;
            LibraryData.CurrentFolder = null;
            LibraryData.CurrentLibrary = null;

            LibraryData.AlgorithmCopied = null;

            SetLibraryCube2DButtons();

            LibraryData.MovingScrambleCube = false;
            LibraryData.MovingAlgorithmCube = false;
            LibraryData.AlgorithmPosition = 0;
            LibraryData.AlgorithmPosTarget = 0;

            // Backgorund worker task for animations
            LibraryData.AnimWork = new BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = false
            };
            LibraryData.AnimWork.DoWork += new DoWorkEventHandler(LibraryAnimWork_DoWork);
            LibraryData.AnimWork.RunWorkerCompleted += new RunWorkerCompletedEventHandler(LibraryAnimWork_RunWorkerCompleted);

            LibraryData.AlgorithmEditionEnabled = false;

            LibraryAlgorithmNameLabelText.Text = AMTexts.Message("OpenAlgorithmMessage");
        }

        /// <summary>
        /// Set buttons arrays for library cube 2D view (initial and algorithm views)
        /// </summary>
        private void SetLibraryCube2DButtons()
        {
            // Buttons for 2D skins
            if (LibraryData.InitPositionStickerButtons == null || LibraryData.InitPositionStickerButtons.Length != 54)
                LibraryData.InitPositionStickerButtons = new Button[54];

            if (LibraryData.AlgorithmStickerButtons == null || LibraryData.AlgorithmStickerButtons.Length != 54)
                LibraryData.AlgorithmStickerButtons = new Button[54];

            // Face up
            LibraryData.InitPositionStickerButtons[(int)StickerPositions.UBL_U] = Library_InitPos_B2D_UBL_U;
            LibraryData.InitPositionStickerButtons[(int)StickerPositions.UB_U] = Library_InitPos_B2D_UB_U;
            LibraryData.InitPositionStickerButtons[(int)StickerPositions.UBR_U] = Library_InitPos_B2D_UBR_U;
            LibraryData.InitPositionStickerButtons[(int)StickerPositions.UL_U] = Library_InitPos_B2D_UL_U;
            LibraryData.InitPositionStickerButtons[(int)StickerPositions.U] = Library_InitPos_B2D_U;
            LibraryData.InitPositionStickerButtons[(int)StickerPositions.UR_U] = Library_InitPos_B2D_UR_U;
            LibraryData.InitPositionStickerButtons[(int)StickerPositions.UFL_U] = Library_InitPos_B2D_UFL_U;
            LibraryData.InitPositionStickerButtons[(int)StickerPositions.UF_U] = Library_InitPos_B2D_UF_U;
            LibraryData.InitPositionStickerButtons[(int)StickerPositions.UFR_U] = Library_InitPos_B2D_UFR_U;

            // Layer up
            LibraryData.InitPositionStickerButtons[(int)StickerPositions.UBL_L] = Library_InitPos_B2D_UBL_L;
            LibraryData.InitPositionStickerButtons[(int)StickerPositions.UL_L] = Library_InitPos_B2D_UL_L;
            LibraryData.InitPositionStickerButtons[(int)StickerPositions.UFL_L] = Library_InitPos_B2D_UFL_L;
            LibraryData.InitPositionStickerButtons[(int)StickerPositions.UFL_F] = Library_InitPos_B2D_UFL_F;
            LibraryData.InitPositionStickerButtons[(int)StickerPositions.UF_F] = Library_InitPos_B2D_UF_F;
            LibraryData.InitPositionStickerButtons[(int)StickerPositions.UFR_F] = Library_InitPos_B2D_UFR_F;
            LibraryData.InitPositionStickerButtons[(int)StickerPositions.UFR_R] = Library_InitPos_B2D_UFR_R;
            LibraryData.InitPositionStickerButtons[(int)StickerPositions.UR_R] = Library_InitPos_B2D_UR_R;
            LibraryData.InitPositionStickerButtons[(int)StickerPositions.UBR_R] = Library_InitPos_B2D_UBR_R;
            LibraryData.InitPositionStickerButtons[(int)StickerPositions.UBR_B] = Library_InitPos_B2D_UBR_B;
            LibraryData.InitPositionStickerButtons[(int)StickerPositions.UB_B] = Library_InitPos_B2D_UB_B;
            LibraryData.InitPositionStickerButtons[(int)StickerPositions.UBL_B] = Library_InitPos_B2D_UBL_B;

            // Layer middle
            LibraryData.InitPositionStickerButtons[(int)StickerPositions.BL_L] = Library_InitPos_B2D_BL_L;
            LibraryData.InitPositionStickerButtons[(int)StickerPositions.L] = Library_InitPos_B2D_L;
            LibraryData.InitPositionStickerButtons[(int)StickerPositions.LF_L] = Library_InitPos_B2D_LF_L;
            LibraryData.InitPositionStickerButtons[(int)StickerPositions.LF_F] = Library_InitPos_B2D_LF_F;
            LibraryData.InitPositionStickerButtons[(int)StickerPositions.F] = Library_InitPos_B2D_F;
            LibraryData.InitPositionStickerButtons[(int)StickerPositions.FR_F] = Library_InitPos_B2D_FR_F;
            LibraryData.InitPositionStickerButtons[(int)StickerPositions.FR_R] = Library_InitPos_B2D_FR_R;
            LibraryData.InitPositionStickerButtons[(int)StickerPositions.R] = Library_InitPos_B2D_R;
            LibraryData.InitPositionStickerButtons[(int)StickerPositions.RB_R] = Library_InitPos_B2D_RB_R;
            LibraryData.InitPositionStickerButtons[(int)StickerPositions.RB_B] = Library_InitPos_B2D_RB_B;
            LibraryData.InitPositionStickerButtons[(int)StickerPositions.B] = Library_InitPos_B2D_B;
            LibraryData.InitPositionStickerButtons[(int)StickerPositions.BL_B] = Library_InitPos_B2D_BL_B;

            // Layer down
            LibraryData.InitPositionStickerButtons[(int)StickerPositions.DBL_L] = Library_InitPos_B2D_DBL_L;
            LibraryData.InitPositionStickerButtons[(int)StickerPositions.DL_L] = Library_InitPos_B2D_DL_L;
            LibraryData.InitPositionStickerButtons[(int)StickerPositions.DFL_L] = Library_InitPos_B2D_DFL_L;
            LibraryData.InitPositionStickerButtons[(int)StickerPositions.DFL_F] = Library_InitPos_B2D_DFL_F;
            LibraryData.InitPositionStickerButtons[(int)StickerPositions.DF_F] = Library_InitPos_B2D_DF_F;
            LibraryData.InitPositionStickerButtons[(int)StickerPositions.DFR_F] = Library_InitPos_B2D_DFR_F;
            LibraryData.InitPositionStickerButtons[(int)StickerPositions.DFR_R] = Library_InitPos_B2D_DFR_R;
            LibraryData.InitPositionStickerButtons[(int)StickerPositions.DR_R] = Library_InitPos_B2D_DR_R;
            LibraryData.InitPositionStickerButtons[(int)StickerPositions.DBR_R] = Library_InitPos_B2D_DBR_R;
            LibraryData.InitPositionStickerButtons[(int)StickerPositions.DBR_B] = Library_InitPos_B2D_DBR_B;
            LibraryData.InitPositionStickerButtons[(int)StickerPositions.DB_B] = Library_InitPos_B2D_DB_B;
            LibraryData.InitPositionStickerButtons[(int)StickerPositions.DBL_B] = Library_InitPos_B2D_DBL_B;

            // Face down
            LibraryData.InitPositionStickerButtons[(int)StickerPositions.DFL_D] = Library_InitPos_B2D_DFL_D;
            LibraryData.InitPositionStickerButtons[(int)StickerPositions.DF_D] = Library_InitPos_B2D_DF_D;
            LibraryData.InitPositionStickerButtons[(int)StickerPositions.DFR_D] = Library_InitPos_B2D_DFR_D;
            LibraryData.InitPositionStickerButtons[(int)StickerPositions.DL_D] = Library_InitPos_B2D_DL_D;
            LibraryData.InitPositionStickerButtons[(int)StickerPositions.D] = Library_InitPos_B2D_D;
            LibraryData.InitPositionStickerButtons[(int)StickerPositions.DR_D] = Library_InitPos_B2D_DR_D;
            LibraryData.InitPositionStickerButtons[(int)StickerPositions.DBL_D] = Library_InitPos_B2D_DBL_D;
            LibraryData.InitPositionStickerButtons[(int)StickerPositions.DB_D] = Library_InitPos_B2D_DB_D;
            LibraryData.InitPositionStickerButtons[(int)StickerPositions.DBR_D] = Library_InitPos_B2D_DBR_D;

            // Face up
            LibraryData.AlgorithmStickerButtons[(int)StickerPositions.UBL_U] = Library_Alg_B2D_UBL_U;
            LibraryData.AlgorithmStickerButtons[(int)StickerPositions.UB_U] = Library_Alg_B2D_UB_U;
            LibraryData.AlgorithmStickerButtons[(int)StickerPositions.UBR_U] = Library_Alg_B2D_UBR_U;
            LibraryData.AlgorithmStickerButtons[(int)StickerPositions.UL_U] = Library_Alg_B2D_UL_U;
            LibraryData.AlgorithmStickerButtons[(int)StickerPositions.U] = Library_Alg_B2D_U;
            LibraryData.AlgorithmStickerButtons[(int)StickerPositions.UR_U] = Library_Alg_B2D_UR_U;
            LibraryData.AlgorithmStickerButtons[(int)StickerPositions.UFL_U] = Library_Alg_B2D_UFL_U;
            LibraryData.AlgorithmStickerButtons[(int)StickerPositions.UF_U] = Library_Alg_B2D_UF_U;
            LibraryData.AlgorithmStickerButtons[(int)StickerPositions.UFR_U] = Library_Alg_B2D_UFR_U;

            // Layer up
            LibraryData.AlgorithmStickerButtons[(int)StickerPositions.UBL_L] = Library_Alg_B2D_UBL_L;
            LibraryData.AlgorithmStickerButtons[(int)StickerPositions.UL_L] = Library_Alg_B2D_UL_L;
            LibraryData.AlgorithmStickerButtons[(int)StickerPositions.UFL_L] = Library_Alg_B2D_UFL_L;
            LibraryData.AlgorithmStickerButtons[(int)StickerPositions.UFL_F] = Library_Alg_B2D_UFL_F;
            LibraryData.AlgorithmStickerButtons[(int)StickerPositions.UF_F] = Library_Alg_B2D_UF_F;
            LibraryData.AlgorithmStickerButtons[(int)StickerPositions.UFR_F] = Library_Alg_B2D_UFR_F;
            LibraryData.AlgorithmStickerButtons[(int)StickerPositions.UFR_R] = Library_Alg_B2D_UFR_R;
            LibraryData.AlgorithmStickerButtons[(int)StickerPositions.UR_R] = Library_Alg_B2D_UR_R;
            LibraryData.AlgorithmStickerButtons[(int)StickerPositions.UBR_R] = Library_Alg_B2D_UBR_R;
            LibraryData.AlgorithmStickerButtons[(int)StickerPositions.UBR_B] = Library_Alg_B2D_UBR_B;
            LibraryData.AlgorithmStickerButtons[(int)StickerPositions.UB_B] = Library_Alg_B2D_UB_B;
            LibraryData.AlgorithmStickerButtons[(int)StickerPositions.UBL_B] = Library_Alg_B2D_UBL_B;

            // Layer middle
            LibraryData.AlgorithmStickerButtons[(int)StickerPositions.BL_L] = Library_Alg_B2D_BL_L;
            LibraryData.AlgorithmStickerButtons[(int)StickerPositions.L] = Library_Alg_B2D_L;
            LibraryData.AlgorithmStickerButtons[(int)StickerPositions.LF_L] = Library_Alg_B2D_LF_L;
            LibraryData.AlgorithmStickerButtons[(int)StickerPositions.LF_F] = Library_Alg_B2D_LF_F;
            LibraryData.AlgorithmStickerButtons[(int)StickerPositions.F] = Library_Alg_B2D_F;
            LibraryData.AlgorithmStickerButtons[(int)StickerPositions.FR_F] = Library_Alg_B2D_FR_F;
            LibraryData.AlgorithmStickerButtons[(int)StickerPositions.FR_R] = Library_Alg_B2D_FR_R;
            LibraryData.AlgorithmStickerButtons[(int)StickerPositions.R] = Library_Alg_B2D_R;
            LibraryData.AlgorithmStickerButtons[(int)StickerPositions.RB_R] = Library_Alg_B2D_RB_R;
            LibraryData.AlgorithmStickerButtons[(int)StickerPositions.RB_B] = Library_Alg_B2D_RB_B;
            LibraryData.AlgorithmStickerButtons[(int)StickerPositions.B] = Library_Alg_B2D_B;
            LibraryData.AlgorithmStickerButtons[(int)StickerPositions.BL_B] = Library_Alg_B2D_BL_B;

            // Layer down
            LibraryData.AlgorithmStickerButtons[(int)StickerPositions.DBL_L] = Library_Alg_B2D_DBL_L;
            LibraryData.AlgorithmStickerButtons[(int)StickerPositions.DL_L] = Library_Alg_B2D_DL_L;
            LibraryData.AlgorithmStickerButtons[(int)StickerPositions.DFL_L] = Library_Alg_B2D_DFL_L;
            LibraryData.AlgorithmStickerButtons[(int)StickerPositions.DFL_F] = Library_Alg_B2D_DFL_F;
            LibraryData.AlgorithmStickerButtons[(int)StickerPositions.DF_F] = Library_Alg_B2D_DF_F;
            LibraryData.AlgorithmStickerButtons[(int)StickerPositions.DFR_F] = Library_Alg_B2D_DFR_F;
            LibraryData.AlgorithmStickerButtons[(int)StickerPositions.DFR_R] = Library_Alg_B2D_DFR_R;
            LibraryData.AlgorithmStickerButtons[(int)StickerPositions.DR_R] = Library_Alg_B2D_DR_R;
            LibraryData.AlgorithmStickerButtons[(int)StickerPositions.DBR_R] = Library_Alg_B2D_DBR_R;
            LibraryData.AlgorithmStickerButtons[(int)StickerPositions.DBR_B] = Library_Alg_B2D_DBR_B;
            LibraryData.AlgorithmStickerButtons[(int)StickerPositions.DB_B] = Library_Alg_B2D_DB_B;
            LibraryData.AlgorithmStickerButtons[(int)StickerPositions.DBL_B] = Library_Alg_B2D_DBL_B;

            // Face down
            LibraryData.AlgorithmStickerButtons[(int)StickerPositions.DFL_D] = Library_Alg_B2D_DFL_D;
            LibraryData.AlgorithmStickerButtons[(int)StickerPositions.DF_D] = Library_Alg_B2D_DF_D;
            LibraryData.AlgorithmStickerButtons[(int)StickerPositions.DFR_D] = Library_Alg_B2D_DFR_D;
            LibraryData.AlgorithmStickerButtons[(int)StickerPositions.DL_D] = Library_Alg_B2D_DL_D;
            LibraryData.AlgorithmStickerButtons[(int)StickerPositions.D] = Library_Alg_B2D_D;
            LibraryData.AlgorithmStickerButtons[(int)StickerPositions.DR_D] = Library_Alg_B2D_DR_D;
            LibraryData.AlgorithmStickerButtons[(int)StickerPositions.DBL_D] = Library_Alg_B2D_DBL_D;
            LibraryData.AlgorithmStickerButtons[(int)StickerPositions.DB_D] = Library_Alg_B2D_DB_D;
            LibraryData.AlgorithmStickerButtons[(int)StickerPositions.DBR_D] = Library_Alg_B2D_DBR_D;

            // Buttons for 2D cube skin views
            for (int n = 0; n < LibraryData.InitPositionStickerButtons.Length; n++)
            {
                LibraryData.InitPositionStickerButtons[n].Click += LibraryInitPosCube2DButtons_Click;
                LibraryData.InitPositionStickerButtons[n].Tag = n;
            }

            for (int n = 0; n < LibraryData.AlgorithmStickerButtons.Length; n++)
            {
                LibraryData.AlgorithmStickerButtons[n].Click += LibraryAlgorithmCube2DButtons_Click;
                LibraryData.AlgorithmStickerButtons[n].Tag = n;
            }

            // Rectangles for layers 2D views
            LibraryData.InitPosLayersRects = new Rectangle[6][]; // One array per face
            LibraryData.AlgorithmLayersRects = new Rectangle[6][]; // One array per face

            for (int n = 0; n < 6; n++)
            {
                LibraryData.InitPosLayersRects[n] = new Rectangle[54];
                LibraryData.AlgorithmLayersRects[n] = new Rectangle[54];
                for (int m = 0; m < 54; m++)
                {
                    LibraryData.InitPosLayersRects[n][m] = null;
                    LibraryData.AlgorithmLayersRects[n][m] = null;
                }
            }

            // Faces views for initial position
            // Layer U
            LibraryData.InitPosLayersRects[(int)Faces.U][(int)StickersULayer.UBL_B] = LibraryInitCanvasUpLayer_UBL_B;
            LibraryData.InitPosLayersRects[(int)Faces.U][(int)StickersULayer.UB_B] = LibraryInitCanvasUpLayer_UB_B;
            LibraryData.InitPosLayersRects[(int)Faces.U][(int)StickersULayer.UBR_B] = LibraryInitCanvasUpLayer_UBR_B;

            LibraryData.InitPosLayersRects[(int)Faces.U][(int)StickersULayer.UBL_L] = LibraryInitCanvasUpLayer_UBL_L;
            LibraryData.InitPosLayersRects[(int)Faces.U][(int)StickersULayer.UBL_U] = LibraryInitCanvasUpLayer_UBL_U;
            LibraryData.InitPosLayersRects[(int)Faces.U][(int)StickersULayer.UB_U] = LibraryInitCanvasUpLayer_UB_U;
            LibraryData.InitPosLayersRects[(int)Faces.U][(int)StickersULayer.UBR_U] = LibraryInitCanvasUpLayer_UBR_U;
            LibraryData.InitPosLayersRects[(int)Faces.U][(int)StickersULayer.UBR_R] = LibraryInitCanvasUpLayer_UBR_R;

            LibraryData.InitPosLayersRects[(int)Faces.U][(int)StickersULayer.UL_L] = LibraryInitCanvasUpLayer_UL_L;
            LibraryData.InitPosLayersRects[(int)Faces.U][(int)StickersULayer.UL_U] = LibraryInitCanvasUpLayer_UL_U;
            LibraryData.InitPosLayersRects[(int)Faces.U][(int)StickersULayer.U] = LibraryInitCanvasUpLayer_U_U;
            LibraryData.InitPosLayersRects[(int)Faces.U][(int)StickersULayer.UR_U] = LibraryInitCanvasUpLayer_UR_U;
            LibraryData.InitPosLayersRects[(int)Faces.U][(int)StickersULayer.UR_R] = LibraryInitCanvasUpLayer_UR_R;

            LibraryData.InitPosLayersRects[(int)Faces.U][(int)StickersULayer.UFL_L] = LibraryInitCanvasUpLayer_UFL_L;
            LibraryData.InitPosLayersRects[(int)Faces.U][(int)StickersULayer.UFL_U] = LibraryInitCanvasUpLayer_UFL_U;
            LibraryData.InitPosLayersRects[(int)Faces.U][(int)StickersULayer.UF_U] = LibraryInitCanvasUpLayer_UF_U;
            LibraryData.InitPosLayersRects[(int)Faces.U][(int)StickersULayer.UFR_U] = LibraryInitCanvasUpLayer_UFR_U;
            LibraryData.InitPosLayersRects[(int)Faces.U][(int)StickersULayer.UFR_R] = LibraryInitCanvasUpLayer_UFR_R;

            LibraryData.InitPosLayersRects[(int)Faces.U][(int)StickersULayer.UFL_F] = LibraryInitCanvasUpLayer_UFL_F;
            LibraryData.InitPosLayersRects[(int)Faces.U][(int)StickersULayer.UF_F] = LibraryInitCanvasUpLayer_UF_F;
            LibraryData.InitPosLayersRects[(int)Faces.U][(int)StickersULayer.UFR_F] = LibraryInitCanvasUpLayer_UFR_F;

            // Layer D
            LibraryData.InitPosLayersRects[(int)Faces.D][(int)StickersDLayer.DFL_F] = LibraryInitCanvasDownLayer_DFL_F;
            LibraryData.InitPosLayersRects[(int)Faces.D][(int)StickersDLayer.DF_F] = LibraryInitCanvasDownLayer_DF_F;
            LibraryData.InitPosLayersRects[(int)Faces.D][(int)StickersDLayer.DFR_F] = LibraryInitCanvasDownLayer_DFR_F;

            LibraryData.InitPosLayersRects[(int)Faces.D][(int)StickersDLayer.DFL_L] = LibraryInitCanvasDownLayer_DFL_L;
            LibraryData.InitPosLayersRects[(int)Faces.D][(int)StickersDLayer.DFL_D] = LibraryInitCanvasDownLayer_DFL_D;
            LibraryData.InitPosLayersRects[(int)Faces.D][(int)StickersDLayer.DF_D] = LibraryInitCanvasDownLayer_DF_D;
            LibraryData.InitPosLayersRects[(int)Faces.D][(int)StickersDLayer.DFR_D] = LibraryInitCanvasDownLayer_DFR_D;
            LibraryData.InitPosLayersRects[(int)Faces.D][(int)StickersDLayer.DFR_R] = LibraryInitCanvasDownLayer_DFR_R;

            LibraryData.InitPosLayersRects[(int)Faces.D][(int)StickersDLayer.DL_L] = LibraryInitCanvasDownLayer_DL_L;
            LibraryData.InitPosLayersRects[(int)Faces.D][(int)StickersDLayer.DL_D] = LibraryInitCanvasDownLayer_DL_D;
            LibraryData.InitPosLayersRects[(int)Faces.D][(int)StickersDLayer.D] = LibraryInitCanvasDownLayer_D_D;
            LibraryData.InitPosLayersRects[(int)Faces.D][(int)StickersDLayer.DR_D] = LibraryInitCanvasDownLayer_DR_D;
            LibraryData.InitPosLayersRects[(int)Faces.D][(int)StickersDLayer.DR_R] = LibraryInitCanvasDownLayer_DR_R;

            LibraryData.InitPosLayersRects[(int)Faces.D][(int)StickersDLayer.DBL_L] = LibraryInitCanvasDownLayer_DBL_L;
            LibraryData.InitPosLayersRects[(int)Faces.D][(int)StickersDLayer.DBL_D] = LibraryInitCanvasDownLayer_DBL_D;
            LibraryData.InitPosLayersRects[(int)Faces.D][(int)StickersDLayer.DB_D] = LibraryInitCanvasDownLayer_DB_D;
            LibraryData.InitPosLayersRects[(int)Faces.D][(int)StickersDLayer.DBR_D] = LibraryInitCanvasDownLayer_DBR_D;
            LibraryData.InitPosLayersRects[(int)Faces.D][(int)StickersDLayer.DBR_R] = LibraryInitCanvasDownLayer_DBR_R;

            LibraryData.InitPosLayersRects[(int)Faces.D][(int)StickersDLayer.DBL_B] = LibraryInitCanvasDownLayer_DBL_B;
            LibraryData.InitPosLayersRects[(int)Faces.D][(int)StickersDLayer.DB_B] = LibraryInitCanvasDownLayer_DB_B;
            LibraryData.InitPosLayersRects[(int)Faces.D][(int)StickersDLayer.DBR_B] = LibraryInitCanvasDownLayer_DBR_B;

            // Layer F
            LibraryData.InitPosLayersRects[(int)Faces.F][(int)StickersFLayer.UFL_U] = LibraryInitCanvasFrontLayer_UFL_U;
            LibraryData.InitPosLayersRects[(int)Faces.F][(int)StickersFLayer.UF_U] = LibraryInitCanvasFrontLayer_UF_U;
            LibraryData.InitPosLayersRects[(int)Faces.F][(int)StickersFLayer.UFR_U] = LibraryInitCanvasFrontLayer_UFR_U;

            LibraryData.InitPosLayersRects[(int)Faces.F][(int)StickersFLayer.UFL_L] = LibraryInitCanvasFrontLayer_UFL_L;
            LibraryData.InitPosLayersRects[(int)Faces.F][(int)StickersFLayer.UFL_F] = LibraryInitCanvasFrontLayer_UFL_F;
            LibraryData.InitPosLayersRects[(int)Faces.F][(int)StickersFLayer.UF_F] = LibraryInitCanvasFrontLayer_UF_F;
            LibraryData.InitPosLayersRects[(int)Faces.F][(int)StickersFLayer.UFR_F] = LibraryInitCanvasFrontLayer_UFR_F;
            LibraryData.InitPosLayersRects[(int)Faces.F][(int)StickersFLayer.UFR_R] = LibraryInitCanvasFrontLayer_UFR_R;

            LibraryData.InitPosLayersRects[(int)Faces.F][(int)StickersFLayer.LF_L] = LibraryInitCanvasFrontLayer_LF_L;
            LibraryData.InitPosLayersRects[(int)Faces.F][(int)StickersFLayer.LF_F] = LibraryInitCanvasFrontLayer_LF_F;
            LibraryData.InitPosLayersRects[(int)Faces.F][(int)StickersFLayer.F] = LibraryInitCanvasFrontLayer_F_F;
            LibraryData.InitPosLayersRects[(int)Faces.F][(int)StickersFLayer.FR_F] = LibraryInitCanvasFrontLayer_FR_F;
            LibraryData.InitPosLayersRects[(int)Faces.F][(int)StickersFLayer.FR_R] = LibraryInitCanvasFrontLayer_FR_R;

            LibraryData.InitPosLayersRects[(int)Faces.F][(int)StickersFLayer.DFL_L] = LibraryInitCanvasFrontLayer_DFL_L;
            LibraryData.InitPosLayersRects[(int)Faces.F][(int)StickersFLayer.DFL_F] = LibraryInitCanvasFrontLayer_DFL_F;
            LibraryData.InitPosLayersRects[(int)Faces.F][(int)StickersFLayer.DF_F] = LibraryInitCanvasFrontLayer_DF_F;
            LibraryData.InitPosLayersRects[(int)Faces.F][(int)StickersFLayer.DFR_F] = LibraryInitCanvasFrontLayer_DFR_F;
            LibraryData.InitPosLayersRects[(int)Faces.F][(int)StickersFLayer.DFR_R] = LibraryInitCanvasFrontLayer_DFR_R;

            LibraryData.InitPosLayersRects[(int)Faces.F][(int)StickersFLayer.DFL_D] = LibraryInitCanvasFrontLayer_DFL_D;
            LibraryData.InitPosLayersRects[(int)Faces.F][(int)StickersFLayer.DF_D] = LibraryInitCanvasFrontLayer_DF_D;
            LibraryData.InitPosLayersRects[(int)Faces.F][(int)StickersFLayer.DFR_D] = LibraryInitCanvasFrontLayer_DFR_D;

            // Layer B
            LibraryData.InitPosLayersRects[(int)Faces.B][(int)StickersBLayer.UBR_U] = LibraryInitCanvasBackLayer_UBR_U;
            LibraryData.InitPosLayersRects[(int)Faces.B][(int)StickersBLayer.UB_U] = LibraryInitCanvasBackLayer_UB_U;
            LibraryData.InitPosLayersRects[(int)Faces.B][(int)StickersBLayer.UBL_U] = LibraryInitCanvasBackLayer_UBL_U;

            LibraryData.InitPosLayersRects[(int)Faces.B][(int)StickersBLayer.UBR_R] = LibraryInitCanvasBackLayer_UBR_R;
            LibraryData.InitPosLayersRects[(int)Faces.B][(int)StickersBLayer.UBR_B] = LibraryInitCanvasBackLayer_UBR_B;
            LibraryData.InitPosLayersRects[(int)Faces.B][(int)StickersBLayer.UB_B] = LibraryInitCanvasBackLayer_UB_B;
            LibraryData.InitPosLayersRects[(int)Faces.B][(int)StickersBLayer.UBL_B] = LibraryInitCanvasBackLayer_UBL_B;
            LibraryData.InitPosLayersRects[(int)Faces.B][(int)StickersBLayer.UBL_L] = LibraryInitCanvasBackLayer_UBL_L;

            LibraryData.InitPosLayersRects[(int)Faces.B][(int)StickersBLayer.RB_R] = LibraryInitCanvasBackLayer_RB_R;
            LibraryData.InitPosLayersRects[(int)Faces.B][(int)StickersBLayer.RB_B] = LibraryInitCanvasBackLayer_RB_B;
            LibraryData.InitPosLayersRects[(int)Faces.B][(int)StickersBLayer.B] = LibraryInitCanvasBackLayer_B_B;
            LibraryData.InitPosLayersRects[(int)Faces.B][(int)StickersBLayer.BL_B] = LibraryInitCanvasBackLayer_BL_B;
            LibraryData.InitPosLayersRects[(int)Faces.B][(int)StickersBLayer.BL_L] = LibraryInitCanvasBackLayer_BL_L;

            LibraryData.InitPosLayersRects[(int)Faces.B][(int)StickersBLayer.DBR_R] = LibraryInitCanvasBackLayer_DBR_R;
            LibraryData.InitPosLayersRects[(int)Faces.B][(int)StickersBLayer.DBR_B] = LibraryInitCanvasBackLayer_DBR_B;
            LibraryData.InitPosLayersRects[(int)Faces.B][(int)StickersBLayer.DB_B] = LibraryInitCanvasBackLayer_DB_B;
            LibraryData.InitPosLayersRects[(int)Faces.B][(int)StickersBLayer.DBL_B] = LibraryInitCanvasBackLayer_DBL_B;
            LibraryData.InitPosLayersRects[(int)Faces.B][(int)StickersBLayer.DBL_L] = LibraryInitCanvasBackLayer_DBL_L;

            LibraryData.InitPosLayersRects[(int)Faces.B][(int)StickersBLayer.DBR_D] = LibraryInitCanvasBackLayer_DBR_D;
            LibraryData.InitPosLayersRects[(int)Faces.B][(int)StickersBLayer.DB_D] = LibraryInitCanvasBackLayer_DB_D;
            LibraryData.InitPosLayersRects[(int)Faces.B][(int)StickersBLayer.DBL_D] = LibraryInitCanvasBackLayer_DBL_D;

            // Layer R
            LibraryData.InitPosLayersRects[(int)Faces.R][(int)StickersRLayer.UFR_U] = LibraryInitCanvasRightLayer_UFR_U;
            LibraryData.InitPosLayersRects[(int)Faces.R][(int)StickersRLayer.UR_U] = LibraryInitCanvasRightLayer_UR_U;
            LibraryData.InitPosLayersRects[(int)Faces.R][(int)StickersRLayer.UBR_U] = LibraryInitCanvasRightLayer_UBR_U;

            LibraryData.InitPosLayersRects[(int)Faces.R][(int)StickersRLayer.UFR_F] = LibraryInitCanvasRightLayer_UFR_F;
            LibraryData.InitPosLayersRects[(int)Faces.R][(int)StickersRLayer.UFR_R] = LibraryInitCanvasRightLayer_UFR_R;
            LibraryData.InitPosLayersRects[(int)Faces.R][(int)StickersRLayer.UR_R] = LibraryInitCanvasRightLayer_UR_R;
            LibraryData.InitPosLayersRects[(int)Faces.R][(int)StickersRLayer.UBR_R] = LibraryInitCanvasRightLayer_UBR_R;
            LibraryData.InitPosLayersRects[(int)Faces.R][(int)StickersRLayer.UBR_B] = LibraryInitCanvasRightLayer_UBR_B;

            LibraryData.InitPosLayersRects[(int)Faces.R][(int)StickersRLayer.FR_F] = LibraryInitCanvasRightLayer_FR_F;
            LibraryData.InitPosLayersRects[(int)Faces.R][(int)StickersRLayer.FR_R] = LibraryInitCanvasRightLayer_FR_R;
            LibraryData.InitPosLayersRects[(int)Faces.R][(int)StickersRLayer.R] = LibraryInitCanvasRightLayer_R_R;
            LibraryData.InitPosLayersRects[(int)Faces.R][(int)StickersRLayer.RB_R] = LibraryInitCanvasRightLayer_RB_R;
            LibraryData.InitPosLayersRects[(int)Faces.R][(int)StickersRLayer.RB_B] = LibraryInitCanvasRightLayer_RB_B;

            LibraryData.InitPosLayersRects[(int)Faces.R][(int)StickersRLayer.DFR_F] = LibraryInitCanvasRightLayer_DFR_F;
            LibraryData.InitPosLayersRects[(int)Faces.R][(int)StickersRLayer.DFR_R] = LibraryInitCanvasRightLayer_DFR_R;
            LibraryData.InitPosLayersRects[(int)Faces.R][(int)StickersRLayer.DR_R] = LibraryInitCanvasRightLayer_DR_R;
            LibraryData.InitPosLayersRects[(int)Faces.R][(int)StickersRLayer.DBR_R] = LibraryInitCanvasRightLayer_DBR_R;
            LibraryData.InitPosLayersRects[(int)Faces.R][(int)StickersRLayer.DBR_B] = LibraryInitCanvasRightLayer_DBR_B;

            LibraryData.InitPosLayersRects[(int)Faces.R][(int)StickersRLayer.DFR_D] = LibraryInitCanvasRightLayer_DFR_D;
            LibraryData.InitPosLayersRects[(int)Faces.R][(int)StickersRLayer.DR_D] = LibraryInitCanvasRightLayer_DR_D;
            LibraryData.InitPosLayersRects[(int)Faces.R][(int)StickersRLayer.DBR_D] = LibraryInitCanvasRightLayer_DBR_D;

            // Layer L
            LibraryData.InitPosLayersRects[(int)Faces.L][(int)StickersLLayer.UBL_U] = LibraryInitCanvasLeftLayer_UBL_U;
            LibraryData.InitPosLayersRects[(int)Faces.L][(int)StickersLLayer.UL_U] = LibraryInitCanvasLeftLayer_UL_U;
            LibraryData.InitPosLayersRects[(int)Faces.L][(int)StickersLLayer.UFL_U] = LibraryInitCanvasLeftLayer_UFL_U;

            LibraryData.InitPosLayersRects[(int)Faces.L][(int)StickersLLayer.UBL_B] = LibraryInitCanvasLeftLayer_UBL_B;
            LibraryData.InitPosLayersRects[(int)Faces.L][(int)StickersLLayer.UBL_L] = LibraryInitCanvasLeftLayer_UBL_L;
            LibraryData.InitPosLayersRects[(int)Faces.L][(int)StickersLLayer.UL_L] = LibraryInitCanvasLeftLayer_UL_L;
            LibraryData.InitPosLayersRects[(int)Faces.L][(int)StickersLLayer.UFL_L] = LibraryInitCanvasLeftLayer_UFL_L;
            LibraryData.InitPosLayersRects[(int)Faces.L][(int)StickersLLayer.UFL_F] = LibraryInitCanvasLeftLayer_UFL_F;

            LibraryData.InitPosLayersRects[(int)Faces.L][(int)StickersLLayer.BL_B] = LibraryInitCanvasLeftLayer_BL_B;
            LibraryData.InitPosLayersRects[(int)Faces.L][(int)StickersLLayer.BL_L] = LibraryInitCanvasLeftLayer_BL_L;
            LibraryData.InitPosLayersRects[(int)Faces.L][(int)StickersLLayer.L] = LibraryInitCanvasLeftLayer_L_L;
            LibraryData.InitPosLayersRects[(int)Faces.L][(int)StickersLLayer.LF_L] = LibraryInitCanvasLeftLayer_LF_L;
            LibraryData.InitPosLayersRects[(int)Faces.L][(int)StickersLLayer.LF_F] = LibraryInitCanvasLeftLayer_LF_F;

            LibraryData.InitPosLayersRects[(int)Faces.L][(int)StickersLLayer.DBL_B] = LibraryInitCanvasLeftLayer_DBL_B;
            LibraryData.InitPosLayersRects[(int)Faces.L][(int)StickersLLayer.DBL_L] = LibraryInitCanvasLeftLayer_DBL_L;
            LibraryData.InitPosLayersRects[(int)Faces.L][(int)StickersLLayer.DL_L] = LibraryInitCanvasLeftLayer_DL_L;
            LibraryData.InitPosLayersRects[(int)Faces.L][(int)StickersLLayer.DFL_L] = LibraryInitCanvasLeftLayer_DFL_L;
            LibraryData.InitPosLayersRects[(int)Faces.L][(int)StickersLLayer.DFL_F] = LibraryInitCanvasLeftLayer_DFL_F;

            LibraryData.InitPosLayersRects[(int)Faces.L][(int)StickersLLayer.DBL_D] = LibraryInitCanvasLeftLayer_DBL_D;
            LibraryData.InitPosLayersRects[(int)Faces.L][(int)StickersLLayer.DL_D] = LibraryInitCanvasLeftLayer_DL_D;
            LibraryData.InitPosLayersRects[(int)Faces.L][(int)StickersLLayer.DFL_D] = LibraryInitCanvasLeftLayer_DFL_D;

            // Faces views for algorithm
            // Layer U
            LibraryData.AlgorithmLayersRects[(int)Faces.U][(int)StickersULayer.UBL_B] = LibraryAlgCanvasUpLayer_UBL_B;
            LibraryData.AlgorithmLayersRects[(int)Faces.U][(int)StickersULayer.UB_B] = LibraryAlgCanvasUpLayer_UB_B;
            LibraryData.AlgorithmLayersRects[(int)Faces.U][(int)StickersULayer.UBR_B] = LibraryAlgCanvasUpLayer_UBR_B;

            LibraryData.AlgorithmLayersRects[(int)Faces.U][(int)StickersULayer.UBL_L] = LibraryAlgCanvasUpLayer_UBL_L;
            LibraryData.AlgorithmLayersRects[(int)Faces.U][(int)StickersULayer.UBL_U] = LibraryAlgCanvasUpLayer_UBL_U;
            LibraryData.AlgorithmLayersRects[(int)Faces.U][(int)StickersULayer.UB_U] = LibraryAlgCanvasUpLayer_UB_U;
            LibraryData.AlgorithmLayersRects[(int)Faces.U][(int)StickersULayer.UBR_U] = LibraryAlgCanvasUpLayer_UBR_U;
            LibraryData.AlgorithmLayersRects[(int)Faces.U][(int)StickersULayer.UBR_R] = LibraryAlgCanvasUpLayer_UBR_R;

            LibraryData.AlgorithmLayersRects[(int)Faces.U][(int)StickersULayer.UL_L] = LibraryAlgCanvasUpLayer_UL_L;
            LibraryData.AlgorithmLayersRects[(int)Faces.U][(int)StickersULayer.UL_U] = LibraryAlgCanvasUpLayer_UL_U;
            LibraryData.AlgorithmLayersRects[(int)Faces.U][(int)StickersULayer.U] = LibraryAlgCanvasUpLayer_U_U;
            LibraryData.AlgorithmLayersRects[(int)Faces.U][(int)StickersULayer.UR_U] = LibraryAlgCanvasUpLayer_UR_U;
            LibraryData.AlgorithmLayersRects[(int)Faces.U][(int)StickersULayer.UR_R] = LibraryAlgCanvasUpLayer_UR_R;

            LibraryData.AlgorithmLayersRects[(int)Faces.U][(int)StickersULayer.UFL_L] = LibraryAlgCanvasUpLayer_UFL_L;
            LibraryData.AlgorithmLayersRects[(int)Faces.U][(int)StickersULayer.UFL_U] = LibraryAlgCanvasUpLayer_UFL_U;
            LibraryData.AlgorithmLayersRects[(int)Faces.U][(int)StickersULayer.UF_U] = LibraryAlgCanvasUpLayer_UF_U;
            LibraryData.AlgorithmLayersRects[(int)Faces.U][(int)StickersULayer.UFR_U] = LibraryAlgCanvasUpLayer_UFR_U;
            LibraryData.AlgorithmLayersRects[(int)Faces.U][(int)StickersULayer.UFR_R] = LibraryAlgCanvasUpLayer_UFR_R;

            LibraryData.AlgorithmLayersRects[(int)Faces.U][(int)StickersULayer.UFL_F] = LibraryAlgCanvasUpLayer_UFL_F;
            LibraryData.AlgorithmLayersRects[(int)Faces.U][(int)StickersULayer.UF_F] = LibraryAlgCanvasUpLayer_UF_F;
            LibraryData.AlgorithmLayersRects[(int)Faces.U][(int)StickersULayer.UFR_F] = LibraryAlgCanvasUpLayer_UFR_F;

            // Layer D
            LibraryData.AlgorithmLayersRects[(int)Faces.D][(int)StickersDLayer.DFL_F] = LibraryAlgCanvasDownLayer_DFL_F;
            LibraryData.AlgorithmLayersRects[(int)Faces.D][(int)StickersDLayer.DF_F] = LibraryAlgCanvasDownLayer_DF_F;
            LibraryData.AlgorithmLayersRects[(int)Faces.D][(int)StickersDLayer.DFR_F] = LibraryAlgCanvasDownLayer_DFR_F;

            LibraryData.AlgorithmLayersRects[(int)Faces.D][(int)StickersDLayer.DFL_L] = LibraryAlgCanvasDownLayer_DFL_L;
            LibraryData.AlgorithmLayersRects[(int)Faces.D][(int)StickersDLayer.DFL_D] = LibraryAlgCanvasDownLayer_DFL_D;
            LibraryData.AlgorithmLayersRects[(int)Faces.D][(int)StickersDLayer.DF_D] = LibraryAlgCanvasDownLayer_DF_D;
            LibraryData.AlgorithmLayersRects[(int)Faces.D][(int)StickersDLayer.DFR_D] = LibraryAlgCanvasDownLayer_DFR_D;
            LibraryData.AlgorithmLayersRects[(int)Faces.D][(int)StickersDLayer.DFR_R] = LibraryAlgCanvasDownLayer_DFR_R;

            LibraryData.AlgorithmLayersRects[(int)Faces.D][(int)StickersDLayer.DL_L] = LibraryAlgCanvasDownLayer_DL_L;
            LibraryData.AlgorithmLayersRects[(int)Faces.D][(int)StickersDLayer.DL_D] = LibraryAlgCanvasDownLayer_DL_D;
            LibraryData.AlgorithmLayersRects[(int)Faces.D][(int)StickersDLayer.D] = LibraryAlgCanvasDownLayer_D_D;
            LibraryData.AlgorithmLayersRects[(int)Faces.D][(int)StickersDLayer.DR_D] = LibraryAlgCanvasDownLayer_DR_D;
            LibraryData.AlgorithmLayersRects[(int)Faces.D][(int)StickersDLayer.DR_R] = LibraryAlgCanvasDownLayer_DR_R;

            LibraryData.AlgorithmLayersRects[(int)Faces.D][(int)StickersDLayer.DBL_L] = LibraryAlgCanvasDownLayer_DBL_L;
            LibraryData.AlgorithmLayersRects[(int)Faces.D][(int)StickersDLayer.DBL_D] = LibraryAlgCanvasDownLayer_DBL_D;
            LibraryData.AlgorithmLayersRects[(int)Faces.D][(int)StickersDLayer.DB_D] = LibraryAlgCanvasDownLayer_DB_D;
            LibraryData.AlgorithmLayersRects[(int)Faces.D][(int)StickersDLayer.DBR_D] = LibraryAlgCanvasDownLayer_DBR_D;
            LibraryData.AlgorithmLayersRects[(int)Faces.D][(int)StickersDLayer.DBR_R] = LibraryAlgCanvasDownLayer_DBR_R;

            LibraryData.AlgorithmLayersRects[(int)Faces.D][(int)StickersDLayer.DBL_B] = LibraryAlgCanvasDownLayer_DBL_B;
            LibraryData.AlgorithmLayersRects[(int)Faces.D][(int)StickersDLayer.DB_B] = LibraryAlgCanvasDownLayer_DB_B;
            LibraryData.AlgorithmLayersRects[(int)Faces.D][(int)StickersDLayer.DBR_B] = LibraryAlgCanvasDownLayer_DBR_B;

            // Layer F
            LibraryData.AlgorithmLayersRects[(int)Faces.F][(int)StickersFLayer.UFL_U] = LibraryAlgCanvasFrontLayer_UFL_U;
            LibraryData.AlgorithmLayersRects[(int)Faces.F][(int)StickersFLayer.UF_U] = LibraryAlgCanvasFrontLayer_UF_U;
            LibraryData.AlgorithmLayersRects[(int)Faces.F][(int)StickersFLayer.UFR_U] = LibraryAlgCanvasFrontLayer_UFR_U;

            LibraryData.AlgorithmLayersRects[(int)Faces.F][(int)StickersFLayer.UFL_L] = LibraryAlgCanvasFrontLayer_UFL_L;
            LibraryData.AlgorithmLayersRects[(int)Faces.F][(int)StickersFLayer.UFL_F] = LibraryAlgCanvasFrontLayer_UFL_F;
            LibraryData.AlgorithmLayersRects[(int)Faces.F][(int)StickersFLayer.UF_F] = LibraryAlgCanvasFrontLayer_UF_F;
            LibraryData.AlgorithmLayersRects[(int)Faces.F][(int)StickersFLayer.UFR_F] = LibraryAlgCanvasFrontLayer_UFR_F;
            LibraryData.AlgorithmLayersRects[(int)Faces.F][(int)StickersFLayer.UFR_R] = LibraryAlgCanvasFrontLayer_UFR_R;

            LibraryData.AlgorithmLayersRects[(int)Faces.F][(int)StickersFLayer.LF_L] = LibraryAlgCanvasFrontLayer_LF_L;
            LibraryData.AlgorithmLayersRects[(int)Faces.F][(int)StickersFLayer.LF_F] = LibraryAlgCanvasFrontLayer_LF_F;
            LibraryData.AlgorithmLayersRects[(int)Faces.F][(int)StickersFLayer.F] = LibraryAlgCanvasFrontLayer_F_F;
            LibraryData.AlgorithmLayersRects[(int)Faces.F][(int)StickersFLayer.FR_F] = LibraryAlgCanvasFrontLayer_FR_F;
            LibraryData.AlgorithmLayersRects[(int)Faces.F][(int)StickersFLayer.FR_R] = LibraryAlgCanvasFrontLayer_FR_R;

            LibraryData.AlgorithmLayersRects[(int)Faces.F][(int)StickersFLayer.DFL_L] = LibraryAlgCanvasFrontLayer_DFL_L;
            LibraryData.AlgorithmLayersRects[(int)Faces.F][(int)StickersFLayer.DFL_F] = LibraryAlgCanvasFrontLayer_DFL_F;
            LibraryData.AlgorithmLayersRects[(int)Faces.F][(int)StickersFLayer.DF_F] = LibraryAlgCanvasFrontLayer_DF_F;
            LibraryData.AlgorithmLayersRects[(int)Faces.F][(int)StickersFLayer.DFR_F] = LibraryAlgCanvasFrontLayer_DFR_F;
            LibraryData.AlgorithmLayersRects[(int)Faces.F][(int)StickersFLayer.DFR_R] = LibraryAlgCanvasFrontLayer_DFR_R;

            LibraryData.AlgorithmLayersRects[(int)Faces.F][(int)StickersFLayer.DFL_D] = LibraryAlgCanvasFrontLayer_DFL_D;
            LibraryData.AlgorithmLayersRects[(int)Faces.F][(int)StickersFLayer.DF_D] = LibraryAlgCanvasFrontLayer_DF_D;
            LibraryData.AlgorithmLayersRects[(int)Faces.F][(int)StickersFLayer.DFR_D] = LibraryAlgCanvasFrontLayer_DFR_D;

            // Layer B
            LibraryData.AlgorithmLayersRects[(int)Faces.B][(int)StickersBLayer.UBR_U] = LibraryAlgCanvasBackLayer_UBR_U;
            LibraryData.AlgorithmLayersRects[(int)Faces.B][(int)StickersBLayer.UB_U] = LibraryAlgCanvasBackLayer_UB_U;
            LibraryData.AlgorithmLayersRects[(int)Faces.B][(int)StickersBLayer.UBL_U] = LibraryAlgCanvasBackLayer_UBL_U;

            LibraryData.AlgorithmLayersRects[(int)Faces.B][(int)StickersBLayer.UBR_R] = LibraryAlgCanvasBackLayer_UBR_R;
            LibraryData.AlgorithmLayersRects[(int)Faces.B][(int)StickersBLayer.UBR_B] = LibraryAlgCanvasBackLayer_UBR_B;
            LibraryData.AlgorithmLayersRects[(int)Faces.B][(int)StickersBLayer.UB_B] = LibraryAlgCanvasBackLayer_UB_B;
            LibraryData.AlgorithmLayersRects[(int)Faces.B][(int)StickersBLayer.UBL_B] = LibraryAlgCanvasBackLayer_UBL_B;
            LibraryData.AlgorithmLayersRects[(int)Faces.B][(int)StickersBLayer.UBL_L] = LibraryAlgCanvasBackLayer_UBL_L;

            LibraryData.AlgorithmLayersRects[(int)Faces.B][(int)StickersBLayer.RB_R] = LibraryAlgCanvasBackLayer_RB_R;
            LibraryData.AlgorithmLayersRects[(int)Faces.B][(int)StickersBLayer.RB_B] = LibraryAlgCanvasBackLayer_RB_B;
            LibraryData.AlgorithmLayersRects[(int)Faces.B][(int)StickersBLayer.B] = LibraryAlgCanvasBackLayer_B_B;
            LibraryData.AlgorithmLayersRects[(int)Faces.B][(int)StickersBLayer.BL_B] = LibraryAlgCanvasBackLayer_BL_B;
            LibraryData.AlgorithmLayersRects[(int)Faces.B][(int)StickersBLayer.BL_L] = LibraryAlgCanvasBackLayer_BL_L;

            LibraryData.AlgorithmLayersRects[(int)Faces.B][(int)StickersBLayer.DBR_R] = LibraryAlgCanvasBackLayer_DBR_R;
            LibraryData.AlgorithmLayersRects[(int)Faces.B][(int)StickersBLayer.DBR_B] = LibraryAlgCanvasBackLayer_DBR_B;
            LibraryData.AlgorithmLayersRects[(int)Faces.B][(int)StickersBLayer.DB_B] = LibraryAlgCanvasBackLayer_DB_B;
            LibraryData.AlgorithmLayersRects[(int)Faces.B][(int)StickersBLayer.DBL_B] = LibraryAlgCanvasBackLayer_DBL_B;
            LibraryData.AlgorithmLayersRects[(int)Faces.B][(int)StickersBLayer.DBL_L] = LibraryAlgCanvasBackLayer_DBL_L;

            LibraryData.AlgorithmLayersRects[(int)Faces.B][(int)StickersBLayer.DBR_D] = LibraryAlgCanvasBackLayer_DBR_D;
            LibraryData.AlgorithmLayersRects[(int)Faces.B][(int)StickersBLayer.DB_D] = LibraryAlgCanvasBackLayer_DB_D;
            LibraryData.AlgorithmLayersRects[(int)Faces.B][(int)StickersBLayer.DBL_D] = LibraryAlgCanvasBackLayer_DBL_D;

            // Layer R
            LibraryData.AlgorithmLayersRects[(int)Faces.R][(int)StickersRLayer.UFR_U] = LibraryAlgCanvasRightLayer_UFR_U;
            LibraryData.AlgorithmLayersRects[(int)Faces.R][(int)StickersRLayer.UR_U] = LibraryAlgCanvasRightLayer_UR_U;
            LibraryData.AlgorithmLayersRects[(int)Faces.R][(int)StickersRLayer.UBR_U] = LibraryAlgCanvasRightLayer_UBR_U;

            LibraryData.AlgorithmLayersRects[(int)Faces.R][(int)StickersRLayer.UFR_F] = LibraryAlgCanvasRightLayer_UFR_F;
            LibraryData.AlgorithmLayersRects[(int)Faces.R][(int)StickersRLayer.UFR_R] = LibraryAlgCanvasRightLayer_UFR_R;
            LibraryData.AlgorithmLayersRects[(int)Faces.R][(int)StickersRLayer.UR_R] = LibraryAlgCanvasRightLayer_UR_R;
            LibraryData.AlgorithmLayersRects[(int)Faces.R][(int)StickersRLayer.UBR_R] = LibraryAlgCanvasRightLayer_UBR_R;
            LibraryData.AlgorithmLayersRects[(int)Faces.R][(int)StickersRLayer.UBR_B] = LibraryAlgCanvasRightLayer_UBR_B;

            LibraryData.AlgorithmLayersRects[(int)Faces.R][(int)StickersRLayer.FR_F] = LibraryAlgCanvasRightLayer_FR_F;
            LibraryData.AlgorithmLayersRects[(int)Faces.R][(int)StickersRLayer.FR_R] = LibraryAlgCanvasRightLayer_FR_R;
            LibraryData.AlgorithmLayersRects[(int)Faces.R][(int)StickersRLayer.R] = LibraryAlgCanvasRightLayer_R_R;
            LibraryData.AlgorithmLayersRects[(int)Faces.R][(int)StickersRLayer.RB_R] = LibraryAlgCanvasRightLayer_RB_R;
            LibraryData.AlgorithmLayersRects[(int)Faces.R][(int)StickersRLayer.RB_B] = LibraryAlgCanvasRightLayer_RB_B;

            LibraryData.AlgorithmLayersRects[(int)Faces.R][(int)StickersRLayer.DFR_F] = LibraryAlgCanvasRightLayer_DFR_F;
            LibraryData.AlgorithmLayersRects[(int)Faces.R][(int)StickersRLayer.DFR_R] = LibraryAlgCanvasRightLayer_DFR_R;
            LibraryData.AlgorithmLayersRects[(int)Faces.R][(int)StickersRLayer.DR_R] = LibraryAlgCanvasRightLayer_DR_R;
            LibraryData.AlgorithmLayersRects[(int)Faces.R][(int)StickersRLayer.DBR_R] = LibraryAlgCanvasRightLayer_DBR_R;
            LibraryData.AlgorithmLayersRects[(int)Faces.R][(int)StickersRLayer.DBR_B] = LibraryAlgCanvasRightLayer_DBR_B;

            LibraryData.AlgorithmLayersRects[(int)Faces.R][(int)StickersRLayer.DFR_D] = LibraryAlgCanvasRightLayer_DFR_D;
            LibraryData.AlgorithmLayersRects[(int)Faces.R][(int)StickersRLayer.DR_D] = LibraryAlgCanvasRightLayer_DR_D;
            LibraryData.AlgorithmLayersRects[(int)Faces.R][(int)StickersRLayer.DBR_D] = LibraryAlgCanvasRightLayer_DBR_D;

            // Layer L
            LibraryData.AlgorithmLayersRects[(int)Faces.L][(int)StickersLLayer.UBL_U] = LibraryAlgCanvasLeftLayer_UBL_U;
            LibraryData.AlgorithmLayersRects[(int)Faces.L][(int)StickersLLayer.UL_U] = LibraryAlgCanvasLeftLayer_UL_U;
            LibraryData.AlgorithmLayersRects[(int)Faces.L][(int)StickersLLayer.UFL_U] = LibraryAlgCanvasLeftLayer_UFL_U;

            LibraryData.AlgorithmLayersRects[(int)Faces.L][(int)StickersLLayer.UBL_B] = LibraryAlgCanvasLeftLayer_UBL_B;
            LibraryData.AlgorithmLayersRects[(int)Faces.L][(int)StickersLLayer.UBL_L] = LibraryAlgCanvasLeftLayer_UBL_L;
            LibraryData.AlgorithmLayersRects[(int)Faces.L][(int)StickersLLayer.UL_L] = LibraryAlgCanvasLeftLayer_UL_L;
            LibraryData.AlgorithmLayersRects[(int)Faces.L][(int)StickersLLayer.UFL_L] = LibraryAlgCanvasLeftLayer_UFL_L;
            LibraryData.AlgorithmLayersRects[(int)Faces.L][(int)StickersLLayer.UFL_F] = LibraryAlgCanvasLeftLayer_UFL_F;

            LibraryData.AlgorithmLayersRects[(int)Faces.L][(int)StickersLLayer.BL_B] = LibraryAlgCanvasLeftLayer_BL_B;
            LibraryData.AlgorithmLayersRects[(int)Faces.L][(int)StickersLLayer.BL_L] = LibraryAlgCanvasLeftLayer_BL_L;
            LibraryData.AlgorithmLayersRects[(int)Faces.L][(int)StickersLLayer.L] = LibraryAlgCanvasLeftLayer_L_L;
            LibraryData.AlgorithmLayersRects[(int)Faces.L][(int)StickersLLayer.LF_L] = LibraryAlgCanvasLeftLayer_LF_L;
            LibraryData.AlgorithmLayersRects[(int)Faces.L][(int)StickersLLayer.LF_F] = LibraryAlgCanvasLeftLayer_LF_F;

            LibraryData.AlgorithmLayersRects[(int)Faces.L][(int)StickersLLayer.DBL_B] = LibraryAlgCanvasLeftLayer_DBL_B;
            LibraryData.AlgorithmLayersRects[(int)Faces.L][(int)StickersLLayer.DBL_L] = LibraryAlgCanvasLeftLayer_DBL_L;
            LibraryData.AlgorithmLayersRects[(int)Faces.L][(int)StickersLLayer.DL_L] = LibraryAlgCanvasLeftLayer_DL_L;
            LibraryData.AlgorithmLayersRects[(int)Faces.L][(int)StickersLLayer.DFL_L] = LibraryAlgCanvasLeftLayer_DFL_L;
            LibraryData.AlgorithmLayersRects[(int)Faces.L][(int)StickersLLayer.DFL_F] = LibraryAlgCanvasLeftLayer_DFL_F;

            LibraryData.AlgorithmLayersRects[(int)Faces.L][(int)StickersLLayer.DBL_D] = LibraryAlgCanvasLeftLayer_DBL_D;
            LibraryData.AlgorithmLayersRects[(int)Faces.L][(int)StickersLLayer.DL_D] = LibraryAlgCanvasLeftLayer_DL_D;
            LibraryData.AlgorithmLayersRects[(int)Faces.L][(int)StickersLLayer.DFL_D] = LibraryAlgCanvasLeftLayer_DFL_D;
        }

        /// <summary>
        /// Returns a polyline with an arrow form
        /// </summary>
        /// <param name="X1">X1</param>
        /// <param name="Y1">Y1</param>
        /// <param name="X2">X2</param>
        /// <param name="Y2">Y2</param>
        /// <param name="Thickness">Arrow thickness</param>
        /// <returns>Polyline arrow</returns>
        public Polyline GetArrowBetweenPoints(double X1, double Y1, double X2, double Y2, double Thickness)
        {
            //Point P1 = new Point(X1, Y1);
            //Point P2 = new Point(X2, Y2);

            double X = X2 - X1,
                   Y = Y2 - Y1;
            double length = Math.Sqrt(X * X + Y * Y);

            if (Thickness <= 0d || length < Thickness) return null;

            double c1 = 1.25 * Thickness / length,
                   c2 = 1d - c1;

            double c3 = 4d * Thickness / length,
                   c4 = 1d - c3;

            Point P3 = new Point(c1 * X1 + c2 * X2, c1 * Y1 + c2 * Y2);
            Point P6 = new Point(c3 * X2 + c4 * X1, c3 * Y2 + c4 * Y1);
            Point P7 = new Point(c3 * X1 + c4 * X2, c3 * Y1 + c4 * Y2);

            double tx = -c1 * Y;
            double ty = c1 * X;

            Point P4 = new Point(P7.X + tx, P7.Y + ty);
            Point P5 = new Point(P7.X - tx, P7.Y - ty);

            return new Polyline
            { // Create a polyline
                Stroke = AMSettings.ArrowsBrush,
                StrokeThickness = Thickness,
                Points = new PointCollection
                { // Collection of points for the polyline
                    P6, P7, P4, P3, P5, P7
                },
                StrokeStartLineCap = PenLineCap.Round,
            };
        }

        /// <summary>
        /// Update initial position library cube skin colors
        /// </summary>
        public void UpdateLibraryInitViewCube2D()
        {
            switch (LibraryInitialPositionTabControl.SelectedIndex)
            {
                case 1: // Full 2D cube skin
                    if (LibraryData.InitScrambleCube != null && LibraryData.InitPositionStickerButtons != null)
                    {
                        for (int Pos = 0; Pos < LibraryData.InitPositionStickerButtons.Length; Pos++)
                        {
                            StickerPositions St = LibraryData.InitScrambleCube.Cube.GetStickerSolvedPosition((StickerPositions)Pos);
                            LibraryData.InitPositionStickerButtons[Pos].BorderBrush = AMSettings.BaseBrush;
                            if (LibraryData.InitScrambleCube.NeutralStickers[(int)St])
                                LibraryData.InitPositionStickerButtons[Pos].Background = AMSettings.NeutralBrush;
                            else
                                LibraryData.InitPositionStickerButtons[Pos].Background =
                                    AMSettings.GetBrush(LibraryData.InitScrambleCube.GetColor((StickerPositions)Pos));
                        }
                        LibraryInitPos2DGrid.Background = AMSettings.BackgroundBrush;
                    }
                    break;

                case 2: // U layer
                    LibraryUpLayerCanvas.Children.Clear();

                    // Draw background
                    Rectangle LibraryInitCanvasUpLayerBackground = new Rectangle
                    {
                        Width = 1000,
                        Height = 1000,
                        Fill = AMSettings.BackgroundBrush
                    };
                    LibraryUpLayerCanvas.Children.Add(LibraryInitCanvasUpLayerBackground);
                    Canvas.SetLeft(LibraryInitCanvasUpLayerBackground, 0);
                    Canvas.SetTop(LibraryInitCanvasUpLayerBackground, 0);
                    Canvas.SetZIndex(LibraryInitCanvasUpLayerBackground, -2);

                    // Draw stickers
                    foreach (StickersULayer sl in Enum.GetValues(typeof(StickersULayer)))
                    {
                        LibraryUpLayerCanvas.Children.Add(LibraryData.InitPosLayersRects[(int)Layers.U][(int)sl]);
                        Canvas.SetZIndex(LibraryData.InitPosLayersRects[(int)Layers.U][(int)sl], -1);
                        StickerPositions St = LibraryData.InitScrambleCube.Cube.GetStickerSolvedPosition((StickerPositions)sl);
                        LibraryData.InitPosLayersRects[(int)Layers.U][(int)sl].Stroke = AMSettings.BaseBrush;
                        if (LibraryData.InitScrambleCube.NeutralStickers[(int)St])
                            LibraryData.InitPosLayersRects[(int)Layers.U][(int)sl].Fill = AMSettings.NeutralBrush;
                        else
                            LibraryData.InitPosLayersRects[(int)Layers.U][(int)sl].Fill =
                                AMSettings.GetBrush(LibraryData.InitScrambleCube.GetColor((StickerPositions)sl));
                    }

                    // Draw arrows
                    if (LibraryInitPosShowArrowsCheckBox.IsChecked ?? true)
                        foreach (StickersUFace sf in Enum.GetValues(typeof(StickersUFace)))
                        {
                            Rectangle R1 = LibraryData.InitPosLayersRects[(int)Layers.U][(int)sf];
                            if (R1.Fill == AMSettings.NeutralBrush) continue;
                            Pieces Piece = StickerData.GetStickerPiece((StickerPositions)sf);
                            foreach (StickersUFace sf2 in Enum.GetValues(typeof(StickersUFace)))
                            {
                                Pieces Piece2 = StickerData.GetStickerPiece((StickerPositions)sf2);
                                if (Piece == Piece2) continue;
                                if (Piece2 == LibraryData.InitScrambleCube.Cube.GetPieceSolvedPosition(Piece))
                                {
                                    Rectangle R2 = LibraryData.InitPosLayersRects[(int)Layers.U][(int)sf2];
                                    double X1 = Canvas.GetLeft(R1) + R1.Width / 2d,
                                           X2 = Canvas.GetLeft(R2) + R2.Width / 2d,
                                           Y1 = Canvas.GetTop(R1) + R1.Height / 2d,
                                           Y2 = Canvas.GetTop(R2) + R2.Height / 2d;
                                    LibraryUpLayerCanvas.Children.Add(GetArrowBetweenPoints(X1, Y1, X2, Y2, 12d));
                                    break;
                                }
                            }
                        }
                    break;

                case 3: // D layer
                    LibraryDownLayerCanvas.Children.Clear();

                    // Draw background
                    Rectangle LibraryInitCanvasDownLayerBackground = new Rectangle
                    {
                        Width = 1000,
                        Height = 1000,
                        Fill = AMSettings.BackgroundBrush
                    };
                    LibraryDownLayerCanvas.Children.Add(LibraryInitCanvasDownLayerBackground);
                    Canvas.SetLeft(LibraryInitCanvasDownLayerBackground, 0);
                    Canvas.SetTop(LibraryInitCanvasDownLayerBackground, 0);
                    Canvas.SetZIndex(LibraryInitCanvasDownLayerBackground, -2);

                    // Draw stickers
                    foreach (StickersDLayer sl in Enum.GetValues(typeof(StickersDLayer)))
                    {
                        LibraryDownLayerCanvas.Children.Add(LibraryData.InitPosLayersRects[(int)Layers.D][(int)sl]);
                        LibraryData.InitPosLayersRects[(int)Layers.D][(int)sl].Stroke = AMSettings.BaseBrush;
                        Canvas.SetZIndex(LibraryData.InitPosLayersRects[(int)Layers.D][(int)sl], -1);
                        StickerPositions St = LibraryData.InitScrambleCube.Cube.GetStickerSolvedPosition((StickerPositions)sl);
                        if (LibraryData.InitScrambleCube.NeutralStickers[(int)St])
                            LibraryData.InitPosLayersRects[(int)Layers.D][(int)sl].Fill = AMSettings.NeutralBrush;
                        else
                            LibraryData.InitPosLayersRects[(int)Layers.D][(int)sl].Fill =
                                AMSettings.GetBrush(LibraryData.InitScrambleCube.GetColor((StickerPositions)sl));
                    }

                    // Draw arrows
                    if (LibraryInitPosShowArrowsCheckBox.IsChecked ?? true)
                        foreach (StickersDFace sf in Enum.GetValues(typeof(StickersDFace)))
                        {
                            Rectangle R1 = LibraryData.InitPosLayersRects[(int)Layers.D][(int)sf];
                            if (R1.Fill == AMSettings.NeutralBrush) continue;
                            Pieces Piece = StickerData.GetStickerPiece((StickerPositions)sf);
                            foreach (StickersDFace sf2 in Enum.GetValues(typeof(StickersDFace)))
                            {
                                Pieces Piece2 = StickerData.GetStickerPiece((StickerPositions)sf2);
                                if (Piece == Piece2) continue;
                                if (Piece2 == LibraryData.InitScrambleCube.Cube.GetPieceSolvedPosition(Piece))
                                {
                                    Rectangle R2 = LibraryData.InitPosLayersRects[(int)Layers.D][(int)sf2];
                                    double X1 = Canvas.GetLeft(R1) + R1.Width / 2d,
                                           X2 = Canvas.GetLeft(R2) + R2.Width / 2d,
                                           Y1 = Canvas.GetTop(R1) + R1.Height / 2d,
                                           Y2 = Canvas.GetTop(R2) + R2.Height / 2d;
                                    LibraryDownLayerCanvas.Children.Add(GetArrowBetweenPoints(X1, Y1, X2, Y2, 12d));
                                    break;
                                }
                            }
                        }
                    break;

                case 4: // F layer
                    LibraryFrontLayerCanvas.Children.Clear();

                    // Draw background
                    Rectangle LibraryInitCanvasFrontLayerBackground = new Rectangle
                    {
                        Width = 1000,
                        Height = 1000,
                        Fill = AMSettings.BackgroundBrush
                    };
                    LibraryFrontLayerCanvas.Children.Add(LibraryInitCanvasFrontLayerBackground);
                    Canvas.SetLeft(LibraryInitCanvasFrontLayerBackground, 0);
                    Canvas.SetTop(LibraryInitCanvasFrontLayerBackground, 0);
                    Canvas.SetZIndex(LibraryInitCanvasFrontLayerBackground, -2);

                    // Draw stickers
                    foreach (StickersFLayer sl in Enum.GetValues(typeof(StickersFLayer)))
                    {
                        LibraryFrontLayerCanvas.Children.Add(LibraryData.InitPosLayersRects[(int)Layers.F][(int)sl]);
                        LibraryData.InitPosLayersRects[(int)Layers.F][(int)sl].Stroke = AMSettings.BaseBrush;
                        Canvas.SetZIndex(LibraryData.InitPosLayersRects[(int)Layers.F][(int)sl], -1);
                        StickerPositions St = LibraryData.InitScrambleCube.Cube.GetStickerSolvedPosition((StickerPositions)sl);
                        if (LibraryData.InitScrambleCube.NeutralStickers[(int)St])
                            LibraryData.InitPosLayersRects[(int)Layers.F][(int)sl].Fill = AMSettings.NeutralBrush;
                        else
                            LibraryData.InitPosLayersRects[(int)Layers.F][(int)sl].Fill =
                                AMSettings.GetBrush(LibraryData.InitScrambleCube.GetColor((StickerPositions)sl));
                    }

                    // Draw arrows
                    if (LibraryInitPosShowArrowsCheckBox.IsChecked ?? true)
                        foreach (StickersFFace sf in Enum.GetValues(typeof(StickersFFace)))
                        {
                            Rectangle R1 = LibraryData.InitPosLayersRects[(int)Layers.F][(int)sf];
                            if (R1.Fill == AMSettings.NeutralBrush) continue;
                            Pieces Piece = StickerData.GetStickerPiece((StickerPositions)sf);
                            foreach (StickersFFace sf2 in Enum.GetValues(typeof(StickersFFace)))
                            {
                                Pieces Piece2 = StickerData.GetStickerPiece((StickerPositions)sf2);
                                if (Piece == Piece2) continue;
                                if (Piece2 == LibraryData.InitScrambleCube.Cube.GetPieceSolvedPosition(Piece))
                                {
                                    Rectangle R2 = LibraryData.InitPosLayersRects[(int)Layers.F][(int)sf2];
                                    double X1 = Canvas.GetLeft(R1) + R1.Width / 2d,
                                           X2 = Canvas.GetLeft(R2) + R2.Width / 2d,
                                           Y1 = Canvas.GetTop(R1) + R1.Height / 2d,
                                           Y2 = Canvas.GetTop(R2) + R2.Height / 2d;
                                    LibraryFrontLayerCanvas.Children.Add(GetArrowBetweenPoints(X1, Y1, X2, Y2, 12d));
                                    break;
                                }
                            }
                        }
                    break;

                case 5: // B layer
                    LibraryBackLayerCanvas.Children.Clear();

                    // Draw background
                    Rectangle LibraryInitCanvasBackLayerBackground = new Rectangle
                    {
                        Width = 1000,
                        Height = 1000,
                        Fill = AMSettings.BackgroundBrush
                    };
                    LibraryBackLayerCanvas.Children.Add(LibraryInitCanvasBackLayerBackground);
                    Canvas.SetLeft(LibraryInitCanvasBackLayerBackground, 0);
                    Canvas.SetTop(LibraryInitCanvasBackLayerBackground, 0);
                    Canvas.SetZIndex(LibraryInitCanvasBackLayerBackground, -2);

                    // Draw stickers
                    foreach (StickersBLayer sl in Enum.GetValues(typeof(StickersBLayer)))
                    {
                        LibraryBackLayerCanvas.Children.Add(LibraryData.InitPosLayersRects[(int)Layers.B][(int)sl]);
                        LibraryData.InitPosLayersRects[(int)Layers.B][(int)sl].Stroke = AMSettings.BaseBrush;
                        Canvas.SetZIndex(LibraryData.InitPosLayersRects[(int)Layers.B][(int)sl], -1);
                        StickerPositions St = LibraryData.InitScrambleCube.Cube.GetStickerSolvedPosition((StickerPositions)sl);
                        if (LibraryData.InitScrambleCube.NeutralStickers[(int)St])
                            LibraryData.InitPosLayersRects[(int)Layers.B][(int)sl].Fill = AMSettings.NeutralBrush;
                        else
                            LibraryData.InitPosLayersRects[(int)Layers.B][(int)sl].Fill =
                                AMSettings.GetBrush(LibraryData.InitScrambleCube.GetColor((StickerPositions)sl));
                    }

                    // Draw arrows
                    if (LibraryInitPosShowArrowsCheckBox.IsChecked ?? true)
                        foreach (StickersBFace sf in Enum.GetValues(typeof(StickersBFace)))
                        {
                            Rectangle R1 = LibraryData.InitPosLayersRects[(int)Layers.B][(int)sf];
                            if (R1.Fill == AMSettings.NeutralBrush) continue;
                            Pieces Piece = StickerData.GetStickerPiece((StickerPositions)sf);
                            foreach (StickersBFace sf2 in Enum.GetValues(typeof(StickersBFace)))
                            {
                                Pieces Piece2 = StickerData.GetStickerPiece((StickerPositions)sf2);
                                if (Piece == Piece2) continue;
                                if (Piece2 == LibraryData.InitScrambleCube.Cube.GetPieceSolvedPosition(Piece))
                                {
                                    Rectangle R2 = LibraryData.InitPosLayersRects[(int)Layers.B][(int)sf2];
                                    double X1 = Canvas.GetLeft(R1) + R1.Width / 2d,
                                           X2 = Canvas.GetLeft(R2) + R2.Width / 2d,
                                           Y1 = Canvas.GetTop(R1) + R1.Height / 2d,
                                           Y2 = Canvas.GetTop(R2) + R2.Height / 2d;
                                    LibraryBackLayerCanvas.Children.Add(GetArrowBetweenPoints(X1, Y1, X2, Y2, 12d));
                                    break;
                                }
                            }
                        }
                    break;

                case 6: // R layer
                    LibraryRightLayerCanvas.Children.Clear();

                    // Draw background
                    Rectangle LibraryInitCanvasRightLayerBackground = new Rectangle
                    {
                        Width = 1000,
                        Height = 1000,
                        Fill = AMSettings.BackgroundBrush
                    };
                    LibraryRightLayerCanvas.Children.Add(LibraryInitCanvasRightLayerBackground);
                    Canvas.SetLeft(LibraryInitCanvasRightLayerBackground, 0);
                    Canvas.SetTop(LibraryInitCanvasRightLayerBackground, 0);
                    Canvas.SetZIndex(LibraryInitCanvasRightLayerBackground, -2);

                    // Draw stickers
                    foreach (StickersRLayer sl in Enum.GetValues(typeof(StickersRLayer)))
                    {
                        LibraryRightLayerCanvas.Children.Add(LibraryData.InitPosLayersRects[(int)Layers.R][(int)sl]);
                        LibraryData.InitPosLayersRects[(int)Layers.R][(int)sl].Stroke = AMSettings.BaseBrush;
                        Canvas.SetZIndex(LibraryData.InitPosLayersRects[(int)Layers.R][(int)sl], -1);
                        StickerPositions St = LibraryData.InitScrambleCube.Cube.GetStickerSolvedPosition((StickerPositions)sl);
                        if (LibraryData.InitScrambleCube.NeutralStickers[(int)St])
                            LibraryData.InitPosLayersRects[(int)Layers.R][(int)sl].Fill = AMSettings.NeutralBrush;
                        else
                            LibraryData.InitPosLayersRects[(int)Layers.R][(int)sl].Fill =
                                AMSettings.GetBrush(LibraryData.InitScrambleCube.GetColor((StickerPositions)sl));
                    }

                    // Draw arrows
                    if (LibraryInitPosShowArrowsCheckBox.IsChecked ?? true)
                        foreach (StickersRFace sf in Enum.GetValues(typeof(StickersRFace)))
                        {
                            Rectangle R1 = LibraryData.InitPosLayersRects[(int)Layers.R][(int)sf];
                            if (R1.Fill == AMSettings.NeutralBrush) continue;
                            Pieces Piece = StickerData.GetStickerPiece((StickerPositions)sf);
                            foreach (StickersRFace sf2 in Enum.GetValues(typeof(StickersRFace)))
                            {
                                Pieces Piece2 = StickerData.GetStickerPiece((StickerPositions)sf2);
                                if (Piece == Piece2) continue;
                                if (Piece2 == LibraryData.InitScrambleCube.Cube.GetPieceSolvedPosition(Piece))
                                {
                                    Rectangle R2 = LibraryData.InitPosLayersRects[(int)Layers.R][(int)sf2];
                                    double X1 = Canvas.GetLeft(R1) + R1.Width / 2d,
                                           X2 = Canvas.GetLeft(R2) + R2.Width / 2d,
                                           Y1 = Canvas.GetTop(R1) + R1.Height / 2d,
                                           Y2 = Canvas.GetTop(R2) + R2.Height / 2d;
                                    LibraryRightLayerCanvas.Children.Add(GetArrowBetweenPoints(X1, Y1, X2, Y2, 12d));
                                    break;
                                }
                            }
                        }
                    break;

                case 7: // L layer
                    LibraryLeftLayerCanvas.Children.Clear();

                    // Draw background
                    Rectangle LibraryInitCanvasLeftLayerBackground = new Rectangle
                    {
                        Width = 1000,
                        Height = 1000,
                        Fill = AMSettings.BackgroundBrush
                    };
                    LibraryLeftLayerCanvas.Children.Add(LibraryInitCanvasLeftLayerBackground);
                    Canvas.SetLeft(LibraryInitCanvasLeftLayerBackground, 0);
                    Canvas.SetTop(LibraryInitCanvasLeftLayerBackground, 0);
                    Canvas.SetZIndex(LibraryInitCanvasLeftLayerBackground, -2);

                    // Draw stickers
                    foreach (StickersLLayer sl in Enum.GetValues(typeof(StickersLLayer)))
                    {
                        LibraryLeftLayerCanvas.Children.Add(LibraryData.InitPosLayersRects[(int)Layers.L][(int)sl]);
                        LibraryData.InitPosLayersRects[(int)Layers.L][(int)sl].Stroke = AMSettings.BaseBrush;
                        Canvas.SetZIndex(LibraryData.InitPosLayersRects[(int)Layers.L][(int)sl], -1);
                        StickerPositions St = LibraryData.InitScrambleCube.Cube.GetStickerSolvedPosition((StickerPositions)sl);
                        if (LibraryData.InitScrambleCube.NeutralStickers[(int)St])
                            LibraryData.InitPosLayersRects[(int)Layers.L][(int)sl].Fill = AMSettings.NeutralBrush;
                        else
                            LibraryData.InitPosLayersRects[(int)Layers.L][(int)sl].Fill =
                                AMSettings.GetBrush(LibraryData.InitScrambleCube.GetColor((StickerPositions)sl));
                    }

                    // Draw arrows
                    if (LibraryInitPosShowArrowsCheckBox.IsChecked ?? true)
                        foreach (StickersLFace sf in Enum.GetValues(typeof(StickersLFace)))
                        {
                            Rectangle R1 = LibraryData.InitPosLayersRects[(int)Layers.L][(int)sf];
                            if (R1.Fill == AMSettings.NeutralBrush) continue;
                            Pieces Piece = StickerData.GetStickerPiece((StickerPositions)sf);
                            foreach (StickersLFace sf2 in Enum.GetValues(typeof(StickersLFace)))
                            {
                                Pieces Piece2 = StickerData.GetStickerPiece((StickerPositions)sf2);
                                if (Piece == Piece2) continue;
                                if (Piece2 == LibraryData.InitScrambleCube.Cube.GetPieceSolvedPosition(Piece))
                                {
                                    Rectangle R2 = LibraryData.InitPosLayersRects[(int)Layers.L][(int)sf2];
                                    double X1 = Canvas.GetLeft(R1) + R1.Width / 2d,
                                           X2 = Canvas.GetLeft(R2) + R2.Width / 2d,
                                           Y1 = Canvas.GetTop(R1) + R1.Height / 2d,
                                           Y2 = Canvas.GetTop(R2) + R2.Height / 2d;
                                    LibraryLeftLayerCanvas.Children.Add(GetArrowBetweenPoints(X1, Y1, X2, Y2, 12d));
                                    break;
                                }
                            }
                        }
                    break;
            }
        }

        /// <summary>
        /// Update algorithm library cube skin colors
        /// </summary>
        public void UpdateLibraryAlgViewCube2D()
        {
            switch (LibraryAlgorithmTabControl.SelectedIndex)
            {
                case 1: // Full 2D cube skin
                    if (LibraryData.AlgorithmCube != null && LibraryData.AlgorithmStickerButtons != null)
                    {
                        for (int Pos = 0; Pos < LibraryData.AlgorithmStickerButtons.Length; Pos++)
                        {
                            StickerPositions St = LibraryData.AlgorithmCube.Cube.GetStickerSolvedPosition((StickerPositions)Pos);
                            LibraryData.AlgorithmStickerButtons[Pos].BorderBrush = AMSettings.BaseBrush;
                            if (LibraryData.AlgorithmCube.NeutralStickers[(int)St])
                                LibraryData.AlgorithmStickerButtons[Pos].Background = AMSettings.NeutralBrush;
                            else
                                LibraryData.AlgorithmStickerButtons[Pos].Background =
                                    AMSettings.GetBrush(LibraryData.AlgorithmCube.GetColor((StickerPositions)Pos));
                        }
                        LibraryAlgorithm2DGrid.Background = AMSettings.BackgroundBrush;
                    }
                    break;

                case 2: // U layer
                    LibraryAlgCanvasUpLayerBackground.Fill = AMSettings.BackgroundBrush;
                    foreach (StickersULayer sl in Enum.GetValues(typeof(StickersULayer)))
                    {
                        StickerPositions St = LibraryData.AlgorithmCube.Cube.GetStickerSolvedPosition((StickerPositions)sl);
                        LibraryData.AlgorithmLayersRects[(int)Layers.U][(int)sl].Stroke = AMSettings.BaseBrush;
                        if (LibraryData.AlgorithmCube.NeutralStickers[(int)St])
                            LibraryData.AlgorithmLayersRects[(int)Layers.U][(int)sl].Fill = AMSettings.NeutralBrush;
                        else
                            LibraryData.AlgorithmLayersRects[(int)Layers.U][(int)sl].Fill =
                                AMSettings.GetBrush(LibraryData.AlgorithmCube.GetColor((StickerPositions)sl));
                    }

                    // TODO: Draw arrows
                    break;

                case 3: // D layer
                    LibraryAlgCanvasDownLayerBackground.Fill = AMSettings.BackgroundBrush;
                    foreach (StickersDLayer sl in Enum.GetValues(typeof(StickersDLayer)))
                    {
                        StickerPositions St = LibraryData.AlgorithmCube.Cube.GetStickerSolvedPosition((StickerPositions)sl);
                        LibraryData.AlgorithmLayersRects[(int)Layers.D][(int)sl].Stroke = AMSettings.BaseBrush;
                        if (LibraryData.AlgorithmCube.NeutralStickers[(int)St])
                            LibraryData.AlgorithmLayersRects[(int)Layers.D][(int)sl].Fill = AMSettings.NeutralBrush;
                        else
                            LibraryData.AlgorithmLayersRects[(int)Layers.D][(int)sl].Fill =
                                AMSettings.GetBrush(LibraryData.AlgorithmCube.GetColor((StickerPositions)sl));
                    }
                    break;

                case 4: // F layer
                    LibraryAlgCanvasFrontLayerBackground.Fill = AMSettings.BackgroundBrush;
                    foreach (StickersFLayer sl in Enum.GetValues(typeof(StickersFLayer)))
                    {
                        StickerPositions St = LibraryData.AlgorithmCube.Cube.GetStickerSolvedPosition((StickerPositions)sl);
                        LibraryData.AlgorithmLayersRects[(int)Layers.F][(int)sl].Stroke = AMSettings.BaseBrush;
                        if (LibraryData.AlgorithmCube.NeutralStickers[(int)St])
                            LibraryData.AlgorithmLayersRects[(int)Layers.F][(int)sl].Fill = AMSettings.NeutralBrush;
                        else
                            LibraryData.AlgorithmLayersRects[(int)Layers.F][(int)sl].Fill =
                                AMSettings.GetBrush(LibraryData.AlgorithmCube.GetColor((StickerPositions)sl));
                    }
                    break;

                case 5: // B layer
                    LibraryAlgCanvasBackLayerBackground.Fill = AMSettings.BackgroundBrush;
                    foreach (StickersBLayer sl in Enum.GetValues(typeof(StickersBLayer)))
                    {
                        StickerPositions St = LibraryData.AlgorithmCube.Cube.GetStickerSolvedPosition((StickerPositions)sl);
                        LibraryData.AlgorithmLayersRects[(int)Layers.B][(int)sl].Stroke = AMSettings.BaseBrush;
                        if (LibraryData.AlgorithmCube.NeutralStickers[(int)St])
                            LibraryData.AlgorithmLayersRects[(int)Layers.B][(int)sl].Fill = AMSettings.NeutralBrush;
                        else
                            LibraryData.AlgorithmLayersRects[(int)Layers.B][(int)sl].Fill =
                                AMSettings.GetBrush(LibraryData.AlgorithmCube.GetColor((StickerPositions)sl));
                    }
                    break;

                case 6: // R layer
                    LibraryAlgCanvasRightLayerBackground.Fill = AMSettings.BackgroundBrush;
                    foreach (StickersRLayer sl in Enum.GetValues(typeof(StickersRLayer)))
                    {
                        StickerPositions St = LibraryData.AlgorithmCube.Cube.GetStickerSolvedPosition((StickerPositions)sl);
                        LibraryData.AlgorithmLayersRects[(int)Layers.R][(int)sl].Stroke = AMSettings.BaseBrush;
                        if (LibraryData.AlgorithmCube.NeutralStickers[(int)St])
                            LibraryData.AlgorithmLayersRects[(int)Layers.R][(int)sl].Fill = AMSettings.NeutralBrush;
                        else
                            LibraryData.AlgorithmLayersRects[(int)Layers.R][(int)sl].Fill =
                                AMSettings.GetBrush(LibraryData.AlgorithmCube.GetColor((StickerPositions)sl));
                    }
                    break;

                case 7: // L layer
                    LibraryAlgCanvasLeftLayerBackground.Fill = AMSettings.BackgroundBrush;
                    foreach (StickersLLayer sl in Enum.GetValues(typeof(StickersLLayer)))
                    {
                        StickerPositions St = LibraryData.AlgorithmCube.Cube.GetStickerSolvedPosition((StickerPositions)sl);
                        LibraryData.AlgorithmLayersRects[(int)Layers.L][(int)sl].Stroke = AMSettings.BaseBrush;
                        if (LibraryData.AlgorithmCube.NeutralStickers[(int)St])
                            LibraryData.AlgorithmLayersRects[(int)Layers.L][(int)sl].Fill = AMSettings.NeutralBrush;
                        else
                            LibraryData.AlgorithmLayersRects[(int)Layers.L][(int)sl].Fill =
                                AMSettings.GetBrush(LibraryData.AlgorithmCube.GetColor((StickerPositions)sl));
                    }
                    break;
            }
        }

        /// <summary>
        /// Updates the library views
        /// </summary>
        public void UpdateLibraryViews()
        {
            LibraryData.InitScrambleCube.Reset();
            LibraryData.InitScrambleCube.ApplyScramble(LibraryData.InitScramble);
            if (LibraryInitialPositionTabControl.SelectedIndex == 0)
            {
                LibraryData.InitScrambleCube.UpdateBitmap((int)LibraryInitImage3D.ActualWidth,
                                                          (int)LibraryInitImage3D.ActualHeight);
                LibraryInitImage3D.Source = LibraryData.InitScrambleCube.renderBMP;
            }
            LibraryInitialpositionLabelText.Text = LibraryInitPosLabelToolTipBody.Text = LibraryData.InitScramble.ToString();
            LibraryAlgorithmBaseLabelText.Text = LibraryAlgorithmBaseLabelToolTipBody.Text = LibraryData.Algorithm.ToString();

            UpdateLibraryAlgorithm();
            LibraryData.AlgorithmCube.Reset();
            LibraryData.AlgorithmCube.ApplyScramble(LibraryData.InitScramble);
            LibraryData.AlgorithmCube.ApplyScramble(LibraryData.Algorithm.SubScramble(0, LibraryData.AlgorithmPosition));
            if (LibraryAlgorithmTabControl.SelectedIndex == 0)
            {
                LibraryData.AlgorithmCube.UpdateBitmap((int)LibraryAlgImage3D.ActualWidth,
                                                       (int)LibraryAlgImage3D.ActualHeight);
                LibraryAlgImage3D.Source = LibraryData.AlgorithmCube.renderBMP;
            }
            UpdateLibraryInitViewCube2D();
            UpdateLibraryAlgViewCube2D();
        }

        /// <summary>
        /// Reads the library directory and updates the library tree
        /// </summary>
        public void UpdateLibraryTree()
        { // *.sml files are stored in <data dir>\libs\xxx.sml [xxx represents the library file name]
            LibraryTreeView.Items.Clear();

            if (!Directory.Exists(AMSettings.LibsFolder)) return;

            DirectoryInfo diLibrary = new DirectoryInfo(AMSettings.LibsFolder);

            FileInfo[] smlFiles = diLibrary.GetFiles("*" + AMSettings.LibsExt);

            foreach (FileInfo fi in smlFiles)
            {
                try { BuildTree(LibraryTreeView, XDocument.Load(fi.FullName)); }
                catch (Exception ex)
                {
                    AMSettings.Log = string.Format("Error loading library '{0}':", fi.FullName) + ex.Message;
                }
            }
        }

        /// <summary>
        /// Build TreeView from XDocument
        /// </summary>
        /// <param name="treeView">TreeView</param>
        /// <param name="xdoc">XDocument</param>
        private void BuildTree(TreeView treeView, XDocument xdoc)
        {
            LibraryRootData LibData = new LibraryRootData
            {
                Name = xdoc.Root.Attributes().First(s => s.Name == "name").Value,
                Description = xdoc.Root.Attributes().First(s => s.Name == "description").Value,
                IsModified = false
            };

            TreeViewItem treeNode = new TreeViewItem
            {
                Header = GetLibraryStackPanel(LibData.Name),
                Tag = LibData,
                IsExpanded = false
            };

            treeView.Items.Add(treeNode);
            BuildNodes(treeNode, xdoc.Root);
        }

        /// <summary>
        /// Build tree view nodes
        /// </summary>
        /// <param name="treeNode">Tree view item nodes</param>
        /// <param name="element">Root XDocument element</param>
        private void BuildNodes(TreeViewItem treeNode, XElement element)
        {
            foreach (XNode child in element.Nodes())
            {
                if (child.NodeType == XmlNodeType.Element)
                {
                    XElement childElement = child as XElement;

                    if (childElement.Name.LocalName == "Folder")
                    {
                        LibraryFolderData FolderData = new LibraryFolderData
                        {
                            Name = childElement.Attributes().First(s => s.Name == "name").Value,
                            Description = childElement.Attributes().First(s => s.Name == "description").Value
                        };

                        TreeViewItem childFolderTreeNode = new TreeViewItem
                        {
                            Header = GetFolderStackPanel(FolderData.Name),
                            IsExpanded = false,
                            Tag = FolderData
                        };

                        treeNode.Items.Add(childFolderTreeNode);
                        BuildNodes(childFolderTreeNode, childElement);
                    }
                    if (childElement.Name.LocalName == "Algorithm")
                    {
                        LibraryAlgorithmData AlgorithmData = new LibraryAlgorithmData
                        {
                            Name = childElement.Attributes().First(s => s.Name == "name").Value,
                            Description = childElement.Attributes().First(s => s.Name == "description").Value,
                            InitScramble = childElement.Attributes().First(s => s.Name == "initial_scramble").Value,
                            Algorithm = childElement.Attributes().First(s => s.Name == "value").Value,
                            NeutralMask = childElement.Attributes().First(s => s.Name == "mask").Value,
                            StartView = childElement.Attributes().First(s => s.Name == "initial_view").Value[0],
                            EndView = childElement.Attributes().First(s => s.Name == "final_view").Value[0],
                            S3DViewAlpha = double.Parse(childElement.Attributes().First(s => s.Name == "initial_alpha3d").Value),
                            S3DViewBeta = double.Parse(childElement.Attributes().First(s => s.Name == "initial_beta3d").Value),
                            E3DViewAlpha = double.Parse(childElement.Attributes().First(s => s.Name == "final_alpha3d").Value),
                            E3DViewBeta = double.Parse(childElement.Attributes().First(s => s.Name == "final_beta3d").Value),
                            DrawArrows = childElement.Attributes().First(s => s.Name == "draw_arrows").Value == "Yes",
                        };

                        TreeViewItem childAlgorithmTreeNode = new TreeViewItem
                        {
                            Header = GetAlgorithmStackPanel(AlgorithmData.Name),
                            IsExpanded = false,
                            Tag = AlgorithmData
                        };

                        treeNode.Items.Add(childAlgorithmTreeNode);
                    }
                }
            }
        }

        /// <summary>
        /// Save library
        /// </summary>
        /// <param name="LibraryItem">Root library tree view item</param>
        /// <returns>True if library was saved</returns>
        public bool SaveLibrary(TreeViewItem LibraryItem)
        {
            if (LibraryItem == null || !(LibraryItem.Tag is LibraryRootData)) return false;

            LibraryRootData CurrentLibraryData = LibraryItem.Tag as LibraryRootData;

            if (!Directory.Exists(AMSettings.LibsFolder)) Directory.CreateDirectory(AMSettings.LibsFolder);
            if (!Directory.Exists(AMSettings.LibsFolder)) return false;

            string LibraryName = CurrentLibraryData.Name;
            if (LibraryName[LibraryName.Length - 1] == '*')
                LibraryName = LibraryName.Substring(0, LibraryName.Length - 1);

            string DefaultLibPath = System.IO.Path.Combine(AMSettings.LibsFolder, LibraryName + AMSettings.LibsExt);
            string DefaultBackUpPath = System.IO.Path.Combine(AMSettings.LibsFolder, LibraryName + AMSettings.BackUpExt);

            if (File.Exists(DefaultLibPath))
            {
                if (File.Exists(DefaultBackUpPath)) File.Delete(DefaultBackUpPath);
                File.Move(DefaultLibPath, DefaultBackUpPath);
            }

            XmlDocument XmlLib = new XmlDocument();

            XmlElement RootElement = XmlLib.CreateElement("SMLibrary");
            RootElement.SetAttribute("name", LibraryName);
            RootElement.SetAttribute("description", CurrentLibraryData.Description);

            XmlElement FolderElement, AlgorithmElement;
            TreeViewItem FolderItem, AlgorithmItem;
            LibraryFolderData LibFolderData;
            LibraryAlgorithmData LibAlgorithmData;

            for (int folderindex = 0; folderindex < LibraryItem.Items.Count; folderindex++)
            {
                FolderItem = LibraryItem.Items[folderindex] as TreeViewItem;
                if (FolderItem == null || !(FolderItem.Tag is LibraryFolderData)) continue;

                LibFolderData = FolderItem.Tag as LibraryFolderData;
                FolderElement = XmlLib.CreateElement("Folder");
                FolderElement.SetAttribute("name", LibFolderData.Name);
                FolderElement.SetAttribute("description", LibFolderData.Description);

                for (int algindex = 0; algindex < FolderItem.Items.Count; algindex++)
                {
                    AlgorithmItem = FolderItem.Items[algindex] as TreeViewItem;
                    if (AlgorithmItem == null || !(AlgorithmItem.Tag is LibraryAlgorithmData)) continue;

                    LibAlgorithmData = AlgorithmItem.Tag as LibraryAlgorithmData;
                    AlgorithmElement = XmlLib.CreateElement("Algorithm");
                    AlgorithmElement.SetAttribute("name", LibAlgorithmData.Name);
                    AlgorithmElement.SetAttribute("description", LibAlgorithmData.Description);
                    AlgorithmElement.SetAttribute("initial_scramble", LibAlgorithmData.InitScramble);
                    AlgorithmElement.SetAttribute("value", LibAlgorithmData.Algorithm);
                    AlgorithmElement.SetAttribute("mask", LibAlgorithmData.NeutralMask);
                    AlgorithmElement.SetAttribute("initial_view", LibAlgorithmData.StartView.ToString());
                    AlgorithmElement.SetAttribute("initial_alpha3d", LibAlgorithmData.SAlphaInt.ToString());
                    AlgorithmElement.SetAttribute("initial_beta3d", LibAlgorithmData.SBetaInt.ToString());
                    AlgorithmElement.SetAttribute("final_view", LibAlgorithmData.EndView.ToString());
                    AlgorithmElement.SetAttribute("final_alpha3d", LibAlgorithmData.EAlphaInt.ToString());
                    AlgorithmElement.SetAttribute("final_beta3d", LibAlgorithmData.EBetaInt.ToString());
                    AlgorithmElement.SetAttribute("draw_arrows", LibAlgorithmData.DrawArrows ? "Yes" : "No");

                    FolderElement.AppendChild(AlgorithmElement);
                }
                RootElement.AppendChild(FolderElement);
            }
            XmlLib.AppendChild(RootElement);
            XmlLib.Save(DefaultLibPath);

            AMSettings.Status = AMTexts.Message("LibraryUpdatedMessage") + LibraryName + AMSettings.LibsExt;

            CurrentLibraryData.Name = LibraryName;
            LibraryItem.Header = GetLibraryStackPanel(LibraryName);
            CurrentLibraryData.IsModified = false;
            return true;
        }

        /// <summary>
        /// Get a stackpanel with a image and a label for library item in library tree view
        /// </summary>
        /// <param name="LibraryName">Library name</param>
        /// <returns>Library treeview stackpanel</returns>
        private StackPanel GetLibraryStackPanel(string LibraryName)
        {
            // Label for library name in tree view
            Label LibraryLabel = new Label
            {
                VerticalAlignment = VerticalAlignment.Center,
                Content = LibraryName
            };

            // Create stack panel for library name in tree view
            StackPanel LibraryStack = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };
            // Add image and label into stack
            LibraryStack.Children.Add(LibraryTreeView.Resources["LibraryTreeViewImage"] as Image);
            LibraryStack.Children.Add(LibraryLabel);

            return LibraryStack;
        }

        /// <summary>
        /// Get a stackpanel with a image and a label for folder item in library tree view
        /// </summary>
        /// <param name="FolderName">Folder name</param>
        /// <returns>Folder treeview stackpanel</returns>
        private StackPanel GetFolderStackPanel(string FolderName)
        {
            // Label for folder name in tree view
            Label FolderLabel = new Label
            {
                VerticalAlignment = VerticalAlignment.Center,
                Content = FolderName
            };

            // Create stack panel for folder name in tree view
            StackPanel FolderStack = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };
            // Add image and label into stack
            FolderStack.Children.Add(LibraryTreeView.Resources["FolderTreeViewImage"] as Image);
            FolderStack.Children.Add(FolderLabel);

            return FolderStack;
        }

        /// <summary>
        /// Get a stackpanel with a image and a label for algorithm item in library tree view
        /// </summary>
        /// <param name="AlgorithmName">Algorithm name</param>
        /// <returns>Algorithm treeview stackpanel</returns>
        private StackPanel GetAlgorithmStackPanel(string AlgorithmName)
        {
            // Label for algorithm name in tree view
            Label AlgorithmLabel = new Label
            {
                VerticalAlignment = VerticalAlignment.Center,
                Content = AlgorithmName
            };

            // Create stack panel for algorithm name in tree view
            StackPanel AlgorithmStack = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };
            // Add image and label into stack
            AlgorithmStack.Children.Add(LibraryTreeView.Resources["AlgorithmTreeViewImage"] as Image);
            AlgorithmStack.Children.Add(AlgorithmLabel);

            return AlgorithmStack;
        }

        /// <summary>
        /// Checks if a library name already exists
        /// </summary>
        /// <param name="LibraryName">Library name</param>
        /// <returns>True if library already exists</returns>
        public bool LibrayNameAlreadyExists(string LibraryName)
        {
            foreach (TreeViewItem Node in LibraryTreeView.Items)
            {
                if (Node.Tag is LibraryRootData &&
                    string.Compare(LibraryName.Trim(),
                                   ((LibraryRootData)(Node.Tag)).Name.Trim()) == 0) return true;
            }
            return false;
        }

        /// <summary>
        /// Updates the library algorithm
        /// </summary>
        public void UpdateLibraryAlgorithm()
        {
            LibraryAlgorithmToolBar.Items.Clear();

            LibraryData.AlgorithmButtons = new Button[LibraryData.Algorithm.Length + 1];

            for (int i = 0; i <= LibraryData.Algorithm.Length; i++)
            {
                LibraryData.AlgorithmButtons[i] = new Button();

                if (i < LibraryData.AlgorithmPosition)
                    LibraryData.AlgorithmButtons[i].Style = (Style)FindResource("ScrambleButtonGreenBackKey");
                else if (i == LibraryData.AlgorithmPosition)
                    LibraryData.AlgorithmButtons[i].Style = (Style)FindResource("ScrambleButtonWhiteBackKey");
                else
                    LibraryData.AlgorithmButtons[i].Style = (Style)FindResource("ScrambleButtonYellowBackKey");

                if (i == 0) LibraryData.AlgorithmButtons[i].Content = AMTexts.Message("NONEStepMessage"); // First step = NONE
                else LibraryData.AlgorithmButtons[i].Content = ScrambleStep.GetText(LibraryData.Algorithm[i - 1]);
                LibraryData.AlgorithmButtons[i].Tag = i;
                LibraryData.AlgorithmButtons[i].Click += LibraryAlgorithmButtons_Click;

                LibraryAlgorithmToolBar.Items.Add(LibraryData.AlgorithmButtons[i]);
            }
        }

        /// <summary>
        /// From a given tree view item, set his library as modified
        /// </summary>
        /// <param name="ChildItem">Library child item</param>
        /// <returns>True if the library is set as odifie</returns>
        public bool SetLibraryAsModified(TreeViewItem ChildItem)
        {
            if (ChildItem == null || ChildItem.Parent == null) return false;

            TreeViewItem RootItem = ChildItem.Parent as TreeViewItem;

            if (RootItem == null ||
                !(RootItem.Tag is LibraryRootData) &&
                !(RootItem.Tag is LibraryFolderData)) return false;

            if (RootItem.Tag is LibraryFolderData) RootItem = RootItem.Parent as TreeViewItem;

            if (RootItem == null || !(RootItem.Tag is LibraryRootData)) return false;

            LibraryRootData RootData = RootItem.Tag as LibraryRootData;

            RootData.IsModified = true;
            if (RootData.Name[RootData.Name.Length - 1] != '*') RootData.Name += "*";
            RootItem.Header = GetLibraryStackPanel(RootData.Name);

            return RootData.IsModified;
        }

        /// <summary>
        /// Updates the name and description of current algorithm, folder and library 
        /// </summary>
        public void UpdateLibraryAlgorithmTexts()
        {
            if (LibraryData.CurrentAlgorithm != null && LibraryData.CurrentAlgorithm.Tag != null &&
                LibraryData.CurrentFolder != null && LibraryData.CurrentFolder.Tag != null &&
                LibraryData.CurrentLibrary != null && LibraryData.CurrentLibrary.Tag != null)
            {
                LibraryAlgorithmData CurrentAlgorithmData = LibraryData.CurrentAlgorithm.Tag as LibraryAlgorithmData;
                LibraryFolderData CurrentFolderData = LibraryData.CurrentFolder.Tag as LibraryFolderData;
                LibraryRootData CurrentLibraryData = LibraryData.CurrentLibrary.Tag as LibraryRootData;

                LibraryAlgorithmNameLabelText.Text = CurrentLibraryData.Name + AMTexts.Message("RightArrowMessage") +
                                                    CurrentFolderData.Name + AMTexts.Message("RightArrowMessage") +
                                                    CurrentAlgorithmData.Name;

                LibraryAlgorithmNameInfoLabelText.Text = CurrentAlgorithmData.Name;
                LibraryAlgorithmInfoDescriptionTextBox.Text = CurrentAlgorithmData.Description;
                LibraryFolderNameInfoLabelText.Text = CurrentFolderData.Name;
                LibraryFolderInfoDescriptionTextBox.Text = CurrentFolderData.Description;
                LibraryNameInfoLabelText.Text = CurrentLibraryData.Name;
                LibraryInfoDescriptionTextBox.Text = CurrentLibraryData.Description;
            }
        }

        #region Library static functions

        /// <summary>
        /// Find TreeViewItem using the VisualTreeHelper
        /// </summary>
        /// <param name="source">Object to search the TreeViewItem</param>
        /// <returns>Current TreeViewItem</returns>
        private static TreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
                source = VisualTreeHelper.GetParent(source);

            return source as TreeViewItem;
        }

        /// <summary>
        /// Check if an item header already exists in an item collection
        /// </summary>
        /// <param name="treeViewItems">Item collection</param>
        /// <param name="input">String to check headers</param>
        /// <returns></returns>
        private static bool CheckItemHeader(ItemCollection treeViewItems, string input)
        {
            for (int index = 0; index < treeViewItems.Count; index++)
            {
                TreeViewItem item = (TreeViewItem)treeViewItems[index];
                if ((string)item.Header == input) return true;
                else if (item.Items.Count > 1) return CheckItemHeader(item.Items, input);
            }
            return false;
        }

        #endregion Library static functions

        #endregion Library functions

        #region Library events

        #region Library algorithm buttons events

        /// <summary>
        /// Click event: Library algorithm buttons (asigned programatically)
        /// </summary>
        public void LibraryAlgorithmButtons_Click(object sender, EventArgs e)
        {
            if (LibraryData.AnimWork != null && !LibraryData.AnimWork.IsBusy)
            {
                Button StepButton = (Button)sender;

                LibraryData.AlgorithmPosTarget = int.Parse(StepButton.Tag.ToString());

                if (LibraryData.AlgorithmPosTarget < 0 || LibraryData.AlgorithmPosTarget > LibraryData.Algorithm.Length ||
                    LibraryData.AlgorithmPosTarget == LibraryData.AlgorithmPosition) return;

                LibraryData.AnimWork.RunWorkerAsync(); // Start scramble animation
            }
        }

        /// <summary>
        /// Click event: Start library algorithm step button
        /// </summary>
        private void LibraryAlgorithmStartButton_Click(object sender, RoutedEventArgs e)
        {
            if (!LibraryData.AnimWork.IsBusy)
            {
                LibraryData.AlgorithmPosition = 0;
                UpdateLibraryViews();
            }
        }

        /// <summary>
        /// Click event: Previous library algorithm step button
        /// </summary>
        private void LibraryAlgorithmPreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (!LibraryData.AnimWork.IsBusy)
            {
                if (LibraryData.AlgorithmPosition > 0)
                    LibraryAlgorithmButtons_Click(LibraryData.AlgorithmButtons[LibraryData.AlgorithmPosition - 1], null);
            }
        }

        /// <summary>
        /// Click event: Plays library algorithm steps sequence button
        /// </summary>
        private void LibraryAlgorithmPlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (!LibraryData.AnimWork.IsBusy)
            {
                LibraryData.AlgorithmPosition = 0;
                UpdateLibraryViews();
                System.Threading.Thread.Sleep(250);
                LibraryAlgorithmButtons_Click(LibraryData.AlgorithmButtons[LibraryData.AlgorithmButtons.Length - 1], null);
            }
        }

        /// <summary>
        /// Click event: Next library algorithm step button
        /// </summary>
        private void LibraryAlgorithmNextButton_Click(object sender, RoutedEventArgs e)
        {
            if (!LibraryData.AnimWork.IsBusy)
            {
                if (LibraryData.AlgorithmPosition >= LibraryData.AlgorithmButtons.Length - 1) return;
                LibraryAlgorithmButtons_Click(LibraryData.AlgorithmButtons[LibraryData.AlgorithmPosition + 1], null);
            }
        }

        /// <summary>
        /// Click event: End library algorithm step button
        /// </summary>
        private void LibraryAlgorithmEndButton_Click(object sender, RoutedEventArgs e)
        {
            if (!LibraryData.AnimWork.IsBusy)
            {
                LibraryData.AlgorithmPosition = LibraryData.Algorithm.Length;
                UpdateLibraryViews();
            }
        }

        #endregion Library algorithm buttons events

        #region BackgroundWorker events

        /// <summary>
        /// Library BackgorundWorker DoWork event (asigned programatically)
        /// </summary>
        private void LibraryAnimWork_DoWork(object sender, DoWorkEventArgs e)
        {
            Steps SD;
            Stopwatch AnimTimeControl = new Stopwatch();

            Dispatcher.Invoke(new Action(delegate { Cursor = Cursors.Wait; }));

            if (LibraryData.AlgorithmPosition > LibraryData.AlgorithmPosTarget)
            {
                while (LibraryData.AlgorithmPosTarget != LibraryData.AlgorithmPosition)
                {
                    SD = ScrambleStep.Inverse(LibraryData.Algorithm[LibraryData.AlgorithmPosition - 1]);

                    Dispatcher.Invoke(new Action(delegate
                    {
                        LibraryData.AlgorithmButtons[LibraryData.AlgorithmPosition].Style = (Style)FindResource("ScrambleButtonWhiteBackKey");
                    }));

                    if (!ScrambleStep.IsParenthesis(SD))
                    {
                        AnimTimeControl.Restart();

                        while (AnimTimeControl.ElapsedMilliseconds < AMSettings.ChronoAnimTime)
                        {
                            Dispatcher.Invoke(new Action(delegate
                            {
                                LibraryData.AlgorithmCube.RotateStepPorcentage(SD, (double)AnimTimeControl.ElapsedMilliseconds / AMSettings.ChronoAnimTime);
                                LibraryData.AlgorithmCube.UpdateBitmap((int)LibraryAlgImage3D.ActualWidth, (int)LibraryAlgImage3D.ActualHeight);
                                LibraryAlgImage3D.Source = LibraryData.AlgorithmCube.renderBMP;
                            }));
                            System.Threading.Thread.Sleep(10);
                        }
                        AnimTimeControl.Stop();
                    }
                    Dispatcher.Invoke(new Action(delegate
                    {
                        LibraryData.AlgorithmCube.RotateToZero();
                        LibraryData.AlgorithmCube.ApplyStep(SD);
                        LibraryData.AlgorithmCube.UpdateBitmap((int)LibraryAlgImage3D.ActualWidth, (int)LibraryAlgImage3D.ActualHeight);
                        LibraryAlgImage3D.Source = LibraryData.AlgorithmCube.renderBMP;

                        LibraryData.AlgorithmButtons[LibraryData.AlgorithmPosition].Style = (Style)FindResource("ScrambleButtonYellowBackKey");

                        UpdateLibraryInitViewCube2D();
                        UpdateLibraryAlgViewCube2D();
                    }));

                    LibraryData.AlgorithmPosition--;
                }
            }
            else if (LibraryData.AlgorithmPosition < LibraryData.AlgorithmPosTarget)
            {
                while (LibraryData.AlgorithmPosTarget != LibraryData.AlgorithmPosition)
                {
                    Dispatcher.Invoke(new Action(delegate
                    {
                        LibraryData.AlgorithmButtons[LibraryData.AlgorithmPosition].Style = (Style)FindResource("ScrambleButtonGreenBackKey");
                    }));

                    LibraryData.AlgorithmPosition++;

                    SD = LibraryData.Algorithm[LibraryData.AlgorithmPosition - 1];

                    Dispatcher.Invoke(new Action(delegate
                    {
                        LibraryData.AlgorithmButtons[LibraryData.AlgorithmPosition].Style = (Style)FindResource("ScrambleButtonWhiteBackKey");
                    }));

                    if (!ScrambleStep.IsParenthesis(SD))
                    {
                        AnimTimeControl.Restart();

                        while (AnimTimeControl.ElapsedMilliseconds < AMSettings.ChronoAnimTime)
                        {
                            Dispatcher.Invoke(new Action(delegate
                            {
                                LibraryData.AlgorithmCube.RotateStepPorcentage(SD, (double)AnimTimeControl.ElapsedMilliseconds / AMSettings.ChronoAnimTime);
                                LibraryData.AlgorithmCube.UpdateBitmap((int)LibraryAlgImage3D.ActualWidth, (int)LibraryAlgImage3D.ActualHeight);
                                LibraryAlgImage3D.Source = LibraryData.AlgorithmCube.renderBMP;
                            }));
                            System.Threading.Thread.Sleep(10);
                        }
                        AnimTimeControl.Stop();
                    }
                    Dispatcher.Invoke(new Action(delegate
                    {
                        LibraryData.AlgorithmCube.RotateToZero();
                        LibraryData.AlgorithmCube.ApplyStep(SD);
                        LibraryData.AlgorithmCube.UpdateBitmap((int)LibraryAlgImage3D.ActualWidth, (int)LibraryAlgImage3D.ActualHeight);
                        LibraryAlgImage3D.Source = LibraryData.AlgorithmCube.renderBMP;

                        LibraryData.AlgorithmButtons[LibraryData.AlgorithmPosition].Style = (Style)FindResource("ScrambleButtonGreenBackKey");

                        UpdateLibraryInitViewCube2D();
                        UpdateLibraryAlgViewCube2D();
                    }));
                }
            }

            Dispatcher.Invoke(new Action(delegate
            {
                LibraryData.AlgorithmButtons[LibraryData.AlgorithmPosition].Style = (Style)FindResource("ScrambleButtonWhiteBackKey");
            }));
        }

        /// <summary>
        /// RunWorkerCompleted event (asigned programatically)
        /// </summary>
        private void LibraryAnimWork_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((e.Cancelled))
            {
            }
            else if (e.Error != null)
            {
                Dispatcher.Invoke(new Action(delegate { AMSettings.Log = "Library animation error: " + e.Error.Message; }));
            }
            else
            {
            }
            Dispatcher.Invoke(new Action(delegate { Cursor = Cursors.Arrow; }));
        }

        #endregion BackgroundWorker events

        #region Library tree view events

        /// <summary>
        /// SelectedItemChanged event: Update context menu and description
        /// </summary>
        private void LibraryTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            LibraryTreeTextBox.Clear(); // Clear description text box

            TreeViewItem item = LibraryTreeView.SelectedItem as TreeViewItem;

            if (item == null)
            {
                LibraryTreeView.ContextMenu = null;
                return;
            }

            if (item.Tag is LibraryRootData)
            {
                LibraryTreeTextBox.Text = ((LibraryRootData)(item.Tag)).Description;
                LibraryTreeView.ContextMenu = LibraryTreeView.Resources["LibraryContext"] as ContextMenu;
                LibraryTreeView.MouseDoubleClick -= LibraryOpenAlgorithmMenuItem_Click;
                GoUpLibraryItemButton.IsEnabled = false;
                GoDownLibraryItemButton.IsEnabled = false;
            }
            else if (item.Tag is LibraryFolderData)
            {
                LibraryTreeTextBox.Text = ((LibraryFolderData)(item.Tag)).Description;
                LibraryTreeView.ContextMenu = LibraryTreeView.Resources["FolderContext"] as ContextMenu;
                LibraryTreeView.MouseDoubleClick -= LibraryOpenAlgorithmMenuItem_Click;
                GoUpLibraryItemButton.IsEnabled = (item.Parent as TreeViewItem).Items.Count > 1;
                GoDownLibraryItemButton.IsEnabled = (item.Parent as TreeViewItem).Items.Count > 1;
            }
            else if (item.Tag is LibraryAlgorithmData)
            {
                LibraryTreeTextBox.Text = ((LibraryAlgorithmData)(item.Tag)).Description;
                LibraryTreeView.ContextMenu = LibraryTreeView.Resources["AlgorithmContext"] as ContextMenu;
                LibraryTreeView.MouseDoubleClick += LibraryOpenAlgorithmMenuItem_Click;
                GoUpLibraryItemButton.IsEnabled = (item.Parent as TreeViewItem).Items.Count > 1;
                GoDownLibraryItemButton.IsEnabled = (item.Parent as TreeViewItem).Items.Count > 1;
            }
            else
            {
                LibraryTreeView.ContextMenu = null;
                GoUpLibraryItemButton.IsEnabled = true;
                GoDownLibraryItemButton.IsEnabled = true;
                LibraryTreeView.MouseDoubleClick -= LibraryOpenAlgorithmMenuItem_Click;
            }
        }

        /// <summary>
        /// PreviewMouseRightButtonDown event: Select current item with right click
        /// </summary>
        private void LibraryTreeView_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);
            if (treeViewItem != null)
            {
                treeViewItem.Focus();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Click event: Create new library
        /// </summary>
        private void NewLibraryButton_Click(object sender, RoutedEventArgs e)
        {
            LibraryTreeViewElement NewLibraryWindow = new LibraryTreeViewElement
            {
                Owner = AlgorithmMasterWindow,
                ElementTitle = AMTexts.Message("NewLibraryMessage")
            };

            if (NewLibraryWindow.ShowDialog() ?? true)
            {
                if (!LibrayNameAlreadyExists(NewLibraryWindow.ElementName))
                {
                    LibraryRootData LibData = new LibraryRootData
                    {
                        Name = NewLibraryWindow.ElementName,
                        Description = NewLibraryWindow.ElementDescription,
                        IsModified = true
                    };

                    TreeViewItem treeNode = new TreeViewItem
                    {
                        Header = GetLibraryStackPanel(LibData.Name),
                        Tag = LibData,
                        IsExpanded = false
                    };

                    LibraryTreeView.Items.Add(treeNode);
                }
                else
                {
                    MessageBox.Show(string.Format(AMTexts.Message("LibraryExistsMessage"), NewLibraryWindow.ElementName),
                                    AMTexts.Message("LibraryNotCreatedMessage"),
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Warning);
                }
            }
        }

        /// <summary>
        /// Click event: Add folder to library
        /// </summary>
        private void LibraryAddFolderMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (LibraryTreeView.SelectedItem == null) return;

            TreeViewItem CurrentLibraryItem = LibraryTreeView.SelectedItem as TreeViewItem;

            if (!(CurrentLibraryItem.Tag is LibraryRootData)) return;

            LibraryRootData CurrentLibraryData = CurrentLibraryItem.Tag as LibraryRootData;

            LibraryTreeViewElement NewFolderWindow = new LibraryTreeViewElement
            {
                Owner = AlgorithmMasterWindow,
                ElementTitle = AMTexts.Message("NewFolderMessage")
            };

            if (NewFolderWindow.ShowDialog() ?? true)
            {
                LibraryFolderData FolderData = new LibraryFolderData
                {
                    Name = NewFolderWindow.ElementName,
                    Description = NewFolderWindow.ElementDescription
                };

                TreeViewItem childFolderTreeNode = new TreeViewItem
                {
                    Header = GetFolderStackPanel(FolderData.Name),
                    IsExpanded = true,
                    Tag = FolderData
                };

                CurrentLibraryItem.Items.Add(childFolderTreeNode);

                CurrentLibraryData.IsModified = true;
            }
        }

        /// <summary>
        /// Click event: Change library name
        /// </summary>
        private void LibraryRenameLibraryMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (LibraryTreeView.SelectedItem == null) return;

            TreeViewItem CurrentLibraryItem = LibraryTreeView.SelectedItem as TreeViewItem;

            if (!(CurrentLibraryItem.Tag is LibraryRootData)) return;

            LibraryRootData CurrentLibraryData = CurrentLibraryItem.Tag as LibraryRootData;

            string LibraryName = CurrentLibraryData.Name;
            if (LibraryName[LibraryName.Length - 1] == '*')
                LibraryName = LibraryName.Substring(0, LibraryName.Length - 1);

            LibraryTreeViewElement RenameLibraryWindow = new LibraryTreeViewElement
            {
                Owner = AlgorithmMasterWindow,
                ElementTitle = AMTexts.Message("RenameLibraryMessage"),
                ElementName = LibraryName,
                ElementDescription = CurrentLibraryData.Description,
                IsDescriptionEnabled = false
            };

            if (RenameLibraryWindow.ShowDialog() ?? true)
            {
                if (!LibrayNameAlreadyExists(RenameLibraryWindow.ElementName))
                {
                    CurrentLibraryData.Name = RenameLibraryWindow.ElementName + '*';
                    CurrentLibraryItem.Header = GetLibraryStackPanel(CurrentLibraryData.Name);
                    CurrentLibraryData.IsModified = true;
                }
                else
                {
                    MessageBox.Show(string.Format(AMTexts.Message("LibraryExistsMessage"), RenameLibraryWindow.ElementName),
                                    AMTexts.Message("LibraryNotRenamedMessage"),
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Warning);
                }
            }
        }

        /// <summary>
        /// Click event: Change library description
        /// </summary>
        private void LibraryEditLibraryDescriptionMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (LibraryTreeView.SelectedItem == null) return;

            TreeViewItem CurrentLibraryItem = LibraryTreeView.SelectedItem as TreeViewItem;

            if (!(CurrentLibraryItem.Tag is LibraryRootData)) return;

            LibraryRootData CurrentLibraryData = CurrentLibraryItem.Tag as LibraryRootData;

            LibraryTreeViewElement ChangeDescriptionLibraryWindow = new LibraryTreeViewElement
            {
                Owner = AlgorithmMasterWindow,
                ElementTitle = AMTexts.Message("ChangeLibraryDescriptionMessage"),
                ElementName = CurrentLibraryData.Name,
                ElementDescription = CurrentLibraryData.Description,
                IsNameEnabled = false
            };

            if (ChangeDescriptionLibraryWindow.ShowDialog() ?? true)
            {
                CurrentLibraryData.Description = ChangeDescriptionLibraryWindow.ElementDescription;
                LibraryTreeTextBox.Text = ChangeDescriptionLibraryWindow.ElementDescription;
                CurrentLibraryData.IsModified = true;
            }
        }

        /// <summary>
        /// Click event: Save library
        /// </summary>
        private void LibrarySaveLibraryMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SaveLibrary(LibraryTreeView.SelectedItem as TreeViewItem);
        }

        /// <summary>
        /// Click event: Delete library description
        /// </summary>
        private void LibraryDeleteLibraryMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (LibraryTreeView.SelectedItem == null) return;

            TreeViewItem CurrentLibraryItem = LibraryTreeView.SelectedItem as TreeViewItem;

            if (!(CurrentLibraryItem.Tag is LibraryRootData)) return;

            LibraryRootData CurrentLibraryData = CurrentLibraryItem.Tag as LibraryRootData;

            if (MessageBox.Show(string.Format(AMTexts.Message("LibraryWillBeDeletedMessage"), CurrentLibraryData.Name),
                                    AMTexts.Message("PleaseConfirmMessage"),
                                    MessageBoxButton.YesNo,
                                    MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                LibraryTreeView.Items.Remove(CurrentLibraryItem);

                if (!Directory.Exists(AMSettings.LibsFolder)) Directory.CreateDirectory(AMSettings.LibsFolder);
                if (!Directory.Exists(AMSettings.LibsFolder)) return;

                string LibraryName = CurrentLibraryData.Name;
                if (LibraryName[LibraryName.Length - 1] == '*')
                    LibraryName = LibraryName.Substring(0, LibraryName.Length - 1);

                string DefaultLibPath = System.IO.Path.Combine(AMSettings.LibsFolder, LibraryName + AMSettings.LibsExt);
                if (File.Exists(DefaultLibPath)) File.Delete(DefaultLibPath);

                DefaultLibPath = System.IO.Path.Combine(AMSettings.LibsFolder, LibraryName + AMSettings.BackUpExt);
                if (File.Exists(DefaultLibPath)) File.Delete(DefaultLibPath);
            }
        }

        /// <summary>
        /// Click event: Add algorithm to folder
        /// </summary>
        private void LibraryAddAlgorithmMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (LibraryTreeView.SelectedItem == null) return;

            TreeViewItem CurrentLibraryItem = LibraryTreeView.SelectedItem as TreeViewItem;

            if (!(CurrentLibraryItem.Tag is LibraryFolderData)) return;

            LibraryFolderData CurrentFolderData = CurrentLibraryItem.Tag as LibraryFolderData;

            LibraryTreeViewElement NewAlgorithmWindow = new LibraryTreeViewElement
            {
                Owner = AlgorithmMasterWindow,
                ElementTitle = AMTexts.Message("NewAlgorithmMessage")
            };

            if (NewAlgorithmWindow.ShowDialog() ?? true)
            {
                LibraryAlgorithmData AlgorithmData = new LibraryAlgorithmData
                {
                    Name = NewAlgorithmWindow.ElementName,
                    Description = NewAlgorithmWindow.ElementDescription
                };

                TreeViewItem childAlgorithmTreeNode = new TreeViewItem
                {
                    Header = GetAlgorithmStackPanel(AlgorithmData.Name),
                    IsExpanded = true,
                    Tag = AlgorithmData
                };

                CurrentLibraryItem.Items.Add(childAlgorithmTreeNode);
                SetLibraryAsModified(CurrentLibraryItem);
            }
        }

        /// <summary>
        /// Click event: Paste algorithm
        /// </summary>
        private void LibraryPasteAlgorithmMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (LibraryTreeView.SelectedItem == null || LibraryData.AlgorithmCopied == null) return;

            TreeViewItem CurrentLibraryItem = LibraryTreeView.SelectedItem as TreeViewItem;

            if (!(CurrentLibraryItem.Tag is LibraryFolderData)) return;

            LibraryAlgorithmData AuxAlgorithmData = new LibraryAlgorithmData
            {
                Name = LibraryData.AlgorithmCopied.Name,
                Description = LibraryData.AlgorithmCopied.Description,
                InitScramble = LibraryData.AlgorithmCopied.InitScramble,
                Algorithm = LibraryData.AlgorithmCopied.Algorithm,
                NeutralMask = LibraryData.AlgorithmCopied.NeutralMask,
                StartView = LibraryData.AlgorithmCopied.StartView,
                EndView = LibraryData.AlgorithmCopied.EndView,
                S3DViewAlpha = LibraryData.AlgorithmCopied.S3DViewAlpha,
                E3DViewAlpha = LibraryData.AlgorithmCopied.E3DViewAlpha,
                S3DViewBeta = LibraryData.AlgorithmCopied.S3DViewBeta,
                E3DViewBeta = LibraryData.AlgorithmCopied.E3DViewBeta,
                DrawArrows = LibraryData.AlgorithmCopied.DrawArrows
            };

            TreeViewItem childAlgorithmTreeNode = new TreeViewItem
            {
                Header = GetAlgorithmStackPanel(AuxAlgorithmData.Name),
                IsExpanded = true,
                Tag = AuxAlgorithmData
            };

            CurrentLibraryItem.Items.Add(childAlgorithmTreeNode);
            SetLibraryAsModified(CurrentLibraryItem);
        }

        /// <summary>
        /// Click event: Change folder name
        /// </summary>
        private void LibraryRenameFolderMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (LibraryTreeView.SelectedItem == null) return;

            TreeViewItem CurrentLibraryItem = LibraryTreeView.SelectedItem as TreeViewItem;

            if (!(CurrentLibraryItem.Tag is LibraryFolderData)) return;

            LibraryFolderData CurrentFolderData = CurrentLibraryItem.Tag as LibraryFolderData;

            LibraryTreeViewElement RenameFolderWindow = new LibraryTreeViewElement
            {
                Owner = AlgorithmMasterWindow,
                ElementTitle = AMTexts.Message("RenameFolderMessage"),
                ElementName = CurrentFolderData.Name,
                ElementDescription = CurrentFolderData.Description,
                IsDescriptionEnabled = false
            };

            if (RenameFolderWindow.ShowDialog() ?? true)
            {
                CurrentFolderData.Name = RenameFolderWindow.ElementName;
                CurrentLibraryItem.Header = GetFolderStackPanel(CurrentFolderData.Name);
                SetLibraryAsModified(CurrentLibraryItem);
            }
        }

        /// <summary>
        /// Click event: Change folder description
        /// </summary>
        private void LibraryEditFolderDescriptionMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (LibraryTreeView.SelectedItem == null) return;

            TreeViewItem CurrentLibraryItem = LibraryTreeView.SelectedItem as TreeViewItem;

            if (!(CurrentLibraryItem.Tag is LibraryFolderData)) return;

            LibraryFolderData CurrentFolderData = CurrentLibraryItem.Tag as LibraryFolderData;

            LibraryTreeViewElement ChangeFolderDescriptionWindow = new LibraryTreeViewElement
            {
                Owner = AlgorithmMasterWindow,
                ElementTitle = AMTexts.Message("ChangeFolderDescriptionMessage"),
                ElementName = CurrentFolderData.Name,
                ElementDescription = CurrentFolderData.Description,
                IsNameEnabled = false
            };

            if (ChangeFolderDescriptionWindow.ShowDialog() ?? true)
            {
                CurrentFolderData.Description = ChangeFolderDescriptionWindow.ElementDescription;
                LibraryTreeTextBox.Text = ChangeFolderDescriptionWindow.ElementDescription;
                SetLibraryAsModified(CurrentLibraryItem);
            }
        }

        /// <summary>
        /// Click event: Delete folder
        /// </summary>
        private void LibraryDeleteFolderMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (LibraryTreeView.SelectedItem == null) return;

            TreeViewItem CurrentLibraryItem = LibraryTreeView.SelectedItem as TreeViewItem;

            if (!(CurrentLibraryItem.Tag is LibraryFolderData)) return;

            LibraryFolderData CurrentFolderData = CurrentLibraryItem.Tag as LibraryFolderData;

            if (MessageBox.Show(string.Format(AMTexts.Message("FolderWillBeDeletedMessage"), CurrentFolderData.Name),
                                    AMTexts.Message("PleaseConfirmMessage"),
                                    MessageBoxButton.YesNo,
                                    MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                SetLibraryAsModified(CurrentLibraryItem);
                (CurrentLibraryItem.Parent as TreeViewItem).Items.Remove(CurrentLibraryItem);
            }
        }

        /// <summary>
        /// Click event: Open algorithm
        /// </summary>
        private void LibraryOpenAlgorithmMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (LibraryTreeView.SelectedItem == null) return;

            TreeViewItem CurrentLibraryItem = LibraryTreeView.SelectedItem as TreeViewItem;
            if (!(CurrentLibraryItem.Tag is LibraryAlgorithmData)) return;

            LibraryData.CurrentAlgorithm = CurrentLibraryItem;
            LibraryData.CurrentFolder = LibraryData.CurrentAlgorithm.Parent as TreeViewItem;
            if (!(LibraryData.CurrentFolder.Tag is LibraryFolderData)) return;

            LibraryData.CurrentLibrary = LibraryData.CurrentFolder.Parent as TreeViewItem;
            if (!(LibraryData.CurrentLibrary.Tag is LibraryRootData)) return;

            if (LibraryData.CurrentAlgorithm.Tag == null || !(LibraryData.CurrentAlgorithm.Tag is LibraryAlgorithmData) ||
                LibraryData.CurrentFolder.Tag == null || !(LibraryData.CurrentFolder.Tag is LibraryFolderData) ||
                LibraryData.CurrentLibrary.Tag == null || !(LibraryData.CurrentLibrary.Tag is LibraryRootData))
            {
                LibraryData.CurrentAlgorithm = null;
                LibraryData.CurrentFolder = null;
                LibraryData.CurrentLibrary = null;
                LibraryAlgorithmNameLabelText.Text = AMTexts.Message("OpenAlgorithmMessage");
                return;
            }

            LibraryAlgorithmEditCheckBox.IsChecked = false;
            LibraryData.AlgorithmEditionEnabled = false;

            LibraryAlgorithmData CurrentAlgorithmData = LibraryData.CurrentAlgorithm.Tag as LibraryAlgorithmData;
            LibraryFolderData CurrentFolderData = LibraryData.CurrentFolder.Tag as LibraryFolderData;
            LibraryRootData CurrentLibraryData = LibraryData.CurrentLibrary.Tag as LibraryRootData;

            UpdateLibraryAlgorithmTexts();

            LibraryData.InitScramble = new Scramble();
            LibraryData.InitScramble.ReadScramble(CurrentAlgorithmData.InitScramble);
            LibraryInitialpositionLabelText.Text = LibraryInitPosLabelToolTipBody.Text = CurrentAlgorithmData.InitScramble;

            LibraryData.InitScrambleCube.NeutralMask = CurrentAlgorithmData.NeutralMask;
            LibraryData.InitScrambleCube.SetCamera(AMSettings.CameraDistance,
                                                   CurrentAlgorithmData.SAlphaInt,
                                                   CurrentAlgorithmData.SBetaInt);

            LibraryData.Algorithm = new Scramble();
            if (AMSettings.LibraryShowParentheses)
                LibraryData.Algorithm.ReadScramble(CurrentAlgorithmData.Algorithm);
            else
                LibraryData.Algorithm.ParseScramble(CurrentAlgorithmData.Algorithm);

            LibraryData.AlgorithmPosition = LibraryData.AlgorithmPosTarget = LibraryData.Algorithm.Length;
            LibraryAlgorithmBaseLabelText.Text = LibraryAlgorithmBaseLabelToolTipBody.Text = CurrentAlgorithmData.Algorithm;

            LibraryData.AlgorithmCube.NeutralMask = CurrentAlgorithmData.NeutralMask;
            LibraryData.AlgorithmCube.SetCamera(AMSettings.CameraDistance,
                                                CurrentAlgorithmData.EAlphaInt,
                                                CurrentAlgorithmData.EBetaInt);

            if (AMSettings.LibraryDefaultPositionStart) LibraryData.AlgorithmPosition = 0;
            else LibraryData.AlgorithmPosition = LibraryData.Algorithm.Length;

            switch (CurrentAlgorithmData.StartView)
            {
                case '2': LibraryInitialPositionTabControl.SelectedIndex = 1; break;
                case 'U': LibraryInitialPositionTabControl.SelectedIndex = 2; break;
                case 'D': LibraryInitialPositionTabControl.SelectedIndex = 3; break;
                case 'F': LibraryInitialPositionTabControl.SelectedIndex = 4; break;
                case 'B': LibraryInitialPositionTabControl.SelectedIndex = 5; break;
                case 'R': LibraryInitialPositionTabControl.SelectedIndex = 6; break;
                case 'L': LibraryInitialPositionTabControl.SelectedIndex = 7; break;
                default: LibraryInitialPositionTabControl.SelectedIndex = 0; break;
            }

            switch (CurrentAlgorithmData.EndView)
            {
                case '2': LibraryAlgorithmTabControl.SelectedIndex = 1; break;
                case 'U': LibraryAlgorithmTabControl.SelectedIndex = 2; break;
                case 'D': LibraryAlgorithmTabControl.SelectedIndex = 3; break;
                case 'F': LibraryAlgorithmTabControl.SelectedIndex = 4; break;
                case 'B': LibraryAlgorithmTabControl.SelectedIndex = 5; break;
                case 'R': LibraryAlgorithmTabControl.SelectedIndex = 6; break;
                case 'L': LibraryAlgorithmTabControl.SelectedIndex = 7; break;
                default: LibraryAlgorithmTabControl.SelectedIndex = 0; break;
            }

            LibraryInitPosShowArrowsCheckBox.IsChecked = CurrentAlgorithmData.DrawArrows;

            LibraryCopyMenuItem.IsEnabled = true;

            UpdateLibraryViews();
        }

        /// <summary>
        /// Click event: Change algorithm name
        /// </summary>
        private void LibraryRenameAlgorithmMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (LibraryTreeView.SelectedItem == null) return;

            TreeViewItem CurrentLibraryItem = LibraryTreeView.SelectedItem as TreeViewItem;

            if (!(CurrentLibraryItem.Tag is LibraryAlgorithmData)) return;

            LibraryAlgorithmData CurrentAlgorithmData = CurrentLibraryItem.Tag as LibraryAlgorithmData;

            LibraryTreeViewElement RenameAlgorithmWindow = new LibraryTreeViewElement
            {
                Owner = AlgorithmMasterWindow,
                ElementTitle = AMTexts.Message("RenameAlgorithmMessage"),
                ElementName = CurrentAlgorithmData.Name,
                ElementDescription = CurrentAlgorithmData.Description,
                IsDescriptionEnabled = false
            };

            if (RenameAlgorithmWindow.ShowDialog() ?? true)
            {
                CurrentAlgorithmData.Name = RenameAlgorithmWindow.ElementName;
                CurrentLibraryItem.Header = GetAlgorithmStackPanel(CurrentAlgorithmData.Name);
                SetLibraryAsModified(CurrentLibraryItem);
            }
        }

        /// <summary>
        /// Click event: Change algorithm description
        /// </summary>
        private void LibraryEditAlgorithmDescriptionMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (LibraryTreeView.SelectedItem == null) return;

            TreeViewItem CurrentLibraryItem = LibraryTreeView.SelectedItem as TreeViewItem;

            if (!(CurrentLibraryItem.Tag is LibraryAlgorithmData)) return;

            LibraryAlgorithmData CurrentAlgorithmData = CurrentLibraryItem.Tag as LibraryAlgorithmData;

            LibraryTreeViewElement ChangeAlgorithmDescriptionWindow = new LibraryTreeViewElement
            {
                Owner = AlgorithmMasterWindow,
                ElementTitle = AMTexts.Message("ChangeAlgorithmDescriptionMessage"),
                ElementName = CurrentAlgorithmData.Name,
                ElementDescription = CurrentAlgorithmData.Description,
                IsNameEnabled = false
            };

            if (ChangeAlgorithmDescriptionWindow.ShowDialog() ?? true)
            {
                CurrentAlgorithmData.Description = ChangeAlgorithmDescriptionWindow.ElementDescription;
                LibraryTreeTextBox.Text = ChangeAlgorithmDescriptionWindow.ElementDescription;
                SetLibraryAsModified(CurrentLibraryItem);
            }
        }

        /// <summary>
        /// Click event: Copy algorithm
        /// </summary>
        private void LibraryCopyAlgorithmMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (LibraryTreeView.SelectedItem == null) return;

            TreeViewItem CurrentLibraryItem = LibraryTreeView.SelectedItem as TreeViewItem;

            if (!(CurrentLibraryItem.Tag is LibraryAlgorithmData)) return;

            LibraryAlgorithmData CurrentAlgorithmData = CurrentLibraryItem.Tag as LibraryAlgorithmData;

            LibraryData.AlgorithmCopied = new LibraryAlgorithmData
            {
                Name = CurrentAlgorithmData.Name,
                Description = CurrentAlgorithmData.Description,
                InitScramble = CurrentAlgorithmData.InitScramble,
                Algorithm = CurrentAlgorithmData.Algorithm,
                NeutralMask = CurrentAlgorithmData.NeutralMask,
                StartView = CurrentAlgorithmData.StartView,
                EndView = CurrentAlgorithmData.EndView,
                S3DViewAlpha = CurrentAlgorithmData.S3DViewAlpha,
                E3DViewAlpha = CurrentAlgorithmData.E3DViewAlpha,
                S3DViewBeta = CurrentAlgorithmData.S3DViewBeta,
                E3DViewBeta = CurrentAlgorithmData.E3DViewBeta,
                DrawArrows = CurrentAlgorithmData.DrawArrows
            };
        }

        /// <summary>
        /// Click event: Delete algorithm
        /// </summary>
        private void LibraryDeleteAlgorithmMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (LibraryTreeView.SelectedItem == null) return;

            TreeViewItem CurrentLibraryItem = LibraryTreeView.SelectedItem as TreeViewItem;

            if (!(CurrentLibraryItem.Tag is LibraryAlgorithmData)) return;

            LibraryAlgorithmData CurrentAlgorithmData = CurrentLibraryItem.Tag as LibraryAlgorithmData;

            if (MessageBox.Show(string.Format(AMTexts.Message("AlgorithmWillBeDeletedMessage"), CurrentAlgorithmData.Name),
                                    AMTexts.Message("PleaseConfirmMessage"),
                                    MessageBoxButton.YesNo,
                                    MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                SetLibraryAsModified(CurrentLibraryItem);
                (CurrentLibraryItem.Parent as TreeViewItem).Items.Remove(CurrentLibraryItem);
            }
        }

        /// <summary>
        /// Click event: Move up library tree view item
        /// </summary>
        private void MoveUpLibraryItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (LibraryTreeView.SelectedItem == null) return;

            TreeViewItem CurrentLibraryItem = LibraryTreeView.SelectedItem as TreeViewItem;

            if (CurrentLibraryItem.Parent == null) return;

            TreeViewItem ParentLibraryItem = CurrentLibraryItem.Parent as TreeViewItem;

            int ItemPos = -1;
            for (int n = 0; n < ParentLibraryItem.Items.Count; n++)
            {
                if (ParentLibraryItem.Items[n] == CurrentLibraryItem)
                {
                    ItemPos = n;
                    break;
                }
            }
            if (ItemPos <= 0) return;

            TreeViewItem AuxItem = ParentLibraryItem.Items[ItemPos - 1] as TreeViewItem;
            ParentLibraryItem.Items.RemoveAt(ItemPos);
            ParentLibraryItem.Items.RemoveAt(ItemPos - 1);
            ParentLibraryItem.Items.Insert(ItemPos - 1, CurrentLibraryItem);
            ParentLibraryItem.Items.Insert(ItemPos, AuxItem);

            CurrentLibraryItem.IsSelected = true;
            SetLibraryAsModified(CurrentLibraryItem);
        }

        /// <summary>
        /// Click event: Move down library tree view item
        /// </summary>
        private void MoveDownLibraryItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (LibraryTreeView.SelectedItem == null) return;

            TreeViewItem CurrentLibraryItem = LibraryTreeView.SelectedItem as TreeViewItem;

            if (CurrentLibraryItem.Parent == null) return;

            TreeViewItem ParentLibraryItem = CurrentLibraryItem.Parent as TreeViewItem;

            int ItemPos = -1;
            for (int n = 0; n < ParentLibraryItem.Items.Count; n++)
            {
                if (ParentLibraryItem.Items[n] == CurrentLibraryItem)
                {
                    ItemPos = n;
                    break;
                }
            }
            if (ItemPos < 0 || ItemPos >= ParentLibraryItem.Items.Count - 1) return;

            TreeViewItem AuxItem = ParentLibraryItem.Items[ItemPos + 1] as TreeViewItem;
            ParentLibraryItem.Items.RemoveAt(ItemPos + 1);
            ParentLibraryItem.Items.RemoveAt(ItemPos);
            ParentLibraryItem.Items.Insert(ItemPos, AuxItem);
            ParentLibraryItem.Items.Insert(ItemPos + 1, CurrentLibraryItem);

            CurrentLibraryItem.IsSelected = true;
            SetLibraryAsModified(CurrentLibraryItem);
        }

        /// <summary>
        /// Context menu opening event: update menu items texts
        /// </summary>
        private void LibraryTreeView_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            MenuItem AuxMenuItem;
            for (int n = 0; n < LibraryTreeView.ContextMenu.Items.Count; n++)
            {
                AuxMenuItem = LibraryTreeView.ContextMenu.Items[n] as MenuItem;

                if (AuxMenuItem == null) continue;

                switch (AuxMenuItem.Name)
                {
                    case "LibraryAddFolderMenuItem":
                        AuxMenuItem.Header = AMTexts.Message("LibraryAddFolderMenuItemText");
                        break;
                    case "LibraryRenameLibraryMenuItem":
                        AuxMenuItem.Header = AMTexts.Message("LibraryRenameLibraryMenuItemText");
                        break;
                    case "LibraryEditLibraryDescriptionMenuItem":
                        AuxMenuItem.Header = AMTexts.Message("LibraryEditLibraryDescriptionMenuItemText");
                        break;
                    case "LibrarySaveLibraryMenuItem":
                        AuxMenuItem.Header = AMTexts.Message("LibrarySaveLibraryMenuItemText");
                        break;
                    case "LibraryDeleteLibraryMenuItem":
                        AuxMenuItem.Header = AMTexts.Message("LibraryDeleteLibraryMenuItemText");
                        break;
                    case "LibraryAddAlgorithmMenuItem":
                        AuxMenuItem.Header = AMTexts.Message("LibraryAddAlgorithmMenuItemText");
                        break;
                    case "LibraryPasteAlgorithmMenuItem":
                        AuxMenuItem.Header = AMTexts.Message("LibraryPasteAlgorithmMenuItemText");
                        break;
                    case "LibraryRenameFolderMenuItem":
                        AuxMenuItem.Header = AMTexts.Message("LibraryRenameFolderMenuItemText");
                        break;
                    case "LibraryEditFolderDescriptionMenuItem":
                        AuxMenuItem.Header = AMTexts.Message("LibraryEditFolderDescriptionMenuItemText");
                        break;
                    case "LibraryDeleteFolderMenuItem":
                        AuxMenuItem.Header = AMTexts.Message("LibraryDeleteFolderMenuItemText");
                        break;
                    case "LibraryOpenAlgorithmMenuItem":
                        AuxMenuItem.Header = AMTexts.Message("LibraryOpenAlgorithmMenuItemText");
                        break;
                    case "LibraryRenameAlgorithmMenuItem":
                        AuxMenuItem.Header = AMTexts.Message("LibraryRenameAlgorithmMenuItemText");
                        break;
                    case "LibraryEditAlgorithmDescriptionMenuItem":
                        AuxMenuItem.Header = AMTexts.Message("LibraryEditAlgorithmDescriptionMenuItemText");
                        break;
                    case "LibraryCopyAlgorithmMenuItem":
                        AuxMenuItem.Header = AMTexts.Message("LibraryCopyAlgorithmMenuItemText");
                        break;
                    case "LibraryDeleteAlgorithmMenuItem":
                        AuxMenuItem.Header = AMTexts.Message("LibraryDeleteAlgorithmMenuItemText");
                        break;
                }
            }
        }

        #endregion Library tree view events

        #region LibraryInitImage3D events

        /// <summary>
        /// Library initial position cube 3D size changed event
        /// </summary>
        private void LibraryInitImage3D_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            LibraryData.InitScrambleCube.UpdateBitmap((int)LibraryInitImage3D.ActualWidth, (int)LibraryInitImage3D.ActualHeight);
            LibraryInitImage3D.Source = LibraryData.InitScrambleCube.renderBMP;
        }

        /// <summary>
        /// Mouse down event: start / stop move in 3D view
        /// </summary>
        private void LibraryInitImage3D_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (LibraryData.MovingScrambleCube)
            {
                LibraryData.MovingScrambleCube = false;
                Cursor = Cursors.Arrow;
                if (LibraryData.CurrentAlgorithm != null &&
                    LibraryData.CurrentAlgorithm.Tag != null &&
                    LibraryData.AlgorithmEditionEnabled)
                {
                    LibraryAlgorithmData AData = LibraryData.CurrentAlgorithm.Tag as LibraryAlgorithmData;
                    AData.S3DViewAlpha = LibraryData.InitScrambleCube.CameraAlpha;
                    AData.S3DViewBeta = LibraryData.InitScrambleCube.CameraBeta;
                    SetLibraryAsModified(LibraryData.CurrentAlgorithm);
                }
            }
            else
            {
                LibraryData.MovingScrambleCube = true;
                Cursor = Cursors.ScrollAll;
            }
        }

        /// <summary>
        /// Mouse leave event: change cursor
        /// </summary>
        private void LibraryInitImage3D_MouseLeave(object sender, MouseEventArgs e)
        {
            if (LibraryData.MovingScrambleCube) Cursor = Cursors.Arrow;
        }

        /// <summary>
        /// Mouse enter event: change cursor
        /// </summary>
        private void LibraryInitImage3D_MouseEnter(object sender, MouseEventArgs e)
        {
            if (LibraryData.MovingScrambleCube) Cursor = Cursors.ScrollAll;
        }

        /// <summary>
        /// Mouse move event: move in 3D view
        /// </summary>
        private void LibraryInitImage3D_MouseMove(object sender, MouseEventArgs e)
        {
            if (LibraryData.MovingScrambleCube)
            {
                double alpha = 360.0 * e.GetPosition(LibraryInitImage3D).X / LibraryInitImage3D.ActualWidth - 90.0;
                double beta = 180.0 * e.GetPosition(LibraryInitImage3D).Y / LibraryInitImage3D.ActualHeight - 90.0;
                LibraryData.InitScrambleCube.SetCamera(LibraryData.InitScrambleCube.CameraDistance, alpha, beta);
                LibraryData.InitScrambleCube.UpdateBitmap((int)LibraryInitImage3D.ActualWidth, (int)LibraryInitImage3D.ActualHeight);
                LibraryInitImage3D.Source = LibraryData.InitScrambleCube.renderBMP;
            }
        }

        /// <summary>
        /// Click event: Plus zoom 3D initial position library view button
        /// </summary>
        private void LibraryInitView3DZoomPlusButton_Click(object sender, RoutedEventArgs e)
        {
            if (!LibraryData.AnimWork.IsBusy)
            {
                LibraryData.InitScrambleCube.SetCamera(LibraryData.InitScrambleCube.CameraDistance * 0.9,
                                                       LibraryData.InitScrambleCube.CameraAlpha,
                                                       LibraryData.InitScrambleCube.CameraBeta);
                LibraryData.InitScrambleCube.UpdateBitmap((int)LibraryInitImage3D.ActualWidth,
                                                          (int)LibraryInitImage3D.ActualHeight);
                LibraryInitImage3D.Source = LibraryData.InitScrambleCube.renderBMP;
            }
        }

        /// <summary>
        /// Click event: Minus zoom 3D initial position library view button
        /// </summary>
        private void LibraryInitView3DZoomMinusButton_Click(object sender, RoutedEventArgs e)
        {
            if (!LibraryData.AnimWork.IsBusy)
            {
                LibraryData.InitScrambleCube.SetCamera(LibraryData.InitScrambleCube.CameraDistance * 1.1,
                                                       LibraryData.InitScrambleCube.CameraAlpha,
                                                       LibraryData.InitScrambleCube.CameraBeta);
                LibraryData.InitScrambleCube.UpdateBitmap((int)LibraryInitImage3D.ActualWidth,
                                                          (int)LibraryInitImage3D.ActualHeight);
                LibraryInitImage3D.Source = LibraryData.InitScrambleCube.renderBMP;
            }
        }

        #endregion LibraryInitImage3D events

        #region LibraryAlgImage3D events

        /// <summary>
        /// Library algorithm cube 3D size changed event
        /// </summary>
        private void LibraryAlgImage3D_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            LibraryData.AlgorithmCube.UpdateBitmap((int)LibraryAlgImage3D.ActualWidth, (int)LibraryAlgImage3D.ActualHeight);
            LibraryAlgImage3D.Source = LibraryData.AlgorithmCube.renderBMP;
        }

        /// <summary>
        /// Mouse down event: start / stop move in 3D view
        /// </summary>
        private void LibraryAlgImage3D_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (LibraryData.MovingAlgorithmCube)
            {
                LibraryData.MovingAlgorithmCube = false;
                Cursor = Cursors.Arrow;
                if (LibraryData.CurrentAlgorithm != null &&
                    LibraryData.CurrentAlgorithm.Tag != null &&
                    LibraryData.AlgorithmEditionEnabled)
                {
                    LibraryAlgorithmData AData = LibraryData.CurrentAlgorithm.Tag as LibraryAlgorithmData;
                    AData.E3DViewAlpha = LibraryData.AlgorithmCube.CameraAlpha;
                    AData.E3DViewBeta = LibraryData.AlgorithmCube.CameraBeta;
                    SetLibraryAsModified(LibraryData.CurrentAlgorithm);
                }
            }
            else
            {
                LibraryData.MovingAlgorithmCube = true;
                Cursor = Cursors.ScrollAll;
            }
        }

        /// <summary>
        /// Mouse leave event: change cursor
        /// </summary>
        private void LibraryAlgImage3D_MouseLeave(object sender, MouseEventArgs e)
        {
            if (LibraryData.MovingAlgorithmCube) Cursor = Cursors.Arrow;
        }

        /// <summary>
        /// Mouse enter event: change cursor
        /// </summary>
        private void LibraryAlgImage3D_MouseEnter(object sender, MouseEventArgs e)
        {
            if (LibraryData.MovingAlgorithmCube) Cursor = Cursors.ScrollAll;
        }

        /// <summary>
        /// Mouse move event: move in 3D view
        /// </summary>
        private void LibraryAlgImage3D_MouseMove(object sender, MouseEventArgs e)
        {
            if (LibraryData.MovingAlgorithmCube)
            {
                double alpha = 360.0 * e.GetPosition(LibraryAlgImage3D).X / LibraryAlgImage3D.ActualWidth - 90.0;
                double beta = 180.0 * e.GetPosition(LibraryAlgImage3D).Y / LibraryAlgImage3D.ActualHeight - 90.0;
                LibraryData.AlgorithmCube.SetCamera(LibraryData.AlgorithmCube.CameraDistance, alpha, beta);
                LibraryData.AlgorithmCube.UpdateBitmap((int)LibraryAlgImage3D.ActualWidth, (int)LibraryAlgImage3D.ActualHeight);
                LibraryAlgImage3D.Source = LibraryData.AlgorithmCube.renderBMP;
            }
        }

        /// <summary>
        /// Click event: Plus zoom 3D algorithm library view button
        /// </summary>
        private void LibraryAlgView3DZoomPlusButton_Click(object sender, RoutedEventArgs e)
        {
            if (!LibraryData.AnimWork.IsBusy)
            {
                LibraryData.AlgorithmCube.SetCamera(LibraryData.AlgorithmCube.CameraDistance * 0.9,
                                                    LibraryData.AlgorithmCube.CameraAlpha,
                                                    LibraryData.AlgorithmCube.CameraBeta);
                LibraryData.AlgorithmCube.UpdateBitmap((int)LibraryAlgImage3D.ActualWidth,
                                                       (int)LibraryAlgImage3D.ActualHeight);
                LibraryAlgImage3D.Source = LibraryData.AlgorithmCube.renderBMP;
            }
        }

        /// <summary>
        /// Click event: Minus zoom 3D algorithm library view button
        /// </summary>
        private void LibraryAlgView3DZoomMinusButton_Click(object sender, RoutedEventArgs e)
        {
            if (!LibraryData.AnimWork.IsBusy)
            {
                LibraryData.AlgorithmCube.SetCamera(LibraryData.AlgorithmCube.CameraDistance * 1.1,
                                                    LibraryData.AlgorithmCube.CameraAlpha,
                                                    LibraryData.AlgorithmCube.CameraBeta);
                LibraryData.AlgorithmCube.UpdateBitmap((int)LibraryAlgImage3D.ActualWidth,
                                                       (int)LibraryAlgImage3D.ActualHeight);
                LibraryAlgImage3D.Source = LibraryData.AlgorithmCube.renderBMP;
            }
        }

        #endregion LibraryAlgImage3D events

        #region Library neutral mask events

        /// <summary>
        /// Click event: Clear library neutral stickers
        /// </summary>
        private void LibraryClearNeutralButton_Click(object sender, RoutedEventArgs e)
        {
            if (LibraryData.CurrentAlgorithm != null && LibraryData.CurrentAlgorithm.Tag != null)
            {
                for (int n = 0; n < LibraryData.InitScrambleCube.NeutralStickers.Length; n++)
                    LibraryData.InitScrambleCube.NeutralStickers[n] = false;
                LibraryData.AlgorithmCube.NeutralMask = LibraryData.InitScrambleCube.NeutralMask;
                UpdateLibraryViews();

                if (LibraryData.AlgorithmEditionEnabled)
                {
                    LibraryAlgorithmData AData = LibraryData.CurrentAlgorithm.Tag as LibraryAlgorithmData;
                    AData.NeutralMask = LibraryData.InitScrambleCube.NeutralMask;
                    SetLibraryAsModified(LibraryData.CurrentAlgorithm);
                }
            }
        }

        /// <summary>
        /// Click event: Invert library neutral stickers
        /// </summary>
        private void LibraryInvertNeutralButton_Click(object sender, RoutedEventArgs e)
        {
            if (LibraryData.CurrentAlgorithm != null && LibraryData.CurrentAlgorithm.Tag != null)
            {
                for (int n = 0; n < LibraryData.InitScrambleCube.NeutralStickers.Length; n++)
                    LibraryData.InitScrambleCube.NeutralStickers[n] = !LibraryData.InitScrambleCube.NeutralStickers[n];
                LibraryData.AlgorithmCube.NeutralMask = LibraryData.InitScrambleCube.NeutralMask;
                UpdateLibraryViews();

                if (LibraryData.AlgorithmEditionEnabled)
                {
                    LibraryAlgorithmData AData = LibraryData.CurrentAlgorithm.Tag as LibraryAlgorithmData;
                    AData.NeutralMask = LibraryData.InitScrambleCube.NeutralMask;
                    SetLibraryAsModified(LibraryData.CurrentAlgorithm);
                }
            }
        }

        /// <summary>
        /// Click event: Copy from editor library neutral stickers
        /// </summary>
        private void LibraryCopyFromEditorNeutralButton_Click(object sender, RoutedEventArgs e)
        {
            if (LibraryData.CurrentAlgorithm != null && LibraryData.CurrentAlgorithm.Tag != null)
            {
                LibraryData.InitScrambleCube.NeutralMask = EditorData.Cube.NeutralMask;
                LibraryData.AlgorithmCube.NeutralMask = EditorData.Cube.NeutralMask;
                UpdateLibraryViews();

                if (LibraryData.AlgorithmEditionEnabled)
                {
                    LibraryAlgorithmData AData = LibraryData.CurrentAlgorithm.Tag as LibraryAlgorithmData;
                    AData.NeutralMask = LibraryData.InitScrambleCube.NeutralMask;
                    SetLibraryAsModified(LibraryData.CurrentAlgorithm);
                }
            }
        }

        #endregion Library neutral mask events

        #region Algorithm edition events

        /// <summary>
        /// Checked event: Change algorithm edition status
        /// </summary>
        private void LibraryAlgorithmEditCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            LibraryData.AlgorithmEditionEnabled = true;
            LibraryEditMenuItem.IsEnabled = true;
        }

        /// <summary>
        /// Unchecked event: Change algorithm edition status
        /// </summary>
        private void LibraryAlgorithmEditCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            LibraryData.AlgorithmEditionEnabled = false;
            LibraryEditMenuItem.IsEnabled = false;
        }

        /// <summary>
        /// Click event: Create library initial position from clipboard
        /// </summary>
        private void LibraryInitPosFromClipboardMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (LibraryData.CurrentAlgorithm != null && LibraryData.CurrentAlgorithm.Tag != null)
            {
                string scramblefromclipboard = null;

                try { scramblefromclipboard = Clipboard.GetText(); }
                catch { }

                if (string.IsNullOrWhiteSpace(scramblefromclipboard)) return;

                if (scramblefromclipboard.Length > 100)
                {
                    if (MessageBox.Show(scramblefromclipboard,
                                        AMTexts.Message("ConfirmScrambleTextMessage"),
                                        MessageBoxButton.YesNo,
                                        MessageBoxImage.Question) != MessageBoxResult.Yes) return;
                }
                LibraryData.InitScramble.ReadScramble(scramblefromclipboard);
                LibraryData.InitScramble = LibraryData.InitScramble.DevelopParentheses(true);
                UpdateLibraryViews();

                if (LibraryData.AlgorithmEditionEnabled)
                {
                    LibraryAlgorithmData AData = LibraryData.CurrentAlgorithm.Tag as LibraryAlgorithmData;
                    AData.InitScramble = LibraryData.InitScramble.ToString();
                    SetLibraryAsModified(LibraryData.CurrentAlgorithm);
                }
            }
        }

        /// <summary>
        /// Click event: Create library initial position from editor
        /// </summary>
        private void LibraryInitPosFromEditorMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (LibraryData.CurrentAlgorithm != null && LibraryData.CurrentAlgorithm.Tag != null)
            {
                LibraryData.InitScramble.Copy(EditorData.Scramble);
                LibraryData.InitScramble = LibraryData.InitScramble.DevelopParentheses(true);
                UpdateLibraryViews();

                if (LibraryData.AlgorithmEditionEnabled)
                {
                    LibraryAlgorithmData AData = LibraryData.CurrentAlgorithm.Tag as LibraryAlgorithmData;
                    AData.InitScramble = LibraryData.InitScramble.ToString();
                    SetLibraryAsModified(LibraryData.CurrentAlgorithm);
                }
            }
        }

        /// <summary>
        /// Click event: Generates library initial position from library algorithm
        /// </summary>
        private void LibraryGenerateInitPosMenuitem_Click(object sender, RoutedEventArgs e)
        {
            if (LibraryData.CurrentAlgorithm != null && LibraryData.CurrentAlgorithm.Tag != null)
            {
                Scramble ScrAux;
                if (AMSettings.LibraryDevelopParentheses) ScrAux = LibraryData.Algorithm.DevelopParentheses(false);
                else
                {
                    ScrAux = new Scramble();
                    ScrAux.Copy(LibraryData.Algorithm);
                }
                if (AMSettings.LibraryBasicSteps) ScrAux.ToBasicSteps();
                if (AMSettings.LibraryRemoveTurns) ScrAux.RemoveTurns();
                if (AMSettings.LibraryShrink) while (ScrAux.Shrink()) ;
                if (AMSettings.LibraryInverse) ScrAux.InverseAndReverseScramble();
                ScrAux.Simplify();
                if (AMSettings.LibraryInsertBeginning && !string.IsNullOrWhiteSpace(AMSettings.LibraryStepsToInsert))
                {
                    Scramble ScrToInsert = new Scramble();
                    ScrToInsert.ReadScramble(AMSettings.LibraryStepsToInsert);
                    ScrToInsert.Add(ScrAux);
                    ScrAux = ScrToInsert;
                }

                LibraryData.InitScramble.Copy(ScrAux);
                UpdateLibraryViews();

                if (LibraryData.AlgorithmEditionEnabled)
                {
                    LibraryAlgorithmData AData = LibraryData.CurrentAlgorithm.Tag as LibraryAlgorithmData;
                    AData.InitScramble = LibraryData.InitScramble.ToString();
                    SetLibraryAsModified(LibraryData.CurrentAlgorithm);
                }
            }
        }

        /// <summary>
        /// Click event: Create library algorithm from clipboard
        /// </summary>
        private void LibraryAlgFromClipboardMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (LibraryData.CurrentAlgorithm != null && LibraryData.CurrentAlgorithm.Tag != null)
            {
                string algorithmfromclipboard = null;

                try { algorithmfromclipboard = Clipboard.GetText(); }
                catch { }

                if (string.IsNullOrWhiteSpace(algorithmfromclipboard)) return;

                if (algorithmfromclipboard.Length > 100)
                {
                    if (MessageBox.Show(algorithmfromclipboard,
                                        AMTexts.Message("ConfirmScrambleTextMessage"),
                                        MessageBoxButton.YesNo,
                                        MessageBoxImage.Question) != MessageBoxResult.Yes) return;
                }
                LibraryData.Algorithm.ReadScramble(algorithmfromclipboard);
                LibraryData.Algorithm = LibraryData.Algorithm.DevelopParentheses(true);

                UpdateLibraryViews();

                if (LibraryData.AlgorithmEditionEnabled)
                {
                    LibraryAlgorithmData AData = LibraryData.CurrentAlgorithm.Tag as LibraryAlgorithmData;
                    AData.Algorithm = LibraryData.Algorithm.ToString();
                    SetLibraryAsModified(LibraryData.CurrentAlgorithm);
                }
            }
        }

        /// <summary>
        /// Click event: Create library algorithm from editor
        /// </summary>
        private void LibraryAlgFromEditorMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (LibraryData.CurrentAlgorithm != null && LibraryData.CurrentAlgorithm.Tag != null)
            {
                LibraryData.Algorithm.Copy(EditorData.Scramble);
                LibraryData.Algorithm = LibraryData.Algorithm.DevelopParentheses(true);
                UpdateLibraryViews();

                if (LibraryData.AlgorithmEditionEnabled)
                {
                    LibraryAlgorithmData AData = LibraryData.CurrentAlgorithm.Tag as LibraryAlgorithmData;
                    AData.Algorithm = LibraryData.Algorithm.ToString();
                    SetLibraryAsModified(LibraryData.CurrentAlgorithm);
                }
            }
        }

        /// <summary>
        /// Click event: Send library initial position to clipboard
        /// </summary>
        private void LibraryInitPosToClipboardMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (LibraryData.CurrentAlgorithm != null && LibraryData.CurrentAlgorithm.Tag != null)
            {
                try { Clipboard.SetText(LibraryData.InitScramble.ToString()); }
                catch (Exception ex) { AMSettings.Log = "Error setting text into clipboard: " + ex.Message; }
            }
        }

        /// <summary>
        /// Click event: Send library initial position to editor
        /// </summary>
        private void LibraryInitPosToEditorMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (LibraryData.CurrentAlgorithm != null && LibraryData.CurrentAlgorithm.Tag != null)
            {
                EditorData.Scramble.Copy(LibraryData.InitScramble);

                AddScrambleToUndoList();
                UpdateEditorViews();

                EditorData.ParenthesesNest = EditorData.Scramble.NestValue;
                CloseParenthesisButton.IsEnabled = EditorData.ParenthesesNest > 0;
            }
        }

        /// <summary>
        /// Click event: Send library algorithm to clipboard
        /// </summary>
        private void LibraryAlgToClipboardMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (LibraryData.CurrentAlgorithm != null && LibraryData.CurrentAlgorithm.Tag != null)
            {
                try { Clipboard.SetText(LibraryData.Algorithm.ToString()); }
                catch (Exception ex) { AMSettings.Log = "Error setting text into clipboard: " + ex.Message; }
            }
        }

        /// <summary>
        /// Click event: Send library algorithm to editor
        /// </summary>
        private void LibraryAlgToEditorMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (LibraryData.CurrentAlgorithm != null && LibraryData.CurrentAlgorithm.Tag != null)
            {
                EditorData.Scramble.Copy(LibraryData.Algorithm);

                AddScrambleToUndoList();
                UpdateEditorViews();

                EditorData.ParenthesesNest = EditorData.Scramble.NestValue;
                CloseParenthesisButton.IsEnabled = EditorData.ParenthesesNest > 0;
            }
        }

        /// <summary>
        /// Click event: Copy current neutral mask to editor tab
        /// </summary>
        private void LibraryMaskToEditorMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (LibraryData.CurrentAlgorithm != null && LibraryData.CurrentAlgorithm.Tag != null)
            {
                EditorData.Cube.NeutralMask = LibraryData.AlgorithmCube.NeutralMask;
                UpdateEditorViews();
            }
        }

        /// <summary>
        /// Click event: rotate x algorithm
        /// </summary>
        private void LibraryAlgorithmXButton_Click(object sender, RoutedEventArgs e)
        {
            // Check parentheses consistency
            if (!LibraryData.Algorithm.AreParenthesesOK ||
                !LibraryData.AlgorithmBase.AreParenthesesOK ||
                !LibraryData.InitScramble.AreParenthesesOK) // No consistency
            {
                MessageBox.Show(AMTexts.Message("CantApplyTurnInAlgorithmsMessage"), "", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            LibraryData.Algorithm.ApplyTurnx(0, LibraryData.Algorithm.Length - 1);
            LibraryData.AlgorithmBase.ApplyTurnx(0, LibraryData.AlgorithmBase.Length - 1);
            LibraryData.InitScramble.ApplyTurnx(0, LibraryData.InitScramble.Length - 1);

            LibraryData.AlgorithmCube.RotateNeutral(Steps.x);
            LibraryData.InitScrambleCube.RotateNeutral(Steps.x);

            UpdateLibraryViews();

            if (LibraryData.AlgorithmEditionEnabled)
            {
                LibraryAlgorithmData AData = LibraryData.CurrentAlgorithm.Tag as LibraryAlgorithmData;
                AData.InitScramble = LibraryData.InitScramble.ToString();
                AData.Algorithm = LibraryData.Algorithm.ToString();
                SetLibraryAsModified(LibraryData.CurrentAlgorithm);
            }
        }

        /// <summary>
        /// Click event: rotate y algorithm
        /// </summary>
        private void LibraryAlgorithmYButton_Click(object sender, RoutedEventArgs e)
        {
            // Check parentheses consistency
            if (!LibraryData.Algorithm.AreParenthesesOK ||
                !LibraryData.AlgorithmBase.AreParenthesesOK ||
                !LibraryData.InitScramble.AreParenthesesOK) // No consistency
            {
                MessageBox.Show(AMTexts.Message("CantApplyTurnInAlgorithmsMessage"), "", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            LibraryData.Algorithm.ApplyTurny(0, LibraryData.Algorithm.Length - 1);
            LibraryData.AlgorithmBase.ApplyTurny(0, LibraryData.AlgorithmBase.Length - 1);
            LibraryData.InitScramble.ApplyTurny(0, LibraryData.InitScramble.Length - 1);

            LibraryData.AlgorithmCube.RotateNeutral(Steps.y);
            LibraryData.InitScrambleCube.RotateNeutral(Steps.y);

            UpdateLibraryViews();

            if (LibraryData.AlgorithmEditionEnabled)
            {
                LibraryAlgorithmData AData = LibraryData.CurrentAlgorithm.Tag as LibraryAlgorithmData;
                AData.InitScramble = LibraryData.InitScramble.ToString();
                AData.Algorithm = LibraryData.Algorithm.ToString();
                SetLibraryAsModified(LibraryData.CurrentAlgorithm);
            }
        }

        /// <summary>
        /// Click event: rotate z algorithm
        /// </summary>
        private void LibraryAlgorithmZButton_Click(object sender, RoutedEventArgs e)
        {
            // Check parentheses consistency
            if (!LibraryData.Algorithm.AreParenthesesOK ||
                !LibraryData.AlgorithmBase.AreParenthesesOK ||
                !LibraryData.InitScramble.AreParenthesesOK) // No consistency
            {
                MessageBox.Show(AMTexts.Message("CantApplyTurnInAlgorithmsMessage"), "", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            LibraryData.Algorithm.ApplyTurnz(0, LibraryData.Algorithm.Length - 1);
            LibraryData.AlgorithmBase.ApplyTurnz(0, LibraryData.AlgorithmBase.Length - 1);
            LibraryData.InitScramble.ApplyTurnz(0, LibraryData.InitScramble.Length - 1);

            LibraryData.AlgorithmCube.RotateNeutral(Steps.z);
            LibraryData.InitScrambleCube.RotateNeutral(Steps.z);

            UpdateLibraryViews();

            if (LibraryData.AlgorithmEditionEnabled)
            {
                LibraryAlgorithmData AData = LibraryData.CurrentAlgorithm.Tag as LibraryAlgorithmData;
                AData.InitScramble = LibraryData.InitScramble.ToString();
                AData.Algorithm = LibraryData.Algorithm.ToString();
                SetLibraryAsModified(LibraryData.CurrentAlgorithm);
            }
        }

        #endregion Algorithm edition events

        #region Other library events

        /// <summary>
        /// Selection changed event: Algorithm tab
        /// </summary>
        private void LibraryAlgorithmTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LibraryData.CurrentAlgorithm != null &&
                LibraryData.CurrentAlgorithm.Tag != null &&
                LibraryData.AlgorithmEditionEnabled &&
                LibraryAlgorithmTabControl.SelectedIndex >= 0 &&
                LibraryAlgorithmTabControl.SelectedIndex < LibraryTabChars.Length)
            {
                LibraryAlgorithmData AData = LibraryData.CurrentAlgorithm.Tag as LibraryAlgorithmData;
                if (LibraryTabChars[LibraryAlgorithmTabControl.SelectedIndex] != AData.EndView)
                {
                    AData.EndView = LibraryTabChars[LibraryAlgorithmTabControl.SelectedIndex];
                    SetLibraryAsModified(LibraryData.CurrentAlgorithm);
                }
            }
            UpdateLibraryViews();
        }

        /// <summary>
        /// Selection changed event: Initial position tab
        /// </summary>
        private void LibraryInitialPositionTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LibraryData.CurrentAlgorithm != null &&
                LibraryData.CurrentAlgorithm.Tag != null &&
                LibraryData.AlgorithmEditionEnabled &&
                LibraryInitialPositionTabControl.SelectedIndex >= 0 &&
                LibraryInitialPositionTabControl.SelectedIndex < LibraryTabChars.Length)
            {
                LibraryAlgorithmData AData = LibraryData.CurrentAlgorithm.Tag as LibraryAlgorithmData;
                if (LibraryTabChars[LibraryInitialPositionTabControl.SelectedIndex] != AData.StartView)
                {
                    AData.StartView = LibraryTabChars[LibraryInitialPositionTabControl.SelectedIndex];
                    SetLibraryAsModified(LibraryData.CurrentAlgorithm);
                }
            }
            UpdateLibraryViews();
        }

        /// <summary>
        /// Checked event: Update init scramble 2D views
        /// </summary>
        private void LibraryInitPosShowArrowsCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (LibraryData.CurrentAlgorithm != null &&
                LibraryData.CurrentAlgorithm.Tag != null &&
                LibraryData.AlgorithmEditionEnabled)
            {
                LibraryAlgorithmData AData = LibraryData.CurrentAlgorithm.Tag as LibraryAlgorithmData;
                AData.DrawArrows = true;
                SetLibraryAsModified(LibraryData.CurrentAlgorithm);
            }
            UpdateLibraryInitViewCube2D();
        }

        /// <summary>
        /// Unchecked event: Update init scramble 2D views
        /// </summary>
        private void LibraryInitPosShowArrowsCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (LibraryData.CurrentAlgorithm != null &&
                LibraryData.CurrentAlgorithm.Tag != null &&
                LibraryData.AlgorithmEditionEnabled)
            {
                LibraryAlgorithmData AData = LibraryData.CurrentAlgorithm.Tag as LibraryAlgorithmData;
                AData.DrawArrows = false;
                SetLibraryAsModified(LibraryData.CurrentAlgorithm);
            }
            UpdateLibraryInitViewCube2D();
        }

        /// <summary>
        /// Click event: Library initial position cube 2D buttons (asigned programatically)
        /// </summary>
        private void LibraryInitPosCube2DButtons_Click(object sender, EventArgs e)
        {
            if (LibraryData.CurrentAlgorithm != null &&
                LibraryData.CurrentAlgorithm.Tag != null)
            {
                Button B = sender as Button;

                StickerPositions SP = LibraryData.InitScrambleCube.Cube.GetStickerSolvedPosition((StickerPositions)B.Tag);
                LibraryData.InitScrambleCube.NeutralStickers[(int)SP] =
                    !LibraryData.InitScrambleCube.NeutralStickers[(int)SP];
                LibraryData.InitScrambleCube.UpdateMaterials();
                LibraryData.AlgorithmCube.NeutralMask = LibraryData.InitScrambleCube.NeutralMask; // Updates materials

                UpdateLibraryViews();
                if (LibraryData.AlgorithmEditionEnabled)
                {
                    LibraryAlgorithmData AData = LibraryData.CurrentAlgorithm.Tag as LibraryAlgorithmData;
                    AData.NeutralMask = LibraryData.InitScrambleCube.NeutralMask;
                    SetLibraryAsModified(LibraryData.CurrentAlgorithm);
                }
            }
        }

        /// <summary>
        /// Click event: Library algorithm cube 2D buttons (asigned programatically)
        /// </summary>
        private void LibraryAlgorithmCube2DButtons_Click(object sender, EventArgs e)
        {
            // Nothing to do
        }

        #endregion Other library events

        #endregion Library events

        #region Settings functions

        /// <summary>
        /// Updates the settings tab controls with current settings
        /// </summary>
        private void UpdateSettingsViews()
        {
            SettingsUpColorPicker.SelectedColor = AMSettings.UColor;
            SettingsDownColorPicker.SelectedColor = AMSettings.DColor;
            SettingsFrontColorPicker.SelectedColor = AMSettings.FColor;
            SettingsBackColorPicker.SelectedColor = AMSettings.BColor;
            SettingsRightColorPicker.SelectedColor = AMSettings.RColor;
            SettingsLeftColorPicker.SelectedColor = AMSettings.LColor;
            SettingsNeutralColorPicker.SelectedColor = AMSettings.NeutralColor;
            SettingsBaseColorPicker.SelectedColor = AMSettings.BaseColor;
            SettingsBackgroundColorPicker.SelectedColor = AMSettings.BackgroundColor;
            SettingsArrowsColorPicker.SelectedColor = AMSettings.ArrowsColor;

            ProgressBarsCanvas.Background = ProgressChartCanvas.Background = AMSettings.GetBrush(AMSettings.ChartBackgroundColor);

            ProgressMaximumColorPicker.SelectedColor = AMSettings.ChartMaximumColor;
            ProgressMinimumColorPicker.SelectedColor = AMSettings.ChartMinimumColor;
            ProgressAverageColorPicker.SelectedColor = AMSettings.ChartAverageColor;
            ProgressDeviationColorPicker.SelectedColor = AMSettings.ChartDeviationColor;
            ProgressMediumColorPicker.SelectedColor = AMSettings.ChartMediumColor;
            ProgressAmountColorPicker.SelectedColor = AMSettings.ChartAmountColor;

            ProgresSolvesUpDown.Value = AMSettings.MinimumSolvesPeriod;

            SettingsNotationERotationAsUCheckBox.IsChecked = AMSettings.RotationOfEAsU;
            SettingsNotationERotationAsDCheckBox.IsChecked = AMSettings.RotationOfEAsD;
            SettingsNotationSRotationAsFCheckBox.IsChecked = AMSettings.RotationOfSAsF;
            SettingsNotationSRotationAsBCheckBox.IsChecked = AMSettings.RotationOfSAsB;
            SettingsNotationMRotationAsRCheckBox.IsChecked = AMSettings.RotationOfMAsR;
            SettingsNotationMRotationAsLCheckBox.IsChecked = AMSettings.RotationOfMAsL;
            SettingsNotationUseAltwCheckBox.IsChecked = AMSettings.UsingAltwChars;

            SettingsChronBeepCheckBox.IsChecked = AMSettings.Beep;
            SettingsDefaultCubeTextBox.Text = AMSettings.DefaultCube;
            SettingsDefaultCommentTextBox.Text = AMSettings.DefaultComment;
            SettingsChronoAnimSpeedSlider.Value = AMSettings.ChronoAnimTime;
            SettingsChronoConvertToBasicCheckBox.IsChecked = AMSettings.ChronoToBasic;
            SettingsChronoRemoveTurnsCheckBox.IsChecked = AMSettings.ChronoRemoveTurns;
            SettingsChronoShrinkCheckBox.IsChecked = AMSettings.ChronoShrink;

            SettingsEditorAllowCompoundCheckBox.IsChecked = AMSettings.AllowExtendedCompoundMovements;
            SettingsEditorAnimSpeedSlider.Value = AMSettings.EditorAnimTime;

            SettingsLibraryStartDefaultPositionCheckBox.IsChecked = AMSettings.LibraryDefaultPositionStart;
            SettingsLibraryShowParenthesesCheckBox.IsChecked = AMSettings.LibraryShowParentheses;
            SettingsLibraryConvertToBasicCheckBox.IsChecked = AMSettings.LibraryBasicSteps;
            SettingsLibraryRemoveTurnsCheckBox.IsChecked = AMSettings.LibraryRemoveTurns;
            SettingsLibraryShrinkCheckBox.IsChecked = AMSettings.LibraryShrink;
            SettingsLibraryInverseCheckBox.IsChecked = AMSettings.LibraryInverse;
            SettingsLibraryInsetCheckBox.IsChecked = AMSettings.LibraryInsertBeginning;
            SettingsLibraryStepsToInsertTextBox.Text = AMSettings.LibraryStepsToInsert;
            SettingsLibraryStepsToInsertTextBox.IsEnabled = AMSettings.LibraryInsertBeginning;
        }

        #endregion Settings functions

        #region Settings events

        #region Color settings events

        /// <summary>
        /// Set colors to default values
        /// </summary>
        private void SettingsFactoryColorsButton_Click(object sender, RoutedEventArgs e)
        {
            AMSettings.SetDefaultColors();
            UpdateAllViews();
        }

        /// <summary>
        /// Change up face color
        /// </summary>
        private void SettingsUpColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            AMSettings.UColor = (Color)SettingsUpColorPicker.SelectedColor;
            UpdateAllViews();
        }

        /// <summary>
        /// Change down face color
        /// </summary>
        private void SettingsDownColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            AMSettings.DColor = (Color)SettingsDownColorPicker.SelectedColor;
            UpdateAllViews();
        }

        /// <summary>
        /// Change front face color
        /// </summary>
        private void SettingsFrontColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            AMSettings.FColor = (Color)SettingsFrontColorPicker.SelectedColor;
            UpdateAllViews();
        }

        /// <summary>
        /// Change back face color
        /// </summary>
        private void SettingsBackColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            AMSettings.BColor = (Color)SettingsBackColorPicker.SelectedColor;
            UpdateAllViews();
        }

        /// <summary>
        /// Change right face color
        /// </summary>
        private void SettingsRightColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            AMSettings.RColor = (Color)SettingsRightColorPicker.SelectedColor;
            UpdateAllViews();
        }

        /// <summary>
        /// Change left face color
        /// </summary>
        private void SettingsLeftColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            AMSettings.LColor = (Color)SettingsLeftColorPicker.SelectedColor;
            UpdateAllViews();
        }

        /// <summary>
        /// Change neutral color
        /// </summary>
        private void SettingsNeutralColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            AMSettings.NeutralColor = (Color)SettingsNeutralColorPicker.SelectedColor;
            UpdateAllViews();
        }

        /// <summary>
        /// Change base color
        /// </summary>
        private void SettingsBaseColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            AMSettings.BaseColor = (Color)SettingsBaseColorPicker.SelectedColor;
            UpdateAllViews();
        }

        /// <summary>
        /// Change background color
        /// </summary>
        private void SettingsBackgroundColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            AMSettings.BackgroundColor = (Color)SettingsBackgroundColorPicker.SelectedColor;
            UpdateAllViews();
        }

        /// <summary>
        /// Change arrows color
        /// </summary>
        private void SettingsArrowsColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            AMSettings.ArrowsColor = (Color)SettingsArrowsColorPicker.SelectedColor;
            UpdateAllViews();
        }

        #endregion Color settings events

        #region Notation settings events

        /// <summary>
        /// Use alternative 'w' chars in notation
        /// </summary>
        private void SettingsNotationUseAltwCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            AMSettings.EnableUseAltwChars();
            SetEditorButtonsTexts();
            UpdateChronoScramble();
            UpdateEditorScramble();
            UpdateLibraryAlgorithm();
        }

        /// <summary>
        /// Don't use alternative 'w' chars in notation
        /// </summary>
        private void SettingsNotationUseAltwCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            AMSettings.DisableUseAltwChars();
            SetEditorButtonsTexts();
            UpdateChronoScramble();
            UpdateEditorScramble();
            UpdateLibraryAlgorithm();
        }

        /// <summary>
        /// Set E layer rotation as U layer rotation
        /// </summary>
        private void SettingsNotationERotationAsUCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            AMSettings.SetERotDirection(true);
        }

        /// <summary>
        /// Set E layer rotation as D layer rotation
        /// </summary>
        private void SettingsNotationERotationAsUCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            AMSettings.SetERotDirection(false);
        }

        /// <summary>
        /// Set E layer rotation as D layer rotation
        /// </summary>
        private void SettingsNotationERotationAsDCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            AMSettings.SetERotDirection(false);
        }

        /// <summary>
        /// Set E layer rotation as U layer rotation
        /// </summary>
        private void SettingsNotationERotationAsDCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            AMSettings.SetERotDirection(true);
        }

        /// <summary>
        /// Set S layer rotation as F layer rotation
        /// </summary>
        private void SettingsNotationSRotationAsFCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            AMSettings.SetSRotDirection(true);
        }

        /// <summary>
        /// Set S layer rotation as B layer rotation
        /// </summary>
        private void SettingsNotationSRotationAsFCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            AMSettings.SetSRotDirection(false);
        }

        /// <summary>
        /// Set S layer rotation as B layer rotation
        /// </summary>
        private void SettingsNotationSRotationAsBCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            AMSettings.SetSRotDirection(false);
        }

        /// <summary>
        /// Set S layer rotation as F layer rotation
        /// </summary>
        private void SettingsNotationSRotationAsBCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            AMSettings.SetSRotDirection(true);
        }

        /// <summary>
        /// Set M layer rotation as R layer rotation
        /// </summary>
        private void SettingsNotationMRotationAsRCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            AMSettings.SetMRotDirection(true);
        }

        /// <summary>
        /// Set M layer rotation as L layer rotation
        /// </summary>
        private void SettingsNotationMRotationAsRCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            AMSettings.SetMRotDirection(false);
        }

        /// <summary>
        /// Set M layer rotation as L layer rotation
        /// </summary>
        private void SettingsNotationMRotationAsLCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            AMSettings.SetMRotDirection(false);
        }

        /// <summary>
        /// Set M layer rotation as R layer rotation
        /// </summary>
        private void SettingsNotationMRotationAsLCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            AMSettings.SetMRotDirection(true);
        }

        #endregion Notation settings events

        #region Chrono settings events

        /// <summary>
        /// Checked event: enable beep sounds
        /// </summary>
        private void SettingsChronBeepCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            AMSettings.Beep = true;
        }

        /// <summary>
        /// Unchecked event: disable beep sounds
        /// </summary>
        private void SettingsChronBeepCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            AMSettings.Beep = false;
        }

        /// <summary>
        /// Set default cube name for solve results
        /// </summary>
        private void SettingsDefaultCubeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            AMSettings.DefaultCube = SettingsDefaultCubeTextBox.Text;
        }

        /// <summary>
        /// Set default comment for solve results
        /// </summary>
        private void SettingsDefaultCommentTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            AMSettings.DefaultComment = SettingsDefaultCommentTextBox.Text;
        }

        /// <summary>
        /// Set speed (animation time for each movement) for 3D chronometer cube animations
        /// </summary>
        private void SettingsChronoAnimSpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            AMSettings.ChronoAnimTime = (int)SettingsChronoAnimSpeedSlider.Value;
        }

        /// <summary>
        /// When importing an scramble convert it to basic movements
        /// </summary>
        private void SettingsChronoConvertToBasicCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            AMSettings.ChronoToBasic = true;
        }

        /// <summary>
        /// When importing an scramble don't convert it to basic movements
        /// </summary>
        private void SettingsChronoConvertToBasicCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            AMSettings.ChronoToBasic = false;
        }

        /// <summary>
        /// When importing an scramble remove turns
        /// </summary>
        private void SettingsChronoRemoveTurnsCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            AMSettings.ChronoRemoveTurns = true;
        }

        /// <summary>
        /// When importing an scramble don't remove turns
        /// </summary>
        private void SettingsChronoRemoveTurnsCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            AMSettings.ChronoRemoveTurns = false;
        }

        /// <summary>
        /// When importing an scramble shrink it
        /// </summary>
        private void SettingsChronoShrinkCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            AMSettings.ChronoShrink = true;
        }

        /// <summary>
        /// When importing an scramble don't shrink it
        /// </summary>
        private void SettingsChronoShrinkCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            AMSettings.ChronoShrink = false;
        }

        #endregion Chrono settings events

        #region Progress settings events

        /// <summary>
        /// Update minimum solves to draw
        /// </summary>
        private void ProgresSolvesUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            AMSettings.MinimumSolvesPeriod = (int)ProgresSolvesUpDown.Value;
            UpdateChartData();
        }

        #endregion Progress settings events

        #region Editor settings events

        /// <summary>
        /// Allow compound movements in editor
        /// </summary>
        private void SettingsEditorAllowCompoundCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            AMSettings.AllowExtendedCompoundMovements = true;
            UpdateEditorModifierCombo();
        }

        /// <summary>
        /// Don't allow compound movements in editor
        /// </summary>
        private void SettingsEditorAllowCompoundCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            AMSettings.AllowExtendedCompoundMovements = false;
            UpdateEditorModifierCombo();
        }

        /// <summary>
        /// Set speed (animation time for each movement) for 3D editor cube animations
        /// </summary>
        private void SettingsEditorAnimSpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            AMSettings.EditorAnimTime = (int)SettingsEditorAnimSpeedSlider.Value;
        }

        #endregion Editor settings events

        #region Library settings events

        /// <summary>
        /// When loading an algorithm show it in the start position
        /// </summary>
        private void SettingsLibraryStartDefaultPositionCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            AMSettings.LibraryDefaultPositionStart = true;
        }

        /// <summary>
        /// When loading an algorithm show it in the end position
        /// </summary>
        private void SettingsLibraryStartDefaultPositionCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            AMSettings.LibraryDefaultPositionStart = false;
        }

        /// <summary>
        /// Allow to show parentheses in main algorithm view
        /// </summary>
        private void SettingsLibraryShowParenthesesCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            AMSettings.LibraryShowParentheses = true;
        }

        /// <summary>
        /// Don't allow to show parentheses in main algorithm view
        /// </summary>
        private void SettingsLibraryShowParenthesesCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            AMSettings.LibraryShowParentheses = false;
        }

        /// <summary>
        /// When creating initial position scramble from algorithm, convert it to basic steps
        /// </summary>
        private void SettingsLibraryConvertToBasicCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            AMSettings.LibraryBasicSteps = true;
        }

        /// <summary>
        /// When creating initial position scramble from algorithm, don't convert it to basic steps
        /// </summary>
        private void SettingsLibraryConvertToBasicCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            AMSettings.LibraryBasicSteps = false;
        }

        /// <summary>
        /// When creating initial position scramble from algorithm, remove turns
        /// </summary>
        private void SettingsLibraryRemoveTurnsCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            AMSettings.LibraryRemoveTurns = true;
        }

        /// <summary>
        /// When creating initial position scramble from algorithm, don't remove turns
        /// </summary>
        private void SettingsLibraryRemoveTurnsCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            AMSettings.LibraryRemoveTurns = false;
        }

        /// <summary>
        /// When creating initial position scramble from algorithm, shrink it
        /// </summary>
        private void SettingsLibraryShrinkCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            AMSettings.LibraryShrink = true;
        }

        /// <summary>
        /// When creating initial position scramble from algorithm, don't shrink it
        /// </summary>
        private void SettingsLibraryShrinkCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            AMSettings.LibraryShrink = false;
        }

        /// <summary>
        /// When creating initial position scramble from algorithm, inverse algorithm steps
        /// </summary>
        private void SettingsLibraryInverseCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            AMSettings.LibraryInverse = true;
        }

        /// <summary>
        /// When creating initial position scramble from algorithm, don't inverse algorithm steps
        /// </summary>
        private void SettingsLibraryInverseCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            AMSettings.LibraryInverse = false;
        }

        /// <summary>
        /// When creating initial position scramble from algorithm, insert given movements at beginning
        /// </summary>
        private void SettingsLibraryInsetCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            AMSettings.LibraryInsertBeginning = true;
            SettingsLibraryStepsToInsertTextBox.IsEnabled = true;
        }

        /// <summary>
        /// When creating initial position scramble from algorithm, don't insert any movements at beginning
        /// </summary>
        private void SettingsLibraryInsetCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            AMSettings.LibraryInsertBeginning = false;
            SettingsLibraryStepsToInsertTextBox.IsEnabled = false;
        }

        /// <summary>
        /// If selected, movements to insert when creating initial position scramble from algorithm
        /// </summary>
        private void SettingsLibraryStepsToInsertTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            AMSettings.LibraryStepsToInsert = SettingsLibraryStepsToInsertTextBox.Text;
        }

        #endregion Library settings events

        #region Folder buttons events

        /// <summary>
        /// Open language files folder 
        /// </summary>
        private void SettingsLanguageFolderButton_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(AMSettings.LangFolder))
            {
                Process process = new Process();
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.FileName = AMSettings.LangFolder;
                process.Start();
            }
        }

        /// <summary>
        /// Open settings files folder 
        /// </summary>
        private void SettingsFolderButton_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(AMSettings.SettingsFolder))
            {
                Process process = new Process();
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.FileName = AMSettings.SettingsFolder;
                process.Start();
            }
        }

        /// <summary>
        /// Open solves files folder 
        /// </summary>
        private void SettingsSolvesFolderButton_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(AMSettings.SolvesFolder))
            {
                Process process = new Process();
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.FileName = AMSettings.SolvesFolder;
                process.Start();
            }
        }

        /// <summary>
        /// Open libraries files folder 
        /// </summary>
        private void SettingsLibrariesFolderButton_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(AMSettings.LibsFolder))
            {
                Process process = new Process();
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.FileName = AMSettings.LibsFolder;
                process.Start();
            }
        }

        /// <summary>
        /// Open video folder
        /// </summary>
        private void SettingsVideoFolderButton_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(ChronoData.LastVideoFolder))
            {
                Process process = new Process();
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.FileName = ChronoData.LastVideoFolder;
                process.Start();
            }
        }

        /// <summary>
        /// Show README files
        /// </summary>
        private void SettingsReadmeButton_Click(object sender, RoutedEventArgs e)
        {
            ReadmeWindow Readme = new ReadmeWindow()
            {
                Owner = this
            };
            Readme.Show();
        }

        #endregion Folder buttons events

        #region Settings & language files control events

        /// <summary>
        /// Load language file
        /// </summary>
        private void SettingsLanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SettingsLanguageComboBox.SelectedItem != null &&
                AMTexts.LoadTexts(SettingsLanguageComboBox.SelectedItem.ToString()))
            {
                AMSettings.CurrentLanguageFile = SettingsLanguageComboBox.SelectedItem.ToString();
                UpdateTexts();
                AMSettings.Status = AMTexts.Message("LanguageFileLoadedMessage") + AMSettings.CurrentLanguageFile;
            }
        }

        /// <summary>
        /// Show translation tool modal window
        /// </summary>
        private void SettingsLanguageToolButton_Click(object sender, RoutedEventArgs e)
        {
            AlgorithmMasterWindow.Hide();
            TranslationTool TransWindow = new TranslationTool();
            TransWindow.ShowDialog();
            AlgorithmMasterWindow.Show();
            UpdateLanguageFiles();
        }

        /// <summary>
        /// Enable load settings button
        /// </summary>
        private void SettingsLoadComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SettingsLoadButton.IsEnabled = SettingsLoadComboBox.SelectedIndex >= 0;
        }

        /// <summary>
        /// Load settings and update views and texts
        /// </summary>
        private void SettingsLoadButton_Click(object sender, RoutedEventArgs e)
        {
            if (SettingsLoadComboBox.SelectedItem != null &&
                AMSettings.LoadXML(System.IO.Path.Combine(AMSettings.SettingsFolder,
                                                          SettingsLoadComboBox.SelectedItem.ToString())))
            {
                UpdateAllViews();
                UpdateTexts();
                UpdateLanguageFiles();
                AMSettings.Status = AMTexts.Message("SettingsFileLoadedMessage") +
                                    SettingsLoadComboBox.SelectedItem.ToString();
            }
        }

        /// <summary>
        /// Check validity of settings file to save
        /// </summary>
        private void SettingsSaveComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SettingsSaveComboBox.SelectedIndex == 0)
            {
                SettingsSaveTextBox.IsEnabled = true;
                bool isValid = !string.IsNullOrEmpty(SettingsSaveTextBox.Text) &&
                               SettingsSaveTextBox.Text.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) < 0 &&
                               !System.IO.File.Exists(System.IO.Path.Combine(AMSettings.SettingsFolder,
                                                                             SettingsSaveTextBox.Text + AMSettings.SettingsExt));

                SettingsSaveButton.IsEnabled = isValid;
                SettingsDeleteButton.IsEnabled = false;
            }
            else if (SettingsSaveComboBox.SelectedIndex > 0)
            {
                SettingsSaveButton.IsEnabled = true;
                SettingsSaveTextBox.IsEnabled = false;
                SettingsDeleteButton.IsEnabled = true;
            }
        }

        /// <summary>
        /// Enable save settings button
        /// </summary>
        private void SettingsSaveTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool isValid = !string.IsNullOrEmpty(SettingsSaveTextBox.Text) &&
                           SettingsSaveTextBox.Text.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) < 0 &&
                           !System.IO.File.Exists(System.IO.Path.Combine(AMSettings.SettingsFolder,
                                                                         SettingsSaveTextBox.Text + AMSettings.SettingsExt));
            if (isValid && SettingsSaveComboBox.SelectedIndex == 0) SettingsSaveButton.IsEnabled = true;
            else SettingsSaveButton.IsEnabled = false;
        }

        /// <summary>
        /// Save settings file
        /// </summary>
        private void SettingsSaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (SettingsSaveComboBox.SelectedIndex > 0) // Overwrite settings file
            {
                if (MessageBox.Show(string.Format(AMTexts.Message("SettingsFileConfirmOverwriteTextMessage"),
                                                  SettingsSaveComboBox.SelectedItem.ToString()),
                                    AMTexts.Message("SettingsFileConfirmOverwriteHeaderMessage"),
                                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        string SettingsFile = System.IO.Path.Combine(AMSettings.SettingsFolder,
                                                                     SettingsSaveComboBox.SelectedItem.ToString());
                        AMSettings.SaveXML(SettingsFile);
                        AMSettings.Status = AMTexts.Message("SettingsFileSavedMessage") +
                                            SettingsSaveComboBox.SelectedItem.ToString();
                    }
                    catch (Exception ex) { AMSettings.Log = "Settings file save fault: " + ex.Message; }
                }
            }
            else if (SettingsSaveComboBox.SelectedIndex == 0) // New settings file
            {
                try
                {
                    AMSettings.SaveXML(System.IO.Path.Combine(AMSettings.SettingsFolder,
                                                              SettingsSaveTextBox.Text + AMSettings.SettingsExt));
                    AMSettings.Status = AMTexts.Message("SettingsFileSavedMessage") + 
                                        SettingsSaveTextBox.Text + AMSettings.SettingsExt;
                    UpdateSettingsFiles();
                    SettingsSaveTextBox.Text = string.Empty;
                }
                catch (Exception ex) { AMSettings.Log = "Settings file save fault: " + ex.Message; }
            }
        }

        /// <summary>
        /// Delete settings file
        /// </summary>
        private void SettingsDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(string.Format(AMTexts.Message("SettingsFileConfirmDeleteTextMessage"),
                                              SettingsSaveComboBox.SelectedItem.ToString()),
                                AMTexts.Message("SettingsFileConfirmDeleteHeaderMessage"),
                                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    System.IO.File.Delete(System.IO.Path.Combine(AMSettings.SettingsFolder,
                                                                 SettingsSaveComboBox.SelectedItem.ToString()));
                    AMSettings.Status = AMTexts.Message("SettingsFileDeletedMessage") + SettingsSaveComboBox.SelectedItem.ToString();
                }
                catch (Exception ex) { AMSettings.Log = "Fault deleting settings file: " + ex.Message; }
                UpdateSettingsFiles();
            }
        }

        #endregion Settings & language files control events

        #endregion Settings events
    }
}
