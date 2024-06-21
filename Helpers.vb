Imports System.Text.Json
Imports System.Globalization

Public Class JsonHelper
    Public Shared Function ObtenerValorDesdeJson(json As String, campo As String) As String
        Try
            Dim respuestaObj As JsonDocument = JsonDocument.Parse(json)

            ' Obtener el valor del campo si existe en el único elemento del array "data"
            Dim dataElement As JsonElement = respuestaObj.RootElement.GetProperty("data").EnumerateArray().FirstOrDefault()
            If Not dataElement.Equals(Nothing) Then
                Dim campoElement As JsonElement
                If dataElement.TryGetProperty(campo, campoElement) Then
                    ' Convertir a cadena para todos los tipos de datos
                    Return Convert.ToString(campoElement.ToString())
                Else
                    Return $"El campo '{campo}' no está presente en la respuesta JSON."
                End If
            Else
                Return "No se encontró el elemento 'data' en la respuesta JSON."
            End If
        Catch ex As JsonException
            ' Manejar errores de parsing del JSON
            Return $"Error al parsear el JSON: {ex.Message}"
        End Try
    End Function
    Public Shared Function EsDataVacio(json As String) As Boolean
        Try
            Dim respuestaObj As JsonDocument = JsonDocument.Parse(json)
            Dim dataArray As JsonElement = respuestaObj.RootElement.GetProperty("data")
            Return dataArray.GetArrayLength() = 0
        Catch ex As JsonException
            ' Manejar errores de parsing del JSON
            Return False
        End Try
    End Function
End Class



Public Class PasswordHelper
    Public Shared Function GenerarPasswordFechaActual() As String
        Dim fechaActual As DateTime = DateTime.Now
        Dim dia As String = fechaActual.ToString("dd")
        Dim mes As String = ObtenerMesEnLetras(fechaActual.Month)
        Dim anio As String = fechaActual.ToString("yyyy")
        Return $"{dia}{mes}{anio}"
    End Function

    Private Shared Function ObtenerMesEnLetras(mes As Integer) As String
        Select Case mes
            Case 1
                Return "ene"
            Case 2
                Return "feb"
            Case 3
                Return "mar"
            Case 4
                Return "abr"
            Case 5
                Return "may"
            Case 6
                Return "jun"
            Case 7
                Return "jul"
            Case 8
                Return "ago"
            Case 9
                Return "sep"
            Case 10
                Return "oct"
            Case 11
                Return "nov"
            Case 12
                Return "dic"
            Case Else
                Return ""
        End Select
    End Function


End Class




