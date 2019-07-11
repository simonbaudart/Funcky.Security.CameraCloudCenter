// -----------------------------------------------------------------------
//  <copyright file="FootageIndexProvider.cs" company="Funcky">
//  Copyright (c) Funcky. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Funcky.Security.CameraCloudCenter.Core.Providers.AzureStorage
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Funcky.Security.CameraCloudCenter.Providers.AzureStorage;

    using Microsoft.WindowsAzure.Storage;

    using Newtonsoft.Json;

    /// <summary>
    /// To avoid storage in another database but to be able to quickly retrieve some information
    /// this classe is used and stored like a database in azure storage
    /// </summary>
    /// <remarks>
    /// Why not SQL ? Because à want to be DB free and only be dependant of the storage
    /// </remarks>
    public class FootageIndexProvider
    {
        /// <summary>
        /// The index semaphore slim
        /// </summary>
        private static readonly SemaphoreSlim IndexSemaphoreSlim = new SemaphoreSlim(1, 1);

        /// <summary>
        /// The azure storage configuration
        /// </summary>
        private readonly AzureStorageConfiguration azureStorageConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="FootageIndexProvider" /> class.
        /// </summary>
        /// <param name="azureStorageConfiguration">The azure storage configuration.</param>
        public FootageIndexProvider(AzureStorageConfiguration azureStorageConfiguration)
        {
            this.azureStorageConfiguration = azureStorageConfiguration;
        }

        /// <summary>
        /// Gets the index of the footage.
        /// </summary>
        /// <returns>The index footage with most of the index informations</returns>
        public async Task<FootageIndex> GetFootageIndex()
        {
            var storageAccount = CloudStorageAccount.Parse(this.azureStorageConfiguration.ConnectionString);
            var storageClient = storageAccount.CreateCloudBlobClient();
            var container = storageClient.GetContainerReference(this.azureStorageConfiguration.Container);

            var indexBlob = container.GetBlockBlobReference("index.json");

            if (await indexBlob.ExistsAsync())
            {
                var indexJson = await indexBlob.DownloadTextAsync();

                return JsonConvert.DeserializeObject<FootageIndex>(indexJson);
            }

            return new FootageIndex();
        }

        /// <summary>
        /// Gets the index of the footage.
        /// </summary>
        /// <param name="footageIndex">Index of the footage.</param>
        /// <returns>
        /// The index footage with most of the index informations
        /// </returns>
        public async Task SetFootageIndex(FootageIndex footageIndex)
        {
            var storageAccount = CloudStorageAccount.Parse(this.azureStorageConfiguration.ConnectionString);
            var storageClient = storageAccount.CreateCloudBlobClient();
            var container = storageClient.GetContainerReference(this.azureStorageConfiguration.Container);

            var indexBlob = container.GetBlockBlobReference("index.json");

            await IndexSemaphoreSlim.WaitAsync();

            try
            {
                using (var stream = await indexBlob.OpenWriteAsync())
                using (var writer = new StreamWriter(stream))
                {
                    var indexJson = JsonConvert.SerializeObject(footageIndex, Formatting.Indented);
                    await writer.WriteAsync(indexJson);
                }
            }
            finally
            {
                IndexSemaphoreSlim.Release();
            }
        }
    }
}