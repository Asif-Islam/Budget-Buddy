using Budgeting.Converters;
using Budgeting.Models;
using Budgeting.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Budgeting.Views
{
    //This is a summary page for all statistics
    public sealed partial class GraphDisplayPage : Page
    {
        Windows.Storage.ApplicationDataContainer savedbudgetsettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        private BudgetViewModel bvm;


        public GraphDisplayPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            bvm = new BudgetViewModel();
        }

        //Initializes the Termlistbox very Term there has been since the budget started
        private void InitializeTermListBox()
        {
            List<string> Terms = new List<string>();
            for (int x = 0; x <= bvm.CurrentBudget.Termnumber; x++) {
                Terms.Add("Term " + x.ToString());
            }
            TermPicker.ItemsSource = Terms;
        }

        //Initialize pie chart, seting it's DataBinding to the Name and Limit variables (from SubBudgetModel) as it's two pie chart variables
        private void InitializeData()
        {
            
            PieSeries pie;
            PieChart.Series.Clear();
            pie = new PieSeries();
            pie.Title = "Budget Portioning Graph";
            //Sets the x and y of hte pie chart and then sets the itemmsource to the subbudgets to read the Name and Limit only
            pie.IndependentValueBinding = GetBinding("Name", false);
            pie.DependentValueBinding = GetBinding("SBLimit", false);
            pie.ItemsSource = bvm.CurrentBudget.SubBudgets;
            this.PieChart.Series.Add(pie);


            //Initialize the ProgressBars of each subbudget (XAML Template has Progress Bars with it's value binded to the SubBudget Limit/Remainder
            Progressbox.ItemsSource = bvm.CurrentBudget.SubBudgets;
        }


        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape;
            await bvm.GetCurrentBudget();
            InitializeData(); //Calls to intialize the PieChart and Progress Bar
            SBChooserListPicker.ItemsSource = bvm.CurrentBudget.SubBudgets; //Populates with subbudget list (XAML template to present information)
            InitializeTermListBox(); //Initializes the termlistbox
          

        }


        //Determines a binding path based on the variable name (C# Code Behind approach to DataBinding)
         private Binding GetBinding(string PropName, bool IsDateTime)
    {
        Binding bind = new Binding();
        bind.Path = new PropertyPath(PropName);

        if (IsDateTime) //If it is a Date, set the converter to the local Converter class to a more readable format
        {
            bind.Converter = new DateToStringConverter();
        }
        return bind;
    }

         private void GoBack_Click(object sender, RoutedEventArgs e)
         {
             DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;   
             if (this.Frame.CanGoBack)
             {
                 this.Frame.GoBack();
             }
         }


        //Starts the Area Chart
         private void StartGraph_Clicked(object sender, RoutedEventArgs e)
         {
             List<EventModel> graphevents = new List<EventModel>(); //List of Relevant Events
             List<EventModel> items = new List<EventModel>(); //List of Events that is changed to accumulate for the Area Chart
           
             SubBudgetModel chosensbm = (SubBudgetModel)SBChooserListPicker.SelectedItem; //Determines chosen SubBudgetModel
          
            /*This algorithm loops through event in bvm.CurrentBudget.Events. First it checks if it is in the proper SubBudget at (a), then checks if 
             * it is within the date range (b), then determines whether it has a unique date: If it does, add it to the list and update the total price
             *as well as setting the most recentdate to that it's date (c), if the date is not unique then go to the most recent event just added and add
             *that event's price to the previous event --> this is point of accumulation for hte area chart (d)
             */
             int recentdate = -1;
              double addedprice = 0;

              for (int i = bvm.CurrentBudget.Events.Count - 1; i >= 0; i--)
              {
                  if (bvm.CurrentBudget.Events[i].SubBudgetName.Equals(chosensbm.Name)) //(a)
                  {
                      if (bvm.CurrentBudget.Events[i].EventDate.DayOfYear >= StartDatePicker.Date.DayOfYear && bvm.CurrentBudget.Events[i].EventDate.DayOfYear <= EndDatePicker.Date.DayOfYear) {
                          if (!(bvm.CurrentBudget.Events[i].EventDate.DayOfYear == recentdate)) //(b)
                          {
                              EventModel addedevent = new EventModel(); //(c)
                              addedevent.EventDate = bvm.CurrentBudget.Events[i].EventDate;
                              if (bvm.CurrentBudget.Events[i].isDeducted == true)
                              {
                                  addedevent.Price = bvm.CurrentBudget.Events[i].Price + addedprice;
                              }
                              else 
                              {
                                  addedevent.Price = addedprice - bvm.CurrentBudget.Events[i].Price;
                              }
                              addedprice = addedevent.Price;
                              items.Add(addedevent);
                              recentdate = bvm.CurrentBudget.Events[i].EventDate.DayOfYear;
                          }
                          else //(d)
                          {
                              if (bvm.CurrentBudget.Events[i].isDeducted == true)
                              {
                                  items[items.Count - 1].Price += bvm.CurrentBudget.Events[i].Price;
                                  addedprice = items[items.Count - 1].Price;
                              }
                              else
                              {
                                  items[items.Count - 1].Price -= bvm.CurrentBudget.Events[i].Price;
                                  addedprice = items[items.Count - 1].Price;

                              }
                          }
                          graphevents.Add(bvm.CurrentBudget.Events[i]); //Adds the unchanged relevant events to graphevents
                      }
                     
                  }
              }

             //Creates the Area series
              AreaSeries Area = new AreaSeries();

              AreaChart.Series.Clear();
              Area.Title = "Accumulative";

              Area.IndependentValueBinding = GetBinding("EventDate", true);
              Area.DependentValueBinding = GetBinding("Price", false);

              Area.ItemsSource = items;
              this.AreaChart.Series.Add(Area);

             //Sets the interval such that it's the difference of the minimum and maximum value divided by 5
              double interval = (items[items.Count - 1].Price - items[0].Price) / 5;
              ((AreaSeries)AreaChart.Series[0]).DependentRangeAxis = new LinearAxis()
              {
                  Minimum = items[0].Price - interval, //minimum set the the lowest value minus one whole interval
                  Maximum = items[items.Count - 1].Price + interval, //maximum set to highest plus one whole interval
                  Orientation = AxisOrientation.Y,
                  Location = AxisLocation.Left,
                  ShowGridLines = true,
                  Interval = interval, //sets interval
              };

              GraphEventList.ItemsSource = graphevents; //Sets the relevant event list itemssource to this saved graphevents so users can examine
             //which events are in their graph

             
         }


         private void SBChooserItemsPicked(ListPickerFlyout sender, ItemsPickedEventArgs args)
         {
             SBchooser.Content = ((SubBudgetModel)SBChooserListPicker.SelectedItem).Name;
         }

        //Navigates to examine a specific event
         private async void GraphEventItemsPicked(ListPickerFlyout sender, ItemsPickedEventArgs args)
         {
             bvm.CurrentEvent = (EventModel)sender.SelectedItem;
             await bvm.SaveCurrentEvent();
             DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;   
             this.Frame.Navigate(typeof(EventDetails));
         }

        //Executes the same code as StartGraph_Clicked except no accumulation occurs
         private void StartKeyGraph_Clicked(object sender, RoutedEventArgs e)
         {
             ObservableCollection<EventModel> graphevents = new ObservableCollection<EventModel>();
             ObservableCollection<EventModel> items = new ObservableCollection<EventModel>();

             int recentdate = -1;
             //maximum and minimum must be manually determined as this is a line chart, the first event might not be lower than the last point
             //comparison at each event is executed to see if it is a maximum or minimum
             double minimum = 10000;
             double maximum = -1;
             
             
             //for (int i = bvm.CurrentBudget.Events.Count - 1; i >= 0; i--)
             for (int i = bvm.CurrentBudget.Events.Count - 1; i >= 0; i--)
             {
                 foreach (string s in bvm.CurrentBudget.Events[i].Tags)
                 {
                     if (KeyWordBox.Text.Equals(s))
                     {
                         if (!(bvm.CurrentBudget.Events[i].EventDate.DayOfYear == recentdate))
                         {
                             EventModel addedevent = new EventModel();
                             addedevent.Price = bvm.CurrentBudget.Events[i].Price;
                             addedevent.EventDate = bvm.CurrentBudget.Events[i].EventDate;
                             items.Add(addedevent);
                             recentdate = bvm.CurrentBudget.Events[i].EventDate.DayOfYear;
                             
                             if (addedevent.Price < minimum) {
                                 minimum = addedevent.Price;
                             }
                             if (addedevent.Price > maximum) {
                                 maximum = addedevent.Price;
                             }
                         }
                         else
                         {
                             items[items.Count - 1].Price += bvm.CurrentBudget.Events[i].Price;
                             if (items[items.Count - 1].Price > maximum)
                             {
                                 maximum = items[items.Count - 1].Price;
                             }
                         }
                         graphevents.Add(bvm.CurrentBudget.Events[i]);
                         break;
                     }
                 }

                 
             }

             GraphEventList.ItemsSource = graphevents;


             LineSeries line = new LineSeries();

             KeywordChart.Series.Clear();
             line.Title = "Keyword";

             line.IndependentValueBinding = GetBinding("EventDate", true);
             line.DependentValueBinding = GetBinding("Price", false);
             //line.ItemsSource = bvm.CurrentBudget.Events;
             line.ItemsSource = items;
             this.KeywordChart.Series.Add(line);

             double interval = (maximum - minimum) / 5;
             ((LineSeries)KeywordChart.Series[0]).DependentRangeAxis = new LinearAxis()
             {
                 Minimum = minimum - interval,
                 Maximum = maximum + interval,
                 Orientation = AxisOrientation.Y,
                 Location = AxisLocation.Left,
                 ShowGridLines = true,
                 Interval = interval,
             };

         }

         private void TermChosen(ListPickerFlyout sender, ItemsPickedEventArgs args)
         {
             ReportListBox.ItemsSource = bvm.CurrentBudget.savedsubbudgets[bvm.CurrentBudget.Termnumber];
         }



        }

    //Converts Date to "MM/dd" form
      public class DateToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string Language)
        {
            return ((DateTimeOffset)value).ToString("MM/dd", System.Globalization.CultureInfo.InvariantCulture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }



    }

