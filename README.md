# SSIS Package Management and Validation

This project provides tools for managing and validating SQL Server Integration Services (SSIS) packages. It includes scripts for converting PowerMart files to DTSX packages and validating DTSX files.

## Table of Contents

- [Overview](#overview)
- [SSISWrapper](#ssiswrapper)
  - [Adding an OLEDB Connection](#adding-an-oledb-connection)
  - [Adding an ODBC Connection](#adding-an-odbc-connection)
  - [Adding an ADO Connection](#adding-an-ado-connection)
  - [Adding an ADO.NET:SQL Connection](#adding-an-adonet-sql-connection)
  - [Adding an ADO.NET:OLEDB Connection](#adding-an-adonet-oledb-connection)
  - [Adding a Flat File Connection](#adding-a-flat-file-connection)
  - [Adding a File Connection](#adding-a-file-connection)
- [Scripts](#scripts)
  - [InformaticaX_SSIS.ps1](#informaticax_ssisps1)
  - [DTSXValidator.ps1](#dtsxvalidatorps1)
- [Usage](#usage)
  - [Converting PowerMart Files](#converting-powermart-files)
  - [Validating DTSX Files](#validating-dtsx-files)
- [Configuration](#configuration)
- [Contributing](#contributing)
- [License](#license)

## Overview

This project contains PowerShell scripts to assist with the conversion of PowerMart files to SSIS packages and the validation of these packages. The main components are:

- `InformaticaX_SSIS.ps1`: Converts PowerMart XML files to SSIS DTSX packages.
- `DTSXValidator.ps1`: Validates the generated DTSX packages using `dtexec`.
- `SSISWrapper`: A C# executable used within the `InformaticaX_SSIS.ps1` script to facilitate the conversion of PowerMart XML files to SSIS DTSX packages.

## SSISWrapper

**Adding an OLEDB Connection**

```bash
SSISWrapper.exe --add-connection --type OLEDB --name MyConnection --connection-string "Provider=SQLNCLI11;Data Source=MyServer;Initial Catalog=MyDatabase;Integrated Security=SSPI;" --output "C:\path\to\output\package.dtsx"
```

**Adding an ODBC Connection**

```bash
SSISWrapper.exe --add-connection --type ODBC --name MyConnection --connection-string "DSN=MyODBCDataSource;UID=MyUser;PWD=MyPassword;" --output "C:\path\to\output\package.dtsx"
```

**Adding an ADO Connection**

```bash
dotnet run --add-connection --type ADO --name "MyADOConnection" --connection-string "Provider=SQLOLEDB.1;Integrated Security=SSPI;Initial Catalog=AdventureWorks;Data Source=(local);" --output "C:\dev\PS_InformaticaX_SSIS\output.dtsx"
```

**Adding an ADO.NET:SQL Connection**

```bash
dotnet run --add-connection --type "ADO.NET:SQL" --name "MyADONETSQLConnection" --connection-string "Data Source=(local);Initial Catalog=AdventureWorks;Integrated Security=True;" --output "C:\dev\PS_InformaticaX_SSIS\output.dtsx"
```

**Adding an ADO.NET:OLEDB Connection**

```bash
dotnet run --add-connection --type "ADO.NET:OLEDB" --name "MyADONETOLEDBConnection" --connection-string "Provider=SQLNCLI11;Data Source=MyServer;Initial Catalog=MyDatabase;Integrated Security=SSPI;" --output "C:\dev\PS_InformaticaX_SSIS\output.dtsx"
```

**Adding a Flat File Connection**

```bash
dotnet run --add-connection --type FLATFILE --name "MyFlatFileConnection" --connection-string "C:\\path\\to\\file.txt" --output "C:\dev\PS_InformaticaX_SSIS\output.dtsx"
```

**Adding a File Connection**

```bash
dotnet run --add-connection --type FLATFILE --name "MyFlatFileConnection" --connection-string "C:\\path\\to\\file.txt" --output "C:\dev\PS_InformaticaX_SSIS\output.dtsx"
```

**Adding a Multi Flat File Connection**

```bash
dotnet run --add-connection --type MULTIFLATFILE --name "MyMultiFlatFileConnection" --connection-string "C:\\path\\to\\files\\*.txt" --output "C:\dev\PS_InformaticaX_SSIS\output.dtsx"
```

**Adding a Multi File Connection**

```bash
dotnet run --add-connection --type MULTIFILE --name "MyMultiFileConnection" --connection-string "C:\\path\\to\\files\\*.txt" --output "C:\dev\PS_InformaticaX_SSIS\output.dtsx"
```

**Adding a SQL Mobile Connection**

```bash
dotnet run --add-connection --type SQLMOBILE --name "MySQLMobileConnection" --connection-string "Data Source=C:\\path\\to\\database.sdf" --output "C:\dev\PS_InformaticaX_SSIS\output.dtsx"
```

**Adding an Analysis Services Connection**

```bash
dotnet run --add-connection --type MSOLAP100 --name "MyAnalysisServicesConnection" --connection-string "Data Source=(local);Initial Catalog=AdventureWorks" --output "C:\dev\PS_InformaticaX_SSIS\output.dtsx"
```

**Adding an FTP Connection**

```dotnet run --add-connection --type FTP --name "MyFTPConnection" --connection-string "Server=myftpserver;User=myuser;Password=mypassword" --output "C:\dev\PS_InformaticaX_SSIS\output.dtsx"
```

**Adding an HTTP Connection**

```dotnet run --add-connection --type HTTP --name "MyHTTPConnection" --connection-string "URL=http://myserver;User=myuser;Password=mypassword" --output "C:\dev\PS_InformaticaX_SSIS\output.dtsx"```

**Adding an MSMQ Connection**

```dotnet run --add-connection --type MSMQ --name "MyMSMQConnection" --connection-string "FormatName:DIRECT=OS:myserver\\private$\\myqueue" --output "C:\dev\PS_InformaticaX_SSIS\output.dtsx"```

**Adding an SMTP Connection**

```bash
dotnet run --add-connection --type SMTP --name "MySMTPConnection" --connection-string "Server=smtp.myserver.com;User=myuser;Password=mypassword" --output "C:\dev\PS_InformaticaX_SSIS\output.dtsx"```

**Adding a WMI Connection**

```bash
dotnet run --add-connection --type WMI --name "MyWMIConnection" --connection-string "Server=\\myserver;Namespace=\\root\\cimv2" --output "C:\dev\PS_InformaticaX_SSIS\output.dtsx"```