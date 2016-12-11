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



        public Main()
        {
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

        }
    }
}
