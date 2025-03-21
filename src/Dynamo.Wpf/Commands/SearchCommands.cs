using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace Dynamo.ViewModels
{
    partial class SearchViewModel
    {
        private ICommand focusSearch;
        public ICommand FocusSearchCommand
        {
            get { return focusSearch ?? (focusSearch = new RelayCommand<object>(FocusSearch, CanFocusSearch)); }
        }

        private ICommand search;
        public ICommand SearchCommand
        {
            get { return search ?? (search = new RelayCommand<object>(Search, CanSearch)); }
            internal set { search = value; }// used by tests
        }

        private ICommand showSearch;
        public ICommand ShowSearchCommand
        {
            get { return showSearch ?? (showSearch = new RelayCommand<object>(ShowSearch, CanShowSearch)); }
        }

        private ICommand hideSearch;
        public ICommand HideSearchCommand
        {
            get { return hideSearch ?? (hideSearch = new RelayCommand<object>(HideSearch, CanHideSearch)); }
        }

        public ICommand ImportLibraryCommand
        {
            get { return dynamoViewModel.ImportLibraryCommand; }
        }


        public ICommand ShowPackageManagerSearchCommand
        {
            get { return dynamoViewModel.ShowPackageManagerSearchCommand; }
        }

        private ICommand toggleLayoutCommand;
        public ICommand ToggleLayoutCommand
        {
            get { return toggleLayoutCommand ?? (toggleLayoutCommand = new RelayCommand<object>(ToggleLayout)); }
        }

        private ICommand selectAllCategoriesCommand;
        public ICommand SelectAllCategoriesCommand
        {
            get
            {
                return selectAllCategoriesCommand ??
                       (selectAllCategoriesCommand = new RelayCommand<object>(SelectAllCategories));
            }
        }
    }
}
