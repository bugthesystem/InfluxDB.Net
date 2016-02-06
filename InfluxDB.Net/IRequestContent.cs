using System.Net.Http;

namespace InfluxDB.Net
{
	internal interface IRequestContent
	{
		HttpContent GetContent();
	}
}