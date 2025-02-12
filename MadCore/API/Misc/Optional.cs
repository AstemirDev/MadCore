using System;

namespace MadCore.API.Misc
{
    public class Optional<T>
    {
        private readonly T _value;

        public Optional(T value)
        {
            _value = value;
        }

        public T Get()
        {
            return _value;
        }

        public void IfPresent(Action<T> action)
        {
            if (IsPresent())
            {
                action.Invoke(_value);
            }
        }
        
        public bool IsPresent()
        {
            return _value != null;
        }
        
        public static Optional<T> Of(T value)
        {
            return new Optional<T>(value);
        }
    }
}