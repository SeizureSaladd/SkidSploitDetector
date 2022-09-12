// everything i could find related to sentinel tabs (if there's something missing, im sorry <3)
private void MoveTab(object sender, System.Windows.Input.MouseEventArgs e)
{
            TabItem tabItem = e.Source as TabItem;
            if (tabItem == null)
            {
                        return;
            }
            if (Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed)
            {
                        if (VisualTreeHelper.HitTest(tabItem, Mouse.GetPosition(tabItem)).VisualHit is System.Windows.Controls.Button)
                        {
                                    return;
                        }
                        DragDrop.DoDragDrop(tabItem, tabItem, System.Windows.DragDropEffects.Move);
            }
}

Stream stream = File.OpenRead("./bin/lua.xshd");
XmlTextReader reader = new XmlTextReader(stream);
AvalonEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);

Stream xshd_stream = File.OpenRead(Environment.CurrentDirectory + @"\bin\" + "Lua.xshd");
XmlTextReader xshd_reader = new XmlTextReader(xshd_stream);
AvalonEditor.SyntaxHighlighting = HighlightingLoader.Load(xshd_reader, HighlightingManager.Instance);
xshd_reader.Close();
xshd_stream.Close();
this.EditTabs.Loaded += delegate (object source, RoutedEventArgs e)
{
            this.EditTabs.GetTemplateItem<System.Windows.Controls.Button>("AddTabButton").Click += delegate (object s, RoutedEventArgs f)
{
            this.MakeTab("", "New Tab");
};

TabItem ti = EditTabs.SelectedItem as TabItem;
ti.GetTemplateItem<System.Windows.Controls.Button>("CloseButton").Visibility = Visibility.Hidden;
ti.GetTemplateItem<System.Windows.Controls.Button>("CloseButton").Width = 0;
ti.Header = "Main Tab";
this.tabScroller = this.EditTabs.GetTemplateItem<ScrollViewer>("TabScrollViewer");
};
private ScrollViewer tabScroller;
        private void ScrollTabs(object sender, MouseWheelEventArgs e)
        {

            this.tabScroller.ScrollToHorizontalOffset(this.tabScroller.HorizontalOffset + (double)(e.Delta / 10));
        }
        private ICSharpCode.AvalonEdit.TextEditor current;

        public ICSharpCode.AvalonEdit.TextEditor GetCurrent()
        {
            if (this.EditTabs.Items.Count == 0)
            {
                return AvalonEditor;
            }
            else
            {
                return this.current = (this.EditTabs.SelectedContent as ICSharpCode.AvalonEdit.TextEditor);
            }
        }

public ICSharpCode.AvalonEdit.TextEditor MakeEditor()
        {
            ICSharpCode.AvalonEdit.TextEditor textEditor = new ICSharpCode.AvalonEdit.TextEditor
            {
                ShowLineNumbers = true,
                Margin = new Thickness(2, 5, 7, -11),
                FontFamily = new System.Windows.Media.FontFamily("Yu Gothic UI Semibold"),
                Background = Brushes.Transparent,
                Foreground = Brushes.White,

                HorizontalScrollBarVisibility = ScrollBarVisibility.Visible,
                VerticalScrollBarVisibility = ScrollBarVisibility.Visible
            };
            textEditor.Options.EnableEmailHyperlinks = false;
            textEditor.Options.EnableHyperlinks = false;
            textEditor.Options.AllowScrollBelowDocument = true;
            Stream xshd_stream = File.OpenRead(Environment.CurrentDirectory + @"\bin\" + "lua.xshd");
            XmlTextReader xshd_reader = new XmlTextReader(xshd_stream);
            textEditor.SyntaxHighlighting = HighlightingLoader.Load(xshd_reader, HighlightingManager.Instance);

            xshd_reader.Close();
            xshd_stream.Close();
            return textEditor;
        }
        public TabItem MakeTab(string text = "", string title = "Tab")
        {
            title = title + "";
            bool loaded = false;
            ICSharpCode.AvalonEdit.TextEditor textEditor = MakeEditor();
            textEditor.Text = text;
            TabItem tab = new TabItem
            {
                Content = textEditor,
                Style = (base.TryFindResource("Tab") as Style),
                AllowDrop = true,
                Header = title
            };
            tab.MouseWheel += this.ScrollTabs;
            tab.Loaded += delegate (object source, RoutedEventArgs e)
            {
                if (loaded)
                {
                    return;
                }
                this.tabScroller.ScrollToRightEnd();
                loaded = true;
            };
            tab.MouseDown += delegate (object sender, MouseButtonEventArgs e)
            {
                if (e.OriginalSource is Border)
                {
                    if (e.MiddleButton == MouseButtonState.Pressed)
                    {
                        this.EditTabs.Items.Remove(tab);
                        return;
                    }
                }
            };
            tab.Loaded += delegate (object s, RoutedEventArgs e)
            {
                tab.GetTemplateItem<System.Windows.Controls.Button>("CloseButton").Click += delegate (object r, RoutedEventArgs f)
                {
                    this.EditTabs.Items.Remove(tab);
                };

                this.tabScroller.ScrollToRightEnd();
                loaded = true;
            };

            tab.MouseMove += this.MoveTab;
            tab.Drop += this.DropTab;
            string oldHeader = title;
            this.EditTabs.SelectedIndex = this.EditTabs.Items.Add(tab);
            return tab;
        }
private void EditTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void DropTab(object sender, System.Windows.DragEventArgs e)
        {
            TabItem tabItem = e.Source as TabItem;
            if (tabItem != null)
            {
                TabItem tabItem2 = e.Data.GetData(typeof(TabItem)) as TabItem;
                if (tabItem2 != null)
                {
                    if (!tabItem.Equals(tabItem2))
                    {
                        System.Windows.Controls.TabControl tabControl = tabItem.Parent as System.Windows.Controls.TabControl;
                        int insertIndex = tabControl.Items.IndexOf(tabItem2);
                        int num = tabControl.Items.IndexOf(tabItem);
                        tabControl.Items.Remove(tabItem2);
                        tabControl.Items.Insert(num, tabItem2);
                        tabControl.Items.Remove(tabItem);
                        tabControl.Items.Insert(insertIndex, tabItem);
                        tabControl.SelectedIndex = num;
                    }
                    return;
                }
            }
        }
