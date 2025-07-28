Imports System.Math
Imports System.IO
Module Module1
    Public Function Exists(ByVal a As String) As Integer
        Dim e As Integer

        On Error GoTo BadExists
        Dim fInfo As New FileInfo(a)
        e = fInfo.Exists
        Exists = e
        Exit Function
BadExists:
        e = False
        Resume Next
    End Function
    Public Function itsanull(ByVal obj As Object) As String
        Dim a As String = ""
        If Not IsNothing(obj) Then
            a = obj.ToString
        End If
        itsanull = a
    End Function
    Public Function LastStr(ByVal st1 As String, ByVal st2 As String) As Integer
        Dim dumy, dd As String
        Dim x, b, a As Integer
        '
        ' This routine finds the last occurence of a string in a string
        '
        dumy = StrDup(Len(st2), Chr(1))
        If st2 = dumy Then dumy = StrDup(Len(st2), Chr(2))

        x = 1 : b = -1 : dd = st1 : a = 0
        While b <> 0
            b = InStr(dd, st2)
            If b <> 0 Then
                Mid(dd, b, Len(st2)) = dumy
                a = b
            End If
        End While
        LastStr = a
    End Function

    Function LastInStr(ByVal st As String, ByVal ins As String) As Integer
        '
        ' searches st$ for ins$ and returns the starting position
        ' of the last occurance of ins$.
        '
        '
        ' example:
        '                123456789012345678901234
        '        st$=   "XXXXXXXXXXXXabcXXXabcXXX"
        '       ins$=   "abc"
        '
        '  LastInStr=19
        '
        '
        Dim lins, lst, ix As Integer
        lins = Len(ins)
        lst = Len(st)
        If (InStr(st, ins) = 0) Or (lins > lst) Then
            LastInStr = 0
            Exit Function
        End If
        For ix = lst - lins + 1 To 1 Step -1
            If Mid(st, ix, lins) = ins Then
                Exit For
            End If
        Next
        LastInStr = ix
    End Function

    Public Sub Fcopy(ByVal Fromfile As String, ByVal TooFile As String, ByVal Rc As Integer)
        Dim g As Integer
        Dim h As String
        On Error GoTo FileError
        Rc = 0
        FileCopy(Fromfile, TooFile)
        Exit Sub

FileError:
        Select Case Err.Number  ' Evaluate error number.
            Case 53              ' "File not found" error.
                Rc = 2
            Case 55              ' "File already open" error.
                Rc = 1
            Case 75, 76          ' "Path Error
                Rc = 1
            Case 725, 730
                Rc = 5
            Case 731
                Rc = 3
            Case Else
                g = Err.Number
                h = Err.Description
                Rc = 1
        End Select
        Resume Next
        Return
    End Sub

    Public Sub PARSE(ByRef LineToParse As String, ByRef NextWord As String, ByVal Delimiter As String, ByRef ReturnCode As Integer)
        ' **---------------------------------------------------** '
        ' ** PARSE                                             ** '
        ' ** This routine Parses a String in search of the     ** '
        ' ** Delimiters Specified.                             ** '
        ' **                                                   ** '
        ' ** Delimiter$  Contains the Delimiters to look for.  ** '
        ' **                                                   ** '
        ' ** LineToParse$ Contains the String to be parsed.    ** '
        ' **                                                   ** '
        ' ** NextWord$   Returns the string parsed out of      ** '
        ' **             LineToParse$.                         ** '
        ' **                                                   ** '
        ' ** ReturnCode  Returns the status of the parse       ** '
        ' **             Operation.                            ** '
        ' **     Where:                                        ** '
        ' **                                                   ** '
        ' **    -1       NO Data in LineToParse$               ** '
        ' **                (NextWord$ is empty)               ** '
        ' **                                                   ** '
        ' **     0       NO DELIMITERS in LineToParse$         ** '
        ' **                                                   ** '
        ' **   >=1       Delimiter Found!!! This is which one  ** '
        ' **                of the delimiters was found.       ** '
        ' **---------------------------------------------------** '
        Dim l, start, i As Integer
        Dim k As String

        NextWord = ""
        ReturnCode = -1
        l = Len(LineToParse)

        If l = 0 Then GoTo ThruParse

        ReturnCode = 0
        start = 0
        For i = 1 To l
            k = Mid$(LineToParse, i, 1)
            If InStr(Delimiter, k) = 0 Then
                start = i
                Exit For
            Else
                ReturnCode = InStr(Delimiter, k)
            End If
        Next i
        If start = 0 Then
            ReturnCode = -1
            GoTo ThruParse
        End If
        For i = start To l
            k = Mid(LineToParse, i, 1)
            If InStr(Delimiter, k) <> 0 Then Exit For
        Next i
        NextWord = Mid(LineToParse, start, i - start)
        LineToParse = Mid(LineToParse, i)
ThruParse:
    End Sub

    Public Sub Parsenum(ByRef a As String, ByRef b As String)
        Dim rc As Integer
        Call PARSE(a, b, " ", rc)
        b = Trim(b)
        a = Trim(a)
    End Sub
    Public Function Center(ByVal st As String, ByVal l As Integer) As String
        Dim c As String
        Dim x As Integer
        st = RTrim(LTrim(st))
        If l > 0 Then
            x = l - Len(st)
            If x > 0 Then
                x = Int(x / 2)
                c = Left(Space(x) + st + Space(l), l)
            Else
                c = Left(st, l)
            End If
            Center = c
        Else
            Center = ""
        End If
    End Function
    Public Sub loadCull(ByVal arrray() As String, ByVal filename As String)
        Dim i As Integer
        '
        ' array must be dim'ed as arrray$(100)
        '     100 is maximum
        ' zero element contains # of entries in file
        '
        ' Dim arrray$(100)
        '
        ' used to cull keywords from a file in conjunction with Cull$
        '
        '  file has
        '         key1 = 14
        '         key2 = freddy boy
        '         keyword = funny
        '
        ' program has
        '
        '    loadcull(arrray(),filename)
        '    key2$=cull("key2","")
        '    amount= val(cull("key1","0")) Note: amount = object that needs the val
        '
        If Exists(filename) Then


            Dim fileReader As System.IO.StreamReader
            fileReader = My.Computer.FileSystem.OpenTextFileReader(filename)

            i = 1
            While Not fileReader.EndOfStream
                arrray(i) = fileReader.ReadLine
                i = i + 1
                If i > 100 Then
                    arrray(0) = "100"
                    Exit Sub
                End If
            End While
            fileReader.Close()
            arrray(0) = Trim(Str(i - 1))
        Else
            arrray(0) = "0"
        End If

    End Sub
    Public Sub LoadListbox(ByVal filename As String, ByVal lstToLoad As ListBox)
        lstToLoad.Items.Clear()
        If Exists(filename) Then
            Dim fileReader As System.IO.StreamReader
            fileReader = My.Computer.FileSystem.OpenTextFileReader(filename)
            While Not fileReader.EndOfStream
                lstToLoad.Items.Add(fileReader.ReadLine)
            End While
            fileReader.Close()
        End If

    End Sub
    Public Sub LoadTextbox(ByVal filename As String, ByVal txtBox As TextBox)
        Dim txt As String
        txtBox.Text = ""
        txt = ""
        If Exists(filename) Then
            Dim fileReader As System.IO.StreamReader
            fileReader = My.Computer.FileSystem.OpenTextFileReader(filename)
            While Not fileReader.EndOfStream
                txt = txt + fileReader.ReadLine + vbCrLf
            End While
            txt = RemoveEndingCrLf(txt)
            txtBox.Text = txt
            fileReader.Close()
        End If

    End Sub
    Public Sub saveTextBox(ByVal filename As String, ByVal txtBox As TextBox)
        Dim txt As String

        txt = Trim(txtBox.Text)
        If txt <> "" Then
            txt = RemoveEndingCrLf(txt)
            Dim fileWriter As System.IO.StreamWriter
            fileWriter = My.Computer.FileSystem.OpenTextFileWriter(filename, False)

            On Error Resume Next
            fileWriter.Write(txt)
            fileWriter.Close()
        End If
    End Sub
    Public Function RemoveEndingCrLf(strng As String) As String
        Dim txt As String
        Dim lentxt, lentxt1, lentxt2 As Integer
        txt = strng
        Do
            lentxt = Len(txt)
            lentxt1 = LastInStr(txt, vbCr)
            If lentxt1 = 0 Then
                lentxt1 = LastInStr(txt, vbLf)
            End If

            If lentxt1 = 0 Then
                Exit Do
            End If

            lentxt2 = lentxt - lentxt1
            If lentxt2 <= 1 Then
                txt = Left(txt, Len(txt) - 1)
            Else
                Exit Do
            End If
        Loop
        RemoveEndingCrLf = txt
    End Function



    Public Function DegToRad(ByVal Degs As Single) As Single
        Dim Pi, d, Deg2Rad As Single

        Pi = Atan(1) * 4
        Deg2Rad = Pi / 180

        d = Degs
        While d > 360
            d = d - (360)
        End While
        While d < -(360)
            d = d + (360)
        End While
        DegToRad = d * Deg2Rad
    End Function

    Public Function RadToDeg(ByVal Rad)
        Dim Pi, Rad2Deg As Single

        Pi = Atan(1) * 4
        Rad2Deg = 180 / Pi

        RadToDeg = Rad * Rad2Deg
    End Function

    Public Function QuickCrypt(ByVal mode As Integer, ByVal Strg As String) As String
        Dim a, b, c, v As Integer
        Dim xtrg As String

        xtrg = ""
        If mode = 0 Then  'Encrypt
            a = Len(Strg)
            b = a Mod 32
            For X = 1 To a
                c = X Mod 32
                v = Asc(Mid(Strg, X, 1))
                v = v + b + c
                xtrg = xtrg + Chr(v)
            Next
        Else              ' Decrypt
            a = Len(Strg)
            b = a Mod 32
            For X = 1 To a
                c = X Mod 32
                v = Asc(Mid(Strg, X, 1))
                v = (v - b) - c
                xtrg = xtrg + Chr(v)
            Next
        End If
        QuickCrypt = xtrg
    End Function

    Public Function Cull(ByVal arrray() As String, ByVal Keyword As String, ByVal DefaultV As String) As String
        '
        'Gets any file you request from File
        '
        ' array must be dim'ed as arrray$(100)
        '
        ' zero element contains # of entries in file
        '
        ' used to cull keywords from a file in conjunction with loadCull$
        '
        Dim inflen, i, pz As Integer
        Dim culled, p, edi As String

        inflen = arrray(0)
        culled = DefaultV
        For i = 1 To inflen
            p = arrray(i)
            edi = Left(p, 20) 'edi is Record in file you are looking for
            pz = InStr(UCase(edi), UCase(Keyword))
            If pz <> 0 Then
                p = Mid(p, pz + Len(Keyword))
                pz = InStr(p, "=")
                If pz <> 0 Then
                    p = Mid(p, pz + 1)
                End If
                'p = Left(p, Len(p) - 1) 'if you want to delete last string Left(P, Len(P) - 1)

                culled = p
                Exit For
            End If
        Next
        Cull = culled
    End Function
    Public Function KillIt(ByVal Its As String)
        On Error Resume Next
        Kill(Its)
        On Error GoTo 0
    End Function

    Public Function IRand(ByVal upperbound As Integer) As Integer
        '
        '   selects a randome number between
        '        1 and upperbound%
        '
        '   Int((upperbound - lowerbound + 1) * Rnd + lowerbound)
        '
        IRand = Int((upperbound) * Rnd() + 1)
    End Function

    Public Function RPad(ByVal st As String, ByVal l As Integer) As String
        RPad$ = Left(st + Space(l), l)
    End Function

    Public Function LPad(ByVal st As String, ByVal l As Integer) As String
        LPad = Right(Space(l) + st, l)
    End Function

    Public Function pathCheck(ByVal paths As String) As String
        Dim p As String
        p = paths
        If Right(p, 1) <> "\" Then
            p = p + "\"
        End If
        pathCheck = p
    End Function

    Public Function ReplaceStr(ByVal Source As String, ByVal Target As String, ByVal Replc As String) As String
        '
        'Replaces each instance of the string Target$ with the string Replc$ in the string Source$
        '
        Dim ns As String
        Dim FromPl, a As Integer
        ns = Source
        FromPl = 1
        While InStr(Mid(UCase(ns), FromPl), UCase(Target)) <> 0
            a = InStr(Mid(UCase(ns), FromPl), UCase$(Target))
            a = a + FromPl - 1
            ns = Left(ns, a - 1) + Replc + Mid(ns, a + Len(Target))
            FromPl = a + Len(Replc)
        End While
        ReplaceStr = ns
    End Function

    Public Function Running(ByVal tsks As String) As Integer
        Dim runs As Integer
        On Error GoTo BadTaskName
        runs = True
        AppActivate(tsks)
        Running = runs
        Exit Function
BadTaskName:
        runs = False
        Resume Next

    End Function
    Sub ScanCommandString(ByVal cmd As String, ByVal kw As String, ByVal Deflt As String, ByRef ans As String)
        '
        '   cmd$ = command line
        '
        '    kw$ = keyword (should be preceeded by a delimiter like:
        '
        '                               \file=Junk.dat or   #time=9:00
        '          and is required      -----               -----
        '
        '          equal sign is required but not contained in kw$
        '
        ' Deflt$ = default value for keyword
        '
        '   Ans$ = answer
        '
        '
        Dim c As Integer
        Dim dl As String
        c = InStr(cmd, kw)
        If c = 0 Then
            ans = Deflt
        Else
            ans = LTrim(Mid(cmd, c + Len(kw)))
            ans = LTrim(RTrim(Mid(ans, 2)))
            dl = Left(kw, 1)
            If InStr(ans, dl) <> 0 Then
                ans = RTrim(Left(ans, InStr(ans, dl) - 1))
            End If
        End If
    End Sub

    Public Function StrRemove(ByVal st As String, ByVal Rm As String) As String
        '
        '  This routine removes from the string st$ each character in rm$
        '
        Dim stx, rx As String
        Dim stp As Integer
        stx = st
        For ir = 1 To Len(Rm)
            rx = Mid(Rm, ir, 1)
            Do While (InStr(stx, rx) <> 0)
                stp = InStr(stx, rx)
                stx = Left(stx, stp - 1) + Mid(stx, stp + 1)
            Loop
        Next
        StrRemove = stx
    End Function
    Public Function removeBadAsc(ByVal st As String) As String
        '
        '  This routine removes from the string st$ each character in rm$
        '
        Dim stx, rx As String

        stx = st
        For ir = 1 To Len(st)
            rx = Mid(st, ir, 1)
            If (AscW(rx) > 127) Or (AscW(rx) < 20) Then rx = " "
            Mid(stx, ir, 1) = rx
        Next
        removeBadAsc = stx
    End Function

    Function UCaseFirst(ByVal nm As String) As String
        Dim uc As Integer
        Dim ix As String
        uc = 1
        For i = 1 To Len(nm)
            ix = Mid(nm, i, 1)
            If uc = 1 Then
                Mid(nm, i, 1) = UCase(Mid(nm, i, 1))
            Else
                Mid(nm, i, 1) = LCase(Mid(nm, i, 1))
            End If

            If InStr(" .-,/()&:;[]{}\|=+_*^%#!", ix) <> 0 Then
                uc = 1
            Else
                uc = 0
            End If
        Next
        UCaseFirst = nm
    End Function


    Public Function Howmany(ByVal Strg As String, ByVal Srch As String) As Integer
        '
        '   This routine searches Strg$ for the string Srch$
        '
        Dim h, hs As Integer
        Dim sv As String
        h = 0
        sv = Strg
        hs = InStr(sv, Srch)
        While hs <> 0
            h = h + 1
            sv = Mid(sv, hs + 1)
            hs = InStr(sv, Srch)
        End While
        Howmany = h

    End Function
    Public Sub DeCull(ByVal arrray() As String, ByVal Keyword As String, ByVal vl As String)
        '
        ' array must be dim'ed as arrray$(100)
        '     100 is maximum
        ' zero element contains # of entries in file
        '
        ' used to save the cullfile in conjunction with saveCull
        '
        '  this places the current values into the config file
        '
        Dim inflen, i, pz, fnd As Integer
        Dim p As String

        inflen = Val(arrray(0))
        fnd = False
        For i = 1 To inflen
            p = arrray(i)
            pz = InStr(UCase(p), UCase(Keyword))
            If pz <> 0 Then
                p = Mid(p, pz + Len(Keyword))
                pz = InStr(p, "=")
                If pz <> 0 Then
                    arrray(i) = Keyword + "=" + vl
                    fnd = True
                    Exit For
                End If
            End If
        Next
        If fnd = False Then
            arrray(i) = Keyword + "=" + vl
            arrray(0) = Trim(Str(i))
        End If
    End Sub
    Public Sub saveCull(ByVal arrray() As String, ByVal Filename As String)
        '
        ' array must be dim'ed as arrray(100)
        '     100 is maximum
        ' zero element contains # of entries in file
        '
        ' used to save the cullfile in conjunction with Cull
        '
        '  file has
        '         key1 = 14
        '         key2 = freddy boy
        '         keyword = funny
        '
        Dim fileWriter As System.IO.StreamWriter
        fileWriter = My.Computer.FileSystem.OpenTextFileWriter(Filename, False)

        On Error Resume Next
        If Val(arrray(0)) > 0 Then
            For i = 1 To Val(arrray(0))
                fileWriter.WriteLine(arrray(i))
            Next
            fileWriter.Close()
        End If
    End Sub
    Public Function BreakFileName(ByVal fln As String, ByVal Piece As Integer) As String
        '
        '
        '  Breaks file name apart
        '
        '        fln$   = "c:\My Documents\Recent\Test.doc"
        '        Piece% =
        '           0 = Drive
        '           1 = FullPath
        '           2 = Path (No Drive)
        '           3 = Full File Name
        '           4 = File name (no ext)
        '           5 = Ext
        '
        '           0 = drv$  = "c:"
        '           1 = pth$  = "c:\My Documents\Recent\"
        '           2 = fldr$ = "\My Documents\Recent\"
        '           3 = fil$  = "Test.doc"
        '           4 = Noext$= "Test"
        '           5 = ext$  = "doc"
        '
        '
        Dim pc As String
        Dim lsl, ldt, fcl, fsl As Integer
        lsl = LastStr(fln, "\")
        ldt = LastStr(fln, ".")
        fcl = InStr(fln, ":")
        fsl = InStr(fln, "\")

        pc = ""
        Select Case Piece
            Case 0
                If fcl > 0 Then
                    pc = Left(fln, fcl)
                End If
            Case 1
                If lsl > 0 Then
                    pc = Left(fln, lsl)
                End If
            Case 2
                If lsl > 0 Then
                    pc = Left(fln, lsl)
                    If fcl > 0 Then
                        pc = Mid(pc, fcl + 1)
                    End If
                End If
            Case 3
                If lsl > 0 Then
                    pc = Mid(fln, lsl + 1)
                Else
                    If fcl > 0 Then
                        pc = Mid(fln, fcl + 1)
                    End If
                End If
            Case 4
                pc = fln
                If lsl > 0 Then
                    pc = Mid(fln, lsl + 1)
                    ldt = LastStr(pc, ".")
                    If ldt > 0 Then
                        pc = Left(pc, ldt - 1)
                    End If
                Else
                    If fcl > 0 Then
                        pc = Mid(fln, fcl + 1)
                    End If
                    ldt = LastStr(pc, ".")
                    If ldt > 0 Then
                        pc = Left(pc, ldt - 1)
                    End If
                End If
            Case 5
                If ldt > 0 Then
                    pc = Mid(fln, ldt + 1)
                End If
            Case Else
        End Select
        BreakFileName = pc
    End Function

    Public Function Colmn(ByVal ix As Integer) As String
        Dim iix, cx As Integer
        Dim c As String
        iix = ix
        c = ""
        cx = 0
        While iix > 26
            cx = cx + 1
            iix = iix - 26
        End While
        If cx > 0 Then
            c = Chr(Asc("A") + cx - 1)
        End If
        c = c + Chr(Asc("A") + iix - 1)
        Colmn = c
    End Function
End Module

