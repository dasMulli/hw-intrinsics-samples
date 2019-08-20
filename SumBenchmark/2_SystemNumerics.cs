using System;
using System.Linq;
using System.Numerics;
using BenchmarkDotNet.Attributes;
using FluentAssertions;
using Xunit;

namespace HardwareIntrinsicsSample
{
    [RankColumn]
    public partial class SumBenchmark
    {
        [Benchmark]
        public float SumUsingSystemNumerics()
        {
            var values = this.values;
            float sum = 0;

            // How may elements can we sum using vectors?
            int vectorizableLength = values.Length - values.Length % Vector<float>.Count;

            // Create vectors from the values and sum them into a temporary vector
            var tempVector = Vector<float>.Zero;
            var valuesSpan = values.AsSpan();
            for(int i = 0; i < vectorizableLength; i += Vector<float>.Count)
            {
                tempVector += new Vector<float>(valuesSpan.Slice(i, Vector<float>.Count));
            }
            
            // Sum the elements in the ttemporary vector
            for(int iVector = 0; iVector < Vector<float>.Count; iVector++)
            {
                sum += tempVector[iVector];
            }

            // Handle remaining elements
            for(int i = vectorizableLength; i < values.Length; i++)
            {
                sum += values[i];
            }

            return sum;
        }
    }

    public partial class SumBenchmarkTests
    {
        [Fact]
        public void ItShallCalculateCorrectly_SystemNumerics()
        {
            subjectUnderTest.SumUsingSystemNumerics().Should().Be(expectedSum);
        }
    }
}