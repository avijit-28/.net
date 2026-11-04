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
        private static readonly List<string> _values = new List<string> { "N", "AVijit", "K", "An" };

        // GET api/values
        public IEnumerable<string> Get()
        {
            return _values;
        }

        // GET api/values/1
        public string Get(int id)
        {
            if (id < 0 || id >= _values.Count)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return _values[id];
        }

        // POST api/values
        public void Post([FromBody] string value)
        {
            _values.Add(value);
        }

        // PUT api/values/1
        public IHttpActionResult Put(int id, [FromBody] string value)
        {
            if (id < 0 || id >= _values.Count)
                return NotFound();

            _values[id] = value;

            return Ok("Value Updated Successfully");
        }

        // DELETE api/values/1
        public IHttpActionResult Delete(int id)
        {
            if (id < 0 || id >= _values.Count)
                return NotFound();

            _values.RemoveAt(id);

            return Ok("Value Deleted Successfully");
        }
    }
}