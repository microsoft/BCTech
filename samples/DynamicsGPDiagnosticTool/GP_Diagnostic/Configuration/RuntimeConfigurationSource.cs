namespace Microsoft.GP.MigrationDiagnostic.Configuration;

using System;

public class RuntimeConfigurationSource<T> : IRuntimeConfigurationSource<T>
{
    private class RuntimeConfiguration(IRuntimeConfigurationSource<T> configurationSource) : IRuntimeConfiguration<T>
    {
        public event EventHandler? OnChange;

        public T CurrentValue => configurationSource.GetConfiguration();

        public void RaiseChange() => this.OnChange?.Invoke(this, EventArgs.Empty);
    }

    private RuntimeConfiguration option;
    private T configuration;

    public RuntimeConfigurationSource(T initialConfiguration)
    {
        this.option = new RuntimeConfiguration(this);
        this.configuration = initialConfiguration;
    }

    public IRuntimeConfiguration<T> Option => this.option;

    public void UpdateConfiguration(T gpConfiguration)
    {
        this.configuration = gpConfiguration;
        this.option.RaiseChange();
    }

    public T GetConfiguration() => this.configuration;
}
