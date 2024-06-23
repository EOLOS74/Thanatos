Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Text
Imports System.Threading.Tasks

Public Class ServicioAltaPF
    Public Class RespuestaPF
        Public Property Success As Boolean
        Public Property msgError As String
        Public Property Data As String
    End Class

    Private ReadOnly _httpClient As HttpClient

    Public Sub New()
        _httpClient = New HttpClient()
        ' Configura la base address si tienes una URL base común para todas las solicitudes
        _httpClient.BaseAddress = New Uri("https://eagora.telefonica.es/")

        ' Configurar las credenciales de autenticación básica
        Dim byteArray = Encoding.ASCII.GetBytes(Configuracion.UserPass)
        _httpClient.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray))
    End Sub

    Public Async Function AltaPF(usuario As Usuario) As Task(Of RespuestaPF)
        Dim respuesta As New RespuestaPF()
        Try
            Dim parametros As String = GetParametrosAltaPF(usuario)
            Dim uri As String = $"/qj/gu/admin/3.0/gui/jsp/users/roles/modifyRoles.jsp{parametros}"

            ' Enviar la solicitud GET
            Dim response = Await _httpClient.GetAsync(uri)

            ' Procesar la respuesta
            If response.IsSuccessStatusCode Then
                Dim responseData = Await response.Content.ReadAsStringAsync()
                respuesta.Data = responseData
                If responseData.Contains("PFADSL - PRUEBA FINAL ADSL") Then
                    respuesta.Success = True
                Else
                    respuesta.msgError = "Ha ocurrido un error en el servicio de altaPF()"
                End If
            Else
                respuesta.msgError = "Error en la solicitud HTTP: " & response.ReasonPhrase
            End If
            Return respuesta
        Catch ex As Exception
            respuesta.msgError = "Excepción: " & ex.Message
            Return respuesta
        End Try
    End Function

    Private Function GetParametrosAltaPF(usuario As Usuario) As String
        Dim provincia As String = ""
        Dim codigoProvincia = ObtenerCodigoProvincia(usuario.provincia)
        provincia = "PROV_" & codigoProvincia

        Dim parametros As String = $"?id={usuario.eagora}&subid=PFADSL&state=0&telefonicaperfilaut={provincia}&telefonicaperfilaut=Usuario"
        Return parametros
    End Function

    Private Function ObtenerCodigoProvincia(nombreProvincia As String) As String
        Dim codigoProvincia As String = Configuracion.Provincias.FirstOrDefault(Function(p) p.Value.Equals(nombreProvincia, StringComparison.OrdinalIgnoreCase)).Key
        Return If(String.IsNullOrEmpty(codigoProvincia), nombreProvincia, codigoProvincia)
    End Function
End Class
