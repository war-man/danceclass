﻿using Services.Members;
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
        [Authorize(Roles = "Admin")]
        [Route("create")]
        public async Task<IHttpActionResult> Create(CreateMemberRq rq)
        {
            CreateMemberRs rs = await _memberService.Create(rq);
            return ApiJson(rs);
        }

        [HttpPost]
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
    }
}
