# SSIS Package Constructor Commandline, PowerShell Package Management and Validation

This project provides tools for managing and validating SQL Server Integration Services (SSIS) packages. It includes scripts for converting PowerMart files to DTSX packages and validating DTSX files.

## Special Note

This project includes a unique command-line utility for managing and validating SSIS packages. As of now, it is likely the only DTSX command-line utility available on the Internet. This tool provides a comprehensive set of features for adding connections, tasks, precedence constraints, variables, and enabling logging within SSIS packages. It also supports the conversion of PowerMart files to DTSX packages and the validation of these packages. The utility is designed to streamline the process of working with SSIS packages, making it easier for developers and data professionals to manage their ETL workflows efficiently.

## Table of Contents

- [Overview](#overview)
- [SSISWrapper](#ssiswrapper)
  - [Adding Connections](#adding-connections)
    - [Adding an OLEDB Connection](#adding-an-oledb-connection)
    - [Adding an ODBC Connection](#adding-an-odbc-connection)
    - [Adding an ADO Connection](#adding-an-ado-connection)
    - [Adding an ADO.NET:SQL Connection](#adding-an-adonet-sql-connection)
    - [Adding an ADO.NET:OLEDB Connection](#adding-an-adonet-oledb-connection)
    - [Adding a Flat File Connection](#adding-a-flat-file-connection)
    - [Adding a File Connection](#adding-a-file-connection)
  - [Adding Tasks](#adding-tasks)
  - [Adding Precedence Constraints](#adding-precedence-constraints)
  - [Adding Variables](#adding-variables)
  - [Enabling Logging](#enabling-logging)
  - [Adding Data Flow Tasks](#adding-data-flow-tasks)
  - [Listing Data Flow Components](#listing-data-flow-components)
  - [Adding Data Flow Components](#adding-data-flow-components)
  - [Connecting Data Flow Components](#connecting-data-flow-components)
  - [Selecting Input Columns](#selecting-input-columns)
- [Scripts](#scripts)
  - [InformaticaX_SSIS.ps1](#informaticax_ssisps1)
  - [DTSXValidator.ps1](#dtsxvalidatorps1)
- [Usage](#usage)
  - [Converting PowerMart Files](#converting-powermart-files)
  - [Validating DTSX Files](#validating-dtsx-files)
- [Configuration](#configuration)
- [Contributing](#contributing)
- [License](#license)

## Informatica Big Data Cloud API to SSIS Migration

Yes, this project can be adapted to work with the Informatica Big Data Cloud API for SSIS migration. Here is how it can be done:

1. **API Integration**: Modify the `InformaticaX_SSIS.ps1` script to include API calls to the Informatica Big Data Cloud. This can be done using PowerShell's `Invoke-RestMethod` or `Invoke-WebRequest` cmdlets to interact with the API and retrieve the necessary data.

2. **Data Extraction**: Extract the required metadata and data from the Informatica Big Data Cloud using the API. This includes information about the source and target systems, mappings, transformations, and other relevant details.

3. **Data Transformation**: Use the extracted data to create the corresponding SSIS components. This involves mapping the Informatica components to their SSIS equivalents. The `SSISWrapper` utility can be extended to support additional components and transformations as needed.

4. **Package Generation**: Generate the SSIS packages using the `SSISWrapper` utility. The utility can be invoked from the PowerShell script with the appropriate parameters to create the DTSX packages based on the extracted data.

5. **Validation**: Validate the generated SSIS packages using the `DTSXValidator.ps1` script. This ensures that the packages are correctly configured and can be executed without errors.

### Example Workflow

1. **Retrieve Metadata**: Use the Informatica Big Data Cloud API to retrieve metadata about the source and target systems.
2. **Map Components**: Map the Informatica components to their SSIS equivalents.
3. **Generate Packages**: Use the `SSISWrapper` utility to generate the SSIS packages.
4. **Validate Packages**: Validate the generated packages using the `DTSXValidator.ps1` script.

By following these steps, you can effectively migrate from Informatica Big Data Cloud to SSIS, leveraging the tools and scripts provided in this project.

## Overview

This project contains PowerShell scripts to assist with the conversion of PowerMart files to SSIS packages and the validation of these packages. The main components are:

- `InformaticaX_SSIS.ps1`: Converts PowerMart XML files to SSIS DTSX packages.
- `DTSXValidator.ps1`: Validates the generated DTSX packages using `dtexec`.
- `SSISWrapper`: A C# executable used within the `InformaticaX_SSIS.ps1` script to facilitate the conversion of PowerMart XML files to SSIS DTSX packages.

## SSISWrapper

```bash
C:\dev\PS_InformaticaX_SSIS\SSISWrapper>dotnet run
  Usage:
  --add-connection --type <connectionType> --name <connectionName> --connection-string <connectionString> --output <outputPath> [--save-location <saveLocation>] [--server-name <serverName>] [--package-name <packageName>]
  Supported connection types:
          **OLEDB**
          **ODBC**
          **ADO**
          **ADO.NET:SQL**
          **ADO.NET:OLEDB**
          **FLATFILE**
          **MULTIFLATFILE**
          **MULTIFILE**
          **SQLMOBILE**
          **MSOLAP100**
          **FTP**
          **HTTP**
          **MSMQ**
          **SMTP**
          **WMI**
  --add-task --task <taskType> --container <containerType> --output <outputPath> [--save-location <saveLocation>] [--server-name <serverName>] [--package-name <packageName>]
  Supported task types:
          **ActiveXScriptTask**
          **BulkInsertTask**
          **ExecuteProcessTask**
          **ExecutePackageTask**
          **Exec80PackageTask**
          **FileSystemTask**
          **FTPTask**
          **MSMQTask**
          **PipelineTask**
          **ScriptTask**
          **SendMailTask**
          **SQLTask**
          **TransferStoredProceduresTask**
          **TransferLoginsTask**
          **TransferErrorMessagesTask**
          **TransferJobsTask**
          **TransferObjectsTask**
          **TransferDatabaseTask**
          **WebServiceTask**
          **WmiDataReaderTask**
          **WmiEventWatcherTask**
          **XMLTask**
  --add-precedence-constraint --from-task <fromTaskName> --to-task <toTaskName> --constraint-type <constraintType> --expression <expression> --output <outputPath> [--save-location <saveLocation>] [--server-name <serverName>] [--package-name <packageName>]
  Supported precedence constraint types:
          **Success**
          **Failure**
          **Completion**
          **Expression**
          **ExpressionAndSuccess**
          **ExpressionAndFailure**
          **ExpressionAndCompletion**
          **ExpressionOrSuccess**
          **ExpressionOrFailure**
          **ExpressionOrCompletion**
  Examples of expressions:
          1. **Simple Arithmetic**: @[User::Variable1] + @[User::Variable2]
          2. **String Concatenation**: "Hello, " + @[User::Name]
          3. **Conditional**: @[User::Age] > 18 ? "Adult" : "Minor"
          4. **Date Functions**: DATEPART("Year", GETDATE())
          5. **Logical AND/OR**: @[User::IsActive] == TRUE && @[User::IsVerified] == TRUE
  --add-variable --name <variableName> --value <variableValue> --output <outputPath> [--save-location <saveLocation>] [--server-name <sRefine commandline for feature-completeness to SSIS DTSX File Building
  erverName>] [--package-name <packageName>]
  --enable-logging --provider <logProviderType> --log-file <logFilePath> --output <outputPath> [--save-location <saveLocation>] [--server-name <serverName>] [--package-name <packageName>]
  Supported log provider types:
          **DTS.LogProviderTextFile.2**
          **DTS.LogProviderSQLServer.2**
          **DTS.LogProviderEventLog.2**
          **DTS.LogProviderXmlFile.2**
  --add-data-flow-task --output <outputPath> [--save-location <saveLocation>] [--server-name <serverName>] [--package-name <packageName>]
  --list-data-flow-components
  Note: Data flow components need to be discovered before being added.
  --add-data-flow-component --component <componentName> --connection <connectionName> --access-mode <accessMode> --sql-command <sqlCommand> --output <outputPath> [--save-location <saveLocation>] [--server-name <serverName>] [--package-name <packageName>]
  AccessMode Enum Values:
          **0 - OpenRowset**: The component will use a table or view.
          **1 - OpenRowsetVariable**: The component will use a table or view name from a variable.
          **2 - SqlCommand**: The component will use an SQL command.
          **3 - SqlCommandVariable**: The component will use an SQL command from a variable.
  --connect-data-flow-components --source <sourceComponentName> --destination <destinationComponentName> --output <outputPath> [--save-location <saveLocation>] [--server-name <serverName>] [--package-name <packageName>]
  --select-input-columns --component <componentName> --output <outputPath> [--save-location <saveLocation>] [--server-name <serverName>] [--package-name <packageName>]
```
