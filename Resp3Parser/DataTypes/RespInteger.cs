using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisResp3Client
{
  public class RespInteger : RespDataType
  {
    public long Value { get; private set; }


    public RespInteger(long value)
    {
      Value = value;
    }

  }
}
