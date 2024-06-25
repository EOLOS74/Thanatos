Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Text
Imports System.Text.Json
Imports System.Threading.Tasks
Imports Thanatos.ServicioAltaPF

Public Class ServicioBajaTotal
    Private ReadOnly _servicioNavegador As ServicioNavegador

    Public Class RespuestaBaja
        Public Property success As Boolean
        Public Property data As String
        Public Property msgError As String
    End Class
    Private ReadOnly _httpClient As HttpClient

    Public Sub New()
        _httpClient = New HttpClient()
        _httpClient.BaseAddress = New Uri("https://eagora.telefonica.es/")
        Dim byteArray = Encoding.ASCII.GetBytes(Configuracion.UserPass)
        _httpClient.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray))
        _servicioNavegador = New ServicioNavegador()
    End Sub

    Public Async Function DarBajaTotal(usuario As Usuario) As Task(Of RespuestaBaja)
        Dim respuestaBaja As New RespuestaBaja()

        Try
            ' Dar de baja en eAgora
            Dim resultadoBajaEagora = Await DarBajaEagora(usuario)
            If resultadoBajaEagora.success Then
                respuestaBaja.success = True
                respuestaBaja.data = resultadoBajaEagora.data

                Dim jsonDataPost = _servicioNavegador.GetJsonDataPostApi("resourceupdate")
                jsonDataPost.Data = New With {
                    .id_resource = usuario.id_resource,
                    .id_estado_sga = "6"
                }

                ' Asegurarse de que los encabezados están presentes
                jsonDataPost.Headers = _servicioNavegador.GetJsonDataPostApi("resourceupdate").Headers

                Dim resultado = Await _servicioNavegador.SetPostAsync(jsonDataPost)
                respuestaBaja = JsonSerializer.Deserialize(Of RespuestaBaja)(resultado)
                Return respuestaBaja
            Else
                respuestaBaja.success = False
                respuestaBaja.msgError = resultadoBajaEagora.msgError
                Return respuestaBaja
            End If

        Catch ex As Exception
            respuestaBaja.success = False
            respuestaBaja.msgError = "Excepción: " & ex.Message
        End Try

        Return respuestaBaja
    End Function

    Private Async Function DarBajaEagora(usuario As Usuario) As Task(Of RespuestaBaja)
        Dim respuesta As New RespuestaBaja()

        Try
            Dim url As String = $"/qj/servlet/baja?object=usuario&id={usuario.eagora}"
            Dim response = Await _httpClient.GetAsync(url)
            'Dim responseData = Await response.Content.ReadAsStringAsync()

            If response.StatusCode = 401 Then
                respuesta.success = True
                respuesta.data = "Baja en eAgora realizada correctamente."
            Else
                respuesta.success = False
                respuesta.msgError = "Error al dar de baja en eAgora"
            End If
        Catch ex As Exception
            respuesta.success = False
            respuesta.msgError = "Excepción: " & ex.Message
        End Try

        Return respuesta
    End Function

End Class
