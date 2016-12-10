﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCSC;
using PCSC.Iso7816;


namespace ConsoleWriter
{
    class Program
    {

        private static readonly byte[] DATA_TO_WRITE = {
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00
        };

        private const byte MSB = 0x00;
        private const byte LSB = 0x08;


        public static void Main()
        {
            var contextFactory = ContextFactory.Instance;
            using (var context = contextFactory.Establish(SCardScope.System))
            {
                var readerNames = context.GetReaders();


                var readerName = ChooseReader(readerNames);
                if (readerName == null)
                {
                    return;
                }

                using (var isoReader = new IsoReader(context, readerName, SCardShareMode.Shared, SCardProtocol.Any, false))
                {
                    var card = new MifareCard(isoReader);

                    var loadKeySuccessful = card.LoadKey(
                        KeyStructure.NonVolatileMemory,
                        0x00, // first key slot
                        new byte[] { 0xD3, 0xF7, 0xD3, 0xF7, 0xD3, 0xF7 } // key
                    );

                    if (!loadKeySuccessful)
                    {
                        throw new Exception("LOAD KEY failed.");
                    }

                    var authSuccessful = card.Authenticate(MSB, LSB, KeyType.KeyA, 0x00);
                    if (!authSuccessful)
                    {
                        throw new Exception("AUTHENTICATE failed.");
                    }

                    var result = card.ReadBinary(MSB, LSB, 16);
                    Console.WriteLine("Result (before BINARY UPDATE): {0}",
                        (result != null)
                            ? BitConverter.ToString(result)
                            : null);

                    var updateSuccessful = card.UpdateBinary(MSB, LSB, DATA_TO_WRITE);

                    if (!updateSuccessful)
                    {
                        throw new Exception("UPDATE BINARY failed.");
                    }

                    result = card.ReadBinary(MSB, LSB, 16);
                    Console.WriteLine("Result (after BINARY UPDATE): {0}",
                        (result != null)
                            ? BitConverter.ToString(result)
                            : null);
                }
            }
            Console.ReadKey();
        }

        /// <summary>
        /// Asks the user to select a smartcard reader containing the Mifare chip
        /// </summary>
        /// <param name="readerNames">Collection of available smartcard readers</param>
        /// <returns>The selected reader name or <c>null</c> if none</returns>
        private static string ChooseReader(IList<string> readerNames)
        {
            Console.WriteLine(new string('=', 79));
            Console.WriteLine("WARNING!! This will overwrite data in MSB {0:X2} LSB {1:X2} using the default key.", MSB,
                LSB);
            Console.WriteLine(new string('=', 79));

            // Show available readers.
            Console.WriteLine("Available readers: ");
            for (var i = 0; i < readerNames.Count; i++)
            {
                Console.WriteLine("[" + i + "] " + readerNames[i]);
            }

            // Ask the user which one to choose.
            Console.Write("Which reader has an inserted Mifare 1k/4k card? ");

            var line = Console.ReadLine();
            int choice;

            if (int.TryParse(line, out choice) && (choice >= 0) && (choice <= readerNames.Count))
            {
                return readerNames[choice];
            }

            Console.WriteLine("An invalid number has been entered.");
            Console.ReadKey();

            return null;
        }

        private static bool NoReaderAvailable(ICollection<string> readerNames)
        {
            return readerNames == null || readerNames.Count < 1;
        }

    }
}