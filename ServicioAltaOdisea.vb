Imports System.Net
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Text
Imports System.Threading.Tasks


Public Class ServicioAltaOdisea

    Public Class RespuestaOdisea
        Public Property Success As Boolean
        Public Property msgError As String
        Public Property Data As String
    End Class

    Private ReadOnly _httpClient As HttpClient

    Public Sub New()
        _httpClient = New HttpClient()
        _httpClient.BaseAddress = New Uri("https://eagora.telefonica.es/")
        Dim byteArray = Encoding.ASCII.GetBytes(Configuracion.UserPass)
        _httpClient.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray))
    End Sub

    Public Async Function AltaUsuarioOdisea(usuario As Usuario) As Task(Of RespuestaOdisea)
        Dim respuesta As New RespuestaOdisea()

        ' Paso 0: Navegar a Odisea
        Dim response = Await _httpClient.GetAsync($"/od/servlet/Controler")

        ' Pausa de 2 segundo para la carga del casco
        Await Task.Delay(2000)


        ' Paso 1: Alta en Odisea
        Dim parametrosUrl = GetParametrosUrl(usuario)
        response = Await _httpClient.GetAsync($"/od/servlet/Controler{parametrosUrl}")
        If response.IsSuccessStatusCode Then
            Dim responseData = Await response.Content.ReadAsStringAsync()
            If responseData.Contains("alt='Correcto'") Then
                respuesta.Success = True
            Else
                respuesta.Success = False
                respuesta.msgError = "Error en el alta de Odisea: " & responseData
                Return respuesta
            End If
        Else
            respuesta.Success = False
            respuesta.msgError = "Error en la petición de alta de Odisea"
            Return respuesta
        End If

        ' Paso 2: Alta en XML
        Dim xmlPost = GetXmlAltaUsuarioOdisea(usuario)
        Dim parametrosXml = New Dictionary(Of String, String) From {
            {"accion", "373"},
            {"validar", "true"},
            {"servicio", "ServicioTecnicos"},
            {"xml", xmlPost}
        }
        response = Await _httpClient.PostAsync("/od/servlet/ControlerEjecutor", New FormUrlEncodedContent(parametrosXml))
        If response.IsSuccessStatusCode Then
            Dim responseData = Await response.Content.ReadAsStringAsync()
            If responseData.Contains("Nivela t") Then
                respuesta.Success = True
            Else
                respuesta.Success = False
                respuesta.msgError = "Error en el alta XML: " & responseData
                Return respuesta
            End If
        Else
            respuesta.Success = False
            respuesta.msgError = "Error en la petición de alta XML"
            Return respuesta
        End If

        ' Paso 3: Configurar Usuario en Odisea
        parametrosUrl = GetParametrosConfiguracion(usuario)
        response = Await _httpClient.GetAsync($"/od/servlet/Controler{parametrosUrl}")
        If response.IsSuccessStatusCode Then
            Dim responseData = Await response.Content.ReadAsStringAsync()
            If responseData.Contains("Activando usuario-perfil") OrElse responseData.Contains("Alguna de las matrículas de sistema origen") Then
                respuesta.Success = True
            Else
                respuesta.Success = False
                respuesta.msgError = "Error en la configuración de usuario: " & responseData
                Return respuesta
            End If
        Else
            respuesta.Success = False
            respuesta.msgError = "Error en la petición de configuración de usuario"
            Return respuesta
        End If

        Return respuesta
    End Function

    Private Function GetParametrosUrl(usuario As Usuario) As String
        Dim codigoProvincia = ObtenerCodigoProvincia(usuario.provincia)

        Dim parametros = New Dictionary(Of String, String) From {
            {"modoError", "error"},
            {"accion", "615"},
            {"origen", "menu"},
            {"urlActual", "/od/servlet/Controler?accion=613"},
            {"numMatriculas", "4"},
            {"canalTemp", "EMPRESAS COLABORADORAS"},
            {"canalCodTemp", "EX002ECG%2CIM002ECN"},
            {"canalActuacion", "EX002ECG%2CIM002ECN"},
            {"login", usuario.eagora},
            {"alias", usuario.id},
            {"movil", usuario.telephoneNumber},
            {"usuario", usuario.eagora},
            {"idioma", "es"},
            {"zonaHoraria", "1"},
            {"asocProvincia", codigoProvincia},
            {"provincia", codigoProvincia},
            {"asocEecc", "009"},
            {"eecc", "009"},
            {"perfil", If(usuario.perfil = "Tecnico", "99", If(usuario.perfil = "Despachador", "99,125", "99,102"))},
            {"activo", If(usuario.perfil = "Tecnico", "99", If(usuario.perfil = "Despachador", "125", "102"))},
            {"sistema1", "UI"},
            {"matricula1", usuario.eagora},
            {"sistema2", "A0"},
            {"matricula2", usuario.sga},
            {"sistema3", "M1"},
            {"matricula3", usuario.sga},
            {"sistema4", "MA"},
            {"matricula4", usuario.sga},
            {"tec_eecc", "009"},
            {"numPropiedades", "0"}
        }
        Return "?" & String.Join("&", parametros.Select(Function(kv) $"{kv.Key}={kv.Value}"))
    End Function

    Private Function GetXmlAltaUsuarioOdisea(usuario As Usuario) As String
        Return $"<?xml version=""1.0"" encoding=""ISO-8859-1""?><SERVICIO_XML_ODISEA><CABECERA><ID_SERVICIO nom_servicio=""ServicioTecnicos"" version_servicio=""100.001""/><ID_LLAMANTE nombre=""X009EAR""/></CABECERA><MENSAJE><SOLICITUD numero=""1""><PARAMETRO nombre=""MT"" valor=""0{usuario.sga}""/><PARAMETRO nombre=""EC"" valor=""009""/><PARAMETRO nombre=""MV"" valor=""{usuario.telephoneNumber}""/><PARAMETRO nombre=""MC"" valor=""12333""/><PARAMETRO nombre=""EP"" valor=""1""/><PARAMETRO nombre=""PR"" valor=""15""/><PARAMETRO nombre=""PP"" valor=""25""/><PARAMETRO nombre=""DG"" valor=""T000000""/><PARAMETRO nombre=""AL"" valor=""2006-01-20""/><PARAMETRO nombre=""BA"" valor=""2727-01-01""/><PARAMETRO nombre=""MO"" valor=""2007-10-20 12:59:10""/></SOLICITUD></MENSAJE></SERVICIO_XML_ODISEA>"
    End Function

    Private Function GetParametrosConfiguracion(usuario As Usuario) As String
        Dim codigoProvincia = ObtenerCodigoProvincia(usuario.provincia)

        Dim parametros = New Dictionary(Of String, String) From {
            {"modo", "36"},
            {"accion", "614"},
            {"origen", "menuGrupo"},
            {"numMatriculas", "4"},
            {"canalCodTemp", "EX002ECG%2CIM002ECN"},
            {"canalActuacion", "EX002ECG%2CIM002ECN"},
            {"bajaLogica", "N"},
            {"login", usuario.eagora},
            {"alias", usuario.id},
            {"movil", usuario.telephoneNumber},
            {"usuario", usuario.eagora},
            {"idioma", "es"},
            {"zonaHoraria", "1"},
            {"asocProvincia", codigoProvincia},
            {"provincia", codigoProvincia},
            {"asocEecc", "009"},
            {"eecc", "009"},
            {"perfil", If(usuario.perfil = "Tecnico", "99", If(usuario.perfil = "Despachador", "99,125", "99,102"))},
            {"activo", If(usuario.perfil = "Tecnico", "99", If(usuario.perfil = "Despachador", "125", "102"))},
            {"sistema1", "A0"},
            {"matricula1", usuario.sga},
            {"sistema2", "M1"},
            {"matricula2", usuario.sga},
            {"sistema3", "MA"},
            {"matricula3", usuario.sga},
            {"sistema4", "UI"},
            {"matricula4", usuario.eagora},
            {"matricula", "0" & usuario.sga},
            {"tec_matricula", "0" & usuario.sga},
            {"tec_eecc", "009"},
            {"tec_movil", usuario.telephoneNumber},
            {"tec_preasignable", "99"},
            {"tec_preasignableProv", "99"},
            {"planificable", "on"},
            {"tec_planificable", "1"},
            {"numPropiedades", "6"},
            {"codPropiedad1", "50"},
            {"dsPropiedad1", "Avisos Activos"},
            {"valorPropiedad1", "1"},
            {"dsvalorPropiedad1", "Activa"},
            {"codPropiedad2", "53"},
            {"dsPropiedad2", "Envío SMS en Asign./Preasign."},
            {"valorPropiedad2", "3"},
            {"dsvalorPropiedad2", "Simpre SMS"},
            {"codPropiedad3", "54"},
            {"dsPropiedad3", "Iconos Odise@Mov"},
            {"valorPropiedad3", "0"},
            {"dsvalorPropiedad3", "NO MOSTRAR"},
            {"codPropiedad4", "55"},
            {"dsPropiedad4", "Protocolo Odise@Mov"},
            {"valorPropiedad4", "1"},
            {"dsvalorPropiedad4", "SEGUN SOPORTE DISPOSITIVO"},
            {"codPropiedad5", "74"},
            {"dsPropiedad5", "Tipo Impresora"},
            {"valorPropiedad5", "2"},
            {"dsvalorPropiedad5", "ITOS"},
            {"codPropiedad6", "75"},
            {"dsPropiedad6", "Captura Firma"},
            {"valorPropiedad6", "3"},
            {"dsvalorPropiedad6", "Tableta Digitalizadora PSP-15"}
        }
        Return "?" & String.Join("&", parametros.Select(Function(kv) $"{kv.Key}={kv.Value}"))

    End Function
    Private Function ObtenerCodigoProvincia(nombreProvincia As String) As String
        Dim codigoProvincia As String = Configuracion.Provincias.FirstOrDefault(Function(p) p.Value.Equals(nombreProvincia, StringComparison.OrdinalIgnoreCase)).Key
        Return If(String.IsNullOrEmpty(codigoProvincia), nombreProvincia, codigoProvincia)
    End Function
End Class