using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtualTrafficLightCoreLibrary.Server;

namespace VirtualTrafficLightServer
{
    public class MessageHandler
    {

        /// <summary>
        ///  Bind the Channel so when they gets the message they can wo
        /// </summary>
        /// <param name="channel">Channel</param>
        public void Bind(ServerChannel channel)
            => channel.OnMessage(async (m, channelPath) => {

                if(m.Distance <= 100)
                {
                    //add lane
                    //return 
                }


                if (m.IsDistanceShrinking == false)
                {
                    //remove
                }

            });
    }
}
