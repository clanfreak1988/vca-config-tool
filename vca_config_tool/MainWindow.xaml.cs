using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Collections;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Net.Http.Headers;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace vca_config_tool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private readonly string currentScriptPath = AppDomain.CurrentDomain.BaseDirectory;
        private readonly string zipFilePath = string.Empty;
        private readonly string luaFilePath = string.Empty;
        private readonly string zipPath = string.Empty;
        private readonly List<LuaFile> luaAL= new List<LuaFile>();
        private string currentVCATransmissionFileString;

        public MainWindow() {
            InitializeComponent();
            _ = downloadGithubTransmissionAsync();

            // Create the needed subfolder for the zip and transmission part
            CreateSubFolders();
            clearFolder(currentScriptPath + "VCA");

            // We unzip the file in the VCA folder and know we have this lua file
            luaFilePath = currentScriptPath + "VCA\\vehicleControlAddonTransmissions.lua";

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
            UnzipFile("C:\\Users\\Andre\\source\\repos\\FS19_VehicleControlAddon_github.zip");
            // This should filled by the downloaded transmission luas
            LuaFile luaFile = new LuaFile("TestFile.lua", "ManualGears");

            // Read the current transmission file
            currentVCATransmissionFileString = File.ReadAllText(luaFilePath, Encoding.UTF8);

            GetAllUsedTransmissions();
            // Read in from all selected transmission of the combobox and iterat over it
            // TODO: Check which exists
            var newTransmission = File.ReadAllText("C:\\Users\\Andre\\source\\repos\\vca-config-tool\\Renault_Ares_836_GIMA_Quadrishift.lua");

            Console.WriteLine("Fin");

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
        private List<string> GetAllUsedTransmissions() {
            List<string> listTransmission = new List<string>();
            Regex regex= new Regex("params.=.+name\\s+=\\s+\"(.*)\"");
            var result = Regex.Matches(currentVCATransmissionFileString, "params.=.+name\\s+=\\s+\"(.*)\"");
            foreach (Match match in regex.Matches(currentVCATransmissionFileString)) {
                if(match.Groups[1].Value != "OWN") {
                    listTransmission.Add(match.Groups[1].Name);
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
        private void clearFolder(string FolderName) {
            DirectoryInfo dir = new DirectoryInfo(FolderName);

            foreach (FileInfo fi in dir.GetFiles()) {
                fi.Delete();
            }

            foreach (DirectoryInfo di in dir.GetDirectories()) {
                clearFolder(di.FullName);
                di.Delete();
            }
        }

        private async Task downloadGithubTransmissionAsync() {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.UserAgent.Add(
                new ProductInfoHeaderValue("MyApplication", "1"));
            var repo = "clanfreak1988/VCA_gearbox_collection";
        //https://github.com/clanfreak1988/VCA_gearbox_collection/tree/main/FS19
            var contentsUrl = $"https://api.github.com/repos/{repo}/contents";
            var contentsJson = await httpClient.GetStringAsync(contentsUrl);
            var contents = (JArray)JsonConvert.DeserializeObject(contentsJson);
            foreach (var file in contents) {
                var fileType = (string)file["type"];
                if (fileType == "dir") {
                    var directoryContentsUrl = (string)file["url"];
                    // use this URL to list the contents of the folder
                    Console.WriteLine($"DIR: {directoryContentsUrl}");
                } else if (fileType == "file") {
                    var downloadUrl = (string)file["download_url"];
                    // use this URL to download the contents of the file
                    Console.WriteLine($"DOWNLOAD: {downloadUrl}");
                }
            }
        }
    } 
}
