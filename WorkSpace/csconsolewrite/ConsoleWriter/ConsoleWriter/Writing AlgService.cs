using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleWriter
{
    static class Writing_AlgService
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
            foreach(byte b in barray)
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

        public Patient(BloodTypes BT, List<String>[] Data)
        {
            this.BloodType = BT;
            this.Diseases = Data[0];
            this.Allergies = Data[1];
            this.Vacinnes = Data[2];
        }

        public override string ToString()
        {
            String DString = "42" + ((int)this.BloodType).ToString("X");


            // start with allergies
            DString += "\\A";
            foreach (string _identifier in this.Allergies)
            {
                DString += _identifier;
            }
            // start with vaccines
            DString += "\\V";
            foreach (string _identifier in this.Vacinnes)
            {
                DString += _identifier;
            }
            // start with deseases
            DString += "\\D";
            foreach (string _identifier in this.Diseases)
            {
                DString += _identifier;
            }
            
            return DString;
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
    }

    public enum BloodTypes
    {
        OP = 0x01,
        ON = 0x03,
        AP = 0x07,
        AN = 0x15,
        BP = 0x1F,
        BN = 0x3F,
        ABP = 0x7F,
        ABN = 0xFF,
    }

}
