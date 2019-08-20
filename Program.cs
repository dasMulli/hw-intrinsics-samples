using System;
using BenchmarkDotNet.Running;

namespace HardwareIntrinsicsSample
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run(typeof(Program).Assembly);
        }
    }
}
