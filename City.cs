using MySql.Data.MySqlClient;
using System.IO;
using System.Net;
using System.Text;


namespace Weather_monitor
{
    class City
    {
        public int CityID { get; set; }
        public string CityName { get; set; }
        public string TomorrowTemperature { get; set; }
        public string CityURL { get; set; }

        public string GetCityTemperature(City city)
        {
            string url = "https://www.gismeteo.by/";
            url += city.CityURL;
            string html = string.Empty;
            // Отправляем GET запрос и получаем в ответ HTML-код сайта 
            HttpWebRequest myHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
            StreamReader myStreamReader = new StreamReader(myHttpWebResponse.GetResponseStream());
            html = myStreamReader.ReadToEnd();
            //Console.WriteLine("Trying create file grabcity");
            FileStream fileStream = new FileStream(@".\grabcity.txt", FileMode.Create);
            //Console.WriteLine("grabcity created!");
            byte[] array = System.Text.Encoding.Default.GetBytes(html);
            fileStream.Write(array, 0, array.Length);
            string[] words = html.Split(',', '>', '<', '/', ':', ';', ' ', '.', '*', '@', '=', '"', ' ', '!');
            string[] cityData = new string[words.Length];
            fileStream.Close();

            string tempData = "";
            string[] dataTemp = File.ReadAllLines(@".\grabcity.txt", Encoding.Default);
            foreach (string str in dataTemp)
            {
                tempData += str;
                //Console.WriteLine(str);
            }
            words = tempData.Split(' ', '"', '<', '>', '/', ',', '.');
            string tomorrowTemperature="";
            int counter = 0;
            int counterRepeats = 0;
            foreach (string word in words)
            {
                word.Trim();
                if (word == "unit_temperature_c")
                {
                    counterRepeats++;
                    if (counterRepeats == 8)
                    {
                        // Console.WriteLine("find 8 rep!!!");
                        //Console.WriteLine("temperature is " + words[counter + 2]);
                        tomorrowTemperature = words[counter + 2];
                    }

                }
                counter++;


            }
            return tomorrowTemperature;
        }

        public void SaveChangesInDatabase(City city)
        {
            string connectionString = "server=localhost;port=3307;user=root;database=weather;password=qwerty12345;";
            MySqlConnection mySqlConnection = new MySqlConnection(connectionString);
            mySqlConnection.Open();
            //string sql = "DELETE FROM weather.table1 WHERE CityName="+city.CityName;
           // MySqlCommand sqlCommand = new MySqlCommand(sql, mySqlConnection);
            /*string request = sqlCommand.ExecuteScalar();*/
            string sql = "INSERT INTO weather.table1(CityName,CityTemp) VALUES ('"+city.CityName+"', '"+city.TomorrowTemperature+"')";
            MySqlCommand sqlCommand = new MySqlCommand(sql, mySqlConnection);
            sqlCommand.ExecuteScalar();
            //Console.WriteLine(request);
            mySqlConnection.Close();
        }
    }
}

