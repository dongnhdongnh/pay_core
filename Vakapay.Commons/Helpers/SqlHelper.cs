using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Vakapay.Commons.Helpers
{
	public class SqlHelper
	{

		public static string Query_Search(string TableName, Dictionary<string, string> whereValue)
		{
			StringBuilder whereStr = new StringBuilder("");
			int count = 0;
			foreach (var prop in whereValue)
			{
				if (prop.Value != null)
				{
					if (count > 0)
						whereStr.Append(" AND ");
					whereStr.AppendFormat(" {0}='{1}'", prop.Key, prop.Value);
					count++;
				}
			}

			string output = string.Format("SELECT * FROM {0} WHERE {1}", TableName, whereStr);
			Console.WriteLine(output);
			return output;
		}

		public static string Query_Update(string TableName, object updateValue, Dictionary<string, string> whereValue)
		{
			StringBuilder updateStr = new StringBuilder("");
			StringBuilder whereStr = new StringBuilder("");

			int count = 0;
			foreach (PropertyInfo prop in updateValue.GetType().GetProperties())
			{
				if (prop.GetValue(updateValue, null) != null)
				{
					if (count > 0)
						updateStr.Append(",");
					updateStr.AppendFormat(" {0}='{1}'", prop.Name, prop.GetValue(updateValue, null));
					count++;
				}
			}

			// if (whereStr != null)
			count = 0;
			foreach (var prop in whereValue)
			{
				if (prop.Value != null)
				{
					if (count > 0)
						whereStr.Append(" AND ");
					whereStr.AppendFormat(" {0}='{1}'", prop.Key, prop.Value);
					count++;
				}
			}
			string output = string.Format(@"UPDATE {0} SET {1} WHERE {2}", TableName, updateStr, whereStr);
			Console.WriteLine(output);
			return output;
		}

		public static string Query_Update(string TableName, Dictionary<string, string> updateValue, Dictionary<string, string> whereValue)
		{
			StringBuilder updateStr = new StringBuilder("");
			StringBuilder whereStr = new StringBuilder("");

			int count = 0;
			foreach (var prop in updateValue)
			{
				if (prop.Value != null)
				{
					if (count > 0)
						updateStr.Append(",");
					updateStr.AppendFormat(" {0}='{1}'", prop.Key, prop.Value);
					count++;
				}
			}

			// if (whereStr != null)
			count = 0;
			foreach (var prop in whereValue)
			{
				if (prop.Value != null)
				{
					if (count > 0)
						whereStr.Append(" AND ");
					whereStr.AppendFormat(" {0}='{1}'", prop.Key, prop.Value);
					count++;
				}
			}
			string output = string.Format(@"UPDATE {0} SET {1} WHERE {2}", TableName, updateStr, whereStr);
			Console.WriteLine(output);
			return output;
		}


	}
}
