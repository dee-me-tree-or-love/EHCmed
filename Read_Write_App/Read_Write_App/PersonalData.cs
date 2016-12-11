using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Read_Write_App
{
    class PersonalData
    {
       public string Allergies { get; set; }
        public string Vaccines { get; set; }
        public string Diseases { get; set; }
        public string Medicines { get; set; }
        public string Name { get; set; }
        public string DoB { get; set; }
        public string Country { get; set; }
        public string Password { get; set; }
        public long Gp_Id { get; set; }
        public string Blood_Group { get; set; }

        public PersonalData(string al, string vac, string med, string dis, string name, string dob, string country, string pass, long gp_id, string bg)
        {
            this.Allergies = al;
            this.Vaccines = vac;
            this.Diseases = dis;
            this.Medicines = med;
            this.Name = name;
            this.DoB = dob;
            this.Country = country;
            this.Password = pass;
            this.Gp_Id = gp_id;
            this.Blood_Group = bg;
        }

    }
}
