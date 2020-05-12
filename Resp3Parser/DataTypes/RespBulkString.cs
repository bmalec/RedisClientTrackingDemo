using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisResp3Client
{
  public class RespBulkString : RespDataType
  {
    public byte[] Value { get; private set; }
    public RespBulkString(byte[] value)
    {
      Value = value;
    }
  }
}
