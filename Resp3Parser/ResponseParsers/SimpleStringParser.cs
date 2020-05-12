using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisResp3Client
{
  public class SimpleStringParser : IResponseParser
  {
    private StringBuilder _sb = new StringBuilder();
    private byte lastByte = 0x00;


    public RespDataType ParseBytes(byte[] data, ref int startingPosition, int lastBytePosition)
    {
      if (data == null)
      {
        throw new ArgumentNullException(nameof(data));
      }
      

      int currentPosition = startingPosition;

      while (currentPosition <= lastBytePosition)
      {
        var currentByte = data[currentPosition++];

        if (currentByte == 0x0A)
        {
          if (lastByte == 0x0D)
          {
            _sb.Append(Encoding.ASCII.GetString(data, startingPosition, currentPosition - startingPosition - 2));
            startingPosition = currentPosition;
            return new RespSimpleString(_sb.ToString());
          }
          else
          {
            // throw resp protocol exception here
          }
        }

        lastByte = currentByte;
      }

      // if we got to here, we've been able to parse some of the string, but have not got to the CR/LF string
      // terminator.  So save what we have so far to the stringbuilder, and we'll complete the string later

      _sb.Append(Encoding.ASCII.GetString(data, startingPosition, currentPosition - startingPosition));
      startingPosition = currentPosition;

      return null;
    }
  }
}

  
