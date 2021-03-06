﻿using DanceClass.Utils;
using Services.Members;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace DanceClass.Api
{
    [RoutePrefix("api/user")]
    public class UserController : ApiBaseController
    {
        private readonly IMemberService _memberService;
        public UserController(IMemberService memberService)
        {
            _memberService = memberService;
        }

        [HttpPost]
        [HierarchicalAuthorize(AuthorizationLevel = AuthorizationLevel.ReceptionistAndHigher)]
        [Route("create")]
        public async Task<IHttpActionResult> Create(CreateMemberRq rq)
        {
            CreateMemberRs rs = await _memberService.Create(rq);
            return ApiJson(rs);
        }

        [HttpPost]
        [Authorize]
        [Route("get")]
        public async Task<IHttpActionResult> Get(GetMemberRq rq)
        {
            GetMemberRs rs = await _memberService.Get(rq);
            return ApiJson(rs);
        }

        [HttpPost]
        [Route("getcurrentuser")]
        public async Task<IHttpActionResult> GetCurrentUser()
        {
            GetMemberRs rs = await _memberService.GetCurrentUser();
            return ApiJson(rs);
        }

        [HttpPost]
        [HierarchicalAuthorize(AuthorizationLevel = AuthorizationLevel.ReceptionistAndHigher)]
        [Route("searchMember")]
        public async Task<IHttpActionResult> SearchMember(SearchMemberRq rq)
        {
            SearchMemberRs rs = await _memberService.Search(rq);
            return ApiJson(rs);
        }

        [HttpPost]
        [HierarchicalAuthorize(AuthorizationLevel = AuthorizationLevel.CollaboratorAndHigher)]
        [Route("getAllMember")]
        public async Task<IHttpActionResult> GetAllMember(GetAllMemberRq rq)
        {
            GetAllMemberRs rs = await _memberService.GetAll(rq);
            return ApiJson(rs);
        }

        [HttpPost]
        [HierarchicalAuthorize(AuthorizationLevel = AuthorizationLevel.AdminOnly)]
        [Route("edit")]
        public async Task<IHttpActionResult> Edit(EditMemberRq rq)
        {
            if (!ModelState.IsValid)
            {
                throw new Exception("Invalid request body");
            }

            EditMemberRs rs = await _memberService.Edit(rq);
            return ApiJson(rs);
        }

        [HttpDelete]
        [HierarchicalAuthorize(AuthorizationLevel = AuthorizationLevel.AdminOnly)]
        [Route("delete/{id}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            DeleteMemberRs rs = await _memberService.Delete(id);
            return ApiJson(rs);
        }
    }
}
