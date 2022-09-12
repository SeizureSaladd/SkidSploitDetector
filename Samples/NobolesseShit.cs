// some weird shit i found in a source leak, it can be found easily in skidded executors nowadays

// Reinstall Roblox
foreach (Process proc in Process.GetProcessesByName("RobloxPlayerBeta"))
            {
                proc.Kill();
            }
            string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/Roblox";
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);

            }
            if (File.Exists("bin/RobloxPlayerLauncher.exe"))
            {
                File.Delete("bin/RobloxPlayerLauncher.exe");
            }
            using (WebClient client = new WebClient())
            {
                string ver = await client.DownloadStringTaskAsync("http://setup.roblox.com/version");
                await client.DownloadFileTaskAsync(new Uri("http://setup.roblox.com/" + ver + "-Roblox.exe"), "bin/RobloxPlayerLauncher.exe");

                Process.Start(Environment.CurrentDirectory + "/bin/RobloxPlayerLauncher.exe");

            }
            
            // theme drop
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                try
                {
                    string[] files = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
                    var uri = new Uri(files[0]);
                    var image = new BitmapImage(uri);
                    Bg.Background = new ImageBrush(image);
                    EditTabs.Background = Brushes.Transparent;
                    AvalonEditor.Background = Brushes.Transparent;
                    ThemeDrop.Background = Brushes.Transparent;

                }
                catch
                {
                    System.Windows.MessageBox.Show("You can't use that file format!");
                }
            }
            
            // oh no
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.InitialDirectory = System.Windows.Forms.Application.StartupPath;
            Nullable<bool> result = saveFileDialog.ShowDialog();
            saveFileDialog.Filter = "Lua Scripts (*.lua)|*.lua|Txt Scripts (*.txt)|*.txt";
            saveFileDialog.Title = "Save Scripts";
            if (result == true)
            {
                File.WriteAllText(saveFileDialog.FileName, GetCurrent().Text);
            }
