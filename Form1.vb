Imports System.Runtime.InteropServices.JavaScript.JSType
Imports System.Xml
Imports System.Xml.XPath
Public Class frmMain
    Public G, M As Graphics
    Public MyPen As New Pen(Color.Black, 1)
    Public MyClr As New Pen(Color.White, 1)
    Public MyRed As New Pen(Color.Red, 1)
    Public bgColor As Color
    Public xPos, yPos As Integer
    Public angl, Speed, TOD, OutsideTemp, LowTemp, HighTemp As Single
    Public cntlfnd As Boolean = True
    Public DaysfromStart As Integer
    Public droidData(30, 16) As Single '(Droid number,Parameter)
    Public Mapping As Boolean
    Public dr As DroidStruct
    Public CurrTI As Single
    '
    'Each Program is stored as a string in the format:
    '
    '                 Descr|Node|Node|Node|Node
    '
    Public Const NoOfDroids = 30

    Const noOfPix = 100

    'Public Droid(NoOfDroids, 30) As Single
    'Public Info(NoOfDroids, 10) As String
    Public status(NoOfDroids) As String
    Public Prg(NoOfDroids, 2, 30) As Single
    Public FailureFrequency As Integer

    Public Const TimeOfTick = 0.002
    Public Const LoTemp = 50
    Public Const HiTemp = 130
    Public Const heatval = (HiTemp - LoTemp) / 8
    Public Const coolval = (HiTemp - LoTemp) / 12
    Public Const DroidHV = 0.3  '1.2
    Public Const DroidAC = -0.3 ' -0.75
    Public Const DroidEnvDrain = 0.001
    Public Const DroidMovingDrain = 0.005
    Public Const DroidCharge = 0.4


    Public Const Warn_Attn = 78
    Public Const Warn_Low = 50
    Public Const Warn_Crit = 25

    '                     
    '  Droid! constants
    '
    '
    ' Public Const D_X = 1  '   1 = X position
    ' Public Const D_Y = 2  '   2 = Y position
    ' Public Const D_Dir = 3  '   3 = Travel Direction
    ' Public Const D_Vel = 4  '   4 = Travel Velocity
    ' Public Const D_TurrDir = 5  '   5 = Turret Direction
    ' Public Const D_Batt = 6  '   6 = Battery Power
    ' Public Const D_ETemp = 7  '   7 = Ext skin temp
    ' Public Const D_ITemp = 8  '   8 = internal temp
    ' Public Const D_EnvSet = 9  '   9 = Environ setting
    ' Public Const D_Speed = 10 '  10 = Speed setting (0-7)
    ' Public Const D_AutoNav = 11 '  11 = Auto Navigate Switch
    ' Public Const D_Color = 12 '  12 = Droid Color
    ' Public Const D_BaseX = 13 '  13 = Base X Position
    ' Public Const D_BaseY = 14 '  14 = Base Y Position
    ' Public Const D_Prgm = 15 '  15 = Currently executing Program
    ' Public Const D_PC = 16 '  16 = Current Program Counter
    ' Public Const D_Connect = 17 '  17 = Connected to a station
    ' Public Const D_PrdCap = 18 '  18 = Product Capacity
    ' Public Const D_Enabled = 19 '  19 = Droid enable/disable switch
    ' Public Const D_Tow = 20 '  20 = Droid is towing another Droid (droid in tow)
    ' Public Const D_DestX = 21 ' Droid Destination X-Coord
    ' Public Const D_DestY = 22 ' Droid Destination Y-Coord
    '
    ' Info (Droid Number, Piece of Info)
    '
    '                     Piece                         example
    '                       0 = garage Assignment          1
    '                       1 = Destination Name           WP6
    '                       2 = Executing Program          |MF4|HOME
    '                       3 = Original Program           WP2|SP1|WP2|WP6|MF4|HOME
    '                       4 = Towing droid Name          None    or    Droid 4
    '
    'Public Const I_Garage = 1 ' Droid Garage Assignment
    'Public Const I_Dest = 2   ' Droid Destination Name
    'Public Const I_PGM = 3    ' Executing Pgm
    'Public Const I_OGpgm = 4  ' Original Program
    'Public Const I_Tow = 5    ' Tow Droid Name
    Public droidlist As New List(Of DroidStruct)
    Public Function DegtoRad(Degg As Integer) As Single
        DegtoRad = Degg * (Math.PI / 180)
    End Function
    Public Function Findcntl(ByVal objName As String, ByVal objIndx As Integer, ByVal root As Control, ByRef fnd As Boolean) As Object
        Dim panels(100) As String
        Dim panelcnt As Integer = 0
        Dim pnlstr As String = ""

        Dim oxj As Object
        pnlstr = objName + Trim(Str(objIndx))
        oxj = Me.Controls.Find(pnlstr, True)
        fnd = False
        For Each cntrl As Control In root.Controls
            If cntrl.Name = pnlstr Then
                oxj = cntrl
                fnd = True
                Exit For
            End If
        Next

        Findcntl = oxj
    End Function
    Public Function RotateImage(ByRef image As Image, ByVal offset As PointF, ByVal angleDeg As Decimal) As Bitmap
        If image Is Nothing Then
            Throw New ArgumentNullException("image")
        End If
        ''create a new empty bitmap to hold rotated image
        Dim rotatedBmp As Bitmap = New Bitmap(image.Width, image.Height)
        'Dim rotatedBmp As Bitmap = New Bitmap(image)
        rotatedBmp.SetResolution(image.HorizontalResolution, image.VerticalResolution)
        ''make a graphics object from the empty bitmap
        Dim g As Graphics = Graphics.FromImage(rotatedBmp)
        ''Put the rotation point in the center of the image

        g.TranslateTransform(offset.X, offset.Y)
        ''rotate the image
        g.RotateTransform(angleDeg)
        ''move the image back
        g.TranslateTransform(-offset.X, -offset.Y)
        ''draw passed in image onto graphics object
        g.DrawImage(image, New PointF(0, 0))
        ' g.DrawImage(image, offset)
        Return rotatedBmp
    End Function

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        G = pnlMain.CreateGraphics()
        M = pnlMap.CreateGraphics()

        For ix = 0 To NoOfDroids
            Dim Droids As New DroidStruct
            Droids.Name = "Droid " + ix.ToString
            droids.Enabled = False
            droidlist.Add(Droids)
        Next
        OutsideTemp = 70
        Dim pnt As PointF
        xPos = 0 : yPos = 0 : angl = 0 : Speed = 0

        txtXPOS.Text = xPos : txtYPOS.Text = yPos : txtANGL.Text = angl : txtSpeed.Text = Speed
        pnt.X = xPos
        pnt.Y = yPos
        picDroid.Image = Image.FromFile("G:\My Drive\Software\Droid1.bmp")
        picClear.Image = Image.FromFile("G:\My Drive\Software\Clear.bmp")
        pb1.Image = Image.FromFile("G:\My Drive\Software\Base.bmp")
        pb2.Image = Image.FromFile("G:\My Drive\Software\Base.bmp")
        pb3.Image = Image.FromFile("G:\My Drive\Software\Base.bmp")
        G.DrawImage(picDroid.Image, pnt)
        bgColor = pnlMain.BackColor
        Call InitializeDroids()
        txtDroid.Text = "1"
        SelDroid.Items.Clear()
        selStation.Items.Clear()
        For drds = 1 To NoOfDroids
            SelDroid.Items.Add("Droid " + Trim(drds.ToString))
        Next
        For prt = 1 To 3
            selStation.Items.Add("H2OTower " + Trim(prt.ToString))
        Next
        For prt = 1 To 28
            selStation.Items.Add("Drill " + Trim(prt.ToString))
        Next
        For prt = 1 To 16
            selStation.Items.Add("Manuf " + Trim(prt.ToString))
        Next
        For prt = 1 To 21
            selStation.Items.Add("Solar " + Trim(prt.ToString))
        Next
        For prt = 1 To 4
            selStation.Items.Add("Pad " + Trim(prt.ToString))
        Next
        btnStartStopDroid.Text = "Stop Droid"
        tmrMovebot.Enabled = True
        tmrFlashLights.Enabled = True
        DaysfromStart = 0
    End Sub
    Private Sub InitializeDroids()
        Dim objst As System.Object
        Dim XX, YY, DD, VV, GRG As Single

        For drds = 1 To NoOfDroids
            dr = droidlist(drds)
            XX = 0 : YY = 0 : DD = 0 : VV = 0 : GRG = 0
            objst = Findcntl("dG", drds, pnlMain, cntlfnd)
            If cntlfnd Then
                Call GetDocDta(objst, XX, YY)

                If drds < 11 Then
                    GRG = 1
                ElseIf drds < 21 Then
                    GRG = 2
                Else
                    GRG = 3
                End If

                dr.X = XX                       'X Coord
                dr.Y = YY                       'Y Coord
                dr.Dir = 0
                dr.Vel = 4                      'Travel Velocity
                dr.Batt = 100                   'Battery Power
                dr.ETemp = 70                   'Ext skin temp
                dr.ITemp = 70                   'internal temp
                dr.Speed = 4                    'Speed setting (0-7)
                dr.BaseX = XX                   'Base X Position
                dr.BaseY = YY                   'Base Y Position
                dr.PrdOnBoard = 0               'Product Capacity
                dr.Enabled = False              'Droid enable/disable switch
                dr.Tow = "None"                 'Droid is towing another Droid (droid in tow)
                dr.DestX = XX                   'Destination X coord
                dr.DestY = YY                   'Destination Y coord
                dr.Garage = GRG.ToString        'garage assignment
                dr.RemainingPGM = ""            'Remaining Program 
                dr.OGpgm = ""                   'Original Program
                dr.Exec = ""                    'Current Operation
                Call LoadPrograms()
                'Droid(drds, D_X) = XX
                'Droid(drds, D_Y) = YY
                'Droid(drds, D_Dir) = 0
                'Droid(drds, D_Vel) = 4          'Travel Velocity
                'Droid(drds, D_TurrDir) = 0      'Turret Direction
                'Droid(drds, D_Batt) = 100       'Battery Power
                'Droid(drds, D_ETemp) = 70       'Ext skin temp
                'Droid(drds, D_ITemp) = 70       'internal temp
                'Droid(drds, D_EnvSet) = 0       'Environ setting
                'Droid(drds, D_Speed) = 4        'Speed setting (0-7)
                'Droid(drds, D_AutoNav) = 0      'Auto Navigate Switch
                'Droid(drds, D_Color) = 1        'Droid Color
                'Droid(drds, D_BaseX) = XX       'Base X Position
                'Droid(drds, D_BaseY) = YY       'Base Y Position
                'Droid(drds, D_Prgm) = 0         'Currently executing Program
                'Droid(drds, D_PC) = 0           'Current Program Counter
                'Droid(drds, D_Connect) = 1      'Connected to a station
                'Droid(drds, D_PrdCap) = 0       'Product Capacity
                'Droid(drds, D_Enabled) = False  'Droid enable/disable switch
                'Droid(drds, D_Tow) = 0          'Droid is towing another Droid (droid in tow)
                'Droid(drds, D_DestX) = XX       'Destination X coord
                'Droid(drds, D_DestY) = YY       'Destination Y coord
                'Info(drds, I_Garage) = GRG
                'Info(drds, I_Dest) = "HOME"
                'Info(drds, I_PGM) = "HOME"
                'Info(drds, I_OGpgm) = "HOME"
                'Info(drds, I_Tow) = "NONE"
            End If
        Next
    End Sub
    Private Sub LoadPrograms()
        Dim st, su, lst As String
        Dim iy, rc As Integer
        lstG1pgm.Items.Clear()
        lstG2pgm.Items.Clear()
        lstG3pgm.Items.Clear()
        For ix = 0 To LstPGMs.Items.Count - 1
            st = LstPGMs.Items(ix)
            su = Mid(st, 1, 4)
            Select Case su
                Case "Gar1"
                    lstG1pgm.Items.Add(st)
                Case "Gar2"
                    lstG2pgm.Items.Add(st)
                Case "Gar3"
                    lstG3pgm.Items.Add(st)
                Case Else
            End Select
        Next
        iy = 0 : st = "" : lst = ""
        For drds = 1 To 10
            st = lstG1pgm.Items.Item(iy)
            dr = droidlist(drds)
            iy = iy + 1
            If iy >= lstG1pgm.Items.Count Then iy = 0
            Call PARSE(st, lst, "|", rc)    'Skip Program name
            dr.OGpgm = st                   'OG Program
            Call PARSE(st, lst, "|", rc)
            dr.RemainingPGM = st            'Next Part of Program
            dr.Dest = lst                   'Destination (Current Operation)
            Call GetDest(dr)
            dr.Enabled = True
        Next
        iy = 0 : st = "" : lst = ""
        For drds = 11 To 20
            st = lstG2pgm.Items.Item(iy)
            dr = droidlist(drds)
            iy = iy + 1
            If iy >= lstG2pgm.Items.Count Then iy = 0
            Call PARSE(st, lst, "|", rc)    'Skip Program name
            dr.OGpgm = st                   'OG Program
            Call PARSE(st, lst, "|", rc)
            dr.RemainingPGM = st            'Next Part of Program
            dr.Dest = lst                   'Destination (Current Operation)
            Call GetDest(dr)
            dr.Enabled = True
        Next
        iy = 0 : st = "" : lst = ""
        For drds = 21 To 30
            st = lstG3pgm.Items.Item(iy)
            dr = droidlist(drds)
            iy = iy + 1
            If iy >= lstG3pgm.Items.Count Then iy = 0
            Call PARSE(st, lst, "|", rc)    'Skip Program name
            dr.OGpgm = st                   'OG Program
            Call PARSE(st, lst, "|", rc)
            dr.RemainingPGM = st            'Next Part of Program
            dr.Dest = lst                   'Destination (Current Operation)
            Call GetDest(dr)
            dr.Enabled = True
        Next

    End Sub
    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click
        G.Clear(bgColor)
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles tmrMovebot.Tick

        Call RealWorld()
        Call MoveAllDroids()
        Call DisplayDroid()
        Call DrawMap()

    End Sub
    Private Sub tmrTick_Tick(sender As Object, e As EventArgs) Handles tmrTick.Tick

        TOD = TOD + TimeOfTick
        If TOD > 20 Then
            DaysfromStart = DaysfromStart + 1
            TOD = 0
        End If

    End Sub
    Private Sub tmrFlashLights_Tick(sender As Object, e As EventArgs) Handles tmrFlashLights.Tick
        pada1.Visible = Not pada1.Visible
    End Sub
    Private Sub MoveAllDroids()
        For drds = 1 To NoOfDroids
            dr = droidlist(drds)
            If dr.Enabled = True Then
                Call DrawDroid(False, dr)
                Call MoveDroid(dr)
                Call DrawDroid(True, dr)
            Else
                Call DrawDroid(True, dr)
            End If
        Next
    End Sub
    Private Sub DrawDroid(drw As Boolean, ByRef dr As DroidStruct)
        Dim cntr As PointF
        Dim pnt As PointF
        '
        ' Droids are 30 pixels square.  Its position is offset by 15 X 15 
        '
        pnt.X = dr.X - 15 : pnt.Y = dr.Y - 15
        If drw Then
            cntr.X = 15 : cntr.Y = 15
            G.DrawImage(RotateImage(picDroid.Image, cntr, Val(dr.Dir).ToString), pnt)
        Else
            cntr.X = 15 : cntr.Y = 15
            G.DrawImage(picClear.Image, pnt)
        End If

    End Sub
    Private Sub MoveDroid(ByRef dr As DroidStruct)
        Dim DrdPoint, DrdVector As Point
        Dim dist As Single
        '
        'adjust speed according to distance to destination
        '
        dist = Dist2Dest(dr)
        Select Case dist
            Case Is > 500
                dr.Speed = 20
            Case Is > 15
                dr.Speed = 15
            Case Else
                dr.Speed = 2
        End Select

        If Arrived(dr) Then
            Call GetNexDest(dr)
        End If

        angl = dr.Dir
        Speed = dr.Speed
        DrdPoint.X = dr.X
        DrdPoint.Y = dr.Y
        DrdVector.X = Speed * Math.Sin(DegtoRad(180 - angl)) + DrdPoint.X
        DrdVector.Y = Speed * Math.Cos(DegtoRad(180 - angl)) + DrdPoint.Y
        dr.X = DrdVector.X
        dr.Y = DrdVector.Y


    End Sub
    Private Sub WGO(ByRef dr As DroidStruct)
        Dim Oper As String

        Oper = Mid(dr.Dest, 1, 2)

        Select Case Oper
            Case "WT"       'Water Tower

            Case "DR"       'Drill Site

            Case "MF"       'Manufacturing Facility

            Case "SP"       'Solar Panel

            Case "LP"       'Landing Pad

            Case "WP"       'Way Point

            Case "HO"       'Home base
            Case Else
        End Select

        If Oper = "WP" Then
            If Dist2Dest(dr) < 20 Then
                '
                ' Get Next Destination
                '
                GetNexDest(dr)
                '
            End If
        End If
    End Sub
    Private Function Dist2Dest(ByRef dr As DroidStruct) As Integer
        Dim dstX, dstY, drdX, drdY As Single
        drdX = dr.X
        drdY = dr.Y
        dstX = dr.DestX
        dstY = dr.DestY
        Dist2Dest = Math.Abs(Math.Sqrt((dstX - drdX) ^ 2 + (dstY - drdY) ^ 2))
    End Function

    Private Sub DrawMap()
        Dim objmap As System.Object
        Dim px, py As Integer
        Dim ps As String
        Dim myBr As New SolidBrush(Color.Blue)
        M.Clear(pnlMap.BackColor)
        cntlfnd = False
        For ix = 1 To 20
            objmap = Findcntl("ps", ix, pnlMain, cntlfnd)
            If cntlfnd Then
                myBr.Color = Color.Blue
                ps = objmap.Name
                px = objmap.location.x : py = objmap.location.y
                px = px * (noOfPix / 7000) : py = py * (noOfPix / 7000)
                M.FillRectangle(myBr, px, py, 4, 4)
            End If
            objmap = Findcntl("picBase", ix, pnlMain, cntlfnd)
            If cntlfnd Then
                myBr.Color = Color.DarkGray
                ps = objmap.Name
                px = objmap.location.x : py = objmap.location.y
                px = px * (noOfPix / 7000) : py = py * (noOfPix / 7000)
                M.FillRectangle(myBr, px, py, 4, 4)
            End If
            objmap = Findcntl("pwt", ix, pnlMain, cntlfnd)
            If cntlfnd Then
                myBr.Color = Color.DodgerBlue
                ps = objmap.Name
                px = objmap.location.x : py = objmap.location.y
                px = px * (noOfPix / 7000) : py = py * (noOfPix / 7000)
                M.FillRectangle(myBr, px, py, 4, 4)
            End If
            objmap = Findcntl("pd", ix, pnlMain, cntlfnd)
            If cntlfnd Then
                myBr.Color = Color.Green
                ps = objmap.Name
                px = objmap.location.x : py = objmap.location.y
                px = px * (noOfPix / 7000) : py = py * (noOfPix / 7000)
                M.FillRectangle(myBr, px, py, 4, 4)
            End If

        Next

        For drd = 1 To NoOfDroids
            dr = droidlist(drd)
            myBr.Color = Color.Red
            px = dr.X : py = dr.Y
            px = px * (noOfPix / 7000) : py = py * (noOfPix / 7000)
            M.FillRectangle(myBr, px, py, 2, 2)
        Next

    End Sub
    Private Sub btnCW_Click(sender As Object, e As EventArgs)
        DrawDroid(False, droidlist(1))
        angl = Val(txtANGL.Text) + 10
        Do While angl > 360
            angl = angl - 360
        Loop
        txtANGL.Text = Trim(Str(angl))
        DrawDroid(True, droidlist(1))
    End Sub

    Private Sub btnCCW_Click(sender As Object, e As EventArgs)
        DrawDroid(False, droidlist(1))
        angl = Val(txtANGL.Text) - 10
        Do While angl < 0
            angl = angl + 360
        Loop
        txtANGL.Text = Trim(Str(angl))
        DrawDroid(True, droidlist(1))
    End Sub

    Private Sub btnStartStopDroid_Click(sender As Object, e As EventArgs) Handles btnStartStopDroid.Click
        If tmrMovebot.Enabled = True Then
            tmrMovebot.Enabled = False
            btnStartStopDroid.Text = "Start Droid"
        Else
            btnStartStopDroid.Text = "Stop Droid"
            tmrMovebot.Enabled = True
        End If
    End Sub
    Private Sub GetDocDta(pic As PictureBox, ByRef X As Single, ByRef Y As Single)

        Select Case pic.Tag
            Case "T"
                X = pic.Location.X + pic.Size.Width / 2
                Y = pic.Location.Y '+ pic.Size.Height
            Case "B"
                X = pic.Location.X + pic.Size.Width / 2
                Y = pic.Location.Y + pic.Size.Height
            Case "R"
                X = pic.Location.X + pic.Size.Width
                Y = pic.Location.Y + pic.Size.Height / 2
            Case "L"
                X = pic.Location.X ' - pic.Size.Width
                Y = pic.Location.Y + pic.Size.Height / 2
            Case "C"
                X = pic.Location.X + pic.Size.Width / 2
                Y = pic.Location.Y + pic.Size.Height / 2
            Case Else
        End Select
    End Sub

    Private Sub dG_Click(sender As Object, e As EventArgs) Handles dG1.Click, dG2.Click, dG3.Click,
            dG4.Click, dG5.Click, dG6.Click, dG7.Click, dG8.Click, dG9.Click, dG10.Click,
        dG11.Click, dG12.Click, dG13.Click, dG14.Click, dG15.Click, dG16.Click, dG17.Click,
        dG18.Click, dG19.Click, dG20.Click, dG21.Click, dG22.Click, dG23.Click, dG24.Click,
        dG25.Click, dG26.Click, dG27.Click, dG28.Click, dG29.Click, dG30.Click

        Dim nm, Tg As String
        Dim drd As Integer
        Dim ax, ay As Single
        nm = sender.name
        Tg = sender.tag
        drd = Val(Mid(nm, 7))
        If drd = 0 Then drd = Val(Mid(nm, 8))
        Select Case Tg
            Case "B"
                angl = 180
                txtANGL.Text = Trim(Str(angl))
            Case "T"
                angl = 0
                txtANGL.Text = Trim(Str(angl))
            Case "R"
                angl = 270
                txtANGL.Text = Trim(Str(angl))
            Case "L"
                angl = 90
                txtANGL.Text = Trim(Str(angl))
            Case Else
        End Select
        dr = droidlist(drd)
        dr.Dir = angl
        dr.Enabled = True
        GetDocDta(sender, ax, ay)
        dr.X = ax : dr.Y = ay

        SelDroid.Text = "Droid " + Trim(Str(drd))
    End Sub

    Private Sub btnDrawMap_Click(sender As Object, e As EventArgs) Handles btnDrawMap.Click
        frmMap.Show()
    End Sub

    Private Function Arrived(ByRef dr As DroidStruct) As Boolean
        Dim drdX, drdY, dstX, dstY, xv, dx, dy As Integer
        Dim dist As Single

        dstX = dr.DestX
        dstY = dr.DestY
        drdX = dr.X
        drdY = dr.Y
        '
        ' Update Direction
        '
        dx = (dstX - drdX) : dy = (dstY - drdY)

        If dx >= 0 Then xv = 90 Else xv = 270
        If dx <> 0 Then
            dr.Dir = RadToDeg((Math.Atan(dy / dx))) + xv
        End If
        '
        ' Calculate Distance to Destination
        '
        dist = Math.Sqrt(Math.Abs(dstX - drdX) ^ 2 + Math.Abs(dstY - drdY) ^ 2)
        If Mid(dr.Dest, 1, 2) = "WP" Then
            If Math.Abs(dist) < 20 Then Arrived = True Else Arrived = False
        Else
            If Math.Abs(dist) < 3 Then
                Arrived = True
            Else
                Arrived = False
            End If
        End If

    End Function

    Private Sub pnlMap_MouseDown(sender As Object, ByVal e As MouseEventArgs) Handles pnlMap.MouseDown
        Dim px, py As Integer
        Dim pnt As Point
        'pnt.X = xPos
        px = e.Location.X : py = e.Location.Y
        pnt.X = px * (7000 / noOfPix) : pnt.Y = py * (7000 / noOfPix)

        pnlView.AutoScrollPosition = pnt
    End Sub

    Private Sub btnSendDroid_Click(sender As Object, e As EventArgs) Handles btnSendDroid.Click
        Dim drds, statn, P1 As String
        Dim drdno, statno As Integer
        Dim drx, dry, dist, xv As Single
        Dim dx, dy As Single
        Dim objDest As New System.Object
        Dim fnd = False

        statn = selStation.Text : P1 = ""
        Parsenum(statn, P1) : statno = Val(statn) : statn = P1

        drds = SelDroid.Text
        Parsenum(drds, P1) : drdno = Val(drds) : drds = P1

        Select Case statn
            Case "H2OTower"
                statn = "dW"
            Case "Drill"
                statn = "dD"
            Case "Manuf"
                statn = "dM"
            Case "Solar"
                statn = "dS"
            Case "Pad"
                statn = "dP"
            Case "WayPoint"
                statn = "wp"
            Case Else
        End Select
        dr = droidlist(drdno)
        objDest = Findcntl(statn, statno, pnlMain, fnd)
        If fnd Then
            dr.Enabled = True
            GetDocDta(objDest, dx, dy)
        End If


        '        Droid(drdno, D_Dir) = angl
        '        Droid(drdno, D_X) = xPos : Droid(drdno, D_Y) = yPos
        '

        dr.DestX = dx
        dr.DestY = dy

        drx = dr.X
        dry = dr.Y
        dist = Math.Sqrt(Math.Abs(dx - drx) ^ 2 + Math.Abs(dy - dry) ^ 2)
        '
        '  Check direction to target
        '
        drx = dx - drx
        dry = dx - dry
        If drx >= 0 Then
            xv = 90
        Else
            xv = 270
        End If
        If drx <> 0 Then
            dr.Dir = RadToDeg(Math.Atan(dry / drx)) + xv
        End If


    End Sub
    Private Sub RealWorld()
        Dim ccc1 As Single
        '
        '  Time Of Day
        '
        '  0-5 =sun rise till noon
        '  5-10=noon till sunset
        '  10-15 = sunset till midnight
        '  15-20 = midnight till sunrise    0   2   5   7   8   9  10  12  15  17  20
        '                                   70  74  80  84  86  84 82  78  74  72  70
        lblTimeOfDay.Text = Format$(DaysfromStart, "0") + " days " + Format$(TOD, "00.00")

        Select Case TOD
            Case Is <= 8
                OutsideTemp = LoTemp + ((TOD) * heatval) + CurrTI
            Case Is <= 20
                OutsideTemp = HiTemp - ((TOD - 8) * coolval) + CurrTI
            Case Else
                OutsideTemp = 0 + CurrTI
        End Select

        ccc1 = Rnd()
        If OutsideTemp > (420 + ccc1) Then OutsideTemp = 420 + ccc1
        If OutsideTemp < (-455 + ccc1) Then OutsideTemp = -455 + ccc1
        lblOTemp.Text = Format$(OutsideTemp, "0.00") + " DegF"

        If LowTemp! > outsidetemp Then LowTemp! = OutsideTemp
        If HighTemp! < OutsideTemp Then HighTemp! = OutsideTemp

        ToolTip1.SetToolTip(lblOTemp, " Highest:" + Str(HighTemp!) + "   Lowest:" + Str(LowTemp!))
        ToolTip1.SetToolTip(lblTemptxt, " Highest:" + Str(HighTemp!) + "   Lowest:" + Str(LowTemp!))
        'Call TimeViewSet(TOD!, outsidetemp)
    End Sub




    Private Sub DisplayDroid()
        Dim drd As Integer


        drd = Val(txtDroid.Text)
        dr = droidlist(drd)

        txtGarge.Text = dr.Garage
        txtDest.Text = dr.Dest
        txtPGM.Text = dr.RemainingPGM
        txtOGpgm.Text = dr.OGpgm
        txtTow.Text = dr.Tow
        txtXPOS.Text = dr.X.ToString
        txtYPOS.Text = dr.Y.ToString
        txtSpeed.Text = dr.Speed.ToString
        txtANGL.Text = dr.Dir.ToString
    End Sub
    Private Sub dM_Click(sender As Object, e As EventArgs) Handles dM1.Click, dM2.Click, dM3.Click,
        dM4.Click, dM5.Click, dM6.Click, dM7.Click, dM8.Click, dM9.Click, dM10.Click,
        dM11.Click, dM12.Click, dM13.Click, dM14.Click, dM15.Click, dM16.Click
        'Manufaturing port
        Dim nm, tg As String
        Dim mfg As Integer
        nm = sender.name
        tg = sender.tag
        mfg = Val(Mid(nm, 3))

        selStation.Text = "Manuf " + Trim(Str(mfg))
        If Mapping = True Then
            txtPGMmap.Text = txtPGMmap.Text + "|MF" + mfg.ToString
        End If
    End Sub

    Private Sub dS_Click(sender As Object, e As EventArgs) Handles dS1.Click, dS2.Click, dS3.Click,
        dS4.Click, dS5.Click, dS6.Click, dS7.Click, dS8.Click, dS9.Click, dS10.Click,
        dS11.Click, dS12.Click, dS13.Click, dS14.Click, dS15.Click, dS16.Click, dS17.Click,
        dS18.Click, dS19.Click, dS20.Click, dS21.Click
        'Solar port
        Dim nm, tg As String
        Dim sol As Integer
        nm = sender.name
        tg = sender.tag
        sol = Val(Mid(nm, 3))

        selStation.Text = "Solar " + Trim(Str(sol))
        If Mapping = True Then
            txtPGMmap.Text = txtPGMmap.Text + "|SP" + sol.ToString
        End If
    End Sub

    Private Sub dP1_Click(sender As Object, e As EventArgs) Handles dP1.Click, dP2.Click, dP3.Click, dP4.Click
        'Landing Pad port
        Dim nm, tg As String
        Dim lp As Integer
        nm = sender.name
        tg = sender.tag
        lp = Val(Mid(nm, 3))

        selStation.Text = "Pad " + Trim(Str(lp))

        If Mapping = True Then
            txtPGMmap.Text = txtPGMmap.Text + "|LP" + lp.ToString
        End If
    End Sub
    Private Sub dW_Click(sender As Object, e As EventArgs) Handles dW1.Click, dW2.Click, dW3.Click
        'H2OTower port
        Dim nm, tg As String
        Dim h20 As Integer
        nm = sender.name
        tg = sender.tag
        h20 = Val(Mid(nm, 3))

        selStation.Text = "H2OTower " + Trim(Str(h20))

        If Mapping = True Then
            txtPGMmap.Text = txtPGMmap.Text + "|WT" + h20.ToString
        End If

    End Sub

    Private Sub dD_Click(sender As Object, e As EventArgs) Handles dD1.Click, dD2.Click, dD3.Click,
        dD4.Click, dD5.Click, dD6.Click, dD7.Click, dD8.Click, dD9.Click, dD10.Click,
        dD11.Click, dD12.Click, dD13.Click, dD14.Click, dD15.Click, dD16.Click, dD17.Click,
        dD18.Click, dD19.Click, dD20.Click, dD21.Click, dD22.Click, dD23.Click, dD24.Click,
        dD25.Click, dD26.Click, dD27.Click, dD28.Click ', dD29.Click
        'Drill port
        Dim nm, tg As String
        Dim drl As Integer
        nm = sender.name
        tg = sender.tag
        drl = Val(Mid(nm, 3))

        selStation.Text = "Drill " + Trim(Str(drl))

        If Mapping = True Then
            txtPGMmap.Text = txtPGMmap.Text + "|DR" + drl.ToString
        End If
    End Sub
    Private Sub wp_Click(sender As Object, e As EventArgs) Handles wp1.Click, wp2.Click, wp3.Click, wp4.Click, wp5.Click,
        wp6.Click, wp7.Click, wp8.Click, wp9.Click, wp10.Click, wp11.Click, wp12.Click, wp13.Click, wp14.Click, wp15.Click,
        wp16.Click, wp17.Click, wp18.Click, wp19.Click, wp20.Click, wp21.Click, wp22.Click, wp23.Click, wp24.Click, wp25.Click,
        wp26.Click, wp27.Click, wp28.Click, wp29.Click, wp30.Click, wp31.Click, wp32.Click, wp33.Click, wp34.Click, wp35.Click,
        wp36.Click, wp37.Click, wp38.Click, wp39.Click, wp40.Click, wp41.Click, wp42.Click, wp43.Click, wp44.Click, wp45.Click,
        wp46.Click, wp47.Click, wp48.Click, wp49.Click, wp50.Click, wp51.Click, wp52.Click, wp53.Click, wp54.Click, wp55.Click,
        wp56.Click, wp57.Click, wp58.Click, wp59.Click, wp60.Click ', wp61.Click, wp62.Click, wp63.Click, wp64.Click, wp65.Click, wp66.Click, wp67.Click, wp68.Click, wp69.Click

        'Drill port
        Dim nm, tg As String
        Dim Wy As Integer
        nm = sender.name
        tg = sender.tag
        Wy = Val(Mid(nm, 3))
        selStation.Text = "WayPoint " + Trim(Str(Wy))

        If Mapping = True Then
            txtPGMmap.Text = txtPGMmap.Text + "|WP" + Wy.ToString
        End If
    End Sub
    Private Sub PnlMain_Mouse(sender As Object, e As MouseEventArgs) Handles pnlMain.MouseClick
        Dim x, y, lastdist, fnddrd, dist As Integer
        x = e.X
        y = e.Y

        lastdist = 130 : fnddrd = 0
        For ix = 1 To NoOfDroids
            dr = droidlist(ix)
            dist = Math.Sqrt(Math.Abs(x - dr.X) ^ 2 + Math.Abs(y - dr.Y) ^ 2)
            If (dist < 25) And (dist < lastdist) Then
                lastdist = dist
                fnddrd = ix
            End If
        Next
        If fnddrd <> 0 Then
            SelDroid.Text = "Droid " + Trim(fnddrd.ToString)
            txtDroid.Text = Trim(fnddrd.ToString)
        End If
    End Sub

    Private Sub btnGoHome_Click(sender As Object, e As EventArgs) Handles btnGoHome.Click
        Dim drds, P1 As String
        Dim drdno, drx, dry, xv As Integer
        P1 = ""
        drds = SelDroid.Text
        Parsenum(drds, P1) : drdno = Val(drds)
        dr = droidlist(drdno)
        dr.DestX = dr.BaseX
        dr.DestY = dr.BaseY

        dr.Enabled = True
        '
        '  Set direction to target

        drx = dr.DestX - dr.X
        dry = dr.DestY - dr.Y
        If drx >= 0 Then
            xv = 90
        Else
            xv = 270
        End If
        If drx <> 0 Then
            dr.Dir = RadToDeg(Math.Atan(dry / drx)) + xv
        Else
            dr.Dir = xv
            dr.Enabled = False
        End If

        xv = Math.Sqrt(Math.Abs(drx) ^ 2 + Math.Abs(dry) ^ 2)
        If Math.Abs(xv) < 3 Then dr.Enabled = False

    End Sub

    Private Sub btnMapping_Click(sender As Object, e As EventArgs) Handles btnMapping.Click
        Mapping = Not Mapping
        If Mapping Then
            btnMapping.BackColor = Color.LightGreen
        Else
            btnMapping.BackColor = Color.LightCoral
        End If
    End Sub



    Private Sub btnLoad_Click(sender As Object, e As EventArgs) Handles btnLoad.Click
        Dim st, lst As String
        Dim rc, drd As Integer
        '
        'Get the Droid Number
        '
        st = ""
        lst = SelDroid.Text
        Parsenum(lst, st) : drd = Val(lst)
        dr = droidlist(drd)
        st = ""
        lst = ""
        '
        'Fill the droid Info
        '
        'PGM NAME|WP30|SP2|MF4|WT1|WP2|HOME
        st = UCase(LstPGMs.SelectedItem)
        Call PARSE(st, lst, "|", rc)    'Skip Program name
        dr.OGpgm = st                   'OG Program
        Call PARSE(st, lst, "|", rc)
        dr.RemainingPGM = st            'Next Part of Program
        dr.Dest = lst                   'Destination (Current Operation)
        Call GetDest(dr)
        dr.Enabled = True
    End Sub
    Private Sub GetDest(ByRef dr As DroidStruct)
        Dim St, statn As String
        Dim statno As Integer
        Dim objDest As New System.Object
        Dim fnd = False
        Dim dx, dy As Single
        St = dr.Dest
        statno = Val(Mid(St, 3))
        St = UCase(Mid(St, 1, 2))
        statn = ""

        Select Case St
            Case "HO"
            Case "WT"   ' water tower
                statn = "dW"
            Case "DR"   ' Drill
                statn = "dD"
            Case "MF"   ' Manuf
                statn = "dM"
            Case "SP"   ' Solar Panel
                statn = "dS"
            Case "LP"   ' pad
                statn = "dP"
            Case "WP"   ' Way Point
                statn = "wp"
            Case Else
                statn = "" : statno = 0
        End Select
        objDest = Findcntl(statn, statno, pnlMain, fnd)
        If fnd Then
            GetDocDta(objDest, dx, dy)
        ElseIf St = "HO" Then
            dx = dr.BaseX
            dy = dr.BaseY
        End If

        dr.DestX = dx
        dr.DestY = dy
    End Sub
    Private Sub GetNexDest(ByRef dr As DroidStruct)
        Dim Nd, St As String
        Dim rc As Integer
        St = ""
        Nd = dr.RemainingPGM
        If Nd = "" Then
            Nd = dr.OGpgm
        End If
        If Nd = "" Then
            dr.Enabled = False
            Exit Sub
        End If
        Call PARSE(Nd, St, "|", rc)
        dr.RemainingPGM = Nd
        dr.Dest = St
        Call GetDest(dr)
    End Sub


End Class
Public Class DroidStruct
    Public Name As String           ' Droid X
    Public Enabled As Boolean       ' Droid enable/disable switch
    Public X As Integer             ' X Coordinate
    Public Y As Integer             ' Y Coordinate

    Public BaseX As Single          ' Home Base X Position
    Public BaseY As Single          ' Home Base Y Position

    Public DestX As Integer         ' Droid Destination X-Coord
    Public DestY As Integer         ' Droid Destination Y-Coord

    Public Speed As Integer         ' Speed setting (0-7)
    Public Dir As Single            ' Travel Direction
    Public Vel As Single            ' Travel Velocity
    Public Batt As Integer          ' Battery Power 0-100
    Public ETemp As Single          ' Ext skin temp
    Public ITemp As Single          ' internal temp
    Public PrdOnBoard As Integer    ' Product Capacity

    Public Connected As Boolean     ' Connected to a station
    Public ConnectionName As String ' Conected Device

    Public RemainingPGM As String   ' remaining Pgm string
    Public OGpgm As String          ' Original Program
    Public Exec As String           ' Current Program executing

    Public Tow As String            ' Droid is towing another Droid (droid in tow)

    Public Garage As String         ' Droid Garage Assignment
    Public Dest As String           ' Droid Destination Name

End Class
