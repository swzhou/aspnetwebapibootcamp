using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Metadata;

namespace AspnetWebapiBootcamp.Controllers
{
    public class ETag
    {
        public string Tag { get; set; }
    }

    public enum ETagMatch
    {
        IfMatch,
        IfNoneMatch
    }

    public class ETagParameterBinding : HttpParameterBinding
    {
        ETagMatch _match;

        public ETagParameterBinding(HttpParameterDescriptor parameter, ETagMatch match)
            : base(parameter)
        {
            _match = match;
        }

        public override Task ExecuteBindingAsync(ModelMetadataProvider metadataProvider,
            HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            EntityTagHeaderValue etagHeader = null;
            switch (_match)
            {
                case ETagMatch.IfNoneMatch:
                    etagHeader = actionContext.Request.Headers.IfNoneMatch.FirstOrDefault();
                    break;

                case ETagMatch.IfMatch:
                    etagHeader = actionContext.Request.Headers.IfMatch.FirstOrDefault();
                    break;
            }

            ETag etag = null;
            if (etagHeader != null)
            {
                etag = new ETag { Tag = etagHeader.Tag };
            }
            actionContext.ActionArguments[Descriptor.ParameterName] = etag;

            var tsc = new TaskCompletionSource<object>();
            tsc.SetResult(null);
            return tsc.Task;
        }
    }

    public abstract class ETagMatchAttribute : ParameterBindingAttribute
    {
        private ETagMatch _match;

        public ETagMatchAttribute(ETagMatch match)
        {
            _match = match;
        }

        public override HttpParameterBinding GetBinding(HttpParameterDescriptor parameter)
        {
            if (parameter.ParameterType == typeof(ETag))
            {
                return new ETagParameterBinding(parameter, _match);
            }
            return parameter.BindAsError("Wrong parameter type");
        }
    }

    public class IfMatchAttribute : ETagMatchAttribute
    {
        public IfMatchAttribute()
            : base(ETagMatch.IfMatch)
        {
        }
    }

    public class IfNoneMatchAttribute : ETagMatchAttribute
    {
        public IfNoneMatchAttribute()
            : base(ETagMatch.IfNoneMatch)
        {
        }
    }

    public class EtagController : ApiController
    {
        public HttpResponseMessage Get([IfNoneMatch] ETag etag)
        {
            return new HttpResponseMessage
            {
                Content = new StringContent(etag.Tag)
            };
        }
    }
}