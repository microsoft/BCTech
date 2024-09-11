<#
.SYNOPSIS
This script deploys Azure Machine Learning resources for use with Dynamics 365 Business Central.

The script has to be executed in the context of an Azure subscription. It requires the Azure CLI to be installed and available in the PATH.

Furthermore, the current directory when executing should be this directory (MachineLearning).

After execution, go to the Azure ML workspace, locate the online endpoint to locate the endpoint URI and key to use in Business Central.

.DESCRIPTION
This script creates and manages Azure resources such as a resource group, a Machine Learning workspace, and a container registry.

.PARAMETER ResourceGroupName
The name of the Azure resource group where the resources will be deployed.

.PARAMETER Location
The location where the Azure resource group will be created.

.PARAMETER WorkspaceName
The name of the Azure Machine Learning workspace. Default is "bcmlworkspace".

.PARAMETER EndpointName
The name of the endpoint. Default is "bcmlendpoint".

.PARAMETER DeploymentName
The name of the deployment. Default is "bcmldeployment".

.PARAMETER RegistryName
The name of the registry. Default is "bcmlregistry".

.PARAMETER ImageName
The name of the image. Default is "businesscentralml".

.PARAMETER ImageTag
The tag of the image. Default is "latest".

.PARAMETER NumberOfDeployments
The number of deployments. Default is 1.
You can increase these if you experience performance issues.

.PARAMETER NumberOfConcurrentRequests
The number of concurrent requests. Default is 4.

.PARAMETER InstanceType
The type of the instance. Default is "Standard_DS2_v2".
You can use a different instance type if you experience performance issues, cf.
https://learn.microsoft.com/en-us/azure/machine-learning/reference-managed-online-endpoints-vm-sku-list?view=azureml-api-2

.PARAMETER RequestTimeoutMs
The request timeout in milliseconds. Requests to the Azure ML endpoint will time out if
they take longer than this value. Default is 90000.

.EXAMPLE
.\DeployMachineLearningResources.ps1 -ResourceGroupName "MyResourceGroup" -Location "West US"
This example deploys the Azure Machine Learning resources in the specified resource group and location.

#>
[cmdletbinding()]
Param
(
    [Parameter(Mandatory=$true)]
    [string] $ResourceGroupName,

    [Parameter(Mandatory=$true)]
    [string] $Location,

    [string] $WorkspaceName = "bcmlworkspace",

    [string] $EndpointName = "bcmlendpoint",

    [string] $DeploymentName = "bcmldeployment",

    [string] $RegistryName = "bcmlregistry",

    [string] $ImageName = "businesscentralml",

    [string] $ImageTag = "latest",

    [int] $NumberOfDeployments = 1,

    [int] $NumberOfConcurrentRequests = 8,

    [string] $InstanceType = "Standard_DS2_v2",

    [int] $RequestTimeoutMs = 90000
)
$ErrorActionPreference = "Stop"
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

function Invoke-AzCliCommand {
    param (
        [Parameter(Mandatory=$true)]
        [string] $Command
    )
    $Result = Invoke-Expression -Command $Command

    if ($LASTEXITCODE -ne 0) {
        Write-Error "The command '$Command' failed with exit code $LASTEXITCODE."
    }

    return $Result
}

$Context = Invoke-AzCliCommand { az account show } | ConvertFrom-Json
Write-Host ("Logged into subscription '{0}'" -f $Context.name)

Write-Host ("Deploying Azure Machine Learning resources in resource group '{0}'." -f $ResourceGroupName)

if (!(Invoke-AzCliCommand { az group exists --name $ResourceGroupName } | ConvertFrom-Json))
{
    Write-Host ("Creating resource group '{0}' in location '{1}'." -f $ResourceGroupName, $Location)
    Invoke-AzCliCommand { az group create --name $ResourceGroupName --location $Location } | Out-Null
}
else
{
    Write-Host ("Resource group '{0}' already exists." -f $ResourceGroupName)
}

if (!(Invoke-AzCliCommand { az ml workspace list --resource-group $ResourceGroupName --query "[?name == '$WorkspaceName']" } | ConvertFrom-Json))
{
    Write-Host ("Creating Azure Machine Learning workspace '{0}' in resource group '{1}'." -f $WorkspaceName, $ResourceGroupName)
    Invoke-AzCliCommand { az ml workspace create --resource-group $ResourceGroupName --name $WorkspaceName --system-datastores-auth-mode identity} | Out-Null
}
else
{
    Write-Host ("Azure Machine Learning workspace '{0}' already exists." -f $WorkspaceName)
}
$Workspace = Invoke-AzCliCommand { az ml workspace show --resource-group $ResourceGroupName --name $WorkspaceName } | ConvertFrom-Json

if (!(Invoke-AzCliCommand { az acr list --query "[?name == '$RegistryName']" } | ConvertFrom-Json))
{
    Write-Host ("Creating Azure Container Registry '{0}' in resource group '{1}'." -f $RegistryName, $ResourceGroupName)
    Invoke-AzCliCommand { az acr create --resource-group $ResourceGroupName --name $RegistryName --sku Standard } | Out-Null
}
else
{
    Write-Host ("Azure Container Registry '{0}' already exists." -f $RegistryName)
}
$Registry = Invoke-AzCliCommand { az acr show --name $RegistryName } | ConvertFrom-Json
$WorkspaceRegistryName = ($Workspace.container_registry -split "/")[-1]

if ($WorkspaceRegistryName -ne $Registry.name)
{
    Write-Host ("Updating Azure Machine Learning workspace '{0}' to use registry '{1}'." -f $WorkspaceName, $RegistryName)
    Invoke-AzCliCommand { az ml workspace update --resource-group $ResourceGroupName --name $WorkspaceName --container-registry $Registry.id --update-dependent-resources } | Out-Null
}
else 
{
    Write-Host ("Azure Machine Learning workspace '{0}' already uses registry '{1}'." -f $WorkspaceName, $RegistryName)
}

$ImageNameAndTag = ("{0}:{1}" -f $ImageName, $ImageTag)
# The following is not wrapped in Invoke-AzCliCommand, because it errors out if the image does not exist
if (!(az acr repository show --image $ImageNameAndTag --name $RegistryName))
{
    Write-Host ("Building container image {0} for Azure Machine Learning model." -f $ImageNameAndTag)
    # The following is not wrapped in Invoke-AzCliCommand because running it requires console output
    az acr build . --registry $RegistryName --image ("{0}:{1}" -f $ImageName, $ImageTag)
}
else
{
    Write-Host ("Container image {0} for Azure Machine Learning model already exists." -f $ImageNameAndTag)
}

if (!(Invoke-AzCliCommand { az ml online-endpoint list --workspace-name $WorkspaceName --resource-group $ResourceGroupName --query "[?name == '$EndpointName']" } | ConvertFrom-Json)) 
{
    Write-Host ("Creating Azure Machine Learning endpoint '{0}'." -f $EndpointName)
    Invoke-AzCliCommand { 
        az ml online-endpoint create `
            --name $EndpointName `
            --resource-group $ResourceGroupName `
            --workspace-name $WorkspaceName `
            --auth-mode Key 
    } | Out-Null
}
else
{
    Write-Host ("Azure Machine Learning endpoint '{0}' already exists." -f $EndpointName)
}

$OnlineDeploymentYamlFile = Join-Path $PSScriptRoot "OnlineDeployment.yaml"
$OnlineDeploymentYaml = @"
    `$schema: https://azuremlschemas.azureedge.net/latest/managedOnlineDeployment.schema.json
    name: $($DeploymentName)
    endpoint_name: $($EndpointName)
    code_configuration:
        code: ./R
        scoring_script: plumber.R
    environment:
        name: $DeploymentName
        version: $ImageTag
        image: $($RegistryName).azurecr.io/$($ImageName)
        inference_config:
            liveness_route:
                port: 8000
                path: /live
            readiness_route:
                port: 8000
                path: /ready
            scoring_route:
                port: 8000
                path: /score
    instance_type: $InstanceType
    instance_count: $NumberOfDeployments
    request_settings:
        max_concurrent_requests_per_instance: $NumberOfConcurrentRequests
        request_timeout_ms: $RequestTimeoutMs
"@
$OnlineDeploymentYaml | Out-File $OnlineDeploymentYamlFile -Force

$Deployment = Invoke-AzCliCommand { az ml online-deployment list --workspace-name $WorkspaceName --endpoint-name $EndpointName --resource-group $ResourceGroupName --query "[?name == '$DeploymentName']"} | ConvertFrom-Json
try {
    if (!$Deployment)
    {
        Write-Host ("Creating Azure Machine Learning deployment '{0}'." -f $DeploymentName)
        Invoke-AzCliCommand {
            az ml online-deployment create `
                --resource-group $ResourceGroupName `
                --workspace-name $WorkspaceName `
                --file $OnlineDeploymentYamlFile `
                --skip-script-validation `
                --all-traffic 
        } | Out-Null
    }
    else
    {
        Write-Host ("Azure Machine Learning deployment '{0}' already exists. Updating." -f $DeploymentName)

        if ($Deployment.provisioning_state -ne "completed") 
        {
            Write-Error ("Cannot update deployment '{0}' because it is in the '{1}' state." -f $DeploymentName, $Deployment.provisioning_state)
        }

        Invoke-AzCliCommand {
            az ml online-deployment update `
                --resource-group $ResourceGroupName `
                --workspace-name $WorkspaceName `
                --file $OnlineDeploymentYamlFile `
                --skip-script-validation
        } | Out-Null
    }
}
finally 
{
    Remove-Item $OnlineDeploymentYamlFile
}

Write-Host "Azure Machine Learning resources deployed successfully."
