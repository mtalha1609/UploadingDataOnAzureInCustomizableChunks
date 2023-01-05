using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Storage.Blob;
using Azure.Storage.Blobs;
using System.IO;

namespace chunkuploading
{
    internal class Program
{
    static void Main(string[] args)
    {
            Console.WriteLine("Starting Dividing FIle into Chunks.....");

            Console.WriteLine("Enter Size in -KB- you want to split the file into");
            int maxChunkSizeKB = Convert.ToInt32(Console.ReadLine());

            int maxChunkSizeB = maxChunkSizeKB * 1024; // convert to bytes (==characters)


            Console.WriteLine("Enter the Path to the file");
            var dirPath = @"" + Console.ReadLine();
            var textFileName = dirPath;   
     
            StreamReader rdr = File.OpenText(textFileName);
            // then take this file and place it into a string buffer

            string fileBuffer = rdr.ReadToEnd();
            rdr.Close();
            // now cut up the file into sizeable files

            int fileCounter = 0;
            while (fileBuffer.Length > 0)
            {
                //
                // get this chunk contents

                fileCounter++;
                string fileChunkBuffer = "";
                if (fileBuffer.Length > maxChunkSizeB)
                {
                    fileChunkBuffer = fileBuffer.Substring(0, maxChunkSizeB);
                    fileBuffer = fileBuffer.Substring(maxChunkSizeB);
                }
                else
                {
                    fileChunkBuffer = fileBuffer;
                    fileBuffer = "";
                }

                //
                // write this chunk file


                string chunkFileName = textFileName + fileCounter.ToString();
                StreamWriter wtr = File.CreateText(chunkFileName);
                wtr.Write(fileChunkBuffer);
                wtr.Close();
            }



            Console.WriteLine("Starting Upload on Azure......");

            var blobStorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=blobstorage160;AccountKey=Lfck/rx6uspuxK2xQIm8pL8dR9E3dyAtssIlFwPTy7DIcvWYXoEVz9KHqKQalGgI3HAkE8D4DotA+AStfVK33A==;EndpointSuffix=core.windows.net";
            var blobStorageContainerName = "fileupload";


            var folderpath = @"C:\Users\talha.asghar\Desktop\for practice\chunkuploading\chunkuploading\bin\filetobeuploaded\";

            var files = Directory.GetFiles(folderpath, "*", SearchOption.AllDirectories);

            var container = new BlobContainerClient(blobStorageConnectionString, blobStorageContainerName);

            foreach (var file in files)
            {
                var filePathOverCloud = file.Replace(folderpath, string.Empty);
                using (MemoryStream stream = new MemoryStream(File.ReadAllBytes(file)))
                {
                    container.UploadBlob(filePathOverCloud, stream);
                    Console.WriteLine(filePathOverCloud + "Uploaded!");
                }
            }
            Console.WriteLine("Upload Completed");



            Console.ReadLine();

        }
    }


}