using System;

static class Parser {
    static Tokenizer tokens;

    static int ParseFactor(){
        int value = 0;
        switch(tokens.Current.Type){
            case TokenTypes.INT:{
                value = tokens.Current.Value;
                tokens.selectNext();
                return value;
            }
            case TokenTypes.PLUS:{
                tokens.selectNext();
                value = ParseFactor();
                return value;
            }
            case TokenTypes.MINUS:{
                tokens.selectNext();
                value = -ParseFactor();
                return value;
            }
            case TokenTypes.LPAR:{
                tokens.selectNext();
                value = parseExpression();
                if(tokens.Current.Type == TokenTypes.RPAR){
                    tokens.selectNext();
                    return value;
                } 
                else throw new RaulException($"Expected ')' at pos {tokens.Position}");
            }
            default:{
                throw new RaulException($"Unexpected symbol at {tokens.Position}");
            }
        }
    }
    static int ParseTerm(){
        int res = ParseFactor();
        while(tokens.currentIsTerm()){
            if(tokens.Current.Type == TokenTypes.STAR){
                tokens.selectNext();
                res*= ParseFactor();
            }else if(tokens.Current.Type == TokenTypes.SLASH){
                tokens.selectNext();
                res/= ParseFactor();
            }
        }
        return res;
    }
    static int parseExpression(){
        int res = ParseTerm();
        while(tokens.currentIsSign()){
            if(tokens.Current.Type == TokenTypes.PLUS){
                tokens.selectNext();
                res+= ParseTerm();
            }else if(tokens.Current.Type == TokenTypes.MINUS){
                tokens.selectNext();
                res-= ParseTerm();
            }
        }
        return res;
    }

    public static int run(string code){
        tokens = new Tokenizer(code, true);
        int res = parseExpression();
        if(tokens.Current.Type != TokenTypes.EOF){
            throw new RaulException($"Expected EOF at pos {tokens.Position}");
        }
        return res;
    }

}