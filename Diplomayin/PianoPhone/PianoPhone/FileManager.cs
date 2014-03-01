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

        async static void SerializeAndSaveFile<T>(T source, string path)
        {
                DataContractJsonSerializer jserializer = new DataContractJsonSerializer(typeof(T));
                using (Stream destination = new MemoryStream())
                {
                    jserializer.WriteObject(destination, source);
                    await SaveFileAsync(path, destination);
                }
        }

        async static Task<Stream> DeserializeAndOpenAsync<T>(string path, FileMode mode, FileAccess access)
        {
            await OpenFileAsync(path, mode, access);
            DataContractJsonSerializer jserializer = new DataContractJsonSerializer(typeof(T));
            var source = new FileStream(path, mode, access);
            jserializer.ReadObject(source);
            return source;
        }

        static IEnumerable<PPhoto> GetPhotosAsync(string path, CancellationToken token)
        {
            return Task.Run(() =>
                {

                });
        }

        public static void GetPhotoAlbumNames()
        {
            List<string> directories = new List<string>();
            using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication())
            {
                file.GetDirectoryNames(Directories.Root + "*");
            }
        }

        static Task<bool> CreatePhotoAlbumAsync(string albumName)
        {
            return Task.Run(() =>
                {
                    using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        if (file.DirectoryExists(Path.Combine(Directories.AlbumRoot, albumName)))
                            return false;
                        file.CreateDirectory(Path.Combine(Directories.AlbumRoot, albumName));
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
                    await CreatePhotoAlbumAsync(photo.AlbumName);
                    token.ThrowIfCancellationRequested();
                }
                SerializeAndSaveFile<PPhoto>(photo, photo.Path);
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
                    file.CreateDirectory(Directories.Videos);
                    file.CreateDirectory(Directories.DefaultAlbum);
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
                    if (await CreatePhotoAlbumAsync(pictureAlbum.Name))
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
                        await CreatePhotoAlbumAsync(albumName);
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
           await Task.Run(() =>
            {
                int count = 0;
                foreach (var contact in contacts)
                {
                    SerializeAndSaveFile<PContact>(contact, contact.Path);
                    count++;
                    cancellationToken.ThrowIfCancellationRequested();
                }
                return count;
            }, cancellationToken);
        }

        async Task<IReadOnlyList<StoredContact>> GetContactsFromPhoneAsync()
        {
            ContactStore store = await ContactStore.CreateOrOpenAsync();
            ContactQueryResult result = store.CreateContactQuery();
            IReadOnlyList<StoredContact> contacts = await result.GetContactsAsync();
            return contacts;
        }

       
    }
}
