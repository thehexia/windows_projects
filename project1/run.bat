
csc /target:library /out:WPCalc.dll /reference:System.Numerics.dll SimpleCalc.cs ComplexCalc.cs
csc /target:exe /reference:WPCalc.dll /reference:System.Numerics.dll /out:MyCalc.exe Calc.cs
MyCalc.exe
