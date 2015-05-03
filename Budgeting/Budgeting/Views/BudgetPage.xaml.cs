using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Budgeting.ViewModels;
using Budgeting.Models;
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
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Popups;


namespace Budgeting.Views
{
 /*This Page lists off all Budgets that the user has uploaded and lets them focus there onwards. Note that we have instatiated our BudgetViewModel called 
  * bvm : bvm contains the observable collection of list of budgets for which Data Binding can occur to our XAML-based Interface
  */ 
  public sealed partial class BudgetPage : Page
    {
      private BudgetViewModel bvm;
      Windows.Storage.ApplicationDataContainer savedbudgetsettings = Windows.Storage.ApplicationData.Current.LocalSettings; //LocalSetting Storage
        public BudgetPage()
        {
           
            this.InitializeComponent();
            hidebar();
            
            bvm = new BudgetViewModel();
        }

        private async void hidebar()
        {
            StatusBar statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
            // Hide the status bar
            await statusBar.HideAsync();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            
            base.OnNavigatedTo(e);
            //Ensures that when the user adds a new Budget it will recognize that no editing is occuring
            savedbudgetsettings.Values["EditingBudget"] = false; 
            
  
            //Check if any budgets exist, if yes, the bind the collection of Budgets to our flyouts and BudgetListBox
            //In the XAML there is an ItemTemplate prepared such that for the ListBox of Budgets, it will take the "position" variable 
            //of the Budget and determine the relevant colour such that an alternating pattern is created for the user
            try
            {
               
                if ((bool)savedbudgetsettings.Values["Exists"] == true)
                {
                    await bvm.GetSavedBudgets();
                    BudgetButtonLB.ItemsSource = bvm.MyBudgets;
                    EditPickerFlyout.ItemsSource = bvm.MyBudgets;
                    DeletePickerFlyout.ItemsSource = bvm.MyBudgets;
                }
                
            }
            catch (NullReferenceException ex)
            {
                //If budget's dont exist, show the instructions
                InstructionBlock.Visibility = Visibility.Visible;    
            }

            

        }

        private void AddBudget_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(NewBudgetSetup));
        }


        private async void NavigateToHome(object sender, RoutedEventArgs e)
        {
           //Checks if the user has a passwrod before navigating away
            Button mybutton = (Button)sender;
            //Sets the CurrentBudget the button's information (DataContext which carries all the Budget Information)
            bvm.CurrentBudget = (BudgetModel)mybutton.DataContext;
            if (bvm.CurrentBudget.PwReq == true)
            {
                BudgetPasswordBox.IsEnabled = true;
                BudgetPasswordBox.Focus(FocusState.Programmatic);

                
            }
            else
            {
                //Serializes the CurrentBudget such that it can be accessed in the next BudgetHomePage
                await bvm.SaveCurrentBudget();
                Frame.Navigate(typeof(BudgetHomePage));
            }
        }

        private async void DeletingBudgetChosen(ListPickerFlyout sender, ItemsPickedEventArgs args)
        {
            //Asks if the user really wants to Delete the Budget
            var messageDialog = new MessageDialog("Are you sure you want to delete this Sub-Budget? All Events under this Sub-Budget will be deleted.");
            messageDialog.Commands.Add(new UICommand("Yes", new UICommandInvokedHandler(this.DeleteConfirmed)));
            messageDialog.Commands.Add(new UICommand("No"));
            messageDialog.DefaultCommandIndex = 0;
            messageDialog.CancelCommandIndex = 1;
            await messageDialog.ShowAsync();
        }

        private async void DeleteConfirmed(IUICommand command)
        {
            //Deletes the Budget and repositions all other Budgets
            bvm.MyBudgets.RemoveAt(DeletePickerFlyout.SelectedIndex);
            int reposition = 0;
            foreach (BudgetModel bm in bvm.MyBudgets)
            {
                bm.position = reposition;
                reposition++;
            }
            await bvm.SaveBudget();
            BudgetButtonLB.ItemsSource = bvm.MyBudgets;

            if (bvm.MyBudgets.Count < 1)
            {
                InstructionBlock.Visibility = Visibility.Visible;   
            }
        }

        private async void EditingBudgetChosen(ListPickerFlyout sender, ItemsPickedEventArgs args)
        {
            //Sets EditingBudget to true, sets the CurrentBudget to the chosen budget and serializes this information
            //This CurrentBudget will be reader under the fact that EditingBudget is true and will populate the "NewBudgetSetup" page
            //With the information as regards to the CurrentBudget such that users can edit
            savedbudgetsettings.Values["EditingBudget"] = true;
            bvm.CurrentBudget = bvm.MyBudgets[EditPickerFlyout.SelectedIndex];
            await bvm.SaveCurrentBudget();
            this.Frame.Navigate(typeof(NewBudgetSetup));
        }

        private async void SubmitPassword(object sender, RoutedEventArgs e)
        {
            if (BudgetPasswordBox.Password == bvm.CurrentBudget.Password)
            {
                await bvm.SaveCurrentBudget();
                Frame.Navigate(typeof(BudgetHomePage));
            }
            else
            {
                var messageDialog = new MessageDialog("Invalid password");
                await messageDialog.ShowAsync();
                BudgetPasswordBox.Password = string.Empty;
                BudgetPasswordBox.IsEnabled = false;
            }
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
