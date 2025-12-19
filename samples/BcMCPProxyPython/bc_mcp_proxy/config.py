from __future__ import annotations

from dataclasses import dataclass
from typing import Optional


@dataclass(slots=True)
class ProxyConfig:
  """Configuration values required to run the Business Central MCP proxy."""

  server_name: str = "BcMCPProxyPython"
  server_version: str = "0.1.0"
  instructions: Optional[str] = None

  tenant_id: Optional[str] = None
  client_id: Optional[str] = None
  token_scope: str = "https://api.businesscentral.dynamics.com/.default"
  base_url: str = "https://api.businesscentral.dynamics.com"
  environment: str = "Production"
  company: Optional[str] = None
  configuration_name: Optional[str] = None

  custom_auth_header: Optional[str] = None
  http_timeout_seconds: float = 30.0
  sse_timeout_seconds: float = 300.0

  device_cache_name: str = "bc_mcp_proxy"
  device_cache_location: Optional[str] = None

  log_level: str = "INFO"
  enable_debug: bool = False


