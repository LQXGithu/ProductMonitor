using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductMonitor.UserControls
{
    public class AlarmModel
    {
        public string Num { get; set; }
        public string Message { get; set; }
        public string Time { get; set; }
        public int Duration { get; set; }
    }
}
