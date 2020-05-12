using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisResp3Client
{
  internal interface IResponseParser
  {
    RespDataType ParseBytes(byte[] bytes, ref int startingPosition, int lastBytePosition);
  }
}
