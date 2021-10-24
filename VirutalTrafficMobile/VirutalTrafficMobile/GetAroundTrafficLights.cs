using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Essentials;

namespace VirutalTrafficMobile
{
    public class TrafficLight
    {
        Location _location;
        public Location Location
        {
            get
            {
                if (_location == null)
                    _location = new Location(Latitude, Longitude);
                return _location;
            }
        }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string ServerPath { get; set; }

        public List<int> LaneAngles { get; set; } = new List<int>();
    }


    public class GetAroundTrafficLights
    {
        private static Lazy<GetAroundTrafficLights> _lazy =
            new Lazy<GetAroundTrafficLights>(() => new GetAroundTrafficLights());

        public static List<TrafficLight> TrafficLights { get; set; } = new List<TrafficLight>();

        public bool IsTrafficLightInfoSet { get; set; } = false;


        public static GetAroundTrafficLights Setting
        {
            get
            {
                return _lazy.Value;
            }
        }

        public bool GetTrafficLightInfoFromConfigFile()
        {
            //some reason reletive path doesnot work (I think due to the visual studio's Xamarin's project bug)
            //var path = Directory.GetCurrentDirectory() + "\\Traffic.txt";

            try
            {
                //var fileStrings = File.ReadAllLines(path);
                var fileStrings = new string[] {
                    "-36.8680954528 174.753720021",
                    "0 90 180 270",
                    "ws://192.168.0.165:45459/"};
                    for (int i = 0; i < fileStrings.Length;)
                {
                    var coordinates = fileStrings[i].Trim().Split(' ');
                    var laneAngles = fileStrings[i+1].Trim().Split(' ');
                    var newTrafficLight = new TrafficLight()
                    {
                        Latitude = Convert.ToDouble(coordinates[0]),
                        Longitude = Convert.ToDouble(coordinates[1]),
                        ServerPath = fileStrings[i + 2],
                    };

                    for(int j = 0; j < laneAngles.Length; j++)
                        newTrafficLight.LaneAngles.Add(Convert.ToInt32(laneAngles[j]));
                    TrafficLights.Add(newTrafficLight);

                    i += 3;
                }
            }
            catch (DirectoryNotFoundException e)
            {
                return false;
            }
            catch (FileNotFoundException e)
            {
                return false;
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

    }
}
