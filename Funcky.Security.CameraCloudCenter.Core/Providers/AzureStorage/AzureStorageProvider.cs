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

    using Funcky.Security.CameraCloudCenter.Providers.AzureStorage;

    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    /// <summary>
    /// Output to Azure
    /// </summary>
    public class AzureStorageProvider : IFootageStorage
    {
        /// <summary>
        /// The container event
        /// </summary>
        private const string ContainerEvent = "event";

        /// <summary>
        /// The container others
        /// </summary>
        private const string ContainerOthers = "others";

        /// <summary>
        /// The container recording
        /// </summary>
        private const string ContainerRecording = "recording";

        /// <summary>
        /// The container snap
        /// </summary>
        private const string ContainerSnap = "snap";

        /// <summary>
        /// The footage date format
        /// </summary>
        private const string FootageDateFormat = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        /// The footage date meta data
        /// </summary>
        private const string FootageDateMetaData = "FootageDate";

        /// <summary>
        /// The azure output configuration
        /// </summary>
        private readonly AzureStorageConfiguration azureStorageConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureStorageProvider" /> class.
        /// </summary>
        /// <param name="azureStorageConfiguration">The azure storage configuration.</param>
        public AzureStorageProvider(AzureStorageConfiguration azureStorageConfiguration)
        {
            this.azureStorageConfiguration = azureStorageConfiguration;
        }

        /// <summary>
        /// Cleanups this storage.
        /// </summary>
        /// <returns>The task to wait</returns>
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
                    if (file is CloudBlockBlob blob)
                    {
                        if (blob.Metadata.TryGetValue(FootageDateMetaData, out var footageDateValue))
                        {
                            if (DateTime.TryParseExact(footageDateValue, FootageDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var footageDate))
                            {
                                if (footageDate < DateTime.UtcNow.AddDays(-this.azureStorageConfiguration.Retention))
                                {
                                    await blob.DeleteAsync();
                                }
                            }
                            else
                            {
                                await blob.DeleteAsync();
                            }
                        }
                        else
                        {
                            await blob.DeleteAsync();
                        }
                    }
                }
            }
            while (continuationToken != null);
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

            var path = $"{fileInfo.CreationTime:yyyy}/{fileInfo.CreationTime:yyyy-MM-dd}/{containerType}/";
            var blobDirectory = container.GetDirectoryReference(path);

            var blob = blobDirectory.GetBlockBlobReference(fileInfo.Name);

            await blob.UploadFromFileAsync(fileInfo.FullName);

            blob.Metadata.Add(FootageDateMetaData, fileInfo.CreationTime.ToUniversalTime().ToString(FootageDateFormat, CultureInfo.InvariantCulture));
            await blob.SetMetadataAsync();
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
                    return ContainerSnap;

                case "mkv":
                case "mp4":
                case "avi":
                case "mov":
                case "wmv":
                    return ContainerRecording;

                case "log":
                    return ContainerEvent;
            }

            return ContainerOthers;
        }
    }
}