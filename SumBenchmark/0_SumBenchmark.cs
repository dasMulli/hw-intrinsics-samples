using System;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace HardwareIntrinsicsSample
{
    [RankColumn]
    [ShortRunJob]
    public partial class SumBenchmark
    {
        internal float[] values;

        [Params(/*100, */ 100_003)]
        public int N;

        [GlobalSetup]
        public void Setup()
        {
            var rnd = new Random(42);
            values = Enumerable.Range(0, N).Select(_ => (float)rnd.NextDouble()).ToArray();
        }
    }
    
    public partial class SumBenchmarkTests
    {
        private SumBenchmark subjectUnderTest;

        private float expectedSum;

        public SumBenchmarkTests()
        {
            subjectUnderTest = new SumBenchmark();
            subjectUnderTest.N = 1_003;
            subjectUnderTest.Setup();
            subjectUnderTest.values = Enumerable.Range(0,1_003).Select(_ => 1f).ToArray();
            expectedSum = 1_003;
        }
    }
}