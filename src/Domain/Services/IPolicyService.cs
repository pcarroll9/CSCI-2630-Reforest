﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EDeviceClaims.Domain.Models;
using EDeviceClaims.Interactors;

namespace EDeviceClaims.Domain.Services
{
    public interface IPolicyService
    {
        IEnumerable<PolicyWithClaimsDomainModel> GetByUserId(string userId);

        PolicyWithClaimsDomainModel GetById(Guid id);
        void AssociateExistingDevices(string userId);
    }

    public class PolicyService : IPolicyService
    {
        private IGetPolicyInteractor _getPolicyInteractor;

        private IGetPolicyInteractor GetPolicyInteractor
        {
            get { return _getPolicyInteractor ?? (_getPolicyInteractor = new GetPolicyInteractor()); }
            set { _getPolicyInteractor = value; }
        }

        private IGetUserInteractor _getUserInteractor;

        private IGetUserInteractor GetUserInteractor
        {
            get { return _getUserInteractor ?? (_getUserInteractor = new GetUserInteractor()); }
            set { _getUserInteractor = value; }
        }

        public IEnumerable<PolicyWithClaimsDomainModel> GetByUserId(string userId)
        {
            var policyEntities = GetPolicyInteractor.GetByUserId(userId);

            return policyEntities.Select(policyEntity => new PolicyWithClaimsDomainModel(policyEntity)).ToList();
        }

        public PolicyWithClaimsDomainModel GetById(Guid id)
        {
            var entity = GetPolicyInteractor.GetById(id);
            if (entity == null) return null;
            return new PolicyWithClaimsDomainModel(entity);
        }

        public void AssociateExistingDevices(string userId)
        {
            var user = GetUserInteractor.GetById(userId);
            if (user == null) throw new Exception();
            var devices = GetPolicyInteractor.GetByCustomerEmailAdress(user.Email);
            foreach (var device in devices)
            {
                device.UserId = user.Id;
            }

            GetPolicyInteractor.Repo.EfUnitOfWork.Context.SaveChanges();
        }
    }
}
