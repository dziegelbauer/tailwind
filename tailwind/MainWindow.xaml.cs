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
using System.Windows.Shapes;

namespace tailwind
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool IsReady = false;
        public MainWindow()
        {
            InitializeComponent();

            var cmd = Environment.CommandLine;
            var args = cmd.Split(' ');

            IsReady = true;
        }

        private void rdoGrep_Checked(object sender, RoutedEventArgs e)
        {
            if (!IsReady) return;

            if(rdoGrep.IsChecked == true)
            {
                chkRegex.IsEnabled = true;
                txtPattern.IsEnabled = true;
                txtLines.IsEnabled = false;
                btnLinesDown.IsEnabled = false;
                btnLinesUp.IsEnabled = false;
            }
        }

        private void rdoHead_Checked(object sender, RoutedEventArgs e)
        {
            if (!IsReady) return;

            if (rdoHead.IsChecked == true)
            {
                chkRegex.IsEnabled = false;
                txtPattern.IsEnabled = false;
                txtLines.IsEnabled = true;
                btnLinesDown.IsEnabled = true;
                btnLinesUp.IsEnabled = true;
            }
        }

        private void rdoTail_Checked(object sender, RoutedEventArgs e)
        {
            if (!IsReady) return;

            if (rdoTail.IsChecked == true)
            {
                chkRegex.IsEnabled = false;
                txtPattern.IsEnabled = false;
                txtLines.IsEnabled = true;
                btnLinesDown.IsEnabled = true;
                btnLinesUp.IsEnabled = true;
            }
        }
    }
}
