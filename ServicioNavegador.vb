Imports System.Net.Http
Imports System.Text
Imports System.Text.Json
Imports Thanatos.Thanatos

Public Class ServicioNavegador

    Private ReadOnly client As New HttpClient()
    Private ReadOnly headers As New Dictionary(Of String, String)() From {
        {"agenda_api_key", "c1fe4e00-d25b-4d8d-a58c-475bad472a91"}
    }
    Private ReadOnly apiUrl As String = "https://api-agenda.atelcosoluciones.es/agendashj/"

    Public Sub New()
        ' Agregar la key de la API a los encabezados predeterminados
        For Each header In headers
            client.DefaultRequestHeaders.Add(header.Key, header.Value)
        Next
    End Sub

    Public Async Function SetPostAsync(jsonDataPost As JsonDataPost) As Task(Of String)
        Dim respuesta As String = String.Empty
        Try
            Dim jsonData As String = JsonSerializer.Serialize(jsonDataPost.Data)
            Dim content As New StringContent(jsonData, Encoding.UTF8, "application/json")

            Dim response = Await client.PostAsync(jsonDataPost.Url, content)

            If response.IsSuccessStatusCode Then
                respuesta = Await response.Content.ReadAsStringAsync()
            Else
                respuesta = $"Error: {response.ReasonPhrase}"
            End If
        Catch ex As Exception
            respuesta = $"Exception: {ex.Message}"
        End Try

        Return respuesta
    End Function

    Public Function GetJsonDataPostApi(servicio As String) As JsonDataPost
        Return New JsonDataPost() With {
            .Url = apiUrl & servicio,
            .Data = Nothing,
            .Headers = headers
        }
    End Function

    Public Async Function LeerTelcosPendientes(invertirTelco As Boolean) As Task(Of String)
        Dim jsonDataPost As JsonDataPost = GetJsonDataPostApi("getAltasTelcoPendientes")
        jsonDataPost.Data = New Dictionary(Of String, Object) From {{"invertirlista", invertirTelco}}
        Return Await SetPostAsync(jsonDataPost)
    End Function

    Public Async Function LeerBajasPendientes(invertirBajas As Boolean) As Task(Of String)
        Dim jsonDataPost As JsonDataPost = GetJsonDataPostApi("getBajasPendientes")
        jsonDataPost.Data = New Dictionary(Of String, Object) From {{"invertirlista", invertirBajas}}
        Return Await SetPostAsync(jsonDataPost)
    End Function

    Public Async Function LeerColiseoPendientes(invertirColiseo As Boolean) As Task(Of String)
        Dim jsonDataPost As JsonDataPost = GetJsonDataPostApi("getAltasColiseoPendientes")
        jsonDataPost.Data = New Dictionary(Of String, Object) From {{"invertirlista", invertirColiseo}}
        Return Await SetPostAsync(jsonDataPost)
    End Function

    Public Async Function LeerAltasPendientes(invertirAltas As Boolean) As Task(Of String)
        Dim jsonDataPost As JsonDataPost = GetJsonDataPostApi("getAltasPendientes")
        jsonDataPost.Data = New Dictionary(Of String, Object) From {{"invertirlista", invertirAltas}}
        Return Await SetPostAsync(jsonDataPost)
    End Function
    'Public Async Function ActualizarAgendaTelco(usuario As Usuario) As Task(Of ApiResponse)
    '    Dim respuesta As New ApiResponse()
    '    Try
    '        Dim jsonDataPost = GetJsonDataPostApi("resourceupdate")

    '        Dim data = New Dictionary(Of String, Object) From {
    '            {"id_resource", usuario.id_resource},
    '            {"pindi", usuario.pindi},
    '            {"id_estado_sga", usuario.estadosgadefecto}
    '        }

    '        jsonDataPost.Data = data

    '        Dim resultado = Await SetPostAsync(jsonDataPost)
    '        Dim respuestaApi = JsonSerializer.Deserialize(Of ApiResponse)(resultado)

    '        If respuestaApi.success Then
    '            respuesta.success = True
    '            respuesta.data = respuestaApi.data
    '        Else
    '            respuesta.success = False
    '            respuesta.msgError = respuestaApi.msgError
    '        End If
    '    Catch ex As Exception
    '        respuesta.success = False
    '        respuesta.msgError = "Excepción: " & ex.Message
    '    End Try
    'End Function
    Public Async Function ActualizarAgendaUsuario(usuario As Usuario) As Task(Of ApiResponse)
        Dim respuesta As New ApiResponse()
        Try
            Dim jsonDataPost = GetJsonDataPostApi("resourceupdate")

            Dim data = New Dictionary(Of String, Object) From {
                {"id_resource", usuario.id_resource},
                {"eagora", usuario.eagora},
                {"sga", usuario.sga},
                {"alias", usuario.id},
                {"telefono", usuario.telephoneNumber},
                {"id_estado_sga", usuario.estadosgadefecto},
                {"coliseo", usuario.coliseo}
            }

            If usuario.empresa <> "ATELCO FIELD FACTORY, S.L." Then
                data.Add("emailExternos", usuario.email)
            End If

            jsonDataPost.Data = data

            Dim resultado = Await SetPostAsync(jsonDataPost)
            Dim respuestaApi = JsonSerializer.Deserialize(Of ApiResponse)(resultado)

            If respuestaApi.success Then
                respuesta.success = True
                respuesta.data = respuestaApi.data
            Else
                respuesta.success = False
                respuesta.msgError = respuestaApi.msgError
            End If
        Catch ex As Exception
            respuesta.success = False
            respuesta.msgError = "Excepción: " & ex.Message
        End Try

        Return respuesta
    End Function

    Public Class ApiResponse
        Public Property success As Boolean
        Public Property data As String
        Public Property msgError As String
    End Class
End Class

Public Class JsonDataPost
    Public Property Url As String
    Public Property Data As Object
    Public Property Headers As IDictionary(Of String, String)

    Public Sub New()
        Headers = New Dictionary(Of String, String)()
    End Sub
End Class
