using System;
using System.Text.RegularExpressions;

class Tokenizer {
    string origin;
    int position;
    Token current;

    const string identifier_regex = @"[\$][a-z]\w*";
    const string number_regex = @"\d+";
    const string keyword_regex = @"[a-zA-Z]+"; 
    
    public Tokenizer(string Origin, bool feedIn){
        Position = 0;
        origin = Origin;
        current = new Token(TokenTypes.None, 0);
        if(feedIn) SelectNext();
    }

    public Token Current { get => current; set => current = value; }
    public int Position { get => position; set => position = value; }

    public void SelectNext(){
        while(position < origin.Length && (origin[position] == ' ' || origin[position] == '\n' || origin[position] == '\r')) position++;
        int cursor = position;
        if(cursor == origin.Length){
            current = new Token(TokenTypes.EOF, 0);
            return;
        }
        switch (origin[cursor])
        {
            case '+':{
                current = new Token(TokenTypes.PLUS, 0);
                position++;
                return;
            }
            case '-':{
                current = new Token(TokenTypes.MINUS, 0);
                position++;
                return;
            }
            case '*':{
                current = new Token(TokenTypes.STAR, 0);
                position++;
                return;
            }
            case '/':{
                current = new Token(TokenTypes.SLASH, 0);
                position++;
                return;
            }
            case '(':{
                current = new Token(TokenTypes.LPAR, 0);
                position++;
                return;
            }
            case ')':{
                current = new Token(TokenTypes.RPAR, 0);
                position++;
                return;
            }
            case '{':{
                current = new Token(TokenTypes.LBRACE, 0);
                position++;
                return;
            }
            case '}':{
                current = new Token(TokenTypes.RBRACE, 0);
                position++;
                return;
            }
            case ';':{
                current = new Token(TokenTypes.SEMI, 0);
                position++;
                return;
            }
            case '$':{
                Regex rg = new Regex(identifier_regex);
                string name = rg.Match(origin,cursor).Value;
                current = new Token(TokenTypes.IDEN, name);
                position+=name.Length;
                return;
            }
            case '=':{
                if((cursor+1) != origin.Length && origin[cursor+1] == '='){
                    current = new Token(TokenTypes.EQEQUAL,0);
                    position+=2;
                }else{
                    current = new Token(TokenTypes.EQUAL,0);
                    position++;
                }
                return;
            }
            //Parseia o numero. Vai dar ruim quando chegar float
            default:{
                if(Char.IsDigit(origin[cursor])){
                    Regex rg = new Regex(number_regex);
                    int a;
                    string cache = rg.Match(origin,cursor).Value;
                    if(!int.TryParse(cache, out a)){
                        throw new Exception($"Erro ao parsear digito '{cache}' na posição {cursor}");
                    }
                    current = new Token(TokenTypes.INT,a);
                    position+=cache.Length;
                    return;
                }else if(Char.IsLetter(origin[cursor])){
                    Regex rg = new Regex(keyword_regex);
                    string cache = rg.Match(origin,cursor).Value;
                    Keywords  kword;
                    if(!Enum.TryParse(cache,true, out kword)){
                        throw new RaulException($"'{cache}' is not defined");
                    }
                    current = new Token(TokenTypes.KEYWORD, kword);
                    position+=cache.Length;
                }
                else{
                    throw new Exception($"Invalid Character at pos {cursor}");
                }
            }
            break;
        }
    }

    public bool CurrentIsSign(){
        switch(current.Type){
            case TokenTypes.PLUS:
            case TokenTypes.MINUS:
            {
                return true;
            }
            default:
            {
                return false;
            }
        }
    }

    public bool CurrentIsTerm(){
        switch(current.Type){
            case TokenTypes.STAR:
            case TokenTypes.SLASH:
            {
                return true;
            }
            default:
            {
                return false;
            }
        }
    }

}