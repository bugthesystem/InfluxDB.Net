using InfluxDB.Net.Models;

namespace InfluxDB.Net.Contracts
{
    public interface IFormatter
    {
        string GetLineTemplate();

        string PointToString(Point point);

        Serie PointToSerie(Point point);
    }
}