Imports System.Text
Imports System.Text.RegularExpressions
Imports System.IO
Imports System.Net
Imports System.Windows.Forms
Imports ML.Plugin.Base
Imports ML.Plugin.Business
Imports ML.Plugin.Authentication
Imports Microsoft.Office.Interop
Imports System.Data.OleDb

Public Class frmUpload
    Public oldAcc As String
    Dim accId As String
    Dim flag As Integer = 0
    'Dim curr As String
    Dim getresp() As String
    Dim dataitem As String
    Dim dataitems As String
    Dim bpifile As String = ""
    Dim plg As New PluginLogic()
    Dim myconfile As OleDbConnection
    Dim myconfilecmd As OleDbCommand
    Dim accountidInt As String = Nothing
    Dim dbp_curr As String
    Dim bdo_curr As String
    Dim bpi_curr As String
    Dim p2p_curr As String
    Dim moex_curr As String
    Dim view As New FrmReportError
    Private cbo_curr As String = Nothing
    Public Shared datalist As New ArrayList
    Public Shared wxl As Microsoft.Office.Interop.Excel.Application
    'Dim rep As New KPServices.MLhuillierSoapClient("MLhuillierSoap2", serv5.getkpservicesendpoint)
    Private Sub btnBrowse_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBrowse.Click
        'closeApplication()


        If cboPartners.Text = "" Then
            MsgBox("Select Partner.", MsgBoxStyle.OkOnly, "error")
            'MessageBox.Show("Select Partner.", "error", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else

            OpenFileDialog1.Filter = "Text File (*.txt)|*.txt|CSV File (*.csv)|*.csv|Excel File (*.xls)|*.xls"
            OpenFileDialog1.ShowDialog()
            validateFileExtension()
            bntUpload.Visible = True
            submitBTN.Visible = False
        End If

    End Sub

    Private Sub OpenFileDialog1_FileOk(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles OpenFileDialog1.FileOk
        txtFileName.Text = OpenFileDialog1.FileName
    End Sub

    Private Sub validateFileExtension()
        DataGridView1.Rows.Clear()
        DataGridView2.Rows.Clear()
        DataGridView4.Rows.Clear()
        DataGridView3.Rows.Clear()
        DataGrid.Rows.Clear()
        Dim serv1 As New PluginLogic()
        Dim resp As New FileUploadService.MLhuillierSoapClient("MLhuillierSoap8", serv1.getAdminPartnersFileUploadServiceendpoint())
        Dim rest As New FileUploadService.getIntegrationType()
        Dim getresp As String = Nothing

        rest = resp.IntegrationType(accountidInt)
        getresp = rest.IntegrationType

        If OpenFileDialog1.FilterIndex = 1 Then
            If getresp = "2" Then
                Intresp = getresp
                If oldAcc = "BPINOY" Then
                    parseDataBPI()
                    DataGridView2.Visible = True
                    If Me.DataGridView2.Rows.Count <= 0 Then
                        Me.DataGridView2.AllowUserToAddRows = False
                        Me.clearGrid()
                    End If
                    DataGridView1.Visible = False
                    DataGridView3.Visible = False
                    DataGridView4.Visible = False
                    DataGridView5.Visible = False
                    DataGrid.Visible = False

                    If Me.DataGridView2.Rows.Count > 0 Then
                        Dim decTAmountPHP As Decimal = System.Convert.ToDecimal(Me.txtTotalPrincipal.Text)
                        Dim decTAmountUSD As Decimal = System.Convert.ToDecimal(Me.txtPrincipalUSD.Text)
                        If decTAmountPHP <> 0.0 And decTAmountUSD = 0.0 Then
                            If checkingTotalAmount(accountidInt, decTAmountPHP, "PHP").Equals(True) Then
                                flag = 1
                                cboPartners.Enabled = False
                                btnBrowse.Enabled = False
                                bntUpload.Enabled = True
                            Else
                                MsgBox("Total Amount is greater than the Running Balance or Credit Limit status is inactive ")
                                bntUpload.Enabled = False
                            End If
                        ElseIf decTAmountUSD <> 0.0 And decTAmountPHP = 0.0 Then
                            If checkingTotalAmount(accountidInt, decTAmountUSD, "USD").Equals(True) Then
                                flag = 2
                                cboPartners.Enabled = False
                                btnBrowse.Enabled = False
                                bntUpload.Enabled = True
                            Else
                                MsgBox("Total Amount is greater than the Running Balance or Credit Limit status is inactive ")
                                bntUpload.Enabled = False
                            End If
                        ElseIf decTAmountUSD <> 0.0 And decTAmountPHP <> 0.0 Then
                            If checkingTotalAmount(accountidInt, decTAmountUSD, "USD").Equals(True) And checkingTotalAmount(accountidInt, decTAmountPHP, "PHP").Equals(True) Then
                                flag = 3
                                cboPartners.Enabled = False
                                btnBrowse.Enabled = False
                                bntUpload.Enabled = True
                            Else
                                MsgBox("Total Amount is greater than the Running Balance or Credit Limit status is inactive ")
                                bntUpload.Enabled = False
                            End If
                        End If
                    Else
                        bntUpload.Enabled = False
                    End If

                ElseIf oldAcc = "FSD7" Then
                    parseDataDBP()

                    Me.DataGridView3.AllowUserToAddRows = False
                    If Me.DataGridView3.Rows.Count <= 0 Then
                        Me.DataGridView3.AllowUserToAddRows = False
                        Me.clearGrid()
                    End If
                    DataGridView4.Visible = False
                    DataGridView3.Visible = True
                    DataGridView2.Visible = False
                    DataGridView1.Visible = False
                    DataGrid.Visible = False
                    Me.DataGridView3.BringToFront()
                    If Me.DataGridView3.Rows.Count > 0 Then
                        Dim decTAmountPHP As Decimal = System.Convert.ToDecimal(Me.txtTotalPrincipal.Text)
                        Dim decTAmountUSD As Decimal = System.Convert.ToDecimal(Me.txtPrincipalUSD.Text)
                        If decTAmountPHP <> 0.0 And decTAmountUSD = 0.0 Then
                            If checkingTotalAmount(accountidInt, decTAmountPHP, "PHP").Equals(True) Then
                                flag = 1
                                cboPartners.Enabled = False
                                btnBrowse.Enabled = False
                                bntUpload.Enabled = True
                            Else
                                MsgBox("Total Amount is greater than the Running Balance or Credit Limit status is inactive ")
                                bntUpload.Enabled = False
                            End If
                        ElseIf decTAmountUSD <> 0.0 And decTAmountPHP = 0.0 Then
                            If checkingTotalAmount(accountidInt, decTAmountUSD, "USD").Equals(True) Then
                                flag = 2
                                cboPartners.Enabled = False
                                btnBrowse.Enabled = False
                                bntUpload.Enabled = True
                            Else
                                MsgBox("Total Amount is greater than the Running Balance or Credit Limit status is inactive ")
                                bntUpload.Enabled = False
                            End If
                        ElseIf decTAmountUSD <> 0.0 And decTAmountPHP <> 0.0 Then
                            If checkingTotalAmount(accountidInt, decTAmountUSD, "USD").Equals(True) And checkingTotalAmount(accountidInt, decTAmountPHP, "PHP").Equals(True) Then
                                flag = 3
                                cboPartners.Enabled = False
                                btnBrowse.Enabled = False
                                bntUpload.Enabled = True
                            Else
                                MsgBox("Total Amount is greater than the Running Balance or Credit Limit status is inactive ")
                                bntUpload.Enabled = False
                            End If
                        End If

                    Else
                        bntUpload.Enabled = False
                    End If
                ElseIf oldAcc = "FSD35" Then
                    parseDataBDO()
                    DataGridView4.Visible = True
                    If Me.DataGridView4.Rows.Count <= 0 Then
                        Me.DataGridView4.AllowUserToAddRows = False
                        Me.clearGrid()
                    End If
                    DataGridView3.Visible = False
                    DataGridView2.Visible = False
                    DataGridView1.Visible = False
                    DataGridView5.Visible = False
                    DataGrid.Visible = False
                    If Me.DataGridView4.Rows.Count > 0 Then
                        Dim decTAmountPHP As Decimal = System.Convert.ToDecimal(Me.txtTotalPrincipal.Text)
                        Dim decTAmountUSD As Decimal = System.Convert.ToDecimal(Me.txtPrincipalUSD.Text)
                        If decTAmountPHP <> 0.0 And decTAmountUSD = 0.0 Then
                            If checkingTotalAmount(accountidInt, decTAmountPHP, "PHP").Equals(True) Then
                                flag = 1
                                cboPartners.Enabled = False
                                btnBrowse.Enabled = False
                                bntUpload.Enabled = True
                            Else
                                MsgBox("Total Amount is greater than the Running Balance or Credit Limit status is inactive ")
                                bntUpload.Enabled = False
                            End If
                        ElseIf decTAmountUSD <> 0.0 And decTAmountPHP = 0.0 Then
                            If checkingTotalAmount(accountidInt, decTAmountUSD, "USD").Equals(True) Then
                                flag = 2
                                cboPartners.Enabled = False
                                btnBrowse.Enabled = False
                                bntUpload.Enabled = True
                            Else
                                MsgBox("Total Amount is greater than the Running Balance or Credit Limit status is inactive ")
                                bntUpload.Enabled = False
                            End If
                        ElseIf decTAmountUSD <> 0.0 And decTAmountPHP <> 0.0 Then
                            If checkingTotalAmount(accountidInt, decTAmountUSD, "USD").Equals(True) And checkingTotalAmount(accountidInt, decTAmountPHP, "PHP").Equals(True) Then
                                flag = 3
                                cboPartners.Enabled = False
                                btnBrowse.Enabled = False
                                bntUpload.Enabled = True
                            Else
                                MsgBox("Total Amount is greater than the Running Balance or Credit Limit status is inactive ")
                                bntUpload.Enabled = False
                            End If
                        End If
                    End If
                ElseIf oldAcc = "MOEX" Then 'for money changer

                    ParseMoneyChanger()

                    DataGridView5.Visible = True
                    If Me.DataGridView5.Rows.Count <= 0 Then
                        Me.DataGridView5.AllowUserToAddRows = False
                        Me.clearGrid()
                    End If
                    DataGridView4.Visible = False
                    DataGridView3.Visible = False
                    DataGridView2.Visible = False
                    DataGridView1.Visible = False
                    DataGrid.Visible = False

                    If Me.DataGridView5.Rows.Count > 0 Then
                        'Me.DataGridView5.AllowUserToAddRows = False
                        Dim decTAmountPHP As Decimal = System.Convert.ToDecimal(Me.txtTotalPrincipal.Text)
                        Dim decTAmountUSD As Decimal = System.Convert.ToDecimal(Me.txtPrincipalUSD.Text)
                        If decTAmountPHP <> 0.0 And decTAmountUSD = 0.0 Then
                            If checkingTotalAmount(accountidInt, decTAmountPHP, "PHP").Equals(True) Then
                                flag = 1
                                cboPartners.Enabled = False
                                btnBrowse.Enabled = False
                                bntUpload.Enabled = True
                            Else
                                MsgBox("Total Amount is greater than the Running Balance or Credit Limit status is inactive ")
                                bntUpload.Enabled = False
                            End If
                        ElseIf decTAmountUSD <> 0.0 And decTAmountPHP = 0.0 Then
                            If checkingTotalAmount(accountidInt, decTAmountUSD, "USD").Equals(True) Then
                                flag = 2
                                cboPartners.Enabled = False
                                btnBrowse.Enabled = False
                                bntUpload.Enabled = True
                            Else
                                MsgBox("Total Amount is greater than the Running Balance or Credit Limit status is inactive ")
                                bntUpload.Enabled = False
                            End If
                        ElseIf decTAmountUSD <> 0.0 And decTAmountPHP <> 0.0 Then
                            If checkingTotalAmount(accountidInt, decTAmountUSD, "USD").Equals(True) And checkingTotalAmount(accountidInt, decTAmountPHP, "PHP").Equals(True) Then
                                flag = 3
                                cboPartners.Enabled = False
                                btnBrowse.Enabled = False
                                bntUpload.Enabled = True
                            Else
                                MsgBox("Total Amount is greater than the Running Balance or Credit Limit status is inactive ")
                                bntUpload.Enabled = False
                            End If
                        End If

                    Else
                        bntUpload.Enabled = False
                    End If


                Else
                    parseData()
                    DataGrid.Visible = True
                    If Me.DataGrid.Rows.Count <= 0 Then
                        Me.DataGrid.AllowUserToAddRows = False
                        Me.clearGrid()
                    End If
                    DataGridView2.Visible = False
                    DataGridView1.Visible = False
                    DataGridView5.Visible = False
                    If Me.DataGrid.Rows.Count > 0 Then
                        Dim decTAmountPHP As Decimal = System.Convert.ToDecimal(Me.txtTotalPrincipal.Text)
                        Dim decTAmountUSD As Decimal = System.Convert.ToDecimal(Me.txtPrincipalUSD.Text)
                        If decTAmountPHP <> 0.0 And decTAmountUSD = 0.0 Then
                            If checkingTotalAmount(accountidInt, decTAmountPHP, "PHP").Equals(True) Then
                                flag = 1
                                cboPartners.Enabled = False
                                btnBrowse.Enabled = False
                                bntUpload.Enabled = True
                            Else
                                MsgBox("Total Amount is greater than the Running Balance or Credit Limit status is inactive ")
                                bntUpload.Enabled = False
                            End If
                        ElseIf decTAmountUSD <> 0.0 And decTAmountPHP = 0.0 Then
                            If checkingTotalAmount(accountidInt, decTAmountUSD, "USD").Equals(True) Then
                                flag = 2
                                cboPartners.Enabled = False
                                btnBrowse.Enabled = False
                                bntUpload.Enabled = True
                            Else
                                MsgBox("Total Amount is greater than the Running Balance or Credit Limit status is inactive ")
                                bntUpload.Enabled = False
                            End If
                        ElseIf decTAmountUSD <> 0.0 And decTAmountPHP <> 0.0 Then
                            If checkingTotalAmount(accountidInt, decTAmountUSD, "USD").Equals(True) And checkingTotalAmount(accountidInt, decTAmountPHP, "PHP").Equals(True) Then
                                flag = 3
                                cboPartners.Enabled = False
                                btnBrowse.Enabled = False
                                bntUpload.Enabled = True
                            Else
                                MsgBox("Total Amount is greater than the Running Balance or Credit Limit status is inactive ")
                                bntUpload.Enabled = False
                            End If
                        End If
                    Else
                        bntUpload.Enabled = False
                    End If
                End If
            Else
                MsgBox("File is a Manual Entry")
                Me.DataGridView1.Rows.Clear()
                Me.cboPartners.Enabled = True
                Me.btnBrowse.Enabled = True
                Me.cboPartners.Text = ""
                Me.txtFileName.Text = ""
                Me.cboPartners.Text = Nothing
                Me.bntUpload.Enabled = False
                Me.txtFileName.Text = ""
                Me.txttotalcount.Text = ""
                Me.lblDatetime.Text = ""
                Me.txtTotalPrincipal.Text = ""
                Me.txtBatchNo.Text = ""
                Me.txtAmountDue.Text = ""
                Me.txtTotalCharges.Text = ""
                Me.txtConctactNum.Text = ""
                Me.txtAddress.Text = ""

                clearGrid()
            End If
        ElseIf OpenFileDialog1.FilterIndex = 2 Then
            'for csv file
        ElseIf OpenFileDialog1.FilterIndex = 3 Then
            If getresp = "4" Then
                'parseDataxls()
                DataGridView1.Visible = True
                DataGrid.Visible = False
                DataGridView2.Visible = False
                If Me.DataGridView1.Rows.Count > 0 Then
                    'Dim decTAmount As Decimal = System.Convert.ToDecimal(Me.txtTotalPrincipal.Text)
                    Dim decTAmount As Double = Me.txtTotalPrincipal.Text
                    If checkingTotalAmount(accountidInt, decTAmount, cbo_curr).Equals(True) Then
                        cboPartners.Enabled = False
                        btnBrowse.Enabled = False
                        bntUpload.Enabled = True
                    Else
                        MsgBox("Total Amount is greater than the Running Balance or Credit Limit status is inactive ")
                        bntUpload.Enabled = False
                    End If
                Else
                    bntUpload.Enabled = False
                End If
            Else
                MsgBox("File is an Upload File")
                Me.DataGridView1.Rows.Clear()
                Me.cboPartners.Enabled = True
                Me.btnBrowse.Enabled = True
                Me.cboPartners.Text = ""
                Me.txtFileName.Text = ""
                Me.cboPartners.Text = Nothing
                Me.bntUpload.Enabled = False
                Me.txtFileName.Text = ""
                Me.txttotalcount.Text = ""
                Me.lblDatetime.Text = ""
                Me.txtTotalPrincipal.Text = ""
                Me.txtBatchNo.Text = ""
                Me.txtAmountDue.Text = ""
                Me.txtTotalCharges.Text = ""
                Me.txtConctactNum.Text = ""
                Me.txtAddress.Text = ""

                clearGrid()
            End If
        ElseIf OpenFileDialog1.FilterIndex = 0 Then

        Else
            MsgBox("File Not Supported.", MsgBoxStyle.OkOnly, "File Upload Error.")
            txtFileName.Text = ""
        End If
    End Sub
    Private Sub Version()
        Dim vread As StreamReader = New StreamReader(Application.StartupPath & "/version.txt")
        vfile = Convert.ToDouble(vread.ReadLine)
    End Sub
    Private Sub releaseObject(ByVal obj As Object)
        Try
            System.Runtime.InteropServices.Marshal.ReleaseComObject(obj)
            obj = Nothing
        Catch ex As Exception
            obj = Nothing
        Finally
            GC.Collect()
        End Try
    End Sub
    'Public Sub parseDataxls()
    '    Dim row As Integer = 0
    '    Dim appPath As String = OpenFileDialog1.FileName
    '    Dim fileEntries As New List(Of String)
    '    Dim dtExcel As New DataTable
    '    Dim principal As String = Nothing
    '    Dim totalPrincipal As Double = 0
    '    Me.DataGridView1.BringToFront()

    '    If Not File.Exists(appPath) Then
    '        Exit Sub
    '    Else

    '        myconfile = New OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & appPath & ";" & "Extended Properties=""Excel 8.0;HDR=NO;IMEX=1""")
    '        Dim wbook As Microsoft.Office.Interop.Excel.Workbook
    '        Dim wsheet As Microsoft.Office.Interop.Excel.Worksheet
    '        Dim shname As String = ""
    '        wxl = CreateObject("Excel.Application")

    '        Dim oXL As Excel.Application


    '        Try
    '            wbook = wxl.Workbooks.Open(appPath)
    '            If myconfile.State = ConnectionState.Open Then ' if closed
    '                myconfile.Close()
    '            End If


    '            For Each wsheet In wbook.Worksheets
    '                shname = wsheet.Name
    '                datalist.Add(shname)
    '            Next
    '            myconfile.Open()
    '            Dim i As Integer = 0
    '            Dim counter As Integer = 1
    '            Dim sheetlist As Integer
    '            Dim dbcommand As OleDbCommand
    '            'Dim mydata As New OleDbDataAdapter("Select * from [SENDOUT$]", myconfile)
    '            For sheetlist = 0 To datalist.Count - 1
    '                dbcommand = New OleDbCommand("Select * from [" & datalist.Item(sheetlist) & "$]", myconfile)
    '                Dim mydata As OleDbDataReader = dbcommand.ExecuteReader()
    '                'If myconfile.State = ConnectionState.Closed Then ' if closed
    '                '    myconfile.Open()
    '                'End If
    '                ';myconfile.Open()
    '                If mydata.HasRows Then
    '                    While mydata.Read()


    '                        If counter >= 9 Then
    '                            Dim data As String = mydata(1).ToString()
    '                            If data = "" Then
    '                                Me.DataGridView1.AllowUserToAddRows = False
    '                                'Me.txtTotalPrincipal.Text = totalPrincipal
    '                                Me.txttotalcount.Text = i
    '                                If totalPrincipal.ToString.Length > 3 Then
    '                                    Me.txtAmountDue.Text = Format(totalPrincipal, "#,###,##0.00")
    '                                    Me.txtTotalPrincipal.Text = Format(totalPrincipal, "#,###,##0.00")
    '                                ElseIf totalPrincipal.ToString.Length > 6 Then
    '                                    Me.txtAmountDue.Text = Format(totalPrincipal, "#,###,##0.00")
    '                                    Me.txtTotalPrincipal.Text = Format(totalPrincipal, "#,###,##0.00")
    '                                Else
    '                                    Me.txtAmountDue.Text = Format(totalPrincipal, "#,###,##0.00")
    '                                    Me.txtTotalPrincipal.Text = Format(totalPrincipal, "#,###,##0.00")
    '                                End If
    '                                Me.btnBrowse.Enabled = False
    '                                Me.Enabled = True
    '                                cboPartners.Enabled = False
    '                                btnBrowse.Enabled = False
    '                                mydata.Close()
    '                                mydata.Dispose()
    '                                closeApplication()
    '                                myconfile.Close()
    '                                Exit Sub
    '                            Else
    '                                Me.DataGridView1.Rows.Add()
    '                                Me.DataGridView1.Rows(i).Cells(0).Value = mydata(0)
    '                                Me.DataGridView1.Rows(i).Cells(1).Value = mydata(1)
    '                                Me.DataGridView1.Rows(i).Cells(2).Value = mydata(2)
    '                                Me.DataGridView1.Rows(i).Cells(3).Value = mydata(3)
    '                                Me.DataGridView1.Rows(i).Cells(4).Value = mydata(4)
    '                                Me.DataGridView1.Rows(i).Cells(5).Value = mydata(5)
    '                                Me.DataGridView1.Rows(i).Cells(6).Value = mydata(6)
    '                                Me.DataGridView1.Rows(i).Cells(7).Value = mydata(7)
    '                                Me.DataGridView1.Rows(i).Cells(8).Value = mydata(8)
    '                                Me.DataGridView1.Rows(i).Cells(9).Value = mydata(9)
    '                                Me.DataGridView1.Rows(i).Cells(10).Value = mydata(10)
    '                                Me.DataGridView1.Rows(i).Cells(11).Value = mydata(11)
    '                                Me.DataGridView1.Rows(i).Cells(12).Value = mydata(12)

    '                                principal = mydata(10)
    '                                totalPrincipal = totalPrincipal + System.Convert.ToDouble(principal)
    '                            End If
    '                            i = i + 1
    '                        End If
    '                        counter = counter + 1
    '                    End While

    '                End If



    '            Next

    '        Catch ex As Exception
    '            'MsgBox("Format file loaded is invalid", MsgBoxStyle.Information, Title:="Information")
    '            myconfile.Close()
    '            MsgBox(ex.ToString)
    '            clearGrid()
    '        End Try






    '    End If

    'End Sub
    Private Function checkingTotalAmount(ByVal AccountId As String, ByVal totalAmount As Decimal, ByVal curr1 As String) As Boolean
        Dim serv1 As New PluginLogic()
        Dim resp As New FileUploadService.MLhuillierSoapClient("MLhuillierSoap8", serv1.getAdminPartnersFileUploadServiceendpoint())
        Dim rest As New FileUploadService.CheckTotalAmountInFile
        Dim ret As Boolean

        rest = resp.CheckTheTotalAmount(AccountId, totalAmount, curr1)
        ret = rest.identifier
        Return ret
    End Function

    Private Sub closeApplication()
        Dim prs() As System.Diagnostics.Process
        Dim process As System.Diagnostics.Process
        prs = System.Diagnostics.Process.GetProcesses()

        For Each process In prs
            If process.ProcessName.ToUpper() = "EXCEL" Then
                process.Refresh()
                If Not process.HasExited = True Then
                    If process.MainWindowHandle = IntPtr.Zero And process.MainWindowTitle.Trim() = "" Then
                        process.Kill()
                        myconfile.Close()
                    End If
                End If
            End If
        Next
    End Sub

    Public Sub clear()

        Me.bntUpload.Enabled = False
        Me.cboPartners.Text = Nothing
        Me.DataGrid.Rows.Clear()
        Me.cboPartners.Enabled = True
        Me.btnBrowse.Enabled = True
        Me.cboPartners.Text = ""
        Me.txtFileName.Text = ""
        Me.txtFileName.Text = ""
        Me.txtBatchNo.Text = ""
        Me.DataGridView1.Rows.Clear()
        Me.txttotalcount.Text = ""
        Me.lblDatetime.Text = ""
        Me.txtTotalPrincipal.Text = ""
        Me.txtAmountDue.Text = ""
        Me.txtTotalCharges.Text = ""
        Me.txtConctactNum.Text = ""
        Me.txtAddress.Text = ""
        Me.OpenFileDialog1.FilterIndex = 0
        Me.DataGridView2.Rows.Clear()
        closeApplication()
        Me.DataGridView3.Rows.Clear()
        Me.DataGridView4.Rows.Clear()
        Me.DataGridView5.Rows.Clear()
        Exit Sub
    End Sub
    Public Sub ParseMoneyChanger()
       
        Dim rowvalue As String
        Dim rowcount As Integer = 0

        Dim serv3 As New PluginLogic
        Dim resp2 As New FileUploadService.MLhuillierSoapClient("MLhuillierSoap8", serv3.getAdminPartnersFileUploadServiceendpoint())
        Dim rest2 As New FileUploadService.ChargeValue()

        Dim appPath As String = OpenFileDialog1.FileName
        If appPath = Nothing Or appPath = "" Then
            Me.DataGrid.BringToFront()
            Me.DataGrid.Visible = True
            'Me.DataGridView1.Rows.Clear()
            Me.DataGridView2.Rows.Clear()
            Me.DataGrid.Rows.Clear()
            Me.cboPartners.Text = Nothing
            Me.txtFileName.Text = ""
            Me.txttotalcount.Text = ""
            Me.lblDatetime.Text = ""
            Me.txtTotalPrincipal.Text = ""
            Me.txtBatchNo.Text = ""
            btnBrowse.Enabled = True
            Me.txtAmountDue.Text = ""
            Me.txtTotalCharges.Text = ""
            Me.txtConctactNum.Text = ""
            Me.txtAddress.Text = ""
            Me.totamountUSD.Text = ""
            Me.txtPrincipalUSD.Text = ""
            Me.txtChargeUSD.Text = ""
            Me.txtcountUSD.Text = ""
            cboPartners.Enabled = True
            Me.cboPartners.Select()
            Me.DataGrid.AllowUserToAddRows = False
            'cboPartners.Items.Clear()
            clearGrid()
            Exit Sub
        End If
        Dim fileEntries As New List(Of String)
        Dim totalAmount As Double = 0
        Dim Amount As Double
        Dim fNAme As String = "MOEX"

        If Not File.Exists(appPath) Then
            Exit Sub
        End If
        If fNAme = oldAcc Then
            Try
                ' Read file into the list ...
                Dim reader As StreamReader = New StreamReader(appPath)
                fileEntries.Clear()
                Dim row As Integer = 0
                Dim dgvrow As New DataGridViewRow
                Dim iden As Integer = 1
                Dim PHPcounter As Integer = 0
                Dim USDcounter As Integer = 0
                Dim totalprincipalPHP As Double = 0
                Dim totalprincipalUSD As Double = 0
                Dim totalphpcharge As Double = 0
                Dim totalusdcharge As Double = 0

                While reader.Peek <> -1
                    rowvalue = reader.ReadLine()
                    If rowvalue = "" Then
                        iden = 0
                        Continue While
                    End If
                    If iden <> 0 Then

                        Me.DataGridView5.Rows.Add()
                        Me.DataGridView5.Rows(row).Cells(0).Value = rowvalue.Substring(0, 13).ToString
                        Me.DataGridView5.Rows(row).Cells(1).Value = rowvalue.Substring(13, 8).ToString
                        Me.DataGridView5.Rows(row).Cells(2).Value = rowvalue.Substring(21, 4).ToString
                        Me.DataGridView5.Rows(row).Cells(3).Value = rowvalue.Substring(25, 25).ToString
                        Me.DataGridView5.Rows(row).Cells(4).Value = rowvalue.Substring(50, 20).ToString
                        Me.DataGridView5.Rows(row).Cells(5).Value = rowvalue.Substring(70, 20).ToString
                        Me.DataGridView5.Rows(row).Cells(6).Value = rowvalue.Substring(90, 25).ToString
                        Me.DataGridView5.Rows(row).Cells(7).Value = rowvalue.Substring(115, 20).ToString
                        Me.DataGridView5.Rows(row).Cells(8).Value = rowvalue.Substring(135, 20).ToString
                        Me.DataGridView5.Rows(row).Cells(9).Value = rowvalue.Substring(155, 80).ToString
                        Me.DataGridView5.Rows(row).Cells(10).Value = rowvalue.Substring(235, 50).ToString
                        Me.DataGridView5.Rows(row).Cells(11).Value = rowvalue.Substring(285, 3).ToString
                        Me.DataGridView5.Rows(row).Cells(12).Value = rowvalue.Substring(288, 20).ToString
                        Me.DataGridView5.Rows(row).Cells(13).Value = rowvalue.Substring(308, 20).ToString
                        Me.DataGridView5.Rows(row).Cells(14).Value = rowvalue.Substring(328, 200).ToString
                        Me.DataGridView5.Rows(row).Cells(15).Value = rowvalue.Substring(528, 9).ToString
                        Me.DataGridView5.Rows(row).Cells(16).Value = rowvalue.Substring(537, 50).ToString
                        Me.DataGridView5.Rows(row).Cells(17).Value = rowvalue.Substring(587, 4).ToString
                        Me.DataGridView5.Rows(row).Cells(18).Value = rowvalue.Substring(591, 50).ToString
                        Me.DataGridView5.Rows(row).Cells(19).Value = rowvalue.Substring(641, 3).ToString
                        'Me.DataGridView5.Rows(row).Cells(20).Value = rowvalue.Substring(644, 10).ToString

                        'If Trim(rowvalue.Substring(654, 2).ToString) = "" Then
                        '    Me.DataGridView5.Rows(row).Cells(20).Value = Trim(rowvalue.Substring(644, 10).ToString) & ".00" '' last 2 characters is decimal places
                        'Else
                        '    Me.DataGridView5.Rows(row).Cells(20).Value = Trim(rowvalue.Substring(644, 10).ToString) & "." & rowvalue.Substring(654, 2).ToString '' last 2 characters is decimal places
                        'End If
                        Dim amountlen As Integer
                        Dim finalamount As String
                        Dim newamount As String



                        amountlen = rowvalue.Substring(644, 12).ToString.Trim.Length 'getstringLENGTH
                        finalamount = rowvalue.Substring(644, 12).ToString.Trim 'getstringWithoutspaces

                        newamount = finalamount.Substring(0, (amountlen - 2)) & "." & finalamount.Substring((amountlen - 2), 2)

                        'If Trim(rowvalue.Substring(654, 2).ToString) = "" Then
                        '    Me.DataGridView5.Rows(row).Cells(20).Value = Trim(rowvalue.Substring(644, 10).ToString) & ".00" '' last 2 characters is decimal places

                        'Else
                        'Me.DataGridView5.Rows(row).Cells(20).Value = Trim(rowvalue.Substring(644, 10).ToString) & "." & rowvalue.Substring(654, 2).ToString '' last 2 characters is decimal places
                        Me.DataGridView5.Rows(row).Cells(20).Value = newamount

                        'End If


                        'MsgBox(rowvalue.Substring(644, 10).ToString & "." & rowvalue.Substring(654, 2).ToString)
                        Me.DataGridView5.Rows(row).Cells(21).Value = rowvalue.Substring(656, 3).ToString
                        Me.DataGridView5.Rows(row).Cells(22).Value = rowvalue.Substring(659, 1).ToString
                        Me.DataGridView5.Rows(row).Cells(23).Value = rowvalue.Substring(660, 30).ToString
                        Me.DataGridView5.Rows(row).Cells(24).Value = rowvalue.Substring(690, 50).ToString
                        Me.DataGridView5.Rows(row).Cells(25).Value = rowvalue.Substring(740, 40).ToString
                        Me.DataGridView5.Rows(row).Cells(26).Value = rowvalue.Substring(780, 12).ToString
                        Me.DataGridView5.Rows(row).Cells(27).Value = rowvalue.Substring(792, 25).ToString
                        Me.DataGridView5.Rows(row).Cells(28).Value = rowvalue.Substring(817, 3).ToString
                        Me.DataGridView5.Rows(row).Cells(29).Value = rowvalue.Substring(820, 20).ToString
                        Me.DataGridView5.Rows(row).Cells(30).Value = rowvalue.Substring(840, 20).ToString
                        Amount = CDbl(Me.DataGridView5.Rows(row).Cells(20).Value)
                        Dim currency As String = Me.DataGridView5.Rows(row).Cells(21).Value
                        rest2 = resp2.ChargesPerLine(accId, Amount, currency)
                        Dim charge As Decimal = rest2.Charges
                        Me.DataGridView5.Rows(row).Cells(31).Value = charge

                        'If cbo_curr <> rowvalue.Substring(656, 3).ToString Then
                        '    MsgBox("Invalid Currency.", MsgBoxStyle.OkOnly, "File Upload")
                        '    Me.DataGridView5.Rows.Clear()
                        '    Me.btnCancel.PerformClick()
                        '    clearGrid()
                        '    Exit Sub
                        'End If
                        '**********************cani new code***********************
                        If currency = "PHP" Then
                            PHPcounter = PHPcounter + 1
                            totalprincipalPHP = totalprincipalPHP + Amount
                            totalphpcharge = totalphpcharge + charge

                        ElseIf currency = "USD" Then
                            USDcounter = USDcounter + 1
                            totalusdcharge = totalusdcharge + charge
                            totalprincipalUSD = totalprincipalUSD + Amount

                        End If





                        'totalAmount = totalAmount + Amount
                        row += 1
                        Dim y As String
                        y = DataGridView5.Item(0, DataGridView5.CurrentRow.Index).Value

                    End If
                End While

                reader.Close()
                Me.DataGridView5.AllowUserToAddRows = False
                '**********************cani new code 3/13/2013********************
                Me.txttotalcount.Text = PHPcounter
                Me.txtcountUSD.Text = USDcounter

                Dim totalAmountDuePHP As Decimal = totalprincipalPHP + totalphpcharge
                Dim totalAmountDueUSD As Decimal = totalprincipalUSD + totalusdcharge

                If totalphpcharge.ToString.Length > 3 Then
                    Me.txtTotalCharges.Text = Format(totalphpcharge, "#,###,##0.00")
                ElseIf totalphpcharge.ToString.Length > 6 Then
                    Me.txtTotalCharges.Text = Format(totalphpcharge, "#,###,##0.00")
                Else
                    Me.txtTotalCharges.Text = Format(totalphpcharge, "#,###,##0.00")
                End If

                If totalusdcharge.ToString.Length > 3 Then
                    Me.txtChargeUSD.Text = Format(totalusdcharge, "#,###,##0.00")
                ElseIf totalusdcharge.ToString.Length > 6 Then
                    Me.txtChargeUSD.Text = Format(totalusdcharge, "#,###,##0.00")
                Else
                    Me.txtChargeUSD.Text = Format(totalusdcharge, "#,###,##0.00")
                End If

                If totalprincipalPHP.ToString.Length > 3 Then
                    Me.txtAmountDue.Text = Format(totalAmountDuePHP, "#,###,##0.00")
                    Me.txtTotalPrincipal.Text = Format(totalprincipalPHP, "#,###,##0.00")
                ElseIf totalprincipalPHP.ToString.Length > 6 Then
                    Me.txtAmountDue.Text = Format(totalAmountDuePHP, "#,###,##0.00")
                    Me.txtTotalPrincipal.Text = Format(totalprincipalPHP, "#,###,##0.00")
                Else
                    Me.txtAmountDue.Text = Format(totalAmountDuePHP, "#,###,##0.00")
                    Me.txtTotalPrincipal.Text = Format(totalprincipalPHP, "#,###,##0.00")
                End If


                If totalprincipalUSD.ToString.Length > 3 Then
                    Me.totamountUSD.Text = Format(totalAmountDueUSD, "#,###,##0.00")
                    Me.txtPrincipalUSD.Text = Format(totalprincipalUSD, "#,###,##0.00")
                ElseIf totalprincipalUSD.ToString.Length > 6 Then
                    Me.totamountUSD.Text = Format(totalAmountDueUSD, "#,###,##0.00")
                    Me.txtPrincipalUSD.Text = Format(totalprincipalUSD, "#,###,##0.00")
                Else
                    Me.totamountUSD.Text = Format(totalAmountDueUSD, "#,###,##0.00")
                    Me.txtPrincipalUSD.Text = Format(totalprincipalUSD, "#,###,##0.00")
                End If

                '*****************************************************************
                'If totalAmount.ToString.Length > 3 Then
                '    Me.txtTotalPrincipal.Text = Format(totalAmount, "#,###,##0.00")
                '    Me.txtAmountDue.Text = Format(totalAmount, "#,###,##0.00")
                'ElseIf totalAmount.ToString.Length > 6 Then
                '    Me.txtTotalPrincipal.Text = Format(totalAmount, "#,###,##0.00")
                '    Me.txtAmountDue.Text = Format(totalAmount, "#,###,##0.00")
                'Else
                '    Me.txtTotalPrincipal.Text = Format(totalAmount, "#,###,##0.00")
                '    Me.txtAmountDue.Text = Format(totalAmount, "#,###,##0.00")
                'End If

            Catch ex As Exception
                MsgBox("Invalid File for Money Exchage", MsgBoxStyle.Critical)
                Me.DataGridView5.BringToFront()
                Me.DataGridView5.Visible = True
                Me.DataGridView5.AllowUserToAddRows = False
                'Me.DataGridView1.Rows.Clear()
                Me.DataGridView4.Rows.Clear()
                Me.DataGridView3.Rows.Clear()
                Me.DataGridView2.Rows.Clear()
                Me.DataGridView5.Rows.Clear()
                Me.DataGrid.Rows.Clear()
                Me.cboPartners.Text = Nothing
                Me.txtFileName.Text = ""
                Me.txttotalcount.Text = ""
                Me.lblDatetime.Text = ""
                Me.txtTotalPrincipal.Text = ""
                Me.txtBatchNo.Text = ""
                btnBrowse.Enabled = True
                Me.txtAmountDue.Text = ""
                Me.txtTotalCharges.Text = ""
                Me.txtConctactNum.Text = ""
                Me.txtAddress.Text = ""
                Me.totamountUSD.Text = ""
                Me.txtPrincipalUSD.Text = ""
                Me.txtChargeUSD.Text = ""
                Me.txtcountUSD.Text = ""
                cboPartners.Enabled = True
                Me.cboPartners.Select()
                'Me.DataGrid.AllowUserToAddRows = False
                'cboPartners.Items.Clear()
                clearGrid()
            End Try
        Else
            MsgBox("Invalid File for MONEY EXCHANGE")
            Me.DataGrid.BringToFront()
            Me.DataGrid.Visible = True
            Me.DataGridView1.Rows.Clear()
            Me.DataGridView4.Rows.Clear()
            Me.DataGridView3.Rows.Clear()
            Me.DataGridView2.Rows.Clear()
            Me.DataGridView5.Rows.Clear()
            Me.DataGrid.Rows.Clear()
            Me.cboPartners.Text = Nothing
            Me.txtFileName.Text = ""
            Me.txttotalcount.Text = ""
            Me.lblDatetime.Text = ""
            Me.txtTotalPrincipal.Text = ""
            Me.txtBatchNo.Text = ""
            btnBrowse.Enabled = True
            Me.txtAmountDue.Text = ""
            Me.txtTotalCharges.Text = ""
            Me.txtConctactNum.Text = ""
            Me.txtAddress.Text = ""
            Me.totamountUSD.Text = ""
            Me.txtPrincipalUSD.Text = ""
            Me.txtChargeUSD.Text = ""
            Me.txtcountUSD.Text = ""
            cboPartners.Enabled = True
            Me.cboPartners.Select()
            Me.DataGrid.AllowUserToAddRows = False
        End If
    End Sub

    Public Sub parseData()
        'Dim sdate As DateTime
        Dim response As New KPServices.MLhuillierSoapClient("MLhuillierSoap2", plg.getkpservicesendpoint)
        Dim ret As New KPServices.kptnResponse

        Dim PHPcounter As Integer = 0
        Dim USDcounter As Integer = 0
        Dim totalphpcharge As Double = 0
        Dim totalusdcharge As Double = 0
        Dim totalprincipalPHP As Double = 0
        Dim totalprincipalUSD As Double = 0
        'String stationcode, String zonecode,  String operatorid , String stationid
        Dim chargevalue As Decimal
        Dim serv3 As New PluginLogic
        Dim resp2 As New FileUploadService.MLhuillierSoapClient("MLhuillierSoap8", serv3.getAdminPartnersFileUploadServiceendpoint())
        Dim rest2 As New FileUploadService.ChargeValue()


        Dim total As Double
        Dim totalcharge As Double
        Dim rowvalue As String
        Dim rowcount As Integer = 0
        Dim appPath As String = OpenFileDialog1.FileName
        Dim fileEntries As New List(Of String)
        Dim branchid As String = Nothing
        If Not File.Exists(appPath) Then
            Exit Sub
        Else
            Dim read As StreamReader = New StreamReader(appPath)
            Dim rdr As String
            fileEntries.Clear()
            rdr = read.ReadLine()
            branchid = rdr.Substring(10, 6).ToString().Trim()
            read.Close()
        End If
        If oldAcc = branchid Then
            Try
                ' Read file into the list ...
                Dim reader As StreamReader = New StreamReader(appPath)
                fileEntries.Clear()
                Dim row As Integer = 0
                Dim dgvrow As New DataGridViewRow
                While reader.Peek <> -1
                    rowvalue = reader.ReadLine()
                    If rowcount = 0 Then
                        rowcount = 1
                        'for header
                    Else
                        If rowvalue.Trim = "" Then
                            Continue While
                        End If

                        Me.DataGrid.Rows.Add()
                        'for details

                        Me.DataGrid.Rows(row).Cells(0).Value = rowvalue.Substring(0, 5).ToString
                        ret = response.getKptn("boswebserviceusr", "boyursa805", branchcode, zonecode, vfile, stationcode)
                        Me.DataGrid.Rows(row).Cells(1).Value = ret.kptn
                        'Me.DataGrid.Rows(row).Cells(1).Value = rowvalue.Substring(5, 15).ToString
                        Me.DataGrid.Rows(row).Cells(2).Value = rowvalue.Substring(20, 19).ToString
                        Me.DataGrid.Rows(row).Cells(3).Value = rowvalue.Substring(39, 20).ToString
                        Try
                            'MsgBox(rowvalue.Substring(59, 12).ToString)
                            Me.DataGrid.Rows(row).Cells(4).Value = Format(CDbl(rowvalue.Substring(59, 12).ToString), "#,###,##0.00")
                        Catch ex As Exception
                            MsgBox("Invalid File", MsgBoxStyle.Critical)
                            clearGrid()
                            clear()
                            Exit Sub
                        End Try


                        'MsgBox(rowvalue.Substring(71, 3).ToString)
                        Dim currency As String = rowvalue.Substring(71, 3).ToString
                        'If cbo_curr <> rowvalue.Substring(71, 3).ToString Then
                        '    MsgBox("Invalid Currency.", MsgBoxStyle.OkOnly, "File Upload")
                        '    Me.DataGrid.Rows.Clear()
                        '    Me.btnCancel.PerformClick()
                        '    clearGrid()
                        '    Exit Sub
                        'End If
                       
                        Dim amount As Decimal = Format(CDbl(rowvalue.Substring(59, 12).ToString), "#,###,##0.00")
                        rest2 = resp2.ChargesPerLine(accId, amount, currency)
                        chargevalue = rest2.Charges

                        '***************new code by cani 3/14/2013*************************
                        If currency = "PHP" Then
                            PHPcounter = PHPcounter + 1
                            totalphpcharge = totalphpcharge + chargevalue
                            totalprincipalPHP = totalprincipalPHP + amount
                        ElseIf currency = "USD" Then
                            USDcounter = USDcounter + 1
                            totalusdcharge = totalusdcharge + chargevalue
                            totalprincipalUSD = totalprincipalUSD + amount
                        End If
                        '******************************************************************
                        Me.DataGrid.Rows(row).Cells(5).Value = rowvalue.Substring(71, 3).ToString
                        Try
                            'If CDbl(rowvalue.Substring(74, 10).ToString) Then
                            '    Me.DataGrid.Rows(row).Cells(6).Value = 
                            '    'Format(CDbl(chargevalue), "#,###,##0.00")
                            'End If
                            If CDbl(chargevalue) Then
                                Me.DataGrid.Rows(row).Cells(6).Value = chargevalue

                            ElseIf CDbl(chargevalue) = 0 Then
                                Me.DataGrid.Rows(row).Cells(6).Value = "0.00"
                            End If
                        Catch ex As Exception
                            Me.DataGrid.Rows(row).Cells(6).Value = "0.00"
                        End Try
                        'Me.DataGrid.Rows(row).Cells(6).Value = Format(CDbl(rowvalue.Substring(74, 10).ToString), "#,###,##0.00")
                        Me.DataGrid.Rows(row).Cells(7).Value = rowvalue.Substring(84, 15).ToString
                        Me.DataGrid.Rows(row).Cells(8).Value = rowvalue.Substring(99, 20).ToString
                        Me.DataGrid.Rows(row).Cells(9).Value = rowvalue.Substring(119, 30).ToString
                        Me.DataGrid.Rows(row).Cells(10).Value = rowvalue.Substring(149, 20).ToString
                        Me.DataGrid.Rows(row).Cells(11).Value = rowvalue.Substring(169, 20).ToString
                        Me.DataGrid.Rows(row).Cells(12).Value = rowvalue.Substring(189, 20).ToString
                        Me.DataGrid.Rows(row).Cells(13).Value = rowvalue.Substring(209, 10).ToString
                        Me.DataGrid.Rows(row).Cells(14).Value = rowvalue.Substring(219, 20).ToString
                        Me.DataGrid.Rows(row).Cells(15).Value = rowvalue.Substring(239, 10).ToString
                        Me.DataGrid.Rows(row).Cells(16).Value = rowvalue.Substring(249, 10).ToString
                        Me.DataGrid.Rows(row).Cells(17).Value = rowvalue.Substring(259, 1).ToString
                        Me.DataGrid.Rows(row).Cells(18).Value = rowvalue.Substring(260, 30).ToString
                        Me.DataGrid.Rows(row).Cells(19).Value = rowvalue.Substring(290, 25).ToString
                        Me.DataGrid.Rows(row).Cells(20).Value = rowvalue.Substring(315, 25).ToString
                        Me.DataGrid.Rows(row).Cells(21).Value = rowvalue.Substring(340, 20).ToString
                        Me.DataGrid.Rows(row).Cells(22).Value = rowvalue.Substring(360, 20).ToString
                        Me.DataGrid.Rows(row).Cells(23).Value = rowvalue.Substring(380, 20).ToString
                        Me.DataGrid.Rows(row).Cells(24).Value = rowvalue.Substring(400, 20).ToString
                        Me.DataGrid.Rows(row).Cells(25).Value = rowvalue.Substring(420, 30).ToString
                        Me.DataGrid.Rows(row).Cells(26).Value = rowvalue.Substring(450, 25).ToString
                        Me.DataGrid.Rows(row).Cells(27).Value = rowvalue.Substring(475, 25).ToString
                        Me.DataGrid.Rows(row).Cells(28).Value = rowvalue.Substring(500, 10).ToString
                        Me.DataGrid.Rows(row).Cells(29).Value = rowvalue.Substring(510, 1).ToString
                        Me.DataGrid.Rows(row).Cells(30).Value = rowvalue.Substring(511, 20).ToString
                        Me.DataGrid.Rows(row).Cells(31).Value = rowvalue.Substring(531, 50).ToString
                        'Me.DataGrid.Rows(row).Cells(31).Value = rowvalue.Substring(531, rowvalue.Length - 531).ToString

                        row += 1
                        Dim y As String
                        y = DataGrid.Item(0, DataGrid.CurrentRow.Index).Value
                        End If
                End While
                reader.Close()
                Dim x As Integer
                Dim charge As Double
                Dim principal As Double
                For i = 0 To Me.DataGrid.Rows.Count - 1
                    Try
                        If CDbl(Me.DataGrid.Rows(i).Cells(4).Value) Then
                            principal = CDbl(Me.DataGrid.Rows(i).Cells(4).Value)
                        End If
                    Catch ex As Exception
                        Me.DataGrid.Rows(i).Cells(4).Value = "#,###,##0.0"
                    Finally
                        total = total + CDbl(principal)
                    End Try
                    Try
                        If CDbl(Me.DataGrid.Rows(i).Cells(6).Value) Then
                            charge = CDbl(Me.DataGrid.Rows(i).Cells(6).Value)
                        End If
                    Catch ex As Exception
                        Me.DataGrid.Rows(i).Cells(6).Value = "#,###,##0.0"
                        charge = Me.DataGrid.Rows(i).Cells(6).Value
                    Finally
                        totalcharge = totalcharge + CDbl(charge)
                    End Try

                Next
                '**********************cani new code 3/13/2013********************
                Me.txttotalcount.Text = PHPcounter
                Me.txtcountUSD.Text = USDcounter
                Dim totalAmountDuePHP As Decimal = totalprincipalPHP + totalphpcharge
                Dim totalAmountDueUSD As Decimal = totalprincipalUSD + totalusdcharge

                If totalphpcharge.ToString.Length > 3 Then
                    Me.txtTotalCharges.Text = Format(totalphpcharge, "#,###,##0.00")
                ElseIf totalphpcharge.ToString.Length > 6 Then
                    Me.txtTotalCharges.Text = Format(totalphpcharge, "#,###,##0.00")
                Else
                    Me.txtTotalCharges.Text = Format(totalphpcharge, "#,###,##0.00")
                End If

                If totalusdcharge.ToString.Length > 3 Then
                    Me.txtChargeUSD.Text = Format(totalusdcharge, "#,###,##0.00")
                ElseIf totalusdcharge.ToString.Length > 6 Then
                    Me.txtChargeUSD.Text = Format(totalusdcharge, "#,###,##0.00")
                Else
                    Me.txtChargeUSD.Text = Format(totalusdcharge, "#,###,##0.00")
                End If

                If totalprincipalPHP.ToString.Length > 3 Then
                    Me.txtAmountDue.Text = Format(totalAmountDuePHP, "#,###,##0.00")
                    Me.txtTotalPrincipal.Text = Format(totalprincipalPHP, "#,###,##0.00")
                ElseIf totalprincipalPHP.ToString.Length > 6 Then
                    Me.txtAmountDue.Text = Format(totalAmountDuePHP, "#,###,##0.00")
                    Me.txtTotalPrincipal.Text = Format(totalprincipalPHP, "#,###,##0.00")
                Else
                    Me.txtAmountDue.Text = Format(totalAmountDuePHP, "#,###,##0.00")
                    Me.txtTotalPrincipal.Text = Format(totalprincipalPHP, "#,###,##0.00")
                End If


                If totalprincipalUSD.ToString.Length > 3 Then
                    Me.totamountUSD.Text = Format(totalAmountDueUSD, "#,###,##0.00")
                    Me.txtPrincipalUSD.Text = Format(totalprincipalUSD, "#,###,##0.00")
                ElseIf totalprincipalUSD.ToString.Length > 6 Then
                    Me.totamountUSD.Text = Format(totalAmountDueUSD, "#,###,##0.00")
                    Me.txtPrincipalUSD.Text = Format(totalprincipalUSD, "#,###,##0.00")
                Else
                    Me.totamountUSD.Text = Format(totalAmountDueUSD, "#,###,##0.00")
                    Me.txtPrincipalUSD.Text = Format(totalprincipalUSD, "#,###,##0.00")
                End If

                '*****************************************************************

                '******************kentoy code************************
                'Me.txttotalcount.Text = Me.DataGrid.Rows.Count
                'If totalcharge.ToString.Length > 3 Then
                '    Me.txtTotalCharges.Text = Format(totalcharge, "#,###,##0.00")
                'ElseIf totalcharge.ToString.Length > 6 Then
                '    Me.txtTotalCharges.Text = Format(totalcharge, "#,###,##0.00")
                'Else
                '    Me.txtTotalCharges.Text = Format(totalcharge, "#,###,##0.00")
                'End If

                'If total.ToString.Length > 3 Then
                '    Me.txtAmountDue.Text = Format(total, "#,###,##0.00")
                '    Me.txtTotalPrincipal.Text = Format(total, "#,###,##0.00")
                'ElseIf total.ToString.Length > 6 Then
                '    Me.txtAmountDue.Text = Format(total, "#,###,##0.00")
                '    Me.txtTotalPrincipal.Text = Format(total, "#,###,##0.00")
                'Else
                '    Me.txtAmountDue.Text = Format(total, "#,###,##0.00")
                '    Me.txtTotalPrincipal.Text = Format(total, "#,###,##0.00")
                'End If
                '***********************************************************
                x = DataGrid.Rows.Count
                cboPartners.Enabled = False
                btnBrowse.Enabled = False
                'exclude last row
                Me.DataGrid.AllowUserToAddRows = False

                Dim serv2 As New PluginLogic()
                'Dim resp As New FileUpload.MLhuillier
                'Dim rest As New FileUpload.getserverDate()
                Dim resp As New FileUploadService.MLhuillierSoapClient("MLhuillierSoap8", serv2.getAdminPartnersFileUploadServiceendpoint())
                Dim rest As New FileUploadService.SearchPartners()
                Dim sd As String = resp.GetDate().getfromDateServer.ToString()
                Me.lblDatetime.Text = Convert.ToDateTime(sd).ToString("yyyy-MM-dd hh:mm:ss")
            Catch ex As Exception
                MsgBox("Invalid File for " + cboPartners.Text + "", MsgBoxStyle.Critical)
                Me.DataGrid.BringToFront()
                Me.DataGrid.Visible = True
                Me.DataGrid.AllowUserToAddRows = False
                'Me.DataGridView1.Rows.Clear()
                Me.DataGridView4.Rows.Clear()
                Me.DataGridView3.Rows.Clear()
                Me.DataGridView2.Rows.Clear()
                Me.DataGrid.Rows.Clear()
                Me.cboPartners.Text = Nothing
                Me.txtFileName.Text = ""
                Me.txttotalcount.Text = ""
                Me.lblDatetime.Text = ""
                Me.txtTotalPrincipal.Text = ""
                Me.txtBatchNo.Text = ""
                btnBrowse.Enabled = True
                Me.txtAmountDue.Text = ""
                Me.txtTotalCharges.Text = ""
                Me.txtConctactNum.Text = ""
                Me.txtAddress.Text = ""
                Me.totamountUSD.Text = ""
                Me.txtPrincipalUSD.Text = ""
                Me.txtChargeUSD.Text = ""
                Me.txtcountUSD.Text = ""
                cboPartners.Enabled = True
                Me.cboPartners.Select()
                Me.DataGrid.AllowUserToAddRows = False
                'cboPartners.Items.Clear()
                clearGrid()
            End Try
        Else
            MsgBox("Invalid File for '" + Me.cboPartners.Text + "'")
            Me.bntUpload.Enabled = False
            Me.cboPartners.Text = Nothing
            Me.DataGrid.Rows.Clear()
            Me.cboPartners.Enabled = True
            Me.btnBrowse.Enabled = True
            Me.cboPartners.Text = ""
            Me.txtFileName.Text = ""
            Me.txtFileName.Text = ""
            Me.txtBatchNo.Text = ""
            Me.DataGridView1.Rows.Clear()
            Me.txttotalcount.Text = ""
            Me.lblDatetime.Text = ""
            Me.txtTotalPrincipal.Text = ""
            Me.txtAmountDue.Text = ""
            Me.txtTotalCharges.Text = ""
            Me.txtConctactNum.Text = ""
            Me.txtAddress.Text = ""
            Me.totamountUSD.Text = ""
            Me.txtPrincipalUSD.Text = ""
            Me.txtChargeUSD.Text = ""
            Me.txtcountUSD.Text = ""
            Me.OpenFileDialog1.FilterIndex = 0
            Me.DataGridView2.Rows.Clear()
            Me.DataGridView3.Rows.Clear()
            Me.DataGridView4.Rows.Clear()
            Me.DataGridView5.Rows.Clear()
        End If
    End Sub
    Private Sub bntUpload_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bntUpload.Click
        Dim response As MsgBoxResult = MsgBoxResult.Yes

        'Dim dataArray As New FileUploadService.ArrayOfString
        If OpenFileDialog1.FilterIndex = 1 Then

            If oldAcc = "BPINOY" Then
                PartnersNameIdentifier = "BPI"
                If response = MsgBox("Do you want to verify your data?", MsgBoxStyle.YesNo, "File Upload") Then
                    MsgBox("Done Checking....", MsgBoxStyle.OkOnly, "File Upload")
                Else

                End If
            ElseIf oldAcc = "FSD7" Then
                PartnersNameIdentifier = "DBP"
                If response = MsgBox("Do you want to verify your data?", MsgBoxStyle.YesNo, "File Upload") Then
                    MsgBox("Done Checking....", MsgBoxStyle.OkOnly, "File Upload")
                Else

                End If
            ElseIf oldAcc = "FSD35" Then
                PartnersNameIdentifier = "BDO"
                If response = MsgBox("Do you want to verify your data?", MsgBoxStyle.YesNo, "File Upload") Then
                    MsgBox("Done Checking....", MsgBoxStyle.OkOnly, "File Upload")
                Else

                End If
            ElseIf oldAcc = "MOEX" Then
                PartnersNameIdentifier = "MOEX"
                If response = MsgBox("Do you want to verify your data?", MsgBoxStyle.YesNo, "File Upload") Then
                    MsgBox("Done Checking....", MsgBoxStyle.OkOnly, "File Upload")
                Else

                End If
            Else
                PartnersNameIdentifier = "P2P"
                If response = MsgBox("Do you want to verify your data?", MsgBoxStyle.YesNo, "File Upload") Then
                    Dim reader As New System.IO.StreamReader(Me.txtFileName.Text)
                    Dim ctr As Integer = 0
                    While reader.Peek <> -1
                        Dim line As String = reader.ReadLine

                        If line.Trim = "" Then
                            Continue While
                        End If
                        If ctr = 0 Then
                            If ReadLine(line, True, ctr) Then
                            Else
                                Exit While
                            End If
                        Else
                            If ReadLine(line, False, ctr) Then
                            Else
                                Exit While
                            End If
                        End If
                        ctr = ctr + 1
                    End While
                    MsgBox("Done Checking....", MsgBoxStyle.OkOnly, "File Upload")
                Else

                End If
            End If

        ElseIf OpenFileDialog1.FilterIndex = 3 Then
            If response = MsgBox("Do you want to verify your data?", MsgBoxStyle.YesNo, "File Upload") Then
                MsgBox("Done Checking....", MsgBoxStyle.OkOnly, "File Upload")
            End If
        End If
        If OpenFileDialog1.FilterIndex = 1 Then
            InsertTXTFile()
        ElseIf OpenFileDialog1.FilterIndex = 3 Then
            'InsertXLSFile()
        End If
    End Sub
    'Public Sub InsertXLSFile()
    '    Dim resp2 As New KPServices.MLhuillierSoapClient("MLhuillierSoap2", plg.getkpservicesendpoint)
    '    Dim ret2 As New KPServices.kptnResponse
    '    Dim kptnno As String
    '    Dim dataArray As New FileUploadService.ArrayOfString
    '    Dim gridItems As String
    '    Dim totalPrincipal As Decimal = Decimal.Parse(txtTotalPrincipal.Text)
    '    For Each row As DataGridViewRow In DataGridView1.Rows
    '        ret2 = resp2.getKptn("boswebserviceusr", "boyursa805", branchcode, zonecode, vfile, stationcode)
    '        kptnno = ret2.kptn
    '        gridItems = row.Cells(0).Value + "|" + row.Cells(1).Value + "|" + row.Cells(2).Value + "|" + row.Cells(3).Value + "|" + row.Cells(4).Value + "|" + row.Cells(5).Value + "|" + row.Cells(6).Value + "|" + row.Cells(7).Value + "|" + row.Cells(8).Value + "|" + row.Cells(9).Value + "|" + row.Cells(10).Value + "|" + row.Cells(11).Value + "|" + row.Cells(12).Value + "|" + kptnno
    '        dataArray.Add(gridItems)
    '    Next

    '    Dim ret As Integer

    '    Dim arrDup() As String
    '    Dim arrInsert() As String
    '    Dim arrTier() As String
    '    Dim arrSuc() As String
    '    Dim arrDupLength As Long = 0
    '    Dim arrErrorInsert As Long = 0
    '    Dim arrErrorTierCode As Long = 0
    '    Dim arrSuccess As Long = 0
    '    Dim msg As String = ""
    '    Dim btchnum As String = Me.txtBatchNo.Text
    '    Dim serv2 As New PluginLogic()
    '    Dim resp1 As New FileUploadService.MLhuillierSoapClient("MLhuillierSoap8", serv2.getAdminPartnersFileUploadServiceendpoint())
    '    Dim rest As New FileUploadService.UploadFile()
    '    Dim restInt As New FileUploadService.getIntegrationType()
    '    Dim getresp As String = Nothing

    '    restInt = resp1.IntegrationType(accountidInt)
    '    getresp = restInt.IntegrationType

    '    '  rest = resp1.FileUpload(dataArray, accId, btchnum, branchname, branchcode, stationcode, zonecode, operatorID, stationID, "EXCEL", totalPrincipal, cbo_curr,)
    '    ret = rest.response





    '    msg = rest.msg
    '    If ret = 1 And msg = "" Then
    '        'MsgBox("File has been already uploaded.", MsgBoxStyle.OkOnly, "File Upload")
    '        'MsgBox(rest.msg, MsgBoxStyle.OkOnly, "File Upload")
    '        'arrSuc = rest.dataSuccessFull.ToArray
    '        'arrDup = rest.dataDuplicate.ToArray
    '        'arrInsert = rest.dataErrorInserting.ToArray
    '        'arrTier = rest.dataErrorTier.ToArray
    '        Dim view As New FrmReportError
    '        'rest.BatchNumber, Me.txtBatchNo.Text, Me.cboPartners.SelectedItem.ToString
    '        view.CollectData(arrSuc, arrTier, arrInsert, arrDup, "EXCEL", "", rest.BatchNumber, Me.txtBatchNo.Text)
    '        view.ShowDialog()
    '        Me.DataGridView1.Rows.Clear()
    '        Me.cboPartners.Enabled = True
    '        Me.btnBrowse.Enabled = True
    '        Me.cboPartners.Text = ""
    '        Me.txtFileName.Text = ""
    '        Me.cboPartners.Text = Nothing
    '        Me.bntUpload.Enabled = False
    '        Me.txtFileName.Text = ""
    '        Me.txttotalcount.Text = ""
    '        Me.lblDatetime.Text = ""
    '        Me.txtTotalPrincipal.Text = ""
    '        Me.txtBatchNo.Text = ""
    '        Me.txtAmountDue.Text = ""
    '        Me.txtTotalCharges.Text = ""
    '        Me.txtConctactNum.Text = ""
    '        Me.txtAddress.Text = ""
    '        clearGrid()

    '    ElseIf ret = 1 And msg <> "" Then

    '        MsgBox(msg)
    '        Me.DataGridView1.Rows.Clear()
    '        Me.cboPartners.Enabled = True
    '        Me.btnBrowse.Enabled = True
    '        Me.cboPartners.Text = ""
    '        Me.txtFileName.Text = ""
    '        Me.cboPartners.Text = Nothing
    '        Me.bntUpload.Enabled = False
    '        Me.txtFileName.Text = ""
    '        Me.txttotalcount.Text = ""
    '        Me.lblDatetime.Text = ""
    '        Me.txtTotalPrincipal.Text = ""
    '        Me.txtBatchNo.Text = ""
    '        Me.txtAmountDue.Text = ""
    '        Me.txtTotalCharges.Text = ""
    '        Me.txtConctactNum.Text = ""
    '        Me.txtAddress.Text = ""
    '        clearGrid()

    '    ElseIf ret = 0 Then

    '        MsgBox("File successfully uploaded." + vbNewLine + "Batch No. =" + rest.BatchNumber, MsgBoxStyle.OkOnly, "File Upload")
    '        Dim f As New frmFileUploadPrint
    '        f.passdata(dataArray.ToArray, rest.BatchNumber, Me.txtBatchNo.Text, Me.cboPartners.SelectedItem.ToString, cbo_curr, getresp)
    '        f.ShowDialog()
    '        Me.DataGridView2.Rows.Clear()
    '        Me.cboPartners.Enabled = True
    '        Me.btnBrowse.Enabled = True
    '        Me.cboPartners.Text = ""
    '        Me.txtFileName.Text = ""
    '        Me.cboPartners.Text = Nothing
    '        Me.bntUpload.Enabled = False
    '        Me.txtFileName.Text = ""
    '        Me.txttotalcount.Text = ""
    '        Me.lblDatetime.Text = ""
    '        Me.txtTotalPrincipal.Text = ""
    '        Me.txtBatchNo.Text = ""
    '        Me.txtAmountDue.Text = ""
    '        Me.txtTotalCharges.Text = ""
    '        Me.txtConctactNum.Text = ""
    '        Me.txtAddress.Text = ""
    '        datalist.Clear()
    '        clearGrid()
    '    End If
    'End Sub

    Public Sub clearGrid()
        clear()

        Me.DataGrid.AllowUserToAddRows = False
        Me.DataGridView1.AllowUserToAddRows = False
        Me.DataGridView2.AllowUserToAddRows = False
        Me.DataGridView4.AllowUserToAddRows = False

        Me.DataGrid.Visible = True
        Me.DataGrid.BringToFront()


    End Sub
    

    Public Sub InsertTXTFile()
        Dim totalPrincipal As Decimal = txtTotalPrincipal.Text
        If oldAcc = "MOEX" Then
            Dim res As Integer
            Dim response As New KPServices.MLhuillierSoapClient("MLhuillierSoap2", plg.getkpservicesendpoint)
            Dim ret1 As New KPServices.kptnResponse
            ret1 = response.getKptn("boswebserviceusr", "boyursa805", branchcode, zonecode, vfile, stationcode)
            Dim l As Integer
            l = Me.DataGridView5.Rows.Count - 1
            Dim dataArray As New FileUploadService.ArrayOfString

            For Each r As DataGridViewRow In DataGridView5.Rows
                ret1 = response.getKptn("boswebserviceusr", "boyursa805", branchcode, zonecode, vfile, stationcode)
                dataitem = r.Cells(0).Value & "|" & r.Cells(1).Value & "|" & r.Cells(2).Value & "|" & r.Cells(3).Value & "|" & r.Cells(4).Value & "|" & r.Cells(5).Value & "|" & r.Cells(6).Value & "|" & r.Cells(7).Value & "|" & r.Cells(8).Value & "|" & r.Cells(9).Value & "|" & r.Cells(10).Value & "|" & r.Cells(11).Value & "|" & r.Cells(12).Value & "|" & r.Cells(13).Value & "|" & r.Cells(14).Value & "|" & r.Cells(15).Value & "|" & r.Cells(16).Value & "|" & r.Cells(17).Value & "|" & r.Cells(18).Value & "|" & r.Cells(19).Value & "|" & r.Cells(20).Value & "|" & r.Cells(21).Value & "|" & r.Cells(22).Value & "|" & r.Cells(23).Value & "|" & r.Cells(24).Value & "|" & r.Cells(25).Value & "|" & r.Cells(26).Value & "|" & r.Cells(27).Value & "|" & r.Cells(28).Value & "|" & r.Cells(29).Value & "|" & r.Cells(30).Value & "|" & ret1.kptn & "|" & r.Cells(31).Value
                dataArray.Add(dataitem)
                moex_curr = r.Cells(21).Value.ToString
                dataitem = ""
            Next
           
            res = FileUploadMethod(dataArray, accId, Me.txtBatchNo.Text, branchname, branchcode, stationcode, zonecode, operatorID, stationID, "TESTINGMOEX", totalPrincipal, moex_curr, 0)
            If res = 0 Then
                viewReport("TEXT", MLbatchNumber, oldAcc, operatorID, res, 0)
            ElseIf res = 1 Then
                'clearData()
                MsgBox("Successfully Upload, Please Click the Submit Button ", MsgBoxStyle.Information)
                submitBTN.Visible = True
                bntUpload.Visible = False
            Else
                MsgBox(RespMsg, MsgBoxStyle.Critical)
                clearData()
            End If

        ElseIf oldAcc = "BPINOY" Then
            Dim res As Integer
            Dim dataArray As New FileUploadService.ArrayOfString
            Dim m As Integer = 0
            For Each r As DataGridViewRow In DataGridView2.Rows
                dataitem = r.Cells(0).Value & "|" & r.Cells(1).Value & "|" & r.Cells(2).Value & "|" & r.Cells(3).Value & "|" & r.Cells(4).Value & "|" & r.Cells(5).Value & "|" & r.Cells(6).Value & "|" & r.Cells(7).Value & "|" & r.Cells(8).Value & "|" & r.Cells(9).Value & "|" & r.Cells(10).Value & "|" & r.Cells(11).Value & "|" & r.Cells(12).Value & "|" & r.Cells(13).Value & "|" & r.Cells(14).Value & "|" & r.Cells(15).Value & "|" & r.Cells(16).Value & "|" & r.Cells(17).Value & "|" & r.Cells(18).Value & "|" & r.Cells(19).Value & "|" & r.Cells(20).Value & "|" & r.Cells(21).Value & "|" & r.Cells(22).Value & "|" & r.Cells(23).Value & "|" & r.Cells(24).Value
                dataArray.Add(dataitem)
                bpi_curr = r.Cells(22).Value.ToString
                dataitem = ""
            Next

            res = FileUploadMethod(dataArray, accId, Me.txtBatchNo.Text, branchname, branchcode, stationcode, zonecode, operatorID, stationID, "TESTINGBPI", totalPrincipal, bpi_curr, 0)
            If res = 0 Then
                viewReport("TEXT", MLbatchNumber, oldAcc, operatorID, res, 0)
            ElseIf res = 1 Then
                'clearData()
                MsgBox("Successfully Upload, Please Click the Submit Button ", MsgBoxStyle.Information)
                submitBTN.Visible = True
                bntUpload.Visible = False
            Else
                MsgBox(RespMsg, MsgBoxStyle.Critical)
                clearData()
            End If

        ElseIf oldAcc = "FSD35" Then 'BDO
            Dim res As Integer
            Dim dataArray1 As New FileUploadService.ArrayOfString
            For Each r As DataGridViewRow In DataGridView4.Rows
                dataitem = r.Cells(0).Value & "|" & r.Cells(1).Value & "|" & r.Cells(2).Value & "|" & r.Cells(3).Value & "|" & r.Cells(4).Value & "|" & r.Cells(5).Value & "|" & r.Cells(6).Value & "|" & r.Cells(7).Value & "|" & r.Cells(8).Value & "|" & r.Cells(9).Value & "|" & r.Cells(10).Value & "|" & r.Cells(11).Value & "|" & r.Cells(12).Value & "|" & r.Cells(13).Value & "|" & r.Cells(14).Value & "|" & r.Cells(15).Value & "|" & r.Cells(16).Value & "|" & r.Cells(17).Value & "|" & r.Cells(18).Value & "|" & r.Cells(19).Value & "|" & r.Cells(20).Value & "|" & r.Cells(21).Value
                dataArray1.Add(dataitem)
                bdo_curr = r.Cells(9).Value.ToString
                dataitem = ""
            Next

            res = FileUploadMethod(dataArray1, accId, Me.txtBatchNo.Text, branchname, branchcode, stationcode, zonecode, operatorID, stationID, "TESTINGBDO", totalPrincipal, bdo_curr, 0)
            If res = 0 Then
                viewReport("TEXT", MLbatchNumber, oldAcc, operatorID, res, 0)
            ElseIf res = 1 Then
                'clearData()
                MsgBox("Successfully Upload, Please Click the Submit Button ", MsgBoxStyle.Information)
                submitBTN.Visible = True
                bntUpload.Visible = False
            Else
                MsgBox(RespMsg, MsgBoxStyle.Critical)
                clearData()
            End If

        ElseIf oldAcc = "FSD7" Then 'DBP
            Dim res As Integer
            Dim dataArray1 As New FileUploadService.ArrayOfString
            For Each r As DataGridViewRow In DataGridView3.Rows
                dataitem = r.Cells(0).Value & "|" & r.Cells(1).Value & "|" & r.Cells(2).Value & "|" & r.Cells(3).Value & "|" & r.Cells(4).Value & "|" & r.Cells(5).Value & "|" & r.Cells(6).Value & "|" & r.Cells(7).Value & "|" & r.Cells(8).Value & "|" & r.Cells(9).Value & "|" & r.Cells(10).Value & "|" & r.Cells(11).Value & "|" & r.Cells(12).Value & "|" & r.Cells(13).Value & "|" & r.Cells(14).Value & "|" & r.Cells(15).Value & "|PHP"
                dataArray1.Add(dataitem)
                dataitem = ""
            Next

            res = FileUploadMethod(dataArray1, accId, Me.txtBatchNo.Text, branchname, branchcode, stationcode, zonecode, operatorID, stationID, "TESTINGDBP", totalPrincipal, "PHP", 0)

            If res = 0 Then
                viewReport("TEXT", MLbatchNumber, oldAcc, operatorID, res, 0)
            ElseIf res = 1 Then
                'clearData()
                MsgBox("Successfully Upload, Please Click the Submit Button ", MsgBoxStyle.Information)
                submitBTN.Visible = True
                bntUpload.Visible = False
            Else
                MsgBox(RespMsg, MsgBoxStyle.Critical)
                clearData()
            End If

        Else
            Dim res As Integer

            Dim dataArray As New FileUploadService.ArrayOfString

            Dim o As Integer = 0

            Dim row12 As String = "."
            Dim row24 As String = "."
            For Each row As DataGridViewRow In DataGrid.Rows



                'MsgBox(row.Cells(0).Value.ToString & "|" & row.Cells(1).Value.ToString & "|" & row.Cells(2).Value.ToString & "|" & row.Cells(3).Value.ToString & "|" & row.Cells(4).Value.ToString & "|" & row.Cells(5).Value.ToString & "|" & row.Cells(6).Value.ToString & "|" & row.Cells(7).Value.ToString & "|" & row.Cells(8).Value.ToString & "|" & row.Cells(9).Value.ToString & "|" & Trim(row.Cells(10).Value.ToString) & "|" & Trim(row.Cells(11).Value.ToString) & "|" & Trim(row.Cells(12).Value.ToString) & "|" & row.Cells(13).Value.ToString & "|" & row.Cells(14).Value.ToString & "|" & row.Cells(15).Value.ToString & "|" & row.Cells(16).Value.ToString & "|" & row.Cells(17).Value.ToString & "|" & row.Cells(18).Value.ToString & "|" & row.Cells(19).Value.ToString & "|" & row.Cells(20).Value.ToString & "|" & row.Cells(21).Value.ToString & "|" & Trim(row.Cells(22).Value.ToString) & "|" & Trim(row.Cells(23).Value.ToString) & "|" & Trim(row.Cells(24).Value.ToString) & "|" & row.Cells(25).Value.ToString & "|" & row.Cells(26).Value.ToString & "|" & row.Cells(27).Value.ToString & "|" & row.Cells(28).Value.ToString & "|" & row.Cells(29).Value.ToString & "|" & row.Cells(30).Value.ToString & "|" & row.Cells(31).Value.ToString)
                Try
                    If row.Cells(12).Value.ToString = Nothing And row.Cells(24).ToString = Nothing Then
                        dataitems = row.Cells(0).Value.ToString & "|" & row.Cells(1).Value.ToString & "|" & row.Cells(2).Value.ToString & "|" & row.Cells(3).Value.ToString & "|" & row.Cells(4).Value.ToString & "|" & row.Cells(5).Value.ToString & "|" & row.Cells(6).Value.ToString & "|" & row.Cells(7).Value.ToString & "|" & row.Cells(8).Value.ToString & "|" & row.Cells(9).Value.ToString & "|" & Trim(row.Cells(10).Value.ToString) & "|" & Trim(row.Cells(11).Value.ToString) & "|" & row12 & "|" & row.Cells(13).Value.ToString & "|" & row.Cells(14).Value.ToString & "|" & row.Cells(15).Value.ToString & "|" & row.Cells(16).Value.ToString & "|" & row.Cells(17).Value.ToString & "|" & row.Cells(18).Value.ToString & "|" & row.Cells(19).Value.ToString & "|" & row.Cells(20).Value.ToString & "|" & row.Cells(21).Value.ToString & "|" & Trim(row.Cells(22).Value.ToString) & "|" & Trim(row.Cells(23).Value.ToString) & "|" & row24 & "|" & row.Cells(25).Value.ToString & "|" & row.Cells(26).Value.ToString & "|" & row.Cells(27).Value.ToString & "|" & row.Cells(28).Value.ToString & "|" & row.Cells(29).Value.ToString & "|" & row.Cells(30).Value.ToString & "|" & row.Cells(31).Value.ToString
                    ElseIf row.Cells(12).ToString = Nothing Then
                        dataitems = row.Cells(0).Value.ToString & "|" & row.Cells(1).Value.ToString & "|" & row.Cells(2).Value.ToString & "|" & row.Cells(3).Value.ToString & "|" & row.Cells(4).Value.ToString & "|" & row.Cells(5).Value.ToString & "|" & row.Cells(6).Value.ToString & "|" & row.Cells(7).Value.ToString & "|" & row.Cells(8).Value.ToString & "|" & row.Cells(9).Value.ToString & "|" & Trim(row.Cells(10).Value.ToString) & "|" & Trim(row.Cells(11).Value.ToString) & "|" & row12 & "|" & row.Cells(13).Value.ToString & "|" & row.Cells(14).Value.ToString & "|" & row.Cells(15).Value.ToString & "|" & row.Cells(16).Value.ToString & "|" & row.Cells(17).Value.ToString & "|" & row.Cells(18).Value.ToString & "|" & row.Cells(19).Value.ToString & "|" & row.Cells(20).Value.ToString & "|" & row.Cells(21).Value.ToString & "|" & Trim(row.Cells(22).Value.ToString) & "|" & Trim(row.Cells(23).Value.ToString) & "|" & row.Cells(24).Value.ToString & "|" & row.Cells(25).Value.ToString & "|" & row.Cells(26).Value.ToString & "|" & row.Cells(27).Value.ToString & "|" & row.Cells(28).Value.ToString & "|" & row.Cells(29).Value.ToString & "|" & row.Cells(30).Value.ToString & "|" & row.Cells(31).Value.ToString
                    ElseIf row.Cells(24).ToString = Nothing Then
                        dataitems = row.Cells(0).Value.ToString & "|" & row.Cells(1).Value.ToString & "|" & row.Cells(2).Value.ToString & "|" & row.Cells(3).Value.ToString & "|" & row.Cells(4).Value.ToString & "|" & row.Cells(5).Value.ToString & "|" & row.Cells(6).Value.ToString & "|" & row.Cells(7).Value.ToString & "|" & row.Cells(8).Value.ToString & "|" & row.Cells(9).Value.ToString & "|" & Trim(row.Cells(10).Value.ToString) & "|" & Trim(row.Cells(11).Value.ToString) & "|" & Trim(row.Cells(12).Value.ToString) & "|" & row.Cells(13).Value.ToString & "|" & row.Cells(14).Value.ToString & "|" & row.Cells(15).Value.ToString & "|" & row.Cells(16).Value.ToString & "|" & row.Cells(17).Value.ToString & "|" & row.Cells(18).Value.ToString & "|" & row.Cells(19).Value.ToString & "|" & row.Cells(20).Value.ToString & "|" & row.Cells(21).Value.ToString & "|" & Trim(row.Cells(22).Value.ToString) & "|" & Trim(row.Cells(23).Value.ToString) & "|" & row24 & "|" & row.Cells(25).Value.ToString & "|" & row.Cells(26).Value.ToString & "|" & row.Cells(27).Value.ToString & "|" & row.Cells(28).Value.ToString & "|" & row.Cells(29).Value.ToString & "|" & row.Cells(30).Value.ToString & "|" & row.Cells(31).Value.ToString
                    Else
                        dataitems = Trim(row.Cells(0).Value.ToString) & "|" & Trim(row.Cells(1).Value.ToString) & "|" & Trim(row.Cells(2).Value.ToString) & "|" & Trim(row.Cells(3).Value.ToString) & "|" & Trim(row.Cells(4).Value.ToString) & "|" & Trim(row.Cells(5).Value.ToString) & "|" & Trim(row.Cells(6).Value.ToString) & "|" & Trim(row.Cells(7).Value.ToString) & "|" & Trim(row.Cells(8).Value.ToString) & "|" & Trim(row.Cells(9).Value.ToString) & "|" & Trim(row.Cells(10).Value.ToString) & "|" & Trim(row.Cells(11).Value.ToString) & "|" & Trim(row.Cells(12).Value.ToString) & "|" & Trim(row.Cells(13).Value.ToString) & "|" & Trim(row.Cells(14).Value.ToString) & "|" & Trim(row.Cells(15).Value.ToString) & "|" & Trim(row.Cells(16).Value.ToString) & "|" & Trim(row.Cells(17).Value.ToString) & "|" & Trim(row.Cells(18).Value.ToString) & "|" & row.Cells(19).Value.ToString & "|" & Trim(row.Cells(20).Value.ToString) & "|" & Trim(row.Cells(21).Value.ToString) & "|" & Trim(row.Cells(22).Value.ToString) & "|" & Trim(row.Cells(23).Value.ToString) & "|" & Trim(row.Cells(24).Value.ToString) & "|" & Trim(row.Cells(25).Value.ToString) & "|" & Trim(row.Cells(26).Value.ToString) & "|" & Trim(row.Cells(27).Value.ToString) & "|" & Trim(row.Cells(28).Value.ToString) & "|" & Trim(row.Cells(29).Value.ToString) & "|" & Trim(row.Cells(30).Value.ToString) & "|" & Trim(row.Cells(31).Value.ToString)
                    End If
                Catch ex As Exception
                    MsgBox(ex.Message)
                End Try
                dataArray.Add(dataitems)


                p2p_curr = row.Cells(5).Value.ToString
                dataitems = ""
            Next

            res = FileUploadMethod(dataArray, accId, Me.txtBatchNo.Text, branchname, branchcode, stationcode, zonecode, operatorID, stationID, "TESTING", totalPrincipal, p2p_curr, 0)

            If res = 0 Then
                viewReport("TEXT", MLbatchNumber, oldAcc, operatorID, res, 0)
            ElseIf res = 1 Then
                'clearData()
                MsgBox("Successfully Upload, Please Click the Submit Button ", MsgBoxStyle.Information)
                submitBTN.Visible = True
                bntUpload.Visible = False
            Else
                MsgBox(RespMsg, MsgBoxStyle.Critical)
                clearData()
            End If
            
        End If

    End Sub
    Public Sub parseDataBPI()
        Dim response As New KPServices.MLhuillierSoapClient("MLhuillierSoap2", plg.getkpservicesendpoint)
        Dim ret As New KPServices.kptnResponse
        Dim appPath As String = OpenFileDialog1.FileName

        Dim serv3 As New PluginLogic
        Dim resp2 As New FileUploadService.MLhuillierSoapClient("MLhuillierSoap8", serv3.getAdminPartnersFileUploadServiceendpoint())
        Dim rest2 As New FileUploadService.ChargeValue()

        If appPath = Nothing Or appPath = "" Then
            Me.DataGrid.BringToFront()
            Me.DataGrid.Visible = True
            'Me.DataGridView1.Rows.Clear()
            Me.DataGridView2.Rows.Clear()
            Me.DataGrid.Rows.Clear()
            Me.cboPartners.Text = Nothing
            Me.txtFileName.Text = ""
            Me.txttotalcount.Text = ""
            Me.lblDatetime.Text = ""
            Me.txtTotalPrincipal.Text = ""
            Me.txtBatchNo.Text = ""
            btnBrowse.Enabled = True
            Me.txtAmountDue.Text = ""
            Me.txtTotalCharges.Text = ""
            Me.txtConctactNum.Text = ""
            Me.txtAddress.Text = ""
            Me.totamountUSD.Text = ""
            Me.txtPrincipalUSD.Text = ""
            Me.txtChargeUSD.Text = ""
            Me.txtcountUSD.Text = ""
            cboPartners.Enabled = True
            Me.cboPartners.Select()
            Me.DataGrid.AllowUserToAddRows = False
            'cboPartners.Items.Clear()
            clearGrid()
            Exit Sub
        End If
        'Dim Fname As String = Path.GetFileName(appPath).Substring(3, 3)
        Dim Fname As String = "BPINOY"
        Dim fileEntries As New List(Of String)

        If Not File.Exists(appPath) Then

            Exit Sub
        End If
        If oldAcc = Fname Then
            Try
                Dim SR As StreamReader = New StreamReader(appPath)
                fileEntries.Clear()
                Dim row As Integer = 0
                Dim rowvalue As String
                Dim splitLine() As String
                Dim sl() As String
                Dim sl1() As String
                Dim PHPcounter As Integer = 0
                Dim USDcounter As Integer = 0
                Dim totalprincipalPHP As Double = 0
                Dim totalprincipalUSD As Double = 0
                Dim totalphpcharge As Double = 0
                Dim totalusdcharge As Double = 0

                'Dim sl2() As String

                While SR.Peek <> -1
                    Me.DataGridView2.Rows.Add()
                    rowvalue = SR.ReadLine()
                    splitLine = Split(rowvalue, "|"c)
                    Me.DataGridView2.Rows(row).Cells(0).Value = splitLine(0).ToString
                    ret = response.getKptn("boswebserviceusr", "boyursa805", branchcode, zonecode, vfile, stationcode)
                    Me.DataGridView2.Rows(row).Cells(1).Value = ret.kptn
                    Me.DataGridView2.Rows(row).Cells(2).Value = splitLine(1).ToString
                    Me.DataGridView2.Rows(row).Cells(3).Value = splitLine(2).ToString
                    Me.DataGridView2.Rows(row).Cells(4).Value = splitLine(3).ToString
                    Me.DataGridView2.Rows(row).Cells(5).Value = splitLine(4).ToString
                    sl = Split(splitLine(5).ToString, " ")
                    Me.DataGridView2.Rows(row).Cells(6).Value = sl(0).ToString
                    Me.DataGridView2.Rows(row).Cells(7).Value = sl(1).ToString
                    Me.DataGridView2.Rows(row).Cells(8).Value = sl(2).ToString
                    sl1 = Split(splitLine(6).ToString, " ")
                    Me.DataGridView2.Rows(row).Cells(9).Value = sl1(0).ToString
                    Me.DataGridView2.Rows(row).Cells(10).Value = sl1(1).ToString
                    Me.DataGridView2.Rows(row).Cells(11).Value = sl1(2).ToString
                    Me.DataGridView2.Rows(row).Cells(12).Value = splitLine(7).ToString
                    Me.DataGridView2.Rows(row).Cells(13).Value = splitLine(8).ToString
                    Me.DataGridView2.Rows(row).Cells(16).Value = splitLine(9).ToString
                    Me.DataGridView2.Rows(row).Cells(17).Value = splitLine(10).ToString
                    Me.DataGridView2.Rows(row).Cells(18).Value = splitLine(11).ToString
                    Me.DataGridView2.Rows(row).Cells(19).Value = splitLine(12).ToString
                    Me.DataGridView2.Rows(row).Cells(20).Value = splitLine(13).ToString
                    Me.DataGridView2.Rows(row).Cells(21).Value = splitLine(14).ToString
                    'If curr <> splitLine(15).ToString Then
                    '    MsgBox("Invalid File", MsgBoxStyle.OkOnly, "File Upload")
                    '    Me.DataGridView2.Rows.Clear()
                    '    Me.btnCancel.PerformClick()
                    '    Exit Sub
                    'End If
                    Me.DataGridView2.Rows(row).Cells(22).Value = splitLine(15).ToString
                    Me.DataGridView2.Rows(row).Cells(23).Value = splitLine(16).ToString
                    Dim currency As String = splitLine(15).ToString
                    Dim amount As Decimal = Convert.ToDecimal(splitLine(16).ToString)

                    rest2 = resp2.ChargesPerLine(accId, amount, currency)
                    Dim charge As Double = rest2.Charges
                    Me.DataGridView2.Rows(row).Cells(24).Value = charge

                    '**********************cani new code***********************
                    If currency = "PHP" Then
                        PHPcounter = PHPcounter + 1
                        totalprincipalPHP = totalprincipalPHP + amount
                        totalphpcharge = totalphpcharge + charge

                    ElseIf currency = "USD" Then
                        USDcounter = USDcounter + 1
                        totalusdcharge = totalusdcharge + charge
                        totalprincipalUSD = totalprincipalUSD + amount

                    End If

                    row += 1

                    'If cbo_curr <> splitLine(15).ToString() Then
                    '    MsgBox("Invalid Currency.", MsgBoxStyle.OkOnly, "File Upload")
                    '    Me.DataGridView2.Rows.Clear()
                    '    Me.btnCancel.PerformClick()
                    '    clearGrid()
                    '    Exit Sub
                    'End If

                End While
                'remove datagrid's last row
                Me.DataGridView2.AllowUserToAddRows = False
                bntUpload.Enabled = True
                'Me.txttotalcount.Text = Me.DataGridView2.Rows.Count
                'Dim principal As Double
                'Dim total As Double
                'For i = 0 To Me.DataGridView2.Rows.Count - 1
                '    Try
                '        If CDbl(Me.DataGridView2.Rows(i).Cells(23).Value) Then
                '            principal = CDbl(Me.DataGridView2.Rows(i).Cells(23).Value)
                '        End If
                '    Catch ex As Exception
                '        Me.DataGridView2.Rows(row).Cells(23).Value = "#,###,##0.00"
                '    Finally
                '        total = total + CDbl(principal)
                '    End Try
                'Next

                '**********************cani new code 3/13/2013********************
                Me.txttotalcount.Text = PHPcounter
                Me.txtcountUSD.Text = USDcounter

                Dim totalAmountDuePHP As Decimal = totalprincipalPHP + totalphpcharge
                Dim totalAmountDueUSD As Decimal = totalprincipalUSD + totalusdcharge

                If totalphpcharge.ToString.Length > 3 Then
                    Me.txtTotalCharges.Text = Format(totalphpcharge, "#,###,##0.00")
                ElseIf totalphpcharge.ToString.Length > 6 Then
                    Me.txtTotalCharges.Text = Format(totalphpcharge, "#,###,##0.00")
                Else
                    Me.txtTotalCharges.Text = Format(totalphpcharge, "#,###,##0.00")
                End If

                If totalusdcharge.ToString.Length > 3 Then
                    Me.txtChargeUSD.Text = Format(totalusdcharge, "#,###,##0.00")
                ElseIf totalusdcharge.ToString.Length > 6 Then
                    Me.txtChargeUSD.Text = Format(totalusdcharge, "#,###,##0.00")
                Else
                    Me.txtChargeUSD.Text = Format(totalusdcharge, "#,###,##0.00")
                End If

                If totalprincipalPHP.ToString.Length > 3 Then
                    Me.txtAmountDue.Text = Format(totalAmountDuePHP, "#,###,##0.00")
                    Me.txtTotalPrincipal.Text = Format(totalprincipalPHP, "#,###,##0.00")
                ElseIf totalprincipalPHP.ToString.Length > 6 Then
                    Me.txtAmountDue.Text = Format(totalAmountDuePHP, "#,###,##0.00")
                    Me.txtTotalPrincipal.Text = Format(totalprincipalPHP, "#,###,##0.00")
                Else
                    Me.txtAmountDue.Text = Format(totalAmountDuePHP, "#,###,##0.00")
                    Me.txtTotalPrincipal.Text = Format(totalprincipalPHP, "#,###,##0.00")
                End If


                If totalprincipalUSD.ToString.Length > 3 Then
                    Me.totamountUSD.Text = Format(totalAmountDueUSD, "#,###,##0.00")
                    Me.txtPrincipalUSD.Text = Format(totalprincipalUSD, "#,###,##0.00")
                ElseIf totalprincipalUSD.ToString.Length > 6 Then
                    Me.totamountUSD.Text = Format(totalAmountDueUSD, "#,###,##0.00")
                    Me.txtPrincipalUSD.Text = Format(totalprincipalUSD, "#,###,##0.00")
                Else
                    Me.totamountUSD.Text = Format(totalAmountDueUSD, "#,###,##0.00")
                    Me.txtPrincipalUSD.Text = Format(totalprincipalUSD, "#,###,##0.00")
                End If

                '*****************************************************************

                '****************kentoy code******************
                'Me.txtTotalPrincipal.Text = Format(total, "#,###,##0.00")
                'Me.txtAmountDue.Text = Format(total, "#,###,##0.00")
                'Me.txtTotalCharges.Text = "0.00"
                'cboPartners.Enabled = False
                btnBrowse.Enabled = False
            Catch ex As Exception
                MsgBox("Invalid File for BPI", MsgBoxStyle.Critical)
                Me.DataGrid.BringToFront()
                Me.DataGrid.Visible = True
                'Me.DataGridView1.Rows.Clear()
                Me.DataGridView2.Rows.Clear()
                Me.DataGrid.Rows.Clear()
                Me.cboPartners.Text = Nothing
                Me.txtFileName.Text = ""
                Me.txttotalcount.Text = ""
                Me.lblDatetime.Text = ""
                Me.txtTotalPrincipal.Text = ""
                Me.txtBatchNo.Text = ""
                btnBrowse.Enabled = True
                Me.txtAmountDue.Text = ""
                Me.txtTotalCharges.Text = ""
                Me.txtConctactNum.Text = ""
                Me.txtAddress.Text = ""
                Me.totamountUSD.Text = ""
                Me.txtPrincipalUSD.Text = ""
                Me.txtChargeUSD.Text = ""
                Me.txtcountUSD.Text = ""
                cboPartners.Enabled = True
                Me.cboPartners.Select()
                Me.DataGrid.AllowUserToAddRows = False
                'cboPartners.Items.Clear()
                clearGrid()
            End Try
        Else
            MsgBox("Invalid File for BPI")
            Me.bntUpload.Enabled = False
            Me.cboPartners.Text = Nothing
            Me.DataGrid.Rows.Clear()
            Me.cboPartners.Enabled = True
            Me.btnBrowse.Enabled = True
            Me.cboPartners.Text = ""
            Me.txtFileName.Text = ""
            Me.txtFileName.Text = ""
            Me.txtBatchNo.Text = ""
            Me.DataGridView1.Rows.Clear()
            Me.txttotalcount.Text = ""
            Me.lblDatetime.Text = ""
            Me.txtTotalPrincipal.Text = ""
            Me.txtAmountDue.Text = ""
            Me.txtTotalCharges.Text = ""
            Me.txtConctactNum.Text = ""
            Me.txtAddress.Text = ""
            Me.totamountUSD.Text = ""
            Me.txtPrincipalUSD.Text = ""
            Me.txtChargeUSD.Text = ""
            Me.txtcountUSD.Text = ""
            Me.OpenFileDialog1.FilterIndex = 0
            Me.DataGridView2.Rows.Clear()
            Me.DataGridView3.Rows.Clear()
            Me.DataGridView4.Rows.Clear()
            Me.DataGridView5.Rows.Clear()
        End If

    End Sub
    Public Sub parseDataDBP()
        Dim response As New KPServices.MLhuillierSoapClient("MLhuillierSoap2", plg.getkpservicesendpoint)
        Dim ret As New KPServices.kptnResponse

        Dim serv3 As New PluginLogic
        Dim resp2 As New FileUploadService.MLhuillierSoapClient("MLhuillierSoap8", serv3.getAdminPartnersFileUploadServiceendpoint())
        Dim rest2 As New FileUploadService.ChargeValue()

        Dim appPath As String = OpenFileDialog1.FileName
        If appPath = Nothing And appPath = "" Then
            Me.DataGrid.BringToFront()
            Me.DataGrid.Visible = True
            'Me.DataGridView1.Rows.Clear()
            Me.DataGridView2.Rows.Clear()
            Me.DataGrid.Rows.Clear()
            Me.cboPartners.Text = Nothing
            Me.txtFileName.Text = ""
            Me.txttotalcount.Text = ""
            Me.lblDatetime.Text = ""
            Me.txtTotalPrincipal.Text = ""
            Me.txtBatchNo.Text = ""
            btnBrowse.Enabled = True
            Me.txtAmountDue.Text = ""
            Me.txtTotalCharges.Text = ""
            Me.txtConctactNum.Text = ""
            Me.txtAddress.Text = ""
            Me.totamountUSD.Text = ""
            Me.txtPrincipalUSD.Text = ""
            Me.txtChargeUSD.Text = ""
            Me.txtcountUSD.Text = ""
            cboPartners.Enabled = True
            Me.cboPartners.Select()
            Me.DataGrid.AllowUserToAddRows = False
            'cboPartners.Items.Clear()
            clearGrid()
            Exit Sub
        End If
        'Dim Fname As String = Path.GetFileName(appPath).Substring(3, 4)
        Dim Fname As String = "FSD7"
        Dim fileEntries As New List(Of String)
      
        If Not File.Exists(appPath) Then
            Exit Sub
        End If
        If oldAcc = Fname Then
            Try
                Dim SR As StreamReader = New StreamReader(appPath)
                fileEntries.Clear()
                Dim row As Integer = 0
                Dim rowvalue As String
                Dim PHPcounter As Integer = 0
                Dim USDcounter As Integer = 0
                Dim totalprincipalPHP As Double = 0
                Dim totalprincipalUSD As Double = 0
                Dim totalphpcharge As Double = 0
                Dim totalusdcharge As Double = 0
                Dim splitLine() As String
                'Dim sl2() As String

                While SR.Peek <> -1
                    Me.DataGridView3.Rows.Add()
                    rowvalue = SR.ReadLine()
                    splitLine = Split(rowvalue, "|"c)
                    Dim strlen As Integer = splitLine.Length()
                    If strlen = 14 Then
                        Me.DataGridView3.Rows(row).Cells(0).Value = splitLine(0).ToString
                        ret = response.getKptn("boswebserviceusr", "boyursa805", branchcode, zonecode, vfile, stationcode)
                        Me.DataGridView3.Rows(row).Cells(1).Value = ret.kptn
                        Me.DataGridView3.Rows(row).Cells(2).Value = splitLine(1).ToString
                        Me.DataGridView3.Rows(row).Cells(3).Value = splitLine(2).ToString
                        Me.DataGridView3.Rows(row).Cells(4).Value = splitLine(3).ToString
                        Me.DataGridView3.Rows(row).Cells(5).Value = splitLine(4).ToString
                        Me.DataGridView3.Rows(row).Cells(6).Value = splitLine(5).ToString
                        Me.DataGridView3.Rows(row).Cells(7).Value = splitLine(6).ToString
                        Me.DataGridView3.Rows(row).Cells(8).Value = splitLine(7).ToString
                        Me.DataGridView3.Rows(row).Cells(9).Value = splitLine(8).ToString
                        Me.DataGridView3.Rows(row).Cells(10).Value = splitLine(9).ToString
                        Me.DataGridView3.Rows(row).Cells(11).Value = splitLine(10).ToString
                        Me.DataGridView3.Rows(row).Cells(12).Value = splitLine(11).ToString
                        Me.DataGridView3.Rows(row).Cells(13).Value = splitLine(12).ToString
                        Dim amount As Decimal = Convert.ToDecimal(splitLine(9).ToString)

                        rest2 = resp2.ChargesPerLine(accId, amount, "PHP")
                        Dim charge As Decimal = rest2.Charges
                        Me.DataGridView3.Rows(row).Cells(15).Value = charge
                        Dim currency = "PHP"

                        '**********************cani new code***********************
                        If currency = "PHP" Then
                            PHPcounter = PHPcounter + 1
                            totalprincipalPHP = totalprincipalPHP + amount
                            totalphpcharge = totalphpcharge + charge

                        ElseIf currency = "USD" Then
                            USDcounter = USDcounter + 1
                            totalusdcharge = totalusdcharge + charge
                            totalprincipalUSD = totalprincipalUSD + amount

                        End If

                        row += 1
                    Else
                        MsgBox("Invalid File for DBP", MsgBoxStyle.Critical)
                        Me.DataGrid.BringToFront()
                        Me.DataGrid.Visible = True
                        Me.DataGrid.AllowUserToAddRows = False
                        'Me.DataGridView1.Rows.Clear()
                        Me.DataGridView4.Rows.Clear()
                        Me.DataGridView3.Rows.Clear()
                        Me.DataGridView2.Rows.Clear()
                        Me.DataGrid.Rows.Clear()
                        Me.cboPartners.Text = Nothing
                        Me.txtFileName.Text = ""
                        Me.txttotalcount.Text = ""
                        Me.lblDatetime.Text = ""
                        Me.txtTotalPrincipal.Text = ""
                        Me.txtBatchNo.Text = ""
                        btnBrowse.Enabled = True
                        Me.txtAmountDue.Text = ""
                        Me.txtTotalCharges.Text = ""
                        Me.txtConctactNum.Text = ""
                        Me.txtAddress.Text = ""
                        Me.totamountUSD.Text = ""
                        Me.txtPrincipalUSD.Text = ""
                        Me.txtChargeUSD.Text = ""
                        Me.txtcountUSD.Text = ""
                        cboPartners.Enabled = True
                        Me.cboPartners.Select()
                        Me.DataGrid.AllowUserToAddRows = False
                        'cboPartners.Items.Clear()
                        clearGrid()
                        Exit Sub
                    End If
                End While
                'remove datagrid's last row
                Me.DataGridView3.AllowUserToAddRows = False
                bntUpload.Enabled = True

                '**********************cani new code 3/13/2013********************
                Me.txttotalcount.Text = PHPcounter
                Me.txtcountUSD.Text = USDcounter

                Dim totalAmountDuePHP As Decimal = totalprincipalPHP + totalphpcharge
                Dim totalAmountDueUSD As Decimal = totalprincipalUSD + totalusdcharge

                If totalphpcharge.ToString.Length > 3 Then
                    Me.txtTotalCharges.Text = Format(totalphpcharge, "#,###,##0.00")
                ElseIf totalphpcharge.ToString.Length > 6 Then
                    Me.txtTotalCharges.Text = Format(totalphpcharge, "#,###,##0.00")
                Else
                    Me.txtTotalCharges.Text = Format(totalphpcharge, "#,###,##0.00")
                End If

                If totalusdcharge.ToString.Length > 3 Then
                    Me.txtChargeUSD.Text = Format(totalusdcharge, "#,###,##0.00")
                ElseIf totalusdcharge.ToString.Length > 6 Then
                    Me.txtChargeUSD.Text = Format(totalusdcharge, "#,###,##0.00")
                Else
                    Me.txtChargeUSD.Text = Format(totalusdcharge, "#,###,##0.00")
                End If

                If totalprincipalPHP.ToString.Length > 3 Then
                    Me.txtAmountDue.Text = Format(totalAmountDuePHP, "#,###,##0.00")
                    Me.txtTotalPrincipal.Text = Format(totalprincipalPHP, "#,###,##0.00")
                ElseIf totalprincipalPHP.ToString.Length > 6 Then
                    Me.txtAmountDue.Text = Format(totalAmountDuePHP, "#,###,##0.00")
                    Me.txtTotalPrincipal.Text = Format(totalprincipalPHP, "#,###,##0.00")
                Else
                    Me.txtAmountDue.Text = Format(totalAmountDuePHP, "#,###,##0.00")
                    Me.txtTotalPrincipal.Text = Format(totalprincipalPHP, "#,###,##0.00")
                End If


                If totalprincipalUSD.ToString.Length > 3 Then
                    Me.totamountUSD.Text = Format(totalAmountDueUSD, "#,###,##0.00")
                    Me.txtPrincipalUSD.Text = Format(totalprincipalUSD, "#,###,##0.00")
                ElseIf totalprincipalUSD.ToString.Length > 6 Then
                    Me.totamountUSD.Text = Format(totalAmountDueUSD, "#,###,##0.00")
                    Me.txtPrincipalUSD.Text = Format(totalprincipalUSD, "#,###,##0.00")
                Else
                    Me.totamountUSD.Text = Format(totalAmountDueUSD, "#,###,##0.00")
                    Me.txtPrincipalUSD.Text = Format(totalprincipalUSD, "#,###,##0.00")
                End If

                'Me.txttotalcount.Text = Me.DataGridView3.Rows.Count
                'For i = 0 To Me.DataGridView3.Rows.Count - 1
                '    Dim totamt As Double
                '    totamt = totamt + CDbl(Me.DataGridView3.Rows(i).Cells(10).Value)
                '    Me.txtTotalPrincipal.Text = Format(totamt, "#,###,##0.00")
                '    Me.txtAmountDue.Text = Format(totamt, "#,###,##0.00")
                'Next
                'Me.txtTotalCharges.Text = Format(0.0, "#,###,##0.00")
                cboPartners.Enabled = False
                btnBrowse.Enabled = False

            Catch ex As Exception
                MsgBox("Invalid File for DBP", MsgBoxStyle.Critical)
                Me.DataGrid.BringToFront()
                Me.DataGrid.Visible = True
                Me.DataGrid.AllowUserToAddRows = False
                'Me.DataGridView1.Rows.Clear()
                Me.DataGridView4.Rows.Clear()
                Me.DataGridView3.Rows.Clear()
                Me.DataGridView2.Rows.Clear()
                Me.DataGrid.Rows.Clear()
                Me.cboPartners.Text = Nothing
                Me.txtFileName.Text = ""
                Me.txttotalcount.Text = ""
                Me.lblDatetime.Text = ""
                Me.txtTotalPrincipal.Text = ""
                Me.txtBatchNo.Text = ""
                btnBrowse.Enabled = True
                Me.txtAmountDue.Text = ""
                Me.txtTotalCharges.Text = ""
                Me.txtConctactNum.Text = ""
                Me.txtAddress.Text = ""
                Me.totamountUSD.Text = ""
                Me.txtPrincipalUSD.Text = ""
                Me.txtChargeUSD.Text = ""
                Me.txtcountUSD.Text = ""
                cboPartners.Enabled = True
                Me.cboPartners.Select()
                Me.DataGrid.AllowUserToAddRows = False
                'cboPartners.Items.Clear()
                clearGrid()
            End Try
        Else
            MsgBox("Invalid File for DBP")
            Me.bntUpload.Enabled = False
            Me.cboPartners.Text = Nothing
            Me.DataGrid.Rows.Clear()
            Me.cboPartners.Enabled = True
            Me.btnBrowse.Enabled = True
            Me.cboPartners.Text = ""
            Me.txtFileName.Text = ""
            Me.txtFileName.Text = ""
            Me.txtBatchNo.Text = ""
            Me.DataGridView1.Rows.Clear()
            Me.txttotalcount.Text = ""
            Me.lblDatetime.Text = ""
            Me.txtTotalPrincipal.Text = ""
            Me.txtAmountDue.Text = ""
            Me.txtTotalCharges.Text = ""
            Me.txtConctactNum.Text = ""
            Me.txtAddress.Text = ""
            Me.totamountUSD.Text = ""
            Me.txtPrincipalUSD.Text = ""
            Me.txtChargeUSD.Text = ""
            Me.txtcountUSD.Text = ""
            Me.OpenFileDialog1.FilterIndex = 0
            Me.DataGridView2.Rows.Clear()
            Me.DataGridView3.Rows.Clear()
            Me.DataGridView4.Rows.Clear()
            Me.DataGridView5.Rows.Clear()
        End If

    End Sub
    Public Sub parseDataBDO()
        Dim response As New KPServices.MLhuillierSoapClient("MLhuillierSoap2", plg.getkpservicesendpoint)
        Dim ret As New KPServices.kptnResponse

        Dim serv3 As New PluginLogic
        Dim resp2 As New FileUploadService.MLhuillierSoapClient("MLhuillierSoap8", serv3.getAdminPartnersFileUploadServiceendpoint())
        Dim rest2 As New FileUploadService.ChargeValue()

        Dim appPath As String = OpenFileDialog1.FileName
        If appPath = Nothing Or appPath = "" Then
            Me.DataGrid.BringToFront()
            Me.DataGrid.Visible = True
            'Me.DataGridView1.Rows.Clear()
            Me.DataGridView2.Rows.Clear()
            Me.DataGrid.Rows.Clear()
            Me.cboPartners.Text = Nothing
            Me.txtFileName.Text = ""
            Me.txttotalcount.Text = ""
            Me.lblDatetime.Text = ""
            Me.txtTotalPrincipal.Text = ""
            Me.txtBatchNo.Text = ""
            btnBrowse.Enabled = True
            Me.txtAmountDue.Text = ""
            Me.txtTotalCharges.Text = ""
            Me.txtConctactNum.Text = ""
            Me.txtAddress.Text = ""
            Me.totamountUSD.Text = ""
            Me.txtPrincipalUSD.Text = ""
            Me.txtChargeUSD.Text = ""
            Me.txtcountUSD.Text = ""
            cboPartners.Enabled = True
            Me.cboPartners.Select()
            Me.DataGrid.AllowUserToAddRows = False
            'cboPartners.Items.Clear()
            clearGrid()
            Exit Sub
        End If
        'Dim Fname As String = Path.GetFileName(appPath).Substring(0, 3)
        Dim Fname As String = "FSD35"
        Dim fileEntries As New List(Of String)


            If Not File.Exists(appPath) Then
                Exit Sub
            End If
        If oldAcc = Fname Then
            Try
                Dim SR As StreamReader = New StreamReader(appPath)
                fileEntries.Clear()
                Dim row As Integer = 0
                Dim rowvalue As String
                Dim splitLine() As String
                Dim PHPcounter As Integer = 0
                Dim USDcounter As Integer = 0
                Dim totalprincipalPHP As Double = 0
                Dim totalprincipalUSD As Double = 0
                Dim totalphpcharge As Double = 0
                Dim totalusdcharge As Double = 0

                'Dim sl2() As String

                While SR.Peek <> -1
                    Me.DataGridView4.Rows.Add()
                    rowvalue = SR.ReadLine()
                    splitLine = Split(rowvalue, "|"c)
                    Dim strlen As Integer = splitLine.Length()
                    If strlen = 21 Then
                        Me.DataGridView4.Rows(row).Cells(0).Value = splitLine(0).ToString 'Reference Number
                        ret = response.getKptn("boswebserviceusr", "boyursa805", branchcode, zonecode, vfile, stationcode)
                        Me.DataGridView4.Rows(row).Cells(1).Value = ret.kptn
                        Me.DataGridView4.Rows(row).Cells(2).Value = splitLine(1).ToString 'Transaction Type
                        Me.DataGridView4.Rows(row).Cells(3).Value = splitLine(2).ToString 'Bene last name
                        Me.DataGridView4.Rows(row).Cells(4).Value = splitLine(3).ToString 'Bene first name
                        Me.DataGridView4.Rows(row).Cells(5).Value = splitLine(4).ToString 'Bene Middle name
                        Me.DataGridView4.Rows(row).Cells(6).Value = splitLine(5).ToString 'Remitter Fullname
                        Me.DataGridView4.Rows(row).Cells(7).Value = splitLine(6).ToString 'Product Name
                        Dim amount As Decimal = Convert.ToDecimal(splitLine(7).ToString)
                        Dim currency As String = splitLine(8).ToString
                        rest2 = resp2.ChargesPerLine(accId, amount, currency)
                        Dim charge As Decimal = rest2.Charges
                        Me.DataGridView4.Rows(row).Cells(8).Value = amount
                        Me.DataGridView4.Rows(row).Cells(9).Value = currency
                        Me.DataGridView4.Rows(row).Cells(10).Value = splitLine(9).ToString 'bank branch
                        Me.DataGridView4.Rows(row).Cells(11).Value = splitLine(10).ToString 'transaction Date
                        Me.DataGridView4.Rows(row).Cells(12).Value = splitLine(11).ToString 'Instruction 1
                        Me.DataGridView4.Rows(row).Cells(13).Value = splitLine(12).ToString 'Instruction 2
                        Me.DataGridView4.Rows(row).Cells(14).Value = splitLine(13).ToString 'Reserved Field
                        Me.DataGridView4.Rows(row).Cells(15).Value = splitLine(14).ToString 'Reserved Field 2
                        Me.DataGridView4.Rows(row).Cells(16).Value = splitLine(15).ToString 'Reserved Field 3
                        Me.DataGridView4.Rows(row).Cells(17).Value = splitLine(16).ToString 'Reserved Field 4
                        Me.DataGridView4.Rows(row).Cells(18).Value = splitLine(17).ToString 'Record Count
                        Me.DataGridView4.Rows(row).Cells(19).Value = splitLine(18).ToString 'Total Amount
                        Me.DataGridView4.Rows(row).Cells(20).Value = charge
                        Me.DataGridView4.Rows(row).Cells(21).Value = splitLine(19).ToString 'Locator code


                        '******************** cani new code ******************
                        If currency = "PHP" Then
                            PHPcounter = PHPcounter + 1
                            totalprincipalPHP = totalprincipalPHP + amount
                            totalphpcharge = totalphpcharge + charge

                        ElseIf currency = "USD" Then
                            USDcounter = USDcounter + 1
                            totalusdcharge = totalusdcharge + charge
                            totalprincipalUSD = totalprincipalUSD + amount

                        End If





                        row += 1
                    Else

                        'If cbo_curr <> splitLine(5).ToString() Then
                        '    MsgBox("Invalid Currency.", MsgBoxStyle.OkOnly, "File Upload")
                        '    Me.DataGridView4.Rows.Clear()
                        '    Me.btnCancel.PerformClick()
                        '    clearGrid()
                        '    Exit Sub
                        MsgBox("Invalid File for BDO", MsgBoxStyle.Critical)
                        Me.DataGrid.BringToFront()
                        Me.DataGrid.Visible = True
                        'Me.DataGridView1.Rows.Clear()
                        Me.DataGridView4.Rows.Clear()
                        Me.DataGridView3.Rows.Clear()
                        Me.DataGridView2.Rows.Clear()
                        Me.DataGrid.Rows.Clear()
                        Me.cboPartners.Text = Nothing
                        Me.txtFileName.Text = ""
                        Me.txttotalcount.Text = ""
                        Me.lblDatetime.Text = ""
                        Me.txtTotalPrincipal.Text = ""
                        Me.txtBatchNo.Text = ""
                        btnBrowse.Enabled = True
                        Me.txtAmountDue.Text = ""
                        Me.txtTotalCharges.Text = ""
                        Me.txtConctactNum.Text = ""
                        Me.txtAddress.Text = ""
                        Me.totamountUSD.Text = ""
                        Me.txtPrincipalUSD.Text = ""
                        Me.txtChargeUSD.Text = ""
                        Me.txtcountUSD.Text = ""
                        cboPartners.Enabled = True
                        Me.cboPartners.Select()
                        Me.DataGrid.AllowUserToAddRows = False
                        'cboPartners.Items.Clear()
                        clearGrid()
                        Exit Sub
                    End If


                End While
                'remove datagrid's last row
                Me.DataGridView4.AllowUserToAddRows = False
                bntUpload.Enabled = True
                Me.txttotalcount.Text = Me.DataGridView4.Rows.Count
                Dim totamt As Double
                Dim iamt As Double
                For i = 0 To Me.DataGridView4.Rows.Count - 1
                    Try
                        If CDbl(Me.DataGridView4.Rows(i).Cells(5).Value.ToString) Then
                            iamt = CDbl(Me.DataGridView4.Rows(i).Cells(5).Value.ToString)
                            totamt = totamt + iamt

                        End If
                    Catch ex As Exception
                        'MsgBox(ex.ToString)
                        iamt = Format(CDbl("0.00"), "#,###,##0.00")
                        totamt = totamt + iamt
                    End Try
                Next


                '**********************cani new code 3/13/2013********************
                Me.txttotalcount.Text = PHPcounter
                Me.txtcountUSD.Text = USDcounter

                Dim totalAmountDuePHP As Decimal = totalprincipalPHP + totalphpcharge
                Dim totalAmountDueUSD As Decimal = totalprincipalUSD + totalusdcharge

                If totalphpcharge.ToString.Length > 3 Then
                    Me.txtTotalCharges.Text = Format(totalphpcharge, "#,###,##0.00")
                ElseIf totalphpcharge.ToString.Length > 6 Then
                    Me.txtTotalCharges.Text = Format(totalphpcharge, "#,###,##0.00")
                Else
                    Me.txtTotalCharges.Text = Format(totalphpcharge, "#,###,##0.00")
                End If

                If totalusdcharge.ToString.Length > 3 Then
                    Me.txtChargeUSD.Text = Format(totalusdcharge, "#,###,##0.00")
                ElseIf totalusdcharge.ToString.Length > 6 Then
                    Me.txtChargeUSD.Text = Format(totalusdcharge, "#,###,##0.00")
                Else
                    Me.txtChargeUSD.Text = Format(totalusdcharge, "#,###,##0.00")
                End If

                If totalprincipalPHP.ToString.Length > 3 Then
                    Me.txtAmountDue.Text = Format(totalAmountDuePHP, "#,###,##0.00")
                    Me.txtTotalPrincipal.Text = Format(totalprincipalPHP, "#,###,##0.00")
                ElseIf totalprincipalPHP.ToString.Length > 6 Then
                    Me.txtAmountDue.Text = Format(totalAmountDuePHP, "#,###,##0.00")
                    Me.txtTotalPrincipal.Text = Format(totalprincipalPHP, "#,###,##0.00")
                Else
                    Me.txtAmountDue.Text = Format(totalAmountDuePHP, "#,###,##0.00")
                    Me.txtTotalPrincipal.Text = Format(totalprincipalPHP, "#,###,##0.00")
                End If


                If totalprincipalUSD.ToString.Length > 3 Then
                    Me.totamountUSD.Text = Format(totalAmountDueUSD, "#,###,##0.00")
                    Me.txtPrincipalUSD.Text = Format(totalprincipalUSD, "#,###,##0.00")
                ElseIf totalprincipalUSD.ToString.Length > 6 Then
                    Me.totamountUSD.Text = Format(totalAmountDueUSD, "#,###,##0.00")
                    Me.txtPrincipalUSD.Text = Format(totalprincipalUSD, "#,###,##0.00")
                Else
                    Me.totamountUSD.Text = Format(totalAmountDueUSD, "#,###,##0.00")
                    Me.txtPrincipalUSD.Text = Format(totalprincipalUSD, "#,###,##0.00")
                End If

                '*****************************************************************
                '*******************************************************************

                '********************kentoy code******************************
                'Me.txtTotalPrincipal.Text = Format(totamt, "#,###,##0.00")
                'Me.txtAmountDue.Text = Format(totamt, "#,###,##0.00")
                'Me.txtTotalCharges.Text = Format(0.0, "#,###,##0.00")
                cboPartners.Enabled = False
                btnBrowse.Enabled = False
            Catch ex As Exception
                MsgBox("Invalid File for BDO", MsgBoxStyle.Critical)
                Me.DataGrid.BringToFront()
                Me.DataGrid.Visible = True
                'Me.DataGridView1.Rows.Clear()
                Me.DataGridView4.Rows.Clear()
                Me.DataGridView3.Rows.Clear()
                Me.DataGridView2.Rows.Clear()
                Me.DataGrid.Rows.Clear()
                Me.cboPartners.Text = Nothing
                Me.txtFileName.Text = ""
                Me.txttotalcount.Text = ""
                Me.lblDatetime.Text = ""
                Me.txtTotalPrincipal.Text = ""
                Me.txtBatchNo.Text = ""
                btnBrowse.Enabled = True
                Me.txtAmountDue.Text = ""
                Me.txtTotalCharges.Text = ""
                Me.txtConctactNum.Text = ""
                Me.txtAddress.Text = ""
                Me.totamountUSD.Text = ""
                Me.txtPrincipalUSD.Text = ""
                Me.txtChargeUSD.Text = ""
                Me.txtcountUSD.Text = ""
                cboPartners.Enabled = True
                Me.cboPartners.Select()
                Me.DataGrid.AllowUserToAddRows = False
                'cboPartners.Items.Clear()
                clearGrid()
            End Try
        Else
            MsgBox("Invalid File for BDO")
            Me.DataGrid.BringToFront()
            Me.DataGrid.Visible = True
            Me.DataGridView1.Rows.Clear()
            Me.DataGridView4.Rows.Clear()
            Me.DataGridView3.Rows.Clear()
            Me.DataGridView2.Rows.Clear()
            Me.DataGridView5.Rows.Clear()
            Me.DataGrid.Rows.Clear()
            Me.cboPartners.Text = Nothing
            Me.txtFileName.Text = ""
            Me.txttotalcount.Text = ""
            Me.lblDatetime.Text = ""
            Me.txtTotalPrincipal.Text = ""
            Me.txtBatchNo.Text = ""
            btnBrowse.Enabled = True
            Me.txtAmountDue.Text = ""
            Me.txtTotalCharges.Text = ""
            Me.txtConctactNum.Text = ""
            Me.txtAddress.Text = ""
            Me.totamountUSD.Text = ""
            Me.txtPrincipalUSD.Text = ""
            Me.txtChargeUSD.Text = ""
            Me.txtcountUSD.Text = ""
            cboPartners.Enabled = True
            Me.cboPartners.Select()
            Me.DataGrid.AllowUserToAddRows = False
            'cboPartners.Items.Clear()
            'clearGrid()
        End If

    End Sub
    Private Function ReadLine(ByVal line As String, ByVal isheader As Boolean, ByVal linecounter As Integer) As Boolean
        ReadLine = True
        If Not isheader Then
            'for details

            If line.Length <> 581 Then
                MsgBox("Error in line " & linecounter & vbNewLine & "Invalid number of characters.")
                ReadLine = False
            Else
                Try
                    Dim l As String = ""
                    Dim itemno As String = line.Substring(l.Length, 5)
                    AddToLine(l, itemno)
                    Dim kptn As String = line.Substring(l.Length, 15)
                    AddToLine(l, kptn)
                    Dim dtfiled As String = line.Substring(l.Length, 19)
                    AddToLine(l, dtfiled)
                    Dim controlno As String = line.Substring(l.Length, 20)
                    AddToLine(l, controlno)
                    Dim principal As Double = line.Substring(l.Length, 12)
                    AddToLine(l, line.Substring(l.Length, 12))
                    Dim curid As String = line.Substring(l.Length, 3)
                    AddToLine(l, curid)
                    Dim charge As String = line.Substring(l.Length, 10)
                    AddToLine(l, charge)
                    Dim sourceoffund As String = line.Substring(l.Length, 15)
                    AddToLine(l, sourceoffund)
                    Dim relationtoreceiver As String = line.Substring(l.Length, 20)
                    AddToLine(l, relationtoreceiver)
                    Dim purpose As String = line.Substring(l.Length, 30)
                    AddToLine(l, purpose)
                    Dim senderlname As String = line.Substring(l.Length, 20)
                    AddToLine(l, senderlname)
                    Dim senderfname As String = line.Substring(l.Length, 20)
                    AddToLine(l, senderfname)
                    Dim sendermname As String = line.Substring(l.Length, 20)
                    AddToLine(l, sendermname)
                    Dim senderidtype As String = line.Substring(l.Length, 10)
                    AddToLine(l, senderidtype)
                    Dim senderidno As String = line.Substring(l.Length, 20)
                    AddToLine(l, senderidno)
                    Dim senderexpiry As String = line.Substring(l.Length, 10)
                    AddToLine(l, senderexpiry)
                    Dim senderbdate As String = line.Substring(l.Length, 10)
                    AddToLine(l, senderbdate)
                    Dim sendergender As String = line.Substring(l.Length, 1)
                    AddToLine(l, sendergender)
                    Dim senderstreet As String = line.Substring(l.Length, 30)
                    AddToLine(l, senderstreet)
                    Dim senderprovince As String = line.Substring(l.Length, 25)
                    AddToLine(l, senderprovince)
                    Dim sendercountry As String = line.Substring(l.Length, 25)
                    AddToLine(l, sendercountry)
                    Dim senderphone As String = line.Substring(l.Length, 20)
                    AddToLine(l, senderphone)
                    Dim reclname As String = line.Substring(l.Length, 20)
                    AddToLine(l, reclname)
                    Dim recfname As String = line.Substring(l.Length, 20)
                    AddToLine(l, recfname)
                    Dim recmname As String = line.Substring(l.Length, 20)
                    AddToLine(l, recmname)
                    Dim recstreet As String = line.Substring(l.Length, 30)
                    AddToLine(l, recstreet)
                    Dim recprovince As String = line.Substring(l.Length, 25)
                    AddToLine(l, recprovince)
                    Dim reccountry As String = line.Substring(l.Length, 25)
                    AddToLine(l, reccountry)
                    Dim recbdate As String = line.Substring(l.Length, 10)
                    AddToLine(l, recbdate)
                    Dim recgender As String = line.Substring(l.Length, 1)
                    AddToLine(l, recgender)
                    Dim recphone As String = line.Substring(l.Length, 20)
                    AddToLine(l, recphone)
                    Dim msg As String = line.Substring(l.Length, 50)
                Catch ex As Exception
                    MsgBox("Error in line " & linecounter & vbNewLine & ex.Message)
                    ReadLine = False
                End Try
            End If
        Else
            'for header
            If line.Length <> 67 Then
                MsgBox("Error in line " & linecounter & vbNewLine & "Invalid number of characters.")
                ReadLine = False
            Else
                Try
                    Dim l As String = ""
                    Dim dtsent As String = line.Substring(l.Length, 10)
                    AddToLine(l, dtsent)
                    Dim branchid As String = line.Substring(l.Length, 6)
                    AddToLine(l, branchid)
                    Dim userid As String = line.Substring(l.Length, 20)
                    AddToLine(l, userid)
                    Dim terminalid As String = line.Substring(l.Length, 10)
                    AddToLine(l, terminalid)
                    Dim secretkey As String = line.Substring(l.Length, 10)
                    AddToLine(l, secretkey)
                    Dim totalcount As String = line.Substring(l.Length, 5)
                    AddToLine(l, totalcount)
                    Dim signature As String = line.Substring(l.Length, 6)
                    If Not IsDate(dtsent) Then
                        MsgBox("Error in line " & linecounter & vbNewLine & "Invalid date sent.")
                        ReadLine = False
                    End If
                Catch ex As Exception
                    MsgBox("Error in line " & linecounter & vbNewLine & ex.Message)
                    ReadLine = False
                End Try
            End If
        End If
    End Function
    Private Sub AddToLine(ByRef l As String, ByVal value As String)
        l = l & value
    End Sub
    Private Sub frmUpload_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        datalist.Clear()
        Me.DataGrid.BringToFront()
        Me.DataGrid.Visible = True
        Me.DataGridView4.Rows.Clear()
        Me.DataGridView1.Rows.Clear()
        Me.DataGrid.Rows.Clear()
        Me.cboPartners.Text = Nothing
        Me.txtFileName.Text = ""
        Me.txttotalcount.Text = ""
        Me.lblDatetime.Text = ""
        Me.txtTotalPrincipal.Text = ""
        Me.txtBatchNo.Text = ""
        btnBrowse.Enabled = True
        Me.txtAmountDue.Text = ""
        Me.txtTotalCharges.Text = ""
        Me.txtConctactNum.Text = ""
        Me.txtAddress.Text = ""
        cboPartners.Enabled = True

        Me.cboPartners.Select()
        Me.DataGrid.AllowUserToAddRows = False
        cboPartners.Items.Clear()

        If DataGrid.RowCount <= 1 Then
            bntUpload.Enabled = False
        Else
            Me.bntUpload.Enabled = True
        End If

        Dim serv3 As New PluginLogic
        Dim resp As New FileUploadService.MLhuillierSoapClient("MLhuillierSoap8", serv3.getAdminPartnersFileUploadServiceendpoint())
        Dim rest As New FileUploadService.SearchPartners()
        Dim AccountName As String

        Dim getresp1() As String
        rest = resp.ListofPartners()
        getresp1 = rest.ReturnData.ToArray
        Dim i As Integer = 0
        For Each dataItem As String In getresp1
            Dim split() As String = dataItem.Split("|")
            AccountName = split(0).ToString
            Me.cboPartners.Items.Add(AccountName.ToString)
        Next
        'Dim curr As String

        'Dim rest As New FileUploadService.SearchPartners()
        'rest = resp.ListofPartners()

        'Dim serv3 As New PluginLogic
        'Dim resp As New FileUploadService.MLhuillierSoapClient("MLhuillierSoap8", serv3.getAdminPartnersFileUploadServiceendpoint())
        'Dim rest As New FileUploadService.SearchCurrency()

        'Dim getresp1() As String
        'rest = resp.Currency
        'getresp1 = rest.ChooseCurrency.ToArray
        'Dim i As Integer = 0
        'For Each dataItem As String In getresp1
        '    curr = dataItem
        '    Me.cboCurrency.Items.Add(curr.ToString)
        'Next
    End Sub
    Private Sub cboCurrency_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        cboPartners.Items.Clear()

        getPartners(cbo_curr)
        cboPartners.Enabled = True
        btnBrowse.Enabled = True
    End Sub
    Private Sub getPartners(ByVal Currency As String)


        'Dim rest As New FileUploadService.SearchPartners()
        'rest = resp.ListofPartners()

        
    End Sub
    Private Sub cboPartners_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboPartners.SelectedIndexChanged
        Dim AccountName As String
        Dim AccountId As String
        Dim address As String
        Dim contanctnum As String
        Dim OldAccount As String

        Dim serv4 As New PluginLogic


        Dim resp As New FileUploadService.MLhuillierSoapClient("MLhuillierSoap8", serv4.getAdminPartnersFileUploadServiceendpoint())
        Dim rest As New FileUploadService.SearchPartners()
        'Dim response As New FileUpload.MLhuillier
        'Dim ret As New FileUpload.SearchPartners
        Dim getresponse1() As String
        rest = resp.ListofPartners()
        getresponse1 = rest.ReturnData.ToArray
        For Each dataItem As String In getresponse1
            Dim split() As String = dataItem.Split("|")
            AccountName = split(0).ToString
            AccountId = split(1).ToString
            address = split(2).ToString
            contanctnum = split(3).ToString
            'curr = split(4).ToString
            OldAccount = split(5).ToString
            If cboPartners.SelectedItem = AccountName Then
                oldAcc = OldAccount
                accId = AccountId
                accountidInt = AccountId
                Me.txtAddress.Text = address
                Me.txtConctactNum.Text = contanctnum
                Exit Sub
            End If
        Next

    End Sub
    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Me.bntUpload.Enabled = False
        Me.cboPartners.Text = Nothing

        Me.DataGrid.Rows.Clear()
        Me.cboPartners.Enabled = True
        Me.totamountUSD.Text = ""
        Me.txtPrincipalUSD.Text = ""
        Me.txtChargeUSD.Text = ""
        Me.txtcountUSD.Text = ""
        Me.btnBrowse.Enabled = True
        Me.cboPartners.Text = ""
        Me.txtFileName.Text = ""
        Me.txtFileName.Text = ""
        Me.txtBatchNo.Text = ""
        Me.DataGridView1.Rows.Clear()
        Me.txttotalcount.Text = ""
        Me.lblDatetime.Text = ""
        Me.txtTotalPrincipal.Text = ""
        Me.txtAmountDue.Text = ""
        Me.txtTotalCharges.Text = ""
        Me.txtConctactNum.Text = ""
        Me.txtAddress.Text = ""
        Me.OpenFileDialog1.FilterIndex = 0
        Me.DataGridView2.Rows.Clear()
        closeApplication()
        Me.DataGridView3.Rows.Clear()
        Me.DataGridView4.Rows.Clear()
        Me.DataGridView5.Rows.Clear()
        Me.DataGrid.AllowUserToAddRows = False
        Me.DataGridView1.AllowUserToAddRows = False
        Me.DataGridView2.AllowUserToAddRows = False
        Me.DataGridView4.AllowUserToAddRows = False
        Me.DataGridView5.AllowUserToAddRows = False
        datalist.Clear()
        Me.DataGrid.Visible = True
        'Me.DataGrid.BringToFront()
        'Me.DataGridView1.BringToFront()
        Me.DataGridView5.BringToFront()


    End Sub
    Private Sub txtBatchNo_TextChanged(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtBatchNo.KeyPress
        'Dim allowedChars As String = "-ABCDEFGHIJKLMNOPQRSTUVWXYZ" & "0123456789" & vbBack & "abcdefghijklmnopqrstuvwxyz"
        'If allowedChars.IndexOf(e.KeyChar) = -1 Then
        '    e.Handled = True
        'End If
        Me.txtBatchNo.MaxLength = 25
    End Sub

    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub
    Public Sub getRunningBalance(ByVal Accid As String, ByVal TotalAmount As Decimal, ByVal Currency As String)
        Dim serv3 As New PluginLogic
        Dim resp2 As New FileUploadService.MLhuillierSoapClient("MLhuillierSoap8", serv3.getAdminPartnersFileUploadServiceendpoint())
        Dim rest2 As New FileUploadService.UpdateNewRbalance()

        While rest2.result = 0
            rest2 = resp2.UpdateNewRunningBalance(Accid, TotalAmount, Currency)

        End While

       
    End Sub



    
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles submitBTN.Click
        submitBTN.Enabled = True
        Dim serv As New PluginLogic
        Dim resp As New FileUploadService.MLhuillierSoapClient("MLhuillierSoap8", serv.getAdminPartnersFileUploadServiceendpoint())

        Dim rest As New FileUploadService.Testing
        rest = resp.SubmitTempTable(PartnersNameIdentifier, partnersSession)

        If rest.respcode = 1 Then

            MsgBox(rest.msg)
            viewReport("TEXT", MLbatchNumber, oldAcc, operatorID, 1, Intresp)

        ElseIf rest.respcode = 0 Then

            MsgBox(rest.msg)

        End If



    End Sub

    Public Function FileUploadMethod(ByRef array As ML.Plugin.Business.FileUploadService.ArrayOfString, ByVal AccountId As String, ByVal Bnumber As String, ByVal bname As String, ByVal bcode As String, ByVal stationcode As String, ByVal zcode As String, ByVal OpId As String, ByVal stId As String, ByVal file As String, ByVal tAmount As Decimal, ByVal currency As String, ByVal charge As Decimal) As Integer
        Dim output As Integer

        Dim serv2 As New PluginLogic()
        Dim resp1 As New FileUploadService.MLhuillierSoapClient("MLhuillierSoap8", serv2.getAdminPartnersFileUploadServiceendpoint())
        Dim rest As New FileUploadService.UploadFile()
        rest = resp1.FileUpload(array, AccountId, Bnumber, bname, bcode, stationcode, zcode, OpId, stId, file, tAmount, currency, charge)

        If rest.response = 0 Then
            output = rest.response
            listResp = rest.array
            RespMsg = rest.msg
            MLbatchNumber = rest.BatchNumber
        ElseIf rest.response = 2 Then
            output = rest.response
            RespMsg = rest.msg
            
        ElseIf rest.response = 1 Then
            output = rest.response
            listResp = rest.array
            RespMsg = rest.msg
            partnersSession = rest.session
            MLbatchNumber = rest.BatchNumber
        End If

        Return output
    End Function
    Public Sub viewReport(ByVal typeFile As String, ByVal BatchNumber As String, ByVal oldAcc As String, ByVal operatorID As String, ByVal response As Integer, ByVal getresp As String)
        Dim view As New FrmReportError
        Dim f As New frmFileUploadPrint
        If response = 0 Then
            view.CollectData(listResp, typeFile, cboPartners.SelectedItem.ToString, BatchNumber, Me.txtBatchNo.Text, oldAcc, operatorID)
            view.Show()
            clearData()
        ElseIf response = 1 Then
            f.SetF(Me)
            If moex_curr Is Nothing Then
                moex_curr = ""
            End If
            Dim cmbPartner As String = Me.cboPartners.SelectedItem.ToString
            f.passdata(listResp, BatchNumber, Me.txtBatchNo.Text, cmbPartner, moex_curr, getresp, oldAcc, txtTotalPrincipal.Text, txtPrincipalUSD.Text, txtChargeUSD.Text, txtTotalCharges.Text, txttotalcount.Text, txtcountUSD.Text, operatorID)
            f.ShowDialog()
            clearData()
        End If
    End Sub

    Public Sub clearData()
        Me.DataGridView5.Rows.Clear()
        Me.DataGridView2.Rows.Clear()
        Me.DataGridView3.Rows.Clear()
        Me.DataGridView4.Rows.Clear()
        Me.DataGridView1.Rows.Clear()
        Me.cboPartners.Enabled = True
        Me.btnBrowse.Enabled = True
        Me.cboPartners.Text = ""
        Me.txtFileName.Text = ""
        Me.cboPartners.Text = Nothing
        Me.bntUpload.Enabled = False
        Me.txtFileName.Text = ""
        Me.txttotalcount.Text = ""
        Me.lblDatetime.Text = ""
        Me.txtTotalPrincipal.Text = ""
        Me.txtBatchNo.Text = ""
        Me.txtAmountDue.Text = ""
        Me.txtTotalCharges.Text = ""
        Me.txtConctactNum.Text = ""
        Me.txtAddress.Text = ""
        Me.totamountUSD.Text = ""
        Me.txtPrincipalUSD.Text = ""
        Me.txtChargeUSD.Text = ""
        Me.txtcountUSD.Text = ""
        clearGrid()
    End Sub
    
End Class
