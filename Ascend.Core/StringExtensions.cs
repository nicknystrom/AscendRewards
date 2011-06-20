using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Ascend.Core.Services;

namespace Ascend.Core
{
    public static class StringExtensions
    {
        public static string[] Clean(this string[] list)
        {
            return list.Where(x => !String.IsNullOrWhiteSpace(x)).ToArray();
        }

        public static Uri GetSafeUrl(this HttpRequest request)
        {
            return new HttpRequestWrapper(request).GetSafeUrl();
        }

        public static Uri GetSafeUrl(this HttpRequestBase request)
        {
            return new Uri(String.Format(
                "{0}://{1}{2}",
                request.IsSecureConnection ? "https" : "http",
                request.ServerVariables["HTTP_HOST"], 
                request.RawUrl
            ));
        }

        public static string ShortenTo(this string a, int length)
        {
            if (null == a) return String.Empty;
            if (a.Length <= length) return a;
            return a.Substring(0, Math.Min(a.Length, length - 3)) + "...";
        }
		
		public static TResult Try<T, TResult>(this T obj, Func<T, TResult> func)
		{
			try
			{
				return func(obj);
			}
			catch
			{
				return default(TResult);
			}
		}

        public static string Or(this string a, string alternate)
        {
            return String.IsNullOrWhiteSpace(a) ? alternate : a;
        }

        public static string Or(this object o, string alternate)
        {
            if (null == o)
                return alternate;
            var a = o as string ?? o.ToString();
            return String.IsNullOrWhiteSpace(a) ? alternate : a;
        }

        public static TResult Or<T, TResult>(this T o, Func<T, TResult> f, TResult def) where TResult : class
        {
            return null == o ? def : f(o) ?? def;
        }

        public static TResult Value<TAttribute, TResult>(
            this Type t,
            Func<TAttribute, TResult> f,
            TResult def)
            where TResult : class
            where TAttribute : class
        {
            return (t.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault() as TAttribute).Or(f, def);
        }

        public static Uri ToAbsoluteUrl(this string a)
        {
            return ToAbsoluteUrl(a, new HttpRequestWrapper(HttpContext.Current.Request));
        }

        public static Uri ToAbsoluteUrl(this string a, HttpRequestBase request)
        {
            if (String.IsNullOrEmpty(a)) throw new ArgumentException("String must not be null or empty.");
            if (a.StartsWith("~/"))
            {
                return new Uri(
                    request.GetSafeUrl(),
                    (request.ApplicationPath + a.Substring(1)).Replace("//", "/")
                );
            }
            else if (a.StartsWith("$/"))
            {
                var files = ServiceLocator.Resolve<IFileStore>();
                return new Uri(files.GetUrl(a.Substring(2)));
            }
            else if (a.StartsWith("http://") ||
                     a.StartsWith("https://"))
            {
                return new Uri(a);
            }
            else
            {
                return new Uri(request.GetSafeUrl(), a);
            }
        }
    }
}
