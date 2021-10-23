using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtualTrafficCoreLibrary.Server;

namespace VirtualTrafficLightServer
{
    public class MessageHandler
    {

        /// <summary>
        /// Bind the Channel so when Channel gets message this can get the flow of the messages handle.
        /// </summary>
        /// <param name="channel">Channel</param>
        public void Bind(ServerChannel channel)
            => channel.OnMessage(async (m, channelPath) => {

                if(m.Distance <= 100)
                {
                    channel.LaneNumber = m.ClosestLane;
                    await CrossIntersection.Intersection.AddVehicleOnLaneAsync(channel,m);
                    return;
                }

                if (m.IsDistanceShrinking == false)
                {
                    await CrossIntersection.Intersection.RemoveVehicleOnLaneAsync(channelPath,channel.LaneNumber);
                    await channel.CloseAsync();
                    return;
                }

            });
    }
}
