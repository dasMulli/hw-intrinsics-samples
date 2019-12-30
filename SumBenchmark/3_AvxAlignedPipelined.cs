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
        public float SumUsingAvxAlignedPipelined()
        {
            var values = this.values;
            float sum = 0;
            
            if (Avx.IsSupported)
            {
                unsafe
                {
                    fixed (float* valuesPtr = values)
                    {
                        // Determine how many elements we need to sum sequential to reach 256 bit alignment
                        const int ElementsPerByte = sizeof(float) / sizeof(byte);
                        var alignmentOffset = (long)(uint)(-(int)valuesPtr / ElementsPerByte) & (Vector256<float>.Count - 1);

                        // handle first values sequentially until we hit the 256bit alignment boundary
                        for(long i = 0; i < alignmentOffset; i ++)
                        {
                            sum += *(valuesPtr + i);
                        }

                        var remainingLength = values.Length - alignmentOffset;

                        var vectorizableLength = values.Length - remainingLength % (long)Vector256<float>.Count;

                        // handle batches of 4 vectors for pipelining benefits
                        var pipelineVectorizableLength = values.Length - remainingLength % (4*(long)Vector256<float>.Count);

                        var tempVector1 = Vector256<float>.Zero;
                        var tempVector2 = Vector256<float>.Zero;
                        var tempVector3 = Vector256<float>.Zero;
                        var tempVector4 = Vector256<float>.Zero;
                        for(long i = alignmentOffset; i < pipelineVectorizableLength; i += 4*(long)Vector256<float>.Count)
                        {
                            var valuesVector1 = Avx.LoadAlignedVector256(valuesPtr + i);
                            tempVector1 = Avx.Add(tempVector1, valuesVector1);
                            var valuesVector2 = Avx.LoadAlignedVector256(valuesPtr + i + Vector256<float>.Count);
                            tempVector2 = Avx.Add(tempVector2, valuesVector2);
                            var valuesVector3 = Avx.LoadAlignedVector256(valuesPtr + i + 2*Vector256<float>.Count);
                            tempVector3 = Avx.Add(tempVector3, valuesVector3);
                            var valuesVector4 = Avx.LoadAlignedVector256(valuesPtr + i + 3*Vector256<float>.Count);
                            tempVector4 = Avx.Add(tempVector4, valuesVector4);
                        }

                        var tempVector = Avx.Add(Avx.Add(Avx.Add(tempVector1, tempVector2), tempVector3), tempVector4);

                        // handle remaining vectors
                        for (long i = pipelineVectorizableLength; i < vectorizableLength; i += Vector256<float>.Count)
                        {
                            var valuesVector = Avx.LoadAlignedVector256(valuesPtr + i);
                            tempVector = Avx.Add(tempVector, valuesVector);
                        }

                        // sum the temporary vector
                        for(int iVector = 0; iVector < Vector256<float>.Count; iVector++)
                        {
                            sum += tempVector.GetElement(iVector);
                        }

                        // handle remaining items
                        for(int i = (int)vectorizableLength; i < values.Length; i++)
                        {
                            sum += values[i];
                        }
                    }
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
        [Theory]
        // not a perfect way to test alignment but better than nothing..
        [Repeat(50)]
        public void ItShallCalculateCorrectly_AvxAlignedPiplined(int _)
        {
            subjectUnderTest.SumUsingAvxAlignedPipelined().Should().Be(expectedSum);
        }
    }
}