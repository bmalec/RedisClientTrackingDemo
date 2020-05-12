using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.InteropServices;

namespace RedisResp3Client
{
  public class RespTcpConnection : IRespTcpClient
  {
    private const int RECEIVE_BUFFER_SIZE = 64 * 1024;

    private readonly SocketAsyncEventArgs[] _receiveEventArgs = new SocketAsyncEventArgs[2];

    private readonly IRespParser _respParser;
    private Socket _socket;

    private readonly Thread _sendThread;
    private readonly Thread _receiveThread;

    private readonly List<ArraySegment<byte>>[] _cmdQueues;
    private int _activeCmdQueueIndex;
    private object _cmdQueueLock = new object();
    private ManualResetEvent _sendSignal = new ManualResetEvent(false);
    private bool _isWaitingForSendSignal;

    private void TcpReceiveLoop()
    {
      var receiveBuffer = new byte[RECEIVE_BUFFER_SIZE];

      while (true)
      {
        var byteCount = _socket.Receive(receiveBuffer);

        _respParser.ParseRawData(receiveBuffer, 0, byteCount);
      }
    }


    private void TcpSendLoop()
    {
      while (true)
      {
        // check to see if any commands have been queued up and are ready to send

        int cmdQueueIndex;
        bool isDataAvailableToSend = false;

        lock (_cmdQueueLock)
        {
          cmdQueueIndex = _activeCmdQueueIndex;
          isDataAvailableToSend = _cmdQueues[cmdQueueIndex].Count > 0;
        }

        // If no commands are availabe to send, set our "is waiting" flag and wait until
        // we get a signal that data is available

        if (!isDataAvailableToSend)
        {
          _sendSignal.Reset();
          _isWaitingForSendSignal = true;
          _sendSignal.WaitOne();
          _isWaitingForSendSignal = false;
        }

        // swap active command queues so while we're transmitting one, the other one is available for clients to queue up more commands

        lock (_cmdQueueLock)
        {
          _activeCmdQueueIndex = (++_activeCmdQueueIndex) % _cmdQueues.Length;
        }

        _socket.Send(_cmdQueues[cmdQueueIndex]);

        _cmdQueues[cmdQueueIndex].Clear();
      }
    }


    public RespTcpConnection(IPEndPoint ipEndPoint, IRespParser respParser)
    {
      _cmdQueues = new List<ArraySegment<byte>>[2] { new List<ArraySegment<byte>>(), new List<ArraySegment<byte>>() };

      _respParser = respParser;

      _socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
      _socket.Connect(ipEndPoint);

      _sendThread = new Thread(new ThreadStart(TcpSendLoop));
      _receiveThread = new Thread(new ThreadStart(TcpReceiveLoop));

      _receiveThread.Start();
      _sendThread.Start();

      _isWaitingForSendSignal = true;
    }


    public void Send(byte[] buffer)
    {
      lock (_cmdQueueLock)
      {
        _cmdQueues[_activeCmdQueueIndex].Add(new ArraySegment<byte>(buffer));
      }

      if (_isWaitingForSendSignal)
      {
        _sendSignal.Set();
      }

    }
  }
}
