using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Runtime.CompilerServices;
using Radzen;
using Radzen.Blazor;

namespace Clients.Dash.Infrastructure;

/// <summary>
/// represnts querable extensions for
/// filterable queries
/// </summary>
public static class QueryableExtension
{
   //
   // Summary:
   //     The filter operators
   internal static readonly IDictionary<string, string> FilterOperators = new Dictionary<string, string>
   {
      { "eq", "=" },
      { "ne", "!=" },
      { "lt", "<" },
      { "le", "<=" },
      { "gt", ">" },
      { "ge", ">=" },
      { "startswith", "StartsWith" },
      { "endswith", "EndsWith" },
      { "contains", "Contains" },
      { "DoesNotContain", "Contains" }
   };

   internal static readonly IDictionary<FilterOperator, string> LinqFilterOperators = new Dictionary<FilterOperator, string>
   {
      {  FilterOperator.Equals, "=" },
      {  FilterOperator.NotEquals, "!=" },
      {  FilterOperator.LessThan, "<" },
      {  FilterOperator.LessThanOrEquals, "<=" },
      {  FilterOperator.GreaterThan, ">" },
      {  FilterOperator.GreaterThanOrEquals, ">=" },
      {  FilterOperator.StartsWith, "StartsWith" },
      {  FilterOperator.EndsWith, "EndsWith" },
      {  FilterOperator.Contains, "Contains" },
      {  FilterOperator.DoesNotContain, "DoesNotContain" },
      {  FilterOperator.In, "In" },
      {  FilterOperator.NotIn, "NotIn" },
      {  FilterOperator.IsNull, "==" },
      {  FilterOperator.IsEmpty, "==" },
      {  FilterOperator.IsNotNull, "!=" },
      {  FilterOperator.IsNotEmpty, "!=" }
   };

   /// <summary>
   /// Builds string query for dynamic linq
   /// </summary>
   /// <typeparam name="T">Type of a grid column</typeparam>
   /// <param name="columns">Column collection</param>
   /// <returns>Query string</returns>
   public static string ToDynamicQuery<T>(this IEnumerable<RadzenGridColumn<T>> columns)
   {
      Func<RadzenGridColumn<T>, bool> predicate = (RadzenGridColumn<T> c) => c.Filterable && !string.IsNullOrEmpty(c.Type) && c.FilterValue != null && !(c.FilterValue as string == string.Empty) && c.GetFilterProperty() != null;
      if (columns.Where(predicate).Any())
      {
         string text = ((columns.FirstOrDefault()?.Grid?.LogicalFilterOperator == LogicalFilterOperator.And) ? "and" : "or");
         List<string> list = new List<string>();
         foreach (RadzenGridColumn<T> item in columns.Where(predicate))
         {
            string value = (string)Convert.ChangeType(item.FilterValue, typeof(string));
            string value2 = (string)Convert.ChangeType(item.SecondFilterValue, typeof(string));
            string type = item.Type;
            _ = item.Format;
            if (!string.IsNullOrEmpty(type) && !string.IsNullOrEmpty(value))
            {
               _ = FilterOperators[item.FilterOperator];
               string value3 = ((item.LogicalFilterOperator == LogicalFilterOperator.And) ? "and" : "or");
               if (string.IsNullOrEmpty(value2))
               {
                  list.Add(GetColumnFilter(item));
                  continue;
               }

               DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(4, 3);
               defaultInterpolatedStringHandler.AppendLiteral("(");
               defaultInterpolatedStringHandler.AppendFormatted(GetColumnFilter(item));
               defaultInterpolatedStringHandler.AppendLiteral(" ");
               defaultInterpolatedStringHandler.AppendFormatted(value3);
               defaultInterpolatedStringHandler.AppendLiteral(" ");
               defaultInterpolatedStringHandler.AppendFormatted(GetColumnFilter(item, second: true));
               defaultInterpolatedStringHandler.AppendLiteral(")");
               list.Add(defaultInterpolatedStringHandler.ToStringAndClear());
            }
         }

         return string.Join(" " + text + " ", list.Where((string i) => !string.IsNullOrEmpty(i)));
      }

      return "";
   }

   /// <summary>
   /// Builds string query for dynamic linq
   /// </summary>
   /// <typeparam name="T">Type of a data grid column</typeparam>
   /// <param name="columns">Column collection</param>
   /// <returns>Query string</returns>
   public static string ToDynamicQuery<T>(this IEnumerable<RadzenDataGridColumn<T>> columns)
   {
      Func<RadzenDataGridColumn<T>, bool> predicate = (RadzenDataGridColumn<T> c) => c.Filterable && c.FilterPropertyType != null && ((c.GetFilterValue() != null && !(c.GetFilterValue() as string == string.Empty)) || c.GetFilterOperator() == FilterOperator.IsNotNull || c.GetFilterOperator() == FilterOperator.IsNull || c.GetFilterOperator() == FilterOperator.IsEmpty || c.GetFilterOperator() == FilterOperator.IsNotEmpty) && c.GetFilterProperty() != null;
      if (columns.Where(predicate).Any())
      {
         string text = ((columns.FirstOrDefault()?.Grid?.LogicalFilterOperator == LogicalFilterOperator.And) ? "and" : "or");
         List<string> list = new List<string>();
         foreach (RadzenDataGridColumn<T> item in columns.Where(predicate))
         {
            string value = "";
            string value2 = "";
            object filterValue = item.GetFilterValue();
            object secondFilterValue = item.GetSecondFilterValue();
            if (PropertyAccess.IsDate(item.FilterPropertyType))
            {
               if (filterValue != null)
               {
                  value = ((filterValue is DateTime) ? ((DateTime)filterValue).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ") : ((filterValue is DateTimeOffset) ? ((DateTimeOffset)filterValue).UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") : ""));
               }

               if (secondFilterValue != null)
               {
                  value2 = ((secondFilterValue is DateTime) ? ((DateTime)secondFilterValue).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ") : ((secondFilterValue is DateTimeOffset) ? ((DateTimeOffset)secondFilterValue).UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") : ""));
               }
            }
            else if (PropertyAccess.IsEnum(item.FilterPropertyType) || PropertyAccess.IsNullableEnum(item.FilterPropertyType))
            {
               if (filterValue != null)
               {
                  value = ((int)filterValue).ToString();
               }

               if (secondFilterValue != null)
               {
                  value2 = ((int)secondFilterValue).ToString();
               }
            }
            else if (IsEnumerable(item.FilterPropertyType) && item.FilterPropertyType != typeof(string))
            {
               IQueryable queryable = ((IEnumerable)((filterValue != null) ? filterValue : Enumerable.Empty<object>())).AsQueryable();
               IQueryable queryable2 = ((IEnumerable)((secondFilterValue != null) ? secondFilterValue : Enumerable.Empty<object>())).AsQueryable();
               string value3 = "new []{" + string.Join(",", (queryable.ElementType == typeof(string)) ? (from string i in queryable
                                                                                                         select $"\"{i}\"").Cast<object>() : queryable.Cast<object>()) + "}";
               string value4 = "new []{" + string.Join(",", (queryable2.ElementType == typeof(string)) ? (from string i in queryable2
                                                                                                          select $"\"{i}\"").Cast<object>() : queryable2.Cast<object>()) + "}";
               if (queryable != null)
               {
                  FilterOperator filterOperator = item.GetFilterOperator();
                  FilterOperator secondFilterOperator = item.GetSecondFilterOperator();
                  _ = LinqFilterOperators[item.GetFilterOperator()];
                  string value5 = ((item.LogicalFilterOperator == LogicalFilterOperator.And) ? "and" : "or");
                  string text2 = PropertyAccess.GetProperty(item.GetFilterProperty());
                  if (text2.IndexOf(".") != -1)
                  {
                     text2 = "(" + text2 + ")";
                  }

                  if (secondFilterValue == null)
                  {
                     if (filterOperator == FilterOperator.Contains || filterOperator == FilterOperator.DoesNotContain)
                     {
                        DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 3);
                        defaultInterpolatedStringHandler.AppendFormatted((filterOperator == FilterOperator.DoesNotContain) ? "!" : "");
                        defaultInterpolatedStringHandler.AppendLiteral("(");
                        defaultInterpolatedStringHandler.AppendFormatted(value3);
                        defaultInterpolatedStringHandler.AppendLiteral(").Contains(");
                        defaultInterpolatedStringHandler.AppendFormatted(text2);
                        defaultInterpolatedStringHandler.AppendLiteral(")");
                        list.Add(defaultInterpolatedStringHandler.ToStringAndClear());
                     }
                  }
                  else if ((filterOperator == FilterOperator.Contains || filterOperator == FilterOperator.DoesNotContain) && (secondFilterOperator == FilterOperator.Contains || secondFilterOperator == FilterOperator.DoesNotContain))
                  {
                     DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(28, 7);
                     defaultInterpolatedStringHandler.AppendFormatted((filterOperator == FilterOperator.DoesNotContain) ? "!" : "");
                     defaultInterpolatedStringHandler.AppendLiteral("(");
                     defaultInterpolatedStringHandler.AppendFormatted(value3);
                     defaultInterpolatedStringHandler.AppendLiteral(").Contains(");
                     defaultInterpolatedStringHandler.AppendFormatted(text2);
                     defaultInterpolatedStringHandler.AppendLiteral(") ");
                     defaultInterpolatedStringHandler.AppendFormatted(value5);
                     defaultInterpolatedStringHandler.AppendLiteral(" ");
                     defaultInterpolatedStringHandler.AppendFormatted((secondFilterOperator == FilterOperator.DoesNotContain) ? "!" : "");
                     defaultInterpolatedStringHandler.AppendLiteral("(");
                     defaultInterpolatedStringHandler.AppendFormatted(value4);
                     defaultInterpolatedStringHandler.AppendLiteral(").Contains(");
                     defaultInterpolatedStringHandler.AppendFormatted(text2);
                     defaultInterpolatedStringHandler.AppendLiteral(")");
                     list.Add(defaultInterpolatedStringHandler.ToStringAndClear());
                  }
               }
            }
            else
            {
               value = (string)Convert.ChangeType(item.GetFilterValue(), typeof(string), CultureInfo.InvariantCulture);
               value2 = (string)Convert.ChangeType(item.GetSecondFilterValue(), typeof(string), CultureInfo.InvariantCulture);
            }

            if (!string.IsNullOrEmpty(value) || item.GetFilterOperator() == FilterOperator.IsNotNull || item.GetFilterOperator() == FilterOperator.IsNull || item.GetFilterOperator() == FilterOperator.IsEmpty || item.GetFilterOperator() == FilterOperator.IsNotEmpty)
            {
               _ = LinqFilterOperators[item.GetFilterOperator()];
               string value6 = ((item.LogicalFilterOperator == LogicalFilterOperator.And) ? "and" : "or");
               if (string.IsNullOrEmpty(value2))
               {
                  list.Add(GetColumnFilter(item, value));
                  continue;
               }

               DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(4, 3);
               defaultInterpolatedStringHandler.AppendLiteral("(");
               defaultInterpolatedStringHandler.AppendFormatted(GetColumnFilter(item, value));
               defaultInterpolatedStringHandler.AppendLiteral(" ");
               defaultInterpolatedStringHandler.AppendFormatted(value6);
               defaultInterpolatedStringHandler.AppendLiteral(" ");
               defaultInterpolatedStringHandler.AppendFormatted(GetColumnFilter(item, value2, second: true));
               defaultInterpolatedStringHandler.AppendLiteral(")");
               list.Add(defaultInterpolatedStringHandler.ToStringAndClear());
            }
         }

         return string.Join(" " + text + " ", list.Where((string i) => !string.IsNullOrEmpty(i)));
      }

      return "";
   }

   private static string GetColumnFilter<T>(RadzenGridColumn<T> column, bool second = false)
   {
      string text = PropertyAccess.GetProperty(column.GetFilterProperty());
      if (text.IndexOf(".") != -1)
      {
         text = "(" + text + ")";
      }

      if (column.Type == "string" && string.IsNullOrEmpty(column.Format))
      {
         DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(18, 2);
         defaultInterpolatedStringHandler.AppendLiteral("(");
         defaultInterpolatedStringHandler.AppendFormatted(text);
         defaultInterpolatedStringHandler.AppendLiteral(" == null ? \"\" : ");
         defaultInterpolatedStringHandler.AppendFormatted(text);
         defaultInterpolatedStringHandler.AppendLiteral(")");
         text = defaultInterpolatedStringHandler.ToStringAndClear();
      }

      string text2 = ((!second) ? column.FilterOperator : column.SecondFilterOperator);
      string text3 = FilterOperators[text2];
      if (text3 == null)
      {
         text3 = "==";
      }

      string text4 = ((!second) ? ((string)Convert.ChangeType(column.FilterValue, typeof(string))) : ((string)Convert.ChangeType(column.SecondFilterValue, typeof(string))))?.Replace("\"", "\\\"");
      string type = column.Type;
      string format = column.Format;
      switch (type)
      {
         case "string":
            {
               string value = ((column.Grid.FilterCaseSensitivity == FilterCaseSensitivity.CaseInsensitive) ? ".ToLower()" : "");
               switch (format)
               {
                  case "date-time":
                  case "date":
                     {
                        DateTime dateTime = DateTime.Parse(text4, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                        DateTime dateTime2 = ((dateTime.TimeOfDay == TimeSpan.Zero) ? dateTime.Date : dateTime);
                        string text5 = ((dateTime.TimeOfDay == TimeSpan.Zero) ? "yyyy-MM-dd" : "yyyy-MM-ddTHH:mm:ss.fffZ");
                        DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(14, 3);
                        defaultInterpolatedStringHandler.AppendFormatted(text);
                        defaultInterpolatedStringHandler.AppendLiteral(" ");
                        defaultInterpolatedStringHandler.AppendFormatted(text3);
                        defaultInterpolatedStringHandler.AppendLiteral(" DateTime(\"");
                        defaultInterpolatedStringHandler.AppendFormatted(dateTime2.ToString(text5));
                        defaultInterpolatedStringHandler.AppendLiteral("\")");
                        return defaultInterpolatedStringHandler.ToStringAndClear();
                     }
                  case "time":
                     {
                        DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(12, 3);
                        defaultInterpolatedStringHandler.AppendFormatted(text);
                        defaultInterpolatedStringHandler.AppendLiteral(" ");
                        defaultInterpolatedStringHandler.AppendFormatted(text3);
                        defaultInterpolatedStringHandler.AppendLiteral(" duration'");
                        defaultInterpolatedStringHandler.AppendFormatted(text4);
                        defaultInterpolatedStringHandler.AppendLiteral("'");
                        return defaultInterpolatedStringHandler.ToStringAndClear();
                     }
                  case "uuid":
                     {
                        DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 3);
                        defaultInterpolatedStringHandler.AppendFormatted(text);
                        defaultInterpolatedStringHandler.AppendLiteral(" ");
                        defaultInterpolatedStringHandler.AppendFormatted(text3);
                        defaultInterpolatedStringHandler.AppendLiteral(" Guid(\"");
                        defaultInterpolatedStringHandler.AppendFormatted(text4);
                        defaultInterpolatedStringHandler.AppendLiteral("\")");
                        return defaultInterpolatedStringHandler.ToStringAndClear();
                     }
               }

               if (!string.IsNullOrEmpty(text4) && text2 == "contains")
               {
                  DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(31, 5);
                  defaultInterpolatedStringHandler.AppendLiteral("(");
                  defaultInterpolatedStringHandler.AppendFormatted(text);
                  defaultInterpolatedStringHandler.AppendLiteral(" == null ? \"\" : ");
                  defaultInterpolatedStringHandler.AppendFormatted(text);
                  defaultInterpolatedStringHandler.AppendLiteral(")");
                  defaultInterpolatedStringHandler.AppendFormatted(value);
                  defaultInterpolatedStringHandler.AppendLiteral(".Contains(\"");
                  defaultInterpolatedStringHandler.AppendFormatted(text4);
                  defaultInterpolatedStringHandler.AppendLiteral("\"");
                  defaultInterpolatedStringHandler.AppendFormatted(value);
                  defaultInterpolatedStringHandler.AppendLiteral(")");
                  return defaultInterpolatedStringHandler.ToStringAndClear();
               }

               if (!string.IsNullOrEmpty(text4) && text2 == "DoesNotContain")
               {
                  DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(32, 5);
                  defaultInterpolatedStringHandler.AppendLiteral("(");
                  defaultInterpolatedStringHandler.AppendFormatted(text);
                  defaultInterpolatedStringHandler.AppendLiteral(" == null ? \"\" : !");
                  defaultInterpolatedStringHandler.AppendFormatted(text);
                  defaultInterpolatedStringHandler.AppendLiteral(")");
                  defaultInterpolatedStringHandler.AppendFormatted(value);
                  defaultInterpolatedStringHandler.AppendLiteral(".Contains(\"");
                  defaultInterpolatedStringHandler.AppendFormatted(text4);
                  defaultInterpolatedStringHandler.AppendLiteral("\"");
                  defaultInterpolatedStringHandler.AppendFormatted(value);
                  defaultInterpolatedStringHandler.AppendLiteral(")");
                  return defaultInterpolatedStringHandler.ToStringAndClear();
               }

               if (!string.IsNullOrEmpty(text4) && text2 == "startswith")
               {
                  DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(33, 5);
                  defaultInterpolatedStringHandler.AppendLiteral("(");
                  defaultInterpolatedStringHandler.AppendFormatted(text);
                  defaultInterpolatedStringHandler.AppendLiteral(" == null ? \"\" : ");
                  defaultInterpolatedStringHandler.AppendFormatted(text);
                  defaultInterpolatedStringHandler.AppendLiteral(")");
                  defaultInterpolatedStringHandler.AppendFormatted(value);
                  defaultInterpolatedStringHandler.AppendLiteral(".StartsWith(\"");
                  defaultInterpolatedStringHandler.AppendFormatted(text4);
                  defaultInterpolatedStringHandler.AppendLiteral("\"");
                  defaultInterpolatedStringHandler.AppendFormatted(value);
                  defaultInterpolatedStringHandler.AppendLiteral(")");
                  return defaultInterpolatedStringHandler.ToStringAndClear();
               }

               if (!string.IsNullOrEmpty(text4) && text2 == "endswith")
               {
                  DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(31, 5);
                  defaultInterpolatedStringHandler.AppendLiteral("(");
                  defaultInterpolatedStringHandler.AppendFormatted(text);
                  defaultInterpolatedStringHandler.AppendLiteral(" == null ? \"\" : ");
                  defaultInterpolatedStringHandler.AppendFormatted(text);
                  defaultInterpolatedStringHandler.AppendLiteral(")");
                  defaultInterpolatedStringHandler.AppendFormatted(value);
                  defaultInterpolatedStringHandler.AppendLiteral(".EndsWith(\"");
                  defaultInterpolatedStringHandler.AppendFormatted(text4);
                  defaultInterpolatedStringHandler.AppendLiteral("\"");
                  defaultInterpolatedStringHandler.AppendFormatted(value);
                  defaultInterpolatedStringHandler.AppendLiteral(")");
                  return defaultInterpolatedStringHandler.ToStringAndClear();
               }

               if (!string.IsNullOrEmpty(text4) && text2 == "eq")
               {
                  DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(24, 5);
                  defaultInterpolatedStringHandler.AppendLiteral("(");
                  defaultInterpolatedStringHandler.AppendFormatted(text);
                  defaultInterpolatedStringHandler.AppendLiteral(" == null ? \"\" : ");
                  defaultInterpolatedStringHandler.AppendFormatted(text);
                  defaultInterpolatedStringHandler.AppendLiteral(")");
                  defaultInterpolatedStringHandler.AppendFormatted(value);
                  defaultInterpolatedStringHandler.AppendLiteral(" == \"");
                  defaultInterpolatedStringHandler.AppendFormatted(text4);
                  defaultInterpolatedStringHandler.AppendLiteral("\"");
                  defaultInterpolatedStringHandler.AppendFormatted(value);
                  return defaultInterpolatedStringHandler.ToStringAndClear();
               }

               if (!string.IsNullOrEmpty(text4) && text2 == "ne")
               {
                  DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(24, 5);
                  defaultInterpolatedStringHandler.AppendLiteral("(");
                  defaultInterpolatedStringHandler.AppendFormatted(text);
                  defaultInterpolatedStringHandler.AppendLiteral(" == null ? \"\" : ");
                  defaultInterpolatedStringHandler.AppendFormatted(text);
                  defaultInterpolatedStringHandler.AppendLiteral(")");
                  defaultInterpolatedStringHandler.AppendFormatted(value);
                  defaultInterpolatedStringHandler.AppendLiteral(" != \"");
                  defaultInterpolatedStringHandler.AppendFormatted(text4);
                  defaultInterpolatedStringHandler.AppendLiteral("\"");
                  defaultInterpolatedStringHandler.AppendFormatted(value);
                  return defaultInterpolatedStringHandler.ToStringAndClear();
               }

               break;
            }
         case "number":
         case "integer":
            {
               DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(2, 3);
               defaultInterpolatedStringHandler.AppendFormatted(text);
               defaultInterpolatedStringHandler.AppendLiteral(" ");
               defaultInterpolatedStringHandler.AppendFormatted(text3);
               defaultInterpolatedStringHandler.AppendLiteral(" ");
               defaultInterpolatedStringHandler.AppendFormatted(text4);
               return defaultInterpolatedStringHandler.ToStringAndClear();
            }
         case "boolean":
            return text + " == " + text4;
      }

      return "";
   }

   private static string GetColumnFilter<T>(RadzenDataGridColumn<T> column, string value, bool second = false)
   {
      string text = PropertyAccess.GetProperty(column.GetFilterProperty());
      if (text.IndexOf(".") != -1)
      {
         text = "(" + text + ")";
      }

      FilterOperator filterOperator = ((!second) ? column.GetFilterOperator() : column.GetSecondFilterOperator());
      string text2 = LinqFilterOperators[filterOperator];
      if (text2 == null)
      {
         text2 = "==";
      }

      if (column.FilterPropertyType == typeof(string))
      {
         string value2 = ((column.Grid.FilterCaseSensitivity == FilterCaseSensitivity.CaseInsensitive) ? ".ToLower()" : "");
         value = value?.Replace("\"", "\\\"");
         if (!string.IsNullOrEmpty(value) && filterOperator == FilterOperator.Contains)
         {
            DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(31, 5);
            defaultInterpolatedStringHandler.AppendLiteral("(");
            defaultInterpolatedStringHandler.AppendFormatted(text);
            defaultInterpolatedStringHandler.AppendLiteral(" == null ? \"\" : ");
            defaultInterpolatedStringHandler.AppendFormatted(text);
            defaultInterpolatedStringHandler.AppendLiteral(")");
            defaultInterpolatedStringHandler.AppendFormatted(value2);
            defaultInterpolatedStringHandler.AppendLiteral(".Contains(\"");
            defaultInterpolatedStringHandler.AppendFormatted(value);
            defaultInterpolatedStringHandler.AppendLiteral("\"");
            defaultInterpolatedStringHandler.AppendFormatted(value2);
            defaultInterpolatedStringHandler.AppendLiteral(")");
            return defaultInterpolatedStringHandler.ToStringAndClear();
         }

         if (!string.IsNullOrEmpty(value) && filterOperator == FilterOperator.DoesNotContain)
         {
            DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(32, 5);
            defaultInterpolatedStringHandler.AppendLiteral("!(");
            defaultInterpolatedStringHandler.AppendFormatted(text);
            defaultInterpolatedStringHandler.AppendLiteral(" == null ? \"\" : ");
            defaultInterpolatedStringHandler.AppendFormatted(text);
            defaultInterpolatedStringHandler.AppendLiteral(")");
            defaultInterpolatedStringHandler.AppendFormatted(value2);
            defaultInterpolatedStringHandler.AppendLiteral(".Contains(\"");
            defaultInterpolatedStringHandler.AppendFormatted(value);
            defaultInterpolatedStringHandler.AppendLiteral("\"");
            defaultInterpolatedStringHandler.AppendFormatted(value2);
            defaultInterpolatedStringHandler.AppendLiteral(")");
            return defaultInterpolatedStringHandler.ToStringAndClear();
         }

         if (!string.IsNullOrEmpty(value) && filterOperator == FilterOperator.StartsWith)
         {
            DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(33, 5);
            defaultInterpolatedStringHandler.AppendLiteral("(");
            defaultInterpolatedStringHandler.AppendFormatted(text);
            defaultInterpolatedStringHandler.AppendLiteral(" == null ? \"\" : ");
            defaultInterpolatedStringHandler.AppendFormatted(text);
            defaultInterpolatedStringHandler.AppendLiteral(")");
            defaultInterpolatedStringHandler.AppendFormatted(value2);
            defaultInterpolatedStringHandler.AppendLiteral(".StartsWith(\"");
            defaultInterpolatedStringHandler.AppendFormatted(value);
            defaultInterpolatedStringHandler.AppendLiteral("\"");
            defaultInterpolatedStringHandler.AppendFormatted(value2);
            defaultInterpolatedStringHandler.AppendLiteral(")");
            return defaultInterpolatedStringHandler.ToStringAndClear();
         }

         if (!string.IsNullOrEmpty(value) && filterOperator == FilterOperator.EndsWith)
         {
            DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(31, 5);
            defaultInterpolatedStringHandler.AppendLiteral("(");
            defaultInterpolatedStringHandler.AppendFormatted(text);
            defaultInterpolatedStringHandler.AppendLiteral(" == null ? \"\" : ");
            defaultInterpolatedStringHandler.AppendFormatted(text);
            defaultInterpolatedStringHandler.AppendLiteral(")");
            defaultInterpolatedStringHandler.AppendFormatted(value2);
            defaultInterpolatedStringHandler.AppendLiteral(".EndsWith(\"");
            defaultInterpolatedStringHandler.AppendFormatted(value);
            defaultInterpolatedStringHandler.AppendLiteral("\"");
            defaultInterpolatedStringHandler.AppendFormatted(value2);
            defaultInterpolatedStringHandler.AppendLiteral(")");
            return defaultInterpolatedStringHandler.ToStringAndClear();
         }

         if (!string.IsNullOrEmpty(value) && filterOperator == FilterOperator.Equals)
         {
            DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(24, 5);
            defaultInterpolatedStringHandler.AppendLiteral("(");
            defaultInterpolatedStringHandler.AppendFormatted(text);
            defaultInterpolatedStringHandler.AppendLiteral(" == null ? \"\" : ");
            defaultInterpolatedStringHandler.AppendFormatted(text);
            defaultInterpolatedStringHandler.AppendLiteral(")");
            defaultInterpolatedStringHandler.AppendFormatted(value2);
            defaultInterpolatedStringHandler.AppendLiteral(" == \"");
            defaultInterpolatedStringHandler.AppendFormatted(value);
            defaultInterpolatedStringHandler.AppendLiteral("\"");
            defaultInterpolatedStringHandler.AppendFormatted(value2);
            return defaultInterpolatedStringHandler.ToStringAndClear();
         }

         if (!string.IsNullOrEmpty(value) && filterOperator == FilterOperator.NotEquals)
         {
            DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(24, 5);
            defaultInterpolatedStringHandler.AppendLiteral("(");
            defaultInterpolatedStringHandler.AppendFormatted(text);
            defaultInterpolatedStringHandler.AppendLiteral(" == null ? \"\" : ");
            defaultInterpolatedStringHandler.AppendFormatted(text);
            defaultInterpolatedStringHandler.AppendLiteral(")");
            defaultInterpolatedStringHandler.AppendFormatted(value2);
            defaultInterpolatedStringHandler.AppendLiteral(" != \"");
            defaultInterpolatedStringHandler.AppendFormatted(value);
            defaultInterpolatedStringHandler.AppendLiteral("\"");
            defaultInterpolatedStringHandler.AppendFormatted(value2);
            return defaultInterpolatedStringHandler.ToStringAndClear();
         }

         switch (filterOperator)
         {
            case FilterOperator.IsNull:
               return "np(" + text + ") == null";
            case FilterOperator.IsEmpty:
               return "np(" + text + ") == \"\"";
            case FilterOperator.IsNotEmpty:
               return "np(" + text + ") != \"\"";
            case FilterOperator.IsNotNull:
               return "np(" + text + ") != null";
         }
      }
      else
      {
         if (PropertyAccess.IsNumeric(column.FilterPropertyType))
         {
            switch (filterOperator)
            {
               case FilterOperator.IsNull:
               case FilterOperator.IsNotNull:
                  return text + " " + text2 + " null";
               case FilterOperator.IsEmpty:
               case FilterOperator.IsNotEmpty:
                  return text + " " + text2 + " \"\"";
               default:
                  {
                     DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(2, 3);
                     defaultInterpolatedStringHandler.AppendFormatted(text);
                     defaultInterpolatedStringHandler.AppendLiteral(" ");
                     defaultInterpolatedStringHandler.AppendFormatted(text2);
                     defaultInterpolatedStringHandler.AppendLiteral(" ");
                     defaultInterpolatedStringHandler.AppendFormatted(value);
                     return defaultInterpolatedStringHandler.ToStringAndClear();
                  }
            }
         }

         if (column.FilterPropertyType == typeof(DateTime) || column.FilterPropertyType == typeof(DateTime?) || column.FilterPropertyType == typeof(DateTimeOffset) || column.FilterPropertyType == typeof(DateTimeOffset?))
         {
            switch (filterOperator)
            {
               case FilterOperator.IsNull:
               case FilterOperator.IsNotNull:
                  return text + " " + text2 + " null";
               case FilterOperator.IsEmpty:
               case FilterOperator.IsNotEmpty:
                  return text + " " + text2 + " \"\"";
               default:
                  {
                     DateTime dateTime = DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                     DateTime dateTime2 = ((dateTime.TimeOfDay == TimeSpan.Zero) ? dateTime.Date : dateTime);
                     string text3 = ((dateTime.TimeOfDay == TimeSpan.Zero) ? "yyyy-MM-dd" : "yyyy-MM-ddTHH:mm:ss.fff");
                     string value3 = ((column.FilterPropertyType == typeof(DateTimeOffset) || column.FilterPropertyType == typeof(DateTimeOffset?)) ? "DateTimeOffset" : "DateTime");
                     DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(6, 4);
                     defaultInterpolatedStringHandler.AppendFormatted(text);
                     defaultInterpolatedStringHandler.AppendLiteral(" ");
                     defaultInterpolatedStringHandler.AppendFormatted(text2);
                     defaultInterpolatedStringHandler.AppendLiteral(" ");
                     defaultInterpolatedStringHandler.AppendFormatted(value3);
                     defaultInterpolatedStringHandler.AppendLiteral("(\"");
                     defaultInterpolatedStringHandler.AppendFormatted(dateTime2.ToString(text3));
                     defaultInterpolatedStringHandler.AppendLiteral("\")");
                     return defaultInterpolatedStringHandler.ToStringAndClear();
                  }
            }
         }

         if (column.FilterPropertyType == typeof(bool) || column.FilterPropertyType == typeof(bool?))
         {
            return text + " == " + value;
         }

         if (column.FilterPropertyType == typeof(Guid) || column.FilterPropertyType == typeof(Guid?))
         {
            switch (filterOperator)
            {
               case FilterOperator.IsNull:
               case FilterOperator.IsNotNull:
                  return text + " " + text2 + " null";
               case FilterOperator.IsEmpty:
               case FilterOperator.IsNotEmpty:
                  return text + " " + text2 + " \"\"";
               default:
                  {
                     DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 3);
                     defaultInterpolatedStringHandler.AppendFormatted(text);
                     defaultInterpolatedStringHandler.AppendLiteral(" ");
                     defaultInterpolatedStringHandler.AppendFormatted(text2);
                     defaultInterpolatedStringHandler.AppendLiteral(" Guid(\"");
                     defaultInterpolatedStringHandler.AppendFormatted(value);
                     defaultInterpolatedStringHandler.AppendLiteral("\")");
                     return defaultInterpolatedStringHandler.ToStringAndClear();
                  }
            }
         }
      }

      return "";
   }


   /// <summary>
   /// Is a type is enumerable
   /// </summary>
   /// <param name="type"></param>
   /// <returns></returns>
   public static bool IsEnumerable(Type type)
   {
      if (!typeof(IEnumerable)!.IsAssignableFrom(type))
      {
         return typeof(IEnumerable<>)!.IsAssignableFrom(type);
      }

      return true;
   }
}