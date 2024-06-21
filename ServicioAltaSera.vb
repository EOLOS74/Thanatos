Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Text
Imports System.Threading.Tasks

Public Class ServicioAltaSera
    Public Class RespuestaSera
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
        Dim byteArray = Encoding.ASCII.GetBytes("x009ear:026telco")
        _httpClient.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray))

    End Sub

    Public Async Function AltaSera(usuario As Usuario) As Task(Of RespuestaSera)
        Dim respuesta As New RespuestaSera()
        Try

            Dim parametros As String = GetParametrosAltaSera(usuario)
            Dim uri As String = $"/qj/gu/admin/3.0/gui/jsp/users/roles/modifyRoles.jsp{parametros}"

            ' Enviar la solicitud GET
            Dim response = Await _httpClient.GetAsync(uri)

            ' Procesar la respuesta
            If response.IsSuccessStatusCode Then
                Dim responseData = Await response.Content.ReadAsStringAsync()
                respuesta.Data = responseData
                If responseData.Contains("SERA - SERA") Then
                    respuesta.Success = True
                    Console.WriteLine("Alta en Sera correcta")
                Else
                    respuesta.msgError = "Ha ocurrido un error en el servicio de altaSera()"
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

    Private Function GetParametrosAltaSera(usuario As Usuario) As String
        Dim provincia As String = ""

        Select Case usuario.provincia
            Case "TODAS"
                provincia = "A:ABENTEL"
            Case "03"
                provincia = "A:ALICANTE"
            Case "05"
                provincia = "A:AVILA"
            Case "06"
                provincia = "A:BADAJOZ"
            Case "08"
                provincia = "A:BARCELONA"
            Case "10"
                provincia = "A:CACERES"
            Case "11", "51"
                provincia = "A:CADIZ1,A:CEUTA"
            Case "21"
                provincia = "A:HUELVA,A:ABENTEL"
            Case "23"
                provincia = "A:JAEN1"
            Case "24"
                provincia = "A:LEON"
            Case "28"
                provincia = "A:MADRID"
            Case "35"
                provincia = "A:LAS PALMAS"
            Case "37"
                provincia = "A:SALAMANCA"
            Case "38"
                provincia = "A:SANTA CRUZ DE TENERIFE"
            Case "41"
                provincia = "A:SEVILLA1"
            Case "46"
                provincia = "A:VALENCIA"
            Case "49"
                provincia = "A:ZAMORA"
        End Select

        Dim parametros As String = $"?id={usuario.eagora}&subid=sera&state=0&telefonicaperfilaut={provincia},OPERADOR"
        Return parametros
    End Function
End Class

