Imports MySql.Data.MySqlClient
Imports System.Globalization

Public Class Form1
    Dim secno As Integer
    Public qrcode As String
    Public dataid As Integer = 0
    Dim sectionCode As String
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadData()
        LoadData1()
        datagrid1.ReadOnly = True
        datagrid2.ReadOnly = True
        LoadComboBoxData()
        DisableInputFields()
        cb_status.Text = "Active"
        btn_edit.Enabled = False


    End Sub


    Private Sub btn_save_Click(sender As Object, e As EventArgs) Handles btn_save.Click

        If Not ValidateFields() Then Return

        Try

            OpenConnection()
            cmd.Connection = con
            cmd.CommandText = "INSERT INTO tblfixedasset (FULLNAME, NO, FANO, FATYPE, SECTION, ITEMDES, DATE, PONO, INVOICE, SINO, AMOUNT,CURRENCY, SUPPLIER, STATUS, REMARK, QRCODE) 
                                VALUES (@fullname, @no, @fano, @fanotype, @section, @itemdes, @date, @pono, @invoice, @sino, @amount,@currency, @supplier, @status, @remark, @qrcode)"
            cmd.Parameters.Clear()
            cmd.Parameters.AddWithValue("@fullname", txt_user.Text)
            cmd.Parameters.AddWithValue("@no", txt_no.Text)
            cmd.Parameters.AddWithValue("@fano", txt_fano.Text)
            cmd.Parameters.AddWithValue("@fanotype", cb_fatype.Text)
            cmd.Parameters.AddWithValue("@section", cb_section.Text.Split(":"c)(1).Trim)
            cmd.Parameters.AddWithValue("@itemdes", txt_itemdes.Text)
            cmd.Parameters.AddWithValue("@date", dt_date.Value.ToString("yyyy-MM-dd"))
            cmd.Parameters.AddWithValue("@pono", txt_pono.Text)
            cmd.Parameters.AddWithValue("@invoice", txt_invoice.Text)
            cmd.Parameters.AddWithValue("@sino", txt_sino.Text)
            cmd.Parameters.AddWithValue("@amount", Convert.ToDecimal(txt_amount.Text))
            cmd.Parameters.AddWithValue("@currency", boxc.Text)
            cmd.Parameters.AddWithValue("@supplier", cb_supplier.Text)
            cmd.Parameters.AddWithValue("@status", cb_status.Text)
            cmd.Parameters.AddWithValue("@remark", txt_remark.Text)
            cmd.Parameters.AddWithValue("@qrcode", txt_fano.Text & "|" & cb_fatype.Text & "|" & dt_date.Value.ToString("yyyy-MM-dd"))

            cmd.ExecuteNonQuery()

            MessageBox.Show("Record added successfully!")

            CloseConnection()
            OpenConnection()
            ' Update the incremented secno in the database without using @section
            'Dim cmdUpdate As New MySqlCommand("UPDATE tblcn SET secno=@newSecNo WHERE selection=@section", con)
            'cmdUpdate.Parameters.AddWithValue("@newSecNo", secno)
            'cmdUpdate.Parameters.AddWithValue("@section", cb_section.Text)
            ' Execute the update command without adding the @section parameter
            'cmdUpdate.ExecuteNonQuery()
            txt_fano.Clear()
            ClearInputFields()
            LoadData()

        Catch ex As Exception
            MessageBox.Show("Error adding record: " & ex.Message)
        Finally

            CloseConnection()
        End Try
    End Sub
    Public Sub SetFullname(ByVal fullname As String)
        txt_user.Text = fullname
    End Sub
    Private Function ValidateFields() As Boolean
        If String.IsNullOrWhiteSpace(txt_no.Text) Or
           String.IsNullOrWhiteSpace(txt_fano.Text) Or
           cb_fatype.SelectedIndex = -1 Or
           cb_section.SelectedIndex = -1 Or
           cb_status.SelectedIndex = -1 Or
           boxc.SelectedIndex = -1 Or
           String.IsNullOrWhiteSpace(txt_itemdes.Text) Or
           String.IsNullOrWhiteSpace(txt_pono.Text) Or
           String.IsNullOrWhiteSpace(txt_invoice.Text) Or
           String.IsNullOrWhiteSpace(txt_sino.Text) Or
           String.IsNullOrWhiteSpace(txt_amount.Text) Or
           cb_supplier.SelectedIndex = -1 Then
            MessageBox.Show("Please fill in all required fields.")
            Return False
        End If
        Return True
    End Function

    Private Sub btnaddservice_Click(sender As Object, e As EventArgs) Handles btnaddservice.Click
        ' Check if access is granted
        If Not accessGranted Then
            MessageBox.Show("Please click the 'Access' button first.")
            Return
        End If
        If cb_servicepro.SelectedIndex = -1 Or
           String.IsNullOrWhiteSpace(txt_amount1.Text) Or
           String.IsNullOrWhiteSpace(txt_sino1.Text) Then
            MessageBox.Show("Please fill in all required fields.")
            Return
        End If

        Try
            OpenConnection()
            cmd.Connection = con
            cmd.CommandText = "INSERT INTO tblservices(FANO,SERVICEPRO, ACCDATE, PODATE,CURRENCY, AMOUNT, SINO, REMARKS) VALUES (@fano, @servicepro, @accdate, @po, @currency,@amount, @sino, @rema)"


            cmd.Parameters.Clear()
            cmd.Parameters.AddWithValue("@fano", txt_fano.Text)
            cmd.Parameters.AddWithValue("@servicepro", cb_servicepro.Text)
            cmd.Parameters.AddWithValue("@accdate", dt_accomdate.Value.ToString("yyyy-MM-dd"))
            cmd.Parameters.AddWithValue("@po", txtpo.Text)
            cmd.Parameters.AddWithValue("@currency", boxc1.Text)
            cmd.Parameters.AddWithValue("@amount", txt_amount1.Text)
            cmd.Parameters.AddWithValue("@sino", txt_sino1.Text)
            cmd.Parameters.AddWithValue("@rema", txtrema.Text)
            cmd.ExecuteNonQuery()

            MessageBox.Show("Record added successfully!")
            ClearInputFields1()
            DisableInputFields()
            LoadData1()
        Catch ex As Exception
            MessageBox.Show("Error adding record: " & ex.Message)
        Finally
            CloseConnection()
        End Try
    End Sub
    Private Sub ClearInputFields()
        txt_no.Clear()
        txt_fano.Clear()
        cb_fatype.SelectedIndex = -1
        cb_section.SelectedIndex = -1
        txt_itemdes.Clear()
        dt_date.Value = DateTime.Now
        txt_pono.Clear()
        txt_invoice.Clear()
        txt_sino.Clear()
        boxc.SelectedIndex = -1
        txt_amount.Clear()
        cb_supplier.SelectedIndex = -1
        txt_remark.Clear()
    End Sub
    Private Sub ClearInputFields1()
        cb_servicepro.SelectedIndex = -1
        boxc1.SelectedIndex = -1
        dt_accomdate.Value = DateTime.Now
        txt_amount1.Clear()
        txt_sino1.Clear()
        txtpo.Clear()
    End Sub
    ' This method enables input fields
    Private Sub EnableInputFields()
        cb_servicepro.Enabled = True
        boxc1.Enabled = True
        txt_amount1.Enabled = True
        txt_sino1.Enabled = True
        dt_accomdate.Enabled = True

    End Sub

    ' This method disables input fields
    Private Sub DisableInputFields()
        cb_servicepro.Enabled = False
        boxc1.Enabled = False
        txt_amount1.Enabled = False
        txt_sino1.Enabled = False
        dt_accomdate.Enabled = False

    End Sub


    Private Sub LoadData()
        Try
            CloseConnection()
            OpenConnection()
            dt.Clear()
            Dim query As String = "SELECT * FROM tblfixedasset"
            da = New MySqlDataAdapter(query, con)
            da.Fill(dt)
            datagrid1.DataSource = dt

            If datagrid1.Columns.Contains("id") Then
                datagrid1.Columns("id").Visible = False
            End If

            CloseConnection()

            OpenConnection()

            ' Prepare the SQL query to fetch data based on the selected section
            Dim cmdSelect As New MySqlCommand("SELECT COUNT(id)  FROM tblfixedasset", con)
            cmdSelect.Parameters.AddWithValue("@section", cb_section.Text)

            ' Execute the command and read the data
            Dim dr As MySqlDataReader = cmdSelect.ExecuteReader()

            If dr.Read() Then
                txt_no.Text = dr.GetInt32(0) + 1

            Else

            End If

        Catch ex As Exception
            MessageBox.Show("Error loading data: " & ex.Message)
        Finally
            CloseConnection()
        End Try
    End Sub

    Private Sub UpdateRecordWithTransaction()
        If datagrid1.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select a record to update.")
            Return
        End If

        Dim selectedRow As DataGridViewRow = datagrid1.SelectedRows(0)
        Dim id As Integer = Convert.ToInt32(selectedRow.Cells("id").Value)

        ' Using transactionConnection As New MySqlConnection("server=localhost;port=3306;username=root;password=;database=trcdatabase")
        Try
            con.Close()
            con.Open()

            Using cmd As New MySqlCommand("UPDATE tblfixedasset SET FULLNAME=@fullname, NO=@no, FANO=@fano, FATYPE=@fanotype, SECTION=@section, ITEMDES=@itemdes, DATE=@date, PONO=@pono, INVOICE=@invoice, SINO=@sino, AMOUNT=@amount, CURRENCY=@currency, SUPPLIER=@supplier, STATUS=@status, REMARK=@remark WHERE id=@id", con)
                cmd.Parameters.Clear()
                cmd.Parameters.AddWithValue("@id", id)
                cmd.Parameters.AddWithValue("@fullname", txt_user.Text)
                cmd.Parameters.AddWithValue("@no", txt_no.Text)

                cmd.Parameters.AddWithValue("@fanotype", cb_fatype.Text)
                cmd.Parameters.AddWithValue("@section", cb_section.Text.Split(":"c)(1).Trim)
                cmd.Parameters.AddWithValue("@itemdes", txt_itemdes.Text)
                cmd.Parameters.AddWithValue("@date", dt_date.Value.ToString("yyyy-MM-dd"))

                cmd.Parameters.AddWithValue("@pono", txt_pono.Text)
                cmd.Parameters.AddWithValue("@invoice", txt_invoice.Text)
                cmd.Parameters.AddWithValue("@sino", txt_sino.Text)
                cmd.Parameters.AddWithValue("@amount", Convert.ToDecimal(txt_amount.Text))
                cmd.Parameters.AddWithValue("@currency", boxc.Text)
                cmd.Parameters.AddWithValue("@supplier", cb_supplier.Text)
                cmd.Parameters.AddWithValue("@status", cb_status.Text)
                cmd.Parameters.AddWithValue("@remark", txt_remark.Text)
                cmd.Parameters.AddWithValue("@fano", txt_fano.Text)
                cmd.ExecuteNonQuery()
            End Using
        Catch ex As Exception
            MessageBox.Show("Error updating record: " & ex.Message)
        Finally
            txt_fano.Text = String.Empty
            con.Close()
        End Try

    End Sub

    Private Sub btn_edit_Click(sender As Object, e As EventArgs) Handles btn_edit.Click
        UpdateRecordWithTransaction()
        ClearInputFields()
        LoadData()

        datagrid1.ClearSelection()
        btn_save.Enabled = False
        btn_save.Enabled = True
    End Sub


    Private Sub datagrid1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles datagrid1.CellContentClick

    End Sub
    'Private Sub datagrid2_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles datagrid2.CellContentClick
    '    If e.RowIndex >= 0 Then
    '        Dim row As DataGridViewRow = datagrid2.Rows(e.RowIndex)
    '        txt_fano.Text = row.Cells("FANO").Value.ToString()
    '        cb_servicepro.Text = row.Cells("SERVICEPRO").Value.ToString()
    '        dt_accomdate.Value = Convert.ToDateTime(row.Cells("ACCDATE").Value)
    '        txtpo.Text = row.Cells("PODATE").Value.ToString()
    '        boxc1.Text = row.Cells("CURRENCY").Value.ToString()
    '        txt_amount1.Text = row.Cells("AMOUNT").Value.ToString()
    '        txt_sino1.Text = row.Cells("SINO").Value.ToString()
    '        txtrema.Text = row.Cells("REMARKS").Value.ToString()
    '    End If
    'End Sub

    Public Sub LoadComboBoxData()
        Try
            'EDIT : more efficient way to add items in combobox and it prevents duplicates
            'EDIT : "DISTINCT" function is used to remove duplicates in one column
            cmb_display("SELECT DISTINCT(selection) FROM cbmasterlist WHERE destination='Supplier'", "selection", cb_supplier)
            cmb_display("SELECT DISTINCT(selection) FROM cbmasterlist WHERE destination='FA Type'", "selection", cb_fatype)
            cmb_display("SELECT DISTINCT(selection) FROM cbmasterlist WHERE destination='Service Provider'", "selection", cb_servicepro)




            'CloseConnection()
            'OpenConnection()

            '' Load FA Type, Supplier, and Service Provider from cbmasterlist
            'Dim query As String = "SELECT selection, destination FROM cbmasterlist WHERE destination IN ('FA Type', 'Supplier', 'Service Provider')"
            'Dim cmd As New MySqlCommand(query, con)
            'Dim reader As MySqlDataReader = cmd.ExecuteReader()

            'While reader.Read()
            '    Select Case reader("destination").ToString()
            '        Case "FA Type"
            '            cb_fatype.Items.Add(reader("selection").ToString())
            '        Case "Supplier"
            '            cb_section.Items.Clear()
            '            cb_supplier.Items.Add(reader("selection").ToString())
            '        Case "Service Provider"
            '            cb_servicepro.Items.Add(reader("selection").ToString())
            '    End Select
            'End While
            'reader.Close()

            ' Load Section from tblcn
            'Dim sectionQuery As String = "SELECT selection FROM tblcn" ' Adjust 'selection' to the actual column name if necessary
            'Dim sectionCmd As New MySqlCommand(sectionQuery, con)
            'Dim sectionReader As MySqlDataReader = sectionCmd.ExecuteReader()

            'While sectionReader.Read()
            '    cb_section.Items.Add(sectionReader("selection").ToString()) ' Adjust 'selection' to the actual column name if necessary
            'End While
            'sectionReader.Close()

        Catch ex As Exception
            MessageBox.Show("Error loading combo box data: " & ex.Message)
        Finally
            CloseConnection()
        End Try
    End Sub


    'Private Sub btn_cancel_Click(sender As Object, e As EventArgs) Handles btn_cancel.Click
    '    ClearInputFields()
    '    ClearInputFields1()
    '    datagrid1.ClearSelection()
    '    datagrid2.ClearSelection()
    'End Sub

    Private Sub btn_exit_Click(sender As Object, e As EventArgs) Handles btn_exit.Click
        Dim result As DialogResult = MessageBox.Show("Are you sure you want to exit?", "Confirm Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If result = DialogResult.Yes Then Application.Exit()
    End Sub

    Private Sub cb_section_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cb_section.SelectedIndexChanged
        Select Case cb_section.Text
            Case "Assembly"
                sectionCode = "AS"
            Case "Painting"
                sectionCode = "PT"
            Case "Molding"
                sectionCode = "MO"
            Case "Motor Assy"
                sectionCode = "MR"
            Case "Rubber"
                sectionCode = "RB"
            Case "Tubepump"
                sectionCode = "TP"
            Case "Shaft"
                sectionCode = "SH"
            Case "Retainer Assy"
                sectionCode = "RA"
            Case "General"
                sectionCode = "GA"
        End Select

    End Sub

    Private Sub cmbsearch_TextChanged(sender As Object, e As EventArgs) Handles cmbsearch.TextChanged

        Try
            con.Close()
            con.Open()

            ' Modify the query to search for FA NO
            Dim cmdSearch As New MySqlCommand("SELECT *
                                           FROM tblfixedasset 
                                           WHERE FANO LIKE @searchText", con)
            cmdSearch.Parameters.AddWithValue("@searchText", "%" & cmbsearch.Text & "%")

            Dim da As New MySqlDataAdapter(cmdSearch)
            Dim dt As New DataTable
            da.Fill(dt)
            datagrid1.DataSource = dt

            Dim cmdSearch1 As New MySqlCommand("SELECT *
                                             FROM tblservices
                                             WHERE FANO LIKE @searchText", con)
            cmdSearch1.Parameters.AddWithValue("@searchText", "%" & cmbsearch.Text & "%")

            Dim da1 As New MySqlDataAdapter(cmdSearch1)
            Dim dt1 As New DataTable
            da1.Fill(dt1)
            datagrid2.DataSource = dt1


        Catch ex As Exception
            MessageBox.Show(ex.Message)
        Finally
            con.Close()
            da.Dispose()
            da1.Dispose()
        End Try
    End Sub

    Private accessGranted As Boolean = False

    Private Sub btn_access_Click(sender As Object, e As EventArgs) Handles btn_access.Click
        ' Code to grant access, if any
        accessGranted = True
        EnableInputFields()
        MessageBox.Show("Access granted! You can now add services.")
    End Sub


    Private Sub datagrid1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles datagrid1.CellClick
        If e.RowIndex >= 0 Then
            btn_edit.Enabled = True
            btn_save.Enabled = False
            Dim row As DataGridViewRow = datagrid1.Rows(e.RowIndex)
            'txt_user.Text = row.Cells("FULLNAME").Value.ToString()
            txt_no.Text = row.Cells("NO").Value.ToString()

            cb_fatype.Text = row.Cells("FATYPE").Value.ToString()


            'cb_section.Text = row.Cells("SECTION").Value.ToString()

            Dim sectionValue As String = row.Cells("SECTION").Value.ToString().Trim().ToUpper()

            Dim matchedIndex As Integer = -1
            For i As Integer = 0 To cb_section.Items.Count - 1
                Dim itemText As String = cb_section.Items(i).ToString().ToUpper()
                If itemText.Contains(sectionValue) Then
                    matchedIndex = i
                    Exit For
                End If
            Next

            If matchedIndex >= 0 Then
                cb_section.SelectedIndex = matchedIndex
            Else
                cb_section.SelectedIndex = -1 ' Optional: clear selection if not found
            End If


            txt_itemdes.Text = row.Cells("ITEMDES").Value.ToString()
            dt_date.Value = Convert.ToDateTime(row.Cells("DATE").Value)
            txt_fano.Text = row.Cells("FANO").Value.ToString()
            txt_pono.Text = row.Cells("PONO").Value.ToString()
            txt_invoice.Text = row.Cells("INVOICE").Value.ToString()
            txt_sino.Text = row.Cells("SINO").Value.ToString()
            boxc.Text = row.Cells("CURRENCY").Value.ToString()
            txt_amount.Text = row.Cells("AMOUNT").Value.ToString()
            cb_supplier.Text = row.Cells("SUPPLIER").Value.ToString()
            cb_status.Text = row.Cells("STATUS").Value.ToString()
            txt_remark.Text = row.Cells("REMARK").Value.ToString()
            qrcode = row.Cells("QRCODE").Value.ToString()
            dataid = row.Cells("id").Value.ToString()
        End If
    End Sub

    Private Sub btn_print_Click(sender As Object, e As EventArgs) Handles btn_print.Click
        If dataid = 0 Then
            MessageBox.Show("Please select item first")
        Else
            Dim print_s As New print_sticker
            With print_s
                .fano = txt_fano.Text
                .fatype = cb_fatype.Text
                .section = cb_section.Text
                .date_ac = dt_date.Value
                .pono = txt_pono.Text
                .sino = txt_sino.Text
                .qrcode = qrcode
                .ShowDialog()
                .BringToFront()
            End With

        End If
    End Sub

    Private Sub Guna2Button1_Click(sender As Object, e As EventArgs) Handles Guna2Button1.Click
        add_supplier.ShowDialog()
        add_supplier.BringToFront()
    End Sub

    Private Sub Guna2Button2_Click(sender As Object, e As EventArgs) Handles Guna2Button2.Click
        add_provider.ShowDialog()
        add_provider.BringToFront()
    End Sub

    Private Sub txt_amount_TextChanged(sender As Object, e As EventArgs) Handles txt_amount.TextChanged, txt_amount1.TextChanged
        Dim textBox As Guna.UI2.WinForms.Guna2TextBox = DirectCast(sender, Guna.UI2.WinForms.Guna2TextBox)


        Dim numericText As String = New String(textBox.Text.Where(Function(c) Char.IsDigit(c) Or c = "."c).ToArray())

        ' Remove existing event handler to prevent infinite loop
        RemoveHandler textBox.TextChanged, AddressOf txt_amount_TextChanged

        ' Format the numeric text as currency
        If Decimal.TryParse(numericText, NumberStyles.Any, CultureInfo.InvariantCulture, Nothing) Then
            Dim formattedText As String = String.Format(CultureInfo.CurrentCulture, "{0:N0}", Convert.ToDecimal(numericText))
            textBox.Text = formattedText
            ' Move the cursor to the end of the text
            textBox.SelectionStart = textBox.Text.Length
        End If

        ' Re-add the event handler
        AddHandler textBox.TextChanged, AddressOf txt_amount_TextChanged
    End Sub
    Private Sub LoadData1()
        Try
            OpenConnection()
            dt1.Clear()
            Dim query As String = "SELECT `ID`, `FANO`, `SERVICEPRO`, `ACCDATE`, `PODATE`, `CURRENCY`, `AMOUNT`, `SINO`, `REMARKS` FROM tblservices"
            da = New MySqlDataAdapter(query, con)
            da.Fill(dt1)
            datagrid2.DataSource = dt1

            If datagrid2.Columns.Contains("id") Then
                datagrid2.Columns("id").Visible = False
            End If
        Catch ex As Exception
            MessageBox.Show("Error loading data: " & ex.Message)
        Finally
            CloseConnection()
        End Try
    End Sub

    Private Sub txt_amount_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txt_amount.KeyPress, txt_amount1.KeyPress
        If Not Char.IsDigit(e.KeyChar) AndAlso Not Char.IsControl(e.KeyChar) Then
            e.Handled = True ' Discard the key press
        End If
    End Sub

    Private Sub dt_date_ValueChanged(sender As Object, e As EventArgs) Handles dt_date.ValueChanged
        If cb_section.SelectedIndex = -1 Then
            Exit Sub
        End If




        CloseConnection()

        OpenConnection()

        Try
            ' Prepare the SQL query to fetch data based on the selected section
            'Dim cmdSelect As New MySqlCommand("SELECT secno, secchar FROM tblcn WHERE selection=@section", con)

            'cmdSelect.Parameters.AddWithValue("@section", cb_section.Text)

            '' Execute the command and read the data
            'Dim dr As MySqlDataReader = cmdSelect.ExecuteReader()

            'If dr.Read() Then
            '    ' Fetch secno and secchar from the result
            '    secno = Convert.ToInt32(dr("secno"))
            '    Dim sechar As String = dr("secchar").ToString()

            '    ' Increment secno
            '    secno += 1


            ' Format the new secno as 5 digits and assign to txt_fano
            '  txt_fano.Text = sechar & "-" & dt_date.Value.ToString("yyyy") & "-" & secno.ToString("00000")


            txt_fano.Text = getFAno(dt_date.Value.ToString("yyyy"), sectionCode)

            '' Close the reader before updating the database
            'dr.Close()


            'Else

            'End If


        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        Finally
            ' Close the connection after execution
            CloseConnection()
        End Try
    End Sub


    Private Function getFAno(ayear As String, sectioncode As String) As String
        Try
            con.Close()
            con.Open()


            Dim query As String = "SELECT CONCAT(@sectioncode, '-', @ayear, '-', LPAD(IFNULL(COUNT(id), 0) + 1, 5, '0')) AS ID " &
                      "FROM tblfixedasset WHERE YEAR(date) = @year AND LEFT(`FANO`, LOCATE('-', `FANO`) - 1) = @sectioncode;"


            Using cmd As New MySqlCommand(query, con)
                cmd.Parameters.AddWithValue("@sectioncode", sectioncode)
                cmd.Parameters.AddWithValue("@ayear", ayear)
                cmd.Parameters.AddWithValue("@year", ayear)

                dr = cmd.ExecuteReader()
                If dr.Read() Then
                    Return dr("ID").ToString()
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        Finally
            con.Close()
        End Try

        Return ""
    End Function


    Private Sub Guna2Panel3_Paint(sender As Object, e As PaintEventArgs) Handles Guna2Panel3.Paint

    End Sub

    Private Sub Guna2Button3_Click(sender As Object, e As EventArgs) Handles Guna2Button3.Click
        add_type.ShowDialog()
        add_type.BringToFront()
    End Sub



    Private Sub Label22_Click(sender As Object, e As EventArgs) Handles Label22.Click

    End Sub

    Private Sub txt_user_TextChanged(sender As Object, e As EventArgs) Handles txt_user.TextChanged

    End Sub

    Private Sub btn_cancel_Click(sender As Object, e As EventArgs) Handles btn_cancel.Click
        txt_fano.Text = String.Empty
        ClearInputFields()
        ClearInputFields1()
        LoadData()
        datagrid1.ClearSelection()
        datagrid2.ClearSelection()
        btn_edit.Enabled = False
        btn_save.Enabled = True
    End Sub
End Class
