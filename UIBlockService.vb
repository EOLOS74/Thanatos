Public Class UIBlockService
    Private blockForm As BlockForm
    Private parentForm As Form

    Public Sub New(parentForm As Form)
        Me.parentForm = parentForm
        blockForm = New BlockForm()
        blockForm.Owner = parentForm

        ' Manejar eventos de cambio de tamaño, movimiento y minimización del formulario principal
        AddHandler parentForm.Resize, AddressOf OnParentFormResizeOrMove
        AddHandler parentForm.Move, AddressOf OnParentFormResizeOrMove
        AddHandler parentForm.ResizeEnd, AddressOf OnParentFormResizeOrMove
    End Sub

    Public Sub Show(message As String)
        If parentForm.InvokeRequired Then
            parentForm.Invoke(New Action(Of String)(AddressOf Show), message)
        Else
            blockForm.UpdateMessage(message)
            blockForm.AdjustSizeAndPosition(parentForm)
            blockForm.Show()
            blockForm.BringToFront()
        End If
    End Sub

    Public Sub Hide()
        If parentForm.InvokeRequired Then
            parentForm.Invoke(New Action(AddressOf Hide))
        Else
            blockForm.Hide()
        End If
    End Sub

    Private Sub OnParentFormResizeOrMove(sender As Object, e As EventArgs)
        If blockForm.Visible Then
            blockForm.AdjustSizeAndPosition(parentForm)
        End If
    End Sub
End Class