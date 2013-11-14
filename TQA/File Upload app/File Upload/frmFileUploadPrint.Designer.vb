<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmFileUploadPrint
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
        Me.FileUploadRptVwr = New CrystalDecisions.Windows.Forms.CrystalReportViewer
        Me.SuspendLayout()
        '
        'FileUploadRptVwr
        '
        Me.FileUploadRptVwr.ActiveViewIndex = -1
        Me.FileUploadRptVwr.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.FileUploadRptVwr.DisplayGroupTree = False
        Me.FileUploadRptVwr.Dock = System.Windows.Forms.DockStyle.Fill
        Me.FileUploadRptVwr.Location = New System.Drawing.Point(0, 0)
        Me.FileUploadRptVwr.Name = "FileUploadRptVwr"
        Me.FileUploadRptVwr.SelectionFormula = ""
        Me.FileUploadRptVwr.ShowCloseButton = False
        Me.FileUploadRptVwr.ShowGroupTreeButton = False
        Me.FileUploadRptVwr.ShowRefreshButton = False
        Me.FileUploadRptVwr.ShowZoomButton = False
        Me.FileUploadRptVwr.Size = New System.Drawing.Size(832, 590)
        Me.FileUploadRptVwr.TabIndex = 0
        Me.FileUploadRptVwr.ViewTimeSelectionFormula = ""
        '
        'frmFileUploadPrint
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(832, 590)
        Me.Controls.Add(Me.FileUploadRptVwr)
        Me.Name = "frmFileUploadPrint"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "File Upload Report"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents FileUploadRptVwr As CrystalDecisions.Windows.Forms.CrystalReportViewer
End Class
