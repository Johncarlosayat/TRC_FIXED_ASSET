'Imports MySql.Data.MySqlClient
'Public Class delete_confirm

'    Public Property EnteredID As String = ""

'    Private Sub btn_ok_Click(sender As Object, e As EventArgs) Handles btn_ok.Click
'        EnteredID = txt_confirmID.Text

'        Dim idwithA As String = "A" & txt_confirmID.Text & "A"
'        Dim idwithoutA As String = txt_confirmID.Text.TrimStart("A"c).TrimEnd("A"c)
'        Dim idwithoutasmall As String = txt_confirmID.Text.TrimStart("a"c).TrimEnd("a"c)
'        Dim cmd As New MySqlCommand("SELECT FULLNAME FROM tblscanoperator WHERE IDNUMBER = @idwithoutA OR IDNUMBER = @idwithA OR IDNUMBER = @idwithoutasmall", con)
'        cmd.Parameters.AddWithValue("@idwithoutA", idwithoutA)
'        cmd.Parameters.AddWithValue("@idwithA", idwithA)
'        cmd.Parameters.AddWithValue("@idwithoutasmall", idwithoutasmall)

'        Me.DialogResult = DialogResult.OK
'        Me.Close()

'    End Sub
'    Private Sub delete_confirm_Load(sender As Object, e As EventArgs) Handles MyBase.Load

'    End Sub
'End Class
Imports MySql.Data.MySqlClient

Public Class delete_confirm
    Public Property EnteredID As String = ""

    Private Sub btn_ok_Click(sender As Object, e As EventArgs) Handles btn_ok.Click
        Dim idInput As String = txt_confirmID.Text.Trim()

        If idInput = "" Then
            MessageBox.Show("Please enter an ID.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim idwithA As String = "A" & idInput & "A"
        Dim idwithoutA As String = idInput.TrimStart("A"c).TrimEnd("A"c)
        Dim idwithoutasmall As String = idInput.TrimStart("a"c).TrimEnd("a"c)

        Try
            con.Open()
            Using cmd As New MySqlCommand("SELECT FULLNAME FROM tblscanoperator WHERE IDNUMBER = @id1 OR IDNUMBER = @id2 OR IDNUMBER = @id3", con)
                cmd.Parameters.AddWithValue("@id1", idwithoutA)
                cmd.Parameters.AddWithValue("@id2", idwithA)
                cmd.Parameters.AddWithValue("@id3", idwithoutasmall)

                Dim reader As MySqlDataReader = cmd.ExecuteReader()
                If reader.HasRows Then
                    EnteredID = idInput
                    Me.DialogResult = DialogResult.OK
                    Me.Close()
                Else
                    MessageBox.Show("Invalid ID. Deletion not allowed.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
                reader.Close()
            End Using
        Catch ex As Exception
            MessageBox.Show("Error checking ID: " & ex.Message)
        Finally
            If con.State = ConnectionState.Open Then con.Close()
        End Try
    End Sub

    Private Sub delete_confirm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        txt_confirmID.Select()
    End Sub
End Class
