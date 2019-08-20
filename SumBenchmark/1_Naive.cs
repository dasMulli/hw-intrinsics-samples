using System;
using System.Linq;
using BenchmarkDotNet.Attributes;
using FluentAssertions;
using Xunit;

namespace HardwareIntrinsicsSample
{
    [RankColumn]
    public partial class SumBenchmark
    {
        [Benchmark(Baseline = true)]
        public float SumNaive()
        {
            var values = this.values;
            float sum = 0;
            for(int i = 0; i < values.Length; i++)
            {
                sum += values[i];
            }
            return (float)sum;
        }

        [Benchmark]
        public float SumUsingLinq()
        {
            return values.Sum();
        }
    }

    public partial class SumBenchmarkTests
    {
        [Fact]
        public void ItShallCalculateCorrectly_Linq()
        {
            subjectUnderTest.SumUsingLinq().Should().Be(expectedSum);
        }

        [Fact]
        public void ItShallCalculateCorrectly_Naive()
        {
            subjectUnderTest.SumNaive().Should().Be(expectedSum);
        }
    }
}