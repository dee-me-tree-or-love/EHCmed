using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleWriter
{
    static class WritingAlgService
    {
        /// <summary>
        /// Converts the Patient input into the byte array, can be then used to write to the sc
        /// </summary>
        /// <param name="pat"></param>
        /// <returns></returns>
        static public byte[] PrepareData(Patient pat)
        {
            String DataString = pat.ToString();
            String DataStringBt = DataString.Substring(0, 4);
            DataString = DataStringBt + HexStringConverter.StringToHex(DataString.Substring(4));

            byte[] barray = HexStringConverter.ToByteArray(DataString);

            Console.WriteLine(DataString);
            foreach (byte b in barray)
            {
                Console.WriteLine(b);
            }

            return barray;
        }


    }

    /// <summary>
    /// Delete!!!!!!!!!!!!!!
    /// </summary>
    public class Patient
    {
        // whatever the things are is
        public List<String> Allergies { get; set; }
        public List<String> Vacinnes { get; set; }
        public BloodTypes BloodType { get; set; }
        public List<String> Diseases { get; set; }
        public List<String> Medication { get; set; }
        private string healthInsuranceId = "46z";
        public string HealthInsuranceId
        {
            get {return this.healthInsuranceId; }
            set { this.healthInsuranceId = value; }
        }

        public Patient(string HIID = "46")
        {
            this.HealthInsuranceId = HIID;
            this.Allergies = new List<string>();
            this.Vacinnes = new List<string>();
            this.Diseases = new List<string>(); 
            this.Medication = new List<string>();
        }

        /// <summary>
        /// In the data section - first the allergies, then vaccines 
        /// </summary>
        /// <param name="BT"></param>
        /// <param name="Data"></param>
        public Patient(BloodTypes BT, List<String>[] Data)
        {
            
            this.BloodType = BT;
            this.Allergies = Data[0];
            this.Vacinnes = Data[1];
            this.Diseases = Data[2];
            this.Medication = Data[3];
        }

        public void recreateDataFile(List<String>[] Data)
        {
            this.Allergies = Data[0];
            this.Vacinnes = Data[1];
            this.Diseases = Data[2];
            this.Medication = Data[3];
        }

        public override string ToString()
        {

            String HexBT = ((int)this.BloodType).ToString("X");
            if(HexBT.Length == 1)
            {
                HexBT = '0' + HexBT;
            }
            String DString = "42" + HexBT;
            Console.WriteLine("Blood type encoded: " + DString);

            // start with allergies
            DString += ">A";
            foreach (string _identifier in this.Allergies)
            {
                DString += "#"+_identifier;
            }
            // start with vaccines
            DString += ">V";
            foreach (string _identifier in this.Vacinnes)
            {
                DString += "#"+_identifier;
            }
            // start with deseases
            DString += ">D";
            foreach (string _identifier in this.Diseases)
            {
                DString += "#"+ _identifier;
            }
            // start with mdeication
            DString += ">M";
            foreach (string _identifier in this.Medication)
            {
                DString += "#"+_identifier;
            }
            DString += ">";
            // start with hiid
            DString += ">H";
            DString += "#" + this.healthInsuranceId;
            DString += ">";
            return DString;
        }
    }


    static class MedInfoDecoder
    {
        /// <summary>
        /// Gets an array of bytes and decodes the blood type value based on it
        /// </summary>
        /// <param name="BTbytes"></param>
        /// <returns></returns>
        public static BloodTypes GetBloodType(byte[] BTbytes)
        {
            byte[] BTpartOfArray = { BTbytes[0], BTbytes[1] };
            string hexString = HexStringConverter.ByteArrayToString(BTpartOfArray);
            if (hexString.Substring(0, 2) == "42")
            {
                
                var bloodTypeNr = Int32.Parse(hexString.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                //int.TryParse(hexString.Substring(2, 2), out bloodTypeNr);

                BloodTypes BT = (BloodTypes)bloodTypeNr;
                return BT;
            }
            else
            {
                return BloodTypes.NOT_DEFINED;
            }
        }

        public static void ParseDataPack(string DPString, Patient pat)
        {
            DPString = DPString.ToUpper();

            List<string> Allergies = new List<string>();
            List<string> Deseases = new List<string>(); 
            List<string> Vaccines = new List<string>(); 
            List<string> Medication = new List<string>();

            char valCategory = '\0';

            string nrAlphabet = "0123456789";

            // shows whether it's already reacording a value
            bool recordingValue = false;
            string IDval = "";

            string HIID = "";

            int curState = 0;
            for(int i=0;i< DPString.Length;i++) // each character
            {
                char inputChar = DPString[i];
                if (curState==0)
                {
                    // you have 2 options:
                    // read B and go into ignore state
                    // read anything else and go into data creation state
                    switch (inputChar)
                    {
                        case 'B':
                            curState = 1;//ignore state
                            break;
                        case 'A':
                            curState = 2;//await new chunk or read the id
                            Allergies = new List<string>();
                            valCategory = 'A';
                            // start recording
                            //start reading allergies
                            break;
                        case 'D':
                            curState = 2;//await new chunk or read the id
                            // start recording
                            Deseases = new List<string>();
                            valCategory = 'D';
                            //start reading deseases
                            break;
                        case 'V':
                            curState = 2;//await new chunk or read the id
                            // start recording
                            Vaccines = new List<string>();
                            valCategory = 'V';
                            //start reading vaccines
                            break;
                        case 'M':
                            curState = 2;//await new chunk or read the id
                            // start recording
                            Medication = new List<string>();
                            valCategory = 'M';
                            //start reading medication
                            break;
                        case 'H':
                            curState = 2;//await new chunk or read the id
                            valCategory = 'H';
                            //start reading medication
                            break;
                    }
                    //break; // new i
                }
                else if(curState==1)
                {
                    // the ignore state
                    if(inputChar=='>')
                    {
                        curState = 0;
                    }
                }else if(curState ==2) //recording
                {
                    switch(inputChar)
                    {
                        case '#':
                            // new value]
                            recordingValue = true;
                            // if it recorded something good, add it to the list
                            if(IDval != "")
                            {
                                switch(valCategory)
                                {
                                    case 'A':
                                        Allergies.Add(IDval);
                                        break;
                                    case 'D':
                                        Deseases.Add(IDval);
                                        break;
                                    case 'V':
                                        Vaccines.Add(IDval);
                                        break;
                                    case 'M':
                                        Medication.Add(IDval);
                                        break;
                                    case 'H':
                                        HIID = IDval;
                                        break;
                                }
                                
                            }
                            IDval = "";
                            //curState = 3;
                            break;
                        case '>':
                            // stop recording the block
                            curState = 0;
                            recordingValue = false;
                            // if it recorded something good, add it to the list
                            if (IDval != "")
                            {
                                switch (valCategory)
                                {
                                    case 'A':
                                        Allergies.Add(IDval);
                                        break;
                                    case 'D':
                                        Deseases.Add(IDval);
                                        break;
                                    case 'V':
                                        Vaccines.Add(IDval);
                                        break;
                                    case 'M':
                                        Medication.Add(IDval);
                                        break;
                                    case 'H':
                                        HIID = IDval;
                                        break;
                                }

                            }
                            valCategory = '\0';

                            IDval = "";
                            break;
                        default:
                            //if (recordingValue && nrAlphabet.Contains(inputChar)) // if is recording and the input character is a number
                            if (recordingValue)
                            { IDval += inputChar; } //add it to the value
                            break;
                    }
                }
            }
            Console.WriteLine("Number of \n\tAllergies: {0}\n\tDiseases: {1}\n\tVaccines: {2}\n\tMedication: {3}",Allergies.Count,Deseases.Count,Vaccines.Count,Medication.Count);
            List<string>[] temp = new List<string>[] { Allergies, Deseases, Vaccines, Medication };

            pat.recreateDataFile(temp);
            pat.HealthInsuranceId = HIID;
            Console.WriteLine(pat.HealthInsuranceId);
        }

    }

    static class HexStringConverter
    {
        

        static public string StringToHex(string hexstring)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char t in hexstring)
            {
                sb.Append(Convert.ToInt32(t).ToString("X")); //Note: X for upper, x for lower
            }
            return sb.ToString();
        }

        public static byte[] ToByteArray(String HexString)
        {
            int NumberChars = HexString.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(HexString.Substring(i, 2), 16);
            }
            return bytes;
        }

        /// <summary>
        /// returns a string of the hex values
        /// </summary>
        /// <param name="ba"></param>
        /// <returns></returns>
        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        /// <summary>
        /// The string is entered with - between the hex values
        /// </summary>
        /// <param name="HexString"></param>
        /// <returns></returns>
        public static string HexStringToCharText(string HexString)
        {
            string[] hexValuesSplit = HexString.Split('-');
            string outString = "";
            foreach (String hex in hexValuesSplit)
            {
                // Convert the number expressed in base-16 to an integer.
                int value = Convert.ToInt32(hex, 16);
                // Get the character corresponding to the integral value.
                string stringValue = Char.ConvertFromUtf32(value);
                char charValue = (char)value;
                outString += charValue;
            }
            return outString;
        }
    }



    public enum BloodTypes
    {
        OP = 1,//0x01,
        ON = 3,//0x03,
        AP = 7,//0x07,
        AN = 15,//0x15,
        BP = 31,//0x1F,
        BN = 63,//0x3F,
        ABP = 127,//0x7F,
        ABN = 255,//0xFF,
        NOT_DEFINED = 0,//0x00,
    }

}
