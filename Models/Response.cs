namespace HomeBankingMindHub.Models
{
    public class Response<T>
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public Response() { }

        // para casos donde solo se necesita statuscode y message
        public Response(int statusCode, string message)
        {
            StatusCode = statusCode;
            Message = message;
        }

        // para casos donde también se necesita un objeto de datos
        public Response(int statusCode, string message, T data)
        {
            StatusCode = statusCode;
            Message = message;
            Data = data;
        }
    }

}
