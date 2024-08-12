using FluentValidation.Results;

namespace Application.Common.Exceptions;

public class ValidationException : Exception
{
    public ValidationException()
        : base("One or more validation failures have occurred.")
    {
        Errors = new Dictionary<string, string[]>();
    }
    public ValidationException(IEnumerable<ValidationFailure> failures)
        : this()
    {
        //Errors = failures.GroupBy(f => f.PropertyName)
        //                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
        Errors = failures.GroupBy(f => f.PropertyName)
                       .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
    }

    public IDictionary<string, string[]> Errors { get; }

    public string GetErrors()
    {
        string errors = string.Empty;
        foreach (var (propertyName, errorMessages) in Errors)
        {
            //{propertyName}, 
            errors += ($"{string.Join(", ", errorMessages)}");
        }
        return errors;
    }
}