using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace tailwind
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void App_Startup(object sender, StartupEventArgs e)
        {
            MainWindow wnd = new();

            var rootCommand = new RootCommand("Simple GUI application encompassing the functionality of \"head\", \"tail\", \"cat\", and \"grep\"");
            var fileArg = new Argument<FileInfo>("file");
            fileArg.SetDefaultValue(String.Empty);
            rootCommand.Add(fileArg);
            
            var modeOption = new Option<string>("--mode")
                .FromAmong(
                    "tail",
                    "t",
                    "head",
                    "h",
                    "cat",
                    "c");
            modeOption.SetDefaultValue("cat");
            modeOption.AddAlias("-m");
            modeOption.IsRequired = false;
            rootCommand.AddOption(modeOption);
            
            var lineOption = new Option<int>("--lines");
            lineOption.SetDefaultValue(3);
            lineOption.AddAlias("-l");
            lineOption.IsRequired = false;
            rootCommand.AddOption(lineOption);
            
            var refreshOption = new Option<int>("--refresh");
            refreshOption.SetDefaultValue(4000);
            refreshOption.AddAlias("-r");
            refreshOption.IsRequired = false;
            rootCommand.AddOption(refreshOption);
            
            var patternOption = new Option<string>("--pattern");
            patternOption.SetDefaultValue(String.Empty);
            patternOption.AddAlias("-p");
            patternOption.IsRequired = false;
            rootCommand.AddOption(patternOption);

            var matchOption = new Option<string>("--matchmode")
                .FromAmong(
                "wildcard",
                "regex"
                );
            matchOption.SetDefaultValue(String.Empty);
            matchOption.AddAlias("-M");
            matchOption.IsRequired = false;
            rootCommand.AddOption(matchOption);

            var Args = rootCommand.Parse(e.Args);

            if(Args != null && Args!.Errors.Count < 1)
            {
                wnd.Settings.File = Args.GetValueForArgument<FileInfo>(fileArg);
                if (wnd.Settings.File == null || !wnd.Settings.File.Exists)
                {
                    var ofd = new OpenFileDialog();
                    ofd.Title = "File not found, please select one";
                    if (ofd.ShowDialog() == true)
                    {
                        wnd.Settings.File = new FileInfo(ofd.FileName);
                    }
                    else
                    {
                        Application.Current.Shutdown();
                    }
                }
                
                var mode = Args.GetValueForOption<string>(modeOption) ?? String.Empty;
                wnd.Settings.Mode = mode switch
                {
                    "cat" => FileMode.Cat,
                    "c" => FileMode.Cat,
                    "head" => FileMode.Head,
                    "h" => FileMode.Head,
                    "tail" => FileMode.Tail,
                    "t" => FileMode.Tail,
                    _ => FileMode.Cat,
                };
                wnd.Settings.LineCount = Args.GetValueForOption<int>(lineOption);
                wnd.Settings.RefreshTime = Args.GetValueForOption<int>(refreshOption);
                wnd.Settings.MatchPattern = Args.GetValueForOption<string>(patternOption) ?? String.Empty;
                
                var matchmode = Args.GetValueForOption<string>(matchOption) ?? String.Empty;
                wnd.Settings.MatchMode = matchmode switch
                {
                    "wildcard" => MatchMode.Wildcard,
                    "regex" => MatchMode.Regex,
                    _ => MatchMode.None
                };
            }
            else
            {
                Application.Current.Shutdown();
            }
            wnd.Show();
            wnd.RefreshTimer.Interval = wnd.Settings.RefreshTime;
            wnd.RefreshTimer.Enabled = true;
            wnd.RefreshTimer.AutoReset = true;
            wnd.UpdateText();
            wnd.RefreshTimer.Start();
        }
    }
}
