using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Media.Devices;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Budgeting.ViewModels;
using Budgeting.Models;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Budgeting.Views
{
  //This page allows the user to take a picture
    public sealed partial class Photopractice : Page
    {
        public MediaCapture captureManager;
        public bool isPreviewing = false;
        public BudgetViewModel bvm;
        public Photopractice()
        {
            this.InitializeComponent();
            bvm = new BudgetViewModel();
            var appView = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
            appView.SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);
            

        }


        protected  override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        //Starts the new mediacapture and initializes for photo capturing and refers to the cameraID (to determine which camera is being used)
        private async void InitializePreview()
        {
            captureManager = new MediaCapture();
            var cameraID = await GetCameraID(Windows.Devices.Enumeration.Panel.Back);


            await captureManager.InitializeAsync(new MediaCaptureInitializationSettings {
                StreamingCaptureMode = StreamingCaptureMode.Video,
                PhotoCaptureSource = PhotoCaptureSource.Photo,
                AudioDeviceId = string.Empty,
                VideoDeviceId = cameraID.Id,
            });
            StartPreview();
        }

        private async void StartPreview()
        {

            previewElement.Source = captureManager;
            await captureManager.StartPreviewAsync();

            isPreviewing = true;
        }

        //Closes the capture process and stream
        private async void CleanCapture()
        {
            if (captureManager != null)
            {
                if (isPreviewing == true)
                {
                    await captureManager.StopPreviewAsync();
                    isPreviewing = false;
                }
                previewElement.Source = null;
                CaptuerButon.Label = "capture";
                
                captureManager.Dispose();
            }
        }

        //Starts the preview or cleans the capture dependant on if we are previewing or not
        private void captureButton_Click(object sender, RoutedEventArgs e)
        {
            if (isPreviewing == false)
            {
                InitializePreview();
                CaptuerButon.Label = "cancel";
            }
            else if (isPreviewing == true)
            {
                CleanCapture();
            }
        }

        //creates a filename based on the time of the photo and saves the photo to LocalFolder
        // The filename is saved to the CurrentEvent in the ViewModel; it is serialized and on returning the NewEventPage, it will deserialize to read the image
        private async void saveButton_Click(object sender, RoutedEventArgs e)
        {
            Windows.Storage.ApplicationDataContainer savedbudgetsettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            string filename = "photo_" + DateTime.Now.Ticks.ToString();
            savedbudgetsettings.Values["CurrentPhoto"] = filename;
            try
            {
                await bvm.GetCurrentEvent();
                bvm.CurrentEvent.photonames.Add(filename);
            }
            catch
            {
                bvm.CurrentEvent = new EventModel();
                bvm.CurrentEvent.photonames = new List<string>();
                bvm.CurrentEvent.photonames.Add(filename);
            }
            
            await bvm.SaveCurrentEvent();
            //declare image format
            ImageEncodingProperties format = ImageEncodingProperties.CreateJpeg();

            //generate file in local folder:
            StorageFile capturefile = await ApplicationData.Current.LocalFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);

            ////take & save photo
            await captureManager.CapturePhotoToStorageFileAsync(format, capturefile);

            this.Frame.GoBack();
        
        }

        private static async Task<DeviceInformation> GetCameraID(Windows.Devices.Enumeration.Panel camera)
        {
            DeviceInformation deviceID = (await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture)).FirstOrDefault(x => x.EnclosureLocation != null && x.EnclosureLocation.Panel == camera);

            return deviceID;
        
        }

        private async void SetFocus(uint? focusValue = null)
        {
            //try catch used to avoid app crash at startup when no CaptureElement is active
            try
            {
                //setting default value
                if (!focusValue.HasValue)
                {
                    focusValue = 500;
                }

                //check if the devices camera supports focus control
                if (captureManager.VideoDeviceController.FocusControl.Supported)
                {
                    //disable flash assist for focus control
                    captureManager.VideoDeviceController.FlashControl.AssistantLightEnabled = false;

                    //configure the FocusControl to manual mode
                    captureManager.VideoDeviceController.FocusControl.Configure(new FocusSettings() { Mode = FocusMode.Manual, Value = focusValue, DisableDriverFallback = true });
                    //update the focus on our MediaCapture
                    await captureManager.VideoDeviceController.FocusControl.FocusAsync();
                }
            }
            catch { }
        }

        private void FocusValueSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            try
            {
                //convert double e.NewValue to uint and call SetFocus()
                uint focus = Convert.ToUInt32(e.NewValue);
                SetFocus(focus);
            }
            catch
            {

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
