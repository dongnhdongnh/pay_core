using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;
using Vakapay.UserBusiness;
using Vakapay.WalletBusiness;

namespace Vakaxa.VakaxaIdAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [EnableCors]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        public IConfiguration Configuration { get; }
        private IHostingEnvironment _hostingEnvironment;

        public UserController(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            Configuration = configuration;
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

                var jsonUser = User.Claims.Where(c => c.Type == "userInfo").Select(c => c.Value).SingleOrDefault();
                var userModel = Vakapay.Models.Entities.User.FromJson(jsonUser);

                var repositoryConfig = new RepositoryConfiguration
                {
                    ConnectionString = Configuration.GetConnectionString("DefaultConnection")
                };
                var persistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
                var userBusiness = new UserBusiness(persistenceFactory);

                var userCheck = userBusiness.getUserInfo(new Dictionary<string, string>
                {
                    {"Email", userModel.Email}
                });

                if (userCheck == null)
                    return ReturnObject.ToJson(new ReturnObject
                    {
                        Status = Status.StatusError,
                        Data = "Can't User"
                    });


                const string folderName = "wwwroot/upload/avatar";
                var link = "/upload/avatar/";
                var webRootPath = Directory.GetCurrentDirectory();
                var newPath = Path.Combine(webRootPath, folderName);
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }

                if (file.Length > 0)
                {
                    char[] myChar = {'"'};
                    var fileName = CommonHelper.GetUnixTimestamp() + ContentDispositionHeaderValue
                                       .Parse(file.ContentDisposition).FileName.ToString()
                                       .Trim(myChar);

                    fileName = fileName.Replace(" ", "-");
                    
                    var fullPath = Path.Combine(newPath, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    var oldAvatar = userCheck.Avatar;

                    link = link + fileName;

                    userCheck.Avatar = fileName;
                    var updateUser = userBusiness.UpdateProfile(userCheck);

                    if (!string.IsNullOrEmpty(oldAvatar))
                    {
                        var oldFullPath = Path.Combine(newPath, oldAvatar);

                        if (System.IO.File.Exists(oldFullPath))
                        {
                            System.IO.File.Delete(oldFullPath);
                        }
                    }

                    if (updateUser.Status == Status.StatusSuccess)
                        return ReturnObject.ToJson(new ReturnObject
                        {
                            Status = Status.StatusSuccess,
                            Message = "Upload avatar success ",
                            Data = link
                        });
                }

                var errorData = new ReturnObject
                {
                    Status = Status.StatusError,
                    Data = "Can't image"
                };


                return ReturnObject.ToJson(errorData);
            }
            catch (Exception ex)
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
                var userModel = Vakapay.Models.Entities.User.FromJson(jsonUser);
                Console.WriteLine(userModel.Email);
                var repositoryConfig = new RepositoryConfiguration
                {
                    ConnectionString = Configuration.GetConnectionString("DefaultConnection")
                };

                var persistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
                var userBusiness = new UserBusiness(persistenceFactory);
                var walletBusiness = new WalletBusiness(persistenceFactory);
                var resultData = userBusiness.Login(walletBusiness, userModel);
                return ReturnObject.ToJson(resultData);
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
        public string Post([FromBody] string value)
        {
            Console.WriteLine("aa");
            return value;
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