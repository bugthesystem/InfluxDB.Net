namespace InfluxDB.Net
{
    internal class FormatterV092 : FormatterV09x
    {
        protected override string ToInt(string result)
        {
            return result;
        }
    }
}