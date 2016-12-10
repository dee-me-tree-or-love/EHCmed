﻿using System;
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