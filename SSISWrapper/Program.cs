using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using DtsRuntimePackage = Microsoft.SqlServer.Dts.Runtime.Package;
using DtsWrapperPackage = Microsoft.SqlServer.Dts.Runtime.Wrapper.Package;
using DtsRuntimeTaskHost = Microsoft.SqlServer.Dts.Runtime.TaskHost;
using DtsWrapperTaskHost = Microsoft.SqlServer.Dts.Runtime.Wrapper.TaskHost;
using DtsRuntimeApplication = Microsoft.SqlServer.Dts.Runtime.Application;
using DtsWrapperApplication = Microsoft.SqlServer.Dts.Runtime.Wrapper.Application;
using System.Runtime.InteropServices;

namespace SSISWrapper
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                string command = args[0];
                switch (command)
                {
                    case "--create":
                        HandleCreateCommand(args);
                        break;
                    default:
                        PrintUsage();
                        break;
                }
            }
            else
            {
                PrintUsage();
            }
        }

        static void HandleCreateCommand(string[] args)
        {
            string packageName = null;
            string folderName = null;
            string folderDescription = null;
            string sourceName = null;
            string sourceConnectionString = null;
            string targetName = null;
            string targetConnectionString = null;
            string outputPath = "C:\\dev\\PS_InformaticaX_SSIS\\SSIS\\MyPackage.dtsx"; // Default output path
            Dictionary<string, string> columnMappings = new Dictionary<string, string>();

            for (int i = 1; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "--name":
                        if (i + 1 < args.Length)
                        {
                            packageName = args[++i];
                        }
                        break;
                    case "--folder":
                        if (i + 1 < args.Length)
                        {
                            folderName = args[++i];
                        }
                        break;
                    case "--folder-description":
                        if (i + 1 < args.Length)
                        {
                            folderDescription = args[++i];
                        }
                        break;
                    case "--source":
                        if (i + 1 < args.Length)
                        {
                            sourceName = args[++i];
                        }
                        break;
                    case "--source-connection":
                        if (i + 1 < args.Length)
                        {
                            sourceConnectionString = args[++i];
                        }
                        break;
                    case "--target":
                        if (i + 1 < args.Length)
                        {
                            targetName = args[++i];
                        }
                        break;
                    case "--target-connection":
                        if (i + 1 < args.Length)
                        {
                            targetConnectionString = args[++i];
                        }
                        break;
                    case "--output":
                        if (i + 1 < args.Length)
                        {
                            outputPath = args[++i];
                        }
                        break;
                    case "--mapping":
                        if (i + 1 < args.Length)
                        {
                            string[] mappings = args[++i].Split(',');
                            foreach (var mapping in mappings)
                            {
                                string[] columns = mapping.Split(':');
                                if (columns.Length == 2)
                                {
                                    columnMappings[columns[0]] = columns[1];
                                }
                            }
                        }
                        break;
                }
            }

            if (!string.IsNullOrEmpty(packageName))
            {
                CreatePackage(outputPath, packageName, folderName, folderDescription, sourceName, sourceConnectionString, targetName, targetConnectionString, columnMappings);
            }
        }

        static void PrintUsage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("--create --name <packageName> [--folder <folderName> --folder-description <folderDescription>] [--source <sourceName> --source-connection <connectionString>] [--target <targetName> --target-connection <connectionString>] [--output <outputPath>] [--mapping <sourceColumn1:targetColumn1,sourceColumn2:targetColumn2,...>]");
        }

        static void CreatePackage(string outputPath, string packageName, string folderName, string folderDescription, string sourceName, string sourceConnectionString, string targetName, string targetConnectionString, Dictionary<string, string> columnMappings)
        {
            DtsRuntimePackage package = new DtsRuntimePackage
            {
                Name = packageName,
                Description = "Sample package created by SSISWrapper"
            };

            // Create a Data Flow Task
            Executable exec = package.Executables.Add("STOCK:PipelineTask");
            DtsRuntimeTaskHost taskHost = exec as DtsRuntimeTaskHost;
            MainPipe dataFlowTask = taskHost.InnerObject as MainPipe;

            IDTSComponentMetaData100 sourceComponent = null;
            IDTSComponentMetaData100 targetComponent = null;

            if (!string.IsNullOrEmpty(sourceName) && !string.IsNullOrEmpty(sourceConnectionString))
            {
                sourceComponent = CreateSource(package, dataFlowTask, sourceName, sourceConnectionString);
            }

            if (!string.IsNullOrEmpty(targetName) && !string.IsNullOrEmpty(targetConnectionString))
            {
                targetComponent = CreateTarget(package, dataFlowTask, targetName, targetConnectionString);
            }

            if (sourceComponent != null && targetComponent != null)
            {
                MapColumns(dataFlowTask, sourceComponent, targetComponent, columnMappings);
            }

            try
            {
                // Ensure the directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
                DtsRuntimeApplication app = new DtsRuntimeApplication();
                app.SaveToXml(outputPath, package, null);
                Console.WriteLine($"Package created and saved to {outputPath} with name {packageName}.");
            }
            catch (DtsRuntimeException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
static IDTSComponentMetaData100 CreateSource(Package package, MainPipe dataFlowTask, string sourceName, string connectionString)
{
    Console.WriteLine($"Creating source with connection string: {connectionString}");

    IDTSComponentMetaData100 sourceComponent = null;
    try
    {
        sourceComponent = dataFlowTask.ComponentMetaDataCollection.New();
        sourceComponent.ComponentClassID = "DTSAdapter.FlatFileSource"; // Check this ID
        CManagedComponentWrapper instance = sourceComponent.Instantiate();
        instance.ProvideComponentProperties();

        sourceComponent.Name = sourceName;

        // Set up the connection manager
        ConnectionManager sourceConnection = package.Connections.Add("FLATFILE");
        sourceConnection.Name = sourceName;
        sourceConnection.ConnectionString = connectionString;

        sourceConnection.Properties["Format"].SetValue(sourceConnection, 0); // 0 for Delimited
        sourceConnection.Properties["ColumnNamesInFirstDataRow"].SetValue(sourceConnection, true);

        // Set the connection manager for the source component
        sourceComponent.RuntimeConnectionCollection[0].ConnectionManagerID = sourceConnection.ID;
        sourceComponent.RuntimeConnectionCollection[0].ConnectionManager = DtsConvert.GetExtendedInterface(sourceConnection);

        // Initialize the component
        instance.AcquireConnections(null);
        instance.ReinitializeMetaData();
        instance.ReleaseConnections();

        Console.WriteLine($"Source {sourceName} created with connection string: {connectionString}");
    }
    catch (COMException ex)
    {
        Console.WriteLine($"COMException while creating source: {ex.Message}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception while creating source: {ex.Message}");
    }

    return sourceComponent;
}

static IDTSComponentMetaData100 CreateTarget(Package package, MainPipe dataFlowTask, string targetName, string connectionString)
{
    Console.WriteLine($"Creating target with connection string: {connectionString}");

    IDTSComponentMetaData100 targetComponent = null;
    try
    {
        targetComponent = dataFlowTask.ComponentMetaDataCollection.New();
        targetComponent.ComponentClassID = "DTSAdapter.FlatFileDestination"; // Check this ID
        CManagedComponentWrapper instance = targetComponent.Instantiate();
        instance.ProvideComponentProperties();

        targetComponent.Name = targetName;

        // Set up the connection manager
        ConnectionManager targetConnection = package.Connections.Add("FLATFILE");
        targetConnection.Name = targetName;
        targetConnection.ConnectionString = connectionString;

        // Set the connection manager for the target component
        targetComponent.RuntimeConnectionCollection[0].ConnectionManagerID = targetConnection.ID;
        targetComponent.RuntimeConnectionCollection[0].ConnectionManager = DtsConvert.GetExtendedInterface(targetConnection);

        // Initialize the component
        instance.AcquireConnections(null);
        instance.ReinitializeMetaData();
        instance.ReleaseConnections();

        Console.WriteLine($"Target {targetName} created with connection string: {connectionString}");
    }
    catch (COMException ex)
    {
        Console.WriteLine($"COMException while creating target: {ex.Message}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception while creating target: {ex.Message}");
    }

    return targetComponent;
}


        static void MapColumns(MainPipe dataFlowTask, IDTSComponentMetaData100 sourceComponent, IDTSComponentMetaData100 targetComponent, Dictionary<string, string> columnMappings)
        {
            // Create a path between the source and target components
            IDTSPath100 path = dataFlowTask.PathCollection.New();
            path.AttachPathAndPropagateNotifications(sourceComponent.OutputCollection[0], targetComponent.InputCollection[0]);

            // Get the input from the target component
            IDTSInput100 input = targetComponent.InputCollection[0];
            IDTSVirtualInput100 vInput = input.GetVirtualInput();

            foreach (IDTSVirtualInputColumn100 vColumn in vInput.VirtualInputColumnCollection)
            {
                if (columnMappings.ContainsKey(vColumn.Name))
                {
                    // Add the input column to the target
                    IDTSVirtualInputColumn100 vInputColumn = vInput.VirtualInputColumnCollection[vColumn.LineageID];
                    IDTSInputColumn100 inputColumn = input.InputColumnCollection.New();
                    inputColumn.LineageID = vInputColumn.LineageID;
                    inputColumn.UsageType = DTSUsageType.UT_READONLY;
                    
                    // Rename the input column if needed
                    inputColumn.Name = columnMappings[vColumn.Name];
                }
            }
        }
    }
}