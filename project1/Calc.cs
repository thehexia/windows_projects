using System;
using System.Collections;
using System.Reflection;
using System.Numerics;
using Calc;

namespace Calc
{

public class Calculator
{
  private ArrayList options;
  private Assembly asm;
  private int numOptions;
  private string dllName;

  public Calculator(string dllName)
  {
    options = new ArrayList();
    numOptions = 0;
    this.dllName = dllName;
    // Load the dll
    LoadCalcDll(dllName);
  }


  private static void print(Complex c)
  {
    Console.WriteLine("{0} + {1}i", c.Real, c.Imaginary);
  }


  // Load the calculator dll and save all of its methods.
  private void LoadCalcDll(string dllName)
  {
    asm = Assembly.Load(dllName);
    if (asm == null) {
      Console.WriteLine("Cannot find assembly " + dllName + ". Confirm this is in your current directory.");
      return;
    }

    // Read through the dll and save all of its method info
    foreach (var type in asm.GetTypes())
    {
      // Install the add/subtract/mult/divide functions.
      MethodInfo add = type.GetMethod("add");
      if (add != null) {
        options.Add(add);
        ++numOptions;
      }

      MethodInfo sub = type.GetMethod("subtract");
      if (sub != null) {
        options.Add(sub);
        ++numOptions;
      }

      MethodInfo mul = type.GetMethod("multiply");
      if (mul != null) {
        options.Add(mul);
        ++numOptions;
      }

      MethodInfo div = type.GetMethod("divide");
      if (div != null) {
        options.Add(div);
        ++numOptions;
      }
    }
  }

  // Prints the content of Simple Calculator
  private void ReflectCalcDll()
  {
    if (asm == null) {
      Console.WriteLine("Cannot find assembly " + dllName + ". Confirm this is in your current directory.");
      return;
    }

    int i = 0;
    foreach (var type in asm.GetTypes())
    {
      // Print the type
      Console.WriteLine();
      Console.WriteLine("=== " + type.ToString() + " ===");
      // Install the add/subtract/mult/divide functions.
      Console.WriteLine("(" + i++ + ") " + type.GetMethod("add").ToString());
      Console.WriteLine("(" + i++ + ") " + type.GetMethod("subtract").ToString());
      Console.WriteLine("(" + i++ + ") " + type.GetMethod("multiply").ToString());
      Console.WriteLine("(" + i++ + ") " + type.GetMethod("divide").ToString());
    }
  }

  // Asks the user to pick one of the options.
  private int QueryOption()
  {
    while (true) {
      // Print the DLL contents.
      ReflectCalcDll();

      Console.WriteLine();
      Console.WriteLine("Which operation would you like to run? (Ctrl + C to quit.)");
      // Try to parse a mode the user input.
      int op;
      if (int.TryParse(Console.ReadLine(), out op)) {
        if (op < numOptions && op >= 0) {
          Console.Clear();
          return op;
        }
      }
      // If the user picks anything else.
      Console.Clear();
      Console.WriteLine("INVALID OPERATION.");
    }
  }

  // Ask for continue
  private bool QueryContinue()
  {
    while (true) {
      Console.WriteLine("Continue? (y/n)");
      char op = Convert.ToChar(Console.Read());
      // Read the rest of the line without doing anything.
      Console.ReadLine();
      if (op == 'y' || op == 'Y') {
        Console.Clear();
        return true;
      }
      else if (op == 'n' || op == 'N') {
        Console.Clear();
        return false;
      }
      else
        Console.WriteLine("Please pick. (y/n)");
    }
  }

  // Start the calculator/
  public void start()
  {
    // The Simple calc.
    Type simple = asm.GetType("Calc.SimpleCalc");
    // The Complex calc
    Type complex = asm.GetType("Calc.ComplexCalc");

    while (true)
    {
      int option = QueryOption();
      // Recover the Method Info from the option #
      MethodInfo fn = (MethodInfo) options[option];
      if (fn == null) {
        Console.WriteLine("ERROR: Failed to load option function.");
      }

      // Get the class that declared this type.
      Type calc = fn.DeclaringType;
      Object obj = Activator.CreateInstance(calc);

      // If the type of calculator is the simple calc, read two int params.
      if (calc.Equals(simple))
      {
        Console.WriteLine(fn.ToString());

        int a, b = 0;
        IntegerArgs(out a, out b);
        // Invoke the method.
        var res = fn.Invoke(obj, new object[]{a, b});
        Console.WriteLine();
        Console.WriteLine("Result: " + res);
      }
      else if (calc.Equals(complex))
      {
        Console.WriteLine(fn.ToString());

        Complex a, b;
        ComplexArgs(out a, out b);
        // Invoke the method.
        var res = fn.Invoke(obj, new object[]{a, b});
        Console.WriteLine();
        Console.Write("Result: ");
        print((Complex) res);
      }

      // Ask for continue
      if (QueryContinue())
        continue;
      else
        break;
    }

  }

  private void IntegerArgs(out int a, out int b)
  {
    Console.WriteLine("Integer arguments...");
    // Read for the first input parameter.
    while (true)
    {
      Console.Write("Param 1: ");
      if (int.TryParse(Console.ReadLine(), out a))
        break;
      else
        Console.WriteLine("Invalid Integer.");
    }

    // Read for the second input parameter.
    while (true)
    {
      Console.Write("Param 2: ");
      if (int.TryParse(Console.ReadLine(), out b))
        break;
      else
        Console.WriteLine("Invalid Integer.");
    }
  }


  private void ComplexArgs(out Complex a, out Complex b)
  {
    Console.WriteLine("Complex arguments...");
    double real1, img1;
    double real2, img2;

    // Read for the first real part of input parameter.
    Console.WriteLine("== Complex Number 1 ==");
    while (true)
    {
      Console.Write("Real Part (Param 1): ");
      if (double.TryParse(Console.ReadLine(), out real1))
        break;
      else
        Console.WriteLine("Invalid double.");
    }

    // Read the imaginary part for the first parameter.
    while (true)
    {
     Console.Write("Imaginary Part (Param 1): ");
     if (double.TryParse(Console.ReadLine(), out img1))
       break;
     else
       Console.WriteLine("Invalid double.");
    }


    // Read for the second real part of input parameter.
    Console.WriteLine("== Complex Number 2 ==");
    while (true)
    {
      Console.Write("Real Part (Param 2): ");
      if (double.TryParse(Console.ReadLine(), out real2))
        break;
      else
        Console.WriteLine("Invalid double.");
    }

    // Read the imaginary part for the first parameter.
    while (true)
    {
     Console.Write("Imaginary Part (Param 2): ");
     if (double.TryParse(Console.ReadLine(), out img2))
       break;
     else
       Console.WriteLine("Invalid double.");
    }

    a = new Complex(real1, img1);
    b = new Complex(real2, img2);
  }

  public static void Main(string[] args)
  {
    Console.WriteLine("===== Console Calculator =====");

    Calculator calc = new Calculator("WCalc");
    calc.start();
    // Ctrl + c to end.
  }
}

} // namespace Calc
