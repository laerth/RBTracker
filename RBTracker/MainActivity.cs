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
				ListView monthView = FindViewById<ListView> (Resource.Id.monthsList);
				smsReader.ReadBalanceData();

				var adapter = new ArrayAdapter<string>(this, Resource.Layout.AdapterLayout, smsReader.GetBalancesArray());
				monthView.Adapter = adapter;
			};
		}
	}
}


