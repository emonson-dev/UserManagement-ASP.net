using System;
using System.Collections.Generic;
namespace UserManagement.Models
{
    public enum Status
    {
        Approved,   // Default value is 1
        Pending,    // Default value is 0
        Rejected    // Default value is 2
    }
    public class User
    {
        public int ID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public Status Status { get; set; }
        public DateTime EnrollmentDate { get; set; }
    }
}
