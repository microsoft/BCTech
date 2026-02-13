interface "IEscape Room Task Validation"
{
    procedure TaskCode(): Code[20];
    procedure GetValidationDescription(): Text;
    procedure ValidateTask(TaskCode: Code[20]): Boolean;
}