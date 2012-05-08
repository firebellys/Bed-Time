using System;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Resources;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using Telerik.Windows.Controls;

namespace BedTime
{
    public partial class Wake : PhoneApplicationPage
    {
        private DateTime _wakeUpTime;

        public Wake()
        {
            InitializeComponent();
        }

        private void FireTile(String text)
        {
            var uri = new Uri("Images/173x173.png", UriKind.Relative);
            StreamResourceInfo sri = Application.GetResourceStream(uri);

            var uriBack = new Uri("Images/173x173Back.png", UriKind.Relative);
            StreamResourceInfo sriBack = Application.GetResourceStream(uriBack);


            var wbm = new WriteableBitmap(173, 173);
            wbm.SetSource(sri.Stream);
            var wbmBack = new WriteableBitmap(173, 173);
            wbmBack.SetSource(sriBack.Stream);


            using (
                IsolatedStorageFileStream stream =
                    IsolatedStorageFile.GetUserStoreForApplication().CreateFile(
                        "/Shared/ShellContent/tile.png"))
            {
                wbm.SaveJpeg(stream, 173, 173, 0, 100);
            }

            using (
                IsolatedStorageFileStream streamBack =
                    IsolatedStorageFile.GetUserStoreForApplication().CreateFile(
                        "/Shared/ShellContent/tileBack.png"))
            {
                wbmBack.SaveJpeg(streamBack, 173, 173, 0, 100);
            }


            ShellTile currentTiles = ShellTile.ActiveTiles.First();
            var tilesUpdatedData = new StandardTileData
                                       {
                                           BackBackgroundImage =
                                               new Uri("isostore:/Shared/ShellContent/tileBack.png",
                                                       UriKind.Absolute),
                                           BackContent = text,
                                           BackTitle = ""
                                       };
            if (currentTiles != null) currentTiles.Update(tilesUpdatedData);
        }

        private string CheckForAlarms()
        {
            var tempstring = "";
            var temp = ScheduledActionService.Find("Bed Time Wake Up ID");
            if (temp != null)
            {
                FireTile("Alarm @ " + temp.BeginTime.ToShortDateString());
                tempstring = "Alarm set to: " + temp.BeginTime.ToShortDateString() + " at " +
                             temp.BeginTime.ToShortTimeString();
            }
            else
            {
                tempstring = "No alarm set.";
                FireTile("No alarm set.");
            }
            return tempstring;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ScheduledAction temp = ScheduledActionService.Find("Go to sleep Reminder ID");
            if (temp != null)
            {
                if (temp.IsScheduled == true)
                {
                    TheCheckBox.IsChecked = true;
                }
                else
                {
                    TheCheckBox.IsChecked = false;
                }
            }
            else
            {
                TheCheckBox.IsChecked = false;
            }
        }

        private void HandleAlarmSet(DateTime thetime)
        {
            try
            {
                var alarm = new Alarm("Bed Time Wake Up ID")
                                {
                                    Content = "Wake Up",
                                    Sound = new Uri("Sounds/sleet.mp3", UriKind.RelativeOrAbsolute),
                                    BeginTime = thetime,
                                    RecurrenceType = RecurrenceInterval.None,
                                };

                ScheduledAction temp = ScheduledActionService.Find("Bed Time Wake Up ID");
                if (temp != null)
                {
                    ScheduledActionService.Remove("Bed Time Wake Up ID");
                    ScheduledActionService.Add(alarm);
                    FireTile("Alarm @ " + alarm.BeginTime.ToShortTimeString());
                }
                else
                {
                    ScheduledActionService.Add(alarm);
                    FireTile("Alarm @ " + alarm.BeginTime.ToShortTimeString());
                }

                MessageBox.Show("Alarm set for: " + alarm.BeginTime.ToShortDateString() + " at " +
                                alarm.BeginTime.ToShortTimeString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception caught: " + ex.Message);
            }
        }

        private void AboutClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/About.xaml", UriKind.Relative));
        }

        private void RefreshClick(object sender, RoutedEventArgs e)
        {
        }

        private void ClearClick(object sender, RoutedEventArgs e)
        {

            ScheduledAction tempReminder = ScheduledActionService.Find("Go to sleep Reminder ID");
            if (tempReminder != null)
            {
                ScheduledActionService.Remove("Go to sleep Reminder ID");
            }
            ScheduledAction temp = ScheduledActionService.Find("Bed Time Wake Up ID");
            if (temp != null)
            {
                ScheduledActionService.Remove("Bed Time Wake Up ID");
                MessageBox.Show("Alarm cleared.");
                FireTile("No alarm set.");
            }
            else
            {
                //MessageBox.Show("No alarm found.");
            }
            sleepButton1.Visibility = Visibility.Collapsed;
            sleepButton2.Visibility = Visibility.Collapsed;
            sleepButton3.Visibility = Visibility.Collapsed;
            sleepButton4.Visibility = Visibility.Collapsed;
            sleepButton5.Visibility = Visibility.Collapsed;
            Dispatcher.BeginInvoke(() => { timePicker.Value = null; });

            //Statusfield2.Text = CheckForAlarms();
            refreshbutton.IsEnabled = true;

            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void TimePickerValueChanged(object sender, ValueChangedEventArgs<object> args)
        {
            if (args.NewValue != null)
            {
                _wakeUpTime = Convert.ToDateTime(args.NewValue);

                if (_wakeUpTime > DateTime.Now.Add(TimeSpan.FromMinutes(120)) ||
                    _wakeUpTime < DateTime.Now.Subtract(TimeSpan.FromSeconds(30)))
                {
                    if (_wakeUpTime < DateTime.Now.Subtract(TimeSpan.FromSeconds(30)))
                    {
                        _wakeUpTime = _wakeUpTime.AddHours(24);
                    }
                    sleepButton1.Content = _wakeUpTime.Subtract(TimeSpan.FromMinutes(360));
                    sleepButton2.Content = _wakeUpTime.Subtract(TimeSpan.FromMinutes(450));
                    sleepButton3.Content = _wakeUpTime.Subtract(TimeSpan.FromMinutes(540));
                    sleepButton4.Content = _wakeUpTime.Subtract(TimeSpan.FromMinutes(630));
                    sleepButton5.Content = _wakeUpTime.Subtract(TimeSpan.FromMinutes(720));

                    sleepButton1.Visibility = Visibility.Visible;
                    sleepButton2.Visibility = Visibility.Visible;
                    sleepButton3.Visibility = Visibility.Visible;
                    sleepButton4.Visibility = Visibility.Visible;
                    sleepButton5.Visibility = Visibility.Visible;
                    HandleAlarmSet(_wakeUpTime);
                    //Statusfield2.Text = CheckForAlarms();
                }
                else
                {
                    Dispatcher.BeginInvoke(() => { timePicker.Value = null; });
                    MessageBox.Show("Please set time ahead at least two hours.");
                }
            }
        }

        #region Nested type: Toggles

        private enum Toggles
        {
            Open,
            Close
        }

        #endregion

        private void TimeClick(object sender, RoutedEventArgs e)
        {
            var reminder = new Reminder("Go to sleep Reminder ID")
                               {
                                   Title = "Bed Time!",
                                   Content = "You should head to bed soon. You should try and be asleep in 30 minutes.",
                                   BeginTime = Convert.ToDateTime(((Button)e.OriginalSource).Content).Subtract(TimeSpan.FromMinutes(14)),
                                   RecurrenceType = RecurrenceInterval.None,
                               };

            //reminder.ExpirationTime = expirationTime;
            if (TheCheckBox.IsChecked != null && TheCheckBox.IsChecked == true)
            {
                reminder.RecurrenceType = RecurrenceInterval.Daily;
            }
            else
            {
                reminder.RecurrenceType = RecurrenceInterval.None;
            }
            //reminder.NavigationUri = navigationUri;
            ScheduledAction temp = ScheduledActionService.Find("Go to sleep Reminder ID");
            if (temp != null)
            {
                ScheduledActionService.Remove("Go to sleep Reminder ID");
                ScheduledActionService.Add(reminder);
                //FireTile("Bed time reminder set @ " + reminder.BeginTime.ToShortTimeString());
            }
            else
            {
                ScheduledActionService.Add(reminder);
                //FireTile("Bed time reminder set @ " + reminder.BeginTime.ToShortTimeString());
            }
            MessageBox.Show("Reminder set for: " + reminder.BeginTime.ToShortDateString() + " at " +
                    reminder.BeginTime.ToShortTimeString());
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));

            // Register the reminder with the system.
            //ScheduledActionService.Add(reminder);


            //HandleAlarmSet(Convert.ToDateTime(((Button)e.OriginalSource).Content));
        }
    }
}