using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace RBTracker
{
	[Activity (Label = "RBTracker", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		SmsReader smsReader;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			smsReader = new SmsReader (this);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button> (Resource.Id.myButton);
			
			button.Click += delegate {
				//button.Text = string.Format ("{0} clicks!", count++);
				TextView textView = FindViewById<TextView> (Resource.Id.textView1);
				textView.Text = smsReader.ReadBalanceData();
			};
		}
	}
}


