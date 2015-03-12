using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using System.Web.Http.ValueProviders;

namespace AspnetWebapiBootcamp.Controllers
{
    public class ValuesController : ApiController
    {
//        public HttpResponseMessage Get([FromUri] GeoPoint location)
//        public HttpResponseMessage Post([FromBody] GeoPoint location)
//        public HttpResponseMessage Get(GeoPoint location)
//        public HttpResponseMessage Get([ModelBinder(typeof(GeoPointModelBinder))]GeoPoint location)
        public HttpResponseMessage Get([ModelBinder]GeoPoint location)
        {
            return new HttpResponseMessage
            {
                Content = new StringContent(string.Format("{0},{1}",location.Latitude,location.Longitude))
            };
        }
    }

//    [TypeConverter(typeof(GeoPointConverter))]
//    [ModelBinder(typeof(GeoPointModelBinder))]
    public class GeoPoint
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public static bool TryParse(string value, out GeoPoint point)
        {
            point = null;
            var parts = value.Split(',');
            if (parts.Length != 2)
            {
                return false;
            }
            double latitude, longitude;
            if (double.TryParse(parts[0], out latitude) &&
                double.TryParse(parts[1], out longitude))
            {
                point = new GeoPoint {Longitude = longitude, Latitude = latitude};
                return true;
            }
            return false;
        }
    }

    public class GeoPointConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof (string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                GeoPoint point;
                if (GeoPoint.TryParse((string) value, out point))
                {
                    return point;
                }
            }
            return base.ConvertFrom(context, culture, value);
        }
    }

    public class GeoPointModelBinder: IModelBinder
    {
        private static ConcurrentDictionary<string,GeoPoint> _locations = new ConcurrentDictionary<string, GeoPoint>(){};

        static GeoPointModelBinder()
        {
            _locations["redmond"] = new GeoPoint() {Latitude = 47.67, Longitude = -122.13};
            _locations["paris"] = new GeoPoint() {Latitude = 48.85, Longitude = 2.34};
        }

        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType != typeof (GeoPoint))
            {
                return false;
            }
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (value == null)
            {
                return false;
            }
            var rawValue = value.RawValue as string;
            if (rawValue == null)
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, "Wrong value type");
                return false;
            }
            GeoPoint result;
            if (GeoPoint.TryParse(rawValue, out result))
            {
                bindingContext.Model = result;
                return true;
            }
            bindingContext.ModelState.AddModelError(bindingContext.ModelName, "Cannot convert to Location");
            return false;
        }
    }

    public class HeaderValueProvider: IValueProvider
    {
        private readonly Dictionary<string, string> _values; 


        public HeaderValueProvider(HttpActionContext actionContext)
        {
            if (actionContext == null)
            {
                throw new ArgumentException("actionContext");
            }
            _values = actionContext.Request.Headers.ToDictionary(
                p=> p.Key,
                p=> p.Value.Aggregate((a,b)=>string.Format("{0},{1}",a,b)));
        }

        public bool ContainsPrefix(string prefix)
        {
            return _values.Keys.Contains(prefix);
        }

        public ValueProviderResult GetValue(string key)
        {
            string value;
            if (_values.TryGetValue(key, out value))
            {
                return new ValueProviderResult(value, value, CultureInfo.InvariantCulture);
            }
            return null;
        }
    }

    public class HeaderValueProviderFactory: ValueProviderFactory
    {
        public override IValueProvider GetValueProvider(HttpActionContext actionContext)
        {
            return new HeaderValueProvider(actionContext);
        }
    }
}