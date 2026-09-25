using System;
using System.Collections.Generic;
using System.Text;

namespace Day5
{
    internal class GenericsDemo
    {
        public static bool AreEqual<T>(T val1, T val2)
        {
            return val1.Equals(val2);
        }
    }
}
