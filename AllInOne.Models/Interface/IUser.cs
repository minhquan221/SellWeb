using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllInOne.Models.Interface
{
    public interface IUser
    {
        Guid Id { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string FullName { get; set; }
        string Username { get; set; }
        string Email { get; set; }
        string HomePhone { get; set; }
        string MobilePhone { get; set; }
        string Password { get; set; }
        string PasswordHash { get; set; }
    }
}
