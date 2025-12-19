# Business Central MCP Proxy (Python)

## ⚠️ Experimental Tool - Not for Production Use

This tool is for **experimentation only** and is **not intended for production use**. It allows you to connect Cursor, VS Code, Claude Desktop, or other MCP-compatible clients to a Business Central Model Context Protocol (MCP) server.

## Overview

The BC MCP Proxy acts as a bridge between MCP-compatible clients (like Cursor, VS Code, Claude Desktop) and Business Central, enabling natural language interactions with your Business Central data and operations. This Python implementation is published as `bc-mcp-proxy` on PyPI.

### Why a Python Version?

This Python implementation was created to provide a simpler, cross-platform alternative to the [.NET version](../BcMCPProxy/README.md). Key differences:

- **Easier Installation**: Install directly from PyPI with a single command (`pip install bc-mcp-proxy`), no need to build or download executables
- **Cross-Platform**: Works on Windows, macOS, and Linux without platform-specific builds
- **Unified Setup**: Interactive setup command (`python -m bc_mcp_proxy setup`) handles configuration, authentication, and generates client configs automatically
- **Better IDE Integration**: Works seamlessly with Cursor, VS Code, and other MCP clients that expect Python-based tools
- **Same Functionality**: Provides the same MCP proxy capabilities as the .NET version, connecting stdio-based MCP clients to Business Central's MCP HTTP endpoint

Both versions provide the same core functionality—choose the Python version if you prefer easier installation and cross-platform support, or the .NET version if you prefer a compiled executable.

## Prerequisites

- Microsoft Dynamics 365 Business Central environment with MCP preview feature enabled
- Azure tenant with appropriate permissions
- Python 3.10 or later
- MCP-compatible client (Cursor, VS Code, Claude Desktop, etc.)

## Setup Instructions

### 1. Set-up Azure AD App Registration

1. Open [ms.portal.azure.com](https://ms.portal.azure.com)
2. Navigate to **Microsoft Entra ID** and create a new **App Registration**
3. In the **Authentication** section, add the desktop app Redirect URL in the format:
   ```
   ms-appx-web://Microsoft.AAD.BrokerPlugin/<clientID>
   ```
   
   Also enable **"Allow public client flows"** under the Authentication section.

4. Add the following API permissions:

   Required permissions:
   - **Dynamics 365 Business Central (2)**
     - `Financials.ReadWrite.All` (Delegated)
     - `user_impersonation` (Delegated)

### 2. Install and Configure the Proxy

1. **Install Python 3.10+** from [python.org](https://www.python.org/downloads/) if not already installed.

2. **Run the unified setup command** in a terminal (PowerShell, Command Prompt, bash, zsh, etc.):

   ```bash
   python -m pip install --upgrade bc-mcp-proxy && python -m bc_mcp_proxy setup
   ```

   The interactive setup will:
   - Prompt for your Business Central tenant ID, client ID, environment, and company
   - Launch the device-code login flow (so you sign in immediately)
   - Generate ready-to-click install URLs for Cursor and VS Code plus a Claude Desktop snippet
   - Save everything under `~/.bc_mcp_proxy/` (or `%USERPROFILE%\.bc_mcp_proxy\` on Windows)

3. **Configure Your MCP Client**

   The setup command generates ready-to-use configuration files and install links:
   
   - **Cursor/VS Code**: Use the clickable install link printed by setup, or copy the generated `cursor_mcp.json` or `vscode_mcp.json` from `~/.bc_mcp_proxy/` into your MCP configuration
   - **Claude Desktop**: Copy the generated `claude_mcp.json` snippet into your `claude_desktop_config.json`

4. **Restart Your MCP Client**

   After adding the configuration, restart your MCP client. You should see Business Central tools available in your client.

## Usage

Once configured, you can interact with Business Central through natural language in your MCP client. The proxy will handle authentication and API calls to your Business Central environment.

Example interactions:
- "Show me the latest sales orders"
- "Create a new customer record"
- "What are the current inventory levels?"
- "List all customers"

## Configuration Parameters

| Parameter | CLI Argument | Environment Variable | Description | Required |
|-----------|--------------|---------------------|-------------|----------|
| Tenant ID | `--TenantId` | `BC_TENANT_ID` | Azure tenant identifier | Yes |
| Client ID | `--ClientId` | `BC_CLIENT_ID` | Azure app registration client ID | Yes |
| Environment | `--Environment` | `BC_ENVIRONMENT` | Business Central environment name (default: `Production`) | Yes |
| Company | `--Company` | `BC_COMPANY` | Business Central company name | Yes |
| Configuration Name | `--ConfigurationName` | `BC_CONFIGURATION_NAME` | Name of the Business Central configuration | No |
| Custom Auth Header | `--CustomAuthHeader` | `BC_CUSTOM_AUTH_HEADER` | Pre-issued bearer token (skips device flow) | No |
| Base URL | `--BaseUrl` | `BC_BASE_URL` | Base API URL (default: `https://api.businesscentral.dynamics.com`) | No |
| Token Scope | `--TokenScope` | `BC_TOKEN_SCOPE` | OAuth scope (default: `https://api.businesscentral.dynamics.com/.default`) | No |
| Log Level | `--LogLevel` | `BC_LOG_LEVEL` | Logging level (default: `INFO`) | No |
| Debug | `--Debug` | `BC_DEBUG=1` | Enable verbose logging | No |

When no custom bearer token is supplied, the proxy performs a device-code flow. Tokens are cached using `msal-extensions` under:

- **Windows**: `%LOCALAPPDATA%\BcMCPProxyPython\bc_mcp_proxy.bin`
- **macOS**: `~/Library/Caches/BcMCPProxyPython/bc_mcp_proxy.bin`
- **Linux**: `$XDG_CACHE_HOME/BcMCPProxyPython/bc_mcp_proxy.bin` (or `~/.cache/...`)

## Running the Proxy

After setup, you can run the proxy manually using the command printed by setup, or with:

```bash
python -m bc_mcp_proxy --TenantId "<tenant-id>" --ClientId "<client-id>" --Environment "<environment>" --Company "<company>"
# or
bc-mcp-proxy --TenantId "<tenant-id>" --ClientId "<client-id>" --Environment "<environment>" --Company "<company>"
```

When the proxy logs `Connecting to Business Central MCP endpoint...` followed by device-code instructions, sign in (if prompted). The proxy stays running until you stop it.

Configuration files, logs, and generated MCP snippets are stored at:

- **Windows**: `%USERPROFILE%\.bc_mcp_proxy\`
- **macOS/Linux**: `~/.bc_mcp_proxy/`

Re-run `python -m bc_mcp_proxy setup` at any time to update or regenerate these files.

## Troubleshooting

### Common Issues

1. **Authentication Failures**
   - Verify the redirect URL format in your Azure app registration: `ms-appx-web://Microsoft.AAD.BrokerPlugin/<clientID>`
   - Ensure "Allow public client flows" is enabled in your Azure app registration
   - Ensure all required API permissions are granted
   - Check that admin consent has been provided if required
   - If device flow times out, rerun setup

2. **Connection Issues**
   - Verify the Business Central environment name is correct
   - Ensure the company name matches exactly (case-sensitive)
   - Check network connectivity to Business Central
   - Confirm the environment supports MCP preview feature

3. **MCP Client Not Detecting Server**
   - Verify the Python path in your MCP configuration is correct
   - Check that the `bc-mcp-proxy` package is installed (run `python -m pip list | grep bc-mcp-proxy`)
   - If using a virtual environment, ensure the MCP client uses the correct Python interpreter
   - Restart your MCP client after configuration changes

4. **"No module named bc_mcp_proxy" Error**
   - Install the package globally: `python -m pip install --upgrade bc-mcp-proxy`
   - Or use `--user` flag: `python -m pip install --user --upgrade bc-mcp-proxy`
   - Or update your MCP config to use the full path to the Python interpreter that has the package installed

5. **Repeated Sign-in Prompts**
   - The token cache may not be writable. Set `--DeviceCacheLocation` to a directory you control
   - Check file permissions on the cache directory

6. **Business Central API Errors**
   - Confirm the environment supports MCP and the company name is spelled exactly (case-sensitive)
   - Verify your Azure app has the correct API permissions

## Security Considerations

- This tool uses delegated permissions and requires user authentication
- Credentials are handled through Azure's authentication flow
- No passwords or secrets are stored in configuration files
- Tokens are cached securely using `msal-extensions` with platform-specific secure storage
- Always follow your organization's security policies when setting up integrations

## Support

This is an experimental tool provided as-is for development and testing purposes. For Business Central specific issues, consult the official Microsoft Dynamics 365 Business Central documentation.

## License

Licensed under the MIT License (see `LICENSE`).
