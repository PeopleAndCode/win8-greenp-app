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
        Dictionary<ListBoxItem, Pushpin> listData;

        public MainPage()
        {
            this.InitializeComponent();

            try{
            data = Util.doInit();

            getPosition();

            listData = new Dictionary<ListBoxItem, Pushpin>();

            foreach (Pushpin point in data.Keys)
            {
                point.Text = "P";
                ListBoxItem item = new ListBoxItem();
                item.Tapped += new TappedEventHandler(listTapped);
                item.Content = data[point]["address"].GetString();
                listData[item] = point;
                listBox.Items.Add(item);
                point.Tapped += new TappedEventHandler(pinEvent);
                double lat = 0;
                double lng = 0;
                Double.TryParse(data[point]["lat"].GetString(), out lat);
                Double.TryParse(data[point]["lng"].GetString(), out lng);
                MapLayer.SetPosition(point, new Location(lat, lng));
                myMap.Children.Add(point);
            }
            }
            catch
            {
                failGracefully();
            }
        }

        private async void failGracefully()
        {
            MessageDialog m = new MessageDialog("This app can not function without internet access. Please connect to the internet and try again.");
            await m.ShowAsync();
            Environment.FailFast("Could not connect to the internet!");
        }

        private void pinEvent(object sender, TappedRoutedEventArgs e)
        {
            Pushpin p = sender as Pushpin;
            myMap.SetView(MapLayer.GetPosition(p), 14);
            myMap.SetView(MapLayer.GetPosition(p), 16);
            infoBlock.Text = infoString(p);
            rateBlock.Text = rateString(p);
        }

        private string rateString(Pushpin p)
        {
            string ret = "";

            foreach (JsonValue val in data[p]["rate_details"].GetObject()["periods"].GetArray())
            {
                ret += val.GetObject()["title"].GetString() + "\n";
                foreach (JsonValue v in val.GetObject()["rates"].GetArray())
                {
                    ret += "   - " + v.GetObject()["when"].GetString() + " : " + v.GetObject()["rate"].GetString() + "\n";
                }
            }

            ret += "Payment Options :\n";
            foreach (JsonValue val in data[p]["payment_options"].GetArray())
            {
                ret += "   - " + val.GetString() + "\n";
            }

            ret += "You can pay with :\n";
            foreach (JsonValue val in data[p]["payment_methods"].GetArray())
            {
                ret +=  "   - " + val.GetString() + "\n";
            }
            return ret;
        }

        private string infoString(Pushpin p)
        {
            string ret = "";

            ret += data[p]["address"].GetString() + "\n";
            try { ret += data[p]["rate"].GetString() + "\n"; }
            catch { ret += "See rate details on next slide \n"; }
            ret += "Carpark Type : " + data[p]["carpark_type_str"].GetString() + "\n";
            try
            {
                if (data[p]["max_height"].GetString() != "0.00")
                    ret += "Maximum height : " + data[p]["max_height"].GetString() + " Meters\n";
            }
            catch { }
            ret += "Maximum Capacity : " + data[p]["capacity"].GetString() + "\n";

            return ret;
        }

        private async void getPosition()
        {

            Geolocator geo = new Geolocator();
            Geoposition position = await geo.GetGeopositionAsync();
            Location loc = new Location(position.Coordinate.Latitude, position.Coordinate.Longitude);
            myMap.SetView(loc, 13);
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void FlipView_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }

        private void listTapped(object sender, TappedRoutedEventArgs e)
        {
            pinEvent(listData[sender as ListBoxItem], e);
        }
    }

}
