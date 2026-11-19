using System.Data;
using System.Text;
using static Dapper.SqlMapper;

namespace ProjectName.Shared.Helpers;

/// <summary>
/// Provides extension methods for formatting strings, converting enum values, creating SQL list parameters, and
/// generating CSV strings.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Formats a string using the specified arguments.
    /// </summary>
    /// <remarks>Uses the current culture to format the string.</remarks>
    /// <param name="value">The composite format string.</param>
    /// <param name="args">The objects to format.</param>
    /// <returns>The formatted string.</returns>
    public static string SetArguments(this string value, params object[] args)
    {
        return string.Format(value, args);
    }

    /// <summary>
    /// Parses the string as a case-insensitive value of enum type T.
    /// </summary>
    /// <typeparam name="T">The enum type to parse the value as.</typeparam>
    /// <param name="value">The string representation of an enum value.</param>
    /// <returns>The parsed enum value.</returns>
    public static T ToEnum<T>(this string value)
        where T : struct
    {
        return (T)System.Enum.Parse(typeof(T), value, true);
    }

    /// <summary>
    /// Converts an enumerable of integers to a table-valued SQL parameter. (only works for SQL Server)
    /// </summary>
    /// <remarks>The parameter uses the <c>UDT_IntList</c> SQL user-defined table type.</remarks>
    /// <param name="list">The integers to include in the SQL table-valued parameter.</param>
    /// <returns>A table-valued parameter that contains the supplied integers.</returns>
    public static ICustomQueryParameter ToSqlList(this IEnumerable<int> list)
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("Value", typeof(int));
        if (list?.Any() == true) foreach (var v in list) dt.Rows.Add(v);
        return dt.AsTableValuedParameter("UDT_IntList");
    }

    /// <summary>
    /// Creates a table-valued SQL parameter from a sequence of strings. (only works for SQL Server)
    /// </summary>
    /// <param name="list">The strings to include in the parameter; null or an empty sequence produces an empty parameter.</param>
    /// <returns>A table-valued query parameter using the UDT_StringList type.</returns>
    public static ICustomQueryParameter ToSqlList(this IEnumerable<string> list)
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("Value", typeof(string));
        if (list?.Any() == true) foreach (var v in list) dt.Rows.Add(v);
        return dt.AsTableValuedParameter("UDT_StringList");
    }

    /// <summary>
    /// Converts a sequence to a comma-separated string.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
    /// <param name="list">The sequence to convert.</param>
    /// <returns>A comma-separated string representing the elements in the sequence.</returns>
    public static string ToCSV<T>(this IEnumerable<T> list)
    {
        return list.ToCSV(",");
    }

    /// <summary>
    /// Converts an enumerable collection to a comma-separated values string.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="list">The collection to convert.</param>
    /// <param name="separator">The string used to separate values.</param>
    /// <returns>A string containing the collection values separated by <paramref name="separator"/>; an empty string if
    /// <paramref name="list"/> is null.</returns>
    public static string ToCSV<T>(this IEnumerable<T> list, string separator)
    {
        if (list == null) return string.Empty;

        StringBuilder sb = new StringBuilder();
        foreach (T item in list)
        {
            if (sb.Length > 0) sb.Append(separator);
            sb.Append(item?.ToString());
        }

        return sb.ToString();
    }
}

