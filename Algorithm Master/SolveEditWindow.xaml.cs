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
using System.Windows;
using System.Windows.Controls;

namespace Algorithm_Master
{
    /// <summary>
    /// Lógica de interacción para SolveEditWindow.xaml
    /// </summary>
    public partial class SolveEditWindow : Window
    {
        #region Fields

        /// <summary>
        /// Dictionary of objects with texts to be translated
        /// </summary>
        private Dictionary<string, object> Translate;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Get / Set the cube of the solve
        /// </summary>
        public string NewCube
        {
            get { return SolveEditCubeTextBox.Text.Trim(); }
            set { SolveEditCubeTextBox.Text = value; }
        }

        /// <summary>
        /// Get / set the comment of the solve
        /// </summary>
        public string NewComment
        {
            get { return SolveEditCommentTextBox.Text.Trim(); }
            set { SolveEditCommentTextBox.Text = value; }
        }

        #endregion Properties

        #region Constructor

        /// <summary>
        /// Window constructor
        /// </summary>
        public SolveEditWindow()
        {
            InitializeComponent();
            SetTranslate();
            AMTexts.AddToSystemTexts(Translate);
            UpdateTexts();
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
                { "SolveEditCubeLabelText", SolveEditCubeLabelText },
                { "SolveEditCommentLabelText", SolveEditCommentLabelText },
                { "SolveEditOKButtonText", SolveEditOKButtonText },
                { "SolveEditCancelButtonText", SolveEditCancelButtonText },
            };
        }

        /// <summary>
        /// Update result window texts
        /// </summary>
        public void UpdateTexts()
        {
            foreach (KeyValuePair<string, object> TextObj in Translate)
                (TextObj.Value as TextBlock).Text = AMTexts.Text(TextObj.Key);

            Title = AMTexts.Message("SolveEditTitleMessage");
        }

        #endregion Functions

        #region Events

        /// <summary>
        /// OK button event
        /// </summary>
        private void SolveEditOKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Hide();
        }

        /// <summary>
        /// Cancel button event
        /// </summary>
        private void SolveEditCancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Hide();
        }

        #endregion Events
    }
}
