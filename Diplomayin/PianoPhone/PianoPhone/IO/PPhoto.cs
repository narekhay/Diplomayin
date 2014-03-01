using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PianoPhone.IO
{
    [DataContract]
    class PPhoto: PFile
    {
        [DataMember]
        public string AlbumName { get; set; }
    }
}
