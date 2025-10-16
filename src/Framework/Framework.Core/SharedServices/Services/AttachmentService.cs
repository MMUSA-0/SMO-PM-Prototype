using Framework.Core.Data.Repositories;
using Framework.Core.Extensions;
using Framework.Core.Globalization;
using Framework.Core.Helpers;
using Framework.Core.SharedServices.Dto;
using Framework.Core.SharedServices.Entities;
using Framework.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Framework.Core.SharedServices.Services
{
    public class AttachmentService
    {
        protected readonly IRepositoryBase<ICommonsDbContext, Attachment> _attachmentRepository;
        protected readonly IRepositoryBase<ICommonsDbContext, AttachmentType> _attachmentTypeRepository;
        protected readonly AppSettingsService _appSettingsService;

        public AttachmentService(IRepositoryBase<ICommonsDbContext, Attachment> attachmentRepository,
            IRepositoryBase<ICommonsDbContext, AttachmentType> attachmentTypeRepository,
        AppSettingsService appSettingsService)
        {
            _attachmentRepository = attachmentRepository;
            _attachmentTypeRepository = attachmentTypeRepository;
            _appSettingsService = appSettingsService;
        }


        public async Task<ReturnResult<Attachment>> AddAttachment(IFormFile file, string title = null,
            string contentType = null, AttachmentTypes attachType = AttachmentTypes.GeneralFileAttachment)
        {
            var result = new ReturnResult<Attachment>();

            var isAcceptedFileExtention = await IsAcceptedFileExtentionAndSize(file, attachType);
            result.Merge(isAcceptedFileExtention);

            if (isAcceptedFileExtention.IsValid)
            {
                if (!IsValidMimeType(file))
                {
                    result.AddErrorItem(string.Empty, SharedResources.FileContentErrorMessage);
                    return result;
                }
                var attResult = this.AddOrUpdateAttachment(file, attachType, null, title, contentType);
                if (!attResult.IsValid)
                {
                    result.Merge(attResult);
                    return result;
                }

                result.Value = attResult.Value;


            }

            return result;
        }

        public static bool IsValidMimeType(IFormFile file)
        {
            return AttachmentMimeType.IsAllowed(file.ContentType.ToLower());
        }

        private async Task<ReturnResult> IsAcceptedFileExtentionAndSize(IFormFile file, AttachmentTypes attachType)
        {
            var result = new ReturnResult();
            var fileExtention = Path.GetExtension(file.FileName);
            var attachmentType = await _attachmentTypeRepository.GetByIdAsync((int)attachType);

            if (string.IsNullOrEmpty(fileExtention) || attachmentType == null)
            {
                result.AddErrorItem(string.Empty, SharedResources.UploadAttachmentError);
                return result;
            }

            var maxSizeAllowed = attachmentType.MaxSizeInMegabytes * 1024 * 1024;
            var acceptedExtesions = attachmentType.AllowedFilesExtension.Trim().ToLower().Split(',');

            var isAllowdType = acceptedExtesions.Contains(fileExtention);
            var isAllowdSize = maxSizeAllowed > file.Length;

            if (!isAllowdSize)
                result.AddErrorItem(string.Empty, SharedResources.FileTypeErrorMessage
                .Replace("{0}", attachmentType.MaxSizeInMegabytes.ToString()));

            if (!isAllowdType)
                result.AddErrorItem(string.Empty, SharedResources.FileTypeErrorMessage
                .Replace("{0}", attachmentType.AllowedFilesExtension.ToLower()));

            return result;
        }


        public ReturnResult<Attachment> AddOrUpdateAttachment(
            IFormFile file,
            AttachmentTypes attType,
            Guid? attachmentId = null,
            string title = null, string contentType = null)
        {
            var result = new ReturnResult<Attachment>();
            if (file == null)
            {
                result.AddErrorItem(string.Empty, SharedResources.FileZeroLengthErrorMessage);
                return result;
            }

            if (file.Length <= 0)
            {
                result.AddErrorItem(string.Empty, SharedResources.FileZeroLengthErrorMessage);
                return result;
            }

            if (!_appSettingsService.SaveFilesToDatabase && string.IsNullOrEmpty(_appSettingsService.AttachmentsPath))
            {
                throw new Exception(
                    "File can not be saved. Current Settings is. SaveFileToDatabase=true and Attachment Path is Missing");
            }

            using (var memoryStream = new MemoryStream())
            {
                file.OpenReadStream().CopyTo(memoryStream);
                result.Value = this.AddOrUpdateAttachment(
                    file.FileName,
                    contentType ?? file.ContentType,
                    memoryStream.ToArray(),
                    attType,
                    attachmentId,
                    title,
                    title);
            }

            return result;
        }

        public Attachment AddOrUpdateAttachment(
            string fileName,
            string contentType,
            byte[] fileBytes,
            AttachmentTypes attTypeId,
            Guid? attachmentId = null,
            string titleAr = null,
            string titleEn = null,
            string descriptionAr = null,
            string descriptionEn = null,
            int? itemOrder = null)
        {
            var isUpdateFile = attachmentId.HasValue && attachmentId.Value != Guid.Empty;

            var attachment = isUpdateFile
                                 ? _attachmentRepository.GetById(attachmentId.Value)
                                 : new Attachment { Id = Guid.NewGuid().AsSequentialGuid() };

            if (attachment == null)
            {
                throw new Exception("The Attachment File You are trying to update Does Not Exist in the database");
            }

            if (_appSettingsService.SaveFilesToDatabase && fileBytes.Length > 0)
            {
                attachment.FileContent = fileBytes;
            }

            attachment.TitleAr = titleAr;
            attachment.TitleEn = titleEn;
            attachment.DescriptionAr = descriptionAr ?? attachment.DescriptionAr;
            attachment.DescriptionEn = descriptionEn ?? attachment.DescriptionEn;
            attachment.ContentType = contentType;
            attachment.Extension = new FileInfo(fileName).Extension;
            attachment.FileName = fileName;
            attachment.AttachmentTypeId = (int)attTypeId;

            if (contentType.StartsWith("image/"))
                attachment.Thumbnail = this.GenerateThumbnail(fileBytes);

            // in updating delete old file
            if (isUpdateFile)
            {
                this.DeleteAttachmentFromFileSystem(attachment.FilePath);
            }

            attachment.FilePath = _appSettingsService.SaveFilesToDatabase
                                      ? null
                                      : this.SaveAttachmentToFileSystem(attachment, fileBytes);
            //attachment.Id = attachment.Id;
            attachment.FileContent = _appSettingsService.SaveFilesToDatabase ? fileBytes : null;

            if (!isUpdateFile)
            {
                _attachmentRepository.Insert(attachment, true);
            }

            return attachment;
        }

        public async Task<Attachment?> GetAttachmentAsync(Guid attachmentId)
        {
            var attachment = await _attachmentRepository
                .TableNoTracking.FirstOrDefaultAsync(at => at.Id == attachmentId);
            return attachment;
        }

        public Attachment GetAttachment(Guid attachmentId)
        {
            var attachment = _attachmentRepository
                .TableNoTracking.FirstOrDefault(at => at.Id == attachmentId);
            return attachment;
        }

        public async Task<List<Attachment>> GetAttachments(List<Guid> attachmentIds)
        {
            var attachments = await _attachmentRepository
                .TableNoTracking.Where(at => attachmentIds.Contains(at.Id)).ToListAsync();
            return attachments;
        }

        public async Task<ReturnResult<Attachment>> GetDownloadableAttachmentAsync(Guid attachmentId)
        {
            var result = new ReturnResult<Attachment>();
            var attachment = await GetAttachmentForDownloadAsync(attachmentId);

            if (attachment == null)
                result.AddErrorItem(string.Empty, SharedResources.ItemNotFoundError);

            result.Value = attachment;
            return result;
        }

        public async Task<byte[]> GetAttachmentContentAsync(Guid attachmentId)
        {
            return (await GetAttachmentForDownloadAsync(attachmentId)).FileContent ?? null;
        }

        public byte[] GetAttachmentContent(Guid attachmentId)
        {
            return (GetAttachmentForDownload(attachmentId)).FileContent ?? null;
        }

        private async Task<Attachment> GetAttachmentForDownloadAsync(Guid? attachmentId)
        {
            var attachment = await _attachmentRepository.TableNoTracking
                .Where(at => at.Id == attachmentId)
                .SingleOrDefaultAsync();

            if (_appSettingsService.SaveFilesToDatabase && attachment?.FileContent != null)
            {
                return attachment;
            }

            if (string.IsNullOrEmpty(_appSettingsService.AttachmentsPath)
                || string.IsNullOrEmpty(attachment?.FilePath))
            {
                return attachment;
            }

            var filePath = attachment.IsTransferred ?
                $"{_appSettingsService.StructuredAttachmentPath}{attachment.FilePath}"
                : $"{_appSettingsService.AttachmentsPath}{attachment.FilePath}";

            if (File.Exists(filePath)) attachment.FileContent = File.ReadAllBytes(filePath);

            return attachment;
        }

        public Attachment GetAttachmentForDownload(Guid? attachmentId)
        {
            var attachment = _attachmentRepository.TableNoTracking.Where(at => at.Id == attachmentId).SingleOrDefault();


            if (string.IsNullOrEmpty(_appSettingsService.AttachmentsPath)
                || string.IsNullOrEmpty(attachment?.FilePath))
            {
                return attachment;
            }

            var filePath = $"{_appSettingsService.AttachmentsPath}{attachment.FilePath}";
            if (File.Exists(filePath))
            {
                attachment.FileContent = File.ReadAllBytes(filePath);
            }

            return attachment;
        }

        public async Task<byte[]> GetAttachmentIMGThumbnailAsync(Guid? attachmentId)
        {
            return await _attachmentRepository.TableNoTracking.Where(at => at.Id == attachmentId).Select(at => at.Thumbnail)
                 .AsNoTracking().SingleOrDefaultAsync();
        }
        public byte[] GetAttachmentIMGThumbnail(Guid? attachmentId)
        {
            return _attachmentRepository.TableNoTracking.Where(at => at.Id == attachmentId).Select(at => at.Thumbnail)
                 .AsNoTracking().SingleOrDefault();
        }

        public void RemoveRange(List<Guid> deleteIds)
        {
            this._attachmentRepository.Delete(a => deleteIds.Contains(a.Id), true);
        }

        public async Task<bool> RemoveAsync(Guid id)
        {
            return await this._attachmentRepository.DeleteAsync(a => a.Id == id, true);
        }

        public async Task<bool> UpdateAttachmentsTitlesAsync(List<Guid> ids, List<string> titles)
        {
            if (ids.IsNullOrEmpty())
                return false;

            var attachments = await _attachmentRepository.Table
                              .Where(a => ids.Contains(a.Id))
                              .OrderBy(a => a.Id)
                              .ToListAsync();

            if (attachments.Count != ids.Count)
                return false;

            for (int i = 0; i < ids.Count; i++)
            {
                for (int j = 0; j < attachments.Count; j++)
                {
                    Attachment attachment = attachments[j];
                    if (ids[i] == attachment.Id)
                    {
                        attachment.TitleEn = titles[i];
                        await _attachmentRepository.UpdateAsync(attachment, true);
                    }
                }
            }

            return true;
        }

        public void DeleteAttachmentFromFileSystem(string fileRelativePath)
        {
            if (string.IsNullOrEmpty(fileRelativePath))
            {
                return;
            }

            var filePath = $@"{_appSettingsService.AttachmentsPath}{fileRelativePath}";
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        public async Task DeleteAttachmentFromDbAndFileSystem(Guid attachmentId)
        {
            if (attachmentId == Guid.Empty)
            {
                return;
            }
            var attachment = _attachmentRepository.TableNoTracking.FirstOrDefault(a => a.Id == attachmentId);
            if (attachment != null)
            {
                var filePath = $@"{_appSettingsService.AttachmentsPath}{attachment.FilePath}";
                if (File.Exists(filePath))
                {
                    //remove from file system
                    File.Delete(filePath);
                }
                //remove from db
                await RemoveAsync(attachmentId);
            }
        }

        private byte[] GenerateThumbnail(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            {
                var thumb = new Bitmap(220, 220);
                using (var bmp = Image.FromStream(ms))
                {
                    using (var g = Graphics.FromImage(thumb))
                    {
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.CompositingQuality = CompositingQuality.HighQuality;
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.DrawImage(bmp, 0, 0, 220, 220);
                    }
                }

                using (var msWrite = new MemoryStream())
                {
                    thumb.Save(msWrite, ImageFormat.Png);
                    return msWrite.ToArray();
                }
            }
        }

        public byte[] GetThumbnailOfFile(string filePath)
        {
            var bytes = File.ReadAllBytes(filePath);
            var thumbnail = GenerateThumbnail(bytes);
            return thumbnail;
        }

        private string SaveAttachmentToFileSystem(Attachment attach, byte[] fileBytes)
        {
            var relativeFolderPath = $"\\{DateTime.Now.Year}\\{DateTime.Now.Month}\\{DateTime.Now.Day}";
            var fullFolderPath = $"{_appSettingsService.AttachmentsPath}{relativeFolderPath}";
            var attachmentIdFormated = attach.Id.ToString("N").ToLower();
            var attachmentName = attach.FileName.ToLower().Trim().RemoveFromEnd(attach.Extension);
            var fileName = attachmentName + "_" + attachmentIdFormated[(attachmentIdFormated.Length - 4)..] + attach.Extension;
            var fileRelativePath = $@"{relativeFolderPath}\{fileName}";

            if (!Directory.Exists(fullFolderPath))
            {
                Directory.CreateDirectory(fullFolderPath);
            }
            using (var bw = new BinaryWriter(File.Open(Path.Combine(fullFolderPath, fileName), FileMode.OpenOrCreate)))
            {
                bw.Write(fileBytes);
            }
            return fileRelativePath;
        }

        public Attachment Insert(Attachment attachment)
        {
            return _attachmentRepository.Insert(attachment, true);
        }

        public async Task UpdateAsync(Attachment attachment)
        {
            await _attachmentRepository.UpdateAsync(attachment, true);
        }

        public async Task<List<AttachmentDto>>
            GetExternalAttachments(List<Guid> attachmentIds, bool isGalleryAttachment = false, int? maxAttachmentListCount = null, bool loadFileData = false)
        {
            var attachments = await GetAttachments(attachmentIds);
            if (!attachments.Any()) return new List<AttachmentDto>();

            var externalAttachments = new List<AttachmentDto>();
            foreach (var item in attachments)
            {
                if (isGalleryAttachment && !item.ContentType.Contains("image")) continue;

                if (maxAttachmentListCount != null && externalAttachments.Count == maxAttachmentListCount) break;
                externalAttachments.Add(new AttachmentDto
                {
                    AttachmentId = item.Id,
                    FileName = item.FileName,
                    ContentType = item.ContentType,
                    FileData = isGalleryAttachment || loadFileData ? (await GetAttachmentForDownloadAsync(item.Id)).FileContent : item.Thumbnail,
                    FilePath = item.FilePath
                });

            }
            return externalAttachments;
        }


        public async Task InsertRangeAsync(List<Attachment> attachments)
        {
            await _attachmentRepository.InsertRangeAsync(attachments, true);
        }

        public async Task InsertAsync(Attachment attachment)
        {
            await _attachmentRepository.InsertAsync(attachment, true);
        }

        public ReturnResult<AttachmentType> GetAttachmentType(int id)
        {
            var result = new ReturnResult<AttachmentType>();
            if (id <= 0)
            {
                result.AddErrorItem(string.Empty, SharedResources.InvalidRequestParametersError);
                return result;
            }

            var attachmentType = _attachmentTypeRepository.TableNoTracking.FirstOrDefault(at => at.Id == id);

            if (attachmentType == null)
            {
                result.AddErrorItem(string.Empty, SharedResources.ItemNotFoundError);
                return result;
            }

            result.Value = attachmentType;
            return result;
        }

        

    }
}
