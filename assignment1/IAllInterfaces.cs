using System;
namespace assignment1
{
    /// <summary>
    /// Summary description for Class1
    /// </summary>


    // robot interface
    public interface ISensor
    {
        void Detect();
    }
    public interface INavigable
    {
        void Navigate(string destination);
    }
    public interface IChargeable
    {
        void Charge(string batteryStatus);
    }




}
