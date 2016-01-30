using System.Numerics;

namespace Calc
{
  
public class ComplexCalc
{
  // Returns the result of adding Complex number a to b.
  public Complex add(Complex a, Complex b)
  {
    return a + b;
  }

  // Returns the result of subtracting Complex number b from a.
  public Complex subtract(Complex a, Complex b)
  {
    return a - b;
  }

  // Returns the result of multiplying Complex number a and b.
  public Complex multiply(Complex a, Complex b)
  {
    return a * b;
  }

  // Returns the result of dividing Complex number b from a.
  public Complex divide(Complex a, Complex b)
  {
    return a / b;
  }
}

} // Namespace Calc
