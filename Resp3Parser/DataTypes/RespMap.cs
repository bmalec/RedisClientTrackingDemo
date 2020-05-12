using RedisResp3Client.DataTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace RedisResp3Client
{
  public class RespMap : RespAggregateDataType
  {
    private readonly RespDataType[] _array;
    private bool _isComplete = false;
    private int _currentIndex = 0;

    public override bool IsComplete { get { return _isComplete; } }


    public RespMap(int keyValuePairCount)
    {
      _array = new RespDataType[keyValuePairCount << 1];
    }

    public override void AppendElement(RespDataType respDataElement)
    {
      _array[_currentIndex++] = respDataElement;

      _isComplete = (_currentIndex == _array.Length);
    }

    public int Length { get { return _array.Length >> 1; } }

    public RespDataType this[int index]
    {
      get { return _array[index]; }
    }

  }
}
