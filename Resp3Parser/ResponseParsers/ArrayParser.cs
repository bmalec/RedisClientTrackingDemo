using RedisResp3Client.DataTypes;
using RedisResp3Client.ResponseParsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisResp3Client
{
  class ArrayParser : IResponseParser, IAggregateResponseParser
  {
    private byte _lastByte = 0x00;
    private int _arraySize = 0;

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
            Result = new RespArray(_arraySize);
            return Result;
          }
          else
          {
            // throw resp protocol exception here
          }
        }

        if (currentByte != 0x0D)
        {
          _arraySize = 10 * _arraySize + currentByte - '0';

        }

        _lastByte = currentByte;
      }

      // if we got to here, we've been able to parse some of the string, but have not got to the CR/LF string
      // terminator.  So save what we have so far to the stringbuilder, and we'll complete the string later

      startingPosition = currentPosition;

      return null;
    }


     public RespAggregateDataType Result { get; private set; }
  }
}

