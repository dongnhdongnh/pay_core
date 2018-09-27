using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vakapay.Models.Domains;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;
using Vakapay.UserBusiness;

namespace Vakaxa.VakaxaIdAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] {"value1", "value2"};
        }
        
        [HttpGet("checkUserLogin")]
        public string CheckUserLogin()
        {
            var jsonUser = new JsonResult(from c in User.Claims where c.Type == "userInfo" select new {c.Value});
            var userModel = Vakapay.Models.Entities.User.FromJson(jsonUser.ToString());
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString =
                    "server=127.0.0.1;userid=root;password=Chelsea1992;database=vakapay;port=3306;Connection Timeout=120;SslMode=none"
            };

            var persistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
            var userBusiness = new UserBusiness(persistenceFactory);
            var resultData = userBusiness.Login(userModel.Email, userModel.Phone, userModel.Fullname);
            return ReturnObject.ToJson(resultData);
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