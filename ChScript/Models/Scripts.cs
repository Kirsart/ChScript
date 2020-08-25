using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChScript.Models
{
    /// <summary>
    /// модель записи обрабатываемой инфы по скриптам
    /// </summary>
    class Scripts
    {
        private string _nameScript;
        private bool _isMark;
        private string _statusScript;

        public Scripts(string _nameScript, string _statusScript)
        {
            NameScript = _nameScript;
            StatusScript = _statusScript;
        }

        
        public bool IsMark
        {
            get { return _isMark; }
            set { _isMark = value; }
        }
        public string NameScript
        {
            get { return _nameScript; }
            set { _nameScript = value; }
        }

        public string StatusScript
        {
            get { return _statusScript; }
            set { _statusScript = value; }
        }

    }
}
