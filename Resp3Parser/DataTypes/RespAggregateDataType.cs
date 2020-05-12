using System;
using System.Collections.Generic;
using System.Text;

namespace RedisResp3Client.DataTypes
{
  public abstract class RespAggregateDataType : RespDataType
  {
    public abstract void AppendElement(RespDataType respDataElement);

    public abstract bool IsComplete { get; }
  }
}
  