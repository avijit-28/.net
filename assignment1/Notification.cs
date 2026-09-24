using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace assignment1
{
    internal class Notification
    {
        public virtual void SendNotification(string message)
        {
            Console.WriteLine($"Sending notification: {message}");
        }
    }

    class EmailNotification : Notification
    {
        public override void SendNotification(string message)
        {
            Console.WriteLine($"Sending email notification: {message}");
        }
    }
    class SMSNotification : Notification
    {
        public override void SendNotification(string message)
        {
            Console.WriteLine($"Sending SMS notification: {message}");
        }
    }
    class whatsappNotification : Notification
    {
        public override void SendNotification(string message)
        {
            Console.WriteLine($"Sending WhatsApp notification: {message}");
        }
    }
    class PushNotification : Notification
    {
        public override void SendNotification(string message)
        {
            Console.WriteLine($"Sending push notification: {message}");
        }
    }


}
