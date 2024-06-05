namespace API.Data
{
    public class MutationApiResult<T>
    {
        public T? Result { get; set; }

        public Boolean Status { get; set; } = false;
    }
}
