# DotNetNumericsBenchmark
This example tries to utilize the [SIMD capabilities](http://instil.co/2016/03/21/parallelism-on-a-single-core-simd-with-c/) (SSE or AVX extensions) of contemporary CPUs through a [simple array averaging example](src/DotNetNumericsBenchMark/ArrayAverageCalculator.cs) using the `System.Numerics.Vectors` namespace which is a feature of .NET's [x64 RyuJIT](https://blogs.msdn.microsoft.com/dotnet/2013/09/30/ryujit-the-next-generation-jit-compiler-for-net/) and .NET Core's [CoreCLR](https://github.com/dotnet/runtime).

## Dependencies ##
* [.NET Core 3.1.2xx SDK](https://dotnet.microsoft.com/download/dotnet-core/3.1)
* If using [Visual Studio](https://www.visualstudio.com/vs/), then at least VS 2019 is required which supports the .NET Core 3.1 SDK

## System Requirements ##
* **x64** CPU with *SSE* or *AVX* extensions running a supported **x64** flavour of Windows, Linux or macOS, 
* [.NET Core 3.1.2xx SDK](https://dotnet.microsoft.com/download/dotnet-core/3.1)

## Summary of set up
* `git clone git@github.com:laszlolukacs/DotNetNumericsBenchmark.git <LOCAL_WORKING_DIR>`
* `dotnet build --configuration release`
---
* Alternatively open the `DotNetNumericsBenchmark.sln` in your IDE of choice
* Build the solution using the `Release` build configuration

## Basic Usage
* From the output directory call `DotNetNumericsBenchmark.exe`
