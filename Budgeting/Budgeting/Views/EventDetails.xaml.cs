using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Budgeting.ViewModels;
using Budgeting.Models;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Budgeting.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EventDetails : Page
    {
        public BudgetViewModel bvm;
        public EventDetails()
        {
            this.InitializeComponent();
            bvm = new BudgetViewModel();
            //this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        }


        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await bvm.GetCurrentEvent();
            this.DataContext = bvm.CurrentEvent;
            string mytags ="";

            foreach (string tag in bvm.CurrentEvent.Tags)
            {
                mytags = mytags + tag + ", ";
            }
            dateblock.Text = bvm.CurrentEvent.EventDate.ToString();
            tagblock.Text = mytags;



            try
            {
                List<BitmapImage> myimages = new List<BitmapImage>();
                foreach (string filename in bvm.CurrentEvent.photonames)
                {
                    StorageFile capturefile = await ApplicationData.Current.LocalFolder.GetFileAsync(filename);
                    BitmapImage img = new BitmapImage(new Uri(capturefile.Path));
                    myimages.Add(img);
                }
                PhotoListbox.ItemsSource = myimages;
            }
            catch
            {

            }

        }

        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            //this.Frame.Navigate(typeof(BudgetHomePage));
           if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }

    }
}
