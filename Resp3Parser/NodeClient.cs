using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;


namespace RedisResp3Client
{

  public class PendingCommand
  {
    public Task<RespDataType> Task { get { return TaskCompletionSource.Task; } }

    public TaskCompletionSource<RespDataType> TaskCompletionSource { get; private set; }

    public PendingCommand()
    {
      TaskCompletionSource = new TaskCompletionSource<RespDataType>();
    }


  }

  public class NodeClient
  {
    private const int RECEIVE_BUFFER_SIZE = 512 * 1024;

    private readonly Mutex _parserMutex = new Mutex();

    private readonly ThreadSafeResponseQueue _responseQueue;
    private readonly Resp2Parser _respParser;
    private readonly IRespTcpClient _redisTcpConnection;
    private readonly Queue<PendingCommand> _pendingCommandQueue = new Queue<PendingCommand>();
    private readonly CommandBuilder _cmdBuilder = new CommandBuilder();



    public NodeClient(IPEndPoint endpoint)
    {
      /*
       _receiveEventArgs[0] = new SocketAsyncEventArgs();
            _receiveEventArgs[1] = new SocketAsyncEventArgs();

            _receiveEventArgs[0].Completed += ReceiveCallback;
            _receiveEventArgs[1].Completed += ReceiveCallback;

            _receiveEventArgs[0].SetBuffer(new byte[RECEIVE_BUFFER_SIZE], 0, RECEIVE_BUFFER_SIZE);
            _receiveEventArgs[1].SetBuffer(new byte[RECEIVE_BUFFER_SIZE], 0, RECEIVE_BUFFER_SIZE);

            _socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            _socket.Connect(endpoint);

        */

      _responseQueue = new ThreadSafeResponseQueue();
      _responseQueue.OnResponseAvailable += OnResponseQueuedCallback;

      _respParser = new Resp2Parser(_responseQueue);

      _redisTcpConnection = new RespTcpConnection(endpoint, _respParser);

      // configure the Redis connection for RESP3


      var bytes = _cmdBuilder.GetCommandBytes("HELLO", "3");

      var pendingCmd = new PendingCommand();

      lock (_pendingCommandQueue)
      {
        _pendingCommandQueue.Enqueue(pendingCmd);
      }

      _redisTcpConnection.Send(bytes);

      pendingCmd.Task.Wait();

      int j = 1;


    }



    public void OnResponseQueuedCallback(object o, EventArgs e)
    {
      var response = _responseQueue.Dequeue();

      PendingCommand pendingCmd = null;

      lock (_pendingCommandQueue)
      {
        pendingCmd = _pendingCommandQueue.Dequeue();
      }

      if (pendingCmd != null)
      {
        if (response is RespSimpleString)
        {
          if (response.ToString() == "OK")
          {
            pendingCmd.TaskCompletionSource.SetResult(response);
          }
          else
          {
            pendingCmd.TaskCompletionSource.SetException(new Exception("bad bad bad"));
          }
        }
        else if (response is RespError)
        {
          pendingCmd.TaskCompletionSource.SetException(new Exception(response.ToString()));
        }
        else
        {
          pendingCmd.TaskCompletionSource.SetResult(response);
        }
      }



      int k = 1;
    }

    public Task<RespDataType> Keys()
    {
      string redisCmd = "keys *\r\n";
      var buffer = Encoding.ASCII.GetBytes(redisCmd);

      var pendingCmd = new PendingCommand();

      lock (_pendingCommandQueue)
      {
        _pendingCommandQueue.Enqueue(pendingCmd);
      }

      _redisTcpConnection.Send(buffer);

      return pendingCmd.Task;
    }


    public void dosomething()
    {
      string redisCmd = "keys *\r\n";
      var buffer = Encoding.ASCII.GetBytes(redisCmd);

      _redisTcpConnection.Send(buffer);
    }


    public Task Set(string key, string value)
    {
      var bytes = _cmdBuilder.GetCommandBytes("SET", key, value);

      var pendingCmd = new PendingCommand();

      lock (_pendingCommandQueue)
      {
        _pendingCommandQueue.Enqueue(pendingCmd);
      }

      _redisTcpConnection.Send(bytes);

      return pendingCmd.Task;
    }








  }
}
