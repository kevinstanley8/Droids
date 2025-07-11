<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMap
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        components = New ComponentModel.Container()
        pnlMap = New Panel()
        tmrMap = New Timer(components)
        SuspendLayout()
        ' 
        ' pnlMap
        ' 
        pnlMap.BackColor = Color.White
        pnlMap.BorderStyle = BorderStyle.FixedSingle
        pnlMap.Location = New Point(0, 0)
        pnlMap.Name = "pnlMap"
        pnlMap.Size = New Size(820, 820)
        pnlMap.TabIndex = 0
        ' 
        ' tmrMap
        ' 
        tmrMap.Interval = 200
        ' 
        ' frmMap
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(864, 861)
        Controls.Add(pnlMap)
        Name = "frmMap"
        Text = "frmMap"
        ResumeLayout(False)
    End Sub

    Friend WithEvents pnlMap As Panel
    Friend WithEvents tmrMap As Timer
End Class
