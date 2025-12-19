"""
Business Central MCP proxy package.

This package exposes the entrypoint for the Python-based proxy that forwards MCP
requests from stdio clients to the official Business Central MCP HTTP endpoint.
"""

from .config import ProxyConfig  # noqa: F401
from .proxy import run_proxy  # noqa: F401

__all__ = ["ProxyConfig", "run_proxy", "__version__"]
__version__ = "0.1.0"


