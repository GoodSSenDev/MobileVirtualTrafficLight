using Newtonsoft.Json.Linq;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VirtualTrafficLightCoreLibrary
{
    public abstract class Channel<SendingDataTrasferObject, ReceivingataTrasferObject> : IAsyncDisposable, IDisposable 
    {
        protected bool _isDisposed = false;
        protected bool _isClosed = false;

        protected readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        protected WebSocket _webSocket;

        protected Func<ReceivingataTrasferObject, string, Task> _messageCallback;

        protected Task _receiveLoopTask;

        public event EventHandler Closed;
        public string ChannelPath { get; set; } = string.Empty;

        public void Attach(WebSocket webSocket)
        {
            _webSocket = webSocket;
            _receiveLoopTask = Task.Run(ReceiveLoop, _cancellationTokenSource.Token);
        }

        public void OnMessage(Func<ReceivingataTrasferObject, string, Task> callbackHandler)
            => _messageCallback = callbackHandler;

        /// <summary>
        /// Sends a message using the WebSocket
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendAsync(SendingDataTrasferObject message)
        {
            if (_isClosed)
            {
                return;
            }
            try
            {
                await _webSocket.SendAsync(new ArraySegment<byte>(Serialize(message))
                    , WebSocketMessageType.Text,
                    true, _cancellationTokenSource.Token).ConfigureAwait(false);

            }
            catch (Exception e)
            {
                return;
            }
        }

        /// <summary>
        /// Receive loop and constantly received and start process the received message,.
        /// 
        /// If exception occurs, then close this connection  
        /// </summary>
        /// <returns></returns>
        protected virtual async Task ReceiveLoop()
        {
            try
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    var receiveBuffer = new byte[1024 * 4];
                    var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), _cancellationTokenSource.Token);
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        await _messageCallback(Deserialize(receiveBuffer), ChannelPath)
                            .ConfigureAwait(false);
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await CloseAsync();
                        break;
                    }
                }
            }
            catch (System.IO.IOException e)
            {
                await CloseAsync(e);
            }
            catch (Exception e)//need to be considered
            {
                await CloseAsync(e);
            }
        }

        /// <summary>
        /// Close this channel including attached socket with error 
        /// </summary>
        public async Task CloseAsync(Exception e)
        {
            if (!_isClosed)
            {
                _isClosed = true;
                if (_webSocket?.State == WebSocketState.Open)
                {
                    await _webSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, $"Socket In {e} CMS is closed",
                       CancellationToken.None).ConfigureAwait(false);
                }
                _cancellationTokenSource.Cancel();
                Closed?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Close this channel including attached socket 
        /// </summary>
        public async Task CloseAsync()
        {
            if (!_isClosed)
            {
                _isClosed = true;
                if (_webSocket?.State == WebSocketState.Open)
                {
                    await _webSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Socket In CMS is closed",
                       CancellationToken.None).ConfigureAwait(false);
                }
                _cancellationTokenSource.Cancel();
                Closed?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Serializes SendingDTO to byte[]
        /// </summary>
        /// <param name="data">SendingDataTrasferObject</param>
        /// <returns>Sending byte[]</returns>
        public abstract byte[] Serialize(SendingDataTrasferObject data);

        /// <summary>
        /// Deserialzing byte[] to ReceivingataTrasferObject
        /// </summary>
        /// <param name="data">Received byte[]</param>
        /// <returns>ReceivingataTrasferObject</returns>
        public abstract ReceivingataTrasferObject Deserialize(byte[] data);


        #region Dispose

        //Finalizer
        ~Channel() => Dispose(false);

        //Dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);//avoid calling finalizer
        }

        /// <summary>
        /// Disposeing 
        /// </summary>
        /// <param name="isDisposing">True: for Disposing both unmanaged and managed resources,
        /// False: for disposing only unmanged resources</param>
        protected virtual void Dispose(bool isDisposing)
        {
            if (_isDisposed)
                return;

            if (isDisposing)
            {
                CloseAsync().GetAwaiter().GetResult();
                _webSocket?.Dispose();
            }
            _isDisposed = true;
        }

        /// <summary>
        /// Dispose Async
        /// </summary>
        /// <returns></returns>
        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore();

            Dispose(false);
            GC.SuppressFinalize(this);//avoid calling finalizer
        }

        /// <summary>
        /// Close Async
        /// </summary>
        /// <returns></returns>
        protected virtual async ValueTask DisposeAsyncCore()
        {
            if (_isDisposed)
                return;
            await CloseAsync();
            _webSocket.Dispose();
        }

        #endregion.

    }
}
