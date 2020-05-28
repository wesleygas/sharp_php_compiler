using System;

public enum TokenTypes{
    None,
    INT,
    STRING,
    PLUS,
    MINUS,
    STAR,
    DOT,
    COMMA,
    SEMI,
    SLASH,
    LPAR,
    RPAR,
    LBRACE,
    RBRACE,
    KEYWORD,
    IDEN,
    FUNCNAME,
    EQUAL,
    EQEQUAL,
    NOT,
    GREATER,
    LESS,
    START_PROG,
    END_PROG,
    EOF
}

public class Token {
    private TokenTypes _type;
    private dynamic _value;

    public Token(TokenTypes type, dynamic value){
        _type = type;
        _value = value;
    }
    public TokenTypes Type { get => _type; set=> this._type = value;}
    public dynamic Value { get => _value; set => this._value = value;}

    
}