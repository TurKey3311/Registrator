using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kassa
{
    class Save
    {
        public string D_FD;

        public string ID_Сlient;
        public string RNM;
        public string ZN_KKT;
        public string N_av;
        public string N_FN;
        public string M_FN;
        public string NameOrganization;
        public string Director_org;
        public string INN_Organization;
        string KPP_Organization;
        string SNO_OSN; string SNO_USN_D; string SNO_USN_D_R; string SNO_PATENT; string SNO_ESHN;
        public string Telephone;
        public string Email;
        public string Address_ras;
        public string Place_ras;
        public string OFD;
        public string INN_OFD;
        public string T_FD;
        public string N_FD;
        public string FP;
        public string Model_KKT;
        public string Adr_Internet;

        public string PrAvtonomS; // сведения регистрации ККТ
        public string PrLotereyaS;
        public string PrAzartS;
        public string PrBankPlatS;
        public string PrPlatAgentS;
        public string PrAvtomatUstrS;
        public string PrInternetS;
        public string PrRazvozS;
        public string PrAkxizTovarS;
        public string PrMarkS;

        public void setValues(string _D_FD, string _ID_Client, string _RNM, string _ZN_KKT, string _N_av, string _N_FN, string _M_FN, string _NameOrganization, 
            string _Director_org, string _INN_Organization, string _KPP_Organization, string _SNO_OSN, string _SNO_USN_D, string _SNO_USN_D_R, string _SNO_PATENT, string _SNO_ESHN, 
            string _Telephone, string _Email, string _Address_ras, string _Place_ras,
            string _OFD, string _INN_OFD, string _T_FD, string _N_FD, string _FP, string _Model_KKT, string _Adr_Internet, string _PrAvtonomS, string _PrLotereyaS, string _PrAzartS, string _PrBankPlatS,
            string _PrPlatAgentS, string _PrAvtomatUstrS, string _PrInternetS, string _PrRazvozS, string _PrAkxizTovarS, string _PrMarkS)
        {
            D_FD = _D_FD;
            ID_Сlient = _ID_Client;
            RNM = _RNM;
            ZN_KKT = _ZN_KKT;
            N_av = _N_av;
            N_FN = _N_FN;
            M_FN = _M_FN;
            NameOrganization = _NameOrganization;
            Director_org = _Director_org;
            INN_Organization = _INN_Organization;
            KPP_Organization = _KPP_Organization;
            SNO_OSN = _SNO_OSN;
            SNO_USN_D = _SNO_USN_D;
            SNO_USN_D_R = _SNO_USN_D_R;
            SNO_PATENT = _SNO_PATENT;
            SNO_ESHN = _SNO_ESHN;
            Telephone = _Telephone;
            Email = _Email;
            Address_ras = _Address_ras;
            Place_ras = _Place_ras;
            OFD = _OFD;
            INN_OFD = _INN_OFD;
            T_FD = _T_FD;
            N_FD = _N_FD;
            FP = _FP;
            Model_KKT = _Model_KKT;
            Adr_Internet = _Adr_Internet;

            PrAvtonomS = _PrAvtonomS;
            PrLotereyaS = _PrLotereyaS;
            PrAzartS = _PrAzartS;
            PrBankPlatS = _PrBankPlatS;
            PrPlatAgentS = _PrPlatAgentS;
            PrAvtomatUstrS = _PrAvtomatUstrS;
            PrInternetS = _PrInternetS;
            PrRazvozS = _PrRazvozS;
            PrAkxizTovarS = _PrAkxizTovarS;
            PrMarkS = _PrMarkS;


            try
            {
                string[] zap_znak = { "\"", "\\", "/", ":", "*", "?", "<", ">", "|", "\"" };
                string NameOrganization_save = NameOrganization;
                if (NameOrganization != "")
                {
                    for (int i = 0; i < zap_znak.Length; i++)
                    {
                        NameOrganization_save = NameOrganization_save.Replace(zap_znak[i], "");
                    }
                }

                string Manufacturer_FN = "";
                if (M_FN != null)
                {
                    if (M_FN.Length > 5)
                    {
                        Manufacturer_FN = M_FN.Substring(0, M_FN.Length - 4);
                    }
                }

                //Выбор оператора
                string Name_operator = System.IO.File.ReadAllText("Authorization.txt");
                string adr_file = System.IO.File.ReadAllText("adr_file.txt");
                string adr_file_save = null;


                FolderBrowserDialog Browserdialog = new FolderBrowserDialog(); //открытие проводника и выбор папки сохраннения
                Browserdialog.RootFolder = Environment.SpecialFolder.Desktop;
                Browserdialog.SelectedPath = adr_file.Remove(adr_file.Length - 2);

                if (Browserdialog.ShowDialog() == DialogResult.OK)
                {
                    adr_file_save = Browserdialog.SelectedPath + "\\";
                }
                //Pass the filepath and filename to the StreamWriter Constructor

                StreamWriter sw = new StreamWriter(adr_file_save + ID_Сlient + "_" + NameOrganization_save + ".txt");
                //Write a line of text
                if (Model_KKT == " ")
                {
                    sw.WriteLine("______________АКТ ВВОДА В ЭКСПЛУАТАЦИЮ______________");
                }
                sw.WriteLine("ЗН ККТ# " + ZN_KKT + " #");
                sw.WriteLine("Модель ККТ# " + Model_KKT + " #");
                sw.WriteLine("Номер автомата# " + N_av + " #");
                sw.WriteLine("Номер ФН# " + N_FN + " #");
                sw.WriteLine("Модель ФН# " + M_FN + " #");
                sw.WriteLine();
                sw.WriteLine("ID клиента# " + ID_Сlient + " #");
                sw.WriteLine("Наименование организации# " + NameOrganization + " #");
                sw.WriteLine("Руководитель организации# " + Director_org + " #");
                sw.WriteLine("ИНН организации# " + INN_Organization + " #");
                sw.Write("СНО:# " + SNO_OSN + " #");
                sw.Write(SNO_USN_D + "#"); sw.Write(SNO_USN_D_R + "#"); sw.Write(SNO_PATENT + "#"); sw.WriteLine(SNO_ESHN + " #");
                sw.WriteLine("Телефон# " + Telephone + " #");
                sw.WriteLine("Почта# " + Email + " #");
                sw.WriteLine();
                sw.WriteLine("Адрес расчетов# " + Address_ras + " #");
                sw.WriteLine("Место расчетов# " + Place_ras + " #");
                sw.WriteLine();
                sw.WriteLine("ОФД# " + OFD + " #");
                sw.WriteLine("ИНН ОФД# " + INN_OFD + " #");
                sw.WriteLine("Производитель ФН# " + Manufacturer_FN + " #");
                sw.WriteLine();
                sw.WriteLine("РНМ# " + RNM + " #");
                sw.WriteLine("Дата# " + D_FD + " #");
                sw.WriteLine("Время# " + T_FD + " #");
                sw.WriteLine("Номер ФД# " + N_FD + " #");
                sw.WriteLine("ФП# " + FP + " #");
                sw.WriteLine("Домен сайта Интернет# " + Adr_Internet + " #");
                sw.WriteLine();
                sw.WriteLine("Автономный режим# " + PrAvtonomS + " #");
                sw.WriteLine("Признак проведения лотереи# " + PrLotereyaS + " #");
                sw.WriteLine("Признак проведения азартных игр# " + PrAzartS + " #");
                sw.WriteLine("Признак деятельности банковского агента# " + PrBankPlatS + " #");
                sw.WriteLine("Признак деятельности платежного агента# " + PrPlatAgentS + " #");
                sw.WriteLine("Признак установки принтера в автомате# " + PrAvtomatUstrS +  " #");
                sw.WriteLine("Применение только в Интернет# " + PrInternetS + " #");
                sw.WriteLine("Применение в сфере услуг# " + PrRazvozS + " #");
                sw.WriteLine("Признак работы с подакцизными товарами# " + PrAkxizTovarS + " #");
                sw.WriteLine("Признак работы с маркированными товарами# " + PrMarkS + " #");
                sw.WriteLine();
                sw.WriteLine("КПП организации# " + KPP_Organization + " #");
                //Close the file
                sw.Close();

                MaterialMessageBox.Show(
        "Файл сохранен",
        "Сообщение");
            }
            catch (Exception ex)
            {
                MaterialMessageBox.Show(
        "Ошибка при формировании файла TXT: " + ex,
        "Ошибка");
            }
            finally
            {

            }
        }        
    }
}
