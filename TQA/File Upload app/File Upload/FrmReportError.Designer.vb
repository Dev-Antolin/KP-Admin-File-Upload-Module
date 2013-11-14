<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrmReportError
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
        Me.FileUploadErrorRptVwr = New CrystalDecisions.Windows.Forms.CrystalReportViewer
        Me.SuspendLayout()
        '
        'FileUploadErrorRptVwr
        '
        Me.FileUploadErrorRptVwr.ActiveViewIndex = -1
        Me.FileUploadErrorRptVwr.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.FileUploadErrorRptVwr.DisplayGroupTree = False
        Me.FileUploadErrorRptVwr.Dock = System.Windows.Forms.DockStyle.Fill
        Me.FileUploadErrorRptVwr.Location = New System.Drawing.Point(0, 0)
        Me.FileUploadErrorRptVwr.Name = "FileUploadErrorRptVwr"
        Me.FileUploadErrorRptVwr.SelectionFormula = ""
        Me.FileUploadErrorRptVwr.ShowCloseButton = False
        Me.FileUploadErrorRptVwr.ShowGroupTreeButton = False
        Me.FileUploadErrorRptVwr.ShowRefreshButton = False
        Me.FileUploadErrorRptVwr.ShowZoomButton = False
        Me.FileUploadErrorRptVwr.Size = New System.Drawing.Size(816, 552)
        Me.FileUploadErrorRptVwr.TabIndex = 1
        Me.FileUploadErrorRptVwr.ViewTimeSelectionFormula = ""
        '
        'FrmReportError
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(816, 552)
        Me.Controls.Add(Me.FileUploadErrorRptVwr)
        Me.Name = "FrmReportError"
        Me.Text = "FrmReportError"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents FileUploadErrorRptVwr As CrystalDecisions.Windows.Forms.CrystalReportViewer
End Class
