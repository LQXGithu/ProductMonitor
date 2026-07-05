using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductMonitor.UserControls
{
    public class WorkShopModel
    {
        /// <summary>
        /// 车间名称
        /// </summary>
        public string WorkShopName { get; set; }
        /// <summary>
        /// 作业数量
        /// </summary>
        public int WorkingCount { get; set; }
        /// <summary>
        /// 故障数量
        /// </summary>
        public int WrongCount { get; set; }
        /// <summary>
        /// 等待数量
        /// </summary>
        public int WaitCount { get; set; }
        /// <summary>
        /// 停机数量
        /// </summary>
        public int StopCount { get; set; }
        public int TotalCount { get { return WorkingCount + WrongCount + WaitCount + StopCount; } set{} }
    }
}
