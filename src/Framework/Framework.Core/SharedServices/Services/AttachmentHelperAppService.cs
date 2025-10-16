using Framework.Core.Extensions;
using Framework.Core.SharedServices.Entities;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Framework.Core.SharedServices.Services
{
    public class AttachmentHelperAppService
    {
        private readonly AttachmentService _attachmentService;
        public AttachmentHelperAppService(AttachmentService attachmentService)
        {
            _attachmentService = attachmentService;
        }

        public async Task<ReturnResult<List<Attachment>>> InsertAttachments(List<IFormFile> files, List<string> titles = null)
        {
            var result = new ReturnResult<List<Attachment>> { Value = new List<Attachment>() };
            for (int i = 0; i < files.Count; i++)
            {
                bool hasNoTitles = titles.IsNullOrEmpty() || titles.Count != files.Count;

                var attachResult = hasNoTitles ? await _attachmentService.AddAttachment(files[i]) :
                                   await _attachmentService.AddAttachment(files[i], titles[i]);

                if (!attachResult.IsValid)
                {
                    result.Merge(attachResult);
                    return result;
                }
                result.Value.Add(attachResult.Value);
            }

            return result;
        }
    }
}
