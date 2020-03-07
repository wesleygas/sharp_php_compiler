using System;

public enum TokenTypes{
    None,
    INT,
    PLUS,
    MINUS,
    MULT,
    DIV,
    EOF
}

public class Token {
    private TokenTypes _type;
    private int _value;

    public Token(TokenTypes type, int value){
        _type = type;
        _value = value;
    }
    public TokenTypes Type { get => _type; set=> this._type = value;}
    public int Value { get => _value; set => this._value = value;}

    
}