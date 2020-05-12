using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisResp3Client
{
  interface IRespTcpClient
  {
    void Send(byte[] buffer);
  }
}
