Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Text
Imports System.Threading.Tasks

Public Class ServicioMovilPin
    Private ReadOnly _httpClient As HttpClient

    Public Sub New()
        _httpClient = New HttpClient()
        ' Configura la base address si tienes una URL base común para todas las solicitudes
        _httpClient.BaseAddress = New Uri("https://eagora.telefonica.es/")

        ' Configurar las credenciales de autenticación básica
        Dim byteArray = Encoding.ASCII.GetBytes("x009ear:026telco")
        _httpClient.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray))
    End Sub

    Public Async Function ConfigurarMovilPin(usuario As Usuario) As Task(Of ApiResponse1)
        Dim respuesta As New ApiResponse1()
        Try
            ' Calcular la fecha de expiración
            Dim fecha As Date = Date.Now
            Dim telefonicafechaexpiracion As String = $"{AddZero(fecha.Day)}/{AddZero(fecha.Month)}/{fecha.Year + 1}"

            ' Construir los parámetros de la solicitud
            Dim parametros As New Dictionary(Of String, String) From {
                {"id", usuario.eagora},
                {"telefonicafechaexpiracion", telefonicafechaexpiracion},
                {"tesawappin", "1111"},
                {"tesawapmobile", "34" & usuario.telephoneNumber}
            }

            ' Convertir los parámetros a contenido URL-encoded
            Dim content As New FormUrlEncodedContent(parametros)

            ' Enviar la solicitud POST
            Dim response = Await _httpClient.PostAsync("qj/gu/admin/2.1/gui/jsp/usr/acc/mod.jsp", content)

            ' Procesar la respuesta
            If response.IsSuccessStatusCode Then
                Dim responseData = Await response.Content.ReadAsStringAsync()
                respuesta.Data = responseData
                respuesta.Success = True
                Console.WriteLine("Configurado movil y PIN")
            Else
                respuesta.msgError = "Error en la solicitud HTTP: " & response.ReasonPhrase
            End If
        Catch ex As Exception
            respuesta.msgError = "Excepción: " & ex.Message
        End Try
        Return respuesta
    End Function

    Private Function AddZero(dato As Integer) As String
        Return If(dato.ToString().Length = 1, "0" & dato.ToString(), dato.ToString())
    End Function
End Class

Public Class ApiResponse1
    Public Property Success As Boolean
    Public Property msgError As String
    Public Property Data As String
End Class
