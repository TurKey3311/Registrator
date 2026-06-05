using MaterialSkin.Controls;
using Registrator.repo.models;
using Registrator.services.common;
using Registrator.ui.components;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Windows.Forms;
using Word = Microsoft.Office.Interop.Word;


namespace Kassa
{
    
    class CreateAkt
    {
        private FileInfo _fileInfo;
        // Получаем строку подключения из App.config
        public string connectionString = ConfigurationManager.ConnectionStrings["SQLiteDB"].ConnectionString;
        public string ID_client = "";
        public string NameOrganization = "";
        private SettingsProgram settings;
        ErrorSnackbar errorSnackbar = new ErrorSnackbar();

        public CreateAkt(string fileName)
        {
            if (File.Exists(fileName))
            {
                _fileInfo = new FileInfo(fileName);
            }
            else
            {
                throw new ArgumentException("Файл не найден");
            }
        }

        internal bool Process(Dictionary<string, string> items, SettingsProgram _setting)
        {
            settings = _setting;

            Word.Application app = new Word.Application();
            try
            {
                Object file = _fileInfo.FullName;
                Object missing = Type.Missing;
                app.Documents.Open(file);
                foreach (var item in items)
                {
                    Word.Find find = app.Selection.Find;
                    find.Text = item.Key;
                    find.Replacement.Text = item.Value;
                    if (find.Text == "<ID_Client>")
                    {
                        ID_client = find.Replacement.Text;
                    }
                    if (find.Text == "<NameOrganization>")
                    {
                        NameOrganization = find.Replacement.Text;
                    }
                    Object wrap = Word.WdFindWrap.wdFindContinue;
                    Object replace = Word.WdReplace.wdReplaceAll;
                    find.Execute(FindText: Type.Missing,
                        MatchCase: false,
                        MatchWholeWord: false,
                        MatchWildcards: false,
                        MatchSoundsLike: missing,
                        MatchAllWordForms: false,
                        Forward: true,
                        Wrap: wrap,
                        Format: false,
                        ReplaceWith: missing,
                        Replace: replace);
                }
                var fileContent = string.Empty;
                var filePath = string.Empty;
                string[] zap_znak = { "\"", "\\", "/", ":", "*", "?", "<", ">", "|", "\"" };
                for (int i = 0; i < zap_znak.Length; i++)
                {
                    NameOrganization = NameOrganization.Replace(zap_znak[i], "");
                }
                if (NameOrganization == "")
                {
                    NameOrganization = "Пустое название";
                }
                if (ID_client == "")
                {
                    ID_client = "ID";
                }
                Object newFileName = Path.Combine(
                    Folder.CreateDirectoryNameBase(settings.AdressFile, ID_client, NameOrganization),
                    "Акт ввода " + Folder.CreateFileName(ID_client, NameOrganization) + ".docx"
                );
                app.ActiveDocument.SaveAs2(newFileName);
                if (settings.PrintAkt == true)
                {
                    app.PrintOut();
                }
            }
            catch (Exception ex)
            {
                Object newFileName = Path.Combine(
                    Folder.CreateDirectoryNameBase(settings.AdressFile, ID_client, NameOrganization),
                    "Акт ввода " + Folder.CreateFileName(ID_client, NameOrganization) + ".docx"
                );
                app.ActiveDocument.SaveAs2(newFileName);
                MaterialMessageBox.Show("Ошибка",ex.Message);
            }
            finally
            {               
                app.ActiveDocument.Close();
                app.Quit();
            }
            
            return false;
        }
    }
}
