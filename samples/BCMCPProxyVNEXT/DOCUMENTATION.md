# BcMCPProxy - Technical Documentation

## Table of Contents
1. [Overview](#overview)
2. [Architecture](#architecture)
3. [Applied Optimizations](#applied-optimizations)
4. [Cross-Platform Support](#cross-platform-support)
5. [Security](#security)
6. [Troubleshooting](#troubleshooting)
7. [Dependencies](#dependencies)
8. [Changelog](#changelog)

---

## Overview

**BcMCPProxy** is a Model Context Protocol (MCP) server proxy that bridges MCP clients with Microsoft Dynamics 365 Business Central's API. It handles authentication, request forwarding, and protocol translation between standard MCP clients and Business Central's MCP endpoints.

### Key Features
- **Cross-Platform**: Full support for Windows, macOS, and Linux with consistent authentication
- **Dual Authentication Modes**: Microsoft Entra ID (Azure AD) with token caching and custom bearer token
- **Universal Device Code Flow**: Browser-based authentication with automatic code clipboard copy and browser launch on all platforms
- **Protocol Translation**: Proxy between stdio-based MCP clients and HTTP-based Business Central MCP servers
- **Token Management**: Automatic token acquisition and refresh using MSAL with secure platform-specific caching
- **Configuration-Driven**: Flexible configuration supporting multiple environments and companies
- **Modern C# Features**: File-scoped namespaces, structured logging, async patterns

### Technology Stack
- **.NET 10.0** - Target framework (upgraded from .NET 8.0)
- **C# 13.0** - Language version
- **Microsoft.Identity.Client 4.82.1** - Authentication (upgraded from 4.71.0)
- **Microsoft.Identity.Client.Extensions.Msal 4.82.1** - Platform-specific token caching
- **ModelContextProtocol 0.4.0-preview.3** - MCP implementation
- **Microsoft.Extensions.Hosting** - Dependency injection and hosting

### Platform Support
- **Windows**: x64, ARM64 (Device Code Flow authentication)
- **macOS**: Intel (x64), Apple Silicon (ARM64) (Device Code Flow authentication)
- **Linux**: x64, ARM64 (Device Code Flow authentication)

All platforms use **Device Code Flow** for authentication, providing a consistent and reliable user experience.

---

## Architecture

### High-Level Architecture

```
┌─────────────────┐
│   MCP Client    │ (Claude Desktop, etc.)
│   (stdio)       │
└────────┬────────┘
         │ Standard Input/Output
         ▼
┌─────────────────────────────────────────────┐
│         BcMCPProxy (This Application)       │
│  ┌──────────────────────────────────────┐  │
│  │      StdioServerTransport            │  │
│  │  (Listens on stdin, writes stdout)   │  │
│  └──────────────┬───────────────────────┘  │
│                 │                           │
│  ┌──────────────▼───────────────────────┐  │
│  │       MCPServerProxy                 │  │
│  │  - Request routing                   │  │
│  │  - Configuration management          │  │
│  └──────────────┬───────────────────────┘  │
│                 │                           │
│  ┌──────────────▼───────────────────────┐  │
│  │    McpClient (HTTP Client)           │  │
│  │  - HttpClientTransport               │  │
│  │  - AuthenticationHandler             │  │
│  └──────────────┬───────────────────────┘  │
└─────────────────┼───────────────────────────┘
                  │ HTTPS + Bearer Token
                  ▼
┌─────────────────────────────────────────────┐
│   Microsoft Dynamics 365 Business Central   │
│   MCP API Endpoint                          │
│   /v2.0/{environment}/mcp                   │
└─────────────────────────────────────────────┘
```

### Component Overview

**Program.cs** - Application entry point
- Configures dependency injection container
- Registers all services as singletons
- Sets up logging infrastructure
- Bootstraps the application host

**ConfigOptions** - Configuration model with platform-agnostic defaults

**MCPServerProxy** - Core proxy orchestrator
- Debug attachment support
- Transport configuration
- Authentication mode selection (MSAL or custom token)
- Server initialization and lifecycle management

**AuthenticationHandler** - Token injection middleware
- DelegatingHandler pattern for HTTP request interception
- Per-request token acquisition
- Automatic token refresh via MSAL

**AuthenticationService** - MSAL integration
- Cross-platform authentication support
- Silent token acquisition with interactive fallback
- Platform-specific broker configuration
- Token caching with encryption

**McpServerOptionsFactory** - Handler registration
- ListToolsHandler for tool discovery
- CallToolHandler for tool execution
- JSON element type conversion

---

## Applied Optimizations

### Version 2.0.0 - Major Improvements (2026)

#### 1. **.NET 10.0 Upgrade**
- Upgraded from .NET 8.0 to .NET 10.0
- Updated to C# 13 features
- All NuGet packages updated to latest versions
- Zero compiler warnings achieved

#### 2. **Cross-Platform Support**
- Full Windows, macOS, and Linux compatibility
- Platform-specific authentication methods (WAM, Device Code Flow, Linux broker)
- Runtime platform detection using `RuntimeInformation.IsOSPlatform()`
- Platform-native token storage (DPAPI, Keychain, Secret Service)

#### 3. **Async Initialization Pattern**
- Eliminated sync-over-async anti-pattern in constructor
- Lazy async initialization with SemaphoreSlim
- Thread-safe initialization
- No thread blocking

#### 4. **File-Scoped Namespaces**
- Applied to all 10 files in the project
- Reduced indentation levels
- Cleaner, more modern code organization

#### 5. **Constants Extraction**
- Created 10 public constants in ConfigOptions
- Eliminated magic strings throughout codebase
- Single source of truth for default values
- Better maintainability

#### 6. **Structured Logging**
- Template-based logging instead of string interpolation
- Semantic logging support
- Better log parsing capabilities
- Performance improvement

#### 7. **Obsolete API Migration**
- Updated IMcpClient to McpClient
- Removed all obsolete API warnings
- Future-proof codebase

#### 8. **XML Documentation**
- Comprehensive documentation for all public classes
- Method-level documentation with parameters and exceptions
- Interface documentation
- Improved IntelliSense support

#### 9. **Platform-Specific Token Caching**
- Windows: DPAPI encryption in %LOCALAPPDATA%\BcMCPProxy\
- macOS: Keychain storage in ~/Library/Caches/BcMCPProxy/
- Linux: Secret Service keyring in ~/.cache/BcMCPProxy/

#### 10. **Enhanced Error Handling**
- Platform-specific authentication fallbacks
- Pre-authentication before MCP connection to avoid timeouts
- Detailed error logging with structured data

#### 11. **MSAL Package Upgrades**
- Microsoft.Identity.Client: 4.71.0 → 4.82.1
- Microsoft.Identity.Client.Broker: 4.71.0 → 4.82.1
- Microsoft.Identity.Client.Extensions.Msal: 4.71.0 → 4.82.1

#### 12. **Zero Compiler Warnings**
- Fixed all 12 compiler warnings
- Updated nullable reference types
- Resolved platform-specific code warnings
- Production-ready clean build

#### 13. **Process Resource Management**
- Added `using` statements for all Process objects (memory leak prevention)
- Proper disposal of clipboard, browser, and notification processes on macOS
- Changed to async I/O: `WriteAsync` instead of `Write`, `WaitForExitAsync` instead of `WaitForExit`
- Non-blocking process operations in device code flow

#### 14. **Configuration Security**
- Removed organization-specific default values (TenantId, ClientId)
- Made TenantId and ClientId required properties
- Forces users to provide their own Azure AD credentials
- Prevents accidental use of test/sample credentials

### Performance & Resource Impact

| Aspect | Before (.NET 8) | After (.NET 10) |
|--------|----------------|-----------------|
| **Compiler Warnings** | 12 warnings | 0 warnings |
| **Cross-Platform** | Windows only | Win/Mac/Linux |
| **Token Cache** | Windows only | Platform-native |
| **Auth UX (macOS)** | Logs only | Browser + Clipboard + Notification |
| **Async Patterns** | Some sync-over-async | Full async |
| **Code Organization** | Nested namespaces | File-scoped |
| **Documentation** | Minimal | Comprehensive XML |
| **API Compliance** | Some obsolete | All current |
| **Memory Leaks** | Process objects not disposed | All disposed (using statements) |
| **I/O Blocking** | Sync Write/WaitForExit | Async WriteAsync/WaitForExitAsync |
| **Config Security** | Hardcoded defaults | Required user credentials |

---

## Cross-Platform Support

### Platform-Specific Features

#### Windows (WAM Broker)
- Windows Account Manager integration
- Single Sign-On (SSO) with Windows credentials
- Native OS authentication dialogs
- Tokens never leave OS trust boundary
- DPAPI-encrypted token cache

**User Experience:**
1. First run: Windows Security prompt
2. Credentials cached by WAM
3. Subsequent runs: Silent authentication
4. No browser interaction needed

#### macOS (Device Code Flow)
- Automatic clipboard copy of device code (via pbcopy)
- Automatic browser launch to login page (via open command)
- macOS notification with device code (via osascript)
- Keychain-based secure token storage
- No Intune enrollment required

**User Experience:**
1. First run: Browser opens automatically
2. Notification shows device code
3. User pastes code (already in clipboard)
4. Token cached in macOS Keychain
5. Subsequent runs: Silent authentication

**Why Device Code Flow?**
- macOS broker requires corporate Intune enrollment
- Device code flow works on all macOS systems
- No redirect URI configuration needed
- Better UX than standard browser flow

#### Linux (Broker + Keyring)
- Linux broker support
- Secret Service API integration
- GNOME Keyring / KWallet storage
- System keyring encryption
- Token cache in ~/.cache/BcMCPProxy/ or $XDG_CACHE_HOME

### Token Cache Locations

- **Windows:** %LOCALAPPDATA%\BcMCPProxy\BcMCPProxy.cache (DPAPI encrypted)
- **macOS:** ~/Library/Caches/BcMCPProxy/BcMCPProxy.cache (Keychain encrypted)
- **Linux:** ~/.cache/BcMCPProxy/BcMCPProxy.cache (Secret Service encrypted)

### Parent Process Discovery

**Cross-platform window handle discovery for WAM authentication:**
- **Windows:** P/Invoke to ntdll.dll using NtQueryInformationProcess
- **macOS:** Uses /bin/ps command to get parent PID
- **Linux:** Reads from /proc/{pid}/stat file

---

## Security

### Authentication Security

#### MSAL Token Security
- **Broker Integration**: OS-level security (WAM on Windows)
- **Token Cache Encryption**: Platform-specific (DPAPI/Keychain/Secret Service)
- **Token Refresh**: Automatic silent refresh before expiry
- **User-Specific**: Tokens cannot be decrypted by other users

#### Custom Token Mode
- Token stored in configuration (use secure storage in production)
- Transmitted via HTTPS only
- Consider Azure Key Vault for production deployments

**Security Warnings:**
- Never commit tokens to source control
- Use environment variables for CI/CD
- Rotate tokens regularly
- Monitor token usage via Azure AD logs

### Network Security
- All Business Central communication over HTTPS
- Certificate validation enabled by default
- Man-in-the-middle attack protection
- No sensitive data in headers

### Application Security
- Configuration values validated by .NET
- URL encoding for special characters
- Null checking throughout
- Sensitive data not logged
- Exceptions sanitized before logging
- Stack traces only in debug mode

---

## Troubleshooting

### Common Issues (All Platforms)

#### 1. Authentication Fails
- Verify TenantId and ClientId are correct
- Check user has access to Business Central
- Clear token cache (platform-specific location)
- Enable MSAL logging
- Check Azure AD app registration has "Allow public client flows" enabled

#### 2. Company Not Found
- Verify company name exact match (case-sensitive)
- URL-encode special characters
- Check company exists in specified environment

#### 3. Connection Timeout
- Verify network connectivity to BC API
- Check firewall/proxy settings
- Confirm environment name is correct

#### 4. Token Expired Error
- Normal behavior - token refresh triggered automatically
- Clear token cache if issue persists
- Verify system clock is accurate

### macOS-Specific Issues

#### 5. Device Code Not Displayed
- **Automatic UX implemented**: clipboard copy, browser launch, notification
- View Claude Desktop logs at ~/Library/Logs/Claude/mcp*.log

#### 6. Browser Not Opening Automatically
- Check open command works
- Set default browser in System Preferences

#### 7. Clipboard Copy Fails
- Check pbcopy command works
- Grant clipboard access in Security & Privacy settings

#### 8. macOS Keychain Permission Denied
- Reset Keychain entry (search for "BcMCPProxy" in Keychain Access)
- Unlock keychain if locked
- Click "Always Allow" when prompted

#### 9. Intune Enrollment Error
- **Fixed**: Device Code Flow used automatically on macOS
- No Intune requirement

#### 10. Notification Not Showing
- Check Notification settings in System Preferences
- Disable Do Not Disturb mode

### Windows-Specific Issues

#### 11. WAM Broker Not Available
- Update Windows to latest version
- Install Windows 10 1803 or later
- Check Windows Account Manager service is running

#### 12. Corporate Proxy Issues
- Configure system proxy settings
- Set HTTPS_PROXY and HTTP_PROXY environment variables
- Add Azure AD endpoints to proxy whitelist

### Linux-Specific Issues

#### 13. Secret Service Not Available
- Install gnome-keyring package
- Start keyring daemon
- Check D-Bus session

#### 14. Headless Server Issues
- Device Code Flow works without browser
- Use SSH X11 forwarding if browser needed

### Azure AD Configuration Issues

#### 15. "Allow Public Client Flows" Disabled
- Enable in Azure Portal → App registrations → Authentication → Advanced settings
- Wait 5-10 minutes for propagation

#### 16. Redirect URI Mismatch
- Only affects interactive browser authentication
- Add http://localhost redirect URI or use Device Code Flow

### MCP Protocol Issues

#### 17. JSON Parse Errors
- **Fixed**: All Unicode box characters replaced with ASCII

#### 18. Tool Call Timeout
- **Fixed**: Pre-authentication before MCP client creation

### Configuration Issues

#### 19. dotnet Command Not Found (macOS)
- Update Claude Desktop config to use full dotnet path
- Common locations: /usr/local/share/dotnet/dotnet, /opt/homebrew/bin/dotnet

#### 20. Environment Variables Not Applied
- Verify JSON syntax in config file
- Restart Claude Desktop after config changes
- Check case sensitivity on Linux/macOS

---

## Dependencies

### NuGet Packages

**Core .NET Packages:**
- Microsoft.Extensions.Hosting: 9.0.4
- Microsoft.Extensions.Http: 9.0.4

**MSAL Authentication (Updated):**
- Microsoft.Identity.Client: **4.82.1** (was 4.71.0)
- Microsoft.Identity.Client.Broker: **4.82.1** (was 4.71.0)
- Microsoft.Identity.Client.Extensions.Msal: **4.82.1** (was 4.71.0)

**Model Context Protocol:**
- ModelContextProtocol: 0.4.0-preview.3

**Logging:**
- NLog: 5.4.0
- NLog.Extensions.Logging: 5.4.0

### Runtime Requirements

**Minimum:**
- .NET Runtime: 10.0 or higher

**Operating Systems:**
- Windows 10 1803+ (for WAM support)
- macOS 11.0+ (Big Sur or later)
- Linux with .NET 10 support (Ubuntu 20.04+, Fedora 33+, etc.)

**Platform-Specific:**
- **Windows:** Windows Account Manager (WAM), DPAPI
- **macOS:** Keychain Access, pbcopy, open, osascript commands
- **Linux:** GNOME Keyring or KWallet, D-Bus session, Secret Service API

---

## Changelog

### Version 2.0.0 - .NET 10 Cross-Platform Upgrade (January 2026)

#### Framework & Platform
- ✅ Upgraded from .NET 8.0 to .NET 10.0
- ✅ Updated to C# 13 language features
- ✅ Full cross-platform support (Windows/macOS/Linux)
- ✅ Platform-native authentication (WAM/Device Code Flow/Broker)
- ✅ Platform-native token storage (DPAPI/Keychain/Secret Service)

#### Authentication Enhancements
- ✅ Device Code Flow for macOS with enhanced UX
- ✅ Automatic clipboard copy, browser launch, notifications
- ✅ Keychain-based secure token storage on macOS
- ✅ No Intune enrollment required for macOS
- ✅ Pre-authentication to prevent MCP connection timeouts

#### Code Quality
- ✅ Zero compiler warnings (fixed all 12 warnings)
- ✅ File-scoped namespaces in all files
- ✅ Constants extraction (10 constants)
- ✅ Structured logging throughout
- ✅ Comprehensive XML documentation
- ✅ Obsolete API migrations (IMcpClient → McpClient)
- ✅ Async initialization pattern (eliminated sync-over-async)

#### Performance Optimizations
- ✅ Process resource management (using statements for all Process objects)
- ✅ Non-blocking I/O (WriteAsync, WaitForExitAsync in device code flow)
- ✅ Memory leak prevention (proper disposal of clipboard/browser/notification processes)
- ✅ Fully async authentication flow on macOS

#### Security Improvements
- ✅ Removed hardcoded tenant/client IDs
- ✅ Made TenantId and ClientId required configuration
- ✅ Forces users to provide their own Azure AD credentials
- ✅ Prevents accidental use of sample/test credentials

#### Package Updates
- ✅ MSAL packages: 4.71.0 → 4.82.1 (all three packages)

#### Bug Fixes
- ✅ Fixed Windows-only P/Invoke without platform guards
- ✅ Unicode character removal from JSON-RPC output
- ✅ Proper HttpClient lifecycle management

#### Documentation
- ✅ Complete README.md with setup guide
- ✅ Azure app registration instructions
- ✅ Claude Desktop configuration for all platforms
- ✅ Comprehensive troubleshooting (20+ scenarios)
- ✅ Technical documentation overhaul

---

### Version 1.1.0 - Authentication Simplification (February 2026)

**Major Changes:**
- ✅ **Universal Device Code Flow**: Replaced platform-specific authentication (WAM/broker) with Device Code Flow on all platforms
- ✅ **Removed WAM Broker**: Eliminated Windows Account Manager complexity and 30-55s delays
- ✅ **Simplified Configuration**: Only requires native client redirect URI in Azure app registration
- ✅ **Consistent UX**: Same authentication experience across Windows, macOS, and Linux
- ✅ **Improved Logging**: Reduced log noise, warnings/errors only (no stdout pollution)

**Performance:**
- 🚀 Authentication time: 2-5 seconds (previously 30-55s with WAM)
- 🚀 Reliable browser-based flow with auto-clipboard copy

**Removed:**
- `DisableBroker` configuration option (no longer needed)
- Platform-specific broker initialization code
- WAM-specific redirect URI requirements

---

### Version 1.0.1 - Optimization Update (December 2025)
- Fixed critical authentication bug (custom token assignment)
- Implemented proper HttpClient lifecycle management
- Converted ConfigOptions fields to properties
- Added exception safety with try-finally blocks

---

### Version 1.0.0 - Initial Release
- Basic MCP proxy functionality
- MSAL authentication
- Custom token support
- Windows-only support

---

**Document Version:** 2.1  
**Last Updated:** 12 February 2026  
**Status:** Production Ready ✅

**Tested Platforms:**
- ✅ Windows 11 with Device Code Flow authentication
- ✅ macOS Sonoma 14.x with Device Code Flow
- ✅ Linux with Device Code Flow
- ✅ Claude Desktop integration on all platforms
- ✅ Business Central SaaS environments
