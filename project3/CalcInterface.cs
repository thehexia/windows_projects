namespace Calc
{

public class cFloat
{
  float real;
  float img;
  public cFloat(float r, float i)
  {
    real = r;
    img = i;
  }

  public float getReal()
  {
    return real;
  }

  public void setReal (float r)
  {
    real = r;
  }

  public float getImg()
  {
    return img;
  }

  public void setImg (float i)
  {
     img = i;
  }
}


interface iSimpleCalc
{
  float add (float f1, float f2);
  float subtract (float f1, float f2);
  float multiply (float f1, float f2);
  float divide (float f1, float f2);
}

interface iComplexCalc
{
  cFloat add (cFloat c1, cFloat c2);
  cFloat subtract (cFloat c1, cFloat c2);
  cFloat multiply (cFloat c1, cFloat c2);
  cFloat divide (cFloat c1, cFloat c2);
}

} // end namespace calc
