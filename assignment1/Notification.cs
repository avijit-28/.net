using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace assignment1
{
    public class Notification
    {
        public virtual void SendNotification(string message)
        {
            Console.WriteLine($"[Default Channel] notification sent: {message}");
        }
    }

    public class EmailNotification : Notification
    {
        public override void SendNotification(string message)
        {
            Console.WriteLine($"[Email Service (SMTP)] sending mail: {message}");
        }
    }
    public class SMSNotification : Notification
    {
        public override void SendNotification(string message)
        {
            Console.WriteLine($"[SMS Service] sending SMS: {message}");
        }
    }
    public class whatsappNotification : Notification
    {
        public override void SendNotification(string message)
        {
            Console.WriteLine($"[WhatsApp Service] sending WhatsApp notification: {message}");
        }
    }
    public class PushNotification : Notification
    {
        public override void SendNotification(string message)
        {
            Console.WriteLine($"[Push Service] sending push notification: {message}");
        }
    }


}
