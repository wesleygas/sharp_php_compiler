import sys

signs = ["+","-"]

try:
    code = sys.argv[1]
except: #se tiver vazio
    raise Exception("RaulError: \"Ele disse q n pode receber nada vazio....\"")

if(code == (" "*len(code))): #se for tudo espaço
    raise Exception("RaulError: \"Ele disse q n pode receber nada vazio....\"")

#code = code.replace(" ", "")
if((code[0] in signs) or (code[-1] in signs)):
    raise Exception("N da pra fazer isso né, amigão. Os sinais vêm ENTRE os números.")


numbers = code.replace("+"," ").replace("-"," ").split()
signs = []
for char in code:
    if(not char in "1234567890-+ "):
        raise Exception(f"Só aceito números e operações '+' e '-'! '{char}' não me parece estar dentro disso")
    if(char in "+-"):
        signs.append(char)

if((len(numbers) - 1) != len(signs)):
    raise Exception("N da pra fazer isso né, amigão. Os sinais vêm ENTRE os números")

result = 0

for i in range(len(signs)):
    result += int(numbers[i])
    if(signs[i] == "+"):
        result+= int(numbers[i+1])
    elif(signs[i] == "-"):
        result-= int(numbers[i+1])

print(result)
