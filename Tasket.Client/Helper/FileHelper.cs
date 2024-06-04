using Microsoft.AspNetCore.Components.Forms;

namespace Tasket.Client.Components.Helper
{
    public static class FileHelper
    {
        public static readonly string DefaultCompanyImage = "/img/DefaultCompanyImgWithBG.png";
        public static readonly string DefaultProfileImage = "/img/DefaultProfile.png";
        public static readonly int MaxFileSize = 5 * 1024 * 1024;

        public static async Task<string> GetDataUrl(IBrowserFile file)
        {
            using Stream fileStream = file.OpenReadStream(MaxFileSize);
            using MemoryStream ms = new MemoryStream();
            await fileStream.CopyToAsync(ms);

            byte[] imageBytes = ms.ToArray();
            string base64String = Convert.ToBase64String(imageBytes);
            string dataUrl = $"data:{file.ContentType};base64,{base64String}";

            return dataUrl; 
        }
    }
}
