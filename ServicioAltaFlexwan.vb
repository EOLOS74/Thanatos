Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Text
Imports System.Threading.Tasks

Public Class ServicioAltaFlexwan

    Public Class RespuestaFlexwan
        Public Property Success As Boolean
        Public Property msgError As String
        Public Property Data As String
    End Class

    Private ReadOnly _httpClient As HttpClient

    Public Sub New()
        _httpClient = New HttpClient()
        _httpClient.BaseAddress = New Uri("https://eagora.telefonica.es/")
        Dim byteArray = Encoding.ASCII.GetBytes(Configuracion.UserPass)
        _httpClient.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray))
    End Sub

    Public Async Function GetSolicitudFlexwan(usuario As String) As Task(Of RespuestaFlexwan)
        Dim respuesta As New RespuestaFlexwan()
        Dim response = Await _httpClient.GetAsync($"/qj/jsp/solicitudes/newMultipleRequest.jsp?uid={usuario}&telefonicanombreaplicacion=o4&descripcionOtraApl=")
        If response.IsSuccessStatusCode Then
            Dim responseData = Await response.Content.ReadAsStringAsync()
            respuesta.Data = GetEntreTextos(responseData, "?rid=", "&source=")
            Dim patronMira = New Text.RegularExpressions.Regex("^\d{6}\d*$")
            If Not String.IsNullOrEmpty(respuesta.Data) AndAlso patronMira.IsMatch(respuesta.Data) Then
                respuesta.Success = True
                ' Logging logic here
            Else
                respuesta.Success = False
                respuesta.msgError = "Error al solicitar FLEXWAN. Más información en la consola del navegador: " & responseData
            End If
        Else
            respuesta.Success = False
            respuesta.msgError = "Error al solicitar FLEXWAN: " & response.ReasonPhrase
        End If
        Return respuesta
    End Function

    Public Function GetParametrosAltaFlexwan(solicitudFlexwan As String) As Dictionary(Of String, String)
        Return New Dictionary(Of String, String) From {
            {"rid", solicitudFlexwan},
            {"telefonicaformcodigocontrata", "009"},
            {"comment", "Sin comentarios"}
        }
    End Function

    Public Async Function AltaFlexwan(parametros As Dictionary(Of String, String)) As Task(Of RespuestaFlexwan)

        Dim respuesta As New RespuestaFlexwan()

        Dim response = Await _httpClient.GetAsync($"/qj/jsp/solicitudes/approveRequest.jsp?rid={parametros("rid")}&telefonicaformcodigocontrata={parametros("telefonicaformcodigocontrata")}&comment={parametros("comment")}")

        ' Procesar la respuesta
        If response.StatusCode = 401 Then
            respuesta.Success = True
            respuesta.Data = Await response.Content.ReadAsStringAsync()
        Else
            respuesta.Success = False
            respuesta.msgError = "Ha ocurrido un error en el servicio de AltaFlexwan()"
        End If

        Return respuesta
    End Function

    Private Function GetEntreTextos(texto As String, subTexto1 As String, subTexto2 As String) As String
        Dim posicion1 = texto.IndexOf(subTexto1)
        Dim posicion2 = texto.IndexOf(subTexto2)
        If posicion1 <> -1 AndAlso posicion2 <> -1 AndAlso posicion1 < posicion2 Then
            Return texto.Substring(posicion1 + subTexto1.Length, posicion2 - posicion1 - subTexto1.Length)
        End If
        Return String.Empty
    End Function
End Class
