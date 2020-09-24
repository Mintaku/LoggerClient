using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggerClient.Model
{
    class Train : ViewModelBase
    {
        private int _time;
        public int Time
        {
            get
            {
                return _time;
            }
            set
            {
                _time = value;
                NotifyPropertyChanged();
            }
        }
    }
}
