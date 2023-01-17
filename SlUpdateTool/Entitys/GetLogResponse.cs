using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XFaceUpdateTool.Entitys
{
    /// <summary>
    /// 获取日志响应
    /// </summary>
    class GetLogResponse
    {
        public GetLogResponse()
        {
            this.list = new List<LogItem>();
        }
        public List<LogItem> list { set; get; }

    }
}
