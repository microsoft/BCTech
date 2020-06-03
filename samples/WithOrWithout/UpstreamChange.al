tableextension 50130 UpstreamChange extends Customer
{
    // /// <summary> 
    // /// UPSTREAM CHANGE! Code compiles, but behavior is different 
    // /// Checks if the Customer Record is Dirty. The Customer Record is sometimes Dirty.
    // /// </summary> 
    // procedure IsDirty(): Boolean
    // begin
    //     exit(Random(10) > 5);
    // end;


    // /// <summary> 
    // /// UPSTREAM CHANGE! Code won't compile 
    // /// Checks if the Customer Record is Dirty. The Customer Record is sometimes Dirty.
    // /// </summary> 
    // procedure IsDirty(i: Integer): Boolean
    // begin
    //     exit(i > 20);
    // end;
}