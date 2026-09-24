using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace assignment1
{
        public interface ICloudStorage
        {
            void UploadFile(string fileName);
            void DownloadFile(string fileName);
            void DeleteFile(string fileName);
        }
        public class GoogleStorage : ICloudStorage
        {
            public void UploadFile(string fileName)
            {
                Console.WriteLine($"Uploading {fileName} to Google storage.");
            }   
            public void DownloadFile(string fileName)
            {
                Console.WriteLine($"Downloading {fileName} from Google storage.");
            }
            public void DeleteFile(string fileName)
            {
                Console.WriteLine($"Deleting {fileName} from Google storage.");
            }
        }

        public class AzureStorage : ICloudStorage
        {
            public void UploadFile(string fileName)
            {
                Console.WriteLine($"Uploading {fileName} to Azure storage.");
            }
            public void DownloadFile(string fileName)
            {
                Console.WriteLine($"Downloading {fileName} from Azure storage.");
            }
            public void DeleteFile(string fileName)
            {
                Console.WriteLine($"Deleting {fileName} from Azure storage.");
            }
        }

}
