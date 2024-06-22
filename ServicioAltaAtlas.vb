Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading.Tasks

Public Class ServicioAltaAtlas
    Public Class RespuestaAtlas
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

    Public Async Function GetSolicitudAtlas(usuario As String) As Task(Of RespuestaAtlas)
        Dim respuesta As New RespuestaAtlas()
        Try
            Dim url As String = $"/qj/jsp/solicitudes/newMultipleRequest.jsp?uid={usuario}&telefonicanombreaplicacion=AtlasPA&descripcionOtraApl="

            ' Enviar la solicitud GET
            Dim response = Await _httpClient.GetAsync(url)

            ' Procesar la respuesta
            If response.IsSuccessStatusCode Then
                Dim responseData = Await response.Content.ReadAsStringAsync()
                Dim solicitudId = GetEntreTextos(responseData, "?rid=", "&source=")
                Dim patronAtlas = "^\d{6}\d*$"

                If Not String.IsNullOrEmpty(solicitudId) AndAlso Regex.IsMatch(solicitudId, patronAtlas) Then
                    respuesta.Success = True
                    respuesta.Data = solicitudId
                Else
                    respuesta.msgError = "Error al solicitar AtlasPA. Más información en la consola del navegador" & responseData
                End If
            Else
                respuesta.msgError = "Error en la solicitud HTTP: " & response.ReasonPhrase
            End If
        Catch ex As Exception
            respuesta.msgError = "Excepción: " & ex.Message
        End Try
        Return respuesta
    End Function

    Public Async Function AltaAtlas(usuario As Usuario, solicitudId As String) As Task(Of RespuestaAtlas)
        Dim respuesta As New RespuestaAtlas()
        Try
            Dim parametros As Dictionary(Of String, String) = GetParametrosAltaAtlas(usuario, solicitudId)

            ' Construir la URL con los parámetros
            Dim uri As String = $"/qj/jsp/solicitudes/approveRequest.jsp?rid={parametros("rid")}&telefonicaformdepartamentotde={parametros("telefonicaformdepartamentotde")}&telefonicaformnombreresponsabletde={parametros("telefonicaformnombreresponsabletde")}&telefonicaformtelefonoresponsabletde={parametros("telefonicaformtelefonoresponsabletde")}&telefonicaformprovincia={parametros("telefonicaformprovincia")}&telefonicaformunidadoperativa={parametros("telefonicaformunidadoperativa")}&comment={parametros("comment")}"

            ' Enviar la solicitud GET
            Dim response = Await _httpClient.GetAsync(uri)

            ' Procesar la respuesta
            If response.StatusCode = 401 Then
                respuesta.Success = True
                respuesta.Data = Await response.Content.ReadAsStringAsync()
            Else
                respuesta.msgError = "Ha ocurrido un error en el servicio de altaAtlas()"
            End If
        Catch ex As Exception
            respuesta.msgError = "Excepción: " & ex.Message
        End Try
        Return respuesta
    End Function

    Private Function GetParametrosAltaAtlas(usuario As Usuario, solicitudId As String) As Dictionary(Of String, String)
        Dim responsable As String = ""
        Dim telefonoResponsable As String = ""
        Dim comentario As String = ""
        Dim telefonicaformunidadoperativa As String = ""

        ' Obtener el código de la provincia usando el diccionario de configuración
        Dim provinciaCodigo As String = Configuracion.Provincias.FirstOrDefault(Function(p) p.Value = usuario.provincia).Key

        ' Configurar los parámetros en función del código de la provincia
        Select Case provinciaCodigo
            Case "05"
                responsable = "Francisco Javier Puyana Estrada"
                telefonoResponsable = "954483611"
                comentario = "Dar de alta en los buzones operativos CTAV0EFF"
                telefonicaformunidadoperativa = "CTAV0EFF"
            Case "06"
                responsable = "Jose Manuel Rodriguez Escalona"
                telefonoResponsable = "924211221"
                comentario = "Dar de alta en el buzon operativo CTBADEFF000"
                telefonicaformunidadoperativa = "CTBADEFF000"
                Exit Select
            Case "10"
                responsable = "Jose Manuel Rodriguez Escalona"
                telefonoResponsable = "924211221"
                comentario = "Dar de alta en el buzon operativo CTCCEFF"
                telefonicaformunidadoperativa = "CTCCEFF"
                Exit Select
            Case "11"
                responsable = "Francisco Jose de Asis Garcia Quiros"
                telefonoResponsable = "956241907"
                comentario = "Dar de alta en el buzon operativo CTCADEFF000"
                telefonicaformunidadoperativa = "CTCADEFF000"
                Exit Select
            Case "21"
                responsable = "Julio Garcia Quintana"
                telefonoResponsable = "959211191"
                comentario = "Dar de alta en el buzon operativo CTHLEFF000"
                telefonicaformunidadoperativa = "CTHLEFF000"
                Exit Select
            Case "23"
                responsable = "Santiago Bernabe Donaire Vico"
                telefonoResponsable = "953218550"
                comentario = "Dar de alta en el buzon operativo CTJAEEFF000"
                telefonicaformunidadoperativa = "CTJAEEFF000"
                Exit Select
            Case "24"
                responsable = "Francisco Javier Puyana Estrada"
                telefonoResponsable = "954483611"
                comentario = "Dar de alta en los buzones operativos CTLN0EFF"
                telefonicaformunidadoperativa = "CTLN0EFF"
                Exit Select
            Case "28"
                responsable = "Jose Maria Gonzalez"
                telefonoResponsable = "915844102"
                comentario = "Dar de alta en el buzon operativo MDCON009000"
                telefonicaformunidadoperativa = "MDCON009000"
                Exit Select
            Case "35"
                responsable = "Francisco Javier Puyana Estrada"
                telefonoResponsable = "954483611"
                comentario = "Dar de alta en los buzones operativos CTGCEEFF"
                telefonicaformunidadoperativa = "CTGCEEFF"
                Exit Select
            Case "37"
                responsable = "Francisco Javier Puyana Estrada"
                telefonoResponsable = "954483611"
                comentario = "Dar de alta en los buzones operativos CTSA0EFF"
                telefonicaformunidadoperativa = "CTSA0EFF"
                Exit Select
            Case "41"
                responsable = "Daniel Catalan Molina"
                telefonoResponsable = "954483424"
                comentario = "Dar de alta en el buzon operativo CTSEVEFF000"
                telefonicaformunidadoperativa = "CTSEVEFF000"
                Exit Select
            Case "49"
                responsable = "Francisco Javier Puyana Estrada"
                telefonoResponsable = "954483611"
                comentario = "Dar de alta en los buzones operativos CTZM0EFF"
                telefonicaformunidadoperativa = "CTZM0EFF"
                Exit Select
        End Select

        Return New Dictionary(Of String, String) From {
            {"rid", solicitudId},
            {"telefonicaformdepartamentotde", "I mas M Provision"},
            {"telefonicaformnombreresponsabletde", responsable},
            {"telefonicaformtelefonoresponsabletde", telefonoResponsable},
            {"telefonicaformprovincia", usuario.provincia},
            {"comment", comentario},
            {"telefonicaformunidadoperativa", telefonicaformunidadoperativa}
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
