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

namespace vca_config_tool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private readonly string currentScriptPath = AppDomain.CurrentDomain.BaseDirectory;
        private readonly string zipFilePath = string.Empty;
        private readonly string luaFilePath = string.Empty;

        public MainWindow() {
            InitializeComponent();

            // Create the needed subfolder for the zip and transmission part
            CreateSubFolders();

            // We unzip the file in the VCA folder and know we have this lua file
            luaFilePath = currentScriptPath + "VCA\\vehicleControlAddonTransmissions.lua";

            // Selecting the path to the vca zip file
            using (OpenFileDialog openFileDialog = new OpenFileDialog()) {
                openFileDialog.InitialDirectory = "C:\\Users\\Andre\\source\\repos";
                openFileDialog.Filter = "Zipfile|*.zip";

                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                    zipFilePath = openFileDialog.FileName;
                }
            }
            UnzipFile(zipFilePath);

            // This should filled by the downloaded transmission luas
            _ = new LuaFile("TestFile.lua", "ManualGears");

            // Read the current transmission file
            var readvCAT = File.ReadAllText(luaFilePath, Encoding.UTF8);

            // Read in from all selected transmission of the combobox and iterat over it
            // TODO: Check which exists
            var newTransmission = File.ReadAllText("C:\\Users\\Andre\\source\\repos\\vca-config-tool\\Renault_Ares_836_GIMA_Quadrishift.lua");
            var result = Regex.Replace(readvCAT, "{.+class.+=.+vehicleControlAddonTransmissionBase,.+\r\n.*params.=.+name.+=.+\"OWN\"", @"" + newTransmission + "$&");

            // Write the result to a temp file
            // TODO: In future correct target
            File.WriteAllText("C:\\Users\\Andre\\source\\repos\\vca-config-tool\\vehicleControlAddonTransmissions_edit.lua", result);

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

        /// <summary>
        /// Iterate over the combobox
        /// </summary>
        /// <param name="selectedItems"></param>
        private void IterateOverSelection(ArrayList selectedItems) {
            foreach( LuaFile lua in selectedItems ) {
                AddTransmission(lua.FileName);
            }
        }

        /// <summary>
        /// Add every transmission to the lua file
        /// </summary>
        /// <param name="fileName"></param>
        private void AddTransmission(string fileName) {

        }

        /// <summary>
        /// Create the default subfolders
        /// </summary>
        private void CreateSubFolders() {
            Directory.CreateDirectory("VCA");
            Directory.CreateDirectory("tmp");
        }
    } 
}
