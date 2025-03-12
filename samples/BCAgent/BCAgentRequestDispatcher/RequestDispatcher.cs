// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
// ------------------------------------------------------------------------------------------------

namespace Microsoft.Dynamics.BusinessCentral.Agent.RequestDispatcher
{
    using Azure.Relay;
    using Microsoft.Dynamics.BusinessCentral.Agent.Common;
    using System;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    public class RequestDispatcher
    {
        private readonly HybridConnectionListener listener;
        private readonly string listenerAddress;
        private readonly ImmutableDictionary<string, AgentPlugin> plugins;
        private readonly ILogger logger;

        /// <summary>
        /// Starts the request dispatcher.
        /// </summary>
        /// <param name="relayNamespace">The ServiceBus namespace that hosts this Relay. E.g. BCDemoAgent.servicebus.windows.net</param>
        /// <param name="hybridConnectionName"></param>
        /// <param name="keyName">The name of your Shared Access Policies key, which is RootManageSharedAccessKey by default</param>
        /// <param name="sharedAccessKey"></param>
        /// <param name="pluginFolder"></param>
        /// <param name="logger">Logger implementation</param>
        /// <param name="cancellationToken">Cancellation Tokan</param>
        public static async void Start(
            string relayNamespace,
            string hybridConnectionName,
            string keyName,
            string sharedAccessKey,
            string pluginFolder,
            ILogger logger,
            CancellationToken cancellationToken
            )
        {
            logger?.LogMessage(LogLevel.Verbose, $"Loading plugins from: {pluginFolder}");
            CompositionContext compositionContext = CompositionHelper.CreateCompositionHost(pluginFolder);
            ImmutableDictionary<string, AgentPlugin> plugins = compositionContext.TryGetExports<IAgentPlugin, AgentPluginMetadata>()
                .Select(exp => AgentPlugin.Create(exp.Metadata, exp.Value, logger))
                .Where(p => p!= null)
                .ToImmutableDictionary(p => p.RootPath);

            if(plugins.IsEmpty)
            {
                logger?.LogMessage(LogLevel.Error, $"No valid plugins. Exiting...");
                return;
            }

            HybridConnectionListener listener = CreateHybridConnectionListener(relayNamespace, hybridConnectionName, keyName, sharedAccessKey, logger);

            await new RequestDispatcher(listener, plugins, logger).RunAsync(cancellationToken);
        }

        private static HybridConnectionListener CreateHybridConnectionListener(string relayNamespace, string hybridConnectionName, string keyName, string sharedAccessKey, ILogger logger)
        {
            Uri address = new Uri($"sb://{relayNamespace}/{hybridConnectionName}");
            logger?.LogMessage(LogLevel.Verbose, $"Connection to: {address}");
            TokenProvider tokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(keyName, sharedAccessKey);
            return new HybridConnectionListener(address, tokenProvider);
        }


        private RequestDispatcher(HybridConnectionListener listener, ImmutableDictionary<string, AgentPlugin> plugins, ILogger logger)
        {
            this.listener = listener;
            this.plugins = plugins;
            this.logger = logger;
            this.listener.RequestHandler = RequestHandler;
            this.listenerAddress = listener.Address.AbsolutePath;
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            await this.listener.OpenAsync(cancellationToken);
        }

        private void RequestHandler(RelayedHttpListenerContext context)
        {
            try
            {
                this.logger?.LogMessage(LogLevel.Verbose, $"Request: {context.Request.HttpMethod} {context.Request.Url.AbsolutePath}");
                PluginMethod method = this.FindMethod(context.Request.Url.AbsolutePath, context.Request.HttpMethod);
                if(method == null)
                {
                    string message = $"Method not found. Uri = {context.Request.Url.AbsoluteUri}, Method = {context.Request.HttpMethod}";

                    this.logger?.LogMessage(LogLevel.Warning, message);

                    context.Response.StatusCode = HttpStatusCode.NotFound;
                    context.Response.StatusDescription = message;
                    return;
                }

                try
                {
                    this.logger?.LogMessage(LogLevel.Verbose, $"Execute: {method}");
                    method.Invoke(context);
                }
                catch (Exception e)
                {
                    context.Response.StatusCode = HttpStatusCode.InternalServerError;
                    this.logger?.LogException(e);
                }
            }
            finally
            {
                context.Response.Close();
            }
        }

        private PluginMethod FindMethod(string uri, string httpMethod)
        {
            Debug.Assert(uri.StartsWith(this.listenerAddress));

            if (uri.Length < this.listenerAddress.Length + 1)
            {
                return null;
            }

            string subPath = uri[(listenerAddress.Length + 1)..];
            int index = subPath.LastIndexOf('/');
            if (index < 0)
            {
                return null;
            }

            if (!this.plugins.TryGetValue(subPath[..index], out AgentPlugin plugin))
            {
                return null;
            }

            return plugin.FindMethod(httpMethod, subPath[(index + 1)..]);
        }
    }
}
