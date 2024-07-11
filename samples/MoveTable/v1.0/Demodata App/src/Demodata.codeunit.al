#pragma warning disable AL0801
// Table 'Task Entry' is marked to be moved. Reason: moving to separate Task Tracker app.. Tag: 1.1.0.0.

codeunit 50200 Demodata
{
    Access = Internal;

    procedure CreateDemodata()
    var
        TaskEntry: Record "Task Entry";
    begin
        if not TaskEntry.IsEmpty() then
            exit;

        CreateEmployees();
        CreateExpectedCoffeeEnergyBoostEntries();
        CreateTasks();

        CreateCoffeeConsumptionEntries();
        CreateEmployeeEnergyLevelEntries();
        CreateTaskEntries();
    end;

    local procedure CreateEmployees()
    var
        Employee: Record "Employee";
    begin
        if not Employee.Get('EMP001') then begin
            Employee.Validate("No.", 'EMP001');
            Employee.Validate("First Name", 'John');
            Employee.Validate("Last Name", 'Doe');
            Employee.Validate("Job Title", 'Software Developer');
            Employee.Insert(true);
        end;

        if not Employee.Get('EMP002') then begin
            Employee.Validate("No.", 'EMP002');
            Employee.Validate("First Name", 'Jane');
            Employee.Validate("Last Name", 'Doe');
            Employee.Validate("Job Title", 'Software Developer');
            Employee.Insert(true);
        end;
    end;

    local procedure CreateExpectedCoffeeEnergyBoostEntries()
    var
        CoffeeEnergyBoost: Record "Exp. Coffee Energy Boost";
    begin
        if not CoffeeEnergyBoost.Get('EMP001') then begin
            CoffeeEnergyBoost.Validate("EmployeeNo.", 'EMP001');
            CoffeeEnergyBoost.Validate("Number of Cups Consumed", 1);
            CoffeeEnergyBoost.Validate("Exp. Energy Level Boost", 0);
            CoffeeEnergyBoost.Insert(true);
        end;

        if not CoffeeEnergyBoost.Get('EMP001') then begin
            CoffeeEnergyBoost.Validate("EmployeeNo.", 'EMP001');
            CoffeeEnergyBoost.Validate("Number of Cups Consumed", 2);
            CoffeeEnergyBoost.Validate("Exp. Energy Level Boost", 1);
            CoffeeEnergyBoost.Insert(true);
        end;

        if not CoffeeEnergyBoost.Get('EMP001') then begin
            CoffeeEnergyBoost.Validate("EmployeeNo.", 'EMP001');
            CoffeeEnergyBoost.Validate("Number of Cups Consumed", 3);
            CoffeeEnergyBoost.Validate("Exp. Energy Level Boost", 2);
            CoffeeEnergyBoost.Insert(true);
        end;

        if not CoffeeEnergyBoost.Get('EMP001') then begin
            CoffeeEnergyBoost.Validate("EmployeeNo.", 'EMP001');
            CoffeeEnergyBoost.Validate("Number of Cups Consumed", 4);
            CoffeeEnergyBoost.Validate("Exp. Energy Level Boost", 2);
            CoffeeEnergyBoost.Insert(true);
        end;

        if not CoffeeEnergyBoost.Get('EMP001') then begin
            CoffeeEnergyBoost.Validate("EmployeeNo.", 'EMP001');
            CoffeeEnergyBoost.Validate("Number of Cups Consumed", 5);
            CoffeeEnergyBoost.Validate("Exp. Energy Level Boost", 3);
            CoffeeEnergyBoost.Insert(true);
        end;

        if not CoffeeEnergyBoost.Get('EMP001') then begin
            CoffeeEnergyBoost.Validate("EmployeeNo.", 'EMP001');
            CoffeeEnergyBoost.Validate("Number of Cups Consumed", 6);
            CoffeeEnergyBoost.Validate("Exp. Energy Level Boost", 3);
            CoffeeEnergyBoost.Insert(true);
        end;

        if not CoffeeEnergyBoost.Get('EMP002') then begin
            CoffeeEnergyBoost.Validate("EmployeeNo.", 'EMP002');
            CoffeeEnergyBoost.Validate("Number of Cups Consumed", 0);
            CoffeeEnergyBoost.Validate("Exp. Energy Level Boost", -1);
            CoffeeEnergyBoost.Insert(true);
        end;

        if not CoffeeEnergyBoost.Get('EMP002') then begin
            CoffeeEnergyBoost.Validate("EmployeeNo.", 'EMP002');
            CoffeeEnergyBoost.Validate("Number of Cups Consumed", 1);
            CoffeeEnergyBoost.Validate("Exp. Energy Level Boost", 1);
            CoffeeEnergyBoost.Insert(true);
        end;
        if not CoffeeEnergyBoost.Get('EMP002') then begin
            CoffeeEnergyBoost.Validate("EmployeeNo.", 'EMP002');
            CoffeeEnergyBoost.Validate("Number of Cups Consumed", 2);
            CoffeeEnergyBoost.Validate("Exp. Energy Level Boost", 1);
            CoffeeEnergyBoost.Insert(true);
        end;
        if not CoffeeEnergyBoost.Get('EMP002') then begin
            CoffeeEnergyBoost.Validate("EmployeeNo.", 'EMP002');
            CoffeeEnergyBoost.Validate("Number of Cups Consumed", 3);
            CoffeeEnergyBoost.Validate("Exp. Energy Level Boost", 2);
            CoffeeEnergyBoost.Insert(true);
        end;
        if not CoffeeEnergyBoost.Get('EMP002') then begin
            CoffeeEnergyBoost.Validate("EmployeeNo.", 'EMP002');
            CoffeeEnergyBoost.Validate("Number of Cups Consumed", 4);
            CoffeeEnergyBoost.Validate("Exp. Energy Level Boost", 2);
            CoffeeEnergyBoost.Insert(true);
        end;
        if not CoffeeEnergyBoost.Get('EMP002') then begin
            CoffeeEnergyBoost.Validate("EmployeeNo.", 'EMP002');
            CoffeeEnergyBoost.Validate("Number of Cups Consumed", 5);
            CoffeeEnergyBoost.Validate("Exp. Energy Level Boost", 2);
            CoffeeEnergyBoost.Insert(true);
        end;
        if not CoffeeEnergyBoost.Get('EMP002') then begin
            CoffeeEnergyBoost.Validate("EmployeeNo.", 'EMP002');
            CoffeeEnergyBoost.Validate("Number of Cups Consumed", 6);
            CoffeeEnergyBoost.Validate("Exp. Energy Level Boost", 2);
            CoffeeEnergyBoost.Insert(true);
        end;
    end;

    local procedure CreateTasks()
    var
        Task: Record "Task";
    begin
        if not Task.Get('TASK001') then begin
            Task.Validate(TaskCode, 'TASK001');
            Task.Validate("Description", 'Write documentation');
            Task.Validate("Expected Duration", 020000T - 000000T);
            Task.Insert(true);
        end;

        if not Task.Get('TASK002') then begin
            Task.Validate(TaskCode, 'TASK002');
            Task.Validate("Description", 'Write code');
            Task.Validate("Expected Duration", 040000T - 000000T);
            Task.Insert(true);
        end;

        if not Task.Get('TASK003') then begin
            Task.Validate(TaskCode, 'TASK003');
            Task.Validate("Description", 'Test code');
            Task.Validate("Expected Duration", 030000T - 000000T);
            Task.Insert(true);
        end;
    end;

    local procedure CreateCoffeeConsumptionEntries()
    var
        CoffeeConsumptionEntry: Record "Coffee Consumption Entry";
    begin
        if not CoffeeConsumptionEntry.Get(1) then begin
            CoffeeConsumptionEntry."EntryNo." := 1;
            CoffeeConsumptionEntry.Validate("Employee No.", 'EMP001');
            CoffeeConsumptionEntry.Validate("Consumed Date", WorkDate());
            CoffeeConsumptionEntry.Validate("Time Of Day", "Time Of Day"::Morning);
            CoffeeConsumptionEntry.Validate("Number of Cups Consumed", 2);
            CoffeeConsumptionEntry.Insert(true);
        end;

        if not CoffeeConsumptionEntry.Get(2) then begin
            CoffeeConsumptionEntry."EntryNo." := 2;
            CoffeeConsumptionEntry.Validate("Employee No.", 'EMP001');
            CoffeeConsumptionEntry.Validate("Consumed Date", WorkDate());
            CoffeeConsumptionEntry.Validate("Time Of Day", "Time Of Day"::EarlyAfternoon);
            CoffeeConsumptionEntry.Validate("Number of Cups Consumed", 1);
            CoffeeConsumptionEntry.Insert(true);
        end;

        if not CoffeeConsumptionEntry.Get(3) then begin
            CoffeeConsumptionEntry."EntryNo." := 3;
            CoffeeConsumptionEntry.Validate("Employee No.", 'EMP002');
            CoffeeConsumptionEntry.Validate("Consumed Date", WorkDate());
            CoffeeConsumptionEntry.Validate("Time Of Day", "Time Of Day"::LateMorning);
            CoffeeConsumptionEntry.Validate("Number of Cups Consumed", 3);
            CoffeeConsumptionEntry.Insert(true);
        end;

        if not CoffeeConsumptionEntry.Get(4) then begin
            CoffeeConsumptionEntry."EntryNo." := 4;
            CoffeeConsumptionEntry.Validate("Employee No.", 'EMP002');
            CoffeeConsumptionEntry.Validate("Consumed Date", WorkDate());
            CoffeeConsumptionEntry.Validate("Time Of Day", "Time Of Day"::EarlyAfternoon);
            CoffeeConsumptionEntry.Validate("Number of Cups Consumed", 2);
            CoffeeConsumptionEntry.Insert(true);
        end;
    end;

    local procedure CreateEmployeeEnergyLevelEntries()
    var
        EmployeeEnergyLevelEntry: Record "Employee Energy Level Entry";
    begin
        if not EmployeeEnergyLevelEntry.Get(1) then begin
            EmployeeEnergyLevelEntry."EntryNo." := 1;
            EmployeeEnergyLevelEntry.Validate("Employee No.", 'EMP001');
            EmployeeEnergyLevelEntry.Validate("Registered Date", WorkDate());
            EmployeeEnergyLevelEntry.Validate("Time Of Day", "Time Of Day"::EarlyMorning);
            EmployeeEnergyLevelEntry.Validate("Energy Level", "Employee Energy Level"::Tired);
            EmployeeEnergyLevelEntry.Insert(true);
        end;

        if not EmployeeEnergyLevelEntry.Get(2) then begin
            EmployeeEnergyLevelEntry."EntryNo." := 2;
            EmployeeEnergyLevelEntry.Validate("Employee No.", 'EMP001');
            EmployeeEnergyLevelEntry.Validate("Registered Date", WorkDate());
            EmployeeEnergyLevelEntry.Validate("Time Of Day", "Time Of Day"::Morning);
            EmployeeEnergyLevelEntry.Validate("Energy Level", "Employee Energy Level"::Energized);
            EmployeeEnergyLevelEntry.Insert(true);
        end;

        if not EmployeeEnergyLevelEntry.Get(3) then begin
            EmployeeEnergyLevelEntry."EntryNo." := 3;
            EmployeeEnergyLevelEntry.Validate("Employee No.", 'EMP001');
            EmployeeEnergyLevelEntry.Validate("Registered Date", WorkDate());
            EmployeeEnergyLevelEntry.Validate("Time Of Day", "Time Of Day"::LateMorning);
            EmployeeEnergyLevelEntry.Validate("Energy Level", "Employee Energy Level"::Balanced);
            EmployeeEnergyLevelEntry.Insert(true);
        end;

        if not EmployeeEnergyLevelEntry.Get(4) then begin
            EmployeeEnergyLevelEntry."EntryNo." := 4;
            EmployeeEnergyLevelEntry.Validate("Employee No.", 'EMP001');
            EmployeeEnergyLevelEntry.Validate("Registered Date", WorkDate());
            EmployeeEnergyLevelEntry.Validate("Time Of Day", "Time Of Day"::LateAfternoon);
            EmployeeEnergyLevelEntry.Validate("Energy Level", "Employee Energy Level"::Exhausted);
            EmployeeEnergyLevelEntry.Insert(true);
        end;

        if not EmployeeEnergyLevelEntry.Get(5) then begin
            EmployeeEnergyLevelEntry."EntryNo." := 5;
            EmployeeEnergyLevelEntry.Validate("Employee No.", 'EMP002');
            EmployeeEnergyLevelEntry.Validate("Registered Date", WorkDate());
            EmployeeEnergyLevelEntry.Validate("Time Of Day", "Time Of Day"::EarlyMorning);
            EmployeeEnergyLevelEntry.Validate("Energy Level", "Employee Energy Level"::Balanced);
            EmployeeEnergyLevelEntry.Insert(true);
        end;

        if not EmployeeEnergyLevelEntry.Get(6) then begin
            EmployeeEnergyLevelEntry."EntryNo." := 6;
            EmployeeEnergyLevelEntry.Validate("Employee No.", 'EMP002');
            EmployeeEnergyLevelEntry.Validate("Registered Date", WorkDate());
            EmployeeEnergyLevelEntry.Validate("Time Of Day", "Time Of Day"::Morning);
            EmployeeEnergyLevelEntry.Validate("Energy Level", "Employee Energy Level"::Vibrant);
            EmployeeEnergyLevelEntry.Insert(true);
        end;

        if not EmployeeEnergyLevelEntry.Get(7) then begin
            EmployeeEnergyLevelEntry."EntryNo." := 7;
            EmployeeEnergyLevelEntry.Validate("Employee No.", 'EMP002');
            EmployeeEnergyLevelEntry.Validate("Registered Date", WorkDate());
            EmployeeEnergyLevelEntry.Validate("Time Of Day", "Time Of Day"::LateMorning);
            EmployeeEnergyLevelEntry.Validate("Energy Level", "Employee Energy Level"::Energized);
            EmployeeEnergyLevelEntry.Insert(true);
        end;

        if not EmployeeEnergyLevelEntry.Get(8) then begin
            EmployeeEnergyLevelEntry."EntryNo." := 8;
            EmployeeEnergyLevelEntry.Validate("Employee No.", 'EMP002');
            EmployeeEnergyLevelEntry.Validate("Registered Date", WorkDate());
            EmployeeEnergyLevelEntry.Validate("Time Of Day", "Time Of Day"::LateAfternoon);
            EmployeeEnergyLevelEntry.Validate("Energy Level", "Employee Energy Level"::Exhausted);
            EmployeeEnergyLevelEntry.Insert(true);
        end;

        if not EmployeeEnergyLevelEntry.Get(9) then begin
            EmployeeEnergyLevelEntry."EntryNo." := 9;
            EmployeeEnergyLevelEntry.Validate("Employee No.", 'EMP002');
            EmployeeEnergyLevelEntry.Validate("Registered Date", WorkDate());
            EmployeeEnergyLevelEntry.Validate("Time Of Day", "Time Of Day"::Evening);
            EmployeeEnergyLevelEntry.Validate("Energy Level", "Employee Energy Level"::Drained);
            EmployeeEnergyLevelEntry.Insert(true);
        end;
    end;

    local procedure CreateTaskEntries()
    var
        TaskEntry: Record "Task Entry";
    begin
        if not TaskEntry.Get(1) then begin
            TaskEntry."EntryNo." := 1;
            TaskEntry.Validate("Employee No.", 'EMP001');
            TaskEntry.Validate(TaskCode, 'TASK001');
            TaskEntry.Validate("Start Datetime", CreateDateTime(WorkDate(), 060000T));
            TaskEntry.Validate("End Datetime", CreateDateTime(WorkDate(), 090000T));
            TaskEntry.Insert(true);
        end;

        if not TaskEntry.Get(2) then begin
            TaskEntry."EntryNo." := 2;
            TaskEntry.Validate("Employee No.", 'EMP001');
            TaskEntry.Validate(TaskCode, 'TASK002');
            TaskEntry.Validate("Start Datetime", CreateDateTime(WorkDate(), 090000T));
            TaskEntry.Validate("End Datetime", CreateDateTime(WorkDate(), 130000T));
            TaskEntry.Insert(true);
        end;

        if not TaskEntry.Get(3) then begin
            TaskEntry."EntryNo." := 3;
            TaskEntry.Validate("Employee No.", 'EMP001');
            TaskEntry.Validate(TaskCode, 'TASK003');
            TaskEntry.Validate("Start Datetime", CreateDateTime(WorkDate(), 130000T));
            TaskEntry.Validate("End Datetime", CreateDateTime(WorkDate(), 153000T));
            TaskEntry.Insert(true);
        end;

        if not TaskEntry.Get(4) then begin
            TaskEntry."EntryNo." := 4;
            TaskEntry.Validate("Employee No.", 'EMP002');
            TaskEntry.Validate(TaskCode, 'TASK001');
            TaskEntry.Validate("Start Datetime", CreateDateTime(WorkDate() - 1, 060000T));
            TaskEntry.Validate("End Datetime", CreateDateTime(WorkDate() - 1, 080000T));
            TaskEntry.Insert(true);
        end;

        if not TaskEntry.Get(5) then begin
            TaskEntry."EntryNo." := 5;
            TaskEntry.Validate("Employee No.", 'EMP002');
            TaskEntry.Validate(TaskCode, 'TASK002');
            TaskEntry.Validate("Start Datetime", CreateDateTime(WorkDate() - 1, 080000T));
            TaskEntry.Validate("End Datetime", CreateDateTime(WorkDate() - 1, 130000T));
            TaskEntry.Insert(true);
        end;

        if not TaskEntry.Get(6) then begin
            TaskEntry."EntryNo." := 6;
            TaskEntry.Validate("Employee No.", 'EMP002');
            TaskEntry.Validate(TaskCode, 'TASK003');
            TaskEntry.Validate("Start Datetime", CreateDateTime(WorkDate() - 1, 130000T));
            TaskEntry.Validate("End Datetime", CreateDateTime(WorkDate() - 1, 144500T));
            TaskEntry.Insert(true);
        end;

        if not TaskEntry.Get(7) then begin
            TaskEntry."EntryNo." := 7;
            TaskEntry.Validate("Employee No.", 'EMP002');
            TaskEntry.Validate(TaskCode, 'TASK001');
            TaskEntry.Validate("Start Datetime", CreateDateTime(WorkDate(), 060000T));
            TaskEntry.Validate("End Datetime", CreateDateTime(WorkDate(), 080000T));
            TaskEntry.Insert(true);
        end;

        if not TaskEntry.Get(8) then begin
            TaskEntry."EntryNo." := 8;
            TaskEntry.Validate("Employee No.", 'EMP002');
            TaskEntry.Validate(TaskCode, 'TASK002');
            TaskEntry.Validate("Start Datetime", CreateDateTime(WorkDate(), 080000T));
            TaskEntry.Validate("End Datetime", CreateDateTime(WorkDate(), 100000T));
            TaskEntry.Insert(true);
        end;

        if not TaskEntry.Get(9) then begin
            TaskEntry."EntryNo." := 9;
            TaskEntry.Validate("Employee No.", 'EMP002');
            TaskEntry.Validate(TaskCode, 'TASK003');
            TaskEntry.Validate("Start Datetime", CreateDateTime(WorkDate(), 100000T));
            TaskEntry.Validate("End Datetime", CreateDateTime(WorkDate(), 120000T));
            TaskEntry.Insert(true);
        end;

        if not TaskEntry.Get(10) then begin
            TaskEntry."EntryNo." := 10;
            TaskEntry.Validate("Employee No.", 'EMP002');
            TaskEntry.Validate(TaskCode, 'TASK001');
            TaskEntry.Validate("Start Datetime", CreateDateTime(WorkDate(), 120000T));
            TaskEntry.Validate("End Datetime", CreateDateTime(WorkDate(), 140000T));
            TaskEntry.Insert(true);
        end;

        if not TaskEntry.Get(11) then begin
            TaskEntry."EntryNo." := 11;
            TaskEntry.Validate("Employee No.", 'EMP002');
            TaskEntry.Validate(TaskCode, 'TASK002');
            TaskEntry.Validate("Start Datetime", CreateDateTime(WorkDate(), 140000T));
            TaskEntry.Validate("End Datetime", CreateDateTime(WorkDate(), 160000T));
            TaskEntry.Insert(true);
        end;

        if not TaskEntry.Get(12) then begin
            TaskEntry."EntryNo." := 12;
            TaskEntry.Validate("Employee No.", 'EMP002');
            TaskEntry.Validate(TaskCode, 'TASK003');
            TaskEntry.Validate("Start Datetime", CreateDateTime(WorkDate(), 160000T));
            TaskEntry.Validate("End Datetime", CreateDateTime(WorkDate(), 180000T));
            TaskEntry.Insert(true);
        end;
    end;
}