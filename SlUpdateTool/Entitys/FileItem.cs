using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XFaceUpdateTool.Entitys
{
    /// <summary>
    /// 更新文件项
    /// </summary>
    class FileItem
    {
        /// <summary>
        /// 相对路径
        /// </summary>
        public string path { set; get; }
        /// <summary>
        /// 文件大小(kb)
        /// </summary>
        public float size { set; get; }

    }
}
