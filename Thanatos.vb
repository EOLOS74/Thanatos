Imports System.Text.Json
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Text




Public Class Thanatos
    Private servicioNavegador As New ServicioNavegador()
    Private navigateForPasswordChange As Boolean = False
    Private logService As LogService
    Private uiBlockService As UIBlockService
    Private pictureBoxList As List(Of PictureBox)
    Private textBoxList As List(Of TextBox)
    Private alto As Integer
    Private ancho As Integer


    ' Estructura para almacenar las listas y contadores
    Private Solicitudes As New EstructuraSolicitudes()



    Private Sub Thanatos_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' Almaceno el tamaño del Thanatos
        alto = Me.Height
        ancho = Me.Width

        ' Ajustar el tamaño al del panel del login
        Me.Width = PanelLogin.Width
        Me.Height = PanelLogin.Height

        ' Mover y ajustar el PanelLogin
        PanelLogin.Top = 0
        PanelLogin.Left = 0

        ' Establecer la posición inicial del formulario en el centro de la pantalla
        Me.CenterToScreen()

        ' Suscribir los controles al evento MouseClick genérico
        SuscribirEventosMouseClick()

        ' Inicializar el servicio de logs
        logService = New LogService(txtLog)

        ' Inicializar el servicio de bloqueo de UI
        uiBlockService = New UIBlockService(Me)

        ' Inicializar la lista de pics y textboxes
        pictureBoxList = New List(Of PictureBox)()
        textBoxList = New List(Of TextBox)()

        ' Eventos clicks
        InicializarPictureBoxes()
        InicializarTextBoxes()

        ' Reseteo todos los checks y textboxes
        resetCheck()
        resetTextBoxes()


    End Sub
    Private Sub InicializarPictureBoxes()
        pictureBoxList.Add(picEagora)
        pictureBoxList.Add(picSera)
        pictureBoxList.Add(picPF)
        pictureBoxList.Add(picAtlas)
        pictureBoxList.Add(picWinest)
        pictureBoxList.Add(picOdiseaCWO)
        pictureBoxList.Add(picVisord)
        pictureBoxList.Add(picOdisea)
        pictureBoxList.Add(picFlexwan)
        pictureBoxList.Add(picMira)
        pictureBoxList.Add(picIgri)
        pictureBoxList.Add(picEscapex)
        pictureBoxList.Add(picUpdate)
        pictureBoxList.Add(picTelco)
        pictureBoxList.Add(picBaja)
        pictureBoxList.Add(picCorreo)

        For Each pic As PictureBox In pictureBoxList
            AddHandler pic.Click, AddressOf PicAplicaciones_Click
        Next
    End Sub

    Private Sub InicializarTextBoxes()
        textBoxList.Add(TextBox1)
        textBoxList.Add(TextBox2)
        textBoxList.Add(TextBox3)
        textBoxList.Add(TextBox4)
        textBoxList.Add(TextBox5)
        textBoxList.Add(TextBox6)
        textBoxList.Add(TextBox7)
        textBoxList.Add(TextBox8)
        textBoxList.Add(TextBox9)
        textBoxList.Add(TextBox10)
        textBoxList.Add(TextBox11)
        textBoxList.Add(TextBox14)
        textBoxList.Add(TextBox15)
        textBoxList.Add(TextBox16)
        textBoxList.Add(TextBox17)
        textBoxList.Add(TextBox18)
    End Sub

    Private Sub PicAplicaciones_Click(sender As Object, e As EventArgs)
        Dim pic As PictureBox = DirectCast(sender, PictureBox)
        ActualizarEstadoCheck(pic)
    End Sub

    Private Sub ActualizarEstadoCheck(check As PictureBox)
        Select Case check.Tag
            Case EstadoCheck.Nada
                check.Image = My.Resources.checkAmarillo
                check.Tag = EstadoCheck.Pendiente
            Case Else
                check.Image = My.Resources.checkGris
                check.Tag = EstadoCheck.Nada
        End Select
    End Sub

    Private Sub DesmarcarTodosLosPics()
        For Each pic As PictureBox In pictureBoxList
            pic.Image = My.Resources.checkGris
            pic.Tag = EstadoCheck.Nada
        Next
    End Sub

    Private Sub MarcarPicsPerfilIM()
        Dim picsToMark As New List(Of PictureBox) From {picEagora, picSera, picPF, picAtlas, picWinest, picOdiseaCWO, picVisord, picOdisea, picFlexwan}
        For Each pic As PictureBox In picsToMark
            pic.Image = My.Resources.checkAmarillo
            pic.Tag = EstadoCheck.Pendiente
        Next
    End Sub

    Private Sub MarcarPicsPerfilPEX()
        Dim picsToMark As New List(Of PictureBox) From {picEagora, picSera, picPF, picAtlas, picWinest, picMira, picIgri, picEscapex}
        For Each pic As PictureBox In picsToMark
            pic.Image = My.Resources.checkAmarillo
            pic.Tag = EstadoCheck.Pendiente
        Next
    End Sub




    Private Async Function ConsultarApi(apiFunction As Func(Of Boolean, Task(Of String)), invertir As Boolean, descripcion As String, lista As List(Of JsonElement), actualizarContador As Action(Of Integer)) As Task
        uiBlockService.Show($"... consultando {descripcion} ...")

        Dim respuesta As String = Await apiFunction(invertir)

        Try
            Dim respuestaApi = JsonSerializer.Deserialize(Of ApiResponse)(respuesta)

            If respuestaApi.success Then
                lista.AddRange(respuestaApi.data.EnumerateArray())
                actualizarContador(lista.Count)
                refrescaTareas(descripcion, lista.Count)
                'logService.AddLog($"{descripcion}: {lista.Count}")
            Else
                logService.AddLog($"Error: {respuestaApi.msgError}")
            End If
        Catch ex As Exception
            logService.AddLog($"Error al deserializar la respuesta: {ex.Message}")
        End Try

        uiBlockService.Hide()
    End Function
    Private Async Sub picRefrescar_Click(sender As Object, e As EventArgs) Handles picRefrescar.Click
        ' Limpiar las listas antes de consultar la API
        Solicitudes.altasTelcoPendientes.lista.Clear()
        Solicitudes.bajasPendientes.lista.Clear()
        Solicitudes.altasColiseoPendientes.lista.Clear()
        Solicitudes.altasPendientes.lista.Clear()

        Await ConsultarApi(AddressOf servicioNavegador.LeerTelcosPendientes, False, "Telcos pendientes", Solicitudes.altasTelcoPendientes.lista, Sub(count) Solicitudes.altasTelcoPendientes.numero = count)
        Await ConsultarApi(AddressOf servicioNavegador.LeerBajasPendientes, False, "Bajas pendientes", Solicitudes.bajasPendientes.lista, Sub(count) Solicitudes.bajasPendientes.numero = count)
        Await ConsultarApi(AddressOf servicioNavegador.LeerColiseoPendientes, False, "Coliseo pendientes", Solicitudes.altasColiseoPendientes.lista, Sub(count) Solicitudes.altasColiseoPendientes.numero = count)
        Await ConsultarApi(AddressOf servicioNavegador.LeerAltasPendientes, False, "Altas pendientes", Solicitudes.altasPendientes.lista, Sub(count) Solicitudes.altasPendientes.numero = count)

    End Sub
    Private Sub refrescaTareas(descripcion As String, cantidad As Integer)
        Select Case descripcion
            Case "Telcos pendientes"
                lblTelco.Text = cantidad.ToString()
            Case "Bajas pendientes"
                lblBaja.Text = cantidad.ToString()
            Case "Coliseo pendientes"
                lblColiseo.Text = cantidad.ToString()
            Case "Altas pendientes"
                lblEagora.Text = cantidad.ToString()
        End Select
    End Sub

    Private Sub SuscribirEventosMouseClick()
        AddHandler TextBox5.MouseClick, AddressOf Control_MouseClick
        AddHandler TextBox6.MouseClick, AddressOf Control_MouseClick
        AddHandler TextBox8.MouseClick, AddressOf Control_MouseClick
        AddHandler TextBox9.MouseClick, AddressOf Control_MouseClick
        AddHandler TextBox10.MouseClick, AddressOf Control_MouseClick
        AddHandler TextBox14.MouseClick, AddressOf Control_MouseClick
        AddHandler TextBox15.MouseClick, AddressOf Control_MouseClick
        AddHandler TextBox16.MouseClick, AddressOf Control_MouseClick
        AddHandler TextBox17.MouseClick, AddressOf Control_MouseClick
        AddHandler TextBox18.MouseClick, AddressOf Control_MouseClick
        AddHandler ComboBox2.MouseClick, AddressOf Control_MouseClick

        ' Suscribir eventos de clic para las carpetas
        AddHandler picCarpetaVerde.Click, AddressOf Carpeta_Click
        AddHandler picCarpetaNaranja.Click, AddressOf Carpeta_Click
        AddHandler picCarpetaAzul.Click, AddressOf Carpeta_Click
        AddHandler picCarpetaMarron.Click, AddressOf Carpeta_Click
    End Sub

    Private Sub Control_MouseClick(sender As Object, e As MouseEventArgs)
        Console.WriteLine($"Mouse button clicked: {e.Button}")

        If e.Button = MouseButtons.Left Then
            Dim control As Control = CType(sender, Control)
            If TypeOf control.Tag Is String AndAlso Not String.IsNullOrEmpty(control.Tag.ToString()) Then
                Clipboard.SetText(control.Tag.ToString())
            End If
        End If
    End Sub


    Private Sub Carpeta_Click(sender As Object, e As EventArgs)
        ' Limpio los checks y los textboxes
        resetCheck()
        resetTextBoxes()

        Dim lista As List(Of JsonElement) = Nothing
        Dim usuarioSolicitante As String = ""

        Select Case CType(sender, PictureBox).Name
            Case "picCarpetaVerde"
                lista = Solicitudes.altasColiseoPendientes.lista
            Case "picCarpetaNaranja"
                lista = Solicitudes.altasPendientes.lista
            Case "picCarpetaAzul"
                lista = Solicitudes.altasTelcoPendientes.lista
            Case "picCarpetaMarron"
                lista = Solicitudes.bajasPendientes.lista
        End Select

        If lista IsNot Nothing AndAlso lista.Count > 0 Then
            Select Case CType(sender, PictureBox).Name
                Case "picCarpetaVerde"
                    usuarioSolicitante = lista(0).GetProperty("usuariosolicitacoliseo").GetString()
                Case "picCarpetaNaranja"
                    usuarioSolicitante = lista(0).GetProperty("usuariosolicitatelefonica").GetString()
                Case "picCarpetaAzul"
                    usuarioSolicitante = lista(0).GetProperty("usuariosolicitatelco").GetString()
                Case "picCarpetaMarron"
                    usuarioSolicitante = lista(0).GetProperty("usuariosolicitabaja").GetString()
            End Select

            Dim nif As String = lista(0).GetProperty("nif").GetString()
            TextBox7.Text = nif
            TextBox7.Tag = usuarioSolicitante

            ' Eliminar el primer elemento de la lista
            lista.RemoveAt(0)

            ' Actualizar el label correspondiente
            Select Case CType(sender, PictureBox).Name
                Case "picCarpetaVerde"
                    lblColiseo.Text = lista.Count.ToString()
                Case "picCarpetaNaranja"
                    lblEagora.Text = lista.Count.ToString()
                Case "picCarpetaAzul"
                    lblTelco.Text = lista.Count.ToString()
                Case "picCarpetaMarron"
                    lblBaja.Text = lista.Count.ToString()
            End Select

            ' Simular el evento KeyPress con la tecla ENTER
            TextBox7_KeyPress(TextBox7, New KeyPressEventArgs(Convert.ToChar(Keys.Enter)))
        Else
            MessageBox.Show("No hay elementos en la lista.")
        End If
    End Sub


    Private Async Sub TextBox7_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TextBox7.KeyPress
        If e.KeyChar = Convert.ToChar(Keys.Enter) Then
            If TextBox7.Text = "" Then
                MsgBox("Revise el campo NIF/NIE")
                Return
            Else
                Dim servicio As New ServicioNavegador()
                Dim jsonDataPost = servicio.GetJsonDataPostApi("resources")
                jsonDataPost.Url += "?dni=" + TextBox7.Text
                Dim respuesta As String = Await servicio.SetPostAsync(jsonDataPost) ' Realizar la solicitud POST a la API

                ' Aquí manejas la respuesta de la API
                If Not String.IsNullOrEmpty(respuesta) Then
                    Try
                        Dim respuestaApi = JsonSerializer.Deserialize(Of ApiResponse)(respuesta)
                        If respuestaApi.success Then
                            ' Procesar y cargar los datos en el modelo Usuario
                            Dim usuario As New Usuario()
                            For Each item In respuestaApi.data.EnumerateArray()
                                usuario.id_resource = item.GetProperty("id_resource").GetInt32()
                                usuario.persona = item.GetProperty("persona").GetInt32()
                                usuario.cliente = item.GetProperty("cliente").GetString()
                                usuario.situacion = item.GetProperty("situacion").GetString()
                                usuario.perfilsga = item.GetProperty("perfilsga").GetString()
                                usuario.givenName = item.GetProperty("givenName").GetString()
                                usuario.sn = item.GetProperty("sn").GetString()
                                usuario.telefonicadocumento = item.GetProperty("telefonicadocumento").GetString()
                                usuario.provincia = item.GetProperty("provincia").GetString()
                                usuario.empresa = item.GetProperty("empresa").GetString()
                                usuario.cif = item.GetProperty("cif").GetString()
                                usuario.actividad = item.GetProperty("actividad").GetString()
                                usuario.eagora = item.GetProperty("eagora").GetString()
                                usuario.userpassword = PasswordHelper.GenerarPasswordFechaActual()
                                usuario.telephoneNumber = item.GetProperty("telephonenumber").GetString()
                                usuario.sga = item.GetProperty("sga").GetString()
                                usuario.email = item.GetProperty("emailezentis").GetString()
                                usuario.id = item.GetProperty("id").GetString()
                                usuario.pindi = item.GetProperty("pindi").GetString()
                                usuario.coliseo = item.GetProperty("coliseo").GetString()
                                usuario.solicitudtelefonica = item.GetProperty("solicitudtelefonica").GetString()
                                usuario.solicitudtelco = item.GetProperty("solicitudtelco").GetString()
                                usuario.solicitudcoliseo = item.GetProperty("solicitudcoliseo").GetString()
                                usuario.solicitudaltacorreo = item.GetProperty("solicitudaltacorreo").GetString()
                                usuario.usuariosolicitatelco = item.GetProperty("usuariosolicitatelco").GetString()
                                usuario.usuariosolicitatelefonica = item.GetProperty("usuariosolicitatelefonica").GetString()
                                usuario.usuariosolicitabaja = item.GetProperty("usuariosolicitabaja").GetString()
                                usuario.usuariosolicitacoliseo = item.GetProperty("usuariosolicitacoliseo").GetString()
                                usuario.usuariosolicitaaltacorreo = item.GetProperty("usuariosolicitaaltacorreo").GetString()
                                usuario.id_status = item.GetProperty("id_status").GetInt32()
                                usuario.id_estado_sga = item.GetProperty("id_estado_sga").GetInt32()
                                usuario.fecha_sga = item.GetProperty("fecha_sga").GetString()
                                usuario.estadosga = item.GetProperty("estadosga").GetString()
                                usuario.estadosgadefecto = item.GetProperty("estadosgadefecto").GetInt32()
                                usuario.atelco = "PEMPR0012165"
                                usuario.usuarioSolicitante = TextBox7.Tag

                                If Not String.IsNullOrEmpty(usuario.email) AndAlso usuario.email.Contains("@atelcosoluciones.es") Then
                                    usuario.perfil = "Despachador"
                                Else
                                    usuario.perfil = "Tecnico"
                                End If

                                ' Asignar gestor padre basado en el perfil y provincia. Buscar el código de la provincia basado en el nombre
                                Dim provinciaCodigo As String = Configuracion.Provincias.FirstOrDefault(Function(p) p.Value = usuario.provincia).Key

                                If Not String.IsNullOrEmpty(provinciaCodigo) Then
                                    If usuario.perfil = "Tecnico" Then
                                        If Configuracion.gestorPadreTecnico.ContainsKey(provinciaCodigo) Then
                                            usuario.gestorPadre = Configuracion.gestorPadreTecnico(provinciaCodigo)
                                        Else
                                            usuario.gestorPadre = "No asignado"
                                        End If
                                    Else
                                        If Configuracion.gestorPadreGestor.ContainsKey(provinciaCodigo) Then
                                            usuario.gestorPadre = Configuracion.gestorPadreGestor(provinciaCodigo)
                                        Else
                                            usuario.gestorPadre = "No asignado"
                                        End If
                                    End If
                                Else
                                    usuario.gestorPadre = "No asignado"
                                End If


                            Next

                            ' Asignar el usuario al modelo global
                            ServicioModelo.Usuario = usuario

                            ' Llamar al método para cargar los datos en los controles
                            CargarDatosEnControles()

                        Else
                            logService.AddLog($"Error: {respuestaApi.msgError}")
                        End If
                    Catch ex As Exception
                        logService.AddLog($"Error al deserializar la respuesta: {ex.Message}")
                    End Try
                Else
                    MessageBox.Show("Hubo un error al realizar la solicitud a la API.")
                End If
            End If
        End If
    End Sub


    Private Async Sub btnCambiarPassword_Click(sender As Object, e As EventArgs) Handles btnCambiarPassword.Click
        ' Actualizar el modelo desde los controles
        ActualizarModeloDesdeControles()

        Dim servicioCambioPass As New ServicioCambioPass
        Dim modoForzado = False

        Dim respuestaPass = Await servicioCambioPass.CambiarPassword(txtUsuario.Text, txtPassword.Text)

        ' Manejar la respuesta
        If respuestaPass.Success Then
            logService.AddLog(respuestaPass.Data)
            Clipboard.SetText(respuestaPass.Data)
        Else
            logService.AddLog(respuestaPass.msgError)
            respuestaPass = Await servicioCambioPass.ForzarPassword(txtUsuario.Text, txtPassword.Text)
            If respuestaPass.Success Then
                logService.AddLog(respuestaPass.Data)
                Clipboard.SetText(respuestaPass.Data)
            End If
        End If
    End Sub

    Private Sub txtPassword_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles txtPassword.MouseDoubleClick
        If e.Button = MouseButtons.Left Then
            txtPassword.Text = PasswordHelper.GenerarPasswordFechaActual()
        End If
    End Sub

    Private Sub Label14_Click(sender As Object, e As EventArgs) Handles Label14.Click
        Clipboard.SetText("BUCLE")
    End Sub

    Private Sub Label14_DoubleClick(sender As Object, e As EventArgs) Handles Label14.DoubleClick
        Clipboard.SetText("eAgora")
    End Sub

    Private Sub resetCheck()
        ' Uso la lista de picturebox de las aplicaciones para guardar si estado en el tag
        For Each pic In pictureBoxList
            pic.Tag = EstadoCheck.Pendiente
            ActualizarEstadoCheck(pic)
        Next
    End Sub

    Private Sub resetTextBoxes()
        For Each text As TextBox In textBoxList
            text.Text = ""
        Next
        ComboBox1.Text = ""
        ComboBox2.Text = ""
    End Sub

    Public Class ApiResponse
        Public Property success As Boolean
        Public Property data As JsonElement
        Public Property msgError As String
        Public Property nombreQuery As String
    End Class





    Public Class EstructuraSolicitudes
        Public Property altasTelcoPendientes As New Solicitud()
        Public Property bajasPendientes As New Solicitud()
        Public Property altasColiseoPendientes As New Solicitud()
        Public Property altasPendientes As New Solicitud()
    End Class

    Public Class Solicitud
        Public Property numero As Integer
        Public Property lista As New List(Of JsonElement)
    End Class

    Private Sub btnLimpiarCampos_Click(sender As Object, e As EventArgs) Handles btnLimpiarCampos.Click
        resetCheck()
        resetTextBoxes()
    End Sub

    Private Async Sub OK_Click(sender As Object, e As EventArgs) Handles OK.Click
        Configuracion.UsuarioLogin = UsernameTextBox.Text
        Configuracion.PasswordLogin = PasswordTextBox.Text
        Configuracion.UserPass = UsernameTextBox.Text & ":" & PasswordTextBox.Text

        Dim _httpClient = New HttpClient()
        _httpClient.BaseAddress = New Uri("https://eagora.telefonica.es/")

        ' Configurar las credenciales de autenticación básica
        Dim byteArray = Encoding.ASCII.GetBytes(Configuracion.UserPass)
        _httpClient.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray))
        Dim response = Await _httpClient.GetAsync("/portal/site/e-agora/")
        If response.IsSuccessStatusCode Then

            Dim responseData = Await response.Content.ReadAsStringAsync()
            Await Task.Delay(250)
            If responseData.Contains("Usuarios Registrados") Then
                MessageBox.Show("Credenciales incorrectas")
                PanelLogin.Visible = False
                Me.Width = ancho
                Me.Height = alto
                Me.CenterToScreen()

            ElseIf responseData.Contains("Bienvenido") Then
                PanelLogin.Visible = False

                Me.Width = ancho
                Me.Height = alto
                Me.CenterToScreen()
                Me.Text = "Thanatos - " & Configuracion.UsuarioLogin.ToUpper
            End If
        Else
            MessageBox.Show("Credenciales incorrectas")
        End If
    End Sub

    Private Sub PasswordTextBox_KeyPress(sender As Object, e As KeyPressEventArgs) Handles PasswordTextBox.KeyPress
        If e.KeyChar = Convert.ToChar(Keys.Enter) Then

            ' Simular el evento ENTER 
            OK_Click(OK, New KeyPressEventArgs(Convert.ToChar(Keys.Enter)))
        End If
    End Sub
    Private Sub CargarDatosEnControles()
        If ServicioModelo.Usuario IsNot Nothing Then
            TextBox1.Text = ServicioModelo.Usuario.id
            TextBox2.Text = ServicioModelo.Usuario.eagora
            TextBox3.Text = ServicioModelo.Usuario.coliseo
            TextBox4.Text = ServicioModelo.Usuario.userpassword
            TextBox5.Text = ServicioModelo.Usuario.givenName
            TextBox5.Tag = ServicioModelo.Usuario.givenName + " " + ServicioModelo.Usuario.sn
            TextBox6.Text = ServicioModelo.Usuario.sn
            TextBox6.Tag = ServicioModelo.Usuario.givenName + " " + ServicioModelo.Usuario.sn
            TextBox8.Text = ServicioModelo.Usuario.telephoneNumber
            TextBox8.Tag = ServicioModelo.Usuario.telephoneNumber
            TextBox9.Text = ServicioModelo.Usuario.email
            TextBox9.Tag = ServicioModelo.Usuario.email
            TextBox10.Text = ServicioModelo.Usuario.sga
            TextBox10.Tag = ServicioModelo.Usuario.sga
            TextBox11.Text = ServicioModelo.Usuario.actividad
            TextBox11.Tag = ServicioModelo.Usuario.actividad
            ComboBox1.Text = ServicioModelo.Usuario.perfil
            ComboBox1.Tag = ServicioModelo.Usuario.perfil
            ComboBox2.Text = ServicioModelo.Usuario.provincia
            ComboBox2.Tag = ServicioModelo.Usuario.provincia
            TextBox14.Text = ServicioModelo.Usuario.pindi
            TextBox14.Tag = ServicioModelo.Usuario.pindi
            TextBox16.Text = ServicioModelo.Usuario.empresa
            TextBox16.Tag = ServicioModelo.Usuario.cif
            TextBox17.Text = ServicioModelo.Usuario.atelco
            TextBox17.Tag = ServicioModelo.Usuario.atelco
            TextBox15.Text = ServicioModelo.Usuario.gestorPadre
            TextBox15.Tag = ServicioModelo.Usuario.gestorPadre
            TextBox18.Text = ServicioModelo.Usuario.usuarioSolicitante
            TextBox18.Tag = ServicioModelo.Usuario.usuarioSolicitante
        End If
    End Sub

    Private Sub ActualizarModeloDesdeControles()
        If ServicioModelo.Usuario IsNot Nothing Then
            ' Ejecutar en el hilo de la interfaz de usuario (UI) utilizando Invoke
            If Me.InvokeRequired Then
                Me.Invoke(New MethodInvoker(AddressOf ActualizarModeloDesdeControles))
            Else
                ServicioModelo.Usuario.id = TextBox1.Text
                ServicioModelo.Usuario.eagora = TextBox2.Text
                ServicioModelo.Usuario.coliseo = TextBox3.Text
                ServicioModelo.Usuario.userpassword = TextBox4.Text
                ServicioModelo.Usuario.givenName = TextBox5.Text
                ServicioModelo.Usuario.sn = TextBox6.Text
                ServicioModelo.Usuario.telephoneNumber = TextBox8.Text
                ServicioModelo.Usuario.email = TextBox9.Text
                ServicioModelo.Usuario.sga = TextBox10.Text
                ServicioModelo.Usuario.actividad = TextBox11.Text
                ServicioModelo.Usuario.perfilsga = ComboBox1.Text
                ServicioModelo.Usuario.provincia = ComboBox2.Text
                ServicioModelo.Usuario.pindi = TextBox14.Text
                ServicioModelo.Usuario.empresa = TextBox16.Text
                ServicioModelo.Usuario.cif = TextBox16.Tag ' Asumiendo que CIF está en el Tag
                ServicioModelo.Usuario.atelco = TextBox17.Text
                ServicioModelo.Usuario.gestorPadre = TextBox15.Text
                ServicioModelo.Usuario.usuarioSolicitante = TextBox18.Text
            End If
        End If
    End Sub


    Private Async Function EjecutarAltaUsuario() As Task
        ActualizarModeloDesdeControles()

        ' Crear una instancia del validador y validar el usuario
        Dim validador As New ValidadorUsuario()
        Dim resultadoValidacion As ResultadoValidacion = validador.CompruebaCampos(ServicioModelo.Usuario)

        ' Verificar si la validación fue exitosa
        If resultadoValidacion.Success Then


            Dim servicioAlta As New ServicioAltaUsuario()
            Dim respuestaAlta = Await servicioAlta.AltaUsuario(ServicioModelo.Usuario)

            If respuestaAlta.Success Then

                ' Crear una instancia del ServicioMovilPin
                Dim servicioMovilPin As New ServicioMovilPin()

                ' Llamar al método para configurar el móvil y el PIN
                Dim respuestaMovilPin = Await servicioMovilPin.ConfigurarMovilPin(ServicioModelo.Usuario)

                ' Manejar la respuesta
                If respuestaMovilPin.Success Then
                    logService.AddLog("Usuario dado de alta correctamente.")

                    ' Marco en verde el check de eAgora
                    picEagora.Image = My.Resources.checkVerde
                    picEagora.Tag = EstadoCheck.Correcto
                Else
                    MessageBox.Show("Error al configurar el móvil y PIN: " & respuestaMovilPin.msgError)
                    logService.AddLog("Error al configurar el móvil y PIN: " & respuestaMovilPin.msgError)
                    ' Marco en rojo el check de eAgora
                    picEagora.Image = My.Resources.checkRojo
                    picEagora.Tag = EstadoCheck.Fallido
                End If
            Else
                MessageBox.Show("Error al dar de alta al usuario: " & respuestaAlta.msgError)
                logService.AddLog("Error al dar de alta al usuario: " & respuestaAlta.msgError)

                ' Marco en rojo el check de eAgora
                picEagora.Image = My.Resources.checkRojo
                picEagora.Tag = EstadoCheck.Fallido
            End If
        Else
            ' Si la validación falla, mostrar el mensaje de error
            MessageBox.Show("Error en los datos del usuario: " & resultadoValidacion.msgError)
            logService.AddLog("Error en los datos del usuario: " & resultadoValidacion.msgError)

            ' Marco en rojo el check de eAgora
            picEagora.Image = My.Resources.checkRojo
            picEagora.Tag = EstadoCheck.Fallido
        End If
    End Function

    Private Async Function EjecutarAltaSera() As Task

        ' Actualizar el modelo desde los controles
        ActualizarModeloDesdeControles()

        ' Crear una instancia del servicio de alta en SERA
        Dim servicioAltaSera As New ServicioAltaSera()

        ' Llamar al método para realizar el alta en SERA
        Dim respuesta = Await servicioAltaSera.AltaSera(ServicioModelo.Usuario)

        ' Manejar la respuesta
        If respuesta.Success Then
            Invoke(Sub()
                       logService.AddLog("Alta en SERA")
                       ' Marco en verde el check de SERA
                       picSera.Image = My.Resources.checkVerde
                       picSera.Tag = EstadoCheck.Correcto
                   End Sub)
        Else
            Invoke(Sub()
                       logService.AddLog("Error al dar de alta en SERA: " & respuesta.msgError)
                       picSera.Image = My.Resources.checkRojo
                       picSera.Tag = EstadoCheck.Fallido
                   End Sub)
        End If
    End Function

    Private Async Function EjecutarAltaPF() As Task

        ' Actualizar el modelo desde los controles
        ActualizarModeloDesdeControles()

        ' Crear una instancia del servicio de alta en PF
        Dim servicioAltaPF As New ServicioAltaPF

        ' Llamar al método para realizar el alta en PF
        Dim respuesta = Await servicioAltaPF.AltaPF(ServicioModelo.Usuario)

        ' Manejar la respuesta
        If respuesta.Success Then
            logService.AddLog("Alta en PF")
            picPF.Image = My.Resources.checkVerde
            picPF.Tag = EstadoCheck.Correcto
        Else
            logService.AddLog("Error al dar de alta en PF: " & respuesta.msgError)
            picPF.Image = My.Resources.checkRojo
            picPF.Tag = EstadoCheck.Fallido
        End If

    End Function

    Private Async Function EjecutarAltaAtlas() As Task

        ' Actualizar el modelo desde los controles
        ActualizarModeloDesdeControles()

        ' Crear una instancia del servicio de alta en ATLAS
        Dim servicioAltaAtlas As New ServicioAltaAtlas
        Dim resultadoSolicitud = Await servicioAltaAtlas.GetSolicitudAtlas(ServicioModelo.Usuario.eagora)

        If resultadoSolicitud.Success Then


            Dim resultadoAlta = Await servicioAltaAtlas.AltaAtlas(ServicioModelo.Usuario, resultadoSolicitud.Data)
            If resultadoAlta.Success Then
                logService.AddLog("Solicitud AtlasPA: " & resultadoSolicitud.Data)
                picAtlas.Image = My.Resources.checkVerde
                picAtlas.Tag = EstadoCheck.Correcto
            Else
                logService.AddLog("Error en la alta en Atlas: " & resultadoAlta.msgError)
                picAtlas.Image = My.Resources.checkRojo
                picAtlas.Tag = EstadoCheck.Fallido
            End If
        Else
            MessageBox.Show("Error en la solicitud de Atlas: " & resultadoSolicitud.msgError)
            logService.AddLog("Error en la solicitud de Atlas: " & resultadoSolicitud.msgError)
            picAtlas.Image = My.Resources.checkRojo
            picAtlas.Tag = EstadoCheck.Fallido
        End If
    End Function

    Private Async Function EjecutarAltaWinest() As Task

        ' Actualizar el modelo desde los controles
        ActualizarModeloDesdeControles()

        Dim servicioWinest As New ServicioAltaWinest

        ' Solicitar la ID de la solicitud
        Dim solicitudRespuesta = Await servicioWinest.GetSolicitudWinest(ServicioModelo.Usuario.eagora)

        If solicitudRespuesta.Success Then

            ' Proceder a completar la solicitud con la ID obtenida
            Dim resultadoAlta = Await servicioWinest.AltaWinest(ServicioModelo.Usuario, solicitudRespuesta.Data)

            If resultadoAlta.Success Then
                logService.AddLog("Solicitud WINEST: " & solicitudRespuesta.Data)
                picWinest.Image = My.Resources.checkVerde
                picWinest.Tag = EstadoCheck.Correcto
            Else
                logService.AddLog("Error al completar el alta en Winest: " & resultadoAlta.msgError)
                picWinest.Image = My.Resources.checkRojo
                picWinest.Tag = EstadoCheck.Fallido
            End If
        Else
            MessageBox.Show("Error al solicitar la ID de solicitud de Winest: " & solicitudRespuesta.msgError)
            logService.AddLog("Error al solicitar la ID de solicitud de Winest: " & solicitudRespuesta.msgError)
            picWinest.Image = My.Resources.checkRojo
            picWinest.Tag = EstadoCheck.Fallido
        End If
    End Function

    Private Async Function EjecutarAltaVisord() As Task
        ' Actualizar el modelo desde los controles
        ActualizarModeloDesdeControles()

        Dim servicioVisord As New ServicioAltaVisord()

        Dim respuesta = Await servicioVisord.AltaVisord(ServicioModelo.Usuario)

        ' Manejar la respuesta
        If respuesta.Success Then
            logService.AddLog("Alta en Visord correcta.")
            picVisord.Image = My.Resources.checkVerde
            picVisord.Tag = EstadoCheck.Correcto
        Else
            logService.AddLog("Error al dar de alta en Visord: " & respuesta.MensajeError)
            picVisord.Image = My.Resources.checkRojo
            picVisord.Tag = EstadoCheck.Fallido
        End If
    End Function

    Private Async Function EjecutarAltaOdiseaCWO() As Task

        ' Actualizar el modelo desde los controles
        ActualizarModeloDesdeControles()

        Dim servicioOdiseaCWO As New ServicioAltaOdiseaCWO()

        Dim respuesta = Await servicioOdiseaCWO.AltaOdiseaCWO(ServicioModelo.Usuario)

        ' Manejar la respuesta
        If respuesta.Success Then
            logService.AddLog("Alta en Odise CWO")
            picOdiseaCWO.Image = My.Resources.checkVerde
            picOdiseaCWO.Tag = EstadoCheck.Correcto
        Else
            logService.AddLog("Error al dar de alta en Odise CWO: " & respuesta.msgError)
            picOdiseaCWO.Image = My.Resources.checkRojo
            picOdiseaCWO.Tag = EstadoCheck.Fallido
        End If
    End Function

    Private Async Function EjecutarAltaOdisea() As Task
        ' Actualizar el modelo desde los controles
        ActualizarModeloDesdeControles()

        Dim servicioAltaOdisea As New ServicioAltaOdisea()

        Dim respuesta = Await servicioAltaOdisea.AltaUsuarioOdisea(ServicioModelo.Usuario)

        ' Manejar la respuesta
        If respuesta.Success Then
            logService.AddLog("Alta en Odisea realizada correctamente.")
            picOdisea.Image = My.Resources.checkVerde
            picOdisea.Tag = EstadoCheck.Correcto
        Else
            logService.AddLog("Error al dar de alta en Odisea: " & respuesta.msgError)
            picOdisea.Image = My.Resources.checkRojo
            picOdisea.Tag = EstadoCheck.Fallido
        End If
    End Function

    Private Async Function EjecutarAltaFlexwan() As Task
        ' Actualizar el modelo desde los controles
        ActualizarModeloDesdeControles()

        ' Crear una instancia del servicio de alta en Flexwan
        Dim servicioAltaFlexwan As New ServicioAltaFlexwan()

        ' Solicitar la ID de la solicitud
        Dim solicitudRespuesta = Await servicioAltaFlexwan.GetSolicitudFlexwan(ServicioModelo.Usuario.eagora)

        If solicitudRespuesta.Success Then

            ' Proceder a completar la solicitud con la ID obtenida
            Dim parametros = servicioAltaFlexwan.GetParametrosAltaFlexwan(solicitudRespuesta.Data)

            Dim resultadoAlta = Await servicioAltaFlexwan.AltaFlexwan(parametros)

            If resultadoAlta.Success Then
                logService.AddLog("Solicitud FLEXWAN: " & solicitudRespuesta.Data)
                picFlexwan.Image = My.Resources.checkVerde
                picFlexwan.Tag = EstadoCheck.Correcto
            Else
                logService.AddLog("Error al completar el alta en Flexwan: " & resultadoAlta.msgError)
                picFlexwan.Image = My.Resources.checkRojo
                picFlexwan.Tag = EstadoCheck.Fallido
            End If
        Else
            MessageBox.Show("Error al solicitar la ID de solicitud de Flexwan: " & solicitudRespuesta.msgError)
            logService.AddLog("Error al solicitar la ID de solicitud de Flexwan: " & solicitudRespuesta.msgError)
            picFlexwan.Image = My.Resources.checkRojo
            picFlexwan.Tag = EstadoCheck.Fallido
        End If
    End Function

    Private Async Function EjecutarAltaMira() As Task
        ' Actualizar el modelo desde los controles
        ActualizarModeloDesdeControles()

        ' Crear una instancia del servicio de alta en Mira
        Dim servicioAltaMira As New ServicioAltaMira()

        ' Solicitar la ID de la solicitud
        Dim solicitudRespuesta = Await servicioAltaMira.GetSolicitudMira(ServicioModelo.Usuario.eagora)

        If solicitudRespuesta.Success Then

            ' Proceder a completar la solicitud con la ID obtenida
            'Dim parametros = servicioAltaMira.GetParametrosAltaMira(ServicioModelo.Usuario, solicitudRespuesta.Data)
            Dim resultadoAlta = Await servicioAltaMira.AltaMira(ServicioModelo.Usuario, solicitudRespuesta.Data)

            If resultadoAlta.Success Then
                logService.AddLog("Solicitud Mir@: " & solicitudRespuesta.Data)
                picMira.Image = My.Resources.checkVerde
                picMira.Tag = EstadoCheck.Correcto
            Else
                logService.AddLog("Error al completar el alta en Mira: " & resultadoAlta.msgError)
                picMira.Image = My.Resources.checkRojo
                picMira.Tag = EstadoCheck.Fallido
            End If
        Else
            MessageBox.Show("Error al solicitar la ID de solicitud de Mira: " & solicitudRespuesta.msgError)
            logService.AddLog("Error al solicitar la ID de solicitud de Mira: " & solicitudRespuesta.msgError)
            picMira.Image = My.Resources.checkRojo
            picMira.Tag = EstadoCheck.Fallido
        End If
    End Function

    Private Async Function EjecutarAltaIgri() As Task
        ' Actualizar el modelo desde los controles
        ActualizarModeloDesdeControles()

        ' Crear una instancia del servicio de alta en IGRI
        Dim servicioAltaIgri As New ServicioAltaIgri()

        ' Solicitar la ID de la solicitud
        Dim solicitudRespuesta = Await servicioAltaIgri.GetSolicitudIgri(ServicioModelo.Usuario.eagora)

        If solicitudRespuesta.Success Then
            ' Proceder a completar la solicitud con la ID obtenida
            Dim parametros = servicioAltaIgri.GetParametrosAltaIgri(ServicioModelo.Usuario, solicitudRespuesta.Data)
            Dim resultadoAlta = Await servicioAltaIgri.AltaIgri(ServicioModelo.Usuario, solicitudRespuesta.Data)

            If resultadoAlta.Success Then
                logService.AddLog("Solicitud IGRI: " & solicitudRespuesta.Data)
                picIgri.Image = My.Resources.checkVerde
                picIgri.Tag = EstadoCheck.Correcto
            Else
                logService.AddLog("Error al completar el alta en IGRI: " & resultadoAlta.msgError)
                picIgri.Image = My.Resources.checkRojo
                picIgri.Tag = EstadoCheck.Fallido
            End If
        Else
            MessageBox.Show("Error al solicitar la ID de solicitud de IGRI: " & solicitudRespuesta.msgError)
            logService.AddLog("Error al solicitar la ID de solicitud de IGRI: " & solicitudRespuesta.msgError)
            picIgri.Image = My.Resources.checkRojo
            picIgri.Tag = EstadoCheck.Fallido
        End If
    End Function

    Private Async Function EjecutarAltaWpex() As Task
        ' Actualizar el modelo desde los controles
        ActualizarModeloDesdeControles()

        ' Crear una instancia del servicio de alta en Wpex
        Dim servicioAltaWpex As New ServicioAltaWpex()

        ' Solicitar la ID de la solicitud
        Dim solicitudRespuesta = Await servicioAltaWpex.GetSolicitudWpex(ServicioModelo.Usuario.eagora)

        If solicitudRespuesta.Success Then
            ' Proceder a completar la solicitud con la ID obtenida
            Dim resultadoAlta = Await servicioAltaWpex.AltaWpex(solicitudRespuesta.Data)

            If resultadoAlta.Success Then
                logService.AddLog("Solicitud ESCAPEX: " & solicitudRespuesta.Data)
                picEscapex.Image = My.Resources.checkVerde
                picEscapex.Tag = EstadoCheck.Correcto
            Else
                logService.AddLog("Error al completar el alta en ESCAPEX: " & resultadoAlta.msgError)
                picEscapex.Image = My.Resources.checkRojo
                picEscapex.Tag = EstadoCheck.Fallido
            End If
        Else
            MessageBox.Show("Error al solicitar la ID de solicitud de ESCAPEX: " & solicitudRespuesta.msgError)
            logService.AddLog("Error al solicitar la ID de solicitud de ESCAPEX: " & solicitudRespuesta.msgError)
            picEscapex.Image = My.Resources.checkRojo
            picEscapex.Tag = EstadoCheck.Fallido
        End If
    End Function

    Private Async Function EjecutarActualizarAgenda() As Task

        ' Actualizar el modelo desde los controles
        ActualizarModeloDesdeControles()

        Dim servicioNavegador As New ServicioNavegador()
        Dim respuesta = Await servicioNavegador.ActualizarAgendaUsuario(ServicioModelo.Usuario)

        If respuesta.success Then
            logService.AddLog("Agenda actualizada correctamente.")
            picUpdate.Image = My.Resources.checkVerde
            picUpdate.Tag = EstadoCheck.Correcto
        Else
            logService.AddLog("Error al actualizar la agenda: " & respuesta.msgError)
            picUpdate.Image = My.Resources.checkRojo
            picUpdate.Tag = EstadoCheck.Fallido
        End If
    End Function
    Private Async Function EjecutarAltaTelco() As Task
        ' Actualizar el modelo desde los controles
        ActualizarModeloDesdeControles()

        ' Crear una instancia del servicio de alta en TELCO
        Dim servicioAltaTelco As New ServicioAltaTelco()

        ' Llamar al método para realizar el alta en TELCO
        Dim resultado = Await servicioAltaTelco.DarAltaTelco(ServicioModelo.Usuario)

        ' Manejar la respuesta
        If resultado Then
            logService.AddLog("PINDI Telco actualizado correctamente.")
            picTelco.Image = My.Resources.checkVerde
            picTelco.Tag = EstadoCheck.Correcto
        Else
            logService.AddLog("Error al actualizar el PINDI Telco.")
            picTelco.Image = My.Resources.checkRojo
            picTelco.Tag = EstadoCheck.Fallido
        End If
    End Function

    Private Async Function EjecutarBajaTotal() As Task

        ' Actualizar el modelo desde los controles
        ActualizarModeloDesdeControles()

        ' Crear una instancia del servicio de baja total
        Dim servicioBajaTotal As New ServicioBajaTotal()

        ' Ejecutar el proceso de baja total
        Dim respuesta = Await servicioBajaTotal.DarBajaTotal(ServicioModelo.Usuario)

        ' Manejar la respuesta
        If respuesta.success Then
            logService.AddLog("Baja total realizada correctamente.")
            picBaja.Image = My.Resources.checkVerde
            picBaja.Image = My.Resources.checkVerde
        Else
            logService.AddLog("Error al realizar la baja total: " & respuesta.msgError)
            picBaja.Image = My.Resources.checkRojo
            picBaja.Image = My.Resources.checkRojo
            ' Mostrar un mensaje de error al usuario
            MessageBox.Show("Error al realizar la baja total: " & respuesta.msgError)
        End If
    End Function
    Private Async Function EjecutarEnvioCorreo() As Task
        ' Actualizar el modelo desde los controles
        ActualizarModeloDesdeControles()

        ' Crear una instancia del servicio de envio de correos
        Dim servicioEnvioEmail As New ServicioEnvioEmail()

        ' Creo una estructura con los valores del servidor de correos
        Dim _estructuraEmail As EstructuraEmail = Configuracion.GetEmailDetails()

        _estructuraEmail.ToAddresses = "eligioalmuedo@gmail.com"
        _estructuraEmail.CcAddresses = Nothing
        _estructuraEmail.BccAddresses = Nothing
        _estructuraEmail.Subject = "Correo de prueba"
        _estructuraEmail.Body = "Soy un puto genio"

        ' Ejecutar el proceso de envio de correo
        Dim respuesta = Await Task.Run(Function() servicioEnvioEmail.SendEmail(_estructuraEmail))
        If respuesta Then
            logService.AddLog("Correo enviado")
            picCorreo.Image = My.Resources.checkVerde
        Else
            logService.AddLog("Error al enviar el correo")
            picCorreo.Image = My.Resources.checkRojo
        End If
    End Function




    Private Async Function ProcesarSolicitudesSecuenciales() As Task
        For Each pic As PictureBox In pictureBoxList
            If pic.Tag = EstadoCheck.Pendiente Then
                Select Case pic.Name
                    Case "picEagora"
                        Await EjecutarAltaUsuario()
                    Case "picSera"
                        Await EjecutarAltaSera()
                    Case "picPF"
                        Await EjecutarAltaPF()
                    Case "picAtlas"
                        Await EjecutarAltaAtlas()
                    Case "picWinest"
                        Await EjecutarAltaWinest()
                    Case "picOdiseaCWO"
                        Await EjecutarAltaOdiseaCWO()
                    Case "picVisord"
                        Await EjecutarAltaVisord()
                    Case "picOdisea"
                        Await EjecutarAltaOdisea()
                    Case "picFlexwan"
                        Await EjecutarAltaFlexwan()
                    Case "picMira"
                        Await EjecutarAltaMira()
                    Case "picIgri"
                        Await EjecutarAltaIgri()
                    Case "picEscapex"
                        Await EjecutarAltaWpex()
                    Case "picUpdate"
                        Await EjecutarActualizarAgenda()
                    Case "picTelco"
                        Await EjecutarAltaTelco()
                    Case "picBaja"
                        Await EjecutarBajaTotal()
                    Case "picCorreo"
                        Await EjecutarEnvioCorreo()

                End Select
            End If
        Next
    End Function

    Private Async Sub btnGestionar_Click(sender As Object, e As EventArgs) Handles btnGestionar.Click
        ' Verificar si hay algún pic marcado
        Dim hayMarcados As Boolean = pictureBoxList.Any(Function(pic) pic.Tag = EstadoCheck.Pendiente)

        ' Si hay marcados, preguntar por confirmación
        If hayMarcados Then
            Dim respuestaGestionar As DialogResult = MessageBox.Show("¿Arrancamos las gestiones?", "Confirmación", MessageBoxButtons.YesNo)
            If respuestaGestionar = DialogResult.Yes Then
                ' Ejecutar las gestiones si el usuario hace clic en 'Sí'
                Await ProcesarSolicitudesSecuenciales()
            End If
        End If
    End Sub


    'Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

    '    Dim smtpServer As String = "smtp-mail.outlook.com"
    '    Dim smtpPort As Integer = 587
    '    Dim smtpUsername As String = "si.soporte@atelcosoluciones.es"
    '    Dim smtpPassword As String = "Saz23122"
    '    Dim toAddress As String = "koko_sifredi@hotmail.com"
    '    Dim ccAddresses As String = Nothing
    '    Dim bccAddresses As String = Nothing
    '    Dim subject As String = "Correo de prueba"
    '    Dim body As String = "Esto es un correo de prueba de envio de correo por el Thanatos"

    '    Try
    '        Dim emailService As New EmailService()
    '        emailService.SendEmail(smtpServer, smtpPort, smtpUsername, smtpPassword, toAddress, ccAddresses, bccAddresses, subject, body)
    '        MessageBox.Show("Correo enviado correctamente.")
    '    Catch ex As Exception
    '        MessageBox.Show($"Error al enviar el correo electrónico: {ex.Message}")
    '    End Try
    'End Sub

    Private Sub btnPerfilIM_Click(sender As Object, e As EventArgs) Handles btnPerfilIM.Click
        DesmarcarTodosLosPics()
        MarcarPicsPerfilIM()
    End Sub

    Private Sub btnPerfilPEX_Click(sender As Object, e As EventArgs) Handles btnPerfilPEX.Click
        DesmarcarTodosLosPics()
        MarcarPicsPerfilPEX()
    End Sub

End Class
