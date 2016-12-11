using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Read_Write_App
{
   public class GP
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public GP(int id, string name, string email, string pass)
        {
            this.ID = id;
            this.Email = email;
            this.Password = pass;
            this.Name = name;
        }

        


    }
}
