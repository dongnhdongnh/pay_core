using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Vakapay.Models.Domains;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;
using Vakapay.UserBusiness;
using Vakapay.WalletBusiness;
using Vakaxa.VakaxaIdAPI.Model;

namespace Vakaxa.VakaxaIdAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [EnableCors]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private IHostingEnvironment _hostingEnvironment;

        public UserController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet("getUserInfo")]
        public IActionResult GetUserInfo()
        {
            return new JsonResult(from c in User.Claims where c.Type == "userInfo" select new {c.Value});
        }

        [HttpPost("upload-avatar"), DisableRequestSizeLimit]
        public string UploadFile()
        {
            try
            {
                var file = Request.Form.Files[0];

                string folderName = "wwwroot/avatar";
                string webRootPath = Directory.GetCurrentDirectory();

                string newPath = Path.Combine(webRootPath, folderName);

                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }

                Console.WriteLine(file.FileName);
                if (file.Length > 0)
                {
                    char[] MyChar = {'"', ' '};
                    string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.ToString()
                        .Trim(MyChar);
                    Console.WriteLine(fileName);
                    string fullPath = Path.Combine(newPath, fileName);
                    Console.WriteLine(fullPath);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    var successData = new ReturnObject
                    {
                        Status = Status.StatusSuccess,
                        Data = fullPath + "--" +  file.FileName + "--" + newPath + "--" + fileName
                    };
                    return ReturnObject.ToJson(successData);
                }

                var errorData = new ReturnObject
                {
                    Status = Status.StatusError,
                    Data = "Can't image"
                };
                return ReturnObject.ToJson(errorData);
            }
            catch (System.Exception ex)
            {
                var errorData = new ReturnObject
                {
                    Status = Status.StatusError,
                    Message = ex.Message
                };
                return ReturnObject.ToJson(errorData);
            }
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
                var walletBusiness = new WalletBusiness(persistenceFactory);
                //var resultData = userBusiness.Login(walletBusiness, userModel);
                return null;
            }
            catch (Exception e)
            {
                var errorData = new ReturnObject
                {
                    Status = Status.StatusError,
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