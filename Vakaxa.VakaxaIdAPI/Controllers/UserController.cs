using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vakapay.Models.Domains;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;
using Vakapay.UserBusiness;
using Vakaxa.VakaxaIdAPI.Model;

namespace Vakaxa.VakaxaIdAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        [HttpGet("getUserInfo")]
        public IActionResult GetUserInfo()
        {
            return new JsonResult(from c in User.Claims where c.Type == "userInfo" select new {c.Value});
        }

        [HttpGet("checkUserLogin")]
        public string CheckUserLogin()
        {
            try
            {
                var jsonUser = User.Claims.Where(c => c.Type == "userInfo").Select(c => c.Value).SingleOrDefault();
                Console.WriteLine(jsonUser);
                var userModel = UserModel.FromJson(jsonUser);
                Console.WriteLine(userModel.Email);
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
            catch (Exception e)
            {
                var errorData = new ReturnObject
                {
                    Status = Status.StatusSuccess,
                    Message = e.Message
                };
                return ReturnObject.ToJson(errorData);
            }
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