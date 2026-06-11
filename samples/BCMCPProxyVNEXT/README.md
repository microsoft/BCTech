# Business Central MCP Proxy

## ⚠️ Experimental Tool - Not for Production Use

This tool is for **experimentation only** and is **not intended for production use**. It allows you to connect Claude Desktop or VS Code to a Business Central Model Context Protocol (MCP) server.

## Overview

The BC MCP Proxy acts as a bridge between MCP-compatible clients (like Claude Desktop) and Business Central, enabling natural language interactions with your Business Central data and operations.

## Prerequisites

- Microsoft Dynamics 365 Business Central environment
- Azure tenant with appropriate permissions
- Claude Desktop application (for Claude integration)

## Setup Instructions

### 1. Set-up Azure AD App Registration

1. **Create the App Registration**
   - Open [portal.azure.com](https://portal.azure.com)
   - Navigate to **Azure Active Directory** → **App registrations**
   - Click **+ New registration**
   - Name: `BcMCPProxy` (or any name you prefer)
   - Supported account types: **Accounts in this organizational directory only (Single tenant)**
   - Redirect URI: Leave blank for now
   - Click **Register**
   - **Copy the Application (client) ID** - you'll need this for configuration

2. **Configure Authentication**
   - In the left menu, click **Authentication**
   - Click **+ Add a platform** → Select **Mobile and desktop applications**
   - Check this redirect URI:
     - ☑ `https://login.microsoftonline.com/common/oauth2/nativeclient`
   - Click **Configure**
   - Scroll down to **Advanced settings** → **Allow public client flows**
   - Set to: **Yes** (toggle should be enabled/blue)
   - Click **Save**

   **Note**: Device Code Flow is used for authentication on all platforms, requiring only the native client redirect URI.

   ![Authentication Setup](./docs/images/authentication-setup.png)

3. **Configure API Permissions**
   - In the left menu, click **API permissions**
   - Click **+ Add a permission**
   - Select **APIs my organization uses** tab
   - Search for: `Dynamics 365 Business Central`
   - Click on it, then select **Delegated permissions**
   - Check these permissions:
     - ☑ `Financials.ReadWrite.All`
     - ☑ `user_impersonation`
   - Click **Add permissions**
   - Click **Grant admin consent for [your tenant]** (if you have admin rights)

   ![API Permissions](./docs/images/api-permissions.png)

   Required permissions:
   - **Dynamics 365 Business Central**
     - `Financials.ReadWrite.All` (Delegated)
     - `user_impersonation` (Delegated)

### 2. Set-up Claude Desktop with Business Central MCP Server

1. **Download and Install Claude Desktop**
   - Download Claude for desktop from the official website
   - Install and sign in to your Claude account

2. **Configure Claude Desktop**
   - Open Claude Desktop
   - Navigate to **Settings** → **Developer**
   - Click **"Edit Config"** to create/edit `claude_desktop_config.json`

3. **Edit Configuration File**
   
   Add the BC MCP server configuration to your `claude_desktop_config.json`:

   **For Windows:**
   ```json
   {
     "mcpServers": {
       "BC_MCP": {
         "command": "C:\\Path\\To\\BcMCPProxy.exe",
         "args": [
           "--TenantId",
           "<Your-Tenant-ID>",
           "--ClientId", 
           "<Your-Client-ID>",
           "--Environment",
           "<BC-Environment-Name>",
           "--Company",
           "<Company-Name>",
           "--ConfigurationName",
           "<Configuration-Name>"
         ]
       }
     }
   }
   ```

   **For macOS:**
   ```json
   {
     "mcpServers": {
       "BC_MCP": {
         "command": "/usr/local/share/dotnet/dotnet",
         "args": [
           "/Users/<username>/path/to/BcMCPProxy.dll",
           "--TenantId",
           "<Your-Tenant-ID>",
           "--ClientId", 
           "<Your-Client-ID>",
           "--Environment",
           "<BC-Environment-Name>",
           "--Company",
           "<Company-Name>",
           "--ConfigurationName",
           "<Configuration-Name>"
         ]
       }
     }
   }
   ```

   **Parameter Details:**
   - `<Your-Tenant-ID>`: Your Azure tenant ID (GUID format)
   - `<Your-Client-ID>`: The Application (client) ID from your Azure app registration
   - `<BC-Environment-Name>`: Name of your Business Central environment (e.g., `Production`, `SandboxUS`)
   - `<Company-Name>`: Business Central company name (case-sensitive)
   - `<Configuration-Name>`: Name of the Business Central configuration (optional)

4. **First-Time Authentication**

   **All Platforms (Windows, macOS, Linux):**
   - Restart Claude Desktop
   - Your browser will open automatically to the device login page (`https://microsoft.com/devicelogin`)
   - The device code is automatically copied to your clipboard
   - Paste the code in the browser (Ctrl+V or Cmd+V)
   - Sign in with your Microsoft account
   - Approve the permissions
   - Your token will be cached securely for future use
   
   **Platform-Specific Details:**
   - **Windows**: Code copied via clipboard, browser auto-opens
   - **macOS**: Code copied via clipboard, browser auto-opens, notification displayed
   - **Linux**: Manual code entry if clipboard/browser helpers unavailable
   
   Authentication uses **Device Code Flow** for a consistent, reliable experience across all platforms.

5. **Verify Setup**
   
   After successful authentication, you should see the BC MCP tools available in Claude:

   ![Claude BC MCP Tools](./docs/images/claude-bc-tools.png)

## Usage

Once configured, you can interact with Business Central through natural language in Claude Desktop. The MCP server will handle authentication and API calls to your Business Central environment.

Example interactions:
- "Show me the latest sales orders"
- "Create a new customer record"
- "What are the current inventory levels?"

## Authentication Methods

The proxy uses **Device Code Flow** for authentication on all platforms (Windows, macOS, and Linux), providing a consistent and reliable authentication experience:

- Browser automatically opens to `https://microsoft.com/devicelogin`
- Device code is automatically copied to your clipboard
- Paste the code in the browser and sign in with your Microsoft account
- Token is cached securely for future use

**Platform-Specific Features:**
- **Windows**: Clipboard copy + browser auto-open
- **macOS**: Clipboard copy + browser auto-open + notification
- **Linux**: Manual code entry if clipboard/browser helpers unavailable

Tokens are cached securely using platform-specific secure storage (managed by MSAL):
- **Windows**: Windows Credential Manager (DPAPI)
- **macOS**: macOS Keychain (`~/Library/Caches/BcMCPProxy/`)
- **Linux**: Secret Service API / keyring

After successful first-time authentication, tokens are reused automatically until they expire.

## Configuration Parameters

| Parameter | Description | Required |
|-----------|-------------|----------|
| `TenantId` | Azure tenant identifier | Yes |
| `ClientId` | Azure app registration client ID | Yes |
| `Environment` | Business Central environment name | Yes |
| `Company` | Business Central company name | Yes |
| `ConfigurationName` | Name of the Business Central configuration | No |

## Troubleshooting

### Common Issues

1. **Authentication Failures**
   - **"Allow public client flows" error (AADSTS7000218)**:
     - Go to Azure Portal → Your App Registration → Authentication → Settings tab
     - Scroll to **Advanced settings** → **Allow public client flows**
     - Set to **Yes** and click **Save**
     - Wait 5-10 minutes for Azure AD to propagate the change
   - **Redirect URI mismatch**:
     - Ensure the native client redirect URI is configured: `https://login.microsoftonline.com/common/oauth2/nativeclient`
     - This URI is required for Device Code Flow on all platforms
   - **API permissions**:
     - Ensure all required API permissions are granted
     - Check that admin consent has been provided if required
     - Verify permissions are for "Dynamics 365 Business Central"

2. **Device Code Flow Issues**
   - **Browser doesn't open automatically**:
     - Manually navigate to https://microsoft.com/devicelogin
     - The device code is copied to your clipboard - paste it in the browser
     - Check Claude Desktop Developer logs for the device code if clipboard copy failed
   - **Device code not visible (macOS)**:
     - Check macOS notifications (top-right corner) for the code
     - The code is automatically copied to clipboard
   - **Token cache issues**:
     - Windows: Token cached in Credential Manager (DPAPI)
     - macOS: Token cached in Keychain at `~/Library/Caches/BcMCPProxy/`
     - Linux: Token cached via Secret Service API
     - To force re-authentication, delete the token cache and restart Claude Desktop

3. **Connection Issues**
   - Verify the Business Central environment name is correct
   - Ensure the company name matches exactly (case-sensitive)
   - Check network connectivity to Business Central
   - Confirm the environment supports MCP preview feature

4. **Claude Desktop Not Detecting MCP Server**
   - **Windows**: Verify the path to `BcMCPProxy.exe` is correct
   - **macOS**: 
     - Verify dotnet path: `/usr/local/share/dotnet/dotnet` (use `which dotnet` in terminal)
     - Verify the path to `BcMCPProxy.dll` is correct (use full absolute path)
   - Check that all required parameters are provided
   - Restart Claude Desktop after configuration changes
   - Check Claude Desktop Developer logs for errors

5. **JSON Parse Errors in Claude**
   - If you see "Unexpected token" errors, check server logs
   - Ensure no special characters are breaking JSON-RPC communication
   - Restart Claude Desktop to clear any cached state

## Security Considerations

- This tool uses delegated permissions and requires user authentication
- Credentials are handled through Azure's authentication flow
- No passwords or secrets are stored in configuration files
- Always follow your organization's security policies when setting up integrations

## Support

This is an experimental tool provided as-is for development and testing purposes. For Business Central specific issues, consult the official Microsoft Dynamics 365 Business Central documentation.

## License

This project is subject to the Microsoft sample code license terms.