using System;

class Tokenizer {
    string origin;
    int position;
    Token current;

    public Tokenizer(string Origin, bool feedIn){
        Position = 0;
        origin = Origin;
        current = new Token(TokenTypes.None, 0);
        if(feedIn) selectNext();
    }

    public Token Current { get => current; set => current = value; }
    public int Position { get => position; set => position = value; }

    public void selectNext(){
        while(position < origin.Length && origin[position] == ' ') position++;
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
            //Parseia o numero. Vai dar ruim quando chegar float
            default:{
                if(Char.IsDigit(origin[cursor])){
                    while(position < origin.Length){
                        position++;
                        if(position == origin.Length){
                            if(position-cursor == 1){
                                current = new Token(TokenTypes.INT, (int)Char.GetNumericValue(origin[cursor]));
                            }else{
                                string v = origin.Substring(cursor,(position-cursor));
                                int a;
                                if(!int.TryParse(v, out a)){
                                    throw new Exception($"Erro ao parsear digito '{v}' na posição {cursor}");
                                }
                                current = new Token(TokenTypes.INT, a);
                            }
                            return;
                        }
                        if(!Char.IsDigit(origin[position])){
                            string v = origin.Substring(cursor,(position-cursor));
                            int a;
                            if(!int.TryParse(v, out a)){
                                throw new Exception($"Erro ao parsear digito '{v}' na posição {cursor}");
                            }
                            current = new Token(TokenTypes.INT, a);
                            return;
                        }
                    }
                }else{
                    throw new Exception($"Invalid Character at pos {cursor}");
                }
            }
            break;
        }
    }

    public bool currentIsSign(){
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

    public bool currentIsTerm(){
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