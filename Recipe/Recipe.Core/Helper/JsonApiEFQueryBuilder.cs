using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Data.Entity;
using System.Reflection;

namespace Recipe.Common.Helper
{
    internal interface ISelector<T>
    {
        IOrderedQueryable<T> ApplyInitially(IQueryable<T> unsortedQuery);

        IOrderedQueryable<T> ApplySubsequently(IOrderedQueryable<T> currentQuery);
    }

    internal class Selector<TResource, TProperty> : ISelector<TResource>
    {
        private readonly bool _isDescending;

        private readonly Expression<Func<TResource, TProperty>> _propertyAccessorExpression;

        public Selector(bool isDescending, Expression<Func<TResource, TProperty>> propertyAccessorExpression)
        {
            _isDescending = isDescending;
            _propertyAccessorExpression = propertyAccessorExpression;
        }

        public IOrderedQueryable<TResource> ApplyInitially(IQueryable<TResource> unsortedQuery)
        {
            if (_isDescending) return unsortedQuery.OrderByDescending(_propertyAccessorExpression);
            return unsortedQuery.OrderBy(_propertyAccessorExpression);
        }

        public IOrderedQueryable<TResource> ApplySubsequently(IOrderedQueryable<TResource> currentQuery)
        {
            if (_isDescending) return currentQuery.ThenByDescending(_propertyAccessorExpression);
            return currentQuery.ThenBy(_propertyAccessorExpression);
        }
    }

    public static class JsonApiEFQueryBuilder
    {
        private const string IncludeQueryParamKey = "include";
        private const string SortQueryParamKey = "sort";
        private const int DefaultPageSize = 100;
        private const string PageNumberQueryParam = "page.number";
        private const string PageSizeQueryParam = "page.size";

        public static IQueryable<T> GenerateQuery<T>(this IQueryable<T> query, HttpRequestMessage request)
        {
            return query.GenerateQuery<T>(request.GetJsonApiRequest());
        }

        public static IQueryable<T> GenerateQuery<T>(this IQueryable<T> query, JsonApiRequest request)
        {
            query = query.GenerateIncludeQuery<T>(request.Include);
            query = query.GenerateFilterQuery<T>(request.Filters);
            query = query.GenerateSortQuery<T>(request.Sort);
            query = query.GeneratePagination<T>(request.Pagination);

            return query;
        }

        public static IQueryable<T> GenerateSortQuery<T>(this IQueryable<T> query, List<string> sortExpressions)
        {
            if (sortExpressions == null || sortExpressions.Count == 0)
            {
                return query;
            }
            var selectors = new List<ISelector<T>>();
            var usedProperties = new Dictionary<PropertyInfo, object>();


            foreach (var sortExpression in sortExpressions)
            {
                if (string.IsNullOrEmpty(sortExpression))
                {
                    throw new Exception("One of the sort expressions is empty.");
                }

                bool ascending;
                string fieldName;
                if (sortExpression[0] == '-')
                {
                    ascending = false;
                    fieldName = sortExpression.Substring(1);
                }
                else
                {
                    ascending = true;
                    fieldName = sortExpression;
                }

                if (string.IsNullOrWhiteSpace(fieldName))
                {
                    throw new Exception("One of the sort expressions is empty.");
                }

                var paramExpr = Expression.Parameter(typeof(T));
                Expression sortValueExpression;

                var property = typeof(T).GetJsonApiProperty(fieldName);
                if (property == null)
                {
                    continue;
                }

                if (usedProperties.ContainsKey(property))
                {
                    continue;
                }

                usedProperties[property] = null;
                sortValueExpression = Expression.Property(paramExpr, property);
                var lambda = Expression.Lambda(sortValueExpression, paramExpr);
                var selectorType = typeof(Selector<,>).MakeGenericType(typeof(T), sortValueExpression.Type);
                var selector = Activator.CreateInstance(selectorType, !ascending, lambda);

                selectors.Add((ISelector<T>)selector);
            }

            var firstSelector = selectors.First();

            IOrderedQueryable<T> workingQuery = firstSelector.ApplyInitially(query);
            query = selectors.Skip(1).Aggregate(workingQuery, (current, selector) => selector.ApplySubsequently(current));
            return query;
        }

        public static IQueryable<T> GenerateIncludeQuery<T>(this IQueryable<T> query, List<string> includeExpressions)
        {
            if (includeExpressions == null || includeExpressions.Count == 0)
            {
                return query;
            }

            var usedProperties = new Dictionary<PropertyInfo, object>();

            foreach (var includeExpression in includeExpressions)
            {
                if (string.IsNullOrEmpty(includeExpression))
                {
                    throw new Exception("One of the include expressions is empty.");
                }

                var property = typeof(T).GetJsonApiProperty(includeExpression);
                if (property == null)
                {
                    continue;
                }

                if (usedProperties.ContainsKey(property))
                {
                    continue;
                }

                usedProperties[property] = null;
                query = query.Include(property.PropertyType.Name);
            }

            return query;
        }

        private static PropertyInfo GetJsonApiProperty(this Type type, string propertyName)
        {
            string name = propertyName.Replace("-", string.Empty).ToLower();
            var property = type.GetProperties().Where(x => x.Name.ToLower() == name).FirstOrDefault();
            return property;
        }

        public static IQueryable<T> GenerateFilterQuery<T>(this IQueryable<T> query, Dictionary<string, string> filters)
        {
            Expression expr = null;
            var param = Expression.Parameter(typeof(T));

            if (filters != null && filters.Keys.Count > 0)
            {
                foreach (var key in filters.Keys)
                {
                    var property = typeof(T).GetJsonApiProperty(key);
                    Expression propertyExpr = null;
                    if (property != null)
                    {
                        propertyExpr = GetPredicateBodyForProperty(property, filters[key], param);
                    }

                    if (expr != null)
                    {
                        expr = Expression.AndAlso(expr, propertyExpr);
                    }
                    else
                    {
                        expr = propertyExpr;
                    }
                }
            }

            if (expr != null)
            {
                var lambdaExpr = Expression.Lambda<Func<T, bool>>(expr, param);
                query = query.Where(lambdaExpr);
            }

            return query;
        }

        private static Expression GetPredicateBodyForProperty(PropertyInfo prop, string queryValue, ParameterExpression param)
        {
            var propertyType = prop.PropertyType;

            Expression expr;
            if (propertyType == typeof(String))
            {
                if (String.IsNullOrWhiteSpace(queryValue))
                {
                    Expression propertyExpr = Expression.Property(param, prop);
                    expr = Expression.Equal(propertyExpr, Expression.Constant(null));
                }
                else
                {
                    Expression propertyExpr = Expression.Property(param, prop);
                    expr = Expression.Equal(propertyExpr, Expression.Constant(queryValue));
                }
            }
            else if (propertyType == typeof(Boolean))
            {
                bool value;
                expr = bool.TryParse(queryValue, out value)
                    ? GetPropertyExpression(value, prop, param)
                    : Expression.Constant(false);
            }
            else if (propertyType == typeof(Boolean?))
            {
                bool tmp;
                var value = bool.TryParse(queryValue, out tmp) ? tmp : (bool?)null;
                expr = GetPropertyExpression(value, prop, param);
            }
            else if (propertyType == typeof(SByte))
            {
                SByte value;
                expr = SByte.TryParse(queryValue, out value)
                    ? GetPropertyExpression(value, prop, param)
                    : Expression.Constant(false);
            }
            else if (propertyType == typeof(SByte?))
            {
                SByte tmp;
                var value = SByte.TryParse(queryValue, out tmp) ? tmp : (SByte?)null;
                expr = GetPropertyExpression(value, prop, param);
            }
            else if (propertyType == typeof(Byte))
            {
                Byte value;
                expr = Byte.TryParse(queryValue, out value)
                    ? GetPropertyExpression(value, prop, param)
                    : Expression.Constant(false);
            }
            else if (propertyType == typeof(Byte?))
            {
                Byte tmp;
                var value = Byte.TryParse(queryValue, out tmp) ? tmp : (Byte?)null;
                expr = GetPropertyExpression(value, prop, param);
            }
            else if (propertyType == typeof(Int16))
            {
                Int16 value;
                expr = Int16.TryParse(queryValue, out value)
                    ? GetPropertyExpression(value, prop, param)
                    : Expression.Constant(false);
            }
            else if (propertyType == typeof(Int16?))
            {
                Int16 tmp;
                var value = Int16.TryParse(queryValue, out tmp) ? tmp : (Int16?)null;
                expr = GetPropertyExpression(value, prop, param);
            }
            else if (propertyType == typeof(UInt16))
            {
                UInt16 value;
                expr = UInt16.TryParse(queryValue, out value)
                    ? GetPropertyExpression(value, prop, param)
                    : Expression.Constant(false);
            }
            else if (propertyType == typeof(UInt16?))
            {
                UInt16 tmp;
                var value = UInt16.TryParse(queryValue, out tmp) ? tmp : (UInt16?)null;
                expr = GetPropertyExpression(value, prop, param);
            }
            else if (propertyType == typeof(Int32))
            {
                Int32 value;
                expr = Int32.TryParse(queryValue, out value)
                    ? GetPropertyExpression(value, prop, param)
                    : Expression.Constant(false);
            }
            else if (propertyType == typeof(Int32?))
            {
                Int32 tmp;
                var value = Int32.TryParse(queryValue, out tmp) ? tmp : (Int32?)null;
                expr = GetPropertyExpression(value, prop, param);
            }
            else if (propertyType == typeof(UInt32))
            {
                UInt32 value;
                expr = UInt32.TryParse(queryValue, out value)
                    ? GetPropertyExpression(value, prop, param)
                    : Expression.Constant(false);
            }
            else if (propertyType == typeof(UInt32?))
            {
                UInt32 tmp;
                var value = UInt32.TryParse(queryValue, out tmp) ? tmp : (UInt32?)null;
                expr = GetPropertyExpression(value, prop, param);
            }
            else if (propertyType == typeof(Int64))
            {
                Int64 value;
                expr = Int64.TryParse(queryValue, out value)
                    ? GetPropertyExpression(value, prop, param)
                    : Expression.Constant(false);
            }
            else if (propertyType == typeof(Int64?))
            {
                Int64 tmp;
                var value = Int64.TryParse(queryValue, out tmp) ? tmp : (Int64?)null;
                expr = GetPropertyExpression(value, prop, param);
            }
            else if (propertyType == typeof(UInt64))
            {
                UInt64 value;
                expr = UInt64.TryParse(queryValue, out value)
                    ? GetPropertyExpression(value, prop, param)
                    : Expression.Constant(false);
            }
            else if (propertyType == typeof(UInt64?))
            {
                UInt64 tmp;
                var value = UInt64.TryParse(queryValue, out tmp) ? tmp : (UInt64?)null;
                expr = GetPropertyExpression(value, prop, param);
            }
            else if (propertyType == typeof(Single))
            {
                Single value;
                expr = Single.TryParse(queryValue, NumberStyles.Any, CultureInfo.InvariantCulture, out value)
                    ? GetPropertyExpression(value, prop, param)
                    : Expression.Constant(false);
            }
            else if (propertyType == typeof(Single?))
            {
                Single tmp;
                var value = Single.TryParse(queryValue, NumberStyles.Any, CultureInfo.InvariantCulture, out tmp) ? tmp : (Single?)null;
                expr = GetPropertyExpression(value, prop, param);
            }
            else if (propertyType == typeof(Double))
            {
                Double value;
                expr = Double.TryParse(queryValue, NumberStyles.Any, CultureInfo.InvariantCulture, out value)
                    ? GetPropertyExpression(value, prop, param)
                    : Expression.Constant(false);
            }
            else if (propertyType == typeof(Double?))
            {
                Double tmp;
                var value = Double.TryParse(queryValue, NumberStyles.Any, CultureInfo.InvariantCulture, out tmp) ? tmp : (Double?)null;
                expr = GetPropertyExpression(value, prop, param);
            }
            else if (propertyType == typeof(Decimal))
            {
                Decimal value;
                expr = Decimal.TryParse(queryValue, NumberStyles.Any, CultureInfo.InvariantCulture, out value)
                    ? GetPropertyExpression(value, prop, param)
                    : Expression.Constant(false);
            }
            else if (propertyType == typeof(Decimal?))
            {
                Decimal tmp;
                var value = Decimal.TryParse(queryValue, NumberStyles.Any, CultureInfo.InvariantCulture, out tmp) ? tmp : (Decimal?)null;
                expr = GetPropertyExpression(value, prop, param);
            }
            else if (propertyType == typeof(DateTime))
            {
                DateTime value;
                expr = DateTime.TryParse(queryValue, out value)
                    ? GetPropertyExpression(value, prop, param)
                    : Expression.Constant(false);
            }
            else if (propertyType == typeof(DateTime?))
            {
                DateTime tmp;
                var value = DateTime.TryParse(queryValue, out tmp) ? tmp : (DateTime?)null;
                expr = GetPropertyExpression(value, prop, param);
            }
            else if (propertyType == typeof(DateTimeOffset))
            {
                DateTimeOffset value;
                expr = DateTimeOffset.TryParse(queryValue, out value)
                    ? GetPropertyExpression<DateTimeOffset>(value, prop, param)
                    : Expression.Constant(false);
            }
            else if (propertyType == typeof(DateTimeOffset?))
            {
                DateTimeOffset tmp;
                var value = DateTimeOffset.TryParse(queryValue, out tmp) ? tmp : (DateTimeOffset?)null;
                expr = GetPropertyExpression(value, prop, param);
            }
            else if (propertyType.IsEnum)
            {
                int value;
                expr = (int.TryParse(queryValue, out value) && System.Enum.IsDefined(propertyType, value))
                    ? GetEnumPropertyExpression(value, prop, param)
                    : Expression.Constant(false);
            }
            else if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                     propertyType.GenericTypeArguments[0].IsEnum)
            {
                int tmp;
                var value = int.TryParse(queryValue, out tmp) ? tmp : (int?)null;
                expr = GetEnumPropertyExpression(value, prop, param);
            }
            else
            {
                expr = Expression.Constant(true);
            }

            return expr;
        }

        private static Expression GetPropertyExpression<T>(T value, PropertyInfo property, ParameterExpression param)
        {
            Expression propertyExpr = Expression.Property(param, property);
            var valueExpr = Expression.Constant(value);
            Expression castedConstantExpr = Expression.Convert(valueExpr, typeof(T));
            return Expression.Equal(propertyExpr, castedConstantExpr);
        }

        private static Expression GetEnumPropertyExpression(int? value, PropertyInfo property, ParameterExpression param)
        {
            Expression propertyExpr = Expression.Property(param, property);
            var castedValueExpr = Expression.Convert(Expression.Constant(value), typeof(int?));
            var castedPropertyExpr = Expression.Convert(propertyExpr, typeof(int?));
            return Expression.Equal(castedPropertyExpr, castedValueExpr);
        }

        public static IQueryable<T> GeneratePagination<T>(this IQueryable<T> query, JsonApiPagination pagination)
        {
            if (pagination != null)
            {
                query = query.Skip(pagination.PageNumber * pagination.PageSize).Take(pagination.PageSize);
            }

            return query;
        }

        public static JsonApiRequest GetJsonApiRequest(this HttpRequestMessage request)
        {
            if (request != null)
            {
                JsonApiRequest jsonAPIRequest = new JsonApiRequest();
                jsonAPIRequest.Include = request.ExtractIncludeExpressions();
                jsonAPIRequest.Sort = request.ExtractSortExpressions();
                jsonAPIRequest.Filters = request.ExtractFilters();
                jsonAPIRequest.Pagination = request.ExtractPagination();

                return jsonAPIRequest;
            }

            return null;
        }

        public static JsonApiPagination ExtractPagination(this HttpRequestMessage request)
        {
            var hasPageNumberParam = false;
            var hasPageSizeParam = false;
            var pageNumber = 0;
            var pageSize = DefaultPageSize;
            foreach (var kvp in request.GetQueryNameValuePairs())
            {
                if (kvp.Key == PageNumberQueryParam)
                {
                    hasPageNumberParam = true;
                    if (!int.TryParse(kvp.Value, out pageNumber))
                    {
                        throw new Exception("Page number must be a positive integer.");
                    }
                }
                else if (kvp.Key == PageSizeQueryParam)
                {
                    hasPageSizeParam = true;
                    if (!int.TryParse(kvp.Value, out pageSize))
                    {
                        throw new Exception("Page size must be a positive integer.");
                    }
                }
            }

            if (!hasPageNumberParam && !hasPageSizeParam)
            {
                return null;
            }

            if ((hasPageNumberParam && !hasPageSizeParam))
            {
                throw new Exception(string.Format("In order for paging to work properly, if either {0} or {1} is set, both must be.",
                        PageNumberQueryParam, PageSizeQueryParam));
            }

            if ((!hasPageNumberParam && hasPageSizeParam))
            {
                throw new Exception(string.Format("In order for paging to work properly, if either {0} or {1} is set, both must be.",
                        PageNumberQueryParam, PageSizeQueryParam));
            }

            if (pageNumber < 0)
            {
                throw new Exception("Page number must not be negative.");
            }

            if (pageSize <= 0)
            {
                throw new Exception("Page size must be greater than or equal to 1.");
            }


            JsonApiPagination pagination = new JsonApiPagination();
            pagination.PageNumber = pageNumber;
            pagination.PageSize = pageSize;

            return pagination;
        }

        public static Dictionary<string, string> ExtractFilters(this HttpRequestMessage request)
        {
            Dictionary<string, string> filters = new Dictionary<string, string>();
            var queryPairs = request.GetQueryNameValuePairs();
            foreach (var queryPair in queryPairs)
            {
                if (String.IsNullOrWhiteSpace(queryPair.Key))
                    continue;

                if (!queryPair.Key.StartsWith("filter."))
                    continue;

                var filterField = queryPair.Key.Substring(7); // Skip "filter."
                filters.Add(filterField, queryPair.Value);
            }

            return filters;

        }

        public static List<string> ExtractSortExpressions(this HttpRequestMessage requestMessage)
        {
            var queryParams = requestMessage.GetQueryNameValuePairs();
            var sortParam = queryParams.FirstOrDefault(kvp => kvp.Key == SortQueryParamKey);
            if (sortParam.Key != SortQueryParamKey) return new List<string>();
            return sortParam.Value.Split(',').ToList();
        }

        public static List<string> ExtractIncludeExpressions(this HttpRequestMessage requestMessage)
        {
            var queryParams = requestMessage.GetQueryNameValuePairs();
            var sortParam = queryParams.FirstOrDefault(kvp => kvp.Key == IncludeQueryParamKey);
            if (sortParam.Key != IncludeQueryParamKey) return new List<string>();
            return sortParam.Value.Split(',').ToList();
        }
    }

    public class JsonApiPagination
    {
        public int PageSize { get; set; }

        public int PageNumber { get; set; }
    }

    public class JsonApiRequest
    {
        public JsonApiRequest()
        {
            this.Filters = new Dictionary<string, string>();
        }

        public List<string> Include { get; set; }

        public List<string> Sort { get; set; }

        public Dictionary<string, string> Filters { get; set; }

        public JsonApiPagination Pagination { get; set; }
    }
}
