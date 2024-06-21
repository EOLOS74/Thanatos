Imports System.Net.Http
Imports System.Threading.Tasks
Imports System.Windows.Forms

Public Class ServicioCambiarPassword
    Private ReadOnly client As HttpClient

    Public Sub New()
        client = New HttpClient()
        client.BaseAddress = New Uri("https://eagora.telefonica.es")
    End Sub

    Public Function CompruebaCampos(datos As DatosUsuario) As Respuesta
        Dim respuesta As New Respuesta With {.Success = False, .msgError = ""}
        If String.IsNullOrEmpty(datos.Password) Then
            respuesta.msgError &= "Contraseña vacía" & vbCrLf
        End If
        If datos.Password.Length < 8 OrElse datos.Password.Length > 14 Then
            respuesta.msgError &= "La contraseña debe tener entre 8 y 14 caracteres." & vbCrLf
        End If
        If String.IsNullOrEmpty(datos.Eagora) Then
            respuesta.msgError &= "Usuario vacío" & vbCrLf
        End If

        If String.IsNullOrEmpty(respuesta.msgError) Then
            respuesta.Success = True
        End If
        Return respuesta
    End Function

    Public Sub IniciarCambioPassword(webBrowserControl As WebBrowser, usuario As String, nuevaPassword As String)
        ' Establecer bandera de navegación
        Dim navigateForPasswordChange As Boolean = True

        ' Navegar a la URL de cambio de contraseña y ejecutar el script para enviar el formulario
        Dim changePasswordUrl As String = "https://eagora.telefonica.es/qj/gu/admin/2.1/gui/jsp/usr/acc/mod.jsp"
        webBrowserControl.Navigate(changePasswordUrl)

        ' Guardar las credenciales en los tags para usarlos después
        webBrowserControl.Tag = New Tuple(Of String, String, Boolean)(usuario, nuevaPassword, navigateForPasswordChange)
    End Sub
End Class

Public Class DatosUsuario
    Public Property Eagora As String
    Public Property Password As String
End Class

'Public Class Respuesta
'    Public Property Success As Boolean
'    Public Property msgError As String
'    Public Property Data As String
'End Class
