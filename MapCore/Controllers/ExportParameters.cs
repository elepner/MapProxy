using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using static System.Char;

namespace MapCore.Controllers
{
    public class ExportParameters
    {
        public int Dpi { get; set; }
        public bool Transparent { get; set; }
        public string Format { get; set; }
        
        public ArrayParamater Layers { get; set; }
        
        public ArrayParamater Bbox { get; set; }

        public ArrayParamater Size { get; set; }
    }

    [TypeConverter(typeof(CommaSeparatedNumberConverter))]
    public class ArrayParamater
    {
        public string Prefix { get; set; }
        public double[] Values { get; set; }
    }
    

    public class CommaSeparatedNumberConverter : TypeConverter
    {
        
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context,
        CultureInfo culture, object value)
        {
            var str = value as string;
            if (str != null)
            {
                var indexDigit = str.TakeWhile(c => !IsDigit(c)).Count();
                var substr = str.Substring(indexDigit, str.Length - indexDigit);
                
                var arr = new ArrayParamater {Values = substr.Split(',').Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToArray()};
                return arr;
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}
