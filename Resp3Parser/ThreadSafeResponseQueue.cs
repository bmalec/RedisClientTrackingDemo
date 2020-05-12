using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisResp3Client
{
  public class ThreadSafeResponseQueue
  {
    private readonly Queue<RespDataType> _queue = new Queue<RespDataType>();

    public event EventHandler OnResponseAvailable;


    public void Enqueue(RespDataType respDataObject)
    {
      lock (_queue)
      {
        _queue.Enqueue(respDataObject);
      }

      if (OnResponseAvailable != null)
      {
        OnResponseAvailable(this, null);
      }
    }


    public RespDataType Dequeue()
    {
      RespDataType result = null;

      lock (_queue)
      {
        result = _queue.Dequeue();
      }

      return result;
    }

    public int Count { get {
        int count = 0;

        lock (_queue)
        {
          count = _queue.Count;
        }

        return count;

      }  }
  }
}
