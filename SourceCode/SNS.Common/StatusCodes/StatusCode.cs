namespace SNS.Common.StatusCodes;

public record StatusCode(
string Category,   
int Code
)
{
    public override string ToString()
        => $"{Category}_{Code}";
}