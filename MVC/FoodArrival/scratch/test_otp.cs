using System;
using FoodArrival.DAL;

namespace Test
{
    class Program
    {
        static void Main()
        {
            var repo = new RiderRepository();
            string err;
            bool badResult = repo.CompleteDelivery(20, 1, 999999, out err);
            Console.WriteLine(Bad OTP Result:  + badResult +  | Error:  + err);

            bool goodResult = repo.CompleteDelivery(20, 1, 823705, out err);
            Console.WriteLine(Good OTP Result:  + goodResult +  | Error:  + err);
        }
    }
}
