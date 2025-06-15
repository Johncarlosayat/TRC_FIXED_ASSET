Imports MySql.Data.MySqlClient
Public Class add_supplier
    Private Sub Guna2Button1_Click(sender As Object, e As EventArgs) Handles Guna2Button1.Click
        Try
            con.Close()
            con.Open()
            Dim addsupplier As New MySqlCommand("INSERT INTO cbmasterlist (selection,destination) 
                                VALUES ('" & txt_supp.Text & "', 'Supplier')", con)
            addsupplier.ExecuteNonQuery()

            Form1.LoadComboBoxData()


            Me.Close()
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
        txt_supp.Clear()
    End Sub


End Class