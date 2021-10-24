using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VirtualTrafficCoreLibrary.Common
{
    public class BinaryReaderReverse : BinaryReader
    {
        public BinaryReaderReverse(System.IO.Stream stream) : base(stream) 
        {

        }

        public override int ReadInt32()
        {
            var data = base.ReadBytes(4);
            Array.Reverse(data);
            return BitConverter.ToInt32(data, 0);
        }

        public Int16 ReadInt16()
        {
            var data = base.ReadBytes(2);
            Array.Reverse(data);
            return BitConverter.ToInt16(data, 0);
        }

        public Int64 ReadInt64()
        {
            var data = base.ReadBytes(8);
            Array.Reverse(data);
            return BitConverter.ToInt64(data, 0);
        }
        public override double ReadDouble()
        {
            var data = base.ReadBytes(8);
            Array.Reverse(data);
            return BitConverter.ToDouble(data, 0);
        }
        public override bool ReadBoolean()
        {
            var data = base.ReadBytes(1);
            Array.Reverse(data);
            return BitConverter.ToBoolean(data, 0);
        }

    }
}
