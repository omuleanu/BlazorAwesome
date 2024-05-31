using System;

namespace Test
{
    public static class TestExtensions
    {
        public static void Out(this object obj)
        {
            Console.WriteLine(obj);
        }
    }
}