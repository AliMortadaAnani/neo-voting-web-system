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
namespace GovernmentSystem.API.Domain.Shared
{
    public record Error(string Code, string Description, ErrorType Type)
    {
        public static readonly Error None =
            new(string.Empty, string.Empty, ErrorType.None);//static object representing no error

        public static readonly Error NullValue =
            new("Error.NullValue", "Null value was provided", ErrorType.Failure);//static object representing null value error

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
        //return an instance of Error with Failure type(general errors)
        public static Error Unauthorized(string code, string description) =>
            new(code, description, ErrorType.Unauthorized);
        //return an instance of Error with UnAuthorized type
        public static Error Forbidden(string code, string description) =>
            new(code, description, ErrorType.Forbidden);
        //return an instance of Error with Forbidden type

    }
}
