# Testing on TestPyPI Before PR Submission

This guide walks you through testing the `bc-mcp-proxy` package on TestPyPI to ensure everything works before submitting a PR to Microsoft.

## Prerequisites

1. **TestPyPI Account**: Create one at https://test.pypi.org/account/register/
2. **API Token**: Get one from https://test.pypi.org/manage/account/token/
   - Click "Add API token"
   - Name it (e.g., "bc-mcp-proxy-test")
   - Scope: "Entire account" (or project-specific)
   - **Copy the token immediately** (starts with `pypi-`)

## Step 1: Build the Package

```powershell
cd samples\BcMCPProxyPython
python -m build
```

This creates `dist/bc_mcp_proxy-0.1.0-py3-none-any.whl` and `dist/bc_mcp_proxy-0.1.0.tar.gz`.

## Step 2: Upload to TestPyPI

Use the helper script (works with or without pre-configured credentials). Run from the package root directory:

**Windows (PowerShell):**
```powershell
./maintainer/scripts/publish.ps1 testpypi
```

**macOS/Linux (bash):**
```bash
./maintainer/scripts/publish.sh testpypi
```

When prompted:
- **Username**: `__token__` (literally type `__token__`)
- **Password**: Paste your API token (the `pypi-...` string). In PowerShell, you can right-click to paste even if the text doesn't appear visible.

**Alternative:** You can also use twine directly:
```powershell
python -m pip install --upgrade twine
python -m twine upload --repository testpypi dist/*
```

## Step 3: Verify Upload

Check that your package appears at:
https://test.pypi.org/project/bc-mcp-proxy/

## Step 4: Test Installation in Clean Environment

Create a clean test environment and run the unified setup flow:

```bash
# Create test directory and virtual environment
mkdir test-install && cd test-install
python3 -m venv venv
source venv/bin/activate  # Windows: .\venv\Scripts\Activate.ps1

# Install from TestPyPI and run setup (same command as Quick Start)
python3 -m pip install --index-url https://test.pypi.org/simple/ --extra-index-url https://pypi.org/simple/ bc-mcp-proxy && python3 -m bc_mcp_proxy setup
```

**Note**: Use `--extra-index-url https://pypi.org/simple/` because TestPyPI doesn't have all dependencies.

Verify:
- Package installs successfully
- Setup prompts for Business Central credentials
- Device-code authentication works
- Config files and install links are generated in `~/.bc_mcp_proxy/` (or `%USERPROFILE%\.bc_mcp_proxy\` on Windows)

## Step 5: Test MCP Client Integration

The setup command generates ready-to-use config files in `~/.bc_mcp_proxy/` (or `%USERPROFILE%\.bc_mcp_proxy\` on Windows):

- `cursor_mcp.json` - Copy this into your Cursor MCP configuration
- `vscode_mcp.json` - Copy this into your VS Code MCP configuration  
- `claude_mcp.json` - Copy this into your Claude Desktop configuration

Alternatively, use the install links printed by the setup command to add the MCP server with one click.

Test in your chosen MCP client to ensure the server connects and Business Central tools are available.

## Troubleshooting

### "Package not found" during installation
- Ensure you're using `--index-url https://test.pypi.org/simple/`
- Check the package name matches exactly: `bc-mcp-proxy`
- Verify the upload succeeded at https://test.pypi.org/project/bc-mcp-proxy/

### "No matching distribution found" for dependencies
- Always include `--extra-index-url https://pypi.org/simple/` when installing from TestPyPI
- TestPyPI doesn't mirror all packages from PyPI

### Authentication errors
- Double-check your API token is correct
- Ensure you're using `__token__` as the username (with underscores)
- Token should start with `pypi-`

### Import errors after installation
- Verify the package structure is correct in `dist/`
- Check that `bc_mcp_proxy/__init__.py` and `__main__.py` are included

### "No module named bc_mcp_proxy" when Cursor/VS Code tries to use it
This happens when the package is installed in a virtual environment, but Cursor/VS Code is using the system Python.

**Solution 1: Install with `--user` flag (recommended)**
```bash
python3 -m pip install --user --index-url https://test.pypi.org/simple/ --extra-index-url https://pypi.org/simple/ bc-mcp-proxy
python3 -m bc_mcp_proxy setup
```

**Solution 2: Install globally**
```bash
# Exit your venv first
deactivate
python3 -m pip install --index-url https://test.pypi.org/simple/ --extra-index-url https://pypi.org/simple/ bc-mcp-proxy
python3 -m bc_mcp_proxy setup
```

**Solution 3: Use full path to venv Python**
If you must use a venv, update the generated config files (in `~/.bc_mcp_proxy/`) to use the full path to your venv's Python executable instead of just `python` or `python3`.

## Success Criteria

✅ Package uploads to TestPyPI without errors  
✅ Package installs from TestPyPI in a clean environment  
✅ Unified setup command works (`python -m bc_mcp_proxy setup`)  
✅ Setup generates config files and install links correctly  
✅ Proxy connects to Business Central successfully  
✅ MCP client can use the proxy via stdio  

Once all these pass, you're ready to submit the PR to Microsoft!

