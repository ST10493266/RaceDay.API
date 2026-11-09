using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace RaceDay.Api.Services;

public static class ImageUploadRules
{
    public static readonly string[] AllowedContentTypes =
    {
        "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp"
    };

    public const long MaxBytes = 5 * 1024 * 1024;

    public static bool TryValidate(IFormFile? file, ModelStateDictionary modelState, string fieldName = "file")
    {
        if (file is null || file.Length == 0)
        {
            modelState.AddModelError(fieldName, "Please choose an image file.");
            return false;
        }

        if (file.Length > MaxBytes)
        {
            modelState.AddModelError(fieldName, "Images must be 5 MB or smaller.");
            return false;
        }

        if (!AllowedContentTypes.Contains(file.ContentType, StringComparer.OrdinalIgnoreCase))
        {
            modelState.AddModelError(fieldName, "Only JPEG, PNG, GIF or WebP images are allowed.");
            return false;
        }

        return true;
    }
}
