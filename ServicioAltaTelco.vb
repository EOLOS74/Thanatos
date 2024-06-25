Imports System.Net.Http
Imports System.Text.Json
Imports System.Threading.Tasks

Public Class ServicioAltaTelco
    Private ReadOnly _servicioNavegador As ServicioNavegador

    Public Sub New()
        _servicioNavegador = New ServicioNavegador()
    End Sub

    Public Class ApiResponse
        Public Property success As Boolean
        Public Property data As String
        Public Property msgError As String
    End Class

    Public Async Function DarAltaTelco(usuario As Usuario) As Task(Of Boolean)
        Dim respuesta As Boolean = False

        If Not String.IsNullOrEmpty(usuario.pindi) Then
            ' Caso 1: Con PINDI
            Dim confirmacion As DialogResult = MessageBox.Show($"Se va a grabar el PINDI: {usuario.pindi}. ¿Desea continuar?", "Confirmación", MessageBoxButtons.YesNo)
            If confirmacion = DialogResult.Yes Then
                Dim jsonDataPost = _servicioNavegador.GetJsonDataPostApi("resourceupdate")
                jsonDataPost.Data = New With {
                    .id_resource = usuario.id_resource,
                    .pindi = usuario.pindi,
                    .id_estado_sga = If(usuario.solicitudtelefonica = "true", "3", usuario.estadosgadefecto)
                }

                ' Asegurarse de que los encabezados están presentes
                jsonDataPost.Headers = _servicioNavegador.GetJsonDataPostApi("resourceupdate").Headers

                Dim resultado = Await _servicioNavegador.SetPostAsync(jsonDataPost)
                Dim respuestaApi = JsonSerializer.Deserialize(Of ApiResponse)(resultado)

                If respuestaApi.success Then
                    respuesta = True
                    ' Lógica para enviar el correo de notificación del alta de TELCO
                Else
                    MessageBox.Show($"Error: {respuestaApi.msgError}")
                End If
            End If
        Else
            ' Caso 2: Sin PINDI
            Dim confirmacion As DialogResult = MessageBox.Show("El campo PINDI está vacío. ¿Quiere eliminar la solicitud de TELCO existente?", "Confirmación", MessageBoxButtons.YesNo)
            If confirmacion = DialogResult.Yes Then
                Dim jsonDataPost = _servicioNavegador.GetJsonDataPostApi("resourceupdate")
                jsonDataPost.Data = New With {
                    .id_resource = usuario.id_resource,
                    .id_estado_sga = If(usuario.solicitudtelefonica = "true", "3", usuario.estadosgadefecto)
                }

                ' Asegurarse de que los encabezados están presentes
                jsonDataPost.Headers = _servicioNavegador.GetJsonDataPostApi("resourceupdate").Headers

                Dim resultado = Await _servicioNavegador.SetPostAsync(jsonDataPost)
                Dim respuestaApi = JsonSerializer.Deserialize(Of ApiResponse)(resultado)

                If respuestaApi.success Then
                    respuesta = True
                    ' Lógica para enviar el correo de cancelación del alta de TELCO
                Else
                    MessageBox.Show($"Error: {respuestaApi.msgError}")
                End If
            End If
        End If

        Return respuesta
    End Function
End Class
