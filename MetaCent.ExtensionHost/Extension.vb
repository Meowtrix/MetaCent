Option Compare Binary
Imports Windows.ApplicationModel.AppExtensions
Imports Windows.ApplicationModel.AppService

Public Class Extension
    Private _extension As AppExtension
    Private serviceName As String

    Public Sub New(extension As AppExtension)
        _extension = extension
        Load()
    End Sub

    Public Function ContainedIn(package As Package) As Boolean
        Return package.Id.FamilyName = _extension.Package.Id.FamilyName
    End Function

    Public Function SameAs(other As AppExtension) As Boolean
        Return _extension.AppInfo.AppUserModelId = other.AppInfo.AppUserModelId AndAlso _extension.Id = other.Id
    End Function

    Public Sub Update(extension As AppExtension)
        Unload()
        _extension = extension
        Load()
    End Sub

    Public Async Sub Load()
        Dim properties = Await _extension.GetExtensionPropertiesAsync()
        Dim obj = Nothing
        If properties.TryGetValue("ServiceName", obj) Then
            serviceName = TryCast(obj, String)
        End If
    End Sub

    Public Async Function Invoke(request As ValueSet) As Task(Of ValueSet)
        If _extension Is Nothing Then Throw New InvalidOperationException

        Using connection = New AppServiceConnection
            connection.AppServiceName = serviceName
            connection.PackageFamilyName = _extension.Package.Id.FamilyName
            Dim status = Await connection.OpenAsync()
            If status <> AppServiceConnectionStatus.Success Then Return Nothing
            Dim response = Await connection.SendMessageAsync(request)
            If response.Status <> AppServiceResponseStatus.Success Then Return Nothing
            Return response.Message
        End Using
    End Function

    Public Sub Unload()
        _extension = Nothing
    End Sub
End Class
