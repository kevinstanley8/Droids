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
    Public angl, Speed, TOD As Single
    Public cntlfnd As Boolean = True
    Public DaysfromStart As Integer
    Public droidData(30, 16) As Single '(Droid number,Parameter)
    Public Mapping As Boolean
    '
    'Each Program is stored as a string in the format:
    '
    '                 Descr|Node|Node|Node|Node
    '
    Public Const NoOfDroids = 30

    Const noOfPix = 100

    Public Droid(NoOfDroids, 30) As Single
    Public Info(NoOfDroids, 10) As String
    Public status(NoOfDroids) As String
    Public Prg(NoOfDroids, 2, 30) As Single
    Public FailureFrequency As Integer
    '                     |
    '  Droid! constants
    '
    '
    Public Const D_X = 1  '   1 = X position
    Public Const D_Y = 2  '   2 = Y position
    Public Const D_Dir = 3  '   3 = Travel Direction
    Public Const D_Vel = 4  '   4 = Travel Velocity
    Public Const D_TurrDir = 5  '   5 = Turret Direction
    Public Const D_Batt = 6  '   6 = Battery Power
    Public Const D_ETemp = 7  '   7 = Ext skin temp
    Public Const D_ITemp = 8  '   8 = internal temp
    Public Const D_EnvSet = 9  '   9 = Environ setting
    Public Const D_Speed = 10 '  10 = Speed setting (0-7)
    Public Const D_AutoNav = 11 '  11 = Auto Navigate Switch
    Public Const D_Color = 12 '  12 = Droid Color
    Public Const D_BaseX = 13 '  13 = Base X Position
    Public Const D_BaseY = 14 '  14 = Base Y Position
    Public Const D_Prgm = 15 '  15 = Currently executing Program
    Public Const D_PC = 16 '  16 = Current Program Counter
    Public Const D_Connect = 17 '  17 = Connected to a station
    Public Const D_PrdCap = 18 '  18 = Product Capacity
    Public Const D_Enabled = 19 '  19 = Droid enable/disable switch
    Public Const D_Tow = 20 '  20 = Droid is towing another Droid (droid in tow)
    Public Const D_DestX = 21 ' Droid Destination X-Coord
    Public Const D_DestY = 22 ' Droid Destination Y-Coord
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
    Public Const I_Garage = 1 ' Droid Garage Assignment
    Public Const I_Dest = 2   ' Droid Destination Name
    Public Const I_PGM = 3    ' Executing Pgm
    Public Const I_OGpgm = 4  ' Original Program
    Public Const I_Tow = 5    ' Tow Droid Name

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

        Dim pnt As PointF
        xPos = 500 : yPos = 255 - 55 : angl = 0 : Speed = 0
        xPos = 535 : yPos = 238 + 55
        xPos = 570
        xPos = 605
        xPos = 640

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
    End Sub

    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click
        G.Clear(bgColor)
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles tmrMovebot.Tick

        Call MoveAllDroids()
        Call DisplayDroid
        Call DrawMap()

    End Sub
    Private Sub tmrFlashLights_Tick(sender As Object, e As EventArgs) Handles tmrFlashLights.Tick
        pada1.Visible = Not pada1.Visible
    End Sub
    Private Sub MoveAllDroids()
        For drds = 1 To NoOfDroids
            If Droid(drds, D_Enabled) = True Then
                Call DrawDroid(False, drds)
                Call MoveDroid(drds)
                Call DrawDroid(True, drds)
            Else
                Call DrawDroid(True, drds)
            End If
        Next
    End Sub
    Private Sub DrawDroid(drw As Boolean, drd As Integer)
        Dim cntr As PointF
        Dim pnt As PointF
        '
        ' Droids are 30 pixels square.  Its position is offset by 15 X 15 
        '
        pnt.X = Droid(drd, D_X) - 15 : pnt.Y = Droid(drd, D_Y) - 15
        If drw Then
            cntr.X = 15 : cntr.Y = 15
            G.DrawImage(RotateImage(picDroid.Image, cntr, Val(Droid(drd, D_Dir).ToString)), pnt)
        Else
            cntr.X = 15 : cntr.Y = 15
            G.DrawImage(picClear.Image, pnt)
        End If

    End Sub
    Private Sub MoveDroid(drd As Integer)
        Dim DrdPoint, DrdVector As Point
        Dim dist As Single
        '
        'adjust speed according to distance to destination
        '
        dist = Dist2Dest(drd)
        Select Case dist
            Case Is > 500
                Droid!(drd, D_Speed) = 20
            Case Is > 50
                Droid!(drd, D_Speed) = 15
            Case Else
                Droid!(drd, D_Speed) = 2
        End Select


        angl = Droid(drd, D_Dir)
        Speed = Droid(drd, D_Speed)
        DrdPoint.X = Droid(drd, D_X)
        DrdPoint.Y = Droid(drd, D_Y)
        DrdVector.X = Speed * Math.Sin(DegtoRad(180 - angl)) + DrdPoint.X
        DrdVector.Y = Speed * Math.Cos(DegtoRad(180 - angl)) + DrdPoint.Y
        Droid(drd, D_X) = DrdVector.X
        Droid(drd, D_Y) = DrdVector.Y

        If Arrived(drd) Then
            Droid(drd, D_Enabled) = False
        End If

    End Sub
    Private Sub WGO(drd As Integer)
        Dim Oper As String

        Oper = Mid(Info(drd, I_Dest), 1, 2)

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
            If Dist2Dest(drd) < 20 Then
                '
                ' Get Next Destination
                '
                GetNexDest(drd)
                '
            End If
        End If
    End Sub
    Private Function Dist2Dest(ByVal drd As Integer) As Integer
        Dim dstX, dstY, drdX, drdY As Single
        drdX = Droid(drd, D_X)
        drdY = Droid(drd, D_Y)
        dstX = Droid(drd, D_DestX)
        dstY = Droid(drd, D_DestY)
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
            myBr.Color = Color.Red
            px = Droid(drd, D_X) : py = Droid(drd, D_Y)
            px = px * (noOfPix / 7000) : py = py * (noOfPix / 7000)
            M.FillRectangle(myBr, px, py, 2, 2)
        Next

    End Sub
    Private Sub btnCW_Click(sender As Object, e As EventArgs)
        DrawDroid(False, 1)
        angl = Val(txtANGL.Text) + 10
        Do While angl > 360
            angl = angl - 360
        Loop
        txtANGL.Text = Trim(Str(angl))
        DrawDroid(True, 1)
    End Sub

    Private Sub btnCCW_Click(sender As Object, e As EventArgs)
        DrawDroid(False, 1)
        angl = Val(txtANGL.Text) - 10
        Do While angl < 0
            angl = angl + 360
        Loop
        txtANGL.Text = Trim(Str(angl))
        DrawDroid(True, 1)
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
    Private Sub InitializeDroids()
        Dim objst As System.Object
        Dim XX, YY, DD, VV, GRG As Single
        For drds = 1 To NoOfDroids


            XX = 0 : YY = 0 : DD = 0 : VV = 0 : GRG = 0
            objst = Findcntl("picDoc", drds, pnlMain, cntlfnd)
            If cntlfnd Then
                Call GetDocDta(objst, XX, YY)
                Droid(drds, D_X) = XX
                Droid(drds, D_Y) = YY
                Droid(drds, D_Dir) = 0
                Droid(drds, D_Vel) = 4          'Travel Velocity
                Droid(drds, D_TurrDir) = 0      'Turret Direction
                Droid(drds, D_Batt) = 100       'Battery Power
                Droid(drds, D_ETemp) = 70       'Ext skin temp
                Droid(drds, D_ITemp) = 70       'internal temp
                Droid(drds, D_EnvSet) = 0       'Environ setting
                Droid(drds, D_Speed) = 4        'Speed setting (0-7)
                Droid(drds, D_AutoNav) = 0      'Auto Navigate Switch
                Droid(drds, D_Color) = 1        'Droid Color
                Droid(drds, D_BaseX) = XX       'Base X Position
                Droid(drds, D_BaseY) = YY       'Base X Position
                Droid(drds, D_Prgm) = 0         'Currently executing Program
                Droid(drds, D_PC) = 0           'Current Program Counter
                Droid(drds, D_Connect) = 1      'Connected to a station
                Droid(drds, D_PrdCap) = 0       'Product Capacity
                Droid(drds, D_Enabled) = False  'Droid enable/disable switch
                Droid(drds, D_Tow) = 0          'Droid is towing another Droid (droid in tow)
                Droid(drds, D_DestX) = XX       'Destination X coord
                Droid(drds, D_DestY) = YY       'Destination Y coord
                If drds < 11 Then
                    GRG = 1
                ElseIf drds < 21 Then
                    GRG = 2
                Else
                    GRG = 3
                End If
                Info(drds, I_Garage) = GRG
                Info(drds, I_Dest) = "HOME"
                Info(drds, I_PGM) = "HOME"
                Info(drds, I_OGpgm) = "HOME"
                Info(drds, I_Tow) = "NONE"
            End If
        Next
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

    Private Sub picDoc_Click(sender As Object, e As EventArgs) Handles picDoc1.Click, picDoc2.Click, picDoc3.Click,
            picDoc4.Click, picDoc5.Click, picDoc6.Click, picDoc7.Click, picDoc8.Click, picDoc9.Click, picDoc10.Click,
        picDoc11.Click, picDoc12.Click, picDoc13.Click, picDoc14.Click, picDoc15.Click, picDoc16.Click, picDoc17.Click,
        picDoc18.Click, picDoc19.Click, picDoc20.Click, picDoc21.Click, picDoc22.Click, picDoc23.Click, picDoc24.Click,
        picDoc25.Click, picDoc26.Click, picDoc27.Click, picDoc28.Click, picDoc29.Click, picDoc30.Click

        Dim nm, Tg As String
        Dim drd As Integer
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
        Droid(drd, D_Dir) = angl
        Droid(drd, D_Enabled) = True
        GetDocDta(sender, xPos, yPos)
        Droid(drd, D_X) = xPos : Droid(drd, D_Y) = yPos

        SelDroid.Text = "Droid " + Trim(Str(drd))
    End Sub

    Private Sub btnDrawMap_Click(sender As Object, e As EventArgs) Handles btnDrawMap.Click
        frmMap.Show()
    End Sub

    Private Function Arrived(DroidNumber As Integer) As Boolean
        Dim drdX, drdY, dstX, dstY, xv, dx, dy As Integer
        Dim dist As Single
        dstX = Droid(DroidNumber, D_DestX)
        dstY = Droid(DroidNumber, D_DestY)
        drdX = Droid(DroidNumber, D_X)
        drdY = Droid(DroidNumber, D_Y)
        '
        ' Update Direction
        '
        dx = (dstX - drdX) : dy = (dstY - drdY)

        If dx >= 0 Then xv = 90 Else xv = 270
        If dx <> 0 Then
            Droid!(DroidNumber, D_Dir) = RadToDeg((Math.Atan(dy / dx))) + xv
        End If
        '
        ' Calculate Distance to Destination
        '
        dist = Math.Sqrt(Math.Abs(dstX - drdX) ^ 2 + Math.Abs(dstY - drdY) ^ 2)
        If Math.Abs(dist) < 3 Then Arrived = True Else Arrived = False

    End Function

    Private Sub pnlMap_MouseDown(sender As Object, ByVal e As MouseEventArgs) Handles pnlMap.MouseDown
        Dim px, py As Integer
        Dim pnt As Point
        pnt.X = xPos
        px = e.Location.X : py = e.Location.Y
        pnt.X = px * (7000 / noOfPix) : pnt.Y = py * (7000 / noOfPix)

        pnlView.AutoScrollPosition = pnt
    End Sub

    Private Sub btnSendDroid_Click(sender As Object, e As EventArgs) Handles btnSendDroid.Click
        Dim drds, statn, P1 As String
        Dim drdno, statno As Integer
        Dim drx, dry, dist, xv As Single
        Dim objDest As New System.Object
        Dim fnd = False

        statn = selStation.Text : P1 = ""
        Parsenum(statn, P1) : statno = Val(statn) : statn = P1

        drds = SelDroid.Text
        Parsenum(drds, P1) : drdno = Val(drds) : drds = P1

        Select Case statn
            Case "H2OTower"
                statn = "picWDoc"
            Case "Drill"
                statn = "picDDoc"
            Case "Manuf"
                statn = "picMDoc"
            Case "Solar"
                statn = "picSDoc"
            Case "Pad"
                statn = "picPDoc"
            Case "WayPoint"
                statn = "wp"
            Case Else
        End Select
        objDest = Findcntl(statn, statno, pnlMain, fnd)
        If fnd Then
            Droid(drdno, D_Enabled) = True
            GetDocDta(objDest, xPos, yPos)
        End If


        '        Droid(drdno, D_Dir) = angl
        '        Droid(drdno, D_X) = xPos : Droid(drdno, D_Y) = yPos
        '

        Droid(drdno, D_DestX) = xPos
        Droid(drdno, D_DestY) = yPos

        drx = Droid(drdno, D_X)
        dry = Droid(drdno, D_Y)
        dist = Math.Sqrt(Math.Abs(xPos - drx) ^ 2 + Math.Abs(yPos - dry) ^ 2)
        '
        '  Check direction to target
        '
        drx = xPos - drx
        dry = yPos - dry
        If drx >= 0 Then
            xv = 90
        Else
            xv = 270
        End If
        If drx <> 0 Then
            Droid!(drdno, D_Dir) = RadToDeg(Math.Atan(dry / drx)) + xv
        End If


    End Sub
    Private Sub DisplayDroid()
        Dim drd As Integer
        drd = Val(txtDroid.Text)
        txtGarge.Text = Info(drd, I_Garage)
        txtDest.Text = Info(drd, I_Dest)
        txtPGM.Text = Info(drd, I_PGM)
        txtOGpgm.Text = Info(drd, I_OGpgm)
        txtTow.Text = Info(drd, I_Tow)
        txtXPOS.Text = Droid(drd, D_X).ToString
        txtYPOS.Text = Droid(drd, D_Y).ToString
        txtSpeed.Text = Droid(drd, D_Speed).ToString
        txtANGL.Text = Droid(drd, D_Dir).ToString
    End Sub
    Private Sub picMDoc_Click(sender As Object, e As EventArgs) Handles picMDoc1.Click, picMDoc2.Click, picMDoc3.Click,
        picMDoc4.Click, picMDoc5.Click, picMDoc6.Click, picMDoc7.Click, picMDoc8.Click, picMDoc9.Click, picMDoc10.Click,
        picMDoc11.Click, picMDoc12.Click, picMDoc13.Click, picMDoc14.Click, picMDoc15.Click, picMDoc16.Click
        'Manufaturing port
        Dim nm, tg As String
        Dim mfg As Integer
        nm = sender.name
        tg = sender.tag
        mfg = Val(Mid(nm, 7))
        If mfg = 0 Then mfg = Val(Mid(nm, 8))
        selStation.Text = "Manuf " + Trim(Str(mfg))
        If Mapping = True Then
            txtPGMmap.Text = txtPGMmap.Text + "|MF" + mfg.ToString
        End If
    End Sub

    Private Sub picSDoc_Click(sender As Object, e As EventArgs) Handles picSDoc1.Click, picSDoc2.Click, picSDoc3.Click,
        picSDoc4.Click, picSDoc5.Click, picSDoc6.Click, picSDoc7.Click, picSDoc8.Click, picSDoc9.Click, picSDoc10.Click,
        picSDoc11.Click, picSDoc12.Click, picSDoc13.Click, picSDoc14.Click, picSDoc15.Click, picSDoc16.Click, picSDoc17.Click,
        picSDoc18.Click, picSDoc19.Click, picSDoc20.Click, picSDoc21.Click
        'Solar port
        Dim nm, tg As String
        Dim sol As Integer
        nm = sender.name
        tg = sender.tag
        sol = Val(Mid(nm, 7))
        If sol = 0 Then sol = Val(Mid(nm, 8))
        selStation.Text = "Solar " + Trim(Str(sol))
        If Mapping = True Then
            txtPGMmap.Text = txtPGMmap.Text + "|SP" + sol.ToString
        End If
    End Sub

    Private Sub picPDoc1_Click(sender As Object, e As EventArgs) Handles picPDoc1.Click, picPDoc2.Click, picPDoc3.Click, picPDoc4.Click
        'Landing Pad port
        Dim nm, tg As String
        Dim lp As Integer
        nm = sender.name
        tg = sender.tag
        lp = Val(Mid(nm, 7))
        If lp = 0 Then lp = Val(Mid(nm, 8))
        selStation.Text = "Pad " + Trim(Str(lp))

        If Mapping = True Then
            txtPGMmap.Text = txtPGMmap.Text + "|LP" + lp.ToString
        End If
    End Sub
    Private Sub picWDoc_Click(sender As Object, e As EventArgs) Handles picWDoc1.Click, picWDoc2.Click, picWDoc3.Click
        'H2OTower port
        Dim nm, tg As String
        Dim h20 As Integer
        nm = sender.name
        tg = sender.tag
        h20 = Val(Mid(nm, 7))
        If h20 = 0 Then h20 = Val(Mid(nm, 8))
        selStation.Text = "H2OTower " + Trim(Str(h20))

        If Mapping = True Then
            txtPGMmap.Text = txtPGMmap.Text + "|WT" + h20.ToString
        End If

    End Sub

    Private Sub picDDoc_Click(sender As Object, e As EventArgs) Handles picDDoc1.Click, picDDoc2.Click, picDDoc3.Click,
        picDDoc4.Click, picDDoc5.Click, picDDoc6.Click, picDDoc7.Click, picDDoc8.Click, picDDoc9.Click, picDDoc10.Click,
        picDDoc11.Click, picDDoc12.Click, picDDoc13.Click, picDDoc14.Click, picDDoc15.Click, picDDoc16.Click, picDDoc17.Click,
        picDDoc18.Click, picDDoc19.Click, picDDoc20.Click, picDDoc21.Click, picDDoc22.Click, picDDoc23.Click, picDDoc24.Click,
        picDDoc25.Click, picDDoc26.Click, picDDoc27.Click, picDDoc28.Click ', picDDoc29.Click
        'Drill port
        Dim nm, tg As String
        Dim drl As Integer
        nm = sender.name
        tg = sender.tag
        drl = Val(Mid(nm, 7))
        If drl = 0 Then drl = Val(Mid(nm, 8))
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
            dist = Math.Sqrt(Math.Abs(x - Droid!(ix, D_X)) ^ 2 + Math.Abs(y - Droid!(ix, D_Y)) ^ 2)
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

        Droid(drdno, D_DestX) = Droid(drdno, D_BaseX)
        Droid(drdno, D_DestY) = Droid(drdno, D_BaseY)

        Droid(drdno, D_Enabled) = True
        '
        '  Set direction to target
        '
        drx = Droid(drdno, D_DestX) - Droid(drdno, D_X)
        dry = Droid(drdno, D_DestY) - Droid(drdno, D_Y)
        If drx >= 0 Then
            xv = 90
        Else
            xv = 270
        End If
        If drx <> 0 Then
            Droid!(drdno, D_Dir) = RadToDeg(Math.Atan(dry / drx)) + xv
        Else
            Droid!(drdno, D_Dir) = xv
            Droid(drdno, D_Enabled) = False
        End If

        xv = Math.Sqrt(Math.Abs(drx) ^ 2 + Math.Abs(dry) ^ 2)
        If Math.Abs(xv) < 3 Then Droid(drdno, D_Enabled) = False

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
        '
        'Fill the droid Info
        '
        st = ""
        lst = ""
        st = ListBox1.SelectedItem
        Call PARSE(st, lst, "|", rc)    'Program name
        Info(drd, I_OGpgm) = st              'OG Program
        Call PARSE(st, lst, "|", rc)
        Info(drd, I_PGM) = st           'Next Part of Program
        Info(drd, I_Dest) = lst         'Destination (Current Operation)
        Droid(drd, D_Enabled) = True
    End Sub
    Private Sub GetNexDest(drd As Integer)
        Dim Nd, St, statn As String
        Dim rc, statno As Integer
        Dim objDest As New System.Object
        Dim fnd = False
        St = ""
        Nd = Info(drd, I_PGM)
        Call PARSE(Nd, St, "|", rc)
        Info(drd, I_Dest) = St
        Info(drd, I_PGM) = Nd
        'WP30|SP2|MF4|WT1|WP2|HOME
        statno = Val(Mid(St, 3))
        St = UCase(Mid(St, 1, 2))
        statn = ""
        Select Case St
            Case "HO"
            Case "WT"   ' water tower
                statn = "picWDoc"
            Case "DR"   ' Drill
                statn = "picDDoc"
            Case "MF"   ' Manuf
                statn = "picMDoc"
            Case "SP"   ' Solar Panel
                statn = "picSDoc"
            Case "LP"   ' pad
                statn = "picPDoc"
            Case "WP"   ' Way Point
                statn = "wp"
            Case Else
                statn = "" : statno = 0
        End Select
        objDest = Findcntl(statn, statno, pnlMain, fnd)
        If fnd Then
            Droid(drd, D_Enabled) = True
            GetDocDta(objDest, xPos, yPos)
        ElseIf St = "HO" Then
            xPos = Droid(drd, D_BaseX)
            yPos = Droid(drd, D_BaseY)
        End If
        '        Droid(drdno, D_Dir) = angl
        '        Droid(drdno, D_X) = xPos : Droid(drdno, D_Y) = yPos
        '
        Droid(drd, D_DestX) = xPos
        Droid(drd, D_DestY) = yPos

    End Sub

End Class
