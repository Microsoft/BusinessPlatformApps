using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Actions.AzureCustom
{
    public class FtpUtilityTest
    {
        public static FtpWebRequest GetRequest(string uri)
        {
            var request = (FtpWebRequest) WebRequest.Create(uri);
            //request.KeepAlive = true;
            //request.ConnectionGroupName = "UploadFunction";
            //request.ServicePoint.ConnectionLimit = 8;
            request.Timeout = -1;
            return request;
        }

        public static void ListDirectory(string ftpserver, string user, string password)
        {
            var request = GetRequest(ftpserver);
            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;

            request.Credentials = new NetworkCredential(user, password);

            FtpWebResponse response = (FtpWebResponse) request.GetResponse();

            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);
            var read = reader.ReadToEnd();
            reader.Close();
            response.Close();
        }

        public static void UploadFile(string ftpserver, string user, string password, string path, string file)
        {
            byte[] data = File.ReadAllBytes(path + "/" + file);
            UploadFile(ftpserver, user, password, file, data);
        }

        public static void UploadFileToServer(string ftpserver, string user, string password, string path, string file)
        {
            byte[] data = File.ReadAllBytes(file);
            CreateFolder(ftpserver, user, password, path);
            UploadFile(ftpserver, user, password, path, data);
        }




        public static void UploadFile(string ftpserver, string user, string password, string relPath, byte[] data)
        {
            var request = GetRequest(ftpserver + "/" + relPath);

            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(user, password);
            request.UseBinary = true;

            request.ContentLength = data.Length;
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(data, 0, data.Length);
            requestStream.Close();
            requestStream.Flush();
            FtpWebResponse response = (FtpWebResponse) request.GetResponse();

            if (response.StatusCode != FtpStatusCode.ClosingData)
            {
                throw new Exception("");
            }

            requestStream.Dispose();
            response.Close();
            response.Dispose();
        }

        public static void CreateFolder(string ftpserver, string user, string password, string relativefoldePath)
        {

            string currentDir = ftpserver;

            var folders = relativefoldePath.Replace("\\", "/").Replace("//", "/").Split('/');
            foreach (var folder in folders)
            {
                if (!string.IsNullOrEmpty(folder) && !folder.Contains("."))
                {
                    try
                    {
                        currentDir = currentDir + "/" + folder;

                        var request = GetRequest(currentDir);
                        request.Method = WebRequestMethods.Ftp.MakeDirectory;
                        request.UseBinary = true;
                        request.Credentials = new NetworkCredential(user, password);
                        FtpWebResponse response = (FtpWebResponse) request.GetResponse();
                        var ftpStream = response.GetResponseStream();

                        ftpStream.Close();
                        response.Close();
                    }
                    catch (Exception)
                    {
                    }
                }
            }

        }
    }
}
