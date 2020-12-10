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
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Algorithm_Master
{
    /// <summary>
    /// Window for input name description for tree view elements
    /// </summary>
    public partial class LibraryTreeViewElement : Window
    {
		#region Fields

        /// <summary>
        /// Dictionary of objects with texts to be translated
        /// </summary>
        private Dictionary<string, object> Translate;
		
		#endregion Fields
		
		#region Properties
		
        /// <summary>
        /// Get / Set the name of the element
        /// </summary>
        public string ElementName
        {
            get { return TreeViewElementNameTextBox.Text.Trim(); }
            set { TreeViewElementNameTextBox.Text = value; }
        }

        /// <summary>
        /// Get the description of the element
        /// </summary>
        public string ElementDescription
        {
            get { return TreeViewElementDescriptionTextBox.Text.Trim(); }
            set { TreeViewElementDescriptionTextBox.Text = value; }
        }

        /// <summary>
        /// Set the window title
        /// </summary>
        public string ElementTitle { set { Title = value; } }

        /// <summary>
        /// Enable / Disable name text box
        /// </summary>
        public bool IsNameEnabled
        {
            get { return TreeViewElementNameTextBox.IsEnabled; }
            set { TreeViewElementNameTextBox.IsEnabled = value; }
        }

        /// <summary>
        /// Enable / Disable description text box
        /// </summary>
        public bool IsDescriptionEnabled
        {
            get { return TreeViewElementDescriptionTextBox.IsEnabled; }
            set { TreeViewElementDescriptionTextBox.IsEnabled = value; }
        }

		#endregion Properties
		
		#region Constructor
		
        /// <summary>
        /// Window constructor
        /// </summary>
        public LibraryTreeViewElement()
        {
            InitializeComponent();
			SetTranslate();
            AMTexts.AddToSystemTexts(Translate);
            UpdateTexts();
            TreeViewElementOKButton.IsEnabled = false;
        }
		
		#endregion Constructor

		#region Functions
		
		/// <summary>
        /// Set the dictionary of objects (text blocks) that contains texts to be translated
        /// </summary>
        private void SetTranslate()
        {
            Translate = new Dictionary<string, object>
            {
                { "TreeViewElementNameLabelText", TreeViewElementNameLabelText },
                { "TreeViewElementDescriptionLabelText", TreeViewElementDescriptionLabelText },
				{ "TreeViewElementOKButtonText", TreeViewElementOKButtonText },
                { "TreeViewElementCancelButtonText", TreeViewElementCancelButtonText },
			};
		}
		
        /// <summary>
        /// Update result window texts
        /// </summary>
            public void UpdateTexts()
        {
            foreach (KeyValuePair<string, object> TextObj in Translate)
                (TextObj.Value as TextBlock).Text = AMTexts.Text(TextObj.Key);
        }

        #endregion Functions

        #region Events

        /// <summary>
        /// Click event: OK button
        /// </summary>
        private void TreeViewElementOKButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TreeViewElementNameTextBox.Text))
            {
                DialogResult = false;
                Hide();
            }

            bool NameOK = true;
            char[] NoValidChars = System.IO.Path.GetInvalidFileNameChars();

            foreach (char c in TreeViewElementNameTextBox.Text)
                if (NoValidChars.Contains(c))
                {
                    NameOK = false;
                    break;
                }

            if (NameOK)
            {
                DialogResult = true;
                Hide();
            }
            else MessageBox.Show(AMTexts.Message("SuitableCharactersMessage"),
                                 AMTexts.Message("InvalidCharactersMessage"),
                                 MessageBoxButton.OK,
                                 MessageBoxImage.Warning);
        }

        /// <summary>
        /// Click event: Cancel button
        /// </summary>
        private void TreeViewElementCancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Hide();
        }

        /// <summary>
        /// TextChanged event: Enable / Disable OK button
        /// </summary>
        private void TreeViewElementNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TreeViewElementOKButton.IsEnabled = TreeViewElementNameTextBox.Text.Length > 0;
        }

        #endregion Events


    }
}
