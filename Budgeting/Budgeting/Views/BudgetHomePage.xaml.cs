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
using Budgeting.Models;
using Budgeting.ViewModels;
using Budgeting.Converters;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;
using System.Collections.ObjectModel;
using Windows.Graphics.Display;
using Windows.UI.Popups;
using Windows.UI;
using Windows.UI.Text;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Budgeting.Views
{
   //The BudgetHomePage is the main page for the application essentially: It carries the most versatility and usability for the users to engage and
   //personalize their budgets accordingly
   
    public sealed partial class BudgetHomePage : Page
    {
        public BudgetViewModel bvm;

   
        Windows.Storage.ApplicationDataContainer subbudstates = Windows.Storage.ApplicationData.Current.LocalSettings;
        Windows.Storage.ApplicationDataContainer savedbudgetsettings = Windows.Storage.ApplicationData.Current.LocalSettings;



        public BudgetHomePage()
        
        {
            
            this.InitializeComponent();
            bvm = new BudgetViewModel();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;   //Ensures we are in portrait mode

           

        }

       
//Navigates to add a New SubBudget
        private void NavigateToNewSB(object sender, RoutedEventArgs e) {
            Frame.Navigate(typeof(NewSubBudgetPage));
    }

        protected async  override void OnNavigatedTo(NavigationEventArgs e)
        {

            base.OnNavigatedTo(e);
            savedbudgetsettings.Values["EditSB"] = false;

            //Populates bvm with Budget Information
            await bvm.GetSavedBudgets();
            await bvm.GetCurrentBudget();

            //If the Current Budget has no budgets, do not let them Edit/Delete or see statistics
            if (bvm.CurrentBudget.SubBudgets.Count < 1)
            {
                GraphButton.IsEnabled = false;
                EditSubBudget.IsEnabled = false;
                DeleteSubBudget.IsEnabled = false;
            }
            else
            {
                GraphButton.IsEnabled = true;
                EditSubBudget.IsEnabled = true;
                DeleteSubBudget.IsEnabled = true;
            }

            //Checks if it is a new term, and updates start and reset dates accordingly
            if (DateTime.Today.DayOfYear >= bvm.CurrentBudget.resetdate.DayOfYear)
            {
                bvm.CurrentBudget.Termnumber++;
                foreach (SubBudgetModel sbm in bvm.CurrentBudget.SubBudgets)
                {
                    sbm.Remainder = sbm.SBLimit;
                }
                bvm.CurrentBudget.startdate = DateTime.Today;
                bvm.CurrentBudget.resetdate = DateTime.Today.AddDays(bvm.CurrentBudget.resetdate.DayOfYear - bvm.CurrentBudget.startdate.DayOfYear);
                SaveToBudgets();
            }

            //Binds Flyouts with the SubBudgets from bvm; In the XAML code these flyouts have ItemTemplates to show the information properly
            this.DataContext = bvm.CurrentBudget;
            EditFlyout.ItemsSource = bvm.CurrentBudget.SubBudgets;
            DeleteFlyout.ItemsSource = bvm.CurrentBudget.SubBudgets;
            SubBudEventListBox.SelectedIndex = -1;         

            try
            {
                if (bvm.CurrentBudget.SubBudgets.Count > 0)
                {

                    SubFlyout.ItemsSource = bvm.CurrentBudget.SubBudgets;
                    NewEventButton.IsEnabled = true;
                    SearchButton.IsEnabled = true;
                }
            }
            catch (NullReferenceException ex)
            {

            }
            


          }




   

       //Populates the information block of a specific sub-budget with specific information using bvm.CurrentSubBudget
        private  void SelectionChanged(ListPickerFlyout sender, ItemsPickedEventArgs args)
        {

            bvm.CurrentSubBudget = (SubBudgetModel)SubFlyout.SelectedItem;
            
           if (bvm.CurrentSubBudget.LimitBool == true)
           {
               LimitBlock.Text = bvm.CurrentSubBudget.SBLimit.ToString();
               RemainderBlock.Text = bvm.CurrentSubBudget.Remainder.ToString();
               double percentspent = bvm.CurrentSubBudget.Remainder / bvm.CurrentSubBudget.SBLimit;
               if (percentspent <= 0.5 && percentspent >= 0.25)
               {
                   RemainderBlock.Foreground = new SolidColorBrush(ColorHelper.FromArgb(255, 232, 255, 0));

               }
               else if (percentspent < 0.25)
               {
                   RemainderBlock.Foreground = new SolidColorBrush(ColorHelper.FromArgb(255, 255, 0, 0));
                   RemainderBlock.FontWeight = FontWeights.Bold;
               }
           }
           else
           {
               LimitBlock.Text = "N/a";
               RemainderBlock.Text = "N/a";
           }
           int count = 0;
           foreach (EventModel em in bvm.CurrentBudget.Events)
           {
               if (bvm.CurrentSubBudget.Name.Equals(em.SubBudgetName))
               {
                   count++;
               }
           }
           EventCountBlock.Text = count.ToString();
           try
           {
               List<EventModel> subbudgetevents = new List<EventModel>();

               foreach (EventModel em in bvm.CurrentBudget.Events)
               {
                   if (em.SubBudgetName.Equals(bvm.CurrentSubBudget.Name))
                   {
                       subbudgetevents.Add(em);

                   }
               }
               SubBudEventListBox.ItemsSource = subbudgetevents;
               
           }
           catch (NullReferenceException ex)
           {
           }
            

         

        }
        //Navigates to a new event page
        private  async void NewEvent_Click(object sender, RoutedEventArgs e)
        {
            bvm.CurrentEvent = new EventModel();
            await bvm.SaveCurrentEvent();
            this.Frame.Navigate(typeof(NewEventSetupPage));
        }

        //Search function that loops through all of bvm's events and populates the search listbox with every relevant event
        //In the XAML code there is a DataTemplate that uses Data Binding to present the information in grid style with the event image and colour, etc.
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            
            ObservableCollection<EventModel> mylist = new ObservableCollection<EventModel>();
            try {
            string mysearch = SearchTextBox.Text;
                foreach(EventModel em in bvm.CurrentBudget.Events) {
                    foreach (string tag in em.Tags)
                    {
                        if (mysearch == tag) {
                            mylist.Add(em);
                            break;
                        }
                    }
                }
            
            }
            catch (NullReferenceException ex) {

            }
            
            try {
                SearchListBox.ItemsSource = mylist;
            }
            catch (NullReferenceException ex) {

}
            

        }

        //Navigates to the sepcific EventDetails page and sets bvm.CurrentEvent to the chosen event and serializes
        private async void ViewEventDetails(object sender, SelectionChangedEventArgs e)
        {
           
            ListBox mybox = (ListBox)sender;
            if (mybox.SelectedIndex > -1)
            {
                bvm.CurrentEvent = (EventModel)mybox.SelectedItem;
                await bvm.SaveCurrentEvent();
                this.Frame.Navigate(typeof(EventDetails));
            }
        }

       

        //Navigates to StatisticsPage
        private void StartGraph_Click(object sender, RoutedEventArgs e)
        {

               

            this.Frame.Navigate(typeof(GraphDisplayPage));

        }

        //Edits the SubBudget
        private void GoToEdit(ListPickerFlyout sender, ItemsPickedEventArgs args)
        {
            
            savedbudgetsettings.Values["EditSB"] = true;
            savedbudgetsettings.Values["SBIndex"] = sender.SelectedIndex;
            this.Frame.Navigate(typeof(NewSubBudgetPage));
        }

        //Asks if the user wants to Delete a SubBudget
        private async void SubBudgetDeleteEvent(ListPickerFlyout sender, ItemsPickedEventArgs args)
        {
            var messageDialog = new MessageDialog("Are you sure you want to delete this Sub-Budget? All Events under this Sub-Budget will be deleted.");
            messageDialog.Commands.Add(new UICommand("Yes",new UICommandInvokedHandler(this.DeleteConfirmed)));
            messageDialog.Commands.Add(new UICommand("No"));
            messageDialog.DefaultCommandIndex = 0;
            messageDialog.CancelCommandIndex = 1;
            await messageDialog.ShowAsync();
        }

        //Delete SubBudget and all Events under the same name as this SubBudget.
        private  void DeleteConfirmed(IUICommand command)
        {

            try
            {
                List<EventModel> subEMlist = bvm.CurrentBudget.Events;
                foreach (EventModel em in subEMlist)
                {
                    if (em.SubBudgetLocation.Equals(bvm.CurrentBudget.SubBudgets[DeleteFlyout.SelectedIndex]))
                    {
                        bvm.CurrentBudget.Events.RemoveAt(subEMlist.IndexOf(em));
                    }
                }
            }
            catch
            {

            }

            bvm.CurrentBudget.SubBudgets.RemoveAt(DeleteFlyout.SelectedIndex);




            SaveToBudgets();
            try
            {
                SubBudEventListBox.Items.Clear();
            }
            catch
            {

            }

            try
            {
                SearchListBox.Items.Clear();
            }
            catch
            {

            }
            SubFlyout.ItemsSource = bvm.CurrentBudget.SubBudgets;
            
            EditFlyout.ItemsSource = bvm.CurrentBudget.SubBudgets;
            DeleteFlyout.ItemsSource = bvm.CurrentBudget.SubBudgets;
            //this.Frame.Navigate(typeof(BudgetHomePage));
        }
       
        //Common method that updates the CurrentBudget: However, the entire budgetviewmodel must recognize this change, and hence updates MyBudgets, serializing
        //both CurrentBudget AND MyBudgets
        private async void SaveToBudgets()
        {
            List<BudgetModel> substitutelist = new List<BudgetModel>();

            foreach (BudgetModel bm in bvm.MyBudgets)
            {
                substitutelist.Add(bm);
            }

            foreach (BudgetModel bm in substitutelist)
            {
                if (bm.Name.Equals(bvm.CurrentBudget.Name))
                {
                    bvm.MyBudgets[bvm.MyBudgets.IndexOf(bm)] = bvm.CurrentBudget;

                }
            }
            await bvm.SaveBudget();
            await bvm.SaveCurrentBudget();
        }
        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }

       
        

        


    }
   

  
}
