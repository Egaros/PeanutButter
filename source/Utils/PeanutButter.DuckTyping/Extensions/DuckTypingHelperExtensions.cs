﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using PeanutButter.DuckTyping.Comparers;
using PeanutButter.DuckTyping.Shimming;

// ReSharper disable MemberCanBePrivate.Global

namespace PeanutButter.DuckTyping.Extensions
{
    internal static class DuckTypingHelperExtensions
    {
        private static readonly Dictionary<Type, PropertyInfoContainer> _propertyCache =
            new Dictionary<Type, PropertyInfoContainer>();
        private static readonly Dictionary<Type, MethodInfoContainer> _methodCache =
            new Dictionary<Type, MethodInfoContainer>();

        private static readonly IPropertyInfoFetcher _defaultPropertyInfoFetcher = new DefaultPropertyInfoFetcher();
        internal static Dictionary<string, PropertyInfo> FindProperties(this Type type)
        {
            return type.FindProperties(_defaultPropertyInfoFetcher);
        }

        internal static Dictionary<string, PropertyInfo> FindProperties(
            this Type type,
            IPropertyInfoFetcher fetcher)
        {
            lock (_propertyCache)
            {
                CachePropertiesIfRequired(type, fetcher);
                return _propertyCache[type].PropertyInfos;
            }
        }

        private static void CachePropertiesIfRequired(Type type, IPropertyInfoFetcher fetcher)
        {
            if (!_propertyCache.ContainsKey(type))
            {
                _propertyCache[type] = GetPropertiesFor(type, fetcher);
            }
        }

        internal static Dictionary<string, PropertyInfo> FindFuzzyProperties(this Type type)
        {
            return FindFuzzyProperties(type, _defaultPropertyInfoFetcher);
        }

        internal static Dictionary<string, PropertyInfo> FindFuzzyProperties(this Type type, IPropertyInfoFetcher fetcher)
        {
            lock (_propertyCache)
            {
                CachePropertiesIfRequired(type, fetcher);
                return _propertyCache[type].FuzzyPropertyInfos;
            }
        }

        private static readonly BindingFlags _seekFlags =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy;

        private static PropertyInfoContainer GetPropertiesFor(Type type, IPropertyInfoFetcher fetcher)
        {
            var immediateProperties = fetcher.GetProperties(type, _seekFlags);
            var interfaceProperties = type.GetAllImplementedInterfaces()
                .Select(itype => fetcher.GetProperties(itype, _seekFlags))
                .SelectMany(p => p);
            var all = immediateProperties.Union(interfaceProperties).ToArray();
            return new PropertyInfoContainer(all);
        }

        internal static Dictionary<string, MethodInfo> FindMethods(this Type type)
        {
            lock (_methodCache)
            {
                CacheMethodInfosIfRequired(type);
                return _methodCache[type].MethodInfos;
            }
        }
        internal static Dictionary<string, MethodInfo> FindFuzzyMethods(
            this Type type
        )
        {
            lock (_methodCache)
            {
                CacheMethodInfosIfRequired(type);
                return _methodCache[type].FuzzyMethodInfos;
            }
        }

        internal static Type[] GetAllImplementedInterfaces(this Type interfaceType)
        {
            var result = new List<Type> { interfaceType };
            foreach (var type in interfaceType.GetInterfaces())
            {
                result.AddRange(type.GetAllImplementedInterfaces());
            }
            return result.ToArray();
        }


        private static void CacheMethodInfosIfRequired(Type type)
        {
            if (!_methodCache.ContainsKey(type))
            {
                _methodCache[type] = GetMethodsFor(type);
            }
        }

        private static MethodInfoContainer GetMethodsFor(Type type)
        {
            return new MethodInfoContainer(
                type.GetMethods(_seekFlags)
                    .Where(mi => !mi.IsSpecial())
                    .ToArray()
            );
        }

        internal static Dictionary<string, PropertyInfo> FindPrimitivePropertyMismatches(
            this Dictionary<string, PropertyInfo> src,
            Dictionary<string, PropertyInfo> other,
            bool allowFuzzy
        )
        {
            return other.Where(kvp => !src.HasNonComplexPropertyMatching(kvp.Value))
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value,
                    allowFuzzy ? Comparers.Comparers.FuzzyComparer : Comparers.Comparers.NonFuzzyComparer);
        }

        internal static bool IsSuperSetOf(
            this Dictionary<string, MethodInfo> src,
            Dictionary<string, MethodInfo> other)
        {
            return other.All(kvp => src.HasMethodMatching(kvp.Value));
        }


        static readonly HashSet<Type> _treatAsPrimitives = new HashSet<Type>(new[] {
            typeof(string),
            typeof(Guid),
            typeof(DateTime),
            typeof(DateTimeOffset),
            typeof(TimeSpan)
        });

        internal static Dictionary<string, PropertyInfo> GetPrimitiveProperties(
            this Dictionary<string, PropertyInfo> props,
            bool allowFuzzy
        )
        {
            // this will cause oddness with structs. Will have to do for now
            return props.Where(kvp => kvp.Value.PropertyType.ShouldTreatAsPrimitive())
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value,
                            allowFuzzy
                                ? Comparers.Comparers.FuzzyComparer
                                : Comparers.Comparers.NonFuzzyComparer);
        }

        internal static bool ShouldTreatAsPrimitive(this Type type)
        {
            return type.IsPrimitive || // types .net thinks are primitive
                    type.IsValueType || // includes enums, structs, https://msdn.microsoft.com/en-us/library/s1ax56ch.aspx
                    type.IsArray || 
                    _treatAsPrimitives.Contains(type); // catch cases like strings and Date(/Time) containers
        }

        internal static bool HasNonComplexPropertyMatching(
            this Dictionary<string, PropertyInfo> haystack,
            PropertyInfo needle
        )
        {
            PropertyInfo matchByName;
            if (!haystack.TryGetValue(needle.Name, out matchByName))
                return false;
            if (!matchByName.PropertyType.ShouldTreatAsPrimitive())
                return true;
            if (needle.IsReadOnly() && 
                matchByName.CanRead && 
                needle.PropertyType.IsAssignableFrom(matchByName.PropertyType))
                return true;
            return matchByName.PropertyType == needle.PropertyType &&
                    needle.IsNoMoreRestrictiveThan(matchByName);
        }

        internal static bool IsReadOnly(this PropertyInfo propInfo)
        {
            return propInfo.CanRead && !propInfo.CanWrite;
        }

        internal static bool IsNoMoreRestrictiveThan(
            this PropertyInfo src,
            PropertyInfo target
        )
        {
            return (!src.CanRead || target.CanRead) &&
                    (!src.CanWrite || target.CanWrite);
        }

        internal static bool IsTryParseMethod(
            this MethodInfo mi
        )
        {
            if (mi.Name != "TryParse")
                return false;
            var parameters = mi.GetParameters();
            if (parameters.Length != 2)
                return false;
            if (parameters[0].ParameterType != typeof(string))
                return false;
            return parameters[1].IsOut;
        }

        internal static bool HasMethodMatching(
            this Dictionary<string, MethodInfo> haystack,
            MethodInfo needle
        )
        {
            MethodInfo matchByName;
            if (!haystack.TryGetValue(needle.Name, out matchByName))
                return false;
            return matchByName.ReturnType == needle.ReturnType &&
                   matchByName.ExactlyMatchesParametersOf(needle);
        }

        internal static bool ExactlyMatchesParametersOf(
            this MethodInfo src,
            MethodInfo other
        )
        {
            var srcParameters = src.GetParameters();
            var otherParameters = other.GetParameters();
            if (srcParameters.Length != otherParameters.Length)
                return false;
            for (var i = 0; i < srcParameters.Length; i++)
            {
                var p1 = srcParameters[i];
                var p2 = otherParameters[i];
                // only care about positioning and type
                if (p1.ParameterType != p2.ParameterType)
                    return false;
            }
            return true;
        }

        private static readonly IEqualityComparer<string>[] _caseInsensitiveComparers = 
        {
            StringComparer.OrdinalIgnoreCase,
            StringComparer.CurrentCultureIgnoreCase,
            StringComparer.InvariantCultureIgnoreCase
        };

        internal static bool IsCaseSensitive(
            this IDictionary<string, object> dictionary
        )
        {
            var comparerProp = dictionary?.GetType().GetProperty("Comparer");
            return comparerProp == null
                    ? BruteForceIsCaseSensitive(dictionary)
                    : !_caseInsensitiveComparers.Contains(comparerProp.GetValue(dictionary) as IEqualityComparer<string>);
        }

        internal static bool ContainsCaseSensitiveDictionary(
            this IDictionary<string, object> dictionary)
        {
            foreach (var kvp in dictionary)
            {
                var asDict = kvp.Value as IDictionary<string, object>;    // WRONG! what about different-typed sub-dicts?
                if (asDict == null)
                    continue;
                if (asDict.IsCaseSensitive())
                    return true;
            }
            return false;
        } 

        private static bool BruteForceIsCaseSensitive(IDictionary<string, object> data)
        {
            if (data == null) return false;
            string upper = null;
            string lower = null;
            foreach (var key in data.Keys)
            {
                upper = key.ToUpper(CultureInfo.InvariantCulture);
                lower = key.ToLower(CultureInfo.InvariantCulture);
                if (upper != lower)
                    break;
                upper = null;
            }
            if (upper == null)
                return false;
            return !(data.ContainsKey(lower) && data.ContainsKey(upper));
        }

        internal static bool IsSpecial(this MethodInfo methodInfo)
        {
            return ((int)methodInfo.Attributes & (int)MethodAttributes.SpecialName) == (int)MethodAttributes.SpecialName;
        }

        internal static IDictionary<string, object> ToCaseInsensitiveDictionary(
            this IDictionary<string, object>  data
        )
        {
            return data.ToCaseInsensitiveDictionary(new List<object>());
        }

        private static IDictionary<string, object> ToCaseInsensitiveDictionary(
            this IDictionary<string, object> data,
            List<object> seenObjects
            )
        {
            // TODO: replace this logic with a case-insensitive passthrough / wrapper
            //   to enable proper write-back to the underlying data source
            if (!data.IsCaseSensitive() && !data.ContainsCaseSensitiveDictionary())
                return data;
            var outer = new Dictionary<string, object>(data, StringComparer.OrdinalIgnoreCase);
            var toReplace = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            foreach (var kvp in outer)
            {
                if (seenObjects.Contains(kvp.Value))
                    continue;
                seenObjects.Add(kvp.Value);
                var inner = kvp.Value as IDictionary<string, object>;  // sub-dicts are being coerced )':
                if (inner.IsCaseSensitive())
                {
                    toReplace[kvp.Key] = inner.ToCaseInsensitiveDictionary(seenObjects);
                }
            }
            foreach (var kvp in toReplace)
            {
                outer[kvp.Key] = kvp.Value;
            }
            return outer;
        } 

        private static readonly Type _nullableGeneric = typeof(Nullable<>);
        internal static bool IsNullable(this Type t)
        {
            if (t == null)
                return false;   // have no idea
            if (!t.IsGenericType)
                return false;
            return t.GetGenericTypeDefinition() == _nullableGeneric;
        }

    }
}