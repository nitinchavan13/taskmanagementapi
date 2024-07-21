using Google.Protobuf.WellKnownTypes;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Web.Http;
using System.Web.Http.Cors;
using TaskManagerApi.db;
using TaskManagerApi.Models;

namespace TaskManagerApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/v1/internal")]
    public class AuthController : ApiController
    {
        private DBHelper _dbHelper;
        public AuthController()
        {
            _dbHelper = new DBHelper();
            _dbHelper.CreateDBObjects(ConfigurationManager.AppSettings["mysqlConnectionString"], DBHelper.DbProviders.MySql);
        }

        [Route("auth/login")]
        [HttpPost]
        public IHttpActionResult Login(UserRequestModel requestModel)
        {
            UserModel model = new UserModel();
            _dbHelper.AddParameter("p_userEmail", requestModel.UserName);
            _dbHelper.AddParameter("p_password", requestModel.Password);
            var reader = _dbHelper.ExecuteReader("sp_login", System.Data.CommandType.StoredProcedure, System.Data.ConnectionState.Open);
            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        model.Id = Convert.ToInt32(reader["id"]);
                        model.Name = Convert.ToString(reader["name"]);
                    }
                    return Ok(GetToken(model));
                }
                else
                {
                    throw new Exception("Username or password is not matched");
                }
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("auth/registerUser")]
        [HttpPost]
        public IHttpActionResult RegisterUser(RegisterUserModel requestModel)
        {
            _dbHelper.AddParameter("p_userEmail", requestModel.UserEmail);
            _dbHelper.AddParameter("p_password", requestModel.Password);
            _dbHelper.AddParameter("p_userName", requestModel.UserName);
            try
            {
                _dbHelper.ExecuteScaler("sp_register_user", System.Data.CommandType.StoredProcedure, System.Data.ConnectionState.Open);
                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private JWTToken GetToken(UserModel model)
        {
            var key = ConfigurationManager.AppSettings["JwtKey"];

            var issuer = ConfigurationManager.AppSettings["JwtIssuer"];

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            //Create a List of Claims, Keep claims name short    
            var permClaims = new List<Claim>();
            permClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            permClaims.Add(new Claim("userid", model.Id.ToString()));
            permClaims.Add(new Claim("username", model.Name));

            //Create Security Token object by giving required parameters    
            var token = new JwtSecurityToken(issuer, //Issure    
                            issuer,  //Audience    
                            permClaims,
                            expires: DateTime.Now.AddDays(1),
                            signingCredentials: credentials);
            var jwt_token = new JwtSecurityTokenHandler().WriteToken(token);
            return new JWTToken(){ Token = jwt_token };
        }
    }
}
