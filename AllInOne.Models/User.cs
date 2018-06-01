using AllInOne.Models.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllInOne.Models
{
    public class User: IUser
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
            set {
                
            }
        }
        public string Username { get; set; }
        public string Email { get; set; }
        public string HomePhone { get; set; }
        public string MobilePhone { get; set; }
        public string Password { get; set; }
        public string PasswordHash
        {
            get
            {
                return string.IsNullOrEmpty(Password) ? string.Empty : AllInOne.Core.Identity.PasswordHashHelper.Current.HashPassword(Password);
            }
            set
            {

            }
        }
    }
}
