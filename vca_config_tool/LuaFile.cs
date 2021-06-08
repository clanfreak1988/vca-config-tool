using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vca_config_tool {
    public class LuaFile {

        private string fileName;
        private string transmissionName;
        public LuaFile(string nameOfFile, string NameOfTransmission) {
            fileName = nameOfFile;
            transmissionName = NameOfTransmission;
        }

        public string TransmissionName {
            get => transmissionName;
            set => transmissionName = value;
        }
        public string FileName {
            get => fileName;
            set => fileName = value;
        }
    }
}
