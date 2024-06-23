Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Text
Imports System.Threading.Tasks

Public Class ServicioAltaVisord

    Public Class RespuestaVisord
        Public Property Success As Boolean
        Public Property MensajeError As String
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

    Public Async Function AltaVisord(usuario As Usuario) As Task(Of RespuestaVisord)
        Dim respuesta As New RespuestaVisord()
        Dim uri = GetParametrosVisordGet(usuario)

        Dim response = Await _httpClient.GetAsync("/qj/gu/admin/3.0/gui/jsp/users/roles/modifyRoles.jsp" & uri)

        If response.IsSuccessStatusCode Then
            Dim responseData = Await response.Content.ReadAsStringAsync()
            If responseData.Contains("MA1 - Web EE.CC. VISORD") Then
                respuesta.Success = True
                respuesta.Data = responseData
            Else
                respuesta.MensajeError = "Error al dar de alta el perfil de Visord"
                respuesta.Data = responseData
            End If
        Else
            respuesta.MensajeError = "Ha ocurrido un error en el servicio de altaVisord(): " & response.ReasonPhrase
        End If

        Return respuesta
    End Function

    Private Function GetParametrosVisordGet(usuario As Usuario) As String

        Dim codigoProvincia = ObtenerCodigoProvincia(usuario.provincia)
        Dim perfil = "&telefonicaperfilaut=PROV_" & codigoProvincia

        Dim parametros As String = $"?id={usuario.eagora}&subid=ma1&state=0&telefonicaperfilaut=CONT_1&telefonicaperfilaut=TIPO_C&telefonicaperfilaut=creacionred_tecnicoEECC&telefonicaperfilaut=provision_tecnico&telefonicaperfilaut=reclamaciones_tecnico_ec{perfil}"

        Return parametros
    End Function

    Private Function GetUrlToParametros(parametros As Dictionary(Of String, String)) As String
        Dim sb As New StringBuilder()
        For Each kvp As KeyValuePair(Of String, String) In parametros
            If sb.Length > 0 Then
                sb.Append("&")
            End If
            sb.Append($"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}")
        Next
        Return "?" & sb.ToString()
    End Function

    Private Function ObtenerCodigoProvincia(nombreProvincia As String) As String
        Dim codigoProvincia As String = Configuracion.Provincias.FirstOrDefault(Function(p) p.Value.Equals(nombreProvincia, StringComparison.OrdinalIgnoreCase)).Key
        Return If(String.IsNullOrEmpty(codigoProvincia), nombreProvincia, codigoProvincia)
    End Function
End Class
