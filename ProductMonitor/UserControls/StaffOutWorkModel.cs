using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductMonitor.UserControls
{
    public class StaffOutWorkModel
    {
        /// <summary>
        /// 员工姓名
        /// </summary>
        public string StaffName { get; set; }
        /// <summary>
        /// 职位
        /// </summary>
        public string Positon { get; set; }
        /// <summary>
        /// 缺岗次数
        /// </summary>
        public int OutWorkCount { get; set; }
    }
}
