using System;
using Meowtrix.MetaCent.ViewModels;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace Meowtrix.MetaCent.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly MRUList MRU;
        public MainPage()
        {
            this.InitializeComponent();
            MRU = new MRUList(Dispatcher);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
        }

        private async void NewRepo(object sender, TappedRoutedEventArgs e)
        {
            e.Handled = true;
            var picker = new FolderPicker();
            picker.FileTypeFilter.Add("*");
            picker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            var folder = await picker.PickSingleFolderAsync();
            if (folder != null)
                MRU.Add(folder, folder.DisplayName);
        }

        private async void OpenRepo(object sender, TappedRoutedEventArgs e)
        {
            var data = (sender as FrameworkElement)?.DataContext as MRUEntry;
            if (data == null) return;
            List.IsEnabled = false;
            var folder = await data.GetFolderAsync();
            if (folder == null)
            {
                var dialog = new ContentDialog
                {
                    Content = "Failed to open the repository. Remove it?",
                    PrimaryButtonText = "Remove",
                    SecondaryButtonText = "Cancel"
                };
                if (await dialog.ShowAsync() == ContentDialogResult.Primary)
                    data.Remove();
            }
            else
            {
                (Window.Current.Content as Frame)?.Navigate(typeof(RepoPage), folder);
            }
            List.IsEnabled = true;
        }
    }
}
