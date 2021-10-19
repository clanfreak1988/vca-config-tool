using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;
using ComboBox = System.Windows.Controls.ComboBox;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using ProgressBar = System.Windows.Controls.ProgressBar;
using Orientation = System.Windows.Controls.Orientation;
using System.Windows.Media.Animation;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace vca_config_tool {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private readonly string currentScriptPath = AppDomain.CurrentDomain.BaseDirectory;
        private string zipFilePath = string.Empty;
        private string luaFilePath = string.Empty;
        private string zipPath = string.Empty;
        private List<LuaFile> removeTransmissionList = new List<LuaFile>();
        private List<LuaFile> addTransmissionList = new List<LuaFile>();
        private ObservableCollection<LuaFile> currentTransmission = new ObservableCollection<LuaFile>();
        private string currentVCATransmissionFileString;
        private static WebClient wc = new WebClient();
        private readonly static string nameAndTextRegex = "params\\s+=\\s+.\\sname\\s+=\\s+\"([\\w\\d\\s-\\/\\*-\\.]*)\",.+?text\\s+=\\s+\"([\\w\\d\\s-\\/\\*-\\.]*)\"\\s*},";

        public MainWindow() {
            Uri iconUri = new Uri("pack://application:,,,/VCA_CT_2.ico", UriKind.RelativeOrAbsolute);
            this.Icon = BitmapFrame.Create(iconUri);
            InitializeComponent();

            // Create the needed subfolder for the zip and transmission part
            CreateSubFolders();
            ClearFolder(currentScriptPath + "VCA");

            // We unzip the file in the VCA folder and know we have this lua file
            luaFilePath = currentScriptPath + "VCA\\vehicleControlAddonTransmissions.lua";
            Console.WriteLine("Fin");
        }

        private void SelectZipButtonClick(object sender, RoutedEventArgs e) {
            // Selecting the path to the vca zip file
            using (OpenFileDialog openFileDialog = new OpenFileDialog()) {
                openFileDialog.InitialDirectory = "C:\\Users\\Andre\\source\\repos";
                openFileDialog.Filter = "Zipfile|*.zip";

                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                    zipFilePath = openFileDialog.FileName;
                    zipPath = System.IO.Path.GetFullPath(zipFilePath);
                }
            }
            UnzipFile(zipFilePath);

            // Read the current transmission file
            currentVCATransmissionFileString = File.ReadAllText(luaFilePath, Encoding.UTF8);
            currentTransmission = GetAllUsedTransmissions();
            transmissionListView.ItemsSource = currentTransmission;
            DownloadBtn.IsEnabled = true;
            ZipFileBtn.IsEnabled = true;
        }

        private void DownloadButtonClick(object sender, RoutedEventArgs e) {          
            DownloadGithubTransmission();
            ReadDownloadedTransmissions();
        }

        private void DoActionAndZipFile(object sender, RoutedEventArgs e) {
            IterateOverSelection();
            ZippingFile();
            tb_Progress.Text = "Zipping finish";
            ZipFileBtn.IsEnabled = false;
        }

        /// <summary>
        /// Unzip the file which is give as a parameter
        /// </summary>
        /// <param name="pathToZip"></param>
        private void UnzipFile(string pathToZip) {
            Directory.CreateDirectory("VCA");
            string targetPath = currentScriptPath + "VCA\\";
            ZipFile.ExtractToDirectory(pathToZip, targetPath);
            SelectZipBtn.IsEnabled = false;
        }

        /// <summary>
        /// Create the zip file from the unzipped file
        /// </summary>
        private void ZippingFile() {
            File.Delete(zipPath);
            ZipFile.CreateFromDirectory("VCA\\", zipPath + ".zip");
        }

        /// <summary>
        /// Iterate over the combobox
        /// </summary>
        /// <param name="selectedItems"></param>
        private void IterateOverSelection() {
            if (addTransmissionList.Count > 0) {
                string allAddTranmissions = string.Empty;
                foreach (LuaFile lua in addTransmissionList) {
                    allAddTranmissions += lua.Transmission;
                }
                AddTransmission(allAddTranmissions);
                currentVCATransmissionFileString = File.ReadAllText(luaFilePath, Encoding.UTF8);
            }
            if (removeTransmissionList.Count > 0) {
                foreach (LuaFile lua in removeTransmissionList) {
                    RemoveTransmission(lua.Transmission);
                    currentVCATransmissionFileString = File.ReadAllText(luaFilePath, Encoding.UTF8);
                }
            }
        }

        /// <summary>
        /// Add every transmission which is choosen in the dropdown (Add)
        /// </summary>
        /// <param name="fileName"></param>
        private void AddTransmission(string luaTransmissionString) {
            var result = Regex.Replace(currentVCATransmissionFileString, "{.+class.+=.+vehicleControlAddonTransmissionBase,.+\r\n.*params.=.+name.+=.+\"OWN\"", @"" + luaTransmissionString + "$&");
            File.WriteAllText(luaFilePath, result);
        }

        /// <summary>
        /// Remove every transmission which is choosen in the dropdown (Remove)
        /// </summary>
        /// <param name="luaTransmissionString"></param>
        private void RemoveTransmission(string luaTransmissionString) {
            var result = currentVCATransmissionFileString.Replace(luaTransmissionString, "");
            File.WriteAllText(luaFilePath, result);
        }

        /// <summary>
        /// Collect all current Transmission
        /// </summary>
        /// <returns> A list of the current transmission</returns>
        private ObservableCollection<LuaFile> GetAllUsedTransmissions() {
            
            ObservableCollection<LuaFile> listTransmission = new ObservableCollection<LuaFile>();
            //Regex regex = new Regex("params\\s+=\\s+.\\sname\\s+=\\s+\"([\\w\\d\\s-\\/\\*]*)\",.+?text\\s+=\\s+\"([\\w\\d\\s-\\/\\*]*)\"\\s+},", RegexOptions.Singleline);
            Regex regex = new Regex(nameAndTextRegex, RegexOptions.Singleline);
            foreach (Match match in regex.Matches(currentVCATransmissionFileString)) {
                Console.WriteLine(match.Groups[1].Value + " & " + match.Groups[2].Value);
                if(match.Groups[2].Value != "own configuration") {
                    Match matchTransmission = Regex.Match(currentVCATransmissionFileString, "(\\s+\\{\\s+class\\s+=\\s+vehicleControlAddonTransmissionBase,\\s+params\\s+=\\s+\\{\\s+name\\s+=\\s+\\\"" + match.Groups[1].Value + "\\\".*text\\s+=\\s+\\\"" + match.Groups[2].Value + "\\\"\\s+\\},)", RegexOptions.Singleline);
                    Console.WriteLine(matchTransmission.Groups[2].Value);
                    listTransmission.Add(new LuaFile("Exists", true, match.Groups[2].Value, matchTransmission.Groups[1].Value, ""));
                }
            }
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
                string fileContent = File.ReadAllText(fi.FullName);
                //Match transmissionNameMatch = Regex.Match(fileContent, "params\\s+=\\s+.\\sname\\s+=\\s+\"([\\w\\d\\s-\\/\\*-\\.]*)\",.+?text\\s+=\\s+\"([\\w\\d\\s-\\/\\*-\\.]*)\"\\s*},", RegexOptions.Singleline);
                Match transmissionNameMatch = Regex.Match(fileContent, nameAndTextRegex, RegexOptions.Singleline);
                string transmissionName = transmissionNameMatch.Groups[2].Value;
                if (!currentTransmission.Any(x => x.TransmissionName == transmissionName && transmissionName.Length > 0)) {
                    currentTransmission.Add(new LuaFile("-", false, transmissionName, File.ReadAllText(fi.FullName), ""));
                }
            }
        }

        private void DownloadGithubTransmission() {
            int i = 1;
            pb_LengthyTaskProgress.Value = 0;
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
                    pb_LengthyTaskProgress.Maximum = content.Count;
                    foreach (var file2 in content) {
                        var uri = new Uri((string)file2["download_url"]);
                        wc.DownloadFile(uri, currentScriptPath + "tmp\\" + (string)file2["name"]);
                        pb_LengthyTaskProgress.Value = i;
                        tb_Progress.Text = i + " from " + content.Count;
                        i++;
                    }
                } else if (fileType == "file") {
                    var downloadUrl = (string)file["download_url"];
                    // use this URL to download the contents of the file
                    Console.WriteLine($"DOWNLOAD: {downloadUrl}");
                }
            }
            DownloadBtn.IsEnabled = false;
        }

        private void cbAction_SelectionChanged(object sender, SelectionChangedEventArgs e) {
        }

        private void cbAction_DropDownClosed(object sender, EventArgs e) {
            ComboBox cb = sender as ComboBox;
            LuaFile currentItem = (LuaFile)cb.DataContext;
            LuaFile updateItem = (LuaFile)cb.DataContext;
            string currentNextAction = currentItem.NextAction;
            updateItem.NextAction = cb.SelectedItem.ToString();
            string updateNextAction = updateItem.NextAction;
            currentTransmission.Insert(currentTransmission.IndexOf(currentItem), updateItem);
            currentTransmission.Remove(currentItem);

            if (updateNextAction.Equals("Remove") && currentNextAction != updateNextAction) {
                removeTransmissionList.Add(updateItem);
            } else if (currentNextAction.Equals("Remove") && updateNextAction.Equals("Exists")) {
                removeTransmissionList.Remove(updateItem);
            }

            if (updateNextAction.Equals("Add") && currentNextAction != updateNextAction) {
                addTransmissionList.Add(updateItem);
            } else if (currentNextAction.Equals("Add") && updateNextAction.Equals("-")) {
                addTransmissionList.Remove(updateItem);
            }
            Console.WriteLine("Size of removeTL: " + removeTransmissionList.Count + "\nSize of addTL: " + addTransmissionList.Count);
        }
    } 
}
