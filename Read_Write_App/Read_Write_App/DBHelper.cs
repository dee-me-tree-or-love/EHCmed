using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace Read_Write_App
{
    class DBHelper
    {
        SQLiteConnection m_dbConnection;
        public void connectToDatabase()
        {
                m_dbConnection = new SQLiteConnection("Data Source=../../../../Database/EHCDB.db;");
                m_dbConnection.Open();

        }
        public int authDoctor(string email, string password)
        {
            int id=0;
            SQLiteCommand cmd = new SQLiteCommand("select ID from GP where Email = '" + email + "' and Password ='" + password + "'", m_dbConnection);
            SQLiteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
                id = reader.GetInt32(0);
            return id;
        }

       public GP getGP(int id)
        {
            string name="";
            string email = "";
            string pass = "";

            GP tempgp = null;
            SQLiteCommand cmd = new SQLiteCommand("select * from GP where ID = '" + id + "'", m_dbConnection);
            SQLiteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                name = (string)reader["Name"];
                pass = (string)reader["Password"];
                email = (string)reader["Email"];

            }
            tempgp = new GP(id, name, email, pass);
            return tempgp;
        }
      public List<PersonalData> getGpPatients(int gpId)
        {
            List<PersonalData> PD=new List<PersonalData>();
           PersonalData tempPD = null;
            string al = "";
            string vac = "";
            string med = "";
            string dis = "";
            string name = "";
            string dob = "";
            string country = "";
            string pass = "";
            int gp_id = 0;
            string bg="";

            SQLiteCommand cmd = new SQLiteCommand("select * from PersonalData where Gp_Id = '" + gpId + "'", m_dbConnection);
            SQLiteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                
                al = (string)reader["Allergy"];
                vac = (string)reader["Vaccination"];
                med = (string)reader["Medication"];
                dis = (string)reader["Disease"];
                dob = (string)reader["DoB"];
                name = (string)reader["Name"];
                pass = (string)reader["Password"];
                country = (string)reader["Country"];
                gp_id = gpId;
                bg = (string)reader["Blood_Group"];
                tempPD = new PersonalData(al, vac, med, dis, name, dob, country, pass, gp_id, bg);
                PD.Add(tempPD);

            }
            return PD;

        }
        public PersonalData getDataFromName(string name)
        {
            PersonalData tempPD = null;
            string al = "";
            string vac = "";
            string med = "";
            string dis = "";
            string naam = "";
            string dob = "";
            string country = "";
            string pass = "";
            long gp_id = 0;
            string bg = "";

            SQLiteCommand cmd = new SQLiteCommand("select * from PersonalData where Name = '" + name + "'", m_dbConnection);
            SQLiteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {

                al = (string)reader["Allergy"];
                vac = (string)reader["Vaccination"];
                med = (string)reader["Medication"];
                dis = (string)reader["Disease"];
                dob = (string)reader["DoB"];
                naam = name ;
                pass = (string)reader["Password"];
                country = (string)reader["Country"];
                gp_id = (long)reader["Gp_Id"];
                bg = (string)reader["Blood_Group"];
                tempPD = new PersonalData(al, vac, med, dis, naam, dob, country, pass, gp_id, bg);
            }
            return tempPD;

         }
        public string allergyToString(int code)
        {
            if (code == 12)
            {
                return "Peanuts";
            }
            else
            {
                return "Wasp-Stings";
            }
        }

        public string diseaseToString(int code)
        {
            if (code == 12)
            {
                return "Diabetes";
            }
            else
            {
                return "Heart Failure";
            }
        }

        public string medicineToString(int code)
        {
            if (code == 12)
            {
                return "Peanuts";
            }
            else
            {
                return "Wasp-Stings";
            }
        }

        public string vaccineToString(int code)
        {
            if (code == 1)
            {
                return "Tetanus";
            }
            else if(code ==4)
            {
                return "ChickenPocks";
            }
            else
            {
                return "Yellow Fever";
            }
        }

        public List<string> getAllDiseases()
        {
            List<string> diseases = new List<string>();
            string tempDisease = "";
            SQLiteCommand cmd = new SQLiteCommand("select * from Diseases", m_dbConnection);
            SQLiteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                tempDisease = (string)reader["Name"];
                diseases.Add(tempDisease);
            }
            return diseases;
        }

        public List<string> getAllAllergies()
        {
            List<string> allergies = new List<string>();
            string tempAllergy = "";
            SQLiteCommand cmd = new SQLiteCommand("select * from Allergies", m_dbConnection);
            SQLiteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                tempAllergy = (string)reader["Name"];
                allergies.Add(tempAllergy);
            }
            return allergies;
        }

        public List<string> getAllVaccines()
        {
            List<string> allergies = new List<string>();
            string tempAllergy = "";
            SQLiteCommand cmd = new SQLiteCommand("select * from Vaccines", m_dbConnection);
            SQLiteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                tempAllergy = (string)reader["Name"];
                allergies.Add(tempAllergy);
            }
            return allergies;
        }

        public List<string> getAllMeds()
        {
            List<string> allergies = new List<string>();
            string tempAllergy = "";
            SQLiteCommand cmd = new SQLiteCommand("select * from Medicines", m_dbConnection);
            SQLiteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                tempAllergy = (string)reader["Name"];
                allergies.Add(tempAllergy);
            }
            return allergies;
        }
    }
}
