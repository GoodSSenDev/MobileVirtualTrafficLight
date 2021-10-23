using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using VirtualTrafficLightCoreLibrary.Server;

namespace VirtualTrafficLightServer
{
    public class ChannelManager
    {
        public static readonly ConcurrentDictionary<string, ServerChannel> _channels
            = new ConcurrentDictionary<string, ServerChannel>();

        public int ChannelCount => _channels.Count;

        public event EventHandler? ChannelAccepted;
        public event EventHandler? ChannelClosed;

        private MessageHandler _messageHandler = new MessageHandler();

        /// <summary>
        /// Tries adding new ServerChannel into channels' concurrent dictionary 
        /// returns to middleware pipeline so the websocket in the channel can get disposed 
        /// </summary>
        /// <param name="webSocket">This goes inside a new ServerChannel</param>]
        /// <param name="uri">uri</param>
        /// <param name="socketFinishedTcs">For checking when the SocketFinished</param>
        public void AddConnection(WebSocket webSocket, string uri, TaskCompletionSource<bool> socketFinishedTcs)
        {
            //TODO: IF XML way is added, then implement a factory pattern to make both XML channel or Json channel.
            var channel = new ServerChannel();
            _channels.TryAdd(uri, channel);
            channel.Closed += (s, e) => {
                _channels.TryRemove(uri, out var _);
                //returns to middleware pipeline so the websocket in the channel can get disposed 
                socketFinishedTcs.TrySetResult(true);
                ChannelClosed?.Invoke(this, EventArgs.Empty);
            };
            channel.ChannelPath = uri;
            channel.Attach(webSocket);

            _messageHandler.Bind(channel);
            ChannelAccepted?.Invoke(this, EventArgs.Empty);
        }
    }
}
