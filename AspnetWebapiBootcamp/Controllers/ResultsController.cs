using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace AspnetWebapiBootcamp.Controllers
{
    public class ResultsController: ApiController
    {
        //return void
        public void Delete()
        {
        }

        //return HttpResponseMessage
        public HttpResponseMessage Get()
        {
            var response = Request.CreateResponse(HttpStatusCode.OK, "value");
            response.Content=new StringContent("hello", Encoding.UTF8);
            return response;
        }

        //return IHttpActionResult
        public IHttpActionResult Put(int id)
        {
            if (id <= 5) return new TextResult("hello", Request);
            return NotFound();
        }


        //return custom type
        public Product Post()
        {
            return new Product
            {
                Name = "Pro ASP.NET Web API",
                Price = 12.25
            };
        }
    }

    public class Product
    {
        public string Name { get; set; }
        public double Price { get; set; }
    }

    public class TextResult: IHttpActionResult
    {
        private readonly string _value;
        private readonly HttpRequestMessage _request;

        public TextResult(string value, HttpRequestMessage request)
        {
            _value = value;
            _request = request;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage
            {
                Content = new StringContent(_value),
                RequestMessage = _request
            };
            return Task.FromResult(response);
        }
    }
}