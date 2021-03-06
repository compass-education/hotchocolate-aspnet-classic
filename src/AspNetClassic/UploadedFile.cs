

namespace HotChocolate.AspNetClassic;

internal sealed class UploadedFile : StreamFile
{
    private readonly IFormFile _file;

    public UploadedFile(IFormFile file)
        : base(file.FileName, file.OpenReadStream, file.Length)
    {
        _file = file ?? throw new ArgumentNullException(nameof(file));
    }

    public override Task CopyToAsync(
        Stream target,
        CancellationToken cancellationToken = default) =>
        _file.CopyToAsync(target, cancellationToken);
}
