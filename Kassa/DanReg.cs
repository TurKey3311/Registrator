using KitCashProtocol;
using MaterialSkin.Controls;
using Microsoft.Data.SqlClient;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;

namespace Kassa
{
    public partial class DanReg : MaterialForm
    {
        //SqlConnection sqlConnection = null;
        string ofd;
        string fn;
        public DanReg(string ofd, string fn)
        {
            InitializeComponent();

            this.ofd = ofd;
            this.fn = fn;
            TextBox_Name_OFD_Data.Text = ofd;
            // Получаем строку подключения из App.config
            string connectionString = ConfigurationManager.ConnectionStrings["SQLiteDB"].ConnectionString;
            // Создание подключения к базе данных
            using (var connection = new SQLiteConnection(connectionString))
            {


                // Открытие подключения
                connection.Open();
                string selectQuery = @"
            SELECT 
                adress_OFD, 
                IP_OFD, 
                TCP_OFD, 
                DNS_OFD, 
                port_OFD,
                adress_OISM_OFD
            FROM options_OFD 
            WHERE name_OFD = @name_OFD";
                using (SQLiteCommand sqlCommand = new SQLiteCommand(selectQuery, connection))
                {
                    // Добавление параметра к запросу для предотвращения SQL-инъекций
                    sqlCommand.Parameters.AddWithValue("@name_OFD", ofd);

                    try
                    {
                        // Выполнение запроса и получение результата
                        using (SQLiteDataReader reader = sqlCommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Запись значений в TextBox'ы
                                TextBox_adress_OFD_Data.Text = reader["adress_OFD"].ToString();
                                TextBox_IP_OFD_Data.Text = reader["IP_OFD"].ToString();
                                TextBox_TCP_OFD_Data.Text = reader["TCP_OFD"].ToString();
                                TextBox_DNS_OFD_Data.Text = reader["DNS_OFD"].ToString();
                                TextBox_port_OFD_Data.Text = reader["port_OFD"].ToString();
                                TextBox_adress2_OFD_Data.Text = reader["adress_OISM_OFD"].ToString();
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        // Обработка возможных ошибок
                        MessageBox.Show("Ошибка: " + ex.Message);
                    }

                }


                selectQuery = @"
                SELECT 
                    manufacture_FN 
                FROM table_model_FN 
                WHERE model_FN = @model_FN";
                using (SQLiteCommand sqlCommand = new SQLiteCommand(selectQuery, connection))
                {
                    // Добавление параметра к запросу для предотвращения SQL-инъекций
                    sqlCommand.Parameters.AddWithValue("@model_FN", fn);

                    try
                    {

                        using (SQLiteDataReader reader = sqlCommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                TextBox_Name_FN_Data.Text = reader["manufacture_FN"].ToString();
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        // Обработка возможных ошибок
                        MessageBox.Show("Ошибка: " + ex.Message);
                    }
                }

                selectQuery = @"
                SELECT 
                    adress_FN, 
                    port_FN 
                FROM options_FN 
                WHERE name_FN = @name_FN";
                using (SQLiteCommand sqlCommand = new SQLiteCommand(selectQuery, connection))
                {
                    // Добавление параметра к запросу для предотвращения SQL-инъекций
                    sqlCommand.Parameters.AddWithValue("@name_FN", TextBox_Name_FN_Data.Text);

                    try
                    {
                        using (SQLiteDataReader reader = sqlCommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                TextBox_adress_FN_Data.Text = reader["adress_FN"].ToString();
                                TextBox_port_FN_Data.Text = reader["port_FN"].ToString();
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
        
        private void butClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
