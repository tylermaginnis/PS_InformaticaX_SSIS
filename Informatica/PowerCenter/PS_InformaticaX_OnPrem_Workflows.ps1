# Set environment variables
$env:INFA_HOME = "/path/to/informatica"
$env:INFA_DOMAINS_FILE = "$env:INFA_HOME/domains.infa"

# Repository connection details
$REPO_NAME = "your_repo_name"
$DOMAIN_NAME = "your_domain_name"
$USER_NAME = "your_username"
$PASSWORD = "your_password"
$FOLDER_NAME = "your_folder_name"

# Connect to the repository
& pmrep connect -r $REPO_NAME -d $DOMAIN_NAME -n $USER_NAME -x $PASSWORD

# List all workflows in the specified folder
$workflows = & pmrep listobjects -o workflow -f $FOLDER_NAME

# Get details for each workflow
foreach ($workflow in $workflows) {
    $workflowName = $workflow.Name
    $workflowDetails = & pmrep objectexport -o workflow -n $workflowName -f $FOLDER_NAME
    $workflowDetails | ConvertTo-Json -Depth 10
}