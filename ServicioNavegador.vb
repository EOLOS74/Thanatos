Imports System.Net.Http
Imports System.Text
Imports System.Text.Json

Public Class ServicioNavegador
    Private ReadOnly client As New HttpClient()

    Private apiKey As String = "c1fe4e00-d25b-4d8d-a58c-475bad472a91"
    Public Sub New()
        ' Agregar la clave de la API a los encabezados predeterminados
        client.DefaultRequestHeaders.Add("agenda_api_key", apiKey)
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
        Dim apiUrl As String = "https://api-agenda.atelcosoluciones.es/agendashj/" & servicio
        Dim apiKey As String = "c1fe4e00-d25b-4d8d-a58c-475bad472a91"

        Dim jsonDataPost As New JsonDataPost() With {
            .Url = apiUrl,
            .Data = Nothing
        }

        ' Agregar la clave de la API a los encabezados
        Dim headers As New Dictionary(Of String, String)()
        headers.Add("agenda_api_key", apiKey)

        jsonDataPost.Headers = headers

        Return jsonDataPost
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

End Class

Public Class JsonDataPost
    Public Property Url As String
    Public Property Data As Object
    Public Property Headers As IDictionary(Of String, String)

    Public Sub New()
        Headers = New Dictionary(Of String, String)()
    End Sub
End Class
