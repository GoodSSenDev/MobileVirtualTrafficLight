using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            try
            {
                _receiveStream.Position = 1;
                _receiveStream.Write(data);
                _receiveStream.Position = 1;
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Something went wrong while Deserializing VehicleDTO package: {ex}");
            }

            return new VehicleDTO(_receiveReader.ReadDouble(),_receiveReader.ReadInt32(),_receiveReader.ReadInt32(),_receiveReader.ReadBoolean());
        }

        public override byte[] Serialize(IndiationDTO data)
        {
            try
            {
                _sendStream.Position = 1;
                _sendWriter.Write(data.SpecialCommand);
                _sendWriter.Write((int)data.TrafficIndication);

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Something went wrong while Serialize IndicationDTO package: {ex}");
            }

            return _sendStream.ToArray();
        }
    }
}
