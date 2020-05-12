using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisResp3Client
{
  public interface IRespParser
  {
    void ParseRawData(byte[] data, int startPosition, int length);
  }
}
