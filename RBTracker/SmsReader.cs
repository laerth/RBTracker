using System;
using System.Collections;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Database;

namespace RBTracker
{
	public class SmsReader
	{
		const string RB_ADDRESS = "Rosbank";
		const string RB_MSG_START = "Karta **0489";
		const string RB_MSG_ANCHOR = "Ostatok: ";
		const int RB_MSG_ANCHOR_L = 9;

		private Dictionary<int, Dictionary<int, int>> balances = new Dictionary<int, Dictionary<int, int>> ();

		readonly Activity _activity;

		public SmsReader (Activity activity)
		{
			_activity = activity;
		}
			
		private DateTime parseDate(string body)
		{
			int last = body.LastIndexOf ("/") + 2;
			int first = last - 7;
			string dt_str = body.Substring (first, last - first + 1);

			return DateTime.ParseExact(dt_str, "dd/MM/yy", System.Globalization.CultureInfo.InvariantCulture);
		}

		private int parseBalance(string body, int idx)
		{
			int last = body.IndexOf (".", idx);
			int start = idx + RB_MSG_ANCHOR_L;
			string value = body.Substring (start, last - start);

			return int.Parse (value);
		}

		private void putValue(int year, int month, int value)
		{
			if (!balances.ContainsKey (year))
			{
				balances.Add (year, new Dictionary<int, int> ());
			}
			Dictionary<int, int> months = balances [year];
			if (!months.ContainsKey (month))
			{
				months.Add (month, 0);
			}
			months [month] += value;
		}

		public string ReadBalanceData()
		{
			var resolver = _activity.ContentResolver;
			ICursor cursor = resolver.Query (Android.Net.Uri.Parse("content://sms/inbox"), null, null, null, null);
			if (cursor.MoveToFirst ())
			{
				do {
					string address = cursor.GetString(2);
					if (address == RB_ADDRESS)
					{
						string body = cursor.GetString(11);
						if (body.StartsWith(RB_MSG_START))
						{
							int idx = body.IndexOf(RB_MSG_ANCHOR);
							if (idx != -1)
							{
								DateTime dt = parseDate(body);
								int v = parseBalance(body, idx);
								this.putValue(dt.Year, dt.Month, v);
							}
						}
					}
				} while(cursor.MoveToNext ());
			}
				
			return "0";
		}
	}
}

