namespace KoboRack.Data.Repository.DTO
{
    public class ResponseDto2<T>
    {
        public string DisplayMessage { get; set; }
        public int StatusCode { get; set; }
        public T Result { get; set; }
    }
}
