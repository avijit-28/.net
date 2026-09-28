//Dictionary<string, string> dictionaryCountries = new Dictionary<string, string>();

//dictionaryCountries.Add("UK", "London, Manchester, Birmingham");
//dictionaryCountries.Add("USA", "Chicago, New York, Washington");
//dictionaryCountries.Add("IND", "Mumbai, Delhi, Kolkata");

//Console.WriteLine("Accessing Dictionary Elements using For Each Loop");
//foreach (KeyValuePair<string, string> KVP in dictionaryCountries)
//{
//    Console.WriteLine($"Key:{KVP.Key}, Value: {KVP.Value}");
//}
////Console.WriteLine("\nAccessing Dictionary Elements using For Loop");
////for (int i = 0; i < dictionaryCountries.Count; i++)
////{
////    string key = dictionaryCountries.Keys.ElementAt(i);
////    string value = dictionaryCountries[key];
////    Console.WriteLine($"Key: {key}, Value: {value}");
////}
////Console.WriteLine("GHUMAAAAAA");

//Console.WriteLine("\nAccessing Dictionary Elements using Keys");
////Console.WriteLine($"Key: UK, Value: {dictionaryCountries["UK"]}");
//Console.WriteLine($"country : USA, Places: {dictionaryCountries["USA"]}");

//dictionaryCountries["UK"] += ",xyz";
//Console.WriteLine($"Key: UK, Value: {dictionaryCountries["UK"]}");

using DictionaryCollection;

Dictionary<int, Country> dt = new Dictionary<int, Country>()
{
    {101,new Country{ Code= 101, CountryName= "India", CountryCapital = "Delhi"} },
    {102,new Country{Code = 102, CountryName = "Nepal", CountryCapital = "Katmandu"} },
    {103, new Country{Code = 103, CountryName= "Bangladhes", CountryCapital = "Dhaka"} }
};

Console.WriteLine("Enter Country Code :");
int c_code = Convert.ToInt32(Console.ReadLine());

foreach (var it in dt)
{
    if (c_code == it.Key)
        Console.WriteLine($"Country Code : {it.Value.Code} | Country Name : {it.Value.CountryName} | Country Capital : {it.Value.CountryCapital}");
}


