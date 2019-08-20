``` ini

BenchmarkDotNet=v0.11.5, OS=Windows 10.0.18362
Intel Core i9-8950HK CPU 2.90GHz (Coffee Lake), 1 CPU, 5 logical and 5 physical cores
.NET Core SDK=3.0.100-preview8-013656
  [Host]     : .NET Core 3.0.0-preview8-28405-07 (CoreCLR 4.700.19.37902, CoreFX 4.700.19.40503), 64bit RyuJIT
  DefaultJob : .NET Core 3.0.0-preview8-28405-07 (CoreCLR 4.700.19.37902, CoreFX 4.700.19.40503), 64bit RyuJIT


```
|                      Method |      N |       Mean |     Error |    StdDev | Ratio | RatioSD | Rank |
|---------------------------- |------- |-----------:|----------:|----------:|------:|--------:|-----:|
|                    SumNaive | 100003 |  94.841 us | 1.8769 us | 1.8433 us |  1.00 |    0.00 |    4 |
|                SumUsingLinq | 100003 | 409.957 us | 4.0358 us | 3.7751 us |  4.32 |    0.11 |    5 |
|      SumUsingSystemNumerics | 100003 |  12.957 us | 0.2524 us | 0.2907 us |  0.14 |    0.00 |    3 |
|                 SumUsingAvx | 100003 |  12.327 us | 0.2289 us | 0.2141 us |  0.13 |    0.00 |    2 |
| SumUsingAvxAlignedPipelined | 100003 |   5.654 us | 0.0638 us | 0.0533 us |  0.06 |    0.00 |    1 |
