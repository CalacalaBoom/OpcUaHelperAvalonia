using app.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Markup.Xaml;

namespace app.Views
{
    public partial class MainView : UserControl
    {
        private WindowNotificationManager? _manager;

        public MainView()
        {
            InitializeComponent();
        }

        //private void Dfs_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        //{
        //    _manager?.Show(new Notification("¥ÌŒÛ", "’À∫≈ªÚ√‹¬Î¥ÌŒÛ£°", NotificationType.Error));
        //}

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            var topLevel = TopLevel.GetTopLevel(this);
            _manager = new WindowNotificationManager(topLevel) { MaxItems = 3 };
            this.DataContext = new MainViewModel(_manager);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}