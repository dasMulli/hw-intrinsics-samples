``` ini

BenchmarkDotNet=v0.11.5, OS=Windows 10.0.18362
Intel Core i9-8950HK CPU 2.90GHz (Coffee Lake), 1 CPU, 5 logical and 5 physical cores
.NET Core SDK=3.0.100-preview8-013656
  [Host]     : .NET Core 3.0.0-preview8-28405-07 (CoreCLR 4.700.19.37902, CoreFX 4.700.19.40503), 64bit RyuJIT
  DefaultJob : .NET Core 3.0.0-preview8-28405-07 (CoreCLR 4.700.19.37902, CoreFX 4.700.19.40503), 64bit RyuJIT


```
|                                            Method |       Mean |     Error |    StdDev | Ratio | RatioSD | Rank |
|-------------------------------------------------- |-----------:|----------:|----------:|------:|--------:|-----:|
|    IndexOfFirstElementGreaterOrEqualToLimit_Naive |  34.489 us | 0.5271 us | 0.4930 us |  1.00 |    0.00 |    2 |
| IndexOfFirstElementGreaterOrEqualToLimit_LinqLike | 476.396 us | 3.9173 us | 3.4726 us | 13.81 |    0.25 |    3 |
|      IndexOfFirstElementGreaterOrEqualToLimit_Avx |   6.353 us | 0.1240 us | 0.1273 us |  0.18 |    0.00 |    1 |
