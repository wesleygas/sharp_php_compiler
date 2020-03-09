# compylador

Para utilizar o compilador, passe a string como parâmetro único para o executável  executaveis/{windows|linux}/compylador.exe


#### EBNF
'''
EXPRESSION = NUMBER, {("+" | "-" | "\*" | "/" ), NUMBER} ;
NUMBER = DIGIT, {DIGIT} ;
DIGIT = 0 | 1 | ... | 9 ;

'''