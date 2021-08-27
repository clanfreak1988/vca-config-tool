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
        private string todoAction;
        private List<string> actionList;
        private string nextAction;

        public LuaFile(String currentAction, bool existing, string NameOfTransmission, string transmissionContent, string action) {
            todoAction = currentAction;
            exists = existing;
            transmissionName = NameOfTransmission;
            transmission = transmissionContent;
            actionList = new List<string>();
            if (exists == true) {
                actionList.Add("Exists");
                actionList.Add("Remove");
            } else {
                actionList.Add("-");
                actionList.Add("Add");
            }
           nextAction = action;
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

        public String TodoAction {
            get => todoAction;
            set => todoAction = value;
        }

        public String NextAction {
            get => nextAction;
            set => nextAction = value;
        }
        public IEnumerator<LuaFile> GetEnumerator() {
            return luaFiles.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator() {
            throw new Exception();
        }

        public IEnumerable<string> ActionList => actionList;
    }
}
