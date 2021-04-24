using System;
using System.Collections.Generic;

namespace JsonReader
{

    public delegate bool InputCondition(Input input);
    public class Input
    {
        private readonly string input;
        private readonly int length;
        private int position;
        private int lineNumber;
        //Properties
        public int Length
        {
            get
            {
                return this.length;
            }
        }
        public int Position
        {
            get
            {
                return this.position;
            }
        }
        public int NextPosition
        {
            get
            {
                return this.position + 1;
            }
        }
        public int LineNumber
        {
            get
            {
                return this.lineNumber;
            }
        }
        public char Character
        {
            get
            {
                if (this.position > -1) return this.input[this.position];
                else return '\0';
            }
        }
        public Input(string input)
        {
            this.input = input;
            this.length = input.Length;
            this.position = -1;
            this.lineNumber = 1;
        }
        public bool hasMore(int numOfSteps = 1)
        {
            if (numOfSteps <= 0) throw new Exception("Invalid number of steps");
            return (this.position + numOfSteps) < this.length;
        }
        public bool hasLess(int numOfSteps = 1)
        {
            if (numOfSteps <= 0) throw new Exception("Invalid number of steps");
            return (this.position - numOfSteps) > -1;
        }
        //callback -> delegate
        public Input step(int numOfSteps = 1)
        {
            if (this.hasMore(numOfSteps))
                this.position += numOfSteps;
            else
            {
                throw new Exception("There is no more step");
            }
            return this;
        }
        public Input back(int numOfSteps = 1)
        {
            if (this.hasLess(numOfSteps))
                this.position -= numOfSteps;
            else
            {
                throw new Exception("There is no more step");
            }
            return this;
        }
        public Input reset() { return this; }
        public char peek(int numOfSteps = 1)
        {
            if (this.hasMore(numOfSteps)) return this.input[this.position+numOfSteps];
            return '\0';
        }
        public string loop(InputCondition condition)
        {
            string buffer = "";
            while (this.hasMore() && condition(this))
                buffer += this.step().Character;
            return buffer;
        }
    }
    public class Token
    {
        public int Position { set; get; }
        public int LineNumber { set; get; }
        public string Type { set; get; }
        public string Value { set; get; }
        public Token(int position, int lineNumber, string type, string value)
        {
            this.Position = position;
            this.LineNumber = lineNumber;
            this.Type = type;
            this.Value = value;
        }
    }
    public abstract class Tokenizable
    {
        public abstract bool tokenizable(Tokenizer tokenizer);
        public abstract Token tokenize(Tokenizer tokenizer);
   
    }
    public class Tokenizer
    {
        public List<Token> tokens;
        public bool enableHistory;
        public Input input;
        public Tokenizable[] handlers;
        public Tokenizer(string source, Tokenizable[] handlers)
        {
            this.input = new Input(source);
            this.handlers = handlers;
        }
        public Tokenizer(Input source, Tokenizable[] handlers)
        {
            this.input = source;
            this.handlers = handlers;
        }
        public Token tokenize()
        {
            foreach (var handler in this.handlers)
                if (handler.tokenizable(this)) return handler.tokenize(this);
            return null;
        }
        public List<Token> all() { return null; }
    }

    //Raneen
    public class JSONObject
    {
        public List<Token> tokens;
        public int i;
        public JSONObject(List<Token> tokens,int index)
        {
            this.tokens = tokens;
            this.i = index;
        }

        public void readJSONObject()
        {
            
            while(this.i<this.tokens.Count)
            {
                if (tokens[this.i].Type.Equals("WhiteSpace"))
                {
                    this.i++;
                    continue;
                }
                

                else if (tokens[this.i].Value.Equals(","))
                {
                    Console.Write(tokens[this.i++].Value);
                    
                    continue;
                }
                
                else if (tokens[this.i].Value.Equals("}")) 
                {
                    this.i++;
                    Console.Write("}");
                    break;
                   
                }
                else
              
                {
                    if (this.i < this.tokens.Count)
                        this.readJKeyValue();
                }


            }
            
        }

        public void readJKeyValue()
        {
           
            
            if (this.tokens[this.i].Type.Equals("String"))
            {
                string key = this.tokens[this.i++].Value;
                Console.Write(key.Substring(1, key.Length - 2));
               
                if (this.tokens[this.i].Type.Equals("WhiteSpace"))
                {
                    Console.Write(this.tokens[this.i++].Value);
                    
                }
                if (this.tokens[this.i].Value.Equals(":"))
                {
                    Console.Write(" = ");
                    this.i++;
                    
                    if (this.tokens[this.i].Type.Equals("WhiteSpace"))
                    {
                        this.i++;
                    }
                    if (this.tokens[this.i].Value.Equals("{"))
                    {
                        Console.Write("{");
                       
                        this.i++;
                        this.readJSONObject();
                    }
                    else if (this.tokens[this.i].Value.Equals("["))
                    {
                        Console.Write("[");
                        this.i++;
                        this.readArray();
                        
                    }
                    else if (this.tokens[this.i].Type.Equals("String"))
                    {
                        Console.Write(this.tokens[this.i++].Value);
                       
                    }
                    else if (this.tokens[this.i].Type.Equals("Number"))
                    {
                        Console.Write(this.tokens[this.i++].Value);
                       
                    }
                    else if (this.tokens[this.i].Type.Equals("Keyword"))
                    {
                        Console.Write(this.tokens[this.i++].Value);
                        
                    }

                    else
                    {
                        throw new Exception("Not a valid Json value");
                    }
                }

            }
            else
            {
                throw new Exception("Not A valid Json key");
            }
        
        }

        public void readArray()
        {
            while (!this.tokens[this.i].Value.Equals("]")&& this.i<this.tokens.Count)
            {
                if (tokens[this.i].Type.Equals("WhiteSpace"))
                {
                    this.i++;
                    continue;
                }


                else if (tokens[this.i].Value.Equals(","))
                {
                    Console.Write(tokens[this.i++].Value);
                   
                    continue;
                }

                else
                
                {
                    if (this.i < this.tokens.Count)
                    {
                        if (this.tokens[this.i].Type.Equals("WhiteSpace"))
                        {
                            this.i++;
                        }
                        if (this.tokens[this.i].Value.Equals("{"))
                        {
                            Console.Write("{");
                            this.i++;
                            this.readJSONObject();
                        }
                        else if (this.tokens[this.i].Value.Equals("["))
                        {
                            Console.Write("[");
                            this.i++;
                            this.readArray();

                        }
                        else if (this.tokens[this.i].Type.Equals("String"))
                        {
                            Console.Write(this.tokens[this.i++].Value);

                        }
                        else if (this.tokens[this.i].Type.Equals("Number"))
                        {
                            Console.Write(this.tokens[this.i++].Value);

                        }
                        else if (this.tokens[this.i].Type.Equals("Keyword"))
                        {
                            Console.Write(this.tokens[this.i++].Value);

                        }

                        else
                        {
                            throw new Exception("Not a valid Json value");
                        }
                    }
                       

                }
            }
            if (tokens[this.i].Value.Equals("]"))
            {
               
                this.i++;
                Console.Write("]");
                
                
            }

        }
    }

    //Raneen
    public class CharacterTokenizer : Tokenizable
    {
        List <char> keychar;
        public CharacterTokenizer(List <char> keychar)
        {
            this.keychar = keychar;
        }
        public override bool tokenizable(Tokenizer t)
        {
            return this.keychar.Contains(t.input.peek());
        }

        public override Token tokenize(Tokenizer t)
        {
            char currentCharacter = t.input.peek();
            if(currentCharacter.Equals('{')) return new Token(t.input.Position, t.input.LineNumber, "Opening_Curly", ""+t.input.step().Character);
            else if (currentCharacter.Equals('}')) return new Token(t.input.Position, t.input.LineNumber, "Closing_Curly", "" + t.input.step().Character);
            else if (currentCharacter.Equals('[')) return new Token(t.input.Position, t.input.LineNumber, "Opening_Square", "" + t.input.step().Character);
            else if (currentCharacter.Equals(']')) return new Token(t.input.Position, t.input.LineNumber, "Closing_Square", "" + t.input.step().Character);
            else if (currentCharacter.Equals(',')) return new Token(t.input.Position, t.input.LineNumber, "Comma", "" + t.input.step().Character);
            else return new Token(t.input.Position, t.input.LineNumber, "Colon", "" + t.input.step().Character);
        }
    }

    public class WhiteSpaceTokenizer : Tokenizable
    {
        public override bool tokenizable(Tokenizer t)
        {
            char currentCharacter = t.input.peek();
            return Char.IsWhiteSpace(currentCharacter);
        }
        public override Token tokenize(Tokenizer t)
        {
            return new Token(t.input.Position, t.input.LineNumber, "WhiteSpace", t.input.loop(new InputCondition(isWhiteSpace)));
            
        }

        public bool isWhiteSpace(Input input)
        {
            char currentCharacter = input.peek();
            return Char.IsWhiteSpace(currentCharacter) || currentCharacter.Equals('\n')
                || currentCharacter.Equals('\t') || currentCharacter.Equals('\r');
      
        }
    }

    //Batool 
    public class StringTokenizer : Tokenizable
    {
        public override bool tokenizable(Tokenizer t)
        {
            return t.input.peek().Equals('"');
        }
        public override Token tokenize(Tokenizer t)
        {
            //1. initialize token
            Token token = new Token(t.input.Position, t.input.LineNumber, "String", "");
            token.Value += t.input.step().Character;
            char currentCharacter = t.input.peek();
            while (t.input.hasMore())
            {
                token.Value += t.input.step().Character;
                if (token.Value[token.Value.Length - 1].Equals('"')) return token;
                else if(token.Value[token.Value.Length - 1].Equals('\\')) 
                {
                    if (isValid(t.input)) continue;
                    throw new Exception("not a valid String");
                }
                   
                currentCharacter = t.input.peek();
            }
            throw new Exception("Not A valid String");
        }

        public bool isValid(Input input)
        {
            char currentCharacter = input.peek();
            bool hex = true;
            if (currentCharacter.Equals('u'))
            {
                for (int i = 2; i < 6; i++)
                {
                    char ch = input.peek(i);
                    if (!((ch >= 48 && ch <= 57) || (ch >= 97 && ch <= 102) || (ch >= 65 && ch <= 70)))

                    {
                        hex = false;
                    }
                }
            }
            return (currentCharacter.Equals('"') || currentCharacter.Equals('t') ||
                currentCharacter.Equals('r') || currentCharacter.Equals('b') ||
                currentCharacter.Equals('f') || currentCharacter.Equals('n') ||
                currentCharacter.Equals('/') || currentCharacter.Equals('\\') || hex);
        }


    }

    //Raneen
    public class IdTokenizer : Tokenizable
    {
        private List<string> keywords;

        public IdTokenizer(List<String> keywords)
        {
            this.keywords = keywords;
        }
        private bool isKeyword(string value)
        {
            return this.keywords.Contains(value);
        }
        public override bool tokenizable(Tokenizer t)
        {
            char currentCharacter = t.input.peek();

            return Char.IsLetter(currentCharacter);
        }
        public override Token tokenize(Tokenizer t)
        {
            //1. initialize token


            Token token = new Token(t.input.Position, t.input.LineNumber, "Identifier", "");
            token.Value = t.input.loop(new InputCondition(isLetter));
            
            if (this.isKeyword(token.Value))
                token.Type = "Keyword";
            return token;
        }

        public bool isLetter(Input input)
        {
            char currentCharacter = input.peek();
            return (Char.IsLetter(currentCharacter));
        }
    }

    //Reema
    public class NumberTokenizer : Tokenizable
    {
        public override bool tokenizable(Tokenizer t)
        {
            char currentCharacter = t.input.peek();
            char secondCharacter = t.input.peek(2);
            char thirdCharacter = t.input.peek(3);
            return Char.IsDigit(currentCharacter)
                || (isNegative(currentCharacter, secondCharacter));
        }
        public override Token tokenize(Tokenizer t)
        {
            Token token = new Token(t.input.Position, t.input.LineNumber, "Number", "");
            char currentCharacter = t.input.peek();
            char secondCharacter = t.input.peek(2);
            while (t.input.hasMore() && ((Char.IsDigit(currentCharacter)) ||
                isNegative(currentCharacter, secondCharacter) || currentCharacter.Equals('.')))
            {
                secondCharacter = t.input.peek(2);
                if (isNegative(currentCharacter, secondCharacter))
                {
                    //token.Type = "integer";
                    token.Value += t.input.step().Character;//+ or -
                    currentCharacter = t.input.peek();
                    while (t.input.hasMore() && (Char.IsDigit(currentCharacter) || currentCharacter.Equals('.')))
                    {
                        token.Value += t.input.step().Character;
                        if (currentCharacter.Equals('.'))
                        {
                            currentCharacter = t.input.peek();
                            while (t.input.hasMore() && (Char.IsDigit(currentCharacter)))
                            {
                                token.Value += t.input.step().Character;
                                currentCharacter = t.input.peek();
                                if (currentCharacter.Equals('E') || currentCharacter.Equals('e'))
                                {
                                    //token.Type = "exponent";
                                    currentCharacter = t.input.peek();
                                    return getExponent(token, currentCharacter, secondCharacter, t);
                                }
                                return getFloat(token, currentCharacter, t); //float
                            }
                            currentCharacter = t.input.peek();
                        }
                        return token; //integer ex -3 or +3
                    }
                }
                // float not signed ex: 24.3
                else if (currentCharacter.Equals('.'))
                {
                    token.Value += t.input.step().Character;
                    currentCharacter = t.input.peek();
                    while (t.input.hasMore() && (Char.IsDigit(currentCharacter)))
                    {
                        token.Value += t.input.step().Character;
                        currentCharacter = t.input.peek();
                        if (currentCharacter.Equals('E') || currentCharacter.Equals('e'))
                        {
                            //token.Type = "exponent";
                            currentCharacter = t.input.peek();
                            return getExponent(token, currentCharacter, secondCharacter, t);
                        }
                    }
                    return getFloat(token, currentCharacter, t); //float
                }
                else
                {
                    token.Value += t.input.step().Character;
                    currentCharacter = t.input.peek();
                }
            }
            return token; //number ex 44
        }
        public bool isNegative(char currentCharacter, char secondCharacter)
        {
            return (currentCharacter.Equals('-') && Char.IsDigit(secondCharacter));
        }
        public Token getFloat(Token token, char currentCharacter, Tokenizer t)
        {
            //token.Type = "fraction";
            currentCharacter = t.input.peek();
            while (t.input.hasMore() && (Char.IsDigit(currentCharacter)))
            {
                token.Value += t.input.step().Character;
                currentCharacter = t.input.peek();
            }
            return token;
        }
        public Token getExponent(Token token, char currentCharacter, char secondCharacter, Tokenizer t)
        {
            //token.Type = "exponent";
            currentCharacter = t.input.peek();
            while (t.input.hasMore() && (Char.IsDigit(currentCharacter) || currentCharacter.Equals('E') || currentCharacter.Equals('e')
                && Char.IsDigit(secondCharacter) || secondCharacter.Equals('+') || secondCharacter.Equals('-')))
            {
                token.Value += t.input.step().Character;
                currentCharacter = t.input.peek();
                secondCharacter = t.input.peek(1);
            }
            return token;
        }


    }

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Tokenizer t = new Tokenizer(new Input(@"{ ""hi"" : [""j"",{""ff"":4}],""f"":{""t3"":3} } "), new Tokenizable[] {

                new WhiteSpaceTokenizer(), new CharacterTokenizer(new List<char> { '{','}','[',']',',',':'}), new NumberTokenizer(),new StringTokenizer(), new IdTokenizer( new List<string> {"true","false","null"})
            });
                Token res = t.tokenize();
                List <Token> tokens=new List<Token> { }; 
                while (res != null)
                {
                    
                    tokens.Add(res);
                    res = t.tokenize();
                   
                }

                    if (tokens[0].Value.Equals("{"))
                    {   
                        Console.Write("{");
                        JSONObject j = new JSONObject(tokens,1);
                        j.readJSONObject();
                    }
                
            }

            catch (Exception error)
            {
                Console.WriteLine(error.Message);
            }
        }

        
    }
}
