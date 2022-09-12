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
