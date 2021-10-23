﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VirtualTrafficCoreLibrary.Client;
using VirtualTrafficCoreLibrary.Common;
using Xamarin.Essentials;

namespace VirutalTrafficMobile
{
    public class TrafficLightIndicator
    {
        private static Lazy<TrafficLightIndicator> _lazy =
            new Lazy<TrafficLightIndicator>(() => new TrafficLightIndicator());
        private double CheckingDuration { get; set; } = 3;
        private bool IsOperating { get; set; } = false;
        private Location _lastlocation;
        private int _lastDistance;
        private ClientChannel _channel;

        /// <summary>
        /// Delegate for when Message arrived
        /// </summary>
        public Action<IndiationDTO> OnMessage { get; set; }

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

        public async Task Operation()
        {
            TrafficLight trafficLightInfo = null;
            while (IsOperating)
            {
                trafficLightInfo = await CheckAvailableConnection();
                if (trafficLightInfo == null)
                    continue;

                if (!await RequestConnection(trafficLightInfo.ServerPath))
                    continue;
                else
                    break;
            }
            var lane = GetLaneNumber(trafficLightInfo.LaneAngles, trafficLightInfo.Latitude, trafficLightInfo.Longitude);

            await SendLocationInfo(lane);      
        }

        private async Task SendLocationInfo(int lane)
        {
            if (_lastlocation == null)
                return;
            var sendingDTO = new VehicleDTO(Convert.ToDouble(_lastlocation.Speed), lane, _lastDistance, true);

            await _channel.SendAsync(sendingDTO);
            
        }

        /// <summary>
        /// This methods gets the lane number using simple trigonometric
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns>return 0 if fail</returns>
        private int GetLaneNumber(List<int> laneAngles, double latitude, double longitude)
        {
            var x = _lastlocation.Latitude - latitude;
            var y = _lastlocation.Longitude - longitude;

            var deg = Math.Atan2(y, x) * 180.0 / Math.PI;

            var lane = 0;
            double difference = 360;

            for(int i = 0; i < laneAngles.Count; i++)
            {
                var currentDifferece = Math.Abs(laneAngles[i] - deg);
                if (currentDifferece < difference)
                {
                    lane = i+1;
                    difference = currentDifferece;
                }
            }
            return lane;
        }

        public async Task<TrafficLight> CheckAvailableConnection()
        {

            while (IsOperating)
            {
                await Task.Delay((int)(CheckingDuration * 1000));

                var location = await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.High));
                
                for(int i = 0; i < GetAroundTrafficLights.Indicator.TrafficLights.Count; i++)
                {
                    var distanceMeter = Location.CalculateDistance(location,GetAroundTrafficLights.Indicator.TrafficLights[i].Location,DistanceUnits.Kilometers)/1000;
                    if (distanceMeter <= 100)
                    {
                        _lastlocation = location;
                        _lastDistance = (int)distanceMeter;
                        return GetAroundTrafficLights.Indicator.TrafficLights[i];
                    }
                }
            }

            return null;
        }

        public async Task<bool> RequestConnection(string serverPath)
        {
            try
            {
                var clientSocket = new ClientWebSocket();
                await clientSocket.ConnectAsync(new Uri(serverPath), CancellationToken.None);

                _channel = new ClientChannel();
                _channel.Attach(clientSocket);

                //_channel.Closed

                _channel.OnMessage(async (m, serverPath) =>
                {
                    OnMessage?.Invoke(m);
                });

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception occured on connecting {uri}:\n RequestConnection: {ex.Message}", serverPath, ex.Message);
                return false;
            }
        }

    }
}
