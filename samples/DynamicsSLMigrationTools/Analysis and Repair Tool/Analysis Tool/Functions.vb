Imports System.Data.SqlClient



Module Functions
    Public IsWindowsAuth As Boolean
    Public SQLAuthUser As String = ""
    Public SQLAuthPwd As String = ""
    Public SQLServerName As String = ""
    Public SysDBName As String = ""
    Public CpnyId As String = ""
    Public UserId As String = ""
    Public SQLCommand As System.Data.SqlClient.SqlCommand
    Public SqlSysDbConn As System.Data.SqlClient.SqlConnection
    Public SqlAppDbConn As System.Data.SqlClient.SqlConnection
    Public sqlMsgConn As SqlConnection = Nothing

    Public AppDbConnStr As String = String.Empty

    Public CpnyDBList As List(Of CpnyDatabase)
    Public AppDBName As String
    Public statusExists As Boolean = False

    Public EventLogDir As String = ""
    Public DBName As String = String.Empty

    Public MinDateValue As Date = "01/01/1900"

    Public DBL_EPSILON As Double = 0.00000000000000022204460492503131

    Public Enum OperationType As Integer
        ReadOp = 1
        UpdateOp = 2
        InsertOp = 3
        DeleteOp = 4
        ExecProc = 5
    End Enum

    Public LTRUE As Short = 1



    Friend LoginSucceeded As Boolean
    Friend Master_pwd As String
    Public Class CpnyDatabase
        Private m_Active As Short
        Private m_CompanyId As String
        Private m_CompanyName As String
        Private m_DatabaseName As String

        Public Property Active() As String

            Get
                Return m_Active
            End Get

            Set(ByVal setval As String)
                m_Active = setval
            End Set

        End Property
        Public Property CompanyId() As String

            Get
                Return m_CompanyId
            End Get

            Set(ByVal setval As String)
                m_CompanyId = setval
            End Set

        End Property
        Public Property CompanyName() As String

            Get
                Return m_CompanyName
            End Get

            Set(ByVal setval As String)
                m_CompanyName = setval
            End Set

        End Property
        Public Property DatabaseName() As String

            Get
                Return m_DatabaseName
            End Get

            Set(ByVal setval As String)
                m_DatabaseName = setval
            End Set

        End Property
    End Class

    Public Class ParmList
        Private m_ParmName As String
        Private m_ParmType As SqlDbType
        Private m_ParmValue As String

        Public Property ParmName() As String

            Get
                Return m_ParmName
            End Get

            Set(ByVal setval As String)
                m_ParmName = setval
            End Set

        End Property
        Public Property ParmType() As SqlDbType
            Get
                Return m_ParmType
            End Get
            Set(value As SqlDbType)
                m_ParmType = value
            End Set
        End Property

        Public Property ParmValue() As String

            Get
                Return m_ParmValue
            End Get

            Set(ByVal setval As String)
                m_ParmValue = setval
            End Set

        End Property
    End Class


    Public Function GetConnectionString(SysDB As Boolean) As String
        Dim pwd As String = ""


        If IsWindowsAuthenticationDatabase() = False Then
            UserId = SQLAuthUser
            pwd = SQLAuthPwd.Trim()
        End If

        Dim sqlConnStr As New System.Data.SqlClient.SqlConnectionStringBuilder()
        sqlConnStr.DataSource = SQLServerName

        If (SysDB = True) Then
            sqlConnStr.InitialCatalog = SysDBName.Trim()
        Else

            ' Get the App Database name from the list.
            sqlConnStr.InitialCatalog = AppDBName.Trim()

        End If

        If (IsWindowsAuth = True) Then
            sqlConnStr.IntegratedSecurity = True
        Else
            sqlConnStr.UserID = UserId.Trim
            sqlConnStr.Password = pwd.Trim
            sqlConnStr.IntegratedSecurity = False
        End If


        Return sqlConnStr.ToString()
    End Function

    Public Function sql_1(ByRef sqlReader As SqlDataReader, cmdText As String, DbConn As SqlConnection, OpType As OperationType, CmdType As CommandType, Optional ParmListArgs As List(Of ParmList) = Nothing) As Integer
        Dim SqlConn As SqlConnection
        Dim retVal As Integer

        SQLCommand = New SqlClient.SqlCommand()
        SQLCommand.Connection = DbConn

        SqlConn = DbConn

        Try


            ' If the connection opened successfully, then get the list of companies.
            If (SqlConn.State = ConnectionState.Closed) Then
                Call SqlConn.Open()
            End If
            SQLCommand.CommandText = cmdText
            SQLCommand.CommandType = CmdType

            Select Case OpType
                Case OperationType.ReadOp
                    sqlReader = SQLCommand.ExecuteReader()
                    Return sqlReader.HasRows
                Case OperationType.UpdateOp, OperationType.InsertOp, OperationType.DeleteOp, OperationType.ExecProc
                    If (ParmListArgs IsNot Nothing) Then
                        For Each parm As ParmList In ParmListArgs
                            SQLCommand.Parameters.Add(parm.ParmName, parm.ParmType)
                            SQLCommand.Parameters.Item(parm.ParmName).Value = parm.ParmValue
                        Next
                    End If

                    ' Execute the update or insert statement.
                    retVal = SQLCommand.ExecuteNonQuery()

                    Return retVal



            End Select


        Catch ex As Exception
            MsgBox("Error: Unable to execute statement - " & ex.Message)
        End Try



    End Function

    Public Function sql_1(ByRef sqlReader As SqlDataReader, cmdText As String, DbConn As SqlConnection, OpType As OperationType, CmdType As CommandType, sqlTran As SqlTransaction, Optional ParmListArgs As List(Of ParmList) = Nothing) As Integer
        Dim SqlConn As SqlConnection
        Dim retVal As Integer

        SQLCommand = New SqlClient.SqlCommand()
        SQLCommand.Connection = DbConn
        SQLCommand.Transaction = sqlTran

        SqlConn = DbConn

        Try


            ' If the connection opened successfully, then get the list of companies.
            If (SqlConn.State = ConnectionState.Closed) Then
                Call SqlConn.Open()
            End If
            SQLCommand.CommandText = cmdText
            SQLCommand.CommandType = CmdType

            Select Case OpType
                Case OperationType.ReadOp
                    sqlReader = SQLCommand.ExecuteReader()
                    Return sqlReader.HasRows
                Case OperationType.UpdateOp, OperationType.InsertOp, OperationType.DeleteOp, OperationType.ExecProc
                    If (ParmListArgs IsNot Nothing) Then
                        For Each parm As ParmList In ParmListArgs
                            SQLCommand.Parameters.Add(parm.ParmName, parm.ParmType)
                            SQLCommand.Parameters.Item(parm.ParmName).Value = parm.ParmValue
                        Next
                    End If

                    ' Execute the update or insert statement.
                    retVal = SQLCommand.ExecuteNonQuery()

                    Return retVal



            End Select


        Catch ex As Exception
            MsgBox("Error: Unable to execute statement - " & ex.Message)
        End Try



    End Function

    Public Sub sqlFetch_1(ByRef sqlReader As SqlDataReader, cmdText As String, SqlConn As SqlConnection, cmdType As CommandType, Optional ParmListArgs As List(Of ParmList) = Nothing)
        SQLCommand = New SqlClient.SqlCommand()


        SQLCommand.Connection = SqlConn

        ' If the connection opened successfully, then get the list of companies.
        If (SqlConn.State = ConnectionState.Closed) Then
            SqlConn.Open()
        End If

        SQLCommand.CommandText = cmdText
        SQLCommand.CommandType = cmdType

        Try

            If (ParmListArgs IsNot Nothing) Then
                For Each parm As ParmList In ParmListArgs
                    SQLCommand.Parameters.Add(parm.ParmName, parm.ParmType)
                    SQLCommand.Parameters.Item(parm.ParmName).Value = parm.ParmValue
                Next
            End If

            sqlReader = SQLCommand.ExecuteReader()
        Catch ex As Exception
            MsgBox("Error: Unable to execute statement - " & ex.Message)
        End Try

    End Sub
    Public Sub sqlFetch_1(ByRef sqlReader As SqlDataReader, cmdText As String, SqlConn As SqlConnection, cmdType As CommandType, sqlTran As SqlTransaction, Optional ParmListArgs As List(Of ParmList) = Nothing)
        SQLCommand = New SqlClient.SqlCommand()


        SQLCommand.Connection = SqlConn

        ' If the connection opened successfully, then get the list of companies.
        If (SqlConn.State = ConnectionState.Closed) Then
            SqlConn.Open()
        End If

        SQLCommand.CommandText = cmdText
        SQLCommand.CommandType = cmdType
        SQLCommand.Transaction = sqlTran

        Try

            If (ParmListArgs IsNot Nothing) Then
                For Each parm As ParmList In ParmListArgs
                    SQLCommand.Parameters.Add(parm.ParmName, parm.ParmType)
                    SQLCommand.Parameters.Item(parm.ParmName).Value = parm.ParmValue
                Next
            End If

            sqlReader = SQLCommand.ExecuteReader()
        Catch ex As Exception
            MsgBox("Error: Unable to execute statement - " & ex.Message)
        End Try

    End Sub

    Public Sub sqlFetch_Num(ByRef numVal As Integer, cmdText As String, SqlConn As SqlConnection)
        Dim retObj As Object

        SQLCommand = New SqlClient.SqlCommand()

        SQLCommand.Connection = SqlConn

        ' If the connection opened successfully, then get the list of companies.
        If (SqlConn.State = ConnectionState.Closed) Then
            SqlConn.Open()
        End If

        SQLCommand.CommandText = cmdText
        Try
            retObj = SQLCommand.ExecuteScalar()
            If (TypeOf (retObj) IsNot System.DBNull) Then
                numVal = CInt(retObj)
            Else
                numVal = 0
            End If
        Catch ex As Exception
            MsgBox("Error: Unable to execute statement - " & ex.Message)
        End Try

    End Sub

    Public Sub sqlFetch_Num(ByRef numVal As Short, cmdText As String, SqlConn As SqlConnection)
        Dim retObj As Object

        SQLCommand = New SqlClient.SqlCommand()

        SQLCommand.Connection = SqlConn

        ' If the connection opened successfully, then get the list of companies.
        If (SqlConn.State = ConnectionState.Closed) Then
            SqlConn.Open()
        End If

        SQLCommand.CommandText = cmdText
        Try
            retObj = SQLCommand.ExecuteScalar()
            If (TypeOf (retObj) IsNot System.DBNull) Then
                numVal = CShort(retObj)
            Else
                numVal = 0
            End If
        Catch ex As Exception
            MsgBox("Error: Unable to execute statement - " & ex.Message)
        End Try

    End Sub

    Public Sub sqlFetch_Num(ByRef numVal As Double, cmdText As String, SqlConn As SqlConnection)
        Dim retObj As Object

        SQLCommand = New SqlClient.SqlCommand()


        SQLCommand.Connection = SqlConn

        ' If the connection opened successfully, then get the list of companies.
        If (SqlConn.State = ConnectionState.Closed) Then
            SqlConn.Open()
        End If

        SQLCommand.CommandText = cmdText
        Try
            retObj = SQLCommand.ExecuteScalar()
            If (TypeOf (retObj) IsNot System.DBNull) Then
                numVal = Convert.ToDouble(retObj)
            Else
                numVal = 0
            End If
        Catch ex As Exception
            MsgBox("Error: Unable to execute statement - " & ex.Message)
        End Try

    End Sub
    Public Sub sqlFetch_Num(ByRef dateVal As Date, cmdText As String, SqlConn As SqlConnection)
        Dim retObj As Object

        SQLCommand = New SqlClient.SqlCommand()


        SQLCommand.Connection = SqlConn

        ' If the connection opened successfully, then get the list of companies.
        If (SqlConn.State = ConnectionState.Closed) Then
            SqlConn.Open()
        End If

        SQLCommand.CommandText = cmdText
        Try
            retObj = SQLCommand.ExecuteScalar()
            If (TypeOf (retObj) IsNot System.DBNull) Then
                dateVal = Convert.ToDateTime(retObj)
            Else
                dateVal = MinDateValue
            End If
        Catch ex As Exception
            MsgBox("Error: Unable to execute statement - " & ex.Message)
        End Try

    End Sub
    Public Function IsWindowsAuthenticationDatabase() As Boolean

        Return IsWindowsAuth

    End Function






    Public Function SParm(parmVal As String) As String
        parmVal = String.Concat(" '", parmVal.Trim, "' ")
        Return parmVal
    End Function

    Public Function TranBeg(sqlConn As SqlConnection) As SqlTransaction

        Dim sqlTran As SqlTransaction

        sqlTran = sqlConn.BeginTransaction()

        Return sqlTran

    End Function



    Public Sub TranEnd(dbTran As SqlTransaction)

        dbTran.Commit()

    End Sub



    Public Function TranStatus(sqlTran As SqlTransaction) As ConnectionState

        Return sqlTran.Connection.State

    End Function

    Public Function FPAdd(arg1 As Double, arg2 As Double, precision As Short) As Double
        Return FPRnd(arg1 + arg2, precision)
    End Function

    Public Function FPSub(arg1 As Double, arg2 As Double, precision As Short) As Double
        Return FPRnd(arg1 - arg2, precision)
    End Function

    Public Function FPRnd(inpVal As Double, precision As UShort) As Double

        Dim ErrMsgNbr As Short
        Dim retval As Double
        Dim flag As Short

        '      Use this to check for exceeding FP limits 
        '       Static double upper_limit_array[] = {
        Dim Upper_Limit_Array() As Double =
                  {999999999999999, 99999999999999.9, 9999999999999.99,
                  999999999999.999, 99999999999.9999, 9999999999.99999,
                  999999999.999999, 99999999.9999999, 9999999.99999999,
                  999999.999999999, 99999.9999999999, 9999.99999999999,
                  999.999999999999, 99.9999999999999, 9.99999999999999,
                  0.999999999999999}
        Dim MaxRoundingPrecision As Short = 9

        '       /* we'll get a performance improvement if we keep these */
        '       /* values static; if precision does Not change, why     */
        '       /* keep re-calculating these values?                    */
        Dim upper_limit As Double
        Dim sPrecision As UShort = 65535
        Dim mval As ULong = 1L
        Dim mval_array() As ULong =
                 {1L, 10L, 100L, 1000L, 10000L, 100000L, 1000000L,
                   10000000L, 100000000L, 1000000000L}
        Dim modDbl As Double
        Dim remDbl As Double



        ' A precision value of -1 tells us (as in 16-bit), that we should
        'just returned the value passed in.  ADD desired this functionality.]
        If (precision <> -1) Then

            '	/* get multiplier value */
            If (precision <> sPrecision) Then
                If (precision > MaxRoundingPrecision) Then
                    sPrecision = MaxRoundingPrecision
                Else
                    sPrecision = precision
                End If
                mval = mval_array(sPrecision)
                upper_limit = Upper_Limit_Array(sPrecision)
            End If


            '	/* see if it Is greater than value we are rounding */
            If (dabs(inpVal, flag) > upper_limit) Then
                '		mess(MSG_FLOAT_TOO_BIG);
                Return (upper_limit)
            End If

            '	/* find remainder And integer portion in order to   */
            '	/* get all significant digits to right of decimal;  */
            '	/* then check to see if we need to round up         */
            '	/* (use .4995 in case remainder Is Not quite .5)     */

            ' Get the remainder value of the input.
            modDbl = (inpVal * mval) Mod 1

            ' Get the integer part of the input.
            remDbl = Math.Truncate(inpVal * mval)

            If (dabs(modDbl, flag) >= 0.4995) Then
                ' If the number is positive, then round up.
                If (flag = 1) Then
                    remDbl = remDbl + (1.0 + Double.Epsilon)
                Else
                    ' Input number was negative
                    remDbl = remDbl - (1 + Double.Epsilon)
                End If

                ' Remove any remainder and get the integer value.
                remDbl = remDbl \ 1

            End If



            '	/* now divide integer portion, moving digits back   */
            '	/* to left of decimal, And return it                */
            '	retval = xdbl / mval; 
            retval = remDbl / mval

            '	Pass our result to the RoundToLimit routine.  Our values must
            '		match, because the 95.50 Database Physical Integrity will be
            '           Using RoundToLimit().
            '       */
            retval = RoundToLimit(retval, ErrMsgNbr)
            '	 If we've done our arithmetic right, won't be any error below 

            Return (retval)

        Else
            Return inpVal
        End If



    End Function

    Function RoundToLimit(inpVal As Double, ByRef retErr As Short) As Double
        '        *
        '	Round the floating point value which has been passed to us 
        '	to the limits of floating point accuracy (15 digits).  This Is Not 
        '	a rounding operation In the traditional sense, in that the
        '	value displayed To the user Is Not changed.  Rather, we are
        '    simply truncating the "16 digit garbage" from the end of the 
        '    InputValue.This "garbage" will prevent 2 numbers from comparing
        '    equal, even though they look Like the same number when displayed.

        '  Returns: Rounded Number(to the 15th significant digit).

        '
        Dim retval As Double   ' Equivalent to the absolute value of the input value.
        Dim AbsInputValue As Double  ' Equivalent to the absolute value of InputValue. 

        Dim LeftDigitCount As Integer        ' Number of digits in InputValue to the Left of the decimal point
        Dim RequestedTotalDecimalDigits As Integer ' The number Of requested Decimal digits when converting the input
        ' Number to ascii.  This might be greater then
        ' 15 digits because the number's significant
        ' digits might Not start till the 2nd Or 3rd
        ' Decimal place. */
        Dim PositiveFlag As Short
        Dim i As Short
        Dim flag As Short = 0
        '		/* Defining this array because the C library pow() function isn't
        '            accurate, And this Is faster anyway */
        Dim ScaleArray() As Double = {
            1.0E+30, 1.0E+29, 1.0E+28, 1.0E+27, 1.0E+26,
            1.0E+25, 1.0E+24, 1.0E+23, 1.0E+22, 1.0E+21,
            1.0E+20, 1.0E+19, 1.0E+18, 1.0E+17, 1.0E+16,
            1.0E+15, 100000000000000.0, 10000000000000.0, 1000000000000.0, 100000000000.0,
            10000000000.0, 1000000000.0, 100000000.0, 10000000.0, 1000000.0,
            100000.0, 10000.0, 1000.0, 100.0, 10.0,
            1.0, 0.1, 0.01, 0.001, 0.0001, 0.00001,
            0.000001, 0.0000001, 0.00000001, 0.000000001, 0.0000000001,
            0.00000000001, 0.000000000001, 0.0000000000001, 0.00000000000001, 0.000000000000001
        }
        Dim Power14Entry As Integer = 0 ' Index into the ScaleArray For the value=1E15 entry
        Dim Power0Entry As Integer = 0   ' Index into the ScaleArray For the value=1 entry
        Dim ScaleArrayCount As Integer = 45
        ' ScaleArrayCount = (sizeof(ScaleArray) / sizeof(ScaleArray[0]))
        Dim MaxSignificantDigits As Short = 15
        Dim errMsg As String = ""
        Dim dblValStr As String = ""

        '	*pErrorDiscovered = 0;  

        '	/* If our static array index values haven't been initialized yet,
        '		Do so now.
        '	*/
        If (Power0Entry = 0) Then
            i = 0

            For i = 0 To ScaleArrayCount

                If (ScaleArray(i) = 100000000000000.0) Then
                    Power14Entry = i
                ElseIf (ScaleArray(i) = 1.0) Then
                    Power0Entry = i
                End If
            Next i
        End If


        AbsInputValue = dabs(inpVal, flag)
        PositiveFlag = flag

        '	/* Check for a number that Is too large.  1E15 Is a 1 followed by 15 zeros.
        '        This Is a number With 16 digits, And we only support 15 digits.
        '	*/
        If (AbsInputValue >= 1.0E+15) Then
            retErr = 118  ' Overflow In floating point, number too large.
        End If

        '	/* Look though the "Power of 10" array, And find out how many digits
        '		to the left of the decimal place this number has.
        '	*/
        For i = Power14Entry To ScaleArrayCount

            If (AbsInputValue >= ScaleArray(i)) Then
                Exit For
            End If
        Next i

        If (i >= ScaleArrayCount) Then
            Return (0.0) ' If the Input value Is less Then 1E-15, Then we define its value as 0 
        End If

        LeftDigitCount = MaxSignificantDigits - (i - Power14Entry)

        ' If the InputValue has no digits to the left of the decimal point,
        '		adjust our LeftDigitCount value And algorithm to accomodate the 
        '        quirks of our ScaleArray.
        '	
        If (i > Power0Entry) Then

            LeftDigitCount = LeftDigitCount - 1

            RequestedTotalDecimalDigits = MaxSignificantDigits - 1 - LeftDigitCount
        Else

            RequestedTotalDecimalDigits = MaxSignificantDigits - LeftDigitCount
        End If


        '	Convert the input value to ascii And then back again.  We tried
        '		the more elegant modf() call, but at the limits of decimal
        '		precision, it didn't always work.  Ex. With 15 significant digits
        '		to the left of the decimal point, the modf would do it's best,
        '		but couldn't clear up the 16th place much (1st decimal digit).
        '		When we then scaled the number back to it's original size,
        '		that 16 digit remainder would cause the number Not to compare
        '        equal to a number freshly converted from ascii.
        '	
        Call ConvertFloatDetectError(inpVal, RequestedTotalDecimalDigits,
                                    dblValStr, errMsg)


        retval = Convert.ToDouble(dblValStr)
        Return retval

    End Function

    ' Convert a floating point value to an edited string,
    Public Sub ConvertFloatDetectError(dval As Double,              ' Float,ing point value 
         desired_dec_places As Integer, ' Number of decimal places to be output 
        ByRef outputbuff As String,            ' Return value - string equivalent of dval is copied here 
        ByRef ErrorMsg As String)   '  Return value 


        Dim pstrg As String         ' Raw ascii conversion of the floating point value 
        Dim pstrgVal As String
        Dim i As Integer = 0
        Dim dec As Integer
        Dim sign As Integer
        Dim nbrFormat As System.Globalization.NumberFormatInfo = New Globalization.NumberFormatInfo

        ErrorMsg = ""

        '	 The caller may have specified a rounding precision that is invalid.
        '	Since floating point values can only be accurate up to 15 digits, a value such
        '	as 1,000,000.00 cannot have a rounding precision greater than 8.  If the caller
        '	specified 9, the result from ConvertFloatDetectError may not converted correctly.

        '	So get the max rounding precision for the "dval" specified, and if invalid
        '	overwrite "desired_dec_places".

        Dim MaxRndPrecision As Short
        If (ChkMaxDecimalPrecision(dval, MaxRndPrecision) = 0) Then

            If (MaxRndPrecision < desired_dec_places) Then

                desired_dec_places = MaxRndPrecision

            End If
        End If


        '     Our databases occationally have extremely small numbers in them,
        '        which (combinded with data import transfers) this should help fix.

        If (Math.Abs(dval) < 0.000000000000001) Then
            dval = 0
        End If

        '    If the absolute value of dval is smaller than 1 and 
        '    desired_dec_places equal to 0 _fcvt() will return a empty string 
        '     this is not what we wanted.
        If (desired_dec_places = 0 And dval < 1 And dval > -1) Then

            outputbuff = "0"
            Return
        End If

        ' Convert the floating point number to a string.
        pstrg = dval.ToString()

        If (pstrg.Substring(0, 1) = "-1") Then
            sign = -1
        End If

        If pstrg.Contains(".") Then
            pstrgVal = New String(pstrg.Substring(0, pstrg.IndexOf(".")))

            pstrgVal = String.Concat(pstrgVal, pstrg.Substring(pstrg.IndexOf(".") + 1))
            dec = pstrg.IndexOf(".") + 1
        Else
            'If there is no decimal point in the value, then the "assumed" position is the string length.
            pstrgVal = New String(pstrg)

            dec = pstrg.Length + 1

        End If

        ' Pad the value to the right to accommodate the total number of required decimal places.  This would be the 
        '       required digits minus the number of digits after the decimal place.
        Dim PadLength As Integer
        PadLength = desired_dec_places - (pstrg.Length - dec)
        If (PadLength > 0) Then
            pstrgVal = pstrgVal.PadRight(PadLength + pstrg.Length, "0"c)
        End If





        ' Determine the position of the decimal point.  If there are more than 16 digits to the left, then return an error.
        If (dec > 15) Then
            ErrorMsg = " Floating point conversion produced invalid value, decimal location has too many digits to the left."
        End If

        If (sign = -1) Then
            outputbuff = sign.ToString().Trim()
        Else outputbuff = String.Empty
        End If


        If (dec > 0) Then

            ' Copy over the string to the output buffer.
            outputbuff = String.Concat(outputbuff, pstrgVal.Substring(0, dec - 1) + ".")
            If (pstrgVal.Length >= dec - 1) Then
                outputbuff = String.Concat(outputbuff, pstrgVal.Substring(dec - 1).Trim())
            End If
            '    '        strcpy( &pbuff[ i], pstrg);
            '    '        i += dec;
            '    '        pbuff[ i++] = '.';
            '    '        strcpy( &pbuff[ i], &pstrg[ dec]);

        Else
            outputbuff = String.Concat(outputbuff, "." + pstrgVal.Trim())

            '    '        pbuff[ i++] = '.';
            '    '        for( ; dec < 0; dec++)
            '    '            pbuff[ i++] = '0';
            '    '        strcpy( &pbuff[ i], pstrg);
        End If

    End Sub

    Public Function ChkMaxDecimalPrecision(
    dval As Double,
     ByRef p_MaxRndPrecision As Short) As Short


        '	 a value of 0 cannot really be rounded to anything to return -1
        If (dval = 0.0) Then

            Return -1
        End If

        Dim AbsInputValue As Double         ' Equivalent to the absolute value of InputValue. 
        Dim LeftDigitCount As Integer         ' Number of digits in InputValue to the left of the decimal point
        Dim PositiveFlag As Short
        Dim i As Short
        Dim flag As Short = 0


        Static ScaleArray() As Double = {
                    1.0E+30, 1.0E+29, 1.0E+28, 1.0E+27, 1.0E+26,
                    1.0E+25, 1.0E+24, 1.0E+23, 1.0E+22, 1.0E+21,
                    1.0E+20, 1.0E+19, 1.0E+18, 1.0E+17, 1.0E+16,
                    1.0E+15, 100000000000000.0, 10000000000000.0, 1000000000000.0, 100000000000.0,
                    10000000000.0, 1000000000.0, 100000000.0, 10000000.0, 1000000.0,
                    100000.0, 10000.0, 1000.0, 100.0, 10.0,
                    1.0, 0.1, 0.01, 0.001, 0.0001, 0.00001,
                    0.000001, 0.0000001, 0.00000001, 0.000000001, 0.0000000001,
                    0.00000000001, 0.000000000001, 0.0000000000001, 0.00000000000001, 0.000000000000001
              }
        Dim Power14Entry As Integer = 0   ' Index into the ScaleArray for the value=1E15 entry
        Dim Power0Entry As Integer = 0  ' Index into the ScaleArray for the value=1 entry
        Dim ScaleArrayCount As Integer = 45
        Dim MaxSignificantDigits As Integer = 15

        '	 If our static array index values haven't been initialized yet,
        '		do so now.

        If (Power0Entry = 0) Then

            For i = 0 To ScaleArrayCount - 1

                If (ScaleArray(i) = 100000000000000.0) Then
                    Power14Entry = i
                ElseIf (ScaleArray(i) = 1.0) Then
                    Power0Entry = i

                End If

            Next i
        End If

        AbsInputValue = dabs(dval, flag)
        PositiveFlag = flag

        '	Check for a number that is too large.  1E15 is a 1 followed by 15 zeros.
        '        This is a number with 16 digits, and we only support 15 digits.
        '	
        If (AbsInputValue >= 1.0E+15) Then
            Return -1
        End If

        '	Look though the "Power of 10" array, and find out how many digits
        '		to the left of the decimal place this number has.
        '	
        For i = Power14Entry To ScaleArrayCount - 1

            If (AbsInputValue >= ScaleArray(i)) Then
                Exit For               '		
            End If
        Next i

        '	Check for a number that is too small.  Input value is less then 1E-15.
        '    This is a number with 16 digits, and we only support 15 digits.
        '	
        If (i >= ScaleArrayCount) Then
            Return -1
        End If

        LeftDigitCount = MaxSignificantDigits - (i - Power14Entry)

        '	If the p_dVal has no digits to the left of the decimal point,
        '		adjust our LeftDigitCount value and algorithm to accomodate the 
        '		quirks of our ScaleArray.
        If (i > Power0Entry) Then

            LeftDigitCount = LeftDigitCount - 1
            p_MaxRndPrecision = MaxSignificantDigits - 1 - LeftDigitCount

        Else
            p_MaxRndPrecision = MaxSignificantDigits - LeftDigitCount
        End If

        Return 0

    End Function ' end ChkMaxDecimalPrecision

    '  Identifies whether the original input value was begative.
    Function dabs(inpval As Double, ByRef flag As Short) As Double


        If (inpval < 0.0) Then

            inpval = -inpval
            flag = 0

        Else
            flag = 1

        End If


        Return inpval

    End Function

    Public Function FToA(inpVal As Double, precision As Short) As String
        Dim retVal As String = String.Empty
        Dim DeclPl As Short
        Dim dblVal As Double
        Dim errMsg As String = ""
        Dim charBuf As String = ""

        dblVal = inpVal

        ' Currently this only supports base currency.  This function may be expanded to handle multi currency.
        If (precision = BaseCuryPrec) Then
            DeclPl = precision
        Else
            DeclPl = 2    ' Default value
        End If

        '/* perform conversion And return result; convert to international
        'since typically this function Is used by application when displaying
        'an amount In a message, And this should be in international format
        '*/
        Call ConvertFloatDetectError(dblVal, precision, charBuf, errMsg)

        ' Check the absolute value - if smaller than the allowed value, then set to zero.
        If (Math.Abs(dblVal) < 0.000000000000001) Then
            dblVal = 0.0

        End If


        ' Convert the value to a string with the defined precision.
        Convert.ToString(dblVal)


        Return String.Format(Convert.ToString(dblVal), "{0:DecPl}")


    End Function


    Public Sub AddStatusInfo(strClause As String, strDescr As String, strResult As String)


        Dim dbTran As SqlTransaction = Nothing

        Dim sqlMsgReader As SqlDataReader = Nothing
        Dim retVal As Integer

        If (sqlMsgConn Is Nothing) Then
            sqlMsgConn = New SqlClient.SqlConnection(AppDbConnStr)
        End If


        sqlMsgConn.Open()
        dbTran = TranBeg(sqlMsgConn)

        ' Insert the record into the xslAnalysisSum table
        retVal = sql_1(sqlMsgReader, strClause, sqlMsgConn, OperationType.InsertOp, CommandType.Text, dbTran)
        Call TranEnd(dbTran)

        sqlMsgConn.Close()

        ' Write the information to the status report.
        Call oEventLog.LogMessage(0, String.Format("       {0} {1}", strDescr, strResult, vbNewLine))


    End Sub

    Public Function PeriodPlusPerNum(CurrPer As String, NumVal As Integer) As String
        ' The period string is in the format YYYYPP.  Separate the period and the year.
        Dim strperNbr As String
        Dim nbrPerNbr As Short
        Dim strYear As String
        Dim nbrYear As Short

        strperNbr = CurrPer.Trim.Substring(4)
        strYear = CurrPer.Trim.Substring(0, 4)

        ' Convert the period and year to numeric.
        Try
            nbrPerNbr = Convert.ToInt16(strperNbr)
            nbrYear = Convert.ToInt16(strYear)
        Catch ex As Exception
            nbrPerNbr = 0
        End Try

        If (NumVal < 0) Then
            For Cnt As Short = NumVal To -1
                nbrPerNbr = nbrPerNbr - 1
                If (nbrPerNbr = 0) Then
                    nbrPerNbr = bGLSetupInfo.NbrPer
                    nbrYear = nbrYear - 1
                End If
            Next Cnt
        ElseIf (NumVal > 0) Then
            For Cnt As Short = 1 To NumVal
                nbrPerNbr = nbrPerNbr + 1
                If (nbrPerNbr > bGLSetupInfo.NbrPer) Then
                    nbrPerNbr = 1
                    nbrYear = nbrYear + 1
                End If
            Next Cnt

        End If


        ' Convert back to string and return.  If the period is less than 2 digits, then left pad with a leading zero.
        strperNbr = nbrPerNbr.ToString()
        If (strperNbr.Length < 2) Then
            strperNbr = String.Concat("0", strperNbr)
        End If
        Return (String.Concat(nbrYear.ToString(), strperNbr))
    End Function

End Module
