using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkerServiceApp1.Utility
{
    public class DbUtility
    {
        public static string QS(object o, bool IncludeEqual = false)
        {
            if ((o == null))
            {
                return (IncludeEqual ? " IS NULL" : "NULL");
            }
            else
            {
                if (string.IsNullOrEmpty(o.ToString()))
                {
                    return (IncludeEqual ? " IS NULL" : "NULL");
                }
                else
                {
                    return (IncludeEqual ? "=" : "") + "'" + Replace(o.ToString()) + "'";
                }
            }
        }
        public static string Replace(string text)
        {
            if (text.Contains("'"))
            {
                text = text.Replace(text, "''");
            }
            if (text.Contains("''''"))
            {
                text = text.Replace(text, "''");
            }
            return text;
        }
        public static List<Dictionary<string, object>> Read(DbDataReader reader)
        {
            List<Dictionary<string, object>> expandolist = new List<Dictionary<string, object>>();
            foreach (var item in reader)
            {
                IDictionary<string, object> expando = new ExpandoObject();
                foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(item))
                {
                    var obj = propertyDescriptor.GetValue(item);
                    expando.Add(propertyDescriptor.Name, obj);
                }
                expandolist.Add(new Dictionary<string, object>(expando));
            }
            return expandolist;
        }
        public static List<Dictionary<string, string>> Read(DbDataReader reader, string forOverLoading = "")
        {
            List<Dictionary<string, string>> expandolist = new List<Dictionary<string, string>>();
            foreach (var item in reader)
            {
                Dictionary<string, string> expando = new Dictionary<string, string>();
                foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(item))
                {
                    var obj = propertyDescriptor.GetValue(item);

                    if (obj == null)
                    {
                        expando.Add(propertyDescriptor.Name, "");
                    }
                    else
                    {
                        expando.Add(propertyDescriptor.Name, Convert.ToString(obj));
                    }

                }
                expandolist.Add(new Dictionary<string, string>(expando));
            }
            return expandolist;
        }
        public static string excludProviderConnectionString(string connectionString)
        {
            return connectionString.Substring(connectionString.IndexOf(";") + 1);
        }
    }

}
