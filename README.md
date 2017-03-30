# ProcapitaBoUExport
Console program that uses IMS Enterprise to export students, faculty and groups from Procapita to a
"flat" structure. 

## Build & deploy
1. Create a new database (Microsoft SQL Server).
2. Create a table using ExportTable.sql to hold the exported data.
3. Rename App.example.config to App.config and update settings to match your environment.
4. Build & run

## App.config
* Connection string - replace [DATABAS SERVER] and [DATABASE NAME] with the server and database name for the destination database.
* [SERVER]:[PORT] - Server and port for the IMS Enterprise web service.
* [DOMAIN ID] - Customer specific domain ID (ask your vendor if you don't have this).
* TruncateQuery - This SQL statement is run against the destination database to prepare the export table.
  You can change this to any statement you want, like a DELETE statement. If you leave it empty it will be ignored.
* DestinationTable - Change this if you named the destination table something other than ProcapitaBoUExport.

## Command line arguments
    -s, --searchdate    Search date.
    -v, --verbose       (Default: False) Prints all messages to standard output.
    --help              Display this help screen.

## Search date
If no search date is specified and the export happens before the wokday is over (before 5:00 PM),
yesterdays data is exported instead. If you schedule an export at night, after midnight, this is 
the behaviour you would expect.
