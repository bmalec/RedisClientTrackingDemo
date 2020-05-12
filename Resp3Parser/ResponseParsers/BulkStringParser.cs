using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisResp3Client
{
  class BulkStringParser : IResponseParser
  {
    private byte _lastByte = 0x00;
    private int _dataLength = 0;
    private byte[] _destBuffer;
    private int _currentDestPosition;


    public RespDataType ParseBytes(byte[] srcData, ref int startingPosition, int lastBytePosition)
    {
      if (srcData == null)
      {
        throw new ArgumentNullException(nameof(srcData));
      }

      int currentSrcPosition = startingPosition;

      byte currentByte = 0x00;

      while (currentSrcPosition <= lastBytePosition)
      {
        if (_destBuffer != null)
        {
          int byteCountNeeded = _destBuffer.Length - _currentDestPosition;

          if (byteCountNeeded > 0)
          {
            int byteCountAvailable = lastBytePosition - currentSrcPosition + 1;

            int length = (byteCountAvailable < byteCountNeeded) ? byteCountAvailable : byteCountNeeded;

            Buffer.BlockCopy(srcData, currentSrcPosition, _destBuffer, _currentDestPosition, length);
            _currentDestPosition += length;
            currentSrcPosition += length;
            currentByte = 0x00;
          }
          else
          {
            currentByte = srcData[currentSrcPosition++];

            if (currentByte == 0x0A)
            {
              if (_lastByte == 0x0D)
              {
                startingPosition = currentSrcPosition;
                return new RespBulkString(_destBuffer);
              }
              else
              {
                // throw resp protocol exception here
              }
            }
          }
        }
        else
        {
          currentByte = srcData[currentSrcPosition++];

          if (currentByte == 0x0A)
          {
            if (_lastByte == 0x0D)
            {
              _destBuffer = new byte[_dataLength];
              continue;
            }
            else
            {
              // throw resp protocol exception here
            }
          }

          if (currentByte != 0x0D)
          {
            _dataLength = 10 * _dataLength + currentByte - '0';
          }
        }

        _lastByte = currentByte;
      }

      // if we got to here, we've been able to parse some of the string, but have not got to the CR/LF string
      // terminator.  So save what we have so far to the stringbuilder, and we'll complete the string later

      startingPosition = currentSrcPosition;

      return null;
    }



  }
}

