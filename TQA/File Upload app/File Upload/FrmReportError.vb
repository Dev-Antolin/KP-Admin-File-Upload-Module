Imports System.IO
Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared
Public Class FrmReportError
    Inherits System.Windows.Forms.Form
    Public orpt As ReportDocument
    Public ds As New DataSet
    Dim rptDirectory As String
    Dim objDiskOpt As New DiskFileDestinationOptions()
    Dim objExOpt As ExportOptions = Nothing
    Dim req_rpt As New rptFileUploadError

    Public Sub CollectData(ByVal arrSuccess As List(Of String), ByVal FType As String, ByVal partner As String, ByVal mlbatchnum As String, ByVal partnersbtchnum As String, ByVal OldAccId As String, ByVal Opid As String)
        Dim dt As New DataTable()
        Dim dt1 As New DataTable()
        Dim dt2 As New DataTable()
        Dim dt3 As New DataTable()
        Dim datetime1 As String = Date.Now().ToString("MMMM dd,yyyy hh:mm:ss tt")
        Dim mergeTable As New DataTable("MergeTable")

        req_rpt.Load()


        dt3 = RptDTSuccess(arrSuccess, "InsertSuccess", dt3, FType, partner, OldAccId)

        dt.Merge(dt1)
        dt.Merge(dt2)
        dt.Merge(dt3)

        mergeTable = dt

        req_rpt.SetDataSource(mergeTable)
        req_rpt.SetParameterValue("PartnersBatchNumber", partnersbtchnum)
        req_rpt.SetParameterValue("MLbatchNo", mlbatchnum)
        req_rpt.SetParameterValue("partnersname", partner)
        req_rpt.SetParameterValue("OperatorId", Opid)
        req_rpt.SetParameterValue("Date", datetime1)
        FileUploadErrorRptVwr.ReportSource = req_rpt

        'Dim rptDocument As New ReportDocument()
        'Dim objDiskOpt As New DiskFileDestinationOptions()
        'Dim objExOpt As ExportOptions = Nothing

        ''Dim Fname As String = Application.StartupPath + "\reports\"
        Dim Fname As String = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\FileUpload\" + partner + "\"
        Dim tDate As Date = Date.Now
        Dim month As String = tDate.Month.ToString
        Dim day As String = tDate.Day.ToString
        Dim year As String = tDate.Year.ToString.Substring(2, 2)
        Dim dtime As DateTime = DateTime.Now
        'Dim time As String = dtime.ToString("hh:mm:ss")
        Dim time As DateTime = DateTime.Now
        Dim format As String = "hh:mm:ss"
        Dim timer As String = time.ToString(format)


        makefolder()
        makeSubFolder(partner)

        objDiskOpt.DiskFileName = Fname + partner.Replace("/", "_") + "-" + month + day + year + "-" + timer.Replace(":", "") + "-Error.pdf"
        objExOpt = req_rpt.ExportOptions
        objExOpt.ExportDestinationType = ExportDestinationType.DiskFile
        objExOpt.ExportFormatType = ExportFormatType.PortableDocFormat
        objExOpt.DestinationOptions = objDiskOpt
        req_rpt.Export()
        'orpt.Close()
        'orpt.Dispose()

    End Sub
    Public Sub makefolder()
        If Not System.IO.File.Exists(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) + "\FileUpload\") Then
            System.IO.Directory.CreateDirectory(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) + "\FileUpload\")
        End If
    End Sub
    Private Sub makeSubFolder(ByRef PartnersName As String)
        If Not System.IO.File.Exists(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) + "\FileUpload\" + PartnersName + "\") Then
            System.IO.Directory.CreateDirectory(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) + "\FileUpload\" + PartnersName + "\")
        End If
    End Sub
    Private Function RptDT(ByVal arr() As String, ByVal table As String, ByVal param1 As String, ByVal paramDt As DataTable, ByVal filetype As String, ByVal PartnersName As String, ByVal TagAccount As String) As DataTable

        paramDt = ds.Tables.Add(table)
        Dim ID As New DataColumn("ID", GetType(System.Int32))
        Dim refno As New DataColumn(param1, GetType(System.String))

        paramDt.Columns.Add(ID)
        paramDt.Columns.Add(refno)

        paramDt.PrimaryKey = New DataColumn() {ID}
        Dim var As Integer = 0
        Dim dtr As DataRow = Nothing

        For Each item As String In arr

            var = var + 1
            dtr = paramDt.NewRow()

            Dim datasplit() As String = item.Split("|"c)
            dtr("ID") = var
            Dim dimle As Integer = datasplit.Length
            If filetype = "TEXT" And dimle <= 1 Then
                dtr(param1) = datasplit(0).ToString()
            ElseIf filetype = "TEXT" And dimle > 1 Then
                If TagAccount = "FSD7" Then
                    dtr(param1) = datasplit(0).ToString
                ElseIf TagAccount = "FSD35" Then
                    dtr(param1) = datasplit(0).ToString
                ElseIf TagAccount = "BPINOY" Then
                    dtr(param1) = datasplit(2).ToString
                ElseIf TagAccount = "MOEX" Then
                    dtr(param1) = datasplit(0).ToString
                Else
                    dtr(param1) = datasplit(3).ToString
                End If
            ElseIf filetype = "EXCEL" And dimle > 1 Then
                dtr(param1) = datasplit(2).ToString()
            ElseIf filetype = "EXCEL" And dimle <= 1 Then
                dtr(param1) = datasplit(0).ToString()
            End If

            paramDt.Rows.Add(dtr)
            paramDt.AcceptChanges()

        Next
        Return paramDt
    End Function
    Private Function RptDTSuccess(ByVal arr As List(Of String), ByVal table As String, ByVal paramDt As DataTable, ByVal filetype As String, ByVal PartnersName As String, ByVal T_Acc As String) As DataTable

        paramDt = ds.Tables.Add(table)
        Dim ID As New DataColumn("ID", GetType(System.Int32))
        Dim kptn As New DataColumn("kptn", GetType(System.String))
        Dim refno As New DataColumn("refno", GetType(System.String))
        Dim SenderName As New DataColumn("Sname", GetType(System.String))
        Dim ReceiverName As New DataColumn("Rname", GetType(System.String))
        Dim Currency As New DataColumn("Currency", GetType(System.String))
        Dim Principal As New DataColumn("PAmount", GetType(System.Decimal))
        Dim Charges As New DataColumn("Charges", GetType(System.Decimal))
        Dim ErrorMsg As New DataColumn("Error", GetType(System.String))

        paramDt.Columns.Add(ID)
        paramDt.Columns.Add(kptn)
        paramDt.Columns.Add(refno)
        paramDt.Columns.Add(SenderName)
        paramDt.Columns.Add(ReceiverName)
        paramDt.Columns.Add(Currency)
        paramDt.Columns.Add(Principal)
        paramDt.Columns.Add(Charges)
        paramDt.Columns.Add(ErrorMsg)

        paramDt.PrimaryKey = New DataColumn() {ID}
        Dim var As Integer = 0
        Dim dtr As DataRow = Nothing

        For Each item As String In arr
            If item IsNot Nothing Then
                var = var + 1
                dtr = paramDt.NewRow()

                Dim datasplit() As String = item.Split("|"c)
                dtr("ID") = var
                Dim dimle As Integer = datasplit.Length
                If filetype = "TEXT" And dimle <= 1 Then
                    dtr("kptn") = ""
                    dtr("refno") = ""
                    dtr("Sname") = ""
                    dtr("Rname") = ""
                    dtr("Currency") = ""
                    dtr("PAmount") = 0
                    dtr("Charges") = 0
                    dtr("Error") = ""
                ElseIf filetype = "TEXT" And dimle > 1 Then
                    If T_Acc = "FSD7" Then
                        dtr("kptn") = datasplit(1).ToString
                        dtr("refno") = datasplit(2).ToString
                        dtr("Sname") = datasplit(3).ToString
                        dtr("Rname") = datasplit(11).ToString
                        dtr("Currency") = "PHP"
                        dtr("PAmount") = System.Convert.ToDecimal(datasplit(10).ToString)
                        dtr("Charges") = System.Convert.ToDecimal(datasplit(15).ToString)
                        dtr("Error") = datasplit(17).ToString
                    ElseIf T_Acc = "FSD35" Then
                        dtr("kptn") = datasplit(1).ToString
                        dtr("refno") = datasplit(0).ToString
                        dtr("Sname") = datasplit(3).ToString + "," + datasplit(4).ToString + " " + datasplit(5).ToString
                        dtr("Rname") = datasplit(6).ToString
                        dtr("Currency") = datasplit(9).ToString
                        dtr("PAmount") = System.Convert.ToDecimal(datasplit(8).ToString)
                        dtr("Charges") = datasplit(20).ToString
                        dtr("Error") = datasplit(22).ToString
                    ElseIf T_Acc = "BPINOY" Then
                        dtr("kptn") = datasplit(1).ToString
                        dtr("refno") = datasplit(0).ToString
                        dtr("Sname") = datasplit(6).ToString + "," + datasplit(7).ToString + " " + datasplit(8).ToString
                        dtr("Rname") = datasplit(9).ToString + "," + datasplit(10).ToString + " " + datasplit(11).ToString
                        dtr("Currency") = datasplit(23).ToString
                        dtr("PAmount") = System.Convert.ToDecimal(datasplit(23).ToString)
                        dtr("Charges") = System.Convert.ToDecimal(datasplit(24).ToString)
                        dtr("Error") = datasplit(25).ToString
                    ElseIf T_Acc = "MOEX" Then
                        dtr("kptn") = datasplit(31).ToString
                        dtr("refno") = datasplit(0).ToString
                        dtr("Sname") = Trim(datasplit(4).ToString) + "," + Trim(datasplit(3).ToString) + " " + Trim(datasplit(5).ToString)
                        dtr("Rname") = Trim(datasplit(7).ToString) + "," + Trim(datasplit(6).ToString) + " " + Trim(datasplit(8).ToString)
                        dtr("Currency") = datasplit(21).ToString
                        dtr("PAmount") = System.Convert.ToDecimal(datasplit(20).ToString)
                        dtr("Charges") = System.Convert.ToDecimal(datasplit(32).ToString)
                        dtr("Error") = datasplit(33).ToString
                    Else
                        dtr("kptn") = datasplit(1).ToString
                        dtr("refno") = datasplit(3).ToString
                        dtr("Sname") = datasplit(10).ToString + "," + datasplit(11).ToString + " " + datasplit(12).ToString
                        dtr("Rname") = datasplit(22).ToString + "," + datasplit(23).ToString + " " + datasplit(24).ToString
                        dtr("Currency") = datasplit(5).ToString
                        dtr("PAmount") = System.Convert.ToDecimal(datasplit(4).ToString)
                        dtr("Charges") = System.Convert.ToDecimal(datasplit(6).ToString)
                        dtr("Error") = datasplit(32).ToString
                    End If
                ElseIf filetype = "EXCEL" And dimle > 1 Then
                    dtr("kptn") = datasplit(0).ToString()
                    dtr("refno") = datasplit(1).ToString()
                    dtr("Sname") = datasplit(2).ToString()
                    dtr("Rname") = datasplit(3).ToString()
                    dtr("Currency") = datasplit(4).ToString()
                    dtr("PAmount") = System.Convert.ToDecimal(datasplit(5).ToString())
                    dtr("Charges") = System.Convert.ToDecimal(datasplit(6).ToString())
                    dtr("Error") = ""
                ElseIf filetype = "EXCEL" And dimle <= 1 Then
                    dtr("kptn") = ""
                    dtr("refno") = ""
                    dtr("Sname") = ""
                    dtr("Rname") = ""
                    dtr("Currency") = ""
                    dtr("PAmount") = 0
                    dtr("Charges") = 0
                    dtr("Error") = ""
                End If

                paramDt.Rows.Add(dtr)
                paramDt.AcceptChanges()
            End If
        Next
        Return paramDt
    End Function
    Private Sub FileUploadRptVwr_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FileUploadErrorRptVwr.Load

    End Sub

    Private Sub FrmReportError_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    End Sub
End Class