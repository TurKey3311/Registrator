using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registrator.repo.models
{
    public class SettingsProgram
    {
        public string AdressFile { get; set; }
        public bool DeleteXML { get; set; }
        public bool PrintAkt { get; set; }
        public bool CreateFolder { get; set; }
        public string NameOperator { get; set; }
        public string StandartModelFN { get; set; }
        public string StandartOFD { get; set; }
        public string PortName { get; set; }
        public string AdrMU_ID { get; set; }
        public string AdrMU_Index { get; set; }
        public string AdrMU_Region { get; set; }
        public string AdrMU_MunRay_Code { get; set; }
        public string AdrMU_MunRay_Name { get; set; }
        public string AdrMU_NasPunkt_type { get; set; }
        public string AdrMU_NasPunkt_Name { get; set; }
        public string AdrMU_Street_type { get; set; }
        public string AdrMU_Street_name { get; set; }
        public string AdrMU_Building_type { get; set; }
        public string AdrMU_Building_number { get; set; }
        public string AdrMU_building_body_type { get; set; }
        public string AdrMU_building_body_number { get; set; }
        public string Adress_registration { get; set; }
        public string ConfigKKTCurrentVersion { get; set; }
    }
}
