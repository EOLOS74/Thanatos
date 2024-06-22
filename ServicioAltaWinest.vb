Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading.Tasks

Public Class ServicioAltaWinest
    Public Class RespuestaWinest
        Public Property Success As Boolean
        Public Property msgError As String
        Public Property Data As String
    End Class

    Private ReadOnly _httpClient As HttpClient

    Public Sub New()
        _httpClient = New HttpClient()
        _httpClient.BaseAddress = New Uri("https://eagora.telefonica.es/")

        ' Configurar las credenciales de autenticación básica
        Dim byteArray = Encoding.ASCII.GetBytes("x009ear:026telco")
        _httpClient.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray))
    End Sub

    Public Async Function GetSolicitudWinest(usuario As String) As Task(Of RespuestaWinest)
        Dim respuesta As New RespuestaWinest()
        Try
            Dim url As String = $"/qj/jsp/solicitudes/newMultipleRequest.jsp?uid={usuario}&telefonicanombreaplicacion=ui&descripcionOtraApl="

            ' Enviar la solicitud GET
            Dim response = Await _httpClient.GetAsync(url)

            ' Procesar la respuesta
            If response.IsSuccessStatusCode Then
                Dim responseData = Await response.Content.ReadAsStringAsync()
                Dim solicitudId = GetEntreTextos(responseData, "?rid=", "&source=")
                Dim patronWinest = "^\d{6}\d*$"

                If Not String.IsNullOrEmpty(solicitudId) AndAlso Regex.IsMatch(solicitudId, patronWinest) Then
                    respuesta.Success = True
                    respuesta.Data = solicitudId
                Else
                    respuesta.msgError = "Error al solicitar Winest. Más información en la consola del navegador" & responseData
                End If
            Else
                respuesta.msgError = "Error en la solicitud HTTP: " & response.ReasonPhrase
            End If
        Catch ex As Exception
            respuesta.msgError = "Excepción: " & ex.Message
        End Try
        Return respuesta
    End Function

    Public Async Function AltaWinest(usuario As Usuario, solicitudId As String) As Task(Of RespuestaWinest)
        Dim respuesta As New RespuestaWinest()
        Try
            Dim parametros As Dictionary(Of String, String) = GetParametrosAltaWinest(usuario, solicitudId)

            ' Construir la URL con los parámetros
            Dim uri As String = $"/qj/jsp/solicitudes/approveRequest.jsp?rid={parametros("rid")}&telefonicaformsegmento={parametros("telefonicaformsegmento")}&telefonicaformmismoperfilusuario={parametros("telefonicaformmismoperfilusuario")}&telefonicaformprovincia={parametros("telefonicaformprovincia")}&comment={parametros("comment")}"

            ' Enviar la solicitud GET
            Dim response = Await _httpClient.GetAsync(uri)

            ' Procesar la respuesta
            If response.StatusCode = 401 Then
                respuesta.Success = True
                respuesta.Data = Await response.Content.ReadAsStringAsync()
            Else
                respuesta.msgError = "Ha ocurrido un error en el servicio de altaWinest()"
            End If
        Catch ex As Exception
            respuesta.msgError = "Excepción: " & ex.Message
        End Try
        Return respuesta
    End Function

    Private Function GetParametrosAltaWinest(usuario As Usuario, solicitudId As String) As Dictionary(Of String, String)
        Return New Dictionary(Of String, String) From {
            {"rid", solicitudId},
            {"telefonicaformsegmento", "TODOS"},
            {"telefonicaformmismoperfilusuario", "X009ALG"},
            {"telefonicaformprovincia", usuario.provincia},
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

