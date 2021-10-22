using System;
using System.Collections.Generic;
using System.Text;

namespace VirtualTrafficLightCoreLibrary.Common
{
    public readonly struct VehicleDTO
    {
        public double Speed { get; }
        
        public int ClosestLane { get; }

        public int Distance { get; }

        public bool IsDistanceShrinking { get; }

        public VehicleDTO(double speed = -1, int closestLane = -1, int distance =-1, bool isDistanceShrinking = false)
        {
            Speed = speed;
            ClosestLane = closestLane;
            Distance = distance;
            IsDistanceShrinking = isDistanceShrinking;
        }
    }
}
