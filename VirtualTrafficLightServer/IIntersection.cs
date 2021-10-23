﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtualTrafficLightCoreLibrary.Common;
using VirtualTrafficLightCoreLibrary.Server;

namespace VirtualTrafficLightServer
{
    public interface IIntersection
    {

        public Task<bool> AddVehicleOnLaneAsync(ServerChannel channel,VehicleDTO message);

        public Task<bool> RemoveVehicleOnLaneAsync(string channelPath, int lane);

        public void StartOperate();

    }
}
