using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisResp3Client
{
  public class RespSimpleString : RespDataType
  {
    public string Value { get; private set; }
    public RespSimpleString(string text)
    {
      Value = text;
    }

    public override string ToString()
    {
      return Value;
    }
      
  }
}
