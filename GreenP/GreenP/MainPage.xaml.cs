using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using Bing.Maps;
using Windows.Data.Json;
using Windows.Devices.Geolocation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace GreenP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        Dictionary<Pushpin, JsonObject> data;
        
        public MainPage()
        {
            this.InitializeComponent();

            data = Util.doInit();

            getPosition();

            foreach (Pushpin point in data.Keys)
            {
                point.Text = "P";
                double lat = 0;
                double lng = 0;
                Double.TryParse(data[point]["lat"].GetString(),out lat);
                Double.TryParse(data[point]["lng"].GetString(),out lng);
                MapLayer.SetPosition(point, new Location (lat,lng));
                myMap.Children.Add(point);
            }
        }

        private async void getPosition()
        {

            Geolocator geo = new Geolocator();
            Geoposition position = await geo.GetGeopositionAsync();
            Location loc = new Location(position.Coordinate.Latitude, position.Coordinate.Longitude);
            myMap.SetView(loc, 12);
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }
    }
}
