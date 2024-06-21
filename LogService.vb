Public Class LogService
    Private logTextBox As TextBox

    Public Sub New(logTextBox As TextBox)
        Me.logTextBox = logTextBox
    End Sub

    Public Sub AddLog(message As String)
        If logTextBox.InvokeRequired Then
            logTextBox.Invoke(New Action(Of String)(AddressOf AddLog), message)
        Else
            logTextBox.AppendText($"{message}{Environment.NewLine}")
        End If
    End Sub
End Class

