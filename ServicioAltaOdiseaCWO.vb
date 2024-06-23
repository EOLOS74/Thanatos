Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Text
Imports System.Threading.Tasks

Public Class ServicioAltaOdiseaCWO

    Public Class RespuestaOdiseaCWO
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

    Public Async Function AltaOdiseaCWO(usuario As Usuario) As Task(Of RespuestaOdiseaCWO)
        Dim respuesta As New RespuestaOdiseaCWO()

        Try
            Dim content As New FormUrlEncodedContent(New Dictionary(Of String, String) From {
                {"id", usuario.eagora},
                {"subid", "od"},
                {"state", "0"},
                {"telefonicaperfilaut", "Usuario"}
            })

            ' Enviar la solicitud POST
            Dim response = Await _httpClient.PostAsync("/qj/gu/admin/3.0/gui/jsp/users/roles/modifyRoles.jsp", content)

            ' Procesar la respuesta
            If response.IsSuccessStatusCode Then
                Dim responseData = Await response.Content.ReadAsStringAsync()
                If responseData.Contains("OD - Odise@") Then
                    respuesta.Success = True
                    respuesta.Data = responseData
                Else
                    respuesta.msgError = "Ha ocurrido un error en el servicio de altaOdisea() -> " & responseData
                End If
            Else
                respuesta.msgError = "Error en la solicitud HTTP: " & response.ReasonPhrase
            End If
        Catch ex As Exception
            respuesta.msgError = "Excepción: " & ex.Message
        End Try

        Return respuesta
    End Function
End Class
