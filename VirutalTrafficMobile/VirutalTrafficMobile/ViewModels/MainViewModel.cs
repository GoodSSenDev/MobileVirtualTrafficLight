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

        private string _circleColour;
        private string _receivedMessage;
        private string _sentMessage;
        public string CircleColour 
        {  
            get { return _circleColour; }
            set
            {
                _circleColour = value;
                OnPropertyChanged();
            }
        }
        public string SentMessage
        {
            get { return _sentMessage; }
            set
            {
                _sentMessage = value;
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

        public MainViewModel()
        {
            TrafficLightIndicator.Indicator.OnMessage = HandleMessage;
        }

        private void HandleMessage(IndiationDTO mesage)
        {

        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
