Imports System.IO
Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared

Public Class frmFileUploadPrint
    Inherits System.Windows.Forms.Form
    Public orpt As ReportDocument
    Public y As List(Of String)
    Dim itemno, refno, kptn, sndrname, rcvrname, charges, pamount, datefiled As String
    Public mlbatchnum As String
    Public partnersbtchnum As String
    Public partnersname As String
    Public curr As String
    Public usdprin As Decimal
    Public phpprin As Decimal
    Public usdC As Decimal
    Public phpC As Decimal
    Public notxtn As Integer
    Public notxnUsd As Integer
    Public integrationtype As String
    Public OpId As String
    Public Time As String
    Dim f As frmUpload
    Dim datetime1 As String = Date.Now().ToString("MMMM dd,yyyy hh:mm:ss tt")
    Dim rsuc_rpt As New rptFileUpload
    Public Sub SetF(ByVal obj As Object)
        f = DirectCast(obj, frmUpload)
    End Sub

    Public Sub passdata(ByVal x As List(Of String), ByVal mlbnum As String, ByVal prtnrbnum As String, ByVal prtnrname As String, ByVal currency As String, ByVal integtype As String, ByVal oldAccid As String, ByVal phpAmount As String, ByVal usdAmount As String, ByVal usdcharge As String, ByVal phpcharge As String, ByVal NtransPhp As String, ByVal NtransUsd As String, ByVal OperId As String)
        Dim asize As Integer
        asize = x.Count
        y = x
        'curr = String.Empty
        curr = currency
        mlbatchnum = mlbnum
        partnersbtchnum = prtnrbnum
        partnersname = prtnrname
        integrationtype = integtype
        usdprin = Convert.ToDecimal(usdAmount)
        phpprin = Convert.ToDecimal(phpAmount)
        usdC = Convert.ToDecimal(usdcharge)
        phpC = Convert.ToDecimal(phpcharge)
        notxtn = Convert.ToInt16(NtransPhp)
        notxnUsd = Convert.ToInt16(NtransUsd)
        OpId = OperId

        'Return x(asize)
        getFileUploadData(oldAccid)


    End Sub

    Public Sub getFileUploadData(ByVal oldAcc As String)
        Dim apppath As New frmFileUploadPrint
        'Dim rptDirectory As String
        'Dim crtableLongoninfo As New TableLogOnInfo
        'Dim crconnectionInfo As New ConnectionInfo
        Dim s As Integer = 0
        Dim z() As String
        Dim x As Integer = 0

        If oldAcc = "BPINOY" And integrationtype = "2" Then
            Try
                'orpt = New ReportDocument
                'rptDirectory = Application.StartupPath & "\\rptFileUpload.rpt"
                'rptDirectory = AppDomain.CurrentDomain.BaseDirectory + "rptFileUpload.rpt"
                'rptDirectory = rptDirectory.Replace("\", "\\")
                'rptDirectory = Trim(rptDirectory)
                rsuc_rpt.Load()
                Dim row As DataRow = Nothing
                Dim ds As New DataSet
                ds.Tables.Add("uploadreport")
                With ds.Tables(0).Columns
                    .Add("itemno", Type.GetType("System.String"))
                    .Add("KPTN", Type.GetType("System.String"))
                    .Add("Refno", Type.GetType("System.String"))
                    .Add("sendersname", Type.GetType("System.String"))
                    .Add("receiversname", Type.GetType("System.String"))
                    .Add("principalamount", Type.GetType("System.Double"))
                    .Add("charges", Type.GetType("System.Double"))
                    .Add("Currency", Type.GetType("System.String"))
                    .Add("AppNum", Type.GetType("System.String"))


                End With

                For x = 0 To y.Count - 1
                    y(x).ToString.Split("|")
                    z = Split(y(x).ToString, "|")
                    row = ds.Tables(0).NewRow
                    ' row(0) = z(2).ToString
                    row(1) = z(1).ToString
                    row(2) = z(0).ToString
                    'row(3) = z(10).ToString + ", " + z(11).ToString + " " + z(12).ToString
                    row(3) = z(6).ToString + ", " + z(7).ToString + " " + z(8).ToString
                    row(4) = z(9).ToString + ", " + z(10).ToString + " " + z(11).ToString
                    row(5) = z(23).ToString
                    row(6) = z(24).ToString()
                    row(7) = z(22).ToString()
                    row(8) = z(2).ToString
                    ds.Tables(0).Rows.Add(row)
                Next

                rsuc_rpt.SetDataSource(ds)

                rsuc_rpt.SetParameterValue("partnerBatchNo", partnersbtchnum)
                rsuc_rpt.SetParameterValue("MLbatchNo", mlbatchnum)
                rsuc_rpt.SetParameterValue("partnersname", partnersname)
                rsuc_rpt.SetParameterValue("Pusd", usdprin)
                rsuc_rpt.SetParameterValue("Pphp", phpprin)
                rsuc_rpt.SetParameterValue("UsdCharge", usdC)
                rsuc_rpt.SetParameterValue("PhpCharge", phpC)
                rsuc_rpt.SetParameterValue("txnNumber", notxtn)
                rsuc_rpt.SetParameterValue("txnNumberusd", notxnUsd)
                rsuc_rpt.SetParameterValue("OperatorId", OpId)
                rsuc_rpt.SetParameterValue("Date", datetime1)

                FileUploadRptVwr.ReportSource = rsuc_rpt

                Dim rptDocument As New ReportDocument()
                Dim objDiskOpt As New DiskFileDestinationOptions()
                Dim objExOpt As ExportOptions = Nothing

                'Dim Fname As String = Application.StartupPath + "\reports\"
                Dim Fname As String = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\FileUpload\" + partnersname + "\"
                Dim dt As Date = Date.Now
                Dim month As String = dt.Month.ToString
                Dim day As String = dt.Day.ToString
                Dim year As String = dt.Year.ToString.Substring(2, 2)
                'Dim time As String = dt.TimeOfDay.ToString
                Dim tm As DateTime = DateTime.Now
                Dim format As String = "hh:mm:ss"
                Dim time As String = tm.ToString(format)
                makefolder()
                makeSubFolder(partnersname)
                objDiskOpt.DiskFileName = Fname + partnersname.Replace("/", "_") + "-" + month + day + year + "-" + time.Replace(":", "") + ".pdf"
                objExOpt = rsuc_rpt.ExportOptions
                objExOpt.ExportDestinationType = ExportDestinationType.DiskFile
                objExOpt.ExportFormatType = ExportFormatType.PortableDocFormat
                objExOpt.DestinationOptions = objDiskOpt
                rsuc_rpt.Export()

            Catch ex1 As Exception
                MsgBox(ex1.Message)
            End Try

        ElseIf oldAcc = "FSD7" And integrationtype = "2" Then
            Try
                'orpt = New ReportDocument
                'rptDirectory = Application.StartupPath & "\\rptFileUpload.rpt"
                'rptDirectory = rptDirectory.Replace("\", "\\")
                'rptDirectory = Trim(rptDirectory)
                rsuc_rpt.Load()
                Dim row As DataRow = Nothing
                Dim ds As New DataSet
                ds.Tables.Add("uploadreport")
                With ds.Tables(0).Columns
                    .Add("itemno", Type.GetType("System.String"))
                    .Add("KPTN", Type.GetType("System.String"))
                    .Add("Refno", Type.GetType("System.String"))
                    .Add("sendersname", Type.GetType("System.String"))
                    .Add("receiversname", Type.GetType("System.String"))
                    .Add("principalamount", Type.GetType("System.Double"))
                    .Add("charges", Type.GetType("System.Double"))
                    .Add("Currency", Type.GetType("System.String"))
                    .Add("AppNum", Type.GetType("System.String"))
                End With

                For x = 0 To y.Count - 1
                    y(x).ToString.Split("|")
                    z = Split(y(x).ToString, "|")
                    row = ds.Tables(0).NewRow
                    row(1) = z(1).ToString
                    row(2) = z(2).ToString
                    'row(3) = z(10).ToString + ", " + z(11).ToString + " " + z(12).ToString
                    row(3) = z(11).ToString
                    row(4) = z(3).ToString
                    Dim pamnt As Double
                    Try
                        If CDbl(z(10).ToString) Then
                            pamnt = CDbl(z(10).ToString)
                        End If
                    Catch ex As Exception
                        'MsgBox(ex.ToString)
                        pamnt = z(24).ToString
                    End Try
                    row(5) = pamnt
                    row(6) = z(15).ToString
                    row(7) = "PHP"
                    row(8) = z(0).ToString
                    ds.Tables(0).Rows.Add(row)
                Next

                rsuc_rpt.SetDataSource(ds)

                rsuc_rpt.SetParameterValue("partnerBatchNo", partnersbtchnum)
                rsuc_rpt.SetParameterValue("MLbatchNo", mlbatchnum)
                rsuc_rpt.SetParameterValue("partnersname", partnersname)
                rsuc_rpt.SetParameterValue("Pusd", usdprin)
                rsuc_rpt.SetParameterValue("Pphp", phpprin)
                rsuc_rpt.SetParameterValue("UsdCharge", usdC)
                rsuc_rpt.SetParameterValue("PhpCharge", phpC)
                rsuc_rpt.SetParameterValue("txnNumber", notxtn)
                rsuc_rpt.SetParameterValue("txnNumberusd", notxnUsd)
                rsuc_rpt.SetParameterValue("OperatorId", OpId)
                rsuc_rpt.SetParameterValue("Date", datetime1)
                FileUploadRptVwr.ReportSource = rsuc_rpt

                Dim rptDocument As New ReportDocument()
                Dim objDiskOpt As New DiskFileDestinationOptions()
                Dim objExOpt As ExportOptions = Nothing

                'Dim Fname As String = Application.StartupPath + "\reports\"
                Dim Fname As String = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\FileUpload\" + partnersname + "\"
                Dim dt As DateTime = DateTime.Now
                Dim month As String = dt.Month.ToString
                Dim day As String = dt.Day.ToString
                Dim year As String = dt.Year.ToString.Substring(2, 2)
                Dim time As DateTime = DateTime.Now
                Dim format As String = "hh:mm:ss"
                Dim timer As String = time.ToString(format)

                makefolder()
                makeSubFolder(partnersname)
                objDiskOpt.DiskFileName = Fname + partnersname + "-" + month + day + year + "-" + timer.Replace(":", "") + ".pdf"
                objExOpt = rsuc_rpt.ExportOptions
                objExOpt.ExportDestinationType = ExportDestinationType.DiskFile
                objExOpt.ExportFormatType = ExportFormatType.PortableDocFormat
                objExOpt.DestinationOptions = objDiskOpt
                rsuc_rpt.Export()


            Catch ex1 As Exception
                MsgBox(ex1.Message)
            End Try
        ElseIf oldAcc = "FSD35" And integrationtype = "2" Then

            Try
                'rsuc_rpt = New ReportDocument
                ' rptDirectory = Application.StartupPath & "\\rptFileUpload.rpt"
                'rptDirectory = rptDirectory.Replace("\", "\\")
                'rptDirectory = Trim(rptDirectory)
                rsuc_rpt.Load()
                Dim row As DataRow = Nothing
                Dim ds As New DataSet
                ds.Tables.Add("uploadreport")
                With ds.Tables(0).Columns
                    .Add("itemno", Type.GetType("System.String"))
                    .Add("KPTN", Type.GetType("System.String"))
                    .Add("Refno", Type.GetType("System.String"))
                    .Add("sendersname", Type.GetType("System.String"))
                    .Add("receiversname", Type.GetType("System.String"))
                    .Add("principalamount", Type.GetType("System.Double"))
                    .Add("charges", Type.GetType("System.Double"))
                    .Add("Currency", Type.GetType("System.String"))
                    .Add("AppNum", Type.GetType("System.String"))
                End With

                For x = 0 To y.Count - 1
                    y(x).ToString.Split("|")
                    z = Split(y(x).ToString, "|")
                    row = ds.Tables(0).NewRow
                    'row(1) = z(2).ToString
                    row(1) = z(1).ToString
                    row(2) = z(0).ToString
                    'row(3) = z(10).ToString + ", " + z(11).ToString + " " + z(12).ToString
                    If z(3).ToString = "" Then

                        row(3) = "-----------"

                    Else
                        row(3) = z(3).ToString + "," + z(4).ToString + " " + z(5).ToString
                    End If


                    If z(4).ToString = "" Then

                        row(4) = "-----------"

                    Else
                        row(4) = z(6).ToString
                    End If



                    row(5) = z(8).ToString 'principal amount

                    'Dim pamnt As Double
                    'Try
                    '    If CDbl(z(10).ToString) Then
                    '        pamnt = CDbl(z(10).ToString)
                    '    End If
                    'Catch ex As Exception
                    '    'MsgBox(ex.ToString)
                    '    pamnt = 0
                    'End Try
                    'row(5) = pamnt
                    row(6) = z(20).ToString
                    row(7) = z(9).ToString
                    row(8) = ""
                    ds.Tables(0).Rows.Add(row)
                Next

                rsuc_rpt.SetDataSource(ds)

                rsuc_rpt.SetParameterValue("partnerBatchNo", partnersbtchnum)
                rsuc_rpt.SetParameterValue("MLbatchNo", mlbatchnum)
                rsuc_rpt.SetParameterValue("partnersname", partnersname)
                rsuc_rpt.SetParameterValue("Pusd", usdprin)
                rsuc_rpt.SetParameterValue("Pphp", phpprin)
                rsuc_rpt.SetParameterValue("UsdCharge", usdC)
                rsuc_rpt.SetParameterValue("PhpCharge", phpC)
                rsuc_rpt.SetParameterValue("txnNumber", notxtn)
                rsuc_rpt.SetParameterValue("txnNumberusd", notxnUsd)
                rsuc_rpt.SetParameterValue("OperatorId", OpId)
                rsuc_rpt.SetParameterValue("Date", datetime1)
                FileUploadRptVwr.ReportSource = rsuc_rpt

                Dim rptDocument As New ReportDocument()
                Dim objDiskOpt As New DiskFileDestinationOptions()
                Dim objExOpt As ExportOptions = Nothing

                'Dim Fname As String = Application.StartupPath + "\reports\"
                Dim Fname As String = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\FileUpload\" + partnersname + "\"
                Dim dt As DateTime = DateTime.Now
                Dim month As String = dt.Month.ToString
                Dim day As String = dt.Day.ToString
                Dim year As String = dt.Year.ToString.Substring(2, 2)
                Dim time As DateTime = DateTime.Now
                Dim format As String = "hh:mm:ss"
                Dim timer As String = time.ToString(format)
                makefolder()
                makeSubFolder(partnersname)
                objDiskOpt.DiskFileName = Fname + partnersname + "-" + month + day + year + "-" + timer.Replace(":", "") + ".pdf"
                objExOpt = rsuc_rpt.ExportOptions
                objExOpt.ExportDestinationType = ExportDestinationType.DiskFile
                objExOpt.ExportFormatType = ExportFormatType.PortableDocFormat
                objExOpt.DestinationOptions = objDiskOpt
                rsuc_rpt.Export()


            Catch ex1 As Exception
                MsgBox(ex1.Message)
            End Try
        ElseIf oldAcc = "MOEX" Then
            Try
                'rsuc_rpt = New ReportDocument
                'rptDirectory = Application.StartupPath & "\\rptFileUpload.rpt"
                'rptDirectory = rptDirectory.Replace("\", "\\")
                'rptDirectory = Trim(rptDirectory)
                rsuc_rpt.Load()
                Dim row As DataRow = Nothing
                Dim ds As New DataSet
                ds.Tables.Add("uploadreport")
                With ds.Tables(0).Columns
                    .Add("itemno", Type.GetType("System.String"))
                    .Add("KPTN", Type.GetType("System.String"))
                    .Add("Refno", Type.GetType("System.String"))
                    .Add("sendersname", Type.GetType("System.String"))
                    .Add("receiversname", Type.GetType("System.String"))
                    .Add("principalamount", Type.GetType("System.Double"))
                    .Add("charges", Type.GetType("System.Double"))
                    .Add("Currency", Type.GetType("System.String"))
                    .Add("AppNum", Type.GetType("System.String"))

                End With

                For x = 0 To y.Count - 1
                    y(x).ToString.Split("|")
                    z = Split(y(x).ToString, "|")
                    row = ds.Tables(0).NewRow
                    row(1) = z(31).ToString() 'z(2).ToString
                    row(2) = z(0).ToString()
                    row(3) = Trim(z(4).ToString()) + "," + Trim(z(3).ToString()) + " " + Trim(z(5).ToString())
                    row(4) = Trim(z(7).ToString()) + "," + Trim(z(6).ToString()) + " " + Trim(z(8).ToString())
                    row(5) = Convert.ToDouble(z(20).ToString()) '
                    row(6) = z(32).ToString 'z(20).ToString
                    row(7) = z(21).ToString
                    row(8) = ""
                    ds.Tables(0).Rows.Add(row)
                Next

                rsuc_rpt.SetDataSource(ds)

                rsuc_rpt.SetParameterValue("partnerBatchNo", partnersbtchnum)
                rsuc_rpt.SetParameterValue("MLbatchNo", mlbatchnum)
                rsuc_rpt.SetParameterValue("partnersname", partnersname)
                rsuc_rpt.SetParameterValue("Pusd", usdprin)
                rsuc_rpt.SetParameterValue("Pphp", phpprin)
                rsuc_rpt.SetParameterValue("UsdCharge", usdC)
                rsuc_rpt.SetParameterValue("PhpCharge", phpC)
                rsuc_rpt.SetParameterValue("txnNumber", notxtn)
                rsuc_rpt.SetParameterValue("txnNumberusd", notxnUsd)
                rsuc_rpt.SetParameterValue("OperatorId", OpId)
                rsuc_rpt.SetParameterValue("Date", datetime1)
                FileUploadRptVwr.ReportSource = rsuc_rpt

                Dim rptDocument As New ReportDocument()
                Dim objDiskOpt As New DiskFileDestinationOptions()
                Dim objExOpt As ExportOptions = Nothing

                'Dim Fname As String = Application.StartupPath + "\reports\"
                Dim Fname As String = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\FileUpload\" + partnersname + "\"
                Dim dt As DateTime = DateTime.Now
                Dim month As String = dt.Month.ToString
                Dim day As String = dt.Day.ToString
                Dim year As String = dt.Year.ToString.Substring(2, 2)
                Dim time As DateTime = DateTime.Now
                Dim format As String = "hh:mm:ss"
                Dim timer As String = time.ToString(format)

                makefolder()
                makeSubFolder(partnersname)
                objDiskOpt.DiskFileName = Fname + partnersname + "-" + month + day + year + "-" + timer.Replace(":", "") + ".pdf"
                objExOpt = rsuc_rpt.ExportOptions
                objExOpt.ExportDestinationType = ExportDestinationType.DiskFile
                objExOpt.ExportFormatType = ExportFormatType.PortableDocFormat
                objExOpt.DestinationOptions = objDiskOpt
                rsuc_rpt.Export()

            Catch ex1 As Exception
                MsgBox(ex1.Message)
            End Try

        ElseIf integrationtype = "4" Then

            Try
                'rsuc_rpt = New ReportDocument
                'rptDirectory = Application.StartupPath & "\\rptFileUpload.rpt"
                'rptDirectory = rptDirectory.Replace("\", "\\")
                'rptDirectory = Trim(rptDirectory)
                rsuc_rpt.Load()
                Dim row As DataRow = Nothing
                Dim ds As New DataSet
                ds.Tables.Add("uploadreport")
                With ds.Tables(0).Columns
                    .Add("itemno", Type.GetType("System.String"))
                    .Add("KPTN", Type.GetType("System.String"))
                    .Add("Refno", Type.GetType("System.String"))
                    .Add("sendersname", Type.GetType("System.String"))
                    .Add("receiversname", Type.GetType("System.String"))
                    .Add("principalamount", Type.GetType("System.Double"))
                    .Add("charges", Type.GetType("System.Double"))
                    .Add("Currency", Type.GetType("System.String"))
                    .Add("AppNum", Type.GetType("System.String"))
                    rsuc_rpt.SetParameterValue("OperatorId", OpId)
                End With

                For x = 0 To y.Count - 1
                    y(x).ToString.Split("|")
                    z = Split(y(x).ToString, "|")
                    row = ds.Tables(0).NewRow
                    row(0) = "" 'z(1).ToString 'itemno
                    row(1) = z(13).ToString 'kptno
                    row(2) = z(1).ToString 'refno
                    row(3) = z(2).ToString + ", " + z(3).ToString + " " + z(4).ToString 'sendersname
                    row(4) = z(5).ToString + ", " + z(6).ToString + " " + z(7).ToString 'receiversname
                    row(5) = CDbl(z(10)) 'principalamount
                    row(6) = CDbl(z(11)) 'charges
                    ds.Tables(0).Rows.Add(row)
                Next

                rsuc_rpt.SetDataSource(ds)
                'rsuc_rpt.SetParameterValue("currency", curr)
                rsuc_rpt.SetParameterValue("partnerBatchNo", partnersbtchnum)
                rsuc_rpt.SetParameterValue("MLbatchNo", mlbatchnum)
                rsuc_rpt.SetParameterValue("partnersname", partnersname)
                rsuc_rpt.SetParameterValue("Pusd", usdprin)
                rsuc_rpt.SetParameterValue("Pphp", phpprin)
                rsuc_rpt.SetParameterValue("UsdCharge", usdC)
                rsuc_rpt.SetParameterValue("PhpCharge", phpC)
                rsuc_rpt.SetParameterValue("txnNumber", notxtn)
                rsuc_rpt.SetParameterValue("txnNumberusd", notxnUsd)
                rsuc_rpt.SetParameterValue("Date", datetime1)
                'FileUploadRptVwr.ReportSource = rsuc_rpt

                Dim rptDocument As New ReportDocument()
                Dim objDiskOpt As New DiskFileDestinationOptions()
                Dim objExOpt As ExportOptions = Nothing

                'Dim Fname As String = Application.StartupPath + "\reports\"

                Dim Fname As String = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\FileUpload\" + partnersname + "\"
                Dim dt As DateTime = DateTime.Now
                Dim month As String = dt.Month.ToString
                Dim day As String = dt.Day.ToString
                Dim year As String = dt.Year.ToString.Substring(2, 2)
                Dim time As DateTime = DateTime.Now
                Dim format As String = "hh:mm:ss"
                Dim timer As String = time.ToString(format)

                makefolder()
                makeSubFolder(partnersname)
                objDiskOpt.DiskFileName = Fname + partnersname + "-" + month + day + year + "-" + timer.Replace(":", "") + ".pdf"
                objExOpt = rsuc_rpt.ExportOptions
                objExOpt.ExportDestinationType = ExportDestinationType.DiskFile
                objExOpt.ExportFormatType = ExportFormatType.PortableDocFormat
                objExOpt.DestinationOptions = objDiskOpt
                rsuc_rpt.Export()
                FileUploadRptVwr.ReportSource = rsuc_rpt
            Catch ex As Exception
                MsgBox(ex.ToString)
            End Try

        Else

            Try
                ' rsuc_rpt = New ReportDocument
                'rptDirectory = Application.StartupPath & "\\rptFileUpload.rpt"
                'rptDirectory = rptDirectory.Replace("\", "\\")
                'rptDirectory = Trim(rptDirectory)
                rsuc_rpt.Load()
                Dim row As DataRow = Nothing
                Dim ds As New DataSet
                ds.Tables.Add("uploadreport")
                With ds.Tables(0).Columns
                    .Add("itemno", Type.GetType("System.String"))
                    .Add("KPTN", Type.GetType("System.String"))
                    .Add("Refno", Type.GetType("System.String"))
                    .Add("sendersname", Type.GetType("System.String"))
                    .Add("receiversname", Type.GetType("System.String"))
                    .Add("principalamount", Type.GetType("System.Double"))
                    .Add("charges", Type.GetType("System.Double"))
                    .Add("Currency", Type.GetType("System.String"))
                    .Add("AppNum", Type.GetType("System.String"))
                End With

                For x = 0 To y.Count - 1
                    y(x).ToString.Split("|")
                    z = Split(y(x).ToString, "|")
                    row = ds.Tables(0).NewRow
                    'row(0) = z(0).ToString 'itemno
                    row(1) = z(1).ToString 'kptno
                    row(2) = z(3).ToString 'refno
                    row(3) = z(10).ToString + ", " + z(11).ToString + " " + z(12).ToString 'sendersname
                    row(4) = z(22).ToString + ", " + z(23).ToString + " " + z(24).ToString 'receiversname
                    row(5) = CDbl(z(4).ToString) 'principalamount
                    row(6) = CDbl(z(6).ToString) 'charges
                    row(7) = z(5).ToString
                    row(8) = z(0).ToString
                    ds.Tables(0).Rows.Add(row)
                Next

                rsuc_rpt.SetDataSource(ds)
                'rsuc_rpt.SetParameterValue("currency", curr)
                rsuc_rpt.SetParameterValue("partnerBatchNo", partnersbtchnum)
                rsuc_rpt.SetParameterValue("MLbatchNo", mlbatchnum)
                rsuc_rpt.SetParameterValue("partnersname", partnersname)
                rsuc_rpt.SetParameterValue("Pusd", usdprin)
                rsuc_rpt.SetParameterValue("Pphp", phpprin)
                rsuc_rpt.SetParameterValue("UsdCharge", usdC)
                rsuc_rpt.SetParameterValue("PhpCharge", phpC)
                rsuc_rpt.SetParameterValue("txnNumber", notxtn)
                rsuc_rpt.SetParameterValue("txnNumberusd", notxnUsd)
                rsuc_rpt.SetParameterValue("OperatorId", OpId)
                rsuc_rpt.SetParameterValue("Date", datetime1)
                FileUploadRptVwr.ReportSource = rsuc_rpt

                Dim rptDocument As New ReportDocument()
                Dim objDiskOpt As New DiskFileDestinationOptions()
                Dim objExOpt As ExportOptions = Nothing

                'Dim Fname As String = Application.StartupPath + "\reports\"
                Dim Fname As String = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\FileUpload\" + partnersname + "\"
                Dim dt As DateTime = DateTime.Now
                Dim month As String = dt.Month.ToString
                Dim day As String = dt.Day.ToString
                Dim year As String = dt.Year.ToString.Substring(2, 2)
                Dim time As DateTime = DateTime.Now
                Dim format As String = "hh:mm:ss"
                Dim timer As String = time.ToString(format)

                makefolder()
                makeSubFolder(partnersname)
                objDiskOpt.DiskFileName = Fname + partnersname.Replace("/", "_") + "-" + month + day + year + "-" + timer.Replace(":", "") + ".pdf"
                objExOpt = rsuc_rpt.ExportOptions
                objExOpt.ExportDestinationType = ExportDestinationType.DiskFile
                objExOpt.ExportFormatType = ExportFormatType.PortableDocFormat
                objExOpt.DestinationOptions = objDiskOpt
                rsuc_rpt.Export()


            Catch ex As Exception
                MsgBox(ex.ToString)
            End Try

        End If



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


    Private Sub FileUploadRptVwr_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FileUploadRptVwr.Load

    End Sub

    Private Sub frmFileUploadPrint_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    End Sub
End Class