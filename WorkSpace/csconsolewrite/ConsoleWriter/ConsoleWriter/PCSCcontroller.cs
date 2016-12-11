using System;
using PCSC;
using PCSC.Iso7816;

namespace ConsoleWriter
{
    public class MifareCard
    {
        private const byte CUSTOM_CLA = 0xFF;
        private readonly IIsoReader _isoReader;

        public MifareCard(IIsoReader isoReader)
        {
            if (isoReader == null)
            {
                throw new ArgumentNullException(nameof(isoReader));
            }
            _isoReader = isoReader;
        }

        public bool LoadKey(KeyStructure keyStructure, byte keyNumber, byte[] key)
        {
            var loadKeyCmd = new CommandApdu(IsoCase.Case3Short, SCardProtocol.Any)
            {
                CLA = CUSTOM_CLA,
                Instruction = InstructionCode.ExternalAuthenticate, //0x82 == 130 dec
                P1 = (byte)keyStructure,
                P2 = keyNumber,
                Data = key
            };

            Console.WriteLine("Load Authentication Keys: {0}", BitConverter.ToString(loadKeyCmd.ToArray()));
            var response = _isoReader.Transmit(loadKeyCmd);
            Console.WriteLine("SW1 SW2 = {0:X2} {1:X2}", response.SW1, response.SW2);

            return IsSuccess(response);
        }

        /// <summary>
        /// alaAPDU = {FF 86 00 00 05 01 00 04 60 00}
        /// </summary>
        /// <param name="msb"></param>
        /// <param name="lsb"></param>
        /// <param name="keyType"></param>
        /// <param name="keyNumber"></param>
        /// <returns></returns>
        public bool Authenticate(byte msb, byte lsb, KeyType keyType, byte keyNumber)
        {
            var authBlock = new GeneralAuthenticate
            {
                MSB = msb,
                LSB = lsb,
                KeyNumber = keyNumber,
                KeyType = keyType
            };

            var authKeyCmd = new CommandApdu(IsoCase.Case3Short, SCardProtocol.Any)
            {
                CLA = CUSTOM_CLA,
                Instruction = InstructionCode.InternalAuthenticate,
                P1 = 0x00,
                P2 = 0x00,
                Data = authBlock.ToArray()
            };

            Console.WriteLine("General Authenticate: {0}", BitConverter.ToString(authKeyCmd.ToArray()));
            var response = _isoReader.Transmit(authKeyCmd);
            Console.WriteLine("SW1 SW2 = {0:X2} {1:X2}", response.SW1, response.SW2);

            return (response.SW1 == 0x90) && (response.SW2 == 0x00);
        }

        public byte[] ReadBinary(byte msb, byte lsb, int size)
        {
            unchecked
            {
                var readBinaryCmd = new CommandApdu(IsoCase.Case2Short, SCardProtocol.Any)
                {
                    CLA = CUSTOM_CLA,
                    Instruction = InstructionCode.ReadBinary,
                    P1 = msb,
                    P2 = lsb,
                    Le = size
                };

                Console.WriteLine("Read Binary (before update): {0}", BitConverter.ToString(readBinaryCmd.ToArray()));
                var response = _isoReader.Transmit(readBinaryCmd);
                Console.WriteLine("SW1 SW2 = {0:X2} {1:X2} Data: {2}",
                    response.SW1,
                    response.SW2,
                    BitConverter.ToString(response.GetData()));

                return IsSuccess(response)
                    ? response.GetData() ?? new byte[0]
                    : null;
            }
        }

        public bool UpdateBinary(byte msb, byte lsb, byte[] data)
        {
            var updateBinaryCmd = new CommandApdu(IsoCase.Case3Short, SCardProtocol.Any)
            {
                CLA = CUSTOM_CLA,
                Instruction = InstructionCode.UpdateBinary,
                P1 = msb,
                P2 = lsb,
                Data = data
            };

            Console.WriteLine("Update Binary: {0}", BitConverter.ToString(updateBinaryCmd.ToArray()));
            var response = _isoReader.Transmit(updateBinaryCmd);
            Console.WriteLine("SW1 SW2 = {0:X2} {1:X2}", response.SW1, response.SW2);

            return IsSuccess(response);
        }


        public bool UpdateCard(byte[] data, KeyType keyType, byte keyNumber)
        {
            // would have to store the whole personal record
            byte[][] _arrayOfArrays = new byte[8][];
            for (int l = 0; l < 8; l++)
            {
                _arrayOfArrays[l] = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            }

            int box = 0;
            int line = 0;
            foreach (byte b in data)
            {
                _arrayOfArrays[line][box] = b;
                box++;
                // if reached the end of acceptable byte line
                if (box == 16)
                {
                    // reset the pointer to the first box, go to next line
                    box = 0;
                    line++;
                }
            }

            // processes the array and writes the data to the card
            try
            {
                this.writeCyclic(_arrayOfArrays, keyType, keyNumber);
            }
            catch(Exception e)
            {
                Console.WriteLine("CRIT: exception occured: " + e.Message);
            }

            return false;
        }

        private bool writeCyclic(byte[][] DataPack, KeyType keyType, byte keyNumber)
        {
            const byte P1 = 0x00; // signifies the P1 block part and is supposed to be zero
            const int mappingIndex = 1; // signifies the second part of a mapping
            // unaccessible blocks - 7th, 11th, 13th, 15th
            int[,] lineToBlockNrMapping = new int[8, 2]
                {
                    { 0, 4 }, { 1, 5 }, { 2, 6 }, { 3, 8 }, { 4, 9 },
                    { 5, 10 }, { 6, 12 }, { 7, 14 }
                };
            bool result = false; // will show if everything went alright
            for (int l = 0; l < 8; l++)
            {
                //for (int b = 0; b < 16; b++)
                //{
                //    Console.Write("[" + DataPack[l][b] + "]");
                //}
                //Console.WriteLine();
                // very careful! very fucking careful!
                // signifies the block number to write to
                byte blockNr = (byte)lineToBlockNrMapping[l, mappingIndex];
                Console.WriteLine("Authenticating block number: " + blockNr);
                var authSuccessful = this.Authenticate(P1, blockNr, KeyType.KeyA, 0x00);
                if (!authSuccessful)
                {
                    throw new Exception("AUTHENTICATE failed.");
                }
                //P1 - kept 0x00, lineNr ->maps-> blockNr, the data of the l'th datapack 
                result = this.UpdateBinary(P1, blockNr, DataPack[l]);
            }
            return result;
        }

        public byte[] ReadCard(out string longText)
        {
            longText = "";
            byte[] barray = { };
            const byte P1 = 0x00; // signifies the P1 block part and is supposed to be zero
            const int mappingIndex = 1; // signifies the second part of a mapping
            // unaccessible blocks - 7th, 11th, 13th, 15th
            int[,] lineToBlockNrMapping = new int[8, 2]
                {
                    { 0, 4 }, { 1, 5 }, { 2, 6 }, { 3, 8 }, { 4, 9 },
                    { 5, 10 }, { 6, 12 }, { 7, 14 }
                };

            for (int l = 0; l<8; l++)
            {
                byte blockNr = (byte)lineToBlockNrMapping[l, mappingIndex];
                Console.WriteLine("Authenticating block number: " + blockNr);
                var authSuccessful = this.Authenticate(P1, blockNr, KeyType.KeyA, 0x00);
                if (!authSuccessful)
                {
                    throw new Exception("AUTHENTICATE failed.");
                }
                //P1 - kept 0x00, lineNr ->maps-> blockNr, the data of the l'th datapack 
                var data = this.ReadBinary(P1, blockNr, 16);
                try
                {
                    Console.WriteLine(BitConverter.ToString(data));
                    longText += "-"+BitConverter.ToString(data);

                    Byte[] temp_ba = barray;
                    barray = new byte[barray.Length + data.Length];
                    temp_ba.CopyTo(barray, 0);
                    data.CopyTo(barray, temp_ba.Length);
                }
                catch
                {
                    Console.WriteLine("Couldn't read");
                }
            }
            // outside of the loop
            longText = longText.Substring(1);
            return barray;
        }



        private static bool IsSuccess(Response response) =>
            (response.SW1 == (byte)SW1Code.Normal) && (response.SW2 == 0x00);
    }


    public class GeneralAuthenticate
    {
        public byte Version { get; } = 0x01;

        public byte MSB { get; set; }
        public byte LSB { get; set; }
        public KeyType KeyType { get; set; }
        public byte KeyNumber { get; set; }

        public byte[] ToArray()
        {
            return new[] { Version, MSB, LSB, (byte)KeyType, KeyNumber };
        }
    }


    public enum KeyType : byte
    {
        KeyA = 0x60,
        KeyB = 0x61
    }


    public enum KeyStructure : byte
    {
        VolatileMemory = 0x00,
        NonVolatileMemory = 0x20
    }
}