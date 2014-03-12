using Microsoft.Phone.Shell;
using Microsoft.Xna.Framework.Media;
using PianoPhone.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Phone.PersonalInformation;

namespace PianoPhone
{
    class FileManager
    {

        static Task<IsolatedStorageFileStream> OpenFileAsync(string path, FileMode fileMode, FileAccess fileAccess )
        {
            return Task.Run(() =>
            {
                using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    return file.OpenFile(path, fileMode, fileAccess);
                }
            });
        }

        static Task SaveFileAsync(string path, Stream data)
        {
            return Task.Run(async () =>
            {
                using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream fs = await OpenFileAsync(path, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        byte [] buffer = new byte[data.Length];
                        await data.ReadAsync(buffer,0,buffer.Length);
                        await fs.WriteAsync(buffer, 0, buffer.Length);
                    }
                }
                
            });
        }

        static Task SerializeAndSaveFile<T>(T source, string path)
        {
            return Task.Run(async () =>
                {
                    DataContractJsonSerializer jserializer = new DataContractJsonSerializer(typeof(T));
                    using (Stream destination = new MemoryStream())
                    {
                        Stream tempStream = new MemoryStream();
                        jserializer.WriteObject(tempStream, source);

                        //Write length
                        int jsonPartSize = (int)tempStream.Position;
                        byte [] jsonLengthArray = BitConverter.GetBytes(jsonPartSize);
                        await destination.WriteAsync(jsonLengthArray, 0, jsonLengthArray.Length);

                        //Write PFile
                        await tempStream.CopyToAsync(destination);

                        //Write image
                        (source as PFile).Data.Seek(0, SeekOrigin.Begin);
                        await (source as PFile).Data.CopyToAsync(destination);
                        return SaveFileAsync(path, destination);
                    }
                });
        }

        async static Task<PFile> DeserializeAndOpenAsync(string path, FileMode mode, FileAccess access)
        {
            using (Stream source = await OpenFileAsync(path, mode, access))
            {
                byte [] lengthArray = new byte[sizeof(int)];
                await source.ReadAsync(lengthArray, 0, lengthArray.Length);
                int jsonLength =  BitConverter.ToInt32(lengthArray,0);

                PFile result = new PFile();
                result.Data = new MemoryStream();
                byte[] jsonArray = new byte[jsonLength];
                await source.ReadAsync(jsonArray, (int)source.Position, jsonArray.Length);

                Stream jsonStream = new MemoryStream();
                await jsonStream.WriteAsync(buffer, 0, buffer.Length);
                DataContractJsonSerializer jserializer = new DataContractJsonSerializer(typeof(PFile));
                PFile obj = (PFile)jserializer.ReadObject(jsonStream);
                byte[] imageBuffer = new byte[str.Length - jsonPartSize];
                await str.ReadAsync(imageBuffer, (int)str.Position, imageBuffer.Length);
                obj.Data = new MemoryStream();
                await obj.Data.WriteAsync(imageBuffer, 0, imageBuffer.Length);
                //var source = new FileStream(path, mode, access);
                return obj;// (T)(jserializer.ReadObject(source));
            }
        }

         public static Task<List<PPhoto>> GetPhotosAsync(string albumName, CancellationToken token)
        {
            return  Task.Run(async () =>
                {
                    List<PPhoto> photos = new List<PPhoto>();
                    using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        try
                        {
                            string[] fileNames = store.GetFileNames(Path.Combine(Directories.AlbumRoot + albumName + "*"));
                            foreach (var fileName in fileNames)
                            {
                                string filePath = Path.Combine(Directories.AlbumRoot, albumName, fileName);
                                photos.Add((await DeserializeAndOpenAsync(filePath, FileMode.Open, FileAccess.ReadWrite)) as PPhoto);
                            }
                        }
                        catch
                        {
                            return photos;
                        }
                        return photos;
                    }
                });
        }

         public static Task<List<PFile>> GetPhotoAlbums(CancellationToken token)
         {
             return Task.Run(async () =>
                 {
                    List<PFile> albums = new List<PFile>();
                     try
                     {
                         using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                         {
                             var fileNames = store.GetFileNames(Directories.Thumbnails + "\\*");
                             foreach (var fileName in fileNames)
                             {
                                 string filePath = Path.Combine(Directories.Thumbnails, fileName);
                                 albums.Add( await DeserializeAndOpenAsync(filePath, FileMode.Open, FileAccess.ReadWrite));
                             }
                         }
                         return albums;
                     }
                     catch
                     {
                         return albums;
                     }
                 }, token);
         }

         public static Task<List<PContact>> GetContactsAsync(CancellationToken token)
         {
             return Task.Run(async () =>
             {
                 List<PContact> contacts = new List<PContact>();
                 try
                 {
                     using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                     {
                         token.ThrowIfCancellationRequested();
                         var fileNames = store.GetFileNames(Directories.Contacts + "\\*");
                         foreach (var fileName in fileNames)
                         {
                             token.ThrowIfCancellationRequested();
                             string filePath = Path.Combine(Directories.Contacts, fileName);
                             StoredContact scontact = new StoredContact(null);// (await DeserializeAndOpenAsync(filePath, FileMode.Open, FileAccess.ReadWrite)) as StoredContact;
                             token.ThrowIfCancellationRequested();
                             PContact contact = new PContact();
                             contact.Name = fileName;
                             contact.Path = filePath;
                             contact.Contact = scontact;
                             token.ThrowIfCancellationRequested();
                             contacts.Add(contact);
                         }
                     }
                     token.ThrowIfCancellationRequested();
                     return contacts;
                 }
                 catch
                 {
                     return contacts;
                 }
             }, token);
         }
        public static IEnumerable<string> GetPhotoAlbumNames()
        {
            List<string> directories = new List<string>();
            using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication())
            {
                return file.GetDirectoryNames(Path.Combine(Directories.AlbumRoot, "*"));
            }
        }

        static Task<bool> CreatePhotoAlbumAsync(string albumName, Stream thumbPicture)
        {
            return Task.Run(async () =>
                {
                    using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        if (file.DirectoryExists(Path.Combine(Directories.AlbumRoot, albumName)))
                        {
                            thumbPicture.Dispose();
                            return false;
                        }
                        file.CreateDirectory(Path.Combine(Directories.AlbumRoot, albumName));
                        PFile album = new PFile();
                        album.Name = albumName;
                        album.Path = Path.Combine(Directories.Thumbnails, albumName);
                        using(thumbPicture)
                        {
                            byte [] buffer=  new byte[thumbPicture.Length];
                            await thumbPicture.ReadAsync(buffer,0, buffer.Length);
                            album.Data = new MemoryStream();
                           await album.Data.WriteAsync(buffer, 0, buffer.Length);
                        }
                        await SerializeAndSaveFile<PFile>(album,album.Path);
                        return true;
                    }
                });
        }

        static bool PhotoAlbumExcists(string albumName)
        {
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                return store.DirectoryExists(Path.Combine(Directories.AlbumRoot, albumName));
            }
        }

        public static async Task<int> AddPhotosAsync(IEnumerable<PPhoto> photos, CancellationToken token)
        {
            return await Task.Run(async () =>{
                int count = 0;
            foreach (var photo in photos)
            {
                if (!PhotoAlbumExcists(photo.AlbumName))
                {
                    await CreatePhotoAlbumAsync(photo.AlbumName, photo.Data);
                    token.ThrowIfCancellationRequested();
                }
                await SerializeAndSaveFile<PPhoto>(photo, photo.Path);
                count++;
                token.ThrowIfCancellationRequested();
            }
            return count;
            },token);
        }

        public static void CreateDefaultDirectories()
        {
            using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (file.DirectoryExists(Directories.Root))
                {
                    return;
                }
                else
                {
                    file.CreateDirectory(Directories.Root);
                    file.CreateDirectory(Directories.AlbumRoot);
                    file.CreateDirectory(Directories.DefaultAlbum);
                    file.CreateDirectory(Directories.Documents);
                    file.CreateDirectory(Directories.DefaultAlbum);
                    file.CreateDirectory(Directories.Thumbnails);
                }
            }
        }


      public async static void SaveItems(List<Models.CollectionControlModel> list)
        {
            var data=  list.First().Data;
            if (data is PictureAlbum)
            {
                foreach (var album in list)
                {
                    var pictureAlbum = album.Data as PictureAlbum;
                    if (await CreatePhotoAlbumAsync(pictureAlbum.Name, pictureAlbum.Pictures.First().GetThumbnail()))
                    {
                        List<PPhoto> photos = new List<PPhoto>();
                        foreach (var picture in pictureAlbum.Pictures)
                        {
                            PPhoto photo = new PPhoto();
                            photo.AlbumName = pictureAlbum.Name;
                            photo.Name = picture.Name;
                            photo.Path = Path.Combine(Directories.AlbumRoot, photo.AlbumName, photo.Name);
                            await Task.Run(async () =>
                            {
                                using (var source = picture.GetImage())
                                {
                                    byte[] arr = new byte[source.Length];
                                    await source.ReadAsync(arr, 0, arr.Length);
                                    photo.Data = new MemoryStream();
                                    await photo.Data.WriteAsync(arr, 0, arr.Length);
                                }
                            });
                            photos.Add(photo);
                        }
                        await FileManager.AddPhotosAsync(photos, CancellationToken.None);
                    }
                    else
                    {
                        ShellToast toast = new ShellToast();
                        toast.Content = "Album is already imported";
                    }
                }
            }
            else
            {
                if (data is Picture)
                {
                    string albumName = (data as Picture).Album.Name;
                    if (!PhotoAlbumExcists(albumName))
                    {
                        await CreatePhotoAlbumAsync(albumName,(data as Picture).GetThumbnail());
                    }
                    List<PPhoto> photos = new List<PPhoto>();
                    foreach (var p in list)
                    {
                        Picture picture = p.Data as Picture;
                        PPhoto photo = new PPhoto();
                        photo.AlbumName = albumName;
                        photo.Name = picture.Name;
                        photo.Path = Path.Combine(Directories.AlbumRoot, photo.AlbumName, photo.Name);
                        await Task.Run(async () =>
                        {
                            using (var source = picture.GetImage())
                            {
                                byte[] arr = new byte[source.Length];
                                await source.ReadAsync(arr, 0, arr.Length);
                                photo.Data = new MemoryStream();
                                await photo.Data.WriteAsync(arr, 0, arr.Length);
                            }
                        });
                        photos.Add(photo);
                    }
                    await FileManager.AddPhotosAsync(photos, CancellationToken.None);
                }
                else
                {
                    if (data is StoredContact)
                    {
                        List<PContact> contacts = new List<PContact>();
                        foreach (var c in list)
                        {
                            var contact = c.Data as StoredContact;
                            PContact pc  = new PContact();
                            pc.Name = contact.DisplayName;
                            pc.Path = Path.Combine(Directories.Contacts, pc.Name);
                            pc.Contact = contact;
                            contacts.Add(pc);
                        }
                        FileManager.AddContactsAsync(contacts, CancellationToken.None);
                    }
                }
            }
        }

        async static void AddContactsAsync(IEnumerable<PContact> contacts, CancellationToken cancellationToken)
        {
           await Task.Run(async () =>
            {
                int count = 0;
                foreach (var contact in contacts)
                {
                    await SerializeAndSaveFile<PContact>(contact, contact.Path);
                    count++;
                    cancellationToken.ThrowIfCancellationRequested();
                }
                return count;
            }, cancellationToken);
        }

        public static async Task<IReadOnlyList<StoredContact>> GetContactsFromPhoneAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            ContactStore store = await ContactStore.CreateOrOpenAsync();
            token.ThrowIfCancellationRequested();
            ContactQueryResult result = store.CreateContactQuery();
            token.ThrowIfCancellationRequested();
            IReadOnlyList<StoredContact> contacts = await result.GetContactsAsync();
            token.ThrowIfCancellationRequested();
            return contacts;
        }
       
    }
}
