using System;
using System.Runtime.Serialization;

namespace VisualizationRecorder {
    [Serializable()]
    public class FilePickFaildException : Exception {
        public FilePickFaildException() : base() { }
        public FilePickFaildException(string message) : base(message) { }
        public FilePickFaildException(string message, Exception inner) : base(message, inner) { }
        protected FilePickFaildException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    [Serializable()]
    public class FileNotSaveException : Exception {
        public FileNotSaveException() : base() { }
        public FileNotSaveException(string message) : base(message) { }
        public FileNotSaveException(string message, Exception inner) : base(message, inner) { }
        protected FileNotSaveException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
