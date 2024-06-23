Imports System.Net
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Text
Imports System.Threading.Tasks

Public Class ServicioCambioPass

    Public Class RespuestaPass
        Public Property Success As Boolean
        Public Property msgError As String
        Public Property Data As String
    End Class

    Private ReadOnly _httpClient As HttpClient

    Public Sub New()
        _httpClient = New HttpClient()
        _httpClient.BaseAddress = New Uri("https://eagora.telefonica.es/")

        ' Configurar las credenciales de autenticación básica
        Dim byteArray = Encoding.ASCII.GetBytes(Configuracion.UserPass)
        _httpClient.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray))
    End Sub

    Public Async Function CambiarPassword(usuario As String, nuevaPassword As String) As Task(Of RespuestaPass)
        Dim respuestaPass As New RespuestaPass()
        Dim content As New FormUrlEncodedContent(New Dictionary(Of String, String) From {
            {"id", usuario},
            {"userPassword", nuevaPassword}
        })

        Dim response = Await _httpClient.PostAsync("/qj/gu/admin/2.1/gui/jsp/usr/acc/mod.jsp", content)

        If nuevaPassword = Thanatos.txtPassword.Text Then

            If response.IsSuccessStatusCode Then
                Dim responseData = Await response.Content.ReadAsStringAsync()
                If responseData.Contains("Se ha producido un error") Then
                    respuestaPass.Success = False
                    respuestaPass.msgError = "Se ha producido un error"
                ElseIf responseData.Contains("Password en Historial") Then
                    respuestaPass.Success = False
                    respuestaPass.msgError = "Forzandola..."
                Else
                    respuestaPass.Data = $"Nueva contraseña: {nuevaPassword}"
                    respuestaPass.Success = True
                End If
            Else
                respuestaPass.msgError = "Error al cambiar la contraseña"
                respuestaPass.Success = False
            End If

            Return respuestaPass
        Else
            respuestaPass.Success = True
            Return respuestaPass

        End If

    End Function

    Public Async Function ForzarPassword(usuario As String, nuevaPassword As String) As Task(Of RespuestaPass)
        Dim respuestaPass As New RespuestaPass()
        Dim intento As Integer = 1

        While intento <= 11
            Dim passwordIntento As String = If(intento <= 11, nuevaPassword & intento.ToString(), nuevaPassword)
            respuestaPass = Await CambiarPassword(usuario, passwordIntento)
            intento += 1
            Await Task.Delay(250)
        End While


        respuestaPass = Await CambiarPassword(usuario, nuevaPassword)
        respuestaPass.Success = True
        respuestaPass.Data = ($"Nueva contraseña: {nuevaPassword}")
        Return respuestaPass
    End Function
End Class
