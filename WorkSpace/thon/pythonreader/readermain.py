from smartcard.System import readers
from smartcard.util import toHexString
from smartcard.ATR import ATR
from smartcard.CardType import AnyCardType
import binascii
import sys
from medDataManagement import *

class PatFile:
    def __init__(self):
        self.alerg = {}
        self.vac = {}
        self.dis = {}
        self.med = {}
        self.bt = ''
        self.hiid = ''

bloodgroups = {
        '1':'OP', '3':'ON',
        '7':'AP','f':'AN',
        '1f':'BP','3f':'BN',
        '7f':'ABP','ff':'ABN'
}




#read all the data from the data blocks in binary
def readCycleInto():
    # to do, starting the script with algorithms
    # detect command "start"

    r = readers()
    if len(r) < 1:
        print("error: No readers available")
        sys.exit()

    print("Available readers: ", r)
    # specified reader to be used
    reader = r[0]
    print("Using: ", reader)

    connection = reader.createConnection()
    connection.connect()

    # load key
    COMMAND = [0xFF, 0x82, 0x00, 0x00, 0x06]
    # the key hardcoded for the temporary use
    key = [0xD3, 0xF7, 0xD3, 0xF7, 0xD3, 0xF7]
    # for i in range(6):
    #     key[i] = int(key[i], 16)
    COMMAND.extend(key)

    # loading the key
    data, sw1, sw2 = connection.transmit(COMMAND)
    print("Status words: {0} {1}".format(sw1, sw2))
    if (sw1, sw2) == (0x90, 0x0):
        print("KeyLoad: Key is loaded successfully to " + str(r[0]) + "key #0.")
    elif (sw1, sw2) == (0x63, 0x0):
        print("KeyLoad: Failed to load key.")

    data = []
    c_b = 4;
    while(c_b <= 15):
        print(str(c_b))
        #avoid control blocks
        if(c_b == 7):
            print("CONTROL BLOCK ENCOUNTERED")
        elif(c_b == 11):
            print("CONTROL BLOCK ENCOUNTERED")
        elif (c_b == 13):
            print("CONTROL BLOCK ENCOUNTERED")
        elif (c_b == 15):
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

#parse med data and get the correct representation of it
def parseMedData(data, loadedPat):
    #data is a string
    print(data)
    print("splitting")
    categories = {'A':{},'D':{},'V':{},'M':{},'H':{}}
    sectionArray = {}
    j=0
    dataArray = str(data).split('>')
    for i in dataArray:
        print(i)# i is a line
        if(i.__contains__('#')):
            print("go")
            k = str(i).split('#')
            print(k.__len__())
            if(k[0] == 'A'):
                loadedPat.alerg = k[1:]
            if (k[0] == 'D'):
                loadedPat.dis = k[1:]
            if (k[0] == 'V'):
                loadedPat.vac = k[1:]
            if (k[0] == 'M'):
                loadedPat.med = k[1:]
            if (k[0] == 'H'):
                loadedPat.hiid = k[1:]
    return loadedPat

#what is starting the app to when running



def retrieveFromCard():
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
    # longpar = longpar.lstrip("0")
    #longpar = longpar.rstrip("0")
    # if(longpar.length)
    k ={}
    if(longpar[3]+longpar[4])=='3e':
        btSection = longpar[:3]
        temper = longpar[3:]

        k = binascii.unhexlify(longpar[3:]+'0')

    else:
        btSection = longpar[:4]
        k=binascii.unhexlify(longpar[4:])
    print(k)

    print(bloodgroups[btSection[2:]])
    return [bloodgroups[btSection[2:]],k]

    #k = bytearray.fromhex(longpar).decode()
# 1,2,3 GOOOOOOOOO!

def run():
    #creating an instance of a file
    loadedPat = PatFile()
    data = retrieveFromCard()
    loadedPat.bt = data[0]
    loadedPat = parseMedData(data[1],loadedPat)

    print("--------------|||--------------")
    print("Info for patient with Health Insurance: " + str(loadedPat.hiid))
    print("Blood Group: " + loadedPat.bt)
    print("Allergies:")
    for a in loadedPat.alerg:
        print(str(a)+": "+str(getName('a',a)))
    print("Diseases:")
    for a in loadedPat.dis:
        print(str(a)+": "+str(getName('d',a)))
    print("Vaccines taken:")
    for a in loadedPat.vac:
        print(str(a)+": "+str(getName('v',a)))
    print("Prescribed medicaments: ")
    for a in loadedPat.med:
        print(str(a)+": "+str(getName('m',a)))

    return loadedPat

if __name__ == "__main__":
    run()
