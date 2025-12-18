using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVoting.Domain.ErrorHandling
{
    public record Error(string Code, string Description, ErrorType Type)
    {
        public static readonly Error None =
            new(string.Empty, string.Empty, ErrorType.None);
        //static object representing no error
        // returned when response is success and we will return the proper data type (not an error)

        public static readonly Error NullValue =
            new("Error.NullValue", "Null value was provided", ErrorType.Failure);
        //static object representing null value error
        //usually we dont encounter this error

        // we made these static readonly methods to create specific error types(no input here)

        public static Error NotFound(string code, string description) =>
            new(code, description, ErrorType.NotFound);
        //return an instance of Error with NotFound type
        public static Error Validation(string code, string description) =>
            new(code, description, ErrorType.Validation);
        //return an instance of Error with Validation type
        public static Error Conflict(string code, string description) =>
            new(code, description, ErrorType.Conflict);
        //return an instance of Error with Conflict type
        public static Error Failure(string code, string description) =>
            new(code, description, ErrorType.Failure);
        //return an instance of Error with Failure type(general errors like 500 or unknown...)
        public static Error Unauthorized(string code, string description) =>
            new(code, description, ErrorType.Unauthorized);
        //return an instance of Error with UnAuthorized type
        public static Error Forbidden(string code, string description) =>
            new(code, description, ErrorType.Forbidden);
        //return an instance of Error with Forbidden type
    }
}
