namespace Microsoft.GP.MigrationDiagnostic.UI;

using System;
using System.ComponentModel;

/// <summary>
/// Used to allow the designer to properly draw the abstract <see cref="TaskGroupView>"/>
/// </summary>
/// <typeparam name="TAbstract"></typeparam>
/// <typeparam name="TBase"></typeparam>
public class AbstractControlDescriptionProvider<TAbstract, TBase> : TypeDescriptionProvider
{
    public AbstractControlDescriptionProvider()
        : base(TypeDescriptor.GetProvider(typeof(TAbstract)))
    {
    }

    public override Type GetReflectionType(Type objectType, object? instance)
    {
        if (objectType == typeof(TAbstract))
            return typeof(TBase);

        return base.GetReflectionType(objectType, instance);
    }

    public override object? CreateInstance(IServiceProvider? provider, Type objectType, Type[]? argTypes, object?[]? args)
    {
        if (objectType == typeof(TAbstract))
            objectType = typeof(TBase);

        return base.CreateInstance(provider, objectType, argTypes, args);
    }
}
