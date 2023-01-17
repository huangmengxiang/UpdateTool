using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XFaceUpdateTool
{
    class UdpDataPackage
    {
        public byte[] TAG { get; set; }
        public UInt32 type { get; set; }
        public UInt32 length{ get; set; }
        public byte[] Body { get; set; }

        public UdpDataPackage()
        {
            TAG = new byte[4] { (byte)'S', (byte)'L', (byte)'8', (byte)'X' };
        }

        public byte[] ToBytes()
        {
            List<byte> list = new List<byte>();
            list.AddRange(TAG);
            byte[] bType = System.BitConverter.GetBytes(type); //得到小端字节序数组 
            Array.Reverse(bType); //反转数组转成大端
            list.AddRange(bType);

            byte[] bLength = System.BitConverter.GetBytes(length); //得到小端字节序数组 
            Array.Reverse(bLength); //反转数组转成大端
            list.AddRange(bLength);

            if (Body != null)
            {
                list.AddRange(Body);
            }
            return list.ToArray();
        }
    }
}
