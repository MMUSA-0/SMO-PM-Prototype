using Framework.Core;
using Framework.Core.AutoMapper;
using Framework.Core.Extensions;
using Framework.Core.SharedServices.Services;
using Framework.Identity.Data.Dtos;
using Framework.Identity.Data.Entities;
using Framework.Identity.Data.Repositories;
using Framework.Identity.Data.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Identity.Data.Services
{
    public class UserOtpAppService
    {
        private readonly UserOtpRepository _userOtpRepository;
        private readonly AppSettingsService _appSettingsService;

        public UserOtpAppService(UserOtpRepository userOtpRepository, AppSettingsService appSettingsService)
        {
            _userOtpRepository = userOtpRepository;
            _appSettingsService = appSettingsService;
        }

        public async Task<UserOtpDto> GetUserOtpByUserId(Guid UserId)
        {
            var model = await _userOtpRepository.TableNoTracking.Where(x => x.UserId == UserId).OrderByDescending(a => a.CreatedOn).FirstOrDefaultAsync();
            return model.MapTo<UserOtpDto>();
        }

        public async Task<UserOtpDto> GetUserOtpByEmailOrMobile(EmailMobileVerificationDto emailMobileVerification)
        {
            var model = await _userOtpRepository.TableNoTracking.Where(x => (emailMobileVerification.Email.IsNotNullOrEmpty() && x.Email == emailMobileVerification.Email) ||
                        (emailMobileVerification.Mobile.IsNotNullOrEmpty() && x.Mobile == emailMobileVerification.Mobile))
                        .OrderByDescending(a => a.CreatedOn).FirstOrDefaultAsync();
            return model.MapTo<UserOtpDto>();
        }

        public async Task DeleteUserOtp(Guid? UserId, EmailMobileVerificationDto emailMobileVerification = null)
        {
            List<UserOtp> OtpList = new();
            if (UserId != Guid.Empty)
                OtpList = await _userOtpRepository.TableNoTracking.Where(x => x.UserId == UserId).OrderByDescending(a => a.CreatedOn).ToListAsync();
            else
            {
                OtpList = await _userOtpRepository.TableNoTracking.Where(x => (emailMobileVerification.Email.IsNotNullOrEmpty() && x.Email == emailMobileVerification.Email) ||
                        (emailMobileVerification.Mobile.IsNotNullOrEmpty() && x.Mobile == emailMobileVerification.Mobile))
                        .OrderByDescending(a => a.CreatedOn).ToListAsync();
            }

            if (OtpList.Count > 0)
                _userOtpRepository.DeleteRange(OtpList, true);
        }
        public async Task<bool> SaveAsync(UserOtpDto userOtpDto, bool autoSave = false)
        {
            try
            {
                if (userOtpDto != null)
                {
                    var mapped = userOtpDto.MapTo<UserOtp>();
                    await _userOtpRepository.InsertAsync(mapped, autoSave);

                    return true;
                }
                return false;
            }

            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<bool> IsUserOTPLimitValidToGenerateNew(Guid? userId = null, EmailMobileVerificationDto model = null)
        {
            List<UserOtp> otpList = new List<UserOtp>();
            if (userId.HasValue && userId != Guid.Empty)
            {
                otpList = await _userOtpRepository.TableNoTracking
                    .Where(x => x.UserId == userId && x.CreatedOn.Date == DateTime.Now.Date)
                    .ToListAsync();
            }
            else
            {
                otpList = await _userOtpRepository.TableNoTracking
                    .Where(x =>
                        ((model.Email.IsNotNullOrEmpty() && x.Email == model.Email) ||
                        (model.Mobile.IsNotNullOrEmpty() && x.Mobile == model.Mobile))
                        && x.CreatedOn.Date == DateTime.Now.Date)
                    .ToListAsync();
            }

            if (!otpList.Any()) return true;

            return _appSettingsService.OTPResendTimes > otpList.Count;
        }

        public async Task<bool> IsUserOTPDurationValidToGenerateNew(Guid? userId = null, EmailMobileVerificationDto model = null)
        {
            UserOtp userOtp = null;
            if (userId.HasValue && userId != Guid.Empty)
            {
                userOtp = await _userOtpRepository.TableNoTracking.Where(x => x.UserId == userId)
               .OrderByDescending(a => a.CreatedOn).FirstOrDefaultAsync();
            }
            else
            {
                userOtp = await _userOtpRepository.TableNoTracking
                .Where(x => (model.Email.IsNotNullOrEmpty() && x.Email == model.Email) ||
                        (model.Mobile.IsNotNullOrEmpty() && x.Mobile == model.Mobile))
                        .OrderByDescending(a => a.CreatedOn).FirstOrDefaultAsync();
            }

            if (userOtp == null) return true;

            return !DateTime.Now.Between(userOtp.CreatedOn, userOtp.CreatedOn.AddMinutes(_appSettingsService.OTPResendDuration));
        }

        public async Task<string> GenerateVerificationCode(Guid? UserId, EmailMobileVerificationDto VerificationObj = null)
        {
            int generatedCode = CommonHelper.GenerateRandomInteger(0, 1000000);
            string code = generatedCode.ToString("D6");

            var CodeToArr = code.ToCharArray();
            if (CodeToArr[0] == '0')
            {
                CodeToArr[0] = '1';
            }
            code = string.Join("", CodeToArr);

            var userOtpDto = new UserOtpDto()
            {
                UserId = UserId == Guid.Empty ? null : UserId,
                Email = (VerificationObj != null && VerificationObj.Email.IsNotNullOrEmpty()) ? VerificationObj.Email : null,
                Mobile = (VerificationObj != null && VerificationObj.Mobile.IsNotNullOrEmpty()) ? VerificationObj.Mobile : null,
                Otp = code,
                CreatedBy = "System"
            };
            var Inserted = await SaveAsync(userOtpDto, true);

            return Inserted ? code : null;
        }


    }
}

