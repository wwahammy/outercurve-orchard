using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Outercurve.Projects
{
    [AttributeUsageAttribute(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class MinLengthAttribute : ValidationAttribute
    {
        public MinLengthAttribute(int length)
        {
            Length = length;
        }

       
        public override bool IsValid(object value) {
            var stringVal = value as string;
            if (stringVal != null) {
                return stringVal.Length >= Length;
            }
            var enumVal = value as IEnumerable;
            if (enumVal != null) {
                return enumVal.Cast<object>().Count() >= Length;
            }

            return false;
        }

        public int Length { get; private set; }

        public override string FormatErrorMessage(string name)
        {
            // TODO:
            return base.FormatErrorMessage(name);
        }
    }
}