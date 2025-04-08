namespace Microsoft.GP.MigrationDiagnostic.Configuration;

using System;

public interface IRuntimeConfiguration<T>
{
    event EventHandler? OnChange;

    T CurrentValue { get; }
}
