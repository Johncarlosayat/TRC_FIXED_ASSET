Imports MySql.Data.MySqlClient
Imports System.Globalization

Public Class Form1
    Dim secno As Integer
    Public qrcode As String
    Public related As Integer
    Public dataid As Integer = 0
    Dim sectionCode As String
    Dim serialcode As Integer
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadData()
        LoadData1()
        datagrid1.ReadOnly = True
        datagrid2.ReadOnly = True
        LoadComboBoxData()
        DisableInputFields()
        cb_status.Text = "Active"
        btn_edit.Enabled = False
        btn_delete.Enabled = False
        btn_print.Enabled = False
        btn_access.Enabled = False
        btnaddservice.Enabled = False
        dt_date.Value = Date.Now
        dt_date.Enabled = True
    End Sub


    Private Sub btn_save_Click(sender As Object, e As EventArgs) Handles btn_save.Click

        If Not ValidateFields() Then Return

        Try
            OpenConnection()

            ' Get count of related services for this FANO
            Dim relatedServiceCount As Integer = 0
            Using serviceCountCmd As New MySqlCommand("SELECT COUNT(*) FROM tblservices WHERE FANO = @fano", con)
                serviceCountCmd.Parameters.AddWithValue("@fano", txt_fano.Text)
                relatedServiceCount = Convert.ToInt32(serviceCountCmd.ExecuteScalar())
            End Using

            ' Insert record into tblfixedasset including NO_OF_RELATED_SERVICES
            cmd.Connection = con
            cmd.CommandText = "INSERT INTO tblfixedasset (
                                FULLNAME, FANO, FATYPE, serial, SECTION, ITEMDES, DATE, MAKER, MACHINE, MODEL, 
                                PONO, INVOICE, SINO, AMOUNT, CURRENCY, SUPPLIER, STATUS, REMARK, QRCODE, NO_OF_RELATED_SERVICES
                            ) 
                            VALUES (
                                @fullname, @fano, @fanotype, @SERIAL, @section, @itemdes, @date, @maker, @machine, @model, 
                                @pono, @invoice, @sino, @amount, @currency, @supplier, @status, @remark, @qrcode, @related
                            )"
            cmd.Parameters.Clear()
            cmd.Parameters.AddWithValue("@fullname", txt_user.Text)
            cmd.Parameters.AddWithValue("@fano", txt_fano.Text)
            cmd.Parameters.AddWithValue("@fanotype", cb_fatype.Text)
            cmd.Parameters.AddWithValue("@SERIAL", serialcode)
            cmd.Parameters.AddWithValue("@section", cb_section.Text)
            cmd.Parameters.AddWithValue("@itemdes", txt_itemdes.Text)
            cmd.Parameters.AddWithValue("@date", dt_date.Value.ToString("yyyy-MM-dd"))
            cmd.Parameters.AddWithValue("@maker", cb_maker.Text)
            cmd.Parameters.AddWithValue("@machine", cb_machine.Text)
            cmd.Parameters.AddWithValue("@model", cb_model.Text)
            cmd.Parameters.AddWithValue("@pono", txt_pono.Text)
            cmd.Parameters.AddWithValue("@invoice", txt_invoice.Text)
            cmd.Parameters.AddWithValue("@sino", txt_sino.Text)
            cmd.Parameters.AddWithValue("@amount", Convert.ToDecimal(txt_amount.Text))
            cmd.Parameters.AddWithValue("@currency", boxc.Text)
            cmd.Parameters.AddWithValue("@supplier", cb_supplier.Text)
            cmd.Parameters.AddWithValue("@status", cb_status.Text)
            cmd.Parameters.AddWithValue("@remark", txt_remark.Text)
            cmd.Parameters.AddWithValue("@qrcode", $"{txt_fano.Text}|{cb_fatype.Text}|{dt_date.Value:yyyy-MM-dd}")
            cmd.Parameters.AddWithValue("@related", relatedServiceCount)

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
            txt_fano.Text = String.Empty
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
        If String.IsNullOrWhiteSpace(txt_fano.Text) Or
           cb_fatype.SelectedIndex = -1 Or
           cb_section.SelectedIndex = -1 Or
           cb_status.SelectedIndex = -1 Or
           boxc.SelectedIndex = -1 Or
            cb_maker.SelectedIndex = -1 Or
            cb_machine.SelectedIndex = -1 Or
            cb_model.SelectedIndex = -1 Or
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

        ' Validate required fields
        If cb_servicepro.SelectedIndex = -1 OrElse
       String.IsNullOrWhiteSpace(txt_amount1.Text) OrElse
       String.IsNullOrWhiteSpace(txt_sino1.Text) Then
            MessageBox.Show("Please fill in all required fields.")
            Return
        End If

        Try
            OpenConnection()

            ' Check existing service count
            Dim count As Integer
            Using checkCmd As New MySqlCommand("SELECT COUNT(*) FROM tblservices WHERE FANO = @fano", con)
                checkCmd.Parameters.AddWithValue("@fano", txt_fano.Text)
                count = Convert.ToInt32(checkCmd.ExecuteScalar())
            End Using

            If count >= 3 Then
                MessageBox.Show("Maximum of 3 related services allowed for this FA No.")
                Return
            End If

            ' Insert new service record
            Using insertCmd As New MySqlCommand("
            INSERT INTO tblservices (FANO, SERVICEPRO, ACCDATE, PODATE, CURRENCY, AMOUNT, SINO, REMARKS)
            VALUES (@fano, @servicepro, @accdate, @po, @currency, @amount, @sino, @rema)", con)

                insertCmd.Parameters.AddWithValue("@fano", txt_fano.Text)
                insertCmd.Parameters.AddWithValue("@servicepro", cb_servicepro.Text)
                insertCmd.Parameters.AddWithValue("@accdate", dt_accomdate.Value.ToString("yyyy-MM-dd"))
                insertCmd.Parameters.AddWithValue("@po", txtpo.Text)
                insertCmd.Parameters.AddWithValue("@currency", boxc1.Text)
                insertCmd.Parameters.AddWithValue("@amount", txt_amount1.Text)
                insertCmd.Parameters.AddWithValue("@sino", txt_sino1.Text)
                insertCmd.Parameters.AddWithValue("@rema", txtrema.Text)

                insertCmd.ExecuteNonQuery()
            End Using

            ' Update NO_OF_RELATED_SERVICES in tblfixedasset
            Using updateCmd As New MySqlCommand("
            UPDATE tblfixedasset
            SET NO_OF_RELATED_SERVICES = (
                SELECT COUNT(*) FROM tblservices WHERE FANO = @fano
            )
            WHERE FANO = @fano", con)

                updateCmd.Parameters.AddWithValue("@fano", txt_fano.Text)
                updateCmd.ExecuteNonQuery()
            End Using

            MessageBox.Show("Service record added successfully!")

            ' Reset UI and reload
            txt_fano.Text = String.Empty
            ClearInputFields()
            ClearInputFields1()
            DisableInputFields()
            LoadData()
            LoadData1()
            datagrid1.ClearSelection()
            datagrid2.ClearSelection()
            btn_access.Enabled = False
            btn_save.Enabled = True
            btn_delete.Enabled = False
            btn_print.Enabled = False
            btn_edit.Enabled = False
            dt_date.Enabled = True

        Catch ex As Exception
            MessageBox.Show("Error adding record: " & ex.Message)
        Finally
            CloseConnection()
        End Try
        btnaddservice.Enabled = False
    End Sub

    Private Sub ClearInputFields()

        txt_fano.Clear()
        cb_fatype.SelectedIndex = -1
        cb_section.SelectedIndex = -1
        txt_itemdes.Clear()
        dt_date.Value = DateTime.Now
        cb_maker.SelectedIndex = -1
        cb_machine.SelectedIndex = -1
        cb_model.SelectedIndex = -1
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
        txtrema.Clear()
    End Sub
    ' This method enables input fields
    Private Sub EnableInputFields()
        cb_servicepro.Enabled = True
        boxc1.Enabled = True
        txt_amount1.Enabled = True
        txt_sino1.Enabled = True
        dt_accomdate.Enabled = True
        txtpo.Enabled = True
        txtrema.Enabled = True
    End Sub

    ' This method disables input fields
    Private Sub DisableInputFields()
        cb_servicepro.Enabled = False
        boxc1.Enabled = False
        txt_amount1.Enabled = False
        txt_sino1.Enabled = False
        dt_accomdate.Enabled = False
        txtpo.Enabled = False
        txtrema.Enabled = False

    End Sub


    Private Sub LoadData()
        Try
            CloseConnection()
            OpenConnection()
            dt.Clear()
            Dim query As String = "Select `ID`,ROW_NUMBER() OVER (ORDER BY ID) AS NO, `FULLNAME`, `FANO`, `FATYPE`, `SECTION`, `ITEMDES`, `DATE`, `MAKER`, `MACHINE`, `MODEL`, `PONO`, `INVOICE`, `SINO`, `AMOUNT`, `CURRENCY`, `SUPPLIER`, `STATUS`, `REMARK`, `QRCODE`, `NO_OF_RELATED_SERVICES` FROM `tblfixedasset`"


            da = New MySqlDataAdapter(query, con)
            da.Fill(dt)
            datagrid1.DataSource = dt

            If datagrid1.Columns.Contains("id") Then
                datagrid1.Columns("id").Visible = False
            End If

            CloseConnection()

            OpenConnection()

            ' Prepare the SQL query to fetch data based on the selected section
            Dim cmdSelect As New MySqlCommand("Select COUNT(id)  FROM tblfixedasset", con)
            cmdSelect.Parameters.AddWithValue("@section", cb_section.Text)

            ' Execute the command and read the data
            Dim dr As MySqlDataReader = cmdSelect.ExecuteReader()

            'If dr.Read() Then
            '    txt_no.Text = dr.GetInt32(0) + 1

            'Else

            'End If

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

        Try
            con.Close()
            con.Open()

            ' Get the current FANO
            Dim fano As String = txt_fano.Text

            ' Get related service count
            Dim relatedServiceCount As Integer = 0
            Using countCmd As New MySqlCommand("SELECT COUNT(*) FROM tblservices WHERE FANO = @fano", con)
                countCmd.Parameters.AddWithValue("@fano", fano)
                relatedServiceCount = Convert.ToInt32(countCmd.ExecuteScalar())
            End Using

            ' Update the fixed asset record
            Using cmd As New MySqlCommand("
            UPDATE tblfixedasset 
            SET 
                FULLNAME = @fullname,
                FANO = @fano,
                FATYPE = @fanotype,
                SECTION = @section,
                ITEMDES = @itemdes,
                DATE = @date,
                MAKER = @maker,
                MACHINE = @machine,
                MODEL = @model,
                PONO = @pono,
                INVOICE = @invoice,
                SINO = @sino,
                AMOUNT = @amount,
                CURRENCY = @currency,
                SUPPLIER = @supplier,
                STATUS = @status,
                REMARK = @remark,
                QRCODE = @qrcode,
                NO_OF_RELATED_SERVICES = @related
            WHERE id = @id", con)

                cmd.Parameters.Clear()
                cmd.Parameters.AddWithValue("@id", id)
                cmd.Parameters.AddWithValue("@fullname", txt_user.Text)
                cmd.Parameters.AddWithValue("@fano", fano)
                cmd.Parameters.AddWithValue("@fanotype", cb_fatype.Text)
                cmd.Parameters.AddWithValue("@section", cb_section.Text)
                cmd.Parameters.AddWithValue("@itemdes", txt_itemdes.Text)
                cmd.Parameters.AddWithValue("@date", dt_date.Value.ToString("yyyy-MM-dd"))
                cmd.Parameters.AddWithValue("@maker", cb_maker.Text)
                cmd.Parameters.AddWithValue("@machine", cb_machine.Text)
                cmd.Parameters.AddWithValue("@model", cb_model.Text)
                cmd.Parameters.AddWithValue("@pono", txt_pono.Text)
                cmd.Parameters.AddWithValue("@invoice", txt_invoice.Text)
                cmd.Parameters.AddWithValue("@sino", txt_sino.Text)
                cmd.Parameters.AddWithValue("@amount", Convert.ToDecimal(txt_amount.Text))
                cmd.Parameters.AddWithValue("@currency", boxc.Text)
                cmd.Parameters.AddWithValue("@supplier", cb_supplier.Text)
                cmd.Parameters.AddWithValue("@status", cb_status.Text)
                cmd.Parameters.AddWithValue("@remark", txt_remark.Text)
                cmd.Parameters.AddWithValue("@qrcode", fano & "|" & cb_fatype.Text & "|" & dt_date.Value.ToString("yyyy-MM-dd"))
                cmd.Parameters.AddWithValue("@related", relatedServiceCount)

                cmd.ExecuteNonQuery()
            End Using

            MessageBox.Show("Record updated successfully!")

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
        btn_edit.Enabled = False
        btn_save.Enabled = True
        btn_delete.Enabled = False
        btn_print.Enabled = False
        btn_access.Enabled = False
        btnaddservice.Enabled = False
        dt_date.Enabled = True
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
            cmb_display("SELECT DISTINCT(selection) FROM cbmasterlist WHERE destination='Maker'", "selection", cb_maker)
            cmb_display("SELECT DISTINCT(selection) FROM cbmasterlist WHERE destination='Machine Type'", "selection", cb_machine)
            cmb_display("SELECT DISTINCT(selection) FROM cbmasterlist WHERE destination='Model'", "selection", cb_model)



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
            Case "Office"
                sectionCode = "OF"
        End Select

    End Sub

    Private Sub cmbsearch_TextChanged(sender As Object, e As EventArgs) Handles cmbsearch.TextChanged
        Dim da As MySqlDataAdapter = Nothing
        Dim da1 As MySqlDataAdapter = Nothing

        Try
            If cmbsearch.Text = "" Then
                LoadData()
            Else
                con.Close()
                con.Open()

                ' Search in tblfixedasset by FANO, ITEMDES, or QRCODE
                Dim cmdSearch As New MySqlCommand("
                SELECT `ID`, ROW_NUMBER() OVER (ORDER BY ID) AS NO, `FULLNAME`, `FANO`, `FATYPE`, `SECTION`, 
                       `ITEMDES`, `DATE`, `MAKER`, `MACHINE`, `MODEL`, `PONO`, `INVOICE`, `SINO`, 
                       `AMOUNT`, `CURRENCY`, `SUPPLIER`, `STATUS`, `REMARK`, `QRCODE`, `NO_OF_RELATED_SERVICES`
                FROM `tblfixedasset` 
                WHERE FANO LIKE @searchText OR ITEMDES LIKE @searchText OR QRCODE LIKE @searchText", con)
                cmdSearch.Parameters.AddWithValue("@searchText", "%" & cmbsearch.Text & "%")

                Dim dt As New DataTable
                da = New MySqlDataAdapter(cmdSearch)
                da.Fill(dt)
                datagrid1.DataSource = dt

                ' Collect matching FANOs
                Dim matchingFANOs As New List(Of String)
                For Each row As DataRow In dt.Rows
                    matchingFANOs.Add(row("FANO").ToString())
                Next

                ' Search related records in tblservices
                Dim dt1 As New DataTable
                If matchingFANOs.Count > 0 Then
                    Dim paramNames As New List(Of String)
                    Dim cmdSearch1 As New MySqlCommand()
                    cmdSearch1.Connection = con

                    For i As Integer = 0 To matchingFANOs.Count - 1
                        Dim paramName As String = "@fano" & i
                        cmdSearch1.Parameters.AddWithValue(paramName, matchingFANOs(i))
                        paramNames.Add(paramName)
                    Next

                    cmdSearch1.CommandText = "SELECT * FROM tblservices WHERE FANO IN (" & String.Join(",", paramNames) & ")"
                    da1 = New MySqlDataAdapter(cmdSearch1)
                    da1.Fill(dt1)
                End If

                datagrid2.DataSource = dt1
            End If

        Catch ex As Exception
            MessageBox.Show("Search error: " & ex.Message)
        Finally
            con.Close()
            If da IsNot Nothing Then da.Dispose()
            If da1 IsNot Nothing Then da1.Dispose()
        End Try
    End Sub



    Private accessGranted As Boolean = False

    Private Sub btn_access_Click(sender As Object, e As EventArgs) Handles btn_access.Click
        ' Code to grant access, if any
        accessGranted = True
        btn_edit.Enabled = False
        btn_print.Enabled = False
        btn_delete.Enabled = False

        EnableInputFields()
        MessageBox.Show("Access granted! You can now add services.")
        btn_access.Enabled = False
    End Sub


    Private Sub datagrid1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles datagrid1.CellClick
        If e.RowIndex >= 0 Then
            ClearInputFields()
            btn_edit.Enabled = True
            btn_save.Enabled = False
            btn_delete.Enabled = True
            btn_print.Enabled = True
            btn_access.Enabled = True
            btnaddservice.Enabled = True
            dt_date.Enabled = False
            Dim row As DataGridViewRow = datagrid1.Rows(e.RowIndex)
            'txt_user.Text = row.Cells("FULLNAME").Value.ToString()


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
            cb_maker.Text = row.Cells("MAKER").Value.ToString()
            cb_machine.Text = row.Cells("MACHINE").Value.ToString()
            cb_model.Text = row.Cells("MODEL").Value.ToString()
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
            related = row.Cells("NO_OF_RELATED_SERVICES").Value.ToString()
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
        'Try
        '    con.Close()
        '    con.Open()


        '    Dim query As String = "SELECT CONCAT(@sectioncode, '-', @ayear, '-', LPAD(IFNULL(MAX(serial), 0) + 1, 5, '0')) AS ID, LPAD(IFNULL(MAX(serial), 0) + 1) AS serial " &
        '              "FROM tblfixedasset WHERE YEAR(date) = @year AND LEFT(`FANO`, LOCATE('-', `FANO`) - 1) = @sectioncode;"


        '    Using cmd As New MySqlCommand(query, con)
        '        cmd.Parameters.AddWithValue("@sectioncode", sectioncode)
        '        cmd.Parameters.AddWithValue("@ayear", ayear)
        '        cmd.Parameters.AddWithValue("@year", ayear)

        '        dr = cmd.ExecuteReader()
        '        If dr.Read() Then

        '            serialcode = dr(1)



        '            Return dr("ID").ToString()
        '        End If
        '    End Using
        'Catch ex As Exception
        '    MessageBox.Show("Error: " & ex.Message)
        'Finally
        '    con.Close()
        'End Try

        'Return ""
        Try
            con.Close()
            con.Open()

            Dim query As String = "SELECT 
  CONCAT(@sectioncode, '-', @ayear, '-', LPAD(IFNULL(MAX(serial), 0) + 1, 5, '0')) AS ID,
  (IFNULL(MAX(serial), 0) + 1) AS serial
From tblfixedasset
Where Year(`date`) = @year 
  And LEFT(`FANO`, LOCATE('-', `FANO`) - 1) = @sectioncode;"


            Using cmd As New MySqlCommand(query, con)
                cmd.Parameters.AddWithValue("@sectioncode", sectioncode)
                cmd.Parameters.AddWithValue("@ayear", ayear)
                cmd.Parameters.AddWithValue("@year", ayear)

                dr = cmd.ExecuteReader()
                If dr.Read() Then
                    serialcode = dr.GetInt32("serial")
                    ' MessageBox.Show(serialcode)
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
        DisableInputFields()
        LoadData()
        LoadData1()
        datagrid1.ClearSelection()
        datagrid2.ClearSelection()
        btn_edit.Enabled = False
        btn_save.Enabled = True
        btn_delete.Enabled = False
        btn_print.Enabled = False
        btn_access.Enabled = False
        btnaddservice.Enabled = False
        dt_date.Enabled = True
    End Sub

    'Private Sub btn_delete_Click(sender As Object, e As EventArgs) Handles btn_delete.Click
    '    txt_fano.Text = String.Empty
    '    If datagrid1.SelectedRows.Count = 0 Then
    '        MessageBox.Show("Please select a record to delete.", "Delete", MessageBoxButtons.OK, MessageBoxIcon.Warning)
    '        Return
    '    End If

    '    Dim result As DialogResult = MessageBox.Show("Are you sure you want to delete this record?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
    '    If result = DialogResult.Yes Then
    '        Try
    '            Dim selectedId As String = datagrid1.SelectedRows(0).Cells("ID").Value.ToString() ' Use the actual primary key column name
    '            con.Open()
    '            cmd = New MySqlCommand("DELETE FROM tblfixedasset WHERE ID = @id", con)
    '            cmd.Parameters.AddWithValue("@id", selectedId)
    '            cmd.ExecuteNonQuery()
    '            con.Close()

    '            MessageBox.Show("Record deleted successfully.", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information)
    '            LoadData()
    '            datagrid1.ClearSelection()
    '            datagrid2.ClearSelection()
    '            ClearInputFields()
    '            ClearInputFields1()
    '            btn_edit.Enabled = False
    '            btn_save.Enabled = True
    '        Catch ex As Exception
    '            MessageBox.Show("Error while deleting: " & ex.Message)
    '            If con.State = ConnectionState.Open Then con.Close()
    '        End Try
    '    End If
    'End Sub
    Private Sub btn_delete_Click(sender As Object, e As EventArgs) Handles btn_delete.Click
        If datagrid1.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select a record to delete.", "Delete", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' Show confirmation form
        Dim confirmForm As New delete_confirm()
        If confirmForm.ShowDialog() = DialogResult.OK Then
            Dim selectedRow As DataGridViewRow = datagrid1.SelectedRows(0)
            Dim selectedId As String = selectedRow.Cells("ID").Value.ToString()
            Dim fanoToDelete As String = selectedRow.Cells("FANO").Value.ToString()

            Dim result As DialogResult = MessageBox.Show("Are you sure you want to delete this record?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            If result = DialogResult.Yes Then
                Try
                    con.Open()

                    ' First delete related services from tblservices
                    Using deleteServicesCmd As New MySqlCommand("DELETE FROM tblservices WHERE FANO = @fano", con)
                        deleteServicesCmd.Parameters.AddWithValue("@fano", fanoToDelete)
                        deleteServicesCmd.ExecuteNonQuery()
                    End Using

                    ' Then delete the main record from tblfixedasset
                    Using deleteAssetCmd As New MySqlCommand("DELETE FROM tblfixedasset WHERE ID = @id", con)
                        deleteAssetCmd.Parameters.AddWithValue("@id", selectedId)
                        deleteAssetCmd.ExecuteNonQuery()
                    End Using

                    MessageBox.Show("Record and related services deleted successfully.", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information)

                    ' Reset UI
                    txt_fano.Text = String.Empty
                    LoadData()
                    LoadData1()
                    datagrid1.ClearSelection()
                    datagrid2.ClearSelection()
                    ClearInputFields()
                    ClearInputFields1()
                    btn_edit.Enabled = False
                    btn_save.Enabled = True
                    btn_delete.Enabled = False
                    btn_access.Enabled = False
                    btnaddservice.Enabled = False
                    dt_date.Enabled = True

                Catch ex As Exception
                    MessageBox.Show("Error while deleting: " & ex.Message)
                Finally
                    If con.State = ConnectionState.Open Then con.Close()
                End Try
            End If
        End If
    End Sub


    Private Sub Guna2Button4_Click(sender As Object, e As EventArgs) Handles Guna2Button4.Click
        exportExcel(datagrid1, "Fixed asset")
    End Sub

    Private Sub Guna2Button5_Click(sender As Object, e As EventArgs) Handles Guna2Button5.Click
        add_maker.ShowDialog()
        add_maker.BringToFront()
    End Sub

    Private Sub Guna2Button6_Click(sender As Object, e As EventArgs) Handles Guna2Button6.Click
        add_machine.ShowDialog()
        add_machine.BringToFront()
    End Sub

    Private Sub Guna2Button7_Click(sender As Object, e As EventArgs) Handles Guna2Button7.Click
        add_model.ShowDialog()
        add_model.BringToFront()
    End Sub


End Class
