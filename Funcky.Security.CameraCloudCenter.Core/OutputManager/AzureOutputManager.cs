// -----------------------------------------------------------------------
//  <copyright file="AzureOutputManager.cs" company="Funcky">
//  Copyright (c) Funcky. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Funcky.Security.CameraCloudCenter.Core.OutputManager
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Funcky.Security.CameraCloudCenter.Core.Configuration;

    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    /// <summary>
    /// Output to Azure
    /// </summary>
    public class AzureOutputManager
    {
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
        private readonly AzureOutputConfiguration azureOutputConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureOutputManager" /> class.
        /// </summary>
        /// <param name="azureOutputConfiguration">The azure output configuration.</param>
        public AzureOutputManager(AzureOutputConfiguration azureOutputConfiguration)
        {
            this.azureOutputConfiguration = azureOutputConfiguration;
        }

        /// <summary>
        /// Cleanups this storage.
        /// </summary>
        /// <returns>The task to wait</returns>
        public async Task Cleanup()
        {
            var storageAccount = CloudStorageAccount.Parse(this.azureOutputConfiguration.ConnectionString);
            var storageClient = storageAccount.CreateCloudBlobClient();
            var container = storageClient.GetContainerReference(this.azureOutputConfiguration.Container);

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
                                if (footageDate < DateTime.UtcNow.AddDays(-this.azureOutputConfiguration.Retention))
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

            var storageAccount = CloudStorageAccount.Parse(this.azureOutputConfiguration.ConnectionString);
            var storageClient = storageAccount.CreateCloudBlobClient();
            var container = storageClient.GetContainerReference(this.azureOutputConfiguration.Container);

            await container.CreateIfNotExistsAsync();

            var path = $"{fileInfo.CreationTime:yyyy}/{fileInfo.CreationTime:yyyy-MM-dd}/";
            var blobDirectory = container.GetDirectoryReference(path);

            var blob = blobDirectory.GetBlockBlobReference(fileInfo.Name);

            await blob.UploadFromFileAsync(fileInfo.FullName);

            blob.Metadata.Add(FootageDateMetaData, fileInfo.CreationTime.ToUniversalTime().ToString(FootageDateFormat, CultureInfo.InvariantCulture));
            await blob.SetMetadataAsync();
        }
    }
}