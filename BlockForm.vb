Public Class BlockForm
    Inherits Form

    Private lblMessage As Label

    Public Sub New()
        Me.FormBorderStyle = FormBorderStyle.None
        Me.ShowInTaskbar = False
        Me.StartPosition = FormStartPosition.Manual
        Me.Opacity = 0.5 ' Hacer el formulario semitransparente
        Me.BackColor = Color.Black ' Color de fondo
        Me.TopMost = True ' Asegurarse de que esté en la parte superior

        ' Añadir un Label para mostrar el mensaje
        lblMessage = New Label()
        lblMessage.ForeColor = Color.White
        lblMessage.BackColor = Color.Transparent
        lblMessage.AutoSize = True
        lblMessage.Font = New Font("Arial", 16, FontStyle.Bold)
        Me.Controls.Add(lblMessage)

        ' Añadir el controlador de eventos para el clic del ratón en el Label
        AddHandler lblMessage.MouseClick, AddressOf lblMessage_MouseClick
    End Sub

    ' Método para actualizar el mensaje
    Public Sub UpdateMessage(message As String)
        lblMessage.Text = message
        CenterLabel()
    End Sub

    ' Método para centrar el label
    Private Sub CenterLabel()
        lblMessage.Location = New Point((Me.Width - lblMessage.Width) / 2, (Me.Height - lblMessage.Height) / 2)
    End Sub

    ' Método para ajustar el tamaño y la posición del BlockForm
    Public Sub AdjustSizeAndPosition(parentForm As Form)
        Me.Size = parentForm.ClientSize
        Me.Location = parentForm.PointToScreen(Point.Empty)
        CenterLabel()
    End Sub

    Private Sub lblMessage_MouseClick(sender As Object, e As MouseEventArgs)
        If e.Button = MouseButtons.Middle Then
            Me.Hide()
        End If
    End Sub
End Class
