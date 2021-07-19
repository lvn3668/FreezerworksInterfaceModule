README for the Freezerworks Interface Module (FIM)

The MITRE Corporation 2010

Installation:
1. Unzip entire contents of the zipfile to an empty folder.

Configuration of Thumbdrive Drive Letters:
1. Open FreezerworksInterfaceModule.exe.config with Notepad or another source editing 
   application (not Internet Explorer or Microsoft Word).
2. Under the <appsettings> section are various configuration items.
   a. Edit this line:
   	<add key="ImportDrive" value="F:"></add>
   replacing F: with the drive letter of the thumbdrive that will contain LFHM CSV data files to import.
   
   b. Edit this line:
   	<add key="ExportDrive" value="F:"></add>
   replacing F: with the drive letter of the thumbdrive that will store the exported FIM CSV data files.

Running the application:
1. Make sure the VisionMate scanner software is already running.
2. Double click on FreezerworksInterfaceModule.exe.
