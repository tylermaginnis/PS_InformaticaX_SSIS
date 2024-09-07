using System;
using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Dts.Pipeline;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;

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
                    case "--add-connection":
                        HandleAddConnectionCommand(args);
                        break;
                    case "--add-task":
                        HandleAddTaskCommand(args);
                        break;
                    case "--add-precedence-constraint":
                        HandleAddPrecedenceConstraintCommand(args);
                        break;
                    case "--add-variable":
                        HandleAddVariableCommand(args);
                        break;
                    case "--enable-logging":
                        HandleEnableLoggingCommand(args);
                        break;
                    case "--add-data-flow-task":
                        HandleAddDataFlowTaskCommand(args);
                        break;
                    case "--list-data-flow-components":
                        ListDataFlowComponents();
                        break;
                    case "--add-data-flow-component":
                        HandleAddDataFlowComponentCommand(args);
                        break;
                    case "--connect-data-flow-components":
                        HandleConnectDataFlowComponentsCommand(args);
                        break;
                    case "--select-input-columns":
                        HandleSelectInputColumnsCommand(args);
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

        static void HandleAddConnectionCommand(string[] args)
        {
            string connectionType = null;
            string connectionName = null;
            string connectionString = null;
            string outputPath = null;
            string saveLocation = "File";
            string serverName = null;
            string packageName = null;

            for (int i = 1; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "--type":
                        if (i + 1 < args.Length)
                        {
                            connectionType = args[++i];
                        }
                        break;
                    case "--name":
                        if (i + 1 < args.Length)
                        {
                            connectionName = args[++i];
                        }
                        break;
                    case "--connection-string":
                        if (i + 1 < args.Length)
                        {
                            connectionString = args[++i];
                        }
                        break;
                    case "--output":
                        if (i + 1 < args.Length)
                        {
                            outputPath = args[++i];
                        }
                        break;
                    case "--save-location":
                        if (i + 1 < args.Length)
                        {
                            saveLocation = args[++i];
                        }
                        break;
                    case "--server-name":
                        if (i + 1 < args.Length)
                        {
                            serverName = args[++i];
                        }
                        break;
                    case "--package-name":
                        if (i + 1 < args.Length)
                        {
                            packageName = args[++i];
                        }
                        break;
                }
            }

            if (!string.IsNullOrEmpty(connectionType) && !string.IsNullOrEmpty(connectionName) && !string.IsNullOrEmpty(connectionString) && !string.IsNullOrEmpty(outputPath))
            {
                AddConnection(connectionType, connectionName, connectionString, outputPath, saveLocation, serverName, packageName);
            }
            else
            {
                Console.WriteLine("Missing required arguments for adding a connection.");
                PrintUsage();
            }
        }

        static void AddConnection(string connectionType, string connectionName, string connectionString, string outputPath, string saveLocation, string serverName, string packageName)
        {
            Package package = new Package();
            ConnectionManager connectionManager = package.Connections.Add(connectionType);
            connectionManager.Name = connectionName;
            connectionManager.ConnectionString = connectionString;

            Console.WriteLine($"Connection of type '{connectionType}' with name '{connectionName}' added successfully.");

            SavePackage(package, outputPath, saveLocation, serverName, packageName);
        }

        static void SavePackage(Package package, string outputPath, string saveLocation = "File", string serverName = null, string packageName = null)
        {
            Application app = new Application();
            try
            {
                switch (saveLocation.ToLower())
                {
                    case "file":
                        app.SaveToXml(outputPath, package, null);
                        Console.WriteLine($"Package saved successfully to file '{outputPath}'.");
                        break;
                    case "dtsserver":
                        if (string.IsNullOrEmpty(serverName) || string.IsNullOrEmpty(packageName))
                        {
                            throw new ArgumentException("Server name and package name must be provided for DTS server save location.");
                        }
                        app.SaveToDtsServer(package, null, serverName, packageName);
                        Console.WriteLine($"Package saved successfully to DTS server '{serverName}' with package name '{packageName}'.");
                        break;
                    case "sqlserver":
                        if (string.IsNullOrEmpty(serverName) || string.IsNullOrEmpty(packageName))
                        {
                            throw new ArgumentException("Server name and package name must be provided for SQL server save location.");
                        }
                        app.SaveToSqlServer(package, null, serverName, packageName, null);
                        Console.WriteLine($"Package saved successfully to SQL server '{serverName}' with package name '{packageName}'.");
                        break;
                    default:
                        throw new ArgumentException("Invalid save location specified.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to save package: {ex.Message}");
            }
        }

        static void HandleAddTaskCommand(string[] args)
        {
            string taskType = null;
            string containerType = "Package";
            string outputPath = null;
            string saveLocation = "File";
            string serverName = null;
            string packageName = null;

            for (int i = 1; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "--task":
                        if (i + 1 < args.Length)
                        {
                            taskType = args[++i];
                        }
                        break;
                    case "--container":
                        if (i + 1 < args.Length)
                        {
                            containerType = args[++i];
                        }
                        break;
                    case "--output":
                        if (i + 1 < args.Length)
                        {
                            outputPath = args[++i];
                        }
                        break;
                    case "--save-location":
                        if (i + 1 < args.Length)
                        {
                            saveLocation = args[++i];
                        }
                        break;
                    case "--server-name":
                        if (i + 1 < args.Length)
                        {
                            serverName = args[++i];
                        }
                        break;
                    case "--package-name":
                        if (i + 1 < args.Length)
                        {
                            packageName = args[++i];
                        }
                        break;
                }
            }

            if (!string.IsNullOrEmpty(taskType) && !string.IsNullOrEmpty(outputPath))
            {
                AddTask(taskType, containerType, outputPath, saveLocation, serverName, packageName);
            }
            else
            {
                Console.WriteLine("Missing required arguments for adding a task.");
                PrintUsage();
            }
        }

        static void AddTask(string taskType, string containerType, string outputPath, string saveLocation, string serverName, string packageName)
        {
            Application app = new Application();
            Package package = app.LoadPackage(outputPath, null);

            Executable exec = null;
            switch (containerType.ToLower())
            {
                case "package":
                    exec = package.Executables.Add(taskType);
                    break;
                case "sequence":
                    Sequence sequence = (Sequence)package.Executables.Add("STOCK:Sequence");
                    exec = sequence.Executables.Add(taskType);
                    break;
                case "forloop":
                    ForLoop forLoop = (ForLoop)package.Executables.Add("STOCK:ForLoop");
                    exec = forLoop.Executables.Add(taskType);
                    break;
                case "foreachloop":
                    ForEachLoop forEachLoop = (ForEachLoop)package.Executables.Add("STOCK:ForEachLoop");
                    exec = forEachLoop.Executables.Add(taskType);
                    break;
                case "dtseventhandler":
                    DtsEventHandler eventHandler = (DtsEventHandler)package.Executables.Add("STOCK:DtsEventHandler");
                    exec = eventHandler.Executables.Add(taskType);
                    break;
                default:
                    Console.WriteLine("Invalid container type specified.");
                    return;
            }

            if (exec != null)
            {
                Console.WriteLine($"Task of type '{taskType}' added successfully to '{containerType}'.");

                SavePackage(package, outputPath, saveLocation, serverName, packageName);
            }
        }

        static void HandleAddPrecedenceConstraintCommand(string[] args)
        {
            string outputPath = null;
            string fromTaskName = null;
            string toTaskName = null;
            string constraintType = "Success"; // Default to Success
            string expression = null;
            string saveLocation = "File";
            string serverName = null;
            string packageName = null;

            for (int i = 1; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "--output":
                        if (i + 1 < args.Length)
                        {
                            outputPath = args[++i];
                        }
                        break;
                    case "--from-task":
                        if (i + 1 < args.Length)
                        {
                            fromTaskName = args[++i];
                        }
                        break;
                    case "--to-task":
                        if (i + 1 < args.Length)
                        {
                            toTaskName = args[++i];
                        }
                        break;
                    case "--constraint-type":
                        if (i + 1 < args.Length)
                        {
                            constraintType = args[++i];
                        }
                        break;
                    case "--expression":
                        if (i + 1 < args.Length)
                        {
                            expression = args[++i];
                        }
                        break;
                    case "--save-location":
                        if (i + 1 < args.Length)
                        {
                            saveLocation = args[++i];
                        }
                        break;
                    case "--server-name":
                        if (i + 1 < args.Length)
                        {
                            serverName = args[++i];
                        }
                        break;
                    case "--package-name":
                        if (i + 1 < args.Length)
                        {
                            packageName = args[++i];
                        }
                        break;
                }
            }

            if (!string.IsNullOrEmpty(outputPath) && !string.IsNullOrEmpty(fromTaskName) && !string.IsNullOrEmpty(toTaskName))
            {
                AddPrecedenceConstraint(outputPath, fromTaskName, toTaskName, constraintType, expression, saveLocation, serverName, packageName);
            }
            else
            {
                Console.WriteLine("Missing required arguments for adding a precedence constraint.");
                PrintUsage();
            }
        }

        static void AddPrecedenceConstraint(string outputPath, string fromTaskName, string toTaskName, string constraintType, string expression, string saveLocation, string serverName, string packageName)
        {
            Application app = new Application();
            Package package = app.LoadPackage(outputPath, null);

            Executable fromExec = package.Executables[fromTaskName];
            Executable toExec = package.Executables[toTaskName];

            if (fromExec != null && toExec != null)
            {
                TaskHost fromTaskHost = fromExec as TaskHost;
                TaskHost toTaskHost = toExec as TaskHost;

                if (fromTaskHost != null && toTaskHost != null)
                {
                    PrecedenceConstraint pc = package.PrecedenceConstraints.Add((DtsContainer)fromTaskHost, (DtsContainer)toTaskHost);

                    switch (constraintType.ToLower())
                    {
                        case "success":
                            pc.Value = DTSExecResult.Success;
                            break;
                        case "failure":
                            pc.Value = DTSExecResult.Failure;
                            break;
                        case "completion":
                            pc.Value = DTSExecResult.Completion;
                            break;
                        case "expression":
                            pc.EvalOp = DTSPrecedenceEvalOp.Expression;
                            pc.Expression = expression ?? "TRUE"; // Default to TRUE if no expression provided
                            break;
                        case "expressionandsuccess":
                            pc.EvalOp = DTSPrecedenceEvalOp.ExpressionAndConstraint;
                            pc.Expression = expression ?? "TRUE"; // Default to TRUE if no expression provided
                            pc.LogicalAnd = true;
                            pc.Value = DTSExecResult.Success;
                            break;
                        case "expressionandfailure":
                            pc.EvalOp = DTSPrecedenceEvalOp.ExpressionAndConstraint;
                            pc.Expression = expression ?? "TRUE"; // Default to TRUE if no expression provided
                            pc.LogicalAnd = true;
                            pc.Value = DTSExecResult.Failure;
                            break;
                        case "expressionandcompletion":
                            pc.EvalOp = DTSPrecedenceEvalOp.ExpressionAndConstraint;
                            pc.Expression = expression ?? "TRUE"; // Default to TRUE if no expression provided
                            pc.LogicalAnd = true;
                            pc.Value = DTSExecResult.Completion;
                            break;
                        case "expressionorsuccess":
                            pc.EvalOp = DTSPrecedenceEvalOp.ExpressionOrConstraint;
                            pc.Expression = expression ?? "TRUE"; // Default to TRUE if no expression provided
                            pc.LogicalAnd = false;
                            pc.Value = DTSExecResult.Success;
                            break;
                        case "expressionorfailure":
                            pc.EvalOp = DTSPrecedenceEvalOp.ExpressionOrConstraint;
                            pc.Expression = expression ?? "TRUE"; // Default to TRUE if no expression provided
                            pc.LogicalAnd = false;
                            pc.Value = DTSExecResult.Failure;
                            break;
                        case "expressionorcompletion":
                            pc.EvalOp = DTSPrecedenceEvalOp.ExpressionOrConstraint;
                            pc.Expression = expression ?? "TRUE"; // Default to TRUE if no expression provided
                            pc.LogicalAnd = false;
                            pc.Value = DTSExecResult.Completion;
                            break;
                        default:
                            Console.WriteLine("Invalid constraint type specified.");
                            return;
                    }

                    Console.WriteLine($"Precedence constraint added successfully from '{fromTaskName}' to '{toTaskName}' with type '{constraintType}'.");

                    SavePackage(package, outputPath, saveLocation, serverName, packageName);
                }
                else
                {
                    Console.WriteLine("Invalid task names specified.");
                }
            }
            else
            {
                Console.WriteLine("Invalid task names specified.");
            }
        }

        static void HandleAddVariableCommand(string[] args)
        {
            string variableName = null;
            string variableValue = null;
            string outputPath = null;
            string saveLocation = "File";
            string serverName = null;
            string packageName = null;

            for (int i = 1; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "--name":
                        if (i + 1 < args.Length)
                        {
                            variableName = args[++i];
                        }
                        break;
                    case "--value":
                        if (i + 1 < args.Length)
                        {
                            variableValue = args[++i];
                        }
                        break;
                    case "--output":
                        if (i + 1 < args.Length)
                        {
                            outputPath = args[++i];
                        }
                        break;
                    case "--save-location":
                        if (i + 1 < args.Length)
                        {
                            saveLocation = args[++i];
                        }
                        break;
                    case "--server-name":
                        if (i + 1 < args.Length)
                        {
                            serverName = args[++i];
                        }
                        break;
                    case "--package-name":
                        if (i + 1 < args.Length)
                        {
                            packageName = args[++i];
                        }
                        break;
                }
            }

            if (!string.IsNullOrEmpty(variableName) && !string.IsNullOrEmpty(variableValue) && !string.IsNullOrEmpty(outputPath))
            {
                AddVariable(variableName, variableValue, outputPath, saveLocation, serverName, packageName);
            }
            else
            {
                Console.WriteLine("Missing required arguments for adding a variable.");
                PrintUsage();
            }
        }

        static void AddVariable(string variableName, string variableValue, string outputPath, string saveLocation, string serverName, string packageName)
        {
            Application app = new Application();
            Package package = app.LoadPackage(outputPath, null);

            Variable myVar = package.Variables.Add(variableName, false, "User", variableValue);

            Console.WriteLine($"Variable '{variableName}' with value '{variableValue}' added successfully.");

            SavePackage(package, outputPath, saveLocation, serverName, packageName);
        }

        static void HandleEnableLoggingCommand(string[] args)
        {
            string logProviderType = null;
            string logFilePath = null;
            string outputPath = null;
            string saveLocation = "File";
            string serverName = null;
            string packageName = null;

            for (int i = 1; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "--provider":
                        if (i + 1 < args.Length)
                        {
                            logProviderType = args[++i];
                        }
                        break;
                    case "--log-file":
                        if (i + 1 < args.Length)
                        {
                            logFilePath = args[++i];
                        }
                        break;
                    case "--output":
                        if (i + 1 < args.Length)
                        {
                            outputPath = args[++i];
                        }
                        break;
                    case "--save-location":
                        if (i + 1 < args.Length)
                        {
                            saveLocation = args[++i];
                        }
                        break;
                    case "--server-name":
                        if (i + 1 < args.Length)
                        {
                            serverName = args[++i];
                        }
                        break;
                    case "--package-name":
                        if (i + 1 < args.Length)
                        {
                            packageName = args[++i];
                        }
                        break;
                }
            }

            if (!string.IsNullOrEmpty(logProviderType) && !string.IsNullOrEmpty(logFilePath) && !string.IsNullOrEmpty(outputPath))
            {
                EnableLogging(logProviderType, logFilePath, outputPath, saveLocation, serverName, packageName);
            }
            else
            {
                Console.WriteLine("Missing required arguments for enabling logging.");
                PrintUsage();
            }
        }

        static void EnableLogging(string logProviderType, string logFilePath, string outputPath, string saveLocation, string serverName, string packageName)
        {
            Application app = new Application();
            Package package = app.LoadPackage(outputPath, null);

            ConnectionManager loggingConnection = package.Connections.Add("FILE");
            loggingConnection.ConnectionString = logFilePath;

            LogProvider provider = package.LogProviders.Add(logProviderType);
            provider.ConfigString = loggingConnection.Name;
            package.LoggingOptions.SelectedLogProviders.Add(provider);
            package.LoggingOptions.EventFilterKind = DTSEventFilterKind.Inclusion;
            package.LoggingOptions.EventFilter = new string[] { "OnPreExecute", "OnPostExecute", "OnError", "OnWarning", "OnInformation" };
            package.LoggingMode = DTSLoggingMode.Enabled;

            Console.WriteLine($"Logging enabled with provider '{logProviderType}' and log file '{logFilePath}'.");

            SavePackage(package, outputPath, saveLocation, serverName, packageName);
        }

        static void HandleAddDataFlowTaskCommand(string[] args)
        {
            string outputPath = null;
            string saveLocation = "File";
            string serverName = null;
            string packageName = null;

            for (int i = 1; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "--output":
                        if (i + 1 < args.Length)
                        {
                            outputPath = args[++i];
                        }
                        break;
                    case "--save-location":
                        if (i + 1 < args.Length)
                        {
                            saveLocation = args[++i];
                        }
                        break;
                    case "--server-name":
                        if (i + 1 < args.Length)
                        {
                            serverName = args[++i];
                        }
                        break;
                    case "--package-name":
                        if (i + 1 < args.Length)
                        {
                            packageName = args[++i];
                        }
                        break;
                }
            }

            if (!string.IsNullOrEmpty(outputPath))
            {
                AddDataFlowTask(outputPath, saveLocation, serverName, packageName);
            }
            else
            {
                Console.WriteLine("Missing required arguments for adding a data flow task.");
                PrintUsage();
            }
        }

        static void AddDataFlowTask(string outputPath, string saveLocation, string serverName, string packageName)
        {
            Application app = new Application();
            Package package = app.LoadPackage(outputPath, null);

            Executable e = package.Executables.Add("STOCK:PipelineTask");
            TaskHost thMainPipe = e as TaskHost;
            MainPipe dataFlowTask = thMainPipe.InnerObject as MainPipe;

            Console.WriteLine("Data Flow task added successfully.");

            SavePackage(package, outputPath, saveLocation, serverName, packageName);
        }

        static void ListDataFlowComponents()
        {
            Application application = new Application();
            PipelineComponentInfos componentInfos = application.PipelineComponentInfos;

            foreach (PipelineComponentInfo componentInfo in componentInfos)
            {
                Console.WriteLine("Name: " + componentInfo.Name + "\n" +
                                  "CreationName: " + componentInfo.CreationName + "\n");
            }
        }

        static void HandleAddDataFlowComponentCommand(string[] args)
        {
            string outputPath = null;
            string componentName = null;
            string connectionName = null;
            string accessMode = null;
            string sqlCommand = null;
            string saveLocation = "File";
            string serverName = null;
            string packageName = null;

            for (int i = 1; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "--output":
                        if (i + 1 < args.Length)
                        {
                            outputPath = args[++i];
                        }
                        break;
                    case "--component":
                        if (i + 1 < args.Length)
                        {
                            componentName = args[++i];
                        }
                        break;
                    case "--connection":
                        if (i + 1 < args.Length)
                        {
                            connectionName = args[++i];
                        }
                        break;
                    case "--access-mode":
                        if (i + 1 < args.Length)
                        {
                            accessMode = args[++i];
                        }
                        break;
                    case "--sql-command":
                        if (i + 1 < args.Length)
                        {
                            sqlCommand = args[++i];
                        }
                        break;
                    case "--save-location":
                        if (i + 1 < args.Length)
                        {
                            saveLocation = args[++i];
                        }
                        break;
                    case "--server-name":
                        if (i + 1 < args.Length)
                        {
                            serverName = args[++i];
                        }
                        break;
                    case "--package-name":
                        if (i + 1 < args.Length)
                        {
                            packageName = args[++i];
                        }
                        break;
                }
            }

            if (!string.IsNullOrEmpty(outputPath) && !string.IsNullOrEmpty(componentName) && !string.IsNullOrEmpty(connectionName) && !string.IsNullOrEmpty(accessMode) && !string.IsNullOrEmpty(sqlCommand))
            {
                AddDataFlowComponent(outputPath, componentName, connectionName, accessMode, sqlCommand, saveLocation, serverName, packageName);
            }
            else
            {
                Console.WriteLine("Missing required arguments for adding a data flow component.");
                PrintUsage();
            }
        }

        static void AddDataFlowComponent(string outputPath, string componentName, string connectionName, string accessMode, string sqlCommand, string saveLocation, string serverName, string packageName)
        {
            Application app = new Application();
            Package package = app.LoadPackage(outputPath, null);

            Executable e = package.Executables.Add("STOCK:PipelineTask");
            TaskHost thMainPipe = e as TaskHost;
            MainPipe dataFlowTask = thMainPipe.InnerObject as MainPipe;

            // Add the specified component to the data flow
            IDTSComponentMetaData100 component = dataFlowTask.ComponentMetaDataCollection.New();
            component.Name = componentName;
            component.ComponentClassID = app.PipelineComponentInfos[componentName].CreationName;

            // Get the design-time instance of the component
            CManagedComponentWrapper instance = component.Instantiate();
            instance.ProvideComponentProperties();

            // Assign the specified connection manager
            ConnectionManager cm = package.Connections[connectionName];
            if (component.RuntimeConnectionCollection.Count > 0)
            {
                component.RuntimeConnectionCollection[0].ConnectionManager = DtsConvert.GetExtendedInterface(cm);
                component.RuntimeConnectionCollection[0].ConnectionManagerID = cm.ID;
            }

            // Set custom properties
            instance.SetComponentProperty("AccessMode", int.Parse(accessMode));
            instance.SetComponentProperty("SqlCommand", sqlCommand);

            // Reinitialize the metadata
            instance.AcquireConnections(null);
            instance.ReinitializeMetaData();
            instance.ReleaseConnections();

            Console.WriteLine($"Data Flow component '{componentName}' added successfully.");

            SavePackage(package, outputPath, saveLocation, serverName, packageName);
        }

        static void HandleConnectDataFlowComponentsCommand(string[] args)
        {
            string outputPath = null;
            string sourceComponentName = null;
            string destinationComponentName = null;
            string saveLocation = "File";
            string serverName = null;
            string packageName = null;

            for (int i = 1; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "--output":
                        if (i + 1 < args.Length)
                        {
                            outputPath = args[++i];
                        }
                        break;
                    case "--source":
                        if (i + 1 < args.Length)
                        {
                            sourceComponentName = args[++i];
                        }
                        break;
                    case "--destination":
                        if (i + 1 < args.Length)
                        {
                            destinationComponentName = args[++i];
                        }
                        break;
                    case "--save-location":
                        if (i + 1 < args.Length)
                        {
                            saveLocation = args[++i];
                        }
                        break;
                    case "--server-name":
                        if (i + 1 < args.Length)
                        {
                            serverName = args[++i];
                        }
                        break;
                    case "--package-name":
                        if (i + 1 < args.Length)
                        {
                            packageName = args[++i];
                        }
                        break;
                }
            }

            if (!string.IsNullOrEmpty(outputPath) && !string.IsNullOrEmpty(sourceComponentName) && !string.IsNullOrEmpty(destinationComponentName))
            {
                ConnectDataFlowComponents(outputPath, sourceComponentName, destinationComponentName, saveLocation, serverName, packageName);
            }
            else
            {
                Console.WriteLine("Missing required arguments for connecting data flow components.");
                PrintUsage();
            }
        }

        static void ConnectDataFlowComponents(string outputPath, string sourceComponentName, string destinationComponentName, string saveLocation, string serverName, string packageName)
        {
            Application app = new Application();
            Package package = app.LoadPackage(outputPath, null);

            MainPipe dataFlowTask = null;
            foreach (Executable exec in package.Executables)
            {
                if (exec is TaskHost taskHost && taskHost.InnerObject is MainPipe)
                {
                    dataFlowTask = taskHost.InnerObject as MainPipe;
                    break;
                }
            }

            if (dataFlowTask == null)
            {
                Console.WriteLine("No Data Flow task found in the package.");
                return;
            }

            IDTSComponentMetaData100 sourceComponent = null;
            IDTSComponentMetaData100 destinationComponent = null;

            foreach (IDTSComponentMetaData100 component in dataFlowTask.ComponentMetaDataCollection)
            {
                if (component.Name == sourceComponentName)
                {
                    sourceComponent = component;
                }
                else if (component.Name == destinationComponentName)
                {
                    destinationComponent = component;
                }
            }

            if (sourceComponent == null || destinationComponent == null)
            {
                Console.WriteLine("Source or destination component not found.");
                return;
            }

            IDTSPath100 path = dataFlowTask.PathCollection.New();
            path.AttachPathAndPropagateNotifications(sourceComponent.OutputCollection[0], destinationComponent.InputCollection[0]);

            Console.WriteLine($"Connected '{sourceComponentName}' to '{destinationComponentName}' successfully.");

            SavePackage(package, outputPath, saveLocation, serverName, packageName);
        }

        static void HandleSelectInputColumnsCommand(string[] args)
        {
            string outputPath = null;
            string componentName = null;
            string saveLocation = "File";
            string serverName = null;
            string packageName = null;

            for (int i = 1; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "--output":
                        if (i + 1 < args.Length)
                        {
                            outputPath = args[++i];
                        }
                        break;
                    case "--component":
                        if (i + 1 < args.Length)
                        {
                            componentName = args[++i];
                        }
                        break;
                    case "--save-location":
                        if (i + 1 < args.Length)
                        {
                            saveLocation = args[++i];
                        }
                        break;
                    case "--server-name":
                        if (i + 1 < args.Length)
                        {
                            serverName = args[++i];
                        }
                        break;
                    case "--package-name":
                        if (i + 1 < args.Length)
                        {
                            packageName = args[++i];
                        }
                        break;
                }
            }

            if (!string.IsNullOrEmpty(outputPath) && !string.IsNullOrEmpty(componentName))
            {
                SelectInputColumns(outputPath, componentName, saveLocation, serverName, packageName);
            }
            else
            {
                Console.WriteLine("Missing required arguments for selecting input columns.");
                PrintUsage();
            }
        }

        static void SelectInputColumns(string outputPath, string componentName, string saveLocation, string serverName, string packageName)
        {
            Application app = new Application();
            Package package = app.LoadPackage(outputPath, null);

            MainPipe dataFlowTask = null;
            foreach (Executable exec in package.Executables)
            {
                if (exec is TaskHost taskHost && taskHost.InnerObject is MainPipe)
                {
                    dataFlowTask = taskHost.InnerObject as MainPipe;
                    break;
                }
            }

            if (dataFlowTask == null)
            {
                Console.WriteLine("No Data Flow task found in the package.");
                return;
            }

            IDTSComponentMetaData100 component = null;
            foreach (IDTSComponentMetaData100 comp in dataFlowTask.ComponentMetaDataCollection)
            {
                if (comp.Name == componentName)
                {
                    component = comp;
                    break;
                }
            }

            if (component == null)
            {
                Console.WriteLine($"Component '{componentName}' not found.");
                return;
            }

            CManagedComponentWrapper instance = component.Instantiate();
            IDTSInput100 input = component.InputCollection[0];
            IDTSVirtualInput100 vInput = input.GetVirtualInput();

            foreach (IDTSVirtualInputColumn100 vColumn in vInput.VirtualInputColumnCollection)
            {
                instance.SetUsageType(input.ID, vInput, vColumn.LineageID, DTSUsageType.UT_READONLY);
            }

            Console.WriteLine($"Input columns selected for component '{componentName}'.");

            SavePackage(package, outputPath, saveLocation, serverName, packageName);
        }

        static void PrintUsage()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Usage:");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("--add-connection --type <connectionType> --name <connectionName> --connection-string <connectionString> --output <outputPath> [--save-location <saveLocation>] [--server-name <serverName>] [--package-name <packageName>]");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Supported connection types:");
            Console.WriteLine("\t**OLEDB**");
            Console.WriteLine("\t**ODBC**");
            Console.WriteLine("\t**ADO**");
            Console.WriteLine("\t**ADO.NET:SQL**");
            Console.WriteLine("\t**ADO.NET:OLEDB**");
            Console.WriteLine("\t**FLATFILE**");
            Console.WriteLine("\t**MULTIFLATFILE**");
            Console.WriteLine("\t**MULTIFILE**");
            Console.WriteLine("\t**SQLMOBILE**");
            Console.WriteLine("\t**MSOLAP100**");
            Console.WriteLine("\t**FTP**");
            Console.WriteLine("\t**HTTP**");
            Console.WriteLine("\t**MSMQ**");
            Console.WriteLine("\t**SMTP**");
            Console.WriteLine("\t**WMI**");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("--add-task --task <taskType> --container <containerType> --output <outputPath> [--save-location <saveLocation>] [--server-name <serverName>] [--package-name <packageName>]");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Supported task types:");
            Console.WriteLine("\t**ActiveXScriptTask**");
            Console.WriteLine("\t**BulkInsertTask**");
            Console.WriteLine("\t**ExecuteProcessTask**");
            Console.WriteLine("\t**ExecutePackageTask**");
            Console.WriteLine("\t**Exec80PackageTask**");
            Console.WriteLine("\t**FileSystemTask**");
            Console.WriteLine("\t**FTPTask**");
            Console.WriteLine("\t**MSMQTask**");
            Console.WriteLine("\t**PipelineTask**");
            Console.WriteLine("\t**ScriptTask**");
            Console.WriteLine("\t**SendMailTask**");
            Console.WriteLine("\t**SQLTask**");
            Console.WriteLine("\t**TransferStoredProceduresTask**");
            Console.WriteLine("\t**TransferLoginsTask**");
            Console.WriteLine("\t**TransferErrorMessagesTask**");
            Console.WriteLine("\t**TransferJobsTask**");
            Console.WriteLine("\t**TransferObjectsTask**");
            Console.WriteLine("\t**TransferDatabaseTask**");
            Console.WriteLine("\t**WebServiceTask**");
            Console.WriteLine("\t**WmiDataReaderTask**");
            Console.WriteLine("\t**WmiEventWatcherTask**");
            Console.WriteLine("\t**XMLTask**");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("--add-precedence-constraint --from-task <fromTaskName> --to-task <toTaskName> --constraint-type <constraintType> --expression <expression> --output <outputPath> [--save-location <saveLocation>] [--server-name <serverName>] [--package-name <packageName>]");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Supported precedence constraint types:");
            Console.WriteLine("\t**Success**");
            Console.WriteLine("\t**Failure**");
            Console.WriteLine("\t**Completion**");
            Console.WriteLine("\t**Expression**");
            Console.WriteLine("\t**ExpressionAndSuccess**");
            Console.WriteLine("\t**ExpressionAndFailure**");
            Console.WriteLine("\t**ExpressionAndCompletion**");
            Console.WriteLine("\t**ExpressionOrSuccess**");
            Console.WriteLine("\t**ExpressionOrFailure**");
            Console.WriteLine("\t**ExpressionOrCompletion**");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Examples of expressions:");
            Console.WriteLine("\t1. **Simple Arithmetic**: @[User::Variable1] + @[User::Variable2]");
            Console.WriteLine("\t2. **String Concatenation**: \"Hello, \" + @[User::Name]");
            Console.WriteLine("\t3. **Conditional**: @[User::Age] > 18 ? \"Adult\" : \"Minor\"");
            Console.WriteLine("\t4. **Date Functions**: DATEPART(\"Year\", GETDATE())");
            Console.WriteLine("\t5. **Logical AND/OR**: @[User::IsActive] == TRUE && @[User::IsVerified] == TRUE");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("--add-variable --name <variableName> --value <variableValue> --output <outputPath> [--save-location <saveLocation>] [--server-name <serverName>] [--package-name <packageName>]");
            Console.WriteLine("--enable-logging --provider <logProviderType> --log-file <logFilePath> --output <outputPath> [--save-location <saveLocation>] [--server-name <serverName>] [--package-name <packageName>]");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Supported log provider types:");
            Console.WriteLine("\t**DTS.LogProviderTextFile.2**");
            Console.WriteLine("\t**DTS.LogProviderSQLServer.2**");
            Console.WriteLine("\t**DTS.LogProviderEventLog.2**");
            Console.WriteLine("\t**DTS.LogProviderXmlFile.2**");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("--add-data-flow-task --output <outputPath> [--save-location <saveLocation>] [--server-name <serverName>] [--package-name <packageName>]");
            Console.WriteLine("--list-data-flow-components");
            Console.WriteLine("Note: Data flow components need to be discovered before being added.");
            Console.WriteLine("--add-data-flow-component --component <componentName> --connection <connectionName> --access-mode <accessMode> --sql-command <sqlCommand> --output <outputPath> [--save-location <saveLocation>] [--server-name <serverName>] [--package-name <packageName>]");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("AccessMode Enum Values:");
            Console.WriteLine("\t**0 - OpenRowset**: The component will use a table or view.");
            Console.WriteLine("\t**1 - OpenRowsetVariable**: The component will use a table or view name from a variable.");
            Console.WriteLine("\t**2 - SqlCommand**: The component will use an SQL command.");
            Console.WriteLine("\t**3 - SqlCommandVariable**: The component will use an SQL command from a variable.");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("--connect-data-flow-components --source <sourceComponentName> --destination <destinationComponentName> --output <outputPath> [--save-location <saveLocation>] [--server-name <serverName>] [--package-name <packageName>]");
            Console.WriteLine("--select-input-columns --component <componentName> --output <outputPath> [--save-location <saveLocation>] [--server-name <serverName>] [--package-name <packageName>]");
            Console.ResetColor();
        }
    }
}