using CF;
using CF.Models.Maintenance;
using Models.Authentication;
using Models.Maintenance.CheckItem;
using Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Utility;
using Utility.Models;

namespace DataAccessor.Maintenance
{
    public class CheckItemDataAccessor
    {
        public static RequestResult Query(QueryParameters queryParameters, Account account)
        {
            RequestResult result = new RequestResult();

            try
            {
                using (CFContext context=new CFContext())
                {
                    var downStreamOrganizationIds = OrganizationDataAccessor.GetDownStreamOrganizationIds(new Guid(queryParameters.OrganizationId), true);

                    var query = context.CheckItems.Where(x => downStreamOrganizationIds.Contains(x.OrganizationId) && account.QueryableOrganizationIds.Contains(x.OrganizationId)).AsQueryable();

                    if (!string.IsNullOrEmpty(queryParameters.Type))
                    {
                        query = query.Where(x => x.OrganizationId == new Guid(queryParameters.OrganizationId) && x.Type == queryParameters.Type);
                    }

                    if (!string.IsNullOrEmpty(queryParameters.Keyword))
                    {
                        query = query.Where(x => x.CIId.Contains(queryParameters.Keyword) || x.Name.Contains(queryParameters.Keyword));
                    }

                    var organization = OrganizationDataAccessor.GetOrganization(new Guid(queryParameters.OrganizationId));

                    result.ReturnData(new GridViewModel()
                    {
                        OrganizationId = queryParameters.OrganizationId,
                        Permission = account.OrganizationPermission(new Guid(queryParameters.OrganizationId)),
                        Type = queryParameters.Type,
                        OrganizationName = organization.Name,
                        FullOrganizationName = organization.FullName,
                        Items = query.ToList().Select(x => new GridItem()
                        {
                            CheckItemId = x.CheckItemId.ToString(),
                            Permission = account.OrganizationPermission(x.OrganizationId),
                            OrganizationName = OrganizationDataAccessor.GetOrganizationName(x.OrganizationId),
                            Type = x.Type,
                            CIId = x.CIId,
                            Name = x.Name,
                            Unit = x.Unit
                        }).OrderBy(x => x.OrganizationName).ThenBy(x => x.Type).ThenBy(x => x.CIId).ToList()
                    });
                }
            }
            catch (Exception ex)
            {
                var err = new Error(MethodBase.GetCurrentMethod(), ex);

                Logger.Log(err);

                result.ReturnError(err);
            }

            return result;
        }

        public static RequestResult GetDetailViewModel(string checkItemId, Account account)
        {
            RequestResult result = new RequestResult();

            try
            {
                using (CFContext context=new CFContext())
                {
                    var checkItem = context.CheckItems.Include("AbnormalReasons").Include("FeelOptions").First(x => x.CheckItemId == new Guid(checkItemId));

                    result.ReturnData(new DetailViewModel()
                    {
                        CheckItemId = checkItem.CheckItemId.ToString(),
                        Permission = account.OrganizationPermission(checkItem.OrganizationId),
                        OrganizationId = checkItem.OrganizationId.ToString(),
                        ParentOrganizationFullName = OrganizationDataAccessor.GetOrganizationFullName(checkItem.OrganizationId),
                        Type = checkItem.Type,
                        CIId = checkItem.CIId,
                        Name = checkItem.Name,
                        IsFeelItem = checkItem.IsFeelItem,
                        UpperLimit = checkItem.UpperLimit.HasValue ? checkItem.UpperLimit.ToString() : "",
                        UpperAlertLimit = checkItem.UpperAlertLimit.HasValue ? checkItem.UpperAlertLimit.ToString() : "",
                        LowerAlertLimit = checkItem.LowerAlertLimit.HasValue ? checkItem.LowerAlertLimit.ToString() : "",
                        LowerLimit = checkItem.LowerLimit.HasValue ? checkItem.LowerLimit.ToString() : "",
                        IsAccumulation = checkItem.IsAccumulation,
                        AccumulationBase = checkItem.AccumulationBase.HasValue ? checkItem.AccumulationBase.ToString() : "",
                        Unit = checkItem.Unit,
                        Remark = checkItem.Remark,
                        TextValueType = checkItem.TextValueType,
                        AbnormalReasonNames = checkItem.AbnormalReasons.Select(x=>x.Name).ToList(),                        
                        FeelOptionNames = checkItem.FeelOptions.Select(x=>x.Name + (x.IsAbnormal ? "(" + Resources.Resource.Abnormal + ")" : "")).ToList()
                        
                    });
                }
            }
            catch (Exception ex)
            {
                var err = new Error(MethodBase.GetCurrentMethod(), ex);

                Logger.Log(err);

                result.ReturnError(err);
            }

            return result;
        }

        public static RequestResult GetCreateFormModel(string organizationId, string type)
        {
            RequestResult result = new RequestResult();

            try
            {
                using (CFContext context=new CFContext())
                {
                    var model = new CreateFormModel()
                    {
                        OrganizationId = organizationId,
                        ParentOrganizationFullName = OrganizationDataAccessor.GetOrganizationFullName(new Guid(organizationId)),
                        TypeSelectListItems = new List<SelectListItem>()
                        {
                            Define.DefaultSelectListItem(Resources.Resource.Select),
                            new SelectListItem()
                            {
                                Text = Resources.Resource.Create + "...",
                                Value = Define.New
                            }
                        },
                        FormInput = new FormInput()
                        {
                            Type = type
                        },
                        FeelOptionModels = new List<FeelOptionModel>()
                        {
                            new FeelOptionModel()
                            {
                                Name = Resources.Resource.Normal,
                                IsAbnormal = false,
                                Seq = 1
                            },
                            new FeelOptionModel()
                            {
                                Name = Resources.Resource.Abnormal,
                                IsAbnormal = true,
                                Seq = 2
                            }
                        }
                    };

                    var upStreamOrganizationIds = OrganizationDataAccessor.GetUpStreamOrganizationIds(new Guid(organizationId), true);

                    model.TypeSelectListItems.AddRange(context.CheckItems.Where(x => upStreamOrganizationIds.Contains(x.OrganizationId)).Select(x => x.Type).Distinct().OrderBy(x => x).Select(x => new SelectListItem
                    {
                        Value = x,
                        Text = x
                    }).ToList());

                    if (!string.IsNullOrEmpty(type) && model.TypeSelectListItems.Any(x => x.Value == type))
                    {
                        model.TypeSelectListItems.First(x => x.Value == type).Selected = true;
                    }

                    result.ReturnData(model);
                }
            }
            catch (Exception ex)
            {
                var err = new Error(MethodBase.GetCurrentMethod(), ex);

                Logger.Log(err);

                result.ReturnError(err);
            }

            return result;
        }

        public static RequestResult GetCopyFormModel(string checkItemId)
        {
            RequestResult result = new RequestResult();

            try
            {
                using (CFContext context = new CFContext())
                {
                    var checkItem = context.CheckItems.Include("AbnormalReasons").Include("FeelOptions").First(x => x.CheckItemId == new Guid(checkItemId));

                    var model = new CreateFormModel()
                    {
                        OrganizationId = checkItem.OrganizationId.ToString(),
                        ParentOrganizationFullName = OrganizationDataAccessor.GetOrganizationFullName(checkItem.OrganizationId),
                        TypeSelectListItems = new List<SelectListItem>()
                        {
                            Define.DefaultSelectListItem(Resources.Resource.Select),
                            new SelectListItem()
                            {
                                Text = Resources.Resource.Create + "...",
                                Value = Define.New
                            }
                        },
                        AbnormalReasonModels =checkItem.AbnormalReasons.Select(x=>new AbnormalReasonModel()
                        {
                            AbnormalReasonId=x.AbnormalReasonId.ToString(),
                            Type=x.Type,
                            ARId=x.ARId,
                            Name=x.Name,
                        }).OrderBy(x=>x.Type).ThenBy(x=>x.ARId).ToList(),                        
                        FormInput = new FormInput()
                        {
                            Type = checkItem.Type,
                            IsFeelItem = checkItem.IsFeelItem ? "Y" : "N",
                            TextValueType = checkItem.TextValueType,
                            LowerLimit = checkItem.LowerLimit.HasValue ? checkItem.LowerLimit.ToString() : "",
                            LowerAlertLimit = checkItem.LowerAlertLimit.HasValue ? checkItem.LowerAlertLimit.ToString() : "",
                            UpperAlertLimit = checkItem.UpperAlertLimit.HasValue ? checkItem.UpperAlertLimit.ToString() : "",
                            UpperLimit = checkItem.UpperLimit.HasValue ? checkItem.UpperLimit.ToString() : "",
                            IsAccumulation = checkItem.IsAccumulation,
                            AccumulationBase = checkItem.AccumulationBase.HasValue ? checkItem.AccumulationBase.ToString() : "",
                            Unit = checkItem.Unit,
                            Remark = checkItem.Remark
                        }
                    };

                    var upStreamOrganizationIds = OrganizationDataAccessor.GetUpStreamOrganizationIds(checkItem.OrganizationId, true);

                    model.TypeSelectListItems.AddRange(context.CheckItems.Where(x => upStreamOrganizationIds.Contains(x.OrganizationId)).Select(x => x.Type).Distinct().OrderBy(x => x).Select(x => new SelectListItem
                    {
                        Value = x,
                        Text = x
                    }).ToList());

                    model.TypeSelectListItems.First(x => x.Value == checkItem.Type).Selected = true;

                    if (checkItem.IsFeelItem)
                    {
                        model.FeelOptionModels = checkItem.FeelOptions.Select(x => new FeelOptionModel()
                        {
                            FeelOptionId = x.FeelOptionId.ToString(),
                            Name = x.Name,
                            IsAbnormal = x.IsAbnormal,
                            Seq = x.Seq
                        }).OrderBy(x => x.Seq).ToList();                            
                    }
                    else
                    {
                        model.FeelOptionModels = new List<FeelOptionModel>()
                        {
                            new FeelOptionModel()
                            {
                                Name = Resources.Resource.Normal,
                                IsAbnormal = false,
                                Seq = 1
                            },
                            new FeelOptionModel()
                            {
                                Name = Resources.Resource.Abnormal,
                                IsAbnormal = true,
                                Seq = 2
                            }
                        };
                    }

                    result.ReturnData(model);
                }
            }
            catch (Exception ex)
            {
                var err = new Error(MethodBase.GetCurrentMethod(), ex);

                Logger.Log(err);

                result.ReturnError(err);
            }

            return result;
        }

        public static RequestResult Create(CreateFormModel createFormModel)
        {
            RequestResult result = new RequestResult();

            try
            {
                if (createFormModel.FormInput.Type == Define.Other || createFormModel.FormInput.Type == Define.New)
                {
                    result.ReturnFailedMessage(string.Format("{0} {1}", Resources.Resource.Unsupported, Resources.Resource.CheckItemType));
                }
                else
                {
                    using (CFContext context = new CFContext())
                    {
                        var exists = context.CheckItems.FirstOrDefault(x => x.OrganizationId == new Guid(createFormModel.OrganizationId) && x.Type == createFormModel.FormInput.Type && x.CIId == createFormModel.FormInput.CIId);

                        if (exists == null)
                        {
                            Guid checkItemId = Guid.NewGuid();

                            var checkItem = new CheckItem()
                            {
                                CheckItemId = checkItemId,
                                OrganizationId = new Guid(createFormModel.OrganizationId),
                                Type = createFormModel.FormInput.Type,
                                CIId = createFormModel.FormInput.CIId,
                                Name = createFormModel.FormInput.Name,
                                IsFeelItem = createFormModel.FormInput.IsFeelItem == "Y",
                                FeelOptions=createFormModel.FormInput.FeelOptionList.Select(x=>new FeelOption()
                                {
                                    FeelOptionId=new Guid(x.FeelOptionId),
                                    Name=x.Name,
                                    IsAbnormal=x.IsAbnormal,
                                    Seq=x.Seq
                                }).ToList(),
                                AbnormalReasons=createFormModel.AbnormalReasonModels.Select(x=>new AbnormalReason()
                                {
                                    AbnormalReasonId=new Guid(x.AbnormalReasonId),
                                    ARId=x.ARId,
                                    Name=x.Name,
                                    Type=x.Type,
                                    OrganizationId=new Guid(createFormModel.OrganizationId),
                                    LastModifyTime=DateTime.Now
                                }).ToList(),
                                IsAccumulation = false,
                                Unit = createFormModel.FormInput.Unit,
                                Remark = createFormModel.FormInput.Remark,
                                LastModifyTime = DateTime.Now
                            };

                            if (createFormModel.FormInput.IsFeelItem == "N")
                            {
                                checkItem.TextValueType = createFormModel.FormInput.TextValueType;

                                if (createFormModel.FormInput.TextValueType == 1)
                                {
                                    checkItem.IsAccumulation = createFormModel.FormInput.IsAccumulation;

                                    if (createFormModel.FormInput.IsAccumulation)
                                    {
                                        checkItem.AccumulationBase = !string.IsNullOrEmpty(createFormModel.FormInput.AccumulationBase) ? double.Parse(createFormModel.FormInput.AccumulationBase) : default(double?);
                                    }

                                    checkItem.LowerLimit = !string.IsNullOrEmpty(createFormModel.FormInput.LowerLimit) ? double.Parse(createFormModel.FormInput.LowerLimit) : default(double?);
                                    checkItem.LowerAlertLimit = !string.IsNullOrEmpty(createFormModel.FormInput.LowerAlertLimit) ? double.Parse(createFormModel.FormInput.LowerAlertLimit) : default(double?);
                                    checkItem.UpperAlertLimit = !string.IsNullOrEmpty(createFormModel.FormInput.UpperAlertLimit) ? double.Parse(createFormModel.FormInput.UpperAlertLimit) : default(double?);
                                    checkItem.UpperLimit = !string.IsNullOrEmpty(createFormModel.FormInput.UpperLimit) ? double.Parse(createFormModel.FormInput.UpperLimit) : default(double?);
                                }
                            }

                            context.CheckItems.Add(checkItem);                                                        

                            context.SaveChanges();

                            result.ReturnData(checkItemId, string.Format("{0} {1} {2}", Resources.Resource.Create, Resources.Resource.CheckItem, Resources.Resource.Success));
                        }
                        else
                        {
                            result.ReturnFailedMessage(string.Format("{0} {1}", Resources.Resource.CIId, Resources.Resource.Exists));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var err = new Error(MethodBase.GetCurrentMethod(), ex);

                Logger.Log(err);

                result.ReturnError(err);
            }

            return result;
        }

        public static RequestResult GetEditFormModel(string checkItemId)
        {
            RequestResult result = new RequestResult();

            try
            {
                using (CFContext context = new CFContext())
                {
                    var checkItem = context.CheckItems.Include("AbnormalReasons").Include("FeelOptions").First(x => x.CheckItemId == new Guid(checkItemId));

                    var model = new EditFormModel()
                    {
                        CheckItemId = checkItem.CheckItemId.ToString(),
                        OrganizationId = checkItem.OrganizationId.ToString(),
                        ParentOrganizationFullName = OrganizationDataAccessor.GetOrganizationFullName(checkItem.OrganizationId),
                        TypeSelectListItems = new List<SelectListItem>()
                        {
                            Define.DefaultSelectListItem(Resources.Resource.Select),
                            new SelectListItem()
                            {
                                Text = Resources.Resource.Create + "...",
                                Value = Define.New
                            }
                        },
                        AbnormalReasonModels = checkItem.AbnormalReasons.Select(x=>new AbnormalReasonModel()
                        {
                            AbnormalReasonId=x.AbnormalReasonId.ToString(),
                            Type=x.Type,
                            ARId=x.ARId,
                            Name=x.Name
                        }).OrderBy(x=>x.Type).ThenBy(x=>x.ARId).ToList(),
                        
                        FormInput = new FormInput()
                        {
                            Type = checkItem.Type,
                            CIId = checkItem.CIId,
                            Name = checkItem.Name,
                            IsFeelItem = checkItem.IsFeelItem ? "Y" : "N",
                            LowerLimit = checkItem.LowerLimit.HasValue ? checkItem.LowerLimit.ToString() : "",
                            LowerAlertLimit = checkItem.LowerAlertLimit.HasValue ? checkItem.LowerAlertLimit.ToString() : "",
                            UpperAlertLimit = checkItem.UpperAlertLimit.HasValue ? checkItem.UpperAlertLimit.ToString() : "",
                            UpperLimit = checkItem.UpperLimit.HasValue ? checkItem.UpperLimit.ToString() : "",
                            IsAccumulation = checkItem.IsAccumulation,
                            AccumulationBase = checkItem.AccumulationBase.HasValue ? checkItem.AccumulationBase.ToString() : "",
                            Unit = checkItem.Unit,
                            Remark = checkItem.Remark,
                            TextValueType = checkItem.TextValueType
                        }
                    };

                    var upStreamOrganizationIds = OrganizationDataAccessor.GetUpStreamOrganizationIds(new Guid(model.OrganizationId), true);

                    model.TypeSelectListItems.AddRange(context.CheckItems.Where(x => upStreamOrganizationIds.Contains(x.OrganizationId)).Select(x => x.Type).Distinct().OrderBy(x => x).Select(x => new SelectListItem
                    {
                        Value = x,
                        Text = x
                    }).ToList());

                    if (!string.IsNullOrEmpty(model.FormInput.Type) && model.TypeSelectListItems.Any(x => x.Value == model.FormInput.Type))
                    {
                        model.TypeSelectListItems.First(x => x.Value == model.FormInput.Type).Selected = true;
                    }

                    if (checkItem.IsFeelItem)
                    {
                        model.FeelOptionModels = checkItem.FeelOptions.Select(x => new FeelOptionModel()
                        {
                            FeelOptionId = x.FeelOptionId.ToString(),
                            Name = x.Name,
                            IsAbnormal = x.IsAbnormal,
                            Seq = x.Seq
                        }).ToList();                            
                    }
                    else
                    {
                        model.FeelOptionModels = new List<FeelOptionModel>()
                        {
                            new FeelOptionModel()
                            {
                                Name = Resources.Resource.Normal,
                                IsAbnormal = false,
                                Seq = 1
                            },
                            new FeelOptionModel()
                            {
                                Name = Resources.Resource.Abnormal,
                                IsAbnormal = true,
                                Seq = 2
                            }
                        };
                    }

                    result.ReturnData(model);
                }
            }
            catch (Exception ex)
            {
                var err = new Error(MethodBase.GetCurrentMethod(), ex);

                Logger.Log(err);

                result.ReturnError(err);
            }

            return result;
        }

        public static RequestResult Edit(EditFormModel editFormModel)
        {
            RequestResult result = new RequestResult();

            try
            {
                if (editFormModel.FormInput.Type == Define.Other || editFormModel.FormInput.Type == Define.New)
                {
                    result.ReturnFailedMessage(string.Format("{0} {1}", Resources.Resource.Unsupported, Resources.Resource.CheckItemType));
                }
                else
                {
                    using (CFContext context = new CFContext())
                    {
                        var checkItem = context.CheckItems.Include("AbnormalReasons").Include("FeelOptions").First(x => x.CheckItemId == new Guid(editFormModel.CheckItemId));

                        var exists = context.CheckItems.FirstOrDefault(x => x.CheckItemId != checkItem.CheckItemId && x.OrganizationId == checkItem.OrganizationId && x.Type == editFormModel.FormInput.Type && x.CIId == editFormModel.FormInput.CIId);

                        if (exists == null)
                        {
#if !DEBUG
                    using (TransactionScope trans = new TransactionScope())
                    {
#endif
                            #region CheckItem
                            checkItem.Type = editFormModel.FormInput.Type;
                            checkItem.CIId = editFormModel.FormInput.CIId;
                            checkItem.Name = editFormModel.FormInput.Name;
                            checkItem.IsFeelItem = editFormModel.FormInput.IsFeelItem == "Y";
                            checkItem.Remark = editFormModel.FormInput.Remark;
                            checkItem.Unit = editFormModel.FormInput.Unit;
                            checkItem.LastModifyTime = DateTime.Now;

                            checkItem.TextValueType = null;
                            checkItem.IsAccumulation = false;
                            checkItem.AccumulationBase = null;
                            checkItem.LowerLimit = null;
                            checkItem.LowerAlertLimit = null;
                            checkItem.UpperAlertLimit = null;
                            checkItem.UpperLimit = null;

                            if (editFormModel.FormInput.IsFeelItem == "N")
                            {
                                checkItem.TextValueType = editFormModel.FormInput.TextValueType;

                                if (editFormModel.FormInput.TextValueType == 1)
                                {
                                    checkItem.IsAccumulation = editFormModel.FormInput.IsAccumulation;

                                    if (editFormModel.FormInput.IsAccumulation)
                                    {
                                        checkItem.AccumulationBase = !string.IsNullOrEmpty(editFormModel.FormInput.AccumulationBase) ? double.Parse(editFormModel.FormInput.AccumulationBase) : default(double?);
                                    }

                                    checkItem.LowerLimit = !string.IsNullOrEmpty(editFormModel.FormInput.LowerLimit) ? double.Parse(editFormModel.FormInput.LowerLimit) : default(double?);
                                    checkItem.LowerAlertLimit = !string.IsNullOrEmpty(editFormModel.FormInput.LowerAlertLimit) ? double.Parse(editFormModel.FormInput.LowerAlertLimit) : default(double?);
                                    checkItem.UpperAlertLimit = !string.IsNullOrEmpty(editFormModel.FormInput.UpperAlertLimit) ? double.Parse(editFormModel.FormInput.UpperAlertLimit) : default(double?);
                                    checkItem.UpperLimit = !string.IsNullOrEmpty(editFormModel.FormInput.UpperLimit) ? double.Parse(editFormModel.FormInput.UpperLimit) : default(double?);
                                }
                            }

                            context.SaveChanges();
                            #endregion

                            #region FeelOption
                            #region Delete
                            checkItem.FeelOptions = new HashSet<FeelOption>();                            

                            context.SaveChanges();
                            #endregion

                            #region Insert
                            if (editFormModel.FormInput.IsFeelItem == "Y")
                            {
                                checkItem.FeelOptions = editFormModel.FormInput.FeelOptionList.Select(x => new FeelOption()
                                {
                                    FeelOptionId = new Guid(x.FeelOptionId),
                                    Name = x.Name,
                                    IsAbnormal = x.IsAbnormal,
                                    Seq = x.Seq
                                }).ToList();                                
                            }

                            context.SaveChanges();
                            #endregion
                            #endregion

                            #region AbnormalReason
                            #region Delete
                            checkItem.AbnormalReasons = new HashSet<AbnormalReason>();                            

                            context.SaveChanges();
                            #endregion

                            #region Insert
                            checkItem.AbnormalReasons = editFormModel.AbnormalReasonModels.Select(x => new AbnormalReason()
                            {
                                AbnormalReasonId = new Guid(x.AbnormalReasonId),
                                ARId = x.ARId,
                                Name = x.Name,
                                Type = x.Type,
                                OrganizationId = new Guid(editFormModel.OrganizationId),
                                LastModifyTime = DateTime.Now
                            }).ToList();                            

                            context.SaveChanges();
                            #endregion
                            #endregion

#if !DEBUG
                        trans.Complete();
                    }
#endif

                            result.ReturnSuccessMessage(string.Format("{0} {1} {2}", Resources.Resource.Edit, Resources.Resource.CheckItem, Resources.Resource.Success));
                        }
                        else
                        {
                            result.ReturnFailedMessage(string.Format("{0} {1}", Resources.Resource.CIId, Resources.Resource.Exists));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var err = new Error(MethodBase.GetCurrentMethod(), ex);

                Logger.Log(err);

                result.ReturnError(err);
            }

            return result;
        }

        public static RequestResult Delete(List<string> selectedList)
        {
            RequestResult result = new RequestResult();

            try
            {
                using (CFContext context = new CFContext())
                {
                    DeleteHelper.DeleteCheckItem(context, selectedList);

                    context.SaveChanges();
                }

                result.ReturnSuccessMessage(string.Format("{0} {1} {2}", Resources.Resource.Delete, Resources.Resource.CheckItem, Resources.Resource.Success));
            }
            catch (Exception ex)
            {
                var err = new Error(MethodBase.GetCurrentMethod(), ex);

                Logger.Log(err);

                result.ReturnError(err);
            }

            return result;
        }

        public static RequestResult AddAbnormalReason(List<AbnormalReasonModel> abnormalReasonModels, List<string> selectedList, string refOrganizationId)
        {
            RequestResult result = new RequestResult();

            try
            {
                using (CFContext context = new CFContext())
                {
                    foreach (string selected in selectedList)
                    {
                        string[] temp = selected.Split(Define.Seperators, StringSplitOptions.None);

                        var organizationId = temp[0];
                        var abnormalType = temp[1];
                        var abnormalReasonId = temp[2];

                        if (!string.IsNullOrEmpty(abnormalReasonId))
                        {
                            if (!abnormalReasonModels.Any(x => x.AbnormalReasonId == abnormalReasonId))
                            {
                                var abnormalReason = context.AbnormalReasons.First(x => x.AbnormalReasonId == new Guid(abnormalReasonId));

                                abnormalReasonModels.Add(new AbnormalReasonModel()
                                {
                                    AbnormalReasonId = abnormalReason.AbnormalReasonId.ToString(),
                                    Type = abnormalReason.Type,
                                    ARId = abnormalReason.ARId,
                                    Name = abnormalReason.Name
                                });
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(abnormalType))
                            {
                                var abnormalReasonList = context.AbnormalReasons.Where(x => x.OrganizationId == new Guid(organizationId) && x.Type == abnormalType).ToList();

                                foreach (var abnormalReason in abnormalReasonList)
                                {
                                    if (!abnormalReasonModels.Any(x => x.AbnormalReasonId == abnormalReason.AbnormalReasonId.ToString()))
                                    {
                                        abnormalReasonModels.Add(new AbnormalReasonModel()
                                        {
                                            AbnormalReasonId = abnormalReason.AbnormalReasonId.ToString(),
                                            Type = abnormalReason.Type,
                                            ARId = abnormalReason.ARId,
                                            Name = abnormalReason.Name
                                        });
                                    }
                                }
                            }
                            else
                            {
                                var availableOrganizationIds = OrganizationDataAccessor.GetUpStreamOrganizationIds(new Guid(refOrganizationId), true);

                                var downStreamOrganizationIds = OrganizationDataAccessor.GetDownStreamOrganizationIds(new Guid(organizationId), true);

                                foreach (var downStreamOrganizationId in downStreamOrganizationIds)
                                {
                                    if (availableOrganizationIds.Any(x => x == downStreamOrganizationId))
                                    {
                                        var abnormalReasons = context.AbnormalReasons.Where(x => x.OrganizationId == downStreamOrganizationId).ToList();

                                        foreach (var abnormalReason in abnormalReasons)
                                        {
                                            if (!abnormalReasonModels.Any(x => x.AbnormalReasonId == abnormalReason.AbnormalReasonId.ToString()))
                                            {
                                                abnormalReasonModels.Add(new AbnormalReasonModel()
                                                {
                                                    AbnormalReasonId = abnormalReason.AbnormalReasonId.ToString(),
                                                    Type = abnormalReason.Type,
                                                    ARId = abnormalReason.ARId,
                                                    Name = abnormalReason.Name
                                                });
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                result.ReturnData(abnormalReasonModels.OrderBy(x => x.Type).ThenBy(x => x.ARId).ToList());
            }
            catch (Exception ex)
            {
                var err = new Error(MethodBase.GetCurrentMethod(), ex);

                Logger.Log(err);

                result.ReturnError(err);
            }

            return result;
        }

        public static RequestResult GetTreeItems(List<Models.Shared.Organization> organizations, Guid organizationId, string type, Account account)
        {
            RequestResult result = new RequestResult();

            try
            {
                var treeItemList = new List<TreeItem>();

                var attributes = new Dictionary<Define.EnumTreeAttribute, string>()
                {
                    { Define.EnumTreeAttribute.NodeType, string.Empty },
                    { Define.EnumTreeAttribute.ToolTip, string.Empty },
                    { Define.EnumTreeAttribute.OrganizationId, string.Empty },
                    { Define.EnumTreeAttribute.CheckType, string.Empty },
                    { Define.EnumTreeAttribute.CheckItemId, string.Empty }
                };

                using (CFContext context = new CFContext())
                {
                    if (string.IsNullOrEmpty(type))
                    {
                        if (account.QueryableOrganizationIds.Contains(organizationId))
                        {
                            var checkTypeList = context.CheckItems.Where(x => x.OrganizationId == organizationId).Select(x => x.Type).Distinct().OrderBy(x => x).ToList();

                            foreach (var checkType in checkTypeList)
                            {
                                var treeItem = new TreeItem() { Title = checkType };

                                attributes[Define.EnumTreeAttribute.NodeType] = Define.EnumTreeNodeType.CheckType.ToString();
                                attributes[Define.EnumTreeAttribute.ToolTip] = checkType;
                                attributes[Define.EnumTreeAttribute.OrganizationId] = organizationId.ToString();
                                attributes[Define.EnumTreeAttribute.CheckType] = checkType;
                                attributes[Define.EnumTreeAttribute.CheckItemId] = string.Empty;

                                foreach (var attribute in attributes)
                                {
                                    treeItem.Attributes[attribute.Key.ToString()] = attribute.Value;
                                }

                                treeItem.State = "closed";

                                treeItemList.Add(treeItem);
                            }
                        }

                        var newOganizations = organizations.Where(x => x.ParentId == organizationId && account.VisibleOrganizationIds.Contains(x.OrganizationId)).OrderBy(x => x.OId).ToList();

                        foreach (var organization in newOganizations)
                        {
                            var treeItem = new TreeItem() { Title = organization.Name };

                            attributes[Define.EnumTreeAttribute.NodeType] = Define.EnumTreeNodeType.Organization.ToString();
                            attributes[Define.EnumTreeAttribute.ToolTip] = string.Format("{0}/{1}", organization.OId, organization.Name);
                            attributes[Define.EnumTreeAttribute.OrganizationId] = organization.OrganizationId.ToString();
                            attributes[Define.EnumTreeAttribute.CheckType] = string.Empty;
                            attributes[Define.EnumTreeAttribute.CheckItemId] = string.Empty;

                            foreach (var attribute in attributes)
                            {
                                treeItem.Attributes[attribute.Key.ToString()] = attribute.Value;
                            }

                            if (organizations.Any(x => x.ParentId == organization.OrganizationId && account.VisibleOrganizationIds.Contains(x.OrganizationId))
                                ||
                                (account.QueryableOrganizationIds.Contains(organization.OrganizationId) && context.CheckItems.Any(x => x.OrganizationId == organization.OrganizationId)))
                            {
                                treeItem.State = "closed";
                            }

                            treeItemList.Add(treeItem);
                        }
                    }
                    else
                    {
                        var checkItems = context.CheckItems.Where(x => x.OrganizationId == organizationId && x.Type == type).OrderBy(x => x.CIId).ToList();

                        foreach (var checkItem in checkItems)
                        {
                            var treeItem = new TreeItem() { Title = checkItem.Name };

                            attributes[Define.EnumTreeAttribute.NodeType] = Define.EnumTreeNodeType.CheckItem.ToString();
                            attributes[Define.EnumTreeAttribute.ToolTip] = string.Format("{0}/{1}", checkItem.CIId, checkItem.Name);
                            attributes[Define.EnumTreeAttribute.OrganizationId] = checkItem.OrganizationId.ToString();
                            attributes[Define.EnumTreeAttribute.CheckType] = checkItem.Type;
                            attributes[Define.EnumTreeAttribute.CheckItemId] = checkItem.CheckItemId.ToString();

                            foreach (var attribute in attributes)
                            {
                                treeItem.Attributes[attribute.Key.ToString()] = attribute.Value;
                            }

                            treeItemList.Add(treeItem);
                        }
                    }
                }

                result.ReturnData(treeItemList);
            }
            catch (Exception ex)
            {
                var err = new Error(MethodBase.GetCurrentMethod(), ex);

                Logger.Log(err);

                result.ReturnError(err);
            }

            return result;
        }

        public static RequestResult GetRootTreeItems(List<Models.Shared.Organization> organizations, Guid organizationId, Account account)
        {
            RequestResult result = new RequestResult();

            try
            {
                var treeItemList = new List<TreeItem>();

                var attributes = new Dictionary<Define.EnumTreeAttribute, string>()
                {
                    { Define.EnumTreeAttribute.NodeType, string.Empty },
                    { Define.EnumTreeAttribute.ToolTip, string.Empty },
                    { Define.EnumTreeAttribute.OrganizationId, string.Empty },
                    { Define.EnumTreeAttribute.CheckType, string.Empty },
                    { Define.EnumTreeAttribute.CheckItemId, string.Empty }
                };

                using (CFContext context = new CFContext())
                {
                    var organization = organizations.First(x => x.OrganizationId == organizationId);

                    var treeItem = new TreeItem() { Title = organization.Name };

                    attributes[Define.EnumTreeAttribute.NodeType] = Define.EnumTreeNodeType.Organization.ToString();
                    attributes[Define.EnumTreeAttribute.ToolTip] = string.Format("{0}/{1}", organization.OId, organization.Name);
                    attributes[Define.EnumTreeAttribute.OrganizationId] = organization.OrganizationId.ToString();
                    attributes[Define.EnumTreeAttribute.CheckType] = string.Empty;
                    attributes[Define.EnumTreeAttribute.CheckItemId] = string.Empty;

                    foreach (var attribute in attributes)
                    {
                        treeItem.Attributes[attribute.Key.ToString()] = attribute.Value;
                    }

                    if (organizations.Any(x => x.ParentId == organization.OrganizationId && account.VisibleOrganizationIds.Contains(x.OrganizationId))
                        ||
                        (account.QueryableOrganizationIds.Contains(organization.OrganizationId) && context.CheckItems.Any(x => x.OrganizationId == organization.OrganizationId)))
                    {
                        treeItem.State = "closed";
                    }

                    treeItemList.Add(treeItem);
                }

                result.ReturnData(treeItemList);
            }
            catch (Exception ex)
            {
                var err = new Error(MethodBase.GetCurrentMethod(), ex);

                Logger.Log(err);

                result.ReturnError(err);
            }

            return result;
        }

        public static RequestResult GetTreeItems(List<Models.Shared.Organization> organizations, Guid refOrganizationId, Guid organizationId, string type)
        {
            RequestResult result = new RequestResult();

            try
            {
                var treeItemList = new List<TreeItem>();

                var attributes = new Dictionary<Define.EnumTreeAttribute, string>()
                {
                    { Define.EnumTreeAttribute.NodeType, string.Empty },
                    { Define.EnumTreeAttribute.ToolTip, string.Empty },
                    { Define.EnumTreeAttribute.OrganizationId, string.Empty },
                    { Define.EnumTreeAttribute.CheckType, string.Empty },
                    { Define.EnumTreeAttribute.CheckItemId, string.Empty }
                };

                using (CFContext context = new CFContext())
                {
                    if (string.IsNullOrEmpty(type))
                    {
                        var checkTypes = context.CheckItems.Where(x => x.OrganizationId == organizationId).Select(x => x.Type).Distinct().OrderBy(x => x).ToList();

                        foreach (var checkType in checkTypes)
                        {
                            var treeItem = new TreeItem() { Title = checkType };

                            attributes[Define.EnumTreeAttribute.NodeType] = Define.EnumTreeNodeType.CheckType.ToString();
                            attributes[Define.EnumTreeAttribute.ToolTip] = checkType;
                            attributes[Define.EnumTreeAttribute.OrganizationId] = organizationId.ToString();
                            attributes[Define.EnumTreeAttribute.CheckType] = checkType;
                            attributes[Define.EnumTreeAttribute.CheckItemId] = string.Empty;

                            foreach (var attribute in attributes)
                            {
                                treeItem.Attributes[attribute.Key.ToString()] = attribute.Value;
                            }

                            treeItem.State = "closed";

                            treeItemList.Add(treeItem);
                        }

                        var availableOrganizationIds = OrganizationDataAccessor.GetUpStreamOrganizationIds(refOrganizationId, true);

                        var newOrganizations = organizations.Where(x => x.ParentId == organizationId && availableOrganizationIds.Contains(x.OrganizationId)).OrderBy(x => x.OId).ToList();

                        foreach (var organization in newOrganizations)
                        {
                            var downStreamOrganizationIds = OrganizationDataAccessor.GetDownStreamOrganizationIds(organization.OrganizationId, true);

                            if (context.CheckItems.Any(x => downStreamOrganizationIds.Contains(x.OrganizationId) && availableOrganizationIds.Contains(x.OrganizationId)))
                            {
                                var treeItem = new TreeItem() { Title = organization.Name };

                                attributes[Define.EnumTreeAttribute.NodeType] = Define.EnumTreeNodeType.Organization.ToString();
                                attributes[Define.EnumTreeAttribute.ToolTip] = string.Format("{0}/{1}", organization.OId, organization.Name);
                                attributes[Define.EnumTreeAttribute.OrganizationId] = organization.OrganizationId.ToString();
                                attributes[Define.EnumTreeAttribute.CheckType] = string.Empty;
                                attributes[Define.EnumTreeAttribute.CheckItemId] = string.Empty;

                                foreach (var attribute in attributes)
                                {
                                    treeItem.Attributes[attribute.Key.ToString()] = attribute.Value;
                                }

                                treeItem.State = "closed";

                                treeItemList.Add(treeItem);
                            }
                        }
                    }
                    else
                    {
                        var checkItems = context.CheckItems.Where(x => x.OrganizationId == organizationId && x.Type == type).OrderBy(x => x.CIId).ToList();

                        foreach (var checkItem in checkItems)
                        {
                            var treeItem = new TreeItem() { Title = checkItem.Name };

                            attributes[Define.EnumTreeAttribute.NodeType] = Define.EnumTreeNodeType.CheckItem.ToString();
                            attributes[Define.EnumTreeAttribute.ToolTip] = string.Format("{0}/{1}", checkItem.CIId, checkItem.Name);
                            attributes[Define.EnumTreeAttribute.OrganizationId] = checkItem.OrganizationId.ToString();
                            attributes[Define.EnumTreeAttribute.CheckType] = checkItem.Type;
                            attributes[Define.EnumTreeAttribute.CheckItemId] = checkItem.CheckItemId.ToString();

                            foreach (var attribute in attributes)
                            {
                                treeItem.Attributes[attribute.Key.ToString()] = attribute.Value;
                            }

                            treeItemList.Add(treeItem);
                        }
                    }
                }

                result.ReturnData(treeItemList);
            }
            catch (Exception ex)
            {
                var err = new Error(MethodBase.GetCurrentMethod(), ex);

                Logger.Log(err);

                result.ReturnError(err);
            }

            return result;
        }
    }
}
