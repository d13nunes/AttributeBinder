using System;
namespace AttributeBinder
{
    [AttributeUsage(AttributeTargets.Property)]
    public class Bind : Attribute
    {
        public string Source { get; private set; }

        public Bind(string source)
        {
            Source = source;
        }
    }
}
