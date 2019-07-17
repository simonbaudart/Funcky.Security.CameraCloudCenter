// -----------------------------------------------------------------------
//  <copyright file="AzureStorageProvider.cs" company="Funcky">
//  Copyright (c) Funcky. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Funcky.Security.CameraCloudCenter.Core.Providers.AzureStorage
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Funcky.Security.CameraCloudCenter.Core.Model;
    using Funcky.Security.CameraCloudCenter.Core.Processor;
    using Funcky.Security.CameraCloudCenter.Providers.AzureStorage;

    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    using Newtonsoft.Json;

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
        /// Gets the footages.
        /// </summary>
        /// <param name="footageDate">The footage date.</param>
        /// <returns>
        /// The list of all footages for this date
        /// </returns>
        public async Task<List<Footage>> GetFootages(DateTime footageDate)
        {
            var footages = new List<Footage>();

            var storageAccount = CloudStorageAccount.Parse(this.azureStorageConfiguration.ConnectionString);
            var storageClient = storageAccount.CreateCloudBlobClient();
            var container = storageClient.GetContainerReference(this.azureStorageConfiguration.Container);

            var snapDirectory = container.GetDirectoryReference($"{footageDate:yyyy}/{footageDate:yyyy-MM-dd}/{AzureConstants.ContainerSnap}/");
            var snapFootages = await this.GetFootages(snapDirectory);
            snapFootages = this.CombineFootage(snapFootages, TimeSpan.FromMinutes(5), "Snaps");
            footages.AddRange(snapFootages);

            var recordDirectory = container.GetDirectoryReference($"{footageDate:yyyy}/{footageDate:yyyy-MM-dd}/{AzureConstants.ContainerRecording}/");
            var recordFootages = await this.GetFootages(recordDirectory);
            recordFootages = this.CombineFootage(recordFootages, TimeSpan.FromMinutes(5), "Records");
            footages.AddRange(recordFootages);

            return footages;
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

            await this.UpdateFootageIndex(fileInfo, containerType, blob.Name);
        }

        /// <summary>
        /// Clones the specified footage.
        /// </summary>
        /// <param name="footage">The footage.</param>
        /// <returns>The cloned footage</returns>
        private Footage Clone(Footage footage)
        {
            return JsonConvert.DeserializeObject<Footage>(JsonConvert.SerializeObject(footage));
        }

        /// <summary>
        /// Combines the footage.
        /// </summary>
        /// <param name="footages">The footages.</param>
        /// <param name="interval">The interval.</param>
        /// <param name="footageName">Name of the footage.</param>
        /// <returns>
        /// The combined list of footages
        /// </returns>
        private List<Footage> CombineFootage(List<Footage> footages, TimeSpan interval, string footageName)
        {
            var combined = new List<Footage>();

            var lastFootage = default(Footage);
            var lastFootageCount = 0;

            foreach (var footage in footages)
            {
                if (lastFootage == null)
                {
                    lastFootage = this.Clone(footage);
                    lastFootageCount = 1;

                    combined.Add(lastFootage);
                }
                else if (lastFootage.FootageDate.Add(interval) < footage.FootageDate)
                {
                    lastFootage = this.Clone(footage);
                    lastFootageCount = 1;

                    combined.Add(lastFootage);
                }
                else
                {
                    lastFootageCount++;
                    lastFootage.Sequences.Add(footage);
                    lastFootage.FootageEndDate = footage.FootageEndDate;
                }

                lastFootage.Title = $"{footageName} : {lastFootageCount} footage{(lastFootageCount > 1 ? "s" : string.Empty)} from {lastFootage.FootageDate.ToString("HH:mm:ss", CultureInfo.InvariantCulture)} to {lastFootage.FootageEndDate.ToString("HH:mm:ss", CultureInfo.InvariantCulture)}";
            }

            return combined;
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
        /// Gets the footages.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <returns>
        /// The list of all footages
        /// </returns>
        private async Task<List<Footage>> GetFootages(CloudBlobDirectory directory)
        {
            var footages = new List<Footage>();

            BlobContinuationToken continuationToken = null;

            do
            {
                var files = await directory.ListBlobsSegmentedAsync(true, BlobListingDetails.Metadata, 1000, continuationToken, null, null);
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

                    if (!blob.Metadata.TryGetValue(AzureConstants.FootageDurationMetaData, out var footageDurationValue))
                    {
                        continue;
                    }

                    if (!double.TryParse(footageDurationValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var footageDuration))
                    {
                        continue;
                    }

                    var footage = new Footage
                                      {
                                          Id = $"{blob.Container.Name}|{blob.Name}",
                                          FootageDate = footageDate,
                                          FootageEndDate = footageDate.AddSeconds(footageDuration),
                                          Title = $"Footage recorded at {footageDate:yyyy-MM-dd HH:mm:ss}"
                                      };

                    footages.Add(footage);
                }
            }
            while (continuationToken != null);

            return footages;
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

        /// <summary>
        /// Updates the index of the footage.
        /// </summary>
        /// <param name="fileInfo">The file information.</param>
        /// <param name="containerType">Type of the container.</param>
        /// <param name="blobName">Name of the BLOB.</param>
        /// <returns>The task to wait for in async</returns>
        private async Task UpdateFootageIndex(FileInfo fileInfo, string containerType, string blobName)
        {
            var footageIndex = await this.footageIndexProvider.GetFootageIndex();

            if (fileInfo.CreationTimeUtc > footageIndex.LastFootageDate)
            {
                footageIndex.LastFootageDate = fileInfo.CreationTimeUtc;

                // If the container is snap, keep the name of the last image to display on the dashboard
                // Use a minimum snap interval to avoid "empty" images
                if (containerType == AzureConstants.ContainerSnap && footageIndex.LastFootageImageDate.Add(AzureConstants.MinimumSnapInterval) < fileInfo.CreationTimeUtc)
                {
                    footageIndex.LastFootageImage = blobName;
                    footageIndex.LastFootageImageDate = fileInfo.CreationTimeUtc;
                }

                await this.footageIndexProvider.SetFootageIndex(footageIndex);
            }
        }
    }
}