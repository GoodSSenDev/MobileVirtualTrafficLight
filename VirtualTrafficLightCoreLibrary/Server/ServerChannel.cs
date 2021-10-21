using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using VirtualTrafficLightCoreLibrary.Common;

namespace VirtualTrafficLightCoreLibrary.Server
{
    public class ServerChannel : Channel<IndiationDTO,VehicleDTO>
    {
        /// <summary>
        /// Memoery stream for sending messages 
        /// </summary>
        readonly MemoryStream _sendStream = new MemoryStream(Marshal.SizeOf<IndiationDTO>());
        readonly BinaryWriter _sendWriter;

        /// <summary>
        /// Memoery stream for reading messages 
        /// </summary>
        readonly MemoryStream _receiveStream = new MemoryStream(Marshal.SizeOf<VehicleDTO>());
        readonly BinaryReader _receiveReader;

        public ServerChannel()
        {
            _sendWriter = new BinaryWriter(_sendStream);
            _receiveReader = new BinaryReader(_receiveStream);
        }

        public override VehicleDTO Deserialize(byte[] data)
        {
            throw new NotImplementedException();
        }

        public override byte[] Serialize(IndiationDTO data)
        {
            throw new NotImplementedException();
        }
    }
}
