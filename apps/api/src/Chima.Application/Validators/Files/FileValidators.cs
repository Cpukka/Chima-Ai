using FluentValidation;

namespace Chima.Application.Validators.Files;

public class FileUploadValidator : AbstractValidator<(string FileName, string ContentType, long SizeBytes)>
{
    private static readonly string[] AllowedExtensions =
        { ".txt", ".md", ".csv", ".pdf", ".docx", ".json" };

    private static readonly string[] AllowedContentTypes =
    {
        "text/plain", "text/markdown", "text/csv", "text/x-csv",
        "application/pdf",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "application/json", "text/json",
        "application/octet-stream" // some browsers send this
    };

    public FileUploadValidator()
    {
        RuleFor(x => x.FileName)
            .NotEmpty()
            .Must(name =>
            {
                var ext = Path.GetExtension(name)?.ToLowerInvariant();
                return ext is not null && AllowedExtensions.Contains(ext);
            })
            .WithMessage($"Only these file types are supported: {string.Join(", ", AllowedExtensions)}");

        RuleFor(x => x.SizeBytes)
            .GreaterThan(0).WithMessage("File is empty.")
            .LessThanOrEqualTo(100 * 1024 * 1024).WithMessage("Maximum file size is 100 MB.");
    }
}