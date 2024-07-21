using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskManagerApi.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class UserRequestModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class RegisterUserModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string UserEmail { get; set; }
    }
}