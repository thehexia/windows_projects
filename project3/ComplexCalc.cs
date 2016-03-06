public class ComplexCalc : iComplexCalc
{
  // Returns the result of adding Complex number a to b.
  public cFloat add(cFloat a, cFloat b)
  {
    return new cFloat(a.getReal() + b.getReal(), a.getImg() + b.getImg());
  }

  // Returns the result of subtracting cFloat number b from a.
  public cFloat subtract(cFloat a, cFloat b)
  {
    return new cFloat(a.getReal() - b.getReal(), a.getImg() - b.getImg());
  }

  // Returns the result of multiplying cFloat number a and b.
  public cFloat multiply(cFloat a, cFloat b)
  {
    float p1 = a.getReal() * b.getReal();
    float p2 = a.getReal() * b.getImg();
    float p3 = a.getImg() * b.getReal();
    float p4 = a.getImg() * b.getImg();

    return new cFloat(p1 + p4, p2 + p3);
  }

  // Returns the result of dividing cFloat number b from a.
  public cFloat divide(cFloat a, cFloat b)
  {
    return new cFloat(a.getReal() / b.getReal(), a.getImg() / b.getImg());
  }
}
