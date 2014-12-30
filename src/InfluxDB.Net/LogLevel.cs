namespace InfluxDB.Net
{
    public enum LogLevel
    {
        /** No logging. */
        None,
        /** Log only the request method and URL and the response status code and execution time. */
        Basic,
        /** Log the basic information along with request and response headers. */
        Headers,
        /*
		 * Log the headers, body, and metadata for both requests and responses.
		 * Note: This requires that the entire request and response body be buffered in memory!
		 */
        Full
    }
}