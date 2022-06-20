using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace tailwind
{
    public enum MatchMode
    {
        None,
        Wildcard,
        Regex
    }
    public enum FileMode
    {
        Head,
        Tail,
        Cat
    }
    public class SessionSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string matchPattern = String.Empty;
        private FileInfo file = default;
        private string windowTitle = String.Empty;
        private FileMode mode = FileMode.Tail;
        private int lineCount = 0;
        private int refreshTime = 0;
        private MatchMode matchMode = MatchMode.None;

        public string MatchPattern
        {
            get { return matchPattern; }
            set
            {
                matchPattern = value;
                if (PropertyChanged != null)
                    NotifyChange();

            }
        }

        public FileInfo File
        {
            get { return file; }
            set
            { 
                file = value;
                if (PropertyChanged != null)
                    NotifyChange();
                WindowTitle = $"tailwind - {mode} {file?.Name}";
            }
        }

        public string WindowTitle
        {
            get { return windowTitle; }
            set
            {
                windowTitle = value;
                if (PropertyChanged != null)
                    NotifyChange();
            }
        }

        public FileMode Mode
        {
            get { return mode; }
            set
            {
                mode = value;
                if (PropertyChanged != null)
                    NotifyChange();
                WindowTitle = $"tailwind - {mode.ToString()} {file?.Name}";
            }
        }

        public int LineCount
        {
            get { return lineCount; }
            set
            {
                lineCount = value;
                if (PropertyChanged != null)
                    NotifyChange();
            }
        }

        public int RefreshTime
        {
            get { return refreshTime; }
            set
            {
                refreshTime = value;
                if (PropertyChanged != null)
                    NotifyChange();
            }
        }

        public MatchMode MatchMode
        {
            get { return matchMode; }
            set
            {
                matchMode = value;
                if (PropertyChanged != null)
                    NotifyChange();
            }
        }

        private void NotifyChange([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
