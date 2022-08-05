using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using WalkthroughApp_API.Data;
using WalkthroughApp_API.Entities;

namespace WalkthroughApp_API.Tests.Controllers
{
    public class UsersControllerTests
    {
        //[Test]
        //public async Task Return_list_of_users()
        //{
        //    var DataContext = BuildContext("Users Controller Tests");
        //}

        //private DataContext BuildContext(string key)
        //{
        //    var builder = new DbContextOptionsBuilder<DataContext>().UseInMemoryDatabase(key);
        //    var context = new DataContext(builder.Options);
        //    context.Users.AddRange(new[]
        //    {
        //        new User(){ Id = 1, UserName = "any user name 1", JobTitle = "any job title", PasswordHash = new byte[]{1}, PasswordSalt = new byte[] { 2 }}
        //    });
        //    context.SaveChanges();

        //    return context;
        //}
    }
}
