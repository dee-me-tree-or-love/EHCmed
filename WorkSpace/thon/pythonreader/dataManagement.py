from readermain import *
import sys
from PyQt5.QtCore import Qt
from PyQt5.QtWidgets import (QWidget, QPushButton, QListWidget,
    QVBoxLayout, QApplication,QGridLayout)


class UI(QWidget):
    def __init__(self):
        super().__init__()

        self.initUI()

    def initUI(self):
        btn1 = QPushButton("Retrieve Data")
        btn1.move(10, 10)
        self.lvAl = QListWidget()

        vbox = QVBoxLayout(self)
        vbox.addWidget(btn1)
        vbox.addWidget(self.lvAl)


        # btn1.setGeometry(120, 120)
        btn1.clicked.connect(self.buttonClicked)


        self.setGeometry(1200, 480, 320, 320)
        self.setWindowTitle('Get Patient Data')
        self.show()
        self.adjustSize()

    def buttonClicked(self):
        patData = run()

        self.lvAl.insertItem(0, "----------|Patient Information|----------")
        self.lvAl.addItem("Health Insurance number: " + str(patData.hiid))
        self.lvAl.addItem("Blood Type: " + str(patData.bt))
        self.lvAl.addItem("Allergies")
        for a in patData.alerg:
            self.lvAl.addItem(str(a) + ": " + str(getName('a', a)))
        self.lvAl.addItem("Diseases:")
        for a in patData.dis:
            self.lvAl.addItem(str(a) + ": " + str(getName('d', a)))
        self.lvAl.addItem("Vaccines taken:")
        for a in patData.vac:
            self.lvAl.addItem(str(a) + ": " + str(getName('v', a)))
        self.lvAl.addItem("Prescribed medicaments: ")
        for a in patData.med:
            self.lvAl.addItem(str(a) + ": " + str(getName('m', a)))
        self.lvAl.show()
        return patData




if __name__ == '__main__':
    app = QApplication(sys.argv)
    ex = UI()
    sys.exit(app.exec_())

