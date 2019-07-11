// -----------------------------------------------------------------------
//  <copyright file="AzureStorageProvider.cs" company="Funcky">
//  Copyright (c) Funcky. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Funcky.Security.CameraCloudCenter.Core.Providers.AzureStorage
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Funcky.Security.CameraCloudCenter.Core.Model;
    using Funcky.Security.CameraCloudCenter.Core.Processor;
    using Funcky.Security.CameraCloudCenter.Providers.AzureStorage;

    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    /// <summary>
    /// Output to Azure
    /// </summary>
    public class AzureStorageProvider : IFootageStorage
    {
        /// <summary>
        /// The azure storage configuration
        /// </summary>
        private readonly AzureStorageConfiguration azureStorageConfiguration;

        /// <summary>
        /// The footage index provider
        /// </summary>
        private readonly FootageIndexProvider footageIndexProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureStorageProvider" /> class.
        /// </summary>
        /// <param name="azureStorageConfiguration">The azure storage configuration.</param>
        public AzureStorageProvider(AzureStorageConfiguration azureStorageConfiguration)
        {
            this.azureStorageConfiguration = azureStorageConfiguration;
            this.footageIndexProvider = new FootageIndexProvider(azureStorageConfiguration);
        }

        /// <summary>
        /// Cleanups the old footages.
        /// </summary>
        /// <returns>
        /// The task to wait for in async
        /// </returns>
        public async Task Cleanup()
        {
            var storageAccount = CloudStorageAccount.Parse(this.azureStorageConfiguration.ConnectionString);
            var storageClient = storageAccount.CreateCloudBlobClient();
            var container = storageClient.GetContainerReference(this.azureStorageConfiguration.Container);

            BlobContinuationToken continuationToken = null;

            do
            {
                var files = await container.ListBlobsSegmentedAsync(null, true, BlobListingDetails.Metadata, 1000, continuationToken, null, null);
                continuationToken = files.ContinuationToken;

                foreach (var file in files.Results)
                {
                    if (!(file is CloudBlockBlob blob))
                    {
                        continue;
                    }

                    if (!blob.Metadata.TryGetValue(AzureConstants.FootageDateMetaData, out var footageDateValue))
                    {
                        continue;
                    }

                    if (!DateTime.TryParseExact(footageDateValue, AzureConstants.FootageDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var footageDate))
                    {
                        continue;
                    }

                    if (footageDate < DateTime.UtcNow.AddDays(-this.azureStorageConfiguration.Retention))
                    {
                        await blob.DeleteAsync();
                    }
                }
            }
            while (continuationToken != null);
        }

        /// <summary>
        /// Fills the last footage.
        /// </summary>
        /// <param name="camera">The camera.</param>
        /// <returns>The task to wait for in async</returns>
        public async Task FillLastFootage(Camera camera)
        {
            var footageIndex = await this.footageIndexProvider.GetFootageIndex();
            camera.LastFootageDate = footageIndex.LastFootageDate;
            camera.LastFootageImage = footageIndex.LastFootageImage;
            camera.LastFootageImageUrl = this.GetTemporaryAccess(footageIndex.LastFootageImage);
        }

        /// <summary>
        /// Uploads the file.
        /// </summary>
        /// <param name="localPath">The local path.</param>
        /// <returns>The task to wait in async</returns>
        public async Task UploadFile(string localPath)
        {
            var fileInfo = new FileInfo(localPath);

            if (!fileInfo.Exists)
            {
                return;
            }

            var storageAccount = CloudStorageAccount.Parse(this.azureStorageConfiguration.ConnectionString);
            var storageClient = storageAccount.CreateCloudBlobClient();
            var container = storageClient.GetContainerReference(this.azureStorageConfiguration.Container);

            await container.CreateIfNotExistsAsync();

            var containerType = this.GetContainerType(fileInfo);

            var path = $"{fileInfo.CreationTimeUtc:yyyy}/{fileInfo.CreationTimeUtc:yyyy-MM-dd}/{containerType}/";
            var blobDirectory = container.GetDirectoryReference(path);

            var blob = blobDirectory.GetBlockBlobReference(fileInfo.Name);

            await blob.UploadFromFileAsync(fileInfo.FullName);

            await this.SetMetadata(blob, containerType, fileInfo);

            var footageIndex = await this.footageIndexProvider.GetFootageIndex();

            if (fileInfo.CreationTimeUtc > footageIndex.LastFootageDate)
            {
                footageIndex.LastFootageDate = fileInfo.CreationTimeUtc;

                if (containerType == AzureConstants.ContainerSnap)
                {
                    footageIndex.LastFootageImage = blob.Name;
                }

                await this.footageIndexProvider.SetFootageIndex(footageIndex);
            }
        }

        /// <summary>
        /// Gets the type of the container.
        /// </summary>
        /// <param name="fileInfo">The file information.</param>
        /// <returns>
        /// Th name of the container for this type
        /// </returns>
        private string GetContainerType(FileInfo fileInfo)
        {
            switch (fileInfo.Extension.Trim('.').ToLowerInvariant())
            {
                case "jpg":
                case "jpeg":
                case "png":
                    return AzureConstants.ContainerSnap;

                case "mkv":
                case "mp4":
                case "avi":
                case "mov":
                case "wmv":
                    return AzureConstants.ContainerRecording;

                case "log":
                    return AzureConstants.ContainerEvent;
            }

            return AzureConstants.ContainerOthers;
        }

        /// <summary>
        /// Gets the temporary access.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>The uri for a temporary access</returns>
        private string GetTemporaryAccess(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return AzureConstants.EmptyFootageUrl;
            }

            var storageAccount = CloudStorageAccount.Parse(this.azureStorageConfiguration.ConnectionString);
            var storageClient = storageAccount.CreateCloudBlobClient();
            var container = storageClient.GetContainerReference(this.azureStorageConfiguration.Container);
            var blob = container.GetBlobReference(path);

            var builder = new UriBuilder(blob.Uri);
            builder.Query = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy { Permissions = SharedAccessBlobPermissions.Read, SharedAccessStartTime = new DateTimeOffset(DateTime.UtcNow.AddMinutes(-2)), SharedAccessExpiryTime = new DateTimeOffset(DateTime.UtcNow.AddMinutes(5)) }).TrimStart('?');

            return builder.Uri.ToString();
        }

        /// <summary>
        /// Sets the metadata.
        /// </summary>
        /// <param name="blob">The BLOB.</param>
        /// <param name="containerType">Type of the container.</param>
        /// <param name="fileInfo">The file information.</param>
        /// <returns>The task to wait for in async</returns>
        private async Task SetMetadata(CloudBlockBlob blob, string containerType, FileInfo fileInfo)
        {
            blob.Metadata.Add(AzureConstants.FootageDateMetaData, fileInfo.CreationTimeUtc.ToString(AzureConstants.FootageDateFormat, CultureInfo.InvariantCulture));

            switch (containerType)
            {
                case AzureConstants.ContainerRecording:
                    var videoInfo = new VideoInfo(fileInfo);
                    blob.Metadata.Add(AzureConstants.FootageDurationMetaData, videoInfo.GetDuration().TotalSeconds.ToString(CultureInfo.InvariantCulture));
                    break;

                default:
                    blob.Metadata.Add(AzureConstants.FootageDurationMetaData, "0");
                    break;
            }

            await blob.SetMetadataAsync();
        }
    }
}