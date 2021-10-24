using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtualTrafficCoreLibrary.Common;
using VirtualTrafficCoreLibrary.Server;

namespace VirtualTrafficLightServer
{
    public class CrossIntersection : IIntersection
    {
        private static Lazy<CrossIntersection> _lazy =
            new Lazy<CrossIntersection>(() => new CrossIntersection());

        private List<ServerChannel>[] _lanes = new List<ServerChannel>[] { new List<ServerChannel>(), new List<ServerChannel>(), new List<ServerChannel>(), new List<ServerChannel>() };
        /// <summary>
        /// traffic Duration in seconds
        /// </summary>
        private double _trafficDuration = 7;
        private TrafficIndicationMode _mode = TrafficIndicationMode.LaneOneThreeGo;

        //CallcelationTokenSource is there for controlling Task(thread auto ThreadPool) operation.
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private Task _operationLoopTask;
        private IndiationDTO _stopIndication = new IndiationDTO(0, TrafficLightColor.RED);
        private IndiationDTO _goIndication = new IndiationDTO(0, TrafficLightColor.GREEN);

        public static CrossIntersection Intersection
        {
            get
            {
                return _lazy.Value;
            }
        }

        /// <summary>
        /// Adding Vehicles On lane 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<bool> AddVehicleOnLaneAsync(ServerChannel channel, VehicleDTO message)
        {
            if (message.ClosestLane > _lanes.Length)
                return false;

            if(message.Distance > 100)
                return false;

            _lanes[message.ClosestLane - 1].Add(channel);

            if (_mode == TrafficIndicationMode.LaneOneThreeGo)
                if (message.ClosestLane == 1 || message.ClosestLane == 3)
                    await channel.SendAsync(_goIndication);
                else
                    await channel.SendAsync(_stopIndication);
            else
                 if (message.ClosestLane == 2 || message.ClosestLane == 4)
                await channel.SendAsync(_goIndication);
            else
                await channel.SendAsync(_stopIndication);

            return true;
        }

        /// <summary>
        /// remove the vehicle on lane
        /// </summary>
        /// <param name="channelPath"></param>
        /// <param name="lane"></param>
        /// <returns>return flase if cannot removing false</returns>
        public async Task<bool> RemoveVehicleOnLaneAsync(string channelPath,int lane)
        {
            if (lane <= 0 || lane > _lanes.Length)
                return false;
            for(int i = 0; i < _lanes[lane-1].Count; i++)
            {
                if (_lanes[lane - 1][i].ChannelPath == channelPath)
                {
                    _lanes[lane - 1].RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Starts the Virtual Traffic light
        /// </summary>
        public void StartOperate()
        {
            _operationLoopTask = Task.Run(OperationLoop, _cancellationTokenSource.Token);
        }

        /// <summary>
        /// Keep Operating the Virtual traffic light 
        /// </summary>
        /// <returns></returns>
        private async Task OperationLoop()
        {

            List<Task> tasks = new List<Task>();
            while (true)
            {
                await Task.Delay((int)(_trafficDuration * 1000));
                if (_mode == TrafficIndicationMode.LaneOneThreeGo)
                    _mode = TrafficIndicationMode.LaneTwoFourGo;
                else
                    _mode = TrafficIndicationMode.LaneOneThreeGo;

                tasks.Add(Task.Run(async () => {
                    if(_mode == TrafficIndicationMode.LaneOneThreeGo)
                        for(int i = 0; i < _lanes[0].Count; i++)
                        {
                            await _lanes[0][i].SendAsync(_goIndication);
                        }
                    else
                        for (int i = 0; i < _lanes[0].Count; i++)
                        {
                            await _lanes[0][i].SendAsync(_stopIndication);
                        }
                }));

                tasks.Add(Task.Run(async () => {
                    if (_mode == TrafficIndicationMode.LaneTwoFourGo)
                        for (int i = 0; i < _lanes[1].Count; i++)
                        {
                            await _lanes[1][i].SendAsync(_goIndication);
                        }
                    else
                        for (int i = 0; i < _lanes[1].Count; i++)
                        {
                            await _lanes[1][i].SendAsync(_stopIndication);
                        }
                }));

                tasks.Add(Task.Run(async () => {
                    if (_mode == TrafficIndicationMode.LaneOneThreeGo)
                        for (int i = 0; i < _lanes[2].Count; i++)
                        {
                            await _lanes[2][i].SendAsync(_goIndication);
                        }
                    else
                        for (int i = 0; i < _lanes[2].Count; i++)
                        {
                            await _lanes[2][i].SendAsync(_stopIndication);
                        }
                }));

                tasks.Add(Task.Run(async () => {
                    if (_mode == TrafficIndicationMode.LaneTwoFourGo)
                        for (int i = 0; i < _lanes[3].Count; i++)
                        {
                            await _lanes[3][i].SendAsync(_goIndication);
                        }
                    else
                        for (int i = 0; i < _lanes[3].Count; i++)
                        {
                            await _lanes[3][i].SendAsync(_stopIndication);
                        }
                }));

                await Task.WhenAll(tasks);

                tasks.Clear();
            }
        }


    }

    public enum TrafficIndicationMode
    {
        LaneOneThreeGo,
        LaneTwoFourGo,
    }
}
