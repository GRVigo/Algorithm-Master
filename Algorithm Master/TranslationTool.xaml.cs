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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Algorithm_Master
{
    /// <summary>
    /// Lógica de interacción para TranslationTool.xaml
    /// </summary>
    public partial class TranslationTool : Window
    {
        /// <summary>
        /// Dictionary of objects with texts to be translated
        /// </summary>
        private Dictionary<string, object> Translate;

        /// <summary>
        /// To know if a modification has been done
        /// </summary>
        private bool ChangesDoneFlag;

        /// <summary>
        /// File name of current edition file
        /// </summary>
        private string CurrentEditionFileName;

        /// <summary>
        /// Constructor
        /// </summary>
        public TranslationTool()
        {
            InitializeComponent();

            SetTranslate();
            AMTexts.AddToSystemTexts(Translate); // Add translation tool window texts to system texts
            UpdateTexts();

            ChangesDoneFlag = false;
            CurrentEditionFileName = string.Empty;

            UpdateLanguageFiles();
            UpdateStadistics();

            Title = AMTexts.Message("TranslationToolTitleMessage");
        }

        /// <summary>
        /// Set the dictionary of objects (text blocks) that contains texts to be translated
        /// </summary>
        private void SetTranslate()
        {
            Translate = new Dictionary<string, object>
            {
                { "TranslationMainLabelText", TranslationMainLabelText },
                { "LanguageEditLabelText", LanguageEditLabelText },
                { "LanguageEditComboToolTipHeader", LanguageEditComboToolTipHeader },
                { "LanguageEditComboToolTipBody", LanguageEditComboToolTipBody },
                { "LanguageEditNewTextBoxToolTipHeader", LanguageEditNewTextBoxToolTipHeader },
                { "LanguageEditNewTextBoxToolTipBody", LanguageEditNewTextBoxToolTipBody },
                { "LanguageEditSaveButtonText", LanguageEditSaveButtonText },
                { "LanguageEditSaveButtonToolTipHeader", LanguageEditSaveButtonToolTipHeader },
                { "LanguageEditSaveButtonToolTipBody", LanguageEditSaveButtonToolTipBody },
				{ "LanguageEditDeleteButtonText", LanguageEditDeleteButtonText },
				{ "LanguageEditDeleteButtonToolTipHeader", LanguageEditDeleteButtonToolTipHeader },
				{ "LanguageEditDeleteButtonToolTipBody", LanguageEditDeleteButtonToolTipBody },
                { "TranslationTypeLabelText", TranslationTypeLabelText },
                { "LanguageTextComboBoxItemText", LanguageTextComboBoxItemText },
                { "LanguageMessageComboBoxItemText", LanguageMessageComboBoxItemText },
                { "LanguageTypeComboToolTipHeader", LanguageTypeComboToolTipHeader },
                { "LanguageTypeComboToolTipBody", LanguageTypeComboToolTipBody },
                { "TranslationItemLabelText", TranslationItemLabelText },
                { "LanguageItemComboToolTipHeader", LanguageItemComboToolTipHeader },
                { "LanguageItemComboToolTipBody", LanguageItemComboToolTipBody },
                { "LanguageNextButtonText", LanguageNextButtonText },
                { "LanguageNextButtonToolTipHeader", LanguageNextButtonToolTipHeader },
                { "LanguageNextButtonToolTipBody", LanguageNextButtonToolTipBody },
                { "TranslationReferenceLabelText", TranslationReferenceLabelText },
                { "LanguageReferenceTextBoxToolTipHeader", LanguageReferenceTextBoxToolTipHeader },
                { "LanguageReferenceTextBoxToolTipBody", LanguageReferenceTextBoxToolTipBody },
                { "TranslationEditedLabelText", TranslationEditedLabelText },
                { "LanguageEditedTextBoxToolTipHeader", LanguageEditedTextBoxToolTipHeader },
                { "LanguageEditedTextBoxToolTipBody", LanguageEditedTextBoxToolTipBody },
                { "LanguageNextFreeButtonText", LanguageNextFreeButtonText },
                { "LanguageNextFreeButtonToolTipHeader", LanguageNextFreeButtonToolTipHeader },
                { "LanguageNextFreeButtonToolTipBody", LanguageNextFreeButtonToolTipBody },

            };
        }

        /// <summary>
        /// Update translation window texts
        /// </summary>
        public void UpdateTexts()
        {
            foreach (KeyValuePair<string, object> TextObj in Translate)
                (TextObj.Value as TextBlock).Text = AMTexts.Text(TextObj.Key);
        }

        /// <summary>
        /// Updates language files
        /// </summary>
        private void UpdateLanguageFiles()
        {
            AMTexts.UpdateLanguageFiles();
            LanguageEditComboBox.Items.Clear();
            LanguageEditComboBox.Items.Add(AMTexts.Message("LanguageNewFileMessage"));
            if (AMTexts.LanguageFiles != null)
                for (int n = 0; n < AMTexts.LanguageFiles.Length; n++)
                    if (string.Compare(AMTexts.LanguageFiles[n], "default" + AMSettings.LangExt) != 0)
                        LanguageEditComboBox.Items.Add(AMTexts.LanguageFiles[n]);
        }

        /// <summary>
        /// Updates the stadistics label
        /// </summary>
        private void UpdateStadistics()
        {
            if (string.IsNullOrWhiteSpace(CurrentEditionFileName))
                TranslationStadisticsLabelText.Text = AMTexts.Message("TranslationToolNoEditionFileMessage");
            else
            {
                int tt = 0, mt = 0;
                if (AMTexts.EditTexts != null)
                    for (int n = 0; n < AMTexts.EditTexts.Count; n++)
                        if (!string.IsNullOrWhiteSpace(AMTexts.EditTexts.Values.ElementAt(n))) tt++;
                if (AMTexts.EditMessages != null)
                    for (int n = 0; n < AMTexts.EditMessages.Count; n++)
                        if (!string.IsNullOrWhiteSpace(AMTexts.EditMessages.Values.ElementAt(n))) mt++;

                TranslationStadisticsLabelText.Text = string.Format(AMTexts.Message("TranslationToolStadisticsMessage"),
                                                                    CurrentEditionFileName,
                                                                    tt,
                                                                    AMTexts.SystemTexts == null ? 0 : AMTexts.SystemTexts.Count,
                                                                    mt,
                                                                    AMTexts.SystemMessages == null ? 0 : AMTexts.SystemMessages.Count);
            }
        }

        /// <summary>
        /// Selection changed event: Change language file to edit
        /// </summary>
        private void LanguageEditComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
			if (ChangesDoneFlag && !string.IsNullOrWhiteSpace(CurrentEditionFileName) &&
                MessageBox.Show(String.Format(AMTexts.Message("TranslationToolSaveChangesMessage"), CurrentEditionFileName),
                                AMTexts.Message("TranslationToolSaveChangesCaptionMessage"),
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                AMTexts.SaveEditTexts(CurrentEditionFileName, true);
            }

            if (LanguageEditComboBox.SelectedIndex < 0) // No language file selected for edition
			{
				CurrentEditionFileName = string.Empty;
				ChangesDoneFlag = false;
				AMTexts.EditMessages = null;
                AMTexts.EditTexts = null;
				LanguageEditNewTextBox.IsEnabled = false;
				LanguageEditSaveButton.IsEnabled = false;	
				LanguageEditDeleteButton.IsEnabled = false;
				LanguageTypeComboBox.IsEnabled = false;
				LanguageTypeComboBox.SelectedIndex = -1;
				LanguageItemComboBox.IsEnabled = false;
				LanguageItemComboBox.Items.Clear();
				LanguageNextButton.IsEnabled = false;
                LanguageNextFreeButton.IsEnabled = false;
                LanguageReferenceTextBox.Text = string.Empty;
				LanguageEditedTextBox.Text = string.Empty;
				LanguageEditedTextBox.IsEnabled = false;
				UpdateStadistics();
			}
            else if (LanguageEditComboBox.SelectedIndex == 0) // New language file
            {			
				CurrentEditionFileName = string.Empty;
				ChangesDoneFlag = false;
				AMTexts.EditMessages = new Dictionary<string, string>();
                AMTexts.EditTexts = new Dictionary<string, string>();
				LanguageEditNewTextBox.IsEnabled = true;
                LanguageEditNewTextBox.Text = string.Empty;
                LanguageEditSaveButton.IsEnabled = false;	
				LanguageEditDeleteButton.IsEnabled = false;
				LanguageTypeComboBox.IsEnabled = false;
				LanguageTypeComboBox.SelectedIndex = -1;
				LanguageItemComboBox.IsEnabled = false;
				LanguageItemComboBox.Items.Clear();
				LanguageNextButton.IsEnabled = false;
                LanguageNextFreeButton.IsEnabled = false;
                LanguageReferenceTextBox.Text = string.Empty;
				LanguageEditedTextBox.Text = string.Empty;
				LanguageEditedTextBox.IsEnabled = false;
				UpdateStadistics();
            }
            else // Existing language file
            {
                CurrentEditionFileName = LanguageEditComboBox.SelectedItem.ToString();
				ChangesDoneFlag = false;
                AMTexts.LoadEditTexts(CurrentEditionFileName);
				LanguageEditNewTextBox.IsEnabled = false;
                //LanguageEditNewTextBox.Text = string.Empty;
				LanguageEditSaveButton.IsEnabled = false;
				LanguageEditDeleteButton.IsEnabled = true;
                LanguageTypeComboBox.IsEnabled = true;
				LanguageTypeComboBox.SelectedIndex = -1;
				LanguageItemComboBox.IsEnabled = false;
				LanguageItemComboBox.Items.Clear();
				LanguageNextButton.IsEnabled = false;
                LanguageNextFreeButton.IsEnabled = false;
                LanguageReferenceTextBox.Text = string.Empty;
				LanguageEditedTextBox.Text = string.Empty;
				LanguageEditedTextBox.IsEnabled = false;
                UpdateStadistics();
            }
        }

		/// <summary>
        /// Text changed event: Checks the validity of new language file
        /// </summary>
        private void LanguageEditNewTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool isValid = !string.IsNullOrEmpty(LanguageEditNewTextBox.Text) &&
               LanguageEditNewTextBox.Text.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) < 0 &&
               !System.IO.File.Exists(System.IO.Path.Combine(AMSettings.LangFolder, 
                                                             LanguageEditNewTextBox.Text + AMSettings.LangExt));
            if (isValid)
            {
                CurrentEditionFileName = LanguageEditNewTextBox.Text + AMSettings.LangExt;
                LanguageEditSaveButton.IsEnabled = true;
            }
            else
            {
                CurrentEditionFileName = string.Empty;
                LanguageEditSaveButton.IsEnabled = false;
            }
		}
		
		/// <summary>
        /// Click event: Save current language file
        /// </summary>
        private void LanguageEditSaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (LanguageEditComboBox.SelectedIndex == 0) // New language file
            {
                AMTexts.SaveEditTexts(CurrentEditionFileName, true);
				AMSettings.Status = AMTexts.Message("NewLanguageFileMessage") + CurrentEditionFileName;
                UpdateLanguageFiles();
            }
			else // Existing language file
				if (MessageBox.Show(string.Format(AMTexts.Message("LanguageFileConfirmOverwriteTextMessage"), CurrentEditionFileName),
								    AMTexts.Message("LanguageFileConfirmOverwriteHeaderMessage"),
								    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
			{
				AMTexts.SaveEditTexts(CurrentEditionFileName, true);
				ChangesDoneFlag = false;
				LanguageEditSaveButton.IsEnabled = false;
                AMSettings.Status = AMTexts.Message("LanguageFileUpdatedMessage") + CurrentEditionFileName;
            }
        }
		
		/// <summary>
        /// Delete language file
        /// </summary>
        private void LanguageEditDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(string.Format(AMTexts.Message("LanguageFileConfirmDeleteTextMessage"), CurrentEditionFileName),
                                AMTexts.Message("LanguageFileConfirmDeleteHeaderMessage"),
                                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    System.IO.File.Delete(System.IO.Path.Combine(AMSettings.LangFolder,
                                                                 LanguageEditComboBox.SelectedItem.ToString()));
                    AMSettings.Status = AMTexts.Message("LanguageFileDeletedMessage") + LanguageEditComboBox.SelectedItem.ToString();
                    ChangesDoneFlag = false;
                }
                catch (Exception ex) { AMSettings.Log = "Fault deleting language file: " + ex.Message; }
				
                UpdateLanguageFiles();
            }
        }
		
        /// <summary>
        /// Selection changed event: Change the type of item for translation (text or message)
        /// </summary>
        private void LanguageTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LanguageItemComboBox.Items.Clear();
            if (LanguageTypeComboBox.SelectedIndex == 0 && AMTexts.SystemTexts != null) // Texts
			{
                foreach (KeyValuePair<string, string> AMText in AMTexts.SystemTexts)
                    LanguageItemComboBox.Items.Add(AMText.Key);
				LanguageItemComboBox.IsEnabled = true;
				LanguageNextButton.IsEnabled = true;
                LanguageNextFreeButton.IsEnabled = true;
            }
            else if (LanguageTypeComboBox.SelectedIndex == 1 && AMTexts.SystemMessages != null) // Messages
			{
                foreach (KeyValuePair<string, string> AMMsg in AMTexts.SystemMessages)
                    LanguageItemComboBox.Items.Add(AMMsg.Key);
				LanguageItemComboBox.IsEnabled = true;
				LanguageNextButton.IsEnabled = true;
                LanguageNextFreeButton.IsEnabled = true;
            }
        }

        /// <summary>
        /// Selection changed event: Change item type to translate
        /// </summary>
        private void LanguageItemComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LanguageItemComboBox.SelectedIndex < 0) // No item selected
			{
				LanguageNextButton.IsEnabled = true;
                LanguageNextFreeButton.IsEnabled = true;
                LanguageReferenceTextBox.Text = string.Empty;
				LanguageEditedTextBox.Text = string.Empty;
				LanguageEditedTextBox.IsEnabled = false;
			}
			else
			{
				if (LanguageTypeComboBox.SelectedIndex == 0) // Texts
				{
					LanguageReferenceTextBox.Text = AMTexts.SystemText(LanguageItemComboBox.SelectedItem.ToString());
					LanguageEditedTextBox.Text = AMTexts.EditText(LanguageItemComboBox.SelectedItem.ToString());
				}
				else if (LanguageTypeComboBox.SelectedIndex == 1) // Messages
				{
					LanguageReferenceTextBox.Text = AMTexts.SystemMessage(LanguageItemComboBox.SelectedItem.ToString());
					LanguageEditedTextBox.Text = AMTexts.EditMessage(LanguageItemComboBox.SelectedItem.ToString());
				}
				LanguageNextButton.IsEnabled = true;
                LanguageNextFreeButton.IsEnabled = true;
                LanguageEditedTextBox.IsEnabled = true;
				UpdateStadistics();
			}
        }

        /// <summary>
        /// Text changed event: Updates translated item
        /// </summary>
        private void LanguageEditedTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (LanguageItemComboBox.SelectedItem == null) return;

            if (LanguageTypeComboBox.SelectedIndex == 0) // Texts
            {
                AMTexts.EditTexts[LanguageItemComboBox.SelectedItem.ToString()] = LanguageEditedTextBox.Text;
				ChangesDoneFlag = true;
				LanguageEditSaveButton.IsEnabled = true;
            }
            else if (LanguageTypeComboBox.SelectedIndex == 1) // Messages
            {
                AMTexts.EditMessages[LanguageItemComboBox.SelectedItem.ToString()] = LanguageEditedTextBox.Text;
				ChangesDoneFlag = true;
				LanguageEditSaveButton.IsEnabled = true;
            }
        }

        /// <summary>
        /// Click event: Select the next item to translate
        /// </summary>
        private void LanguageNextButton_Click(object sender, RoutedEventArgs e)
        {
			if (LanguageItemComboBox.SelectedIndex < 0) LanguageItemComboBox.SelectedIndex = 0;	
            else if (LanguageItemComboBox.SelectedIndex < LanguageItemComboBox.Items.Count - 1)
				LanguageItemComboBox.SelectedIndex++;
        }

        /// <summary>
        /// Click event: Select the next free item to translate
        /// </summary>
        private void LanguageNextFreeButton_Click(object sender, RoutedEventArgs e)
        {
            if (LanguageItemComboBox.SelectedIndex < 0) LanguageItemComboBox.SelectedIndex = 0;

            for (int n = LanguageItemComboBox.SelectedIndex; n < LanguageItemComboBox.Items.Count; n++)
            {
                string T;
                if (LanguageTypeComboBox.SelectedIndex == 0) // Texts
                    T = AMTexts.EditText(LanguageItemComboBox.Items[n].ToString());
                else // Messages
                    T = AMTexts.EditMessage(LanguageItemComboBox.Items[n].ToString());

                if (string.IsNullOrWhiteSpace(T))
                {
                    LanguageItemComboBox.SelectedIndex = n;
                    break;
                }
            }
        }

        /// <summary>
        /// Key down event: With ENTER key select next item to translate
        /// </summary>
        private void LanguageEditedTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
			{
				if (LanguageItemComboBox.SelectedIndex < 0) LanguageItemComboBox.SelectedIndex = 0;	
				else if (LanguageItemComboBox.SelectedIndex < LanguageItemComboBox.Items.Count - 1)
					LanguageItemComboBox.SelectedIndex++;
			}
        }

        /// <summary>
        /// Closing event: check if there are unsaved changes
        /// </summary>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ChangesDoneFlag && !string.IsNullOrWhiteSpace(CurrentEditionFileName) &&
                MessageBox.Show(String.Format(AMTexts.Message("TranslationToolSaveChangesMessage"), CurrentEditionFileName),
                                AMTexts.Message("TranslationToolSaveChangesCaptionMessage"),
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                AMTexts.SaveEditTexts(CurrentEditionFileName, true);
            }
        }


    }
}
