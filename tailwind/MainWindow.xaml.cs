using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        private readonly bool IsReady = false;
        public SessionSettings Settings { get; set; }
        public Timer RefreshTimer = new();
        private DateTime? lastFileModTime = null;
        private bool settingsChanged = false;
        private bool regexValid = false;
        private Regex? matchingRegex = null;
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
            if (Settings.File != null && Settings.File.Exists)
            {
                if (lastFileModTime != Settings.File.LastWriteTime || settingsChanged)
                {
                    settingsChanged = false;

                    if (lastFileModTime == null || lastFileModTime != Settings.File.LastWriteTime)
                        lastFileModTime = Settings.File.LastWriteTime;

                    var FileContents = File.ReadAllLines(Settings.File.FullName).ToList();
                    Dispatcher.Invoke((Action)delegate () { lstData.Items.Clear(); });
                    
                    int hits = 0;

                    switch (Settings.Mode)
                    {
                        case FileMode.Cat:
                            foreach (var line in FileContents)
                            {
                                if(Settings.MatchMode == MatchMode.None)
                                {
                                    Dispatcher.Invoke((Action)delegate () { lstData.Items.Add(line); });
                                }
                                else
                                {
                                    if(matchingRegex != null && matchingRegex.IsMatch(line))
                                    {
                                        Dispatcher.Invoke((Action)delegate () { lstData.Items.Add(line); });
                                    }
                                }
                            }
                            break;
                        case FileMode.Tail:
                            var lines = new Queue<string>();
                            for (int i = (FileContents.Count - 1); (i >= 0) && (hits <= Settings.LineCount); i--)
                            {
                                var line = FileContents[i];
                                switch (Settings.MatchMode)
                                {
                                    case MatchMode.None:
                                        lines.Enqueue(line);
                                        hits++;
                                        break;
                                    case MatchMode.Wildcard:
                                    case MatchMode.Regex:
                                        if(matchingRegex != null && matchingRegex.IsMatch(line))
                                        {
                                            lines.Enqueue(line);
                                            hits++;
                                        }
                                        break;
                                }
                            }
                            while (lines.Count > 0)
                                Dispatcher.Invoke((Action)delegate () { lstData.Items.Add(lines.Dequeue()); });
                            break;
                        case FileMode.Head:
                            for (int i = 0; (hits <= Settings.LineCount) && (i < FileContents.Count); i++)
                            {
                                var line = FileContents[i];
                                switch (Settings.MatchMode)
                                {
                                    case MatchMode.None:
                                        Dispatcher.Invoke((Action)delegate () { lstData.Items.Add(line); });
                                        hits++;
                                        break;
                                    case MatchMode.Wildcard:
                                    case MatchMode.Regex:
                                        if(matchingRegex != null && matchingRegex.IsMatch(line))
                                        {
                                            Dispatcher.Invoke((Action)delegate () { lstData.Items.Add(line); });
                                            hits++;
                                        }
                                        break;
                                }
                            }
                            break;
                    }
                }
            }
        }

        private void Wildcard_Checked(object sender, RoutedEventArgs e)
        {
            if (!IsReady) return;

            txtPattern.IsEnabled = true;
            Settings.MatchMode = MatchMode.Wildcard;
        }

        private void NoMatch_Checked(object sender, RoutedEventArgs e)
        {
            if (!IsReady) return;

            txtPattern.IsEnabled = false;
            Settings.MatchMode = MatchMode.None;
        }

        private void Regex_Checked(object sender, RoutedEventArgs e)
        {
            if (!IsReady) return;

            txtPattern.IsEnabled = true;
            Settings.MatchMode = MatchMode.Regex;
        }

        private void Head_Checked(object sender, RoutedEventArgs e)
        {
            if (!IsReady) return;

            txtLines.IsEnabled = true;
            Settings.Mode = FileMode.Head;
            settingsChanged = true;
            UpdateText();
        }

        private void Tail_Checked(object sender, RoutedEventArgs e)
        {
            if (!IsReady) return;

            txtLines.IsEnabled = true;
            Settings.Mode = FileMode.Tail;
            settingsChanged = true;
            UpdateText();
        }

        private void Cat_Checked(object sender, RoutedEventArgs e)
        {
            if (!IsReady) return;

            txtLines.IsEnabled = false;
            Settings.Mode = FileMode.Cat;
            settingsChanged = true;
            UpdateText();
        }

        private void PatternTextChanged(object sender, TextChangedEventArgs e)
        {
            if(Settings.MatchMode == MatchMode.Regex)
            {
                try
                {
                    matchingRegex = new Regex(txtPattern.Text);
                    txtPattern.Background = txtLines.Background;
                    regexValid = true;
                }
                catch
                {
                    matchingRegex = null;
                    txtPattern.Background = new SolidColorBrush(Color.FromRgb(255,128,128));
                    regexValid = false;
                }
            }
            else
            {
                try
                {
                    matchingRegex = new Regex(Regex.Escape(txtPattern.Text).Replace("\\?",".").Replace("\\*",".*"));
                    txtPattern.Background = txtLines.Background;
                    regexValid = true;
                }
                catch
                {
                    matchingRegex = null;
                    txtPattern.Background = new SolidColorBrush(Color.FromRgb(255,128,128));
                    regexValid = false;
                }
            }
        }
    }
}
