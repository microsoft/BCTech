codeunit 66669 TestExceptions
{
    trigger OnRun()
    var
        anInt: Integer;
        frame: List of [Text];
    begin
        frameNumber := 1;
        frame.Add('OnRun');      
        Entry(frame);
    end;

    procedure Entry(var frame: List of [Text])
    var      
    begin
        frameNumber := frameNumber + 1;              
        frame.Add('Entry');
        CallMethod(frame);
    end;

    procedure CallMethod(var frame: List of [Text])
    var
        someVariable: decimal;
    begin
        frameNumber := frameNumber + 1;  
        frame.Add('CallMethod');       

        // Comment line and above an empty line
        someVariable := 1.0;

        CallBreakingMethod(frame);
    end;  
    
    procedure CallBreakingMethod(var frame: List of [Text])
    var
        aCurrency: Record Currency;
        ResovedSymbol, Symbol: Code[10];
        hufFound : Boolean;        
        counter: Integer;       
    begin
        Symbol := '';
        frame.Add('CallBreakingMethod');
        frameNumber := frameNumber + 1;
        aCurrency.Init();
        
        if (aCurrency.FindSet()) then begin
            repeat
                counter := counter + 1;               
                ResovedSymbol := aCurrency.ResolveGLCurrencySymbol(aCurrency.Code);    
                Sleep(100);
                if ((aCurrency.Code = 'HUF') or (aCurrency.Code = 'EUR')) then begin
                    Symbol := aCurrency.Code;
                    if (aCurrency.Code = 'HUF') then begin
                        hufFound := true;
                        break;
                    end;                
                end;  
                        
            until aCurrency.Next() = 0;
        end;
        
         Message('Found %1 currencies', counter);
         Sleep(100);

         if (Dialog.Confirm('Please comfirm this dialog')) then begin            
            // Snapshot debugging can be used to see if a line is hit without having variable information
            if (hufFound = false) then begin
                // This is an exception point. For snapshot debugging this line should always contain variable information.       
                Error('We are hitting an error. Something went wrong.')           
            end 
            else if Symbol <> '' then begin;               
                Symbol := aCurrency.ResolveGLCurrencySymbol(Symbol) ; 
                Message('Symbol %1 is', Symbol);
            end;       
         end;        
     
    end;

    [EventSubscriber(ObjectType::Table, 18, 'OnAfterDeleteEvent', '', false, false)]
    local procedure AnEvenaction()
    begin

    end;
    var
        frameNumber: Integer;
}
