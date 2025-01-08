using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using KitCashProtocol;
using MaterialSkin;
using MaterialSkin.Controls;
using System.IO.Compression;
using System.Configuration;
using System.Data.SQLite;
using System.IO.Ports;
using System.Net.NetworkInformation;



namespace Kassa
{
    public partial class Form_Start : MaterialForm
    {
        public bool Internet_status = false;
        public int t = 0;
        public long result;
        public string adr_file;
        public string delete_xml;
        public string print_akt;
        public string name_operator;
        public string standart_model_FN;
        public string standart_OFD;
        public string standart_ModelKKT = "Терминал-ФА";
        public string vers_config = "------";
        public string vers_FFD = "------";

        public string M_FN;
        public bool otherModelFN = false;
        public bool[] Save_parametrs = new bool[39];

        // Заполнение версии программы на всех 4 страницах
        string program_version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        private TerminalFA CashRegister { get; set; }

        // Получаем строку подключения из App.config
        public string connectionString = ConfigurationManager.ConnectionStrings["SQLiteDB"].ConnectionString;



        public Form_Start()
        {
            InitializeComponent();     
            
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Indigo500, Primary.Indigo700, Primary.Amber50, Accent.Indigo400, TextShade.WHITE);
            
        }



        private void Form_Start_Load(object sender, EventArgs e)
        {
            
                // Создание подключения к базе данных
                using (var sqliteConnection = new SQLiteConnection(connectionString))
            {
                // Открываем соединение
                sqliteConnection.Open();
                try
                {               

                //запрос Адреса сохранения по умолчанию
                string selectQuery = "SELECT * FROM options_program WHERE parameter = @adr_file";
                using (SQLiteCommand command = new SQLiteCommand(selectQuery, sqliteConnection))
                {
                    command.Parameters.AddWithValue("@adr_file", "adr_file");
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())   // построчно считываем данные
                        {
                            adr_file = (string)reader["meaning"];
                        }
                    }
                }
                //запрос ОФД по умолчанию сохранения по умолчанию
                selectQuery = "SELECT * FROM options_program WHERE parameter = @standart_OFD";

                using (SQLiteCommand command = new SQLiteCommand(selectQuery, sqliteConnection))
                {
                    command.Parameters.AddWithValue("@standart_OFD", "standart_OFD");
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())   // построчно считываем данные
                        {
                            standart_OFD = (string)reader["meaning"];
                            //delete_xml = (string)reader[1];
                            //standart_model_FN = (string)reader[1];
                            //standart_OFD = (string)reader[1];
                            //name_operator = (string)reader[1];
                        }
                    }
                }
                    //запрос ФН по умолчанию сохранения по умолчанию
                    selectQuery = "SELECT * FROM options_program WHERE parameter = @standart_FN";

                    using (SQLiteCommand command = new SQLiteCommand(selectQuery, sqliteConnection))
                    {
                        command.Parameters.AddWithValue("@standart_FN", "standart_FN");
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())   // построчно считываем данные
                            {
                                standart_model_FN = (string)reader["meaning"];
                                //delete_xml = (string)reader[1];
                                //standart_model_FN = (string)reader[1];
                                //standart_OFD = (string)reader[1];
                                //name_operator = (string)reader[1];
                            }
                        }
                    }
                    //запрос del_XML по умолчанию сохранения по умолчанию
                    selectQuery = "SELECT * FROM options_program WHERE parameter = @delete_xml";

                    using (SQLiteCommand command = new SQLiteCommand(selectQuery, sqliteConnection))
                    {
                        command.Parameters.AddWithValue("@delete_xml", "del_xml");
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())   // построчно считываем данные
                            {
                                delete_xml = (string)reader["meaning"];
                                //delete_xml = (string)reader[1];
                                //standart_model_FN = (string)reader[1];
                                //standart_OFD = (string)reader[1];
                                //name_operator = (string)reader[1];
                            }
                        }
                    }

                    //запрос print_akt по умолчанию сохранения по умолчанию
                    selectQuery = "SELECT * FROM options_program WHERE parameter = @print_akt";

                    using (SQLiteCommand command = new SQLiteCommand(selectQuery, sqliteConnection))
                    {
                        command.Parameters.AddWithValue("@print_akt", "print_akt");
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())   // построчно считываем данные
                            {
                                print_akt = (string)reader["meaning"];
                                //delete_xml = (string)reader[1];
                                //standart_model_FN = (string)reader[1];
                                //standart_OFD = (string)reader[1];
                                //name_operator = (string)reader[1];
                            }
                        }
                    }

                    //запрос name_operator по умолчанию сохранения по умолчанию
                    selectQuery = "SELECT * FROM options_program WHERE parameter = @name_operator";
                    using (SQLiteCommand command = new SQLiteCommand(selectQuery, sqliteConnection))
                    {
                        command.Parameters.AddWithValue("@name_operator", "name_operator");
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())   // построчно считываем данные
                            {
                                name_operator = (string)reader["meaning"];
                                //delete_xml = (string)reader[1];
                                //standart_model_FN = (string)reader[1];
                                //standart_OFD = (string)reader[1];
                                //name_operator = (string)reader[1];
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MaterialMessageBox.Show("Ошибка: " + ex.Message);
                }

                
                labelVers1.Text = program_version;
                labelVers2.Text = program_version;
                labelVers3.Text = program_version;
                labelVers4.Text = program_version;

                //_______________________________________________________________________ Заполнение Параметров ОФД данными из Базы
                string name_OFD = standart_OFD;
                string query = @"
                SELECT 
                    inn_OFD, 
                    email_OFD, 
                    adress_OFD, 
                    IP_OFD, 
                    TCP_OFD, 
                    DNS_OFD, 
                    adress_OISM_OFD,
                    port_OFD 
                FROM options_OFD 
                WHERE name_OFD = @name_OFD";
                //using (SQLiteConnection sqliteConnection = new SQLiteConnection(connectionString))
                //{
                //    // Открываем соединение
                //    sqliteConnection.Open();

                    using (SQLiteCommand sqliteCommand = new SQLiteCommand(query, sqliteConnection))
                    {
                        // Добавление параметра к запросу для предотвращения SQL-инъекций
                        sqliteCommand.Parameters.AddWithValue("@name_OFD", name_OFD);

                        try
                        {
                            // Выполнение запроса
                            using (SQLiteDataReader reader = sqliteCommand.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    // Вкладка 1
                                    TextBox_INN_OFD1.Text = reader["inn_OFD"].ToString();
                                    TextBox_Email_OFD1.Text = reader["email_OFD"].ToString();

                                    // Вкладка 2
                                    TextBox_INN_OFD2.Text = reader["inn_OFD"].ToString();

                                    // Вкладка 3
                                    TextBox_INN_OFD3.Text = reader["inn_OFD"].ToString();
                                    TextBox_Email_OFD3.Text = reader["email_OFD"].ToString();
                                    TextBox_adress_OFD3.Text = reader["adress_OFD"].ToString();
                                    TextBox_IP_OFD3.Text = reader["IP_OFD"].ToString();
                                    TextBox_TCP_OFD3.Text = reader["TCP_OFD"].ToString();
                                    TextBox_DNS_OFD3.Text = reader["DNS_OFD"].ToString();
                                    TextBox_adress2_OFD3.Text = reader["adress_OISM_OFD"].ToString();
                                    TextBox_port_OFD3.Text = reader["port_OFD"].ToString();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MaterialMessageBox.Show("Ошибка: " + ex.Message);
                        }
                    //}
                }
                //_______________________________________________________________________ Заполнение Параметров ФН на Странице 3 данными из Базы
                string name_FN = "Инвента";
                query = @"
                SELECT  
                    adress_FN, 
                    port_FN
                FROM options_FN 
                WHERE name_FN = @name_FN";
                //using (SQLiteConnection sqliteConnection = new SQLiteConnection(connectionString))
                //{
                    using (SQLiteCommand sqliteCommand = new SQLiteCommand(query, sqliteConnection))
                    {
                        // Добавление параметра к запросу для предотвращения SQL-инъекций
                        sqliteCommand.Parameters.AddWithValue("@name_FN", name_FN);

                        try
                        {
                            using (SQLiteDataReader reader = sqliteCommand.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    // Вкладка 3
                                    TextBox_adress_FN3.Text = reader["adress_FN"].ToString();
                                    TextBox_port_FN3.Text = reader["port_FN"].ToString();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MaterialMessageBox.Show("Ошибка: " + ex.Message);
                        }
                    //}
                }

                //_______________________________________________________________________ Заполнение ComboBox_Name_OFD массивом из Базы
                // SQL-запрос для получения значений name_OFD
                query = "SELECT name_OFD FROM options_OFD ORDER BY id_ofd";
                //using (SQLiteConnection sqliteConnection = new SQLiteConnection(connectionString))
                //{
                    using (SQLiteCommand sqliteCommand = new SQLiteCommand(query, sqliteConnection))
                    {
                        try
                        {
                            using (SQLiteDataReader reader = sqliteCommand.ExecuteReader())
                            {
                                // Очистка текущих элементов ComboBox
                                ComboBox_Name_OFD1.Items.Clear();
                                ComboBox_Name_OFD2.Items.Clear();
                                ComboBox_Name_OFD3.Items.Clear();
                                ComboBox_Name_OFD4.Items.Clear();

                                // Заполнение ComboBox значениями из базы данных
                                while (reader.Read())
                                {
                                    ComboBox_Name_OFD1.Items.Add(reader["name_OFD"].ToString());
                                    ComboBox_Name_OFD2.Items.Add(reader["name_OFD"].ToString());
                                    ComboBox_Name_OFD3.Items.Add(reader["name_OFD"].ToString());
                                    ComboBox_Name_OFD4.Items.Add(reader["name_OFD"].ToString());
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MaterialMessageBox.Show("Ошибка при заполнении параметров списка ОФД: " + ex.Message);
                        }
                    }
                //}
                // Постановка значений по умолчанию в ComboBox на всех вкладках
                ComboBox_Name_OFD1.SelectedItem = standart_OFD;
                ComboBox_Name_OFD2.SelectedItem = standart_OFD;
                ComboBox_Name_OFD3.SelectedItem = standart_OFD;
                ComboBox_Name_OFD4.SelectedItem = standart_OFD;
                ComboBox_Model_FN4.SelectedItem = standart_OFD;

                

                


                //_______________________________________________________________________ Заполнение ComboBox_Model_FN1 массивом из Базы

                query = "SELECT model_FN FROM table_model_FN";
                //using (SQLiteConnection sqliteConnection = new SQLiteConnection(connectionString))
                //{
                    using (SQLiteCommand sqliteCommand = new SQLiteCommand(query, sqliteConnection))
                    {
                        try
                        {
                            using (SQLiteDataReader reader = sqliteCommand.ExecuteReader())
                            {
                                // Очистка текущих элементов ComboBox
                                ComboBox_Model_FN1.Items.Clear();
                                ComboBox_Model_FN4.Items.Clear();


                                // Заполнение ComboBox значениями из базы данных
                                while (reader.Read())
                                {
                                    ComboBox_Model_FN1.Items.Add(reader["model_FN"].ToString());
                                    ComboBox_Model_FN4.Items.Add(reader["model_FN"].ToString());
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MaterialMessageBox.Show("Ошибка при заполнении параметров списка ОФД: " + ex.Message);
                        }
                    }
                    
                //}
                // Постановка значений по умолчанию в ComboBox на всех вкладках
                ComboBox_Model_FN1.SelectedItem = standart_model_FN;
                ComboBox_Model_FN4.SelectedItem = standart_model_FN;


                

                for (int i = 0; i < Save_parametrs.Length; i++) // Массив проверки сохранения
                {
                    Save_parametrs[i] = true;
                }
                label_save_status.Text = "";
                label_image_save_status.Text = "";

            }
            
        }

        
        private void OFD_TextChanged(object sender, EventArgs e) // заполнение полей ИНН ОФД и почта отправителя
        {
            string name_OFD = ComboBox_Name_OFD1.Text;
            string query = @"
            SELECT 
                inn_OFD, 
                email_OFD
                FROM options_OFD 
            WHERE name_OFD = @name_OFD";
            using (SQLiteConnection sqliteConnection = new SQLiteConnection(connectionString))
            {
                // Открытие соединения
                sqliteConnection.Open();
                using (SQLiteCommand sqliteCommand = new SQLiteCommand(query, sqliteConnection))
                {
                    // Добавление параметра к запросу для предотвращения SQL-инъекций
                    sqliteCommand.Parameters.AddWithValue("@name_OFD", name_OFD);

                    try
                    {

                        // Выполнение запроса и получение результата
                        using (SQLiteDataReader reader = sqliteCommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Запись значений в TextBox'ы
                                TextBox_INN_OFD1.Text = reader["inn_OFD"].ToString();
                                TextBox_Email_OFD1.Text = reader["email_OFD"].ToString();

                            }
                        }
                    }

                    catch (Exception ex)
                    {
                        // Обработка возможных ошибок
                        MaterialMessageBox.Show("Ошибка: " + ex.Message);
                    }
                }
            }
            Save_parametrs[14] = false;
        }

        // __________________________ Проверки на изменения перед перез закрытием и на правильность ввода данных _____________
        private void Model_KKT_Changet(object sender, EventArgs e) // Проверка Модели ККТ
        {
            Save_parametrs[0] = false;
            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void ZN_KKT_TextChanged(object sender, EventArgs e) // Проверка ЗН ККТ, заполнение номера автомата
        {
            TextBox_Number_automatic.Text = TextBox_ZN_KKT.Text.Substring(Math.Max(0, TextBox_ZN_KKT.Text.Length - 6));
            Save_parametrs[1] = false;
            TextBox_ZN_KKT.Text = TextBox_ZN_KKT.Text.Replace(" ", "");
            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void ZN_KKT_Leave(object sender, EventArgs e) // проверки ЗН ККТ
        {
            if ((TextBox_ZN_KKT.Text.Length != 12) && (TextBox_Model_KKT.Text == "Терминал-ФА") && TextBox_ZN_KKT.Text.Length != 0)
            {
                MaterialMessageBox.Show("Некорректно указан Заводской номер ККТ. Номер должен содержать 12 символов");
            }
            if (TextBox_ZN_KKT.Text.Length != 0)
            {
                try { result = Convert.ToInt64(TextBox_ZN_KKT.Text); }
                catch { MaterialMessageBox.Show("В поле Заводской номер ККТ допускается ввод только цифр"); }
            }

        }
        private void Number_automatic_Changet(object sender, EventArgs e) // Проверка Номера автомата
        {
            Save_parametrs[2] = false;
            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void Model_FN1_Changet(object sender, EventArgs e) // Проверка Модели ФН
        {
            Save_parametrs[3] = false;
            if ((CheckBox_Podakziz.Checked == true) && (ComboBox_Model_FN1.Text.Substring(2, 2) != "15")) 
            { 
                MaterialMessageBox.Show("Некорретный выбор модели ФН. С Подакцизными товарами можно работать только на ФН 15 месяцев");
            }
            if ((Checkbox_OSN.Checked == true) && (ComboBox_Model_FN1.Text.Substring(2, 2) != "15"))
            {
                MaterialMessageBox.Show("Некорретный выбор модели ФН. С системой налогоообложения ОСН можно работать только на ФН 15 месяцев");
            }
            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void ZN_FN_Changet(object sender, EventArgs e) // ЗН ФН + автоподстановка модели ФН по номеру ФН
        {
            
            Save_parametrs[4] = false;
            if (TextBox_ZN_FN.Text.Length > 7)
            {
                if (TextBox_ZN_FN.Text.Substring(0, 8) == "73814408") { ComboBox_Model_FN1.Text = null; ComboBox_Model_FN1.SelectedIndex = 0; } // Ин36-4
                if (TextBox_ZN_FN.Text.Substring(0, 8) == "72804405") { ComboBox_Model_FN1.Text = null; ComboBox_Model_FN1.SelectedIndex = 1; } // Ин36-3
                if (TextBox_ZN_FN.Text.Substring(0, 8) == "73804408") { ComboBox_Model_FN1.Text = null; ComboBox_Model_FN1.SelectedIndex = 7; } // Ин15-4
                if (TextBox_ZN_FN.Text.Substring(0, 8) == "72814407") { ComboBox_Model_FN1.Text = null; ComboBox_Model_FN1.SelectedIndex = 2; } // Ин15-3
                if (TextBox_ZN_FN.Text.Substring(0, 8) == "99604403") { ComboBox_Model_FN1.Text = null; ComboBox_Model_FN1.SelectedIndex = 10; } // Ин15-1
                if (TextBox_ZN_FN.Text.Substring(0, 8) == "72824405") { ComboBox_Model_FN1.Text = null; ComboBox_Model_FN1.SelectedIndex = 5; } // Эв15-3
                //if (TextBox_ZN_FN.Text.Substring(0, 8) == "") { ComboBox_Model_FN1.Text = null; ComboBox_Model_FN1.SelectedIndex = "Эв36-3"; }
                if (TextBox_ZN_FN.Text.Substring(0, 8) == "72844405") { ComboBox_Model_FN1.Text = null; ComboBox_Model_FN1.SelectedIndex = 3;  }  // Ав15-3
                if (TextBox_ZN_FN.Text.Substring(0, 8) == "72854405") { ComboBox_Model_FN1.Text = null; ComboBox_Model_FN1.SelectedIndex = 4; } // Ав36-3
            }
            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void ZN_FN_Leave(object sender, EventArgs e) // Проверка ЗН ФН
        {
            if (TextBox_ZN_FN.Text.Length != 16 && TextBox_ZN_FN.Text.Length != 0)
            {
                MaterialMessageBox.Show("Некорректно указан заводской номер ФН. Номер должен содержать 16 символов");
            }
            if (TextBox_ZN_FN.Text.Length != 0)
            {
                try { result = Convert.ToInt64(TextBox_ZN_FN.Text); }
                catch { MaterialMessageBox.Show("В поле Заводской номер ФН допускается ввод только цифр"); }
            }
        }
        private void ID_Changet(object sender, EventArgs e) //автоудаление пробелов в ID Клиента
        {
            TextBox_ID_client.Text = TextBox_ID_client.Text.Replace(" ", "");
            Save_parametrs[5] = false;
            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void ID_Leave(object sender, EventArgs e) // Проверка ID
        {
            if (TextBox_ID_client.Text.Length != 0)
            {
                try { result = Convert.ToInt64(TextBox_ID_client.Text); }
                catch { MaterialMessageBox.Show("В поле ID клиента допускается ввод только цифр"); }
            }
        }
        private void NameOr_TextChanged(object sender, EventArgs e) // открытие поля КПП если ЮЛ и ввод имя руководителя
        {
            string[] n = TextBox_Name_organization.Text.Split(' ');
            string NOrganization = n[0];
            if (NOrganization != "ИП" && NOrganization.Length > 2)
            {
                TextBox_KPP_organization.Visible = true; // открытие поля КПП
                TextBox_KPP_organization.Visible = true; // открытие поля КПП
            }
            else if (NOrganization == "ИП" && TextBox_Name_organization.Text.Length > 2)
            {
                TextBox_KPP_organization.Visible = false;
                TextBox_Director_org.Text = TextBox_Name_organization.Text.Substring(Math.Max(0, 3)); // ввод имя руководителя
            }
            Save_parametrs[6] = false;
            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void Director_org_Changet(object sender, EventArgs e) // ФИО Руководителя
        {
            Save_parametrs[7] = false;
            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void INNOr_TextChanged(object sender, EventArgs e) // ИНН Организации
        {
            TextBox_INN_organization.Text = TextBox_INN_organization.Text.Replace(" ", "");
            Save_parametrs[8] = false;
            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void INNOr_Leave(object sender, EventArgs e) // Проверка ИНН Организации
        {
            if (TextBox_INN_organization.Text.Length != 10 && TextBox_INN_organization.Text.Length != 12 && TextBox_INN_organization.Text.Length != 0)
            {
                MaterialMessageBox.Show("Некорректно указан ИНН организации. ИНН должен состоять из 10 (ЮЛ) или 12 (ИП) символов");
                try { result = Convert.ToInt64(TextBox_INN_organization.Text); }
                catch { MaterialMessageBox.Show("В поле ИНН Организации допускается ввод только цифр"); }
            }
        }
        private void KPP_organization_Chenged(object sender, EventArgs e) // КПП Организации
        {
            TextBox_KPP_organization.Text = TextBox_KPP_organization.Text.Replace(" ", "");
            Save_parametrs[9] = false;
            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void KPP_organization_Leave(object sender, EventArgs e) // Проверка КПП Организации
        {
            if (TextBox_KPP_organization.Text.Length != 9 && TextBox_KPP_organization.Text.Length != 0)
            {
                MaterialMessageBox.Show("Некорректно указан КПП организации. КПП должен состоять из 9 символов");
            }
            try { result = Convert.ToInt64(TextBox_KPP_organization.Text); }
            catch { MaterialMessageBox.Show("В поле КПП Организации допускается ввод только цифр"); }
        }
        private void Telephone_Changet(object sender, EventArgs e) //автоудаление символом из Номера телефона
        {
            bool remove9 = false;
            string Telephone = TextBox_Telephon_number.Text;

            if ((Telephone.Length == 19) && (Telephone.Substring(0, 3) == "+ 7") && (remove9 == false))
            {
                Telephone = Telephone.Replace("+ 7", "7");
            }
            if ((Telephone.Length == 19) && (Telephone.Substring(0, 3) == "+ 8"))
            {
                Telephone = Telephone.Replace("+ 8", "7");
            }
            TextBox_Telephon_number.Text = Telephone;
            Save_parametrs[10] = false;
            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void Email_Changet(object sender, EventArgs e) // Email организации
        {
            Save_parametrs[11] = false;
            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void Adress_Changet(object sender, EventArgs e) // Адрес расчетов
        {
            Save_parametrs[12] = false;
            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void Place_Changet(object sender, EventArgs e) // Место расчетов
        {
            Save_parametrs[13] = false;
            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void INN_OFD_Changet(object sender, EventArgs e) // ИНН ОФД
        {
            Save_parametrs[15] = false;
            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void INN_OFD_Leave(object sender, EventArgs e) // Проверка ИНН ОФД
        {
            if (TextBox_INN_OFD1.Text.Length != 10 && TextBox_INN_OFD1.Text.Length != 0)
            {
                MaterialMessageBox.Show("Некорректно указан ИНН ОФД. ИНН должен состоять из 10 символов");
                try { result = Convert.ToInt64(TextBox_INN_OFD1.Text); }
                catch
                {
                    MaterialMessageBox.Show("В поле ИНН ОФД допускается ввод только цифр");
                }
            }
        }
        private void Name_OFD_Textbox_Changet(object sender, EventArgs e) // ОФД в TextBox
        {
            Save_parametrs[16] = false;
            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void Email_OFD_Changet(object sender, EventArgs e) // Email ОФД
        {
            Save_parametrs[17] = false;
            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void RNM_Changed(object sender, EventArgs e) //автоудаление пробелов в РНМ
        {
            TextBox_RNM1.Text = TextBox_RNM1.Text.Replace(" ", "");
            Save_parametrs[18] = false;
            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void RNM_Leave(object sender, EventArgs e) // Проверка РНМ
        {
            if (TextBox_RNM1.Text.Length != 16 && TextBox_RNM1.Text.Length != 0)
            {
                MaterialMessageBox.Show("Некорректно указан РНМ. РНМ должен состоять из 16 символов");
            }
            if (TextBox_RNM1.Text.Length != 0)
            {
                try { result = Convert.ToInt64(TextBox_RNM1.Text); }
                catch { MaterialMessageBox.Show("В поле РНМ допускается ввод только цифр"); }
            }
        } 
        private void Number_FD_Changed(object sender, EventArgs e) // Номер ФД
        {
            Save_parametrs[19] = false;
            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
            if (TextBox_Number_FD.Text.Length != 0)
            {
                try { result = Convert.ToInt64(TextBox_Number_FD.Text); }
                catch { MaterialMessageBox.Show("В поле Номер ФД допускается ввод только цифр"); }
            }
        }
        private void Datetime_Changed(object sender, EventArgs e) // Дата и время ФД
        {
            Save_parametrs[20] = false;
            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void Datetime_Leave(object sender, EventArgs e) // Проверка Даты и времени ФД
        { 
            
            string D_FD = TextBox_Datetime_FD.Text;
            if (D_FD[0] != ' ' && D_FD[1] != ' ' && D_FD[3] != ' ' && D_FD[4] != ' ' && D_FD[6] != ' ' && D_FD[7] != ' ' && D_FD[8] != ' ' && D_FD[9] != ' ' && D_FD[11] != ' ' && D_FD[12] != ' ' && D_FD[14] != ' ' && D_FD[15] != ' '){
                //char a0 = D_FD[0];
                //char a1 = D_FD[1];
                //char a2 = D_FD[2];
                //char a3 = D_FD[3];
                //char a4 = D_FD[4];
                //char a5 = D_FD[5];
                //char a6 = D_FD[6];
                //char a7 = D_FD[7];
                //char a8 = D_FD[8];
                //char a9 = D_FD[9];
                //char a10 = D_FD[10];
                //char a11 = D_FD[11];
                //char a12 = D_FD[12];
                //char a13 = D_FD[13];
                //char a14 = D_FD[14];
                //char a15 = D_FD[15];


                if (D_FD[2] != '.' || D_FD[5] != '.' || D_FD[10] != ' ' || D_FD[13] != ':')
                {
                    MaterialMessageBox.Show("Некорректно указанны дата и время. Введите по следующему формату: дд.мм.гггг чч:мм");
                }
                if (D_FD[0] != '0' && D_FD[0] != '1' && D_FD[0] != '2' && D_FD[0] != '3') // ограничения первого числа дней
                { MaterialMessageBox.Show("Некорректно указано число"); }

                if (D_FD[0] == '0' && D_FD[1] == '0') { MaterialMessageBox.Show("Некорректно указано число"); } // ограничение 0 месяца

                if (D_FD[3] != '0' && D_FD[3] != '1') { MaterialMessageBox.Show("Некорректно указан месяц"); } // ограничение первого числа месяца

                if (D_FD[3] == '0' && D_FD[4] == '0') { MaterialMessageBox.Show("Некорректно указан месяц"); } // ограничение 0 месяца

                if (D_FD[3] == '1' && (D_FD[4] != '0' && D_FD[4] != '1' & D_FD[4] != '2')) { MaterialMessageBox.Show("Некорректно указан месяц"); } // ограничение второй цифры месяца

                if (D_FD[11] != '0' && D_FD[11] != '1' && D_FD[11] != '2') { MaterialMessageBox.Show("Некорректно указаны часы"); } // ограничение первой цифры часа

                if (D_FD[11] == '2' && (D_FD[12] != '0' && D_FD[12] != '1' && D_FD[12] != '2' && D_FD[12] != '3')) { MaterialMessageBox.Show("Некорректно указаны часы"); } // ограничения второй цифры часа

                if (D_FD[14] != '0' && D_FD[14] != '1' && D_FD[14] != '2' && D_FD[14] != '3' && D_FD[14] != '4' && D_FD[14] != '5') { MaterialMessageBox.Show("Некорректно указаны минуты"); } // ограничение первой цифры минут

                if (D_FD[3] == '0' && D_FD[4] == '2' && (D_FD[0] != '0' && D_FD[0] != '1' && D_FD[0] != '2')) // проверка феврала
                {
                    MaterialMessageBox.Show("В феврале может быть только 28 или 29 дней");
                }
                if (D_FD[3] == '0' && (D_FD[4] == '3' || D_FD[4] == '6' || D_FD[4] == '9' || D_FD[4] == '1')) // проверка месяцев с 30 днями
                {
                    if (D_FD[0] == '3' && D_FD[1] != '0')
                        MaterialMessageBox.Show("Указан месяц в котором не может быть 31 день.");
                }

                if (D_FD[0] == '3' && (D_FD[1] != '0' && D_FD[1] != '1')) { MaterialMessageBox.Show("Указано некорректное число"); } }
        }
        private void FP_FD_Changed(object sender, EventArgs e) // ФП ФД
        {
            Save_parametrs[21] = false;
            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void FP_FD_Leave(object sender, EventArgs e) // ФП ФД
        {
            if (TextBox_FP_FD.Text.Length != 10 && TextBox_FP_FD.Text.Length != 9 && TextBox_FP_FD.Text.Length != 0)
            {
                MaterialMessageBox.Show("Некорректно указан Фискальный признак документа. ФП должен состоять из 10 или 9 символов");
            }
            if (TextBox_FP_FD.Text.Length != 0)
            {
                try { result = Convert.ToInt64(TextBox_FP_FD.Text); }
                catch { MaterialMessageBox.Show("В поле Фискальный признак допускается ввод только цифр"); }
            }
        }
        // _________________________________________________________ Перечень СНО
        private void SNO_OSN_Checked(object sender, EventArgs e)
        {
            Save_parametrs[22] = false;
            if ((Checkbox_OSN.Checked == true) && (ComboBox_Model_FN1.Text.Substring(2, 2) != "15"))
            {
                MaterialMessageBox.Show("Некорретный выбор СНО. С системой налогоообложения ОСН можно работать только на ФН 15 месяцев");
            }
            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void SNO_USN_Dohod_Checked(object sender, EventArgs e)
        {
            Save_parametrs[23] = false;
            if ((Checkbox_USN_Dohod.Checked == true) && (ComboBox_Model_FN1.Text.Substring(2, 2) == "15"))
            {
                MaterialMessageBox.Show("Некорретный выбор СНО. С системой налогоообложения УСН можно работать только на ФН 36 месяцев");
            }
            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void SNO_USN_Dohod_rashod_Checked(object sender, EventArgs e)
        {
            Save_parametrs[24] = false;
            if ((Checkbox_USN_Dohod_rashod.Checked == true) && (ComboBox_Model_FN1.Text.Substring(2, 2) == "15"))
            {
                MaterialMessageBox.Show("Некорретный выбор СНО. С системой налогоообложения УСН можно работать только на ФН 36 месяцев");
            }
            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void SNO_Patent_Checked(object sender, EventArgs e)
        {
            Save_parametrs[25] = false;
            if ((Checkbox_Patent.Checked == true) && (ComboBox_Model_FN1.Text.Substring(2, 2) == "15"))
            {
                MaterialMessageBox.Show("Некорретный выбор СНО. С системой налогоообложения ПАТЕНТ можно работать только на ФН 36 месяцев");
            }
            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void SNO_ESHN_Checked(object sender, EventArgs e)
        {
            Save_parametrs[26] = false;
            if ((Checkbox_ESHN.Checked == true) && (ComboBox_Model_FN1.Text.Substring(2, 2) == "15"))
            {
                MaterialMessageBox.Show("Некорретный выбор СНО. С системой налогоообложения ЕСХН можно работать только на ФН 36 месяцев");
            }
            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        // _________________________________________________________ Перечень режимов работы
        private void Podakziz_Checked(object sender, EventArgs e)
        {
            Save_parametrs[27] = false;
            if ((CheckBox_Podakziz.Checked == true) && (ComboBox_Model_FN1.Text.Substring(2, 2) != "15"))
            {
                MaterialMessageBox.Show("Некорретный выбор режима работы. Для работы с подакцизными товарами требуется ФН 15 месяцев");
            }
            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void Mark_Checked(object sender, EventArgs e)
        {
            Save_parametrs[28] = false;
            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void Azart_play_Checked(object sender, EventArgs e)
        {
            Save_parametrs[29] = false;
            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void Lotereya_Checked(object sender, EventArgs e)
        {
            Save_parametrs[30] = false;
            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void Printer_v_avtomate_Checked(object sender, EventArgs e)
        {
            Save_parametrs[31] = false;
            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void Bank_agent_Checked(object sender, EventArgs e)
        {
            Save_parametrs[32] = false;
            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void Plat_agent_Checked(object sender, EventArgs e)
        {
            Save_parametrs[33] = false;
            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void Shifr_Checked(object sender, EventArgs e)
        {
            Save_parametrs[34] = false;
            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void Avtonom_Checked(object sender, EventArgs e)
        {
            Save_parametrs[35] = false;
            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void Domen_Changed(object sender, EventArgs e)
        {
            Save_parametrs[38] = false;
            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void Internet_Checked(object sender, EventArgs e) // проверка одновременной развозной торговли и интернет и выпадающий адрес Интернет
        {
            if ((CheckBox_Delivery.Checked == true)&&(CheckBox_Internet.Checked == true))
            {
                MaterialMessageBox.Show(
                    "Запрещено отмечать в параметрах регистрации одновременно развозную торговлю и применение ККТ в сети Интернет. Измените выбор параметров",
                "Оповещение");
                CheckBox_Internet.Checked = false;
            }
            if (CheckBox_Internet.Checked == true) //Открытие поля Домен сайта
            {
                TextBox_Domen.Visible = true;
                TextBox_Domen.Text = TextBox_PlaceSale.Text;
            }
            if (CheckBox_Internet.Checked == false) //Закрытие поля Домен сайта
            {
                TextBox_Domen.Visible = false;
                TextBox_Domen.Clear();
            }
            Save_parametrs[37] = false;
            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        private void Delivery_Checked(object sender, EventArgs e) // проверка одновременной развозной торговли и интернет
        {
            if ((CheckBox_Delivery.Checked == true) && (CheckBox_Internet.Checked == true))
            {
                MaterialMessageBox.Show(
                    "Запрещено отмечать в параметрах регистрации одновременно развозную торговлю и применение ККТ в сети Интернет. Измените выбор параметров",
                "Оповещение");
                CheckBox_Delivery.Checked = false;
            }

            if ((CheckBox_Delivery.Checked == true) && (CheckBox_Internet.Checked == false) && (TextBox_PlaceSale.Text.Contains("Курьер") == false))
            {
                string text_place_calculations = TextBox_PlaceSale.Text + "; Курьер"; // добавление к месту расчетов "Курьер" при отметке развозной торговли
                TextBox_PlaceSale.Text = text_place_calculations;
            }
            Save_parametrs[38] = false;
            label_save_status.Text = "Требуется сохранение";
            label_image_save_status.Text = "×";
        }
        
        private void buttonParOFD_Click(object sender, EventArgs e) // кнопка Параметры ОФД
        {

            DanReg f = new DanReg(this.ComboBox_Name_OFD1.Text, this.ComboBox_Model_FN1.Text);
            f.ShowDialog();
        }
        public void butSave_Click(object sender, EventArgs e) // кнопка Сохранить
        {
            string adr_file_save = adr_file;
            FolderBrowserDialog Browserdialog = new FolderBrowserDialog(); //открытие проводника и выбор папки сохраннения
            Browserdialog.RootFolder = Environment.SpecialFolder.Desktop;
            Browserdialog.SelectedPath = adr_file;

            if (Browserdialog.ShowDialog() == DialogResult.OK)
            {
                adr_file_save = Browserdialog.SelectedPath;
                SaveData(adr_file_save);
            }
            else
            {
                return;
            }

            for (int i = 0; i < Save_parametrs.Length; i++) // Массив проверки сохранения
            {
                Save_parametrs[i] = true;
            }
            label_save_status.Text = "Сохранено";
            label_image_save_status.Text = "🗸";
        }
        private void butLoading_Click(object sender, EventArgs e)// кнопка Открыть
        {
            int local_close = 1;
            for (int i = 0; i < Save_parametrs.Length; i++) // Массив проверки сохранения
            {
                if (Save_parametrs[i] == false)
                {
                    local_close *= 0;
                }
            }
            if (local_close == 0)
            {
                DialogResult result = MaterialMessageBox.Show("Уверены что хотите открыть файл? Несохраненные данный на форме исчезнут", "Да", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    Clear_form();
                    local_close = 1;
                }
            }
            if (local_close == 1)
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
                    TextBox_ZN_KKT.Text = str_mas[1].Trim();
                    TextBox_Number_automatic.Text = str_mas[5].Trim();
                    TextBox_Model_KKT.Text = str_mas[3].Trim();
                    TextBox_ZN_FN.Text = str_mas[7].Trim();
                    ComboBox_Model_FN1.Text = str_mas[9].Trim();
                    TextBox_ID_client.Text = str_mas[11].Trim();
                    TextBox_Name_organization.Text = str_mas[13].Trim();
                    TextBox_Director_org.Text = str_mas[15].Trim();
                    TextBox_INN_organization.Text = str_mas[17].Trim();

                    if (str_mas[19].Trim() == "ОСН") { Checkbox_OSN.Checked = true; } // СНО
                    if (str_mas[20].Trim() == "УСН Доход") { Checkbox_USN_Dohod.Checked = true; }
                    if (str_mas[21].Trim() == "УСН Доход - расход") { Checkbox_USN_Dohod_rashod.Checked = true; }
                    if (str_mas[22].Trim() == "Патент") { Checkbox_Patent.Checked = true; }
                    if (str_mas[23].Trim() == "ЕСХН") { Checkbox_ESHN.Checked = true; }
                    TextBox_Telephon_number.Text = str_mas[25].Trim();
                    TextBox_Email_organization.Text = str_mas[27].Trim();
                    TextBox_adressSale.Text = str_mas[29].Trim();
                    TextBox_PlaceSale.Text = str_mas[31].Trim();
                    ComboBox_Name_OFD1.Text = str_mas[33].Trim();
                    TextBox_RNM1.Text = str_mas[39].Trim();
                    string d_fd = str_mas[41].Trim() + str_mas[43].Trim(); //объединение даты и времени
                    TextBox_Datetime_FD.Text = d_fd;
                    TextBox_Number_FD.Text = str_mas[45].Trim();
                    TextBox_FP_FD.Text = str_mas[47].Trim();
                    TextBox_Domen.Text = str_mas[49].Trim();


                    if (str_mas[51].Trim() == "1") { CheckBox_Avtonom.Checked = true; } // сведения регистрации ККТ
                    if (str_mas[53].Trim() == "1") { CheckBox_Lotereya.Checked = true; }
                    if (str_mas[55].Trim() == "1") { CheckBox_Azart_play.Checked = true; }
                    if (str_mas[57].Trim() == "1") { CheckBox_Bank_agent.Checked = true; }
                    if (str_mas[59].Trim() == "1") { CheckBox_Plat_agent.Checked = true; }
                    if (str_mas[61].Trim() == "1") { CheckBox_Printer_v_avtomate.Checked = true; }
                    if (str_mas[63].Trim() == "1") { CheckBox_Internet.Checked = true; }
                    if (str_mas[65].Trim() == "1") { CheckBox_Delivery.Checked = true; }
                    if (str_mas[67].Trim() == "1") { CheckBox_Podakziz.Checked = true; }
                    if (str_mas[69].Trim() == "1") { CheckBox_Mark.Checked = true; }
                    TextBox_KPP_organization.Text = str_mas[71].Trim(); //КПП организации раннее забыл подставить
                    if (str_mas.Length > 71)
                    { if (str_mas[71] == program_version) { } }
                }
                else { MaterialMessageBox.Show("Версия файла устарела. Открыть его неполучится. Прости :("); }

                for (int i = 0; i < Save_parametrs.Length; i++) // Массив проверки сохранения
                {
                    Save_parametrs[i] = true;
                }
                label_save_status.Text = "Сохранено";
                label_image_save_status.Text = "🗸";
            }
        }

        private static SerialPort Port { get; set; }
        private byte[] TLV { get; set; }
        private ushort TLVPosition { get; set; }
        private TaxType DefaultTaxType { get; set; }
        
        private void butReaddata_Click(object sender, EventArgs e) //кнопка Считать данные
        {
            string dataTime_KKT = "01.01.2000 00:00";
            bool result_open = false;
            if (switch_open_KKT1.Checked == false) 
            {
                result_open = open_KKT(!switch_open_KKT1.Checked);
            }
            if (result_open == true) {
                try
                {
                    TextBox_ZN_KKT.Text = CashRegister.GetZN(); // запрос ЗН ККТ
                    try { TextBox_ZN_FN.Text = CashRegister.GetFN(); } // запрос ЗН ФН 
                    catch { MaterialMessageBox.Show("Нет данных об ФН"); }
                    dataTime_KKT = CashRegister.GetDATATIME(); // запрос времени в ККТ
                    label_datatime.Text = dataTime_KKT;
                    vers_config = CashRegister.GetVersConfig().Replace("rw","");// запрос версии конфигурации
                    if (vers_config.Substring(4,1) == "4" || vers_config.Substring(4, 2) == "54") {vers_FFD = "1.2";}
                    else { vers_FFD = "1.05"; }
                }
                catch { MaterialMessageBox.Show("Не удалось считать данные с ККТ"); }

                DateTime dateTime;
                DateTime.TryParseExact(dataTime_KKT, "dd.MM.yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dateTime);
                // Получаем текущее время на ПК
                DateTime dateTime_PK = DateTime.Now;

                // Сравниваем разницу во времени
                TimeSpan difference = dateTime_PK - dateTime;

                // Проверяем, превышает ли разница 5 минут
                if (Math.Abs(difference.TotalMinutes) > 5)
                {
                    MaterialMessageBox.Show("Разница во времени на ККТ и в ПК более 5 минут. Введите корректное время в ККТ");
                }


                label_vers_config.Text = vers_config;
                label_vers_FFD.Text = vers_FFD;
            }
            
        }

        private void Clean_Click(object sender, EventArgs e) // Книпка Очистить поля
        {
            DialogResult result = MaterialMessageBox.Show("Уверены что хотите очистить поля?", "Да", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Clear_form();
            }


        }
        public bool open_KKT(bool isOpened)
        {
            if (isOpened == true)
            {
                try
                {
                    CashRegister = new TerminalFA("COM3");
                    ErrorCode answer = CashRegister.Initialize();
                    switch_open_KKT1.Checked = true;
                    switch_open_KKT2.Checked = true;
                    return true;
                }
                catch
                {
                    CashRegister?.CloseConnection(); // Явное закрытие соединения
                    MaterialMessageBox.Show("Не удалось подключиться к COM3");
                    switch_open_KKT1.Checked = false;
                    switch_open_KKT2.Checked = false;
                    return false;
                }
            }
            else
            {
                try
                {
                    CashRegister = new TerminalFA("COM3");
                    CashRegister.CloseConnection();
                    MaterialMessageBox.Show("Соединение с COM3 успешно закрыто.");
                    switch_open_KKT1.Checked = false;
                    switch_open_KKT2.Checked = false;
                    return false;
                }
                catch
                {
                    MaterialMessageBox.Show("Не удалось отключиться от COM3");
                    switch_open_KKT1.Checked = false;
                    switch_open_KKT2.Checked = false;
                    return false;
                }
            }
        }
        private void switchclick_openKKT1(object sender, EventArgs e) //Подключение ККТ
        {
            bool result_open = open_KKT(switch_open_KKT1.Checked);
            //if (switch_open_KKT1.Checked == true)
            //{
            //    try 
            //    { 
            //        CashRegister = new TerminalFA("COM3"); 
            //        ErrorCode answer = CashRegister.Initialize();
            //        switch_open_KKT2.Checked = true;
            //    }
            //    catch 
            //    {
            //        CashRegister.CloseConnection(); // Явное закрытие соединения
            //        MaterialMessageBox.Show("Не удалось подключиться к COM3");

            //    }
            //}
            //else 
            //{
            //    try 
            //    {
            //        CashRegister = new TerminalFA("COM3");
            //        CashRegister.CloseConnection();
            //    }
            //    catch {
            //        MaterialMessageBox.Show("Не удалось отключиться COM3");
            //    }

            //}
        }
        private void buttonXML_Click(object sender, EventArgs e) // кнопка Файл регистрации
        {
            string D_FD = TextBox_Datetime_FD.Text;
            if (D_FD[2] == '.' && D_FD[5] == '.' && D_FD[10] == ' ' && D_FD[13] == ':')
            {
                ////Адрес по умолчанию
                //string query = "SELECT adr_file FROM table_adr_file";
                //using (SQLiteConnection sqliteConnection = new SQLiteConnection(connectionString))
                //{// Открытие соединения
                //    sqliteConnection.Open();
                //    using (SQLiteCommand sqliteCommand = new SQLiteCommand(query, sqliteConnection))
                //    {
                //        try
                //        {
                //            adr_file = (string)sqliteCommand.ExecuteScalar();
                //        }
                //        catch (Exception ex)
                //        {
                //            MaterialMessageBox.Show("Ошибка: " + ex.Message);
                //        }
                //    }
                //}

                string ZN_KKT = TextBox_ZN_KKT.Text;
                string M_KKT = TextBox_Model_KKT.Text;
                string N_FN = TextBox_ZN_FN.Text;
                string M_FN = ComboBox_Model_FN1.Text.Replace(" ", "");
                string NameOrganization = TextBox_Name_organization.Text;

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
                string Director_org = TextBox_Director_org.Text.ToUpper(); //Конвертация в заглавные буквы ФИО директора
                string INN_Organization = TextBox_INN_organization.Text;
                string Place_ras = TextBox_PlaceSale.Text;
                string OFD = ComboBox_Name_OFD1.Text;
                string INN_OFD = TextBox_INN_OFD1.Text;
                string KPP_Organization = TextBox_KPP_organization.Text;

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

                if (CheckBox_Avtonom.Checked == true) { PrAvtonomS = "1"; } // сведения регистрации ККТ
                if (CheckBox_Azart_play.Checked == true) { PrAzartS = "1"; }
                if (CheckBox_Mark.Checked == true) { PrMarkS = "1"; }
                if (CheckBox_Bank_agent.Checked == true) { PrBankPlatS = "1"; }
                if (CheckBox_Plat_agent.Checked == true) { PrPlatAgentS = "1"; }
                if (CheckBox_Lotereya.Checked == true) { PrLotereyaS = "1"; }
                if (CheckBox_Internet.Checked == true) { PrInternetS = "1"; }
                if (CheckBox_Delivery.Checked == true) { PrRazvozS = "1"; }
                if (CheckBox_Podakziz.Checked == true) { PrAkxizTovarS = "1"; }
                if (CheckBox_Printer_v_avtomate.Checked == true) { PrAvtomatUstrS = "1"; }

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


                if (fio.Length == 2 || fio.Length == 3)
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
                    XmlText NaimOrgT = xmlDocument.CreateTextNode(NameOrganization.Replace("\"", "&quot;"));


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
                    
                    string adr_file_save = null;
                    string[] zap_znak = { "\"", "\\", "/", ":", "*", "?", "<", ">", "|", "\"" };
                    string NameOrganization_save = NameOrganization;
                    if (NameOrganization != "")
                    {
                        for (int i = 0; i < zap_znak.Length; i++)
                        {
                            NameOrganization_save = NameOrganization_save.Replace(zap_znak[i], "");
                        }
                    }

                    FolderBrowserDialog Browserdialog = new FolderBrowserDialog(); //открытие проводника и выбор папки сохраннения
                    Browserdialog.RootFolder = Environment.SpecialFolder.Desktop;
                    Browserdialog.SelectedPath = adr_file;
                    if (Browserdialog.ShowDialog() == DialogResult.OK)
                    {
                        adr_file_save = Browserdialog.SelectedPath;
                    }
                    else { return; }
                    Directory.CreateDirectory(adr_file_save + "\\" + ID_file);
                    xmlDocument.Save(adr_file_save + "\\" + ID_file + "\\" + ID_file + ".xml"); //сохранение файла xml
                    
                    ZipFile.CreateFromDirectory(adr_file_save + "\\" + ID_file, adr_file_save + "\\" + NameOrganization_save + ".zip"); //сохранение zip (что упаковываем, куда)
                    if (delete_xml == "true")
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
                "Неверно введены ФИО руководителя. Программа принимает фамилию, имя или полное ФИО",
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
            string DanaT_FD = TextBox_Datetime_FD.Text;
            if (DanaT_FD[2] == '.' && DanaT_FD[5] == '.' && DanaT_FD[10] == ' ' && DanaT_FD[13] == ':')
            {
                string Model_KKT = TextBox_Model_KKT.Text;
                string ID_Сlient = TextBox_ID_client.Text;
                string RNM = TextBox_RNM1.Text;
                string ZN_KKT = TextBox_ZN_KKT.Text;
                string N_FN = TextBox_ZN_FN.Text;

                string NameOrganization = TextBox_Name_organization.Text;
                string INN_Organization = TextBox_INN_organization.Text;
                string OFD = ComboBox_Name_OFD1.Text;

                string D_FD = DanaT_FD.Substring(0,10);
                string T_FD = DanaT_FD.Substring(Math.Max(0, DanaT_FD.Length - 5));
                string N_FD = TextBox_Number_FD.Text;
                string FP = TextBox_FP_FD.Text;
                string INN_OFD = TextBox_INN_OFD1.Text;

                //// запрос Фамилии ИО оператора
                //string query = "SELECT name_operator FROM table_name_operator";
                //using (SQLiteConnection sqliteConnection = new SQLiteConnection(connectionString))
                //{// Открытие соединения
                //    sqliteConnection.Open();
                //    using (SQLiteCommand sqliteCommand = new SQLiteCommand(query, sqliteConnection))
                //    {
                //        try
                //        {
                //            Name_operator = (string)sqliteCommand.ExecuteScalar();
                //        }
                //        catch (Exception ex)
                //        {
                //            MaterialMessageBox.Show("Ошибка: " + ex.Message);
                //        }
                //    }
                //}


                var newakt = new NewAKT("Akt.docx");

                var items = new Dictionary<string, string>
            {
                {"<Model_KKT>", Model_KKT },
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
                {"<Name_operator>", name_operator },
                {"<ID_Client>", ID_Сlient },
            };

                newakt.Process(items, adr_file, print_akt);
                
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
            string RNM = TextBox_RNM1.Text;
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
            "Значение РНМ введено некорректно. РНМ должен состоять из 16 цифр без пробелов.",
            "Внимательнее");
            }
        }
        
        private bool closingProcessed = false; // Флаг для отслеживания состояния закрытия формы
        private void Form1_Closing(object sender, FormClosingEventArgs e) //Сохранение при закрытии формы
        {
            if (closingProcessed)
            {
                // Если уже обработали событие закрытия, выходим
                return;
            }
            int local_close = 1;
            for (int i = 0; i < Save_parametrs.Length; i++) // Массив проверки сохранения
            {
                if (Save_parametrs[i] == false)
                {
                    local_close *= 0;
                }
            }
            if ((local_close == 0) && (materialTabControl1.SelectedIndex == 0))
            {
                DialogResult result_close = MaterialMessageBox.Show("У вас есть несохраненные данные. Сохранить?", "Да", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (result_close == DialogResult.Yes)
                {
                    ////Адрес по умолчанию
                    //string query = "SELECT adr_file FROM table_adr_file";
                    //using (SQLiteConnection sqliteConnection = new SQLiteConnection(connectionString))
                    //{// Открытие соединения
                    //    sqliteConnection.Open();
                    //    using (SQLiteCommand sqliteCommand = new SQLiteCommand(query, sqliteConnection))
                    //    {
                    //        try
                    //        {
                    //            adr_file = (string)sqliteCommand.ExecuteScalar();
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            MaterialMessageBox.Show("Ошибка: " + ex.Message);
                    //        }
                    //    }
                    //}
                    string adr_file_save = adr_file;
                    FolderBrowserDialog Browserdialog = new FolderBrowserDialog(); //открытие проводника и выбор папки сохраннения
                    Browserdialog.RootFolder = Environment.SpecialFolder.Desktop;
                    Browserdialog.SelectedPath = adr_file;

                    if (Browserdialog.ShowDialog() == DialogResult.OK)
                    {
                        adr_file_save = Browserdialog.SelectedPath;
                        SaveData(adr_file_save);
                        closingProcessed = true;
                        Application.Exit();
                    }
                    else { e.Cancel = true; return;  }
                    
                }
                else if (result_close == DialogResult.No)
                {
                    closingProcessed = true;
                    Application.Exit();
                }
                else if (result_close == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }        

        //____Страница_2________________________________________________
        private void OFD2_TextChanged(object sender, EventArgs e) // Подстановка ИНН ОФД на второй странице
        {
            string OFD = ComboBox_Name_OFD2.Text;
            string query = @"
            SELECT 
                inn_OFD            
            FROM options_OFD 
            WHERE name_OFD = @name_OFD";
            using (SQLiteConnection sqliteConnection = new SQLiteConnection(connectionString))
            {
                // Открытие соединения
                sqliteConnection.Open();
                using (SQLiteCommand sqliteCommand = new SQLiteCommand(query, sqliteConnection))
                {
                    sqliteCommand.Parameters.AddWithValue("@name_OFD", OFD);

                    try
                    {
                        using (SQLiteDataReader reader = sqliteCommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                TextBox_INN_OFD2.Text = reader["inn_OFD"].ToString();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MaterialMessageBox.Show("Ошибка: " + ex.Message);
                    }
                }
            }

        }
        private void switchclick_openKKT2(object sender, EventArgs e) // Подключение к ККТ
        {
            if (switch_open_KKT2.Checked == true)
            {
                try
                {
                    CashRegister = new TerminalFA("COM3");
                    ErrorCode answer = CashRegister.Initialize();
                    switch_open_KKT1.Checked = true;
                }
                catch
                {
                    MaterialMessageBox.Show("Не удалось подключиться к COM3");
                    switch_open_KKT1.Checked = false;
                    switch_open_KKT2.Checked = false;
                }
            }
            else
            {
                try
                {
                    CashRegister = new TerminalFA("COM3");
                    ErrorCode answer = CashRegister.Initialize();
                    MaterialMessageBox.Show("Соединение отключено");
                }
                catch
                {
                    MaterialMessageBox.Show("Не удалось отключиться от COM3");
                }
            }
        }
        
        private void buttonAkt2_Click(object sender, EventArgs e) // Кнопка Акт ввода на второй странице
        {
            string DateT_FD = TextBox_Date2.Text;
            if (DateT_FD[2] == '.' && DateT_FD[5] == '.' && DateT_FD[10] == ' ' && DateT_FD[13] == ':')
            {

                string ID_Сlient = TextBox_ID_client2.Text;
                string RNM = TextBox_RNM2.Text;
                string Model_KKT = TextBox_Model_KKT2.Text;
                string ZN_KKT = TextBox_ZN_KKT2.Text;
                string ZN_FN = TextBox_ZN_FN2.Text;

                string NameOrganization = TextBox_NameOrganization2.Text;
                string INN_Organization = TextBox_INNOrganization2.Text;
                string OFD = ComboBox_Name_OFD2.Text;

                string D_FD = DateT_FD.Substring(0, 10);
                string T_FD = DateT_FD.Substring(Math.Max(0, DateT_FD.Length - 5));
                string Number_FD = TextBox_NumberFD2.Text;
                string FP = TextBox_FPDocument2.Text;
                string INN_OFD = TextBox_INN_OFD2.Text;

                ////запрос Фамилии ИО оператора
                //string query = "SELECT name_operator FROM table_name_operator";
                //using (SQLiteConnection sqliteConnection = new SQLiteConnection(connectionString))
                //{// Открытие соединения
                //    sqliteConnection.Open();
                //    using (SQLiteCommand sqliteCommand = new SQLiteCommand(query, sqliteConnection))
                //    {
                //        try
                //        {
                //            Name_operator = (string)sqliteCommand.ExecuteScalar();
                //        }
                //        catch (Exception ex)
                //        {
                //            MaterialMessageBox.Show("Ошибка: " + ex.Message);
                //        }
                //    }
                //}

                var newakt = new NewAKT("Akt.docx");

                var items = new Dictionary<string, string>
            {
                {"<Model_KKT>", Model_KKT},
                {"<ZN_KKT>", ZN_KKT },
                {"<RNM>", RNM },
                {"<N_FN>", ZN_FN },
                {"<D_FD>", D_FD },
                {"<T_FD>", T_FD },
                {"<N_FD>", Number_FD },
                {"<FP>", FP },
                {"<INN_Organization>", INN_Organization },
                {"<INN_OFD>", INN_OFD },
                {"<NameOrganization>", NameOrganization },
                {"<Name_operator>", name_operator },
                {"<ID_Client>", ID_Сlient },
            };

                newakt.Process(items, adr_file, print_akt);
            }


            else
            {
                MaterialMessageBox.Show(
            "Ошибка в вводе даты и времени. Введите по формату (дд.мм.гггг чч:мм)",
            "Внимательнее");
            }
        
        }
        private void butSaveAKT_Click(object sender, EventArgs e) // Кнопка Сохранить на второй странцие
        {
            string D_FD = TextBox_Date2.Text;
            if (D_FD[2] != '.' || D_FD[5] != '.' || D_FD[10] != ' ' || D_FD[13] != ':')
            {
                MaterialMessageBox.Show(
            "Проверьте формат ввода даты и времени (дд.мм.гггг чч:мм)",
            "Внимательнее");


            }
            else
            {
                string ID_Сlient = TextBox_ID_client2.Text;
                string RNM = TextBox_RNM2.Text;
                string ZN_KKT = TextBox_ZN_KKT2.Text;
                string N_av = " ";
                string N_FN = TextBox_ZN_FN2.Text;
                string M_FN = " ";
                string NameOrganization = TextBox_NameOrganization2.Text;
                string Director_org = " ";
                string INN_Organization = TextBox_INNOrganization2.Text;
                string KPP_Organization = " ";
                string Telephone = " ";
                string Email = " ";
                string Address_ras = " ";
                string Place_ras = " ";
                string OFD = ComboBox_Name_OFD2.Text;
                string INN_OFD = TextBox_INN_OFD2.Text;
                string T_FD = D_FD.Substring(Math.Max(0, D_FD.Length - 5)); // нахождение даты ФД
                string N_FD = TextBox_NumberFD2.Text;
                string FP = TextBox_FPDocument2.Text;
                string Model_KKT = TextBox_Model_KKT2.Text;
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
                
                FolderBrowserDialog Browserdialog = new FolderBrowserDialog(); //открытие проводника и выбор папки сохраннения
                Browserdialog.RootFolder = Environment.SpecialFolder.Desktop;
                Browserdialog.SelectedPath = adr_file;

                if (Browserdialog.ShowDialog() == DialogResult.OK)
                {
                    adr_file = Browserdialog.SelectedPath;
                    SaveData(adr_file);
                }
                else
                {
                    return;
                }

                Save s = new Save();
                s.setValues(D_FD, ID_Сlient, RNM, ZN_KKT, N_av, N_FN, M_FN, NameOrganization,
            Director_org, INN_Organization, KPP_Organization, SNO_OSN, SNO_USN_D, SNO_USN_D_R, SNO_PATENT, SNO_ESHN,
            Telephone, Email, Address_ras, Place_ras,
            OFD, INN_OFD, T_FD, N_FD, FP, Model_KKT, Adr_Internet, PrAvtonom, PrLotereya, PrAzart, PrBankPlat,
            PrPlatAgent, PrAvtomatUstr, PrInternet, PrRazvoz, PrAkxizTovar, PrMark, adr_file);

                for (int i = 0; i < Save_parametrs.Length; i++) // Массив проверки сохранения
                {
                    Save_parametrs[i] = true;
                }
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
            string[] str_mas = str.Split('#');
            if (str_mas.Length > 47)
            {
                TextBox_ZN_KKT2.Text = str_mas[1].Trim();
                TextBox_Model_KKT2.Text = str_mas[3].Trim();
                TextBox_ZN_FN2.Text = str_mas[7].Trim();
                TextBox_ID_client2.Text = str_mas[11].Trim();
                TextBox_NameOrganization2.Text = str_mas[13].Trim();
                TextBox_INNOrganization2.Text = str_mas[17].Trim();
                ComboBox_Name_OFD2.Text = str_mas[33].Trim();
                TextBox_INN_OFD2.Text = str_mas[35].Trim();
                TextBox_RNM2.Text = str_mas[39].Trim();
                TextBox_Date2.Text = str_mas[41].Trim() + str_mas[43].Trim(); //объединение даты и времени
                TextBox_NumberFD2.Text = str_mas[45].Trim();
                TextBox_FPDocument2.Text = str_mas[47].Trim();
            }
        }
        private void butReaddata2_Click(object sender, EventArgs e) // Кнопка Считать на второй странице
        {
            bool result_open = false;
            if (switch_open_KKT2.Checked == false)
            {
                result_open = open_KKT(!switch_open_KKT2.Checked);
            }
            if (result_open == true)
            {
                try
                {
                    TextBox_ZN_KKT2.Text = CashRegister.GetZN(); // запрос ЗН ККТ
                    try { TextBox_ZN_FN2.Text = CashRegister.GetFN(); } // запрос ЗН ФН 
                    catch { MaterialMessageBox.Show("Нет данных об ФН"); }
                }
                catch { MaterialMessageBox.Show("Не удалось считать данные с ККТ"); }
            }
        }
        private void butCleare2_Click(object sender, EventArgs e) // Кнопка Очистить поля на второй странице
        {
            DialogResult result = MaterialMessageBox.Show("Уверены что хотите очистить поля?", "Да", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                TextBox_ZN_KKT2.Text = null;
                TextBox_Model_KKT2.Text = null;
                TextBox_ZN_FN2.Text = null;
                TextBox_ID_client2.Text = null;
                TextBox_NameOrganization2.Text = null;
                TextBox_INNOrganization2.Text = null;
                TextBox_RNM2.Text = null;
                TextBox_Date2.Text = null; 
                TextBox_NumberFD2.Text = null;
                TextBox_FPDocument2.Text = null;
                ComboBox_Name_OFD2.Text = standart_OFD;
                string query = @"
                SELECT 
                    inn_OFD 
                    FROM options_OFD 
                WHERE name_OFD = @name_OFD";
                using (SQLiteConnection sqliteConnection = new SQLiteConnection(connectionString))
                {
                    // Открытие соединения
                    sqliteConnection.Open();
                    using (SQLiteCommand sqliteCommand = new SQLiteCommand(query, sqliteConnection))
                    {
                        sqliteCommand.Parameters.AddWithValue("@name_OFD", ComboBox_Name_OFD1.Text);
                        try
                        {
                            using (SQLiteDataReader reader = sqliteCommand.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    // Запись значений в TextBox'ы
                                    TextBox_INN_OFD2.Text = reader["inn_OFD"].ToString();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            // Обработка возможных ошибок
                            MaterialMessageBox.Show("Ошибка: " + ex.Message);
                        }
                    }
                }


            }
        }



        //____Страница_3________________________________________________
        private void butSave_OFD_Click(object sender, EventArgs e) // Сохранение парметров ОФД
        {
            if (CheckButton_AddNewOFD.Checked == false)
            {
                DialogResult result = MaterialMessageBox.Show("Уверены что хотите сохранить данные ОФД? Отменить действие будет невозможно", "Подтверждение",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        //Получение выбранного значения из ComboBox
                        string selectedOFDName = ComboBox_Name_OFD3.SelectedItem.ToString();

                        // SQL-запрос для обновления данных
                        string query = @"
                    UPDATE options_OFD 
                    SET 
                        inn_OFD = @inn_OFD, 
                        email_OFD = @email_OFD, 
                        adress_OFD = @adress_OFD, 
                        IP_OFD = @IP_OFD, 
                        TCP_OFD = @TCP_OFD, 
                        DNS_OFD = @DNS_OFD, 
                        port_OFD = @port_OFD,
                        adress_OISM_OFD = @adress_OISM_OFD
                    WHERE name_OFD = @name_OFD";
                        using (SQLiteConnection sqliteConnection = new SQLiteConnection(connectionString))
                        {// Открытие соединения
                            sqliteConnection.Open();
                            // Создание команды SQL
                            using (SQLiteCommand sqliteCommand = new SQLiteCommand(query, sqliteConnection))
                            {
                                // Добавление параметров к запросу
                                sqliteCommand.Parameters.AddWithValue("@inn_OFD", TextBox_INN_OFD3.Text);
                                sqliteCommand.Parameters.AddWithValue("@email_OFD", TextBox_Email_OFD3.Text);
                                sqliteCommand.Parameters.AddWithValue("@adress_OFD", TextBox_adress_OFD3.Text);
                                sqliteCommand.Parameters.AddWithValue("@IP_OFD", TextBox_IP_OFD3.Text);
                                sqliteCommand.Parameters.AddWithValue("@TCP_OFD", TextBox_TCP_OFD3.Text);
                                sqliteCommand.Parameters.AddWithValue("@DNS_OFD", TextBox_DNS_OFD3.Text);
                                sqliteCommand.Parameters.AddWithValue("@port_OFD", TextBox_port_OFD3.Text);
                                sqliteCommand.Parameters.AddWithValue("@adress_OISM_OFD", TextBox_adress2_OFD3.Text);
                                sqliteCommand.Parameters.AddWithValue("@name_OFD", selectedOFDName);


                                // Выполнение запроса
                                int rowsAffected = sqliteCommand.ExecuteNonQuery();

                                //Проверка, были ли обновлены строки
                                if (rowsAffected > 0)
                                {
                                    MaterialMessageBox.Show(
                    "Данные сохранены",
                    "ОК");
                                }
                                else
                                {
                                    MaterialMessageBox.Show("Не удалось обновить данные. Проверьте выбранное имя ОФД.");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Обработка возможных ошибок
                        MaterialMessageBox.Show("Ошибка: " + ex.Message);
                    }

                }
            }

            // добавление новой записи о ОФД
            else
            {
                DialogResult result2 = MaterialMessageBox.Show("Уверены что хотите добавить ОФД? Отменить действие будет невозможно", "Подтверждение",
                                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result2 == DialogResult.Yes)
                {
                    // Получение нового имени ОФД из TextBox
                    string newNameOFD = TextBox_NewName_OFD3.Text;


                    // SQL-запрос для вставки новой записи
                    string query = @"
        INSERT INTO options_OFD (name_OFD, inn_OFD, email_OFD, adress_OFD, IP_OFD, TCP_OFD, DNS_OFD, port_OFD, adress_OISM_OFD) 
        VALUES (@name_OFD, @inn_OFD, @email_OFD, @adress_OFD, @IP_OFD, @TCP_OFD, @DNS_OFD, @port_OFD, @adress_OISM_OFD)";

                    using (SQLiteConnection sqliteConnection = new SQLiteConnection(connectionString))
                    {
                        // Открытие соединения
                        sqliteConnection.Open();

                        using (SQLiteCommand sqliteCommand = new SQLiteCommand(query, sqliteConnection))
                        {
                            // Добавление параметров к запросу
                            sqliteCommand.Parameters.AddWithValue("@name_OFD", newNameOFD);
                            sqliteCommand.Parameters.AddWithValue("@inn_OFD", TextBox_INN_OFD3.Text);
                            sqliteCommand.Parameters.AddWithValue("@email_OFD", TextBox_Email_OFD3.Text);
                            sqliteCommand.Parameters.AddWithValue("@adress_OFD", TextBox_adress_OFD3.Text);
                            sqliteCommand.Parameters.AddWithValue("@IP_OFD", TextBox_IP_OFD3.Text);
                            sqliteCommand.Parameters.AddWithValue("@TCP_OFD", TextBox_TCP_OFD3.Text);
                            sqliteCommand.Parameters.AddWithValue("@DNS_OFD", TextBox_DNS_OFD3.Text);
                            sqliteCommand.Parameters.AddWithValue("@port_OFD", TextBox_port_OFD3.Text);
                            sqliteCommand.Parameters.AddWithValue("@adress_OISM_OFD", TextBox_adress2_OFD3.Text);

                            try
                            {
                                // Выполнение запроса
                                int rowsAffected = sqliteCommand.ExecuteNonQuery();

                                // Проверка, была ли добавлена запись
                                if (rowsAffected > 0)
                                {
                                    MaterialMessageBox.Show("Запись о новом ОФД добавлена.");
                                }
                                else
                                {
                                    MaterialMessageBox.Show("Не удалось добавить новую запись ОФД.");
                                }
                            }
                            catch (Exception ex)
                            {
                                // Обработка возможных ошибок
                                MaterialMessageBox.Show("Ошибка: " + ex.Message);
                            }
                        }
                    }
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
                    //Получение выбранного значения из ComboBox
                    string selectedFNName = ComboBox_Name_FN3.SelectedItem.ToString();

                    // SQL-запрос для обновления данных
                    string query = @"
                    UPDATE options_FN 
                    SET 
                        
                        adress_FN = @adress_FN,
                        port_FN = @port_FN
                    WHERE name_FN = @name_FN";
                    using (SQLiteConnection sqliteConnection = new SQLiteConnection(connectionString))
                    {// Открытие соединения
                        sqliteConnection.Open();
                        // Создание команды SQL
                        using (SQLiteCommand sqliteCommand = new SQLiteCommand(query, sqliteConnection))
                        {
                            // Добавление параметров к запросу
                            sqliteCommand.Parameters.AddWithValue("@adress_FN", TextBox_adress_FN3.Text);
                            sqliteCommand.Parameters.AddWithValue("@adress_FN", TextBox_port_FN3.Text);
                            sqliteCommand.Parameters.AddWithValue("@name_FN", selectedFNName);


                            // Выполнение запроса
                            int rowsAffected = sqliteCommand.ExecuteNonQuery();

                            //Проверка, были ли обновлены строки
                            if (rowsAffected > 0)
                            {
                                MaterialMessageBox.Show(
                "Данные сохранены",
                "ОК");
                            }
                            else
                            {
                                MaterialMessageBox.Show("Не удалось обновить данные. Проверьте выбранное имя ФН.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Обработка возможных ошибок
                    MaterialMessageBox.Show("Ошибка: " + ex.Message);
                }
            }
        }
        private void CheckButton_AddNewOFD_Checked(object sender, EventArgs e) // Открытие поля Наименование нового ОФД
        {
            if (CheckButton_AddNewOFD.Checked == true) 
            { 
                TextBox_NewName_OFD3.Visible = true;
                ComboBox_Name_OFD3.Visible = false;
            }
            else { TextBox_NewName_OFD3.Visible = false; ComboBox_Name_OFD3.Visible = true; }
        }
        private void Name_OFD_Changed(object sender, EventArgs e) // Подстановка параметров ОФД в соответствии с ComboBox
        {
            string name_OFD = ComboBox_Name_OFD3.Text;
            string query = @"
            SELECT 
                inn_OFD, 
                email_OFD, 
                adress_OFD, 
                IP_OFD, 
                TCP_OFD, 
                DNS_OFD, 
                adress_OISM_OFD,
                port_OFD
            FROM options_OFD 
            WHERE name_OFD = @name_OFD";
            using (SQLiteConnection sqliteConnection = new SQLiteConnection(connectionString))
            {// Открытие соединения
                sqliteConnection.Open();
                using (SQLiteCommand sqliteCommand = new SQLiteCommand(query, sqliteConnection))
                {
                    // Добавление параметра к запросу для предотвращения SQL-инъекций
                    sqliteCommand.Parameters.AddWithValue("@name_OFD", name_OFD);

                    try
                    {

                        // Выполнение запроса и получение результата
                        using (SQLiteDataReader reader = sqliteCommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Запись значений в TextBox'ы
                                TextBox_INN_OFD3.Text = reader["inn_OFD"].ToString();
                                TextBox_Email_OFD3.Text = reader["email_OFD"].ToString();
                                TextBox_adress_OFD3.Text = reader["adress_OFD"].ToString();
                                TextBox_IP_OFD3.Text = reader["IP_OFD"].ToString();
                                TextBox_TCP_OFD3.Text = reader["TCP_OFD"].ToString();
                                TextBox_DNS_OFD3.Text = reader["DNS_OFD"].ToString();
                                TextBox_adress2_OFD3.Text = reader["adress_OISM_OFD"].ToString();
                                TextBox_port_OFD3.Text = reader["port_OFD"].ToString();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Обработка возможных ошибок
                        MessageBox.Show("Ошибка: " + ex.Message);
                    }
                }
            }

        }
        private void Name_FN_Changed(object sender, EventArgs e) // Подстановка параметров FN в соответствии с ComboBox
        {
            string name_FN = ComboBox_Name_FN3.Text;
            string query = @"
            SELECT 
                adress_FN, 
                port_FN 
            FROM options_FN 
            WHERE name_FN = @name_FN";
            using (SQLiteConnection sqliteConnection = new SQLiteConnection(connectionString))
            {// Открытие соединения
                sqliteConnection.Open();
                using (SQLiteCommand sqliteCommand = new SQLiteCommand(query, sqliteConnection))
                {
                    // Добавление параметра к запросу для предотвращения SQL-инъекций
                    sqliteCommand.Parameters.AddWithValue("@name_FN", name_FN);

                    try
                    {
                        using (SQLiteDataReader reader = sqliteCommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                TextBox_adress_FN3.Text = reader["adress_FN"].ToString();
                                TextBox_port_FN3.Text = reader["port_FN"].ToString();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Обработка возможных ошибок
                        MessageBox.Show("Ошибка: " + ex.Message);
                    }

                }
            }
        }
        


        //____Страница_4________________________________________________
        private void TabControl_Selected(object sender, TabControlEventArgs e) // событие автозаполнения textBox настроек при активации вкладки
        {
            // Адрес по умолчанию
            ////Адрес по умолчанию
            //string query = "SELECT adr_file FROM table_adr_file";
            using (SQLiteConnection sqliteConnection = new SQLiteConnection(connectionString))
            {// Открытие соединения
                sqliteConnection.Open();

                TextBox_Adr_file.Text = adr_file;
                TextBox_name_operator.Text = name_operator;
                if (delete_xml == "true") { Switch_Del_xml.Checked = true; }
                else { Switch_Del_xml.Checked = false; }
                if (print_akt == "true") { Switch_Print_Akt.Checked = true; }
                else { Switch_Print_Akt.Checked = false; }
            }
        }
        private void materialButton1_Click(object sender, EventArgs e) // Кнопка сохранение
        {
            DialogResult result = MaterialMessageBox.Show("Уверены что хотите сохранить данные? Отменить действие будет невозможно", "Подтверждение",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    if (Switch_Del_xml.Checked == true) { delete_xml = "true"; }
                    else { delete_xml = "false"; }
                    if (Switch_Print_Akt.Checked == true) { print_akt = "true"; }
                    else { print_akt = "false"; }

                    string adr_file = TextBox_Adr_file.Text;
                    if (adr_file.Substring(adr_file.Length - 1, 1) == "\\") { adr_file = adr_file.Remove(adr_file.Length - 1); }

                    string query = @"
            UPDATE options_program
            SET 
                meaning = @adr_file
                WHERE parameter = 'adr_file'";
                    using (SQLiteConnection sqliteConnection = new SQLiteConnection(connectionString))
                    {// Открытие соединения
                        sqliteConnection.Open();
                        using (SQLiteCommand sqliteCommand = new SQLiteCommand(query, sqliteConnection))
                        {
                            sqliteCommand.Parameters.AddWithValue("@adr_file", adr_file);
                            try
                            {
                                int rowsAffected = sqliteCommand.ExecuteNonQuery();
                                if (rowsAffected <= 0)
                                {
                                    MaterialMessageBox.Show("Не удалось обновить данные адреса по умочанию");
                                }
                            }
                            catch (Exception ex)
                            {
                                MaterialMessageBox.Show("Ошибка: " + ex.Message);
                            }

                            // Сохранение Фамилии ИО опреатора -------------------------------------------------------------------------

                            query = @"
            UPDATE options_program
            SET 
                meaning = @name_operator
                WHERE parameter = 'name_operator'";
                            using (SQLiteCommand sqliteCommand2 = new SQLiteCommand(query, sqliteConnection))
                            {
                                sqliteCommand2.Parameters.AddWithValue("@name_operator", TextBox_name_operator.Text);
                                try
                                {
                                    int rowsAffected = sqliteCommand2.ExecuteNonQuery();
                                    if (rowsAffected <= 0)
                                    {
                                        MaterialMessageBox.Show("Не удалось обновить данные оператора");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MaterialMessageBox.Show("Ошибка: " + ex.Message);
                                }
                            }

                            // Удаление xml  -------------------------------------------------------------------------
                            string del_xml = "";
                            if (delete_xml == "true")
                            {
                                del_xml = "true";
                            }
                            else
                            {
                                del_xml = "false";
                            }

                            query = @"
            UPDATE options_program
            SET 
                meaning = @del_xml
                WHERE parameter = 'del_xml'";
                            // Создание команды SQL
                            using (SQLiteCommand sqliteCommand3 = new SQLiteCommand(query, sqliteConnection))
                            {
                                // Добавление параметров к запрос
                                sqliteCommand3.Parameters.AddWithValue("@del_xml", del_xml);
                                try
                                {
                                    int rowsAffected = sqliteCommand3.ExecuteNonQuery();
                                    // Проверка, были ли обновлены строки
                                    if (rowsAffected <= 0)
                                    {
                                        MaterialMessageBox.Show("Не удалось обновить данные удаление папки с файлом XML.");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MaterialMessageBox.Show("Ошибка: " + ex.Message);
                                }
                            }

                            // Печать акта ввода по умолчанию  -------------------------------------------------------------------------
                            string pr_akt = "";
                            if (print_akt == "true")
                            {
                                pr_akt = "true";
                            }
                            else
                            {
                                pr_akt = "false";
                            }

                            query = @"
            UPDATE options_program
            SET 
                meaning = @print_akt
                WHERE parameter = 'print_akt'";
                            // Создание команды SQL
                            using (SQLiteCommand sqliteCommand4 = new SQLiteCommand(query, sqliteConnection))
                            {
                                // Добавление параметров к запрос
                                sqliteCommand4.Parameters.AddWithValue("@print_akt", pr_akt);
                                try
                                {
                                    int rowsAffected = sqliteCommand4.ExecuteNonQuery();
                                    // Проверка, были ли обновлены строки
                                    if (rowsAffected <= 0)
                                    {
                                        MaterialMessageBox.Show("Не удалось обновить статус печати акта ввода по умолчанию.");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MaterialMessageBox.Show("Ошибка: " + ex.Message);
                                }
                            }

                            // Сохранение ОФД по умолчанию -------------------------------------------------------------------------

                            query = @"
            UPDATE options_program
            SET 
                meaning = @standart_OFD
                WHERE parameter = 'standart_OFD'";
                            using (SQLiteCommand sqliteCommand5 = new SQLiteCommand(query, sqliteConnection))
                            {
                                sqliteCommand5.Parameters.AddWithValue("@standart_OFD", ComboBox_Name_OFD4.Text);
                                try
                                {
                                    int rowsAffected = sqliteCommand5.ExecuteNonQuery();
                                    if (rowsAffected <= 0)
                                    {
                                        MaterialMessageBox.Show("Не удалось обновить ОФД по умолчанию.");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MaterialMessageBox.Show("Ошибка: " + ex.Message);
                                }
                            }

                            // Сохранение ФН по умолчанию -------------------------------------------------------------------------

                            query = @"
            UPDATE options_program
            SET 
                meaning = @standart_FN
                WHERE parameter = 'standart_FN'";
                            using (SQLiteCommand sqliteCommand6 = new SQLiteCommand(query, sqliteConnection))
                            {
                                sqliteCommand6.Parameters.AddWithValue("@standart_FN", ComboBox_Model_FN4.Text);
                                try
                                {
                                    int rowsAffected = sqliteCommand6.ExecuteNonQuery();
                                    if (rowsAffected <= 0)
                                    {
                                        MaterialMessageBox.Show("Не удалось обновить ФН по умолчанию");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MaterialMessageBox.Show("Ошибка: " + ex.Message);
                                }
                            }
                        }
                    }
                MaterialMessageBox.Show("Данные сохранены. Все настройки применятся после перезагрузки программы");
                }
                catch (Exception ex)
                {
                    MaterialMessageBox.Show(
            "Ошибка сохранения " + ex,
            "Ошибка");
                }
            }
        }
        private void materialButton2_Click(object sender, EventArgs e) //открытие проводника
        {
            FolderBrowserDialog Browserdialog = new FolderBrowserDialog();
            if (Browserdialog.ShowDialog() == DialogResult.OK)
            {
                TextBox_Adr_file.Text = Browserdialog.SelectedPath;
            }
        }

        private void SaveData(string _adr_file_save)
        {
            string adr_file_save = _adr_file_save;
            string ID_Сlient = TextBox_ID_client.Text;
            string RNM = TextBox_RNM1.Text;
            string ZN_KKT = TextBox_ZN_KKT.Text;
            string N_av = TextBox_Number_automatic.Text;
            string N_FN = TextBox_ZN_FN.Text;
            string M_FN = ComboBox_Model_FN1.Text;
            string NameOrganization = TextBox_Name_organization.Text;
            string Director_org = TextBox_Director_org.Text;
            string INN_Organization = TextBox_INN_organization.Text;
            string KPP_Organization = TextBox_KPP_organization.Text;
            string Telephone = TextBox_Telephon_number.Text;
            string Email = TextBox_Email_organization.Text;
            string Address_ras = TextBox_adressSale.Text;
            string Place_ras = TextBox_PlaceSale.Text;
            string OFD = ComboBox_Name_OFD1.Text;
            string INN_OFD = TextBox_INN_OFD1.Text;
            string D_FD = TextBox_Datetime_FD.Text.Substring(0, 10); // нахождение даты ФД
            string T_FD = TextBox_Datetime_FD.Text.Substring(11);
            string N_FD = TextBox_Number_FD.Text;
            string FP = TextBox_FP_FD.Text;
            string Model_KKT = TextBox_Model_KKT.Text;
            string Adr_Internet = TextBox_Domen.Text;

            string SNO_OSN = " ";
            string SNO_USN_D = " ";
            string SNO_USN_D_R = " ";
            string SNO_PATENT = " ";
            string SNO_ESHN = " ";
            if (Checkbox_OSN.Checked == true) { SNO_OSN = "ОСН"; }
            if (Checkbox_USN_Dohod.Checked == true) { SNO_USN_D = "УСН Доход"; }
            if (Checkbox_USN_Dohod_rashod.Checked == true) { SNO_USN_D_R = "УСН Доход - расход"; }
            if (Checkbox_Patent.Checked == true) { SNO_PATENT = "Патент"; }
            if (Checkbox_ESHN.Checked == true) { SNO_ESHN = "ЕСХН"; }


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
            if (CheckBox_Avtonom.Checked == true) { PrAvtonom = "1"; } // сведения регистрации ККТ
            if (CheckBox_Azart_play.Checked == true) { PrAzart = "1"; }
            if (CheckBox_Mark.Checked == true) { PrMark = "1"; }
            if (CheckBox_Bank_agent.Checked == true) { PrBankPlat = "1"; }
            if (CheckBox_Plat_agent.Checked == true) { PrPlatAgent = "1"; }
            if (CheckBox_Lotereya.Checked == true) { PrLotereya = "1"; }
            if (CheckBox_Internet.Checked == true) { PrInternet = "1"; }
            if (CheckBox_Delivery.Checked == true) { PrRazvoz = "1"; }
            if (CheckBox_Podakziz.Checked == true) { PrAkxizTovar = "1"; }
            if (CheckBox_Printer_v_avtomate.Checked == true) { PrAvtomatUstr = "1"; }

            Save s = new Save();
            s.setValues(D_FD, ID_Сlient, RNM, ZN_KKT, N_av, N_FN, M_FN, NameOrganization,
        Director_org, INN_Organization, KPP_Organization, SNO_OSN, SNO_USN_D, SNO_USN_D_R, SNO_PATENT, SNO_ESHN,
        Telephone, Email, Address_ras, Place_ras,
        OFD, INN_OFD, T_FD, N_FD, FP, Model_KKT, Adr_Internet, PrAvtonom, PrLotereya, PrAzart, PrBankPlat,
        PrPlatAgent, PrAvtomatUstr, PrInternet, PrRazvoz, PrAkxizTovar, PrMark, adr_file_save);
        }
        private void Clear_form()
        {
            for (int i = 0; i < Save_parametrs.Length; i++) // Массив проверки сохранения
            {
                Save_parametrs[i] = true;
            }
            label_save_status.Text = "";
            label_image_save_status.Text = "";

            label_vers_FFD.Text = "----";
            label_vers_config.Text = "----";
            label_datatime.Text = "----";

            Checkbox_OSN.Checked = false;
            Checkbox_USN_Dohod.Checked = false;
            Checkbox_USN_Dohod_rashod.Checked = false;
            Checkbox_Patent.Checked = false;
            Checkbox_ESHN.Checked = false;

            TextBox_ID_client.Text = null;
            TextBox_RNM1.Text = null;
            TextBox_ZN_KKT.Text = null;
            TextBox_Number_automatic.Text = null;
            TextBox_ZN_FN.Text = null;
            ComboBox_Model_FN1.Text = standart_model_FN;
            TextBox_Name_organization.Text = null;
            TextBox_Director_org.Text = null;
            TextBox_INN_organization.Text = null;
            TextBox_KPP_organization.Text = null;
            TextBox_Telephon_number.Text = null;
            TextBox_Email_organization.Text = null;
            TextBox_adressSale.Text = "440000, г.Пенза, ул. Суворова, стр 92";
            TextBox_PlaceSale.Text = null;
            ComboBox_Name_OFD1.Text = standart_OFD;
            string query = @"
                SELECT 
                    inn_OFD, 
                    email_OFD
                    FROM options_OFD 
                WHERE name_OFD = @name_OFD";
            using (SQLiteConnection sqliteConnection = new SQLiteConnection(connectionString))
            {
                // Открытие соединения
                sqliteConnection.Open();
                using (SQLiteCommand sqliteCommand = new SQLiteCommand(query, sqliteConnection))
                {
                    sqliteCommand.Parameters.AddWithValue("@name_OFD", ComboBox_Name_OFD1.Text);
                    try
                    {
                        using (SQLiteDataReader reader = sqliteCommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Запись значений в TextBox'ы
                                TextBox_INN_OFD1.Text = reader["inn_OFD"].ToString();
                                TextBox_Email_OFD1.Text = reader["email_OFD"].ToString();

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Обработка возможных ошибок
                        MaterialMessageBox.Show("Ошибка: " + ex.Message);
                    }
                }
            }

            TextBox_Datetime_FD.Text = null;
            TextBox_Number_FD.Text = "1";
            TextBox_FP_FD.Text = null;
            TextBox_Model_KKT.Text = "Терминал-ФА";
            TextBox_Domen.Text = null;




            CheckBox_Avtonom.Checked = false; // сведения регистрации ККТ
            CheckBox_Azart_play.Checked = false;
            CheckBox_Mark.Checked = false;
            CheckBox_Bank_agent.Checked = false;
            CheckBox_Plat_agent.Checked = false;
            CheckBox_Lotereya.Checked = false;
            CheckBox_Internet.Checked = false;
            CheckBox_Delivery.Checked = false;
            CheckBox_Podakziz.Checked = false;
            CheckBox_Printer_v_avtomate.Checked = false;
        }

        private void buttonCopy1_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(TextBox_Model_KKT.Text);
        }

        private void buttonCopy2_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(TextBox_ZN_KKT.Text);
        }

        private void buttonCopy3_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(TextBox_Number_automatic.Text);
        }

        private void buttonCopy4_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(ComboBox_Model_FN1.Text);
        }

        private void buttonCopy5_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(TextBox_ZN_FN.Text);
        }

        private void buttonCopy6_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(TextBox_ID_client.Text);
        }

        private void buttonCopy7_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(TextBox_Name_organization.Text);
        }

        private void buttonCopy8_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(TextBox_Director_org.Text);
        }

        private void buttonCopy9_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(TextBox_INN_organization.Text);
        }

        private void buttonCopy10_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(TextBox_KPP_organization.Text);
        }

        private void buttonCopy11_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(TextBox_Telephon_number.Text);
        }

        private void buttonCopy12_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(TextBox_Email_organization.Text);
        }

        private void buttonCopy13_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(TextBox_adressSale.Text);
        }

        private void buttonCopy14_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(TextBox_PlaceSale.Text);
        }

        private void buttonCopy15_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(ComboBox_Name_OFD1.Text);
        }

        private void buttonCopy16_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(TextBox_INN_OFD1.Text);
        }

        private void buttonCopy17_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(TextBox_Email_OFD1.Text);
        }

        private void buttonCopy18_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(TextBox_RNM1.Text);
        }

        private void buttonCopy19_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(TextBox_Number_FD.Text);
        }

        private void buttonCopy20_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(TextBox_Datetime_FD.Text);
        }

        private void buttonCopy21_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(TextBox_FP_FD.Text);
        }

        private void materialButton2_Click_1(object sender, EventArgs e)
        {
            CashRegister.InputDATATIME();
            DateTime now = DateTime.Now;
            label_datatime.Text = now.ToString("dd.MM.yyyy HH:mm");
        }
    }
} 
