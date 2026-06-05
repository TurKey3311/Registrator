using MaterialSkin.Controls;
using Registrator.services.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registrator
{
    public class Mask
    {
        public Result<string> MaskPhoneNumber(string phone_number)
        {
            if (string.IsNullOrWhiteSpace(phone_number))
            {
                return Result<string>.Failure("Номер телефона не может быть пустым");
            }

            string clean_number = phone_number.Replace(" ", "")
                                              .Replace("(", "")
                                              .Replace(")", "")
                                              .Replace("-", "")
                                              .Replace("+", "");

            // Проверка на символы '[]' с предупреждением
            if (clean_number.Contains("[") || clean_number.Contains("]"))
            {
                //MaterialMessageBox.Show("Кияс специально попросил сделать проверку на символы '[]' и он их нашел!");
                clean_number = clean_number.Replace("[", "").Replace("]", "");
            }

            if (clean_number.Length > 10)
            {
                clean_number = clean_number.Remove(0, 1);
            }

            clean_number = new string(clean_number.Where(char.IsDigit).ToArray());

            if (clean_number.Length != 10)
            {
                return Result<string>.Failure($"Неверная длина номера телефона. Ожидается 10 цифр, получено {clean_number.Length}");
            }

            StringBuilder formatted_number = new StringBuilder("+7(___)___-__-__");
            for (int i = 0; i < clean_number.Length; i++)
            {
                if (i < 3)
                    formatted_number[i + 3] = clean_number[i];
                else if (i < 6)
                    formatted_number[i + 4] = clean_number[i];
                else if (i < 8)
                    formatted_number[i + 5] = clean_number[i];
                else if (i < 10)
                    formatted_number[i + 6] = clean_number[i];
            }

            return Result<string>.Success(formatted_number.ToString());
        }

        public void MaskDateTime(string dataTime)
        {
            
            if (dataTime[0] != ' ' && dataTime[1] != ' ' && dataTime[3] != ' ' && dataTime[4] != ' ' && dataTime[6] != ' ' && dataTime[7] != ' ' && dataTime[8] != ' ' && dataTime[9] != ' ' && dataTime[11] != ' ' && dataTime[12] != ' ' && dataTime[14] != ' ' && dataTime[15] != ' ')
            {

                if (dataTime[2] != '.' || dataTime[5] != '.' || dataTime[10] != ' ' || dataTime[13] != ':')
                {
                    MaterialMessageBox.Show("Некорректно указанны дата и время. Введите по следующему формату: дд.мм.гггг чч:мм");
                }
                if (dataTime[0] != '0' && dataTime[0] != '1' && dataTime[0] != '2' && dataTime[0] != '3') // ограничения первого числа дней
                { MaterialMessageBox.Show("Некорректно указано число"); }

                if (dataTime[0] == '0' && dataTime[1] == '0') { MaterialMessageBox.Show("Некорректно указано число"); } // ограничение 0 месяца

                if (dataTime[3] != '0' && dataTime[3] != '1') { MaterialMessageBox.Show("Некорректно указан месяц"); } // ограничение первого числа месяца

                if (dataTime[3] == '0' && dataTime[4] == '0') { MaterialMessageBox.Show("Некорректно указан месяц"); } // ограничение 0 месяца

                if (dataTime[3] == '1' && (dataTime[4] != '0' && dataTime[4] != '1' & dataTime[4] != '2')) { MaterialMessageBox.Show("Некорректно указан месяц"); } // ограничение второй цифры месяца

                if (dataTime[11] != '0' && dataTime[11] != '1' && dataTime[11] != '2') { MaterialMessageBox.Show("Некорректно указаны часы"); } // ограничение первой цифры часа

                if (dataTime[11] == '2' && (dataTime[12] != '0' && dataTime[12] != '1' && dataTime[12] != '2' && dataTime[12] != '3')) { MaterialMessageBox.Show("Некорректно указаны часы"); } // ограничения второй цифры часа

                if (dataTime[14] != '0' && dataTime[14] != '1' && dataTime[14] != '2' && dataTime[14] != '3' && dataTime[14] != '4' && dataTime[14] != '5') { MaterialMessageBox.Show("Некорректно указаны минуты"); } // ограничение первой цифры минут

                if (dataTime[3] == '0' && dataTime[4] == '2' && (dataTime[0] != '0' && dataTime[0] != '1' && dataTime[0] != '2')) // проверка феврала
                {
                    MaterialMessageBox.Show("В феврале может быть только 28 или 29 дней");
                }
                if (dataTime[3] == '0' && (dataTime[4] == '4' || dataTime[4] == '6' || dataTime[4] == '9' || (dataTime[3] == '1' && dataTime[4] == '1'))) // проверка месяцев с 30 днями
                {
                    if (dataTime[0] == '3' && dataTime[1] != '0')
                        MaterialMessageBox.Show("Указан месяц в котором не может быть 31 день.");
                }

                if (dataTime[0] == '3' && (dataTime[1] != '0' && dataTime[1] != '1')) { MaterialMessageBox.Show("Указано некорректное число"); }
                if (dataTime[3] != '0' && (dataTime[3] == '1' && dataTime[4] != '0' && dataTime[4] != '1' && dataTime[4] != '2')) { MaterialMessageBox.Show("Указано некорректное число"); }
            }
        }
    }
}
