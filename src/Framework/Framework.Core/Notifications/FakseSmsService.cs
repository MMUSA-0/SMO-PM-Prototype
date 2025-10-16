// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeSmsService.cs" company="Usama Nada">
//   No Copyright .. Copy, Share, and Evolve.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Text;
using System;
using Framework.Core.SharedServices.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;

namespace Framework.Core.Notifications
{
    /// <summary>
    ///     The default sms service.
    /// </summary>
    public class FakseSmsService //: ISmsService
    {
        private readonly HttpClient _client;
        private readonly IWebHostEnvironment _appEnvironment;
        private readonly AppSettingsService _appSettingsService;
        private readonly ILogger<FakseSmsService> _logger;

        public FakseSmsService(HttpClient client, IWebHostEnvironment appEnvironment, ILogger<FakseSmsService> logger, AppSettingsService appSettingsService)
        {
            _client = client;
            _appEnvironment = appEnvironment;
            _logger = logger;
            _appSettingsService = appSettingsService;
        }

        public  void SendSms(SmsMessage smsMessage)
        {
            
        }
    }
}