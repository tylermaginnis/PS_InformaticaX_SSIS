# Define the server URL and session ID
$serverUrl = "https://your-informatica-server.com"
$icSessionId = "your-session-id"

# Define the API endpoint for workflows
$apiEndpoint = "$serverUrl/saas/api/v2/user/workflows"

# Create the headers
$headers = @{
    "Accept" = "application/json"
    "icSessionId" = $icSessionId
}

# Make the GET request
$response = Invoke-RestMethod -Uri $apiEndpoint -Method Get -Headers $headers

# Check if the response is successful
if ($response) {
    # Iterate through each workflow and get details
    foreach ($workflow in $response.workflows) {
        $workflowId = $workflow.id
        $workflowDetailsEndpoint = "$serverUrl/api/v2/workflow/$workflowId"
        
        # Make the GET request for workflow details
        $workflowDetailsResponse = Invoke-RestMethod -Uri $workflowDetailsEndpoint -Method Get -Headers $headers
        
        # Check if the response is successful
        if ($workflowDetailsResponse) {
            # Output the response
            $workflowDetailsResponse | ConvertTo-Json -Depth 10
        } else {
            Write-Host "Failed to retrieve details for workflow ID: $workflowId"
        }
    }
} else {
    Write-Host "Failed to retrieve workflows."
}