> ## Disclaimer
> Microsoft Corporation ("Microsoft") grants you a nonexclusive, perpetual, royalty-free right to use and modify the software code provided by us for the purposes of illustration ("Sample Code") and to reproduce and distribute the object code form of the Sample Code, provided that you agree: (i) to not use our name, logo, or trademarks to market your software product in which the Sample Code is embedded; (ii) to include a valid copyright notice on your software product in which the Sample Code is embedded; and (iii) to indemnify, hold harmless, and defend us and our suppliers from and against any claims or lawsuits, whether in an action of contract, tort or otherwise, including attorneys' fees, that arise or result from the use or distribution of the Sample Code or the use or other dealings in the Sample Code. Unless applicable law gives you more rights, Microsoft reserves all other rights not expressly granted herein, whether by implication, estoppel or otherwise.
>
> THE SAMPLE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL MICROSOFT OR ITS LICENSORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THE SAMPLE CODE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

# BC Admin Center MCP Proxy

A thin local stdio MCP server that wraps the online Business Central Admin Center MCP service, adding transparent Entra ID authentication.

## What it does

```
VS Code ──stdio──► BcAdminMcpProxy ──HTTP+Auth──► mcp.businesscentral.dynamics.com
```

- **Runs locally** as a stdio MCP server (works with VS Code, Claude Desktop, etc.)
- **Injects authentication** — acquires tokens via MSAL for the specified Entra tenant
- **Adds `tenant_id`** parameter to every tool, so you can target any tenant
- **DPAPI-encrypted token cache** — log in once, tokens persist across restarts

## Prerequisites

- .NET 8.0 SDK
- No app registration needed by default (uses a built-in development client ID)
- Optionally, set a custom `ClientId` in `appsettings.json` for production use

## Install

```powershell
.\scripts\install.ps1
```

This publishes to `%LOCALAPPDATA%\BcMcpProxy` and prints the VS Code config snippet.

## VS Code Configuration

Add to your VS Code `settings.json` (or `.vscode/mcp.json`):

```json
{
  "mcp": {
    "servers": {
      "bc-admin-center": {
        "type": "stdio",
        "command": "dotnet",
        "args": [
          "run",
          "--project",
          "C:\\path\\to\\BcAdminMcpProxy"
        ]
      }
    }
  }
}
```

Or after running `install.ps1`:

```json
{
  "mcp": {
    "servers": {
      "bc-admin-center": {
        "type": "stdio",
        "command": "C:\\path\\to\\BcAdminMcpProxy.exe"
      }
    }
  }
}
```

## Configuration

Settings are loaded from (in priority order):

1. Command-line arguments (`--McpProxy:RemoteUrl=...`)
2. Environment variables prefixed with `BCMCP_` (`BCMCP_McpProxy__RemoteUrl=...`)
3. `appsettings.json`

| Setting | Default | Description |
|---------|---------|-------------|
| `McpProxy:RemoteUrl` | `https://mcp.businesscentral.dynamics.com/admin/v1` | Remote MCP endpoint URL |
| `McpProxy:ClientId` | *(empty)* | Entra app client ID. Empty = uses built-in dev client ID. Set a GUID for your own app registration |
| `McpProxy:Scopes` | `["https://api.businesscentral.dynamics.com/.default"]` | OAuth scopes |
| `McpProxy:LoginAuthority` | `https://login.microsoftonline.com` | Entra ID login authority base URL |
| `McpProxy:EntraTenantIds` | `[]` | Allowlist of tenant IDs. Empty = any tenant allowed |

## Authentication

### Default (zero configuration)

Out of the box, no app registration is needed. The proxy uses MSAL with a built-in development client ID. On first use, a browser window opens for interactive login. Tokens are cached locally with DPAPI encryption, so subsequent runs authenticate silently.

### Custom app registration

For production or shared use, register your own Entra ID app and set its client ID in `appsettings.json`. This enables MSAL with a DPAPI-encrypted persistent token cache, so you only log in once.

#### How to create an Entra app registration

1. Go to the [Azure Portal](https://portal.azure.com) > **Microsoft Entra ID** > **App registrations** > **New registration**
2. Set a name (e.g., `BC Admin MCP Proxy`)
3. Under **Supported account types**, select **Accounts in this organizational directory only** (or multi-tenant if needed)
4. Under **Redirect URI**, select **Public client/native (mobile & desktop)** and set the URI to `http://localhost`
5. Click **Register**
6. Copy the **Application (client) ID** from the overview page
7. Go to **API permissions** > **Add a permission** > **APIs my organization uses**
8. Search for **Dynamics 365 Business Central** and select it
9. Select **Delegated permissions** and check:
   - `user_impersonation`
   - `Financials.ReadWrite.All` (optional, for data plane access)
10. Click **Add permissions**
11. Click **Grant admin consent for [your org]** (requires admin role)

#### Configure the proxy

Set the client ID in `appsettings.json`:

```json
{
  "McpProxy": {
    "ClientId": "00000000-0000-0000-0000-000000000000"
  }
}
```

When a `ClientId` is set, the proxy uses MSAL (`PublicClientApplication`) with:

- Silent token acquisition from DPAPI-encrypted cache (`%LOCALAPPDATA%\BcMcpProxy\`)
- Interactive browser login only when the cache is empty or expired

## Getting started

### First time setup

1. **Install** the proxy: `.\scripts\install.ps1`
2. **Configure VS Code** with the MCP config shown above
3. **Call `authenticate`** — a browser window will open for interactive Entra ID login. Sign in with your account. You can optionally pass `tenant_id` to scope the login to a specific tenant, or omit it to sign in with any account.
4. **Call `auth_status`** to verify you are authenticated and see the active tenant ID
5. **Start using tools** — after authentication, all BC Admin Center tools (e.g., `get_environments`, `get_environment_details`) become available
6. On **subsequent runs**, authentication is silent (cached token from DPAPI-encrypted store). If the token has expired, a browser window will open again automatically.

> **Note**: If you are accessing a tenant where you are a guest or GDAP partner, the first `authenticate` call may resolve to your home tenant. In that case, call `authenticate` again with the target tenant's `tenant_id` (GUID) — this may require another interactive login to consent for that tenant.

## Usage

1. Start the proxy (VS Code does this automatically via the MCP config above)
2. Call the `authenticate` tool (optionally with your Entra tenant ID)
3. A browser window opens for interactive login (first time only)
4. All remote tools become available after authentication
5. Subsequent calls reuse the cached token silently

### Tenant access requirements

To use the MCP Admin Center tools on a tenant, one of the following must be true:

- **You are an admin** of the target tenant (direct member with admin role), or
- **GDAP relationship**: your partner tenant has a [Granular Delegated Admin Privileges (GDAP)](https://learn.microsoft.com/en-us/partner-center/gdap-introduction) relationship with the target tenant, and your user is in an AAD security group assigned to the relationship with appropriate roles (e.g., Dynamics 365 Administrator), or
- **Guest user**: you have been invited as a guest (B2B) to the target tenant with appropriate admin roles

### Example flow in VS Code Copilot Chat

```
User: List my BC environments

Copilot: [calls authenticate]
         [calls get_environments]
         Here are your environments: ...
```

## How it works

1. VS Code connects to the proxy via stdio (JSON-RPC 2.0)
2. The proxy handles `initialize` locally, returning server capabilities
3. On `tools/list`, before authentication only the `authenticate` and `auth_status` tools are returned. After authentication, the proxy forwards to the remote MCP and augments each tool's schema with a `tenant_id` parameter
4. On `tools/call`, the proxy:
   - Extracts `tenant_id` from arguments
   - Acquires a Bearer token for that tenant
   - Removes `tenant_id` from the forwarded arguments
   - POSTs the JSON-RPC request to the remote MCP with `Authorization` and `TenantId` headers
   - Returns the response to VS Code
