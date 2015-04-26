using System;
using System.Linq;
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

		public void ReadBalanceData()
		{
			var resolver = _activity.ContentResolver;
			ICursor cursor = resolver.Query (Android.Net.Uri.Parse("content://sms/inbox"), null, null, null, null);
			if (cursor.MoveToFirst ())
			{
				Dictionary<long, int> rawBalances = new Dictionary<long, int> ();
				do {
					string address = cursor.GetString(2);
					if (address == RB_ADDRESS)
					{
						string body = cursor.GetString(11);
						int idx = body.IndexOf(RB_MSG_ANCHOR);
						if (idx != -1)
						{
							DateTime dt = parseDate(body);
							int v = parseBalance(body, idx);
							if (rawBalances.ContainsKey(dt.Ticks))
							{
								rawBalances[dt.Ticks] = v;
							}
							else
							{
								rawBalances.Add(dt.Ticks, v);
							}
						}
					}
				} while(cursor.MoveToNext ());
					
				var sorted = rawBalances.OrderBy (key => key.Key).ToDictionary (ki => ki.Key, vi => vi.Value);
				int prev = sorted.First().Value;
				foreach (var key in sorted.Keys)
				{
					DateTime dt = new DateTime (key);
					int cur = sorted [key] - prev;
					this.putValue (dt.Year, dt.Month, cur);
					prev = sorted [key];
				}
			}
		}

		public string[] GetBalancesArray()
		{
			List<string> result = new List<string> ();
			foreach (var year in balances.Keys) {
				Dictionary<int, int> months = balances [year];
				foreach (var month in months.Keys) {
					var cur = month.ToString () + " - " + year.ToString () + " > " + months [month].ToString ();
					result.Add (cur);
				}
			}

			return result.ToArray<string> ();
		}
	}
}

