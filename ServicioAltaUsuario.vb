﻿Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading.Tasks

Public Class ServicioAltaUsuario

    Public Class RespuestaAlta
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
        Dim byteArray = Encoding.ASCII.GetBytes(Configuracion.UserPass)
        _httpClient.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray))
    End Sub

    Public Async Function AltaUsuario(usuario As Usuario) As Task(Of RespuestaAlta)
        Dim respuestaAlta As New RespuestaAlta()
        Try
            ' Patrón para validar el DNI (8 dígitos seguido de una letra)
            Dim patronDocumentoDNI As String = "^\d{8}[a-zA-Z]$"
            ' Patrón para validar el NIE (una letra, 7 dígitos y una letra)
            Dim patronDocumentoOtros As String = "^[a-zA-Z]\d{7}[a-zA-Z]$"

            If Regex.IsMatch(usuario.telefonicadocumento, patronDocumentoDNI) Then
                usuario.telefonicatipodocumento = "D"
                usuario.telefonicanacionalidad = "ES"
            ElseIf Regex.IsMatch(usuario.telefonicadocumento, patronDocumentoOtros) Then
                usuario.telefonicatipodocumento = "O"
                usuario.telefonicanacionalidad = "EC"
            Else
                ' Manejar casos donde el documento no coincide con ninguno de los patrones
                usuario.telefonicatipodocumento = ""
                usuario.telefonicanacionalidad = ""
                respuestaAlta.msgError = "Revise el NIF/NIE"

            End If

            ' Construir los parámetros de la solicitud
            Dim parametros As New Dictionary(Of String, String) From {
                {"id", usuario.eagora},
                {"type", "0"},
                {"subid", ""},
                {"telefonicacodigocontrata", ""},
                {"object", "usuario"},
                {"subobject", ""},
                {"checked", "true"},
                {"uid", usuario.eagora},
                {"userPassword", usuario.userpassword},
                {"telefonicafechaexpiracion", "01/01/3000"},
                {"givenName", usuario.givenName},
                {"sn", usuario.sn},
                {"telefonicanacionalidad", usuario.telefonicanacionalidad},
                {"tiposel", usuario.telefonicatipodocumento},
                {"telefonicatipodocumento", usuario.telefonicatipodocumento},
                {"telefonicadocumento", usuario.telefonicadocumento},
                {"telefonicaempresa", "1481718674094"},
                {"toplevelorganization", "1481718674094"},
                {"telefonicadescripcionempresa", "EFF-EXCELLENCE FIELD FACTORY S.L.U. 009"},
                {"telefonicaidempleado", ""},
                {"telephoneNumber", usuario.telephoneNumber},
                {"mobile", usuario.telephoneNumber},
                {"facsimileTelephoneNumber", ""},
                {"mail", usuario.email},
                {"postalAddress", ""},
                {"postalCode", ""},
                {"l", ""},
                {"st", ""},
                {"c", "ES"}
            }

            ' Convertir los parámetros a contenido URL-encoded
            Dim content As New FormUrlEncodedContent(parametros)

            ' Enviar la solicitud POST
            Dim response = Await _httpClient.PostAsync("qj/servlet/alta", content)

            ' Procesar la respuesta
            If response.IsSuccessStatusCode Then
                Dim responseData = Await response.Content.ReadAsStringAsync()
                respuestaAlta.Data = responseData
                If responseData.Contains("El usuario ha sido dado de alta correctamente") Then
                    respuestaAlta.Success = True
                ElseIf responseData.Contains("Ya existe un usuario con el mismo numero de documento en la empresa") Then
                    respuestaAlta.msgError = "Ya existe un usuario con el mismo numero de documento en la empresa"
                ElseIf responseData.Contains("No esta autorizado a acceder a la funcionalidad solicitada") Then
                    respuestaAlta.msgError = "No esta autorizado a acceder a la funcionalidad solicitada"
                ElseIf responseData.Contains("El identificador ya esta en uso") Then
                    respuestaAlta.msgError = "El identificador ya esta en uso"
                Else
                    respuestaAlta.msgError = "Error desconocido: " & responseData
                End If
            Else
                respuestaAlta.msgError = "Error en la solicitud HTTP: " & response.ReasonPhrase
            End If
            Return respuestaAlta
        Catch ex As Exception
            respuestaAlta.msgError = "Excepción: " & ex.Message
            Return respuestaAlta
        End Try

    End Function

End Class



