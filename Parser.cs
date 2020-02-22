using System;

static class Parser {
    static Tokenizer tokens;
    static int result = 0;
    static int parseExpression(){
        if(tokens.Current.Type == TokenTypes.INT){
            result += tokens.Current.Value;
            tokens.selectNext();
            while(tokens.currentIsSign()){
                if(tokens.Current.Type == TokenTypes.PLUS){
                    tokens.selectNext();
                    if(tokens.Current.Type == TokenTypes.INT){
                        result+=tokens.Current.Value;
                    }else{
                        throw new RaulException($"Sinal '+' inesperado na posição {tokens.Position}");
                    }
                }else if(tokens.Current.Type == TokenTypes.MINUS){
                    tokens.selectNext();
                    if(tokens.Current.Type == TokenTypes.INT){
                        result-=tokens.Current.Value;
                    }else{
                        throw new RaulException($"Sinal '-' inesperado na posição {tokens.Position}");
                    }
                }
                tokens.selectNext();
            }
            if(tokens.Current.Type == TokenTypes.EOF){
                return result;
            }else if(tokens.Current.Type == TokenTypes.INT){
                throw new RaulException($"Numero encontrado após outro número na posição {tokens.Position}");
            }else{
                throw new Exception($"Sinceramente n sei oq aconteceu na pos {tokens.Position}");
            }
        }else{
            throw new RaulException("Nao se pode comecar com sinais");
        }
    }
    public static int run(string code){
        tokens = new Tokenizer(code, true);

        return parseExpression();
    }

}