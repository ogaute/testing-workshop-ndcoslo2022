namespace StringCalculator;

public class Calculator
{
    private static readonly char[] Separators = { ',', '\n' };
    
    public int Add(string numbers)
    {
        if (numbers.Equals(string.Empty))
        {
            return 0;
        }
        
        return numbers.Split(Separators).Select(int.Parse).Sum();
    }
}
