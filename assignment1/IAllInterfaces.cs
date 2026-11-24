using System;
namespace assignment1
{
    /// <summary>
    /// Summary description for Class1
    /// </summary>
    

        // robot interface
        interface Isensor
        {
            void Detect();
        }
        interface Inevigable
        {
            void Navigate(string destination);
        }
        interface Ichargeable
        {
            void Charge();
        }



    
}
