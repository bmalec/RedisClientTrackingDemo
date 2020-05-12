using System;
using System.Net;
using System.Threading;
using RedisResp3Client;

namespace RedisClientTrackingDemo
{
  class Program
  {
    static void Main(string[] args)
    {
      //      IPAddress[] hostAddresses = Dns.GetHostAddresses("TinRedisCache01");
      IPAddress[] hostAddresses = Dns.GetHostAddresses("192.168.14.215");


      var nodeClient = new NodeClient(new IPEndPoint(hostAddresses[0], 6379));
      //      var result = nodeClient.Keys();

      var result = nodeClient.Set("bam_key", "bam_value");


      result.Wait();

      while (!Console.KeyAvailable)
      {
        Thread.Sleep(100);
      }
    }
  }
}
