using System;
using System.Collections.Generic;

namespace compylador
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length == 0) throw new RaulException("Não pode ter nada vazio");

            string code = args[0];
            if (string.IsNullOrWhiteSpace(code)) throw new RaulException("Não pode ter nada vazio...Espaços contam como vazio, se você não entendeu.");
            
            code = Preproc.Run(code);

            int result = Parser.run(code);
            Console.WriteLine(result);
            return 0;
        }
    }
}
