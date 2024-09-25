namespace CopilotToolkitDemo.Common;

codeunit 54306 "Isolated Storage Wrapper"
{
    SingleInstance = true;
    Access = Internal;

    var
        IsolatedStorageSecretKeyKey: Label 'CopilotToolkitDemoSecret', Locked = true;
        IsolatedStorageDeploymentKey: Label 'CopilotToolkitDemoDeployment', Locked = true;
        IsolatedStorageEndpointKey: Label 'CopilotToolkitDemoEndpoint', Locked = true;

    procedure GetSecretKey() SecretKey: Text
    begin
        IsolatedStorage.Get(IsolatedStorageSecretKeyKey, SecretKey);
    end;

    procedure GetDeployment() Deployment: Text
    begin
        IsolatedStorage.Get(IsolatedStorageDeploymentKey, Deployment);
    end;

    procedure GetEndpoint() Endpoint: Text
    begin
        IsolatedStorage.Get(IsolatedStorageEndpointKey, Endpoint);
    end;

    procedure SetSecretKey(SecretKey: Text)
    begin
        IsolatedStorage.Set(IsolatedStorageSecretKeyKey, SecretKey);
    end;

    procedure SetDeployment(Deployment: Text)
    begin
        IsolatedStorage.Set(IsolatedStorageDeploymentKey, Deployment);
    end;

    procedure SetEndpoint(Endpoint: Text)
    begin
        IsolatedStorage.Set(IsolatedStorageEndpointKey, Endpoint);
    end;

}