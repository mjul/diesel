namespace Diesel.Transformations
{
    /// <summary>
    /// Represents a known type in the model.
    /// </summary>
    public class KnownType
    {
        public string FullName { get; private set; }
        public bool IsValueType { get; private set; }

        public KnownType(string fullName, bool isValueType)
        {
            FullName = fullName;
            IsValueType = isValueType;
        }
    }
}