using CommunityToolkit.Mvvm.Input;
using Dynamo.ViewModels;

namespace Dynamo.Wpf.ViewModels.Core
{
    internal class ShortcutToolbarViewModel : ViewModelBase
    {
        /// <summary>
        /// Exports an image from the user's 3D background or workpace
        /// </summary>
        public RelayCommand<object> ValidateWorkSpaceBeforeToExportAsImageCommand { get; set; }

        public RelayCommand SignOutCommand { get; set; }

        private int notificationsNumber;
        private bool showMenuItemText;
        public DynamoViewModel DynamoViewModel { get; private set; }

        public ShortcutToolbarViewModel(DynamoViewModel dynamoViewModel)
        {
            this.DynamoViewModel = dynamoViewModel;
            NotificationsNumber = 0;
            ValidateWorkSpaceBeforeToExportAsImageCommand = new RelayCommand<object>(dynamoViewModel.ValidateWorkSpaceBeforeToExportAsImage);
            this.DynamoViewModel.WindowRezised += OnDynamoViewModelWindowRezised;
            ShowMenuItemText = true;
        }

        private void OnDynamoViewModelWindowRezised(object sender, System.EventArgs e)
        {
            if (sender is Boolean) ShowMenuItemText = !(bool)sender;
        }

        /// <summary>
        /// This property represents the number of new notifications 
        /// </summary>
        public int NotificationsNumber
        {
            get
            {
                return notificationsNumber;
            }
            set
            {
                notificationsNumber = value;
                OnPropertyChanged(nameof(IsNotificationsCounterVisible));
            }
        }



        public bool IsNotificationsCounterVisible
        {
            get
            {
                if (NotificationsNumber == 0)
                {
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Indicates if a MenuItem display its text or not (only Icon)
        /// </summary>
        public bool ShowMenuItemText
        {
            get
            {
                return showMenuItemText;
            }
            set
            {
                showMenuItemText = value;
                OnPropertyChanged(nameof(ShowMenuItemText));
            }
        }

        public override void Dispose()
        {
            this.DynamoViewModel.WindowRezised -= OnDynamoViewModelWindowRezised;
            base.Dispose();
        }
    }
}
