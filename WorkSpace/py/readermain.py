from smartcard.System import readers
from smartcard.util import toHexString
from smartcard.ATR import ATR
from smartcard.CardType import AnyCardType
import binascii

import sys



#to do, starting the script with algorithms
#detect command "start"

r = readers()
if len(r) < 1:
    print("error: No readers available")
    sys.exit()

print("Available readers: ", r)
# specified reader to be used
reader = r[0]
print ("Using: ", reader)

connection = reader.createConnection()
connection.connect()




#load key
COMMAND = [0xFF, 0x82, 0x00, 0x00, 0x06]
# the key hardcoded for the temporary use
key = [0xD3, 0xF7, 0xD3, 0xF7, 0xD3, 0xF7]
# for i in range(6):
#     key[i] = int(key[i], 16)
COMMAND.extend(key)


#loading the key
data, sw1, sw2 = connection.transmit(COMMAND)
print("Status words: {0} {1}".format(sw1, sw2))
if (sw1, sw2) == (0x90, 0x0):
    print("KeyLoad: Key is loaded successfully to " + str(r[0]) + "key #0.")
elif (sw1, sw2) == (0x63, 0x0):
    print("KeyLoad: Failed to load key.")


#read all the data from the data blocks in binary
def readCycleInto():
    data = []
    c_b = 4;
    while(c_b <= 15):
        print(str(c_b))
        #avoid control blocks
        if(c_b == 7):
            print("CONTROL BLOCK ENCOUNTERED")
        elif(c_b == 13):
            print("CONTROL BLOCK ENCOUNTERED")
        else:
            #if not falling on the control blocks
            #Authentication
                # FF
                # 88
                # 00
                # Block Number
                # Key Type
                # Key Number
            APDUauth = [0xFF, 0x88, 0x00, int(str(c_b),16), 0x60, 0x00]
            APDUread = [0xFF, 0xB0, 0x00, int(str(c_b),16), 0x10]
            #try to authorize
            try:
                dataRead, sw1, sw2 = connection.transmit(APDUauth)
                #if authorized
                if (sw1, sw2) == (0x90, 0x0):
                    print("Status: Decryption block " + str(c_b) + " using key #0 as Key A successful.")
                    #try to read
                    dataRead, sw1, sw2 = connection.transmit(APDUread)
                    data.append(dataRead)
                    #display what was read
                    print("Status: Done, block " + str(c_b) + " data: " + str(dataRead))
                    if (sw1, sw2) == (0x90, 0x0):
                        print("Status: The read procedure completed successfully.")
                    elif (sw1, sw2) == (0x63, 0x0):
                        print("Status: The read procedure failed. Maybe go fuck yourself.")
            # dataRead, sw1, sw2 = connection.transmit(APDUauth)
            # # if authorized
            # if (sw1, sw2) == (0x90, 0x0):
            #     print("Status: Decryption block " + str(c_b) + " using key #0 as Key A successful.")
            #     # try to read
            #     dataRead, sw1, sw2 = connection.transmit(APDUread)
            #     data.append(dataRead)
            #     # display what was read
            #     print("Status: Done, block " + str(c_b) + " data: " + str(dataRead))
            #     if (sw1, sw2) == (0x90, 0x0):
            #         print("Status: The read procedure completed successfully.")
            #     elif (sw1, sw2) == (0x63, 0x0):
            #         print("Status: The read procedure failed. Maybe go fuck yourself.")

            except Exception as e:
                print("ni!"+str(e))
                #if something went wrong, go out
                break
            # sort of oky, if everything was executed properly, get next block and repeat
        c_b += 1  # next block
    print("I am done")
    return data






#what is starting the app to when running
def run():
    # start cycling
    dataArray = readCycleInto()
    print(str(dataArray))
    longpar = ""
    for block in dataArray:
        if (sum(block) != 0):
            for bb in block:
                longpar+=(hex(bb)[2:])

            # longpar.append(bytes(hex(bb)))
    print(longpar)
    # longpar = longpar.decode('utf-8')

    # rrr = ''.join(hex(ord(x))[2:] for x in 'Hello World!')
    # print(rrr)
    k = binascii.unhexlify(longpar.lstrip("0"))
    print(k)
    #k = bytearray.fromhex(longpar).decode()
# 1,2,3 GOOOOOOOOO!
run()