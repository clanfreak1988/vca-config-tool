using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vca_config_tool {
    public class LuaFile : IEnumerable<LuaFile> {

        private string fileName;
        private string transmissionName;
        private bool exists;
        private List<LuaFile> luaFiles;
        public LuaFile(bool existing, string NameOfTransmission, string transmissionContent) {
            fileName = transmissionContent;
            transmissionName = NameOfTransmission;
            exists = existing;
        }

        public LuaFile(bool existing, string nameOfFile) {
            exists = existing;
            transmissionName = nameOfFile;
        }

        public string TransmissionName {
            get => transmissionName;
            set => transmissionName = value;
        }
        public string FileName {
            get => fileName;
            set => fileName = value;
        }

        public bool Exists {
            get => exists;
            set => exists = value;
        }

        public IEnumerator<LuaFile> GetEnumerator() {
            return luaFiles.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator() {
            
            throw new Exception();
        }
    }
}
