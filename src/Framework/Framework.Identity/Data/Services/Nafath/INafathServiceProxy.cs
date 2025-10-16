using Framework.Identity.Data.Services.Nafath.Dto;
using Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Identity.Data.Services.Nafath
{
    public interface INafathServiceProxy
    {
        Task<ApiResponse<SendRequestResponseDto>> SendRequest(string UserIdentity);
        Task<ApiResponse<CheckRequestStatusResponseDto>> CheckRequestStatus(CheckRequestStatusDto CheckRequestStatus);
    }
}
