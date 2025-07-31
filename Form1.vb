Imports System.ComponentModel.DataAnnotations
Imports System.Diagnostics.CodeAnalysis
Imports System.IO
Imports System.Runtime.InteropServices.JavaScript.JSType
Imports System.Xml

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
    'Public droidData(30, 16) As Single '(Droid number,Parameter)
    Public Mapping As Boolean
    Public RocketOnPad As Boolean
    Public dr As DroidStruct
    Public pl As PlaceStruct
    Public CurrTI As Single
    Public arrray(100) As String
    '
    'Each Program is stored as a string in the format:
    '
    '                 Descr|Node|Node|Node|Node
    '
    Public Const NoOfDroids = 30
    Public Const NoOfPlaces = 67                '    30   + 2   + 3   + 21   + 3   + 7   + 1
    '                                           ' HO=30  MF=2  MZ=3  SP=21  WT=3  DR=7  LP=1
    '


    Public Const noOfPix = 100
    Public Const ScreenSize = 7000 'the total coordinate screen space is 7000 X 7000

    'Public Droid(NoOfDroids, 30) As Single
    'Public Info(NoOfDroids, 10) As String
    Public status(NoOfDroids) As String
    Public Prg(NoOfDroids, 2, 30) As Single
    Public FailureFrequency As Integer

    Public Const TimeOfTick = 0.01    '0.002
    Public Const LoTemp = -250
    Public Const HiTemp = 250

    Public Const heatval = (HiTemp - LoTemp) / 8
    Public Const coolval = (HiTemp - LoTemp) / 12
    Public Const DroidHV = 1  '1.2   0.03
    Public Const DroidAC = -1 ' -0.75
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
    Public PlacesList As New List(Of PlaceStruct)
    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        G = pnlMain.CreateGraphics()
        M = pnlMap.CreateGraphics()
        Me.Top = 0
        Me.Left = 0
        For ix = 0 To NoOfDroids
            Dim Droids As New DroidStruct
            Droids.Name = "Droid " + ix.ToString
            Droids.Enabled = False
            droidlist.Add(Droids)
        Next

        For ix = 0 To NoOfPlaces
            Dim plc As New PlaceStruct
            plc.Name = "PL" + ix.ToString
            PlacesList.Add(plc)
        Next
        OutsideTemp = 100
        TOD = 6
        RocketOnPad = False
        Dim pnt As PointF
        xPos = 0 : yPos = 0 : angl = 0 : Speed = 0

        Call InitializeDroids()
        txtXPOS.Text = xPos : txtYPOS.Text = yPos : txtANGL.Text = angl : txtSpeed.Text = Speed
        pnt.X = xPos
        pnt.Y = yPos
        picDroid.Image = imgListD.Images(0) ' droid1
        'picDroid.Image = Image.FromFile(txtMainPath.Text + "Droid1.bmp")
        'picDroidSel.Image = imgListD.Images(1) ' droid1r
        'picDroidSel.Image = Image.FromFile(txtMainPath.Text + "Droid1r.bmp")

        'picDroid.Image = Image.FromFile(txtMainPath.Text + "Droid2.bmp")
        'picDroidSel.Image = Image.FromFile(txtMainPath.Text + "Droid2r.bmp")

        'picDroidSel.Image = Image.FromFile(txtMainPath.Text + "Droid2e.bmp")
        'picDroid.Image = Image.FromFile(txtMainPath.Text + "Droid2er.bmp")

        picClear.Image = Image.FromFile(txtMainPath.Text + "Clear.bmp")

        'pb1.Image = Image.FromFile(txtMainPath.Text + "Base.bmp")
        'pb2.Image = Image.FromFile(txtMainPath.Text + "Base.bmp")
        'pb3.Image = Image.FromFile(txtMainPath.Text + "Base.bmp")
        G.DrawImage(picDroid.Image, pnt)
        bgColor = pnlMain.BackColor

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
        For prt = 1 To 4
            selStation.Items.Add("Manuf " + Trim(prt.ToString))
        Next
        For prt = 1 To 21
            selStation.Items.Add("Solar " + Trim(prt.ToString))
        Next
        For prt = 1 To 4
            selStation.Items.Add("Pad " + Trim(prt.ToString))
        Next
        For prt = 1 To 3
            selStation.Items.Add("MineZone " + Trim(prt.ToString))
        Next
        btnStartStopDroid.Text = "Stop Droid"
        tmrMovebot.Enabled = True
        tmrFlashLights.Enabled = True
        DaysfromStart = 0

    End Sub
    Private Sub InitializeDroids()
        Dim objst As System.Object
        Dim XX, YY, DD, VV, GRG As Single
        Dim plx As Integer

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
            End If
        Next
        plx = 0
        For ix = 1 To 30 'garages
            plx = plx + 1
            pl = PlacesList(plx)
            pl.PType = "HO"
            pl.OType = "dG"
            pl.Name = pl.PType + ix.ToString
            pl.LBLName = ""
            pl.Goods = 0
            pl.HStatus = 70
            pl.X = 0
            pl.Y = 0
            objst = Findcntl(pl.OType, ix, pnlMain, cntlfnd)
            If cntlfnd Then
                GetDocDta(objst, pl.X, pl.Y)
            End If

        Next
        For ix = 1 To 2
            plx = plx + 1
            pl = PlacesList(plx)
            pl.PType = "MF"
            pl.OType = "dM"
            pl.LBLName = "lblm" + ix.ToString
            pl.Name = pl.PType + ix.ToString
            pl.Goods = 0
            pl.HStatus = 70
            pl.X = 0
            pl.Y = 0
            objst = Findcntl(pl.OType, ix, pnlMain, cntlfnd)
            If cntlfnd Then
                GetDocDta(objst, pl.X, pl.Y)
            End If
        Next
        For ix = 1 To 3
            plx = plx + 1
            pl = PlacesList(plx)
            pl.PType = "MZ"
            pl.OType = "dN"
            pl.Name = pl.PType + ix.ToString
            pl.LBLName = ""
            pl.Goods = 0
            pl.HStatus = 70
            pl.X = 0
            pl.Y = 0
            objst = Findcntl(pl.OType, ix, pnlMain, cntlfnd)
            If cntlfnd Then
                GetDocDta(objst, pl.X, pl.Y)
            End If
        Next
        For ix = 1 To 21
            plx = plx + 1
            pl = PlacesList(plx)
            pl.PType = "SP"
            pl.OType = "dS"
            pl.Name = pl.PType + ix.ToString
            pl.LBLName = "lbls" + ix.ToString

            pl.Goods = 0
            pl.HStatus = 70
            pl.X = 0
            pl.Y = 0
            objst = Findcntl(pl.OType, ix, pnlMain, cntlfnd)
            If cntlfnd Then
                GetDocDta(objst, pl.X, pl.Y)
            End If
        Next
        For ix = 1 To 3
            plx = plx + 1
            pl = PlacesList(plx)
            pl.PType = "WT"
            pl.OType = "dW"
            pl.Name = pl.PType + ix.ToString
            pl.LBLName = "lblwt" + ix.ToString

            pl.Goods = 0
            pl.HStatus = 70
            pl.X = 0
            pl.Y = 0
            objst = Findcntl(pl.OType, ix, pnlMain, cntlfnd)
            If cntlfnd Then
                GetDocDta(objst, pl.X, pl.Y)
            End If
        Next
        For ix = 1 To 7
            plx = plx + 1
            pl = PlacesList(plx)
            pl.PType = "DR"
            pl.OType = "dD"
            pl.Name = pl.PType + ix.ToString
            pl.LBLName = "lbld" + ix.ToString

            pl.Goods = 0
            pl.HStatus = 70
            pl.X = 0
            pl.Y = 0
            objst = Findcntl(pl.OType, ix, pnlMain, cntlfnd)
            If cntlfnd Then
                GetDocDta(objst, pl.X, pl.Y)
            End If
        Next
        For ix = 1 To 1
            plx = plx + 1
            pl = PlacesList(plx)
            pl.PType = "LP"
            pl.OType = "dP"
            pl.Name = pl.PType + ix.ToString
            pl.LBLName = "lblp" + ix.ToString
            pl.Goods = 0
            pl.HStatus = 70
            pl.X = 0
            pl.Y = 0
            objst = Findcntl(pl.OType, ix, pnlMain, cntlfnd)
            If cntlfnd Then
                GetDocDta(objst, pl.X, pl.Y)
            End If
        Next

    End Sub
    Private Sub LoadCfgFiles()
        Call loadCull(arrray, txtConfigFileName.Text)
        txtGarages.Text = Cull(arrray, "Garages", "Garage")
        txtMainPath.Text = pathCheck(Cull(arrray, "MainPath", "C:\Files\Droid\"))
        LoadListbox(pathCheck(txtMainPath.Text) + txtGarages.Text, LstPGMs)
    End Sub
    Private Sub LoadPrograms()
        Dim st, su, lst As String
        Dim iy, rc As Integer
        lstG1pgm.Items.Clear()
        lstG2pgm.Items.Clear()
        lstG3pgm.Items.Clear()
        Call LoadCfgFiles()

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
            Call PARSE(st, lst, "|", rc)
            dr.DroidType = Val(lst)         'Droid Type
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
            Call PARSE(st, lst, "|", rc)
            dr.DroidType = Val(lst)         'Droid Type
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
            Call PARSE(st, lst, "|", rc)
            dr.DroidType = Val(lst)         'Droid Type
            dr.OGpgm = st                   'OG Program
            Call PARSE(st, lst, "|", rc)
            dr.RemainingPGM = st            'Next Part of Program
            dr.Dest = lst                   'Destination (Current Operation)
            Call GetDest(dr)
            dr.Enabled = True
        Next

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
        '
        ' This timer is to make the Flashing lights on the landing pad work
        ' It also displays either the Empty Pad or a Pad with a Rocket on the Pad
        '
        If RocketOnPad Then
            pad2a.Visible = Not pad2a.Visible   'The Launch Pad with a Rocket
            pad2b.Visible = True
            pad1a.Visible = False
            pad1b.Visible = False
        Else
            pad1a.Visible = Not pad1a.Visible   'The Launch Pad without a Rocket
            pad1b.Visible = True
            pad2a.Visible = False
            pad2b.Visible = False
        End If

    End Sub
    Private Sub MoveAllDroids()
        For drds = 1 To NoOfDroids
            dr = droidlist(drds)
            DroidTemperature(dr)
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
        ' droid types 0=Maintenance 1=SolarRep 2=DrillRep 3=LaunchRep 4=WTRep 5=MfgRep 6=Mining
        '
        pnt.X = dr.X - 15 : pnt.Y = dr.Y - 15
        If drw Then
            cntr.X = 15 : cntr.Y = 15
            Select Case dr.DroidType
                Case 0          'Maintenance Droid
                    If Mid(dr.Name, 7) <> txtDroid.Text Then
                        G.DrawImage(RotateImage(imgListD.Images(0), cntr, Val(dr.Dir).ToString), pnt)
                    Else
                        G.DrawImage(RotateImage(imgListD.Images(1), cntr, Val(dr.Dir).ToString), pnt)
                    End If
                Case 1          'Mining Full Droid
                    If Mid(dr.Name, 7) <> txtDroid.Text Then
                        G.DrawImage(RotateImage(imgListD.Images(2), cntr, Val(dr.Dir).ToString), pnt)
                    Else
                        G.DrawImage(RotateImage(imgListD.Images(3), cntr, Val(dr.Dir).ToString), pnt)
                    End If
                Case 2          'Mining Empty Droid
                    If Mid(dr.Name, 7) <> txtDroid.Text Then
                        G.DrawImage(RotateImage(imgListD.Images(4), cntr, Val(dr.Dir).ToString), pnt)
                    Else
                        G.DrawImage(RotateImage(imgListD.Images(5), cntr, Val(dr.Dir).ToString), pnt)
                    End If
                Case 3          'Solar Repair Droid
                    If Mid(dr.Name, 7) <> txtDroid.Text Then
                        G.DrawImage(RotateImage(imgListD.Images(6), cntr, Val(dr.Dir).ToString), pnt)
                    Else
                        G.DrawImage(RotateImage(imgListD.Images(7), cntr, Val(dr.Dir).ToString), pnt)
                    End If
                Case 4          'Drill Repair Droid
                    If Mid(dr.Name, 7) <> txtDroid.Text Then
                        G.DrawImage(RotateImage(imgListD.Images(8), cntr, Val(dr.Dir).ToString), pnt)
                    Else
                        G.DrawImage(RotateImage(imgListD.Images(9), cntr, Val(dr.Dir).ToString), pnt)
                    End If
                Case 5          'Water Tower Repair Droid
                    If Mid(dr.Name, 7) <> txtDroid.Text Then
                        G.DrawImage(RotateImage(imgListD.Images(10), cntr, Val(dr.Dir).ToString), pnt)
                    Else
                        G.DrawImage(RotateImage(imgListD.Images(11), cntr, Val(dr.Dir).ToString), pnt)
                    End If
                Case 6          'Water Tower Repair Droid
                    If Mid(dr.Name, 7) <> txtDroid.Text Then
                        G.DrawImage(RotateImage(imgListD.Images(12), cntr, Val(dr.Dir).ToString), pnt)
                    Else
                        G.DrawImage(RotateImage(imgListD.Images(13), cntr, Val(dr.Dir).ToString), pnt)
                    End If
                Case Else       ' unknown Droid type
                    If Mid(dr.Name, 7) <> txtDroid.Text Then
                        G.DrawImage(RotateImage(imgListD.Images(0), cntr, Val(dr.Dir).ToString), pnt)
                    Else
                        G.DrawImage(RotateImage(imgListD.Images(1), cntr, Val(dr.Dir).ToString), pnt)
                    End If
            End Select

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
            Call WhatsGoingOn(dr)

        End If

        angl = dr.Dir
        Speed = dr.Speed
        DrdPoint.X = dr.X
        DrdPoint.Y = dr.Y
        DrdVector.X = Speed * Math.Sin(DegtoRad(180 - angl)) + DrdPoint.X
        DrdVector.Y = Speed * Math.Cos(DegtoRad(180 - angl)) + DrdPoint.Y
        If DrdVector.X < 0 Then DrdVector.X = 0
        If DrdVector.Y < 0 Then DrdVector.Y = 0
        If DrdVector.X > ScreenSize Then DrdVector.X = ScreenSize
        If DrdVector.Y > ScreenSize Then DrdVector.Y = ScreenSize

        dr.X = DrdVector.X
        dr.Y = DrdVector.Y


    End Sub
    Private Sub WhatsGoingOn(ByRef dr As DroidStruct)
        Dim st As String
        Dim ix As Integer
        Dim fnd As Boolean

        '
        ' We Have arrived at ???
        '
        st = Mid(dr.Dest, 1, 2)
        Select Case st
            Case "WP"
                Call GetNexDest(dr)
            Case "SP", "LP", "DR", "WT", "HO", "MF", "MZ"

                fnd = False
                For ix = 1 To NoOfPlaces
                    pl = PlacesList(ix)
                    If pl.Name = dr.Dest Then
                        fnd = True
                        Exit For
                    End If
                Next
                If Not fnd Then
                    Call GetNexDest(dr)
                Else
                    dr.Connected = True
                    dr.ConnectionName = dr.Dest
                    dr.Dir = 180
                    dr.Speed = 0
                    pl.HStatus = pl.HStatus + 10
                    If pl.HStatus > 100 Then
                        pl.HStatus = 100
                        dr.Connected = False
                        dr.ConnectionName = ""
                        Call GetNexDest(dr)
                    Else
                        'Stay connected
                        fnd = False
                    End If
                End If
            Case Else
                Call GetNexDest(dr)
        End Select

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
        '
        ' This routine Fills in the small map on the left side of the screen
        '
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
                px = px * (noOfPix / ScreenSize) : py = py * (noOfPix / ScreenSize)
                M.FillRectangle(myBr, px, py, 4, 4)
            End If
            objmap = Findcntl("picBase", ix, pnlMain, cntlfnd)
            If cntlfnd Then
                myBr.Color = Color.DarkGray
                ps = objmap.Name
                px = objmap.location.x : py = objmap.location.y
                px = px * (noOfPix / ScreenSize) : py = py * (noOfPix / ScreenSize)
                M.FillRectangle(myBr, px, py, 4, 4)
            End If
            objmap = Findcntl("pwt", ix, pnlMain, cntlfnd)
            If cntlfnd Then
                myBr.Color = Color.DodgerBlue
                ps = objmap.Name
                px = objmap.location.x : py = objmap.location.y
                px = px * (noOfPix / ScreenSize) : py = py * (noOfPix / ScreenSize)
                M.FillRectangle(myBr, px, py, 4, 4)
            End If
            objmap = Findcntl("pmz", ix, pnlMain, cntlfnd)
            If cntlfnd Then
                myBr.Color = Color.DimGray
                ps = objmap.Name
                px = objmap.location.x : py = objmap.location.y
                px = px * (noOfPix / ScreenSize) : py = py * (noOfPix / ScreenSize)
                M.FillRectangle(myBr, px, py, 4, 4)
            End If
            objmap = Findcntl("pd", ix, pnlMain, cntlfnd)
            If cntlfnd Then
                myBr.Color = Color.Green
                ps = objmap.Name
                px = objmap.location.x : py = objmap.location.y
                px = px * (noOfPix / ScreenSize) : py = py * (noOfPix / ScreenSize)
                M.FillRectangle(myBr, px, py, 4, 4)
            End If
        Next

        For drd = 1 To NoOfDroids
            dr = droidlist(drd)
            myBr.Color = Color.Red
            px = dr.X : py = dr.Y
            px = px * (noOfPix / ScreenSize) : py = py * (noOfPix / ScreenSize)
            M.FillRectangle(myBr, px, py, 2, 2)
        Next

    End Sub
    Public Function DegtoRad(Degg As Integer) As Single
        DegtoRad = Degg * (Math.PI / 180)
    End Function
    Public Function Findcntl(ByVal objName As String, ByVal objIndx As Integer, ByVal root As Control, ByRef fnd As Boolean) As Object
        Dim panels(100) As String
        Dim panelcnt As Integer = 0
        Dim pnlstr As String = ""

        Dim oxj As Object
        pnlstr = objName + Trim(Str(objIndx))
        If objIndx = 0 Then
            pnlstr = objName
        End If
        fnd = False
        If pnlstr = "" Then Exit Function
        oxj = Me.Controls.Find(pnlstr, True)
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
            Case Else   '  "C"
                X = pic.Location.X + pic.Size.Width / 2
                Y = pic.Location.Y + pic.Size.Height / 2
        End Select
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
    Private Sub RealWorld()
        Dim ccc1 As Single
        Dim fnd As Boolean
        Dim iv As Integer
        Dim objHStat As New System.Object
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

        If LowTemp! > OutsideTemp Then LowTemp! = OutsideTemp
        If HighTemp! < OutsideTemp Then HighTemp! = OutsideTemp

        ToolTip1.SetToolTip(lblOTemp, " Highest:" + Str(HighTemp!) + "   Lowest:" + Str(LowTemp!))
        ToolTip1.SetToolTip(lblTemptxt, " Highest:" + Str(HighTemp!) + "   Lowest:" + Str(LowTemp!))
        'Call TimeViewSet(TOD!, outsidetemp)
        '
        'Display Health Status
        '

        For ix = 1 To NoOfPlaces
            pl = PlacesList(ix)
            fnd = False
            Select Case pl.PType
                Case "SP", "LP", "DR", "WT", "MF", "MZ"
                    iv = Val(Mid(pl.Name, 3))

                    objHStat = Findcntl(pl.LBLName, 0, pnlMain, fnd)
                    If fnd Then
                        objHStat.text = pl.HStatus.ToString
                    End If
                Case Else
            End Select
        Next
    End Sub
    Private Sub DisplayDroid()
        Dim drd As Integer
        Dim dty(20) As String
        dty(0) = "Maintenance"
        dty(1) = "Full Miner"
        dty(2) = "Empty Miner"
        dty(3) = "Solar Repair"
        dty(4) = "Drill Repair"
        dty(5) = "Water T Repair"
        dty(6) = "L Pad Repair"

        drd = Val(txtDroid.Text)
        dr = droidlist(drd)

        txtGarge.Text = dr.Garage
        txtType.Text = dty(dr.DroidType)
        txtDest.Text = dr.Dest
        txtPGM.Text = dr.RemainingPGM
        txtOGpgm.Text = dr.OGpgm
        txtTow.Text = dr.Tow
        txtXPOS.Text = dr.X.ToString
        txtYPOS.Text = dr.Y.ToString
        txtSpeed.Text = dr.Speed.ToString
        txtANGL.Text = dr.Dir.ToString
        txtETemp.Text = dr.ETemp.ToString
        txtITemp.Text = dr.ITemp.ToString
        txtEnvCnt.Text = dr.EnvSet.ToString
    End Sub
    Private Sub DroidTemperature(ByRef dr As DroidStruct)
        Dim et, it, dt, hc As Single
        Dim ev As Integer
        et = dr.ETemp
        it = dr.ITemp
        ev = dr.EnvSet
        '
        ' Handle Droid external skin Temp
        '
        dt = et - OutsideTemp
        If dt > 0 Then                      ' is it Hot outside?
            If Math.Abs(dt) < coolval Then
                et = OutsideTemp            'leave et alone
            Else
                et = et - (dt / 200)        'skin temp will get colder
            End If
        ElseIf (dt < 0) Then
            If Math.Abs(dt) < heatval Then
                et = OutsideTemp            'leave et alone
            Else
                et = et + (Math.Abs(dt) / 200) 'skin temp will get hotter 
            End If
        End If
        '
        ' Handle Droid Internal Skin temp
        '
        dt = et - it                    ' Temp difference
        txtDiff.Text = dt.ToString
        it = it + (dt / 60)            ' ext temp effects internal temp
        Select Case it
            Case < -30
                ev = 10
            Case < -20
                ev = 9
            Case < 0
                ev = 8
            Case < 30
                ev = 7
            Case < 50
                ev = 6
            Case < 60
                ev = 5
            Case < 64
                ev = 4
            Case < 66
                ev = 3
            Case < 68
                ev = 2
            Case < 70
                ev = 1
            Case > 250
                ev = -20
            Case > 200
                ev = -16
            Case > 170
                ev = -14
            Case > 100
                ev = -12
            Case > 90
                ev = -10
            Case > 80
                ev = -8
            Case > 78
                ev = -6
            Case > 74
                ev = -3
            Case > 72
                ev = -2
            Case > 70
                ev = -1
            Case Else
                ev = 0
        End Select
        hc = 1 * ev

        If dr.Batt > 20 Then
            it = it + hc!
            dr.Batt = dr.Batt - DroidEnvDrain - Math.Abs(hc! / 10)
        End If
        dr.ETemp = et
        dr.ITemp = it
        dr.EnvSet = ev
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
            Case "MZ"   ' Mining Zone
                statn = "dN"
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
        If dx = 0 And dy = 0 Then
            dx = 0
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

    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click
        G.Clear(bgColor)
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
    Private Sub pnlMap_MouseDown(sender As Object, ByVal e As MouseEventArgs) Handles pnlMap.MouseDown
        Dim px, py As Integer
        Dim pnt As Point
        'pnt.X = xPos
        px = e.Location.X : py = e.Location.Y
        pnt.X = px * (ScreenSize / noOfPix) : pnt.Y = py * (ScreenSize / noOfPix)

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
            Case "MineZone"
                statn = "dN"
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
    Private Sub dM_Click(sender As Object, e As EventArgs) Handles dM1.Click, dM2.Click
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
    Private Sub dP1_Click(sender As Object, e As EventArgs) Handles dP1.Click
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
    Private Sub dN_Click(sender As Object, e As EventArgs) Handles dN1.Click, dN2.Click, dN3.Click
        'Mining Zone port
        Dim nm, tg As String
        Dim mnz As Integer
        nm = sender.name
        tg = sender.tag
        mnz = Val(Mid(nm, 3))

        selStation.Text = "MineZone " + Trim(Str(mnz))

        If Mapping = True Then
            txtPGMmap.Text = txtPGMmap.Text + "|MZ" + mnz.ToString
        End If

    End Sub
    Private Sub dD_Click(sender As Object, e As EventArgs) Handles dD1.Click, dD2.Click, dD3.Click, dD4.Click, dD5.Click, dD6.Click, dD7.Click
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
        wp56.Click, wp57.Click, wp58.Click, wp59.Click, wp60.Click, wp61.Click, wp62.Click, wp63.Click, wp64.Click, wp65.Click,
        wp66.Click, wp67.Click, wp68.Click, wp69.Click, wp70.Click, wp71.Click, wp72.Click, wp73.Click, wp74.Click, wp75.Click,
        wp76.Click, wp77.Click, wp78.Click, wp79.Click, wp80.Click ', wp81.Click, wp82.Click, wp83.Click, wp84.Click, wp85.Click,
        'wp86.Click, wp87.Click, wp88.Click, wp89.Click

        'way point
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
    Private Sub btnShowStatus_Click(sender As Object, e As EventArgs) Handles btnShowStatus.Click
        frmDroids.Show()
    End Sub
    Private Sub SelDroid_SelectedIndexChanged(sender As Object, e As EventArgs) Handles SelDroid.SelectedIndexChanged
        Dim dd As String
        dd = SelDroid.Text
        txtDroid.Text = Mid(dd, 7)
        DisplayDroid()
    End Sub
    Private Sub chkRocket_CheckedChanged(sender As Object, e As EventArgs) Handles chkRocket.CheckedChanged
        RocketOnPad = chkRocket.Checked
    End Sub
End Class
Public Class DroidStruct
    Public Name As String           ' Droid X
    Public Enabled As Boolean       ' Droid enable/disable switch
    Public DroidType As Integer     ' type of Droid                 0=Maintenance 1=SolarRep 2=DrillRep 3=LaunchRep 4=WTRep 5=MfgRep 6=Mining
    Public X As Integer             ' X Coordinate                  
    Public Y As Integer             ' Y Coordinate
    Public Status As String         ' Droid Status                       Env Mtr Con
    '                               '                                    Off Off Dis
    Public BaseX As Single          ' Home Base X Position
    Public BaseY As Single          ' Home Base Y Position

    Public Dest As String           ' Droid Destination Name
    Public DestX As Integer         ' Droid Destination X-Coord
    Public DestY As Integer         ' Droid Destination Y-Coord

    Public Speed As Integer         ' Speed setting (0-7)
    Public Dir As Single            ' Travel Direction
    Public Vel As Single            ' Travel Velocity
    Public Batt As Integer          ' Battery Power 0-100
    Public ETemp As Single          ' Ext skin temp
    Public ITemp As Single          ' internal temp
    Public EnvSet As Integer        ' Environmental Control setting
    Public PrdOnBoard As Integer    ' Product Capacity

    Public Connected As Boolean     ' Connected to a station
    Public ConnectionName As String ' Conected Device

    Public RemainingPGM As String   ' remaining Pgm string
    Public OGpgm As String          ' Original Program
    Public Exec As String           ' Current Program executing

    Public Tow As String            ' Droid is towing another Droid (droid in tow)

    Public Garage As String         ' Droid Garage Assignment
End Class
Public Class PlaceStruct
    Public Name As String           ' Place Name
    Public PType As String          ' type of Place     HO=Garage SP=SolarPanel DR=Drill LP=LaunchPad WP=WaterTower MF=MFG MZ=MiningZone
    Public OType As String          ' object type       dG=Garage dS=SolarPanel dD=Drill dP=LaunchPad dW=WaterTower dM=MFG dN=MiningZone
    Public LBLName As String        ' name of the status display label
    Public X As Integer             ' X Coordinate                  
    Public Y As Integer             ' Y Coordinate
    Public HStatus As Single        ' Health Status 
    Public Goods As Integer         ' How many goods produced or needed 
    '
End Class
