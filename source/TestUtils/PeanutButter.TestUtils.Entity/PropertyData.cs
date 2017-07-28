﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using PeanutButter.Utils;

namespace PeanutButter.TestUtils.Entity
{
    public class PropertyData<T>
    {
        public string PropertyPath { get; private set; }
        public PropertyInfo PropertyInfo { get; private set; }
        public IEnumerable<Attribute> CustomAttributes { get; private set; }
        public Type ParentType { get; private set; }

        public PropertyData(T item, Expression<Func<T, object>> propertyExpression)
        {
            PropertyPath = ExpressionUtil.GetMemberPathFor(propertyExpression);
            PropertyInfo = item.GetPropertyInfoFor(PropertyPath);
            CustomAttributes = PropertyInfo.GetCustomAttributes(true).Cast<Attribute>().ToArray();
            ParentType = typeof(T);
        }

        public TAttribute CustomAttribute<TAttribute>() where TAttribute: Attribute
        {
            return CustomAttributes.OfType<TAttribute>().FirstOrDefault();
        }
    }
}
