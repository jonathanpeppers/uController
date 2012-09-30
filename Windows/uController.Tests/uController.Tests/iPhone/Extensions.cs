using System;

namespace System.iPhone.UIKit
{
	public static class Extensions
	{	
		public static DateTime? ToNullableDate(this string s)
		{
			DateTime val;
			if (DateTime.TryParse(s, out val))
			{
				return val;	
			}
			return default(DateTime?);
		}
		
		public static DateTime ToDate(this string s)
		{
			DateTime val;
			DateTime.TryParse(s, out val);
			return val;
		}
		
		public static int ToInt(this string s)
		{
			int x;
			int.TryParse(s, out x);
			return x;
		}
		
		public static int? ToNullableInt(this string s)
		{
			int x;
			if (int.TryParse(s, out x))
			{
				return x;	
			}
			return default(int?);
		}
		
		public static decimal ToDecimal(this string s)
		{
			decimal x;
			decimal.TryParse(s, out x);
			return x;
		}
		
		public static decimal? ToNullableDecimal(this string s)
		{
			//Bug in Mono?
			if (s != ".")
			{
				decimal x;
				if (decimal.TryParse(s, out x))
				{
					return x;	
				}
			}
			return default(decimal?);
		}
		
		public static float ToFloat(this string s)
		{
			float x;
			float.TryParse(s, out x);
			return x;
		}
		
		public static float? ToNullableFloat(this string s)
		{
			//Bug in Mono?
			if (s != ".")
			{
				float x;
				if (float.TryParse(s, out x))
				{
					return x;	
				}
			}
			return default(float?);
		}
		
		public static long ToLong(this string s)
		{
			long x;
			long.TryParse(s, out x);
			return x;
		}
		
		public static long? ToNullableLong(this string s)
		{
			long x;
			if (long.TryParse(s, out x))
			{
				return x;	
			}
			return default(long?);
		}
	}
}

