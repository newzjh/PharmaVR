﻿//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//

using UnityEngine;
using System.Collections;
using System.Net;
using System;
using System.IO;
using System.Collections.Generic;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Function used to communicate with the device through the REST API
    /// </summary>
    public class BuildDeployPortal
    {
        // Consts
        public const float TimeOut = 6.0f;
        public const int TimeoutMS = (int)(TimeOut * 1000.0f);
        public const float MaxWaitTime = 20.0f;

        public static readonly string kAPI_ProcessQuery = @"http://{0}/api/resourcemanager/processes";
        public static readonly string kAPI_PackagesQuery = @"http://{0}/api/appx/packagemanager/packages";
        public static readonly string kAPI_InstallQuery = @"http://{0}/api/app/packagemanager/package";
        public static readonly string kAPI_InstallStatusQuery = @"http://{0}/api/app/packagemanager/state";
        public static readonly string kAPI_AppQuery = @"http://{0}/api/taskmanager/app";
        public static readonly string kAPI_FileQuery = @"http://{0}/api/filesystem/apps/file";

        // Enums
        public enum AppInstallStatus
        {
            Invalid,
            Installing,
            InstallSuccess,
            InstallFail
        }

        // Classes & Structs
        public struct ConnectInfo
        {
            public ConnectInfo(string ip, string user, string password)
            {
                IP = ip;
                User = user;
                Password = password;
            }

            public string IP;
            public string User;
            public string Password;
        }
        [Serializable]
        public class AppDetails
        {
            public string Name;
            public string PackageFamilyName;
            public string PackageFullName;
            public int PackageOrigin;
            public string PackageRelativeId;
            public string Publisher;
        }
        [Serializable]
        public class AppList
        {
            public AppDetails[] InstalledPackages;
        }
        [Serializable]
        public class ProcessDesc
        {
            public float CPUUsage;
            public string ImageName;
            public float PageFileUsage;
            public int PrivateWorkingSet;
            public int ProcessId;
            public int SessionId;
            public string UserName;
            public int VirtualSize;
            public int WorkingSetSize;
        }
        [Serializable]
        public class ProcessList
        {
            public ProcessDesc[] Processes;
        }
        [Serializable]
        public class InstallStatus
        {
            public int Code;
            public string CodeText;
            public string Reason;
            public bool Success;
        }
        [Serializable]
        public class Response
        {
            public string Reason;
        }
        private class TimeoutWebClient : WebClient
        {
            protected override WebRequest GetWebRequest(Uri uri)
            {
                WebRequest lWebRequest = base.GetWebRequest(uri);
                lWebRequest.Timeout = BuildDeployPortal.TimeoutMS;
                ((HttpWebRequest)lWebRequest).ReadWriteTimeout = BuildDeployPortal.TimeoutMS;
                return lWebRequest;
            }
        }

        // Functions
        public static bool IsAppInstalled(string baseAppName, ConnectInfo connectInfo)
        {
            // Look at the device for a matching app name (if not there, then not installed)
            return (QueryAppDetails(baseAppName, connectInfo) != null);
        }

        public static bool IsAppRunning(string baseAppName, ConnectInfo connectInfo)
        {
            using (var client = new TimeoutWebClient())
            {
                client.Credentials = new NetworkCredential(connectInfo.User, connectInfo.Password);
                string query = string.Format(kAPI_ProcessQuery, connectInfo.IP);
                string procListJSON = client.DownloadString(query);
#if UNITY_5_3_OR_NEWER
                ProcessList procList = JsonUtility.FromJson<ProcessList>(procListJSON);
                for (int i = 0; i < procList.Processes.Length; ++i)
                {
                    string procName = procList.Processes[i].ImageName;
                    if (procName.Contains(baseAppName))
                    {
                        return true;
                    }
                }
#endif
            }
            return false;
        }

        public static AppInstallStatus GetInstallStatus(ConnectInfo connectInfo)
        {
            using (var client = new TimeoutWebClient())
            {
                client.Credentials = new NetworkCredential(connectInfo.User, connectInfo.Password);
                string query = string.Format(kAPI_InstallStatusQuery, connectInfo.IP);
                string statusJSON = client.DownloadString(query);
#if UNITY_5_3_OR_NEWER
                InstallStatus status = JsonUtility.FromJson<InstallStatus>(statusJSON);

                if (status == null)
                {
                    return AppInstallStatus.Installing;
                }
                else if (status.Success == false)
                {
                    Debug.LogError(status.Reason + "(" + status.CodeText + ")");
                    return AppInstallStatus.InstallFail;
                }
#endif
                return AppInstallStatus.InstallSuccess;
            }
        }

        public static AppDetails QueryAppDetails(string baseAppName, ConnectInfo connectInfo)
        {
            using (var client = new TimeoutWebClient())
            {
                client.Credentials = new NetworkCredential(connectInfo.User, connectInfo.Password);
                string query = string.Format(kAPI_PackagesQuery, connectInfo.IP);
                string appListJSON = client.DownloadString(query);
#if UNITY_5_3_OR_NEWER
                AppList appList = JsonUtility.FromJson<AppList>(appListJSON);
                for (int i = 0; i < appList.InstalledPackages.Length; ++i)
                {
                    string appName = appList.InstalledPackages[i].Name;
                    if (appName.Contains(baseAppName))
                    {
                        return appList.InstalledPackages[i];
                    }
                }
#endif
            }
            return null;
        }

        public static bool InstallApp(string appFullPath, ConnectInfo connectInfo, bool waitForDone = true)
        {
            try
            {
                // Calc the cert and dep paths
                string fileName = Path.GetFileName(appFullPath);
                string certFullPath = Path.ChangeExtension(appFullPath, ".cer");
                string certName = Path.GetFileName(certFullPath);
                string depPath = Path.GetDirectoryName(appFullPath) + @"\Dependencies\x86\";

                // Post it using the REST API
                WWWForm form = new WWWForm();

                // APPX file
                FileStream stream = new FileStream(appFullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                BinaryReader reader = new BinaryReader(stream);
                form.AddBinaryData(fileName, reader.ReadBytes((int)reader.BaseStream.Length), fileName);
                stream.Close();

                // CERT file
                stream = new FileStream(certFullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                reader = new BinaryReader(stream);
                form.AddBinaryData(certName, reader.ReadBytes((int)reader.BaseStream.Length), certName);
                stream.Close();

                // Dependencies
                FileInfo[] depFiles = (new DirectoryInfo(depPath)).GetFiles();
                foreach (FileInfo dep in depFiles)
                {
                    stream = new FileStream(dep.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
                    reader = new BinaryReader(stream);
                    string depFilename = Path.GetFileName(dep.FullName);
                    form.AddBinaryData(depFilename, reader.ReadBytes((int)reader.BaseStream.Length), depFilename);
                    stream.Close();
                }

                // Credentials
                Dictionary<string, string> headers = form.headers;
                headers["Authorization"] = "Basic " + EncodeTo64(connectInfo.User + ":" + connectInfo.Password);

                // Unity places an extra quote in the content-type boundary parameter that the device portal doesn't care for, remove it
                if (headers.ContainsKey("Content-Type"))
                {
                    headers["Content-Type"] = headers["Content-Type"].Replace("\"", "");
                }

                // Query
                string query = string.Format(kAPI_InstallQuery, connectInfo.IP);
                query += "?package=" + WWW.EscapeURL(fileName);
                WWW www = new WWW(query, form.data, headers);
                DateTime queryStartTime = DateTime.Now;
                while (!www.isDone &&
                       ((DateTime.Now - queryStartTime).TotalSeconds < TimeOut))
                {
                    System.Threading.Thread.Sleep(10);
                }

                // Give it a short time before checking
                System.Threading.Thread.Sleep(250);

                // Report
                if (www.isDone)
                {
#if UNITY_5_3_OR_NEWER
                    Debug.Log(JsonUtility.FromJson<Response>(www.text).Reason);
#endif
                }

                // Wait for done (if requested)
                DateTime waitStartTime = DateTime.Now;
                while (waitForDone &&
                    ((DateTime.Now - waitStartTime).TotalSeconds < MaxWaitTime))
                {
                    AppInstallStatus status = GetInstallStatus(connectInfo);
                    if (status == AppInstallStatus.InstallSuccess)
                    {
                        Debug.Log("Install Successful!");
                        break;
                    }
                    else if (status == AppInstallStatus.InstallFail)
                    {
                        Debug.LogError("Install Failed!");
                        break;
                    }

                    // Wait a bit and we'll ask again
                    System.Threading.Thread.Sleep(1000);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.ToString());
                return false;
            }

            return true;
        }

        public static bool UninstallApp(string appName, ConnectInfo connectInfo)
        {
            try
            {
                // Find the app description
                AppDetails appDetails = QueryAppDetails(appName, connectInfo);
                if (appDetails == null)
                {
                    Debug.LogError(string.Format("Application '{0}' not found", appName));
                    return false;
                }

                // Setup the command
                string query = string.Format(kAPI_InstallQuery, connectInfo.IP);
                query += "?package=" + WWW.EscapeURL(appDetails.PackageFullName);

                // Use HttpWebRequest for a delete query
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(query);
                request.Timeout = TimeoutMS;
                request.Credentials = new NetworkCredential(connectInfo.User, connectInfo.Password);
                request.Method = "DELETE";
                using (HttpWebResponse httpResponse = (HttpWebResponse)request.GetResponse())
                {
                    Debug.Log("Response = " + httpResponse.StatusDescription);
                    httpResponse.Close();
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.ToString());
                return false;
            }

            return true;
        }

        public static bool LaunchApp(string appName, ConnectInfo connectInfo)
        {
            // Post it using the REST API
            WWWForm form = new WWWForm();
            form.AddField("user", "user");        // Unity needs some data in the form

            // Find the app description
            AppDetails appDetails = QueryAppDetails(appName, connectInfo);
            if (appDetails == null)
            {
                Debug.LogError("Appliation not found");
                return false;
            }

            // Setup the command
            string query = string.Format(kAPI_AppQuery, connectInfo.IP);
            query += "?appid=" + WWW.EscapeURL(EncodeTo64(appDetails.PackageRelativeId));
            query += "&package=" + WWW.EscapeURL(EncodeTo64(appDetails.PackageFamilyName));

            // Credentials
            Dictionary<string, string> headers = form.headers;
            headers["Authorization"] = "Basic " + EncodeTo64(connectInfo.User + ":" + connectInfo.Password);

            // Query
            WWW www = new WWW(query, form.data, headers);
            DateTime queryStartTime = DateTime.Now;
            while (!www.isDone &&
                   ((DateTime.Now - queryStartTime).TotalSeconds < TimeOut))
            {
                System.Threading.Thread.Sleep(10);
            }

            // Error
            if (!string.IsNullOrEmpty(www.error))
            {
                Debug.LogError(www.error);
                return false;
            }

            return true;
        }

        public static bool KillApp(string appName, ConnectInfo connectInfo)
        {
            try
            {
                // Find the app description
                AppDetails appDetails = QueryAppDetails(appName, connectInfo);
                if (appDetails == null)
                {
                    Debug.LogError("Appliation not found");
                    return false;
                }

                // Setup the command
                string query = string.Format(kAPI_AppQuery, connectInfo.IP);
                query += "?package=" + WWW.EscapeURL(EncodeTo64(appDetails.PackageFullName));

                // And send it across
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(query);
                request.Timeout = TimeoutMS;
                request.Credentials = new NetworkCredential(connectInfo.User, connectInfo.Password);
                request.Method = "DELETE";
                using (HttpWebResponse httpResponse = (HttpWebResponse)request.GetResponse())
                {
                    Debug.Log("Response = " + httpResponse.StatusDescription);
                    httpResponse.Close();
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.ToString());
                return false;
            }

            return true;
        }

        public static bool DeviceLogFile_View(string appName, ConnectInfo connectInfo)
        {
            using (var client = new TimeoutWebClient())
            {
                client.Credentials = new NetworkCredential(connectInfo.User, connectInfo.Password);
                try
                {
                    // Setup
                    string logFile = Application.temporaryCachePath + @"/deviceLog.txt";

                    // Download the file
                    string query = string.Format(kAPI_FileQuery, connectInfo.IP);
                    query += "?knownfolderid=LocalAppData";
                    query += "&filename=UnityPlayer.log";
                    query += "&packagefullname=" + QueryAppDetails(appName, connectInfo).PackageFullName;
                    query += "&path=%5C%5CTempState";
                    client.DownloadFile(query, logFile);

                    // Open it up in default text editor
                    System.Diagnostics.Process.Start(logFile);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError(ex.ToString());
                    return false;
                }
            }

            return true;
        }

        // Helpers
        static string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(toEncode);
            string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }
        static string DecodeFrom64(string encodedData)
        {
            byte[] encodedDataAsBytes = System.Convert.FromBase64String(encodedData);
            string returnValue = System.Text.ASCIIEncoding.ASCII.GetString(encodedDataAsBytes);
            return returnValue;
        }
    }
}
