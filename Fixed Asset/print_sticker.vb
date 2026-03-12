Imports QRCoder
Imports System.IO
Imports CrystalDecisions.CrystalReports

Public Class print_sticker

    Dim dt_records As New DataTable

    Public fano As String
    Public fatype As String
    Public section As String
    Public date_ac As Date
    Public pono As String
    Public sino As String
    Public qrcode As String
    Public _property As String
    Private Sub print_sticker_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        loadrpt()
    End Sub

    Private Sub loadrpt()
        Try
            dt_records.Clear()
            InitializeDataTable()

            ' Generate and store QR code
            Dim qrImageBytes As Byte() = GenerateQRCodeBytes(qrcode)
            dt_records.Rows.Add(fano, fatype, section, date_ac, pono, sino, qrImageBytes, _property)

            BindReportToViewer()
        Catch ex As Exception
            MessageBox.Show($"Error setting report data source: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub InitializeDataTable()
        dt_records.Columns.Add("FANO", GetType(String))
        dt_records.Columns.Add("FATYPE", GetType(String))
        dt_records.Columns.Add("SECTION", GetType(String))
        dt_records.Columns.Add("DATE", GetType(Date))
        dt_records.Columns.Add("PONO", GetType(String))
        dt_records.Columns.Add("SINO", GetType(String))
        dt_records.Columns.Add("qrcode", GetType(Byte()))
        dt_records.Columns.Add("_property", GetType(String))
    End Sub

    Private Function GenerateQRCodeBytes(serial As String) As Byte()
        Using qrImage As Bitmap = GenerateQRCode(serial)
            Return ImageToByteArray(qrImage)
        End Using
    End Function

    Private Sub BindReportToViewer()
        Dim myrpt As New sticker()
        CrystalReportViewer1.ReportSource = Nothing

        Dim reportTable As CrystalDecisions.CrystalReports.Engine.Table = myrpt.database.Tables("tblfixedasset")
        If reportTable Is Nothing Then
            MessageBox.Show("Table 'tblfixedasset' not found in the report.")
            Return
        End If

        reportTable.SetDataSource(dt_records)
        CrystalReportViewer1.ReportSource = myrpt
    End Sub


    Private Function GenerateQRCode(serial As String) As Bitmap
        Dim qrGenerator As New QRCodeGenerator()
        Dim qrCodeData As QRCodeData = qrGenerator.CreateQrCode(serial, QRCodeGenerator.ECCLevel.Q)
        Dim qrCode As New QRCode(qrCodeData)
        Return qrCode.GetGraphic(20)
    End Function
    Private Function ImageToByteArray(image As Image) As Byte()
        Using ms As New MemoryStream()
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Png)
            Return ms.ToArray()
        End Using
    End Function


End Class