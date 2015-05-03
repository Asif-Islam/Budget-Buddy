using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;



namespace Budgeting.Views
{
   
    /*This is the Main Page of the Application: The only importance is to this page is that it is the opening naviagation to the BudgetPage
     */
    public sealed partial class MainPage : Page
    {
        Windows.Storage.ApplicationDataContainer savedbudgetsettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            hidebar();


            
        }
        //Navigate To Budget Page
        private void GoToBudgetPage_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(BudgetPage));
        }
      
       
        private async void hidebar()
        {
            StatusBar statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
            // Hide the status bar
            await statusBar.HideAsync();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }       

    }
}
