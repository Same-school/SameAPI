using Google;
using Google.Cloud.Storage.V1;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace SameApi.Business.Community.Query
{
    public class GetProfilePictureQuery : IRequest<Stream?>
    {
        public required string CommunitySlug { get; set; }
    }

    public class GetProfilePictureQueryHandler : IRequestHandler<GetProfilePictureQuery, Stream?>
    {
        private readonly StorageClient _storage;
        private readonly string _bucket;

        public GetProfilePictureQueryHandler(StorageClient storage, IConfiguration config)
        {
            _storage = storage;
            _bucket = config["Storage:Bucket"]!;
        }

        public async Task<Stream?> Handle(GetProfilePictureQuery request, CancellationToken cancellationToken)
        {
            var output = new MemoryStream();

            var path = $"communities/{request.CommunitySlug}/profile.jpg";
            try
            {
                await _storage.DownloadObjectAsync(_bucket, path, output, cancellationToken: cancellationToken);
            }
            catch (GoogleApiException e) when (e.HttpStatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            output.Position = 0;
            return output;
        }
    }
}