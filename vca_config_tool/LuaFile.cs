using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vca_config_tool {
    public class LuaFile : IEnumerable<LuaFile> {

        private string transmission;
        private string transmissionName;
        private bool exists;
        private List<LuaFile> luaFiles;

        public LuaFile(string action, bool existing, string NameOfTransmission, string transmissionContent) {
            TodoAction = action;
            exists = existing;
            transmissionName = NameOfTransmission;
            transmission = transmissionContent;
        }

        public LuaFile(bool existing, string nameOfFile) {
            exists = existing;
            transmissionName = nameOfFile;
        }

        public string TransmissionName {
            get => transmissionName;
            set => transmissionName = value;
        }
        public string Transmission {
            get => transmission;
            set => transmission = value;
        }

        public bool Exists {
            get => exists;
            set => exists = value;
        }

        public string TodoAction { get; set; }

        public IEnumerator<LuaFile> GetEnumerator() {
            return luaFiles.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator() {
            
            throw new Exception();
        }
    }
}
