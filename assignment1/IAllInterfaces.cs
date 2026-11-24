using System;

/// <summary>
/// Summary description for Class1
/// </summary>
public class Class1
{
	public Class1()
	{
		//
		// TODO: Add constructor logic here
		//
	}
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
