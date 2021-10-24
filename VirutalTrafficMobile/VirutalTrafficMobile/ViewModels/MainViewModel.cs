using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using VirtualTrafficCoreLibrary.Common;
using Xamarin.Forms;

namespace VirutalTrafficMobile.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Brush _circleColour = new SolidColorBrush(Color.Gray);
        private string _receivedMessage;
        private bool _isSwitchOn;
        public Brush CircleColour 
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
            TrafficLightIndicator.Indicator.OnChannelClosing = ColorChangeWhenClose;
        }

        private void HandleMessage(IndiationDTO message)
        {
            if (message.TrafficIndication == TrafficLightColor.RED)
                CircleColour = new SolidColorBrush(Color.Red);
            if (message.TrafficIndication == TrafficLightColor.GREEN)
                CircleColour = new SolidColorBrush(Color.Green);

            ReceivedMessage = $"Received Message TrafficLightColor: {message.TrafficIndication}";
        }
        private void ColorChangeWhenClose()
        {
            ReceivedMessage = $"Received Message channel closed";
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
