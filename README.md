# SinsSpreadsheetGenerator
SinsSpreadsheetGenerator is a command line tool used for analysing the entity files of one or more Sins of a Solar Empire mods and generating a spreadsheet of statistics.

# Installation
Download the latest build version for your operating system. Run SinsSpreadsheetGenerator.exe from the command line or batch program.

# Usage
SinsSpreadsheetGenerator has only one required arguement, a file name or absolute path for the output file. Beyond that, a number of option flags allow customization of the output.
* -d|--Directories: Allows you to give a file path of a mod's base folder to be included in the file. If this flag is not provided, the current directory of the tool is assumed. You can provide this flag multiple times to combine the output of several mods in the spreadsheet. A given entity name will only be output once, and the mods listed first will have priority.
* -f|--Filter: Accepts a substring filter. An entity must have this substring in this file name to be included in the output.
* -s|--Sort: Accepts an integer flag that changes the initial sorting order of the spreadsheet.
  * 0: Type then StatCountType (default) 
  * 1: Name
  * 2: Type
  * 3: StatCountType
  
# File Types
Currently the only supported file type is CSV (Comma Separated Values). This format is supported by almost all spreadsheet and text editor programs, but does not support much formatting.
  
# Credits
Developed by GoaFan77.
  
# License
This project is licensed under the Apache 2.0 License. If you wish to expand on this project, please reach out to me first, but if its clearly abandoned feel free to go ahead and revive it.

If you use this tool to help balance your mod, consider adding a shoutout to GoaFan77 and link to this tool in the credits. :)
