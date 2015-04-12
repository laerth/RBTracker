using System;
using Android.App;
using Android.Content;
using Android.Database;

namespace RBTracker
{
	public class SmsReader
	{
		readonly Activity _activity;

		public SmsReader (Activity activity)
		{
			_activity = activity;
		}

		public int GetSmsCount()
		{
			int c = 1;
			var resolver = _activity.ContentResolver;
			ICursor cursor = resolver.Query (Android.Net.Uri.Parse("content://sms/inbox"), null, null, null, null);
			while (cursor.MoveToNext ()) {
				c++;
			}

			return c;
		}
	}
}

