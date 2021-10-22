using System;
using System.Collections.Generic;
using System.Text;

namespace VirutalTrafficMobile
{
    public class TrafficLightIndicator
    {
        private static Lazy<TrafficLightIndicator> _lazy =
            new Lazy<TrafficLightIndicator>(() => new TrafficLightIndicator());

        private TrafficLightIndicator()
        {

        }

        public static TrafficLightIndicator Indicator
        {
            get
            {
                return _lazy.Value;
            }
        }



    }
}
