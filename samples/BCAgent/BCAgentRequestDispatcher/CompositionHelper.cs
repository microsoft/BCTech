// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
// ------------------------------------------------------------------------------------------------

namespace Microsoft.Dynamics.BusinessCentral.Agent.RequestDispatcher
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Composition.Hosting;
    using System.IO;
    using System.Reflection;

    public static class CompositionHelper
    {

        public static CompositionHost CreateCompositionHost(string pluginFolder)
        {
            HashSet<Assembly> assemblies = [];

            LoadPluginAssemblies(pluginFolder, assemblies);

            ContainerConfiguration compositionConfiguration =
                new ContainerConfiguration()
                    .WithAssemblies(assemblies);

            return compositionConfiguration.CreateContainer();
        }

        private static void LoadPluginAssemblies(string pluginFolder, HashSet<Assembly> builder)
        {
            ImmutableArray<string> assemblyFileNames = GetAssemblyPathsFromPluginFolder(pluginFolder);
            if (!assemblyFileNames.IsEmpty)
            {
                LoadExternalAssemblies(assemblyFileNames, builder);
            }
        }

        private static ImmutableArray<string> GetAssemblyPathsFromPluginFolder(string pluginFolder)
            => Directory.Exists(pluginFolder) ?
                [.. Directory.EnumerateFiles(pluginFolder, "*.dll", SearchOption.AllDirectories)] :
                [];

        private static void LoadExternalAssemblies(IEnumerable<string> analyzerFileNames, HashSet<Assembly> builder)
        {
            foreach (string assemblyFileName in analyzerFileNames)
            {
                if (File.Exists(assemblyFileName))
                {
                    Assembly assembly = TryLoadAnalyzerAssembly(assemblyFileName);
                    if (assembly != null)
                    {
                        builder.Add(assembly);
                    }
                }
            }
        }

        private static Assembly TryLoadAnalyzerAssembly(string assemblyFileName)
        {
            try
            {
                return Assembly.LoadFrom(assemblyFileName);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static IEnumerable<Lazy<TExtension, TMetadata>> TryGetExports<TExtension, TMetadata>(this CompositionContext compositionContext)
        {
            try
            {
                return compositionContext.GetExports<Lazy<TExtension, TMetadata>>();
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}