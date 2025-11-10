from __future__ import annotations

from dataclasses import dataclass
import asyncio
import logging
import os
from pathlib import Path
import sys
from typing import Callable, Optional, Protocol

import msal
from msal_extensions import FilePersistence, PersistedTokenCache

from .config import ProxyConfig


class TokenProvider(Protocol):
  async def get_token(self) -> str:
    """Return a fresh OAuth token for Business Central requests."""


@dataclass
class StaticTokenProvider:
  """Simple token provider that always returns the same bearer token."""

  token: str

  async def get_token(self) -> str:
    return self.token


DeviceFlowCallback = Callable[[dict[str, str]], None]


class MsalDeviceCodeTokenProvider(TokenProvider):
  """Token provider that acquires tokens via the MSAL device code flow."""

  def __init__(
      self,
      tenant_id: str,
      client_id: str,
      scopes: list[str],
      cache_path: Path,
      logger: Optional[logging.Logger] = None,
      device_flow_callback: Optional[DeviceFlowCallback] = None,
  ) -> None:
    if not tenant_id:
      raise ValueError("Tenant ID is required for device code authentication.")
    if not client_id:
      raise ValueError("Client ID is required for device code authentication.")
    if not scopes:
      raise ValueError("At least one scope must be supplied.")

    cache_path.parent.mkdir(parents=True, exist_ok=True)

    self._logger = logger or logging.getLogger(__name__)
    self._scopes = scopes
    self._cache = PersistedTokenCache(FilePersistence(str(cache_path)))
    authority = f"https://login.microsoftonline.com/{tenant_id}"
    self._app = msal.PublicClientApplication(
        client_id=client_id,
        authority=authority,
        token_cache=self._cache,
    )
    self._lock = asyncio.Lock()
    self._flow_callback = device_flow_callback or self._default_flow_callback

  async def get_token(self) -> str:
    async with self._lock:
      return await asyncio.to_thread(self._acquire_token)

  def _acquire_token(self) -> str:
    accounts = self._app.get_accounts() or []
    for account in accounts:
      result = self._app.acquire_token_silent(self._scopes, account=account)
      token = self._extract_token(result)
      if token:
        self._logger.debug("Using cached token for account %s", account.get("username"))
        return token

    flow = self._app.initiate_device_flow(scopes=self._scopes)
    if "user_code" not in flow:
      message = flow.get("error_description") or "Unable to initiate device code flow."
      raise RuntimeError(message)

    self._flow_callback(flow)

    result = self._app.acquire_token_by_device_flow(flow)
    token = self._extract_token(result)
    if not token:
      message = result.get("error_description") or str(result)
      raise RuntimeError(f"Authentication failed: {message}")

    return token

  def _extract_token(self, result: Optional[dict[str, str]]) -> Optional[str]:
    if not result:
      return None
    if "access_token" in result:
      return result["access_token"]
    return None

  def _default_flow_callback(self, flow: dict[str, str]) -> None:
    message = flow.get(
        "message",
        f"To sign in, use code {flow.get('user_code')} at {flow.get('verification_uri')}.")
    self._logger.warning(message)
    print(message, flush=True)


def create_token_provider(
    config: ProxyConfig,
    logger: Optional[logging.Logger] = None,
) -> TokenProvider:
  """Create an appropriate token provider based on the configuration."""
  if config.custom_auth_header:
    return StaticTokenProvider(token=config.custom_auth_header)

  scopes = [config.token_scope]
  cache_path = _resolve_cache_path(config)
  return MsalDeviceCodeTokenProvider(
      tenant_id=_require_value(config.tenant_id, "TenantId"),
      client_id=_require_value(config.client_id, "ClientId"),
      scopes=scopes,
      cache_path=cache_path,
      logger=logger,
  )


def _resolve_cache_path(config: ProxyConfig) -> Path:
  if config.device_cache_location:
    base = Path(config.device_cache_location).expanduser()
  else:
    base = _default_cache_dir()

  filename = config.device_cache_name
  if not filename.endswith(".bin"):
    filename = f"{filename}.bin"
  return base / filename


def _default_cache_dir() -> Path:
  if sys.platform.startswith("win"):
    root = Path(os.environ.get("LOCALAPPDATA") or Path.home() / "AppData/Local")
  elif sys.platform == "darwin":
    root = Path.home() / "Library/Caches"
  else:
    root = Path(os.environ.get("XDG_CACHE_HOME", Path.home() / ".cache"))
  return root / "BcMCPProxyPython"


def _require_value(value: Optional[str], name: str) -> str:
  if value:
    return value
  raise ValueError(f"{name} is required when device code authentication is used.")

