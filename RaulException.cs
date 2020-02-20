using System;


[Serializable()]
public class RaulException : System.Exception{
    public RaulException() : base(){ }
    public RaulException(string message) : base(message) {}
    public RaulException(string message, System.Exception inner) : base(message, inner) { }


    // A constructor is needed for serialization when an
    // exception propagates from a remoting server to the client. 
    protected RaulException(System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}