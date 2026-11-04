using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiPractice.Controllers
{
    public class ValuesController : ApiController
    {
        private readonly List<string> _values = new List<string> { "Nishat", "AVijit", "Komal", "Anubhaw" };


        // GET api/values
        public IEnumerable<string> Get()
        {

            return _values;
        }

        // GET api/values/5
        public string Get(int i)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
