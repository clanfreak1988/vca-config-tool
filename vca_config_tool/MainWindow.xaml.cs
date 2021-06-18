using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Net.Http.Headers;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;
using ListViewItem = System.Windows.Controls.ListViewItem;
using CheckBox = System.Windows.Forms.CheckBox;
using System.Windows.Media;

namespace vca_config_tool {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private readonly string currentScriptPath = AppDomain.CurrentDomain.BaseDirectory;
        private string zipFilePath = string.Empty;
        private string luaFilePath = string.Empty;
        private string zipPath = string.Empty;
        private List<LuaFile> luaAL = new List<LuaFile>();
        private List<LuaFile> checkedList = new List<LuaFile>();
        private List<LuaFile> unCheckedList = new List<LuaFile>();
        private List<LuaFile> currentTransmission = new List<LuaFile>();
        private string currentVCATransmissionFileString;
        private static WebClient wc = new WebClient();

        public MainWindow() {
            InitializeComponent();
            //wtransmissionListView.ItemsSource = luaAL;
            // Create the needed subfolder for the zip and transmission part
            //eateSubFolders();
            //ClearFolder(currentScriptPath + "VCA");

            // We unzip the file in the VCA folder and know we have this lua file
            luaFilePath = currentScriptPath + "VCA\\vehicleControlAddonTransmissions.lua";

            // Read in from all selected transmission of the combobox and iterat over it
            // TODO: Check which exists
            //var newTransmission = File.ReadAllText("C:\\Users\\Andre\\source\\repos\\vca-config-tool\\Renault_Ares_836_GIMA_Quadrishift.lua");
            Console.WriteLine("Fin");

        }

        private void DownloadButtonClick(object sender, RoutedEventArgs e) {
            //DownloadGithubTransmissionAsync();
            ReadDownloadedTransmissions();
        }

        private void SelectZipButtonClick(object sender, RoutedEventArgs e) {
            // Selecting the path to the vca zip file
           /** using (OpenFileDialog openFileDialog = new OpenFileDialog()) {
                openFileDialog.InitialDirectory = "C:\\Users\\Andre\\source\\repos";
                openFileDialog.Filter = "Zipfile|*.zip";

                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                    zipFilePath = openFileDialog.FileName;
                    zipPath = System.IO.Path.GetFullPath(zipFilePath);
                }
            }
            UnzipFile(zipFilePath);
           */
            // Read the current transmission file
            currentVCATransmissionFileString = File.ReadAllText(luaFilePath, Encoding.UTF8);
            currentTransmission = GetAllUsedTransmissions();
            DownloadBtn.IsEnabled = true;
            ZipFileBtn.IsEnabled = true;
        }

        /// <summary>
        /// Unzip the file which is give as a parameter
        /// </summary>
        /// <param name="pathToZip"></param>
        private void UnzipFile(string pathToZip) {
            Directory.CreateDirectory("VCA");
            string targetPath = currentScriptPath + "VCA\\";
            ZipFile.ExtractToDirectory(pathToZip, targetPath);
            
        }

        private void ZippingFile() {
            ZipFile.CreateFromDirectory("VCA\\", zipPath);
        }

        /// <summary>
        /// Iterate over the combobox
        /// </summary>
        /// <param name="selectedItems"></param>
        private void IterateOverSelection(List<LuaFile> selectedItems) {
            foreach (LuaFile lua in selectedItems) {
                AddTransmission(lua.FileName);
            }
        }

        /// <summary>
        /// Add every transmission to the lua file
        /// </summary>
        /// <param name="fileName"></param>
        private void AddTransmission(string fileName) {

            var result = Regex.Replace(currentVCATransmissionFileString, "{.+class.+=.+vehicleControlAddonTransmissionBase,.+\r\n.*params.=.+name.+=.+\"OWN\"", @"" + fileName + "$&");

            // Write the result to a temp file
            // TODO: In future correct target
            File.WriteAllText("C:\\Users\\Andre\\source\\repos\\vca-config-tool\\vehicleControlAddonTransmissions_edit.lua", result);
        }

        /// <summary>
        /// Collect all current Transmission
        /// </summary>
        /// <returns> A list of the current transmission</returns>
        private List<LuaFile> GetAllUsedTransmissions() {
            List<LuaFile> listTransmission = new List<LuaFile>();
            Regex regex= new Regex("params.=.+name\\s+=\\s+\"(.*)\"");
            var result = Regex.Matches(currentVCATransmissionFileString, "params.=.+name\\s+=\\s+\"(.*)\"");
            foreach (Match match in regex.Matches(currentVCATransmissionFileString)) {
                if(match.Groups[1].Value != "OWN") {
                    luaAL.Add(new LuaFile(true, match.Groups[1].Value));
                    listTransmission.Add(new LuaFile(true, match.Groups[1].Value));
                }
            }
            transmissionListView.ItemsSource = luaAL;
            return listTransmission;
        }

        /// <summary>
        /// Create the default subfolders
        /// </summary>
        private void CreateSubFolders() {
            Directory.CreateDirectory("VCA");
            Directory.CreateDirectory("tmp");
        }


        /// <summary>
        /// Cleaning the folder VCA, when maybe was a previous run
        /// </summary>
        /// <param name="FolderName"></param>
        private void ClearFolder(string FolderName) {
            DirectoryInfo dir = new DirectoryInfo(FolderName);

            foreach (FileInfo fi in dir.GetFiles()) {
                fi.Delete();
            }

            foreach (DirectoryInfo di in dir.GetDirectories()) {
                ClearFolder(di.FullName);
                di.Delete();
            }
        }

        private void ReadDownloadedTransmissions() {
            DirectoryInfo tmpDir = new DirectoryInfo(currentScriptPath + "tmp");
            foreach(FileInfo fi in tmpDir.GetFiles()) {
                luaAL.Add(new LuaFile(false, fi.Name.Split('.')[0], File.ReadAllText(fi.FullName)));
            }
            transmissionListView.ItemsSource = null;
            transmissionListView.ItemsSource = luaAL;
        }

        private void DownloadGithubTransmissionAsync() {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.UserAgent.Add(
                new ProductInfoHeaderValue("MyApplication", "1"));
            var repo = "clanfreak1988/VCA_gearbox_collection";
            var contentsUrl = $"https://api.github.com/repos/{repo}/contents";
            var contentsJson = httpClient.GetStringAsync(contentsUrl).GetAwaiter().GetResult();
            var contents = (JArray)JsonConvert.DeserializeObject(contentsJson);
            foreach (var file in contents) {
                var fileType = (string)file["type"];
                if (fileType == "dir") {
                    var directoryContentsUrl = (string)file["url"];
                    // use this URL to list the contents of the folder
                    Console.WriteLine($"DIR: {directoryContentsUrl}");
                    var contentsJson2 = httpClient.GetStringAsync(contentsUrl + "\\" + (string)file["name"]).GetAwaiter().GetResult();
                    var content = (JArray)JsonConvert.DeserializeObject(contentsJson2);
                    foreach (var file2 in content) {
                        var uri = new Uri((string)file2["download_url"]);
                        wc.DownloadFile(uri, currentScriptPath + "tmp\\" + (string)file2["name"]);
                    }
                } else if (fileType == "file") {
                    var downloadUrl = (string)file["download_url"];
                    // use this URL to download the contents of the file
                    Console.WriteLine($"DOWNLOAD: {downloadUrl}");
                }
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e) {
            ListViewItem listViewItem = GetVisualAncestor<ListViewItem>((DependencyObject)sender);

            transmissionListView.SelectedValue = transmissionListView.ItemContainerGenerator.ItemFromContainer(listViewItem);
            LuaFile selectedItem = (LuaFile)transmissionListView.SelectedItem;
            if (checkedList.Contains(selectedItem)) {
                checkedList.Remove(selectedItem);
            } else {
                checkedList.Add(selectedItem);
            }
        }
        private static T GetVisualAncestor<T>(DependencyObject o) where T : DependencyObject {
            do {
                o = VisualTreeHelper.GetParent(o);
            } while (o != null && !typeof(T).IsAssignableFrom(o.GetType()));

            return (T)o;
        }
    } 
}
