Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading.Tasks

Public Class ServicioAltaWpex
    Public Class RespuestaWpex
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

    Public Async Function GetSolicitudWpex(usuario As String) As Task(Of RespuestaWpex)
        Dim respuesta As New RespuestaWpex()
        Try
            Dim url As String = $"/qj/jsp/solicitudes/newMultipleRequest.jsp?uid={usuario}&telefonicanombreaplicacion=wpex&descripcionOtraApl="
            Dim response = Await _httpClient.GetAsync(url)
            If response.IsSuccessStatusCode Then
                Dim responseData = Await response.Content.ReadAsStringAsync()
                Dim solicitudId = GetEntreTextos(responseData, "?rid=", "&source=")
                Dim patronWpex = "^\d{6}\d*$"
                If Not String.IsNullOrEmpty(solicitudId) AndAlso Regex.IsMatch(solicitudId, patronWpex) Then
                    respuesta.Success = True
                    respuesta.Data = solicitudId
                Else
                    respuesta.msgError = "Error al solicitar ESCAPEX: " & responseData
                End If
            Else
                respuesta.msgError = "Error en la solicitud HTTP: " & response.ReasonPhrase
            End If
        Catch ex As Exception
            respuesta.msgError = "Excepción: " & ex.Message
        End Try
        Return respuesta
    End Function

    Public Async Function AltaWpex(solicitudId As String) As Task(Of RespuestaWpex)
        Dim respuesta As New RespuestaWpex()
        Try
            Dim parametros As Dictionary(Of String, String) = GetParametrosAltaWpex(solicitudId)
            Dim uri As String = $"/qj/jsp/solicitudes/approveRequest.jsp?rid={parametros("rid")}&telefonicaformcodigocontrata={parametros("telefonicaformcodigocontrata")}&comment={parametros("comment")}"
            Dim response = Await _httpClient.GetAsync(uri)
            If response.StatusCode = 401 Then
                respuesta.Success = True
                respuesta.Data = Await response.Content.ReadAsStringAsync()
            Else
                respuesta.Success = False
                respuesta.msgError = "Ha ocurrido un error en el servicio de altaWpex()"
            End If
        Catch ex As Exception
            respuesta.msgError = "Excepción: " & ex.Message
        End Try
        Return respuesta
    End Function

    Public Function GetParametrosAltaWpex(solicitudId As String) As Dictionary(Of String, String)
        Return New Dictionary(Of String, String) From {
            {"rid", solicitudId},
            {"telefonicaformcodigocontrata", "009"},
            {"comment", "Sin comentarios"}
        }
    End Function

    Private Function GetEntreTextos(texto As String, subTexto1 As String, subTexto2 As String) As String
        Dim entreTextos As String = ""
        Dim posicion1 = texto.IndexOf(subTexto1)
        Dim posicion2 = texto.IndexOf(subTexto2)
        If posicion1 <> -1 AndAlso posicion2 <> -1 AndAlso posicion1 < posicion2 Then
            entreTextos = texto.Substring(posicion1 + subTexto1.Length, posicion2 - (posicion1 + subTexto1.Length))
        End If
        Return entreTextos
    End Function
End Class
