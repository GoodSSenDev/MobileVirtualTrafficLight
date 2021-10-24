using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using VirtualTrafficCoreLibrary.Common;

namespace VirtualTrafficCoreLibrary.Client
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
            _channelType = "Client";
            _sendWriter = new BinaryWriter(_sendStream);
            _receiveReader = new BinaryReader(_receiveStream);
        }

        public override IndiationDTO Deserialize(byte[] data)
        {
            try
            {
                _receiveStream.Position = 0;
                _receiveStream.Write(data);
                _receiveStream.Position = 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Something went wrong while Deserializing IndiationDTO package: {ex}");
            }
            var ReceivedDTO = new IndiationDTO(_receiveReader.ReadInt32(), _receiveReader.ReadInt32());

            return ReceivedDTO;
        }

        public override byte[] Serialize(VehicleDTO data)
        {
            try
            {
                _sendStream.Position = 0;
                _sendWriter.Write(data.Speed);
                _sendWriter.Write(data.ClosestLane);
                _sendWriter.Write(data.Distance);
                _sendWriter.Write(data.IsDistanceShrinking);

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Something went wrong while Serialize IndicationDTO package: {ex}");
            }

            return _sendStream.ToArray();
        }
    }
}
