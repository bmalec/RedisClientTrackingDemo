using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RedisResp3Client
{
  public class CommandBuilder
  {
    public byte[] GetCommandBytes(string cmd, string param1, string param2)
    {
      return Encoding.ASCII.GetBytes($"*3\r\n${cmd.Length}\r\n{cmd}\r\n${param1.Length}\r\n{param1}\r\n${param2.Length}\r\n{param2}\r\n");
    }

    public byte[] GetCommandBytes(string cmd, string param1)
    {
      return Encoding.ASCII.GetBytes($"*2\r\n${cmd.Length}\r\n{cmd}\r\n${param1.Length}\r\n{param1}\r\n");
    }
  }
}
