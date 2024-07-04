using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SahoAcs.DBModel
{
    public class City
    {
        List<City> Cities = new List<DBModel.City>();
        public string CityName { get; set; }
        public string CityVal { get; set; }



        public List<City> GetCities()
        {            
            this.Cities.Add(new City { CityName = "台北市", CityVal = "A" });
            this.Cities.Add(new City { CityName = "新北市", CityVal = "F" });
            this.Cities.Add(new City { CityName = "桃園市", CityVal = "H" });
            this.Cities.Add(new City { CityName = "台中市", CityVal = "B" });
            this.Cities.Add(new City { CityName = "台南市", CityVal = "D" });
            this.Cities.Add(new City { CityName = "高雄市", CityVal = "E" });
            this.Cities.Add(new City { CityName = "基隆市", CityVal = "C" });
            this.Cities.Add(new City { CityName = "新竹市", CityVal = "O" });
            this.Cities.Add(new City { CityName = "新竹縣", CityVal = "J" });
            this.Cities.Add(new City { CityName = "苗栗縣", CityVal = "K" });
            this.Cities.Add(new City { CityName = "彰化縣", CityVal = "M" });
            this.Cities.Add(new City { CityName = "南投縣", CityVal = "N" });
            this.Cities.Add(new City { CityName = "雲林縣", CityVal = "P" });
            this.Cities.Add(new City { CityName = "嘉義市", CityVal = "I" });
            this.Cities.Add(new City { CityName = "嘉義縣", CityVal = "Q" });
            this.Cities.Add(new City { CityName = "屏東縣", CityVal = "T" });
            this.Cities.Add(new City { CityName = "台東縣", CityVal = "V" });
            this.Cities.Add(new City { CityName = "花蓮縣", CityVal = "U" });
            this.Cities.Add(new City { CityName = "宜蘭縣", CityVal = "G" });
            this.Cities.Add(new City { CityName = "澎湖縣", CityVal = "X" });
            this.Cities.Add(new City { CityName = "金門縣", CityVal = "W" });
            this.Cities.Add(new City { CityName = "連江縣", CityVal = "Z" });
            return this.Cities;
        }
    }//end entity class
}//end namespace