Imports Windows.ApplicationModel.AppExtensions

Public Class ExtensionHost
    Private WithEvents Catalog As AppExtensionCatalog
    Private ReadOnly m_extensions As New ObservableCollection(Of Extension)

    Public ReadOnly Property Extensions As ReadOnlyObservableCollection(Of Extension)

    Public Sub New()
        Extensions = New ReadOnlyObservableCollection(Of Extension)(m_extensions)
        Catalog = AppExtensionCatalog.Open("Meowtrix.MetaCent.Provider")
        Initialize()
    End Sub

    Private Async Sub Initialize()
        Load(Await Catalog.FindAllAsync())
    End Sub

    Private Sub Catalog_PackageInstalled(catalog As AppExtensionCatalog, args As AppExtensionPackageInstalledEventArgs) Handles Catalog.PackageInstalled
        Load(args.Extensions)
    End Sub

    Private Sub Catalog_PackageUninstalling(catalog As AppExtensionCatalog, args As AppExtensionPackageUninstallingEventArgs) Handles Catalog.PackageUninstalling
        For Each e In GetContaining(args.Package).ToList()
            e.Unload()
            m_extensions.Remove(e)
        Next
    End Sub

    Private Sub Catalog_PackageStatusChanged(catalog As AppExtensionCatalog, args As AppExtensionPackageStatusChangedEventArgs) Handles Catalog.PackageStatusChanged
        Dim status = args.Package.Status
        If status.VerifyIsOK() Then
            For Each ext In GetContaining(args.Package)
                ext.Load()
            Next
        ElseIf status.PackageOffline Then
            For Each ext In GetContaining(args.Package)
                ext.Unload()
            Next
        ElseIf status.Servicing OrElse status.DeploymentInProgress Then 'do nothing
        Else
            For Each ext In GetContaining(args.Package).ToList()
                ext.Unload()
                m_extensions.Remove(ext)
            Next
        End If
    End Sub

    Private Sub Catalog_PackageUpdating(catalog As AppExtensionCatalog, args As AppExtensionPackageUpdatingEventArgs) Handles Catalog.PackageUpdating
        For Each e In GetContaining(args.Package)
            e.Unload()
        Next
    End Sub

    Private Sub Catalog_PackageUpdated(catalog As AppExtensionCatalog, args As AppExtensionPackageUpdatedEventArgs) Handles Catalog.PackageUpdated
        Load(args.Extensions)
    End Sub

    Private Function GetContaining(package As Package) As IEnumerable(Of Extension)
        Return From ext In m_extensions Where ext.ContainedIn(package)
    End Function

    Private Sub Load(extensions As IEnumerable(Of AppExtension))
        For Each e In extensions
            Dim existing = (From ext In m_extensions Where ext.SameAs(e)).FirstOrDefault()
            If existing IsNot Nothing Then
                existing.Update(e)
            Else
                m_extensions.Add(New Extension(e))
            End If
        Next
    End Sub
End Class
