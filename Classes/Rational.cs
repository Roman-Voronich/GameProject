public struct Rational
{
    public int Whole { get; set; }
    public int Fraction { get; private set; }
    public int Denumerator => denumerator;
    private int denumerator;

    public Rational(int denumerator = 1)
    {
        this.denumerator = denumerator;
    }

    public Rational(int whole, int fraction, int denumerator)
    {
        Whole = whole + fraction / denumerator;
        Fraction = fraction % denumerator;
    }

    public void AddFraction(int fraction)
    {
        if (fraction < 0) 
        {
            SubFraction(fraction);
            return;
        }
        Fraction += fraction;
        Whole += Fraction / denumerator;
        Fraction %= denumerator;
    }

    private void SubFraction(int fraction)
    {
        Whole += fraction / Denumerator;
        Fraction += fraction % Denumerator;
        if (Fraction < 0)
        {
            Fraction += Denumerator;
            Whole--;
        }
    }

    public void SetFraction(int fraction)
    {
        Fraction = fraction;
        Whole += Fraction / denumerator;
        Fraction %= denumerator;
    }

    public static Rational operator +(Rational a, Rational b)
    {
        var res = new Rational(a.Denumerator);
        res.Whole = a.Whole + b.Whole;
        res.AddFraction(a.Fraction + b.Fraction);
        return res;
    }

    public static Rational operator +(Rational a, int b) => new(a.Whole + b, a.Fraction, a.Denumerator);

    public static Rational operator -(Rational a, Rational b)
    {
        var res = new Rational(a.Denumerator);
        res.Whole = a.Whole - b.Whole;
        res.AddFraction(a.Fraction - b.Fraction);
        return res;
    }

    public static implicit operator int(Rational r) => r.Whole;
    public static implicit operator float(Rational r) => r.Whole + (float)r.Fraction / r.Denumerator;
    public static implicit operator Rational(int i) => new Rational(i, 0, 1);
}