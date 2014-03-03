using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PianoPhone
{
    class Directories
    {
        public static string Root = "\\PrivateData\\";
        public static string AlbumRoot = Root + "Albums";
        public static string DefaultAlbum = AlbumRoot + "\\DefaultAlbum";
        public static string Contacts = Root + "Contacts";
        public static string Documents = Root + "Docs";
        public static string Thumbnails = Root + "Thumbnails";
    }
}
