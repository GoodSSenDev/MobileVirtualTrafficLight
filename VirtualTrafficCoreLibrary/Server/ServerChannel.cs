using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VirtualTrafficCoreLibrary.Common;

namespace VirtualTrafficCoreLibrary.Server
{
    public class ServerChannel : Channel<IndiationDTO,VehicleDTO>
    {
        /// <summary>
        /// Memoery stream for sending messages 
        /// </summary>
        readonly MemoryStream _sendStream = new MemoryStream(8);
        readonly BinaryWriter _sendWriter;

        /// <summary>
        /// Memoery stream for reading messages 
        /// </summary>
        readonly MemoryStream _receiveStream = new MemoryStream(Marshal.SizeOf<VehicleDTO>());
        readonly BinaryReader _receiveReader;

        public int LaneNumber { get; set; } = 0;

        public ServerChannel()
        {
            _sendWriter = new BinaryWriter(_sendStream);
            _receiveReader = new BinaryReader(_receiveStream);
        }

        public override VehicleDTO Deserialize(byte[] data)
        {
            try
            {
                _receiveStream.Position = 0;
                _receiveStream.Write(data);
                _receiveStream.Position = 0;
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Something went wrong while Deserializing VehicleDTO package: {ex}");
            }
            var speed = _receiveReader.ReadDouble();
            var closestLane = _receiveReader.ReadInt32();
            var distance = _receiveReader.ReadInt32();
            var isDistanceShrinking = _receiveReader.ReadBoolean();
            var returnDTO = new VehicleDTO(speed, closestLane, distance, isDistanceShrinking);
            return returnDTO;
        }

        public override byte[] Serialize(IndiationDTO data)
        {
            try
            {
                _sendStream.Position = 0;
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
