using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace assignment1
{
    public class S3Storage : ICloudStorageProvider
    {
        public void UploadFile(string path)
        {
            Console.WriteLine($"[Amazon S3] Uploading file from path '{path}' to S3 bucket.");
        }
        public void DownloadFile(string fileId)
        {
            Console.WriteLine($"[Amazon S3] Downloading object with Key '{fileId}' from S3 bucket.");
        }
        public void DeleteFile(string fileId)
        {
            Console.WriteLine($"[Amazon S3] Deleting object with Key '{fileId}' from S3 bucket.");
        }
    }

    public class GoogleCloudStorage : ICloudStorageProvider
    {
        public void UploadFile(string path)
        {
            Console.WriteLine($"[Google Cloud] Uploading '{path}' to GCS Bucket.");
        }
        public void DownloadFile(string fileId)
        {
            Console.WriteLine($"[Google Cloud] Fetching object '{fileId}' via Cloud Storage API.");
        }
        public void DeleteFile(string fileId)
        {
            Console.WriteLine($"[Google Cloud] Deleting object '{fileId}' from GCS Bucket.");
        }
    }

    public class AzureStorage : ICloudStorageProvider
    {
        public void UploadFile(string path)
        {
            Console.WriteLine($"[Azure Blob] Uploading '{path}' as Block Blob to Azure Storage Container.");
        }
        public void DownloadFile(string fileId)
        {
            Console.WriteLine($"[Azure Blob] Streaming blob content for BlobId '{fileId}'.");
        }
        public void DeleteFile(string fileId)
        {
            Console.WriteLine($"[Azure Blob] Marking and deleting blob '{fileId}'.");
        }
    }

}
