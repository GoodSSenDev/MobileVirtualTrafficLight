﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using VirtualTrafficLightCoreLibrary.Common;

namespace VirtualTrafficLightCoreLibrary.Client
{
    public class ClientChannel : Channel<VehicleDTO, IndiationDTO>
    {
        /// <summary>
        /// Memoery stream for sending messages 
        /// </summary>
        readonly MemoryStream _sendStream = new MemoryStream(Marshal.SizeOf<VehicleDTO>());
        readonly BinaryWriter _sendWriter;

        /// <summary>
        /// Memoery stream for reading messages 
        /// </summary>
        readonly MemoryStream _receiveStream = new MemoryStream(Marshal.SizeOf<IndiationDTO>());
        readonly BinaryReader _receiveReader;

        public ClientChannel()
        {
            _sendWriter = new BinaryWriter(_sendStream);
            _receiveReader = new BinaryReader(_receiveStream);
        }

        public override IndiationDTO Deserialize(byte[] data)
        {
            throw new NotImplementedException();
        }

        public override byte[] Serialize(VehicleDTO data)
        {
            throw new NotImplementedException();
        }
    }
}
