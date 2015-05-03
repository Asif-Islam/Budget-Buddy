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
using Windows.UI;
using Budgeting.Converters;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Budgeting.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NewSubBudgetPage : Page
    {
        private BudgetViewModel bvm;
        private SubBudgetViewModel sbvm;
        private int editindex;
        Windows.Storage.ApplicationDataContainer savedbudgetsettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        public NewSubBudgetPage()
        {
            this.InitializeComponent();
            sbvm = new SubBudgetViewModel();
            bvm = new BudgetViewModel();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await bvm.GetSavedBudgets();
             await bvm.GetCurrentBudget();
             ColorList colorlist = new ColorList();
             List<SolidColorBrush> mycolors = new List<SolidColorBrush>();
             foreach (Color color in colorlist.colors)
             {
                 mycolors.Add(new SolidColorBrush(color));
             }
             /*mycolors.Add(new SolidColorBrush(Colors.Red));
             mycolors.Add(new SolidColorBrush(Colors.Blue));
             mycolors.Add(new SolidColorBrush(Colors.Green));
             mycolors.Add(new SolidColorBrush(Colors.Orange));
             mycolors.Add(new SolidColorBrush(Colors.Purple));
             mycolors.Add(new SolidColorBrush(Colors.Magenta));
             mycolors.Add(new SolidColorBrush(Colors.Brown);
            mycolors.Add(new SolidColorBrush(Colors.Pink));
            mycolors.Add(new SolidColorBrush(Colors.DodgerBlue));
            mycolors.Add(new SolidColorBrush(Colors.DeepSkyBlue));
            mycolors.Add(new SolidColorBrush(Colors.Crimson));
            mycolors.Add(new SolidColorBrush(Colors.Lime));
            mycolors.Add(new SolidColorBrush(Colors.PapayaWhip));
            mycolors.Add(new SolidColorBrush(Colors.));*/
             ColorListPick.ItemsSource = mycolors;
             ColorListPick.SelectedIndex = 0;
             Colorchooser.Background = (SolidColorBrush)ColorListPick.SelectedItem;

                 if ((bool)savedbudgetsettings.Values["EditSB"] == true)
                 {
                     editindex = (int)savedbudgetsettings.Values["SBIndex"];
                     SBudgetNameTextBox.Text = bvm.CurrentBudget.SubBudgets[editindex].Name;
                     SBudgetLimitCheck.IsChecked = bvm.CurrentBudget.SubBudgets[editindex].LimitBool;
                     SBudgetLimitTextbox.Text = bvm.CurrentBudget.SubBudgets[editindex].SBLimit.ToString();
                     //Add Color to this
                 }
             
 //SET TO FALSE ON HOMEPAGE NAVIGATION
        }

        private void SBudgetLimitCheckBox_State(object sender, RoutedEventArgs e)
        {
         
            if (SBudgetLimitCheck.IsChecked == true)
            {
                SBudgetLimitTextbox.IsEnabled = true;
            }
            else
            {
                SBudgetLimitTextbox.IsEnabled = false;
                SBudgetLimitTextbox.Text = "0";
            }
        
        }

        private async void Complete_Click(object sender, RoutedEventArgs e)
        {

            //SBudgetNameTextBox.Text = ((SolidColorBrush)ColorListPick.SelectedItem).Color.ToString();
            Color chosencolor = ((SolidColorBrush)ColorListPick.SelectedItem).Color;
            List<byte> colorbytes = new List<byte>();
            colorbytes.Add(chosencolor.A);
            colorbytes.Add(chosencolor.R);
            colorbytes.Add(chosencolor.G);
            colorbytes.Add(chosencolor.B);
            SubBudgetModel NewSubBudget = new SubBudgetModel(SBudgetNameTextBox.Text, (bool)SBudgetLimitCheck.IsChecked, Convert.ToDouble(SBudgetLimitTextbox.Text), Convert.ToDouble(SBudgetLimitTextbox.Text), colorbytes);

            if ((bool)savedbudgetsettings.Values["EditSB"] == true)
            {
                bvm.CurrentBudget.SubBudgets[editindex] = NewSubBudget;
            }
            else
            {
                bvm.CurrentBudget.SubBudgets.Add(NewSubBudget);
            }//NULLREFERENCEEXCEPTION FIX
            try
            {
                bvm.CurrentBudget.savedsubbudgets[bvm.CurrentBudget.Termnumber] = bvm.CurrentBudget.SubBudgets;
            }
            catch
            {
                bvm.CurrentBudget.savedsubbudgets.Add(bvm.CurrentBudget.SubBudgets);
            }
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
            Frame.Navigate(typeof(BudgetHomePage));

            
        }

        private void SelectionChanged(ListPickerFlyout sender, ItemsPickedEventArgs args)
        {
            Colorchooser.Background = (SolidColorBrush)ColorListPick.SelectedItem;
        }

        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

    }



}
