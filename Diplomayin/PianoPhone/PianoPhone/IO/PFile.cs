using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PianoPhone.IO
{
    [KnownType(typeof(MemoryStream))]
    [DataContract]
    class PFile
    {
        public MemoryStream Data { get; set; }

        [DataMember]
        public string Path { get; set; }

        [DataMember]
        public string Name { get; set; }
    }
}
