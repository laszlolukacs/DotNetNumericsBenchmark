# DotNetNumericsBenchmark
This example tries to utilize the [SIMD capabilities](http://instil.co/2016/03/21/parallelism-on-a-single-core-simd-with-c/) (SSE of AVX extensions) of contemporary CPUs using the `System.Numerics.Vectors` namespace which is a feature of .NET's [x64 RyuJIT](https://blogs.msdn.microsoft.com/dotnet/2013/09/30/ryujit-the-next-generation-jit-compiler-for-net/).

## Dependencies ##
* [.NET 4.6.2 Developer Pack](https://www.microsoft.com/en-us/download/details.aspx?id=53321)
* If using [Visual Studio](https://www.visualstudio.com/vs/), then at least VS 2017 is required to open the SDK-style project format

## System Requirements ##
* **x64** CPU with *SSE* or *AVX* extensions running an **x64** Windows OS, 
* [.NET 4.6.2 Framework](https://www.microsoft.com/en-us/download/details.aspx?id=53345)

## Summary of set up
* `git clone git@github.com:laszlolukacs/DotNetNumericsBenchmark.git <LOCAL_WORKING_DIR>`
* Open the `DotNetNumericsBenchmark.sln` in your IDE of choice
* Build the solution

## Basic Usage
* From the output directory: `DotNetNumericsBenchmark.exe --i <NUMBER_OF_ITERATIONS>`, where `NUMBER_OF_ITERATIONS` is the number of test iterations