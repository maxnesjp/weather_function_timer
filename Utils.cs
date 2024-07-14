using System;
using System.Text.RegularExpressions;

public static class Utils
{
    public static string GetErrorMessage(Exception ex) => ex.Message;

    public static bool ValidateEmailFormat(string email)
    {
        var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        return emailRegex.IsMatch(email);
    }

    public static bool ValidateString(string str, int maxLength)
    {
        return !string.IsNullOrEmpty(str) && str.Length <= maxLength;
    }

    public class WeatherApiResponse
    {
        public Location Location { get; set; }
        public Current Current { get; set; }
    }


    public class Location
    {
        public string Name { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public string Tz_Id { get; set; }
        public long Localtime_Epoch { get; set; }
        public string Localtime { get; set; }
    }

    public class Current
    {
        public long Last_Updated_Epoch { get; set; }
        public string Last_Updated { get; set; }
        public double Temp_C { get; set; }
        public double Temp_F { get; set; }
        public int Is_Day { get; set; }
        public Condition Condition { get; set; }
        public double Wind_Mph { get; set; }
        public double Wind_Kph { get; set; }
        public int Wind_Degree { get; set; }
        public string Wind_Dir { get; set; }
        public double Pressure_Mb { get; set; }
        public double Pressure_In { get; set; }
        public double Precip_Mm { get; set; }
        public double Precip_In { get; set; }
        public int Humidity { get; set; }
        public int Cloud { get; set; }
        public double Feelslike_C { get; set; }
        public double Feelslike_F { get; set; }
        public double Windchill_C { get; set; }
        public double Windchill_F { get; set; }
        public double Heatindex_C { get; set; }
        public double Heatindex_F { get; set; }
        public double Dewpoint_C { get; set; }
        public double Dewpoint_F { get; set; }
        public double Vis_Km { get; set; }
        public double Vis_Miles { get; set; }
        public double Uv { get; set; }
        public double Gust_Mph { get; set; }
        public double Gust_Kph { get; set; }
    }

    public class Condition
    {
        public string Text { get; set; }
        public string Icon { get; set; }
        public int Code { get; set; }
    }
}
