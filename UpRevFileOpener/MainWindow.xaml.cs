using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace UpRevFileOpener
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            comboFontFamily.ItemsSource = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            comboFontSize.ItemsSource = new List<double>() { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 };
            this.DataContext = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!Properties.Settings.Default.ProductKeyEntered)
            {
                DisplayLoginScreen();
            }
        }

        private void DisplayLoginScreen()
        {
            Login win = new Login();

            win.Owner = this;
            win.ShowDialog();
            if (win.DialogResult.HasValue && win.DialogResult.Value)
                MessageBox.Show("User Logged In");
            else
                Close();
        }

        private void rtbEditor_SelectionChanged(object sender, RoutedEventArgs e)
        {
            object temp = rtbEditor.Selection.GetPropertyValue(Inline.FontWeightProperty);
            btnBold.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontWeights.Bold));
            temp = rtbEditor.Selection.GetPropertyValue(Inline.FontStyleProperty);
            btnItalic.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontStyles.Italic));
            temp = rtbEditor.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            btnUnderline.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(TextDecorations.Underline));

            temp = rtbEditor.Selection.GetPropertyValue(Inline.FontFamilyProperty);
            comboFontFamily.SelectedItem = temp;
            temp = rtbEditor.Selection.GetPropertyValue(Inline.FontSizeProperty);
            comboFontSize.Text = temp.ToString();
        }

        private void exitApp(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void showOpenPasswordBox()
        {
            passwordOpenBox.Password = "";
            openFilePopup.IsOpen = true;
            Keyboard.Focus(passwordOpenBox);
        }

        private void clearOpenItems()
        {
            openFilePopup.IsOpen = false;
            passwordOpenBox.Password = "";
        }

        string checkProtection;
        string lastOpenedFile;
        string id;

        private void openPasswordOk(object sender, RoutedEventArgs e)
        {
            string password = passwordOpenBox.Password.Insert(0, id);
            checkProtection = PasswordVerification.verifyPassword(password);
            if (checkProtection == "Verified" || checkProtection == "Not protected")
            {
                openFileActions(lastOpenedFile);
                clearOpenItems();
            }
            else
            {
                clearOpenItems();
                MessageBox.Show("Incorrect password. Try again.");
            }
        }

        private void checkOpenFile(string fileName)
        {
            id = IDVerification.getIdFromFileNames(fileName);
            if (id != "No files")
            {
                showOpenPasswordBox();
            }
            else
            {
                MessageBox.Show("This file isn't password protected");
                openFileActions(fileName);
                clearOpenItems();
            }
        }

        private void openPasswordCancel(object sender, RoutedEventArgs e)
        {
            clearOpenItems();
        }

        private void openFileActions(string fileName)
        {
            loadFileIntoRTB(fileName);
            rtbEditor.IsReadOnly = true;
            editItem.IsEnabled = true;
            saveItem.IsEnabled = true;
            closeItem.IsEnabled = true;
            updateRecentItems(fileName);
        }

        private void openFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.DefaultExt = ".UpRev";
            openFile.CheckFileExists = true;
            openFile.Filter = "UpRev Files|*.UpRev";
            if (openFile.ShowDialog() == true)
            {
                lastOpenedFile = openFile.FileName;
                checkOpenFile(lastOpenedFile);
            }
        }

        private void loadFileIntoRTB(string fileName)
        {
            try
            {
                using (var fileStream = File.Open(fileName, FileMode.Open))
                {
                    TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
                    range.Load(fileStream, DataFormats.Rtf);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                if (ex.Source != null)
                {
                    using (var fileStream = File.Open(fileName, FileMode.Open, FileAccess.Read))
                    {
                        TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
                        range.Load(fileStream, DataFormats.Rtf);
                    }
                }
            }
        }

        private void openRecentFile(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)e.OriginalSource;
            string fileName = item.Header.ToString();
            if (fileName != "RecentItems" && fileName != null) {
                lastOpenedFile = fileName;
                checkOpenFile(lastOpenedFile);
            }            
        }

        private void updateRecentItems(string fileName)
        {
            if (Properties.Settings.Default.RecentItems.Count < 10 && !Properties.Settings.Default.RecentItems.Contains(fileName))
            {
                Properties.Settings.Default.RecentItems.Add(fileName);
                Properties.Settings.Default.Save();
                Properties.Settings.Default.Reload();
            }
            else if (!Properties.Settings.Default.RecentItems.Contains(fileName) && fileName != null)
            {
                Properties.Settings.Default.RecentItems.RemoveAt(0);
                Properties.Settings.Default.RecentItems.Add(fileName);
                Properties.Settings.Default.Save();
                Properties.Settings.Default.Reload();
            }
        }

        private void showSavePasswordBox()
        {
            passwordSaveBox.Password = "";
            saveFilePopup.IsOpen = true;
            Keyboard.Focus(passwordSaveBox);
        }        

        private void savePasswordOk(object sender, RoutedEventArgs e)
        {
            if (passwordSaveBox.Password.Length < 6)
            {
                MessageBox.Show("Password must be 6 or more characters");
                passwordSaveBox.Password = "";
            }
            else
            {
                saveFilePopup.IsOpen = false;
                SaveFileDialog saveFile = new SaveFileDialog();
                saveFile.Filter = "UpRev Files|*.UpRev";
                if (saveFile.ShowDialog() == true)
                {
                    using (var fileStream = File.Create(saveFile.FileName))
                    {
                        TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
                        range.Save(fileStream, DataFormats.Rtf);
                    }
                    if (PasswordVerification.isPasswordProtected(saveFile.FileName))
                    {
                        PasswordVerification.updatePassword(saveFile.FileName);
                    }
                    string saveId = IDVerification.getId();
                    string newFileName = saveFile.FileName.Insert(0, saveId);
                    string newPassword = passwordSaveBox.Password.Insert(0, saveId);
                    Properties.Settings.Default.FileNames.Add(newFileName);
                    Properties.Settings.Default.Passwords.Add(newPassword);
                    Properties.Settings.Default.Save();
                    Properties.Settings.Default.Reload();
                    clearSaveItems();
                }
            }
        }

        private void savePasswordCancel(object sender, RoutedEventArgs e)
        {
            clearSaveItems();
        }

        private void clearSaveItems()
        {
            passwordSaveBox.Password = "";
            saveFilePopup.IsOpen = false;
        }

        private void saveFile(object sender, RoutedEventArgs e)
        {
            showSavePasswordBox();
        }

        private void editFile(object sender, RoutedEventArgs e)
        {
            rtbEditor.IsReadOnly = false;
        }

        private void closeFile(object sender, RoutedEventArgs e)
        {
            rtbEditor.Document.Blocks.Clear();
            editItem.IsEnabled = false;
            saveItem.IsEnabled = false;
            closeItem.IsEnabled = false;
        }

        private void comboFontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboFontFamily.SelectedItem != null && rtbEditor.IsReadOnly == false)
                rtbEditor.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, comboFontFamily.SelectedItem);
        }

        private void comboFontSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Regex.IsMatch(comboFontSize.Text, @"^[0-9]*$") && rtbEditor.IsReadOnly == false)
            {
                rtbEditor.Selection.ApplyPropertyValue(Inline.FontSizeProperty, comboFontSize.Text);
            }
        }
    }
}
