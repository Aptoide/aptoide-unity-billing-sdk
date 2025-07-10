[System.Serializable]
public class UnfetchedProduct
{
    public string ProductId;
    public string ProductType;
    public int StatusCode;

    public static class StatusCodes
    {
        public const int ProductNotFound = 3;
        public const int Unknown = 0;
    }
}
