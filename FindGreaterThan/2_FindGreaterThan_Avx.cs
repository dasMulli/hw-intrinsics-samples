using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using BenchmarkDotNet.Attributes;
using FluentAssertions;
using Xunit;

namespace HardwareIntrinsicsSample
{
    public partial class FindGreaterThanBenchmark
    {
        [Benchmark]
        public int IndexOfFirstElementGreaterOrEqualToLimit_Avx()
        {
            var values = this.values;
            float limit = this.limitToFind;

            if (Avx.IsSupported)
            {
                unsafe
                {
                    fixed (float* valuesPtr = values)
                    {
                        const int ElementsPerByte = sizeof(float) / sizeof(byte);
                        var alignmentOffset = (long)(uint)(-(int)valuesPtr / ElementsPerByte) & (Vector256<float>.Count - 1);

                        // handle first values sequentially until we hit the 256bit alignment boundary
                        for(long i = 0; i < alignmentOffset; i ++)
                        {
                            if(*(valuesPtr + i) >= limit)
                            {
                                return (int)i;
                            }
                        }

                        var remainingLength = values.Length - alignmentOffset;
                        var vectorizableLength = values.Length - remainingLength % (long)Vector256<float>.Count;

                        // handle vectorizable items
                        var limitVector = Vector256.Create(limit);
                        for (var i = alignmentOffset; i < vectorizableLength; i += Vector256<float>.Count)
                        {
                            var valuesVector = Avx.LoadAlignedVector256(valuesPtr + i);
                            var comparisonResultVector = Avx.Compare(valuesVector, limitVector, FloatComparisonMode.OrderedGreaterThanOrEqualNonSignaling);
                            
                            // create int bitmask from vector bitmask
                            // the first bit (right-to-left) that is 1 indicates a comparision yielding true
                            var comparisonResult = (uint)Avx.MoveMask(comparisonResultVector);

                            if (comparisonResult == 0)
                            {
                                // no element of the vector matches the compare criteria
                                continue;
                            }

                            // a match was found
                            var matchedLocation = i + Bmi1.TrailingZeroCount(comparisonResult);
                            return (int)matchedLocation;
                        }

                        // handle remaining items
                        for (var i = (int)vectorizableLength; i < values.Length; i++)
                        {
                            if (values[i] >= limit)
                            {
                                return i;
                            }
                        }

                        return -1;
                    }
                }
            }
            else
            {
                for(int i = 0; i < values.Length; i++)
                {
                    if (values[i] >= limit)
                    {
                        return i;
                    }
                }
                return -1;
            }
        }
    }

    public partial class FindGreaterThanBenchmarkTests
    {
        [Fact]
        public void ItShallFindLimitIndex_Avx()
        {
            subjectUnderTest.IndexOfFirstElementGreaterOrEqualToLimit_Avx().Should().Be(expectedResult);
        }

        [Theory]
        // not a perfect way to test alignment but better than nothing..
        [Repeat(50)]
        public void ItShallNotFindIndexForTooHighLimit_Avx(int _)
        {
            subjectUnderTest.limitToFind = int.MaxValue;
            subjectUnderTest.IndexOfFirstElementGreaterOrEqualToLimit_Avx().Should().Be(-1);
        }
    }
}