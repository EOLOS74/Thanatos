Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading.Tasks

Public Class ServicioAltaMira
    Public Class RespuestaMira
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

    Public Async Function GetSolicitudMira(usuario As String) As Task(Of RespuestaMira)
        Dim respuesta As New RespuestaMira()
        Try
            Dim url As String = $"/qj/jsp/solicitudes/newMultipleRequest.jsp?uid={usuario}&telefonicanombreaplicacion=mira&descripcionOtraApl="
            Dim response = Await _httpClient.GetAsync(url)
            If response.IsSuccessStatusCode Then
                Dim responseData = Await response.Content.ReadAsStringAsync()
                Dim solicitudId = GetEntreTextos(responseData, "?rid=", "&source=")
                Dim patronMira = "^\d{6}\d*$"
                If Not String.IsNullOrEmpty(solicitudId) AndAlso Regex.IsMatch(solicitudId, patronMira) Then
                    respuesta.Success = True
                    respuesta.Data = solicitudId
                Else
                    respuesta.msgError = "Error al solicitar Mir@: " & responseData
                End If
            Else
                respuesta.msgError = "Error en la solicitud HTTP: " & response.ReasonPhrase
            End If
        Catch ex As Exception
            respuesta.msgError = "Excepción: " & ex.Message
        End Try
        Return respuesta
    End Function

    Public Async Function AltaMira(usuario As Usuario, solicitudId As String) As Task(Of RespuestaMira)
        Dim respuesta As New RespuestaMira()
        Try
            Dim parametros As Dictionary(Of String, String) = GetParametrosAltaMira(usuario, solicitudId)
            Dim uri As String = $"/qj/jsp/solicitudes/approveRequest.jsp?rid={parametros("rid")}&telefonicaformdepartamentotde={parametros("telefonicaformdepartamentotde")}&telefonicaformnombreresponsabletde={parametros("telefonicaformnombreresponsabletde")}&telefonicaformtelefonoresponsabletde={parametros("telefonicaformtelefonoresponsabletde")}&telefonicaformprovincia={parametros("telefonicaformprovincia")}&comment={parametros("comment")}"
            Dim response = Await _httpClient.GetAsync(uri)
            If response.StatusCode = 401 Then
                respuesta.Success = True
                respuesta.Data = Await response.Content.ReadAsStringAsync()
            Else
                respuesta.Success = False
                respuesta.msgError = "Ha ocurrido un error en el servicio de altaMira()"
            End If
        Catch ex As Exception
            respuesta.msgError = "Excepción: " & ex.Message
        End Try
        Return respuesta
    End Function

    Public Function GetParametrosAltaMira(usuario As Usuario, solicitudId As String) As Dictionary(Of String, String)
        Dim responsable As String = ""
        Dim telefonoResponsable As String = ""
        Select Case usuario.provincia
            Case "05", "AVILA"
                responsable = "Francisco Javier Puyana Estrada"
                telefonoResponsable = "954483611"
            Case "06", "BADAJOZ"
                responsable = "Jose Manuel Rodriguez Escalona"
                telefonoResponsable = "924211221"
            Case "10", "CACERES"
                responsable = "Jose Manuel Rodriguez Escalona"
                telefonoResponsable = "924211221"
            Case "11", "CADIZ"
                responsable = "Francisco Jose de Asis Garcia Quiros"
                telefonoResponsable = "956241907"
            Case "21", "HUELVA"
                responsable = "Julio Garcia Quintana"
                telefonoResponsable = "959211191"
            Case "23", "JAEN"
                responsable = "Santiago Bernabe Donaire Vico"
                telefonoResponsable = "953218550"
            Case "24", "LEON"
                responsable = "Francisco Javier Puyana Estrada"
                telefonoResponsable = "954483611"
            Case "28", "MADRID"
                responsable = "Jose Maria Gonzalez"
                telefonoResponsable = "915844102"
            Case "35", "LAS PALMAS"
                responsable = "Francisco Javier Puyana Estrada"
                telefonoResponsable = "954483611"
            Case "37", "SALAMANCA"
                responsable = "Francisco Javier Puyana Estrada"
                telefonoResponsable = "954483611"
            Case "41", "SEVILLA"
                responsable = "Daniel Catalan Molina"
                telefonoResponsable = "954483424"
            Case "49", "ZAMORA"
                responsable = "Francisco Javier Puyana Estrada"
                telefonoResponsable = "954483611"
        End Select

        Return New Dictionary(Of String, String) From {
            {"rid", solicitudId},
            {"telefonicaformdepartamentotde", "I mas M Provision"},
            {"telefonicaformnombreresponsabletde", responsable},
            {"telefonicaformtelefonoresponsabletde", telefonoResponsable},
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
