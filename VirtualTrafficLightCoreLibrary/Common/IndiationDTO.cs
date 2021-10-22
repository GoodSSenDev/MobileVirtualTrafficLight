using System;
using System.Collections.Generic;
using System.Text;

namespace VirtualTrafficLightCoreLibrary.Common
{
    /// <summary>
    /// Represent a message package that can be used to send from the server to the vehicle
    /// </summary>
    public readonly struct IndiationDTO
    {
        //Any Speical Commands, for example 0 means normal situation 1 means all stop 2 means warning... (for preventing unexpected situation)
        public int SpecialCommand { get; }

        public TrafficLightColor TrafficIndication { get; }

        public IndiationDTO(int specialCommand, TrafficLightColor trafficIndication)
        {
            SpecialCommand = specialCommand;
            TrafficIndication = trafficIndication;
        }

        public IndiationDTO(int specialCommand, int trafficIndication)
        {
            SpecialCommand = specialCommand;
            TrafficIndication = (TrafficLightColor)trafficIndication;
        }
    }


    /// <summary>
    /// Enum of TrafficLight Color, Yellow is not included, Since the red sign can work was yellow -> red
    /// </summary>
    public enum TrafficLightColor
    {
        RED,
        GREEN,
    }
}
