using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using MySql.Data.MySqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Weather_monitor
{
    class Program
    {
        // Queue<City> Cities = new Queue<City>();
        public static ArrayList CityList = new ArrayList(); 
        static void Main(string[] args)
        {
            /*string connectionString="server=localhost;port=3307;user=root;database=weather;password=qwerty12345;";
            MySqlConnection mySqlConnection = new MySqlConnection(connectionString);
            mySqlConnection.Open();
            string sql = "TRUNCATE TABLE weather.table1";
            MySqlCommand sqlCommand = new MySqlCommand(sql,mySqlConnection);
            sqlCommand.ExecuteScalar();*/
            //Console.WriteLine(request);
             GetCityList();
            //mySqlConnection.Close();
            //CityList= new ArrayList();
        }

        public static void GetCityList()
        {
            string url = "https://www.gismeteo.by/";
            string html = string.Empty;
            // Отправляем GET запрос и получаем в ответ HTML-код сайта 
            HttpWebRequest myHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
            StreamReader myStreamReader = new StreamReader(myHttpWebResponse.GetResponseStream());
            html = myStreamReader.ReadToEnd();
            FileStream fileStream = new FileStream(@"\grab.txt", FileMode.Create);
            byte[] array = System.Text.Encoding.Default.GetBytes(html);
            fileStream.Write(array, 0, array.Length);
            string[] words=html.Split(',','>','<','/',':',';',' ','.','*','@','=','"',' ','!');
            string[] cityData=new string[words.Length];
            int counterCityData = 0;
            StreamWriter SW = new StreamWriter(new FileStream(@"\grab_to_words.txt", FileMode.Create, FileAccess.Write));

            bool test = false;
            /*foreach (string str in words)
            {
                Console.WriteLine(str);
                SW.WriteLine(str);
            }*/
            for (int i = 0; i < words.Length; i++)
            {
                string str = words[i];
                if (words[i] == "City" && words[i+1]=="frame"&&words[i-1]!="End")
                {
                    test = true;

                }
                else if(words[i] == "End" && words[i+1] == "City")
                {
                    test = false;
                }else 
                if (test == true&&str.Length>1)//отбрасываем пустые строки 
                {
                   // Console.WriteLine(str);
                    SW.WriteLine(str);
                    cityData[counterCityData] = str;
                    counterCityData++;
                }
            }
            for (int i = 0; i < cityData.Length; i++)
            {
                if (cityData[i] == "data-name")
                {
                    City city = new City();
                    city.CityName = cityData[i + 1];
                    city.CityURL = cityData[i + 6];
                    CityList.Add(city);
                     
                    i += 10;
                }
                
            }
            SW.Close();
            
            foreach(City city in CityList)
            {
                //Console.WriteLine(city.CityName+"    "+city.CityURL);
               string temp= city.GetCityTemperature(city);
                city.TomorrowTemperature = temp;
                //Console.WriteLine("get temp"+temp);
                Console.WriteLine(city.CityName + "    " + city.CityURL+"--->" +city.TomorrowTemperature);
                //city.SaveChangesInDatabase(city);

            }
            // Регулярное выражение
           



           
            Console.ReadLine();
        }

    }
}
