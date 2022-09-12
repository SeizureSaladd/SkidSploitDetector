using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Highlighting;
using Microsoft.Win32;

namespace ExploitTemplate
{
    public partial class MainWindow : Window
    {
        WeAreDevs_API.ExploitAPI WeAreDevsLibrary = new WeAreDevs_API.ExploitAPI();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Editor.Options.EnableEmailHyperlinks = false;
            Editor.Options.EnableHyperlinks = false;
            Editor.Options.AllowScrollBelowDocument = false;
            Editor.Drop += delegate (object Send, DragEventArgs Event)
            {
                string[] Files = Event.Data.GetData(DataFormats.FileDrop) as string[];
                foreach (string S in Files)
                {
                    string FileExtension = Path.GetExtension(S);
                    if (FileExtension.Equals(".txt") || FileExtension.Equals(".lua"))
                    {
                        if (File.Exists(S))
                        {
                            string ReaderText = File.ReadAllText(S);
                            Editor.Text = ReaderText;
                        }
                        else { MessageBox.Show("File Not Found", "Error: File Missing", MessageBoxButton.OK, MessageBoxImage.Error); return; }
                    }
                    else { MessageBox.Show("Drag And Drop Only Accepts .txt / .lua File Types", "Error: Invalid File Type", MessageBoxButton.OK, MessageBoxImage.Error); }
                }
            };
            Stream XshdStream = File.OpenRead(Environment.CurrentDirectory + @"\Bin\" + "Syntax.dll");
            XmlTextReader XshdReader = new XmlTextReader(XshdStream);
            Editor.SyntaxHighlighting = HighlightingLoader.Load(XshdReader, HighlightingManager.Instance);
            XshdReader.Close(); XshdStream.Close();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
            => Process.GetCurrentProcess().Kill();

        private void MiniBtn_Click(object sender, RoutedEventArgs e)
            => WindowState = WindowState.Minimized;

        private void ExecuteBtn_Click(object sender, RoutedEventArgs e)
            => WeAreDevsLibrary.SendLuaScript(Editor.Text);

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
            => Editor.Clear();

        private void OpenFileBtn_Click(object sender, RoutedEventArgs e)
        {
            var OpenFile = new OpenFileDialog()
            {
                Title = "Exploit Name - Open File",
                Filter = "Script Files (*.lua, *.txt)|*.lua;*.txt",
            };

            if (OpenFile.ShowDialog() == true) Editor.Text = File.ReadAllText(OpenFile.FileName);
        }

        private void SaveFileBtn_Click(object sender, RoutedEventArgs e)
        {
            var SaveFile = new SaveFileDialog
            {
                Title = "Exploit Name - Save File",
                Filter = "Script Files (*.lua, *.txt)|*.lua;*.txt",
                FileName = "",
            };

            if (SaveFile.ShowDialog() == true) File.WriteAllText(SaveFile.FileName, Editor.Text);
        }

        private void AttachBtn_Click(object sender, RoutedEventArgs e)
            => WeAreDevsLibrary.LaunchExploit();

        private void SettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ExecutionBox.Visibility == Visibility.Visible)
            {
                ExecutionBox.Visibility = Visibility.Hidden;
                SettingsBox.Visibility = Visibility.Visible;
            }
            else if (SettingsBox.Visibility == Visibility.Visible)
            {
                SettingsBox.Visibility = Visibility.Hidden;
                ExecutionBox.Visibility = Visibility.Visible;
            }
        }

        private void TopMostToggle_Checked(object sender, RoutedEventArgs e)
            => Topmost = true;

        private void TopMostToggle_Unchecked(object sender, RoutedEventArgs e)
            => Topmost = false;

        private async void AutoAttachToggle_Checked(object sender, RoutedEventArgs e)
        {
            ProcessScanning:
            await Task.Delay(1000);
            if (Process.GetProcessesByName("RobloxPlayerBeta").Length > 0)
            {
                if (WeAreDevsLibrary.isAPIAttached() && WeAreDevsLibrary.IsUpdated())
                {
                    WeAreDevsLibrary.LaunchExploit();
                }
                else goto ProcessScanning;
            }
        }

        private void AutoAttachToggle_Unchecked(object sender, RoutedEventArgs e) { return; }

        private void OpacityToggle_Checked(object sender, RoutedEventArgs e)
            => base.Opacity = 0.5;

        private void OpacityToggle_Unchecked(object sender, RoutedEventArgs e)
            => base.Opacity = 1;
    }
}
