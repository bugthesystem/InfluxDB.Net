namespace InfluxDB.Net.Infrastructure.Formatters
{
    internal class FormatterV092 : FormatterBase
    {
        protected override string ToInt(string result)
        {
            return result;
        }
    }
}