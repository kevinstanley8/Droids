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
        btmClose = New Button()
        tmrMap = New Timer(components)
        SuspendLayout()
        ' 
        ' pnlMap
        ' 
        pnlMap.BackColor = Color.White
        pnlMap.Location = New Point(24, 11)
        pnlMap.Name = "pnlMap"
        pnlMap.Size = New Size(500, 500)
        pnlMap.TabIndex = 0
        ' 
        ' btmClose
        ' 
        btmClose.Location = New Point(219, 520)
        btmClose.Name = "btmClose"
        btmClose.Size = New Size(118, 29)
        btmClose.TabIndex = 1
        btmClose.Text = "Close"
        btmClose.UseVisualStyleBackColor = True
        ' 
        ' tmrMap
        ' 
        tmrMap.Interval = 200
        ' 
        ' frmMap
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(547, 561)
        Controls.Add(btmClose)
        Controls.Add(pnlMap)
        Name = "frmMap"
        Text = "frmMap"
        ResumeLayout(False)
    End Sub

    Friend WithEvents pnlMap As Panel
    Friend WithEvents btmClose As Button
    Friend WithEvents tmrMap As Timer
End Class
