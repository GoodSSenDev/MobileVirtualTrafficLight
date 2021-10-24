using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using VirtualTrafficCoreLibrary.Common;

namespace VirutalTrafficMobile.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _circleColour="grey";
        private string _receivedMessage;
        private bool _isSwitchOn;
        public string CircleColour 
        {  
            get { return _circleColour; }
            set
            {
                _circleColour = value;
                OnPropertyChanged();
            }
        }
        public string ReceivedMessage
        {
            get { return _receivedMessage; }
            set
            {
                _receivedMessage = value;
                OnPropertyChanged();
            }
        }

        public bool IsSwitchOn
        {
            get { return _isSwitchOn; }
            set
            {
                if(_isSwitchOn != value)
                {
                    _isSwitchOn = value;
                    if (value)
                    {
                        TrafficLightIndicator.Indicator.StartOperation();
                    }
                    else
                    {
                        TrafficLightIndicator.Indicator.StopOperation();
                    }
                }

                _isSwitchOn = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            GetAroundTrafficLights.Setting.GetTrafficLightInfoFromConfigFile();
            TrafficLightIndicator.Indicator.OnMessage = HandleMessage;
        }

        private void HandleMessage(IndiationDTO message)
        {
            if (message.TrafficIndication == TrafficLightColor.RED)
                CircleColour = "red";
            if (message.TrafficIndication == TrafficLightColor.GREEN)
                CircleColour = "green";

            ReceivedMessage = $"Received Message TrafficLightColor: {message.TrafficIndication}";
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
