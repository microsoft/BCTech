codeunit 50230 ServiceBusQueueProcessing
{
    trigger OnRun()
    var
        JobQueueEntry: record "Job Queue Entry";
        AzureServiceBusQueue: codeunit AzureServiceBusQueue;
        Message: Text;
        LockToken: Text;
    begin
        if not AzureServiceBusQueue.IsEnabled() then begin
            JobQueueEntry.FindJobQueueEntry(JobQueueEntry."Object Type to Run"::Codeunit, Codeunit::ServiceBusQueueProcessing);
            JobQueueEntry.SetStatus(JobQueueEntry.Status::"On Hold");

            exit;
        end;

        while AzureServiceBusQueue.PeekLockMessage(Message, LockToken) do begin
            // Process message.
            ProcessMessage(Message);
            AzureServiceBusQueue.DeleteMessage(LockToken)
        end;
    end;

    local procedure ProcessMessage(Message: Text): Boolean;
    begin
        exit(true);
    end;
}
