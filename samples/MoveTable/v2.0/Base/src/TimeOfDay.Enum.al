enum 50100 "Time Of Day"
{
    Extensible = true;

    value(0; EarlyMorning)
    {
        // 'The time of day between 5:00 AM and 8:00 AM';
    }
    value(1; Morning)
    {
        // 'The time of day between 8:00 AM and 10:00 PM';
    }
    value(2; LateMorning)
    {
        // 'The time of day between 10:00 PM and 12:00 PM';
    }
    value(4; EarlyAfternoon)
    {
        //  'The time of day between 12:00 PM and 14:00 PM';
    }
    value(5; Afternoon)
    {
        //  'The time of day between 14:00 PM and 16:00 AM';
    }
    value(6; LateAfternoon)
    {
        //  'The time of day between 16:00 AM and 18:00 AM';
    }
    value(7; Evening)
    {
        //  'The time of day between 18:00 AM and 20:00 AM';
    }
    value(8; Night)
    {
        //  'The time of day between 20:00 AM and 23:00 PM';
    }
    value(9; LateNight)
    {
        //  'The time of day between 23:00 and 05:00';
    }
}