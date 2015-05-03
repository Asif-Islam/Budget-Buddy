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
using Budgeting.Converters;
using Budgeting.Models;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Budgeting.Views
{
    //This is Page that allows users to Setup a New Event in their Budget
    public sealed partial class NewEventSetupPage : Page
    {
        private BudgetViewModel bvm;

        Windows.Storage.ApplicationDataContainer savedbudgetsettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        public NewEventSetupPage()

        {
            this.InitializeComponent();
            bvm = new BudgetViewModel();

            setComboBoxes();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;

            
            //SBBox.CacheMode = new BitmapCache();
        }

        //Populates the ComboBoxes with a) The possible Sub Budgets and b) The possible Pay Methods
        private async void setComboBoxes(){
         await bvm.GetCurrentBudget();
         SBBox.ItemsSource = bvm.CurrentBudget.SubBudgets;

            List<string> paymethods = new List<string>()
            {
               "Cash",
               "Credit",
               "Debit"
            };
            PMBox.ItemsSource = paymethods;

            //Because These boxes do not save when Cached during page navigation, it was required that the information of which index was chosen was saved
            //into the localsettings temporarily such that when the user navigates to take a picture, his settings prior to were saved
            try
            {
                SBBox.SelectedIndex = (int)savedbudgetsettings.Values["SBBoxSelected"];
                PMBox.SelectedIndex = (int)savedbudgetsettings.Values["PMBoxSelected"];
            }
            catch
            {

            }
    }
        //Populates the list of event images with the filepath and title in a class called "EventImage" (see botom)
        private List<EventImage> InitializeEventImages()
        {
            List<EventImage> eventimages = new List<EventImage>();
            eventimages.Add(new EventImage("/Assets/AccessoriesX.png","Accessories"));
            eventimages.Add(new EventImage("/Assets/Bills.png", "Bills"));
            eventimages.Add(new EventImage("/Assets/Books.png", "Education"));
            eventimages.Add(new EventImage("/Assets/Bus.png", "Transportation"));
            eventimages.Add(new EventImage("/Assets/car.png", "Vehicle"));
            eventimages.Add(new EventImage("/Assets/clothes.png", "Clothing"));
            eventimages.Add(new EventImage("/Assets/electricity.png", "Electricity"));
            eventimages.Add(new EventImage("/Assets/entertainment.png", "Entertainment"));
            eventimages.Add(new EventImage("/Assets/family.png", "Family"));
            eventimages.Add(new EventImage("/Assets/forkknife.png", "Restaurant"));
            eventimages.Add(new EventImage("/Assets/home.png", "Home"));
            eventimages.Add(new EventImage("/Assets/medicine.png", "Medicine"));
            eventimages.Add(new EventImage("Assets/Logo.png", "Miscellaneous"));
            eventimages.Add(new EventImage("/Assets/Money.png", "Money"));
            eventimages.Add(new EventImage("/Assets/office.png", "Office"));
            eventimages.Add(new EventImage("/Assets/oil.png", "Gas"));
            eventimages.Add(new EventImage("/Assets/pets.png", "Pets"));
            eventimages.Add(new EventImage("/Assets/produce.png", "Groceries"));
            eventimages.Add(new EventImage("/Assets/tech.png", "Technology"));
            eventimages.Add(new EventImage("/Assets/water.png", "Water"));
            
            //sets the BitmapImage based on the filepath
            foreach (EventImage ei in eventimages)
            {
                ei.eImage = ImageFromRelativePath(this, ei.eImagename);
            }
            return eventimages;
        }

        //Sets the BitmapImage based on filepath
        public static BitmapImage ImageFromRelativePath(FrameworkElement parent, string path)
        {
            var uri = new Uri(parent.BaseUri, path);
            BitmapImage result = new BitmapImage();
            result.UriSource = uri;
            return result;
        }

        //Initializes the eventimagepicker and tries to initialize the photolistbox photos were taken
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await bvm.GetCurrentBudget();
            await bvm.GetSavedBudgets();
            EventImagePicker.ItemsSource = InitializeEventImages();
            EventImagePicker.SelectedIndex = 0;
            ImagePreview.Source = ((EventImage)EventImagePicker.SelectedItem).eImage;
            
            //Bind PhotoListBox
            try
            {
                await bvm.GetCurrentEvent();
 
                PhotoListBoxItemsSourceBinding();
            } catch {

            }
            
          
        }


        //Saves the information about the new event
         private async void Complete_Click(object sender, RoutedEventArgs e)
        {

                //Prepares every possible tag for the given event
                List<string> mytags = new List<string>();
                foreach (string o in TagListBox.Items)
                {
                    mytags.Add(o);
                }
                tagstringsplitter(mytags, NameBox.Text);
                tagstringsplitter(mytags, ((SubBudgetModel)SBBox.SelectedItem).Name);
                tagstringsplitter(mytags, LocationBox.Text);
                tagstringsplitter(mytags, PriceTextBox.Text);
                tagstringsplitter(mytags, PMBox.SelectedItem.ToString());
                tagstringsplitter(mytags, DateBox.Date.ToString());
                tagstringsplitter(mytags, DateBox.Date.Year.ToString());
                tagstringsplitter(mytags, DateBox.Date.Month.ToString());


             //Creates the new Event Model calling the constructor
                EventModel NewEvent = new EventModel(NameBox.Text, (SubBudgetModel)SBBox.SelectedItem, ((SubBudgetModel)SBBox.SelectedItem).Name, LocationBox.Text, Convert.ToDouble(PriceTextBox.Text),
                    PriceTextBox.Text, (bool)DeductionBox.IsChecked, PMBox.SelectedItem.ToString(), mytags, DateBox.Date, bvm.CurrentEvent.photonames, ((EventImage)EventImagePicker.SelectedItem).eImagename);
             
             //Sorts the event based on it's date, ensuring that the event list goes from most recent to least recent
                bool isadded = false;  
                if (bvm.CurrentBudget.Events != null)
                {
                    foreach (EventModel em in bvm.CurrentBudget.Events)
                    {
                        if (NewEvent.EventDate.DayOfYear >= em.EventDate.DayOfYear)
                        {
                            bvm.CurrentBudget.Events.Insert(bvm.CurrentBudget.Events.IndexOf(em), NewEvent);
                            isadded = true;
                            break;
                        }
                    }

                    if (isadded == false)
                    {
                        bvm.CurrentBudget.Events.Add(NewEvent);
                    }

                }
                else
                {
                    //else just add it in to the end
                    bvm.CurrentBudget.Events.Add(NewEvent);
                }

                //Updates the term-based subbudgetlist of the BudgetModel, this ensures that we are tracking every change to Budget throughout
                bvm.CurrentBudget.savedsubbudgets[bvm.CurrentBudget.Termnumber] = bvm.CurrentBudget.SubBudgets;
                List<BudgetModel> substitutelist = new List<BudgetModel>();
                SubBudgetModel chosenSB = (SubBudgetModel)SBBox.SelectedItem;

               
                    foreach (SubBudgetModel sbm in bvm.CurrentBudget.SubBudgets)
                    {
                        if (chosenSB.Name.Equals(sbm.Name))
                        {
                            if (DeductionBox.IsChecked == true) //Checks if it is a deduction or not
                            {
                                sbm.Remainder = sbm.Remainder - Convert.ToDouble(PriceTextBox.Text);
                            }
                            else
                            {
                                sbm.Remainder = sbm.Remainder + Convert.ToDouble(PriceTextBox.Text);
                            }
                        }
                    }
               
             //Saves both the CurrentBudget and Saves to the larger scale MyBudgets in bvm.MyBudgets;
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




                savedbudgetsettings.Values["SBBoxSelected"] = null;
                savedbudgetsettings.Values["PMBoxSelected"] = null;
                this.Frame.Navigate(typeof(BudgetHomePage));

        }

        //Splits tags based on spaces so individual words can be searched
         public void tagstringsplitter(List<string> tags, string text)
         {
             string[] addingtags = text.Split(' ');
             foreach (string s in addingtags)
             {
                 tags.Add(s);
             }
             tags.Add(text);
         }

         private void AddTagButton_Click(object sender, RoutedEventArgs e)
         {
             TagListBox.Items.Add(TagBox.Text);
             TagBox.Text = "";

         }

         private void DeleteTagButton_Click(object sender, RoutedEventArgs e)
         {
             if (TagListBox.SelectedIndex > -1)
             {
                 TagListBox.Items.Remove(TagListBox.SelectedItem.ToString());
             }
         }
       
         private  void AddPhoto_Click(object sender, RoutedEventArgs e)
         {
             
                    this.Frame.Navigate(typeof(Photopractice));
          
         }

         private void GoBack_Click(object sender, RoutedEventArgs e)
         {
             savedbudgetsettings.Values["SBBoxSelected"] = null;
             savedbudgetsettings.Values["PMBoxSelected"] = null;
             this.Frame.GoBack();
         }

         private void ChosenImageChanged(ListPickerFlyout sender, ItemsPickedEventArgs args)
         {
             ImagePreview.Source = ((EventImage)EventImagePicker.SelectedItem).eImage;
         }

         private void DeletePhotoButton_Click(object sender, RoutedEventArgs e)
         {
             if (PhotoListBox.SelectedIndex > -1)
             {
                 
                 
                 bvm.CurrentEvent.photonames.RemoveAt(PhotoListBox.SelectedIndex);
                 PhotoListBoxItemsSourceBinding();
             }
             
         }

         private void SBChanged(object sender, ItemsPickedEventArgs e)
         {
             SBBoxButton.Content = ((SubBudgetModel)SBBox.SelectedItem).Name;
             savedbudgetsettings.Values["SBBoxSelected"] = SBBox.SelectedIndex;
         }

         private void PMChanged(object sender, ItemsPickedEventArgs e)
         {
             PMBoxButton.Content = (string)PMBox.SelectedItem;
             savedbudgetsettings.Values["PMBoxSelected"] = PMBox.SelectedIndex;
         }

        //Tries to read every filepathname in bvm.CurrentEvent saved when a picture was taken and converts it to a Bitmap Image
         private async void PhotoListBoxItemsSourceBinding()
         {

             try
             {
                 List<BitmapImage> myimages = new List<BitmapImage>();
                 foreach (string filename in bvm.CurrentEvent.photonames)
                 {
                     StorageFile capturefile = await ApplicationData.Current.LocalFolder.GetFileAsync(filename);
                     BitmapImage img = new BitmapImage(new Uri(capturefile.Path));
                     myimages.Add(img);
                 }
                 PhotoListBox.ItemsSource = myimages;
             }
             catch
             {

             }
         }

   
    }

    //Public class that carries the information about our possible event images/icons containing a BitmapImage, it's filename name and the "title" of image
    public class EventImage
    {
        public BitmapImage eImage { get; set; }
        public string eImagename { get; set; }
        public string Name { get; set; }

        public EventImage(string _eImagename, string _Name)
        {
            eImagename = _eImagename;
            Name = _Name;

        }
    }

}
