using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Phone.PersonalInformation;

namespace PianoPhone.IO
{
    [DataContract]
    class PContact:PFile
    {
        [DataMember]
        public StoredContact Contact { get; set; }
    }
}
