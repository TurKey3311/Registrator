using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Word = Microsoft.Office.Interop.Word;

namespace Kassa
{
    internal class Addnew
    {
        private FileInfo _fileInfo;

        public Addnew(string fileName)
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
        internal bool Process(Dictionary<string, string> itens)
        {
            try
            {
                var app = new Word.Application();
                Object file = _fileInfo.FullName;

                Object missing = Type.Missing;

                app.Documents.Open(file);
                foreach (var iten in itens)
                {
                    Word.Find find = app.Selection.Find;
                    find.Text = iten.Key;
                    find.Replacement.Text = iten.Value;

                    Object wrap = Word.WdFindWrap.wdFindContinue;
                    Object replase = Word.WdReplace.wdReplaceAll;

                    find.Execute(
                        FindText: Type.Missing,
                        MatchCase: false,
                        MatchWholeWord: false,
                        MatchWildcards: false,
                        MatchSoundsLike: missing,
                        MatchAllWordForms: false,
                        Forward: true,
                        Wrap: wrap,
                        Format: false,
                        ReplaceWith: missing, Replace: replase);
                }
                
                Object newFileName = Path.Combine("C:\\Users\\Пользователь\\Downloads","Акт ввода в эксплуатацию");
                app.ActiveDocument.SaveAs2(newFileName);
                app.ActiveDocument.Close();
                app.Quit();
                return true;
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            finally
            {
               
            }
            return false;
        }
    }
}
