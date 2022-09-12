using ICSharpCode.AvalonEdit.Highlighting;
using System.Xml;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using HorizonWPF.Extensions;
using ICSharpCode.AvalonEdit;
using Microsoft.Win32;
using HorizonWPF.Controls;
using WeAreDevs_API;

namespace HorizonWPF
{
    public partial class MainWindow : Window
    {
        ExploitAPI api = new ExploitAPI();  

        private double _actualWidth = 0.0;

        private double _actualMargin = 0.0;

        private bool _isSettings = false;

        private ScrollViewer _tabScrollViewer = null;

        private Monaco _currentEditor = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs args)
        {
            AppDomain.CurrentDomain.UnhandledException += (_, e) =>
            {
                var result = MessageBox.Show("Horizon has encountered an error. Would you like to copy the exception?", "Horizon - Exception", MessageBoxButton.YesNo, MessageBoxImage.Error);

                if (result == MessageBoxResult.Yes)
                    Clipboard.SetText((e.ExceptionObject as Exception).Message);

                Environment.Exit(0);
            };

            if (!Directory.Exists("bin"))
                Directory.CreateDirectory("bin");

            if (!Directory.Exists("scripts"))
                Directory.CreateDirectory("scripts");

            if (!Directory.Exists("autoexec"))
                Directory.CreateDirectory("autoexec");

            if (!File.Exists("Lua.xshd"))
                throw new FileNotFoundException($"Couldn't find {Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Lua.xshd")}");
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ButtonState == MouseButtonState.Pressed)
                DragMove();
        }

        private void Background_Loaded(object sender, RoutedEventArgs e)
        {
            _actualWidth = Background.ActualWidth;
            _actualMargin = _actualWidth * 2;

            ExecutorPage.Width = _actualWidth;
            SettingsPage.Width = _actualWidth;
            
            SettingsPage.Margin = new Thickness(_actualMargin, 0, 0, 0);

            EditorOptions.Width = _actualWidth / 3;
            InterfaceOptions.Width = _actualWidth / 3;
        }

        private void EditorTab_Loaded(object sender, RoutedEventArgs args)
        {
            EditorTab.FindElementChild<Button>("AddTabButton").Click += (_, e)
                => AddTab("Script");

            _tabScrollViewer = EditorTab.FindElementChild<ScrollViewer>("TabScrollViewer");
            AddTab("Script", "print('Hello World!')");
        }

        private void LibraryButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isSettings)
                return;

            ExecutorPage.Move(new Thickness(-_actualMargin, 0, 0, 0), 750);
            ExecutorPage.FadeOut(750);
            ExecutorPage.IsHitTestVisible = false;

            SettingsPage.Move(new Thickness(0, 0, 0, 0), 750);
            SettingsPage.FadeIn(750, 150);
            SettingsPage.IsHitTestVisible = true;

            _isSettings = true;
        }

        private async void ExecuteButton_Click(object sender, RoutedEventArgs e)
        {
           
            {
                if (GetCurrentEditor() == null)
                    return;

                string Text = await _currentEditor.GetText();

                api.SendLuaScript(Text);
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            if (GetCurrentEditor() == null)
                return;

            _currentEditor.SetText("");
        }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Horizon - Open File",
                Filter = "Script Files (*.lua, *.txt)|*.lua;*.txt"
            };

            if (dialog.ShowDialog() == true)
                AddTab(dialog.SafeFileName, File.ReadAllText(dialog.FileName));
        }

        private async void SaveFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (GetCurrentEditor() == null)
                return;

            var tab = EditorTab.SelectedItem as TabItem;

            var dialog = new SaveFileDialog
            {
                Title = "Horizon - Open File",
                Filter = "Script Files (*.lua, *.txt)|*.lua;*.txt",
                FileName = tab.Header.ToString()
            };

            if (dialog.ShowDialog() == true)
                File.WriteAllText(dialog.FileName, await _currentEditor.GetText());
        }

        private void HideLibraryButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void HideSettingsButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!_isSettings)
                return;

            ExecutorPage.Move(new Thickness(0, 0, 0, 0), 750);
            ExecutorPage.FadeIn(750, 150);
            ExecutorPage.IsHitTestVisible = true;

            SettingsPage.Move(new Thickness(_actualMargin, 0, 0, 0), 750);
            SettingsPage.FadeOut(750);
            SettingsPage.IsHitTestVisible = false;

            _isSettings = false;
        }

        private bool _isSliderLoaded = false;

        private void Slider_Loaded(object sender, RoutedEventArgs e)
            => _isSliderLoaded = true;

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_isSliderLoaded)
                return;

            double offset = ScaleSlider.Value / 100;

            ScaleEffect.ScaleX = offset;
            ScaleEffect.ScaleY = offset;

            if (offset < 1)
                _actualWidth = this.Width / offset;
            else
                _actualWidth = this.Width * offset;
            
            _actualMargin = _actualWidth * 2;

            ExecutorPage.Width = _actualWidth;
            SettingsPage.Width = _actualWidth;

            if (_isSettings)
                ExecutorPage.Margin = new Thickness(-_actualMargin, 0, 0, 0);

            if (!_isSettings)
                SettingsPage.Margin = new Thickness(_actualWidth, 0, 0, 0);

            double columnWidth = _actualWidth / 3;

            EditorOptions.Width = columnWidth;
            InterfaceOptions.Width = columnWidth;
        }

        private Monaco GetCurrentEditor()
        {
            if (EditorTab.Items.Count == 0)
                return null;

            return _currentEditor = (Monaco)EditorTab.SelectedContent;
        }

        private void AddTab(string title, string value = "", bool isLocal = false)
        {
            if (!isLocal)
                title += " " + (EditorTab.Items.Count + 1).ToString();

            var tab = new TabItem
            {
                Header = title,
                Content = new Monaco(new MonacoSettings
                {
                    dragAndDrop = (bool)DragAndDrop.IsChecked,
                    fontSize = (int)FontSize.Value,
                    lineNumbers = (bool)LineNumbers.IsChecked,
                    minimap = new EditorSettings
                    {
                        enabled = (bool)MinimapEnabled.IsChecked
                    }
                }),
                AllowDrop = true
            };

            tab.MouseWheel += (_, e)
                => _tabScrollViewer.ScrollToHorizontalOffset(_tabScrollViewer.HorizontalOffset + (double)(e.Delta / 10));

            tab.MouseMove += (_, e) =>
            {
                if (Mouse.PrimaryDevice.LeftButton != MouseButtonState.Pressed)
                    return;

                var hitTest = VisualTreeHelper.HitTest(tab, Mouse
                    .GetPosition(tab));

                if (hitTest != null && !(hitTest.VisualHit is Button))
                    DragDrop.DoDragDrop(tab, tab, DragDropEffects.Move);
            };

            tab.Drop += (_, e) =>
            {
                var item = e.Data.GetData(typeof(TabItem)) as TabItem;

                if (item == null)
                    return;

                if (tab.Equals(item))
                    return;

                var insertIndex = EditorTab.Items.IndexOf(item);

                EditorTab.Items.Remove(item);
                EditorTab.Items.Insert(insertIndex, item);
                EditorTab.Items.Remove(tab);
                EditorTab.Items.Insert(insertIndex, tab);
            };

            tab.Loaded += (_, e) =>
            {
                tab.FindElementChild<Button>("RemoveTabButton").Click += (sender, args) =>
                {
                    if (EditorTab.Items.Count == 1)
                        return;

                    EditorTab.Items.Remove(tab);

                    foreach (TabItem item in EditorTab.Items)
                        if (item.Header.ToString().StartsWith("Tab"))
                            item.Header = "Tab " + (EditorTab.Items.IndexOf(item) + 1).ToString();
                };

                _tabScrollViewer.ScrollToRightEnd();
            };

            EditorTab.SelectedIndex = EditorTab.Items.Add(tab);
        }

        private void Inject_Click(object sender, RoutedEventArgs e)
        {
            api.LaunchExploit();
        }
    }
}
