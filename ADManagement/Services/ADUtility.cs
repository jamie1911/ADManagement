using ADManagement.Models;
using System;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Security.Principal;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace ADManagement.Services
{
    public class ADUtility
    {
        readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public List<Person> GetPeople(string inputUserSearch)
        {
            string searchparse = inputUserSearch;
            string[] userstofind = searchparse.Split(';').Select(sValue => sValue.Trim()).ToArray();
            Person uInfo = new Person();
            DirectoryEntry ldapConnection = createDirectoryEntry();
            DirectorySearcher ldap_searcher = new DirectorySearcher(ldapConnection);
            WindowsIdentity winId = (WindowsIdentity)HttpContext.Current.User.Identity;
            WindowsImpersonationContext impersonationContext = null;
            var UserList = new List<Person>();
            try
            {
                foreach (string user in userstofind)
                {
                    if (!string.IsNullOrEmpty(user))
                    {
                        log.Info("" + winId.Name + " is searching for: " + user + "");
                        impersonationContext = WindowsIdentity.Impersonate(winId.Token);
                        string filter = "(&(anr=" + user + "*)(samAccountType=805306368)(!userAccountControl:1.2.840.113556.1.4.803:=2))";
                        //properties to get
                        ldap_searcher.PropertiesToLoad.Clear();
                        ldap_searcher.PropertiesToLoad.Add("sAMAccountName");
                        ldap_searcher.PropertiesToLoad.Add("title");
                        ldap_searcher.PropertiesToLoad.Add("cn");
                        ldap_searcher.PropertiesToLoad.Add("department");
                        ldap_searcher.PropertiesToLoad.Add("distinguishedName");
                        ldap_searcher.PropertiesToLoad.Add("mail");
                        ldap_searcher.PropertiesToLoad.Add("thumbnailPhoto");
                        ldap_searcher.PropertiesToLoad.Add("physicalDeliveryOfficeName");
                        ldap_searcher.Filter = filter;
                        //create search collection
                        SearchResultCollection allemployee_results = ldap_searcher.FindAll();
                        if (allemployee_results.Count == 0)
                        {
                            filter = "(&(sAMAccountName=" + user + "*)(samAccountType=805306368)(!userAccountControl:1.2.840.113556.1.4.803:=2))";
                            if (inputUserSearch.Contains("@"))
                            {
                                string convertToUsername = user.Split('@')[0];
                                filter = "(&(sAMAccountName=" + convertToUsername + "*)(samAccountType=805306368)(!userAccountControl:1.2.840.113556.1.4.803:=2))";
                            }
                            ldap_searcher.Filter = filter;
                            //find users
                            allemployee_results = ldap_searcher.FindAll();
                        }
                        if (allemployee_results.Count > 300)
                        {
                            UserList.Add(new Person
                            {
                                FullName = "Too Many People",
                            });

                            return UserList;
                        }

                        if (allemployee_results.Count > 0)
                        {
                            foreach (SearchResult employeeEntryToGet in allemployee_results)
                            {
                                //get sAMAccountName
                                if (employeeEntryToGet.Properties.Contains("sAMAccountName") && employeeEntryToGet.Properties["sAMAccountName"] != null)
                                {
                                    uInfo.SAMAccountName = employeeEntryToGet.Properties["sAMAccountName"][0].ToString();

                                }
                                else
                                {
                                    uInfo.SAMAccountName = "";
                                }
                                //get Full Name
                                if (employeeEntryToGet.Properties.Contains("cn") && employeeEntryToGet.Properties["cn"] != null)
                                {
                                    uInfo.FullName = employeeEntryToGet.Properties["cn"][0].ToString();
                                }
                                else
                                {
                                    uInfo.FullName = "";
                                }
                                //get Title
                                if (employeeEntryToGet.Properties.Contains("title") && employeeEntryToGet.Properties["title"] != null)
                                {
                                    uInfo.Title = employeeEntryToGet.Properties["title"][0].ToString();
                                }
                                else
                                {
                                    uInfo.Title = "";
                                }
                                //get Departament
                                if (employeeEntryToGet.Properties.Contains("department") && employeeEntryToGet.Properties["department"] != null)
                                {
                                    uInfo.Department = employeeEntryToGet.Properties["department"][0].ToString();
                                }
                                else
                                {
                                    uInfo.Department = "";
                                }
                                //get Email
                                if (employeeEntryToGet.Properties.Contains("mail") && employeeEntryToGet.Properties["mail"] != null)
                                {
                                    uInfo.EmailAddress = employeeEntryToGet.Properties["mail"][0].ToString();
                                }
                                else
                                {
                                    uInfo.EmailAddress = "";
                                }
                                //get Office
                                if (employeeEntryToGet.Properties.Contains("physicalDeliveryOfficeName") && employeeEntryToGet.Properties["physicalDeliveryOfficeName"] != null)
                                {
                                    uInfo.Office = employeeEntryToGet.Properties["physicalDeliveryOfficeName"][0].ToString();
                                }
                                else
                                {
                                    uInfo.Office = "";
                                }
                                //get photo
                                if (employeeEntryToGet.Properties.Contains("thumbnailPhoto") && employeeEntryToGet.Properties["thumbnailPhoto"] != null)
                                {
                                    uInfo.HasPhoto = "Yes";
                                }
                                else
                                {
                                    uInfo.HasPhoto = "No";
                                }

                                //get Distinguished Name
                                if (employeeEntryToGet.Properties.Contains("distinguishedName") && employeeEntryToGet.Properties["distinguishedName"] != null)
                                {
                                    uInfo.DistinguishedName = employeeEntryToGet.Properties["distinguishedName"][0].ToString();
                                }
                                else
                                {
                                    uInfo.DistinguishedName = "";
                                }
                                //add user to list                           
                                UserList.Add(new Person
                                {
                                    SAMAccountName = uInfo.SAMAccountName,
                                    Title = uInfo.Title,
                                    Department = uInfo.Department,
                                    EmailAddress = uInfo.EmailAddress,
                                    Office = uInfo.Office,
                                    DistinguishedName = uInfo.DistinguishedName,
                                    FullName = uInfo.FullName,
                                    HasPhoto = uInfo.HasPhoto
                                });
                            }
                            UserList = UserList.OrderBy(newlist => newlist.SAMAccountName).ToList();
                        }
                        else
                        {
                            //uList.Status.error = true;
                            //uList.Status.message = "Employee Not Found";
                            //log.Info("" + winId.Name + " has encountered an error: " + uList.Status.message + "");
                        }
                    }
                    else
                    {
                        //uList.Status.error = true;
                        //uList.Status.message = "No Employee Entered";
                        //log.Info("" + winId.Name + " has encountered an error: " + uList.Status.message + "");
                    }
                }
            }

            catch (Exception ex)
            {
                //uList.Status.error = true;
                //uList.Status.message = ex.Message;
                log.Info("" + winId.Name + " has encountered an error: " + ex.Message + "");
            }
            finally
            {
                ldap_searcher.Dispose();
                ldapConnection.Dispose();
                winId.Dispose();
                if (impersonationContext != null)
                {
                    impersonationContext.Undo();
                    impersonationContext.Dispose();
                }
            }
            return UserList;
        }

        public Person GetPerson(string selectedUser)
        {
            Person userInfo = new Person();
            DirectoryEntry ldapConnection = createDirectoryEntry();
            DirectorySearcher ldap_searcher = new DirectorySearcher(ldapConnection);
            WindowsIdentity winId = (WindowsIdentity)HttpContext.Current.User.Identity;
            WindowsImpersonationContext impersonationContext = null;
            PrincipalContext principalCtx = createPrincipalCtx();
            UserPrincipal userGroupToFind = UserPrincipal.FindByIdentity(principalCtx, selectedUser);
            try
            {
                impersonationContext = WindowsIdentity.Impersonate(winId.Token);
                ldap_searcher.Filter = "(&(sAMAccountName=" + selectedUser + "*)(samAccountType=805306368))";
                //properties to get
                ldap_searcher.PropertiesToLoad.Clear();
                ldap_searcher.PropertiesToLoad.Add("sAMAccountName");
                ldap_searcher.PropertiesToLoad.Add("title");
                ldap_searcher.PropertiesToLoad.Add("cn");
                ldap_searcher.PropertiesToLoad.Add("department");
                ldap_searcher.PropertiesToLoad.Add("manager");
                ldap_searcher.PropertiesToLoad.Add("telephoneNumber");
                ldap_searcher.PropertiesToLoad.Add("thumbnailPhoto");
                ldap_searcher.PropertiesToLoad.Add("distinguishedName");
                ldap_searcher.PropertiesToLoad.Add("mail");
                ldap_searcher.PropertiesToLoad.Add("givenName");
                ldap_searcher.PropertiesToLoad.Add("sn");
                ldap_searcher.PropertiesToLoad.Add("displayName");
                ldap_searcher.PropertiesToLoad.Add("physicalDeliveryOfficeName");
                ldap_searcher.PropertiesToLoad.Add("DirectReports");
                ldap_searcher.PropertiesToLoad.Add("memberOf");
                //create search collection
                SearchResult l_result = ldap_searcher.FindOne();

                if (l_result != null)
                {
                    using (DirectoryEntry employeeEntryToGet = l_result.GetDirectoryEntry())
                    {
                        //get sAMAccountName
                        if (employeeEntryToGet.Properties.Contains("sAMAccountName") && employeeEntryToGet.Properties["sAMAccountName"] != null)
                        {
                            userInfo.SAMAccountName = employeeEntryToGet.Properties["sAMAccountName"][0].ToString();
                            impersonationContext.Undo();
                            log.Info("" + winId.Name + " is retreiving to information for: " + userInfo.SAMAccountName + "");
                            impersonationContext = WindowsIdentity.Impersonate(winId.Token);
                        }
                        else
                        {
                            userInfo.SAMAccountName = "";
                        }
                        //get Full Name
                        if (employeeEntryToGet.Properties.Contains("cn") && employeeEntryToGet.Properties["cn"] != null)
                        {
                            userInfo.FullName = employeeEntryToGet.Properties["cn"][0].ToString();
                            impersonationContext.Undo();
                            log.Info("" + winId.Name + " is retreiving property cn for: " + userInfo.SAMAccountName + ": " + userInfo.FullName + "");
                            impersonationContext = WindowsIdentity.Impersonate(winId.Token);
                        }
                        else
                        {
                            userInfo.FullName = "";
                        }
                        //get First Name
                        if (employeeEntryToGet.Properties.Contains("givenName") && employeeEntryToGet.Properties["givenName"] != null)
                        {
                            userInfo.FirstName = employeeEntryToGet.Properties["givenName"][0].ToString();
                            impersonationContext.Undo();
                            log.Info("" + winId.Name + " is retreiving property givenName for: " + userInfo.SAMAccountName + ": " + userInfo.FirstName + "");
                            impersonationContext = WindowsIdentity.Impersonate(winId.Token);
                        }
                        else
                        {
                            userInfo.FirstName = "";
                        }
                        //get Last Name
                        if (employeeEntryToGet.Properties.Contains("sn") && employeeEntryToGet.Properties["sn"] != null)
                        {
                            userInfo.LastName = employeeEntryToGet.Properties["sn"][0].ToString();
                            impersonationContext.Undo();
                            log.Info("" + winId.Name + " is retreiving property sn for: " + userInfo.SAMAccountName + ": " + userInfo.LastName + "");
                            impersonationContext = WindowsIdentity.Impersonate(winId.Token);
                        }
                        else
                        {
                            userInfo.LastName = "";
                        }
                        //get displayName
                        if (employeeEntryToGet.Properties.Contains("displayName") && employeeEntryToGet.Properties["displayName"] != null)
                        {
                            userInfo.DisplayName = employeeEntryToGet.Properties["displayName"][0].ToString();
                            impersonationContext.Undo();
                            log.Info("" + winId.Name + " is retreiving property displayName for: " + userInfo.SAMAccountName + ": " + userInfo.DisplayName + "");
                            impersonationContext = WindowsIdentity.Impersonate(winId.Token);
                        }
                        else
                        {
                            userInfo.DisplayName = "";
                        }

                        //get Title
                        if (employeeEntryToGet.Properties.Contains("title") && employeeEntryToGet.Properties["title"] != null)
                        {
                            userInfo.Title = employeeEntryToGet.Properties["title"][0].ToString();
                            impersonationContext.Undo();
                            log.Info("" + winId.Name + " is retreiving property title for: " + userInfo.SAMAccountName + ": " + userInfo.Title + "");
                            impersonationContext = WindowsIdentity.Impersonate(winId.Token);
                        }
                        else
                        {
                            userInfo.Title = "";
                        }
                        //get Phone Number
                        if (employeeEntryToGet.Properties.Contains("telephoneNumber") && employeeEntryToGet.Properties["telephoneNumber"] != null)
                        {
                            userInfo.Telephone = employeeEntryToGet.Properties["telephoneNumber"][0].ToString();
                            impersonationContext.Undo();
                            log.Info("" + winId.Name + " is retreiving property telephoneNumber for: " + userInfo.SAMAccountName + ": " + userInfo.Telephone + "");
                            impersonationContext = WindowsIdentity.Impersonate(winId.Token);
                        }
                        else
                        {
                            userInfo.Telephone = "";
                        }
                        //get Departament
                        if (employeeEntryToGet.Properties.Contains("department") && employeeEntryToGet.Properties["department"] != null)
                        {
                            userInfo.Department = employeeEntryToGet.Properties["department"][0].ToString();
                            impersonationContext.Undo();
                            log.Info("" + winId.Name + " is retreiving property department for: " + userInfo.SAMAccountName + ": " + userInfo.Department + "");
                            impersonationContext = WindowsIdentity.Impersonate(winId.Token);
                        }
                        else
                        {
                            userInfo.Department = "";
                        }
                        //get Email
                        if (employeeEntryToGet.Properties.Contains("mail") && employeeEntryToGet.Properties["mail"] != null)
                        {
                            userInfo.EmailAddress = employeeEntryToGet.Properties["mail"][0].ToString();
                            impersonationContext.Undo();
                            log.Info("" + winId.Name + " is retreiving property mail for: " + userInfo.SAMAccountName + ": " + userInfo.EmailAddress + "");
                            impersonationContext = WindowsIdentity.Impersonate(winId.Token);
                        }
                        else
                        {
                            userInfo.EmailAddress = "";
                        }
                        //get Office
                        if (employeeEntryToGet.Properties.Contains("physicalDeliveryOfficeName") && employeeEntryToGet.Properties["physicalDeliveryOfficeName"] != null)
                        {
                            userInfo.Office = employeeEntryToGet.Properties["physicalDeliveryOfficeName"][0].ToString();
                            impersonationContext.Undo();
                            log.Info("" + winId.Name + " is retreiving property physicalDeliveryOfficeName for: " + userInfo.SAMAccountName + ": " + userInfo.Office + "");
                            impersonationContext = WindowsIdentity.Impersonate(winId.Token);
                        }
                        else
                        {
                            userInfo.Office = "";
                        }
                        //get Direct Report Info
                        userInfo.DirectsCount = (employeeEntryToGet.Properties["directreports"] != null) ? employeeEntryToGet.Properties["directreports"].Count : 0;
                        if (userInfo.DirectsCount > 0)
                        {
                            userInfo.Directs = new List<Person> { };
                            for (int i = 0; i < userInfo.DirectsCount; i++)
                            {
                                var directreport = employeeEntryToGet.Properties["directreports"][i].ToString();
                                directreport = directreport.TrimEnd(',');
                                directreport = directreport.Replace("\\, ", ", ");
                                directreport = directreport.Split(',')[0];

                                ldap_searcher.Filter = "(&(objectCategory=person)(objectClass=user)(" + directreport + "*))";
                                ldap_searcher.PropertiesToLoad.Clear();
                                ldap_searcher.PropertiesToLoad.Add("sAMAccountName");
                                ldap_searcher.PropertiesToLoad.Add("cn");
                                SearchResult l_result_direct = ldap_searcher.FindOne();
                                var direct_FullName = "";
                                var direct_SamAccountName = "";
                                using (DirectoryEntry directEntryToGet = l_result_direct.GetDirectoryEntry())
                                {
                                    if (directEntryToGet.Properties["cn"] != null)
                                    {
                                        direct_FullName = directEntryToGet.Properties["cn"][0].ToString();
                                    }
                                    if (directEntryToGet.Properties["sAMAccountName"] != null)
                                    {
                                        direct_SamAccountName = directEntryToGet.Properties["sAMAccountName"][0].ToString();
                                    }
                                }
                                userInfo.Directs.Add(new Person
                                {

                                    SAMAccountName = direct_SamAccountName,
                                    FullName = direct_FullName

                                });
                            }
                            impersonationContext.Undo();
                            log.Info("" + winId.Name + " is retreiving property directreports for: " + userInfo.SAMAccountName + "");
                            impersonationContext = WindowsIdentity.Impersonate(winId.Token);
                        }
                        else
                        {
                            userInfo.DirectsCount = 0;
                            userInfo.Directs = null;
                        }
                        //get Distinguished Name
                        if (employeeEntryToGet.Properties.Contains("distinguishedName") && employeeEntryToGet.Properties["distinguishedName"] != null)
                        {
                            userInfo.DistinguishedName = employeeEntryToGet.Properties["distinguishedName"][0].ToString();
                            impersonationContext.Undo();
                            log.Info("" + winId.Name + " is retreiving property distinguishedName for: " + userInfo.SAMAccountName + ": " + userInfo.DistinguishedName + "");
                            impersonationContext = WindowsIdentity.Impersonate(winId.Token);
                        }
                        else
                        {
                            userInfo.DistinguishedName = "";
                        }
                        //get manager and cleanup manager DN name
                        if (employeeEntryToGet.Properties.Contains("manager") && employeeEntryToGet.Properties["manager"] != null)
                        {
                            userInfo.ManagerDistinguishedName = employeeEntryToGet.Properties["manager"][0].ToString();

                            var managertofind = employeeEntryToGet.Properties["manager"][0].ToString();
                            managertofind = managertofind.TrimEnd(',');
                            managertofind = managertofind.Replace("\\, ", ", ");
                            managertofind = managertofind.Split(',')[0];

                            ldap_searcher.Filter = "(&(objectCategory=person)(objectClass=user)(" + managertofind + "*))";
                            ldap_searcher.PropertiesToLoad.Clear();
                            ldap_searcher.PropertiesToLoad.Add("sAMAccountName");
                            SearchResult l_result_manager = ldap_searcher.FindOne();

                            using (DirectoryEntry managerEntryToGet = l_result_manager.GetDirectoryEntry())
                            {
                                if (managerEntryToGet.Properties["sAMAccountName"] != null)
                                {
                                    userInfo.ManagerSamAccountName = managerEntryToGet.Properties["sAMAccountName"][0].ToString();
                                }
                                else
                                {

                                }
                                if (managerEntryToGet.Properties["cn"] != null)
                                {
                                    userInfo.ManagerName = managerEntryToGet.Properties["cn"][0].ToString();
                                }
                                else
                                {
                                    userInfo.ManagerName = null;
                                }
                            }

                            impersonationContext.Undo();
                            log.Info("" + winId.Name + " is retreiving property manager for: " + userInfo.SAMAccountName + ": " + userInfo.ManagerDistinguishedName + "");
                            impersonationContext = WindowsIdentity.Impersonate(winId.Token);
                        }
                        else
                        {
                            userInfo.ManagerName = "";
                            userInfo.ManagerDistinguishedName = "";
                            userInfo.ManagerSamAccountName = "";
                        }
                        //get thumbnail photo
                        if (employeeEntryToGet.Properties.Contains("thumbnailPhoto") && employeeEntryToGet.Properties["thumbnailPhoto"] != null)
                        {
                            byte[] bb = (byte[])employeeEntryToGet.Properties["thumbnailPhoto"][0];
                            {
                                string base64String = Convert.ToBase64String(bb, 0, bb.Length);
                                userInfo.ThumbnailPhoto = "data:image/jpeg;base64," + base64String;
                            }
                            impersonationContext.Undo();
                            log.Info("" + winId.Name + " is retreiving property thumbnailPhoto for: " + userInfo.SAMAccountName + "");
                            impersonationContext = WindowsIdentity.Impersonate(winId.Token);
                        }
                        else
                        {
                            // display default image if no thumbnail
                            userInfo.ThumbnailPhoto = "/Images/default-user-profile.png";
                            impersonationContext.Undo();
                            log.Info("" + winId.Name + " is retreiving property thumbnailPhoto for: " + userInfo.SAMAccountName + "");
                            impersonationContext = WindowsIdentity.Impersonate(winId.Token);
                        }

                        if (userGroupToFind != null)
                        {
                            userInfo.MemberOf = userGroupToFind.GetGroups().Select(r => r.Name).ToList();
                            impersonationContext.Undo();
                            log.Info("" + winId.Name + " is retreiving property memberOf for: " + userInfo.SAMAccountName + "");
                            impersonationContext = WindowsIdentity.Impersonate(winId.Token);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                impersonationContext.Undo();
                log.Info("" + winId.Name + " has encountered an error: " + ex.Message + "");
            }
            finally
            {
                ldap_searcher.Dispose();
                ldapConnection.Dispose();
                winId.Dispose();
                principalCtx.Dispose();
                userGroupToFind.Dispose();
                if (impersonationContext != null)
                {
                    impersonationContext.Undo();
                    impersonationContext.Dispose();
                }
            }
            return userInfo;
        }

        public StatusInfo SetUser(Person uInfo)
        {
            StatusInfo status = new StatusInfo();
            status.StatusDetail = new List<StatusDetails> { };
            {
                DirectoryEntry ldapConnection = createDirectoryEntry();
                DirectorySearcher ldap_searcher = new DirectorySearcher(ldapConnection);
                WindowsImpersonationContext impersonationContext = null;
                WindowsIdentity winId = (WindowsIdentity)HttpContext.Current.User.Identity;
                impersonationContext = WindowsIdentity.Impersonate(winId.Token);
                try
                {
                    ldap_searcher.PropertiesToLoad.Add("title");
                    ldap_searcher.PropertiesToLoad.Add("cn");
                    ldap_searcher.PropertiesToLoad.Add("department");
                    ldap_searcher.PropertiesToLoad.Add("manager");
                    ldap_searcher.PropertiesToLoad.Add("telephoneNumber");
                    ldap_searcher.PropertiesToLoad.Add("thumbnailPhoto");
                    ldap_searcher.PropertiesToLoad.Add("physicalDeliveryOfficeName");
                    ldap_searcher.Filter = "(&(samAccountType=805306368)(sAMAccountName=" + uInfo.SAMAccountName + "))";

                    SearchResult l_result = ldap_searcher.FindOne();

                    impersonationContext.Undo();
                    log.Info("" + winId.Name + " is trying to set information for: " + uInfo.SAMAccountName + "");
                    impersonationContext = WindowsIdentity.Impersonate(winId.Token);
                    using (DirectoryEntry employeeToModify = l_result.GetDirectoryEntry())
                    {
                        //save Title
                        if (!string.IsNullOrWhiteSpace(uInfo.Title) && employeeToModify.Properties["title"].Value as string != uInfo.Title)
                        {
                            employeeToModify.Properties["title"].Clear();
                            employeeToModify.Properties["title"].Add(uInfo.Title);
                            impersonationContext.Undo();
                            log.Info("" + winId.Name + " is trying to set property title for: " + uInfo.SAMAccountName + ": " + uInfo.Title + "");
                            impersonationContext = WindowsIdentity.Impersonate(winId.Token);
                        }
                        //save Phone Number
                        if (!string.IsNullOrWhiteSpace(uInfo.Telephone) && employeeToModify.Properties["telephoneNumber"].Value as string != uInfo.Telephone)
                        {
                            employeeToModify.Properties["telephoneNumber"].Clear();
                            employeeToModify.Properties["telephoneNumber"].Add(uInfo.Telephone);
                            impersonationContext.Undo();
                            log.Info("" + winId.Name + " is trying to set property telephoneNumber for: " + uInfo.SAMAccountName + ": " + uInfo.Telephone + "");
                            impersonationContext = WindowsIdentity.Impersonate(winId.Token);
                        }
                        //save Departament
                        if (!string.IsNullOrWhiteSpace(uInfo.Department) && employeeToModify.Properties["department"].Value as string != uInfo.Department)
                        {
                            employeeToModify.Properties["department"].Clear();
                            employeeToModify.Properties["department"].Add(uInfo.Department);
                            impersonationContext.Undo();
                            log.Info("" + winId.Name + " is trying to set property department for: " + uInfo.SAMAccountName + ": " + uInfo.Department + "");
                            impersonationContext = WindowsIdentity.Impersonate(winId.Token);
                        }
                        //get Office Location
                        if (!string.IsNullOrWhiteSpace(uInfo.Office) && employeeToModify.Properties["physicalDeliveryOfficeName"].Value as string != uInfo.Office)
                        {
                            employeeToModify.Properties["physicalDeliveryOfficeName"].Clear();
                            employeeToModify.Properties["physicalDeliveryOfficeName"].Add(uInfo.Office);
                            impersonationContext.Undo();
                            log.Info("" + winId.Name + " is trying to set property physicalDeliveryOfficeName for: " + uInfo.SAMAccountName + ": " + uInfo.Office + "");
                            impersonationContext = WindowsIdentity.Impersonate(winId.Token);
                        }
                        //save new direct report
                        if (uInfo.NewDirects.Count > 0)
                        {

                            foreach (string newdirect in uInfo.NewDirects)
                            {
                                try
                                {
                                    ldap_searcher.Filter = "(&(samAccountType=805306368)(sAMAccountName=" + newdirect + "))";
                                    SearchResult newdirect_result = ldap_searcher.FindOne();
                                    using (DirectoryEntry newdirectToModify = newdirect_result.GetDirectoryEntry())
                                    {
                                        newdirectToModify.Properties["manager"].Clear();
                                        newdirectToModify.Properties["manager"].Add(uInfo.DistinguishedName);
                                        impersonationContext.Undo();
                                        log.Info("" + winId.Name + " is trying to set property manager for: " + newdirect + ": " + uInfo.DistinguishedName + "");
                                        impersonationContext = WindowsIdentity.Impersonate(winId.Token);
                                        newdirectToModify.CommitChanges();
                                        //get names
                                        string newdirectcn = newdirectToModify.Properties["cn"].Value.ToString();
                                        string newmanagername = newmanagername = uInfo.DistinguishedName.Replace("CN=", "");
                                        newmanagername = newmanagername.TrimEnd(',');
                                        newmanagername = newmanagername.Replace("\\, ", ", ");
                                        newmanagername = newmanagername.Split(',')[0];
                                        //generate status alerts
                                        status.StatusDetail.Add(new StatusDetails
                                        {
                                            StatusDescType = "success",
                                            StatusDesc = "Successfully changed " + newdirectcn + " manager to: " + newmanagername + "",
                                            StatusReal = ""
                                        });
                                    }
                                }
                                catch (Exception ex)
                                {
                                    //generate status alerts
                                    status.StatusDetail.Add(new StatusDetails
                                    {
                                        StatusDescType = "warning",
                                        StatusDesc = "Errors changing " + newdirect + " manager to: " + uInfo.DistinguishedName + "",
                                        StatusReal = ex.Message
                                    });
                                    impersonationContext.Undo();
                                    log.Info("" + winId.Name + " had error trying to set property manager for: " + newdirect + ": " + uInfo.DistinguishedName + " - " + ex + "");
                                    impersonationContext = WindowsIdentity.Impersonate(winId.Token);
                                }
                            }
                        }
                        //save new manager
                        if (!string.IsNullOrWhiteSpace(uInfo.ManagerDistinguishedName) && employeeToModify.Properties["manager"].Value as string != uInfo.ManagerDistinguishedName)
                        {
                            employeeToModify.Properties["manager"].Clear();
                            employeeToModify.Properties["manager"].Add(uInfo.ManagerDistinguishedName);
                            impersonationContext.Undo();
                            log.Info("" + winId.Name + " is trying to set property manager for: " + uInfo.SAMAccountName + ": " + uInfo.ManagerDistinguishedName + "");
                            impersonationContext = WindowsIdentity.Impersonate(winId.Token);
                        }
                        //save thumbnail photo
                        if (!string.IsNullOrWhiteSpace(uInfo.ThumbnailPhoto) && employeeToModify.Properties["thumbnailPhoto"].Value as string != uInfo.ThumbnailPhoto || (uInfo.DeleteThumbnailPhoto))
                        {
                            if (!uInfo.DeleteThumbnailPhoto && !uInfo.ThumbnailPhoto.Contains("default-user-profile.png"))
                            {
                                var jsonbaseimage = uInfo.ThumbnailPhoto.Remove(0, 23);
                                var newthumbnailbinary = Convert.FromBase64String(jsonbaseimage);

                                employeeToModify.Properties["thumbnailPhoto"].Clear();
                                employeeToModify.Properties["thumbnailPhoto"].Insert(0, newthumbnailbinary);
                                impersonationContext.Undo();
                                log.Info("" + winId.Name + " is trying to set property thumbnailPhoto for: " + uInfo.SAMAccountName + "");
                                impersonationContext = WindowsIdentity.Impersonate(winId.Token);
                            }
                            if (uInfo.DeleteThumbnailPhoto)
                            {
                                employeeToModify.Properties["thumbnailPhoto"].Clear();
                                impersonationContext.Undo();
                                log.Info("" + winId.Name + " is trying to clear property thumbnailPhoto for: " + uInfo.SAMAccountName + "");
                                impersonationContext = WindowsIdentity.Impersonate(winId.Token);
                            }
                        }
                        //save all info
                        employeeToModify.CommitChanges();
                        //email new full name
                        //    if (!string.IsNullOrWhiteSpace(uInfo.newcn))
                        //    {
                        //        SendEmailNameChange(uInfo.cn, uInfo.newcn, uInfo.sAMAccountName);
                        //        log.Info("" + winId.Name + " is requesting " + uInfo.sAMAccountName + "'s name change to: " + uInfo.newcn + "");
                        //    }
                        status.Type = "success";
                        status.Message = "" + uInfo.SAMAccountName + "'s Information Updated";
                        impersonationContext.Undo();
                        log.Info("" + winId.Name + " has successfully updated information for: " + uInfo.SAMAccountName + "");
                        impersonationContext = WindowsIdentity.Impersonate(winId.Token);
                    }
                }

                catch (Exception ex)
                {
                    status.Type = "error";
                    status.Message = ex.Message;
                    impersonationContext.Undo();
                    log.Info("" + winId.Name + " has encountered an error: " + ex.Message + "");

                }
                finally
                {
                    ldapConnection.Dispose();
                    ldap_searcher.Dispose();
                    winId.Dispose();
                    if (impersonationContext != null)
                    {
                        impersonationContext.Undo();
                        impersonationContext.Dispose();
                    }
                }
                return status;
            }
        }

        static DirectoryEntry createDirectoryEntry()
        {
            //get AD INFO
            string ADSERVERFQDN = System.Configuration.ConfigurationManager.AppSettings["ADSERVERFQDN"].ToString();
            string ADLOOKUPPATH = System.Configuration.ConfigurationManager.AppSettings["ADLOOKUPPATH"].ToString();

            // create and return new LDAP connection with desired settings  
            DirectoryEntry ldapConnection = new DirectoryEntry("LDAP://" + ADSERVERFQDN + "/" + ADLOOKUPPATH + "");
            ldapConnection.AuthenticationType = AuthenticationTypes.Secure;

            return ldapConnection;


        }

        static PrincipalContext createPrincipalCtx()
        {
            //get AD INFO
            string ADSERVERFQDN = System.Configuration.ConfigurationManager.AppSettings["ADSERVERFQDN"].ToString();
            string ADLOOKUPPATH = System.Configuration.ConfigurationManager.AppSettings["ADLOOKUPPATH"].ToString();

            // create and return new LDAP connection with desired settings  
            PrincipalContext principalCtx = new PrincipalContext(ContextType.Domain, ADSERVERFQDN, ADLOOKUPPATH, ContextOptions.Negotiate);

            return principalCtx;
        }
    }
}