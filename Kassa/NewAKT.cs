using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Word = Microsoft.Office.Interop.Word;


namespace Kassa
{
    
    class NewAKT
    {
        private FileInfo _fileInfo;
        // Получаем строку подключения из App.config
        public string connectionString = ConfigurationManager.ConnectionStrings["SQLiteDB"].ConnectionString;
        public string adr_file_save = "";
        public string ID_client = "";
        public string NameOrganization = "";
        public string NameOrganization_save = "";
        
        public string adr_file = "";

        public NewAKT(string fileName)
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

        internal bool Process(Dictionary<string, string> items, string adr_file, string print_akt)
        {
            //Создание подключения к базе данных
            //using (var sqliteConnection = new SQLiteConnection(connectionString))
            //{
            //    Открываем соединение
            //    sqliteConnection.Open();
            //    try
            //    {

            //        запрос Адреса сохранения по умолчанию
            //        string selectQuery = "SELECT * FROM options_program WHERE parameter = @adr_file";
            //        using (SQLiteCommand command = new SQLiteCommand(selectQuery, sqliteConnection))
            //        {
            //            command.Parameters.AddWithValue("@adr_file", "adr_file");
            //            using (SQLiteDataReader reader = command.ExecuteReader())
            //            {
            //                while (reader.Read())   // построчно считываем данные
            //                {
            //                    adr_file = (string)reader["meaning"];
            //                }
            //            }
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        MaterialMessageBox.Show("Ошибка: " + ex.Message);
            //    }
            //}

            adr_file_save = adr_file;

            FolderBrowserDialog Browserdialog = new FolderBrowserDialog(); //открытие проводника и выбор папки сохранения
            Browserdialog.RootFolder = Environment.SpecialFolder.Desktop; // открытие папки по умолчанию
            Browserdialog.SelectedPath = adr_file;
            if (Browserdialog.ShowDialog() == DialogResult.OK)
            {
                adr_file_save = Browserdialog.SelectedPath + "\\";

                Word.Application app = null;
                try
                {
                    app = new Word.Application();
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
                    string[] zap_znak = { "\\", "/", ":", "*", "?", "<", ">", "|", "\"" };
                    NameOrganization_save = NameOrganization;
                    if (NameOrganization_save != "")
                    {
                        for (int i = 0; i < zap_znak.Length; i++)
                        {
                            NameOrganization_save = NameOrganization_save.Replace(zap_znak[i], "");
                        }
                    }
                    Object newFileName = Path.Combine(adr_file_save, "Акт ввода " + ID_client + " " + NameOrganization_save + ".docx");
                    app.ActiveDocument.SaveAs2(newFileName);
                    
                    MaterialMessageBox.Show(
            "Акт ввода сформирован и сохранен",
            "Сообщение");
                }
                catch (Exception ex)
                {
                    Object newFileName = Path.Combine(adr_file_save, "Акт ввода " + ID_client + " " + NameOrganization_save + ".docx");
                    app.ActiveDocument.SaveAs2(newFileName);
                    MaterialMessageBox.Show(ex.Message);
                }
                finally
                {
                    if (print_akt == "true") { app.PrintOut(); }                    
                    app.ActiveDocument.Close();
                    app.Quit();
                }
            }
            return false;
        }
    }
}
