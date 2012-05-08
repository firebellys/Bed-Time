using System;
using System.Reflection;
using System.Windows;
using Microsoft.Phone.Tasks;

namespace BedTime
{
    public partial class About
    {
        public About()
        {
            InitializeComponent();
            Loaded += AboutLoaded;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PrivacyClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "This app does not store any information. It just rings after some time.");
        }
        private void AboutLoaded(object sender, RoutedEventArgs e)
        {
            //if (!IsolatedStorageSettings.ApplicationSettings.TryGetValue("LocalSettings", out _localSettings))
            //{
            //    _localSettings = new CurrentSettings();
            //}
            var assembly = Assembly.GetExecutingAssembly();
            var myassemblyName = new AssemblyName(assembly.FullName);
            VersionNumber.Text = myassemblyName.Version.Major + "." + myassemblyName.Version.Minor + "." +
                             myassemblyName.Version.Build;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HelpClick(object sender, RoutedEventArgs e)
        {
            var marketplaceDetailTask = new MarketplaceDetailTask
                                            {ContentIdentifier = "3b044f00-becf-4d99-bec4-9229d1591719"};
            marketplaceDetailTask.Show();
        }

        private void EmailClick(object sender, RoutedEventArgs e)
        {
            var emailcomposer = new EmailComposeTask {To = "firebelly@gmail.com", Subject = "Bed Time Help", Body = ""};
            emailcomposer.Show();
        }

        private void TwitterClick(object sender, RoutedEventArgs e)
        {
            var task = new WebBrowserTask
                           {
                               Uri =
                                   new Uri("https://twitter.com/share?related=firebellys&via=firebellys",
                                           UriKind.RelativeOrAbsolute)
                           };
            task.Show();
        }

        private void WebClick(object sender, RoutedEventArgs e)
        {
            var task = new WebBrowserTask
            {
                Uri =
                    new Uri("http://www.firebelly-studios.com",
                            UriKind.RelativeOrAbsolute)
            };
            task.Show();
        }
    }
}