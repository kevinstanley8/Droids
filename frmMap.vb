Imports System.DirectoryServices.ActiveDirectory
Imports System.Security.Cryptography.Xml

Public Class frmMap
    Public M As Graphics
    Const NoOfPix = 820
    Private dr1 As DroidStruct
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

    Private Sub btmClose_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub frmMap_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        M = pnlMap.CreateGraphics()
        tmrMap.Enabled = True
    End Sub
    Private Sub DrawMap()
        Dim objmap As System.Object
        Dim px, py, sx, sy As Integer
        Dim cntlfnd As Boolean
        Dim ps As String
        Dim myBr As New SolidBrush(Color.Blue)
        M.Clear(pnlMap.BackColor)
        cntlfnd = False
        For ix = 1 To 100
            'ps Solar Panel
            objmap = Findcntl("ps", ix, frmMain.pnlMain, cntlfnd)
            If cntlfnd Then
                myBr.Color = Color.Blue
                ps = objmap.Name
                px = objmap.location.x : py = objmap.location.y
                px = px * (NoOfPix / 7000) : py = py * (NoOfPix / 7000)
                sx = objmap.size.width : sy = objmap.size.height
                sx = sx * (NoOfPix / 7000) : sy = sy * (NoOfPix / 7000)
                M.FillRectangle(myBr, px, py, sx, sy)
            End If
            'pb Base/garage
            objmap = Findcntl("pb", ix, frmMain.pnlMain, cntlfnd)
            If cntlfnd Then
                myBr.Color = Color.DarkGray
                ps = objmap.Name
                px = objmap.location.x : py = objmap.location.y
                px = px * (NoOfPix / 7000) : py = py * (NoOfPix / 7000)
                sx = objmap.size.width : sy = objmap.size.height
                sx = sx * (NoOfPix / 7000) : sy = sy * (NoOfPix / 7000)
                M.FillRectangle(myBr, px, py, sx, sy)
            End If
            'pwt Water Tower
            objmap = Findcntl("pwt", ix, frmMain.pnlMain, cntlfnd)
            If cntlfnd Then
                myBr.Color = Color.DodgerBlue
                ps = objmap.Name
                px = objmap.location.x : py = objmap.location.y
                px = px * (NoOfPix / 7000) : py = py * (NoOfPix / 7000)
                sx = objmap.size.width : sy = objmap.size.height
                sx = sx * (NoOfPix / 7000) : sy = sy * (NoOfPix / 7000)
                M.FillRectangle(myBr, px, py, sx, sy)
            End If
            'pada  Landing Pad
            objmap = Findcntl("pada", ix, frmMain.pnlMain, cntlfnd)
            If cntlfnd Then
                myBr.Color = Color.Orange
                ps = objmap.Name
                px = objmap.location.x : py = objmap.location.y
                px = px * (NoOfPix / 7000) : py = py * (NoOfPix / 7000)
                sx = objmap.size.width : sy = objmap.size.height
                sx = sx * (NoOfPix / 7000) : sy = sy * (NoOfPix / 7000)
                M.FillRectangle(myBr, px, py, sx, sy)
            End If
            'pm Manuf
            objmap = Findcntl("pm", ix, frmMain.pnlMain, cntlfnd)
            If cntlfnd Then
                myBr.Color = Color.DarkGray
                ps = objmap.Name
                px = objmap.location.x : py = objmap.location.y
                px = px * (NoOfPix / 7000) : py = py * (NoOfPix / 7000)
                sx = objmap.size.width : sy = objmap.size.height
                sx = sx * (NoOfPix / 7000) : sy = sy * (NoOfPix / 7000)
                M.FillRectangle(myBr, px, py, sx, sy)
            End If
            'pd Drill
            objmap = Findcntl("pd", ix, frmMain.pnlMain, cntlfnd)
            If cntlfnd Then
                myBr.Color = Color.Green
                ps = objmap.Name
                px = objmap.location.x : py = objmap.location.y
                px = px * (NoOfPix / 7000) : py = py * (NoOfPix / 7000)
                sx = objmap.size.width : sy = objmap.size.height
                sx = sx * (NoOfPix / 7000) : sy = sy * (NoOfPix / 7000)
                M.FillRectangle(myBr, px, py, sx, sy)
            End If
            'wp Way Point
            objmap = Findcntl("wp", ix, frmMain.pnlMain, cntlfnd)
            If cntlfnd Then
                myBr.Color = Color.Fuchsia
                ps = objmap.Name
                px = objmap.location.x : py = objmap.location.y
                px = px * (NoOfPix / 7000) : py = py * (NoOfPix / 7000)
                sx = objmap.size.width : sy = objmap.size.height
                sx = sx * (NoOfPix / 7000) : sy = sy * (NoOfPix / 7000)
                M.FillRectangle(myBr, px, py, 2, 2)
            End If



        Next
        For drd = 1 To frmMain.NoOfDroids
            dr1 = frmMain.droidlist(drd)
            myBr.Color = Color.Red
            px = dr1.X : py = dr1.Y
            px = px * (NoOfPix / 7000) : py = py * (NoOfPix / 7000)
            M.FillRectangle(myBr, px, py, 3, 3)
        Next

    End Sub

    Private Sub tmrMap_Tick(sender As Object, e As EventArgs) Handles tmrMap.Tick
        Call DrawMap()
    End Sub

    Private Sub frmMap_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        tmrMap.Enabled = False
    End Sub
End Class