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
    /// Lógica de interacción para ResultWindow.xaml
    /// </summary>
    public partial class ResultWindow : Window
    {
        #region Fields

        /// <summary>
        /// Dictionary of objects with texts to be translated
        /// </summary>
        private Dictionary<string, object> Translate;

        /// <summary>
        /// Get cube name in result window
        /// </summary>
        public string Cube => CubeResultTextBox.Text;
        
        /// <summary>
        /// Get comment in result window
        /// </summary>
        public string Comment => CommentResultTextBox.Text;

        /// <summary>
        /// Get +2s penalty in result window
        /// </summary>
        public bool SolvePenalty2s => Result2SRadioButton.IsChecked ?? true;

        /// <summary>
        /// Get DNF penalty in result window
        /// </summary>
        public bool SolveDNF => ResultDNFRadioButton.IsChecked ?? true;

        /// <summary>
        /// Get DNS penalty in result window
        /// </summary>
        public bool SolveDNS => ResultDNSRadioButton.IsChecked ?? true;

        #endregion Fields

        /// <summary>
        /// Result window constructor
        /// </summary>
        public ResultWindow()
        {
            InitializeComponent();
            SetTranslate();
            AMTexts.AddToSystemTexts(Translate);
            UpdateTexts();
        }

        #region Functions

        /// <summary>
        /// Set the dictionary of objects (text blocks) that contains texts to be translated
        /// </summary>
        private void SetTranslate()
        {
            Translate = new Dictionary<string, object>
            {
                { "StartTimeTitleLabelText", StartTimeTitleLabelText },
                { "SolvingTypeTitleLabelText", SolvingTypeTitleLabelText },
                { "ChronoTimeTitleLabelText", ChronoTimeTitleLabelText },
                { "DelayPenaltyTitleLabelText", DelayPenaltyTitleLabelText },
                { "ScrambleTitleLabelText", ScrambleTitleLabelText },
                { "CubeTitleLabelText", CubeTitleLabelText },
                { "CommentTitleLabelText", CommentTitleLabelText },
                { "ResultOKRadioButtonText", ResultOKRadioButtonText },
                { "Result2SRadioButtonText", Result2SRadioButtonText },
                { "ResultDNSRadioButtonText", ResultDNSRadioButtonText },
                { "ResultDNFRadioButtonText", ResultDNFRadioButtonText },
                { "SaveResultButtonText", SaveResultButtonText },
                { "CancelResultButtonText", CancelResultButtonText },
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

        /// <summary>
        /// Update result window data
        /// </summary>
        /// <param name="R">Result</param>
        public void UpdateResult(ChronoResult R)
        {
            StartTimeResultLabelText.Text = R.StartTime.ToString();
            ChronoTimeResultLabelText.Text = AMTexts.MilliseconsToString(R.MeasuredTime);
            ScrambleResultLabelText.Text = R.Scramble;
            SolvingTypeResultLabelText.Text = AMTexts.Message("ResolutionType" + string.Format("{0:00}", (int)R.SolvingType));
            CubeResultTextBox.Text = AMSettings.DefaultCube;
            CommentResultTextBox.Text = AMSettings.DefaultComment;
            if (R.StartDelayDNF) DelayPenaltyResultLabelText.Text = AMTexts.Message("DNFMessage");
            else if (R.StartDelayPenalty2s) DelayPenaltyResultLabelText.Text = AMTexts.Message("Penalty2sMessage");
            else DelayPenaltyResultLabelText.Text = AMTexts.Message("NoPenaltyMessage");

            ResultOKRadioButton.IsChecked = false;
            Result2SRadioButton.IsChecked = false;
            ResultDNFRadioButton.IsChecked = false;
            ResultDNSRadioButton.IsChecked = false;

            ResultOKRadioButton.Focus();

            SaveResultButton.IsEnabled = false;
        }

        #endregion Functions

        #region Events

        /// <summary>
        /// Click event: Save result button
        /// </summary>
        private void SaveResultButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Hide();
        }

        /// <summary>
        /// Click event: Cancel result button
        /// </summary>
        private void CancelResultButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Hide();
        }

        /// <summary>
        /// Checked event: if radio button checked, enable save result button
        /// </summary>
        private void ResultRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            SaveResultButton.IsEnabled = true;
        }

        #endregion Events
    }
}
