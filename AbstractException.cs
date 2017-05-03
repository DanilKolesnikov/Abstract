using System;

namespace ConsolCourse
{
    class AbstractException:Exception
    {
        public AbstractExceptionKey exceptionKey;
        public AbstractException() : base() { }
        public AbstractException(AbstractException e) : base(e.Message) { this.exceptionKey = e.exceptionKey; }
        public AbstractException(string message) : base(message) { }
        public AbstractException(string message, AbstractExceptionKey exceptionKey) : base(message) {
            this.exceptionKey = exceptionKey;
        }
        public AbstractException(string message, System.Exception inner) : base(message, inner) { }
    }
    public enum AbstractExceptionKey
    {
        MorphAnalysis,
        File,
        Json,
        Clear,
        Abstract
    }
}
