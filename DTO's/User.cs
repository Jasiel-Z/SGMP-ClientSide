using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGMP_Client.DTO_s
{
    public class User
    {
        private static User userClient;

        public static User UserClient { get { return userClient; } set { userClient = value; } }


        public int UserId {get; set;}

        public int Email { get; set; }
        public int Password { get; set; }
        public string UserName { get; set; }
        public int EmployerId { get; set; }
        public int LocationId { get; set; }

    }
}
