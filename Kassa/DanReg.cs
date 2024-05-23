using MaterialSkin;
using MaterialSkin.Controls;
using Microsoft.Office.Interop.Access;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kassa
{
    public partial class DanReg : MaterialForm
    {
        public DanReg(string data, string fn)
        {
            InitializeComponent();
            
            this.data = data;
            this.fn = fn;

            string Pr_FN = "";
            string Ad_FN = "";
            string Port_FN = "";

            string str = File.ReadAllText("options_OFD.txt");
            string[] str_mas = str.Split(':');

            if ((data == "Эвотор ОФД") || (data == "ООО «Эвотор ОФД»"))
            {
                textBox1.Text = str_mas[1];
                textBox2.Text = str_mas[7];
                textBox3.Text = str_mas[9];
                textBox4.Text = str_mas[11];
                textBox5.Text = str_mas[13];

                textBox6.Text = str_mas[7];
                textBox7.Text = str_mas[15];
            }
            else if ((data == "ЭСК") || (data == "АО «ЭСК»"))
            {
                textBox1.Text = str_mas[17];
                textBox2.Text = str_mas[23];
                textBox3.Text = str_mas[25];
                textBox4.Text = str_mas[27];
                textBox5.Text = str_mas[29];

                textBox6.Text = str_mas[23];
                textBox7.Text = str_mas[31];
            }
            else if ((data == "АО Контур НТТ") || (data == "Контур НТТ"))
            {
                textBox1.Text = str_mas[33];
                textBox2.Text = str_mas[39];
                textBox3.Text = str_mas[41];
                textBox4.Text = str_mas[43];
                textBox5.Text = str_mas[45];

                textBox6.Text = str_mas[39];
                textBox7.Text = str_mas[47];
            }
            else if ((data == "Такском") || (data == "ООО «Такском»"))
            {
                textBox1.Text = str_mas[49];
                textBox2.Text = str_mas[55];
                textBox3.Text = str_mas[57];
                textBox4.Text = str_mas[59];
                textBox5.Text = str_mas[61];

                textBox6.Text = str_mas[55];
                textBox7.Text = str_mas[63];
            }
            else if ((data == "Калуга Астрал") || (data == "АО «Калуга Астрал»"))
            {
                textBox1.Text = str_mas[65];
                textBox2.Text = str_mas[71];
                textBox3.Text = str_mas[73];
                textBox4.Text = str_mas[75];
                textBox5.Text = str_mas[77];

                textBox6.Text = str_mas[71];
                textBox7.Text = str_mas[79];
            }
            else { textBox1.Text = "ОФД не определен"; }

            if (fn != "Автоматика" || fn != "Инвента")
            {
                if (fn.Length == 6) { fn = fn.Remove(fn.Length - 4); }
                if (fn.Length == 7) { fn = fn.Remove(fn.Length - 5); }
            }
            string str2 = File.ReadAllText("options_FN.txt");
            string[] str_mas2 = str2.Split(':');
            if (fn == "Инвента" || fn == "Ин" || fn == "ин" || fn == "ИН") 
                {
                    Pr_FN = str_mas2[1];
                    Ad_FN = str_mas2[3];
                    Port_FN = str_mas2[5];
                }

                else if (fn == "Автоматика" || fn == "Ав" || fn == "ав" || fn == "АВ")
                {
                Pr_FN = str_mas2[7];
                Ad_FN = str_mas2[9];
                Port_FN = str_mas2[11];
                }
                
                else { textBox8.Text = "Производитель ФН не определен"; }

            textBox8.Text = Pr_FN;
            textBox9.Text = Ad_FN;
            textBox10.Text = Port_FN;


        }
        string data;
        string fn;
        private void butClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
