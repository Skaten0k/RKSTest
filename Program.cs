using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        List<Person> persons;

        using (StreamReader reader = new StreamReader(@"../../../data.json"))
        {
            persons = JsonSerializer.Deserialize<List<Person>>(reader.ReadToEnd(), new JsonSerializerOptions
            {
                NumberHandling = JsonNumberHandling.AllowReadingFromString
            })?.ToList() ?? new List<Person>();
        }

        using (StreamWriter writer = new StreamWriter(@"../../../formated_data.json"))
        {
            writer.WriteLine("[");
            foreach(Person person in persons)
            {
                var json = JSONGenerator.FormPersonRecord(person);
                writer.WriteLine(json);
            }
            writer.WriteLine("]");
        }

        List<Person> top20persons = persons.OrderByDescending(p => p.salary).Take(20).ToList();

        using (StreamWriter writer = new StreamWriter(@"../../../result.txt", false))
        {

            foreach (Person person in top20persons) 
            {
                writer.WriteLine(person.GetFormatedInfo());
            }
        }
    }
}

class JSONGenerator
{
    public static string FormPersonRecord(Person person)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(@" {""name"":""" + person.Name + @""",");
        sb.Append(@"""phoneNumber"":""" + person.GetFormatedPhoneNumber() + @""",");
        sb.Append(@"""salary"":""" + person.GetFormatedSalary() + @"""},");
        var json = sb.ToString();
        return json;
    }
}

class Person
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("phoneNumber")]
    public string phoneNumber { get; set; }

    [JsonPropertyName("salary")]
    public int salary { get; set; } 

    public virtual string GetFormatedInfo()
    {
        StringBuilder sb = new StringBuilder(64);
        sb.Append(this.Name + " ");
        sb.Append(this.GetFormatedPhoneNumber() + ", ");
        sb.Append(this.GetFormatedSalary());
        return sb.ToString();
    }

    public string GetFormatedPhoneNumber()
    {
        if(this.phoneNumber.Length != 11)
        {
            throw new Exception("Некорректный номер телефона");
        }
        return Regex.Replace(this.phoneNumber, @"(\d{1})(\d{3})(\d{3})(\d{2})(\d{2})", "+$1($2)$3-$4-$5");
    }

    public string GetFormatedSalary()
    {
        return this.salary.ToString("N0") + " р.";
    }
}