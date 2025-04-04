namespace Microsoft.GP.MigrationDiagnostic.Configuration;

public interface IRuntimeConfigurationSource<T>
{
    IRuntimeConfiguration<T> Option { get; }

    T GetConfiguration();
    void UpdateConfiguration(T gpConfiguration);
}
