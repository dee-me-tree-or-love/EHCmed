from openpyxl import load_workbook

wb = load_workbook(filename='allergies.xlsx', read_only=True)
ws = wb['a']
def showValues():

    for row in ws.rows:
        print(row[1].value)


def getName(catPage,id):
    ws = wb[catPage]
    for row in ws.rows:
        if(row[1].value[1:] == str(id)):
            return[row[0].value]
    return "NaN"