Imports System.Net
Imports System.Net.Mail

Public Class ServicioEnvioEmail
    Friend Function SendEmail(ByVal _estructuraEmail As EstructuraEmail) As Boolean
        Try
            ' Creación del cliente SMTP
            Using client As New SmtpClient(_estructuraEmail.SmtpServer, _estructuraEmail.SmtpPort)
                client.EnableSsl = True
                client.Credentials = New NetworkCredential(_estructuraEmail.SmtpUsername, _estructuraEmail.SmtpPassword)

                ' Creación del mensaje
                Dim message As New MailMessage()
                message.From = New MailAddress(_estructuraEmail.SmtpUsername)

                ' Agregar destinatarios (manejo de comas y puntos y comas)
                Dim toAddressList = _estructuraEmail.ToAddresses.Split({","c, ";"c}, StringSplitOptions.RemoveEmptyEntries)
                For Each address As String In toAddressList
                    message.To.Add(address.Trim())
                Next

                ' Agregar destinatarios en copia (CC)
                If Not String.IsNullOrEmpty(_estructuraEmail.CcAddresses) Then
                    Dim ccAddressList = _estructuraEmail.CcAddresses.Split({","c, ";"c}, StringSplitOptions.RemoveEmptyEntries)
                    For Each address As String In ccAddressList
                        message.CC.Add(address.Trim())
                    Next
                End If

                ' Agregar destinatarios en copia oculta (BCC)
                If Not String.IsNullOrEmpty(_estructuraEmail.BccAddresses) Then
                    Dim bccAddressList = _estructuraEmail.BccAddresses.Split({","c, ";"c}, StringSplitOptions.RemoveEmptyEntries)
                    For Each address As String In bccAddressList
                        message.Bcc.Add(address.Trim())
                    Next
                End If

                message.Subject = _estructuraEmail.Subject
                message.Body = _estructuraEmail.Body

                ' Envío del correo electrónico
                client.Send(message)

                Return True
            End Using
        Catch ex As Exception
            Throw
        End Try
    End Function
End Class
