using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PuttyLauncher
{
    internal class SessionObject : INotifyPropertyChanged
    {
        private string name;
        private string host;

        public string Name
        {
            get => name;
            set
            {
                if (name != value)
                {
                    name = value;

                    OnPropertyChanged();
                }
            }
        }

        public string Host
        {
            get => host;
            set
            {
                if (host != value)
                {
                    host = value;

                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
