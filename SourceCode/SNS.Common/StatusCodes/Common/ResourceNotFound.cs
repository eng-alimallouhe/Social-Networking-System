namespace SNS.Common.StatusCodes.Common
{
    public class Resources
    {
        public static readonly StatusCode ResourceFound =
            new("Resource", 200);


        public static readonly StatusCode ResourceNotFound =
            new("Resource", 404);

        public static readonly StatusCode ResourceReadError =
            new("Resource", 500);
    }
}
