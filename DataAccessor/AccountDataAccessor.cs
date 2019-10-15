using CF;
using CF.Models;
using Models.Authentication;
using Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Utility;
using Utility.Models;

namespace DataAccessor
{
    public class AccountDataAccessor
    {
        public static RequestResult GetAccount(LoginFormModel loginFormModel)
        {
            RequestResult requestResult = new RequestResult();
            try
            {
                using (CFContext context = new CFContext())
                {
                    var person = context.People.FirstOrDefault(x => x.LoginId == loginFormModel.LoginId);
                    if (person != null)
                    {
                        requestResult.ReturnData(person);
                    }
                    else
                    {
                        requestResult.ReturnFailedMessage(Resources.Resource.LoginIdNotExist);
                    }
                }
            }
            catch (Exception e)
            {
                var error = new Error(MethodBase.GetCurrentMethod(), e);
                Logger.Log(error);
                requestResult.ReturnError(error);
            }

            return requestResult;
        }

        public static List<PersonModel> GetAllPeople()
        {
            var accountList = new List<PersonModel>();

            try
            {
                using (CFContext context = new CFContext())
                {
                    accountList = context.People.Select(p => new PersonModel
                    {
                        ID = p.LoginId,
                        Name = p.Name,
                        OrganizationId = p.OrganizationId.ToString()
                    }).ToList();
                }
            }
            catch (Exception e)
            {
                accountList = new List<PersonModel>();
                Logger.Log(MethodBase.GetCurrentMethod(), e);
                //throw;
            }

            return accountList;
        }

        public static RequestResult GetAccount(List<Models.Shared.Organization> organizations, Guid personId)
        {
            RequestResult requestResult = new RequestResult();
            try
            {
                using (CFContext context = new CFContext())
                {
                    var person = context.People.FirstOrDefault(x => x.PersonId == personId);
                    if (person != null)
                    {
                        //requestResult = GetAccount(organizations, person);
                    }
                    else
                    {
                        requestResult.Failed();
                    }
                }
            }
            catch (Exception e)
            {
                var error = new Error(MethodBase.GetCurrentMethod(), e);
                Logger.Log(error);
                requestResult.ReturnError(error);
            }

            return requestResult;
        }

        public static RequestResult GetAccount(List<Models.Shared.Organization> organizations, Person person)
        {
            RequestResult requestResult = new RequestResult();
            try
            {
                using (CFContext context = new CFContext())
                {
                    var newPerson = context.People.Include("Roles").Where(p => p.PersonId == person.PersonId).FirstOrDefault();
                    var webPermissionsAll = new List<WebPermission>();
                    var webFunctionsAll = new List<WebFunction>();
                    var webPermissionWebFunctions = new List<WebPermissionWebFunctionModel>();
                    var roleIds = new List<string>();

                    foreach (var r in newPerson.Roles)
                    {
                        var webPermissionIds = r.WebPermissionWebFunctions.Select(wpwf => wpwf.WebPermissionId).ToList();
                        foreach (var webPermissionId in webPermissionIds)
                        {
                            var webPermission = context.WebPermissions.Where(wp => wp.WebPermissionId == webPermissionId).FirstOrDefault();
                            var webPermissionWebFunction = r.WebPermissionWebFunctions.Select(wpwf => new WebPermissionWebFunctionModel
                            {
                                WebPermissionId = wpwf.WebPermissionId,
                                WebFunctionId = wpwf.WebFunctionId,
                                Area = webPermission.Area,
                                Controller = webPermission.Controller,
                                Action = webPermission.Action
                            }).ToList();
                            webPermissionWebFunctions.AddRange(webPermissionWebFunction);
                        }


                        //var personPermissions = context.WebPermissions.Where(wp => wp.WebPermissionId == webPermissionId).Select(x => new
                        //{
                        //    x.WebPermissionId,
                        //    x.ParentId,
                        //}).ToList();
                    }

                    roleIds = newPerson.Roles.Select(r => r.RoleId).ToList();

                    //foreach (var role in newPerson.Roles)
                    //{
                    //    var webPermissionIds = role.WebPermissionWebFunctions.Select(wpwf => wpwf.WebPermissionId);                        
                    //    var webPermissions = context.WebPermissions.Include("WebPermissionWebFunctions").Where(wp => webPermissionIds.Contains(wp.WebPermissionId) && wp.ParentId != "*");
                    //    webPermissionsAll.AddRange(webPermissions);
                    //}                    

                    var account = new Account()
                    {
                        Id = person.LoginId,
                        Name = person.Name,
                        OrganizationId = person.OrganizationId,
                        //WebPermissions = webPermissionsAll.Distinct(new PropertyCompare<WebPermission>("WebPermissionId")).ToList(),
                        WebPermissionFunctions = webPermissionWebFunctions,
                        RoleIds = roleIds,
                        OrganizationPermissions = OrganizationDataAccessor.GetOrganizationPermissions(person.OrganizationId)
                    };

                    account.RootOrganizationId = OrganizationDataAccessor.GetRootOrganizationId(organizations, account);

                    var personPermissions = (from r in newPerson.Roles
                                             join x in context.RolePermissionFunctions
                                             on r.RoleId equals x.RoleId
                                             join p in context.WebPermissions
                                             on x.WebPermissionId equals p.WebPermissionId
                                             select new
                                             {
                                                 p.WebPermissionId,
                                                 p.ParentId,
                                             }).Distinct().ToList();

                    foreach (var wp in personPermissions)
                    {
                        var parent = new WebPermission();

                        parent = context.WebPermissions.First(p => p.WebPermissionId == wp.ParentId);

                        var permission = context.WebPermissions.First(p => p.WebPermissionId == wp.WebPermissionId);

                        if (parent.ParentId == "*")
                        {
                            var ancestorMenuItem = account.MenuItems.FirstOrDefault(m => m.Id == parent.WebPermissionId);
                            if (ancestorMenuItem == null)
                            {
                                ancestorMenuItem = new MenuItem()
                                {
                                    Id = parent.WebPermissionId,
                                    Name = context.WebPermissionNames.Where(wpd => wpd.WebPermissionId == parent.WebPermissionId).ToDictionary(wpd => wpd.Language, wpd => wpd.Name),
                                    Area = string.Empty,
                                    Controller = string.Empty,
                                    Action = string.Empty,
                                    Icon = parent.Icon,
                                    Seq = parent.Seq
                                };
                                account.MenuItems.Add(ancestorMenuItem);
                            }

                            ancestorMenuItem.SubItemList.Add(new MenuItem()
                            {
                                Id = permission.WebPermissionId,
                                Name = context.WebPermissionNames.Where(wpd => wpd.WebPermissionId == permission.WebPermissionId).ToDictionary(wpd => wpd.Language, wpd => wpd.Name),
                                Area = permission.Area,
                                Controller = permission.Controller,
                                Action = permission.Action,
                                Icon = permission.Icon,
                                Seq = permission.Seq
                            });
                        }
                        else
                        {
                            var ancestor = context.WebPermissions.First(p => p.WebPermissionId == parent.ParentId);
                            var ancestorMenuItem = account.MenuItems.FirstOrDefault(m => m.Id == ancestor.WebPermissionId);
                            if (ancestorMenuItem == null)
                            {
                                ancestorMenuItem = new MenuItem()
                                {
                                    Id = ancestor.WebPermissionId,
                                    Name = context.WebPermissionNames.Where(wpd => wpd.WebPermissionId == ancestor.WebPermissionId).ToDictionary(wpd => wpd.Language, wpd => wpd.Name),
                                    Area = string.Empty,
                                    Controller = string.Empty,
                                    Action = string.Empty,
                                    Icon = ancestor.Icon,
                                    Seq = ancestor.Seq
                                };

                                account.MenuItems.Add(ancestorMenuItem);
                            }

                            var parentMenuItem = ancestorMenuItem.SubItemList.FirstOrDefault(s => s.Id == parent.WebPermissionId);
                            if (parentMenuItem == null)
                            {
                                parentMenuItem = new MenuItem()
                                {
                                    Id = parent.WebPermissionId,
                                    Name = context.WebPermissionNames.Where(wpd => wpd.WebPermissionId == parent.WebPermissionId).ToDictionary(wpd => wpd.Language, wpd => wpd.Name),
                                    Area = string.Empty,
                                    Controller = string.Empty,
                                    Action = string.Empty,
                                    Icon = parent.Icon,
                                    Seq = parent.Seq
                                };

                                ancestorMenuItem.SubItemList.Add(parentMenuItem);
                            }

                            parentMenuItem.SubItemList.Add(new MenuItem()
                            {
                                Id = permission.WebPermissionId,
                                Name = context.WebPermissionNames.Where(wpd => wpd.WebPermissionId == permission.WebPermissionId).ToDictionary(wpd => wpd.Language, wpd => wpd.Name),
                                Area = permission.Area,
                                Controller = permission.Controller,
                                Action = permission.Action,
                                Icon = permission.Icon,
                                Seq = permission.Seq
                            });
                        }
                    }

                    foreach (var menu in account.MenuItems)
                    {
                        foreach (var item in menu.SubItemList)
                        {
                            if (item.SubItemList != null)
                            {
                                item.SubItemList = item.SubItemList.OrderBy(x => x.Seq).ToList();
                            }
                        }

                        menu.SubItemList = menu.SubItemList.OrderBy(x => x.Seq).ToList();
                    }

                    requestResult.ReturnData(account);
                }
            }
            catch (Exception e)
            {
                var error = new Error(MethodBase.GetCurrentMethod(), e);
                Logger.Log(error);
                requestResult.ReturnError(error);
            }

            return requestResult;
        }

        public static RequestResult ChangePassword(PasswordFormModel passwordFormModel, Account account)
        {
            RequestResult requestResult = new RequestResult();

            try
            {
                using (CFContext context = new CFContext())
                {
                    var person = context.People.First(p => p.LoginId == account.Id);
                    if (string.Compare(person.Password, passwordFormModel.OriginalPassword, false) == 0)
                    {
                        person.Password = passwordFormModel.NewPassword;
                        person.LastModifyTime = DateTime.Now;

                        context.SaveChanges();

                        requestResult.ReturnSuccessMessage(Resources.Resource.ChangePassword + " " + Resources.Resource.Success);
                    }
                    else
                    {
                        requestResult.ReturnFailedMessage(Resources.Resource.WrongPassword);
                    }
                }
            }
            catch (Exception e)
            {
                var error = new Error(MethodBase.GetCurrentMethod(), e);

                Logger.Log(error);

                requestResult.ReturnError(error);
            }

            return requestResult;
        }

        //public static RequestResult ChangeUserPhoto(UserPhotoFormModel userPhotoFormModel,Account account)
        //{
        //    RequestResult requestResult = new RequestResult();
        //    try
        //    {
        //        if (userPhotoFormModel.Photo != null && userPhotoFormModel.Photo.ContentLength > 0)
        //        {
        //            var uniqueID = Guid.NewGuid().ToString();
        //            string extension = userPhotoFormModel.Photo.FileName.Substring(userPhotoFormModel.Photo.FileName.LastIndexOf('.') + 1);
        //            userPhotoFormModel.Photo.SaveAs(Path.Combine(Config.UserPhotoFolderPath, string.Format("{0}.{1}", uniqueID, extension)));
        //            using (CFContext context = new CFContext())
        //            {
        //                var userPhoto = context.UserPhoto.FirstOrDefault(x => x.UserID == account.Id);
        //            }
        //        }
        //    }
        //}
    }
}
