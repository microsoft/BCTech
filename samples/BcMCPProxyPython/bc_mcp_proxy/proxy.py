from __future__ import annotations

import asyncio
import logging
from typing import Any
from urllib.parse import unquote

import httpx
from mcp.client.session import ClientSession
from mcp.client.streamable_http import streamablehttp_client
from mcp.server import Server
from mcp.server.stdio import stdio_server
from mcp.types import Implementation

from .auth import TokenProvider, create_token_provider
from .config import ProxyConfig


class _AsyncBearerAuth(httpx.Auth):
  """httpx authentication helper that fetches tokens on-demand."""

  def __init__(self, token_provider: TokenProvider) -> None:
    self._token_provider = token_provider

  async def async_auth_flow(self, request: httpx.Request) -> Any:
    token = await self._token_provider.get_token()
    request.headers["Authorization"] = f"Bearer {token}"
    yield request


async def run_proxy(config: ProxyConfig) -> None:
  """Run the stdio proxy until the MCP client disconnects."""
  logger = logging.getLogger("bc_mcp_proxy")
  if config.enable_debug:
    logger.setLevel(logging.DEBUG)

  token_provider = create_token_provider(config, logger=logger)

  headers = _build_transport_headers(config)
  url = _build_endpoint_url(config)

  logger.info("Connecting to Business Central MCP endpoint at %s", url)

  auth = _AsyncBearerAuth(token_provider)

  async with streamablehttp_client(
      url=url,
      headers=headers,
      timeout=config.http_timeout_seconds,
      sse_read_timeout=config.sse_timeout_seconds,
      auth=auth,
  ) as (remote_read, remote_write, get_session_id):
    client_info = Implementation(name=config.server_name, version=config.server_version)
    async with ClientSession(
        remote_read,
        remote_write,
        client_info=client_info,
    ) as remote_session:
      init_result = await remote_session.initialize()
      logger.debug("Connected to remote MCP server (protocol %s)", init_result.protocolVersion)

      instructions = config.instructions or (
          "Bridge MCP stdio clients to Microsoft Dynamics 365 Business Central."
          " All tool definitions and executions are forwarded to the configured Business"
          " Central environment.")
      server = Server(
          name=config.server_name,
          version=config.server_version,
          instructions=instructions,
      )

      @server.list_tools()
      async def _list_tools() -> Any:
        session_id = get_session_id() or "<pending>"
        logger.debug("Listing tools via remote MCP session %s", session_id)
        return await remote_session.list_tools()

      @server.call_tool()
      async def _call_tool(name: str, arguments: dict[str, Any]) -> Any:
        session_id = get_session_id() or "<pending>"
        logger.debug("Calling tool '%s' (session %s)", name, session_id)
        return await remote_session.call_tool(name, arguments or {})

      init_options = server.create_initialization_options()

      async with stdio_server() as (local_read, local_write):
        await server.run(local_read, local_write, init_options)


def _build_transport_headers(config: ProxyConfig) -> dict[str, str]:
  headers: dict[str, str] = {
      "X-Client-Application": config.server_name,
  }
  if config.company:
    headers["Company"] = unquote(config.company)
  if config.configuration_name:
    headers["ConfigurationName"] = unquote(config.configuration_name)
  return headers


def _build_endpoint_url(config: ProxyConfig) -> str:
  base = config.base_url.rstrip("/")
  return f"{base}/v2.0/{config.environment}/mcp"


def run_sync(config: ProxyConfig) -> None:
  """Helper to run the proxy from synchronous entry points."""
  asyncio.run(run_proxy(config))

