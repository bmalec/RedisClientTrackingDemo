using RedisResp3Client.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisResp3Client
{

  public class RespArray : RespAggregateDataType
  {
    private readonly RespDataType[] _array;
    private int _currentIndex = 0;

    public override bool IsComplete { get { return _currentIndex == _array.Length; } }


    public RespArray(int arraySize)
    {
      _array = new RespDataType[arraySize];
    }

    public override void AppendElement(RespDataType respDataElement)
    {
      _array[_currentIndex++] = respDataElement;
    }

    public int Length {  get { return _array.Length; } }

    public RespDataType this[int index]
    {
      get { return _array[index]; }
    }








  }
}
