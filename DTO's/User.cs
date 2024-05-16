using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace SGMP_Client.DTO_s
{
    public class User
    {
        private static User userClient;

        public static User UserClient { 
            get { return userClient; } 
            set { userClient = value; } }

        public int UserId { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string UserName { get; set; }

        public int EmployeeNumber { get; set; }

        public string Name { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string Role { get; set; }

        public string PhoneNumber { get; set; }

        public string City { get; set; }

        public string Street {  get; set; }

        public int Number { get; set; }

        public int LocationId { get; set; }

        public void Logout()
        {
            userClient = null;
        }
    }
}
