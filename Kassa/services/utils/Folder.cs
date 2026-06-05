using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registrator.services.common
{
    class Folder
    {
        public static string CreateAdress(string folderBase, string id, string nameOrganization)
        {
            nameOrganization = RemoveInvalidChars(nameOrganization);
            return folderBase + "//" + id + " " + nameOrganization;
        }
        public static string CreateFileName(string id, string nameOrganization)
        {
            nameOrganization = RemoveInvalidChars(nameOrganization);
            return id + " " + nameOrganization;
        }
        public static string CreateDirectoryNameBase(string folderBase, string id, string nameOrganization)
        {
            string adressFile = CreateAdress(folderBase, id, nameOrganization);
            Directory.CreateDirectory(adressFile);
            return adressFile;
        }

        public static string RemoveInvalidChars (string nameOrganization)
        {
            string[] zap_znak = { "\\", "/", ":", "*", "?", "<", ">", "|", "\"" };
            if (nameOrganization != "")
            {
                for (int i = 0; i < zap_znak.Length; i++)
                {
                    nameOrganization = nameOrganization.Replace(zap_znak[i], "");
                }
            }
            return nameOrganization;
        }

    }
}
