namespace InfluxDB.Net.Infrastructure.Formatters
{
    internal class FormatterV092 : FormatterV09x
    {
        protected override string ToInt(string result)
        {
            return result;
        }
    }
}