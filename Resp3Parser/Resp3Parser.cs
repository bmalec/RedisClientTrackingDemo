using RedisResp3Client.DataTypes;
using RedisResp3Client.ResponseParsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisResp3Client
{
  public class Resp2Parser : IRespParser
  {
    private readonly ThreadSafeResponseQueue _responseQueue;
    private readonly Stack<IResponseParser> _pendingParserStack = new Stack<IResponseParser>();
    private readonly object _parsingLock = new object();


    public Resp2Parser(ThreadSafeResponseQueue responseQueue)
    {
      _responseQueue = responseQueue;
    }



    private static IResponseParser GetParserForType(byte typeByte)
    {
      switch (typeByte)
      {
        case 0x24:
          return new BulkStringParser();

        case 0x2A:
          return new ArrayParser();

        case 0x25:
          return new MapParser();

        case 0x2B:
          return new SimpleStringParser();

        case 0x2D:
          return new ErrorParser();


        case 0x3A:
          return new IntegerParser();
      }

      return null;
    }


    public void ParseRawData(byte[] data, int startPosition, int length)
    {
      if (data == null) throw new ArgumentNullException(nameof(data));
      if (data.Length == 0) throw new ArgumentOutOfRangeException(nameof(data), "Empty array");
      if ((startPosition < 0) || (startPosition >= data.Length)) throw new ArgumentOutOfRangeException(nameof(startPosition));
      if ((length < 1) || ((startPosition + length - 1) > data.Length)) throw new ArgumentOutOfRangeException(nameof(length));

      int currentPosition = startPosition;
      int lastBytePosition = startPosition + length - 1;

      while (currentPosition <= lastBytePosition)
      {
        IResponseParser responseParser = null;

        if (_pendingParserStack.Count == 0)
        {
          // no incomplete parsing operations in-flight, so read the type byte and instantiate the correct parser

          responseParser = GetParserForType(data[currentPosition++]);
          _pendingParserStack.Push(responseParser);
        }
        else
        {
          // we have an unfinished parsing operation in progress, so continue with it

          responseParser = _pendingParserStack.Peek();

          if (responseParser is IAggregateResponseParser)
          {
            if (((IAggregateResponseParser)responseParser).Result != null)
            {
              responseParser = GetParserForType(data[currentPosition++]);
              _pendingParserStack.Push(responseParser);
            }
          }
        }

        // only call the parser if there's data to actually parse

        if (currentPosition > lastBytePosition) continue;

        var parserResult = responseParser.ParseBytes(data, ref currentPosition, lastBytePosition);

        // if the parser returns null then there was not enough data available to finish parsing the RESP data type
        // so leave the parser on the stack so we can feed it more data when we receive it

        if (parserResult == null) continue;

        // otherwise, do array logic:

        if (!(parserResult is RespAggregateDataType) || ((parserResult is RespAggregateDataType) && (((RespAggregateDataType) parserResult).IsComplete)))
        {
          _pendingParserStack.Pop();

          // check if there is still an array parser on the stack,
          // if so that means the item we just parsed was part of an array
          // and we should add it to the array

          while (_pendingParserStack.Count > 0)
          {
            var aggregateParser = (IAggregateResponseParser)_pendingParserStack.Peek();

            aggregateParser.Result.AppendElement(parserResult);

            if (aggregateParser.Result.IsComplete)
            {
              parserResult = aggregateParser.Result;
              _pendingParserStack.Pop();
            }
            else
            {
              parserResult = null;
              break;
            }
          }

          if (parserResult != null)
          {
            _responseQueue.Enqueue(parserResult);
          }

        }
      }







    }

  }
}

