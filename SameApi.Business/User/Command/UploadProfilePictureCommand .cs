using Google.Cloud.Storage.V1;
using ImageMagick;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace SameApi.Business.User.Command
{
    public class UploadProfilePictureCommand : IRequest<string>
    {
        public required string Uid { get; set; }
        public required Stream Content { get; set; }
    }

    public class UploadProfilePictureCommandHandler : IRequestHandler<UploadProfilePictureCommand, string>
    {
        private readonly StorageClient _storage;
        private readonly string _bucket;

        public UploadProfilePictureCommandHandler(StorageClient storage, IConfiguration config)
        {
            _storage = storage;
            _bucket = config["Storage:Bucket"]!;
        }

        public async Task<string> Handle(UploadProfilePictureCommand request, CancellationToken cancellationToken)
        {
            var imageBase = new MagickImage(request.Content);
            var image = TransformImage(imageBase);

            using var output = new MemoryStream();

            await image.WriteAsync(output, cancellationToken);

            var path = $"users/{request.Uid}/profile.jpg";

            output.Position = 0;

            await _storage.UploadObjectAsync(_bucket, path, "image/jpeg", output, cancellationToken: cancellationToken);

            return path;
        }

        private static MagickImage TransformImage(MagickImage image)
        {
            image.AutoOrient();
            image.Strip();
            image.Resize(new MagickGeometry(512, 512) { FillArea = true });
            image.Crop(new MagickGeometry(512, 512), Gravity.Center);
            image.ResetPage();
            image.Format = MagickFormat.Jpeg;
            image.Quality = 80;

            return image;
        }
    }
}