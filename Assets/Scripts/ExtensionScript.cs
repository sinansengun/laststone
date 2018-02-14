using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

/// <summary>
/// Author: Sinan Şengün
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Format string with extension method power:))
    /// </summary>
    /// <param name="input"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public static string F(this string input, params object[] args)
    {
        return string.Format(input, args);
    }
    /// <summary>
    /// Crops string to the requested byte length
    /// </summary>
    /// <param name="input"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public static string Crop(this string input, int length)
    {
        return input.Crop(length, Encoding.UTF8);
    }
    /// <summary>
    /// Crops string to the requested byte length
    /// </summary>
    /// <param name="input"></param>
    /// <param name="length"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    public static string Crop(this string input, int length, Encoding encoding)
    {
        if (input == null) return null;

        if (input.Length > length)
            input = input.Substring(0, length);

        int byteLength = encoding.GetBytes(input).Length;
        if (byteLength > length)
            input = input.Substring(0, length - (byteLength - input.Length));

        return input;
    }

    public static string ToSystemName(this string input)
    {
        return Regex.Replace(input, @"^[^A-Za-z_]+|[^A-Za-z_\d]+", String.Empty);
    }

    public static Stream ToStream(this string text)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);

        writer.Write(text);
        writer.Flush();
        stream.Position = 0;

        return stream;
    }

    public static bool HasLength(this string value)
    {
        return !String.IsNullOrEmpty(value);
    }

    public static bool IsNullOrEmpty(this string value)
    {
        return String.IsNullOrEmpty(value);
    }

    public static bool IsNullOrWhiteSpace(this string value)
    {
        return string.IsNullOrEmpty(value) ||
            value.All(Char.IsWhiteSpace);
    }

    public static bool EqualsIgnoreCase(this string value, string value2)
    {
        return value.Equals(value2, StringComparison.CurrentCultureIgnoreCase);
    }

    public static string ToSafeUpper(this string value)
    {
        if (!value.IsNullOrEmpty()) {
            return value.ToUpper();
        }
        return value;
    }

    public static string ToSafeLower(this string value)
    {
        if (!value.IsNullOrEmpty()) {
            return value.ToLower();
        }
        return value;
    }
}

/// <summary>
/// Author: Sinan Şengün
/// </summary>
public static class DateTimeExtension
{
    public static string F(this DateTime date, string format)
    {
        return date.ToString(format, CultureInfo.GetCultureInfo("tr-TR"));
    }

    public static string F(this DateTime date, string format, CultureInfo culture)
    {
        return date.ToString(format, culture);
    }
}

/// <summary>
/// Author: Sinan Şengün
/// </summary>
public static class ExceptionExtensions
{
    public static string GetFullMessage(this Exception exc)
    {
        var builder = new StringBuilder(exc.Message);

        int index = 0;
        while (exc.InnerException != null) {
            exc = exc.InnerException;
            builder.AppendLine("\nInnerMessage{0}|{1}".F(++index, exc.Message));
        }
        return builder.ToString();
    }
    public static string GetFullStackTrace(this Exception exc)
    {
        var builder = new StringBuilder(exc.StackTrace);

        int index = 0;
        while (exc.InnerException != null) {
            exc = exc.InnerException;
            builder.AppendLine("\nInnerStackTrace{0}|{1}".F(++index, exc.StackTrace));
        }
        return builder.ToString();
    }
    public static string GetFullSource(this Exception exc)
    {
        var builder = new StringBuilder(exc.Source);

        int index = 0;
        while (exc.InnerException != null) {
            exc = exc.InnerException;
            builder.AppendLine("\nInnerSource{0}|{1}".F(++index, exc.Source));
        }
        return builder.ToString();
    }
}

/// <summary>
/// Author: Sinan Şengün
/// </summary>
public static class DateTimeExtensions
{
    public static DateTime? ToNull(this DateTime value)
    {
        if (value == default(DateTime)) {
            return null;
        }
        return new DateTime?(value);
    }
}

/// <summary>
/// Author: Sinan Şengün
/// </summary>
public static class ObjectExtensions
{
    public static bool IsNullOrEmpty(this object value)
    {
        return String.IsNullOrEmpty(value.ToString());
    }

    public static string ToSafeString(this object value)
    {
        if (value != null &&
            value != DBNull.Value) {
            return value.ToString();
        }
        return null;
    }

    public static long? ToSafeLong(this object value)
    {
        long result;
        if (value != null &&
            value != DBNull.Value &&
            long.TryParse(value.ToString(), out result)) {

            return result;
        }
        return null;
    }

    public static DateTime? ToSafeDateTime(this object value)
    {
        return value.ToSafeDateTime(CultureInfo.CurrentCulture);
    }

    public static DateTime? ToSafeDateTime(this object value, CultureInfo culture)
    {
        DateTime result;
        if (value != null &&
            value != DBNull.Value &&
            DateTime.TryParse(value.ToString(), culture, DateTimeStyles.None, out result)) {

            return result;
        }
        return null;
    }

    public static short? ToSafeShort(this object value)
    {
        short result;
        if (value != null &&
            value != DBNull.Value &&
            short.TryParse(value.ToString(), out result)) {

            return result;
        }
        return null;
    }

    public static decimal? ToSafeDecimal(this object value)
    {
        decimal result;
        if (value != null &&
            value != DBNull.Value &&
            decimal.TryParse(value.ToString(), out result)) {

            return result;
        }
        return null;
    }

    public static int? ToSafeInt(this object value)
    {
        int result;
        if (value != null &&
            value != DBNull.Value &&
            int.TryParse(value.ToString(), out result)) {

            return result;
        }
        return null;
    }

    public static byte? ToSafeByte(this object value)
    {
        byte result;
        if (value != null &&
            value != DBNull.Value &&
            byte.TryParse(value.ToString(), out result)) {

            return result;
        }
        return null;
    }

    public static object TryConvert(this object val, Type conversionType)
    {
        if (val == null || val == DBNull.Value) {
            return null;
        }

        Type nullableType;
        if ((nullableType = Nullable.GetUnderlyingType(conversionType)) != null) {
            conversionType = nullableType;
        }

        string value = val.ToString();
        if (conversionType == typeof(Guid)) {
            return new Guid(value);
        }
        else if (conversionType == typeof(DateTime)) {
            return DateTime.Parse(value, CultureInfo.InvariantCulture);
        }
        return Convert.ChangeType(value, conversionType);
    }

    public static decimal ToDecimal(this object value)
    {
        return value.ToSafeDecimal() ?? default(decimal);
    }

    public static int ToInt(this object value)
    {
        return value.ToSafeInt() ?? default(int);
    }

    public static long ToLong(this object value)
    {
        return value.ToSafeLong() ?? default(long);
    }

    public static DateTime ToDateTime(this object value)
    {
        return value.ToSafeDateTime() ?? default(DateTime);
    }

    public static T Clone<T>(this object value)
        where T : class, new()
    {
        var cloned = new T();
        var clonedType = cloned.GetType();
        var infos = value.GetType().GetProperties();

        foreach (var info in infos) {

            if (info.PropertyType.IsValueType ||
                info.PropertyType == typeof(string)) {

                var clonedInfo = clonedType.GetProperty(info.Name);
                var entityValue = info.GetValue(value, null);
                clonedInfo.SetValue(cloned, entityValue, null);
            }
        }
        return cloned;
    }


}

/// <summary>
/// Author: Sinan Şengün
/// </summary>
public static class CollectionExtensions
{
    public static bool AnyMember(this ICollection value)
    {
        if (value == null ||
            value.Count == 0) {
            return false;
        }
        else {
            return true;
        }
    }

    public static List<T> ToValue<T>(this List<T> value)
    {
        if (value == null) {
            return new List<T>();
        }
        return value;
    }
}
