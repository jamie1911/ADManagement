using System;
using System.Collections.Generic;
using System.Web.Http;
using ADManagement.Services;
using ADManagement.Models;


namespace ADManagement.Controllers
{
    /// <summary>
    /// API controller to manage users
    /// </summary>
    public class UsersController : ApiController
    {
        //public Person GetUser(string user)
        //{
        //    ADUtility utility = new ADUtility();

        //    var userInfo = utility.GetUser(user);

        //    return userInfo;
        //}

        public List<Person> GetPeople(string people)
        {
            ADUtility utility = new ADUtility();

            var userList = utility.GetPeople(people);

            return userList;
        }


        public Person GetPerson(string person)
        {
            ADUtility utility = new ADUtility();

            var userInfo = utility.GetPerson(person);

            return userInfo;
        }


        public StatusInfo SetUserInfo(Person userInfo)
        {
            ADUtility utility = new ADUtility();

            var status = utility.SetUser(userInfo);

            return status;
        }
    }
}
