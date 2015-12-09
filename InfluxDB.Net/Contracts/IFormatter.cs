using InfluxDB.Net.Models;

namespace InfluxDB.Net.Contracts
{
    public interface IFormatter
    {
        string PointToString(Point point);
        string GetLineTemplate();
        Serie PointToSerie(Point point);
    }
}