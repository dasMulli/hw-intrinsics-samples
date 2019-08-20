using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit.Sdk;

namespace HardwareIntrinsicsSample
{
    public sealed class RepeatAttribute : DataAttribute
    {
        private readonly int count;

        public RepeatAttribute(int count)
        {
            if (count < 1)
            {
                throw new System.ArgumentOutOfRangeException(
                    paramName: nameof(count),
                    message: "Repeat count must be greater than 0."
                    );
            }
            this.count = count;
        }

        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
            => Enumerable.Range(start: 1, count: this.count).Select(i => new object[] { i });
    }
}