Imports MySql.Data.MySqlClient

Module Module1
    Public Function connection() As MySqlConnection
        'Return New MySqlConnection("server=localhost;port=3306;username=root;password=;database=trcsystem")
        Return New MySqlConnection("server=PTI-032;user id=inventoryf2;password=admin123@;database=trcsystem")
    End Function

    Public con As MySqlConnection = connection()
    Public cmd As New MySqlCommand
    Public da As New MySqlDataAdapter
    Public dt As New DataTable
    Public dt1 As New DataTable
    Public da1 As New MySqlDataAdapter
    Public dr As MySqlDataReader


    Public Sub OpenConnection()
        Try
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
        Catch ex As Exception
            MessageBox.Show("Error opening connection: " & ex.Message)
        End Try
    End Sub

    Public Sub CloseConnection()
        Try
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        Catch ex As Exception
            MessageBox.Show("Error closing connection: " & ex.Message)
        End Try
    End Sub

    Public Sub display_form(form As Windows.Forms.Form)
        With form
            .Refresh()
            .TopLevel = False
            Mainframe.Panel1.Controls.Add(form)
            .BringToFront()
            .Show()
        End With
    End Sub
    Public Sub cmb_display(sql As String, column As String, cmb As Guna.UI2.WinForms.Guna2ComboBox)
        Try
            con.Close()
            con.Open()
            Dim cmdselect As New MySqlCommand(sql, con)
            dr = cmdselect.ExecuteReader
            cmb.Items.Clear()
            While (dr.Read())
                cmb.Items.Add(dr.GetString(column))
            End While
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

End Module