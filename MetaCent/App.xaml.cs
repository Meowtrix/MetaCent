using System;
using Meowtrix.MetaCent.Views;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Meowtrix.MetaCent
{
    /// <summary>
    /// 提供特定于应用程序的行为，以补充默认的应用程序类。
    /// </summary>
    public sealed partial class App : Application
    {
        /// <summary>
        /// 初始化单一实例应用程序对象。这是执行的创作代码的第一行，
        /// 已执行，逻辑上等同于 main() 或 WinMain()。
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// 在应用程序由最终用户正常启动时进行调用。
        /// 将在启动应用程序以打开特定文件等情况下使用。
        /// </summary>
        /// <param name="e">有关启动请求和过程的详细信息。</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            if (e.PrelaunchActivated == false)
            {
                if (e.ViewSwitcher == null)
                {
                    ApplicationViewSwitcher.DisableSystemViewActivationPolicy();
                    InitialzeCurrentWindow();
                }
                else
                {
                    CreateNewWindow();
                }
            }
        }

        /// <summary>
        /// 在将要挂起应用程序执行时调用。  在不知道应用程序
        /// 无需知道应用程序会被终止还是会恢复，
        /// 并让内存内容保持不变。
        /// </summary>
        /// <param name="sender">挂起的请求的源。</param>
        /// <param name="e">有关挂起请求的详细信息。</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: 保存应用程序状态并停止任何后台活动
            deferral.Complete();
        }

        public async void CreateNewWindow()
        {
            var newView = CoreApplication.CreateNewView();
            int newAppViewId = 0;
            await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                newAppViewId = InitialzeCurrentWindow();
            });
            await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newAppViewId);
        }

        private int InitialzeCurrentWindow()
        {
            var rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null)
            {
                rootFrame = new Frame();
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
                rootFrame.Navigate(typeof(MainPage), null);

            SystemNavigationManager.GetForCurrentView().BackRequested += App_BackRequested;

            Window.Current.Activate();

            var view = ApplicationView.GetForCurrentView();
            var titleBar = view.TitleBar;

            view.Consolidated += (_, __) => Window.Current.Content = null;

            titleBar.BackgroundColor = Colors.Transparent;
            titleBar.InactiveBackgroundColor = Colors.Transparent;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;

            return view.Id;
        }

        private void App_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (Window.Current.Content is Frame rootFrame
                && rootFrame.CanGoBack
                && e.Handled == false)
            {
                e.Handled = true;
                rootFrame.GoBack();
            }
        }
    }
}
