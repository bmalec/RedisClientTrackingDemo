using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisResp3Client
{
  class IntegerParser : IResponseParser
  {
    private byte _lastByte = 0x00;
    private long _value = 0;

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
          if (_lastByte == 0x0D)
          {
            startingPosition = currentPosition;
            return new RespInteger(_value);
          }
          else
          {
            // throw resp protocol exception here
          }
        }

        if (currentByte != 0x0D)
        {
          _value = 10 * _value + currentByte - '0';
        }

        _lastByte = currentByte;
      }

      // if we got to here, we've been able to parse some of the string, but have not got to the CR/LF string
      // terminator.  So save what we have so far to the stringbuilder, and we'll complete the string later

      startingPosition = currentPosition;

      return null;
    }
  }
}

