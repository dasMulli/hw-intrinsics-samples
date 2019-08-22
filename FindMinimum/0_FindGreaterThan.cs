using System.Linq;
using BenchmarkDotNet.Attributes;

namespace HardwareIntrinsicsSample
{
    [RankColumn]
    [ShortRunJob]
    public partial class FindGreaterThanBenchmark
    {
        internal float[] values;

        internal float limitToFind;

        [GlobalSetup]
        public void Setup()
        {
            values = Enumerable.Range(0,100_003).Select(i => (float)i).ToArray();
            limitToFind = 90_000;
        }
    }

    public partial class FindGreaterThanBenchmarkTests
    {
        private readonly FindGreaterThanBenchmark subjectUnderTest;
        private readonly int expectedResult = 90_000;

        public FindGreaterThanBenchmarkTests()
        {
            subjectUnderTest = new FindGreaterThanBenchmark();
            subjectUnderTest.Setup();
        }
    }
}