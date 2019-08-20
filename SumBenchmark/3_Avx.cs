using System;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using BenchmarkDotNet.Attributes;
using FluentAssertions;
using Xunit;

namespace HardwareIntrinsicsSample
{
    [RankColumn]
    public partial class SumBenchmark
    {
        [Benchmark]
        public float SumUsingAvx()
        {
            var values = this.values;
            float sum = 0;

            if (Avx.IsSupported)
            {
                // How may elements can we sum using vectors?
                int vectorizableLength = values.Length - values.Length % Vector256<float>.Count;

                // Create vectors from the values and sum them into a temporary vector
                var tempVector = Vector256<float>.Zero;
                unsafe
                {
                    fixed (float* valuesPtr = values)
                    {
                        for(int i = 0; i < vectorizableLength; i += Vector256<float>.Count)
                        {
                            var valuesVector = Avx.LoadVector256(valuesPtr + i);
                            tempVector = Avx.Add(tempVector, valuesVector);
                        }
                    }
                }
                
                // Sum the elements in the ttemporary vector
                for(int iVector = 0; iVector < Vector256<float>.Count; iVector++)
                {
                    sum += tempVector.GetElement(iVector);
                }

                // Handle remaining elements
                for(int i = vectorizableLength; i < values.Length; i++)
                {
                    sum += values[i];
                }
            }
            else
            {
                // non-AVX capable machines
                for (int i = 0; i < values.Length; i++)
                {
                    sum += values[i];
                }
            }

            return sum;
        }
    }

    public partial class SumBenchmarkTests
    {
        [Fact]
        public void ItShallCalculateCorrectly_Avx()
        {
            subjectUnderTest.SumUsingAvx().Should().Be(expectedSum);
        }
    }
}