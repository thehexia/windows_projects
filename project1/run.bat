
csc /target:library /out:WCalc.dll /reference:System.Numerics.dll SimpleCalc.cs ComplexCalc.cs
csc /target:exe /reference:WCalc.dll /reference:System.Numerics.dll Calc.cs
Calc.exe
