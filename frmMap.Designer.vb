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
        chkDroids = New CheckBox()
        SuspendLayout()
        ' 
        ' pnlMap
        ' 
        pnlMap.BackColor = Color.White
        pnlMap.BorderStyle = BorderStyle.FixedSingle
        pnlMap.Location = New Point(91, 3)
        pnlMap.Name = "pnlMap"
        pnlMap.Size = New Size(820, 820)
        pnlMap.TabIndex = 0
        ' 
        ' tmrMap
        ' 
        tmrMap.Interval = 200
        ' 
        ' chkDroids
        ' 
        chkDroids.Appearance = Appearance.Button
        chkDroids.AutoSize = True
        chkDroids.Checked = True
        chkDroids.CheckState = CheckState.Checked
        chkDroids.FlatStyle = FlatStyle.Flat
        chkDroids.Location = New Point(13, 45)
        chkDroids.Name = "chkDroids"
        chkDroids.Size = New Size(70, 25)
        chkDroids.TabIndex = 1
        chkDroids.Text = "Droids On"
        chkDroids.UseVisualStyleBackColor = True
        ' 
        ' frmMap
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(914, 861)
        Controls.Add(chkDroids)
        Controls.Add(pnlMap)
        Name = "frmMap"
        Text = "frmMap"
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents pnlMap As Panel
    Friend WithEvents tmrMap As Timer
    Friend WithEvents chkDroids As CheckBox
End Class
