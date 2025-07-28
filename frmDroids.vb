Imports System.Runtime.InteropServices.JavaScript.JSType

Public Class frmDroids
    Public drd As DroidStruct
    Dim cntlfnd As Boolean
    Private Sub frmDroids_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        tmrScanDroids.Enabled = True
        Me.Left = 0 : Me.Top = 0

    End Sub

    Private Sub tmrScanDroids_Tick(sender As Object, e As EventArgs) Handles tmrScanDroids.Tick
        Dim objst As System.Object
        For ix = 1 To frmMain.NoOfDroids
            drd = frmMain.droidlist(ix)
            objst = frmMain.Findcntl("txtDroid", ix, pnlDroid, cntlfnd)
            If cntlfnd Then
                objst.text = ix.ToString
            End If

            objst = frmMain.Findcntl("txtGarge", ix, pnlDroid, cntlfnd)
            If cntlfnd Then
                objst.text = drd.Garage.ToString
            End If

            objst = frmMain.Findcntl("txtDest", ix, pnlDroid, cntlfnd)
            If cntlfnd Then
                objst.text = drd.Dest
            End If

            objst = frmMain.Findcntl("txtPGM", ix, pnlDroid, cntlfnd)
            If cntlfnd Then
                objst.text = drd.RemainingPGM
            End If

            objst = frmMain.Findcntl("txtOGpgm", ix, pnlDroid, cntlfnd)
            If cntlfnd Then
                objst.text = drd.OGpgm
            End If

            objst = frmMain.Findcntl("txtTow", ix, pnlDroid, cntlfnd)
            If cntlfnd Then
                objst.text = drd.Tow.ToString
            End If

            objst = frmMain.Findcntl("txtXPOS", ix, pnlDroid, cntlfnd)
            If cntlfnd Then
                objst.text = drd.X.ToString
            End If

            objst = frmMain.Findcntl("txtYPOS", ix, pnlDroid, cntlfnd)
            If cntlfnd Then
                objst.text = drd.Y.ToString
            End If

            objst = frmMain.Findcntl("txtANGL", ix, pnlDroid, cntlfnd)
            If cntlfnd Then
                objst.text = drd.Dir.ToString
            End If

            objst = frmMain.Findcntl("txtSpeed", ix, pnlDroid, cntlfnd)
            If cntlfnd Then
                objst.text = drd.Speed.ToString
            End If
        Next
    End Sub
End Class