using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace tailwind
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool IsReady = false;
        public SessionSettings Settings { get; set; }
        public Timer RefreshTimer = new();
        private DateTime? lastFileModTime = null;
        public MainWindow()
        {
            InitializeComponent();
            
            Settings = new SessionSettings();
            DataContext = Settings;
            RefreshTimer.Elapsed += RefreshTimer_Elapsed;

            IsReady = true;
        }

        private void RefreshTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            UpdateText();
        }

        public void UpdateText()
        {
            if (Settings.File.Exists)
            {
                if (lastFileModTime != Settings.File.LastWriteTime)
                {
                    if (lastFileModTime == null)
                        lastFileModTime = Settings.File.LastWriteTime;

                    var FileContents = File.ReadAllLines(Settings.File.FullName).ToList();
                    Dispatcher.Invoke((Action)delegate () { lstData.Items.Clear(); });

                    switch (Settings.Mode)
                    {
                        case FileMode.Cat:
                            foreach (var line in FileContents)
                            {
                                Dispatcher.Invoke((Action)delegate () { lstData.Items.Add(line); });
                            }
                            break;
                        case FileMode.Tail:
                            var lines = new Queue<string>();
                            int hits = 0;
                            for (int i = (FileContents.Count - 1); (i >= 0) && (hits <= Settings.LineCount); i--)
                            {
                                var line = FileContents[i];
                                if (Settings.MatchMode == MatchMode.None)
                                {
                                    lines.Enqueue(line);
                                    hits++;
                                }
                            }
                            while (lines.Count > 0)
                                Dispatcher.Invoke((Action)delegate () { lstData.Items.Add(lines.Dequeue()); });
                            break;
                        case FileMode.Head:
                            for (int i = 0; (i < Settings.LineCount) && (i < FileContents.Count); i++)
                            {
                                var line = FileContents[i];
                                Dispatcher.Invoke((Action)delegate () { lstData.Items.Add(line); });
                            }
                            break;
                    }
                }
            }
        }

        private void chkGrep_Checked(object sender, RoutedEventArgs e)
        {
            if (!IsReady) return;

            if(chkGrep.IsChecked == true)
            {
                chkRegex.IsEnabled = true;
            }
            else
            {
                chkRegex.IsEnabled = false;
            }
        }

        private void rdoHead_Checked(object sender, RoutedEventArgs e)
        {
            if (!IsReady) return;

            if (rdoHead.IsChecked == true)
            {
                txtLines.IsEnabled = true;
            }
        }

        private void rdoTail_Checked(object sender, RoutedEventArgs e)
        {
            if (!IsReady) return;

            if (rdoTail.IsChecked == true)
            {
                txtLines.IsEnabled = true;
            }
        }

        private void rdoCat_Checked(object sender, RoutedEventArgs e)
        {
            if (!IsReady) return;

            if (rdoCat.IsChecked == true)
            {
                txtLines.IsEnabled = false;
            }
        }
    }
}
