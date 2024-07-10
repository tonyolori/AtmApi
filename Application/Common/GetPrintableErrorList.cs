using FluentValidation.Results;

namespace Application.Common;

public static class Utils
{
    public static List<string> GetPrintableErrorList(List<ValidationFailure> errors)
    {
        List<string> errorList = [];
        foreach (ValidationFailure failure in errors)
        {
            //{failure.PropertyName}:
            errorList.Add($"{failure.ErrorMessage}");
        }

        return errorList;
    }
    public static bool IsValidAccountnumber(long accountNumber)
    {
        return accountNumber.ToString().Length == 10;
    }
}

