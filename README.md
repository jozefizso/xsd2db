# XSD2DB

> XSD2DB is a command line tool written in C#, that will read a Microsoft ADO.NET compatible DataSet Schema File (XSD) and generate a database.

## Introduction

A fork of the original XSD2DB project started by Alexis Smirnov in 2003, released under a BSD license.

## Features

Generate databases from Microsoft ADO.NET compatible DataSet Schema Files (XSD).
Supports relations, primary keys, blob fields and memo fields.

## Available Parameters

| Parameter        | Description                                                                                                                                                                |
|------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| help (-h)        | Display usage instructions                                                                                                                                                 |
| force (-f)       | drop the database if it exists.                                                                                                                                            |
| location (-l)    | The type of database to connect to (servername). When SQL Server is used, the connection is created using Windows Authentication, SQL Authentication is not supported yet. |
| name (-n)        | the name of the database to be created [version 0.1 does support using an existing database]                                                                               |
| schema (-s)      | The the XSD file containing the schema                                                                                                                                     |
| type (-t)        | The type of database to connect to [ Jet \| Sql \| OleDb ]                                                                                                                 |
| tableprefix (-p) | This is a prefix added to each table                                                                                                                                       |
| dbowner (-o)     | prefix added to each table. This parameter is ignored if a database other than SQL Server is used.                                                                         |
| UseExisting (-e) | Use an existing database                                                                                                                                                   |

### Use with Powershell

```powershell
#
# xsd2db.ps1
#
#
$exePath = 'c:\SomeFolder\xsd2db.exe'            # Path to the installation folder of xsd2dg.exe
$directoryPath = 'c:\SomeFolder\SomeChildFolder' # Path your directory containing schemas
$serverName = 'ServerName'                       # Your Database Server Name, i.e. localhost
$dbName = 'DatabaseName'                         # Your Database Name

Get-ChildItem $directoryPath -Filter '*.xsd' | ForEach-Object {
  & $exePath -f -l $serverName -n $dbName -s $_.FullName -t sql -e
}
```

## Licence

Copyright (c) 2003, Zero-Knowledge Systems Inc.  
Copyright (c) 2003, Alexis Smirnov

Credit to [Adam Bertram](http://www.adamtheautomator.com/ "Adam Bertram - Adam the Automator")
