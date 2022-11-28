namespace FTPPlugin {

    using FluentFTP;
    using FluentFTP.Helpers;
    using Microsoft.Dynamics.BusinessCentral.Agent.Common;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;

    [AgentPlugin("ftp/V1.0")]
    public class FTPPlugin : IAgentPlugin {
        protected string HostName { get; set; }
        protected string UserName { get; set; }
        protected string Passwd { get; set; }
        protected string RootFolder { get; set; }


        [PluginMethod("GET")]
        public string ConnectFtp(string jsonsettings) {
            string returnval = string.Empty;
            SetSettings(jsonsettings);

            try {
                Connect(this.HostName, this.UserName, this.Passwd);
            }
            catch (Exception ex) {
                returnval = string.Format("Error connecting:,{0}", ex.Message);
            }

            return JsonConvert.SerializeObject(returnval);
        }

        [PluginMethod("GET")]
        public string GetFilesFtp(string jsonsettings, string foldername) {
            string returnval = string.Empty;
            SetSettings(jsonsettings);

            try {
                returnval = GetListing(this.HostName, this.UserName, this.Passwd, foldername).ToString();
            }
            catch (Exception ex) {
                returnval = JsonConvert.SerializeObject(string.Format("Error getting files:\n,{0}", ex.Message));
            }

            return returnval;
        }

        [PluginMethod("GET")]
        public string DownloadFolderFtp(string jsonsettings, string foldername) {
            string returnval = string.Empty;
            SetSettings(jsonsettings);

            try {
                DownloadDirectory(this.HostName, this.UserName, this.Passwd, foldername);
            }
            catch (Exception ex) {
                returnval = string.Format("Error downloading folders:\n,{0}", ex.Message);
            }

            return JsonConvert.SerializeObject(returnval);
        }

        [PluginMethod("GET")]
        public string SetWorkingDirectoryFtp(string jsonsettings, string foldername) {
            string returnval = string.Empty;
            SetSettings(jsonsettings);

            try {
                SetWorkingDirectory(this.HostName, this.UserName, this.Passwd, foldername);
            }
            catch (Exception ex) {
                returnval = string.Format("Error downloading folders:\n,{0}", ex.Message);
            }

            return JsonConvert.SerializeObject(returnval);
        }

        [PluginMethod("GET")]
        public string DownloadDirectoryFtp(string jsonsettings, string foldername) {
            string returnval = string.Empty;
            SetSettings(jsonsettings);

            try {
                return (JsonConvert.SerializeObject(DownloadDirectory(this.HostName, this.UserName, this.Passwd, foldername)));
            }
            catch (Exception ex) {
                returnval = string.Format("Error downloading folders:\n,{0}", ex.Message);
            }

            return JsonConvert.SerializeObject(returnval);
        }

        [PluginMethod("GET")]
        public string DownloadFileFtp(string jsonsettings, string filearray) {
            string returnval = string.Empty;
            SetSettings(jsonsettings);

            JArray files = JArray.Parse(filearray);
            JArray failedlist = new JArray();
            try {
                foreach (JObject item in files) {
                    JToken jtoken = item.GetValue("fileName");
                    if (! DownloadFile(this.HostName, this.UserName, this.Passwd, jtoken.Value<string>())) {
                        failedlist.Add(item); 
                    }
                }
            }
            catch (Exception ex) {
                return (JsonConvert.SerializeObject(string.Format("Error downloading file:\n,{0}", ex.Message)));
            }

            return JsonConvert.SerializeObject(failedlist);
        }

        [PluginMethod("GET")]
        public void SetWorkingDirectory(string hostname, string username, string passwd, string foldername) {
            using (var conn = new FtpClient(hostname, username, passwd)) {
                conn.Connect();
                conn.SetWorkingDirectory(foldername);
            }
        }

        [PluginMethod("GET")]
        public string GetWorkingDirectory(string hostname, string username, string passwd, string foldername) {
            using (var conn = new FtpClient(hostname, username, passwd)) {
                conn.Connect();
                return JsonConvert.SerializeObject(conn.GetWorkingDirectory());
            }
        }

        internal static void Connect(string hostname, string username, string passwd) {
            using (var conn = new FtpClient(hostname, username, passwd)) {
                conn.Connect();
            }
        }

        internal bool DownloadDirectory(string hostname, string username, string passwd, string foldername) {
            using (var ftp = new FtpClient(hostname, username, passwd)) {
                ftp.Connect();

                // download a folder and all its files
                //ftp.DownloadDirectory(@"C:\website\logs\", @"/public_html/logs", FtpFolderSyncMode.Update);

                // download a folder and all its files, and delete extra files on disk
                //List<FtpResult> result = ftp.DownloadDirectory(@"C:\temp\", foldername, FtpFolderSyncMode.Mirror);
                return false;
            }
        }

        internal Boolean DownloadFile(string hostname, string username, string passwd, string filename) {
            using (var ftp = new FtpClient(hostname, username, passwd)) {
                ftp.Connect();

                // download a file
                FtpStatus res = ftp.DownloadFile(string.Format(@"c:\temp\{0}", filename), filename);
                return res.IsSuccess();
            }
        }

        internal string GetListing(string hostname, string username, string passwd, string foldername) {
            using (var conn = new FtpClient(hostname, username, passwd)) {
                const string FileTypeLbl = "F";
                const string FolderTypeLbl = "D";

                conn.Connect();

                // get a recursive listing of the files & folders in a specific folder 
                RootObject root = new RootObject {
                    items = new List<FileItem>()
                };

                foreach (var item in conn.GetListing(foldername, FtpListOption.AllFiles)) {
                    switch (item.Type) {
                        case FtpObjectType.Directory:
                            FileItem folder = new FileItem {
                                type = FolderTypeLbl,
                                fullName = item.FullName,
                                modified = conn.GetModifiedTime(item.FullName)
                            };

                            root.items.Add(folder);
                            break;

                        case FtpObjectType.File:
                            FileItem file = new FileItem {
                                type = FileTypeLbl,
                                fullName = item.FullName,
                                modified = conn.GetModifiedTime(item.FullName),
                                fileSize = conn.GetFileSize(item.FullName)
                            };

                            root.items.Add(file);
                            break;

                        case FtpObjectType.Link:
                            break;
                    }
                }
                return JsonConvert.SerializeObject(root);
            }
        }

        internal void SetSettings(string JsonString) {
            const string HostNameLbl = "hostName";
            const string UserNameLbl = "userName";
            const string PasswdLbl = "passwd";
            const string RootFolderLbl = "rootFolder";

            JObject Json = JObject.Parse(JsonString);
            this.HostName = Json.GetValue(HostNameLbl).ToString();
            this.UserName = Json.GetValue(UserNameLbl).ToString();
            this.Passwd = Json.GetValue(PasswdLbl).ToString();
            this.RootFolder = Json.GetValue(RootFolderLbl).ToString();
        }
    }

    internal class FileItem {
        public string type { get; set; }
        public string fullName { get; set; }
        public DateTime modified { get; set; }
        public long fileSize { get; set; }
    }
    internal class RootObject {
        public List<FileItem> items { get; set; }
    }
}