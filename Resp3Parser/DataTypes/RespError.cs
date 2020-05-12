using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisResp3Client
{
  public class RespError : RespDataType
  {
    public string Value { get; private set; }
    public RespError(string text)
    {
      Value = text;
    }

    public override string ToString()
    {
      return Value;
    }
  }
}
