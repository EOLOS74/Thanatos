Imports System.Text.RegularExpressions

Public Class ValidadorUsuario
    Public Function CompruebaCampos(usuario As Usuario) As ResultadoValidacion
        Dim respuesta As New ResultadoValidacion()
        Dim patronSGA As String = "^\d{6}$"
        Dim patronNombre As String = "^[a-zA-ZÑñáéíóúüÁÉÍÓÚÜ\s]+$"
        Dim patronApellidos As String = "^[a-zA-ZÑñáéíóúÁÉÍÓÚ ]+$"
        Dim patronDocumentoDNI As String = "^\d{8}[a-zA-Z]$"
        Dim patronDocumentoOtros As String = "^[a-zA-Z]\d{7}[a-zA-Z]$"
        Dim patronDocumentoFalso As String = "^[a-zA-Z]\d{8}[a-zA-Z]$"
        Dim patronTelefono As String = "^\d{9}$"
        Dim patronEmail As String = "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"

        If String.IsNullOrEmpty(usuario.id) Then
            respuesta.MensajeError = "Revise el campo Alias de eAgora. Es posible que solo estén pidiendo SGA y no un alta nueva." & Environment.NewLine
        End If
        If String.IsNullOrEmpty(usuario.userpassword) Then
            respuesta.MensajeError &= "Revise el campo Password." & Environment.NewLine
        End If
        If String.IsNullOrEmpty(usuario.eagora) Then
            respuesta.MensajeError &= "Revise el eAgora." & Environment.NewLine
        End If
        If String.IsNullOrEmpty(usuario.givenName) OrElse Not Regex.IsMatch(usuario.givenName, patronNombre) Then
            respuesta.MensajeError &= "Revise el campo Nombre." & Environment.NewLine
        End If
        If String.IsNullOrEmpty(usuario.sn) OrElse Not Regex.IsMatch(usuario.sn, patronApellidos) Then
            respuesta.MensajeError &= "Revise el campo Apellidos." & Environment.NewLine
        End If
        If Not Regex.IsMatch(usuario.telefonicadocumento, patronDocumentoDNI) AndAlso Not Regex.IsMatch(usuario.telefonicadocumento, patronDocumentoOtros) AndAlso Not Regex.IsMatch(usuario.telefonicadocumento, patronDocumentoFalso) Then
            respuesta.MensajeError &= "Revise el campo nº Documento (patrón NIF: 12345678X, patrón Otros: X1234567X)." & Environment.NewLine
        End If
        If Regex.IsMatch(usuario.telefonicadocumento, patronDocumentoFalso) Then
            Dim c = MessageBox.Show("El documento es de formato FALSO. ¿Desea continuar el alta?", "Advertencia", MessageBoxButtons.YesNo)
            If c = DialogResult.No Then
                respuesta.MensajeError &= "PETICIÓN DE DOCUMENTO FALSO CANCELADO." & Environment.NewLine
            End If
        End If
        If Regex.IsMatch(usuario.telefonicadocumento, patronDocumentoDNI) Then
            usuario.telefonicatipodocumento = "D"
            usuario.telefonicanacionalidad = "ES"
        ElseIf Regex.IsMatch(usuario.telefonicadocumento, patronDocumentoOtros) Then
            usuario.telefonicatipodocumento = "O"
            usuario.telefonicanacionalidad = "EC"
        ElseIf Regex.IsMatch(usuario.telefonicadocumento, patronDocumentoFalso) Then
            usuario.telefonicatipodocumento = "O"
            usuario.telefonicanacionalidad = "EC"
        End If
        If String.IsNullOrEmpty(usuario.telephoneNumber) OrElse Not Regex.IsMatch(usuario.telephoneNumber, patronTelefono) Then
            respuesta.MensajeError &= "Revise el campo Teléfono." & Environment.NewLine
        End If
        If String.IsNullOrEmpty(usuario.provincia) Then
            respuesta.MensajeError &= "Revise el campo Provincia." & Environment.NewLine
        End If
        If String.IsNullOrEmpty(usuario.perfil) Then
            respuesta.MensajeError &= "Rellene el campo Perfil." & Environment.NewLine
        End If
        If String.IsNullOrEmpty(usuario.email) OrElse Not Regex.IsMatch(usuario.email, patronEmail) Then
            respuesta.MensajeError &= "Revise el campo Email." & Environment.NewLine
        End If
        If String.IsNullOrEmpty(usuario.sga) OrElse Not Regex.IsMatch(usuario.sga, patronSGA) Then
            respuesta.MensajeError &= "Revise el campo SGA." & Environment.NewLine
        End If

        If String.IsNullOrEmpty(respuesta.MensajeError) Then
            respuesta.Success = True
        End If

        Return respuesta
    End Function
End Class

Public Class ResultadoValidacion
    Public Property Success As Boolean
    Public Property MensajeError As String
End Class
