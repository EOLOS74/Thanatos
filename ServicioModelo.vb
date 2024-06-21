Public Class ServicioModelo
    Private Shared _usuario As Usuario

    Public Shared Property Usuario As Usuario
        Get
            Return _usuario
        End Get
        Set(value As Usuario)
            _usuario = value
        End Set
    End Property
End Class
