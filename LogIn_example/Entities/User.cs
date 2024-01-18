using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LogIn_example.Entities
{
    public class User
    {
        public long? id;
        public string password;
        public string login;

        public User(string password, string login, long? id = null)
        {
            this.id = id;
            this.password = password;
            this.login = login;
        }
    }
}
