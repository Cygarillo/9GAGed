using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;


namespace _9GAGed
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            InitContent();
            SettingsPane.GetForCurrentView().CommandsRequested += SettingsCommandsRequested;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            InitContent();
        }

        private async void InitContent()
        {
            progressRing.IsActive = true;
            JsonObject jsonObject;
            do
            {
                jsonObject = await GetJsonObject();
            } while (jsonObject.Count == 1);

            try
            {
                titleBox.Text = jsonObject["title"].GetString();
                string imgSource = "http:" + jsonObject["gag"].GetString();
                var uri = new Uri(imgSource, UriKind.Absolute);
                pictureBox.Source = new BitmapImage(uri);
            }
            catch (Exception)
            {
                InitContent();
            }
            progressRing.IsActive = false;

        }



        private void SettingsCommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            var privacyStatement = new SettingsCommand("privacy", "Privacy Statement", x => Launcher.LaunchUriAsync(
                    new Uri("https://docs.google.com/document/pub?id=1-vYAN6x83MAT1bgbXPS3U2_ATbXCUN5_8QTmHNTyefI")));

            args.Request.ApplicationCommands.Clear();
            args.Request.ApplicationCommands.Add(privacyStatement);
        }
        private async Task<JsonObject> GetJsonObject()
        {
            var client = new HttpClient();
            HttpResponseMessage responseMessage = await client.GetAsync("http://9gag.cocacoca.it/rand");
            var input = await responseMessage.Content.ReadAsStringAsync();
            return JsonObject.Parse(input);

        }

        static DispatcherTimer timer = new DispatcherTimer();
        private void slideshowButton_Click(object sender, RoutedEventArgs e)
        {

            if (timer.IsEnabled)
            {
                timer.Stop();
                slideshowButton.Content = "Autonext";
                nextButton.IsEnabled = true;
            }
            else
            {
                InitContent();
                slideshowButton.Content = "Stop";
                nextButton.IsEnabled = false;
                timer.Interval = new TimeSpan(0, 0, 0, 10);

                timer.Tick += (o, o1) =>
                    {
                        timer.Stop();
                        InitContent();
                        timer.Start();
                    };
                timer.Start();
            }

        }


    }
}
