namespace CopilotToolkitDemo.ItemSubstitution;

codeunit 54306 "Isolated Storage Wrapper"
{
    SingleInstance = true;
    Access = Internal;

    var
        IsolatedStorageSecretKeyKey: Label 'CopilotToolkitDemoSecret', Locked = true;
        IsolatedStorageAccountNameKey: Label 'CopilotToolkitDemoAccountName', Locked = true;

    procedure GetSecretKey() SecretKey: Text
    begin
        IsolatedStorage.Get(IsolatedStorageSecretKeyKey, SecretKey);
    end;

    procedure GetAccountName() AccountName: Text
    begin
        IsolatedStorage.Get(IsolatedStorageAccountNameKey, AccountName);
    end;

    procedure SetSecretKey(SecretKey: Text)
    begin
        IsolatedStorage.Set(IsolatedStorageSecretKeyKey, SecretKey);
    end;

    procedure SetAccountName(AccountName: Text)
    begin
        IsolatedStorage.Set(IsolatedStorageAccountNameKey, AccountName);
    end;
}