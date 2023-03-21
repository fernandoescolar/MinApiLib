namespace MinApiLib.Validation.Tests;

public class Test
{

}

internal class MyException : Exception
{
    public MyException(string message) : base(message)
    {
    }
}

public class MyClass
{
    public void MyMethod()
    {
        MyPrivateMethod();
    }

    private void MyPrivateMethod()
    {
        throw new MyException("MyException");
    }
}
