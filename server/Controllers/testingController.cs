using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HospiTec_Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class testingController : ControllerBase
    {
        // GET: api/<testingController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<testingController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<testingController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<testingController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<testingController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
