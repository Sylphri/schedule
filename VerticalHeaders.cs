namespace schedule
{
    public class VerticalHeaders
    {
        public static readonly string[] days = new string[7]
        {
            "Неділя",
            "Понеділок",
            "Вівторок",
            "Середа",
            "Четвер",
            "П'ятниця",
            "Субота"
        };

        /// <summary>
        /// Повертає масив у вигляді:
        /// Понеділок 1
        ///           2
        ///           3
        ///           4
        /// Вівторок  1
        /// ...
        /// </summary>
        /// <param name="firstDay">Перший день 0 - неділя, 6 - субота </param>
        /// <param name="lastDay">Останній день (включається в список) 0 - неділя, 6 - субота </param>
        /// <param name="maxNumber">Найбільше номер, включається в список</param>
        /// <returns>Двовимірний масив заголовків</returns>
        public static string[,] CreateDayNumberHeaders(int firstDay, int lastDay, int maxNumber)
        {
            
            if (lastDay - firstDay < 0)
            {
                lastDay += 7;
            }
            string[,] result = new string[(lastDay - firstDay+1)*maxNumber, 2];
            for (int dayIndex = firstDay; dayIndex <= lastDay; ++dayIndex)
            {
                result[(dayIndex - firstDay) * maxNumber, 0] = days[dayIndex%7];
                for(int number=1; number<=maxNumber; ++number)
                {
                    result[(dayIndex - firstDay) * maxNumber+(number-1), 1] = number.ToString();
                }
            }
            return result;
        }
    }
}
