using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XFaceUpdateTool
{

    /// <summary>
    /// 数据包
    /// </summary>
    public class Package
    {
        //where T : class, new()

        public string Version { set; get; }
        public string Magic { set; get; }
        public int SN { set; get; }
        public string ContentType { set; get; }
        public int ContentLength { set; get; }
        public byte[] Body { set; get; }
        /// <summary>
        /// 将数据包转换为标准协议
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("{0}:{1}", Host.version, Version));
            sb.AppendLine(string.Format("{0}:{1}", Host.magic, Magic));
            sb.AppendLine(string.Format("{0}:{1}", Host.sn, SN));
            sb.AppendLine(string.Format("{0}:{1}", Host.contentType, ContentType));
            sb.AppendLine(string.Format("{0}:{1}", Host.contentLength, ContentLength));
            sb.Append("\r\n");
            if (Body != null)
            {
                sb.Append(Encoding.UTF8.GetString(Body));
            }
            return sb.ToString();
        }
        /// <summary>
        /// 将数据包转换为字节
        /// </summary>
        /// <remarks>注意Body不进行编码转换，默认字节</remarks>
        /// <returns></returns>
        public byte[] ToBytes()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("{0}:{1}", Host.version, Version));
            sb.AppendLine(string.Format("{0}:{1}", Host.magic, Magic));
            sb.AppendLine(string.Format("{0}:{1}", Host.sn, SN));
            sb.AppendLine(string.Format("{0}:{1}", Host.contentType, ContentType));
            sb.AppendLine(string.Format("{0}:{1}", Host.contentLength, ContentLength));
            sb.Append("\r\n");

            List<byte> list = new List<byte>();

            byte[] bs = Encoding.UTF8.GetBytes(sb.ToString());
            list.AddRange(bs);
            
            if (Body != null)
            {
                list.AddRange(Body);
            }
            return list.ToArray();
        }
    }
}
