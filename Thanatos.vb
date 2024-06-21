Imports System.Text.Json

Public Class Thanatos
    Private WithEvents webBrowserControl As New WebBrowser()
    Private servicioNavegador As New ServicioNavegador()
    Private servicioCambiarPassword As New ServicioCambiarPassword()
    Private navigateForPasswordChange As Boolean = False
    Private logService As LogService
    Private uiBlockService As UIBlockService
    Private pictureBoxList As List(Of PictureBox)
    Private textBoxList As List(Of TextBox)



    ' Estructura para almacenar las listas y contadores
    Private Solicitudes As New EstructuraSolicitudes()

    Private Sub Thanatos_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Establecer las propiedades del control WebBrowser
        webBrowserControl.Dock = DockStyle.Fill
        webBrowserControl.ScriptErrorsSuppressed = True

        ' Agregar el control WebBrowser al panel
        Panel1.Controls.Add(webBrowserControl)

        ' Navega a eagora para forzar las credenciales
        Dim urlEagora As String = "https://eagora.telefonica.es/portal/site/e-agora/"
        webBrowserControl.Navigate(urlEagora)

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

        ' Reseteo todos los checks y textboex
        resetCheck()
        resetTextBoxes()

    End Sub
    Private Sub InicializarPictureBoxes()
        pictureBoxList.Add(picEagora)
        pictureBoxList.Add(picContrasenia)
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

    Private Async Sub webBrowserControl_DocumentCompleted(sender As Object, e As WebBrowserDocumentCompletedEventArgs) Handles webBrowserControl.DocumentCompleted

        ' Verificar si estamos en la página de inicio para refrescar
        If webBrowserControl.Url.ToString().Contains("portal/site/e-agora/") Then
            webBrowserControl.Refresh()
            Return
        End If

        ' Obtener las credenciales almacenadas en el tag
        Dim credentials As Tuple(Of String, String, Boolean) = TryCast(webBrowserControl.Tag, Tuple(Of String, String, Boolean))
        If credentials Is Nothing Then Return

        navigateForPasswordChange = credentials.Item3

        ' Verificar si estamos navegando para el cambio de contraseña
        If navigateForPasswordChange Then

            webBrowserControl.Tag = New Tuple(Of String, String, Boolean)(credentials.Item1, credentials.Item2, False)

            ' Inyectar el script para capturar alertas y enviar el formulario
            Dim script As String = $"window.alert = function(msg) {{" &
                               $"window.external.Notify(msg);" &
                               $"}};" &
                               $"var form = document.createElement('form');" &
                               $"form.method = 'POST';" &
                               $"form.action = '{webBrowserControl.Url.ToString()}';" &
                               $"var idField = document.createElement('input');" &
                               $"idField.name = 'id';" &
                               $"idField.value = '{credentials.Item1}';" &
                               $"form.appendChild(idField);" &
                               $"var passwordField = document.createElement('input');" &
                               $"passwordField.name = 'userPassword';" &
                               $"passwordField.value = '{credentials.Item2}';" &
                               $"form.appendChild(passwordField);" &
                               $"document.body.appendChild(form);" &
                               $"form.submit();"

            webBrowserControl.Document.InvokeScript("eval", New Object() {script})
        End If

        ' Verificar la respuesta del cambio de contraseña
        If webBrowserControl.Url.ToString().Contains("mod.jsp") Then
            If webBrowserControl.DocumentText.Contains("Se ha producido un error") Then
                ' No hacemos nada. No se presentan mensajes tampoco por la parte del webbrowser
            ElseIf webBrowserControl.DocumentText.Contains("alert('Password en Historial')") Then
                logService.AddLog($"Password en Historial: " + txtPassword.Text)
            ElseIf webBrowserControl.DocumentText.Contains("var user") Then
                ' Actualizar el tag para indicar que la navegación ya se ha realizado
                webBrowserControl.Tag = New Tuple(Of String, String, Boolean)(credentials.Item1, credentials.Item2, False)
                logService.AddLog($"Nueva contraseña: " + txtPassword.Text)
                Clipboard.SetText($"Nueva contraseña: " + txtPassword.Text)
            End If
        End If
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
            If TypeOf control.Tag Is String AndAlso control.Tag IsNot Nothing Then
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
                usuarioSolicitante = lista(0).GetProperty("usuariosolicitacoliseo").GetString()
            Case "picCarpetaNaranja"
                lista = Solicitudes.altasPendientes.lista
                usuarioSolicitante = lista(0).GetProperty("usuariosolicitatelefonica").GetString()
            Case "picCarpetaAzul"
                lista = Solicitudes.altasTelcoPendientes.lista
                usuarioSolicitante = lista(0).GetProperty("usuariosolicitatelco").GetString()
            Case "picCarpetaMarron"
                lista = Solicitudes.bajasPendientes.lista
                usuarioSolicitante = lista(0).GetProperty("usuariosolicitabaja").GetString()
        End Select

        If lista IsNot Nothing AndAlso lista.Count > 0 Then
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


    Private Sub btnCambiarPassword_Click(sender As Object, e As EventArgs) Handles btnCambiarPassword.Click
        Dim usuario As String = txtUsuario.Text
        Dim nuevaPassword As String = txtPassword.Text

        ' Validar los campos antes de intentar cambiar la contraseña
        Dim datos As New DatosUsuario With {
            .Eagora = usuario,
            .Password = nuevaPassword
        }

        Dim respuesta = servicioCambiarPassword.CompruebaCampos(datos)
        If Not respuesta.Success Then
            logService.AddLog($"Error de validación: " + respuesta.msgError)
            Return
        End If

        servicioCambiarPassword.IniciarCambioPassword(webBrowserControl, usuario, nuevaPassword)
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
            ComboBox1.Text = ServicioModelo.Usuario.perfilsga
            ComboBox1.Tag = ServicioModelo.Usuario.perfilsga
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

    Private Sub ActualizarModeloDesdeControles()  ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''' Usar antes de subir info por la api
        If ServicioModelo.Usuario IsNot Nothing Then
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
    End Sub

    Private Async Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        ActualizarModeloDesdeControles()

        ' Crear una instancia del validador y validar el usuario
        Dim validador As New ValidadorUsuario()
        Dim resultadoValidacion As ResultadoValidacion = validador.CompruebaCampos(ServicioModelo.Usuario)

        ' Verificar si la validación fue exitosa
        If resultadoValidacion.Success Then


            Dim servicioAlta As New ServicioAltaUsuario()
            Dim respuestaAlta = Await servicioAlta.AltaUsuario(ServicioModelo.Usuario)

            If respuestaAlta.Success Then
                ' MessageBox.Show("Usuario dado de alta correctamente.")
                logService.AddLog("Usuario dado de alta correctamente.")

                ' Marco en verde el check de eAgora
                picEagora.Image = My.Resources.checkVerde
                picEagora.Tag = EstadoCheck.Correcto

                ' Crear una instancia del ServicioMovilPin
                Dim servicioMovilPin As New ServicioMovilPin()

                ' Llamar al método para configurar el móvil y el PIN
                Dim respuestaMovilPin = Await servicioMovilPin.ConfigurarMovilPin(ServicioModelo.Usuario)

                ' Manejar la respuesta
                If respuestaMovilPin.Success Then
                    logService.AddLog("Móvil y PIN configurados correctamente.")
                Else
                    MessageBox.Show("Error al configurar el móvil y PIN: " & respuestaMovilPin.msgError)
                    logService.AddLog("Error al configurar el móvil y PIN: " & respuestaMovilPin.msgError)
                End If
            Else
                ' MessageBox.Show("Error al dar de alta al usuario: " & respuestaAlta.msgError)
                logService.AddLog("Error al dar de alta al usuario: " & respuestaAlta.msgError)

                ' Marco en rojo el check de eAgora
                picEagora.Image = My.Resources.checkRojo
                picEagora.Tag = EstadoCheck.Fallido
            End If
        Else
            ' Si la validación falla, mostrar el mensaje de error
            MessageBox.Show("Error en los datos del usuario: " & resultadoValidacion.MensajeError)
            logService.AddLog("Error en los datos del usuario: " & resultadoValidacion.MensajeError)

            ' Marco en rojo el check de eAgora
            picEagora.Image = My.Resources.checkRojo
            picEagora.Tag = EstadoCheck.Fallido
        End If
    End Sub



End Class
