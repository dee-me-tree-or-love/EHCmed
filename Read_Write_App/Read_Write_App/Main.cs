using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Read_Write_App
{
    public partial class Main : Form
    {
        public Dictionary<string, ConsoleWriter.BloodTypes> BloodTypesDict = new Dictionary<string, ConsoleWriter.BloodTypes>()
        {
            {"AB-", ConsoleWriter.BloodTypes.ABN },
            {"AB+", ConsoleWriter.BloodTypes.ABP },
            {"A+", ConsoleWriter.BloodTypes.AP },
            {"A-", ConsoleWriter.BloodTypes.AN },
            {"B+", ConsoleWriter.BloodTypes.BP },
            {"B-", ConsoleWriter.BloodTypes.BN },
            {"O+", ConsoleWriter.BloodTypes.OP },
            {"O-", ConsoleWriter.BloodTypes.ON },
        };

        private GP user;
        private DBHelper dbh;
        int gpid;

        public Main(int gpId)
        {
            gpid = gpId;
            InitializeComponent();

            //transparent background
            label1.Parent = pictureBox3;
            label1.BackColor = Color.Transparent;
            label2.Parent = pictureBox1;
            label2.BackColor = Color.Transparent;
            groupBox1.Parent = pictureBox1;
            groupBox1.BackColor = Color.Transparent;
            gbDandR.Parent = pictureBox1;
            gbDandR.BackColor = Color.Transparent;
            
            
            this.cbBloodGroup.Items.AddRange(BloodTypesDict.Keys.ToArray());

            dbh = new DBHelper();
            dbh.connectToDatabase();
            user = dbh.getGP(gpId);
            label1.Text = "Welcome " + user.Name;

            //get patients
            List<PersonalData> patients = dbh.getGpPatients(gpId);
            foreach(PersonalData p in patients)
            {
                comboBox1.Items.Add(p.Name);
            }
        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            // changes the subcategory choices
            // does not enable the grid
            var cat = (DiagnosisAndProcedures)this.cbDandRCat.SelectedIndex;
            this.cbDandRSubCat.Items.Clear();
            try
            {
                List<DAndRSubCat> tempList = new List<DAndRSubCat>();
                MedTerms.DandRCategorization.TryGetValue(cat, out tempList);
                foreach (DAndRSubCat sc in tempList)
                {
                    this.cbDandRSubCat.Items.Add(sc.ToString());
                }
            }
            catch
            {

            }



        }

        private void cbDandRSubCat_SelectedIndexChanged(object sender, EventArgs e)
        {
            // changes displayed data in the grid
            // when set enables the grid
            // to do - connect to database/or a certain list of stuff
            if (cbDandRSubCat.SelectedIndex == 0 && cbDandRCat.SelectedIndex == 1)
            {
                listBox1.Items.Clear();
                List<string> diseases = dbh.getAllDiseases();
                foreach (string a in diseases)
                {
                    
                    listBox1.Items.Add(a);
                }
            }

            if (cbDandRSubCat.SelectedIndex == 0 && cbDandRCat.SelectedIndex == 0)
            {
                listBox1.Items.Clear();
                List<string> diseases = dbh.getAllAllergies();
                foreach (string a in diseases)
                {
                    
                    listBox1.Items.Add(a);
                }
            }

            if (cbDandRSubCat.SelectedIndex == 1 && cbDandRCat.SelectedIndex == 0)
            {
                listBox1.Items.Clear();
                List<string> diseases = dbh.getAllVaccines();
                foreach (string a in diseases)
                {
                    
                    listBox1.Items.Add(a);
                }
            }
            if (cbDandRSubCat.SelectedIndex == 1 && cbDandRCat.SelectedIndex == 1)
            {
                listBox1.Items.Clear();
                List<string> diseases = dbh.getAllMeds();
                foreach (string a in diseases)
                {
                    
                    listBox1.Items.Add(a);
                }
            }

        }


        /// <summary>
        /// load patient from card
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            lbAllergies.Items.Clear();
            lbDiseases.Items.Clear();
            lbVaccines.Items.Clear();
            lbMedHistory.Items.Clear();
            try
            {
                ConsoleWriter.Patient pd = ConsoleWriter.CardManager.ReadFromCard();
                MessageBox.Show("You have a new patient!\n" + pd.ToString());
                lbAllergies.Items.AddRange(pd.Allergies.ToArray());
                lbDiseases.Items.AddRange(pd.Diseases.ToArray());
                lbVaccines.Items.AddRange(pd.Vacinnes.ToArray());
                lbMedHistory.Items.AddRange(pd.Medication.ToArray());
                cbBloodGroup.SelectedItem = this.BloodTypesDict.FirstOrDefault(x => x.Value == pd.BloodType).Key;
            }
            catch
            {
                MessageBox.Show("Fuck");
            }

        }

        /// <summary>
        /// Add to patient file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            if (cbDandRSubCat.SelectedIndex == 0 && cbDandRCat.SelectedIndex == 0 && listBox1.SelectedIndex != -1)
            {
                lbAllergies.Items.Add(listBox1.SelectedItem.ToString());
            }
            if (cbDandRSubCat.SelectedIndex == 0 && cbDandRCat.SelectedIndex == 1 && listBox1.SelectedIndex != -1)
            {
                lbDiseases.Items.Add(listBox1.SelectedItem.ToString());
            }
            if (cbDandRSubCat.SelectedIndex == 1 && cbDandRCat.SelectedIndex == 0 && listBox1.SelectedIndex != -1)
            {
                lbVaccines.Items.Add(listBox1.SelectedItem.ToString());
            }
            if (cbDandRSubCat.SelectedIndex == 1 && cbDandRCat.SelectedIndex == 1 && listBox1.SelectedIndex != -1)
            {
                lbMedHistory.Items.Add(listBox1.SelectedItem.ToString());
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            PersonalData patient = dbh.getDataFromName(comboBox1.Text);
            textBox2.Text = patient.Name;
            textBox3.Text = patient.Name;
            cbBloodGroup.Text = patient.Blood_Group;
            textBox4.Text = patient.DoB;
            textBox1.Text = "12-05-1996";

            lbAllergies.Items.Clear();
            string al = patient.Allergies;
            string[] allergies = al.Split('#');
            foreach(string a in allergies)
            {
                if (a != "")
                {

                    lbAllergies.Items.Add(dbh.allergyToString(Convert.ToInt32(a)));
                }
            }

            lbDiseases.Items.Clear();
            string dis = patient.Diseases;
            string[] disiases = dis.Split('#');
            foreach (string a in disiases)
            {
                if (a != "")
                    lbDiseases.Items.Add(dbh.diseaseToString(Convert.ToInt32(a)));
            }

            lbMedHistory.Items.Clear();
            string med = patient.Medicines;
            string[] meds = med.Split('#');
            foreach (string a in meds)
            {
                if (a != "")
                    lbMedHistory.Items.Add(dbh.medicineToString(Convert.ToInt32(a)));
            }

            lbVaccines.Items.Clear();
            string vac = patient.Vaccines;
            string[] vacs = vac.Split('#');
            foreach (string a in vacs)
            {
                if (a != "")
                   lbVaccines.Items.Add(dbh.vaccineToString(Convert.ToInt32(a))) ;
            }
        }

        private void cbDandRSubCat_TextUpdate(object sender, EventArgs e)
        {
            
        }
    }
}
