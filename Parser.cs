using System;

static class Parser {
    static Tokenizer tokens;

    static int ParseTerm(){
        if(tokens.Current.Type == TokenTypes.INT){
            int result = tokens.Current.Value;
            tokens.selectNext();
            while(tokens.currentIsTerm()){
                if(tokens.Current.Type == TokenTypes.MULT){
                    tokens.selectNext();
                    if(tokens.Current.Type == TokenTypes.INT){
                        result*=tokens.Current.Value;
                    }else{
                        throw new RaulException($"Sinal '+' inesperado na posição {tokens.Position}");
                    }
                }else if(tokens.Current.Type == TokenTypes.DIV){
                    tokens.selectNext();
                    if(tokens.Current.Type == TokenTypes.INT){
                        result /= tokens.Current.Value;
                    }else{
                        throw new RaulException($"Sinal '-' inesperado na posição {tokens.Position}");
                    }
                }
                tokens.selectNext();
            }
            return result;
        }else{
            throw new RaulException("Nao se pode comecar com sinais");
        }
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