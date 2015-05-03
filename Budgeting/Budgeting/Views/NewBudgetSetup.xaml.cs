using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Budgeting.Models;
using Budgeting.ViewModels;
using System.Collections.ObjectModel;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Budgeting.Views
{
    //This Page simply allows the user to create a NewBudget, fulfilling all the information as shown in the BudgetModel
    public sealed partial class NewBudgetSetup : Page
    {
        private BudgetViewModel bvm;
        Windows.Storage.ApplicationDataContainer savedbudgetsettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        public NewBudgetSetup()
        {
            this.InitializeComponent();
            bvm = new BudgetViewModel();
        }


        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            //If other budgets exist then populate bvm.MyBudgets from deserialization, else create a new collection of BudgetModels
            try
            {
                bool savedsettings;
                savedsettings = (bool)savedbudgetsettings.Values["Exists"];

                if ((bool)savedbudgetsettings.Values["Exists"] == true)
                {
                    await bvm.GetSavedBudgets();
                }
                else
                {
                    bvm.MyBudgets = new ObservableCollection<BudgetModel>();
                }
               

            }
            catch (NullReferenceException ex)
            {
                bvm.MyBudgets = new ObservableCollection<BudgetModel>();
            }

            //If the user is Editing then populate all the textboxs and inputs with the information of CurrentBudget (first deserialize then populate)
            if ((bool)savedbudgetsettings.Values["EditingBudget"] == true)
            {
                await bvm.GetCurrentBudget();
                BudgetNameTextBox.Text = bvm.CurrentBudget.Name;
                BudgetPwCheckBox.IsChecked = bvm.CurrentBudget.PwReq;
                BudgetPassword.Password = bvm.CurrentBudget.Password;
                StartDatePicker.Date = bvm.CurrentBudget.startdate;
                EndDatePicker.Date = bvm.CurrentBudget.resetdate;
            }

        }

        private void DatePicker_Loaded(object sender, RoutedEventArgs e)
        {

            DatePicker picker = sender as DatePicker;
            picker.MinYear = DateTimeOffset.Now;
        }

        //Enables or Disables the password button
        private void BudgetPwCheckBox_State(object sender, RoutedEventArgs e)
        {
            
            if (BudgetPwCheckBox.IsChecked == true)
            {
                BudgetPassword.IsEnabled = true;
            }
            else
            {
                BudgetPassword.IsEnabled = false;
            }
        }

        //Changes the DatePickers according to the chosen reset time
        private void RB_Checked(object sender, RoutedEventArgs e)
        {

            var radio = sender as RadioButton;
            int RBValue = Convert.ToInt16(radio.Tag);

            switch (RBValue)
            {
                case 1:
                    EndDatePicker.Date = DateTimeOffset.Now.AddDays(7);
                    EndDatePicker.IsEnabled = false;
                    break;
                case 2:
                    EndDatePicker.Date = DateTimeOffset.Now.AddMonths(1);
                    EndDatePicker.IsEnabled = false;
                    break;
                case 3:
                    EndDatePicker.Date = DateTimeOffset.Now.AddYears(1);
                    EndDatePicker.IsEnabled = false;
                    break;
                case 4:
                    EndDatePicker.IsEnabled = true;
                    break;
            }

        }

        //Saves the information of the new added Budget
        private async void Complete_Click(object sender, RoutedEventArgs e)
        {


            Windows.Storage.ApplicationDataContainer subbudstates = Windows.Storage.ApplicationData.Current.LocalSettings;
            Windows.Storage.ApplicationDataContainer savedbudgetsettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            List<SubBudgetModel> mysub = new List<SubBudgetModel>();
            List<EventModel> myevents = new List<EventModel>();
            List<List<SubBudgetModel>> mysavedsubs = new List<List<SubBudgetModel>>();
            BudgetModel NewBudget = new BudgetModel(BudgetNameTextBox.Text, (bool)BudgetPwCheckBox.IsChecked, true, BudgetPassword.Password, StartDatePicker.Date, EndDatePicker.Date, mysub, myevents, mysavedsubs);

            //If the user is editing, find the currrent budget and update it
            if ((bool)savedbudgetsettings.Values["EditingBudget"] == true)
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
                        NewBudget.position = bvm.CurrentBudget.position;
                        NewBudget.Termnumber = bvm.CurrentBudget.Termnumber;
                        bvm.MyBudgets[bvm.MyBudgets.IndexOf(bm)] = NewBudget;

                    }
                }
                await bvm.SaveBudget(); //Serialize the budgets

            }
            else
            {
                //Else add a new Budget to this list and save the list of Budgets
                NewBudget.position = bvm.MyBudgets.Count + 1;
                NewBudget.Termnumber = 0;
                subbudstates.Values[NewBudget.Name] = false;
                savedbudgetsettings.Values["Exists"] = true;
                bvm.MyBudgets.Add(NewBudget);
                await bvm.SaveBudget();
            }
            



             Frame.Navigate(typeof(BudgetPage));
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

