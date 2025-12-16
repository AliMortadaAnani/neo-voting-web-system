#region Code Implementation and Explanation in old way
/*namespace GovernmentSystem.API.Domain.Shared
{
    // We implement IEquatable<Error> to enforce standard equality checking methods.
    public class Error : IEquatable<Error>
    {
        // 1. BACKING FIELDS
        // These private fields hold the actual data in memory.
        // 'readonly' ensures they cannot be changed after the object is created (Immutability).
        private readonly string _code;
        private readonly string _description;
        private readonly ErrorType _type;

        // 2. PRIVATE CONSTRUCTOR
        // We make this private to force users to use the Static Factory methods below.
        // This ensures objects are always created in a valid state.
        private Error(string code, string description, ErrorType type)
        {
            this._code = code;
            this._description = description;
            this._type = type;
        }

        // 3. PUBLIC PROPERTIES
        // Explicit getters that read from the backing fields.
        public string Code
        {
            get { return this._code; }
        }

        public string Description
        {
            get { return this._description; }
        }

        public ErrorType Type
        {
            get { return this._type; }
        }

        // 4. STATIC PRE-DEFINED FIELDS

        // Represents "No Error".
        public static readonly Error None = new Error(string.Empty, string.Empty, ErrorType.None);

        // Represents a Generic Null Value error.
        public static readonly Error NullValue = new Error("Error.NullValue", "Null value was provided", ErrorType.Failure);

        // 5. STATIC FACTORY METHODS
        // These create specific types of errors based on the ErrorType enum.

        // Maps to HTTP 404 (Not Found)
        public static Error NotFound(string code, string description)
        {
            return new Error(code, description, ErrorType.NotFound);
        }

        // Maps to HTTP 400 (Bad Request)
        public static Error Validation(string code, string description)
        {
            return new Error(code, description, ErrorType.Validation);
        }

        // Maps to HTTP 409 (Conflict)
        public static Error Conflict(string code, string description)
        {
            return new Error(code, description, ErrorType.Conflict);
        }

        // Maps to HTTP 500 (Internal Server Error) or Logic Failure
        public static Error Failure(string code, string description)
        {
            return new Error(code, description, ErrorType.Failure);
        }

        // Maps to HTTP 401 (Unauthorized) - e.g., Invalid Token
        public static Error UnAuthorized(string code, string description)
        {
            return new Error(code, description, ErrorType.Unauthorized);
        }

        // Maps to HTTP 403 (Forbidden) - e.g., Admin only area
        public static Error Forbidden(string code, string description)
        {
            return new Error(code, description, ErrorType.Forbidden);
        }

        // 6. VALUE EQUALITY IMPLEMENTATION
        // Since we converted a 'record' to a 'class', we lost automatic value equality.
        // We must manually write code to say: "Two Error objects are equal if their Data is equal."

        public override bool Equals(object? obj)
        {
            // If the other object is null, they are not equal.
            if (obj is null)
            {
                return false;
            }

            // If the other object is not an Error type, they are not equal.
            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            // Check values using the strong-typed helper method below.
            return Equals((Error)obj);
        }

        public bool Equals(Error? other)
        {
            // If the other is null, false.
            if (other is null)
            {
                return false;
            }

            // If it is the exact same reference in memory, true.
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            // Compare the actual data fields.
            // Two errors are equal if Code, Description, and Type match.
            return this._code == other._code &&
                   this._description == other._description &&
                   this._type == other._type;
        }

        public override int GetHashCode()
        {
            // Combine the hash codes of the fields to create a unique ID for this specific data combination.
            return HashCode.Combine(this._code, this._description, this._type);
        }

        // Operator overrides to allow using '==' and '!=' with this class.
        public static bool operator ==(Error? a, Error? b)
        {
            if (a is null && b is null) return true;
            if (a is null || b is null) return false;
            return a.Equals(b);
        }

        public static bool operator !=(Error? a, Error? b)
        {
            return !(a == b);
        }
    }
}

*/


/*using System;

namespace GovernmentSystem.API.Domain.Shared
{
    // <T> means this class is Generic. T can be a Voter, a Candidate, an int, or anything.
    public class Result<T>
    {
        // 1. BACKING FIELDS
        // These are the private variables where the actual data is stored in memory.
        private readonly bool _isSuccess;
        private readonly Error _error;
        private readonly T _value;

        // 2. PRIVATE CONSTRUCTOR
        // It is private so no one can type 'new Result<T>(...)' directly.
        // They MUST use the static methods 'Success' or 'Failure' below.
        private Result(bool isSuccess, Error error, T value)
        {
            // Assign the incoming 'isSuccess' parameter to our private field.
            this._isSuccess = isSuccess;

            // Assign the incoming 'error' parameter to our private field.
            this._error = error;

            // Assign the incoming 'value' (the data) to our private field.
            this._value = value;
        }

        // 3. PUBLIC PROPERTIES
        // These allow outside classes (like Controllers) to READ the private fields.

        public bool IsSuccess
        {
            get
            {
                // Return the value stored in the private field.
                return this._isSuccess;
            }
        }

        public bool IsFailure
        {
            get
            {
                // Logic: If IsSuccess is true, IsFailure is false.
                // Logic: If IsSuccess is false, IsFailure is true.
                // The '!' symbol flips the boolean value.
                return !this._isSuccess;
            }
        }

        public Error Error
        {
            get
            {
                // Return the error object stored in memory.
                return this._error;
            }
        }

        public T Value
        {
            get
            {
                // Return the actual data (e.g., the Voter object) stored in memory.
                return this._value;
            }
        }

        // 4. STATIC FACTORY METHODS
        // These are the only way to create an object of this class.

        // Usage: var result = Result<Voter>.Success(myVoter);
        public static Result<T> Success(T value)
        {
            // Create a new instance of the Result class.
            // 1. Set isSuccess to 'true'.
            // 2. Set Error to 'Error.None' (meaning no error occurred).
            // 3. Pass the actual data (value) to store it.
            Result<T> newResultObject = new Result<T>(true, Error.None, value);

            // Return this newly created object to the caller.
            return newResultObject;
        }

        // Usage: var result = Result<Voter>.Failure(Error.NotFound);
        public static Result<T> Failure(Error error)
        {
            // Define the default value for type T because we have no data to return.
            // If T is a class (Voter), default is 'null'.
            // If T is an int, default is '0'.
            T? defaultValue = default(T);

            // Create a new instance of the Result class.
            // 1. Set isSuccess to 'false'.
            // 2. Pass the specific Error object explaining what went wrong.
            // 3. Pass null (or default) as the value, because the operation failed.
            Result<T> newResultObject = new Result<T>(false, error, defaultValue!);

            // Return this newly created object to the caller.
            return newResultObject;
        }
    }
}*/
#endregion


namespace GovernmentSystem.API.Domain.Shared
{
    public class Result<T>
    {
        private Result(bool isSuccess, Error error, T value)
        {
            IsSuccess = isSuccess;
            Error = error;
            Value = value;
        }

        public bool IsSuccess { get; }

        public bool IsFailure => !IsSuccess;
        public Error Error { get; }

        public T Value { get; }

        public static Result<T> Success(T value) => new(true, Error.None, value);

        public static Result<T> Failure(Error error) => new(false, error, default!);
    }
}