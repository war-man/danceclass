﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using DataAccess;
using DataAccess.Enums;
using Microsoft.AspNet.Identity;
using Services.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Services.Registration
{
    public interface IRegistrationService
    {
        Task<int> Cancel(int id);
        Task<CreateRegistrationRs> Create(CreateRegistrationRq rq);
        Task<List<RegistrationDTO>> GetByScheduleDetail(int scheduleDetailId);
        Task ConfirmAttendance(int scheduleDetailId);
    }

    public class RegistrationService : BaseService, IRegistrationService
    {
        private readonly IConfigurationProvider _mappingConfig;

        public RegistrationService(DanceClassDbContext dbContext, IMapper mapper, IConfigurationProvider mappingConfig) : base(dbContext, mapper)
        {
            _mappingConfig = mappingConfig;
        }

        public async Task<int> Cancel(int id)
        {
            DataAccess.Entities.Registration registration = await _dbContext.Registrations.FirstOrDefaultAsync(r => r.Id == id);
            if (registration == null)
            {
                throw new Exception("Đăng ký không tồn tại!");
            }

            var currentUserId = HttpContext.Current.User.Identity.GetUserId();
            if (registration.UserId.ToString() != currentUserId && !HttpContext.Current.User.IsInRole("Admin"))
            {
                throw new Exception("Không đủ quyền hủy đăng ký!");
            }

            var scheduleDetail = registration.ScheduleDetail;
            if (scheduleDetail == null)
            {
                throw new Exception("Lịch học không tồn tại!");
            }

            //DateTime dateAttending = scheduleDetail.Date.Add(scheduleDetail.Schedule.StartTime);

            // TODO: ADMIN CAN CANCEL

            //if (DateTime.Now >= dateAttending || (dateAttending - DateTime.Now).TotalHours < 1)
            //{
            //    throw new Exception("Chỉ có thể hủy đăng ký ít nhất 1 tiếng trước khi tập!");
            //}
            
            // Remove registration
            _dbContext.Registrations.Remove(registration);

            // Increase remaining sessions of the user
            var memberPackage = _dbContext.MemberPackages.FirstOrDefault(x => x.UserId.ToString() == currentUserId && x.IsActive);
            if (memberPackage.RemainingSessions >= memberPackage.Package.NumberOfSessions)
            {
                throw new Exception("Không thể hủy. Bạn vẫn chưa đăng ký buổi học nào mà!");
            }

            memberPackage.RemainingSessions++;

            return await _dbContext.SaveChangesAsync();
        }

        public async Task<CreateRegistrationRs> Create(CreateRegistrationRq rq)
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                throw new Exception("Vui lòng đăng nhập để đăng ký lớp học");
            }

            // Decrease remaining sessions of the user
            var memberPackage = _dbContext.MemberPackages.FirstOrDefault(x => x.UserId.ToString() == userId && x.IsActive);
            if (memberPackage.RemainingSessions <= 0)
            {
                throw new Exception("Bạn đã dùng hết gói tập hiện tại. Vui lòng đăng ký gói mới!");
            }

            memberPackage.RemainingSessions--;

            // Add registration
            DataAccess.Entities.Registration registration = _mapper.Map<DataAccess.Entities.Registration>(rq.Registration);
            //if (registration.ScheduleDetail.Date < DateTime.Now)
            //{
            //    throw new Exception("Không thể đăng ký lớp học trong quá khứ");
            //}

            registration.UserId = int.Parse(userId);
            registration.Status = RegistrationStatus.Registered;
            registration.DateRegistered = DateTime.Now;

            _dbContext.Registrations.Add(registration);

            await _dbContext.SaveChangesAsync();

            CreateRegistrationRs rs = new CreateRegistrationRs()
            {
                Registration = _mapper.Map<RegistrationDTO>(registration)
            };

            return rs;
        }

        public async Task<List<RegistrationDTO>> GetByScheduleDetail(int scheduleDetailId)
        {
            var registrations = await _dbContext.Registrations
                .Where(r => r.ScheduleDetailId == scheduleDetailId)
                .ProjectTo<RegistrationDTO>(_mappingConfig, dest => dest.User)
                .ToListAsync();

            return registrations;
        }

        public async Task ConfirmAttendance(int scheduleDetailId)
        {
            var registration = await _dbContext.Registrations.FirstOrDefaultAsync(r => r.Id == scheduleDetailId);
            if (registration == null)
            {
                throw new Exception("Đăng ký không tồn tại");
            }

            registration.Status = RegistrationStatus.Attended;
            await _dbContext.SaveChangesAsync();
        }
    }
}