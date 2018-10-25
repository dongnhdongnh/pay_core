using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Vakapay.ApiAccess.ActionFilter;
using Vakapay.Models.Entities;

namespace Vakapay.ApiAccess.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [BaseActionFilter]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            var userModel = (User) RouteData.Values["UserModel"];
            var apiKey = (ApiKey) RouteData.Values["ApiKeyModel"];
            Console.WriteLine("ValuesController ==>> userModel: " + userModel.Id + " ==>> ApiKeyModel: " + apiKey.Id);

            return new string[] {"value1", "value2"};
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}