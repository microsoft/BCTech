# Publishing `bc-mcp-proxy` to PyPI

> **Note for Maintainers**: This document is for Microsoft maintainers who have permissions to publish the `bc-mcp-proxy` package to PyPI. End users do not need this information.

These steps assume you have maintainer permissions for the `bc-mcp-proxy`
package on PyPI.

## 1. Prerequisites

- Python 3.10 or later
- Valid PyPI API token stored in `~/.pypirc` (recommended)
- Latest versions of `pip`, `build`, and `twine`

```powershell
python -m pip install --upgrade pip
```

## 2. Build the distribution

Run the helper script (PowerShell or Bash) from the package root directory. It installs
`build` automatically, then produces `dist/*.whl` and `dist/*.tar.gz`.

```powershell
./maintainer/scripts/build.ps1
```

```bash
./maintainer/scripts/build.sh
```

## 3. Upload to PyPI

Once artifacts exist in `dist/`, publish them with `twine`. The helper scripts
install `twine` and call `python -m twine upload`.

```powershell
./maintainer/scripts/publish.ps1          # uploads to pypi
./maintainer/scripts/publish.ps1 testpypi # uploads to TestPyPI
```

```bash
./maintainer/scripts/publish.sh           # uploads to pypi
./maintainer/scripts/publish.sh testpypi  # uploads to TestPyPI
```

## 4. Post-publish checklist

- Verify the release appears on https://pypi.org/project/bc-mcp-proxy/
- Update any documentation that references the version number if a bump was made
- Announce availability to internal teams or update change logs as needed

