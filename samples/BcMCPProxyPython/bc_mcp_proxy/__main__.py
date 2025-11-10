from __future__ import annotations

import argparse
import logging
import os
import sys
from typing import Optional

from .config import ProxyConfig
from .proxy import run_sync
from .setup_flow import run_interactive_setup


def build_parser() -> argparse.ArgumentParser:
  parser = argparse.ArgumentParser(
      description="Business Central MCP proxy implemented in Python.")
  parser.add_argument("--TenantId", dest="tenant_id")
  parser.add_argument("--ClientId", dest="client_id")
  parser.add_argument("--Company", dest="company")
  parser.add_argument("--Environment", dest="environment")
  parser.add_argument("--ConfigurationName", dest="configuration_name")
  parser.add_argument("--CustomAuthHeader", dest="custom_auth_header")
  parser.add_argument("--BaseUrl", dest="base_url")
  parser.add_argument("--TokenScope", dest="token_scope")
  parser.add_argument("--ServerName", dest="server_name")
  parser.add_argument("--ServerVersion", dest="server_version")
  parser.add_argument("--Instructions", dest="instructions")
  parser.add_argument("--LogLevel", dest="log_level")
  parser.add_argument("--HttpTimeoutSeconds", type=float, dest="http_timeout_seconds")
  parser.add_argument("--SseTimeoutSeconds", type=float, dest="sse_timeout_seconds")
  parser.add_argument("--DeviceCacheLocation", dest="device_cache_location")
  parser.add_argument("--DeviceCacheName", dest="device_cache_name")
  parser.add_argument("--Debug", action="store_true", dest="enable_debug")
  return parser


def parse_args(argv: list[str] | None = None) -> ProxyConfig:
  parser = build_parser()
  args = parser.parse_args(argv)

  defaults = ProxyConfig()
  env = _config_from_env()

  return ProxyConfig(
      tenant_id=_select("tenant_id", args.tenant_id, env, defaults.tenant_id),
      client_id=_select("client_id", args.client_id, env, defaults.client_id),
      company=_select("company", args.company, env, defaults.company),
      environment=_select("environment", args.environment, env, defaults.environment),
      configuration_name=_select(
          "configuration_name", args.configuration_name, env, defaults.configuration_name),
      custom_auth_header=_select(
          "custom_auth_header", args.custom_auth_header, env, defaults.custom_auth_header),
      base_url=_select("base_url", args.base_url, env, defaults.base_url),
      token_scope=_select("token_scope", args.token_scope, env, defaults.token_scope),
      server_name=_select("server_name", args.server_name, env, defaults.server_name),
      server_version=_select("server_version", args.server_version, env, defaults.server_version),
      instructions=_select("instructions", args.instructions, env, defaults.instructions),
      http_timeout_seconds=_select_float(
          "http_timeout_seconds", args.http_timeout_seconds, env, defaults.http_timeout_seconds),
      sse_timeout_seconds=_select_float(
          "sse_timeout_seconds", args.sse_timeout_seconds, env, defaults.sse_timeout_seconds),
      device_cache_location=_select(
          "device_cache_location", args.device_cache_location, env, defaults.device_cache_location),
      device_cache_name=_select(
          "device_cache_name", args.device_cache_name, env, defaults.device_cache_name),
      log_level=_select("log_level", args.log_level, env, defaults.log_level).upper(),
      enable_debug=args.enable_debug or _env_flag("BC_DEBUG"),
  )


def main(argv: list[str] | None = None) -> None:
  if argv is None:
    argv = sys.argv[1:]

  if argv and argv[0].lower() == "setup":
    run_interactive_setup()
    return

  config = parse_args(argv)
  logging.basicConfig(level=getattr(logging, config.log_level.upper(), logging.INFO))
  run_sync(config)


def _config_from_env() -> dict[str, Optional[str]]:
  return {
      "tenant_id": os.getenv("BC_TENANT_ID"),
      "client_id": os.getenv("BC_CLIENT_ID"),
      "company": os.getenv("BC_COMPANY"),
      "environment": os.getenv("BC_ENVIRONMENT"),
      "configuration_name": os.getenv("BC_CONFIGURATION_NAME"),
      "custom_auth_header": os.getenv("BC_CUSTOM_AUTH_HEADER"),
      "base_url": os.getenv("BC_BASE_URL"),
      "token_scope": os.getenv("BC_TOKEN_SCOPE"),
      "server_name": os.getenv("BC_SERVER_NAME"),
      "server_version": os.getenv("BC_SERVER_VERSION"),
      "instructions": os.getenv("BC_INSTRUCTIONS"),
      "http_timeout_seconds": os.getenv("BC_HTTP_TIMEOUT_SECONDS"),
      "sse_timeout_seconds": os.getenv("BC_SSE_TIMEOUT_SECONDS"),
      "device_cache_location": os.getenv("BC_DEVICE_CACHE_LOCATION"),
      "device_cache_name": os.getenv("BC_DEVICE_CACHE_NAME"),
      "log_level": os.getenv("BC_LOG_LEVEL"),
  }


def _select(
    key: str,
    cli_value: Optional[str],
    env: dict[str, Optional[str]],
    default_value: Optional[str],
) -> Optional[str]:
  if cli_value is not None:
    return cli_value
  env_value = env.get(key)
  if env_value is not None:
    return env_value
  return default_value


def _select_float(
    key: str,
    cli_value: Optional[float],
    env: dict[str, Optional[str]],
    default_value: float,
) -> float:
  if cli_value is not None:
    return cli_value
  env_value = env.get(key)
  if env_value is not None:
    try:
      return float(env_value)
    except ValueError as exc:
      raise ValueError(f"Environment variable for {key} must be numeric.") from exc
  return default_value


def _env_flag(name: str) -> bool:
  value = os.getenv(name)
  if value is None:
    return False
  return value.strip().lower() in {"1", "true", "yes", "on"}


if __name__ == "__main__":
  main()

