using System;
using System.Globalization;
using System.Windows;

namespace CalcApp
{
    public partial class MainWindow : Window
    {
        private string expr = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Btn(object sender, RoutedEventArgs e)
        {
            expr += ((System.Windows.Controls.Button)sender).Content.ToString();
            Display.Text = expr;
            FitText();
        }

        private void Func(object sender, RoutedEventArgs e)
        {
            string f = ((System.Windows.Controls.Button)sender).Content.ToString();
            expr += f + "(";
            Display.Text = expr;
            FitText();
        }

        private void Clear(object sender, RoutedEventArgs e)
        {
            expr = "";
            Display.Text = "0";
            ExpressionText.Text = "";
            FitText();
        }

        private void Eq(object sender, RoutedEventArgs e)
        {
            try
            {
                ExpressionText.Text = expr;

                double result = Evaluate(expr);

                Display.Text = result.ToString(CultureInfo.InvariantCulture);
                expr = Display.Text;

                FitText();
            }
            catch
            {
                Display.Text = "Error";
                expr = "";
                FitText();
            }
        }


        private double Evaluate(string s)
        {
            s = s.Replace("tg", "tan");
            s = ProcessFunctions(s);

            var dt = new System.Data.DataTable();
            return Convert.ToDouble(dt.Compute(s, ""), CultureInfo.InvariantCulture);
        }

        private string ProcessFunctions(string s)
        {
            while (true)
            {
                int i = FindFunc(s);
                if (i == -1) break;

                string name = GetName(s, i);
                int start = i + name.Length;

                int end = FindBracket(s, start);

                string inner = s.Substring(start + 1, end - start - 1);
                double val = Evaluate(inner);

                double res = Apply(name, val);

                s = s.Substring(0, i) +
                    res.ToString(CultureInfo.InvariantCulture) +
                    s.Substring(end + 1);
            }

            return s;
        }

        private double Apply(string f, double x)
        {
            double r = x * Math.PI / 180.0;

            switch (f)
            {
                case "sin": return Math.Sin(r);
                case "cos": return Math.Cos(r);
                case "tan": return Math.Tan(r);
                case "log": return Math.Log10(x);
                case "√": return Math.Sqrt(x);
                default: throw new Exception();
            }
        }

        private int FindFunc(string s)
        {
            string[] f = { "sin", "cos", "tan", "log", "√" };

            foreach (var x in f)
            {
                int i = s.IndexOf(x, StringComparison.Ordinal);
                if (i != -1) return i;
            }

            return -1;
        }

        private string GetName(string s, int i)
        {
            if (s.Substring(i).StartsWith("sin")) return "sin";
            if (s.Substring(i).StartsWith("cos")) return "cos";
            if (s.Substring(i).StartsWith("tan")) return "tan";
            if (s.Substring(i).StartsWith("log")) return "log";
            if (s.Substring(i).StartsWith("√")) return "√";

            throw new Exception();
        }

        private int FindBracket(string s, int open)
        {
            int c = 0;

            for (int i = open; i < s.Length; i++)
            {
                if (s[i] == '(') c++;
                if (s[i] == ')') c--;

                if (c == 0)
                    return i;
            }

            throw new Exception();
        }


        private void FitText()
        {
            if (Display.Text.Length > 18)
                Display.FontSize = 22;
            else if (Display.Text.Length > 12)
                Display.FontSize = 28;
            else
                Display.FontSize = 40;
        }
    }
}