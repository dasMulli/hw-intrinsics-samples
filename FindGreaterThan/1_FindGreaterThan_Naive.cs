using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using FluentAssertions;
using Xunit;

namespace HardwareIntrinsicsSample
{
    public partial class FindGreaterThanBenchmark
    {
        [Benchmark(Baseline = true)]
        public int IndexOfFirstElementGreaterOrEqualToLimit_Naive()
        {
            var values = this.values;
            float limit = this.limitToFind;
            for(int i = 0; i < values.Length; i++)
            {
                if (values[i] >= limit)
                {
                    return i;
                }
            }
            return -1;
        }

        [Benchmark]
        public int IndexOfFirstElementGreaterOrEqualToLimit_LinqLike()
        {
            float limit = this.limitToFind;
            return values.IndexOf(v => v >= limit);
        }
    }

    public static class LinqExtensions
    {
        public static int IndexOf<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            var index = 0;
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    return index;
                }
                index++;
            }

            return -1;
        }
    }

    public partial class FindGreaterThanBenchmarkTests
    {
        [Fact]
        public void ItShallFindLimitIndex_Naive()
        {
            subjectUnderTest.IndexOfFirstElementGreaterOrEqualToLimit_Naive().Should().Be(expectedResult);
        }

        [Fact]
        public void ItShallNotFindIndexForTooHighLimit_Naive()
        {
            subjectUnderTest.limitToFind = int.MaxValue;
            subjectUnderTest.IndexOfFirstElementGreaterOrEqualToLimit_Naive().Should().Be(-1);
        }
        
        [Fact]
        public void ItShallFindLimitIndex_Linq()
        {
            subjectUnderTest.IndexOfFirstElementGreaterOrEqualToLimit_LinqLike().Should().Be(expectedResult);
        }

        [Fact]
        public void ItShallNotFindIndexForTooHighLimit_Linq()
        {
            subjectUnderTest.limitToFind = int.MaxValue;
            subjectUnderTest.IndexOfFirstElementGreaterOrEqualToLimit_LinqLike().Should().Be(-1);
        }
    }
}