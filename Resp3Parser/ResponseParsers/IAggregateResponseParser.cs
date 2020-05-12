using RedisResp3Client.DataTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace RedisResp3Client.ResponseParsers
{
  interface IAggregateResponseParser
  {
    RespAggregateDataType Result { get;  }
  }
}
