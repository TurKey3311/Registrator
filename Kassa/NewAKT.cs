using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Word = Microsoft.Office.Interop.Word;


namespace Kassa
{
    class NewAKT
    {
        private FileInfo _fileInfo;

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

        internal bool Process(Dictionary<string, string> items)
        {
            Word.Application app = null;

            
            try
            {
                app = new Word.Application();
                Object file = _fileInfo.FullName;
                Object missing = Type.Missing;
                app.Documents.Open(file);
                foreach (var item in items )
                {
                    Word.Find find = app.Selection.Find;
                    find.Text = item.Key;
                    find.Replacement.Text = item.Value;

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
                        ReplaceWith: missing, Replace: replace);
                }
                var fileContent = string.Empty;
                var filePath = string.Empty;
                string adr_file = File.ReadAllText("adr_file.txt");


                string adr_file_save = null;

                FolderBrowserDialog Browserdialog = new FolderBrowserDialog(); //открытие проводника и выбор папки сохранения
                Browserdialog.RootFolder = Environment.SpecialFolder.Desktop; // открытие папки по умолчанию
                Browserdialog.SelectedPath = adr_file.Remove(adr_file.Length - 2);
                if (Browserdialog.ShowDialog() == DialogResult.OK)
                {
                    adr_file_save = Browserdialog.SelectedPath + "\\";
                }
                //= File.ReadAllText("adr_file.txt");
                //adr_file = adr_file.Remove(adr_file.Length - 3);
                Object newFileName = Path.Combine(adr_file_save, "Акт ввода.docx");
                app.ActiveDocument.SaveAs2(newFileName);
                app.ActiveDocument.Close();
                app.Quit();
                return true;
            }
            catch(Exception ex)
            { 
              Console.WriteLine(ex.Message); 
            }
            finally
            {
                
                if (app != null)
                {
                    app.Quit();
                }
                
            }
            return false;

        }
    }
}
