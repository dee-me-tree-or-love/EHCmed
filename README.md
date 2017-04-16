## Smart European Health Cards   
A prototype of a  medical information system evolving around the use of NFC smartcards (Mifare 1 as a test prototype).
Here you can find a presentation explaining the basic idea: [Google Slides](https://docs.google.com/presentation/d/1D8QVglbv19uhtTNWZLzixNHsGRtxk2SA-pyhikk94fU/edit?usp=sharing) 

### Disclaimer
Work done during the hackathon, so it is indeed quite a mess to navigate through
The below is an attempt to revise the structure and explain it better, but...  
`"...some things that should not have been forgotten were lost. History became legend."`  

### Repo Structure   
We have separated the global project into two parts: one for Windows enabled PCs, one part for RasPis   
#### Database  
Was supposed to serve the use, but was discarded during the hackathon for the lack of time  
#### Web Application  
A demo of the web application that would've been used in the IS by the clients to notify the doctors of any changes  
#### Read Write App   
A basic win forms application that works with the NFC cards in C#.
#### Workspace   
Intended to be a location for all the work, but now stores the folders:  
- ConsoleWriter (C# application that interacts with the NFC device reader via USB)   
- py (was used for testing, but was not deleted...)  
- pythonreader (the application in python to interact with the smart cards)   
  - excel files are the "datasets"
  - dataManagement.py - is a very basic GUI application
  - medDataManagement.py - works with the excel spreadsheets (supposedly)
  - readerMain.py - used for reading the data from the card
