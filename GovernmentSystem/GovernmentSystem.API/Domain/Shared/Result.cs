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

        //public bool IsFailure => !IsSuccess;
        public Error Error { get; }

        public T Value { get; }

        public static Result<T> Success(T value) => new(true, Error.None, value);

        public static Result<T> Failure(Error error) => new(false, error, default!);
    }
}