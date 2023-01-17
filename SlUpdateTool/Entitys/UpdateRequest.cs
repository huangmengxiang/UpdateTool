using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XFaceUpdateTool.Entitys
{
    /// <summary>
    /// 软件升级请求
    /// </summary>
    class UpdateRequest
    {
        public UpdateRequest()
        {
            this.list = new List<FileItem>();
        }
        public string version { set; get; }
        public List<FileItem> list { set; get; }

    }
}
