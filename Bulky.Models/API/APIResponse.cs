using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models.API
{
    public class APIResponse
    {
        public APIResponse()
        {
            ErrorMessages = new List<string>();
        }
        public IEnumerable<object> entries { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string token { get; set; }
        public int id { get; set; }
        public bool isSuccess { get; set; } = true;
        public List<string> ErrorMessages { get; set; }
        public object Result { get; set; }
    }
}
