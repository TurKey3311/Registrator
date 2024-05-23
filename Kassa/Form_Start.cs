using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using KitCashProtocol;
using MaterialSkin;
using MaterialSkin.Controls;
using Microsoft.Office.Interop.Word;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using Microsoft.Data.SqlClient;
using System.Net;
using Microsoft.Data.Sqlite;
using System.IO.Compression;

namespace Kassa
{
    public partial class Form_Start : MaterialForm
    {
        public bool Internet_status = false;
        public int t = 0;
        public bool save_closing = false;
        public string nameDB = "RegistratorDB.db";
        public bool delete_xml;
        private TerminalFA CashRegister { get; set; }
        static SqliteConnection connection;
        static SqliteCommand command;

        //static public bool Connect(string nameDB)
        //{
        //    try
        //    {
        //        connection = new SqliteConnection("Data Source=" + nameDB + ";Version=3; FailIfMissing=False");
        //        connection.Open();
        //        return true;
        //    }
        //    catch (SqliteException ex)
        //    {
        //        Console.WriteLine($"Ошибка доступа к базе данных. Исключение: {ex.Message}");
        //        return false;
        //    }
        //}
        public Form_Start()
        {
            InitializeComponent();

            //if (Connect(nameDB))
            //{
            //    materialLabel17.Text = "Connected";
            //}


            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Indigo500, Primary.Indigo700, Primary.Amber50, Accent.Indigo400, TextShade.WHITE);

            string str2 = System.IO.File.ReadAllText("options_OFD.txt");
            string[] str_mas2 = str2.Split(':');
            materialTextBox25.Text = str_mas2[3]; // ИНН ОФД на второй странице

            string curvers = Assembly.GetExecutingAssembly().GetName().Version.ToString(); // Заполнение версии
            labelVers1.Text = curvers;
            labelVers2.Text = curvers;
            labelVers3.Text = curvers;
            labelVers4.Text = curvers;

            string OFD = materialComboBox1.Text;
            string str = File.ReadAllText("options_OFD.txt");
            string[] str_mas = str.Split(':');

            string INN_OFD = "";
            string email_OFD = "";
            string adr_OFD = "";
            string IP = "";
            string TCP = "";
            string DNS = "";
            string port = "";


            if ((OFD == str_mas[1].Trim()))
            {
                INN_OFD = str_mas[3].Trim();
                email_OFD = str_mas[5].Trim();
                adr_OFD = str_mas[7].Trim();
                IP = str_mas[9].Trim();
                TCP = str_mas[11].Trim();
                DNS = str_mas[13].Trim();
                port = str_mas[15].Trim();

            }
            if ((OFD == str_mas[17].Trim()))
            {
                INN_OFD = str_mas[19].Trim();
                email_OFD = str_mas[21].Trim();
                adr_OFD = str_mas[23].Trim();
                IP = str_mas[25].Trim();
                TCP = str_mas[27].Trim();
                DNS = str_mas[29].Trim();
                port = str_mas[31].Trim();
            }
            if ((OFD == str_mas[33].Trim()))
            {
                INN_OFD = str_mas[35].Trim();
                email_OFD = str_mas[37].Trim();
                adr_OFD = str_mas[39].Trim();
                IP = str_mas[41].Trim();
                TCP = str_mas[43].Trim();
                DNS = str_mas[45].Trim();
                port = str_mas[47].Trim();
            }
            if ((OFD == str_mas[49].Trim()))
            {
                INN_OFD = str_mas[51].Trim();
                email_OFD = str_mas[53].Trim();
                adr_OFD = str_mas[55].Trim();
                IP = str_mas[57].Trim();
                TCP = str_mas[59].Trim();
                DNS = str_mas[61].Trim();
                port = str_mas[63].Trim();
            }
            if ((OFD == str_mas[65].Trim()))
            {
                INN_OFD = str_mas[67].Trim();
                email_OFD = str_mas[69].Trim();
                adr_OFD = str_mas[71].Trim();
                IP = str_mas[73].Trim();
                TCP = str_mas[75].Trim();
                DNS = str_mas[77].Trim();
                port = str_mas[79].Trim();
            }
            materialTextBox31.Text = Convert.ToString(INN_OFD);
            materialTextBox32.Text = Convert.ToString(email_OFD);
            materialTextBox33.Text = Convert.ToString(adr_OFD);
            materialTextBox34.Text = Convert.ToString(IP);
            materialTextBox35.Text = Convert.ToString(TCP);
            materialTextBox36.Text = Convert.ToString(DNS);
            materialTextBox37.Text = Convert.ToString(adr_OFD);
            materialTextBox38.Text = Convert.ToString(port);

            str = "";

            str = File.ReadAllText("options_FN.txt");
            str_mas = str.Split(':');

            string adr = "";
            string port_fn = "";


            string FN = materialComboBox2.Text;
            if ((FN == str_mas[1].Trim()))
            {
                adr = str_mas[3].Trim();
                port_fn = str_mas[5].Trim();
            }
            if ((FN == str_mas[7].Trim()))
            {
                adr = str_mas[9].Trim();
                port_fn = str_mas[11].Trim();
            }

            materialTextBox39.Text = adr;
            materialTextBox310.Text = port_fn;
            materialTextBox212.Text = comboBox1.Text;

            str = System.IO.File.ReadAllText("options_OFD.txt"); // заполнение по умолчанию параметров ОФД Эвотор
            str_mas = str.Split(':');
            textBox4.Text = str_mas[3];
            textBox20.Text = str_mas[5];

            string str_del = System.IO.File.ReadAllText("del_xml.txt"); //кнопка настроек удаление файла xml
            if(str_del == "true")
            {
                delete_xml = true;
            }

            CashRegister = new TerminalFA("COM1"); // вводные для Терминала-ФА
            //ErrorCode answer = CashRegister.Initialize();
            //textBox1.Text += EnumHelper.GetTypeDescription(answer) + Environment.NewLine;
        }

        private void NameOr_TextChanged(object sender, EventArgs e) // открытие поля КПП если ЮЛ и ввод имя руководителя
        {
            string[] n = textBox7.Text.Split(' ');
            string NOrganization = n[0];
            if (NOrganization != "ИП" && NOrganization.Length > 2)
            {
                textBox15.Visible = true; // открытие поля КПП
            }
            else if (NOrganization == "ИП")
            {
                textBox15.Visible = false;
                textBox8.Text = textBox7.Text.Substring(Math.Max(0, 3)); // ввод имя руководителя
            }
            
        }
        private void OFD_TextChanged(object sender, EventArgs e) // заполнение полей ИНН ОФД и почта отправителя
        {
            string INN_OFD = "";
            string mail_OFD = "";

            string str = System.IO.File.ReadAllText("options_OFD.txt");
            string[] str_mas = str.Split(':');
            string OFD = comboBox1.Text;
            if ((OFD == "Эвотор ОФД") || (OFD == "ООО «Эвотор ОФД»"))
            {

                INN_OFD = str_mas[3];
                mail_OFD = str_mas[5];
            }
            if ((OFD == "ЭСК") || (OFD == "АО «ЭСК»"))
            {

                INN_OFD = str_mas[19];
                mail_OFD = str_mas[21];
            }
            if ((OFD == "АО Контур НТТ") || (OFD == "Контур НТТ"))
            {

                INN_OFD = str_mas[35];
                mail_OFD = str_mas[37];
            }
            if ((OFD == "Такском") || (OFD == "ООО «Такском»"))
            {

                INN_OFD = str_mas[51];
                mail_OFD = str_mas[53];
            }
            if ((OFD == "Калуга Астрал") || (OFD == "АО «Калуга Астрал»"))
            {

                INN_OFD = str_mas[67];
                mail_OFD = str_mas[69];
            }
            textBox4.Text = INN_OFD;
            textBox20.Text = mail_OFD;
            materialTextBox212.Text = comboBox1.Text;

        }
        private void NAvtomat_TextChanged(object sender, EventArgs e) // заполнение номера автомата
        {
            textBox21.Text = textBox3.Text.Substring(Math.Max(0, textBox3.Text.Length - 6));
        }
        private void Internet_Click(object sender, EventArgs e) //выпадающий адрес Интернет
        {
            if ((Internet_status == false) && (checkBox12.Checked == false))
            {
                textBox6.Text = textBox14.Text;
                Internet_status = true;
            }
            else
            {
                textBox6.Clear();
                Internet_status = false;
            }

        }
        private void RNM_Changed(object sender, EventArgs e) //автоудаление пробелов в РНМ
        {
            textBox2.Text = textBox2.Text.Replace(" ", "");
        }
        private void Telephone_Changet(object sender, EventArgs e) //автоудаление символом из Номера телефона
        {
            //string Telephone = textBox11.Text;
            //Telephone = Telephone.Replace(" ", "");
            //Telephone = Telephone.Replace("-", "");
            //Telephone = Telephone.Replace("(", "");
            //Telephone = Telephone.Replace(")", "");
            //if ((Telephone.Length == 12) && (Telephone.Substring(0,2) == "+7"))
            //{
            //    Telephone = Telephone.Replace("+7", "");
            //}
            //if ((Telephone.Length == 11) && (Telephone.Substring(0, 2) == "8"))
            //{
            //    Telephone = Telephone.Replace("8", "");
            //}
            //textBox11.Text = Telephone;
        }
        private void Internet_Checked(object sender, EventArgs e) // проверка одновременной развозной торговли и интернет
        {
            if ((checkBox10.Checked == true)&&(checkBox12.Checked == true))
            {
                MaterialMessageBox.Show(
                    "Запрещено отмечать в параметрах регистрации одновременно развозную торговлю и применение ККТ в сети Интернет. Измените выбор параметров",
                "Оповещение");
            } 
        }
        private void Razvoz_Checked(object sender, EventArgs e) // проверка одновременной развозной торговли и интернет
        {
            if ((checkBox10.Checked == true) && (checkBox12.Checked == true))
            {
                MaterialMessageBox.Show(
                    "Запрещено отмечать в параметрах регистрации одновременно развозную торговлю и применение ККТ в сети Интернет. Измените выбор параметров",
                "Оповещение");
            }
        }

        private void buttonParOFD_Click(object sender, EventArgs e) // кнопка Параметры ОФД
        {
            DanReg f = new DanReg(this.comboBox1.Text, this.comboBox2.Text);
            f.ShowDialog();
        }
        public void butSave_Click(object sender, EventArgs e) // кнопка Сохранить
        {
            string DandT_FD = textBox17.Text;
            string D_FD = textBox17.Text.Substring(0,10);
            //if (D_FD[2] != '.' || D_FD[5] != '.' || D_FD[10] != ' ' || D_FD[13] != ':')
            //{
            //MaterialMessageBox.Show(
            //"Проверьте формат ввода даты и времени (дд.мм.гггг чч:мм)",
            //"Внимательнее");


            //}
            //else
            //{
                string ID_Сlient = textBox1.Text;
                string RNM = textBox2.Text;
                string ZN_KKT = textBox3.Text;
                string N_av = textBox21.Text;
                string N_FN = textBox5.Text;
                string M_FN = comboBox2.Text;
                string NameOrganization = textBox7.Text;
                string Director_org = textBox8.Text;
                string INN_Organization = textBox9.Text;
                string KPP_Organization = textBox15.Text;
                string Telephone = textBox11.Text;
                string Email = textBox12.Text;
                string Address_ras = textBox13.Text;
                string Place_ras = textBox14.Text;
                string OFD = comboBox1.Text;
                string INN_OFD = textBox4.Text;
                string T_FD = textBox17.Text.Substring(Math.Max(0, textBox17.Text.Length - 5)); // нахождение даты ФД
                string N_FD = TextBoxNFD.Text;
                string FP = textBox19.Text;
                string Model_KKT = textBox18.Text;
                string Adr_Internet = textBox6.Text;

                string SNO_OSN = " ";
                string SNO_USN_D = " ";
                string SNO_USN_D_R = " ";
                string SNO_PATENT = " ";
                string SNO_ESHN = " ";
                if (materialCheckbox11.Checked == true) { SNO_OSN = "ОСН"; }
                if (materialCheckbox10.Checked == true) { SNO_USN_D = "УСН Доход"; }
                if (materialCheckbox9.Checked == true) { SNO_USN_D_R = "УСН Доход - расход"; }
                if (materialCheckbox8.Checked == true) { SNO_PATENT = "Патент"; }
                if (materialCheckbox12.Checked == true) { SNO_ESHN = "ЕСХН"; }


                string PrAvtonom = "2"; // сведения регистрации ККТ
                string PrLotereya = "2";
                string PrAzart = "2";
                string PrBankPlat = "2";
                string PrPlatAgent = "2";
                string PrAvtomatUstr = "2";
                string PrInternet = "2";
                string PrRazvoz = "2";
                string PrAkxizTovar = "2";
                string PrMark = "2";


                if (checkBox9.Checked == true) { PrAvtonom = "1"; } // сведения регистрации ККТ
                if (checkBox3.Checked == true) { PrAzart = "1"; }
                if (checkBox2.Checked == true) { PrMark = "1"; }
                if (checkBox6.Checked == true) { PrBankPlat = "1"; }
                if (checkBox7.Checked == true) { PrPlatAgent = "1"; }
                if (checkBox4.Checked == true) { PrLotereya = "1"; }
                if (checkBox12.Checked == true) { PrInternet = "1"; }
                if (checkBox10.Checked == true) { PrRazvoz = "1"; }
                if (checkBox1.Checked == true) { PrAkxizTovar = "1"; }
                if (checkBox5.Checked == true) { PrAvtomatUstr = "1"; }

                Save s = new Save();
                s.setValues(D_FD, ID_Сlient, RNM, ZN_KKT, N_av, N_FN, M_FN, NameOrganization,
            Director_org, INN_Organization, KPP_Organization, SNO_OSN, SNO_USN_D, SNO_USN_D_R, SNO_PATENT, SNO_ESHN,
            Telephone, Email, Address_ras, Place_ras,
            OFD, INN_OFD, T_FD, N_FD, FP, Model_KKT, Adr_Internet, PrAvtonom, PrLotereya, PrAzart, PrBankPlat,
            PrPlatAgent, PrAvtomatUstr, PrInternet, PrRazvoz, PrAkxizTovar, PrMark);

            //}
        }
        private void butLoading_Click(object sender, EventArgs e)// кнопка Открыть
        {
            string str = "";
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (Path.GetExtension(ofd.FileName).ToUpper().ToLower().Equals(".txt", StringComparison.CurrentCultureIgnoreCase))
                    {
                        str = System.IO.File.ReadAllText(ofd.FileName);
                    }
                }


            }
            string[] str_mas = str.Split('#');
            if (str_mas.Length > 70)
            {
                textBox3.Text = str_mas[1].Trim();
                textBox21.Text = str_mas[5].Trim();
                textBox18.Text = str_mas[3].Trim();
                textBox5.Text = str_mas[7].Trim();
                comboBox2.Text = str_mas[9].Trim();
                textBox1.Text = str_mas[11].Trim();
                textBox7.Text = str_mas[13].Trim();
                textBox8.Text = str_mas[15].Trim();
                textBox9.Text = str_mas[17].Trim();

                if (str_mas[19].Trim() == "ОСН") { materialCheckbox11.Checked = true; } // СНО
                if (str_mas[20].Trim() == "УСН Доход") { materialCheckbox10.Checked = true; }
                if (str_mas[21].Trim() == "УСН Доход - расход") { materialCheckbox9.Checked = true; }
                if (str_mas[22].Trim() == "Патент") { materialCheckbox8.Checked = true; }
                if (str_mas[23].Trim() == "ЕСХН") { materialCheckbox12.Checked = true; }
                textBox11.Text = str_mas[25].Trim();
                textBox12.Text = str_mas[27].Trim();
                textBox13.Text = str_mas[29].Trim();
                textBox14.Text = str_mas[31].Trim();
                comboBox1.Text = str_mas[33].Trim();
                textBox2.Text = str_mas[39].Trim();
                string d_fd = str_mas[41].Trim() + str_mas[43].Trim(); //объединение даты и времени
                textBox17.Text = d_fd;
                TextBoxNFD.Text = str_mas[45].Trim();
                textBox19.Text = str_mas[47].Trim();
                textBox6.Text = str_mas[49].Trim();


                if (str_mas[51].Trim() == "1") { checkBox9.Checked = true; } // сведения регистрации ККТ
                if (str_mas[53].Trim() == "1") { checkBox3.Checked = true; }
                if (str_mas[55].Trim() == "1") { checkBox2.Checked = true; }
                if (str_mas[57].Trim() == "1") { checkBox6.Checked = true; }
                if (str_mas[59].Trim() == "1") { checkBox7.Checked = true; }
                if (str_mas[61].Trim() == "1") { checkBox4.Checked = true; }
                if (str_mas[63].Trim() == "1") { checkBox12.Checked = true; }
                if (str_mas[65].Trim() == "1") { checkBox10.Checked = true; }
                if (str_mas[67].Trim() == "1") { checkBox1.Checked = true; }
                if (str_mas[69].Trim() == "1") { checkBox5.Checked = true; }
                textBox15.Text = str_mas[71].Trim(); //КПП организации раннее забыл подставить
            }
        }
        private void butReaddata_Click(object sender, EventArgs e) //кнопка Считать данные
        {
            MaterialMessageBox.Show(
                "Функционал кнопки в разработке. Скоро она будет считавать данные из ККТ, а пока можете просто тыкнуть на нее еще раз",
                "ТЫК");
        }
        private void buttonXML_Click(object sender, EventArgs e) // кнопка Файл регистрации
        {
            string D_FD = textBox17.Text;
            if (D_FD[2] == '.' && D_FD[5] == '.' && D_FD[10] == ' ' && D_FD[13] == ':')
            {
                //Создание папки Регистрация
                string adr_file = System.IO.File.ReadAllText("adr_file.txt");
                Directory.CreateDirectory(adr_file);

                string RNM = textBox2.Text;
                string ZN_KKT = textBox3.Text;
                string M_KKT = textBox18.Text;
                string N_FN = textBox5.Text;
                string M_FN = comboBox2.Text;
                string NameOrganization = textBox7.Text;

                string[] n = NameOrganization.Split(' ');
                string NOrganization = n[0];
                if (NOrganization == "ООО")
                {
                    NameOrganization = "ОБЩЕСТВО С ОГРАНИЧЕННОЙ ОТВЕТСТВЕННОСТЬЮ " + NameOrganization.Substring(4);
                }
                if (NOrganization == "АО")
                {
                    NameOrganization = "АКЦИОНЕРНОЕ ОБЩЕСТВО " + NameOrganization.Substring(3);
                }
                string Director_org = textBox8.Text;
                string INN_Organization = textBox9.Text;
                string Place_ras = textBox14.Text;
                string OFD = comboBox1.Text;
                string INN_OFD = "";
                string KPP_Organization = textBox15.Text;

                string PrAvtonomS = "2"; // сведения регистрации ККТ
                string PrLotereyaS = "2";
                string PrAzartS = "2";
                string PrBankPlatS = "2";
                string PrPlatAgentS = "2";
                string PrAvtomatUstrS = "2";
                string PrInternetS = "2";
                string PrRazvozS = "2";
                string PrAkxizTovarS = "2";
                string PrMarkS = "2";

                if (checkBox9.Checked == true) { PrAvtonomS = "1"; } // сведения регистрации ККТ
                if (checkBox3.Checked == true) { PrAzartS = "1"; }
                if (checkBox2.Checked == true) { PrMarkS = "1"; }
                if (checkBox6.Checked == true) { PrBankPlatS = "1"; }
                if (checkBox7.Checked == true) { PrPlatAgentS = "1"; }
                if (checkBox4.Checked == true) { PrLotereyaS = "1"; }
                if (checkBox12.Checked == true) { PrInternetS = "1"; }
                if (checkBox10.Checked == true) { PrRazvozS = "1"; }
                if (checkBox1.Checked == true) { PrAkxizTovarS = "1"; }
                if (checkBox5.Checked == true) { PrAvtomatUstrS = "1"; }


                string str = System.IO.File.ReadAllText("options_OFD.txt");
                string[] str_mas = str.Split(':');

                if ((OFD == "Эвотор ОФД") || (OFD == "ООО «Эвотор ОФД»")){ INN_OFD = str_mas[3]; }
                if ((OFD == "ЭСК") || (OFD == "АО \"ЭСК\"")){INN_OFD = str_mas[19];}
                if ((OFD == "АО Контур НТТ") || (OFD == "Контур НТТ")){INN_OFD = str_mas[35];}
                if ((OFD == "Такском") || (OFD == "ООО «Такском»")){INN_OFD = str_mas[51];}
                if ((OFD == "Калуга Астрал") || (OFD == "АО «Калуга Астрал»")){INN_OFD = str_mas[67];}
                
                XmlDocument xmlDocument = new XmlDocument();

                xmlDocument.Load("XML_FNS.xml");
                XmlElement Fail = xmlDocument.DocumentElement;


                //XmlElement Fail = xmlDocument.CreateElement("Файл");
                XmlAttribute VersProg = xmlDocument.CreateAttribute("ВерсПрог");
                XmlAttribute VersForm = xmlDocument.CreateAttribute("ВерсФорм");
                XmlAttribute IdFail = xmlDocument.CreateAttribute("ИдФайл");

                XmlElement Document = xmlDocument.CreateElement("Документ");

                XmlAttribute DataDoc = xmlDocument.CreateAttribute("ДатаДок");
                XmlAttribute KND = xmlDocument.CreateAttribute("КНД");
                XmlAttribute KodNO = xmlDocument.CreateAttribute("КодНО");

                XmlElement SvNP = xmlDocument.CreateElement("СвНП");

                //if (NOrganization == "ИП")
                //{

                XmlElement NPFL = xmlDocument.CreateElement("НПФЛ");

                XmlAttribute INNFL = xmlDocument.CreateAttribute("ИННФЛ");

                XmlElement FIO = xmlDocument.CreateElement("ФИО");

                XmlAttribute Imy = xmlDocument.CreateAttribute("Имя");
                XmlAttribute Otcestvo = xmlDocument.CreateAttribute("Отчество");
                XmlAttribute Familiya = xmlDocument.CreateAttribute("Фамилия");
                //}
                //else
                //{
                XmlElement NPYUL = xmlDocument.CreateElement("НПЮЛ");
                XmlAttribute INNYUL = xmlDocument.CreateAttribute("ИННЮЛ");
                XmlAttribute KPP = xmlDocument.CreateAttribute("КПП");
                XmlAttribute NaimOrg = xmlDocument.CreateAttribute("НаимОрг");
                //}
                //</НПФЛ>
                //</СвНП>
                XmlElement Podpisant = xmlDocument.CreateElement("Подписант");

                XmlAttribute PrPodp = xmlDocument.CreateAttribute("ПрПодп");

                XmlElement FIO2 = xmlDocument.CreateElement("ФИО");

                XmlAttribute Imy2 = xmlDocument.CreateAttribute("Имя");
                XmlAttribute Otcestvo2 = xmlDocument.CreateAttribute("Отчество");
                XmlAttribute Familiya2 = xmlDocument.CreateAttribute("Фамилия");

                //XmlElement SvPred = xmlDocument.CreateElement("СвПред");

                //XmlAttribute NaimDoc = xmlDocument.CreateAttribute("НаимДок");

                //</Подписант>

                XmlElement ZayavRegKKT = xmlDocument.CreateElement("ЗаявРегККТ");

                //XmlAttribute RegNomerKKT = xmlDocument.CreateAttribute("РегНомерККТ");
                XmlAttribute VidDoc = xmlDocument.CreateAttribute("ВидДок");
                XmlAttribute KodNOMUst = xmlDocument.CreateAttribute("КодНОМУст");
                //XmlAttribute PrAvtonomRezim = xmlDocument.CreateAttribute("ПрАвтономРежим");
                //XmlAttribute PrZamFN = xmlDocument.CreateAttribute("ПрЗамФН");
                //XmlAttribute PrIzmAvtUstr = xmlDocument.CreateAttribute("ПрИзмАвтУстр");
                //XmlAttribute PrIzmAdrMU = xmlDocument.CreateAttribute("ПрИзмАдрМУ");
                //XmlAttribute PrIzmNaimNP = xmlDocument.CreateAttribute("ПрИзмНаимНП");
                //XmlAttribute PrIniePricini = xmlDocument.CreateAttribute("ПрИныеПричины");
                //XmlAttribute PrSmenOFD = xmlDocument.CreateAttribute("ПрСменОФД");
                //XmlAttribute PrElectrRezim = xmlDocument.CreateAttribute("ПрЭлектрРежим");

                XmlElement SvedRegKKT = xmlDocument.CreateElement("СведРегККТ");

                XmlAttribute ZavodNomerKKT = xmlDocument.CreateAttribute("ЗаводНомерККТ");
                XmlAttribute ZavodNomerFN = xmlDocument.CreateAttribute("ЗаводНомерФН");
                XmlAttribute ModelKKT = xmlDocument.CreateAttribute("МоделККТ");
                XmlAttribute ModelFN = xmlDocument.CreateAttribute("МоделФН");
                XmlAttribute PrAvtomatUstr = xmlDocument.CreateAttribute("ПрАвтоматУстр");
                XmlAttribute PrAvtonom = xmlDocument.CreateAttribute("ПрАвтоном");
                XmlAttribute PrAzart = xmlDocument.CreateAttribute("ПрАзарт");
                XmlAttribute PrAkxizTovar = xmlDocument.CreateAttribute("ПрАкцизТовар");
                XmlAttribute PrBankPlat = xmlDocument.CreateAttribute("ПрБанкПлат");
                XmlAttribute PrBlank = xmlDocument.CreateAttribute("ПрБланк");
                XmlAttribute PrIgorZaved = xmlDocument.CreateAttribute("ПрИгорнЗавед");
                XmlAttribute PrInternet = xmlDocument.CreateAttribute("ПрИнтернет");
                XmlAttribute PrLotereya = xmlDocument.CreateAttribute("ПрЛотерея");
                XmlAttribute PrPlatAgent = xmlDocument.CreateAttribute("ПрПлатАгент");
                XmlAttribute PrRazvozRaznos = xmlDocument.CreateAttribute("ПрРазвозРазнос");
                XmlAttribute PrRascMark = xmlDocument.CreateAttribute("ПрРасчМарк");

                XmlElement SvedOFD = xmlDocument.CreateElement("СведОФД");

                XmlAttribute INNUYL = xmlDocument.CreateAttribute("ИННЮЛ");
                XmlAttribute NaimOrgOFD = xmlDocument.CreateAttribute("НаимОрг");

                XmlElement SvedAdrMUst = xmlDocument.CreateElement("СведАдрМУст");

                XmlAttribute NaimMUst = xmlDocument.CreateAttribute("НаимМУст");

                XmlElement AdrMUstKKT = xmlDocument.CreateElement("АдрМУстККТ");

                XmlElement AdrFIAS = xmlDocument.CreateElement("АдрФИАС");

                XmlAttribute IdNom = xmlDocument.CreateAttribute("ИдНом");
                XmlAttribute Index = xmlDocument.CreateAttribute("Индекс");

                XmlElement Region = xmlDocument.CreateElement("Регион");
                XmlElement MunixipRayon = xmlDocument.CreateElement("МуниципРайон");

                XmlAttribute VidKod = xmlDocument.CreateAttribute("ВидКод");
                XmlAttribute Naim = xmlDocument.CreateAttribute("Наим");

                XmlElement NaselenPunkt = xmlDocument.CreateElement("НаселенПункт");

                XmlAttribute Vid = xmlDocument.CreateAttribute("Вид");
                XmlAttribute Naim2 = xmlDocument.CreateAttribute("Наим");

                XmlElement ElUlDorSeti = xmlDocument.CreateElement("ЭлУлДорСети");

                XmlAttribute Naim3 = xmlDocument.CreateAttribute("Наим");
                XmlAttribute Tip = xmlDocument.CreateAttribute("Тип");

                XmlElement Zdanie = xmlDocument.CreateElement("Здание");

                XmlAttribute Nomer = xmlDocument.CreateAttribute("Номер");
                XmlAttribute Tip2 = xmlDocument.CreateAttribute("Тип");

                XmlElement userElem = xmlDocument.CreateElement("Здание");
                XmlAttribute Name = xmlDocument.CreateAttribute("Тип");
                //</АдрФИАС>
                //</АдрМУстККТ>
                //</СведАдрМУст>
                //</СведРегККТ>
                //<ЗаявРегККТ>
                //</Документ>
                //</Файл>

                DateTime data = DateTime.Today; //получение даты ПК
                string d = Convert.ToString(data);
                d = d.Substring(0, d.Length - 8);
                string[] fio = Director_org.Split(' ');// Получение ФИО


                string[] dd = d.Split('.');
                Random rnd = new Random();
                int a = rnd.Next();
                string rand = Convert.ToString(a);

                XmlText Imy2T = null;
                XmlText Familiya2T = null;
                XmlText Otcestvo2T = null;

                if (KPP_Organization == "Заполняется только для ЮЛ")
                {
                    KPP_Organization = "";
                }



                string ID_file = "KO_ZVLREGKKT_5018_5018_" + INN_Organization + KPP_Organization + "_" + dd[2] + dd[1] + dd[0] + "_" + rand;


                if (fio.Length > 2)
                {

                    XmlText VersProgT = xmlDocument.CreateTextNode("1.0");
                    //  XmlText  = xmlDocument.CreateTextNode("");
                    XmlText VersFormT = xmlDocument.CreateTextNode("5.06");
                    XmlText IdFailT = xmlDocument.CreateTextNode(ID_file);
                    XmlText DataDokT = xmlDocument.CreateTextNode(d);
                    XmlText KNDT = xmlDocument.CreateTextNode("1110061");
                    XmlText KodNOT = xmlDocument.CreateTextNode("9965"); //«Система обозначений налоговых органов» 


                    XmlText INNFLT = xmlDocument.CreateTextNode(INN_Organization); //поправить 
                    XmlText ImyT = xmlDocument.CreateTextNode(fio[1]);
                    XmlText FamiliyaT = xmlDocument.CreateTextNode(fio[0]);
                    XmlText OtcestvoT = xmlDocument.CreateTextNode(fio[2]);


                    XmlText KPPT = xmlDocument.CreateTextNode(KPP_Organization);
                    XmlText NaimOrgT = xmlDocument.CreateTextNode(NameOrganization);


                    XmlText PrPodpT = xmlDocument.CreateTextNode("1"); //Подписант 
                    
                    Imy2T = xmlDocument.CreateTextNode(fio[1]);
                    Familiya2T = xmlDocument.CreateTextNode(fio[0]);
                    Otcestvo2T = xmlDocument.CreateTextNode(fio[2]);
                    

                    //XmlText NaimDocT = xmlDocument.CreateTextNode("Свидетельство");

                    //XmlText RegNomerKKTT = xmlDocument.CreateTextNode(RNM);//ЗаявРегККТ 
                    XmlText VidDocT = xmlDocument.CreateTextNode("1"); // 1-регистрация / 2-перерегистрация
                    XmlText KodNOMUstT = xmlDocument.CreateTextNode("5800");
                    //XmlText PrAvtonomRezimT = xmlDocument.CreateTextNode("2"); //обязателен при <ВидДок>=2           
                    //XmlText PrZamFNT = xmlDocument.CreateTextNode("2");        //обязателен при <ВидДок>=2  
                    //XmlText PrIzmAvtUstrT = xmlDocument.CreateTextNode("2");   //обязателен при <ВидДок>=2  
                    //XmlText PrIzmAdrMUT = xmlDocument.CreateTextNode("2");     //обязателен при <ВидДок>=2 
                    //XmlText PrIzmNaimNPT = xmlDocument.CreateTextNode("2");    //обязателен при <ВидДок>=2 
                    //XmlText PrIniePriciniT = xmlDocument.CreateTextNode("2");  //обязателен при <ВидДок>=2
                    //XmlText PrSmenOFDT = xmlDocument.CreateTextNode("2");      //обязателен при <ВидДок>=2 
                    //XmlText PrElektrRezimT = xmlDocument.CreateTextNode("2");  // не заполняется при <ВидДок>=1  < ПрЭлектрРежим >≠< ПрАвтономРежим > при < ПрЭлектрРежим >= 1


                    XmlText ZavodNomerKKTT = xmlDocument.CreateTextNode(ZN_KKT); //СведРегККТ 
                    XmlText ZavodNomerFNT = xmlDocument.CreateTextNode(N_FN);
                    XmlText ModelKKTT = xmlDocument.CreateTextNode(M_KKT);
                    string mfn = "Шифровальное (криптографическое) средство защиты фискальных данных фискальный накопитель «ФН-1.2 исполнение " + M_FN + "»";
                    XmlText ModelFNT = xmlDocument.CreateTextNode(mfn);
                    XmlText PrAvtomatUstrT = xmlDocument.CreateTextNode(PrAvtomatUstrS);
                    XmlText PrAvtonomT = xmlDocument.CreateTextNode(PrAvtonomS);
                    XmlText PrAzartT = xmlDocument.CreateTextNode(PrAzartS);
                    XmlText PrAkxizTovarT = xmlDocument.CreateTextNode(PrAkxizTovarS);
                    XmlText PrBankPlatT = xmlDocument.CreateTextNode(PrBankPlatS);
                    XmlText PrIgorZavedT = xmlDocument.CreateTextNode("2"); //нет данных
                    XmlText PrInternetT = xmlDocument.CreateTextNode(PrInternetS);
                    XmlText PrLotereyaT = xmlDocument.CreateTextNode(PrLotereyaS);
                    XmlText PrPlatAgentT = xmlDocument.CreateTextNode(PrPlatAgentS);
                    XmlText PrRazvozRaznosT = xmlDocument.CreateTextNode(PrRazvozS);
                    XmlText PrRascMarkT = xmlDocument.CreateTextNode(PrMarkS);

                    XmlText INNYLT = xmlDocument.CreateTextNode(INN_OFD); //СведОФД
                    XmlText NaimOrgOFDT = xmlDocument.CreateTextNode(OFD);
                    XmlText NaimMUstT = xmlDocument.CreateTextNode(Place_ras);

                    XmlText IdNomT = xmlDocument.CreateTextNode("307e942a-83b6-4f99-8a94-9996b5a1b953"); //АдрФИАС 
                    XmlText IndexT = xmlDocument.CreateTextNode("440000");
                    XmlText RegionT = xmlDocument.CreateTextNode("58");
                    XmlText VidKodT = xmlDocument.CreateTextNode("2");
                    XmlText NaimT = xmlDocument.CreateTextNode("город Пенза");
                    XmlText VidT = xmlDocument.CreateTextNode("г");
                    XmlText Naim2T = xmlDocument.CreateTextNode("Пенза");
                    XmlText Naim3T = xmlDocument.CreateTextNode("Суворова");
                    XmlText TipT = xmlDocument.CreateTextNode("ул");
                    XmlText NomerT = xmlDocument.CreateTextNode("92");
                    XmlText Tip2T = xmlDocument.CreateTextNode("стр.");


                    Imy.AppendChild(ImyT);
                    Otcestvo.AppendChild(OtcestvoT);
                    Familiya.AppendChild(FamiliyaT);
                    
                    Imy2.AppendChild(Imy2T);
                    Otcestvo2.AppendChild(Otcestvo2T);
                    Familiya2.AppendChild(Familiya2T);
                    
                    //NaimDoc.AppendChild(NaimDocT);

                    ZavodNomerKKT.AppendChild(ZavodNomerKKTT); //Атрибуты <СведРегККТ>
                    ZavodNomerFN.AppendChild(ZavodNomerFNT);
                    ModelKKT.AppendChild(ModelKKTT);
                    ModelFN.AppendChild(ModelFNT);
                    PrAvtomatUstr.AppendChild(PrAvtomatUstrT);
                    PrAvtonom.AppendChild(PrAvtonomT);
                    PrAzart.AppendChild(PrAzartT);
                    PrAkxizTovar.AppendChild(PrAkxizTovarT);
                    PrBankPlat.AppendChild(PrBankPlatT);
                    PrIgorZaved.AppendChild(PrIgorZavedT);
                    PrInternet.AppendChild(PrInternetT);
                    PrLotereya.AppendChild(PrLotereyaT);
                    PrPlatAgent.AppendChild(PrPlatAgentT);
                    PrRazvozRaznos.AppendChild(PrRazvozRaznosT);
                    PrRascMark.AppendChild(PrRascMarkT);

                    INNUYL.AppendChild(INNYLT);
                    NaimOrg.AppendChild(NaimOrgT);

                    NaimMUst.AppendChild(NaimMUstT);

                    IdNom.AppendChild(IdNomT);
                    Index.AppendChild(IndexT);
                    VidKod.AppendChild(VidKodT);
                    Naim.AppendChild(NaimT);
                    Vid.AppendChild(VidT);
                    Naim2.AppendChild(Naim2T);
                    Naim3.AppendChild(Naim3T);
                    Tip.AppendChild(TipT);
                    Nomer.AppendChild(NomerT);
                    Tip2.AppendChild(Tip2T);
                    //-----------------------------------------------------
                    Region.AppendChild(RegionT);
                    MunixipRayon.Attributes.Append(VidKod);
                    MunixipRayon.Attributes.Append(Naim);
                    NaselenPunkt.Attributes.Append(Vid);
                    NaselenPunkt.Attributes.Append(Naim2);
                    ElUlDorSeti.Attributes.Append(Naim3);
                    ElUlDorSeti.Attributes.Append(Tip);
                    Zdanie.Attributes.Append(Nomer);
                    Zdanie.Attributes.Append(Tip2);
                    //-----------------------------------------------------
                    AdrFIAS.Attributes.Append(IdNom);
                    AdrFIAS.Attributes.Append(Index);
                    AdrFIAS.AppendChild(Region);
                    AdrFIAS.AppendChild(MunixipRayon);
                    AdrFIAS.AppendChild(NaselenPunkt);
                    AdrFIAS.AppendChild(ElUlDorSeti);
                    AdrFIAS.AppendChild(Zdanie);

                    AdrMUstKKT.AppendChild(AdrFIAS);
                    NaimOrgOFD.AppendChild(NaimOrgOFDT);
                    //-----------------------------------------------------
                    if (NOrganization == "ИП")
                    {
                        FIO.Attributes.Append(Imy);
                        FIO.Attributes.Append(Otcestvo);
                        FIO.Attributes.Append(Familiya);
                    }
                    else
                    {
                        NaimOrg.AppendChild(NaimOrgT);
                        KPP.AppendChild(KPPT);
                        INNYUL.AppendChild(INNFLT);
                    }

                    SvedOFD.Attributes.Append(INNUYL);
                    SvedOFD.Attributes.Append(NaimOrgOFD);
                    SvedAdrMUst.Attributes.Append(NaimMUst);
                    SvedAdrMUst.AppendChild(AdrMUstKKT);
                    //-----------------------------------------------------
                    if (NOrganization == "ИП")
                    {
                        NPFL.AppendChild(FIO);
                        INNFL.AppendChild(INNFLT);
                        NPFL.Attributes.Append(INNFL);
                    }
                    else
                    {
                        NPYUL.Attributes.Append(NaimOrg);
                        NPYUL.Attributes.Append(INNYUL);
                        NPYUL.Attributes.Append(KPP);
                    }

                    FIO2.Attributes.Append(Imy2);
                    FIO2.Attributes.Append(Otcestvo2);
                    FIO2.Attributes.Append(Familiya2);
                    //NaimDoc.AppendChild(NaimDocT);
                    //SvPred.Attributes.Append(NaimDoc);
                    SvedRegKKT.AppendChild(SvedOFD);
                    SvedRegKKT.AppendChild(SvedAdrMUst);

                    SvedRegKKT.Attributes.Append(ZavodNomerKKT); //Атрибуты <СведРегККТ>
                    SvedRegKKT.Attributes.Append(ZavodNomerFN);
                    SvedRegKKT.Attributes.Append(ModelKKT);
                    SvedRegKKT.Attributes.Append(ModelFN);
                    SvedRegKKT.Attributes.Append(PrAvtomatUstr);
                    SvedRegKKT.Attributes.Append(PrAvtonom);
                    SvedRegKKT.Attributes.Append(PrAzart);
                    SvedRegKKT.Attributes.Append(PrAkxizTovar);
                    SvedRegKKT.Attributes.Append(PrBankPlat);
                    SvedRegKKT.Attributes.Append(PrIgorZaved);
                    SvedRegKKT.Attributes.Append(PrInternet);
                    SvedRegKKT.Attributes.Append(PrLotereya);
                    SvedRegKKT.Attributes.Append(PrPlatAgent);
                    SvedRegKKT.Attributes.Append(PrRazvozRaznos);
                    SvedRegKKT.Attributes.Append(PrRascMark);

                    VidDoc.AppendChild(VidDocT); //Атрибуты <ЗаявРегККТ>
                    KodNOMUst.AppendChild(KodNOMUstT);
                    //RegNomerKKT.AppendChild(RegNomerKKTT); //включается только при регистрации!!!

                    //PrAvtonomRezim.AppendChild(PrAvtonomRezimT);
                    //PrZamFN.AppendChild(PrZamFNT);
                    //PrIzmAvtUstr.AppendChild(PrIzmAvtUstrT);
                    //PrIzmAdrMU.AppendChild(PrIzmAdrMUT);
                    //PrIzmNaimNP.AppendChild(PrIzmNaimNPT);
                    //PrIniePricini.AppendChild(PrIniePriciniT);
                    //PrSmenOFD.AppendChild(PrSmenOFDT);
                    //PrElectrRezim.AppendChild(PrElektrRezimT);

                    //-----------------------------------------------------
                    if (NOrganization == "ИП")
                    {
                        SvNP.AppendChild(NPFL);
                    }
                    else
                    {
                        SvNP.AppendChild(NPYUL);
                    }
                    PrPodp.AppendChild(PrPodpT);
                    Podpisant.Attributes.Append(PrPodp);
                    Podpisant.AppendChild(FIO2);
                    //Podpisant.AppendChild(SvPred);
                    ZayavRegKKT.AppendChild(SvedRegKKT);
                    //ZayavRegKKT.Attributes.Append(RegNomerKKT);
                    ZayavRegKKT.Attributes.Append(VidDoc);
                    ZayavRegKKT.Attributes.Append(KodNOMUst);
                    //ZayavRegKKT.Attributes.Append(PrAvtonomRezim);
                    //ZayavRegKKT.Attributes.Append(PrZamFN);
                    //ZayavRegKKT.Attributes.Append(PrIzmAvtUstr);
                    //ZayavRegKKT.Attributes.Append(PrIzmAdrMU);
                    //ZayavRegKKT.Attributes.Append(PrIzmNaimNP);
                    //ZayavRegKKT.Attributes.Append(PrIniePricini);
                    //ZayavRegKKT.Attributes.Append(PrSmenOFD);
                    //ZayavRegKKT.Attributes.Append(PrElectrRezim);

                    DataDoc.AppendChild(DataDokT);
                    KND.AppendChild(KNDT);
                    KodNO.AppendChild(KodNOT);
                    Document.Attributes.Append(DataDoc);
                    Document.Attributes.Append(KND);
                    Document.Attributes.Append(KodNO);
                    //-----------------------------------------------------


                    Document.AppendChild(SvNP);
                    Document.AppendChild(Podpisant);
                    Document.AppendChild(ZayavRegKKT);

                    VersProg.AppendChild(VersProgT);
                    VersForm.AppendChild(VersFormT);
                    IdFail.AppendChild(IdFailT);
                    Fail.Attributes.Append(VersProg);
                    Fail.Attributes.Append(VersForm);
                    Fail.Attributes.Append(IdFail);
                    Fail.AppendChild(Document);


                    adr_file = System.IO.File.ReadAllText("adr_file.txt");
                    string adr_file_save = null;

                    FolderBrowserDialog Browserdialog = new FolderBrowserDialog(); //открытие проводника и выбор папки сохраннения
                    Browserdialog.RootFolder = Environment.SpecialFolder.Desktop;
                    Browserdialog.SelectedPath = adr_file.Remove(adr_file.Length - 2);
                    if (Browserdialog.ShowDialog() == DialogResult.OK)
                    {
                        adr_file_save = Browserdialog.SelectedPath;
                    }
                    Directory.CreateDirectory(adr_file_save + "\\" + ID_file);
                    xmlDocument.Save(adr_file_save + "\\" + ID_file + "\\" + ID_file + ".xml"); //сохранение файла xml
                    ZipFile.CreateFromDirectory(adr_file_save + "\\" + ID_file, adr_file_save + "\\" + ID_file + ".zip"); //сохранение zip (что упаковываем, куда)
                    if (delete_xml == true)
                    {
                        Directory.Delete(adr_file_save + "\\" + ID_file, true);
                    }
                    

                    MaterialMessageBox.Show(
            "Файл XML создан и сохранен",
            "Сообщение");
                }
                else
                {
                    MaterialMessageBox.Show(
                "Неверно введены ФИО руководителя",
                "Внимательнее");
                }
            }
            else
            {
                MaterialMessageBox.Show(
            "Ошибка в вводе даты и времени. Введите по формату (дд.мм.гггг чч:мм)",
            "Внимательнее");
            }
        }
        private void buttonAkt_Click(object sender, EventArgs e) //кнопка Акт ввода в эксплуатацию
        {
            string DanaT_FD = textBox17.Text;
            if (DanaT_FD[2] == '.' && DanaT_FD[5] == '.' && DanaT_FD[10] == ' ' && DanaT_FD[13] == ':')
            {

                string ID_Сlient = textBox1.Text;
                string RNM = textBox2.Text;
                string ZN_KKT = textBox3.Text;
                string N_FN = textBox5.Text;

                string NameOrganization = textBox7.Text;
                string INN_Organization = textBox9.Text;
                string OFD = comboBox1.Text;

                string D_FD = DanaT_FD.Substring(0,10);
                string T_FD = DanaT_FD.Substring(Math.Max(0, DanaT_FD.Length - 5));
                string N_FD = TextBoxNFD.Text;
                string FP = textBox19.Text;
                string Name_operator = "";
                string INN_OFD = "";

                string str = System.IO.File.ReadAllText("options_OFD.txt");
                string[] str_mas = str.Split(':');
                if ((OFD == "Эвотор ОФД") || (OFD == "ООО «Эвотор ОФД»")) { INN_OFD = str_mas[3]; }
                if ((OFD == "ЭСК") || (OFD == "АО «ЭСК»")) { INN_OFD = str_mas[19]; }
                if ((OFD == "АО Контур НТТ") || (OFD == "Контур НТТ")) { INN_OFD = str_mas[35]; }
                if ((OFD == "Такском") || (OFD == "ООО «Такском»")) { INN_OFD = str_mas[51]; }
                if ((OFD == "Калуга Астрал") || (OFD == "АО «Калуга Астрал»")) { INN_OFD = str_mas[67]; }

                //Выбор оператора
                Name_operator = System.IO.File.ReadAllText("Authorization.txt");


                var newakt = new NewAKT("Akt.docx");

                var items = new Dictionary<string, string>
            {
                {"<ZN_KKT>", ZN_KKT },
                {"<RNM>", RNM },
                {"<N_FN>", N_FN },
                {"<D_FD>", D_FD },
                {"<T_FD>", T_FD },
                {"<N_FD>", N_FD },
                {"<FP>", FP },
                {"<INN_Organization>", INN_Organization },
                {"<INN_OFD>", INN_OFD },
                {"<NameOrganization>", NameOrganization },
                {"<Name_operator>", Name_operator },
                {"<ID_Client>", ID_Сlient },
            };

                newakt.Process(items);

                //adr_file = File.ReadAllText("adr_file.txt");
                MaterialMessageBox.Show(
            "Акт ввода сформирован и сохранен",
            "Сообщение");
            }


            else
            {
                MaterialMessageBox.Show(
            "Ошибка в вводе даты и времени. Введите по формату (дд.мм.гггг чч:мм)",
            "Внимательнее");
            }
        }
        private void butReg_Terminal_FA_Click(object sender, EventArgs e) // кнопка Регистрация Терминал-ФА
        {
            //string inn = "004004007928";
            //string factoryNumber = "550101000005";
            //string rn = "0000001072";

            //byte[] array = new byte[42];
            //byte i, j;
            //ushort crc = 0xFFFF;
            //for (i = 0; i < 10; i++) array[i] = (byte)rn[i];
            //for (; i < 22; i++) array[i] = (byte)inn[i - 10];
            //for (j = 0; j < 20 - factoryNumber.Length; j++) array[i++] = (byte)'0';
            //for (; i < 42; i++) array[i] = (byte)factoryNumber[i - 22 - j];
            //for (i = 0; i < 42; i++) crc = crc16_update(crc, array[i]);

            //string sign = crc.ToString();



            // временное оповещение
            string RNM = textBox2.Text;
            if (RNM.Length == 16)
            {
                t++;
                if (t == 1)
                {
                    MaterialMessageBox.Show(
                    "Функционал кнопки в разработке. Скоро она будет вводить данные регистрации сразу в ККТ, а пока можете просто тыкнуть на нее еще раз",
                "ТЫК");
                    //    DialogResult result = MessageBox.Show(
                    //        "Уверены что хотите ввести данные регистрации ККТ? Отменить действие будет невозможно",
                    //        "ВНИМАНИЕ! ПРОВЕРЬ ДАННЫЕ!",
                    //        MessageBoxButtons.YesNo);
                    //if (result == DialogResult.Yes)
                    //{

                    //}
                }
                if (t > 1)
                {
                    MaterialMessageBox.Show(
                "НУ тыкнул раз, тыкнул два, дальше ничего не произойдет! Иди работай!",
                "ТЫК");
                }
            }
            else
            {
                MaterialMessageBox.Show(
            "Значени РНМ введено некорректно. РНМ должен состоять из 16 цифр без пробелов.",
            "Внимательнее");
            }
        }
        
        
        private void Form1_Closing(object sender, FormClosingEventArgs e) //Сохранение при закрытии формы
        {
            
            if (save_closing == false)
            {
                DialogResult result = MaterialMessageBox.Show("Возможно у вас есть несохраненные данные. Сохранить?", "Да", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    save_closing = true;
                    string ID_Сlient = textBox1.Text;
                    string RNM = textBox2.Text;
                    string ZN_KKT = textBox3.Text;
                    string N_av = textBox21.Text;
                    string N_FN = textBox5.Text;
                    string M_FN = comboBox2.Text;
                    string NameOrganization = textBox7.Text;
                    string Director_org = textBox8.Text;
                    string INN_Organization = textBox9.Text;
                    string KPP_Organization = textBox15.Text;
                    string Telephone = textBox11.Text;
                    string Email = textBox12.Text;
                    string Address_ras = textBox13.Text;
                    string Place_ras = textBox14.Text;
                    string OFD = comboBox1.Text;
                    string INN_OFD = textBox4.Text;
                    string DandT_FD = textBox17.Text;
                    string D_FD = textBox17.Text.Substring(10);
                    string T_FD = textBox17.Text.Substring(Math.Max(0, textBox17.Text.Length - 5)); // нахождение даты ФД
                    string N_FD = TextBoxNFD.Text;
                    string FP = textBox19.Text;
                    string Model_KKT = textBox18.Text;
                    string Adr_Internet = textBox6.Text;

                    string SNO_OSN = " ";
                    string SNO_USN_D = " ";
                    string SNO_USN_D_R = " ";
                    string SNO_PATENT = " ";
                    string SNO_ESHN = " ";
                    if (materialCheckbox11.Checked == true) { SNO_OSN = "ОСН"; }
                    if (materialCheckbox10.Checked == true) { SNO_USN_D = "УСН Доход"; }
                    if (materialCheckbox9.Checked == true) { SNO_USN_D_R = "УСН Доход - расход"; }
                    if (materialCheckbox8.Checked == true) { SNO_PATENT = "Патент"; }
                    if (materialCheckbox12.Checked == true) { SNO_ESHN = "ЕСХН"; }


                    string PrAvtonom = "2"; // сведения регистрации ККТ
                    string PrLotereya = "2";
                    string PrAzart = "2";
                    string PrBankPlat = "2";
                    string PrPlatAgent = "2";
                    string PrAvtomatUstr = "2";
                    string PrInternet = "2";
                    string PrRazvoz = "2";
                    string PrAkxizTovar = "2";
                    string PrMark = "2";
                    if (checkBox9.Checked == true) { PrAvtonom = "1"; } // сведения регистрации ККТ
                    if (checkBox3.Checked == true) { PrAzart = "1"; }
                    if (checkBox2.Checked == true) { PrMark = "1"; }
                    if (checkBox6.Checked == true) { PrBankPlat = "1"; }
                    if (checkBox7.Checked == true) { PrPlatAgent = "1"; }
                    if (checkBox4.Checked == true) { PrLotereya = "1"; }
                    if (checkBox12.Checked == true) { PrInternet = "1"; }
                    if (checkBox10.Checked == true) { PrRazvoz = "1"; }
                    if (checkBox1.Checked == true) { PrAkxizTovar = "1"; }
                    if (checkBox5.Checked == true) { PrAvtomatUstr = "1"; }

                    Save s = new Save();
                    s.setValues(D_FD, ID_Сlient, RNM, ZN_KKT, N_av, N_FN, M_FN, NameOrganization,
                Director_org, INN_Organization, KPP_Organization, SNO_OSN, SNO_USN_D, SNO_USN_D_R, SNO_PATENT, SNO_ESHN,
                Telephone, Email, Address_ras, Place_ras,
                OFD, INN_OFD, T_FD, N_FD, FP, Model_KKT, Adr_Internet, PrAvtonom, PrLotereya, PrAzart, PrBankPlat,
                PrPlatAgent, PrAvtomatUstr, PrInternet, PrRazvoz, PrAkxizTovar, PrMark);

                }
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
                save_closing = true;
            }
        }




        // служебные для Терминала-ФА
        //private ushort crc16_update(ushort crc16, byte data)
        //{
        //    crc16 ^= (ushort)(data << 8);
        //    for (byte i = 0; i < 8; i++) crc16 = (ushort)(((crc16 & 0x8000) != 0) ? (crc16 << 1) ^ 0x1021 : crc16 << 1);
        //    return crc16;
        //}

        

        //____Страница_2________________________________________________
        private void OFD2_TextChanged(object sender, EventArgs e) // Подстановка ИНН ОФД на второй странице
        {
            string INN_OFD = "";

            string str = System.IO.File.ReadAllText("options_OFD.txt");
            string[] str_mas = str.Split(':');
            string OFD = materialComboBox1.Text;
            if ((OFD == "Эвотор ОФД") || (OFD == "ООО «Эвотор ОФД»"))
            {
                INN_OFD = str_mas[3];
            }
            if ((OFD == "ЭСК") || (OFD == "АО «ЭСК»"))
            {
                INN_OFD = str_mas[19];
            }
            if ((OFD == "АО Контур НТТ") || (OFD == "Контур НТТ"))
            {
                INN_OFD = str_mas[35];
            }
            if ((OFD == "Такском") || (OFD == "ООО «Такском»"))
            {
                INN_OFD = str_mas[51];
            }
            if ((OFD == "Калуга Астрал") || (OFD == "АО «Калуга Астрал»"))
            {
                INN_OFD = str_mas[67];
            }
            materialTextBox25.Text = INN_OFD;
        }
        private void buttonAkt2_Click(object sender, EventArgs e) // Кнопка Акт ввода на второй странице
        {
            string DanaT_FD = textBox17.Text;
            if (DanaT_FD[2] == '.' && DanaT_FD[5] == '.' && DanaT_FD[10] == ' ' && DanaT_FD[13] == ':')
            {

                string ID_Сlient = textBox1.Text;
                string RNM = textBox2.Text;
                string ZN_KKT = textBox3.Text;
                string N_FN = textBox5.Text;

                string NameOrganization = textBox7.Text;
                string INN_Organization = textBox9.Text;
                string OFD = comboBox1.Text;

                string D_FD = DanaT_FD.Substring(0, 10);
                string T_FD = DanaT_FD.Substring(Math.Max(0, DanaT_FD.Length - 5));
                string N_FD = TextBoxNFD.Text;
                string FP = textBox19.Text;
                string Name_operator = "";
                string INN_OFD = "";

                string str = System.IO.File.ReadAllText("options_OFD.txt");
                string[] str_mas = str.Split(':');
                if ((OFD == "Эвотор ОФД") || (OFD == "ООО «Эвотор ОФД»")) { INN_OFD = str_mas[3]; }
                if ((OFD == "ЭСК") || (OFD == "АО «ЭСК»")) { INN_OFD = str_mas[19]; }
                if ((OFD == "АО Контур НТТ") || (OFD == "Контур НТТ")) { INN_OFD = str_mas[35]; }
                if ((OFD == "Такском") || (OFD == "ООО «Такском»")) { INN_OFD = str_mas[51]; }
                if ((OFD == "Калуга Астрал") || (OFD == "АО «Калуга Астрал»")) { INN_OFD = str_mas[67]; }

                //Выбор оператора
                Name_operator = System.IO.File.ReadAllText("Authorization.txt");


                var newakt = new NewAKT("Akt.docx");

                var items = new Dictionary<string, string>
            {
                {"<ZN_KKT>", ZN_KKT },
                {"<RNM>", RNM },
                {"<N_FN>", N_FN },
                {"<D_FD>", D_FD },
                {"<T_FD>", T_FD },
                {"<N_FD>", N_FD },
                {"<FP>", FP },
                {"<INN_Organization>", INN_Organization },
                {"<INN_OFD>", INN_OFD },
                {"<NameOrganization>", NameOrganization },
                {"<Name_operator>", Name_operator },
                {"<ID_Client>", ID_Сlient },
            };

                newakt.Process(items);
            }
        }
        private void butSaveAKT_Click(object sender, EventArgs e) // Кнопка Сохранить на второй странцие
        {
            string D_FD = materialTextBox23.Text;
            if (D_FD[2] != '.' || D_FD[5] != '.' || D_FD[10] != ' ' || D_FD[13] != ':')
            {
                MaterialMessageBox.Show(
            "Проверьте формат ввода даты и времени (дд.мм.гггг чч:мм)",
            "Внимательнее");


            }
            else
            {
                string ID_Сlient = materialTextBox28.Text;
                string RNM = materialTextBox24.Text;
                string ZN_KKT = materialTextBox210.Text;
                string N_av = " ";
                string N_FN = materialTextBox29.Text;
                string M_FN = " ";
                string NameOrganization = materialTextBox27.Text;
                string Director_org = " ";
                string INN_Organization = materialTextBox26.Text;
                string KPP_Organization = " ";
                string Telephone = " ";
                string Email = " ";
                string Address_ras = " ";
                string Place_ras = " ";
                string OFD = materialComboBox1.Text;
                string INN_OFD = materialTextBox25.Text;
                string T_FD = D_FD.Substring(Math.Max(0, D_FD.Length - 5)); // нахождение даты ФД
                string N_FD = materialTextBox21.Text;
                string FP = materialTextBox22.Text;
                string Model_KKT = materialTextBox211.Text;
                string Adr_Internet = " ";

                string SNO_OSN = " ";
                string SNO_USN_D = " ";
                string SNO_USN_D_R = " ";
                string SNO_PATENT = " ";
                string SNO_ESHN = " ";


                string PrAvtonom = " "; // сведения регистрации ККТ
                string PrLotereya = " ";
                string PrAzart = " ";
                string PrBankPlat = " ";
                string PrPlatAgent = " ";
                string PrAvtomatUstr = " ";
                string PrInternet = " ";
                string PrRazvoz = " ";
                string PrAkxizTovar = " ";
                string PrMark = " ";

                Save s = new Save();
                s.setValues(D_FD, ID_Сlient, RNM, ZN_KKT, N_av, N_FN, M_FN, NameOrganization,
            Director_org, INN_Organization, KPP_Organization, SNO_OSN, SNO_USN_D, SNO_USN_D_R, SNO_PATENT, SNO_ESHN,
            Telephone, Email, Address_ras, Place_ras,
            OFD, INN_OFD, T_FD, N_FD, FP, Model_KKT, Adr_Internet, PrAvtonom, PrLotereya, PrAzart, PrBankPlat,
            PrPlatAgent, PrAvtomatUstr, PrInternet, PrRazvoz, PrAkxizTovar, PrMark);

            }
        }
        private void butLoadingAKT_Click(object sender, EventArgs e) // Кнопка Открыть на второй странице
        {
            string str = "";
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (Path.GetExtension(ofd.FileName).ToUpper().ToLower().Equals(".txt", StringComparison.CurrentCultureIgnoreCase))
                    {
                        str = System.IO.File.ReadAllText(ofd.FileName);
                    }
                }


            }
            string[] str_mas = str.Split('+');
            if (str_mas.Length > 47)
            {
                materialTextBox210.Text = str_mas[1].Trim();
                materialTextBox211.Text = str_mas[3].Trim();
                materialTextBox29.Text = str_mas[7].Trim();
                materialTextBox28.Text = str_mas[11].Trim();
                materialTextBox27.Text = str_mas[13].Trim();
                materialTextBox26.Text = str_mas[17].Trim();
                materialComboBox1.Text = str_mas[33].Trim();
                materialTextBox25.Text = str_mas[35].Trim();
                materialTextBox24.Text = str_mas[39].Trim();
                materialTextBox23.Text = str_mas[41].Trim();
                materialTextBox21.Text = str_mas[45].Trim();
                materialTextBox22.Text = str_mas[47].Trim();
            }
        }



        //____Страница_3________________________________________________
        private void butSave_OFD_Click(object sender, EventArgs e) // Сохранение ОФД
        {
            DialogResult result = MaterialMessageBox.Show("Уверены что хотите сохранить данные ОФД? Отменить действие будет невозможно", "Подтверждение", 
                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    string INN_OFD = materialTextBox31.Text;
                    string email_OFD = materialTextBox32.Text;
                    string adr_OFD = materialTextBox33.Text;
                    string IP = materialTextBox34.Text;
                    string TCP = materialTextBox35.Text;
                    string DNS = materialTextBox36.Text;
                    string port = materialTextBox38.Text;

                    string str = File.ReadAllText("options_OFD.txt");
                    string[] str_mas = str.Split(':');

                    string OFD = materialComboBox1.Text;
                    if ((OFD == str_mas[1].Trim()))
                    {

                        str_mas[3] = INN_OFD;
                        str_mas[5] = email_OFD;
                        str_mas[7] = adr_OFD;
                        str_mas[9] = IP;
                        str_mas[11] = TCP;
                        str_mas[13] = DNS;
                        str_mas[15] = port;
                    }
                    if ((OFD == str_mas[17].Trim()))
                    {

                        str_mas[19] = INN_OFD;
                        str_mas[21] = email_OFD;
                        str_mas[23] = adr_OFD;
                        str_mas[25] = IP;
                        str_mas[27] = TCP;
                        str_mas[29] = DNS;
                        str_mas[31] = port;
                    }
                    if ((OFD == str_mas[33].Trim()))
                    {

                        str_mas[35] = INN_OFD;
                        str_mas[37] = email_OFD;
                        str_mas[39] = adr_OFD;
                        str_mas[41] = IP;
                        str_mas[43] = TCP;
                        str_mas[45] = DNS;
                        str_mas[47] = port;
                    }
                    if ((OFD == str_mas[49].Trim()))
                    {

                        str_mas[51] = INN_OFD;
                        str_mas[53] = email_OFD;
                        str_mas[55] = adr_OFD;
                        str_mas[57] = IP;
                        str_mas[59] = TCP;
                        str_mas[61] = DNS;
                        str_mas[63] = port;
                    }
                    if ((OFD == str_mas[65].Trim()))
                    {

                        str_mas[67] = INN_OFD;
                        str_mas[69] = email_OFD;
                        str_mas[71] = adr_OFD;
                        str_mas[73] = IP;
                        str_mas[75] = TCP;
                        str_mas[77] = DNS;
                        str_mas[79] = port;
                    }
                    StreamWriter str2 = new StreamWriter("options_OFD.txt");

                    str2.WriteLine("ОФД:" + str_mas[1] + ":");
                    str2.WriteLine("ИНН:" + str_mas[3] + ":");
                    str2.WriteLine("Почта:" + str_mas[5] + ":");
                    str2.WriteLine("Адрес:" + str_mas[7] + ":");
                    str2.WriteLine("IP:" + str_mas[9] + ":");
                    str2.WriteLine("TCP-порт:" + str_mas[11] + ":");
                    str2.WriteLine("DNS ОФД:" + str_mas[13] + ":");
                    str2.WriteLine();
                    str2.WriteLine("Порт:" + str_mas[15] + ":");
                    str2.WriteLine();
                    str2.WriteLine("ОФД:" + str_mas[17] + ":");
                    str2.WriteLine("ИНН:" + str_mas[19] + ":");
                    str2.WriteLine("Почта:" + str_mas[21] + ":");
                    str2.WriteLine("Адрес:" + str_mas[23] + ":");
                    str2.WriteLine("IP:" + str_mas[25] + ":");
                    str2.WriteLine("TCP-порт:" + str_mas[27] + ":");
                    str2.WriteLine("DNS ОФД:" + str_mas[29] + ":");
                    str2.WriteLine();
                    str2.WriteLine("Порт:" + str_mas[31] + ":");
                    str2.WriteLine();
                    str2.WriteLine("ОФД:" + str_mas[33] + ":");
                    str2.WriteLine("ИНН:" + str_mas[35] + ":");
                    str2.WriteLine("Почта:" + str_mas[37] + ":");
                    str2.WriteLine("Адрес:" + str_mas[39] + ":");
                    str2.WriteLine("IP:" + str_mas[41] + ":");
                    str2.WriteLine("TCP-порт:" + str_mas[43] + ":");
                    str2.WriteLine("DNS ОФД:" + str_mas[45] + ":");
                    str2.WriteLine();
                    str2.WriteLine("Порт:" + str_mas[47] + ":");
                    str2.WriteLine();
                    str2.WriteLine("ОФД:" + str_mas[49] + ":");
                    str2.WriteLine("ИНН:" + str_mas[51] + ":");
                    str2.WriteLine("Почта:" + str_mas[53] + ":");
                    str2.WriteLine("Адрес:" + str_mas[55] + ":");
                    str2.WriteLine("IP:" + str_mas[57] + ":");
                    str2.WriteLine("TCP-порт:" + str_mas[59] + ":");
                    str2.WriteLine("DNS ОФД:" + str_mas[61] + ":");
                    str2.WriteLine();
                    str2.WriteLine("Порт:" + str_mas[63] + ":");
                    str2.WriteLine();
                    str2.WriteLine("ОФД:" + str_mas[65] + ":");
                    str2.WriteLine("ИНН:" + str_mas[67] + ":");
                    str2.WriteLine("Почта:" + str_mas[69] + ":");
                    str2.WriteLine("Адрес:" + str_mas[71] + ":");
                    str2.WriteLine("IP:" + str_mas[73] + ":");
                    str2.WriteLine("TCP-порт:" + str_mas[75] + ":");
                    str2.WriteLine("DNS ОФД:" + str_mas[77] + ":");
                    str2.WriteLine();
                    str2.WriteLine("Порт:" + str_mas[79] + ":");
                    str2.Close();


                }
                catch (Exception ex)
                {
                    MaterialMessageBox.Show(
            "Ошибка при сохранении" + ex,
            "Сообщение");
                }
            }



        }
        private void butSave_FN_Click(object sender, EventArgs e) // Сохранение КП ФН
        {
            DialogResult result = MaterialMessageBox.Show(
                "Уверены что хотите сохранить данные КП ФН? Отменить действие будет невозможно",
                "Подтверждение",
                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    string adr = materialTextBox39.Text;
                    string port_fn = materialTextBox310.Text;

                    string str = File.ReadAllText("options_FN.txt");
                    string[] str_mas = str.Split(':');

                    string FN = materialComboBox1.Text;
                    if ((FN == str_mas[1].Trim()))
                    {

                        str_mas[3] = adr;
                        str_mas[5] = port_fn;

                    }
                    if ((FN == str_mas[7].Trim()))
                    {

                        str_mas[9] = adr;
                        str_mas[11] = port_fn;
                    }

                    StreamWriter str2 = new StreamWriter("options_FN.txt");

                    str2.WriteLine("Производитель ФН:" + str_mas[1] + ":");
                    str2.WriteLine("Адрес КМ:" + str_mas[3] + ":");
                    str2.WriteLine("Порт:" + str_mas[5] + ":");
                    str2.WriteLine();
                    str2.WriteLine("Производитель ФН:" + str_mas[7] + ":");
                    str2.WriteLine("Адрес КМ:" + str_mas[9] + ":");
                    str2.WriteLine("Порт:" + str_mas[11] + ":");

                    str2.Close();
                }
                catch (Exception ex)
                {
                    MaterialMessageBox.Show(
            "Ошибка при сохранении" + ex,
            "Сообщение");
                }
            }
        }

        private void FN_Leave(object sender, EventArgs e) //вывод в поля данных КП при неактивном поле Производитель ФН
        {
            string str = "";

            str = File.ReadAllText("options_FN.txt");
            string[] str_mas = str.Split(':');

            string adr = "";
            string port_fn = "";


            string FN = materialComboBox2.Text;
            if ((FN == str_mas[1].Trim()))
            {
                adr = str_mas[3].Trim();
                port_fn = str_mas[5].Trim();
            }
            if ((FN == str_mas[7].Trim()))
            {

                adr = str_mas[9].Trim();
                port_fn = str_mas[11].Trim();
            }

            materialTextBox39.Text = adr;
            materialTextBox310.Text = port_fn;

        }



        //____Страница_4________________________________________________
        private void TabControl_Selected(object sender, TabControlEventArgs e) // событие автозаполнения textBox настроек при активации вкладки
        {
            StreamReader sr = new StreamReader("adr_file.txt"); // автозаполнение textBox на странице настроек
            string line1 = sr.ReadLine();
            materialTextBox1.Text = line1;
            sr.Close();

            StreamReader r = new StreamReader("Authorization.txt"); // автозаполнение textBox на странице настроек
            string line2 = r.ReadLine();
            materialTextBox2.Text = line2;
            r.Close();

            StreamReader str_del = new StreamReader("del_xml.txt"); // автозаполнение switch на странице настроек
            string line3 = str_del.ReadLine();
            str_del.Close();
            if (line3 == "true") { materialSwitch1.Checked = true; }
            if (line3 == "false") { materialSwitch1.Checked = false; }
        } 
        private void materialButton1_Click(object sender, EventArgs e) // Кнопка сохранение
        {
            try
            {
                if (materialSwitch1.Checked == true) { delete_xml = true; }
                if (materialSwitch1.Checked == false) { delete_xml = false; }

                StreamWriter sr = new StreamWriter("adr_file.txt", false);
                sr.WriteLine(materialTextBox1.Text);
                sr.Close();
                StreamWriter r = new StreamWriter("Authorization.txt", false);
                r.WriteLine(materialTextBox2.Text);
                r.Close();

                StreamWriter str_del = new StreamWriter("del_xml.txt", false);
                if (delete_xml == true)
                {
                    str_del.WriteLine("true");
                }
                if (delete_xml == false)
                {
                    str_del.WriteLine("false");
                }
                str_del.Close();

                MaterialMessageBox.Show(
        "Настройки сохранены",
        "Сообщение");



            }
            catch (Exception ex)
            {
                MaterialMessageBox.Show(
        "Ошибка сохранения " + ex,
        "Ошибка");
            }
            finally
            {

            }
        }
        private void materialButton2_Click(object sender, EventArgs e) //открытие проводника
        {
            FolderBrowserDialog Browserdialog = new FolderBrowserDialog();
            if (Browserdialog.ShowDialog() == DialogResult.OK)
            {
                materialTextBox1.Text = Browserdialog.SelectedPath;
            }
        }

        
    }
} 
