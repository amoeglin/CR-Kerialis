using System;

namespace CompteResultat.BL
{
    //Create a custom Exception
    public class TestException : Exception
    {
        public TestException(string message) : base(message)
        {
        }
    }
}