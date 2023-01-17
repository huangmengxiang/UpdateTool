using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XFaceUpdateTool.Entitys
{
    /// <summary>
    /// 心跳请求
    /// </summary>
    class BeatRequest
    {
        public string version { set; get; }
        public string sn { set; get; }

    }
}
