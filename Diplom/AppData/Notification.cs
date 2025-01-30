using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diplom.AppData
{
    public class Notification
    {
        public string Message { get; set; }
        public DateTime Date { get; set; }
        public bool IsRead { get; set; } 

        public Notification(string message)
        {
            Message = message;
            Date = DateTime.Now;
            IsRead = false;  
        }
    }
}
