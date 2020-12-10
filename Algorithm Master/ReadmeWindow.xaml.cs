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
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Algorithm_Master
{
    /// <summary>
    /// Lógica de interacción para ReadmeWindow.xaml
    /// </summary>
    public partial class ReadmeWindow : Window
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ReadmeWindow()
        {
            InitializeComponent();

            DirectoryInfo diAppData;

            Title = AMTexts.Message("StartUpMessage");

            diAppData = new DirectoryInfo(AMSettings.ReadmeFolder);
            try
            {
                ReadmeTab.Items.Clear();
                FileInfo[] fiReadme = diAppData.GetFiles("*.txt");
                foreach (FileInfo fi in fiReadme)
                {
                    TabItem NewTabItem = new TabItem()
                    {
                        Header = fi.Name,
                        Content = new TextBox()
                        {
                            Text = File.ReadAllText(fi.FullName),
                            FontFamily = new System.Windows.Media.FontFamily("Courier New"),
                            IsReadOnly = true,
                            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto
                        }
                    };
                    ReadmeTab.Items.Add(NewTabItem);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Fault reading readme file..."); }
        }

        /// <summary>
        /// Close event
        /// </summary>
        private void CloseReadmeButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Click event: increase font size
        /// </summary>
        private void IncreaseReadmeButton_Click(object sender, RoutedEventArgs e)
        {
            if (ReadmeTab.Items != null && ReadmeTab.Items.Count > 0)
            {
                foreach (TabItem RTab in ReadmeTab.Items)
                {
                    TextBox TB = RTab.Content as TextBox;
                    if (TB.FontSize < 32) TB.FontSize++;
                }
            }
        }

        /// <summary>
        /// Click event: decrease font size
        /// </summary>
        private void DecreaseReadmeButton_Click(object sender, RoutedEventArgs e)
        {
            if (ReadmeTab.Items != null && ReadmeTab.Items.Count > 0)
            {
                foreach (TabItem RTab in ReadmeTab.Items)
                {
                    TextBox TB = RTab.Content as TextBox;
                    if (TB.FontSize > 6) TB.FontSize--;
                }
            }
        }
    }
}
