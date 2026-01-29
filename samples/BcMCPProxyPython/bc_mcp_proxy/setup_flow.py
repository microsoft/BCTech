from __future__ import annotations

import asyncio
import base64
import json
import logging
import sys
from dataclasses import asdict
from pathlib import Path
from typing import Any, Dict, List, Optional
from urllib.parse import quote
import shlex

from .auth import create_token_provider
from .config import ProxyConfig

LOGGER = logging.getLogger("bc_mcp_proxy.setup")

OUTPUT_DIR = Path.home() / ".bc_mcp_proxy"
OUTPUT_DIR.mkdir(parents=True, exist_ok=True)


def run_interactive_setup() -> None:
  """Run the interactive setup wizard."""
  print("Business Central MCP Proxy – Setup Wizard")
  print("-----------------------------------------\n")

  config = collect_configuration()

  save_configuration(config)

  authenticate_with_business_central(config)

  cursor_config, vscode_config, claude_config = generate_client_configs(config)
  cursor_url, vscode_url = generate_install_links(cursor_config, vscode_config)

  write_client_configs(cursor_config, vscode_config, claude_config)

  print_next_steps(config, cursor_url, vscode_url)


def collect_configuration() -> ProxyConfig:
  """Prompt the user for Business Central connection details."""
  def prompt(label: str, default: Optional[str] = None, allow_empty: bool = False) -> str:
    while True:
      suffix = f" [{default}]" if default else ""
      value = input(f"{label}{suffix}: ").strip()
      if value:
        return value
      if not value and default is not None:
        return default
      if allow_empty:
        return ""
      print("  Value required. Please try again.")

  tenant_id = prompt("Azure Tenant ID (Directory ID)")
  client_id = prompt("Azure App Registration Client ID")
  environment = prompt("Business Central Environment", default="Production")
  company = prompt("Business Central Company (exact name)")
  configuration_name = prompt("Business Central MCP Configuration Name (optional)", allow_empty=True)

  config = ProxyConfig(
      tenant_id=tenant_id,
      client_id=client_id,
      environment=environment,
      company=company,
      configuration_name=configuration_name or None,
  )

  print("\nCollected Configuration:")
  print(f"  Tenant ID         : {config.tenant_id}")
  print(f"  Client ID         : {config.client_id}")
  print(f"  Environment       : {config.environment}")
  print(f"  Company           : {config.company}")
  if config.configuration_name:
    print(f"  Configuration Name: {config.configuration_name}")
  print("")

  return config


def save_configuration(config: ProxyConfig) -> None:
  """Persist configuration so subsequent runs can reuse it."""
  try:
    OUTPUT_DIR.mkdir(parents=True, exist_ok=True)
    path = OUTPUT_DIR / "config.json"
    with path.open("w", encoding="utf-8") as stream:
      json.dump(asdict(config), stream, indent=2)
    print(f"Configuration saved to {path}")
  except Exception as exc:  # pragma: no cover - log and continue
    LOGGER.warning("Unable to persist configuration: %s", exc)


def authenticate_with_business_central(config: ProxyConfig) -> None:
  """Trigger device-code authentication to ensure the setup is complete."""
  print("\nStarting device-code authentication with Business Central...")
  provider = create_token_provider(config, logger=LOGGER)
  try:
    asyncio.run(provider.get_token())
  except KeyboardInterrupt:
    print("\nAuthentication cancelled by user.")
    sys.exit(1)
  except Exception as exc:  # pragma: no cover - handled interactively
    print(f"\nAuthentication failed: {exc}")
    sys.exit(1)
  else:
    print("Authentication successful!\n")


def generate_client_configs(
    config: ProxyConfig,
) -> tuple[Dict[str, Any], Dict[str, Any], Dict[str, Any]]:
  """Produce MCP client configuration dictionaries."""
  # Use the current Python interpreter, with platform-aware fallback
  executable = sys.executable or ("python3" if sys.platform != "win32" else "python")
  args: List[str] = [
      "-m",
      "bc_mcp_proxy",
      "--TenantId",
      config.tenant_id,
      "--ClientId",
      config.client_id,
      "--Environment",
      config.environment,
      "--Company",
      config.company,
  ]
  if config.configuration_name:
    args.extend(["--ConfigurationName", config.configuration_name])

  base_config: Dict[str, Any] = {
      "type": "stdio",
      "command": executable,
      "args": args,
  }

  cursor_config = base_config.copy()
  vscode_config = base_config.copy()
  claude_config = {
      "command": executable,
      "args": args,
  }

  return cursor_config, vscode_config, claude_config


def generate_install_links(
    cursor_config: Dict[str, Any],
    vscode_config: Dict[str, Any],
) -> tuple[str, str]:
  """Generate quick-install URLs for Cursor and VS Code."""
  cursor_json = json.dumps(cursor_config, separators=(",", ":")).encode("utf-8")
  cursor_b64 = base64.urlsafe_b64encode(cursor_json).decode("utf-8")
  cursor_url = f"https://cursor.com/en-US/install-mcp?name=bc-mcp-proxy&config={cursor_b64}"

  vscode_json = json.dumps(vscode_config, separators=(",", ":"))
  vscode_urlencoded = quote(vscode_json)
  vscode_url = (
      "https://vscode.dev/redirect/mcp/install"
      f"?name=bc-mcp-proxy&config={vscode_urlencoded}"
  )

  return cursor_url, vscode_url


def write_client_configs(
    cursor_config: Dict[str, Any],
    vscode_config: Dict[str, Any],
    claude_config: Dict[str, Any],
) -> None:
  """Write generated configurations to disk for reference."""
  files = {
      "cursor_mcp.json": cursor_config,
      "vscode_mcp.json": vscode_config,
      "claude_mcp.json": claude_config,
  }
  for name, config in files.items():
    path = OUTPUT_DIR / name
    with path.open("w", encoding="utf-8") as stream:
      json.dump(config, stream, indent=2)
    print(f"Wrote {name} -> {path}")


def print_next_steps(config: ProxyConfig, cursor_url: str, vscode_url: str) -> None:
  """Display final instructions and helpful links."""
  args = [
      "--TenantId", config.tenant_id,
      "--ClientId", config.client_id,
      "--Environment", config.environment,
      "--Company", config.company,
  ]
  if config.configuration_name:
    args.extend(["--ConfigurationName", config.configuration_name])

  python_cmd = sys.executable or ("python3" if sys.platform != "win32" else "python")
  command_preview = " ".join([repr(python_cmd), "-m", "bc_mcp_proxy", *map(_shell_quote, args)])

  print("\nSetup complete! Next steps:")
  print("1) Add the MCP server to your preferred client:")
  print(f"   • Cursor: {cursor_url}")
  print(f"   • VS Code: {vscode_url}")
  print("   • Claude Desktop: see claude_mcp.json in your configuration folder.\n")

  print("2) Start the proxy whenever you want to use it:")
  print(f"   {command_preview}\n")

  print("3) Configuration files have been saved to:")
  print(f"   {OUTPUT_DIR}\n")

  python_cmd = "python3" if sys.platform != "win32" else "python"
  print(f"You can rerun '{python_cmd} -m bc_mcp_proxy setup' at any time to update these settings.")


def _shell_quote(value: str) -> str:
  return shlex.quote(value)

