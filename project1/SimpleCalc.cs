/*
  (C) Hoang Nguyen
*/

using System;
using System.Numerics;

namespace Calc
{

public class SimpleCalc
{
  // Returns the result of adding a and b.
  public int add(int a, int b)
  {
    return a + b;
  }

  // Returns the result of subtracting a and b.
  public int subtract(int a, int b)
  {
    return a - b;
  }

  // Returns the result of multiplying a and b.
  public int multiply(int a, int b)
  {
    return a * b;
  }

  // Returns the reslt of dividing a and b.
  public int divide(int a, int b) {
    try {
      int res = a / b;
      return res;
    }
    catch(System.DivideByZeroException)
    {
      Console.WriteLine("ERROR: Dividing by 0.");
      return 0;
    }
  }
}

} // namespace Calc
