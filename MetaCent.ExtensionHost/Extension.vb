Option Compare Binary
Imports Windows.ApplicationModel.AppExtensions

Public Class Extension
    Private _extension As AppExtension

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

    Public Sub Load()

    End Sub

    Public Sub Unload()

    End Sub
End Class
