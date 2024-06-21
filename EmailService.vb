' EmailService.vb
Imports System.Net
Imports System.Net.Mail

Public Class EmailService
    Public Function SendEmail(ByVal smtpServer As String, ByVal smtpPort As Integer, ByVal smtpUsername As String, ByVal smtpPassword As String, ByVal toAddresses As String, ByVal subject As String, ByVal body As String) As Boolean
        Try
            ' Creación del cliente SMTP
            Using client As New SmtpClient(smtpServer, smtpPort)
                client.EnableSsl = True
                client.Credentials = New NetworkCredential(smtpUsername, smtpPassword)

                ' Creación del mensaje
                Dim message As New MailMessage()
                message.From = New MailAddress(smtpUsername)

                ' Agregar destinatarios
                For Each address As String In toAddresses.Split(","c)
                    message.To.Add(address.Trim())
                Next

                message.Subject = subject
                message.Body = body

                ' Envío del correo electrónico
                client.Send(message)

                Return True
            End Using
        Catch ex As Exception
            Throw
        End Try
    End Function
End Class
