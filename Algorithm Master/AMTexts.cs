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
using System.Text;
using System.Windows.Controls;
using System.Xml;

namespace Algorithm_Master
{
    /// <summary>
    /// Static class to manage Algorithm Master texts
    /// </summary>
    public static class AMTexts
    {
        /// <summary>
        /// Dictionary with current texts
        /// </summary>
        private static Dictionary<string, string> CurrentTexts;

        /// <summary>
        /// Dictionary with system texts
        /// </summary>
        public static Dictionary<string, string> SystemTexts;

        /// <summary>
        /// Dictionary of texts to be edited with the translation tool
        /// </summary>
        public static Dictionary<string, string> EditTexts;

        /// <summary>
        /// Dictionary with current messages (texts not associated permanently to an object)
        /// </summary>
        private static Dictionary<string, string> CurrentMessages;

        /// <summary>
        /// Dictionary with system messages (texts not associated permanently to an object)
        /// </summary>
        public static Dictionary<string, string> SystemMessages;

        /// <summary>
        /// Dictionary with messages to be edited (texts not associated permanently to an object)
        /// </summary>
        public static Dictionary<string, string> EditMessages;

        /// <summary>
        /// Array of language files (file name only)
        /// </summary>
        public static string[] LanguageFiles;

        /// <summary>
        /// Constructor, reads avaliable language files 
        /// </summary>
        static AMTexts()
        {
            SetDefaultSystemMessages();
            UpdateLanguageFiles();
        }

        /// <summary>
        /// Updates language files
        /// </summary>
        public static void UpdateLanguageFiles()
        {
            if (System.IO.Directory.Exists(AMSettings.LangFolder))
            {
                System.IO.DirectoryInfo diLang;
                System.IO.FileInfo[] fiLang;
                try
                {
                    diLang = new System.IO.DirectoryInfo(AMSettings.LangFolder);
                    fiLang = diLang.GetFiles("*" + AMSettings.LangExt);
                }
                catch (Exception ex)
                {
                    AMSettings.Status = "Problem reading language folder: " + ex.Message;
                    LanguageFiles = null;
                    return;
                }

                if (fiLang.Length > 0)
                {
                    LanguageFiles = new string[fiLang.Length];
                    for (int n = 0; n < fiLang.Length; n++) LanguageFiles[n] = fiLang[n].Name;
                }
                else LanguageFiles = null;
            }
            else LanguageFiles = null;
        }

        /// <summary>
        /// Sets the default message texts
        /// </summary>
        public static void SetDefaultSystemMessages()
        {
            SystemMessages = new Dictionary<string, string>()
            {
                { "StartUpMessage", "Algorithm Master V1.0" },

                { "ResolutionType00", "WCA Speed solving" }, // Max. 100 types of resolution (from 00 to 99)
                { "ResolutionType01", "WCA Blindfold" },
                { "ResolutionType02", "WCA One-handed" },
                { "ResolutionType03", "WCA With feet" },
                /*
                { "ResolutionType04", "White cross" },
                { "ResolutionType05", "Any cross" },
                { "ResolutionType06", "White cross (down)" },
                { "ResolutionType07", "Any cross (down)" },
                { "ResolutionType08", "F2L" },
                { "ResolutionType09", "Cross + F2L" },
                { "ResolutionType10", "OLL" },
                { "ResolutionType11", "Cross + F2L + OLL" },
                { "ResolutionType12", "PLL" },
                { "ResolutionType13", "OLL + PLL" },
                */
                { "DNFInspectionMessage", "DNF (inspection)" },
                { "DNFSolveMessage", "DNF (solve)" },
                { "DNFMessage", "DNF" },
                { "DNSMessage", "DNS" },
                { "Penalty4sMessage", "+4s (inspection & solve)" },
                { "Penalty2sInspectionMessage", "+2s (inspection)" },
                { "Penalty2sSolveMessage", "+2s (solve)" },
                { "Penalty2sMessage", "+2s penalty" },
                { "NoPenaltyMessage", "No" },

                { "NONEStepMessage", "NONE" },
                { "NewChronoMessage", "New chronometer time saved: " },
                { "EnableChronoMessage", "Click here to enable chronometer!" },
                { "NewVideoMessage", "New video file created: " },
                { "ChronoNewRunMessage", "Press SPACE key for a new run" },
                { "NoChronoSavedMessage", "Chronometer result no saved: cancelled by user" },
                { "NoTimeMessage", "No time" },
                { "PerformingInspectionMessage", "Performing inspection" },
                { "ChronoReadyMessage", "Chrono ready! Release SPACE key to start..." },
                { "KeepPressedMessage", "Keep SPACE key pressed to start chrono..." },
                { "ClickToStopChronoMessage", "Click here to stop chronometer!" },
                { "ClickToEnableChronoMessage", "Click here to enable chronometer!" },
                { "CompletedMeasureMessage", "Completed measure." },
                { "ChronoRunningMessage", "Chrono running! Press SPACE key or click here to stop..." },
                { "KeepSpacePressedMessage", "Keep SPACE key pressed..." },
                { "NewRandomScrambleMessage", "New random scramble created" },
                { "NewScrambleClipboardMessage", "New scramble from clipboard" },
                { "NoScrambleMessage", "Solve without scramble" },
                { "ConfirmScrambleTextMessage", "Please, confirm if clipboard text is an algorithm or scramble..." },

                { "DiscardingLastResultMessage", "Continue discarding last result?" },
                { "LastResultNotSavedMessage", "Last result not saved!" },
                { "CantCreateCSVFileMessage", "Can't new create CSV file: " },
                { "CSVFileCreatedMessage", "CSV data file created: " },
                { "DeleteSolveMessage", "Solve performed at '{0}' will be permanently deleted, are you sure?" },
                { "DeleteSolveTitleMessage", "Please, confirm deletion..." },

                { "SingleLayerModifierMessage", "Single layer" },
                { "AdjacentLayersSameDirectionMessage", "Adjacent layers - same direction" },
                { "AdjacentLayersInverseDirectionMessage", "Adjacent layers - inverse direction" },
                { "OppositeLayersSameDirectionMessage", "Opposite layers  - same direction" },
                { "OppositeLayersInverseDirectionMessage", "Opposite layers - inverse direction" },

                { "CantApplyTurnMessage", "Can't apply turn: parentheses inconsistence inside selection" },
                { "CantCreateNewParenthesesMessage", "Can't create new parentheses: parentheses inconsistence inside selection" },
                { "CantDevelopParenthesesMessage", "Can't develop parentheses: parenthesis inconsistence into selection" },
                { "CantDeleteSingleParenthesesMessage", "Can't delete single parenthesis" },
                { "CantDeleteParenthesisInconsitenceMessage", "Can't delete: parenthesis inconsistence inside selection" },
                { "CantReverseParenthesisInconsistenceMessage", "Can't reverse: parenthesis inconsistence" },
                { "CantReverseParenthesisInconsistenceSelectionMessage", "Can't reverse: parenthesis inconsistence into selection" },

                { "SettingsFileLoadedMessage", "Settings file loaded: " },
                { "SettingsFileSavedMessage", "Settings file saved: " },
                { "DefaultSettingsFileNotFoundMessage", "Default settings file not found: " },
                { "NewSettingsFileComboItemMessage", "New settings file..." },
                { "LanguageFileLoadedMessage", "Language file loaded: " },
                { "SettingsFileDeletedMessage", "Settings file deleted: " },
                { "SettingsFileConfirmDeleteHeaderMessage", "Please confirm deletion..." },
                { "SettingsFileConfirmDeleteTextMessage", "Delete settings file '{0}'?" },
				{ "SettingsFileConfirmOverwriteHeaderMessage", "Please confirm overwriting..." },
                { "SettingsFileConfirmOverwriteTextMessage", "Overwrite settings file '{0}'?" },

                { "TranslationToolTitleMessage", "Algorithm Master translation tool" },
				{ "TranslationToolNoEditionFileMessage", "No edition file selected."}, 
				{ "TranslationToolStadisticsMessage", "File '{0}' - Texts {1} / {2} - Messages {3} / {4}" },
				{ "TranslationToolSaveChangesMessage", "Save changes to file '{0}'?" },
				{ "TranslationToolSaveChangesCaptionMessage", "Changes were made..." },
				{ "LanguageFileConfirmOverwriteHeaderMessage", "Please confirm overwriting..." },
                { "LanguageFileConfirmOverwriteTextMessage", "Overwrite language file '{0}'?" },
                { "LanguageFileConfirmDeleteHeaderMessage", "Please confirm deleting..." },
                { "LanguageFileConfirmDeleteTextMessage", "Delete language file '{0}'?" },
                { "LanguageFileDeletedMessage", "Language file deleted: " },
                { "NewLanguageFileMessage", "New language file created: " },
                { "LanguageFileUpdatedMessage", "Language file updated: " },
				{ "LanguageNewFileMessage", "New file..." },

                { "OpenAlgorithmMessage", "Open an algorithm in the library navigator" },
                { "LibraryUpdatedMessage", "Library updated: " },
                { "RightArrowMessage", " → " },
                { "NewLibraryMessage", "New library..." },
                { "LibraryExistsMessage", "Library '{0}' already exists!" },
                { "LibraryNotCreatedMessage", "Library not created..." },
                { "NewFolderMessage", "New folder..." },
                { "RenameLibraryMessage", "Rename library..." },
                { "LibraryNotRenamedMessage", "Library not renamed!" },
                { "ChangeLibraryDescriptionMessage", "Change library description..." },
                { "LibraryWillBeDeletedMessage", "Library '{0}' will be deleted!" },
                { "PleaseConfirmMessage", "Please confirm..." },
                { "NewAlgorithmMessage", "New algorithm..." },
                { "RenameFolderMessage", "Rename folder..." },
                { "ChangeFolderDescriptionMessage", "Change folder description..." },
                { "FolderWillBeDeletedMessage", "Folder '{0}' will be deleted!" },
                { "RenameAlgorithmMessage", "Rename algorithm..." },
                { "ChangeAlgorithmDescriptionMessage", "Change algorithm description..." },
                { "AlgorithmWillBeDeletedMessage", "Algorithm '{0}' will be deleted!" },
                { "CantApplyTurnInAlgorithmsMessage", "Can't apply turn: parentheses inconsistence inside algorithm(s)" },

                { "LibraryAddFolderMenuItemText", "Add folder..." },
                { "LibraryRenameLibraryMenuItemText", "Rename library..." },
                { "LibraryEditLibraryDescriptionMenuItemText", "Edit library description..." },
                { "LibrarySaveLibraryMenuItemText", "Save library" },
                { "LibraryDeleteLibraryMenuItemText", "Delete library" },
                { "LibraryAddAlgorithmMenuItemText", "Add algorithm..." },
                { "LibraryPasteAlgorithmMenuItemText", "Paste algorithm" },
                { "LibraryRenameFolderMenuItemText", "Rename folder..." },
                { "LibraryEditFolderDescriptionMenuItemText", "Edit folder description..." },
                { "LibraryDeleteFolderMenuItemText", "Delete folder" },
                { "LibraryOpenAlgorithmMenuItemText", "Open algorithm" },
                { "LibraryRenameAlgorithmMenuItemText", "Rename algorithm..." },
                { "LibraryEditAlgorithmDescriptionMenuItemText", "Edit algorithm description..." },
                { "LibraryCopyAlgorithmMenuItemText", "Copy algorithm" },
                { "LibraryDeleteAlgorithmMenuItemText", "Delete algorithm" },

                { "LibraryModifiedMessage", "Library '{0}' was modified..." },
                { "LibrarySaveChangesMessage", "Save changes?" },

                { "SuitableCharactersMessage", "Please, use only characters suitables for file names (don't use ':', '\' or similar)." },
                { "InvalidCharactersMessage", "Invalid characters in name..." },

                { "ChronoTableColumnDateTimeText", "Date & time" },
                { "ChronoTableColumnChronoTimeText", "Solve time" },
                { "ChronoTableColumnPenaltyText", "Penalty" },
                { "ChronoTableColumnScrambleText", "Scramble" },
                { "ChronoTableColumnSolveTypeText", "Solve type" },
                { "ChronoTableColumnCubeText", "Cube" },
                { "ChronoTableColumnCommentText", "Comment" },

                { "ProgressDrawMessage", "Chart drawn with data from '{0}' to '{1}', data grouped {2}." },
                { "ProgressNotEnoughPointsMessage", "Can't draw the chart: there are not enough points" },
                { "ProgressTooMuchPointsMessage", "Can't draw the entire graph, there are too many points: please restrict dates" },

                { "ProgressMaximumCheckBoxText", "Maximum" },
                { "ProgressMaximumCheckBoxToolTipHeader", "Maximum chart" },
                { "ProgressMaximumCheckBoxToolTipBody", "Draw/hide maximum solve times chart" },
                { "ProgressMinimumCheckBoxText", "Minimum" },
                { "ProgressMinimumCheckBoxToolTipHeader", "Minimum chart" },
                { "ProgressMinimumCheckBoxToolTipBody", "Draw/hide minimum solve times chart" },
                { "ProgressAverageCheckBoxText", "Average" },
                { "ProgressAverageCheckBoxToolTipHeader", "Average chart" },
                { "ProgressAverageCheckBoxToolTipBody", "Show/hide average solve times chart" },
                { "ProgressDeviationCheckBoxText", "Standard deviation" },
                { "ProgressDeviationCheckBoxToolTipHeader", "Standard deviation chart" },
                { "ProgressDeviationCheckBoxToolTipBody", "Show/hide standard deviation solve time chart" },
                { "ProgressMediumCheckBoxText", "Medium" },
                { "ProgressMediumCheckBoxToolTipHeader", "Medium chart" },
                { "ProgressMediumCheckBoxToolTipBody", "Show/hide medium solve times chart" },
                { "ProgressBarsCheckBoxText", "Amount of solves" },
                { "ProgressBarsCheckBoxToolTipHeader", "Show amount of solves graph" },
                { "ProgressBarsCheckBoxToolTipBody", "Show / hide the bars graph for amount of solves" },

                { "ProgressDailyComboBoxText", "Daily" },
                { "ProgressWeeklyComboBoxText", "Weekly" },
                { "ProgressMonthlyComboBoxText", "Monthly" },
                { "ProgressAnnuallyComboBoxText", "Annually" },

                { "SolveEditTitleMessage", "Edit cube and comment..." },

                { "NewVideoFileTitle", "Save video file..." },
                { "NewVideoFileName", "New solve" },
                { "NewVideoFileFilter", "AVI video|*.avi" },

                { "NewSearchMessage", "New search..." },
                { "RenameSearchMessage", "Rename search..." },
                { "EditDescriptionSearchMessage", "Edit search description..." },
                { "SearchExistsMessage", "Search '{0}' already exists!" },
                { "SearchNotCreatedMessage", "Search not created..." },
                { "SearchNotRenamedMessage", "Search not renamed..." },
                { "SearchNoResultsMessage", "Search complete - no results found" },
                { "SearchCompleteMessage", "Search complete, {0} results found" },
                { "SearchParametersNoOKMessage", "Incorrect search parameters!" },
                { "SearchTypeCustomMessage", "Custom search" },
                { "SearchTypeUpCrossMessage", "Up cross" },
                { "SearchTypeDownCrossMessage", "Down cross" },
                { "SearchTypeFrontCrossMessage", "Front cross" },
                { "SearchTypeBackCrossMessage", "Back cross" },
                { "SearchTypeRightCrossMessage", "Right cross" },
                { "SearchTypeLeftCrossMessage", "Left cross" },
                { "SearchUpdatedMessage", "Search saved:" },
                { "DeleteSearchQuestionMessage", "Delete search '{0}'?" },
                { "DeleteSearchCaptionMessage", "Confirm search deletion..." },
                { "SearchDeletedMessage", "Search '{0}' deleted." },

                { "SearchRenameMainMenuItemText", "Rename search..." },
                { "SearchEditMainDescriptionMenuItemText", "Edit description..." },
                { "SearchSaveMainMenuItemText", "Save search" },
                { "SearchDeleteMainMenuItemText", "Delete search..." },
                { "SearchRenameDefinitionMenuItemText", "Rename search parameters..." },
                { "SearchEditDefinitionDescriptionMenuItemText", "Edit search parameters description..." },
                { "SearchDeleteDefinitionMenuItemText", "Delete search parameters and results..." },
                { "SearchDeleteResultMenuItemText", "Delete search result" },
                { "SearchDeleteAllResultsButMenuItemText", "Delete all search results but this" },
                { "SearchScrambleTooltipText", " (Scramble)" },

                { "RenameSearchInfoMessage", "Rename search parameters..." },
                { "DeleteInfoQuestionMessage", "Delete search '{0}' and results?" },
                { "DeleteResultCaptionMessage", "Confirm result delete..." },
                { "DeleteResultQuestionMessage", "Delete result '{0}'?" },
                { "ResultDeletedMessage", "Result '{0}' deleted." },
                { "DeleteAllResultsButQuestionMessage", "Delete all results except '{0}'?" },
                { "DeleteAllResultsButCaptionMessage", "Confirm results delete..." },
                { "AllResultsButDeletedMessage", "All results but '{0}' deleted." },

            };

            if (CurrentMessages == null) CurrentMessages = new Dictionary<string, string>();
            else CurrentMessages.Clear();
            foreach (KeyValuePair<string, string> Msg in SystemMessages)
                CurrentMessages.Add(Msg.Key, Msg.Value);
        }

        /// <summary>
        /// Updates the system text dictionary with current asigned texts
        /// </summary>
        /// <param name="TextObjects">System text objects dictionary</param>
        public static void AddToSystemTexts(Dictionary<string, object> TextObjects)
        {
            if (TextObjects == null || TextObjects.Count <= 0) return;

            if (SystemTexts == null) SystemTexts = new Dictionary<string, string>();
            foreach (KeyValuePair<string, object> TextObject in TextObjects)
            {
                if (!SystemTexts.TryGetValue(TextObject.Key, out string val))
                    SystemTexts.Add(TextObject.Key, (TextObject.Value as TextBlock).Text);
            }
        }

        /// <summary>
        /// Copy the system texts to current texts
        /// </summary>
        public static void CopySystemTextsToCurrent()
        {
            if (CurrentTexts == null) CurrentTexts = new Dictionary<string, string>();
            else CurrentTexts.Clear();

            if (SystemTexts == null || SystemTexts.Count <= 0) return;

            foreach (KeyValuePair<string, string> STexts in SystemTexts)
                CurrentTexts.Add(STexts.Key, STexts.Value);
        }

        /// <summary>
        /// Save system texts to language directory
        /// </summary>
        /// <returns>True if file is saved</returns>
        public static bool SaveSystemTexts()
        {
            if (SystemTexts == null || SystemTexts.Count <= 0) return false;
            if (SystemMessages == null || SystemMessages.Count <= 0) return false;

            try
            {
                if (!System.IO.Directory.Exists(AMSettings.LangFolder))
                    System.IO.Directory.CreateDirectory(AMSettings.LangFolder);
            }
            catch (Exception Ex)
            {
                AMSettings.Status = "Language folder problem: " + Ex.Message;
                return false;
            }

            string DefaultLanguagePath = System.IO.Path.Combine(AMSettings.LangFolder, "default" + AMSettings.LangExt);

            try { if (System.IO.File.Exists(DefaultLanguagePath)) System.IO.File.Delete(DefaultLanguagePath); }
            catch (Exception ex)
            {
                AMSettings.Status = "Can't delete language file:" + ex.Message;
                return false;
            }

            XmlDocument XmlLan = new XmlDocument();
            XmlElement RootElement = XmlLan.CreateElement("AMLanguage");

            XmlElement AuxElement;

            foreach (KeyValuePair<string, string> AMText in SystemTexts)
            {
                AuxElement = XmlLan.CreateElement("AMText");
                AuxElement.SetAttribute("id", AMText.Key);
                AuxElement.SetAttribute("text", AMText.Value);
                RootElement.AppendChild(AuxElement);
            }

            foreach (KeyValuePair<string, string> AMMessage in SystemMessages)
            {
                AuxElement = XmlLan.CreateElement("AMMessage");
                AuxElement.SetAttribute("id", AMMessage.Key);
                AuxElement.SetAttribute("message", AMMessage.Value);
                RootElement.AppendChild(AuxElement);
            }

            XmlLan.AppendChild(RootElement);

            try { XmlLan.Save(DefaultLanguagePath); }
            catch (Exception ex)
            {
                AMSettings.Log = string.Format("Error saving language file '{0}': ", DefaultLanguagePath) + ex.Message;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Save current texts to lan directory
        /// </summary>
        /// <param name="filename">Lan file name without extension</param>
        /// <returns>True if file is saved</returns>
        public static bool SaveCurrentTexts(string filename)
        {
            if (CurrentTexts == null || CurrentTexts.Count <= 0) return false;
            if (CurrentMessages == null || CurrentMessages.Count <= 0) return false;

            if (string.IsNullOrWhiteSpace(filename)) return false;

            try
            {
                if (!System.IO.Directory.Exists(AMSettings.LangFolder))
                    System.IO.Directory.CreateDirectory(AMSettings.LangFolder);
            }
            catch (Exception ex)
            {
                AMSettings.Status = "Can't create language folder: " + ex.Message;
                return false;
            }

            string DefaultLanguagePath = System.IO.Path.Combine(AMSettings.LangFolder, filename + AMSettings.LangExt);

            try { if (System.IO.File.Exists(DefaultLanguagePath)) System.IO.File.Delete(DefaultLanguagePath); }
            catch (Exception ex)
            {
                AMSettings.Status = "Can't delete language file:" + ex.Message;
                return false;
            }

            XmlDocument XmlLang = new XmlDocument();
            XmlElement RootElement = XmlLang.CreateElement("AMLanguage");

            XmlElement AuxElement;

            foreach (KeyValuePair<string, string> AMText in SystemTexts)
            {
                AuxElement = XmlLang.CreateElement("AMText");
                AuxElement.SetAttribute("id", AMText.Key);
                if (CurrentTexts.TryGetValue(AMText.Key, out string Txt))
                {
                    AuxElement.SetAttribute("text", Txt);
                    RootElement.AppendChild(AuxElement);
                }
            }

            foreach (KeyValuePair<string, string> AMMessage in SystemMessages)
            {
                AuxElement = XmlLang.CreateElement("AMMessage");
                AuxElement.SetAttribute("id", AMMessage.Key);

                if (CurrentMessages.TryGetValue(AMMessage.Key, out string Msg))
                {
                    AuxElement.SetAttribute("message", Msg);
                    RootElement.AppendChild(AuxElement);
                }
            }

            XmlLang.AppendChild(RootElement);

            try { XmlLang.Save(DefaultLanguagePath); }
            catch (Exception ex)
            {
                AMSettings.Log = string.Format("Error saving language file '{0}': ", DefaultLanguagePath) + ex.Message;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Save edition texts (translations) to lan directory 
        /// </summary>
        /// <param name="filename">Lan file name</param>
        /// <param name="WithExtension">True if file name comes with extension</param>
        /// <returns>True if file is saved</returns>
        public static bool SaveEditTexts(string filename, bool WithExtension)
        {
            if (CurrentTexts == null || CurrentTexts.Count < 0) return false;
            if (CurrentMessages == null || CurrentMessages.Count < 0) return false;

            if (string.IsNullOrWhiteSpace(filename)) return false;

            try
            {
                if (!System.IO.Directory.Exists(AMSettings.LangFolder))
                    System.IO.Directory.CreateDirectory(AMSettings.LangFolder);
            }
            catch (Exception ex)
            {
                AMSettings.Status = "Can't create language folder: " + ex.Message;
                return false;
            }

            string DefaultLanguagePath;
            if (WithExtension) DefaultLanguagePath = System.IO.Path.Combine(AMSettings.LangFolder, filename);
            else DefaultLanguagePath = System.IO.Path.Combine(AMSettings.LangFolder, filename + AMSettings.LangExt);

            try { if (System.IO.File.Exists(DefaultLanguagePath)) System.IO.File.Delete(DefaultLanguagePath); }
            catch (Exception ex)
            {
                AMSettings.Status = "Can't delete language file:" + ex.Message;
                return false;
            }

            XmlDocument XmlLang = new XmlDocument();
            XmlElement RootElement = XmlLang.CreateElement("AMLanguage");

            XmlElement AuxElement;

            foreach (KeyValuePair<string, string> AMText in SystemTexts)
            {
                AuxElement = XmlLang.CreateElement("AMText");
                AuxElement.SetAttribute("id", AMText.Key);
                if (EditTexts.TryGetValue(AMText.Key, out string Txt))
                {
                    AuxElement.SetAttribute("text", Txt);
                    RootElement.AppendChild(AuxElement);
                }
            }

            foreach (KeyValuePair<string, string> AMMessage in SystemMessages)
            {
                AuxElement = XmlLang.CreateElement("AMMessage");
                AuxElement.SetAttribute("id", AMMessage.Key);

                if (EditMessages.TryGetValue(AMMessage.Key, out string Msg))
                {
                    AuxElement.SetAttribute("message", Msg);
                    RootElement.AppendChild(AuxElement);
                }
            }

            XmlLang.AppendChild(RootElement);

            try { XmlLang.Save(DefaultLanguagePath); }
            catch (Exception ex)
            {
                AMSettings.Log = string.Format("Error saving language file '{0}': ", DefaultLanguagePath) + ex.Message;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Load default texts from given file
        /// </summary>
        /// <returns>True if file is loaded</returns>
        public static bool LoadTexts(string LanguageFileName)
        {
            if (string.IsNullOrWhiteSpace(LanguageFileName)) return false;

            string FullFileName = System.IO.Path.Combine(AMSettings.LangFolder, LanguageFileName);

            if (!System.IO.File.Exists(FullFileName)) return false;

            XmlDocument LangFile = new XmlDocument();
            try { LangFile.Load(FullFileName); }
            catch (Exception ex)
            {
                AMSettings.Log = string.Format("Error loading language file '{0}': ", FullFileName) + ex.Message;
                return false;
            }

            if (CurrentTexts == null) CurrentTexts = new Dictionary<string, string>();
            if (CurrentMessages == null) CurrentMessages = new Dictionary<string, string>();

            CurrentTexts.Clear();
            CurrentMessages.Clear();

            try
            {
                foreach (XmlNode Node in LangFile.SelectNodes("/AMLanguage/AMText"))
                    CurrentTexts.Add(Node.Attributes["id"].Value, Node.Attributes["text"].Value);

                foreach (XmlNode Node in LangFile.SelectNodes("/AMLanguage/AMMessage"))
                    CurrentMessages.Add(Node.Attributes["id"].Value, Node.Attributes["message"].Value);
            }
            catch (Exception ex)
            {
                AMSettings.Log = string.Format("Fault reading text file '{0}' - ", FullFileName) + ex.Message;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Load texts for edition from given file
        /// </summary>
        /// <param name="LanguageFileName">Language file to load texts (with extension)</param>
        /// <returns>True if file is loaded</returns>
        public static bool LoadEditTexts(string LanguageFileName)
        {
            if (string.IsNullOrWhiteSpace(LanguageFileName)) return false;

            string FullFileName = System.IO.Path.Combine(AMSettings.LangFolder, LanguageFileName);

            if (!System.IO.File.Exists(FullFileName)) return false;

            XmlDocument LangFile = new XmlDocument();
            try { LangFile.Load(FullFileName); }
            catch (Exception ex)
            {
                AMSettings.Log = string.Format("Error loading language file '{0}': ", FullFileName) + ex.Message;
                return false;
            }

            if (EditTexts == null) EditTexts = new Dictionary<string, string>();
            if (EditMessages == null) EditMessages = new Dictionary<string, string>();

            EditTexts.Clear();
            EditMessages.Clear();

            try
            {
                foreach (XmlNode Node in LangFile.SelectNodes("/AMLanguage/AMText"))
                    EditTexts.Add(Node.Attributes["id"].Value, Node.Attributes["text"].Value);

                foreach (XmlNode Node in LangFile.SelectNodes("/AMLanguage/AMMessage"))
                    EditMessages.Add(Node.Attributes["id"].Value, Node.Attributes["message"].Value);
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Get message text
        /// </summary>
        /// <param name="messageid">Message identifier</param>
        /// <returns>Message text</returns>
        public static string Message(string messageid)
        {
            if (CurrentMessages != null && CurrentMessages.TryGetValue(messageid, out string Msg)) return Msg;
            if (SystemMessages != null && SystemMessages.TryGetValue(messageid, out Msg)) return Msg;
            if (!messageid.StartsWith("ResolutionType")) AMSettings.Log = "Message not found: " + messageid;
            return string.Empty;
        }

        /// <summary>
        /// Get system text
        /// </summary>
        /// <param name="textid">Text identifier</param>
        /// <returns>System text</returns>
        public static string Text(string textid)
        {
            if (CurrentTexts != null && CurrentTexts.TryGetValue(textid, out string Txt)) return Txt;
            if (SystemTexts != null && SystemTexts.TryGetValue(textid, out Txt)) return Txt;
            AMSettings.Log = "Text not found: " + textid;
            return string.Empty;
        }

        /// <summary>
        /// Get edition message text
        /// </summary>
        /// <param name="messageid">Message identifier</param>
        /// <returns>Message text</returns>
        public static string EditMessage(string messageid)
        {
            if (EditMessages != null && EditMessages.TryGetValue(messageid, out string Msg)) return Msg;
            return string.Empty;
        }

        /// <summary>
        /// Get edition text
        /// </summary>
        /// <param name="textid">Text identifier</param>
        /// <returns>System text</returns>
        public static string EditText(string textid)
        {
            if (EditTexts != null && EditTexts.TryGetValue(textid, out string Txt)) return Txt;
            return string.Empty;
        }

        /// <summary>
        /// Get system message text
        /// </summary>
        /// <param name="messageid">Message identifier</param>
        /// <returns>Message text</returns>
        public static string SystemMessage(string messageid)
        {
            if (SystemMessages != null && SystemMessages.TryGetValue(messageid, out string Msg)) return Msg;
            return string.Empty;
        }

        /// <summary>
        /// Get system text
        /// </summary>
        /// <param name="textid">Text identifier</param>
        /// <returns>System text</returns>
        public static string SystemText(string textid)
        {
            if (SystemTexts != null && SystemTexts.TryGetValue(textid, out string Txt)) return Txt;
            return string.Empty;
        }

        /// <summary>
        /// Convert ms to string in chrono format
        /// </summary>
        /// <param name="ms">milliseconds</param>
        /// <returns>String with milliseconds</returns>
        public static string MilliseconsToString(long ms)
        {
            StringBuilder sb = new StringBuilder();
            if (ms / 60000 < 10) sb.Append('0');
            sb.Append(ms / 60000);
            sb.Append(':');
            ms %= 60000;
            if (ms / 1000 < 10) sb.Append('0');
            sb.Append(ms / 1000);
            sb.Append(':');
            ms %= 1000;
            if (ms < 10) sb.Append("00");
            else if (ms < 100) sb.Append('0');
            sb.Append(ms);
            return sb.ToString();
        }

        /// <summary>
        /// Convert string in chrono format (xx:xx:xxx) to milliseconds
        /// </summary>
        /// <param name="t">String with milliseconds</param>
        /// <returns>milliseconds</returns>
        public static long StringToMilliseconds(string t)
        {
            if (t == null || t.Length != 9 || t[2] != ':' || t[5] != ':') return 0; // Format must be **:**:***

            if (!int.TryParse(t.Substring(0, 2), out int aux_ms)) return 0;
            long ms = aux_ms * 60000;
            if (!int.TryParse(t.Substring(3, 2), out aux_ms)) return 0;
            ms += aux_ms * 1000;
            if (!int.TryParse(t.Substring(6, 3), out aux_ms)) return 0;
            return ms + aux_ms;
        }
    }
}


