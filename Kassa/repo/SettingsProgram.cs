using MaterialSkin.Controls;
using Registrator.repo.models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registrator.repo
{
    // Класс для загрузки настроек
    public class SettingsLoader
    {
        private readonly SQLiteConnection connection;
        public string connectionString = ConfigurationManager.ConnectionStrings["SQLiteDB"].ConnectionString;
        SettingsProgram settings = new SettingsProgram();

        public SettingsLoader()
        {
            // Инициализация и открытие соединения
            connection = new SQLiteConnection(connectionString);
            connection.Open();
        }

        public SettingsProgram GetSettings()
        {            
            string GetParameterValue(string parameterName)
            {
                string query = "SELECT * FROM options_program WHERE parameter = @param";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@param", parameterName);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return (string)reader["meaning"];
                        }
                    }
                }
                return null;
            }

            // Получение и присвоение значений
            settings.AdressFile = GetParameterValue("adr_file");
            settings.StandartOFD = GetParameterValue("standart_OFD");
            settings.StandartModelFN = GetParameterValue("standart_FN");
            settings.NameOperator = GetParameterValue("name_operator");
            settings.PortName = GetParameterValue("port_name");
            settings.AdrMU_ID = GetParameterValue("id_fias");
            settings.AdrMU_Index = GetParameterValue("index");
            settings.AdrMU_Region = GetParameterValue("region");
            settings.AdrMU_MunRay_Code = GetParameterValue("mr_code");
            settings.AdrMU_MunRay_Name = GetParameterValue("mr_name");
            settings.AdrMU_NasPunkt_type = GetParameterValue("np_type");
            settings.AdrMU_NasPunkt_Name = GetParameterValue("np_name");
            settings.AdrMU_Street_type = GetParameterValue("street_type");
            settings.AdrMU_Street_name = GetParameterValue("street_name");
            settings.AdrMU_Building_type = GetParameterValue("building_type");
            settings.AdrMU_Building_number = GetParameterValue("building_number");
            settings.AdrMU_building_body_type = GetParameterValue("building_body_type");
            settings.AdrMU_building_body_number = GetParameterValue("building_body_number");
            settings.Adress_registration = GetParameterValue("adress_registration");
            settings.ConfigKKTCurrentVersion = GetParameterValue("config_kkt_current_version");

            // Обработка булевых значений
            string delXmlStr = GetParameterValue("del_xml");
            settings.DeleteXML = delXmlStr == "true";

            string printAktStr = GetParameterValue("print_akt");
            settings.PrintAkt = printAktStr == "true";

            string createFolderStr = GetParameterValue("create_folder");
            settings.CreateFolder = createFolderStr == "true";

            return settings;
        }
        public void Dispose()
        {
            if (connection != null)
            {
                connection.Close();
                connection.Dispose();
            }
        }
    }
}
