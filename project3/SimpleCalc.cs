using Calc;

public class SimpleCalc : iSimpleCalc
{
  // Returns the result of adding a and b.
  public float add(float a, float b)
  {
    return a + b;
  }

  // Returns the result of subtracting a and b.
  public float subtract(float a, float b)
  {
    return a - b;
  }

  // Returns the result of multiplying a and b.
  public float multiply(float a, float b)
  {
    return a * b;
  }

  // Returns the reslt of dividing a and b.
  public float divide(float a, float b) {
    return a / b;
  }
}
