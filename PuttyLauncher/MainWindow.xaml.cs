using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PuttyLauncher
{
    public partial class MainWindow : Window
    {
        private byte[] hexToInt;

        public MainWindow()
        {
            hexToInt = new byte[127];
            hexToInt['0'] = 0;
            hexToInt['1'] = 1;
            hexToInt['2'] = 2;
            hexToInt['3'] = 3;
            hexToInt['4'] = 4;
            hexToInt['5'] = 5;
            hexToInt['6'] = 6;
            hexToInt['7'] = 7;
            hexToInt['8'] = 8;
            hexToInt['9'] = 9;
            hexToInt['A'] = 10;
            hexToInt['B'] = 11;
            hexToInt['C'] = 12;
            hexToInt['D'] = 13;
            hexToInt['E'] = 14;
            hexToInt['F'] = 15;
            hexToInt['a'] = 10;
            hexToInt['b'] = 11;
            hexToInt['c'] = 12;
            hexToInt['d'] = 13;
            hexToInt['e'] = 14;
            hexToInt['f'] = 15;

            InitializeComponent();

            UpdateSessions();
        }

        private void UpdateSessions()
        {
            RegistryKey sessionsKey = Registry.CurrentUser.OpenSubKey("Software\\Simontatham\\PuTTY\\Sessions");

            if (sessionsKey != null)
            {
                string selected = cbSessions.Text;

                foreach (string name in sessionsKey.GetSubKeyNames())
                {
                    RegistryKey sessionKey = sessionsKey.OpenSubKey(name);
                    string host = sessionKey.GetValue("HostName")?.ToString() ?? "";

                    int length = 0;
                    byte[] decoded = Encoding.ASCII.GetBytes(name);

                    for (int i = 0; i < decoded.Length; i++)
                    {
                        if (decoded[i] == '%' && i + 2 < decoded.Length)
                        {
                            byte chr = 0;

                            chr |= (byte)(hexToInt[decoded[i + 1]] << 4);
                            chr |= (byte)(hexToInt[decoded[i + 2]] << 0);

                            decoded[length++] = chr;

                            i += 2;
                        }
                        else
                        {
                            decoded[length++] = decoded[i];
                        }
                    }
                    string decodedName = Encoding.Default.GetString(decoded, 0, length);

                    cbSessions.Items.Add(new SessionObject { Name = decodedName });
                }
                cbSessions.Text = selected;
            }
        }

        private void OnExecuteClick(object sender, RoutedEventArgs e)
        {
            try
            {
                SessionObject sessionObject = cbSessions.SelectedValue as SessionObject;

                if (sessionObject != null)
                {
                    Settings settings = Settings.Load();

                    if (tbEditable.IsChecked == true)
                    {
                        Process.Start(settings.PuttyPath, $"-load \"{sessionObject.Name}\" \"{tbHost.Text}\"");
                    }
                    else
                    {
                        Process.Start(settings.PuttyPath, $"-load \"{sessionObject.Name}\"");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void OnEditableChanged(object sender, RoutedEventArgs e)
        {
            tbHost.Visibility = tbEditable.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SessionObject sessionObject = cbSessions.SelectedValue as SessionObject;

            if (sessionObject != null)
            {
                tbHost.Text = sessionObject.Host;
            }
        }

        private void OnSettingClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Filter = "Executable File (*.exe)|*.exe";

            if (ofd.ShowDialog() == true)
            {
                Settings settings = Settings.Load();
                settings.PuttyPath = ofd.FileName;
                Settings.Save(settings);
            }
        }

        private void OnRefreshClick(object sender, RoutedEventArgs e)
        {
            UpdateSessions();
        }
    }
}
